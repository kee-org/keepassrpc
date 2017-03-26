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

namespace Jayrock.JsonRpc
{
    #region Imports

    using System;
    using System.Reflection;
    using Jayrock.Services;

    #endregion

    [ Serializable ]
    [ AttributeUsage(AttributeTargets.Class) ]
    public sealed class JsonRpcServiceAttribute : Attribute, IServiceClassReflector, IAttributeAttachment
    {
        private string _name;
        private Type _type;

        public JsonRpcServiceAttribute() {}

        public JsonRpcServiceAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return Mask.NullString(_name); }
            set { _name = value; }
        }

        void IServiceClassReflector.Build(ServiceClassBuilder builder)
        {
            if (_type == null)
                throw new InvalidOperationException();

            builder.Name = Name;

            //
            // Get all the public instance methods on the type and create a
            // filtered table of those to expose from the service.
            //

            MethodInfo[] publicMethods = _type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (MethodInfo method in publicMethods)
            {
                if (JsonRpcServiceReflector.ShouldBuild(method))
                    JsonRpcServiceReflector.BuildMethod(builder.DefineMethod(), method);
            }
        }

        void IAttributeAttachment.SetAttachment(ICustomAttributeProvider obj)
        {
            if (obj == null) 
                throw new ArgumentNullException("obj");

            Type type = obj as Type;
            if (type == null)
                throw new ArgumentException(null, "obj");

            _type = type;
        }
    }
}
