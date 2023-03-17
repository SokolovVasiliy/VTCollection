// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using MV;
using System;
using System.Diagnostics.CodeAnalysis;

namespace VT.Collections.Specialized
{
    /// <inheritdoc/>
    [Serializable]
    public class ListWithKeys<TKey, TValue> : ReadOnlyListWithKeys<TKey, TValue>
    {
        /// <summary>
        /// Add new value with unique key
        /// </summary>
        /// <param name="key">Unique key of this value</param>
        /// <param name="value">Adding item</param>
        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
        }
        public ListWithKeys() : base()
        {
        }
        /// <summary>
        /// Create new list with keys collection with indication of capacity
        /// </summary>
        /// <param name="capacity">Approximate number of items in the collection</param>
        public ListWithKeys(int capacity) : base(capacity)
        {
        }
    }
}
