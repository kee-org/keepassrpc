#region License, Terms and Conditions
//
// Jayrock - A JSON-RPC implementation for the Microsoft .NET Framework
// Written by Atif Aziz (www.raboof.com)
// Copyright (c) Atif Aziz. All rights reserved.
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

namespace Jayrock.Json
{
    public abstract class JsonTokenWriterBase : JsonWriterBase 
    {
        protected abstract void Write(JsonToken token);

        protected override void WriteStartObjectImpl() { Write(JsonToken.Object()); }
        protected override void WriteEndObjectImpl() { Write(JsonToken.EndObject()); }
        protected override void WriteMemberImpl(string name) { Write(JsonToken.Member(name)); }
        protected override void WriteStartArrayImpl() { Write(JsonToken.Array()); }
        protected override void WriteEndArrayImpl() { Write(JsonToken.EndArray()); }
        protected override void WriteStringImpl(string value) { Write(JsonToken.String(value)); }
        protected override void WriteNumberImpl(string value) { Write(JsonToken.Number(value)); }
        protected override void WriteBooleanImpl(bool value) { Write(JsonToken.Boolean(value)); }
        protected override void WriteNullImpl() { Write(JsonToken.Null()); }
    }
}