namespace System.Collections.Generic
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using Cinchoo.Core;

    #endregion NameSpaces

    public static class ChoListEx
    {
        public static T[] ToNArray<T>(this IList<T> list)
        {
            if (list == null) return new T[] { };
            return list.ToArray();
        }

        public static void MoveToNext<T>(this IList<T> list, T item)
        {
            ChoGuard.ArgumentNotNull(list, "List");
            ChoGuard.ArgumentNotNull(item, "Item");

            int index = list.IndexOf(item);
            if (index == -1) return;

            if (index == list.Count - 1)
                return;
            else
            {
                list.RemoveAt(index);
                list.Insert(index + 1, item);
            }
        }

        public static void MoveToPrevious<T>(this IList<T> list, T item)
        {
            ChoGuard.ArgumentNotNull(list, "List");
            ChoGuard.ArgumentNotNull(item, "Item");

            int index = list.IndexOf(item);
            if (index == -1) return;

            if (index == 0)
                return;
            else
            {
                list.RemoveAt(index);
                list.Insert(index - 1, item);
            }
        }

        public static void MoveItem<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            ChoGuard.ArgumentNotNull(list, "List");
            if (oldIndex < 0 || oldIndex >= list.Count)
                throw new ArgumentException("oldIndex is out of range.");
            if (newIndex < 0 || newIndex >= list.Count)
                throw new ArgumentException("newIndex is out of range.");

            T item = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
        }

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
