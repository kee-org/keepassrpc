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

    using System.Diagnostics;

    #endregion

    /// <summary>
    /// Serves as the shared backing storage for <see cref="JsonBuffer" /> 
    /// objects. A <see cref="JsonBuffer" /> object is merely a 
    /// constrained view on an object of this type.
    /// </summary>

    internal sealed class JsonBufferStorage
    {
        private JsonToken[] _tokens;
        private int _count;

        internal JsonBufferStorage(int initialCapacity)
        {
            Debug.Assert(initialCapacity >= 0);
            if (initialCapacity > 0)
                _tokens = new JsonToken[initialCapacity];
        }

        public int Length { get { return _count; } }

        public JsonBufferStorage Write(JsonToken token)
        {
            if (_tokens == null)
            {
                _tokens = new JsonToken[16];
            }
            else if (_count == _tokens.Length)
            {
                JsonToken[] tokens = new JsonToken[_tokens.Length * 2];
                _tokens.CopyTo(tokens, 0);
                _tokens = tokens;
            }

            _tokens[_count++] = token;
            return this;
        }

        public JsonBufferStorage Write(params JsonToken[] tokens)
        {
            foreach (JsonToken token in tokens)
                Write(token);
            return this;
        }

        public JsonToken this[int index]
        {
            get
            {
                Debug.Assert(index >= 0);
                Debug.Assert(index < _count);
                return _tokens[index];
            }
        }

        public JsonBuffer ToBuffer()
        {
            return new JsonBuffer(this, 0, _count);
        }
    }
}