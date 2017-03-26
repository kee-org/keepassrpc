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
    public class TestDateTimeImporter
    {
        [ Test ]
        public void ImportNull()
        {
            Assert.IsNull(Import("null"));
        }

        [ Test ]
        public void ImportString()
        {
            DateTime time = new DateTime(1999, 12, 31, 23, 30, 59, 999);
            AssertImport(time, "\"1999-12-31T23:30:59.9990000" + Tzd(time) + "\"");
        }

        [ Test ]
        public void ImportNumber()
        {
            AssertImport(new DateTime(2006, 7, 17, 10, 56, 56), "1153133816", true);
        }

        [ Test ]
        public void ImportNegativeNumber()
        {
            AssertImport(new DateTime(1098, 7, 6, 5, 43, 21), "-27501531399", true);
        }

        [ Test ]
        public void ImportFractionalNumber()
        {
            AssertImport(new DateTime(2006, 7, 17, 10, 56, 56, 456), "1153133816.456", true);
        }

        [ Test ]
        public void ImportNegativeFractionalNumber()
        {
            AssertImport(new DateTime(1098, 7, 6, 5, 43, 21, 234), "-27501531398.766", true);
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportTrue()
        {
            Import("true");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportFalse()
        {
            Import("false");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportArray()
        {
            Import("[]");
        }
        
        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportObject()
        {
            Import("{}");
        }
        
        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportOutOfRangeNumber()
        {
            Import(new string('9', 309));
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportBadIsoDate()
        {
            Import("'1999-12=31T23:30:59'");
        }

        [ Test ]
        public void ImportMicrosoftAjaxFormat()
        {
            AssertImport(new DateTime(2006, 7, 17, 10, 56, 56, 456), @"'\/Date(1153133816456)\/'", true);
            AssertImport(new DateTime(1098, 7, 06, 05, 43, 21, 234), @"'\/Date(-27501531398766)\/'", true);
        }

        [ Test ]
        public void ImportMicrosoftAjaxFormatWithUpperCase()
        {
            AssertImport(new DateTime(2006, 7, 17, 10, 56, 56, 456), @"'\/DATE(1153133816456)\/'", true);
            AssertImport(new DateTime(1098, 7, 06, 05, 43, 21, 234), @"'\/DATE(-27501531398766)\/'", true);
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportBadMicrosoftAjaxFormat()
        {
            Import(@"'/Date[1285511729000]/'");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportMicrosoftAjaxFormatCausingOverflow()
        {
            Import(@"'\/Date(9999999999999999999)\/'");
        }

        private static void AssertImport(DateTime expected, string input)
        {
            AssertImport(expected, input, false);
        }

        private static void AssertImport(DateTime expected, string input, bool expectingUTC)
        {
            object o = Import(input);
            Assert.IsInstanceOfType(typeof(DateTime), o);
            if (expectingUTC)
                Assert.AreEqual(expected, ((DateTime) o).ToUniversalTime());
            else 
                Assert.AreEqual(expected, o);
        }

        private static object Import(string input)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(input));
            ImportContext context = new ImportContext();
            object value = context.Import(typeof(DateTime), reader);
            Assert.IsTrue(reader.EOF, "Reader must be at EOF.");
            if (value != null)
                Assert.IsInstanceOfType(typeof(DateTime), value);
            return value;
        }

        private static string Tzd(DateTime localTime)
        {
            TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset(localTime);
            string offsetString = offset.ToString();
            return offset.Ticks < 0 ? 
                   (offsetString.Substring(0, 6)) : 
                   ("+" + offsetString.Substring(0, 5));
        }
    }
}