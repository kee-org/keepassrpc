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
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonBufferImporter
    {        
        [ Test ]
        public void OutputypeIsJsonBuffer()
        {
            Assert.AreSame(typeof(JsonBuffer), (new JsonBufferImporter()).OutputType);
        }

        [ Test ]
        public void Import()
        {
            JsonReader reader = JsonText.CreateReader("[42,{x:123,y:456},foo,true,null,false]");
            JsonBufferImporter importer = new JsonBufferImporter();
            JsonBuffer buffer = (JsonBuffer) importer.Import(JsonConvert.CreateImportContext(), reader);
            Assert.AreEqual("[42,{\"x\":123,\"y\":456},\"foo\",true,null,false]", buffer.ToString());
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotImportWithNullContext()
        {
            new JsonBufferImporter().Import(null, JsonText.CreateReader(string.Empty));
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotImportWithNullWriter()
        {
            new JsonBufferImporter().Import(JsonConvert.CreateImportContext(), null);
        }
    }
}