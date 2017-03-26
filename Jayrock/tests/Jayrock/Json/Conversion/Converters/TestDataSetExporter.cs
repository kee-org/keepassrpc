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
    using System.Data;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestDataSetExporter
    {
        [ Test ]
        public void Superclass()
        {
            Assert.IsInstanceOfType(typeof(ExporterBase), new DataSetExporter());
        }

        [ Test ]
        public void InputTypeIsDataSet()
        {
            Assert.AreSame(typeof(DataSet), (new DataSetExporter()).InputType);
        }

        [ Test ]
        public void EmptyDataSetYieldsEmptyJsonObject()
        {
            DataSet ds = new DataSet();
            Assert.AreEqual("{}", JsonConvert.ExportToString(ds));
        }

        [ Test ]
        public void TableNamesBecomeJsonObjectMemberNames()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable("Table1"));
            ds.Tables.Add(new DataTable("Table2"));
            
            JsonRecorder writer = new JsonRecorder();
            JsonConvert.Export(ds, writer);
            JsonReader reader = writer.CreatePlayer();

            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("Table1", reader.ReadMember());
            reader.Skip(); // skip table contents
            Assert.AreEqual("Table2", reader.ReadMember());
            reader.Skip(); // skip table contents
            reader.ReadToken(JsonTokenClass.EndObject);
            Assert.IsTrue(reader.EOF);
        }

        [ Test ]
        public void TableExportedViaItsExporter()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable("Table1"));

            ExportContext context = new ExportContext();
            TestDataTableExporter exporter = new TestDataTableExporter();
            context.Register(exporter);
            context.Export(ds, new JsonRecorder());

            Assert.AreSame(ds.Tables[0], exporter.LastExported);
        }

        [ Test ]
        public void TablesExportedEvenWithoutExporterInContext()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable("Table1"));

            TestExportContext context = new TestExportContext();
            context.Register(new DataSetExporter());
            JsonRecorder writer = new JsonRecorder();
            context.Export(ds, writer);
            JsonReader reader = writer.CreatePlayer();

            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("Table1", reader.ReadMember());
            reader.Skip(); // skip table contents
            reader.ReadToken(JsonTokenClass.EndObject);
            Assert.IsTrue(reader.EOF);
        }

        private sealed class TestExportContext : ExportContext
        {
            private readonly Hashtable _exporters = new Hashtable();

            public override void Register(IExporter exporter)
            {
                _exporters[exporter.InputType] = exporter;
            }

            public override IExporter FindExporter(Type type)
            {
                return (IExporter) _exporters[type];
            }
        }

        private sealed class TestDataTableExporter : IExporter
        {
            public DataTable LastExported;

            public Type InputType
            {
                get { return typeof(DataTable); }
            }

            public void Export(ExportContext context, object value, JsonWriter writer)
            {
                LastExported = (DataTable) value;
                (new DataTableExporter(InputType)).Export(context, value, writer);
            }
        }
    }
}