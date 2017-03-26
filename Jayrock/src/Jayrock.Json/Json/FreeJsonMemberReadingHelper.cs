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
    using System.Diagnostics;

    #if NET_1_0 || NET_1_1
        using NamedJsonBufferList = System.Collections.ArrayList;
    #else
        using NamedJsonBufferList = System.Collections.Generic.List<NamedJsonBuffer>;
    #endif

    #endregion

    /// <summary>
    /// Helps reading JSON members from an underlying <see cref="JsonReader"/>
    /// without regard to the order (free) in which they appear. Members are 
    /// buffered or streamed as needed.
    /// </summary>
    /// <remarks>
    /// A <see cref="JsonReader"/> has a stream-oriented interface where 
    /// the underlying JSON data is tokenized and yielded sequentially.
    /// When reading a JSON object, members must be processed in the order 
    /// in which they are yielded by the underlying reader. If an 
    /// application requires some members of a JSON object to correctly
    /// process the remaining members then those member may need to be read 
    /// in advance to prepare for further processing. For example, suppose
    /// the JSON object <c>{ "type": "date", "format": "M/d/yyyy", "value": "5/5/2008" }</c>
    /// represents a date encoded in the <c>value</c> member as a JSON 
    /// string. Suppose further, the <c>type</c> confirms that the entire
    /// JSON object indeed describes a date and the <c>format</c> member
    /// specifies how to interpret the <c>value</c> member. To correctly 
    /// create the date, an application would have to assert the <c>type</c> 
    /// member, then read the <c>format</c> member to determine how to 
    /// decode the <c>value</c> member value correctly. If the application
    /// assumes that the members are always provided in the natural order 
    /// just described then it can simply use a <see cref="JsonReader"/> and 
    /// process them in sequence along with their values. However, if the 
    /// JSON object instead has members out of order, as in 
    /// <c>{ "value": "5/5/2008", "type": "date", "format": "M/d/yyyy" }</c>,
    /// then reading becomes a bit more cumbersome. The application would 
    /// have to buffer the <c>value</c> and <c>format</c> member values 
    /// until it has at least seen the <c>type</c> member and asserted its 
    /// value to be <c>date</c>. Then it has to return to the buffered value 
    /// of the <c>format</c> member and use it to finally decode the 
    /// also-buffered <c>value</c> member. This is where 
    /// <see cref="FreeJsonMemberReadingHelper"/> can help tremendously.
    /// The application can call its <see cref="ReadMember"/> method to
    /// read members in the natural order of processing (e.g., <c>type</c>,
    /// <c>format</c> and <c>value</c>) rather than how they have been 
    /// actually ordered by some source. <see cref="FreeJsonMemberReadingHelper"/>
    /// will buffer or stream from the underlying <see cref="JsonReader"/>
    /// as needed. Once the required members have been processed, the
    /// <see cref="GetTailReader"/> method can be used to read auxillary
    /// or optional members in their order of appearance.
    /// </remarks>

    public sealed class FreeJsonMemberReadingHelper
    {
        private readonly JsonReader _reader;
        private bool _started;
        private bool _ended;
        private NamedJsonBufferList _members; // buffered ones

        public FreeJsonMemberReadingHelper(JsonReader reader)
        {
            if (reader == null) 
                throw new ArgumentNullException("reader");

            JsonTokenClass clazz = reader.TokenClass;

            if (clazz != JsonTokenClass.BOF
                && clazz != JsonTokenClass.Object
                && clazz != JsonTokenClass.Member)
            {
                throw new ArgumentException(null, "reader");
            }

            _reader = reader;
        }

        /// <summary>
        /// Gets the <see cref="JsonReader"/> with which this instance was
        /// initialized.
        /// </summary>

        public JsonReader BaseReader
        {
            get { return _reader; }
        }

        private bool HasBufferedMembers
        {
            get { return _members != null && _members.Count > 0; }
        }

        /// <summary>
        /// Gets a reader than can be used to read remaining members,
        /// which could include buffered and non-buffered members.
        /// </summary>

        public JsonReader GetTailReader()
        {
            JsonReader reader = new TailReader(_reader, _members);
            reader.ReadToken(JsonTokenClass.Object);
            return reader;
        }

        /// <summary>
        /// Reads a member with a given (case-sensitive) name and returns 
        /// a <see cref="JsonReader"/> that can be used to read the value.
        /// Otherwise it throws an exception.
        /// </summary>
        /// <exception cref="JsonException">
        /// A member with the requested name was not found.
        /// </exception>
        /// <remarks>
        /// The caller should not use the returned <see cref="JsonReader"/>
        /// instance for any other purpose but reading the value. It is
        /// possible that this method will return the same instance as
        /// <see cref="BaseReader"/> or a separate instance.
        /// </remarks>

        public JsonReader ReadMember(string name)
        {
            JsonReader reader = TryReadMember(name);

            if (reader == null)
            {
                throw new JsonException(string.Format(
                    @"Member named '{0}' not present in JSON object.", name));
            }

            return reader;
        }

        /// <summary>
        /// Attempts to locate a member with a given (case-sensitive) name 
        /// and returns a <see cref="JsonReader"/> that can be used to read 
        /// the value. Otherwise it returns <c>null</c>.
        /// </summary>
        /// <remarks>
        /// The caller should not use the returned <see cref="JsonReader"/>
        /// instance for any other purpose but reading the value. It is
        /// possible that this method will return the same instance as
        /// <see cref="BaseReader"/> or a separate instance.
        /// </remarks>

        public JsonReader TryReadMember(string name)
        {
            //
            // Is the member already buffered? If yes then return a reader 
            // on its buffered value.
            //

            JsonBuffer value = TryPopBufferedMember(name);
            if (!value.IsEmpty)
                return value.CreateReader();

            //
            // Use the base reader from here on if it has not already been
            // exhausted...
            //

            if (_ended)
                return null;

            JsonReader reader = BaseReader;

            if (!_started)
            {
                _started = true;
                if (!reader.MoveToContent())
                {
                    throw new JsonException(string.Format(
                        @"Unexpected EOF while attempting to look for member named '{0}'.",
                        name));
                }

                JsonTokenClass clazz = reader.TokenClass;
                if (clazz != JsonTokenClass.Object &&
                    clazz != JsonTokenClass.Member)
                {
                    throw new JsonException(string.Format(
                        @"Found {0} where a JSON Object or Member was expected.", clazz));
                }
            }

            //
            // If the base reader is sitting on the start of an object then 
            // move into it. This case should only arise on the first read
            // into a JSON object.
            //

            if (reader.TokenClass == JsonTokenClass.Object)
                reader.Read();

            //
            // Go over the entire JSON object until its end.
            //

            while (reader.TokenClass != JsonTokenClass.EndObject)
            {
                //
                // Read the next member and if it matches what's being
                // sought then simply return the base reader that
                // should be aligned on the value.
                //

                string actualName = reader.ReadMember();
                if (string.CompareOrdinal(actualName, name) == 0)
                    return reader;

                //
                // Not the sought member so buffer it to be served
                // later when it is sought or as part of the tail.
                //

                NamedJsonBufferList members = _members;
                if (members == null)
                    members = _members = new NamedJsonBufferList(4);
                members.Add(new NamedJsonBuffer(actualName, JsonBuffer.From(reader)));
            }

            _ended = true;

            //
            // Member not found.
            //

            return null;
        }

        private JsonBuffer TryPopBufferedMember(string name)
        {
            if (HasBufferedMembers)
            {
                NamedJsonBufferList members = _members;
                for (int i = 0; i < members.Count; i++)
                {
                    NamedJsonBuffer member = (NamedJsonBuffer) members[i];
                    if (string.CompareOrdinal(member.Name, name) == 0)
                    {
                        members.RemoveAt(i);
                        if (members.Count == 0)
                            _members = null;
                        return member.Buffer;
                    }
                }
            }

            return JsonBuffer.Empty;
        }

        /// <summary>
        /// Returns the same string returned by the 
        /// <see cref="JsonReader.ToString"/> implementation of 
        /// <see cref="BaseReader"/>.
        /// </summary>

        public override string ToString()
        {
            return _reader.ToString();
        }

        private sealed class TailReader : JsonReaderBase
        {
            private JsonReader _reader;
            private int _innerDepth;
            private readonly NamedJsonBufferList _bufferedMembers;
            private JsonBuffer _memberValue;
            private JsonReader _memberStructuredValueReader;
            private int _index; // -1 = BOF; 0...(N-1) = buffered members; N = streamed members

            public TailReader(JsonReader reader, NamedJsonBufferList bufferedMembers)
            {
                Debug.Assert(reader != null);

                _reader = reader;
                _innerDepth = reader.Depth;
                _bufferedMembers = bufferedMembers;
                _index = -1;
            }

            protected override JsonToken ReadTokenImpl()
            {
                JsonReader baseReader = _reader;

                int index = _index;
                if (index < 0)
                {
                    //
                    // If the base reader was never started then do it now.
                    // Check for zero depth is the same as BOF.
                    //
                    if (baseReader.Depth == 0)
                    {
                        baseReader.ReadToken(JsonTokenClass.Object);
                        _innerDepth = baseReader.Depth;
                    }

                    _index = 0;
                    return JsonToken.Object();
                }

                //
                // Buffered members, if any, get served first.
                //

                NamedJsonBufferList bufferedMembers = _bufferedMembers;
                if (bufferedMembers != null && index < bufferedMembers.Count)
                {
                    //
                    // Is there a value to serve?
                    //

                    JsonBuffer value = _memberValue;
                    if (!value.IsEmpty)
                    {
                        if (value.IsStructured) // JSON Array or Object
                        {
                            //
                            // Get a reader on the structured (array or 
                            // object) value if not already acquired.
                            //

                            JsonReader valueReader = _memberStructuredValueReader;
                            if (valueReader == null)
                                valueReader = _memberStructuredValueReader = value.CreateReader();

                            //
                            // Serve tokens from the value reader until OEF.
                            //

                            if (valueReader.Read())
                                return valueReader.Token;

                            //
                            // Proceed with next.
                            //

                            _memberValue = JsonBuffer.Empty;
                            _memberStructuredValueReader = null;
                            _index++;
                            return ReadTokenImpl();
                        }
                        else // JSON Null, Boolean, Number or String
                        {
                            Debug.Assert(value.IsNull || value.IsScalar);

                            //
                            // Establish state to proceed with next and 
                            // serve token pertaining to scalar value.
                            //

                            _memberValue = JsonBuffer.Empty;
                            _index++;
                            
                            return value.IsNull 
                                       ? JsonToken.Null() 
                                       : value.CreateReader().Token;
                        }
                    }

                    //
                    // Serve buffered member name and establish state so 
                    // that the value will be served next.
                    //

                    NamedJsonBuffer member = (NamedJsonBuffer) bufferedMembers[index];
                    _memberValue = member.Buffer;
                    return JsonToken.Member(member.Name);
                }

                //
                // Done with buffered members so now move on to remaining
                // members from the underlying reader.
                //

                if (baseReader == null)
                    return JsonToken.EOF(); // Done with serving

                //
                // If base reader is on the object end at the same depth as
                // on entry then serve the last end token. The reader is
                // released so that on next read, the above will signal EOF.
                //

                if (baseReader.Depth == _innerDepth 
                    && baseReader.TokenClass == JsonTokenClass.EndObject)
                {
                    _reader = null;
                }

                //
                // Move through the base reader.
                //

                JsonToken token = baseReader.Token;
                baseReader.Read();
                return token;
            }
        }
    }
}