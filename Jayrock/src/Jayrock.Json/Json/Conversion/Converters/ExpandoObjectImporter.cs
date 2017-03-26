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
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Dynamic;

    #endregion

    public class ExpandoObjectImporter : ImporterBase
    {
        public ExpandoObjectImporter() :
            base(typeof(ExpandoObject)) { }

        protected override object ImportFromObject(ImportContext context, JsonReader reader)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (reader == null) throw new ArgumentNullException("reader");

            reader.ReadToken(JsonTokenClass.Object);
            var expando = (IDictionary<string, object>) new ExpandoObject();
            while (reader.TokenClass != JsonTokenClass.EndObject)
                expando[reader.ReadMember()] = ImportValue(context, reader);
            reader.Read();
            return expando;
        }

        private object ImportValue(ImportContext context, JsonReader reader)
        {
            Debug.Assert(context != null);
            Debug.Assert(reader != null);

            return reader.TokenClass == JsonTokenClass.Object
                 ? ImportFromObject(context, reader)
                 : reader.TokenClass == JsonTokenClass.Array
                 ? ImportArray(context, reader)
                 : context.Import(AnyType.Value, reader);
        }

        private object ImportArray(ImportContext context, JsonReader reader)
        {
            Debug.Assert(context != null);
            Debug.Assert(reader != null);

            reader.ReadToken(JsonTokenClass.Array);
            var list = new List<object>();
            while (reader.TokenClass != JsonTokenClass.EndArray)
                list.Add(ImportValue(context, reader));
            reader.Read();
            return new ReadOnlyCollection<object>(list);
        }
    }
}

#endif // !NET_1_0 && !NET_1_1 && !NET_2_0
