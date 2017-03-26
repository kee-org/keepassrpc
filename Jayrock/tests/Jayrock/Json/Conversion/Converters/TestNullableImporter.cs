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

#if !NET_1_0 && !NET_1_1 

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestNullableImporter
    {
        [ Test ]
        public void Superclass()
        {
            Assert.IsInstanceOfType(typeof(ImporterBase), new NullableImporter(typeof(int?)));
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void CannotInitializeWithNonNullableType()
        {
            new NullableImporter(typeof(int));
        }

        [ Test ]
        public void ImportNull()
        {
            Assert.IsNull(Import(false, "null"));
        }

        [ Test ]
        public void ImportNumber()
        {
            Import(true, "42");
        }

        [ Test ]
        public void ImportBoolean()
        {
            Import(true, "true");
        }

        [ Test ]
        public void ImportString()
        {
            Import(true, "'string'");
        }
       
        [ Test ]
        public void ImportArray()
        {
            Import(true, "[]");
        }
        
        [ Test ]
        public void ImportObject()
        {
            Import(true, "{}");
        }

        private static object Import(bool hasValue, string input)
        {
            JsonReader reader = JsonText.CreateReader(input);
            ImportContext context = JsonConvert.CreateImportContext();
            ThingImporter thingImporter = new ThingImporter();
            context.Register(thingImporter);
            NullableImporter importer = new NullableImporter(typeof(Thing?));
            object thing = importer.Import(context, reader);
            Assert.AreEqual(hasValue, thingImporter.ImportCalled);
            if (hasValue)
                Assert.IsInstanceOfType(typeof(Thing), thing);
            else
                Assert.IsNull(thing);
            Assert.IsTrue(reader.EOF, "Reader must be at EOF.");
            return thing;
        }

        private struct Thing {}

        private sealed class ThingImporter : IImporter
        {
            public bool ImportCalled;

            public Type OutputType { get { return typeof(Thing); } }

            public object Import(ImportContext context, JsonReader reader)
            {
                ImportCalled = true;
                reader.Skip();
                return new Thing();
            }
        }
    }
}

#endif // !NET_1_0 && !NET_1_1
