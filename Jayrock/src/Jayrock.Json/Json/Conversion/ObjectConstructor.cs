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
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;

    #endregion

    public sealed class ObjectConstructor : IObjectConstructor
    {
        private readonly Type _type;
        private readonly ConstructorInfo[] _ctors;

        private static readonly IComparer _arrayLengthComparer = new ReverseComparer(new DelegatingComparer(new ComparableSelector(GetParametersCount)));

        public ObjectConstructor(Type type) : this(type, null) {}

        public ObjectConstructor(Type type, ConstructorInfo[] ctors)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (ctors == null)
            {
                ctors = type.GetConstructors();
            }
            else
            {
                foreach (ConstructorInfo ctor in ctors)
                {
                    if (ctor.DeclaringType != type)
                        throw new ArgumentException(null, "ctors");
                }

                ctors = (ConstructorInfo[]) ctors.Clone();
            }

            if (type.IsClass && ctors.Length == 0)
            {
                //
                // Value types are excluded here because they always have
                // a default constructor available but one which does not
                // show up in reflection.
                //

                throw new ArgumentException(null, "ctors");
            }

            _type = type;
            _ctors = ctors;
            Array.Sort(_ctors, _arrayLengthComparer);
        }

        public ObjectConstructionResult CreateObject(ImportContext context, JsonReader reader)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (reader == null) throw new ArgumentNullException("reader");

            return CreateObject(context, JsonBuffer.From(reader).GetMembersArray());
        }

        public ObjectConstructionResult CreateObject(ImportContext context, NamedJsonBuffer[] members)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (members == null) throw new ArgumentNullException("members");

            if (_ctors.Length > 0)
            {
                foreach (ConstructorInfo ctor in _ctors)
                {
                    ObjectConstructionResult result = TryCreateObject(context, ctor, members);
                    if (result != null)
                        return result;
                }
            }

            if (_type.IsValueType)
            {
                //
                // Value types always have a default constructor available 
                // but one which does not show up in reflection. If no other
                // constructors matched then just use the default one.
                //

                object obj = Activator.CreateInstance(_type);
                JsonReader tail = NamedJsonBuffer.ToObject(members).CreateReader();
                return new ObjectConstructionResult(obj, tail);
            }

            throw new JsonException(string.Format("None constructor could be used to create {0} object from JSON.", _ctors[0].DeclaringType));
        }

        private static ObjectConstructionResult TryCreateObject(ImportContext context, ConstructorInfo ctor, NamedJsonBuffer[] members)
        {
            Debug.Assert(context != null);
            Debug.Assert(ctor != null);
            Debug.Assert(members != null);

            ParameterInfo[] parameters = ctor.GetParameters();
            
            if (parameters.Length > members.Length)
                return null;

            int[] bindings = Bind(context, parameters, members);

            int argc = 0;
            object[] args = null;
            JsonBufferWriter tailw = null;

            for (int i = 0; i < bindings.Length; i++)
            {
                int binding = bindings[i] - 1;
                
                if (binding >= 0)
                {
                    if (args == null)
                        args = new object[parameters.Length];

                    Type type = parameters[binding].ParameterType;
                    JsonBuffer arg = members[i].Buffer;
                    args[binding] = context.Import(type, arg.CreateReader());
                    argc++;
                }
                else
                {
                    if (tailw == null)
                    {
                        tailw = new JsonBufferWriter();
                        tailw.WriteStartObject();
                    }

                    NamedJsonBuffer member = members[i];
                    tailw.WriteMember(member.Name);
                    tailw.WriteFromReader(member.Buffer.CreateReader());
                }
            }

            if (tailw != null)
                tailw.WriteEndObject();

            if (argc != parameters.Length) 
                return null;

            object obj = ctor.Invoke(args);

            JsonBuffer tail = tailw != null 
                            ? tailw.GetBuffer() 
                            : StockJsonBuffers.EmptyObject;
            
            return new ObjectConstructionResult(obj, tail.CreateReader());
        }

        /// <remarks>
        /// Bound indicies returned in the resulting array are one-based
        /// therefore zero means unbound.
        /// </remarks>
        
        private static int[] Bind(ImportContext context, ParameterInfo[] parameters, NamedJsonBuffer[] members)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (parameters == null) throw new ArgumentNullException("parameters");
            if (members == null) throw new ArgumentNullException("members");

            int[] bindings = new int[members.Length];
            
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                
                if (parameter == null)
                    throw new ArgumentException(null, "parameters");
                
                int mi = FindMember(members, parameter.Name);
                
                if (mi >= 0)
                    bindings[mi] = i + 1;
            }

            return bindings;
        }

        private static int FindMember(NamedJsonBuffer[] members, string name)
        {
            for (int i = 0; i < members.Length; i++)
            {
                NamedJsonBuffer member = members[i];
                
                if (member.IsEmpty)
                    throw new ArgumentException(null, "members");
                
                if (0 == CultureInfo.InvariantCulture.CompareInfo.Compare(name, member.Name, CompareOptions.IgnoreCase))
                    return i;
            }

            return -1;
        }

        private static IComparable GetParametersCount(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            
            MethodBase method = obj as MethodBase;
            
            if (method == null)
                throw new ArgumentException("obj");
            
            return method.GetParameters().Length;
        }
    }
}