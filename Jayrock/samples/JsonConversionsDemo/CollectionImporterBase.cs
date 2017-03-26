namespace JsonConversionsDemo
{
    #region Imports

    using System;
    using Jayrock.Json;
    using Jayrock.Json.Conversion;
    using Jayrock.Json.Conversion.Converters;

    #endregion

    /// <summary>
    /// An abstract base class for importer implementations that can import
    /// a concrete collection instance from a JSON array.
    /// </summary>

    public abstract class CollectionImporterBase : ImporterBase
    {
        private readonly Type _elementType;

        public CollectionImporterBase(Type outputType, Type elementType) : 
            base(outputType)
        {
            if (elementType == null) throw new ArgumentNullException("elementType");
            
            _elementType = elementType;
        }

        public Type ElementType
        {
            get { return _elementType; }
        }

        protected override object ImportFromArray(ImportContext context, JsonReader reader)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (reader == null) throw new ArgumentNullException("reader");

            object collection = CreateCollection();

            reader.ReadToken(JsonTokenClass.Array);

            ImportElements(collection, context, reader);

            if (reader.TokenClass != JsonTokenClass.EndArray)
                throw new Exception("Implementation error.");

            reader.Read();
            return collection;
        }

        protected abstract object CreateCollection();
        protected abstract void ImportElements(object collection, ImportContext context, JsonReader reader);
    }
}