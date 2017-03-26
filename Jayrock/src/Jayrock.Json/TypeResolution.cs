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

namespace Jayrock
{
    #region Imports

    using System;

    #endregion

    public delegate Type TypeResolutionHandler(string typeName, bool throwOnError, bool ignoreCase);

    public sealed class TypeResolution
    {
        private static TypeResolutionHandler _current;
        private static readonly TypeResolutionHandler _default;

        static TypeResolution()
        {
            _current = _default = new TypeResolutionHandler(Type.GetType);
        }

        public static TypeResolutionHandler Default
        {
            get { return _default; }
        }

        public static TypeResolutionHandler Current
        {
            get { return _current; }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _current = value;
            }
        }

        public static Type FindType(string typeName)
        {
            return Current(typeName, /* throwOnError = */ false, /* ignoreCase = */ false);
        }

        public static Type GetType(string typeName)
        {
            return Current(typeName, /* throwOnError = */ true, /* ignoreCase = */ false);
        }
        
        private TypeResolution()
        {
            throw new NotSupportedException();
        }
    }
}
