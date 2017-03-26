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
    using System.Collections;
    #if !NET_1_0 && !NET_1_1
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    #endif

    #endregion

    public class ListImporter : ImportAwareImporter
    {
        public ListImporter() : 
            base(typeof(IList)) {}

        protected override IJsonImportable CreateObject()
        {
            return new JsonArray();
        }
    }

    #if !NET_1_0 && !NET_1_1 

    internal class CollectionImporter<TCollection, TOutput, TItem> : ImporterBase 
        where TCollection : ICollection<TItem>, new()
        where TOutput : IEnumerable
    {
        private readonly bool _isOutputReadOnly;

        public CollectionImporter() : 
            this(false) {}

        public CollectionImporter(bool isOutputReadOnly) : 
            base(typeof(TOutput))
        {
            _isOutputReadOnly = isOutputReadOnly;
        }

        public bool IsOutputReadOnly { get { return _isOutputReadOnly; } }

        protected override object ImportFromArray(ImportContext context, JsonReader reader)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (reader == null) throw new ArgumentNullException("reader");

            reader.Read();
            TCollection collection = new TCollection();

            while (reader.TokenClass != JsonTokenClass.EndArray)
                collection.Add(context.Import<TItem>(reader));

            object result = IsOutputReadOnly
                          ? (object) new ReadOnlyCollection<TItem>((IList<TItem>) collection)
                          : collection;

            return ReadReturning(reader, result);
        } 
    }

    internal class CollectionImporter<TOutput, TItem> : 
        CollectionImporter<List<TItem>, TOutput, TItem>
        where TOutput : IEnumerable
    {
        public CollectionImporter() {}

        public CollectionImporter(bool isOututReadOnly) : 
            base(isOututReadOnly) {}
    }

    #endif // !NET_1_0 && !NET_1_1 
}