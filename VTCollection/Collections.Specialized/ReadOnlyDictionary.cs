using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using MV;

namespace VT.Collections.Specialized
{
    public abstract class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        /// <summary>
        /// Values by keys
        /// </summary>
        private ConcurrentDictionary<TKey, TValue> m_values;
        public bool ContainsKey(TKey key) => m_values.ContainsKey(key);
        /// <inheritdoc/>
        public virtual TValue this[TKey key]
        {
            get => m_values[key];
        }
        public IEnumerable<TKey> Keys => m_values.Keys;
        //     An enumerable collection that contains the values in the read-only dictionary.

        public IEnumerable<TValue> Values => m_values.Values;

        public bool TryGetValue(TKey key, out TValue value) => m_values.TryGetValue(key, out value);
        /// <inheritdoc/>
        public int Count => m_values.Count;
        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => m_values.GetEnumerator();
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => m_values.GetEnumerator();
        /// <summary>
        /// Add new value with unique key
        /// </summary>
        /// <param name="key">Unique key of this value</param>
        /// <param name="value">Adding item</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(key));
            m_values.TryAdd(key, value);
        }

        protected void Clear()
        {
            m_values.Clear();
        }
        protected void Remove(TKey key)
        {
            m_values.TryRemove(key, out TValue _);
        }
        /// <summary>
        /// Create new collection with keys collection.
        /// </summary>
        public ReadOnlyDictionary()
        {
            m_values = new ConcurrentDictionary<TKey, TValue>();
        }
        /// <summary>
        /// Create new list with keys collection with indication of capacity
        /// </summary>
        /// <param name="capacity">Approximate number of items in the collection</param>
        public ReadOnlyDictionary(int capacity)
        {
            m_values = new ConcurrentDictionary<TKey, TValue>(3, capacity);
        }
    }
}
