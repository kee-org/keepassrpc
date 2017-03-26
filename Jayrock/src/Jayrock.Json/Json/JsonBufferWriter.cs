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

    #endregion

    /// <summary>
    /// Represents a writer that provides a fast, non-cached, forward-only means of 
    /// emitting JSON data info a memory.
    /// </summary>

    public sealed class JsonBufferWriter : JsonTokenWriterBase
    {
        private readonly JsonBufferStorage _storage;

        public JsonBufferWriter() : 
            this(16) {}

        public JsonBufferWriter(int initialCapacity)
        {
            if (initialCapacity < 0)
                throw new ArgumentOutOfRangeException("initialCapacity", initialCapacity, null);

            _storage = new JsonBufferStorage(initialCapacity);
        }

        protected override void Write(JsonToken token)
        {
            _storage.Write(token);
        }

        /// <summary>
        /// Gets the buffered JSON data.
        /// </summary>
        /// <returns>Returns a <see cref="JsonBuffer"/> object with the
        /// written and buffered JSON data.</returns>
        /// <remarks>
        /// This method method auto-completes the JSON data if it has not 
        /// been written in its entirety.
        /// </remarks>

        public JsonBuffer GetBuffer()
        {
            if (Depth > 0)
                AutoComplete();
            return _storage.ToBuffer();
        }
    }
}