#region License, Terms and Conditions
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (www.raboof.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
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
    using System.Diagnostics;

    #endregion
    
    public sealed class DataTableExporter : ExporterBase
    {
        public DataTableExporter() :
            this(typeof(DataTable)) {}

        public DataTableExporter(Type inputType) : 
            base(inputType) {}

        protected override void ExportValue(ExportContext context, object value, JsonWriter writer)
        {
            Debug.Assert(context != null);
            Debug.Assert(value != null);
            Debug.Assert(writer != null);

            ExportTable(context, (DataTable) value, writer);
        }

        internal static void ExportTable(ExportContext context, DataTable table, JsonWriter writer)
        {
            Debug.Assert(context != null);
            Debug.Assert(table != null);
            Debug.Assert(writer != null);

            DataView view = table.DefaultView;

            //
            // If there is an exporter (perhaps an override) for the 
            // DataView in effect then use it. Otherwise our 
            // DataViewExporter.
            //

            IExporter exporter = context.FindExporter(view.GetType());
            
            if (exporter != null)
                exporter.Export(context, view, writer);
            else
                DataViewExporter.ExportView(context, view, writer);
       }
    }
}
