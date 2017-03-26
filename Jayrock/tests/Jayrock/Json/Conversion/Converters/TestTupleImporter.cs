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

#if !NET_1_0 && !NET_1_1 && !NET_2_0 

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestTupleImporter
    {
        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void CannotInitializeWithNonTupleType()
        {
            new TupleImporter(typeof(object));
        }

        [ Test ]
        public void Superclass()
        {
            Assert.IsInstanceOfType(typeof(ImporterBase), new TupleImporter(typeof(Tuple<object>)));
        }

        [ Test ]
        public void InputTypeIsTuple()
        {
            Assert.AreSame(typeof(Tuple<object>), new TupleImporter(typeof(Tuple<object>)).OutputType);
        }

        [ Test ]
        public void ImportTuple1FromNumber()
        {
            AssertImport(Tuple.Create(42), "42");
        }

        [ Test ]
        public void ImportTuple1FromString()
        {
            AssertImport(Tuple.Create("foo"), "foo");
        }

        [ Test ]
        public void ImportTuple1FromBoolean()
        {
            AssertImport(Tuple.Create(true), "true");
        }

        [ Test ]
        public void ImportTuple1FromNull()
        {
            var importer = new TupleImporter(typeof(Tuple<int>));
            var result = importer.Import(JsonConvert.CreateImportContext(), JsonText.CreateReader("null"));
            Assert.IsNull(result);
        }

        [ Test ]
        public void ImportTuple1FromObject()
        {
            var tuple = JsonConvert.Import<Tuple<JsonObject>>("{x:123,y:456}");
            Assert.IsNotNull(tuple.Item1);
            dynamic obj = tuple.Item1;
            Assert.AreEqual(123, obj.x.ToInt32());
            Assert.AreEqual(456, obj.y.ToInt32());
        }

        [ Test ]
        public void ImportTuple1FromArray()
        {
            var expected = Tuple.Create(new[] { 123, 456, 789 });
            var actual = JsonConvert.Import<Tuple<int[]>>("[123, 456, 789]");
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Item1, actual.Item1);
        }

        [ Test ]
        public void ImportTuple3FromArray()
        {
            AssertImport(Tuple.Create(42, "foo", true), "[42,foo,true]");
        }
        
        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportTuple2FromNumber()
        {
            JsonConvert.Import<Tuple<int, int>>("42");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportTuple2FromString()
        {
            JsonConvert.Import<Tuple<string, string>>("foo");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportTuple2FromBoolean()
        {
            JsonConvert.Import<Tuple<bool, bool>>("true");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportTuple2FromObject()
        {
            JsonConvert.Import<Tuple<JsonObject, JsonObject>>("{}");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportWhenArrayHasTooFewElements()
        {
            JsonConvert.Import<Tuple<int, string, bool>>("[123,foo]");
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void SubClassCannotExportValueWithNullContext()
        {
            var tuple = Tuple.Create(42);
            var exporter = new ExportValueTestExporter(tuple.GetType());
            exporter.ForceNullContext = true;
            var context = JsonConvert.CreateExportContext();
            var writer = new JsonBufferWriter();
            exporter.Export(context, tuple, writer);
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void SubClassCannotExportValueWithNullWriter()
        {
            var tuple = Tuple.Create(42);
            var exporter = new ExportValueTestExporter(tuple.GetType());
            exporter.ForceNullWriter = true;
            var context = JsonConvert.CreateExportContext();
            var writer = new JsonBufferWriter();
            exporter.Export(context, tuple, writer);
        }

        private static void AssertImport(object expected, string input)
        {
            var importer = new TupleImporter(expected.GetType());
            var reader = JsonText.CreateReader(input);
            var context = JsonConvert.CreateImportContext();
            var actual = importer.Import(context, reader);
            Assert.IsTrue(reader.EOF, "Reader must be at EOF.");
            Assert.AreEqual(expected, actual);
        }

        class ExportValueTestExporter : TupleExporter
        {
            public bool ForceNullContext;
            public bool ForceNullWriter;

            public ExportValueTestExporter(Type inputType) : 
                base(inputType) {}

            protected override void ExportValue(ExportContext context, object value, JsonWriter writer)
            {
                base.ExportValue(ForceNullContext ? null : context, value, ForceNullWriter ? null : writer);
            }
        }
    }
}

#endif
