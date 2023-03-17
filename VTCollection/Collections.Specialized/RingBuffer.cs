// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections;
using System.Collections.Generic;

namespace VT.Collections.Specialized
{
    /// <summary>
    /// Provides a fixed-size sliding window
    /// </summary>
    public partial class RingBuffer<T> : IEnumerable,
                                            IEnumerable<T>,
                                            ICollection,
                                            ICollection<T>,
                                            IList,
                                            IList<T>,
                                            IReadOnlyCollection<T>,
                                            IReadOnlyList<T>
    {
        #region Private members
        /// <summary>
        /// List of elements
        /// </summary>
        private List<T> m_items;
        /// <summary>
        /// Point on head of list
        /// </summary>
        private int m_head;
        /// <summary>
        /// Limit of count elements
        /// </summary>
        private int m_capacity;
        /// <summary>
        /// Used for 'EventIsCompleted'. True if need run this event, otherwise false
        /// </summary>
        private bool m_is_complete_flag = false;
        /// <summary>
        /// Enumerator
        /// </summary>
        private RingBufferEnumerator<T> m_enumerator;
        #endregion
        #region Private methods
        /// <summary>
        /// Before the regularization: {5, 6, head->2, 3, 4}, after: {2, 3, 4, 5, head -> 6}
        /// (Danger: it work slowly)
        /// </summary>
        private void Regularization()
        {
            //-- Regularization complete if elements is order
            if (m_head == m_items.Count - 1)
                return;
            T[] temp_array = new T[m_head + 1];
            m_items.CopyTo(0, temp_array, 0, m_head + 1);
            int dst_index = 0;
            for (int i = m_head + 1; i < m_items.Count; i++, dst_index++)
                m_items[dst_index] = m_items[i];
            for (int i = dst_index, k = 0; k < temp_array.Length; i++, k++)
                m_items[i] = temp_array[k];
            m_head = m_items.Count - 1;
        }
        /// <summary>
        /// Convert virtual index to real index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int ToRealIndex(int index)
        {
            index = Reverse(index);
            if (index >= m_items.Count)
                throw new IndexOutOfRangeException();
            int max_count = m_items.Count;
            if (max_count < m_capacity)
                return index;
            int delta = (max_count - 1) - m_head;
            if (index < delta)
                return max_count + (index - delta);
            return index - delta;
        }
        /// <summary>
        /// Convert real index to virtual index
        /// </summary>
        /// <param name="index">virtual index</param>
        /// <returns>real index</returns>
        private int ToVirtualIndex(int index)
        {
            int virt_index = 0;
            if (index >= m_head)
                virt_index = index - m_head - 1;
            else
                virt_index = m_items.Count - m_head + index - 1;
            return Reverse(virt_index);
        }
        /// <summary>
        /// Revers index if RevertedIndex flag enable
        /// </summary>
        /// <param name="index">Virtual index of collection</param>
        /// <returns></returns>
        private int Reverse(int index)
        {
            if (!RevertedIndex)
                return index;
            return m_items.Count - 1 - index;
        }
        /// <summary>
        /// Return count of treem elements
        /// </summary>
        /// <param name="n_capacity">new value of capacity</param>
        /// <returns></returns>
        private int TrimElementsCount(int n_capacity)
        {
            if (n_capacity == m_capacity)
                return 0;
            if (n_capacity > m_capacity)
                return 0;
            if (n_capacity > m_items.Count)
                return 0;
            int count_trim = m_items.Count - n_capacity;
            return count_trim;
        }
        #endregion
        #region Events
        /// <summary>
        /// This delegate is runing, occur adding removing or inserting of element 
        /// </summary>
        /// <param name="index">index of element</param>
        /// <param name="insert_item">Remove or insert element</param>
        public delegate void FifoHendler(int index, T item);
        /// <summary>
        /// This delegate is runing, when RingBuffer is complete (Count == Capacity)
        /// </summary>
        public delegate void FifoIsComplete();
        /// <summary>
        /// Run when new element insert at collection
        /// </summary>
        public event FifoHendler EventInsert;
        /// <summary>
        /// Run when old element remove from collection
        /// </summary>
        public event FifoHendler EventRemove;
        /// <summary>
        /// This event run, when RingBuffer is complete (Count == Capacity)
        /// </summary>
        public event FifoIsComplete EventIsComplete;
        #endregion
        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public RingBuffer() : this(int.MaxValue)
        {
        }
        /// <summary>
        /// Capacity set
        /// </summary>
        /// <param name="capacity"></param>
        public RingBuffer(int capacity)
        {
            m_items = new List<T>();
            m_enumerator.Init(this);
            m_head = -1;
            Capacity = capacity;
        }
        /// <summary>
        /// Init RingBuffer from collection. Property capacity will be equal count of collection
        /// </summary>
        /// <param name="collection">Collection</param>
        public RingBuffer(ICollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (collection.Count == 0)
                throw new Exception("Collection will not must be empty");
            m_items = new List<T>(collection);
            m_enumerator.Init(this);
            m_head = collection.Count - 1;
            Capacity = collection.Count;
        }
        #endregion
        #region State methods
        /// <summary>
        /// Count of elements
        /// </summary>
        public int Count
        {
            get { return m_items.Count; }
        }
        /// <summary>
        /// Set/Get capacity.
        /// </summary>
        public int Capacity
        {
            get { return m_capacity; }
            set
            {
                //if()
                int trim_el = TrimElementsCount(value);
                if (value != m_capacity)
                    Regularization();
                if (trim_el == 0)
                {
                    m_capacity = value;
                    return;
                }
                m_items.RemoveRange(0, trim_el);
                m_head = m_items.Count - 1;
                m_capacity = value;
            }
        }
        /// <summary>
        /// Return true if element 'item' exists in collection, otherwise return false
        /// </summary>
        /// <param name="item">element for finding</param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return m_items.Contains(item);
        }
        /// <summary>
        /// Return true if colletion has fixed size, otherwise return false
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return m_capacity == m_items.Count;
            }
        }
        /// <summary>
        /// By default reverse is false. Set true, if you need reverse index.
        /// For example, if this flag set operation 'this[0]' will return element by index Count-1
        /// </summary>
        public bool RevertedIndex
        {
            get; set;
        }
        #endregion
        #region CopyTo methods
        /// <summary>
        /// Copy to array [T]
        /// </summary>
        /// <param name="array"></param>
        /// <param name="begin_index"></param>
        public void CopyTo(T[] array, int begin_index)
        {
            int count = m_items.Count - m_head - 1;
            m_items.CopyTo(m_head + 1, array, begin_index, count);
            m_items.CopyTo(0, array, begin_index + count, m_head + 1);
        }
        /// <summary>
        /// Copy elements into an array
        /// </summary>
        /// <param name="src_index">Source index of ring buffer</param>
        /// <param name="array">Destination array</param>
        /// <param name="dst_index">Destination index</param>
        /// <param name="count">Count copy element</param>
        public void CopyTo(int src_index, T[] array, int dst_index, int count)
        {
            //-- Copy right part of ring buffer
            int begin_index = ToRealIndex(src_index);
            int min_count = Math.Min(count, m_items.Count - begin_index);
            m_items.CopyTo(begin_index, array, dst_index, min_count);
            //-- Copy left part of ring buffer
            int left_count = count - (m_items.Count - begin_index);
            if (left_count > 0)
                m_items.CopyTo(0, array, dst_index + min_count, left_count);
        }
        /// <summary>
        /// Copy to objects array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="begin_index"></param>
        public void CopyTo(Array array, int begin_index)
        {
            for (int i = m_head + 1; i < m_items.Count; i++)
                array.SetValue(m_items[i], i + begin_index);
            for (int i = 0; i < m_head; i++)
                array.SetValue(m_items[i], i + begin_index);
        }
        #endregion
        #region Syncronized state
        /// <summary>
        /// It is modify collection
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        /// <summary>
        /// It is not synchronized collection
        /// </summary>
        public bool IsSynchronized
        {
            get { return false; }
        }
        /// <summary>
        /// 
        /// </summary>
        public object SyncRoot
        {
            get { return this; }
        }
        #endregion
        #region Add/Insert/Remove/Clear methods
        #region IList
        /// <summary>
        /// Index for IList
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object IList.this[int index]
        {
            get
            {
                return m_items[ToRealIndex(index)];
            }
            set
            {
                m_items[ToRealIndex(index)] = (T)value;
            }
        }

        /// <summary>
        /// Remove for IList
        /// </summary>
        /// <param name="value"></param>
        void IList.Remove(object value)
        {
            Remove((T)value);
        }
        /// <summary>
        /// Add object
        /// </summary>
        /// <param name="obj"></param>
        int IList.Add(object obj)
        {
            Add((T)obj);
            return m_items.Count;
        }
        /// <summary>
        /// Insert for IList
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }
        /// <summary>
        /// Contains for IList
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool IList.Contains(object obj)
        {
            return m_items.Contains((T)obj);
        }
        /// <summary>
        /// IndexOf for IList
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }
        #endregion
        #region List<T>
        /// <summary>
        /// Remove element 'item' from collection
        /// </summary>
        /// <param name="item"></param>
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index == -1)
                return false;
            RemoveAt(index);
            return true;
        }
        /// <summary>
        /// Remove element by index 
        /// </summary>
        /// <param name="index">Index of element</param>
        public void RemoveAt(int index)
        {
            Regularization();
            int rev_ind = Reverse(index);
            EventRemove?.Invoke(index, m_items[rev_ind]);
            m_items.RemoveAt(rev_ind);
            m_head--;
        }
        /// <summary>
        /// Insert element in position of collection
        /// </summary>
        /// <param name="index">index of position</param>
        /// <param name="item">insert element</param>
        public void Insert(int index, T item)
        {
            Regularization();
            bool fix = IsFixedSize;
            int rev_i = RevertedIndex ? 1 : 0;
            m_items.Insert(Reverse(index) + rev_i, item);
            EventInsert?.Invoke(index, item);
            m_head++;
            if (fix)
                m_capacity++;
        }
        /// <summary>
        /// Add new element in collection
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            m_head++;
            if (m_items.Count < m_capacity)
            {
                m_is_complete_flag = false;
                m_items.Add(item);
                EventInsert?.Invoke(Reverse(m_head), item);
                return;
            }
            //-- Run an event only once
            else if (!m_is_complete_flag)
            {
                EventIsComplete?.Invoke();
                m_is_complete_flag = true;
            }
            if (m_head == m_capacity)
                m_head = 0;
            //T el_old = 
            EventRemove?.Invoke(Reverse(0), m_items[m_head]);
            m_items[m_head] = item;
            EventInsert?.Invoke(Reverse(m_items.Count - 1), item);
        }
        /// <summary>
        /// Item[]
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                return m_items[ToRealIndex(index)];
            }
            set
            {
                m_items[ToRealIndex(index)] = value;
            }
        }
        /// <summary>
        /// Return index of element 'item'. Return -1 if element not find
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            int index = m_items.IndexOf(item);
            if (index == -1)
                return -1;
            return ToVirtualIndex(index);
        }
        /// <summary>
        /// Return last value
        /// </summary>
        public T Last
        {
            get
            {
                if (RevertedIndex)
                    return this[0];
                else
                    return this[Count - 1];
            }
        }
        public T LastOrDefault
        {
            get
            {
                if (Count == 0)
                    return default(T);
                return Last;
            }
        }
        /// <summary>
        /// Return first value
        /// </summary>
        public T First
        {
            get
            {
                if (RevertedIndex)
                    return this[Count - 1];
                else
                    return this[0];
            }
        }
        #endregion
        /// <summary>
        /// Clear all elements
        /// </summary>
        public void Clear()
        {
            m_items.Clear();
            m_items = new List<T>();
            m_enumerator.Init(this);
            m_head = -1;
        }
        #endregion
        #region IEnumerable mehtods
        /// <summary>
        /// Return enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return m_enumerator;
        }
        /// <summary>
        /// Return enumerator for non-generic
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_enumerator;
        }
        /* Type <T> of  RingBufferEnumerator always equal type <T> of RingBuffer */
#pragma warning disable 0693
        /// <summary>
        /// Enumerator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private struct RingBufferEnumerator<T> : IDisposable, IEnumerator<T>, IEnumerator
        {
            private RingBuffer<T> m_this_ring_buffer;
            private int m_pos;
            /// <summary>
            /// Reset enumerator
            /// </summary>
            public void Reset()
            {
                m_pos = -1;
            }
            /// <summary>
            /// Init enumerator
            /// </summary>
            /// <param name="rbuff"></param>
            public void Init(RingBuffer<T> rbuff)
            {
                m_this_ring_buffer = rbuff;
                m_pos = -1;
            }
            /// <summary>
            /// Return current element
            /// </summary>
            public T Current
            {
                get
                {
                    return m_this_ring_buffer[m_pos];
                }
            }
            /// <summary>
            /// Current for IEnumerator
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return m_this_ring_buffer[m_pos];
                }
            }
            /// <summary>
            /// Get next element
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                return ++m_pos < m_this_ring_buffer.Count;
            }
            /// <summary>
            /// There are not managment resource
            /// </summary>
            public void Dispose()
            {
            }
        }
#pragma warning restore
        #endregion
    }
}
