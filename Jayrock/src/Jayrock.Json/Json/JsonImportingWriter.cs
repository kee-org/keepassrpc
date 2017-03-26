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

namespace Jayrock.Json
{
    using System.Collections;

    public class JsonImportingWriter : JsonWriterBase
    {
        private readonly Stack _valueStack = new Stack();
        private readonly Stack _memberStack = new Stack();
        private JsonObject _object;
        private JsonArray _array;
        private object _value;
        private string _member;

        public object Value { get { return _value; } }
        public bool IsObject { get { return _object != null; } }
        public bool IsArray { get { return _array != null; } }

        private void Push()
        {
            _valueStack.Push(_value);
            _memberStack.Push(_member);
            _array = null;
            _object = null;
            _value = null;
            _member = null;
        }

        private void Pop()
        {
            object current = _value;
            object popped = _valueStack.Pop();
            _member = (string) _memberStack.Pop();
            if (popped == null) // Final result?
                return;
            _object = popped as JsonObject;
            _array = _object == null ? (JsonArray) popped : null;
            _value = popped;
            WriteValue(current);
        }
        
        protected override void WriteStartObjectImpl()
        {
            Push();
            _value = _object = new JsonObject();
        }

        protected override void WriteEndObjectImpl()
        {
            Pop();
        }

        protected override void WriteMemberImpl(string name)
        {
            _member = name;
        }

        protected override void WriteStartArrayImpl()
        {
            Push();
            _value = _array = new JsonArray();
        }

        protected override void WriteEndArrayImpl()
        {
            Pop();
        }

        private void WriteValue(object value)
        {
            if (IsObject)
            {
                _object[_member] = value;
                _member = null;
            }
            else
            {
                _array.Add(value);
            }
        }

        protected override void WriteStringImpl(string value)
        {
            WriteValue(value);
        }

        protected override void WriteNumberImpl(string value)
        {
            WriteValue(new JsonNumber(value));
        }

        protected override void WriteBooleanImpl(bool value)
        {
            WriteValue(value);
        }

        protected override void WriteNullImpl()
        {
            WriteValue(null);
        }
    }
}
