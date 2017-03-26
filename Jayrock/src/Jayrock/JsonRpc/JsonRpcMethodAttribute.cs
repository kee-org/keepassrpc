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
    using System.Diagnostics;
    using System.Reflection;
    using Jayrock.Services;
    using System.ComponentModel;
    using CustomTypeDescriptor = Jayrock.Json.Conversion.CustomTypeDescriptor;

    #endregion

    [ Serializable ]
    [ AttributeUsage(AttributeTargets.Method) ]
    public sealed class JsonRpcMethodAttribute : Attribute, IMethodReflector, IAttributeAttachment
    {
        private string _name;
        private bool _idempotent;
        private bool _warpedParameters;
        private MethodInfo _method;

        public JsonRpcMethodAttribute() {}

        public JsonRpcMethodAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return Mask.NullString(_name); }
            set { _name = value; }
        }

        public bool Idempotent
        {
            get { return _idempotent; }
            set { _idempotent = value; }
        }
        
        public bool WarpedParameters
        {
            get { return _warpedParameters; }
            set { _warpedParameters = value; }
        }

        void IMethodReflector.Build(MethodBuilder builder)
        {
            if (_method == null)
                throw new InvalidOperationException();

            builder.Name = Name;
            builder.Idempotent = Idempotent;

            //
            // Build the method parameters.
            //

            MethodInfo method = _method;
            ParameterInfo[] parameters = method.GetParameters();

            if (method.IsStatic)
            {
                ParameterInfo[] logicalParameters = new ParameterInfo[parameters.Length - 1];
                Array.Copy(parameters, 1, logicalParameters, 0, logicalParameters.Length);
                parameters = logicalParameters;
            }

            if (WarpedParameters)
            {
                if (parameters.Length != 1)
                {
                    // TODO: Use a more specific exception type
                    throw new Exception(string.Format(
                        "Methods using warped parameters must accept a single argument of the warped type whereas method {1} on {0} accepts {2}.",
                        method.DeclaringType.FullName, method.Name, parameters.Length.ToString()));
                }

                PropertyDescriptorCollection args = GetProperties(parameters[0].ParameterType);
                foreach (PropertyDescriptor arg in args)
                {
                    ParameterBuilder parameter = builder.DefineParameter();
                    parameter.Name = arg.Name;
                    parameter.ParameterType = arg.PropertyType;
                }

                PropertyDescriptor result = null;

                if (method.ReturnType != typeof(void))
                {
                    PropertyDescriptorCollection results = GetProperties(method.ReturnType);
                    if (results.Count > 0)
                    {
                        result = results[0];
                        builder.ResultType = result.PropertyType;
                    }
                }

                builder.Handler = new WarpedMethodImpl(builder.Handler, 
                    parameters[0].ParameterType, 
                    args, result);
            }
            else
            {
                foreach (ParameterInfo parameter in parameters)
                    JsonRpcServiceReflector.BuildParameter(builder.DefineParameter(), parameter);
            }
        }

        private static PropertyDescriptorCollection GetProperties(Type type) 
        {
            Debug.Assert(type != null);
            CustomTypeDescriptor customType = new CustomTypeDescriptor(type);
            return customType.GetProperties();
        }

        void IAttributeAttachment.SetAttachment(ICustomAttributeProvider obj)
        {
            if (obj == null) 
                throw new ArgumentNullException("obj");

            MethodInfo method = obj as MethodInfo;
            if (method == null)
                throw new ArgumentException(null, "obj");

            _method = method;
        }
    }
}
