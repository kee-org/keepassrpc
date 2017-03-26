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
    using System.Collections;
    using System.Globalization;
    using Jayrock.Json.Conversion.Converters;
    using Jayrock.Json.Conversion;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonReaderBase
    {
        [ Test ]
        public void EOF()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().End();

            Assert.AreEqual(JsonTokenClass.BOF, reader.TokenClass);
            Assert.IsFalse(reader.Read());
            Assert.IsTrue(reader.EOF);
        }
        
        [ Test ]
        public void ReadAfterEOF()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().End();
            
            Assert.AreEqual(JsonTokenClass.BOF, reader.TokenClass);
            Assert.IsFalse(reader.Read());
            Assert.IsTrue(reader.EOF);
            Assert.IsFalse(reader.Read());
        }
        
        [ Test, ExpectedException(typeof(JsonException)) ]
        public void ReadUnexpectedToken()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().End();

            reader.ReadToken(JsonTokenClass.Object);
        }
        
        [ Test ]
        public void ReadNull()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Null().End();
           
            reader.ReadNull();
            Assert.IsTrue(reader.EOF);
        }

        [ Test ]
        public void ReadString()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().String("hello").End();
           
            Assert.AreEqual("hello", reader.ReadString());
            Assert.IsTrue(reader.EOF);
        }

        [ Test ]
        public void ReadNumber()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Number(123456).End();
           
            Assert.AreEqual("123456", reader.ReadNumber().ToString());
            Assert.IsTrue(reader.EOF);
        }

        [ Test ]
        public void ReadBoolean()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Boolean(true).End();
           
            Assert.IsTrue(reader.ReadBoolean());
            Assert.IsTrue(reader.EOF);

            reader = new MockedJsonReader();
            reader.Begin().Boolean(false).End();

            Assert.IsFalse(reader.ReadBoolean());
            Assert.IsTrue(reader.EOF);
        }

        [ Test ]
        public void ReadTypedNumber()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array().
                Number(123).
                Number(456).
                Number(2.5).
                Number(4.2).
                Number(9.99m).
            EndArray().End();
           
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual(123, (int) reader.ReadNumber());
            Assert.AreEqual(456L, (long) reader.ReadNumber());
            Assert.AreEqual(2.5f, (float) reader.ReadNumber());
            Assert.AreEqual(4.2, (double) reader.ReadNumber());
            Assert.AreEqual(9.99m, (decimal) reader.ReadNumber());
            reader.ReadToken(JsonTokenClass.EndArray);
            Assert.IsFalse(reader.Read());
        }
        
        [ Test ]
        public void ReadMember()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Object().Member("mon", "Monday").EndObject().End();
           
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("mon", reader.ReadMember());
            Assert.AreEqual("Monday", reader.ReadString());
            Assert.AreEqual(JsonTokenClass.EndObject, reader.TokenClass);
            Assert.IsFalse(reader.Read());
        }
        
        [ Test ]
        public void StepOutOfArrayFromStart()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array().String("one").String("two").String("three").EndArray().End();
            
            reader.MoveToContent();
            reader.StepOut();
            Assert.IsTrue(reader.EOF);
        }
        
        [ Test ]
        public void StepOutOfArrayFromMiddle()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array().String("one").String("two").String("three").EndArray().End();
            
            reader.ReadToken(JsonTokenClass.Array);
            reader.ReadString();
            reader.ReadString();
            reader.StepOut();
            Assert.IsTrue(reader.EOF);
        }
        
        [ Test ]
        public void StepOutOfArrayFromEnd()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array().String("one").String("two").String("three").EndArray().End();
            
            reader.ReadToken(JsonTokenClass.Array);
            reader.ReadString();
            reader.ReadString();
            reader.ReadString();
            Assert.AreEqual(JsonTokenClass.EndArray, reader.TokenClass);
            reader.StepOut();
            Assert.IsTrue(reader.EOF);
        }

        [ Test ]
        public void StepOutFromWithinNestedArray()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array()
                .String("one")
                .String("two").Array()
                    .String("three")
                    .String("four")
                    .EndArray()
                .String("five")
                .EndArray()
            .End();
            
            reader.ReadToken(JsonTokenClass.Array);
            reader.ReadString();
            reader.ReadString();
            reader.StepOut();
            Assert.AreEqual("five", reader.ReadString());
        }

        [ Test ]
        public void StepOutThroughNestedArray()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array()
                .String("one")
                .String("two").Array()
                    .String("three")
                    .String("four")
                    .EndArray()
                .String("five")
                .EndArray()
            .End();
            
            reader.MoveToContent();
            reader.StepOut();
            Assert.IsTrue(reader.EOF);
        }

        [ Test ]
        public void SkipScalar()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().String("test").End();
            reader.Skip();
            Assert.IsTrue(reader.EOF);
        }

        [ Test ]
        public void SkipObject()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Object().Member("foo", "bar").EndObject().End();
            reader.Skip();
            Assert.IsTrue(reader.EOF);
        }

        [ Test ]
        public void SkipArray()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array().String("test").EndArray().End();
            reader.Skip();
            Assert.IsTrue(reader.EOF);
        }

        [ Test ]
        public void SkipArrayElement()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array().String("foo").String("bar").EndArray().End();
            reader.Read();
            reader.Read();
            reader.Skip();
            Assert.AreEqual("bar", reader.ReadString());
        }

        [ Test ]
        public void SkipObjectMember()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Object()
                .Member("m1", "v1")
                .Member("m2").Object()
                    .Member("m3", "v3")
                    .Member("m4", "v4")
                    .EndObject()
                .Member("m5", "v5")
                .EndObject()
            .End();
            reader.Read();
            reader.Read();
            reader.Skip();
            Assert.AreEqual(JsonTokenClass.Member, reader.TokenClass);
            Assert.AreEqual("m2", reader.Text);
            reader.Skip();
            Assert.AreEqual("m5", reader.ReadMember());
        }

        [ Test ]
        public void SkipAdjacentObjects()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array().Object().EndObject().Object().EndObject().EndArray().End();
            reader.ReadToken(JsonTokenClass.Array);
            reader.Skip();
            Assert.AreEqual(JsonTokenClass.Object, reader.TokenClass);
            reader.Skip();
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [Test]
        public void DefaultMaxDepth()
        {
            MockedJsonReader reader = new MockedJsonReader();
            Assert.AreEqual(100, reader.MaxDepth);
        }

        [Test]
        public void SetMaxDepth()
        {
            MockedJsonReader reader = new MockedJsonReader();
            Assert.AreNotEqual(42, reader.MaxDepth);
            reader.MaxDepth = 42;
            Assert.AreEqual(42, reader.MaxDepth);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void CannotExceedMaxDepth()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.MaxDepth = 2;
            reader.Array().Array().Array();
            reader.Read();
            reader.Read();
            reader.Read();
            
            // Exception will be thrown here, when attempting to read 
            // first value *after* the depth has been exceeded.

            reader.Read();
        }

        private sealed class MockedJsonReader : JsonReaderBase
        {
            private Queue _queue = new Queue();

            protected override JsonToken ReadTokenImpl()
            {
                return (JsonToken) _queue.Dequeue();
            }

            private MockedJsonReader Append(JsonToken token)
            {
                _queue.Enqueue(token);
                return this;
            }

            public MockedJsonReader Begin()
            {
                _queue.Clear();
                return this;
            }

            public MockedJsonReader End()
            {
                return Append(JsonToken.EOF());
            }

            public MockedJsonReader Array()
            {
                return Append(JsonToken.Array());
            }

            public MockedJsonReader EndArray()
            {
                return Append(JsonToken.EndArray());
            }

            public MockedJsonReader String(string s)
            {
                return Append(JsonToken.String(s));
            }

            public MockedJsonReader Number(int i)
            {
                return Append(JsonToken.Number(i.ToString(CultureInfo.InvariantCulture)));
            }

            public MockedJsonReader Number(double i)
            {
                return Append(JsonToken.Number(i.ToString(CultureInfo.InvariantCulture)));
            }

            public MockedJsonReader Number(decimal i)
            {
                return Append(JsonToken.Number(i.ToString(CultureInfo.InvariantCulture)));
            }
            
            public MockedJsonReader Boolean(bool b)
            {
                return Append(JsonToken.Boolean(b));
            }

            public MockedJsonReader Object()
            {
                return Append(JsonToken.Object());
            }

            public MockedJsonReader EndObject()
            {
                return Append(JsonToken.EndObject());
            }

            public MockedJsonReader Member(string name, string value)
            {
                return Member(name).Append(JsonToken.String(value));
            }

            public MockedJsonReader Member(string name)
            {
                return Append(JsonToken.Member(name));
            }
            
            public MockedJsonReader Null()
            {
                return Append(JsonToken.Null());
            }
        }
    }
}