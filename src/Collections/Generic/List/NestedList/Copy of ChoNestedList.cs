#region Header

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Copyright (c) 2009-2010 Raj Nagalingam.
 *    All rights reserved.
 *
 * This program and the accompanying materials are made available under
 * the terms of the Common Public License v1.0 which accompanies this
 * distribution.
 *
 * Redistribution and use in source and binary forms, with or
 * without modification, are permitted provided that the following
 * conditions are met:
 *
 * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in
 * the documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
 * OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 *<author>Raj Nagalingam</author>
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace Cinchoo.Core.Collections.Generic
{
    #region NameSpaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using System.Collections.ObjectModel;

    #endregion NameSpaces

    [Serializable]
    [ComVisible(false)]
    [DebuggerDisplay("Count = {Count}")]
    [XmlRoot("ChoNestedList")]
    public partial class ChoNestedList<T> : ObservableCollection<T>, IChoList<T>, IXmlSerializable, ICloneable, ICloneable<ChoNestedList<T>>
    {
        #region ChoNestedListTuple Class

        [Serializable]
        public class ChoNestedListTuple<T1, T2> : ChoTuple<T1, T2>
        {
            public ChoNestedListTuple()
            {
            }

            public ChoNestedListTuple(T1 first, T2 second)
                : base(first, second)
            {
            }

            public override void ReadXml(System.Xml.XmlReader reader)
            {
                if (reader.IsStartElement() && reader.Name == "ChoTuple")
                {
                    if (reader.IsEmptyElement) //If ChoTuple is empty
                    {
                        reader.ReadStartElement(); //Move to next element
                        return;
                    }

                    reader.ReadStartElement(); //First

                    if (reader.IsEmptyElement) //If First is empty
                        reader.ReadStartElement(); //Move to next element
                    else
                    {
                        reader.ReadStartElement(); //First Content
                        if (reader.IsEmptyElement) //First Content is empty
                            reader.ReadStartElement(); //Move to next element
                        else
                        {
                            XmlSerializer serializer1 = new XmlSerializer(typeof(T1));
                            First = (T1)serializer1.Deserialize(reader);
                            reader.ReadEndElement(); //First Content
                        }
                    }

                    if (reader.IsEmptyElement) //If Second is empty
                        reader.ReadStartElement(); //Move to next element
                    else
                    {
                        reader.ReadStartElement(); //Second Content
                        if (reader.Name == "ChoNestedList")
                        {
                            if (reader.IsEmptyElement) //Second Content is empty
                                reader.ReadStartElement(); //Move to next element
                            else
                            {
                                XmlSerializer serializer21 = new XmlSerializer(typeof(ChoNestedList<T1>));
                                Second = (T2)serializer21.Deserialize(reader);
                                reader.ReadEndElement(); //Second Content
                            }
                        }
                        else
                        {
                            XmlSerializer serializer22 = new XmlSerializer(typeof(List<T1>));
                            Second = (T2)serializer22.Deserialize(reader);
                        }
                        reader.ReadEndElement();
                    }
                    reader.ReadEndElement(); //ChoTuple
                }
            }
        }

        #endregion ChoNestedListTuple Class

        #region Instance Data Members (Private)

        private readonly object _syncRoot = new object();
        private readonly List<ChoNestedListTuple<T, IList<T>>> _innerList = new List<ChoNestedListTuple<T, IList<T>>>();

		#endregion Instance Data Members (Private)

        #region Constructors

		/// <summary>
		/// Initializes a new instance of the ChoNestedList<T> class
		/// that is empty and had the default initial capacity
		/// </summary>
		public ChoNestedList()
        {
        }

		/// <summary>
		/// Initializes a new instance of the ChoNestedList<T> class
		/// that contains element copied from the passed item object.
		/// </summary>
		/// <param name="item">An object to be copied to the the new list.</param>
		public ChoNestedList(T item)
        {
            Add(item);
        }

		/// <summary>
		/// Initializes a new instance of the ChoNestedList<T> class
		/// that contains element copied from the passed list object as an element.
		/// </summary>
		/// <param name="list">A collection object to be copied as an element to the the new list.</param>
        public ChoNestedList(IList<T> list)
        {
            Add(list);
        }

		/// <summary>
		/// Initializes a new instance of the ChoNestedList<T> class
		/// that contains elements copied from the specified collection and has sufficient
		/// capacity to accommodate the number of elements copied.
		/// </summary>
		/// <param name="collection">The collection whose elements are copied to the new list.</param>
		/// <exception cref="System.ArgumentNullException">collection is null.</remarks>
		public ChoNestedList(IEnumerable<T> collection)
		{
			ChoGuard.ArgumentNotNull(collection, "collection is null");
			AddRange(collection);
		}

		/// <summary>
		/// Initializes a new instance of the ChoNestedList<T> class
		/// that contains elements copied from the specified collection and has sufficient
		/// capacity to accommodate the number of elements copied.
		/// </summary>
		/// <param name="collection">The collection whose elements are copied to the new list.</param>
		/// <exception cref="System.ArgumentNullException">collection is null.</remarks>
		public ChoNestedList(IEnumerable<IList<T>> collection)
        {
            ChoGuard.ArgumentNotNull(collection, "collection");
            AddRange(collection);
        }

        #endregion Constructors

        #region ICloneable<ChoNestedList<T>> Members

        public ChoNestedList<T> Clone()
        {
            return this.CloneObject<ChoNestedList<T>>();
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return this.CloneObject<ChoNestedList<T>>() as object;
        }

        //private IList<T> Clone(IEnumerable<T> inList)
        //{
        //    if (inList is ChoNestedList<T>)
        //    {
        //        ChoNestedList<T> outList = new ChoNestedList<T>();

        //        foreach (ChoTuple<T, IList<T>> tuple in ((ChoNestedList<T>)inList)._innerList)
        //        {
        //            if (tuple.Second == null)
        //            {
        //                if (tuple.First == null)
        //                    outList.Add(default(T));
        //                else if (tuple.First is ICloneable)
        //                    outList.Add((T)((ICloneable)tuple.First).Clone());
        //                else
        //                    throw new ChoNestedListException("One of the list item is not a Clonable object.");
        //            }
        //            else
        //                outList.AddList(Clone(tuple.Second));
        //        }

        //        return outList;
        //    }
        //    else if (inList is IList<T>)
        //    {
        //        List<T> subOutList = new List<T>();

        //        foreach (T item in inList)
        //        {
        //            if (item is ICloneable)
        //                subOutList.Add((T)((ICloneable)item).Clone());
        //            else
        //                throw new ChoNestedListException("One of the list item is not a Clonable object.");
        //        }

        //        return subOutList;
        //    }
        //    else
        //        throw new ChoNestedListException("Unsupported list item found.");
        //}

        #endregion

        #region IChoList<T> Members

        #region Sort Overloads

        public void Sort()
        {
            throw new NotImplementedException();
        }

        public void Sort(Comparison<T> comparison)
        {
            throw new NotImplementedException();
        }

        public void Sort(IComparer<T> comparer)
        {
            throw new NotImplementedException();
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            throw new NotImplementedException();
        }

        #endregion Sort Overloads

        #region BinarySearch Overloads

        public int BinarySearch(T item)
        {
            T[] array = ToSortedArray();
            return Array.BinarySearch<T>(array, 0, array.Length, item);
        }

        public int BinarySearch(T item, IComparer<T> comparer)
        {
            T[] array = ToSortedArray();
            return Array.BinarySearch<T>(array, 0, array.Length, item, comparer);
        }

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            T[] array = ToSortedArray();
			array.CheckRange<T>(index, count);
            return Array.BinarySearch<T>(array, index, count, item, comparer);
        }

        #endregion BinarySearch Overloads

        #region ToSortedArray Overloads

        public T[] ToSortedArray(Comparison<T> comparison)
        {
            T[] array = ToArray();
            Array.Sort<T>(array, comparison);
            return array;
        }

        public T[] ToSortedArray()
        {
            T[] array = ToArray();
            Array.Sort<T>(array, 0, array.Length, Comparer<T>.Default);
            return array;
        }

        public T[] ToSortedArray(IComparer<T> comparer)
        {
            T[] array = ToArray();
            Array.Sort<T>(array, 0, array.Length, comparer);
            return array;
        }

        public T[] ToSortedArray(int index, int count, IComparer<T> comparer)
        {
            T[] array = ToArray();
            array.CheckRange<T>(index, count);
            Array.Sort<T>(array, index, count, Comparer<T>.Default);
            return array;
        }

        #endregion ToSortedArray Overloads

        #region Add Overloads

        public new void Add(T item)
        {
            _innerList.Add(new ChoNestedListTuple<T, IList<T>>(item, null));
        }

        public void Add(IList<T> listItem)
        {
            ChoGuard.ArgumentNotNull(listItem, "list");

            if (listItem == this)
                throw new InvalidOperationException("Can't add same list to itself.");

            if (listItem is ChoNestedList<T>)
            {
				if (((ChoNestedList<T>)listItem).Contains(this))
					throw new InvalidOperationException("Can't add item to the list, because it'll makes circular reference.");

				if (CheckCircularReferenceExists(listItem as ChoNestedList<T>))
                    throw new InvalidOperationException("Can't add item to the list, because it'll makes circular reference.");
            }

            _innerList.Add(new ChoNestedListTuple<T, IList<T>>(default(T), listItem));
        }

		private bool CheckCircularReferenceExists(ChoNestedList<T> listItem)
        {
			using (ChoNestedList<T>.ChoNestedCollectionListEnumerator<T> x1 = new ChoNestedList<T>.ChoNestedCollectionListEnumerator<T>(listItem))
			{
				while (x1.MoveNext())
				{
					ChoNestedList<T>.ChoNestedCollectionListEnumerator<T> x2 = new ChoNestedList<T>.ChoNestedCollectionListEnumerator<T>(this);
					while (x2.MoveNext())
					{
						if (x2.Current == x1.Current)
							return true;
					}
				}
			}
			return false;
        }

        #endregion Add Overloads

        #region AddRange Overloads

        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null) return;

            foreach (T item in collection)
                Add(item);
        }

        public void AddRange(IEnumerable<IList<T>> collection)
        {
            if (collection == null) return;

            foreach (IList<T> list in collection)
                Add(list);
        }

        #endregion AddRange Overloads

        #region ConvertAll Overloads

        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
			List<TOutput> outList = new List<TOutput>();
			foreach (T item in this)
				outList.Add(converter((T)item));

			return outList;
        }

        #endregion ConvertAll Overloads

        #region CopyTo Overloads

        public void CopyTo(T[] array)
        {
            this.CopyTo(array, 0);
        }

        public new void CopyTo(T[] array, int arrayIndex)
        {
            CopyTo(0, array, arrayIndex, Int32.MinValue);
        }

        public void CopyTo(int startIndex, T[] array, int arrayIndex, int count)
        {
            ChoGuard.ArgumentNotNull(array, "Destination array can't be null.");

            int listCount = Count;
            if (count == Int32.MinValue) count = listCount - startIndex;

			this.CheckRange(startIndex, count, listCount);

            int arrayLength = array.Length;
            if (arrayIndex < 0 || arrayIndex > arrayLength)
                throw new IndexOutOfRangeException("Destination array index is outside the bound of array.");

            int index = 0;
            int maxIndex = startIndex + count;

            foreach (T item in this)
            {
                if (index >= maxIndex) break;
                if (arrayIndex >= arrayLength) break;

                if (index >= startIndex)
                    array[arrayIndex++] = item;
                else
                    index++;
            }
        }

        #endregion CopyTo Overloads

        #region Exists Overloads

        public bool Exists(Predicate<T> match)
        {
            ChoGuard.ArgumentNotNull(match, "match");
            return FindIndex(match) != -1;
        }

        public bool Exists(T item)
        {
            return IndexOf(item) != -1;
        }

        #endregion Exists Overloads

        #region Find Overloads

        public T Find(Predicate<T> match)
        {
            ChoGuard.ArgumentNotNull(match, "match");

            foreach (T item in this)
            {
                if (match(item))
                    return item;
            }

            return default(T);
        }

        #endregion Find Overloads

        #region FindAll Overloads

        public List<T> FindAll(Predicate<T> match)
        {
            ChoGuard.ArgumentNotNull(match, "match");

            List<T> outList = new List<T>();
            foreach (T item in this)
            {
                if (match(item))
                    outList.Add((T)item);
            }

            return outList;
        }

        #endregion FindAll Overloads

        #region FindIndex Overloads

        public int FindIndex(Predicate<T> match)
        {
            ChoGuard.ArgumentNotNull(match, "match");
            return FindIndex(0, match);
        }

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            ChoGuard.ArgumentNotNull(match, "match");
            this.CheckIndex<T>(startIndex);
            return FindIndex(startIndex, Int32.MinValue, match);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            ChoGuard.ArgumentNotNull(match, "match");

            int listCount = Count;
            if (count == Int32.MinValue) count = listCount - startIndex;

			this.CheckRange(startIndex, count, listCount);

            int index = 0;
            int maxIndex = startIndex + count;
            foreach (T item in this)
            {
                if (index >= maxIndex) break;

                if (index >= startIndex && match(item))
                    return index;
                else
                    index++;
            }

            return -1;
        }

        #endregion FindIndex Overloads

        #region FindLast Overloads

        public T FindLast(Predicate<T> match)
        {
            ChoGuard.ArgumentNotNull(match, "match");

			using (ChoNestedCollectionReverseEnumerator<T> enumerator = new ChoNestedList<T>.ChoNestedCollectionReverseEnumerator<T>(this, match))
			{
				while (enumerator.MoveNext())
				{
					Console.WriteLine(enumerator.ListIndex);
					return enumerator.Current;
				}
			}

			return default(T);
		}

        #endregion FindLast Overloads

        #region FindLastIndex Overloads

        public int FindLastIndex(Predicate<T> match)
        {
            ChoGuard.ArgumentNotNull(match, "match");
            return FindLastIndex(0, match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            ChoGuard.ArgumentNotNull(match, "match");
            this.CheckIndex<T>(startIndex);
            return FindLastIndex(startIndex, Int32.MinValue, match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            ChoGuard.ArgumentNotNull(match, "match");

            int listCount = Count;
            if (count == Int32.MinValue) count = listCount - startIndex;

			this.CheckRange(startIndex, count, listCount);

            int index = listCount - 1;
            int maxIndex = startIndex + count;
            foreach (T item in ReverseList())
            {
                if (index < startIndex) break;

                if (index < maxIndex && match(item))
                    return index;
                else
                    index--;
            }

            return -1;
        }

        #endregion FindLastIndex Overloads

        #region IndexOf Overloads

        public new int IndexOf(T item)
        {
            return IndexOf(item, 0);
        }

        public int IndexOf(T item, int startIndex)
        {
            this.CheckIndex<T>(startIndex);
            return IndexOf(item, startIndex, Int32.MinValue);
        }

        public int IndexOf(T item, int startIndex, int count)
        {
            int listCount = Count;
            if (count == Int32.MinValue) count = listCount - startIndex;

			this.CheckRange(startIndex, count, listCount);

            int index = 0;
            int maxIndex = startIndex + count;
            foreach (T item1 in this)
            {
                if (index >= maxIndex) break;

                if (index >= startIndex && EqualityComparer<T>.Default.Equals(item, item1))
                    return index;
                else
                    index++;
            }

            return -1;
        }

        #endregion IndexOf Overloads

        #region InsertRange Overloads

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            ChoGuard.ArgumentNotNull(collection, "collection");
            this.CheckIndex<T>(index);

            foreach (T item in collection)
                Insert(index, item);
        }

        public void InsertRange(int index, IEnumerable<IList<T>> collection)
        {
            ChoGuard.ArgumentNotNull(collection, "collection");
            this.CheckIndex<T>(index);

            foreach (IList<T> item in collection)
                Insert(index, item);
        }

        #endregion InsertRange Overloads

        #region GetRange Overloads

        public List<T> GetRange(int index, int count)
        {
			this.CheckRange<T>(index, count);

			List<T> list = new List<T>();

			int counter = -1;
			foreach (T item in this)
			{
				if (++counter < index)
					continue;

				list.Add(item);

				if (counter >= index + count - 1)
					break;
			}

			return list;
        }

        #endregion GetRange Overloads

        #region RemoveRange Overloads

        public void RemoveRange(int index, int count)
        {
			this.CheckRange(index, count, Count);

			for (int listIndex = index; listIndex < index + count; listIndex++)
				RemoveAt(index);
        }

        #endregion RemoveRange Overloads

        #region ForEach Overloads

        public void ForEach(Action<T> action)
        {
            ChoGuard.ArgumentNotNull(action, "action");

            foreach (T item in this)
                action(item);
        }

        #endregion ForEach Overloads

        #region LastIndexOf Overloads

        public int LastIndexOf(T item)
        {
            return LastIndexOf(item, 0);
        }

        public int LastIndexOf(T item, int startIndex)
        {
            return LastIndexOf(item, startIndex, Int32.MinValue);
        }

        public int LastIndexOf(T item, int startIndex, int count)
        {
            int listCount = Count;
            if (count == Int32.MinValue) count = listCount - startIndex;

			this.CheckRange(startIndex, count, listCount);

            int index = listCount - 1;
            int maxIndex = startIndex + count;
            foreach (T item1 in ReverseList())
            {
                if (index < startIndex) break;

                if (index < maxIndex && EqualityComparer<T>.Default.Equals(item, item1))
                    return index;
                else
                    index--;
            }

            return -1;
        }

        #endregion LastIndexOf Overloads

        #region Reverse Overloads

        public void Reverse()
        {
            Reverse(this);
        }

        public void Reverse(int startIndex, int count)
        {
            throw new NotSupportedException();
        }

        private void Reverse(IList<T> inList)
        {
			if (inList is ChoNestedList<T>)
			{
				foreach (ChoTuple<T, IList<T>> item in ((ChoNestedList<T>)inList)._innerList)
				{
					if (item.Second != null)
						Reverse(item.Second);
				}
			}

			if (inList is ChoNestedList<T>)
				((ChoNestedList<T>)inList)._innerList.Reverse();
            else if (inList is List<T>)
                ((List<T>)inList).Reverse();
            else
				throw new ChoNestedListException("Unsupported list item found.");
		}

        #endregion Reverse Overloads

        #region RemoveAll Overloads

        public int RemoveAll(Predicate<T> match)
        {
            ChoGuard.ArgumentNotNull(match, "match");

			List<T> itemsToBeRemoved = new List<T>();
			foreach (T item in this)
			{
				if (match(item))
					itemsToBeRemoved.Add(item);
			}

			foreach (T item in itemsToBeRemoved)
				Remove(item);

			return itemsToBeRemoved.Count;
        }

        #endregion RemoveAll Overloads

        #region ToArray Overloads

        public T[] ToArray()
        {
            List<T> list = new List<T>();
            
			foreach (T item in this)
				list.Add(item);

            return list.ToArray();
        }

        #endregion ToArray Overloads

        #region TrimExcess Overloads

        public void TrimExcess()
        {
			using (ChoNestedList<T>.ChoNestedCollectionListEnumerator<T> x1 = new ChoNestedList<T>.ChoNestedCollectionListEnumerator<T>(this, item => item is ChoNestedList<T> || item is IList<T>))
			{
				while (x1.MoveNext())
				{
					if (x1.Current is ChoNestedList<T>)
						(((ChoNestedList<T>)x1.Current))._innerList.TrimExcess();
					else
						((List<T>)x1.Current).TrimExcess();
				}
			}
        }

        #endregion TrimExcess Overloads

        #region TrueForAll Overloads

        public bool TrueForAll(Predicate<T> match)
        {
            ChoGuard.ArgumentNotNull(match, "match");

            foreach (T item in this)
            {
                if (!match((T)item))
                    return false;
            }

            return true;
        }

        #endregion TrueForAll Overloads

        #region Instance Properties (Public)

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsNotNullable
        {
            get { return false; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public bool IsUnique
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        #endregion Instance Properties (Public)

        #endregion

        #region IList<T> Members

        #region Insert Overloads (Public)

        public new void Insert(int index, T item)
        {
            _innerList.Insert(index, new ChoNestedListTuple<T, IList<T>>(item, null));
        }

        public void Insert(int index, IList<T> item)
        {
            _innerList.Insert(index, new ChoNestedListTuple<T, IList<T>>(default(T), item));
        }

        #endregion Insert Overloads (Public)

        #region RemoveAt Overloads

        public new void RemoveAt(int index)
        {
            this.CheckIndex<T>(index);

			using (ChoNestedCollectionEnumerator<T> enumerator = GetEnumerator() as ChoNestedCollectionEnumerator<T>)
			{
				int counter = -1;
				while (enumerator.MoveNext())
				{
					if (++counter == index)
						break;
				}

				bool isListEmpty = false;
				if (enumerator.ContainedList is ChoNestedList<T>)
				{
					((ChoNestedList<T>)enumerator.ContainedList)._innerList.RemoveAt(enumerator.ListIndex);
					isListEmpty = ((ChoNestedList<T>)enumerator.ContainedList)._innerList.Count == 0;
				}
				else
				{
					enumerator.ContainedList.RemoveAt(enumerator.ListIndex);
					isListEmpty = enumerator.ContainedList.Count == 0;
				}
			}
        }


        #endregion RemoveAt Overloads

        #region Indexer

        public new T this[int index]
        {
            get
            {
                this.CheckIndex<T>(index);

                int counter = 0;
                foreach (T item in this)
                {
                    if (counter == index) return item;
                    counter++;
                }

                return default(T);
            }
            set
            {
                this.CheckIndex<T>(index);

				using (ChoNestedCollectionEnumerator<T> enumerator = GetEnumerator() as ChoNestedCollectionEnumerator<T>)
				{
					int counter = -1;
					while (enumerator.MoveNext())
					{
						if (++counter == index)
							break;
					}

					if (enumerator.ContainedList is ChoNestedList<T>)
						((ChoNestedList<T>)enumerator.ContainedList)._innerList[enumerator.ListIndex].First = value;
					else if (enumerator.ContainedList is IList<T>)
						((IList<T>)enumerator.ContainedList)[enumerator.ListIndex] = value;
				}
            }
        }

        #endregion Indexer

        #endregion

        #region ICollection<T> Members

        #region Clear Overloads

        public new void Clear()
        {
            _innerList.Clear();
        }

        #endregion Clear Overloads

        #region Contains Overloads

        public new bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public bool Contains(IList<T> listItem)
        {
            ChoGuard.ArgumentNotNull(listItem, "listItem");

			using (ChoNestedList<T>.ChoNestedCollectionListEnumerator<T> x1 = new ChoNestedList<T>.ChoNestedCollectionListEnumerator<T>(this, item => item is ChoNestedList<T> || item is IList<T>))
			{
				while (x1.MoveNext())
				{
					if (x1.Current == listItem)
						return true;
				}
			}

			return false;
		}

        #endregion Contains Overloads

        #region Count Method

        public new int Count
        {
            get
            {
                int counter = 0;

				foreach (object item in this)
					counter++;

                return counter;
            }
        }

		public int Length
		{
			get { return _innerList.Count; }
		}

        #endregion Count Method

		#region Remove Overloads

        public new bool Remove(T item)
        {
			bool found = false;
			bool isListEmpty = false;

			using (ChoNestedCollectionEnumerator<T> enumerator = GetEnumerator() as ChoNestedCollectionEnumerator<T>)
			{
				while (enumerator.MoveNext())
				{
					if (EqualityComparer<T>.Default.Equals(enumerator.Current, item))
					{
						found = true;
						break;
					}
				}

				if (found)
				{
					if (enumerator.ContainedList is ChoNestedList<T>)
					{
						((ChoNestedList<T>)enumerator.ContainedList)._innerList.RemoveAt(enumerator.ListIndex);
						isListEmpty = ((ChoNestedList<T>)enumerator.ContainedList)._innerList.Count == 0;
					}
					else
					{
						enumerator.ContainedList.RemoveAt(enumerator.ListIndex);
						isListEmpty = enumerator.ContainedList.Count == 0;
					}
				}

				//if (isListEmpty)
				//{
				//    if (enumerator.ParentList is ChoNestedList<T>)
				//        ((ChoNestedList<T>)enumerator.ParentList)._innerList.Remove(new ChoTuple<T, IList<T>>(default(T), enumerator.ContainedList));
				//}
			}

			return found;
        }

		public bool Remove(IList<T> list)
		{
			ChoGuard.ArgumentNotNull(list, "list");

			if (list == this)
				throw new InvalidOperationException("Can't remove itself.");

			return RemoveList(_innerList, list);
		}

		private bool RemoveList(IEnumerable inList, IList<T> listItem)
		{
			bool found = false;
			int listIndex = -1;

			if (inList is ChoNestedList<T>)
				inList = ((ChoNestedList<T>)inList)._innerList;

			foreach (object item in inList)
			{
				listIndex++;
				if (item is IEnumerable)
				{
					if (Object.Equals(item, listItem))
					{
						found = true;
						break;
					}

					found = RemoveList(item as IEnumerable, listItem);
					if (found)
						return true;
				}
			}

			if (found && listIndex >= 0)
			{
				if (inList is IList)
					((IList)inList).RemoveAt(listIndex);
				else if (inList is ChoNestedList<T>)
					((ChoNestedList<T>)inList)._innerList.RemoveAt(listIndex);
				else if (inList is IList<T>)
					((IList<T>)inList).RemoveAt(listIndex);
				else
					throw new ChoNestedListException("Unsupported list item found.");

				return true;
			}
			else
				return false;
		}

		#endregion Remove Overloads

		#endregion

		#region IEnumerable<T> Members

		public new IEnumerator<T> GetEnumerator()
        {
            return new ChoNestedCollectionEnumerator<T>(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new ChoNestedCollectionEnumerator<T>(this);
        }

        #endregion

        #region GetListItemAt Overloads

        public IList<T> GetListItemAt(int index)
        {
            this.CheckIndex<T>(index);

			using (ChoNestedCollectionEnumerator<T> enumerator = GetEnumerator() as ChoNestedCollectionEnumerator<T>)
			{
				int counter = -1;
				while (enumerator.MoveNext())
				{
					if (++counter == index)
						break;
				}

				return enumerator.ContainedList;
			}
        }

		#endregion GetListItemAt Overloads

        #region EnumerateAllChildLists

        public IEnumerable<IList<T>> EnumerateAllChildLists()
        {
            using (ChoNestedList<T>.ChoNestedCollectionListEnumerator<T> enumerator = new ChoNestedList<T>.ChoNestedCollectionListEnumerator<T>(this,
                (item) => item is IList<T> || item is ChoNestedList<T>))
            {
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
        }

        #endregion EnumerateAllChildLists

        #region Instance Members (Private)

        private IEnumerable<T> ReverseList()
        {
			using (ChoNestedCollectionReverseEnumerator<T> enumerator = new ChoNestedList<T>.ChoNestedCollectionReverseEnumerator<T>(this))
			{
				while (enumerator.MoveNext())
				{
					yield return enumerator.Current;
				}
			}
        }

        #endregion Instance Members (Private)

        #region ChoNestedCollectionEnumerator Class

        private sealed class ChoNestedCollectionEnumerator<T1> : ChoSyncDisposableObject, IEnumerator<T1>
        {
            #region Instance Data Members (Private)

            private T1 _current;
			private ChoTuple<IList<T1>, IList<T1>, int> _collectionPair;

			private readonly Predicate<T1> _match = item => true;
			private readonly Stack<ChoTuple<IList<T1>, IList<T1>, int>> _collectionPairs = new Stack<ChoTuple<IList<T1>, IList<T1>, int>>();
			private readonly Stack<ChoTuple<IEnumerator<ChoTuple<T1, IList<T1>>>, IEnumerator<T1>>> _collectionStack = new Stack<ChoTuple<IEnumerator<ChoTuple<T1,IList<T1>>>,IEnumerator<T1>>>();

            #endregion Instance Data Members (Private)

            #region Constructors

            public ChoNestedCollectionEnumerator(ChoNestedList<T1> nestedList) : this(nestedList, item => true)
            {
            }

			public ChoNestedCollectionEnumerator(ChoNestedList<T1> nestedList, Predicate<T1> match)
			{
				ChoGuard.ArgumentNotNull(nestedList, "NestedList");

				if (match != null) _match = match;
				_collectionStack.Push(new ChoTuple<IEnumerator<ChoTuple<T1, IList<T1>>>, IEnumerator<T1>>(nestedList._innerList.GetEnumerator(), null));

				_collectionPairs.Push(new ChoTuple<IList<T1>, IList<T1>, int>(nestedList as IList<T1>, null, -1));
				_collectionPair = _collectionPairs.Peek();
			}

            #endregion Constructors

            #region ChoSyncDisposableObject Overrides

            protected override void Dispose(bool finalize)
            {
            }

            #endregion ChoSyncDisposableObject Overrides

            #region IEnumerator<T1> Members

            public T1 Current
            {
                get { return _current; }
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return _current; }
            }

            public bool MoveNext()
            {
                return Move2NextItem();
            }

            public void Reset()
            {
				foreach (ChoTuple<IEnumerator<ChoTuple<T1, IList<T1>>>, IEnumerator<T1>> tuple in _collectionStack)
				{
					if (tuple.First != null) tuple.First.Reset();
					if (tuple.Second != null) tuple.Second.Reset();
				}

                _collectionStack.Clear();
				_collectionPairs.Clear();
				_collectionPair = null;
				_current = default(T1);
            }

            #endregion

			#region Instance Members (Public)

			public int ListIndex
			{
				get { return _collectionPair.Third; }
			}

			public IList<T1> ContainedList
			{
				get { return _collectionPair.First; }
			}

			public IList<T1> ParentList
			{
				get { return _collectionPair.Second; }
			}

			#endregion Instance Members (Public)

			#region Instance Members (Private)

			private bool Move2NextItem()
            {
				while (true)
				{
					if (_collectionStack.Count == 0)
					{
						_current = default(T1);
						return false;
					}
					else if (Move2NextItem(_collectionStack.Peek(), _collectionPairs.Peek()))
						return true;
				}
            }

			private bool Move2NextItem(ChoTuple<IEnumerator<ChoTuple<T1, IList<T1>>>, IEnumerator<T1>> tuple, ChoTuple<IList<T1>, IList<T1>, int> collectionPair)
            {
				bool retVal = false;

				if (tuple.First != null)
					retVal = tuple.First.MoveNext();
				else if (tuple.Second != null)
					retVal = tuple.Second.MoveNext();
				else
					return false;

				if (!retVal)
				{
					_collectionStack.Pop();
					_collectionPairs.Pop();
					_collectionPair = _collectionPairs.Count > 0 ? _collectionPairs.Peek() : null;

					return Move2NextItem();
				}
				else
				{
					if (tuple.First != null)
					{
						if (_collectionPair != null)
							_collectionPair.Third++;
						if (tuple.First.Current.Second != null)
						{
							if (tuple.First.Current.Second is ChoNestedList<T1>)
								_collectionStack.Push(new ChoTuple<IEnumerator<ChoTuple<T1, IList<T1>>>, IEnumerator<T1>>(((ChoNestedList<T1>)tuple.First.Current.Second)._innerList.GetEnumerator(), null));
							else if (tuple.First.Current.Second is IList<T1>)
								_collectionStack.Push(new ChoTuple<IEnumerator<ChoTuple<T1, IList<T1>>>, IEnumerator<T1>>(null, ((IList<T1>)tuple.First.Current.Second).GetEnumerator()));

							_collectionPairs.Push(new ChoTuple<IList<T1>, IList<T1>, int>(tuple.First.Current.Second as IList<T1>, collectionPair.First != null ? collectionPair.First : collectionPair.Second, -1));
							_collectionPair = _collectionPairs.Peek();

							return Move2NextItem();
						}
						else
						{
							if (_match(tuple.First.Current.First))
							{
								_current = tuple.First.Current.First;
								return true;
							}
							else
								return false;
						}
					}
					else if (tuple.Second != null)
					{
						if (_collectionPair != null)
							_collectionPair.Third++;
						if (_match(tuple.Second.Current))
						{
							_current = tuple.Second.Current;
							return true;
						}
						else
							return false;
					}
					else
						return false;
				}
            }

            #endregion Instance Members (Private)

        };

        #endregion ChoNestedCollectionEnumerator Class

		#region ChoNestedCollectionReverseEnumerator Class

		private sealed class ChoNestedCollectionReverseEnumerator<T1> : ChoSyncDisposableObject, IEnumerator<T1>
		{
			#region Instance Data Members (Private)

			private T1 _current;
			private ChoTuple<IList<T1>, IList<T1>, int> _collectionPair;

			private readonly Predicate<T1> _match;
			private readonly Stack<ChoTuple<IList<T1>, IList<T1>, int>> _collectionPairs = new Stack<ChoTuple<IList<T1>, IList<T1>, int>>();
			private readonly Stack<ChoTuple<IEnumerator<ChoTuple<T1, IList<T1>>>, IEnumerator<T1>>> _collectionStack = new Stack<ChoTuple<IEnumerator<ChoTuple<T1, IList<T1>>>, IEnumerator<T1>>>();

			#endregion Instance Data Members (Private)

			#region Constructors

			public ChoNestedCollectionReverseEnumerator(ChoNestedList<T1> nestedList)
				: this(nestedList, item => true)
			{
			}

			public ChoNestedCollectionReverseEnumerator(ChoNestedList<T1> nestedList, Predicate<T1> match)
			{
				ChoGuard.ArgumentNotNull(nestedList, "NestedList");

				_match = match;
				_collectionStack.Push(new ChoTuple<IEnumerator<ChoTuple<T1, IList<T1>>>, IEnumerator<T1>>(new ChoReverseEnumerator<ChoTuple<T1, IList<T1>>>(nestedList._innerList), null));

				_collectionPairs.Push(new ChoTuple<IList<T1>, IList<T1>, int>(nestedList as IList<T1>, null, -1));
				_collectionPair = _collectionPairs.Peek();
			}

			#endregion Constructors

			#region ChoSyncDisposableObject Overrides

			protected override void Dispose(bool finalize)
			{
			}

			#endregion ChoSyncDisposableObject Overrides

			#region IEnumerator<T1> Members

			public T1 Current
			{
				get { return _current; }
			}

			#endregion

			#region IEnumerator Members

			object System.Collections.IEnumerator.Current
			{
				get { return _current; }
			}

			public bool MoveNext()
			{
				return Move2NextItem();
			}

			public void Reset()
			{
				foreach (ChoTuple<IEnumerator<ChoTuple<T1, IList<T1>>>, IEnumerator<T1>> tuple in _collectionStack)
				{
					if (tuple.First != null)
						tuple.First.Reset();
					if (tuple.Second != null)
						tuple.Second.Reset();
				}

				_collectionStack.Clear();
			}

			#endregion

			#region Instance Members (Public)

			public int ListIndex
			{
				get 
				{
					if (ContainedList is ChoNestedList<T>)
						return ((ChoNestedList<T>)ContainedList)._innerList.Count - 1 - _collectionPair.Third;
					else
						return ContainedList.Count - _collectionPair.Third;
				}
			}

			public IList<T1> ContainedList
			{
				get { return _collectionPair.First; }
			}

			public IList<T1> ParentList
			{
				get { return _collectionPair.Second; }
			}

			#endregion Instance Members (Public)

			#region Instance Members (Private)

			private bool Move2NextItem()
			{
				while (true)
				{
					if (_collectionStack.Count == 0)
					{
						_current = default(T1);
						return false;
					}
					else if (Move2NextItem(_collectionStack.Peek(), _collectionPairs.Peek()))
							return true;
				}
			}

			private bool Move2NextItem(ChoTuple<IEnumerator<ChoTuple<T1, IList<T1>>>, IEnumerator<T1>> tuple, ChoTuple<IList<T1>, IList<T1>, int> collectionPair)
			{
				bool retVal = false;

				if (tuple.First != null)
					retVal = tuple.First.MoveNext();
				else if (tuple.Second != null)
					retVal = tuple.Second.MoveNext();
				else
					return false;

				if (!retVal)
				{
					_collectionStack.Pop();
					_collectionPairs.Pop();
					_collectionPair = _collectionPairs.Count > 0 ? _collectionPairs.Peek() : null;

					return Move2NextItem();
				}
				else
				{
					if (tuple.First != null)
					{
						if (_collectionPair != null)
							_collectionPair.Third++;
						if (tuple.First.Current.Second != null)
						{
							if (tuple.First.Current.Second is ChoNestedList<T1>)
								_collectionStack.Push(new ChoTuple<IEnumerator<ChoTuple<T1, IList<T1>>>, IEnumerator<T1>>(new ChoReverseEnumerator<ChoTuple<T1, IList<T1>>>(((ChoNestedList<T1>)tuple.First.Current.Second)._innerList), null));
							else if (tuple.First.Current.Second is IList<T1>)
								_collectionStack.Push(new ChoTuple<IEnumerator<ChoTuple<T1, IList<T1>>>, IEnumerator<T1>>(null, new ChoReverseEnumerator<T1>(((IList<T1>)tuple.First.Current.Second))));

							_collectionPairs.Push(new ChoTuple<IList<T1>, IList<T1>, int>(tuple.First.Current.Second as IList<T1>, collectionPair.First != null ? collectionPair.First : collectionPair.Second, -1));
							_collectionPair = _collectionPairs.Peek();

							return Move2NextItem();
						}
						else
						{
							if (_match(tuple.First.Current.First))
							{
								_current = tuple.First.Current.First;
								return true;
							}
							else
								return false;
						}
					}
					else if (tuple.Second != null)
					{
						if (_collectionPair != null)
							_collectionPair.Third++;
						if (_match(tuple.Second.Current))
						{
							_current = tuple.Second.Current;
							return true;
						}
						else
							return false;
					}
					else
						return false;
				}
			}

			#endregion Instance Members (Private)

		};

		#endregion ChoNestedCollectionReverseEnumerator Class

		#region ChoNestedCollectionListEnumerator Class

        private sealed class ChoNestedCollectionListEnumerator<T1> : ChoSyncDisposableObject, IEnumerator<IList<T1>>
        {
            #region Instance Data Members (Private)

			private IList<T1> _current;
			private Predicate<IEnumerable> _match;
			private Stack<IEnumerator> _collectionStack = new Stack<IEnumerator>();

            #endregion Instance Data Members (Private)

            #region Constructors

            // Constructor.
			public ChoNestedCollectionListEnumerator(ChoNestedList<T1> collection)
				: this(collection, (item) => item != null && item is ChoNestedList<T1>)
            {
            }

			// Constructor.
			public ChoNestedCollectionListEnumerator(ChoNestedList<T1> collection, Predicate<IEnumerable> match)
			{
				ChoGuard.ArgumentNotNull(collection, "collection");
				ChoGuard.ArgumentNotNull(match, "match");

				_collectionStack.Push(collection._innerList.GetEnumerator());
				_match = match;
			}

            #endregion Constructors

            #region ChoSyncDisposableObject Overrides

            protected override void Dispose(bool finalize)
            {
            }

            #endregion ChoSyncDisposableObject Overrides

            #region IEnumerator<T1> Members

			public IList<T1> Current
            {
                get { return _current; }
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return _current; }
            }

            public bool MoveNext()
            {
                return Move2NextItem();
            }

            public void Reset()
            {
                foreach (IEnumerator<T1> collection in _collectionStack)
                    collection.Reset();

                _collectionStack.Clear();
            }


            #endregion

            #region Instance Members (Private)

            private bool Move2NextItem()
            {
                if (_collectionStack.Count == 0)
                {
					_current = default(ChoNestedList<T1>);
                    return false;
                }
                else
                    return Move2NextItem(_collectionStack.Peek());
            }

			private bool Move2NextItem(IEnumerator collection)
            {
                bool retVal = false;

                while (true)
                {
					retVal = collection.MoveNext();
					if (!retVal)
                        break;

					if (collection.Current is ChoTuple<T1, IList<T1>> && ((ChoTuple<T1, IList<T1>>)collection.Current).Second != null
						&& _match(((ChoTuple<T1, IList<T1>>)collection.Current).Second as IEnumerable))
					{
						retVal = true;
						_current = ((ChoTuple<T1, IList<T1>>)collection.Current).Second;

						if (((ChoTuple<T1, IList<T1>>)collection.Current).Second is ChoNestedList<T1>)
							_collectionStack.Push(((ChoNestedList<T1>)((ChoTuple<T1, IList<T1>>)collection.Current).Second)._innerList.GetEnumerator());
						else if (((ChoTuple<T1, IList<T1>>)collection.Current).Second is IList<T1>)
							_collectionStack.Push(((IList<T1>)((ChoTuple<T1, IList<T1>>)collection.Current).Second).GetEnumerator());

						break;
					}
                }

                if (!retVal)
                {
                    _collectionStack.Pop();
                    return Move2NextItem();
                }

                return retVal;
            }

            #endregion Instance Members (Private)
        };

		#endregion ChoNestedCollectionListEnumerator Class

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            reader.MoveToElement();

            _innerList.Clear();
            Boolean isEmptyElement = reader.IsEmptyElement; //ChoTuple
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                while (!reader.EOF)
                {
                    ChoNestedListTuple<T, IList<T>> tuple = new ChoNestedListTuple<T, IList<T>>();
                    tuple.ReadXml(reader);
                    _innerList.Add(tuple);

                    if (!reader.IsStartElement() && reader.Name == "ChoNestedList")
                        break;
                }
            }
            //Boolean isEmptyElement = reader.IsEmptyElement; // (1)
            //reader.ReadStartElement();
            //if (!isEmptyElement) // (1)
            //{
            //    while (true)
            //    {
            //        ChoNestedListTuple<T, IList<T>> tuple = new ChoNestedListTuple<T, IList<T>>();
            //        tuple.ReadXml(reader);
            //        _innerList.Add(tuple);
            //        reader.ReadEndElement();
            //        if (reader.Name == "ChoNestedList" || reader.EOF)
            //            break;
            //    }
            //}
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (ChoTuple<T, IList<T>> tuple in ((ChoNestedList<T>)this)._innerList)
            {
                tuple.WriteXml(writer);
            }
        }

        #endregion
    }
}