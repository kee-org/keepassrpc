#region License, Terms and Conditions
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (www.raboof.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
//
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library; if not, write to the Free Software Foundation, Inc.,
// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
//
#endregion

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using CustomTypeDescriptor = Conversion.CustomTypeDescriptor;

    #endregion
    
    public sealed class ComponentExporter : ExporterBase
    {
        private readonly PropertyDescriptorCollection _properties; // TODO: Review thread-safety of PropertyDescriptorCollection
        private readonly IObjectMemberExporter[] _exporters;

        public ComponentExporter(Type inputType) :
            this(inputType, (ICustomTypeDescriptor) null) {}

        public ComponentExporter(Type inputType, ICustomTypeDescriptor typeDescriptor) :
            this(inputType, typeDescriptor != null 
                            ? typeDescriptor.GetProperties() 
                            : (new CustomTypeDescriptor(inputType)).GetProperties()) {}

        private ComponentExporter(Type inputType, PropertyDescriptorCollection properties) : 
            base(inputType)
        {
            Debug.Assert(properties != null);
            
            _properties = properties;

            int count = 0;
            IObjectMemberExporter[] exporters = null;

            for (int i = 0; i < properties.Count; i++)
            {
                IServiceProvider sp = properties[i] as IServiceProvider;

                if (sp == null)
                    continue;

                IObjectMemberExporter exporter = (IObjectMemberExporter)sp.GetService(typeof(IObjectMemberExporter));

                if (exporter == null)
                    continue;

                if (exporters == null) // fault
                    exporters = new IObjectMemberExporter[properties.Count];

                exporters[i] = exporter;
                count++;
            }
            
            if (count > 0)
                _exporters = exporters;
        }

        protected override void ExportValue(ExportContext context, object value, JsonWriter writer)
        {
            Debug.Assert(context != null);
            Debug.Assert(value != null);
            Debug.Assert(writer != null);
            
            if (_properties.Count == 0)
            {
                writer.WriteString(value.ToString());
            }
            else
            {
                ObjectReferenceTracker tracker = null;
                
                try
                {
                    writer.WriteStartObject();
                    
                    int index = 0;

                    foreach (PropertyDescriptor property in _properties)
                    {
                        IObjectMemberExporter exporter = _exporters != null && index < _exporters.Length ? 
                            _exporters[index++] : null;
                        
                        if (exporter != null)
                        {
                            exporter.Export(context, writer, value);
                        }
                        else
                        {
                            object propertyValue = property.GetValue(value);
                
                            if (!JsonNull.LogicallyEquals(propertyValue))
                            {
                                writer.WriteMember(property.Name);

                                if (value.GetType().IsClass && tracker == null)
                                {
                                    //
                                    // We are about to enter a deeper scope so 
                                    // start tracking the current object being 
                                    // exported. This will help to detect 
                                    // recursive references that may occur 
                                    // through this exporter deeper in the tree.
                                    //

                                    tracker = TrackObject(context, value);
                                }

                                context.Export(propertyValue, writer);
                            }
                        }
                    }

                    writer.WriteEndObject();
                }
                finally
                {
                    if (tracker != null)
                        tracker.Pop(value);
                }
            }
        }

        private static ObjectReferenceTracker TrackObject(ExportContext context, object value) 
        {
            Debug.Assert(context != null);
            Debug.Assert(value != null);
            
            //
            // Get the object reference tracker from the current context. If
            // it's not there, then create one.
            //
            
            Type key = typeof(ComponentExporter);
            ObjectReferenceTracker tracker = (ObjectReferenceTracker) context.Items[key];
            
            if (tracker == null)
            {
                tracker = new ObjectReferenceTracker();
                context.Items.Add(key, tracker);
            }
           
            tracker.PushNew(value);
            return tracker;
        }

        private sealed class ObjectReferenceTracker
        {
            private readonly ArrayList _stack = new ArrayList(6);
            
            public void PushNew(object value)
            {
                Debug.Assert(value != null);
                Debug.Assert(value.GetType().IsClass);
                
                for (int i = _stack.Count - 1; i >= 0; i--)
                {
                    if (object.ReferenceEquals(_stack[i], value))
                        throw new JsonException(string.Format("{0} does not support export of an object graph containing circular references. A value of type {1} has already been exported.", typeof(ComponentExporter).FullName, value.GetType().FullName));
                }
                
                _stack.Add(value);
            }
            
            public void Pop(object value)
            {
                int index = _stack.Count - 1;
                
                Debug.Assert(index >= 0);
                Debug.Assert(object.ReferenceEquals(_stack[index], value));
                
                _stack.RemoveAt(index);
            }
        }
    }
}
