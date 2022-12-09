using System.Collections.Generic;

namespace UnityUtils.Extensions
{
    public static class ListExt
    {
        public static void Reverse_NoHeapAlloc<T>(this IList<T> list)
        {
            int count = list.Count;

            for (int i = 0; i < count / 2; i++)
            {
                var j = count - i - 1;
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}