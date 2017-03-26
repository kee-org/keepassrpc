#region License, Terms and Conditions
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (www.raboof.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
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
    using System.Text;
    using Jayrock.Json;

    #endregion

    public delegate JsonReader JsonTextReaderFactoryHandler(TextReader reader, object options);
    public delegate JsonWriter JsonTextWriterFactoryHandler(TextWriter writer, object options);

    /// <summary>
    /// Facade for working with JsonReader and JsonWriter implementations
    /// that work with JSON text.
    /// </summary>

    public sealed class JsonText
    {
        private static readonly JsonTextReaderFactoryHandler _defaultReaderFactory;
        private static readonly JsonTextWriterFactoryHandler _defaultWriterFactory;

        private static JsonTextReaderFactoryHandler _currentReaderFactory;
        private static JsonTextWriterFactoryHandler _currentWriterFactory;

        static JsonText()
        {
            _currentReaderFactory = _defaultReaderFactory = new JsonTextReaderFactoryHandler(DefaultReaderFactoryImpl);
            _currentWriterFactory = _defaultWriterFactory = new JsonTextWriterFactoryHandler(DefaultWriterFactoryImpl);
        }

        private static JsonReader DefaultReaderFactoryImpl(TextReader reader, object options)
        {
            return new JsonTextReader(reader);
        }

        private static JsonWriter DefaultWriterFactoryImpl(TextWriter writer, object options)
        {
            return new JsonTextWriter(writer);
        }

        public static JsonTextReaderFactoryHandler DefaultReaderFactory { get { return _defaultReaderFactory; } }
        public static JsonTextWriterFactoryHandler DefaultWriterFactory { get { return _defaultWriterFactory; } }

        public static JsonTextReaderFactoryHandler CurrentReaderFactory
        {
            get { return _currentReaderFactory; }
            
            set
            {
                if (value == null) 
                    throw new ArgumentNullException("value");
                
                _currentReaderFactory = value;
            }
        }

        public static JsonTextWriterFactoryHandler CurrentWriterFactory
        {
            get { return _currentWriterFactory; }
 
            set
            {
                if (value == null) 
                    throw new ArgumentNullException("value");
                
                _currentWriterFactory = value;
            }
        }

        public static JsonReader CreateReader(TextReader reader)
        {
            return CurrentReaderFactory(reader, null);
        }

        public static JsonReader CreateReader(string source)
        {
            return CreateReader(new StringReader(source));
        }

        public static JsonWriter CreateWriter(TextWriter writer)
        {
            return CurrentWriterFactory(writer, null);
        }

        public static JsonWriter CreateWriter(StringBuilder sb)
        {
            return CreateWriter(new StringWriter(sb));
        }

        private JsonText()
        {
            throw new NotSupportedException();
        }
    }
}