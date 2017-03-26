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

    using System;
    using System.Collections;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestEnumerableExporter
    {
        [ Test ]
        public void Superclass()
        {
            Assert.IsInstanceOfType(typeof(ExporterBase), new EnumerableExporter(typeof(Array)));    
        }

        [ Test ]
        public void InputTypeInitialization()
        {
            Type type = typeof(Array);
            EnumerableExporter exporter = new EnumerableExporter(type);
            Assert.AreSame(type, exporter.InputType);
        }

        [ Test ]
        public void ExportEmpty()
        {
            JsonReader reader = Export(new object[] {});
            reader.ReadToken(JsonTokenClass.Array);
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [ Test ]
        public void ExportFlatArray()
        {
            JsonReader reader = Export(new int[] { 11, 22, 33 });
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual(11, reader.ReadNumber().ToInt32());
            Assert.AreEqual(22, reader.ReadNumber().ToInt32());
            Assert.AreEqual(33, reader.ReadNumber().ToInt32());
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [ Test ]
        public void ExportList()
        {
            JsonReader reader = Export(new ArrayList(new int[] { 11, 22, 33 }));
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual(11, reader.ReadNumber().ToInt32());
            Assert.AreEqual(22, reader.ReadNumber().ToInt32());
            Assert.AreEqual(33, reader.ReadNumber().ToInt32());
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [ Test ]
        public void ExportNestedArrays()
        {
            JsonReader reader = Export(new ArrayList(new object[] { 11, 22, new object[] { 33, 44 }, 55 }));
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual(11, reader.ReadNumber().ToInt32());
            Assert.AreEqual(22, reader.ReadNumber().ToInt32());
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual(33, reader.ReadNumber().ToInt32());
            Assert.AreEqual(44, reader.ReadNumber().ToInt32());
            reader.ReadToken(JsonTokenClass.EndArray);
            Assert.AreEqual(55, reader.ReadNumber().ToInt32());
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        private static JsonReader Export(IEnumerable values)
        {
            JsonRecorder writer = new JsonRecorder();
            JsonConvert.Export(values, writer);
            return writer.CreatePlayer();
        }
    }
}