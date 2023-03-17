// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using MV;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Concurrent;

namespace VT.Collections.Specialized
{
    public struct AsKey<TKey>
    {
        public TKey Value { get; set; }
    }
    public static class to
    {
        public static AsKey<TKey> key<TKey>(TKey key)
        {
            AsKey<TKey> askey = new AsKey<TKey>();
            askey.Value = key;
            return askey;
        }
    }
    
    /// <inheritdoc/>
    [Serializable]
    public class ReadOnlyListWithKeys<TKey, TValue> : IReadOnlyListWithKeys<TKey, TValue>
    {
        /// <summary>
        /// Values by indexes
        /// </summary>
        protected readonly List<TValue> m_list_values;

        /// <summary>
        /// Values by keys
        /// </summary>
        private readonly ConcurrentDictionary<TKey, TValue> m_values;

        /// <summary>
        /// Fast access to key by index
        /// </summary>
        private readonly ConcurrentDictionary<int, TKey> m_key_by_ind;

        /// <summary>
        /// Fast access to key by index
        /// </summary>
        private readonly ConcurrentDictionary<TKey, int> m_ind_by_key;

        /// <inheritdoc/>
        public bool Contein(TKey key)
        {
            return m_values.ContainsKey(key);
        }

        /// <inheritdoc/>
        public bool Contein(AsKey<TKey> key)
        {
            return m_values.ContainsKey(key.Value);
        }

        /// <inheritdoc/>
        public bool TryGetValueByKey(TKey key, out TValue val)
        {
            return m_values.TryGetValue(key, out val);
        }

        /// <inheritdoc/>
        public TValue this[int index]
        {
            get => m_list_values[index];
        }
        
        /// <inheritdoc/>
        public virtual TValue this[TKey key]
        {
            get => m_values[key];
        }
        public TValue this[AsKey<TKey> key]
        {
            get => m_values[key.Value];
        }
        public TValue this[ValueTuple<TKey> key]
        {
            get => m_values[key.Item1];
        }
        
        /// <inheritdoc/>
        public int Count => m_list_values.Count;

        /// <inheritdoc/>
        public IEnumerator<TValue> GetEnumerator() => m_list_values.GetEnumerator();
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => m_list_values.GetEnumerator();

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
            m_list_values.Add(value);
            int c = m_list_values.Count - 1;
            m_key_by_ind.TryAdd(c, key);
            m_ind_by_key.TryAdd(key, c);
        }
        protected void Remove(TKey key)
        {
            int ind = GetIndexByKey(key);
            m_list_values.RemoveAt(ind);
            m_values.TryRemove(key, out TValue _);
            m_ind_by_key.TryRemove(key, out int _);
            //m_key_by_ind.Remove(ind);
            m_key_by_ind.Clear();
            int i = 0;
            foreach(var kvp in m_values)
            {
                m_key_by_ind.TryAdd(i, kvp.Key);
                i++;
            }
        }
        /// <inheritdoc/>
        public TKey GetKeyByIndex(int index) => m_key_by_ind[index];
        /// <inheritdoc/>
        public int GetIndexByKey(TKey key) => m_ind_by_key[key];
        /// <summary>
        /// Create new collection with keys collection.
        /// </summary>
        public ReadOnlyListWithKeys()
        {
            m_list_values = new List<TValue>();
            m_values = new ConcurrentDictionary<TKey, TValue>();
            m_key_by_ind = new ConcurrentDictionary<int, TKey>();
            m_ind_by_key = new ConcurrentDictionary<TKey, int>();
        }
        /// <summary>
        /// Create new list with keys collection with indication of capacity
        /// </summary>
        /// <param name="capacity">Approximate number of items in the collection</param>
        public ReadOnlyListWithKeys(int capacity)
        {
            m_list_values = new List<TValue>(capacity);
            m_values = new ConcurrentDictionary<TKey, TValue>(4, capacity);
            m_key_by_ind = new ConcurrentDictionary<int, TKey>(4, capacity);
            m_ind_by_key = new ConcurrentDictionary<TKey, int>(4, capacity);
        }
    }
}
