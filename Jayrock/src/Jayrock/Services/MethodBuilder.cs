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

namespace Jayrock.Services
{
    #region Imports

    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Reflection;

    #endregion

    [ Serializable ]
    public sealed class MethodBuilder
    {
        private string _name;
        private string _internalName;
        private Type _resultType = typeof(void);
        private ArrayList _parameterList;
        private IList _roParameterList;
        private IMethodImpl _handler;
        private string _description;
        private readonly ServiceClassBuilder _serviceClass;
        private ICustomAttributeProvider _attributes;
        private bool _idempotent;

        public static readonly ICustomAttributeProvider ZeroAttributes = new NoCustomAttributeProvider();

        internal MethodBuilder(ServiceClassBuilder serviceClass)
        {
            Debug.Assert(serviceClass != null);
            _serviceClass = serviceClass;
            _attributes = ZeroAttributes;
        }

        public ServiceClassBuilder ServiceClass
        {
            get { return _serviceClass; }
        }

        public string Name
        {
            get { return Mask.NullString(_name); }
            set { _name = value; }
        }

        public string InternalName
        {
            get { return Mask.NullString(_internalName); }
            set { _internalName = value; }
        }

        public Type ResultType
        {
            get { return _resultType; }
                
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                    
                _resultType = value;
            }
        }

        public IMethodImpl Handler
        {
            get { return _handler; }
            set { _handler = value; }
        }

        public ICustomAttributeProvider CustomAttributes
        {
            get { return _attributes; }

            set
            {
                if (value == null) 
                    throw new ArgumentNullException("value");

                _attributes = value;
            }
        }

        public string Description
        {
            get { return Mask.NullString(_description); }
            set { _description = value; }
        }

        public bool Idempotent
        {
            get { return _idempotent; }
            set { _idempotent = value; }
        }

        public ParameterBuilder DefineParameter()
        {
            ParameterBuilder builder = new ParameterBuilder(this);
            ParameterList.Add(builder);
            return builder;
        }

        public IList Parameters
        {
            get
            {
                if (_roParameterList == null)
                    _roParameterList = ArrayList.ReadOnly(ParameterList);

                return _roParameterList;
            }
        }

        public bool HasParameters
        {
            get { return _parameterList != null && _parameterList.Count > 0; }
        }

        private ArrayList ParameterList
        {
            get
            {
                if (_parameterList == null)
                    _parameterList = new ArrayList();
                
                return _parameterList;
            }
        }

        private sealed class NoCustomAttributeProvider : ICustomAttributeProvider
        {
            private static readonly object[] _zeroObjects = new object[0];

            public object[] GetCustomAttributes(Type attributeType, bool inherit)
            {
                if (attributeType == null) throw new ArgumentNullException("attributeType");
                return _zeroObjects;
            }

            public object[] GetCustomAttributes(bool inherit)
            {
                return GetCustomAttributes(typeof(object), true);
            }

            public bool IsDefined(Type attributeType, bool inherit)
            {
                if (attributeType == null) throw new ArgumentNullException("attributeType");
                return false;
            }
        }
    }
}