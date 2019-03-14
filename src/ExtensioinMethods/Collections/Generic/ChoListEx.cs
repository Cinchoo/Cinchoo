namespace System.Collections.Generic
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public static class ChoListEx
    {
        public static void RemoveAll<T>(this IList<T> list, IEnumerable<T> itemsToBeRemoved)
        {
			if (list == null)
				throw new ArgumentNullException("list");
            
			if (itemsToBeRemoved == null) return;

            foreach (T item in itemsToBeRemoved)
            {
                if (!list.Contains(item)) continue;
                list.Remove(item);
            }
        }

		public static void CheckIndex<T>(this IList<T> list, int index)
		{
			if (list == null)
				throw new ArgumentNullException("list");

			if (index == 0 && list.Count == 0)
				return;

			if (index < 0 || (uint)index >= (uint)list.Count)
				throw new ArgumentOutOfRangeException("index");
		}

		public static void CheckIndex(this IList list, int index)
		{
			if (list == null)
				throw new ArgumentNullException("list");

			if (index == 0 && list.Count == 0)
				return;

			if (index < 0 || (uint)index >= (uint)list.Count)
				throw new ArgumentOutOfRangeException("index");
		}

		public static void CheckRange<T>(this IList<T> list, int idx, int count)
		{
			if (list == null)
				throw new ArgumentNullException("list");

			int size = list.Count;

			if (idx < 0)
				throw new ArgumentOutOfRangeException("index");

			if (count < 0)
				throw new ArgumentOutOfRangeException("count");

			if ((uint)idx + (uint)count > (uint)size)
				throw new ArgumentException("index and count exceed length of list");
		}

		public static void CheckRange(this IList list, int idx, int count)
		{
			if (list == null)
				throw new ArgumentNullException("list");

			int size = list.Count;

			if (idx < 0)
				throw new ArgumentOutOfRangeException("index");

			if (count < 0)
				throw new ArgumentOutOfRangeException("count");

			if ((uint)idx + (uint)count > (uint)size)
				throw new ArgumentException("index and count exceed length of list");
		}
	}
}
