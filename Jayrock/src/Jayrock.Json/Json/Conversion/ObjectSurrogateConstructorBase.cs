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
    using System;

    public abstract class ObjectSurrogateConstructorBase : 
        IObjectSurrogateConstructor, 
        INonObjectMemberImporter
    {
        private JsonBufferWriter _tailw;

        public virtual bool Import(ImportContext context, string name, JsonReader reader)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (reader == null) throw new ArgumentNullException("reader");

            JsonBufferWriter tailw = _tailw;
            if (tailw == null)
            {
                tailw = _tailw = new JsonBufferWriter();
                tailw.WriteStartObject();
            }
            tailw.WriteMember(name);
            tailw.WriteFromReader(reader);
            return true;
        }

        public virtual ObjectConstructionResult CreateObject(ImportContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            JsonBufferWriter tailw = _tailw;
            _tailw = null;
            if (tailw != null)
                tailw.WriteEndObject();
            JsonBuffer tail = (tailw != null ? tailw.GetBuffer() : StockJsonBuffers.EmptyObject);
            object obj = OnCreateObject(context);
            return new ObjectConstructionResult(obj, tail.CreateReader());
        }

        public abstract object OnCreateObject(ImportContext context);
    }
}