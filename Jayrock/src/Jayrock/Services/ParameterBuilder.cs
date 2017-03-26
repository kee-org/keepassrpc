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
    using System.Diagnostics;

    #endregion

    [ Serializable ]
    public sealed class ParameterBuilder
    {
        private string _name;
        private Type _parameterType = typeof(object);
        private bool _isParamArray;
        private MethodBuilder _method;

        internal ParameterBuilder(MethodBuilder method)
        {
            Debug.Assert(method != null);
                
            _method = method;
        }

        public MethodBuilder Method
        {
            get { return _method; }
        }

        public string Name
        {
            get { return Mask.NullString(_name); }
            set { _name = value; }
        }

        public int Position
        {
            get { return Method.Parameters.IndexOf(this); }
        }
            
        public Type ParameterType
        {
            get { return _parameterType; }
                
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                    
                _parameterType = value;
            }
        }

        public bool IsParamArray
        {
            get { return _isParamArray; }
            set { _isParamArray = value; }
        }
    }
}