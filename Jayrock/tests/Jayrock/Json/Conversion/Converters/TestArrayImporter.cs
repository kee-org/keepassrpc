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
    using System.Collections.Specialized;
    using System.IO;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestArrayImporter
    {
        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void ElementTypeMustBeArray()
        {
            new ArrayImporter(typeof(object));
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void ElementTypeMustBeOneDimensionArray()
        {
            new ArrayImporter(typeof(object[,]));
        }

        [ Test ]
        public void ImportNull()
        {
            ArrayImporter importer = new ArrayImporter();
            Assert.IsNull(importer.Import(new ImportContext(), CreateReader("null")));
        }

        [ Test ]
        public void ImportEmptyArray()
        {
            AssertImport(new int[0], "[]");
        }

        [ Test ]
        public void ImportInt32Array()
        {
            AssertImport(new int[] { 123, 789, 456 }, "[ 123, 789, 456 ]");
        }
        
        [ Test ]
        public void ImportStringArray()
        {
            AssertImport(new string[] { "see no evil", "hear no evil", "speak no evil" }, 
                "[ 'see no evil', 'hear no evil', 'speak no evil' ]");
        }

        [ Test ]
        public void ImportDateArray()
        {
            AssertImport(new DateTime[] { new DateTime(1999, 12, 31), new DateTime(2000, 1, 1),  }, "[ '1999-12-31', '2000-01-01' ]");
        }
        
        [ Test ]
        public void ImportStringAsArray()
        {
            AssertImport(new string[] { "foobar" }, "'foobar'");
        }

        [ Test ]
        public void ImportNumberAsArray()
        {
            AssertImport(new int[] { 123 }, "123");
        }

        [ Test ]
        public void ImportBooleanAsArray()
        {
            AssertImport(new bool[] { true }, "true");
            AssertImport(new bool[] { true }, "123");
            AssertImport(new bool[] { false }, "false");
            AssertImport(new bool[] { false }, "0");
        }

        private static void AssertImport(Array expected, string s)
        {
            JsonReader reader = CreateReader(s);
            
            ImportContext context = new ImportContext();            
            object o = context.Import(expected.GetType(), reader);
            Assert.IsTrue(reader.EOF);

            if (expected == null)
                Assert.IsNull(o);
            
            Assert.AreEqual(expected, o);
        }

        private static JsonReader CreateReader(string s)
        {
            return new JsonTextReader(new StringReader(s));
        }
    }
}