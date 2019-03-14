namespace Cinchoo.Core.Collections.Generic
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;

    #endregion NameSpaces

    interface IChoList<T> : IList<T>
    {
        void AddRange(System.Collections.Generic.IEnumerable<T> collection);
        int BinarySearch(T item);
        int BinarySearch(T item, System.Collections.Generic.IComparer<T> comparer);
        int BinarySearch(int index, int count, T item, System.Collections.Generic.IComparer<T> comparer);
        //object Clone();
        List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter);
        void CopyTo(int index, T[] array, int arrayIndex, int count);
        void CopyTo(T[] array);
        bool Exists(Predicate<T> match);
        T Find(Predicate<T> match);
        List<T> FindAll(Predicate<T> match);
        int FindIndex(Predicate<T> match);
        int FindIndex(int startIndex, int count, Predicate<T> match);
        int FindIndex(int startIndex, Predicate<T> match);
        T FindLast(Predicate<T> match);
        int FindLastIndex(int startIndex, Predicate<T> match);
        int FindLastIndex(int startIndex, int count, Predicate<T> match);
        int FindLastIndex(Predicate<T> match);
        void ForEach(Action<T> action);
        List<T> GetRange(int index, int count);
        int IndexOf(T item, int index);
        int IndexOf(T item, int index, int count);
        void InsertRange(int index, System.Collections.Generic.IEnumerable<T> collection);
        bool IsFixedSize { get; }
        bool IsNotNullable { get; }
        bool IsSynchronized { get; }
        bool IsUnique { get; }
        int LastIndexOf(T item, int index);
        int LastIndexOf(T item, int index, int count);
        int LastIndexOf(T item);
        int RemoveAll(Predicate<T> match);
        void RemoveRange(int index, int count);
        //void Reverse(int index, int count);
        void Reverse();
        //void Sort(Comparison<T> comparison);
        //void Sort(System.Collections.Generic.IComparer<T> comparer);
        //void Sort(int index, int count, System.Collections.Generic.IComparer<T> comparer);
        //void Sort();
        object SyncRoot { get; }
        T[] ToArray();
        void TrimExcess();
        bool TrueForAll(Predicate<T> match);
    }
}
