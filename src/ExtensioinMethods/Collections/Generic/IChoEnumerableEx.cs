using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinchoo.Core.Collections;

namespace Cinchoo.Core
{
    public static class IChoEnumerableEx
    {
        public static IEnumerable<T> Repeat<T>(T value)
        {
            while (true)
                yield return value;
        }

        public static IEnumerable<T> Repeat<T>(IEnumerable<T> source)
        {
            while (true)
                foreach (var item in source)
                    yield return item;
        }

        public static ChoMemoizeEnumerable<T> Memoize<T>(this IEnumerable<T> source)
        {
            return new ChoMemoizeEnumerable<T>(source);
        }

        //public static ChoMemoizeEnumerable<ChoMemoizeEnumerable<T>> Memoize<T>(this IEnumerable<IEnumerable<T>> source)
        //{
        //    foreach (IEnumerable<T> list in source)
        //        yield return new ChoMemoizeEnumerable<T>(list);
        //}

        public static IEnumerable<U> Let<T, U>(this IEnumerable<T> source, Func<IEnumerable<T>, IEnumerable<U>> function)
        {
            using (var mem = new ChoMemoizeEnumerable<T>(source))
            {
                foreach (var item in function(mem))
                    yield return item;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null || action == null) return;

            foreach (T item in source)
                action(item);
        }

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

        public static IEnumerable<T> TryForEach<T>(this IEnumerable<T> list, Func<Exception, bool> executeCatch)
        {
            if (list == null) yield break;

            IEnumerator<T> enumerator = list.GetEnumerator();
            bool success = false;

            do
            {
                try
                {
                    success = enumerator.MoveNext();
                }
                catch (Exception ex)
                {
                    success = executeCatch(ex);
                    if (success)
                        continue;
                }

                if (success)
                {
                    T item = enumerator.Current;
                    yield return item;
                }
            } while (success);
        }
    }
}
