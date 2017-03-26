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
    #region Imports

    using System;
    using System.Diagnostics;

    #endregion

    /// <summary>
    /// Represents a reader that provides fast, non-cached, forward-only 
    /// access to a <see cref="JsonBuffer" /> object.
    /// </summary>

    public sealed class JsonBufferReader : JsonReaderBase
    {
        private readonly JsonBuffer _buffer;
        private int _index;

        public JsonBufferReader(JsonBuffer buffer) : 
            this(buffer, -1) {}

        private JsonBufferReader(JsonBuffer buffer, int index)
        {
            if (buffer.IsEmpty)
                throw new ArgumentException("buffer");

            Debug.Assert(index >= -1);
            Debug.Assert(index < buffer.Length);

            _buffer = buffer;
            _index = index;
        }

        protected override JsonToken ReadTokenImpl()
        {
            if (_buffer.IsStructured)
            {
                Debug.Assert(!EOF); // sanity check
                if (++_index < _buffer.Length)
                    return _buffer[_index];
            }
            else
            {
                switch (++_index)
                {
                    case 0: return JsonToken.Array();
                    case 1: return _buffer[0];
                    case 2: return JsonToken.EndArray();                    
                }
            }
            
            return JsonToken.EOF();
        }

        /// <summary>
        /// Buffers the value at which the reader is positioned.
        /// </summary>
        /// <returns>Returns a <see cref="JsonBuffer" /> object that holds
        /// the buffered value.</returns>

        public JsonBuffer BufferValue()
        {
            if (EOF)
                return JsonBuffer.Empty;

            if (!_buffer.IsStructured)
                return _buffer;
            
            JsonTokenClass tokenClass = TokenClass;
            if (tokenClass.IsTerminator || tokenClass == JsonTokenClass.Member)
                Read();

            int start = _index;
            Skip();
            return _buffer.Slice(start, _index);
        }
    }
}