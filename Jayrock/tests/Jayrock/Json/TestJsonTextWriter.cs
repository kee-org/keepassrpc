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
    using System.IO;
    using NUnit.Framework;
    using Jayrock.Json.Conversion;

    #endregion

    [ TestFixture ]
    public class TestJsonTextWriter
    {
        [ Test ]
        public void Blank()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            Assert.AreEqual(string.Empty, writer.ToString());
        }

        [ Test ]
        public void WriteString()
        {
            WriteString("[\"Hello\"]", "Hello");
            WriteString("[\"Hello World\"]", "Hello World");
            WriteString("[\"And before he parted, he said, \\\"Goodbye, people!\\\"\"]", "And before he parted, he said, \"Goodbye, people!\"");
            WriteString("[\"Hello\\tWorld\"]", "Hello\tWorld");
            WriteString("[\"Hello\\u0000World\"]", "Hello" + ((char) 0) + "World");
        }

        private static void WriteString(string expected, string value)
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteString(value);
            Assert.AreEqual(expected, writer.ToString());
        }

        [ Test ]
        public void WriteNumber()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteNumber(123);
            Assert.AreEqual("[123]", writer.ToString());
        }

        [ Test ]
        public void WriteNull()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteNull();
            Assert.AreEqual("[null]", writer.ToString());
        }

        [ Test ]
        public void WriteTrueBoolean()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteBoolean(true);
            Assert.AreEqual("[true]", writer.ToString());
        }

        [ Test ]
        public void WriteFalseBoolean()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteBoolean(false);
            Assert.AreEqual("[false]", writer.ToString());
        }

        [ Test ]
        public void WriteEmptyArray()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteStringArray(new string[0]);
            Assert.AreEqual("[]", writer.ToString());
        }

        [ Test ]
        public void WriteArray()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteStringArray(new object[] { 123, "Hello \"Old\" World", true });
            Assert.AreEqual("[\"123\",\"Hello \\\"Old\\\" World\",\"True\"]", writer.ToString());
        }

        [ Test ]
        public void WriteEmptyObject()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteStartObject();
            writer.WriteEndObject();
            Assert.AreEqual("{}", writer.ToString());
        }

        [ Test ]
        public void WriteObject()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteStartObject();
            writer.WriteMember("Name");
            writer.WriteString("John Doe");
            writer.WriteMember("Salary");
            writer.WriteNumber(123456789);
            writer.WriteEndObject();
            Assert.AreEqual("{\"Name\":\"John Doe\",\"Salary\":123456789}", writer.ToString());
        }

        [ Test ]
        public void WriteNullValue()
        {
            Assert.AreEqual("[null]", JsonConvert.ExportToString(JsonNull.Value));
        }

        [ Test ]
        public void WriteValue()
        {
            Assert.AreEqual("[123]", WriteValue((byte) 123), "Byte");
            Assert.AreEqual("[\"123\"]", WriteValue((sbyte) 123), "Short byte");
            Assert.AreEqual("[123]", WriteValue((short) 123), "Short integer");
            Assert.AreEqual("[123]", WriteValue(123), "Integer");
            Assert.AreEqual("[123]", WriteValue(123L), "Long integer");
            Assert.AreEqual("[123]", WriteValue(123m), "Decimal");
        }

        [ Test ]
        public void WriteObjectArray()
        {
            JsonObject o = new JsonObject();
            o.Put("one", 1);
            o.Put("two", 2);
            o.Put("three", 3);
            Assert.AreEqual("[{\"one\":1,\"two\":2,\"three\":3},{\"one\":1,\"two\":2,\"three\":3},{\"one\":1,\"two\":2,\"three\":3}]", WriteValue(new object[] { o, o, o }));
        }

        [ Test ]
        public void WriteNestedArrays()
        {
            int[] inner = new int[] { 1, 2, 3 };
            int[][] outer = new int[][] { inner, inner, inner };
            Assert.AreEqual("[[1,2,3],[1,2,3],[1,2,3]]", WriteValue(outer));
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

            JsonTextWriter writer = new JsonTextWriter();
            writer.WriteFromReader(reader);
            Assert.AreEqual("{\"menu\":{\"id\":\"file\",\"value\":\"File:\",\"popup\":{\"menuitem\":[{\"value\":\"New\",\"onclick\":\"CreateNewDoc()\"},{\"value\":\"Open\",\"onclick\":\"OpenDoc()\"},{\"value\":\"Close\",\"onclick\":\"CloseDoc()\"}]}}}", writer.ToString());
        }

        [ Test ]
        public void PrettyPrinting()
        {
            JsonTextWriter writer = new JsonTextWriter();
            writer.PrettyPrint = true;
            writer.WriteFromReader(new JsonTextReader(new StringReader("{'menu':{'id':'file','value':'File:','popup':{'menuitem':[{'value':'New','onclick':'CreateNewDoc()'},{'value':'Open','onclick':'OpenDoc()'},{'value':'Close','onclick':'CloseDoc()'}]}}}")));
            Assert.AreEqual(RewriteLines(@"{ 
    ""menu"": { 
        ""id"": ""file"",
        ""value"": ""File:"",
        ""popup"": { 
            ""menuitem"": [ { 
                ""value"": ""New"",
                ""onclick"": ""CreateNewDoc()""
            }, { 
                ""value"": ""Open"",
                ""onclick"": ""OpenDoc()""
            }, { 
                ""value"": ""Close"",
                ""onclick"": ""CloseDoc()""
            } ]
        }
    }
}"), writer.ToString() + Environment.NewLine);
        }
        
        private static string WriteValue(object value)
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            JsonConvert.Export(value, writer);
            return writer.ToString();
        }

        private static string RewriteLines(string s)
        {
            StringReader reader = new StringReader(s);
            StringWriter writer = new StringWriter();

            string line = reader.ReadLine();
            while (line != null)
            {
                writer.WriteLine(line);
                line = reader.ReadLine();
            }
            
            return writer.ToString();
        }
    }
}
