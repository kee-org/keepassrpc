#region License, Terms and Conditions
//
// The MIT License
// Copyright (c) 2006, Atif Aziz. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files 
// (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, subject 
// to the following conditions:
//
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
// Author(s):
//  Atif Aziz (http://www.raboof.com)
//
#endregion

namespace TidyJson
{
    #region Imports

    using System;
    using Jayrock.Json;

    #endregion
    
    internal sealed class JsonColorWriter : JsonWriter
    {
        private readonly JsonWriter inner;

        public JsonColorWriter(JsonWriter inner) :
            this(inner, null) {}

        public JsonColorWriter(JsonWriter inner, JsonPalette palette)
        {
            this.inner = inner;
            this.Palette = palette ?? JsonPalette.Auto();
        }

        public JsonWriter InnerWriter
        {
            get { return inner; }
        }

        public JsonPalette Palette { get; set; }

        public override int Index
        {
            get { return inner.Index; }
        }

        public override JsonWriterBracket Bracket
        {
            get { return inner.Bracket; }
        }

        public override void WriteStartObject()
        {
            Palette.Object.Apply();
            inner.WriteStartObject();
        }

        public override void WriteEndObject()
        {
            Palette.Object.Apply();
            inner.WriteEndObject();
        }

        public override void WriteMember(string name)
        {
            Palette.Member.Apply();
            inner.WriteMember(name);
        }

        public override void WriteStartArray()
        {
            Palette.Array.Apply();
            inner.WriteStartArray();
        }

        public override void WriteEndArray()
        {
            Palette.Array.Apply();
            inner.WriteEndArray();
        }

        public override void WriteString(string value)
        {
            Palette.String.Apply();
            inner.WriteString(value);
        }

        public override void WriteNumber(string value)
        {
            Palette.Number.Apply();
            inner.WriteNumber(value);
        }

        public override void WriteBoolean(bool value)
        {
            Palette.Boolean.Apply();
            inner.WriteBoolean(value);
        }

        public override void WriteNull()
        {
            Palette.Null.Apply();
            inner.WriteNull();
        }

        public override void Flush()
        {
            inner.Flush();
        }

        public override void Close()
        {
            inner.Close();
        }

        public override int Depth
        {
            get { return inner.Depth; }
        }

        public override int MaxDepth
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }
    }
}
