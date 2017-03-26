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

#if !NET_1_0 && !NET_1_1 && !NET_2_0

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;

    #endregion

    public class ExpandoObjectExporter : ExporterBase
    {
        public ExpandoObjectExporter() :
            base(typeof(ExpandoObject)) { }

        protected override void ExportValue(ExportContext context, object value, JsonWriter writer)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (writer == null) throw new ArgumentNullException("writer");

            writer.WriteStartObject();
            ExportMembers(context, (ExpandoObject) value, writer);
            writer.WriteEndObject();
        }

        private static void ExportMembers(ExportContext context, IEnumerable<KeyValuePair<string, object>> members, JsonWriter writer)
        {
            Debug.Assert(context != null);
            Debug.Assert(members != null);
            Debug.Assert(writer != null);

            foreach (var member in members)
            {
                writer.WriteMember(member.Key);
                context.Export(member.Value, writer);
            }
        }
    }
}

#endif // !NET_1_0 && !NET_1_1 && !NET_2_0
