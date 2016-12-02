using System;
using System.Collections.Generic;

namespace Market
{
    public static class ListExtension
    {
        public static IList<T> GetFrontPartial<T>(this IList<T> list, int count)
        {
            if (count > list.Count)
                throw new ArgumentException(string.Format("List does not have {0} items.", count));
            IList<T> partial = new List<T>(count);
            int i = 0;
            foreach (var t in list)
            {
                partial.Add(t);
                i++;
                if (i == count)
                    break;
            }
            return partial;
        }

        public static IList<T> GetRearPartial<T>(this IList<T> list, int count)
        {
            if (count > list.Count)
                throw new ArgumentException(string.Format("List does not have {0} items.", count));
            IList<T> partial = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                partial.Add(list[list.Count - count + i]);
            }
            return partial;
        }

        public static IList<double> GetData<T>(this IList<T> list, Func<T, double> func)
        {
            IList<double> data = new List<double>(list.Count);
            foreach (T t in list)
            {
                data.Add(func(t));
            }
            return data;
        }
    }
}