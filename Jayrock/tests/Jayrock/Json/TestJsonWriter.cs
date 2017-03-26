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
    using System.Text;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonWriter
    {
        private bool _disposed;
        
        [ SetUp ]
        public void Init()
        {
            _disposed = false;
        }
        
        [ Test ]
        public void ClosingRaisesDisposed()
        {
            JsonWriter writer = new StubJsonWriter();   
            writer.Disposed += new EventHandler(Writer_Disposed);
            Assert.IsFalse(_disposed);
            writer.Close();
            Assert.IsTrue(_disposed);
        }

        [ Test ]
        public void CloseWithoutDisposedHandlerHarmless()
        {
            JsonWriter writer = new StubJsonWriter();   
            writer.Close();
        }

        [ Test ]
        public void AutoCompletion()
        {
            JsonRecorder writer = new JsonRecorder();
            
            writer.WriteStartArray();
            writer.WriteStartObject();
            writer.WriteMember("outer");
            writer.WriteStartObject();
            writer.WriteMember("inner");
            writer.AutoComplete();
            
            JsonReader reader = writer.CreatePlayer();
            
            reader.ReadToken(JsonTokenClass.Array);
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("outer", reader.ReadMember());
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("inner", reader.ReadMember());
            reader.ReadNull();
            reader.ReadToken(JsonTokenClass.EndObject);
            reader.ReadToken(JsonTokenClass.EndObject);
            reader.ReadToken(JsonTokenClass.EndArray);
            Assert.IsTrue(reader.EOF);
        }

        private void Writer_Disposed(object sender, EventArgs e)
        {
            _disposed = true;
        }

        private sealed class StubJsonWriter : JsonWriter
        {
            public override int Depth
            {
                get { throw new NotImplementedException(); }
            }

            public override int MaxDepth
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public override JsonWriterBracket Bracket
            {
                get { throw new NotImplementedException(); }
            }

            public override int Index
            {
                get { throw new NotImplementedException(); }
            }

            public override void WriteStartObject()
            {
                throw new NotImplementedException();
            }

            public override void WriteEndObject()
            {
                throw new NotImplementedException();
            }

            public override void WriteMember(string name)
            {
                throw new NotImplementedException();
            }

            public override void WriteStartArray()
            {
                throw new NotImplementedException();
            }

            public override void WriteEndArray()
            {
                throw new NotImplementedException();
            }

            public override void WriteString(string value)
            {
                throw new NotImplementedException();
            }

            public override void WriteNumber(string value)
            {
                throw new NotImplementedException();
            }

            public override void WriteBoolean(bool value)
            {
                throw new NotImplementedException();
            }

            public override void WriteNull()
            {
                throw new NotImplementedException();
            }
        }
    }
}