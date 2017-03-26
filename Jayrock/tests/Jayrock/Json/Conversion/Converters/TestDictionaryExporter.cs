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
    public class TestDictionaryExporter
    {
        [ Test ]
        public void Superclass()
        {
            Assert.IsInstanceOfType(typeof(ExporterBase), new DictionaryExporter(typeof(Hashtable)));    
        }

        [ Test ]
        public void InputTypeInitialization()
        {
            Type type = typeof(Hashtable);
            DictionaryExporter exporter = new DictionaryExporter(type);
            Assert.AreSame(type, exporter.InputType);
        }

        [ Test ]
        public void ExportEmpty()
        {
            JsonReader reader = Export(new Hashtable());
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual(JsonTokenClass.EndObject, reader.TokenClass);
        }

        [ Test ]
        public void ExportFlat()
        {
            Hashtable h = new Hashtable();
         
            h.Add("FirstName", "John");
            h.Add("LastName", "Doe");
            h.Add("MiddleName", null);
            
            JsonReader reader = Export(h);
            
            //
            // We need a complex assertions loop here because the order in 
            // which members are written cannot be guaranteed to follow
            // the order of insertion.
            //
            
            reader.ReadToken(JsonTokenClass.Object);
            while (reader.TokenClass != JsonTokenClass.EndObject)
            {
                string member = reader.ReadMember();
                Assert.IsTrue(h.Contains(member));
            
                object expected = h[member];
                
                if (expected == null)
                    reader.ReadNull();
                else
                    Assert.AreEqual(expected, reader.ReadString());
            }
        }

        private static JsonReader Export(IDictionary value)
        {
            JsonRecorder writer = new JsonRecorder();
            JsonConvert.Export(value, writer);
            return writer.CreatePlayer();
        }
    }
}