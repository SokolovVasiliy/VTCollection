using System;
using System.Collections.Generic;

namespace VT
{
    public static class EnumarationExtension
    {
        /// <summary>
        /// Call function 'action' for each element in sequence (mapping).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seq">Enumerable sequence</param>
        /// <param name="action">Need action</param>
        public static void Map<T>(this IEnumerable<T> seq, Action<T> action)
        {
            if (seq == null)
                return;
            IReadOnlyList<T> list = seq as IReadOnlyList<T>;
            if (list != null)
                Map(list, action);
            else
            {
                foreach (var item in seq)
                    action(item);
            }
        }

        /// <summary>
        /// Call function 'action' for each element in sequence (mapping).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seq">Enumerable sequence</param>
        /// <param name="action">Need action</param>
        public static void Map<T>(this IReadOnlyList<T> seq, Action<T> action)
        {
            if (seq == null && seq.Count == 0)
                return;
            for(int i = 0; i < seq.Count; i++)
                action(seq[i]);
        }
    }
}
