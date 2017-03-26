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

namespace Jayrock.Json.Conversion
{
    #region Imports

    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using Jayrock.Json;
    using Jayrock.Json.Conversion;

    #endregion

    /// <summary>
    /// Specifies the default value for a property. At export time, if the 
    /// property value compares equal to the specified default value then its
    /// JSON representation is skipped.
    /// </summary>

    [ Serializable ]
    [ AttributeUsage(AttributeTargets.Field | AttributeTargets.Property) ]
    public sealed class JsonDefaultValueAttribute : Attribute, IPropertyDescriptorCustomization
    {
        private object _value;

        public JsonDefaultValueAttribute(object value)
        {
            _value = value;
        }

        public JsonDefaultValueAttribute(string value) : this((object) value) {}
        public JsonDefaultValueAttribute(bool value) : this((object) value) {}
        public JsonDefaultValueAttribute(double value) : this((object) value) {}
        public JsonDefaultValueAttribute(float value) : this((object) value) {}
        public JsonDefaultValueAttribute(long value) : this((object) value) {}
        public JsonDefaultValueAttribute(int value) : this((object) value) {}
        public JsonDefaultValueAttribute(short value) : this((object) value) {}
        public JsonDefaultValueAttribute(byte value) : this((object) value) {}
        public JsonDefaultValueAttribute(char value) : this((object) value) {}
 
        public JsonDefaultValueAttribute(Type type, string value) :
            this(TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value)) {}
        
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        void IPropertyDescriptorCustomization.Apply(PropertyDescriptor property)
        {
            if (property == null) 
                throw new ArgumentNullException("property");

            if (Value == null)
                return;

            IServiceContainer container = (IServiceContainer) property;
            container.AddService(typeof(IObjectMemberExporter), new PropertyExporter(property, Value));
        }

        private sealed class PropertyExporter : IObjectMemberExporter
        {
            private readonly PropertyDescriptor _property;
            private readonly object _defaultValue;

            public PropertyExporter(PropertyDescriptor property, object defaultValue)
            {
                Debug.Assert(property != null);

                _property = property;
                _defaultValue = defaultValue;
            }

            public void Export(ExportContext context, JsonWriter writer, object source)
            {
                if (context == null) throw new ArgumentNullException("context");
                if (writer == null) throw new ArgumentNullException("writer");
                if (source == null) throw new ArgumentNullException("source");

                object value = _property.GetValue(source);
                
                if (JsonNull.LogicallyEquals(value) || value.Equals(_defaultValue))
                    return;

                writer.WriteMember(_property.Name);
                context.Export(value, writer);
            }
        }
    }
}
