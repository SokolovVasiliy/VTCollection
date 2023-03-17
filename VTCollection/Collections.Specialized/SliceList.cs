// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections;
using System.Collections.Generic;

namespace VT.Collections.Specialized
{
    public class SliceList<T> : IReadOnlyList<T>
    {
#pragma warning disable 0693
        private class SliceEnumerable<T> : IEnumerator<T>
        {
            /// <summary>
            /// IReadOnly slice collection
            /// </summary>
            private SliceList<T> m_slice_list = null;
            /// <summary>
            /// Current index
            /// </summary>
            private int m_curr_index = 0;
            /// <summary>
            /// Current as object
            /// </summary>
            object IEnumerator.Current => Current;
            /// <summary>
            /// Current T
            /// </summary>
            public T Current => m_slice_list.m_orig_list[m_curr_index];
            public void Reset() => m_curr_index = m_slice_list.m_begin_index - 1;
            /// <summary>
            /// Dispose
            /// </summary>
            public void Dispose() { }
            /// <summary>
            /// Seek next element.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                m_curr_index++;
                if (m_curr_index > m_slice_list.m_end_index)
                    return false;
                return true;
            }
            /// <summary>
            /// Need create enumeration with slice list
            /// </summary>
            /// <param name="list"></param>
            public SliceEnumerable(SliceList<T> list)
            {
                m_slice_list = list;
                m_curr_index = list.m_begin_index - 1;
            }
        }
        /// <summary>
        /// Original IReadOnlyList
        /// </summary>
        private IReadOnlyList<T> m_orig_list = null;
        /// <summary>
        /// Begin index
        /// </summary>
        private int m_begin_index = 0;
        /// <summary>
        /// End index
        /// </summary>
        private int m_end_index = 0;
        /// <summary>
        /// Create new Slice IReadOnlyList based on original list with begin and end indexes
        /// </summary>
        /// <param name="original_list"></param>
        /// <param name="begin_index"></param>
        /// <param name="end_index"></param>
        public SliceList(IReadOnlyList<T> original_list, int begin_index, int end_index)
        {
            m_orig_list = original_list ?? throw new NullReferenceException("Parameter" + nameof(original_list) + " is null");
            if (begin_index < 0)
                throw new Exception("Parameters " + nameof(begin_index) + " and " + nameof(end_index) + " must be non negative");
            if (end_index >= original_list.Count)
                throw new Exception("Parameters " + nameof(end_index) + " must be less count of " + nameof(original_list) + " (" + original_list.Count + ")");
            if (end_index < begin_index)
                throw new Exception("Parameter " + nameof(end_index) + " must be more " + nameof(begin_index));
            m_begin_index = begin_index;
            m_end_index = end_index;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException("Value of " + nameof(index) + " must be not negative");
                if (m_begin_index + index > m_end_index)
                    throw new ArgumentOutOfRangeException("Value of " + nameof(index) + " must be less " + Count);
                return m_orig_list[m_begin_index + index];
            }
        }

        /// <summary>
        /// Count of IReadOnlyListCollection
        /// </summary>
        public int Count => m_end_index - m_begin_index + 1;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<T> GetEnumerator() => new SliceEnumerable<T>(this);

    }
}
