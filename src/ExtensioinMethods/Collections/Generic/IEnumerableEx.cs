using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class IEnumerableEx
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source.Where((x, i) => i % chunkSize == 0).Select((x, i) => source.Skip(i * chunkSize).Take(chunkSize));
        }

        public static List<List<T>> Loop<T>(this IEnumerable<IEnumerable<T>> lists)
        {
            List<List<T>> ret = new List<List<T>>();
            if (lists != null)
            {
                //Iterate
                foreach (IEnumerable<T> taskResult in lists)
                {
                    ret.Add(new List<T>(taskResult));
                }
            }
            return ret;
        }

        public static List<T> Loop<T>(this IEnumerable<T> list)
        {
            if (list != null)
            {
                return new List<T>(list);
                ////Iterate
                //foreach (T obj in list)
                //{
                //}
            }
            else
                return new List<T>();
        }
    }
}
