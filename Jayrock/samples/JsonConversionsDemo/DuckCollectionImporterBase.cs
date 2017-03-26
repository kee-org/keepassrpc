namespace JsonConversionsDemo
{
    #region Imports

    using System;
    using Jayrock.Json;
    using Jayrock.Json.Conversion;

    #endregion

    /// <summary>
    /// An base importer implementation capable of importing a duck-typed 
    /// collection of elements from a JSON array.
    /// </summary>

    public abstract class DuckCollectionImporterBase : CollectionImporterBase
    {
        public DuckCollectionImporterBase(Type outputType, Type elementType) : 
            base(outputType, elementType) {}
        
        protected override void ImportElements(object collection, ImportContext context, JsonReader reader) 
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (context == null) throw new ArgumentNullException("context");
            if (reader == null) throw new ArgumentNullException("reader");

            object[] args = null;
            while (reader.TokenClass != JsonTokenClass.EndArray)
            {
                if (args == null)           // on-demand
                    args = new object[1];

                args[0] = context.Import(ElementType, reader);
                InvokeAdd(collection, args);
            }
        }

        protected override object CreateCollection() 
        {
            return Activator.CreateInstance(OutputType);
        }

        /// <remarks>
        /// The <see cref="args"/> parameter is always an array of a single 
        /// element containing the value to add to the collection.
        /// </remarks>

        protected abstract void InvokeAdd(object collection, object[] args);
    }
}