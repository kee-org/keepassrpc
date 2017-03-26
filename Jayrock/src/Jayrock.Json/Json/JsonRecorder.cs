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
    #region Imports

    using System;
    using System.Collections;
    using System.Diagnostics;

    #endregion

    [ Serializable ]
    public sealed class JsonRecorder : JsonWriterBase
    {
        private int _count;
        private JsonToken[] _tokens;

        private void Write(JsonToken token)
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
        }

        protected override void WriteStartObjectImpl()
        {
            Write(JsonToken.Object());
        }

        protected override void WriteEndObjectImpl()
        {
            Write(JsonToken.EndObject());
        }

        protected override void WriteMemberImpl(string name)
        {
            Write(JsonToken.Member(name));
        }

        protected override void WriteStartArrayImpl()
        {
            Write(JsonToken.Array());
        }

        protected override void WriteEndArrayImpl()
        {
            Write(JsonToken.EndArray());
        }

        protected override void WriteStringImpl(string value)
        {
            Write(JsonToken.String(value));
        }

        protected override void WriteNumberImpl(string value)
        {
            Write(JsonToken.Number(value));
        }

        protected override void WriteBooleanImpl(bool value)
        {
            Write(JsonToken.Boolean(value));
        }

        protected override void WriteNullImpl()
        {
            Write(JsonToken.Null());
        }

        public JsonReader CreatePlayer()
        {
            if (Bracket != JsonWriterBracket.Pending && 
                Bracket != JsonWriterBracket.Closed)
            {
                throw new InvalidOperationException("JSON data cannot be read before it is complete.");
            }

            JsonToken[] tokens = new JsonToken[_count + 2];
            
            if (_count > 0)
                Array.Copy(_tokens, 0, tokens, 1, _count);
            
            tokens[0] = JsonToken.BOF();
            tokens[tokens.Length - 1] = JsonToken.EOF();

            return new JsonPlayer(tokens);
        }

        public void Playback(JsonWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.WriteFromReader(CreatePlayer());
        }

        public static JsonRecorder Record(JsonReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            JsonRecorder recorder = new JsonRecorder();
            recorder.WriteFromReader(reader);
            return recorder;
        }

        [ Serializable ]
        private sealed class JsonPlayer : JsonReaderBase
        {
            private int _index;
            private readonly JsonToken[] _tokens;

            public JsonPlayer(JsonToken[] tokens)
            {
                Debug.Assert(tokens != null);
                
                _tokens = tokens;
            }

            protected override JsonToken ReadTokenImpl()
            {
                return _tokens[++_index];
            }
        }
    }
}
