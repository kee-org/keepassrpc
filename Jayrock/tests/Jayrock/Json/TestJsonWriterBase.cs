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
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonWriterBase
    {
        private JsonWriter _writer;
        
        [ SetUp ]
        public void Init()
        {
            _writer = new EmptyJsonWriter();
        }

        [ Test ]
        public void StartingDepthIsZero()
        {
            Assert.AreEqual(0, _writer.Depth);
        }
       
        [ Test ]
        public void DepthIncreasesInsideObject()
        {
            _writer.WriteStartObject();
            Assert.AreEqual(1, _writer.Depth);
            _writer.WriteMember(string.Empty);
            _writer.WriteStartObject();
            Assert.AreEqual(2, _writer.Depth);
            _writer.WriteEndObject();
            Assert.AreEqual(1, _writer.Depth);
            _writer.WriteEndObject();
            Assert.AreEqual(0, _writer.Depth);
        }

        [ Test ]
        public void DepthIncreasesInsideArray()
        {
            _writer.WriteStartArray();
            Assert.AreEqual(1, _writer.Depth);
            _writer.WriteStartArray();
            Assert.AreEqual(2, _writer.Depth);
            _writer.WriteEndArray();
            Assert.AreEqual(1, _writer.Depth);
            _writer.WriteEndArray();
            Assert.AreEqual(0, _writer.Depth);
        }
        
        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotWriteObjectAfterRoot()
        {
            _writer.WriteStartObject();
            _writer.WriteEndObject();
            _writer.WriteStartObject();
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotWriteArrayAfterRoot()
        {
            _writer.WriteStartObject();
            _writer.WriteEndObject();
            _writer.WriteStartArray();
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotWriteMemberInsideArray()
        {
            _writer.WriteStartArray();
            _writer.WriteMember("Member");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotEndObjectWithArray()
        {
            _writer.WriteStartObject();
            _writer.WriteEndArray();
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotEndArrayWithObject()
        {
            _writer.WriteStartArray();
            _writer.WriteEndObject();
        }
        
        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotWriteStringWithoutMember()
        {
            _writer.WriteStartObject();
            _writer.WriteString("string");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotWriteNullWithoutMember()
        {
            _writer.WriteStartObject();
            _writer.WriteNull();
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotWriteNumberWithoutMember()
        {
            _writer.WriteStartObject();
            _writer.WriteNumber(123);
        }
        
        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotWriteBooleanWithoutMember()
        {
            _writer.WriteStartObject();
            _writer.WriteBoolean(true);
        }
        
        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotWriteNestedObjectWithoutMember()
        {
            _writer.WriteStartObject();
            _writer.WriteStartObject();
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotWriteNestedArrayWithoutMember()
        {
            _writer.WriteStartObject();
            _writer.WriteStartArray();
        }

        [ Test ]
        public void WriteStringInsideArray()
        {
            _writer.WriteStartArray();
            _writer.WriteString("string");
        }

        [ Test ]
        public void WriteNumberInsideArray()
        {
            _writer.WriteStartArray();
            _writer.WriteNumber(123);
        }

        [ Test ]
        public void WriteBooleanInsideArray()
        {
            _writer.WriteStartArray();
            _writer.WriteBoolean(true);
        }
        
        [ Test ]
        public void WriteNullInsideArray()
        {
            _writer.WriteStartArray();
            _writer.WriteNull();
        }
     
        [ Test ]
        public void WriteObjectMemberString()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("member");
            _writer.WriteString("string");
        }

        [ Test ]
        public void WriteObjectMemberNumber()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("member");
            _writer.WriteNumber(123);
        }

        [ Test ]
        public void WriteObjectMemberBoolean()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("member");
            _writer.WriteBoolean(true);
        }
        
        [ Test ]
        public void WriteObjectMemberNull()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("member");
            _writer.WriteNull();
        }

        [ Test ]
        public void WriteObjectWithNullMembers()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("member1");
            _writer.WriteNull();
            _writer.WriteMember("member2");
            _writer.WriteNull();
        }

        [ Test ]
        public void WriteObjectWithNumberMembers()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("member1");
            _writer.WriteNumber(123);
            _writer.WriteMember("member2");
            _writer.WriteNumber(456);
        }

        [ Test ]
        public void WriteObjectWithBooleanMembers()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("member1");
            _writer.WriteBoolean(true);
            _writer.WriteMember("member2");
            _writer.WriteBoolean(false);
        }

        [ Test ]
        public void WriteObjectWithStringMembers()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("member1");
            _writer.WriteString("string1");
            _writer.WriteMember("member2");
            _writer.WriteString("string2");
        }

        [ Test ]
        public void InitialBracket()
        {
            Assert.AreEqual(JsonWriterBracket.Pending, _writer.Bracket);
        }

        [ Test ]
        public void BracketAfterStartingObject()
        {
            _writer.WriteStartObject();
            Assert.AreEqual(JsonWriterBracket.Object, _writer.Bracket);
        }

        [ Test ]
        public void BracketAfterStartingArray()
        {
            _writer.WriteStartArray();
            Assert.AreEqual(JsonWriterBracket.Array, _writer.Bracket);
        }

        [ Test ]
        public void BracketAfterWritingMember()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("test");
            Assert.AreEqual(JsonWriterBracket.Member, _writer.Bracket);
        }

        [ Test ]
        public void BracketAfterWritingMemberValue()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("test");
            _writer.WriteNull();
            Assert.AreEqual(JsonWriterBracket.Object, _writer.Bracket);
        }

        [ Test ]
        public void BracketAfterEndingObject()
        {
            _writer.WriteStartObject();
            _writer.WriteEndObject();
            Assert.AreEqual(JsonWriterBracket.Closed, _writer.Bracket);
        }

        [ Test ]
        public void BracketAfterEndingArray()
        {
            _writer.WriteStartArray();
            _writer.WriteEndArray();
            Assert.AreEqual(JsonWriterBracket.Closed, _writer.Bracket);
        }

        [ Test ]
        public void NestedBracketing()
        {
            Assert.AreEqual(JsonWriterBracket.Pending, _writer.Bracket);
            _writer.WriteStartArray();
            Assert.AreEqual(JsonWriterBracket.Array, _writer.Bracket);
            _writer.WriteStartObject();
            Assert.AreEqual(JsonWriterBracket.Object, _writer.Bracket);
            _writer.WriteEndObject();
            Assert.AreEqual(JsonWriterBracket.Array, _writer.Bracket);
            _writer.WriteEndArray();
            Assert.AreEqual(JsonWriterBracket.Closed, _writer.Bracket);
        }

        [ Test ]
        public void InitialIndex()
        {
            Assert.AreEqual(-1, _writer.Index);
        }

        [ Test ]
        public void IndexZeroOnStartingObject()
        {
            _writer.WriteStartObject();
            Assert.AreEqual(0, _writer.Index);
        }

        [ Test ]
        public void IndexZeroAfertStartingArray()
        {
            _writer.WriteStartArray();
            Assert.AreEqual(0, _writer.Index);
        }

        [ Test ]
        public void IndexRunInsideArray()
        {
            _writer.WriteStartArray();
            Assert.AreEqual(0, _writer.Index);
            _writer.WriteNull();
            Assert.AreEqual(1, _writer.Index);
            _writer.WriteString("foo");
            Assert.AreEqual(2, _writer.Index);
            _writer.WriteNumber(42);
            Assert.AreEqual(3, _writer.Index);
            _writer.WriteBoolean(true);
            Assert.AreEqual(4, _writer.Index);
            _writer.WriteEndArray();
            Assert.AreEqual(-1, _writer.Index);
        }

        [ Test ]
        public void IndexRunInsideObject()
        {
            _writer.WriteStartObject();
            Assert.AreEqual(0, _writer.Index);
            
            _writer.WriteMember("m1");
            Assert.AreEqual(0, _writer.Index);
            _writer.WriteNull();
            Assert.AreEqual(1, _writer.Index);
            
            _writer.WriteMember("m2");
            Assert.AreEqual(1, _writer.Index);
            _writer.WriteString("foo");
            Assert.AreEqual(2, _writer.Index);
            
            _writer.WriteMember("m3");
            Assert.AreEqual(2, _writer.Index);
            _writer.WriteNumber(42);
            Assert.AreEqual(3, _writer.Index);

            _writer.WriteMember("m4");
            Assert.AreEqual(3, _writer.Index);
            _writer.WriteBoolean(true);
            Assert.AreEqual(4, _writer.Index);
            
            _writer.WriteEndObject();
            Assert.AreEqual(-1, _writer.Index);
        }

        [ Test ]
        public void IndexingAcrossNestings()
        {
            _writer.WriteStartObject();
            
            _writer.WriteMember("m1");
            _writer.WriteNull();
            Assert.AreEqual(1, _writer.Index);
            
            _writer.WriteMember("m2");
            _writer.WriteStartArray();
            Assert.AreEqual(0, _writer.Index);
        
            _writer.WriteNull();
            Assert.AreEqual(1, _writer.Index);

            _writer.WriteNull();
            Assert.AreEqual(2, _writer.Index);
            
            _writer.WriteNull();
            Assert.AreEqual(3, _writer.Index);

            _writer.WriteEndArray();
            Assert.AreEqual(2, _writer.Index);
        }

        [ Test ]
        public void DefaultMaxDepth()
        {
            Assert.AreEqual(30, _writer.MaxDepth);
        }

        [ Test ]
        public void SetMaxDepth()
        {
            Assert.AreNotEqual(42, _writer.MaxDepth);
            _writer.MaxDepth = 42;
            Assert.AreEqual(42, _writer.MaxDepth);
        }
        
        [ Test ]
        [ ExpectedException(typeof(Exception)) ]
        public void CannotExceedMaxDepth()
        {
            _writer.MaxDepth = 2;
            _writer.WriteStartArray();
            _writer.WriteStartArray();
            _writer.WriteStartArray();
        }
    }
}