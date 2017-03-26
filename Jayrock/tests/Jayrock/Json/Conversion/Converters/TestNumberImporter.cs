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
    public class TestNumberImporter
    {
        [ Test ]
        public void ImportByte()
        {
            AssertImport((byte) 123, "123");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportOutsideByte()
        {
            AssertImport((byte) 0, "456");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportObject()
        {
            AssertImport((byte) 0, "{}");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportArray()
        {
            AssertImport((byte) 0, "[]");
        }

        [ Test ]
        public void ImportInt16()
        {
            AssertImport((short) 456, "456");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportOutsideInt16()
        {
            AssertImport((short) 0, "456789");
        }
        
        [ Test ]
        public void ImportString()
        {
            AssertImport(123456789, "'123456789'");
        }

        [ Test ]
        public void ImportBoolean()
        {
            AssertImport(1, "true");
            AssertImport(0, "false");
        }

        #if !NET_1_0 && !NET_1_1 && !NET_2_0

        [ Test ]
        public void ImportBigInteger()
        {
            AssertImport(System.Numerics.BigInteger.Pow(long.MaxValue, 3), "784637716923335095224261902710254454442933591094742482943");
        }

        #endif // !NET_1_0 && !NET_1_1 && !NET_2_0

        [ Test ]
        public void ImportDecimalUsingExponentialNotation()
        {
            //
            // Exercises bug #13396
            // http://developer.berlios.de/bugs/?func=detailbug&bug_id=13396&group_id=4638
            //

            AssertImport(7.25e-5m, "7.25e-005");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportNull()
        {
            ImportContext importContext = new ImportContext();
            JsonTextReader reader = new JsonTextReader(new StringReader("null"));
            new Int32Importer().Import(importContext, reader);
        }

        private static void AssertImport(object expected, string input)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(input));
            Type expectedType = expected.GetType();
            ImportContext context = new ImportContext();
            object o = context.Import(expectedType, reader);
            Assert.IsTrue(reader.EOF, "Reader must be at EOF.");
            Assert.IsInstanceOfType(expectedType, o);
            Assert.AreEqual(expected, o);
        }
    }
}