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
    using System.IO;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestTypeImporterBase
    {
        [ Test ]
        public void OutputTypeInitialization()
        {
            TestImporter importer = new TestImporter();
            Assert.AreSame(typeof(object), importer.OutputType);
        }
        
        [ Test ]
        public void NullHandling()
        {
            JsonReader reader = CreateReader("null");
            TestImporter importer = new TestImporter();
            Assert.IsNull(importer.Import(new ImportContext(), reader));
            Assert.IsTrue(reader.EOF);
        }
        
        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportNumber()
        {
            Import("42");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportBoolean()
        {
            Import("true");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportString()
        {
            Import("'string'");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportObject()
        {
            Import("{}");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportArray()
        {
            Import("[]");
        }
        
        [ Test ]
        public void NumberCallsImportNumber()
        {
            JsonReader reader = CreateReader("42");
            ImporterMock importer = new ImporterMock();
            const int result = 42;
            importer.Number = result;
            Assert.AreEqual(result, importer.Import(new ImportContext(), reader));
        }

        [ Test ]
        public void StringCallsImportString()
        {
            JsonReader reader = CreateReader("''");
            ImporterMock importer = new ImporterMock();
            const string result = "hello";
            importer.String = result;
            Assert.AreEqual(result, importer.Import(new ImportContext(), reader));
        }
        
        [ Test ]
        public void BooleanCallsImportBoolean()
        {
            JsonReader reader = CreateReader("true");
            ImporterMock importer = new ImporterMock();
            importer.Boolean = true;
            Assert.AreEqual(true, importer.Import(new ImportContext(), reader));
        }

        [ Test ]
        public void ArrayCallsImportArray()
        {
            JsonReader reader = CreateReader("[]");
            ImporterMock importer = new ImporterMock();
            object result = new object();
            importer.Array = result;
            Assert.AreEqual(result, importer.Import(new ImportContext(), reader));
        }

        [ Test ]
        public void ObjectCallsImportObject()
        {
            JsonReader reader = CreateReader("{}");
            ImporterMock importer = new ImporterMock();
            object result = new object();
            importer.Object = result;
            Assert.AreEqual(result, importer.Import(new ImportContext(), reader));
        }
        
        private static void Import(string s)
        {
            (new TestImporter()).Import(new ImportContext(), CreateReader(s));
        }

        private static JsonReader CreateReader(string s)
        {
            return new JsonTextReader(new StringReader(s));
        }
        
        private class TestImporter : ImporterBase
        {
            public TestImporter() : 
                base(typeof(object)) {}
        }

        private class ImporterMock : TestImporter
        {
            public object Boolean = null;
            public object Number = null;
            public object String = null;
            public object Object = null;
            public object Array = null;

            protected override object ImportFromBoolean(ImportContext context, JsonReader reader)
            {
                Assert.IsNotNull(context);
                Assert.IsNotNull(reader);
                
                return Boolean;
            }

            protected override object ImportFromNumber(ImportContext context, JsonReader reader)
            {
                Assert.IsNotNull(context);
                Assert.IsNotNull(reader);
                
                return Number;
            }

            protected override object ImportFromString(ImportContext context, JsonReader reader)
            {
                Assert.IsNotNull(context);
                Assert.IsNotNull(reader);
                
                return String;
            }

            protected override object ImportFromArray(ImportContext context, JsonReader reader)
            {
                Assert.IsNotNull(context);
                Assert.IsNotNull(reader);
                
                return Array;
            }

            protected override object ImportFromObject(ImportContext context, JsonReader reader)
            {
                Assert.IsNotNull(context);
                Assert.IsNotNull(reader);
                
                return Object;
            }
        }
    }
}