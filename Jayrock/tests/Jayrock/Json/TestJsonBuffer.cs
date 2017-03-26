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
    public class TestJsonBuffer
    {
        [ Test ]
        public void IsSerializable()
        {
            Assert.IsTrue(typeof(JsonBuffer).IsSerializable);
        }

        [ Test ]
        public void EmptyHasZeroLength()
        {
            Assert.AreEqual(0, JsonBuffer.Empty.Length);
        }

        [Test]
        public void EmptyIsNotNull()
        {
            Assert.IsFalse(JsonBuffer.Empty.IsNull);
        }

        [Test]
        public void EmptyIsNotScalar()
        {
            Assert.IsFalse(JsonBuffer.Empty.IsScalar);
        }

        [Test]
        public void EmptyIsNotStructured()
        {
            Assert.IsFalse(JsonBuffer.Empty.IsStructured);
        }

        [Test]
        public void EmptyIsEmpty()
        {
            Assert.IsTrue(JsonBuffer.Empty.IsEmpty);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void EmptyCannotBeRead()
        {
            JsonBuffer.Empty.CreateReader();
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EmptyCannotBeIndexedInto()
        {
            /* JsonBuffer unused = */ JsonBuffer.Empty[0] /* [1] */ .ToString();
            
            //
            // [1] The ToString call is unnecessary but it is added here
            //     instead of taking the indexed value into an unused 
            //     variable to avoid the CS0219 warning issue from Mono's 
            //     C# compiler. See: 
            //     http://bugzilla.novell.com/show_bug.cgi?id=316137
        }

        [Test]
        public void WritingReading()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteStartArray();
            writer.WriteString("foo");
            writer.WriteString("bar");
            writer.WriteString("baz");
            writer.WriteEndArray();
            JsonBuffer buffer = writer.GetBuffer();
            Assert.AreEqual(5, buffer.Length);
            JsonBufferReader reader = buffer.CreateReader();
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("foo", reader.ReadString());
            Assert.AreEqual("bar", reader.ReadString());
            Assert.AreEqual("baz", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);
            Assert.IsTrue(reader.EOF);
        }

        [Test]
        public void ScalarValueReading()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteString("foobar");
            AssertBufferedValueScalarOrNull(JsonToken.String("foobar"), writer);
        }

        [Test]
        public void NullValueReading()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteNull();
            AssertBufferedValueScalarOrNull(JsonToken.Null(), writer);
        }

        [Test]
        public void BufferValueOnArrayEnd()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteFromReader(JsonText.CreateReader("[[],foo]"));
            JsonBufferReader reader = writer.GetBuffer().CreateReader();
            reader.Read(); // outer array start
            reader.Read(); // inner array start
            reader.Read(); // inner array end
            Assert.AreEqual(JsonToken.String("foo"), reader.BufferValue()[0]);
        }

        [Test]
        public void BufferValueOnObjectEnd()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteFromReader(JsonText.CreateReader("[{},foo]"));
            JsonBufferReader reader = writer.GetBuffer().CreateReader();
            reader.Read(); // outer object start
            reader.Read(); // inner object start
            reader.Read(); // inner object end
            Assert.AreEqual(JsonToken.String("foo"), reader.BufferValue()[0]);
        }

        [Test]
        public void BufferValueOnMember()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteFromReader(JsonText.CreateReader("{foo:bar}"));
            JsonBufferReader reader = writer.GetBuffer().CreateReader();
            reader.Read(); // object start
            reader.Read(); // foo
            Assert.AreEqual(JsonToken.String("bar"), reader.BufferValue().CreateReader().Token);
        }

        [Test]
        public void BufferValueOnEOF()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteFromReader(JsonText.CreateReader("[]"));
            JsonBufferReader reader = writer.GetBuffer().CreateReader();
            reader.Read(); // array start
            reader.Read(); // array end
            Assert.IsTrue(reader.BufferValue().IsEmpty);
        }

        [Test]
        public void BufferValueOnBOF()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteFromReader(JsonText.CreateReader("[]"));
            JsonBuffer buffer = writer.GetBuffer().CreateReader().BufferValue();
            Assert.AreEqual(2, buffer.Length);
            Assert.AreEqual(JsonToken.Array(), buffer[0]);
            Assert.AreEqual(JsonToken.EndArray(), buffer[1]);
        }

        [Test]
        public void BufferScalarValue()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteString("foobar");
            JsonBuffer buffer = writer.GetBuffer();
            JsonBufferReader reader = buffer.CreateReader();
            reader.Read(); // array start
            reader.Read(); // string
            Assert.AreEqual("foobar", reader.BufferValue().CreateReader().BufferValue().CreateReader().ReadString());
        }

        [Test]
        public void GetArrayLength()
        {
            Assert.AreEqual(3, JsonBuffer.From("[1,2,3]").GetArrayLength());
        }

        [Test]
        public void GetArray()
        {
            JsonBuffer[] values = new JsonBuffer[3];
            int count = JsonBuffer.From("[1,2,3]").GetArray(values);
            Assert.AreEqual(3, count);
            Assert.AreEqual(1, (int)values[0].GetNumber());
            Assert.AreEqual(2, (int)values[1].GetNumber());
            Assert.AreEqual(3, (int)values[2].GetNumber());
        }

        [Test]
        public void GetArrayShort()
        {
            JsonBuffer[] values = new JsonBuffer[2];
            int count = JsonBuffer.From("[1,2,3]").GetArray(values);
            Assert.AreEqual(~2, count);
            Assert.AreEqual(1, (int)values[0].GetNumber());
            Assert.AreEqual(2, (int)values[1].GetNumber());
        }

        [Test]
        public void GetArrayLong()
        {
            JsonBuffer[] values = new JsonBuffer[4];
            int count = JsonBuffer.From("[1,2,3]").GetArray(values);
            Assert.AreEqual(3, count);
            Assert.AreEqual(1, (int)values[0].GetNumber());
            Assert.AreEqual(2, (int)values[1].GetNumber());
            Assert.AreEqual(3, (int)values[2].GetNumber());
            Assert.IsTrue(values[3].IsEmpty);
        }

        [Test]
        public void GetArrayLengthOnNull()
        {
            Assert.AreEqual(0, JsonBuffer.From("[null]").GetArray()[0].GetArrayLength());
        }

        [Test]
        public void GetMemberCount()
        {
            Assert.AreEqual(3, JsonBuffer.From("{a:1,b:2,c:3}").GetMemberCount());
        }

        [Test]
        public void GetMembersIntoArray()
        {
            NamedJsonBuffer[] members = new NamedJsonBuffer[3];
            int count = JsonBuffer.From("{a:1,b:2,c:3}").GetMembers(members);
            Assert.AreEqual(3, count);
            Assert.AreEqual("a", members[0].Name);
            Assert.AreEqual(1, (int) members[0].Buffer.GetNumber());
            Assert.AreEqual("b", members[1].Name);
            Assert.AreEqual(2, (int) members[1].Buffer.GetNumber());
            Assert.AreEqual("c", members[2].Name);
            Assert.AreEqual(3, (int) members[2].Buffer.GetNumber());
        }

        [Test]
        public void GetMembersIntoShortArray()
        {
            NamedJsonBuffer[] members = new NamedJsonBuffer[2];
            int count = JsonBuffer.From("{a:1,b:2,c:3}").GetMembers(members);
            Assert.AreEqual(~2, count);
            Assert.AreEqual("a", members[0].Name);
            Assert.AreEqual(1, (int) members[0].Buffer.GetNumber());
            Assert.AreEqual("b", members[1].Name);
            Assert.AreEqual(2, (int) members[1].Buffer.GetNumber());
        }

        [Test]
        public void GetMembersIntoLongArray()
        {
            NamedJsonBuffer[] members = new NamedJsonBuffer[4];
            int count = JsonBuffer.From("{a:1,b:2,c:3}").GetMembers(members);
            Assert.AreEqual(3, count);
            Assert.AreEqual("a", members[0].Name);
            Assert.AreEqual(1, (int) members[0].Buffer.GetNumber());
            Assert.AreEqual("b", members[1].Name);
            Assert.AreEqual(2, (int) members[1].Buffer.GetNumber());
            Assert.AreEqual("c", members[2].Name);
            Assert.AreEqual(3, (int) members[2].Buffer.GetNumber());
            Assert.IsTrue(members[3].IsEmpty);
        }
        
        [Test]
        public void GetMembersIntoArrayPermitsNull()
        {
            Assert.AreEqual(3, JsonBuffer.From("{a:1,b:2,c:3}").GetMembers(null));
        }

        [Test]
        public void GetMembersArray()
        {
            NamedJsonBuffer[] members = JsonBuffer.From("{a:1,b:2,c:3}").GetMembersArray();
            Assert.AreEqual(3, members.Length);
            Assert.AreEqual("a", members[0].Name);
            Assert.AreEqual(1, (int) members[0].Buffer.GetNumber());
            Assert.AreEqual("b", members[1].Name);
            Assert.AreEqual(2, (int) members[1].Buffer.GetNumber());
            Assert.AreEqual("c", members[2].Name);
            Assert.AreEqual(3, (int) members[2].Buffer.GetNumber());
        }

        [Test]
        public void GetMembersCountOnNull()
        {
            Assert.AreEqual(0, JsonBuffer.From("[null]").GetArray()[0].GetMemberCount());
        }

        [Test]
        public void IsObject()
        {
            Assert.IsTrue(JsonBuffer.From("{}").IsObject);
        }

        [Test]
        public void IsArray()
        {
            Assert.IsTrue(JsonBuffer.From("[]").IsArray);
        }

        [Test]
        public void Equality()
        {
            Assert.AreEqual(JsonBuffer.From("[1,2,3]"), JsonBuffer.From("[1,2,3]"));
        }

        [Test]
        public void EmptyEquality()
        {
            Assert.AreEqual(JsonBuffer.Empty, JsonBuffer.Empty);
        }

        [Test]
        public void Inequality()
        {
            Assert.AreNotEqual(JsonBuffer.From("[1,2,three,4]"), JsonBuffer.From("[1,2,THREE,4]"));
        }

        [Test]
        public void OpEquality()
        {
            Assert.IsTrue(JsonBuffer.From("[1,2,3]") == JsonBuffer.From("[1,2,3]"));
        }

        [Test]
        public void OpInequality()
        {
            Assert.IsTrue(JsonBuffer.From("[1,2,three,4]") != JsonBuffer.From("[1,2,THREE,4]"));
        }

        [Test]
        public void StringRepresentation()
        {
            string str = JsonBuffer.From("[42,foobar,null,true,false,{x:123,y:456}]").ToString();
            Assert.AreEqual("[42,\"foobar\",null,true,false,{\"x\":123,\"y\":456}]", str);
        }

        [Test]
        public void StringRepresentationForEmpty()
        {
            Assert.AreEqual(string.Empty, JsonBuffer.Empty.ToString());
        }

        [Test]
        public void StringRepresentationForNull()
        {
            Assert.AreEqual("null", JsonBuffer.From(JsonToken.Null()).ToString());
        }

        [Test]
        public void StringRepresentationForString()
        {
            Assert.AreEqual("\"foo\\nbar\"", JsonBuffer.From(JsonToken.String("foo\nbar")).ToString());
        }

        [Test]
        public void StringRepresentationForNumber()
        {
            Assert.AreEqual("42", JsonBuffer.From(JsonToken.Number("42")).ToString());
        }

        [Test]
        public void StringRepresentationForTrue()
        {
            Assert.AreEqual("true", JsonBuffer.From(JsonToken.True()).ToString());
        }

        [Test]
        public void StringRepresentationForFalse()
        {
            Assert.AreEqual("false", JsonBuffer.From(JsonToken.False()).ToString());
        }

        [Test(Description = @"http://code.google.com/p/jayrock/issues/detail?id=26")]
        public void Issue26()
        {
            // 1. Create JsonBuffer from array with objects
            JsonBuffer buffer = JsonBuffer.From(@"[{},{a:{},b:{}}]");
            // 2. Create reader from the buffer...
            JsonBufferReader reader = buffer.CreateReader();
            //    ...read in the first object
            while (reader.TokenClass != JsonTokenClass.Object)
                reader.Read();
            reader.Read(); // Read Object token
            reader.Read(); // Read EndObject token

            //    ...create a subbuffer to buffer the next object
            JsonBuffer subBuffer = JsonBuffer.From(reader);
            //    ...create reader from the subbuffer
            JsonBufferReader reader2 = subBuffer.CreateReader();
            
            // 3. Call reader.BufferValue() this should break
            JsonBuffer buffer2 = reader2.BufferValue();
            Assert.IsTrue(buffer2.IsObject);
        }

        private static void AssertBufferedValueScalarOrNull(JsonToken expected, JsonBufferWriter writer) 
        {
            JsonBuffer buffer = writer.GetBuffer();
            JsonBufferReader reader = buffer.CreateReader();
            reader.Read();
            reader.Read();
            JsonBuffer value = reader.BufferValue();
            if (expected.Class == JsonTokenClass.Null)
                Assert.IsTrue(value.IsNull);
            else
                Assert.IsTrue(value.IsScalar);
            JsonBufferReader vr = value.CreateReader();
            Assert.AreEqual(1, vr.Depth);
            Assert.AreEqual(expected, vr.Token);
            vr.Read();
            vr.ReadToken(JsonTokenClass.EndArray);
            Assert.IsTrue(vr.EOF);
        }
    }
}