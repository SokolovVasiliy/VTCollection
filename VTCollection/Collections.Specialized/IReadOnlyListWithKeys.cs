using System;
using System.Text;
using System.Collections.Generic;

namespace VT.Collections.Specialized
{
    /// <summary>
    /// Defined gibrid collection with access by key and index
    /// (complicated part begins when TKey is int as well as index)
    /// </summary>
    /// <typeparam name="TValue">Value</typeparam>
    /// <typeparam name="TKey">Key</typeparam>
    public interface IReadOnlyListWithKeys<TKey, TValue> : IReadOnlyList<TValue>
    {
        /// <summary>
        /// Get element by its key.
        /// </summary>
        /// <param name="key">Unique key of element</param>
        /// <returns>Element</returns>
        TValue this[TKey key] { get; }

        /// <summary>
        /// Return true, if element with KEY exists in the collection, otherwise returns false
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Contein(TKey key);

        /// <summary>
        /// Get key by its index.
        /// </summary>
        /// <param name="index">Index of element</param>
        /// <returns>Key of element</returns>
        TKey GetKeyByIndex(int index);

        /// <summary>
        /// Get index by its key.
        /// </summary>
        /// <param name="key">Key of element</param>
        /// <returns>Index of element</returns>
        int GetIndexByKey(TKey key);

        /// <summary>
        /// Return true, if element with KEY exists in the collection, otherwise returns false
        /// </summary>
        /// <param name="key">Key of element !BE VERY CAREFUL WHEN TKey IS INTEGER!</param>
        /// <param name="val">element, if found in the collection</param>
        /// <returns></returns>
        bool TryGetValueByKey(TKey key, out TValue val);
    }
}
