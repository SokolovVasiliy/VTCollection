using System;
using System.Collections.Generic;
using System.Text;

namespace MV.Collections.Specialized
{
    public static class BinarySearch
    {
        public static void InsertSort<T>(this List<T> list, T item)
        {
            Comparer<T> compare = Comparer<T>.Default;
            InsertSort(list, item, compare);
        }
        public static void InsertSort<T>(this List<T> list, T item, Comparison<T> comparison)
        {
            Comparer<T> compare = Comparer<T>.Create(comparison);
            InsertSort(list, item, compare);
        }
        public static void InsertSort<T>(this List<T> list, T item, Comparer<T> comparer)
        {
            int find_index = list.BinarySearch(item, comparer);
            int insert_index = find_index;
            if (find_index < 0)
                insert_index = ~find_index;
            list.Insert(insert_index, item);
        }
    }
}
