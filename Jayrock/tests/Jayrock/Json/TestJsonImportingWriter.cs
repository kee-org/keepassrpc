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
    using System.IO;
    using Conversion;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonImportingWriter
    {
        [ Test ]
        public void Blank()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            Assert.IsNull(writer.Value);
            Assert.IsFalse(writer.IsArray);
            Assert.IsFalse(writer.IsObject);
        }

        [ Test ]
        public void WriteString()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteString("foobar");
            Assert.AreEqual("foobar", GetSingleValue(writer));
        }

        [ Test ]
        public void WriteNumber()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteNumber("1234");
            Assert.AreEqual(new JsonNumber("1234"), GetSingleValue(writer));
        }

        [ Test ]
        public void WriteNull()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteNull();
            Assert.AreEqual(null, GetSingleValue(writer));
        }

        [ Test ]
        public void WriteTrueBoolean()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteBoolean(true);
            Assert.AreEqual(true, GetSingleValue(writer));
        }

        [ Test ]
        public void WriteFalseBoolean()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteBoolean(false);
            Assert.AreEqual(false, GetSingleValue(writer));
        }

        [ Test ]
        public void WriteEmptyArray()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteStringArray(new string[0]);
            Assert.AreEqual(new object[0], GetArray(writer));
        }

        [ Test ]
        public void WriteArray()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteStartArray();
            writer.WriteNumber(123);
            writer.WriteString("Hello World");
            writer.WriteBoolean(true);
            writer.WriteEndArray();
            Assert.AreEqual(new object[] { new JsonNumber("123"), "Hello World", true }, GetArray(writer));
        }

        [ Test ]
        public void WriteEmptyObject()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteStartObject();
            writer.WriteEndObject();
            Assert.AreEqual(0, ((IDictionary) writer.Value).Count);
        }

        [ Test ]
        public void WriteObject()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteStartObject();
            writer.WriteMember("Name");
            writer.WriteString("John Doe");
            writer.WriteMember("Salary");
            writer.WriteNumber(123456789);
            writer.WriteEndObject();
            Assert.IsNotNull(writer.Value);
            IDictionary obj = (IDictionary) writer.Value;
            Assert.AreEqual(2, obj.Count);
            Assert.AreEqual("John Doe", obj["Name"]);
            Assert.AreEqual(123456789, Convert.ToInt32(obj["Salary"]));
        }

        [ Test ]
        public void WriteFromReader()
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(@"
                { 'menu': {
                    'id': 'file',
                    'value': 'File:',
                    'popup': {
                      'menuitem': [
                        {'value': 'New', 'onclick': 'CreateNewDoc()'},
                        {'value': 'Open', 'onclick': 'OpenDoc()'},
                        {'value': 'Close', 'onclick': 'CloseDoc()'}
                      ]
                    }
                  }
                }"));

            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteFromReader(reader);

            IDictionary root = (IDictionary) writer.Value;
            Assert.AreEqual(1, root.Count);
            IDictionary menu = (IDictionary) root["menu"];
            Assert.AreEqual(3, menu.Count);
            Assert.AreEqual("file", menu["id"]);
            Assert.AreEqual("File:", menu["value"]);
            IDictionary popup = (IDictionary) menu["popup"];
            Assert.AreEqual(1, popup.Count);
            IList menuitems = (IList) popup["menuitem"];
            Assert.AreEqual(3, menuitems.Count);
            IDictionary menuitem;
            menuitem = (IDictionary) menuitems[0];
            Assert.AreEqual("New", menuitem["value"]);
            Assert.AreEqual("CreateNewDoc()", menuitem["onclick"]);
            menuitem = (IDictionary) menuitems[1];
            Assert.AreEqual("Open", menuitem["value"]);
            Assert.AreEqual("OpenDoc()", menuitem["onclick"]);
            menuitem = (IDictionary) menuitems[2];
            Assert.AreEqual("Close", menuitem["value"]);
            Assert.AreEqual("CloseDoc()", menuitem["onclick"]);
        }

        private static object GetSingleValue(JsonImportingWriter writer)
        {
            Assert.IsTrue(writer.IsArray);
            Assert.IsFalse(writer.IsObject);
            IEnumerator e = ((IEnumerable)writer.Value).GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            object result = e.Current;
            Assert.IsFalse(e.MoveNext());
            return result;
        }

        private static object[] GetArray(JsonImportingWriter writer)
        {
            Assert.IsTrue(writer.IsArray);
            Assert.IsFalse(writer.IsObject);
            ICollection collection = ((ICollection) writer.Value);
            object[] result = new object[collection.Count];
            collection.CopyTo(result, 0);
            return result;
        }
    }
}