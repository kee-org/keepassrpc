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
    public class TestFreeJsonMemberReadingHelper
    {
        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotInitializeWithNullReader()
        {
            new FreeJsonMemberReadingHelper(null);
        }

        [ Test ]
        public void BaseReaderInitialization()
        {
            JsonReader reader = JsonText.CreateReader("[]");
            FreeJsonMemberReadingHelper helper = new FreeJsonMemberReadingHelper(reader);
            Assert.AreSame(reader, helper.BaseReader);
        }

        [ Test ]
        public void StringRepresentation()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper("[]");
            Assert.AreEqual(helper.BaseReader.ToString(), helper.ToString());
        }

        [ Test ]
        public void UnorderedReading()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper(@"{ y: 456, x: 123, z: 789 }");
            Assert.AreEqual(123, helper.ReadMember("x").ReadNumber().ToInt32());
            Assert.AreEqual(456, helper.ReadMember("y").ReadNumber().ToInt32());
            Assert.AreEqual(789, helper.ReadMember("z").ReadNumber().ToInt32());
            Assert.AreEqual(JsonTokenClass.EndObject, helper.BaseReader.TokenClass);
        }

        [ Test ]
        public void OrderedReading()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper(@"{ x: 123, y: 456, z: 789 }");
            Assert.AreEqual(123, helper.ReadMember("x").ReadNumber().ToInt32());
            Assert.AreEqual(456, helper.ReadMember("y").ReadNumber().ToInt32());
            Assert.AreEqual(789, helper.ReadMember("z").ReadNumber().ToInt32());
            Assert.AreEqual(JsonTokenClass.EndObject, helper.BaseReader.TokenClass);
        }
        
        [ Test ]
        public void TailMemberAfterUnorderedReader()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper(@"
                { y: 456, x: 123, z: 789, comment: tail }");
            Assert.AreEqual(123, helper.ReadMember("x").ReadNumber().ToInt32());
            Assert.AreEqual(456, helper.ReadMember("y").ReadNumber().ToInt32());
            Assert.AreEqual(789, helper.ReadMember("z").ReadNumber().ToInt32());
            JsonReader reader = helper.BaseReader;
            Assert.AreEqual("comment", reader.ReadMember());
            Assert.AreEqual("tail", reader.ReadString());
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotReadNonExistingMember()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper(@"{ y: 456, x: 123, z: 789 }");
            helper.ReadMember("foo");
        }

        [ Test ]
        public void TryReadNonExistingMemberGivenEmptyObject()
        {
            Assert.IsNull(CreateHelper("{}").TryReadMember("foo"));
        }

        [ Test ]
        public void TryReadNonExistingMember()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper(@"{ y: 456, x: 123, z: 789 }");
            Assert.IsNull(helper.TryReadMember("foo"));
        }

        [ Test ]
        public void TryReadMemberCaseSensitivity()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper(@"{ y: 456, x: 123, z: 789 }");
            Assert.IsNull(helper.TryReadMember("X"));
        }

        [ Test ]
        public void TailReading()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper(@"
                { y: 456, 
                  x: 123, 
                  z: 789, 
                  obj1: { foo: bar }, 
                  comment: null, 
                  arr: [ 123, 456, 789 ],
                  obj2: { a: 1, b: 2, }, }");
            Assert.AreEqual(123, helper.ReadMember("x").ReadNumber().ToInt32());
            helper.ReadMember("comment").ReadNull();
            JsonReader reader = helper.BaseReader;
            Assert.AreEqual(JsonTokenClass.Member, reader.TokenClass);
            Assert.AreEqual("arr", reader.Text);
            JsonReader tail = helper.GetTailReader();
            Assert.AreEqual("y", tail.ReadMember());
            Assert.AreEqual(456, tail.ReadNumber().ToInt32());
            Assert.AreEqual("z", tail.ReadMember());
            Assert.AreEqual(789, tail.ReadNumber().ToInt32());
            Assert.AreEqual("obj1", tail.ReadMember());
            tail.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("foo", tail.ReadMember());
            Assert.AreEqual("bar", tail.ReadString());
            tail.ReadToken(JsonTokenClass.EndObject);
            Assert.AreEqual("arr", tail.ReadMember());
            tail.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual(123, tail.ReadNumber().ToInt32());
            Assert.AreEqual(456, tail.ReadNumber().ToInt32());
            Assert.AreEqual(789, tail.ReadNumber().ToInt32());
            tail.ReadToken(JsonTokenClass.EndArray);
            Assert.AreEqual("obj2", tail.ReadMember());
            tail.StepOut();
            Assert.IsFalse(reader.Read());
        }

        [ Test ]
        public void EmptyTailReading()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper(@"{ y: 456, x: 123, z: 789 }");
            Assert.AreEqual(123, helper.ReadMember("x").ReadNumber().ToInt32());
            Assert.AreEqual(456, helper.ReadMember("y").ReadNumber().ToInt32());
            Assert.AreEqual(789, helper.ReadMember("z").ReadNumber().ToInt32());
            JsonReader tail = helper.GetTailReader();
            tail.ReadToken(JsonTokenClass.EndObject);
            Assert.IsFalse(tail.Read());
        }

        [ Test ]
        public void TailReadingWithNoBufferedMembers()
        {
            JsonReader tail = CreateHelper(@"{ y: 456, x: 123, z: 789 }").GetTailReader();
            Assert.AreEqual("y", tail.ReadMember());
            Assert.AreEqual(456, tail.ReadNumber().ToInt32());
            Assert.AreEqual("x", tail.ReadMember());
            Assert.AreEqual(123, tail.ReadNumber().ToInt32());
            Assert.AreEqual("z", tail.ReadMember());
            Assert.AreEqual(789, tail.ReadNumber().ToInt32());
            tail.ReadToken(JsonTokenClass.EndObject);
            Assert.IsFalse(tail.Read());
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotTailReadEmptySource()
        {
            CreateHelper(string.Empty).GetTailReader();
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotTailReadOnArray()
        {
            CreateHelper("[]").GetTailReader();
        }

        [Test, ExpectedException(typeof(JsonException))]
        public void CannotReadMemberOnArray()
        {
            CreateHelper("[]").ReadMember("foo");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotReadMemberOnString()
        {
            CreateHelper("foo").ReadMember("foo");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotReadMemberOnNumber()
        {
            CreateHelper("123").ReadMember("foo");
        }

        [ Test ]
        public void ScopedToCurrentObject()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper("[{},{foo:bar}]");
            helper.BaseReader.ReadToken(JsonTokenClass.Array);
            Assert.IsNull(helper.TryReadMember("foo"));
            Assert.IsNull(helper.TryReadMember("foo"));
        }

        [ Test ]
        public void ReadingSameMemberMoreThanOnce()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper("{foo:bar}");
            helper.ReadMember("foo").Skip();
            Assert.IsNull(helper.TryReadMember("foo"));
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void CannotInitializeWithReaderOnEnd()
        {
            JsonReader reader = JsonText.CreateReader("42");
            reader.ReadNumber();
            new FreeJsonMemberReadingHelper(reader);
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void CannotInitializeWithReaderOnNull()
        {
            JsonReader reader = JsonText.CreateReader("null");
            reader.MoveToContent();
            new FreeJsonMemberReadingHelper(reader);
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void CannotInitializeWithReaderOnNumber()
        {
            JsonReader reader = JsonText.CreateReader("42");
            reader.MoveToContent();
            new FreeJsonMemberReadingHelper(reader);
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void CannotInitializeWithReaderOnBoolean()
        {
            JsonReader reader = JsonText.CreateReader("true");
            reader.MoveToContent();
            new FreeJsonMemberReadingHelper(reader);
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void CannotInitializeWithReaderOnString()
        {
            JsonReader reader = JsonText.CreateReader("foobar");
            reader.MoveToContent();
            new FreeJsonMemberReadingHelper(reader);
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void CannotInitializeWithReaderOnArray()
        {
            JsonReader reader = JsonText.CreateReader("[]");
            reader.MoveToContent();
            new FreeJsonMemberReadingHelper(reader);
        }

        [ Test ]
        public void InitializeWithReaderOnObjectStart()
        {
            JsonReader reader = JsonText.CreateReader("{}");
            reader.MoveToContent();
            new FreeJsonMemberReadingHelper(reader);
        }

        [ Test ]
        public void InitializeWithReaderOnMember()
        {
            JsonReader reader = JsonText.CreateReader("{foo:bar}");
            reader.MoveToContent();
            reader.ReadToken(JsonTokenClass.Object);
            new FreeJsonMemberReadingHelper(reader);
        }

        private static FreeJsonMemberReadingHelper CreateHelper(string json)
        {
            return new FreeJsonMemberReadingHelper(JsonText.CreateReader(json));
        }
    }
}
