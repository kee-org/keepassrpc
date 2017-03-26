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
    using Jayrock.Json.Conversion;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestImportAwareImporter
    {
        [ Test ]
        public void ImportTellsObjectToImportSelf()
        {
            ImportAwareImporter importer = new ImportAwareImporter(typeof(Thing));
            JsonRecorder writer = new JsonRecorder();
            writer.WriteString(string.Empty);
            Thing thing = (Thing) importer.Import(new ImportContext(), writer.CreatePlayer());
            Assert.IsTrue(thing.ImportCalled);
        }

        [ Test ]
        public void ImportNull()
        {
            ImportAwareImporter importer = new ImportAwareImporter(typeof(Thing));
            JsonRecorder writer = new JsonRecorder();
            writer.WriteNull();
            JsonReader reader = writer.CreatePlayer();
            reader.ReadToken(JsonTokenClass.Array);
            Assert.IsNull(importer.Import(new ImportContext(), reader));
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotInitWithNullType()
        {
            new ImportAwareImporter(null);
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotSendNullReaderToImport()
        {
            ImportAwareImporter importer = new ImportAwareImporter(typeof(Thing));
            importer.Import(null, null);
        }
        
        private sealed class Thing : IJsonImportable
        {
            public bool ImportCalled;
            
            public void Import(ImportContext context, JsonReader reader)
            {
                ImportCalled = true;
            }
        }
    }
}