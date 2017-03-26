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
    public class TestTypeExporterBase
    {
        private readonly ThingExporter _exporter = new ThingExporter();
        
        [ Test ]
        public void ExportNull()
        {
            Export(null).ReadNull();
        }

        [ Test ]
        public void InputTypeInitialization()
        {
            Assert.AreSame(typeof(Thing), _exporter.InputType);
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotUseNullContext()
        {
            _exporter.Export(null, null, null);
        }
 
        private JsonReader Export(object value)
        {
            JsonRecorder writer = new JsonRecorder();
            _exporter.Export(new ExportContext(), value, writer);
            return writer.CreatePlayer();
        }
        
        private class Thing
        {
        }

        private class ThingExporter : ExporterBase
        {
            public ThingExporter() : base(typeof(Thing)) {}
            
            protected override void ExportValue(ExportContext context, object value, JsonWriter writer)
            {
            }
        }
    }
}