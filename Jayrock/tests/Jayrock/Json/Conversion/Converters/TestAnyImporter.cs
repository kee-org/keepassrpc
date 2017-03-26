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

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System.IO;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestAnyImporter
    {
        [ Test ]
        public void ParseNumber()
        {
            Assert.AreEqual(123, (int) (JsonNumber) Parse("123"));
        }

        [ Test ]
        public void ParseString()
        {
            // TODO: Move some of these test to JsonTextWriter since these have more to do with parsing than importing.
            
            Assert.AreEqual("Hello World", Parse("\"Hello World\""), "Double-quoted string.");
            Assert.AreEqual("Hello World", Parse("'Hello World'"), "Single-quoted string.");
            Assert.AreEqual("Hello 'World'", Parse("\"Hello 'World'\""));
            Assert.AreEqual("Hello \"World\"", Parse("'Hello \"World\"'"));
        }

        [ Test ]
        public void ParseBoolean()
        {
            Assert.AreEqual(true, Parse("true"));
            Assert.AreEqual(false, Parse("false"));
        }

        [ Test ]
        public void ParseNull()
        {
            Assert.IsNull(Parse("null"));
        }

        [ Test ]
        public void ParseEmptyArray()
        {
            JsonArray values = (JsonArray) Parse("[]");
            Assert.IsNotNull(values);
            Assert.AreEqual(0, values.Length);
        }

        [ Test ]
        public void ParseArray()
        {
            JsonArray values = (JsonArray) Parse("[123,'Hello World',true]");
            Assert.IsNotNull(values);
            Assert.AreEqual(3, values.Length);
            Assert.AreEqual(123, (int) (JsonNumber) values[0]);
            Assert.AreEqual("Hello World", values[1]);
            Assert.AreEqual(true, values[2]);
        }

        [ Test ]
        public void ParseEmptyObject()
        {
            JsonObject o = (JsonObject) Parse("{}");
            Assert.IsNotNull(o);
            Assert.AreEqual(0, o.Count);
        }

        [ Test ]
        public void ParseObject()
        {
            JsonObject article = (JsonObject) Parse(@"
                /* Article */ {
                    Title : 'Introduction to JSON',
                    Rating : 2,
                    Abstract : null,
                    Author : {
                        Name : 'John Doe',
                        'E-Mail Address' : 'john.doe@example.com' 
                    },
                    References : [
                        { Title : 'JSON RPC', Link : 'http://www.json-rpc.org/' }
                    ]
                }");

            Assert.IsNotNull(article);
            Assert.AreEqual(5, article.Count);
            Assert.AreEqual("Introduction to JSON", article["Title"]);
            Assert.AreEqual(2, (int) (JsonNumber) article["Rating"]);
            Assert.IsTrue(article.Contains("Abstract"));
            Assert.IsNull(article["Abstract"]);
            
            JsonObject author = (JsonObject) article["Author"];
            Assert.IsNotNull(author);
            Assert.AreEqual(2, author.Count);
            Assert.AreEqual("John Doe", author["Name"]);
            Assert.AreEqual("john.doe@example.com", author["E-Mail Address"]);

            JsonArray references = (JsonArray) article["References"];
            Assert.IsNotNull(references);
            Assert.AreEqual(1, references.Length);

            JsonObject reference = (JsonObject) references[0];
            Assert.IsNotNull(reference);
            Assert.AreEqual(2, reference.Count);
            Assert.AreEqual("JSON RPC", reference["Title"]);
            Assert.AreEqual("http://www.json-rpc.org/", reference["Link"]);
        }

        private object Parse(string s)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(s));
            object value = JsonConvert.Import(reader);
            Assert.IsTrue(reader.EOF);
            return value;
        }
    }
}