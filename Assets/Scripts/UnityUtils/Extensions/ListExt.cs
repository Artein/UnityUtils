using System;
using System.Collections.Generic;

namespace UnityUtils.Extensions
{
    public static class ListExt
    {
        public static void Reverse_NoHeapAlloc<T>(this IList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            
            int count = list.Count;

            for (int i = 0; i < count / 2; i++)
            {
                var j = count - i - 1;
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static List<T> EnsureSize<T>(this List<T> list, int size, T value = default)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            int count = list.Count;
            if (count < size)
            {
                int capacity = list.Capacity;
                if (capacity < size)
                {
                    list.Capacity = Math.Max(size, capacity * 2);
                }

                while (count < size)
                {
                    list.Add(value);
                    ++count;
                }
            }

            return list;
        }
    }
}