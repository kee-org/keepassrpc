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

namespace Jayrock.JsonML
{
    #region Imports

    using System;
    using System.IO;
    using System.Xml;
    using Json;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonMLCodec
    {
        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void EncodeDoesNotAcceptNullXmlReader()
        {
            JsonMLCodec.Encode(null, new JsonRecorder());
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void EncodeDoesNotAcceptNullJsonWriter()
        {
            JsonMLCodec.Encode(new XmlTextReader("http://www.example.com/"), null);
        }

        [ Test ]
        public void EncodeArrayOnSingleEmptyElement()
        {
            JsonReader reader = EncodeArray("<root />");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("root", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [ Test ]
        public void EncodeArrayOnSingleEmptyElementWithAttributes()
        {
            JsonReader reader = EncodeArray("<root a1='v1' a2='v2' />");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("root", reader.ReadString());
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("a1", reader.ReadMember());
            Assert.AreEqual("v1", reader.ReadString());
            Assert.AreEqual("a2", reader.ReadMember());
            Assert.AreEqual("v2", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndObject);
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [ Test ]
        public void EncodeArrayOnSingleElementWithAttributesAndContent()
        {
            JsonReader reader = EncodeArray("<root a1='v1' a2='v2'>content</root>");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("root", reader.ReadString());
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("a1", reader.ReadMember());
            Assert.AreEqual("v1", reader.ReadString());
            Assert.AreEqual("a2", reader.ReadMember());
            Assert.AreEqual("v2", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndObject);
            Assert.AreEqual("content", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [ Test ]
        public void EncodeArrayOnNestedEmptyElements()
        {
            JsonReader reader = EncodeArray(@"
                <root>
                    <child1 />
                    <child2 />
                    <child3 />
                </root>");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("root", reader.ReadString());
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("child1", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("child2", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("child3", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [ Test ]
        public void EncodeArrayOnInterspersedElementsAndText()
        {
            JsonReader reader = EncodeArray(@"<root>text1<e1 />text2<e2 />text3</root>");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("root", reader.ReadString());
            Assert.AreEqual("text1", reader.ReadString());
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("e1", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);
            Assert.AreEqual("text2", reader.ReadString());
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("e2", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);
            Assert.AreEqual("text3", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [ Test ]
        public void EncodeArrayCDataHandling()
        {
            JsonReader reader = EncodeArray("<e><![CDATA[<content>]]></e>");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("e", reader.ReadString());
            Assert.AreEqual("<content>", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [ Test ]
        public void EncodeArrayOnComplexXml()
        {
            JsonReader reader = EncodeArray(
                "<a ichi='1' ni='2'><b>The content of b</b> and " +
                "<c san='3'>The content of c</c><d>do</d><e></e>" + 
                "<d>re</d><f/><d>mi</d></a>");

            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("a", reader.ReadString());
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("ichi", reader.ReadMember());
            Assert.AreEqual("1", reader.ReadString());
            Assert.AreEqual("ni", reader.ReadMember());
            Assert.AreEqual("2", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndObject);
            
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("b", reader.ReadString());
            Assert.AreEqual("The content of b", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);
            
            Assert.AreEqual(" and ", reader.ReadString());
            
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("c", reader.ReadString());
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("san", reader.ReadMember());
            Assert.AreEqual("3", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndObject);
            Assert.AreEqual("The content of c", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);
            
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("d", reader.ReadString());
            Assert.AreEqual("do", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);
            
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("e", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);

            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("d", reader.ReadString());
            Assert.AreEqual("re", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);

            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("f", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);

            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("d", reader.ReadString());
            Assert.AreEqual("mi", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);

            reader.ReadToken(JsonTokenClass.EndArray); // a
        }

        [ Test ]
        public void EncodeArraySkipsComments()
        {
            JsonReader reader = EncodeArray("<e><!--comment--></e>");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("e", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void EncodeArrayExpectsXmlReaderOnElement()
        {
            XmlTextReader reader = new XmlTextReader(new StringReader("<e>text</e>"));
            reader.Read();
            reader.Read();
            JsonMLCodec.EncodeArrayForm(reader, new JsonRecorder());
        }

        private static JsonReader EncodeArray(string xml)
        {
            JsonRecorder writer = new JsonRecorder();
            JsonMLCodec.EncodeArrayForm(new XmlTextReader(new StringReader(xml)), writer);
            return writer.CreatePlayer();
        }

        [ Test ]
        public void EncodeObjectOnSingleEmptyElement()
        {
            JsonReader reader = EncodeObject("<root />");
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("tagName", reader.ReadMember());
            Assert.AreEqual("root", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndObject);
        }

        [ Test ]
        public void EncodeObjectOnSingleEmptyElementWithAttributes()
        {
            JsonReader reader = EncodeObject("<root a1='v1' a2='v2' />");
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("tagName", reader.ReadMember());
            Assert.AreEqual("root", reader.ReadString());
            Assert.AreEqual("a1", reader.ReadMember());
            Assert.AreEqual("v1", reader.ReadString());
            Assert.AreEqual("a2", reader.ReadMember());
            Assert.AreEqual("v2", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndObject);
        }

        [ Test ]
        public void EncodeObjectOnSingleElementWithAttributesAndContent()
        {
            JsonReader reader = EncodeObject("<root a1='v1' a2='v2'>content</root>");
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("tagName", reader.ReadMember());
            Assert.AreEqual("root", reader.ReadString());
            Assert.AreEqual("a1", reader.ReadMember());
            Assert.AreEqual("v1", reader.ReadString());
            Assert.AreEqual("a2", reader.ReadMember());
            Assert.AreEqual("v2", reader.ReadString());
            Assert.AreEqual("childNodes", reader.ReadMember());
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("content", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.EndObject);
        }

        [ Test ]
        public void EncodeObjectOnNestedEmptyElements()
        {
            JsonReader reader = EncodeObject(@"
                <root>
                    <child1 />
                    <child2 />
                    <child3 />
                </root>");
            
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("tagName", reader.ReadMember());
            Assert.AreEqual("root", reader.ReadString());
            Assert.AreEqual("childNodes", reader.ReadMember());
            reader.ReadToken(JsonTokenClass.Array);

                reader.ReadToken(JsonTokenClass.Object);
                Assert.AreEqual("tagName", reader.ReadMember());
                Assert.AreEqual("child1", reader.ReadString());
                reader.ReadToken(JsonTokenClass.EndObject);
                
                reader.ReadToken(JsonTokenClass.Object);
                Assert.AreEqual("tagName", reader.ReadMember());
                Assert.AreEqual("child2", reader.ReadString());
                reader.ReadToken(JsonTokenClass.EndObject);
                
                reader.ReadToken(JsonTokenClass.Object);
                Assert.AreEqual("tagName", reader.ReadMember());
                Assert.AreEqual("child3", reader.ReadString());
                reader.ReadToken(JsonTokenClass.EndObject);
            
            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.EndObject);
        }

        [ Test ]
        public void EncodeObjectOnInterspersedElementsAndText()
        {
            JsonReader reader = EncodeObject(@"<root>text1<e1 />text2<e2 />text3</root>");

            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("tagName", reader.ReadMember());
            Assert.AreEqual("root", reader.ReadString());
            Assert.AreEqual("childNodes", reader.ReadMember());
            reader.ReadToken(JsonTokenClass.Array);

                Assert.AreEqual("text1", reader.ReadString());
            
                reader.ReadToken(JsonTokenClass.Object);
                Assert.AreEqual("tagName", reader.ReadMember());
                Assert.AreEqual("e1", reader.ReadString());
                reader.ReadToken(JsonTokenClass.EndObject);

                Assert.AreEqual("text2", reader.ReadString());

                reader.ReadToken(JsonTokenClass.Object);
                Assert.AreEqual("tagName", reader.ReadMember());
                Assert.AreEqual("e2", reader.ReadString());
                reader.ReadToken(JsonTokenClass.EndObject);

                Assert.AreEqual("text3", reader.ReadString());
            
            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.EndObject);
        }

        [ Test ]
        public void EncodeObjectCDataHandling()
        {
            JsonReader reader = EncodeObject("<e><![CDATA[<content>]]></e>");
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("tagName", reader.ReadMember());
            Assert.AreEqual("e", reader.ReadString());
            Assert.AreEqual("childNodes", reader.ReadMember());
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual("<content>", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.EndObject);
        }

        [ Test ]
        public void EncodeObjectOnComplexXml()
        {
            JsonReader reader = EncodeObject(
                "<a ichi='1' ni='2'><b>The content of b</b> and " +
                "<c san='3'>The content of c</c><d>do</d><e></e>" + 
                "<d>re</d><f/><d>mi</d></a>");

            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("tagName", reader.ReadMember());
            Assert.AreEqual("a", reader.ReadString());
            Assert.AreEqual("ichi", reader.ReadMember());
            Assert.AreEqual("1", reader.ReadString());
            Assert.AreEqual("ni", reader.ReadMember());
            Assert.AreEqual("2", reader.ReadString());
            Assert.AreEqual("childNodes", reader.ReadMember());
            reader.ReadToken(JsonTokenClass.Array);

                reader.ReadToken(JsonTokenClass.Object);
                Assert.AreEqual("tagName", reader.ReadMember());
                Assert.AreEqual("b", reader.ReadString());
                Assert.AreEqual("childNodes", reader.ReadMember());
                reader.ReadToken(JsonTokenClass.Array);
                Assert.AreEqual("The content of b", reader.ReadString());
                reader.ReadToken(JsonTokenClass.EndArray);
            
                Assert.AreEqual(" and ", reader.ReadString());

                reader.ReadToken(JsonTokenClass.Object);
                Assert.AreEqual("tagName", reader.ReadMember());
                Assert.AreEqual("c", reader.ReadString());
                Assert.AreEqual("san", reader.ReadMember());
                Assert.AreEqual("3", reader.ReadString());
                Assert.AreEqual("childNodes", reader.ReadMember());
                reader.ReadToken(JsonTokenClass.Array);
                Assert.AreEqual("The content of c", reader.ReadString());
                reader.ReadToken(JsonTokenClass.EndArray);

                reader.ReadToken(JsonTokenClass.Object);
                Assert.AreEqual("tagName", reader.ReadMember());
                Assert.AreEqual("d", reader.ReadString());
                Assert.AreEqual("childNodes", reader.ReadMember());
                reader.ReadToken(JsonTokenClass.Array);
                Assert.AreEqual("do", reader.ReadString());
                reader.ReadToken(JsonTokenClass.EndArray);

                reader.ReadToken(JsonTokenClass.Object);
                Assert.AreEqual("tagName", reader.ReadMember());
                Assert.AreEqual("e", reader.ReadString());
                reader.ReadToken(JsonTokenClass.EndObject);

                reader.ReadToken(JsonTokenClass.Object);
                Assert.AreEqual("tagName", reader.ReadMember());
                Assert.AreEqual("d", reader.ReadString());
                Assert.AreEqual("childNodes", reader.ReadMember());
                reader.ReadToken(JsonTokenClass.Array);
                Assert.AreEqual("re", reader.ReadString());
                reader.ReadToken(JsonTokenClass.EndArray);

                reader.ReadToken(JsonTokenClass.Object);
                Assert.AreEqual("tagName", reader.ReadMember());
                Assert.AreEqual("f", reader.ReadString());
                reader.ReadToken(JsonTokenClass.EndObject);

                reader.ReadToken(JsonTokenClass.Object);
                Assert.AreEqual("tagName", reader.ReadMember());
                Assert.AreEqual("d", reader.ReadString());
                Assert.AreEqual("childNodes", reader.ReadMember());
                reader.ReadToken(JsonTokenClass.Array);
                Assert.AreEqual("mi", reader.ReadString());
                reader.ReadToken(JsonTokenClass.EndArray);
                reader.ReadToken(JsonTokenClass.EndObject);
                
            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.EndObject); // a
        }

        [ Test ]
        public void EncodeObjectSkipsComments()
        {
            JsonReader reader = EncodeObject("<e><!--comment--></e>");
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("tagName", reader.ReadMember());
            Assert.AreEqual("e", reader.ReadString());
            reader.ReadToken(JsonTokenClass.EndObject);
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void EncodeObjectExpectsXmlReaderOnElement()
        {
            XmlTextReader reader = new XmlTextReader(new StringReader("<e>text</e>"));
            reader.Read();
            reader.Read();
            JsonMLCodec.EncodeObjectForm(reader, new JsonRecorder());
        }

        private static JsonReader EncodeObject(string xml)
        {
            JsonRecorder writer = new JsonRecorder();
            JsonMLCodec.EncodeObjectForm(new XmlTextReader(new StringReader(xml)), writer);
            return writer.CreatePlayer();
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DecodeDoesNotAcceptNullJsonReader()
        {
            JsonMLCodec.Decode(null, new XmlTextWriter(new StringWriter()));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DecodeDoesNotAcceptNullXmlWriter()
        {
            JsonMLCodec.Decode(new JsonRecorder().CreatePlayer(), null);
        }

        [Test]
        public void DecodeArrayOnSingleElement()
        {
            Assert.AreEqual("<root />", DecodeArray("[root]"));
        }

        [Test, ExpectedException(typeof(JsonException))]
        public void DecodeArrayExpectsJsonArray()
        {
            DecodeArray("{}");
        }

        [Test]
        public void DecodeArrayOnSingleElementWithAttributes()
        {
            Assert.AreEqual("<root a='1' b='2' />", DecodeArray("[root,{a:1,b:2}]"));
        }

        [Test, ExpectedException(typeof(JsonMLException))]
        public void DecodeArrayThrowsWhenAttributeIsObject()
        {
            DecodeArray("[root,{bad:{}}]");
        }

        [Test, ExpectedException(typeof(JsonMLException))]
        public void DecodeArrayThrowsWhenAttributeIsArray()
        {
            DecodeArray("[root,{bad:[]}]");
        }

        [Test]
        public void DecodeArrayOnAttributeWithNullYieldsEmptyXmlAttributeValue()
        {
            Assert.AreEqual("<root nullattr='' />", DecodeArray("[root,{nullattr:null}]"));
        }

        [Test]
        public void DecodeArrayOnSingleElementWithAttributesAndContent()
        {
            Assert.AreEqual("<root a='1' b='2'>content</root>", DecodeArray("[root,{a:1,b:2},content]"));
        }

        [Test]
        public void DecodeArrayOnNestedEmptyElements()
        {
            Assert.AreEqual("<root><child1 /><child2 /><child3 /></root>", DecodeArray("[root,[child1],[child2],[child3]]"));
        }

        [Test]
        public void DecodeArrayOnInterspersedElementsAndText()
        {
            Assert.AreEqual("<root>text1<e1 />text2<e2 />text3</root>", DecodeArray("[root,text1,[e1],text2,[e2],text3]"));
        }

        [Test]
        public void DecodeArrayOnComplexJson()
        {
            Assert.AreEqual(
                "<a ichi='1' ni='2'><b>The content of b</b> and " +
                "<c san='3'>The content of c</c><d>do</d><e />" +
                "<d>re</d><f /><d>mi</d></a>",
                DecodeArray(@"
                    [a, {ichi: 1, ni: 2},
                        [b, The content of b],
                        ' and ',
                        [c, {san: 3}, The content of c],
                        [d, do],
                        [e],
                        [d, re],
                        [f],
                        [d, mi]
                    ]"));
        }

        [Test]
        public void DecodeArrayOnElementWithEmptyAttributesObject()
        {
            Assert.AreEqual("<e>text</e>", DecodeArray("[e,{},text]"));
        }

        [Test]
        public void DecodeArrayOnElementPermitsChildNodes()
        {
            Assert.AreEqual("<e>abcdef</e>", DecodeArray("[e,abc,[],def]"));
            Assert.AreEqual("<e />", DecodeArray("[e,[]]"));
            Assert.AreEqual("<e a='1' />", DecodeArray("[e,{a:1},[]]"));
        }

        [Test, ExpectedException(typeof(JsonMLException))]
        public void DecodeArrayDoesNotLikeEmptyRootElement()
        {
            DecodeArray("[]");
        }

        [Test]
        public void DecodeArrayAllowsEmptyRootElementAsLongAsXmlWriterIsStarted()
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(sw);
            writer.WriteStartElement("root");
            JsonMLCodec.Decode(new JsonTextReader(new StringReader("[]")), writer);
        }

        [Test, ExpectedException(typeof(JsonMLException))]
        public void DecodeArrayDoesNotLikeObjectsAfterAttributes()
        {
            DecodeArray("[e,{a:1},text,{/*bad*/}]");
        }

        [Test]
        public void DecodeArrayIgnoresNullElements()
        {
            Assert.AreEqual("<e>text</e>", DecodeArray("[e,null,text,null]"));
        }

        private static string DecodeArray(string json)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(sw);
            writer.QuoteChar = '\'';
            JsonMLCodec.DecodeArrayForm(new JsonTextReader(new StringReader(json)), writer);
            return sw.ToString();
        }

        [Test]
        public void DecodeObjectOnSingleElement()
        {
            Assert.AreEqual("<root />", DecodeObject("{tagName:root}"));
        }

        [Test, ExpectedException(typeof(JsonException))]
        public void DecodeObjectExpectsJsonArray()
        {
            DecodeObject("[]");
        }

        [Test]
        public void DecodeObjectOnSingleElementWithAttributes()
        {
            Assert.AreEqual("<root a='1' b='2' />", DecodeObject("{tagName:root,a:1,b:2}"));
        }

        [Test]
        public void DecodeObjectAcceptsTagNameAsNonFirstMember()
        {
            Assert.AreEqual("<root a='1' b='2' />", DecodeObject("{a:1,b:2,tagName:root}"));
        }

        [Test, ExpectedException(typeof(JsonMLException))]
        public void DecodeObjectThrowsWhenAttributeIsObject()
        {
            DecodeObject("{tagName:root,bad:{}}");
        }

        [Test, ExpectedException(typeof(JsonMLException))]
        public void DecodeObjectThrowsWhenAttributeIsArray()
        {
            DecodeObject("{tagName:root,bad:[]}");
        }

        [Test]
        public void DecodeObjectOnAttributeWithNullSkipsXmlAttribute()
        {
            Assert.AreEqual("<root />", DecodeObject("{tagName:root,nullattr:null}"));
        }

        [Test]
        public void DecodeObjectToleratesDuplicateTagNames()
        {
            Assert.AreEqual("<root />", DecodeObject("{tagName:root,tagName:root}"));
        }

        [Test, ExpectedException(typeof(JsonMLException))]
        public void DecodeObjectThrowsWhenTagNameIsRedefined()
        {
            DecodeObject("{tagName:root,tagName:ROOT}");
        }

        [Test]
        public void DecodeObjectOnSingleElementWithAttributesAndContent()
        {
            Assert.AreEqual("<root a='1' b='2'>content</root>", 
                DecodeObject("{tagName:root,a:1,b:2,childNodes:[content]}"));
        }

        [Test, ExpectedException(typeof(JsonMLException))]
        public void DecodeObjectThrowsWhenChildNodesIsObject()
        {
            DecodeObject("{tagName:root,childNodes:{}}");
        }

        [Test, ExpectedException(typeof(JsonMLException))]
        public void DecodeObjectThrowsWhenChildNodesIsString()
        {
            DecodeObject("{tagName:root,childNodes:foo}");
        }

        [Test, ExpectedException(typeof(JsonMLException))]
        public void DecodeObjectThrowsWhenChildNodesIsBoolean()
        {
            DecodeObject("{tagName:root,childNodes:true}");
        }

        [Test, ExpectedException(typeof(JsonMLException))]
        public void DecodeObjectThrowsWhenChildNodesIsNumber()
        {
            DecodeObject("{tagName:root,childNodes:123}");
        }

        [Test]
        public void DecodeObjectToleratesNullChildNodes()
        {
            Assert.AreEqual("<root />", DecodeObject("{tagName:root,childNodes:null}"));
        }

        [Test]
        public void DecodeObjectOnNestedEmptyElements()
        {
            Assert.AreEqual("<root><child1 /><child2 /><child3 /></root>",
                DecodeObject(@"{
                    tagName: root,
                    childNodes: [
                        {tagName:child1},
                        {tagName:child2},
                        {tagName:child3}
                    ]
                }"));
        }

        [Test]
        public void DecodeObjectOnInterspersedElementsAndText()
        {
            Assert.AreEqual("<root>text1<e1 />text2<e2 />text3</root>", 
                DecodeObject(@"{
                    tagName: root,
                    childNodes: [
                        text1,
                        {tagName:e1},
                        text2,
                        {tagName:e2},
                        text3
                    ]
                }"));
        }

        [Test]
        public void DecodeObjectSkipsNullsInsideChildNodes()
        {
            Assert.AreEqual("<root>text1<e1 /><e2 /></root>", 
                DecodeObject(@"{
                    tagName: root,
                    childNodes: [
                        text1,
                        {tagName:e1},
                        null,
                        {tagName:e2},
                        null
                    ]
                }"));
        }

        [Test]
        public void DecodeObjectHandlesNonStringChildNodes()
        {
            Assert.AreEqual("<root>stringtruefalse123.456</root>",
                DecodeObject(@"{
                    tagName:root,
                    childNodes:[
                        string,
                        true,
                        false,
                        123.456
                    ]
                }"));
        }

        [Test]
        public void DecodeObjectAccumulatesDuplicateChildNodes()
        {
            Assert.AreEqual("<root a1='1' a2='2'>123.456foo<child1 />truefalsefalsetrue<child2 />bar456.789</root>",
            DecodeObject(@"{
                tagName:root,
                childNodes:[123.456,foo,null,{tagName:child1},true,false],
                a1: 1,
                childNodes:[false,true,{tagName:child2},null,bar,456.789],
                a2: 2
            }"));
        }

        [Test]
        public void DecodeObjectOnComplexJson()
        {
            Assert.AreEqual(
                "<a ichi='1' ni='2'><b>The content of b</b> and " +
                "<c san='3'>The content of c</c><d>do</d><e />" +
                "<d>re</d><f /><d>mi</d></a>",
                DecodeObject(@"{
                    tagName: a,
                    ichi: 1,
                    ni: 2,
                    childNodes: [ {
                            tagName: b,
                            childNodes: [ 'The content of b' ] 
                        },
                        ' and ', {
                            tagName: c,
                            san: 3,
                            childNodes: [ 'The content of c' ] 
                        },
                        { tagName: d, childNodes: [ do ] },
                        { tagName: e },
                        { tagName: d, childNodes: [ re ] },
                        { tagName: f },
                        { tagName: d, childNodes: [ mi ] }
                    ]
                }"));
        }

        [Test, ExpectedException(typeof(JsonMLException))]
        public void DecodeObjectThrowsWhenMissingTagName()
        {
            DecodeObject("{}");
        }

        [Test, ExpectedException(typeof(JsonMLException))]
        public void DecodeObjectThrowsWhenTagNameIsEmpty()
        {
            DecodeObject("{tagName:''}");
        }

        [Test]
        public void DecodeObjectAllowsEmptyRootElementAsLongAsXmlWriterIsStarted()
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(sw);
            writer.WriteStartElement("root");
            JsonMLCodec.Decode(new JsonTextReader(new StringReader("{}")), writer);
        }

        private static string DecodeObject(string json)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(sw);
            writer.QuoteChar = '\'';
            JsonMLCodec.DecodeObjectForm(new JsonTextReader(new StringReader(json)), writer);
            return sw.ToString();
        }
    }
}