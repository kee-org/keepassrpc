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

    #endregion

    /// <summary>
    /// Represents the object resulting from construction as well as
    /// a tail holding any remainders.
    /// </summary>

    public sealed class ObjectConstructionResult
    {
        private readonly object _obj;
        private readonly JsonReader _tail;

        public ObjectConstructionResult(object obj, JsonReader tail)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (tail == null) throw new ArgumentNullException("tail");

            _obj = obj;
            _tail = tail;
        }

        public object Object { get { return _obj; } }
        public JsonReader TailReader { get { return _tail; } }
    }
}