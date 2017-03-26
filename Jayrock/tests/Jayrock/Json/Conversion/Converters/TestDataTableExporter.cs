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
    using System.Data;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestDataTableExporter
    {
        [ Test ]
        public void Superclass()
        {
            Assert.IsInstanceOfType(typeof(ExporterBase), new DataTableExporter());
        }

        [ Test ]
        public void InputTypeIsDataTable()
        {
            Assert.AreSame(typeof(DataTable), (new DataTableExporter()).InputType);
        }

        [ Test ]
        public void TableExportedViaDataView()
        {
            DataTable table = new DataTable();

            ExportContext context = new ExportContext();
            TestDataViewExporter exporter = new TestDataViewExporter();
            context.Register(exporter);
            context.Export(table, new JsonRecorder());

            Assert.AreSame(table.DefaultView, exporter.LastExported);
        }

        private sealed class TestDataViewExporter : IExporter
        {
            public DataView LastExported;

            public Type InputType
            {
                get { return typeof(DataView); }
            }

            public void Export(ExportContext context, object value, JsonWriter writer)
            {
                LastExported = (DataView) value;
                (new DataViewExporter(InputType)).Export(context, value, writer);
            }
        }
    }
}