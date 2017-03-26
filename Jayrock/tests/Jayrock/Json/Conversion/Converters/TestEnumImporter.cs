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
    public class TestEnumImporter
    {
        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotInitWithNull()
        {
            new EnumImporter(null);
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void ActualTypeMustBeEnum()
        {
            new EnumImporter(typeof(object));
        }

        [ Test ]
        public void ImportNull()
        {
            Assert.IsNull(Import(typeof(Days), JsonNull.Text));
        }

        [ Test ]
        public void ImportString()
        {
            Assert.AreEqual(Days.Friday, Import(typeof(Days), "'Friday'"));
        }

        [ Test ]
        public void ImportStringIgnoresCase()
        {
            Assert.AreEqual(Days.Friday, Import(typeof(Days), "'FRIDAY'"));
            Assert.AreEqual(Days.Friday, Import(typeof(Days), "'friday'"));
            Assert.AreEqual(Days.Friday, Import(typeof(Days), "'FrIdAy'"));
        }

        [ Test ]
        public void ImportStringIgnoresWhitespace()
        {
            Assert.AreEqual(Days.Friday, Import(typeof(Days), "' Friday '"));
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportUnknownName()
        {
            Assert.AreEqual(Days.Friday, Import(typeof(Days), "'???'"));
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportNumericalString()
        {
            Import(typeof(Days), "' 3 '");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportPositiveNumericalString()
        {
            Import(typeof(Days), "' +3 '");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportNegativeNumericalString()
        {
            Import(typeof(Days), "' -3 '");
        }
        
        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportNumber()
        {
            Import(typeof(Days), "3");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportBoolean()
        {
            Import(typeof(Days), JsonBoolean.TrueText);
            Import(typeof(Days), JsonBoolean.FalseText);
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportObject()
        {
            Import(typeof(Days), "{}");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportArray()
        {
            Import(typeof(Days), "[]");
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void BitFieldEnumsNotSupported()
        {
            new EnumImporter(typeof(AttributeTargets));
        }

        private static object Import(Type type, string s)
        {
            EnumImporter importer = new EnumImporter(type);
            return importer.Import(new ImportContext(), CreateReader(s));
        }

        private static JsonReader CreateReader(string s)
        {
            return new JsonTextReader(new StringReader(s));
        }
        
        private enum Days
        {
            Sunday,
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday
        }
    }
}