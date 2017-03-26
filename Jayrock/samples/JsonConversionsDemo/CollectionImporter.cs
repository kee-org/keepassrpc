namespace JsonConversionsDemo
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Jayrock.Json;
    using Jayrock.Json.Conversion;

    #endregion

    /// <summary>
    /// An importer for importing a collection of elements from a JSON array.
    /// </summary>

    public class CollectionImporter<Collection, Element> : CollectionImporterBase
        where Collection : ICollection<Element>, new()
    {
        public CollectionImporter() :
            base(typeof(Collection), typeof(Element)) { }

        protected override object CreateCollection()
        {
            return new Collection();
        }

        protected override void ImportElements(object collection, ImportContext context, JsonReader reader)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (context == null) throw new ArgumentNullException("context");
            if (reader == null) throw new ArgumentNullException("reader");

            ImportElements((ICollection<Element>) collection, context, reader);
        }

        private static void ImportElements(ICollection<Element> collection, ImportContext context, JsonReader reader)
        {
            Debug.Assert(collection != null);
            Debug.Assert(context != null);
            Debug.Assert(reader != null);

            while (reader.TokenClass != JsonTokenClass.EndArray)
                collection.Add((Element) context.Import(typeof(Element), reader));
        }
    }
}