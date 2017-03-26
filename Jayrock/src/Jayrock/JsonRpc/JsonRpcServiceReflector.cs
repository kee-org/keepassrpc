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
    using System.Collections;
    using System.Diagnostics;
    using System.Reflection;
    using Jayrock.Services;

    #endregion

    internal sealed class JsonRpcServiceReflector
    {
        private static readonly Hashtable _classByTypeCache = Hashtable.Synchronized(new Hashtable());
        
        public static ServiceClass FromType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            ServiceClass clazz = (ServiceClass) _classByTypeCache[type];

            if (clazz == null)
            {
                clazz = BuildFromType(type);
                _classByTypeCache[type] = clazz;
            }

            return clazz;
        }

        private static ServiceClass BuildFromType(Type type)
        {
            ServiceClassBuilder builder = new ServiceClassBuilder();
            BuildClass(builder, type);
            return builder.CreateClass();
        }

        private static void BuildClass(ServiceClassBuilder builder, Type type)
        {
            //
            // Build...
            //

            IServiceClassReflector reflector = (IServiceClassReflector) FindCustomAttribute(type, typeof(IServiceClassReflector), true);

            if (reflector == null)
            {
                reflector = new JsonRpcServiceAttribute();
                TrySetAttachment(reflector, type);
            }

            reflector.Build(builder);

            //
            // Fault in the type name if still without name.
            //

            if (builder.Name.Length == 0)
                builder.Name = type.Name;

            //
            // Modify...
            //

            object[] modifiers = GetCustomAttributes(type, typeof(IServiceClassModifier), true);
            foreach (IServiceClassModifier modifier in modifiers)
                modifier.Modify(builder);
        }

        internal static bool ShouldBuild(MethodInfo method)
        {
            Debug.Assert(method != null);

            return !method.IsAbstract && method.IsDefined(typeof(IMethodReflector), true);
        }

        internal static void BuildMethod(MethodBuilder builder, MethodInfo method)
        {
            Debug.Assert(method != null);
            Debug.Assert(builder != null);

            builder.InternalName = method.Name;
            builder.ResultType = method.ReturnType;
            builder.Handler = new TypeMethodImpl(method);

            //
            // Build...
            //

            IMethodReflector reflector = (IMethodReflector) FindCustomAttribute(method, typeof(IMethodReflector), true);

            if (reflector == null)
            {
                reflector = new JsonRpcMethodAttribute();
                TrySetAttachment(reflector,  method);
            }

            reflector.Build(builder);

            //
            // Fault in the method name if still without name.
            //

            if (builder.Name.Length == 0)
                builder.Name = method.Name;

            builder.CustomAttributes = method;

            //
            // Modify...
            //

            IMethodModifier[] modifiers = (IMethodModifier[]) GetCustomAttributes(method, typeof(IMethodModifier), true);
            foreach (IMethodModifier modifier in modifiers)
                modifier.Modify(builder);
        }

        internal static void BuildParameter(ParameterBuilder builder, ParameterInfo parameter)
        {
            Debug.Assert(parameter != null);
            Debug.Assert(builder != null);

            //
            // Build...
            //
            
            builder.Name = parameter.Name;
            builder.ParameterType = parameter.ParameterType;
            builder.IsParamArray = parameter.IsDefined(typeof(ParamArrayAttribute), true);

            //
            // Modify...
            //
            
            object[] modifiers = GetCustomAttributes(parameter, typeof(IParameterModifier), true);
            foreach (IParameterModifier modifier in modifiers)
                modifier.Modify(builder);
        }

        private static object FindCustomAttribute(ICustomAttributeProvider provider, Type attributeType, bool inherit)
        {
            object[] attributes = GetCustomAttributes(provider, attributeType, inherit);
            return attributes.Length > 0 ? attributes[0] : null;
        }

        private static object[] GetCustomAttributes(ICustomAttributeProvider provider, Type attributeType, bool inherit)
        {
            object[] attributes = provider.GetCustomAttributes(attributeType, inherit);
            foreach (object attribute in attributes)
                TrySetAttachment(attribute, provider);
            return attributes;
        }

        private static void TrySetAttachment(object obj, ICustomAttributeProvider source)
        {
            IAttributeAttachment attachment = obj as IAttributeAttachment;
            if (attachment != null)
                attachment.SetAttachment(source);
        }

        private JsonRpcServiceReflector()
        {
            throw new NotSupportedException();
        }
    }
}
