namespace JsonConversionsDemo
{
    #region Imports

    using System;
    using System.Reflection;
    using Jayrock.Json;
    using Jayrock.Json.Conversion;

    #endregion

    /// <summary>
    /// An importer for importing a duck-typed collection of elements from a 
    /// JSON array.
    /// </summary>
    /// <remarks>
    /// The importer can infer the element type provided that
    /// the collection has an instance-base and public Add method that 
    /// takes a single argument of the element type.
    /// </remarks>

    public class DuckCollectionImporter : DuckCollectionImporterBase
    {
        private readonly MethodInfo _adder;

        public DuckCollectionImporter(Type outputType) :
            base(outputType, DuckCollectionReflector.InferElementType(outputType))
        {
            try
            {
                _adder = DuckCollectionReflector.FindAddMethod(outputType, ElementType);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(e.Message, "outputType", e);
            }
        }

        protected override void InvokeAdd(object collection, object[] args)
        {
            _adder.Invoke(collection, args);
        }
    }

    /// <summary>
    /// This is the generic version of <see cref="DuckCollectionImporter"/>.
    /// </summary>

    public sealed class DuckCollectionImporter<Collection, Element> : DuckCollectionImporterBase
        where Collection : new()
    {
        public DuckCollectionImporter() :
            base(typeof(Collection), typeof(Element)) { }

        protected override void ImportElements(object collection, ImportContext context, JsonReader reader)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (context == null) throw new ArgumentNullException("context");
            if (reader == null) throw new ArgumentNullException("reader");

            Action<Element> adder = DuckCollectionReflector.GetAdder<Element>(collection);

            while (reader.TokenClass != JsonTokenClass.EndArray)
                adder((Element) context.Import(typeof(Element), reader));
        }

        protected override object CreateCollection()
        {
            return new Collection();
        }

        protected override void InvokeAdd(object collection, object[] args)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (args == null) throw new ArgumentNullException("args");
            if (args.Length != 1) throw new ArgumentException(null, "args");

            //
            // NOTE! This implementation is horribly slow.
            // It is provided here only for sake of completeness but it 
            // should never be needed for any practical reason.
            //

            DuckCollectionReflector.GetAdder<Element>(collection)((Element) args[0]);
        }
    }
}