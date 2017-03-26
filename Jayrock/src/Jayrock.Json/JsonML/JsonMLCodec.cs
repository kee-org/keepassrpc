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
    using System.Collections;
    using System.Diagnostics;
    using System.Xml;
    using Json;

    #endregion

    #region JsonML BNF grammar
    //
    // The following BNF grammar represents how XML-based markup (e.g. XHTML) 
    // is encoded into JsonML:
    //
    //  element 
    //      = '[' tag-name ',' attributes ',' element-list ']' 
    //      | '[' tag-name ',' attributes ']' 
    //      | '[' tag-name ',' element-list ']' 
    //      | '[' tag-name ']' 
    //      | json-string 
    //      ; 
    //  tag-name 
    //      = json-string 
    //      ; 
    //  attributes 
    //      = '{' attribute-list '}' 
    //      | '{' '}' 
    //      ; 
    //  attribute-list 
    //      = attribute ',' attribute-list 
    //      | attribute 
    //      ; 
    //  attribute 
    //      = attribute-name ':' attribute-value 
    //      ; 
    //  attribute-name 
    //      = json-string 
    //      ; 
    //  attribute-value 
    //      = json-string 
    //      ; 
    //  element-list 
    //      = element ',' element-list 
    //      | element 
    //      ; 
    //
    #endregion

    /// <summary>
    /// Encodes or decodes JsonML.
    /// </summary>

    public sealed class JsonMLCodec
    {
        /// <summary>
        /// Converts JsonML in object or array form to XML.
        /// </summary>
        /// <remarks>
        /// The JsonML form is automatically detected. If the reader is 
        /// positioned on an object then decoding occurs using the object
        /// form. Otherwise the array form is assumed.
        /// </remarks>

        public static void Decode(JsonReader reader, XmlWriter writer)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            if (writer == null) throw new ArgumentNullException("writer");

            if (!reader.MoveToContent())
                throw new JsonMLException("Unexpected EOF.");

            if (reader.TokenClass == JsonTokenClass.Object)
                DecodeObjectForm(reader, writer);
            else
                DecodeArrayForm(reader, writer);
        }

        /// <summary>
        /// Converts JsonML to XML. A parameter specifies the input JsonML form.
        /// </summary>

        public static void Decode(JsonReader reader, XmlWriter writer, JsonMLForm form)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            if (writer == null) throw new ArgumentNullException("writer");

            switch (form)
            {
                case JsonMLForm.ObjectForm: DecodeObjectForm(reader, writer); break;
                case JsonMLForm.ArrayForm: DecodeArrayForm(reader, writer); break;
                default: throw new ArgumentException(null, "form");
            }
        }

        /// <summary>
        /// Converts JsonML in array form to XML.
        /// </summary>
        
        public static void DecodeArrayForm(JsonReader reader, XmlWriter writer)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            if (writer == null) throw new ArgumentNullException("writer");

            reader.ReadToken(JsonTokenClass.Array);

            if (reader.TokenClass == JsonTokenClass.EndArray)
            {
                if (writer.WriteState != WriteState.Element && writer.WriteState != WriteState.Content)
                    throw new JsonMLException("Missing root element.");
            }
            else
            {
                writer.WriteStartElement(reader.ReadString());

                //
                // If there is a second element that is a JSON object then
                // it represents the element attributes.
                //

                if (reader.TokenClass == JsonTokenClass.Object)
                {
                    reader.Read();

                    while (reader.TokenClass != JsonTokenClass.EndObject)
                    {
                        string name = reader.ReadMember();

                        if (reader.TokenClass == JsonTokenClass.Object ||
                            reader.TokenClass == JsonTokenClass.Array)
                        {
                            throw new JsonMLException(
                                "Attribute value cannot be structural, such as a JSON object or array.");
                        }

                        writer.WriteAttributeString(name,
                            reader.TokenClass == JsonTokenClass.Null ?
                                string.Empty : reader.Text);

                        reader.Read();
                    }

                    reader.Read();
                }

                //
                // Process any remaining elements as child elements
                // and text content.
                //

                while (reader.TokenClass != JsonTokenClass.EndArray)
                {
                    if (reader.TokenClass == JsonTokenClass.Object)
                    {
                        throw new JsonMLException(
                            "Found JSON object when expecting " +
                            "either a JSON array representing a child element " +
                            "or a JSON string representing text content.");
                    }

                    if (reader.TokenClass == JsonTokenClass.Array)
                    {
                        Decode(reader, writer);
                    }
                    else if (reader.TokenClass == JsonTokenClass.Null)
                    {
                        reader.Read();
                    }
                    else
                    {
                        writer.WriteString(reader.Text);
                        reader.Read();
                    }
                }

                writer.WriteEndElement();
            }

            reader.Read();
        }

        /// <summary>
        /// Converts JsonML in object form to XML.
        /// </summary>

        public static void DecodeObjectForm(JsonReader reader, XmlWriter writer)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            if (writer == null) throw new ArgumentNullException("writer");

            reader.ReadToken(JsonTokenClass.Object);

            string tagName = null;
            JsonObject attributes = null;
            JsonRecorder childNodes = null;
            ArrayList childNodesList = null;
            
            while (reader.TokenClass != JsonTokenClass.EndObject)
            {
                string memberName = reader.ReadMember();
                
                switch (memberName)
                {
                    case "tagName":
                    {
                        if (tagName != null && string.CompareOrdinal(reader.Text, tagName) != 0)
                            throw new JsonMLException("Tag name already defined.");
                        tagName = reader.Text;
                        if (tagName.Length == 0)
                            throw new JsonMLException("Tag name cannot be empty.");
                        reader.Read();
                        break;
                    }
                    case "childNodes":
                    {
                        if (reader.TokenClass == JsonTokenClass.Null)
                        {
                            reader.Read();
                        }
                        else
                        {
                            if (reader.TokenClass != JsonTokenClass.Array)
                                throw new JsonMLException("Child nodes must be a JSON array.");
                            
                            JsonRecorder aChildNodes = new JsonRecorder();
                            aChildNodes.WriteFromReader(reader);
                            if (childNodes == null)
                            {
                                childNodes = aChildNodes;
                            }
                            else
                            {
                                if (childNodesList == null)
                                    childNodesList = new ArrayList(4);
                                childNodesList.Add(aChildNodes);
                            }
                        }
                        break;
                    }
                    default:
                    {
                        if (attributes == null)
                            attributes = new JsonObject();
                        if (reader.TokenClass == JsonTokenClass.Array || reader.TokenClass == JsonTokenClass.Object)
                            throw new JsonMLException("Non-scalar attribute value.");
                        if (reader.TokenClass != JsonTokenClass.Null)
                            attributes.Add(memberName, reader.Text);
                        reader.Read();
                        break;
                    }
                }
            }

            reader.Read();

            if (tagName == null && writer.WriteState != WriteState.Element && writer.WriteState != WriteState.Content)
                throw new JsonMLException("Tag name not found.");

            writer.WriteStartElement(tagName);

            if (attributes != null)
            {
                foreach (JsonMember attribute in attributes)
                    writer.WriteAttributeString(attribute.Name, attribute.Value.ToString());
            }

            DecodeChildNodes(childNodes, writer);

            if (childNodesList != null)
            {
                foreach (JsonRecorder aChildNodes in childNodesList)
                    DecodeChildNodes(aChildNodes, writer);
            }

            writer.WriteEndElement();
        }

        private static void DecodeChildNodes(JsonRecorder recording, XmlWriter writer)
        {
            Debug.Assert(writer != null);

            if (recording != null)
                DecodeChildNodes(recording.CreatePlayer(), writer);
        }

        private static void DecodeChildNodes(JsonReader reader, XmlWriter writer) 
        {
            Debug.Assert(reader != null);
            Debug.Assert(writer != null);

            reader.ReadToken(JsonTokenClass.Array);
            
            while (reader.TokenClass != JsonTokenClass.EndArray)
            {
                if (reader.TokenClass == JsonTokenClass.Object)
                {
                    DecodeObjectForm(reader, writer);
                }
                else if (reader.TokenClass == JsonTokenClass.Array)
                {
                    throw new JsonMLException("JSON array is not a valid child node.");
                }
                else
                {
                    if (reader.TokenClass != JsonTokenClass.Null)
                        writer.WriteString(reader.Text);
                    reader.Read();
                }
            }
            
            reader.Read();
        }

        /// <summary>
        /// Converts XML to JsonML array form.
        /// </summary>

        public static void Encode(XmlReader reader, JsonWriter writer)
        {
            EncodeArrayForm(reader, writer);
        }

        /// <summary>
        /// Converts XML to JsonML. A parameter specifies the output JsonML form.
        /// </summary>

        public static void Encode(XmlReader reader, JsonWriter writer, JsonMLForm form)
        {
            switch (form)
            {
                case JsonMLForm.ObjectForm: EncodeObjectForm(reader, writer); break;
                case JsonMLForm.ArrayForm: EncodeArrayForm(reader, writer); break;
                default: throw new ArgumentException(null, "form");
            }
        }

        /// <summary>
        /// Converts XML to JsonML array form.
        /// </summary>

        public static void EncodeArrayForm(XmlReader reader, JsonWriter writer)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            if (writer == null) throw new ArgumentNullException("writer");

            if (reader.MoveToContent() != XmlNodeType.Element)
                throw new ArgumentException(null, "reader");

            writer.WriteStartArray();
            writer.WriteString(reader.Name);

            //
            // Write attributes
            //

            if (reader.MoveToFirstAttribute())
            {
                writer.WriteStartObject();

                do
                {
                    writer.WriteMember(reader.Name);
                    writer.WriteString(reader.Value);
                }
                while (reader.MoveToNextAttribute());

                writer.WriteEndObject();
                reader.MoveToElement();
            }

            bool isEmpty = reader.IsEmptyElement;

            if (!isEmpty)
            {
                reader.Read();

                //
                // Write child nodes (text, CDATA and element)
                //

                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    if (reader.NodeType == XmlNodeType.Text || reader.NodeType == XmlNodeType.CDATA)
                    {
                        writer.WriteString(reader.Value);
                        reader.Read();
                    }
                    else if (reader.NodeType == XmlNodeType.Element)
                    {
                        Encode(reader, writer);
                    }
                    else
                    {
                        reader.Read();
                    }
                }
            }

            writer.WriteEndArray();
            reader.Read();
        }

        /// <summary>
        /// Converts XML to JsonML object form.
        /// </summary>

        public static void EncodeObjectForm(XmlReader reader, JsonWriter writer)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            if (writer == null) throw new ArgumentNullException("writer");

            if (reader.MoveToContent() != XmlNodeType.Element)
                throw new ArgumentException(null, "reader");

            writer.WriteStartObject();
            writer.WriteMember("tagName");
            writer.WriteString(reader.Name);

            //
            // Write attributes
            //

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    writer.WriteMember(reader.Name);
                    writer.WriteString(reader.Value);
                }
                while (reader.MoveToNextAttribute());

                reader.MoveToElement();
            }

            bool isEmpty = reader.IsEmptyElement;

            if (!isEmpty)
            {
                reader.Read();

                int childCount = 0;

                //
                // Write child nodes (text, CDATA and element)
                //

                XmlNodeType nodeType;
                while ((nodeType = reader.NodeType) != XmlNodeType.EndElement)
                {
                    if (nodeType == XmlNodeType.Text 
                        || nodeType == XmlNodeType.CDATA 
                        || nodeType == XmlNodeType.Element)
                    {
                        if (childCount == 0)
                        {
                            writer.WriteMember("childNodes");
                            writer.WriteStartArray();
                        }

                        if (nodeType == XmlNodeType.Text || nodeType == XmlNodeType.CDATA)
                        {
                            writer.WriteString(reader.Value);
                            reader.Read();
                        }
                        else if (nodeType == XmlNodeType.Element)
                        {
                            EncodeObjectForm(reader, writer);
                        }

                        childCount++;
                    }
                    else
                    {
                        reader.Read();
                    }
                }

                if (childCount > 0)
                    writer.WriteEndArray();
            }

            writer.WriteEndObject();
            reader.Read();
        }

        private JsonMLCodec()
        {
            throw new NotSupportedException();
        }
    }
}
