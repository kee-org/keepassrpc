namespace JsonConversionsDemo
{
    #region Imports

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// Imports <see cref="List{T}"/> from a JSON array.
    /// </summary>
    
    public class ListImporter<T> : CollectionImporter<List<T>, T>
        where T : new() { }
}