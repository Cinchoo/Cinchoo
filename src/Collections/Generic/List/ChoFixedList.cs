namespace Cinchoo.Core.Collections.Generic
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.InteropServices;
    using System.Diagnostics;

    #endregion NameSpaces

    public partial class ChoList<T> : IChoList<T>, ICloneable
    {
        [Serializable]
        private class ChoFixedList<T1> : ChoList<T1>, ICloneable
        {
            #region Instance Data Members (Private)

            private new readonly ChoList<T1> _innerList;
            private readonly bool _silent;

            #endregion Instance Data Members (Private)

            #region Constructors

            internal ChoFixedList(ChoList<T1> list)
                : this(list, false)
            {
            }

            internal ChoFixedList(ChoList<T1> list, bool silent)
            {
                _innerList = list;
                _silent = silent;
            }

            #endregion Constructors

            #region Instance Properties (Public)

            public override bool IsNotNullable
            {
                get { return _innerList.IsNotNullable; }
            }

            public override bool IsUnique
            {
                get { return _innerList.IsUnique; }
            }

            public override bool IsFixedSize
            {
                get { return true; }
            }

            public override bool IsReadOnly
            {
                get { return _innerList.IsReadOnly; }
            }

            public override bool IsSynchronized
            {
                get { return _innerList.IsSynchronized; }
            }

            public override int Count
            {
                get { return _innerList.Count; }
            }

            public override object SyncRoot
            {
                get { return _innerList.SyncRoot; }
            }

            #endregion Instance Properties (Public)

            #region Indexers (Public)

            public override T1 this[int index]
            {
                get { return _innerList[index]; }
                set { _innerList[index] = value; }
            }

            #endregion Indexers (Public)

            #region List Overloads

            // Summary:
            //     Adds an object to the end of the System.Collections.Generic.List<T>.
            //
            // Parameters:
            //   item:
            //     The object to be added to the end of the System.Collections.Generic.List<T>.
            //     The value can be null for reference types.
            public override void Add(T1 item)
            {
                CheckFixed();
            }

            //
            // Summary:
            //     Adds the elements of the specified collection to the end of the System.Collections.Generic.List<T>.
            //
            // Parameters:
            //   collection:
            //     The collection whose elements should be added to the end of the System.Collections.Generic.List<T>.
            //     The collection itself cannot be null, but it can contain elements that are
            //     null, if type T is a reference type.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     collection is null.
            public override void AddRange(IEnumerable<T1> collection)
            {
                if (collection == null) return;

                CheckFixed();
            }

            //
            // Summary:
            //     Searches the entire sorted System.Collections.Generic.List<T> for an element
            //     using the default comparer and returns the zero-based index of the element.
            //
            // Parameters:
            //   item:
            //     The object to locate. The value can be null for reference types.
            //
            // Returns:
            //     The zero-based index of item in the sorted System.Collections.Generic.List<T>,
            //     if item is found; otherwise, a negative number that is the bitwise complement
            //     of the index of the next element that is larger than item or, if there is
            //     no larger element, the bitwise complement of System.Collections.Generic.List<T>.Count.
            //
            // Exceptions:
            //   System.InvalidOperationException:
            //     The default comparer System.Collections.Generic.Comparer<T>.Default cannot
            //     find an implementation of the System.IComparable<T> generic interface or
            //     the System.IComparable interface for type T.
            public override int BinarySearch(T1 item)
            {
                return _innerList.BinarySearch(item);
            }

            //
            // Summary:
            //     Searches the entire sorted System.Collections.Generic.List<T> for an element
            //     using the specified comparer and returns the zero-based index of the element.
            //
            // Parameters:
            //   item:
            //     The object to locate. The value can be null for reference types.
            //
            //   comparer:
            //     The System.Collections.Generic.IComparer<T> implementation to use when comparing
            //     elements.  -or- null to use the default comparer System.Collections.Generic.Comparer<T>.Default.
            //
            // Returns:
            //     The zero-based index of item in the sorted System.Collections.Generic.List<T>,
            //     if item is found; otherwise, a negative number that is the bitwise complement
            //     of the index of the next element that is larger than item or, if there is
            //     no larger element, the bitwise complement of System.Collections.Generic.List<T>.Count.
            //
            // Exceptions:
            //   System.InvalidOperationException:
            //     comparer is null, and the default comparer System.Collections.Generic.Comparer<T>.Default
            //     cannot find an implementation of the System.IComparable<T> generic interface
            //     or the System.IComparable interface for type T.
            public override int BinarySearch(T1 item, IComparer<T1> comparer)
            {
                return _innerList.BinarySearch(item, comparer);
            }

            //
            // Summary:
            //     Searches a range of elements in the sorted System.Collections.Generic.List<T>
            //     for an element using the specified comparer and returns the zero-based index
            //     of the element.
            //
            // Parameters:
            //   index:
            //     The zero-based starting index of the range to search.
            //
            //   count:
            //     The length of the range to search.
            //
            //   item:
            //     The object to locate. The value can be null for reference types.
            //
            //   comparer:
            //     The System.Collections.Generic.IComparer<T> implementation to use when comparing
            //     elements, or null to use the default comparer System.Collections.Generic.Comparer<T>.Default.
            //
            // Returns:
            //     The zero-based index of item in the sorted System.Collections.Generic.List<T>,
            //     if item is found; otherwise, a negative number that is the bitwise complement
            //     of the index of the next element that is larger than item or, if there is
            //     no larger element, the bitwise complement of System.Collections.Generic.List<T>.Count.
            //
            // Exceptions:
            //   System.ArgumentOutOfRangeException:
            //     index is less than 0.  -or- count is less than 0.
            //
            //   System.ArgumentException:
            //     index and count do not denote a valid range in the System.Collections.Generic.List<T>.
            //
            //   System.InvalidOperationException:
            //     comparer is null, and the default comparer System.Collections.Generic.Comparer<T>.Default
            //     cannot find an implementation of the System.IComparable<T> generic interface
            //     or the System.IComparable interface for type T.
            public override int BinarySearch(int index, int count, T1 item, IComparer<T1> comparer)
            {
                return _innerList.BinarySearch(index, count, item, comparer);
            }

            //
            // Summary:
            //     Removes all elements from the System.Collections.Generic.List<T>.
            public override void Clear()
            {
                CheckFixed();
            }

            //
            // Summary:
            //     Determines whether an element is in the System.Collections.Generic.List<T>.
            //
            // Parameters:
            //   item:
            //     The object to locate in the System.Collections.Generic.List<T>. The value
            //     can be null for reference types.
            //
            // Returns:
            //     true if item is found in the System.Collections.Generic.List<T>; otherwise,
            //     false.
            public override bool Contains(T1 item)
            {
                return _innerList.Contains(item);
            }

            //
            // Summary:
            //     Converts the elements in the current System.Collections.Generic.List<T> to
            //     another type, and returns a list containing the converted elements.
            //
            // Parameters:
            //   converter:
            //     A System.Converter<TInput,TOutput> delegate that converts each element from
            //     one type to another type.
            //
            // Type parameters:
            //   TOutput:
            //     The type of the elements of the target array.
            //
            // Returns:
            //     A System.Collections.Generic.List<T> of the target type containing the converted
            //     elements from the current System.Collections.Generic.List<T>.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     converter is null.
            public override List<TOutput> ConvertAll<TOutput>(Converter<T1, TOutput> converter)
            {
                return _innerList.ConvertAll<TOutput>(converter);
            }

            //
            // Summary:
            //     Copies the entire System.Collections.Generic.List<T> to a compatible one-dimensional
            //     array, starting at the beginning of the target array.
            //
            // Parameters:
            //   array:
            //     The one-dimensional System.Array that is the destination of the elements
            //     copied from System.Collections.Generic.List<T>. The System.Array must have
            //     zero-based indexing.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     array is null.
            //
            //   System.ArgumentException:
            //     The number of elements in the source System.Collections.Generic.List<T> is
            //     greater than the number of elements that the destination array can contain.
            public override void CopyTo(T1[] array)
            {
                _innerList.CopyTo(array);
            }

            //
            // Summary:
            //     Copies the entire System.Collections.Generic.List<T> to a compatible one-dimensional
            //     array, starting at the specified index of the target array.
            //
            // Parameters:
            //   array:
            //     The one-dimensional System.Array that is the destination of the elements
            //     copied from System.Collections.Generic.List<T>. The System.Array must have
            //     zero-based indexing.
            //
            //   arrayIndex:
            //     The zero-based index in array at which copying begins.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     array is null.
            //
            //   System.ArgumentOutOfRangeException:
            //     arrayIndex is less than 0.
            //
            //   System.ArgumentException:
            //     arrayIndex is equal to or greater than the length of array.  -or- The number
            //     of elements in the source System.Collections.Generic.List<T> is greater than
            //     the available space from arrayIndex to the end of the destination array.
            public override void CopyTo(T1[] array, int arrayIndex)
            {
                _innerList.CopyTo(array, arrayIndex);
            }

            //
            // Summary:
            //     Copies a range of elements from the System.Collections.Generic.List<T> to
            //     a compatible one-dimensional array, starting at the specified index of the
            //     target array.
            //
            // Parameters:
            //   index:
            //     The zero-based index in the source System.Collections.Generic.List<T> at
            //     which copying begins.
            //
            //   array:
            //     The one-dimensional System.Array that is the destination of the elements
            //     copied from System.Collections.Generic.List<T>. The System.Array must have
            //     zero-based indexing.
            //
            //   arrayIndex:
            //     The zero-based index in array at which copying begins.
            //
            //   count:
            //     The number of elements to copy.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     array is null.
            //
            //   System.ArgumentOutOfRangeException:
            //     index is less than 0.  -or- arrayIndex is less than 0.  -or- count is less
            //     than 0.
            //
            //   System.ArgumentException:
            //     index is equal to or greater than the System.Collections.Generic.List<T>.Count
            //     of the source System.Collections.Generic.List<T>.  -or- arrayIndex is equal
            //     to or greater than the length of array.  -or- The number of elements from
            //     index to the end of the source System.Collections.Generic.List<T> is greater
            //     than the available space from arrayIndex to the end of the destination array.
            public override void CopyTo(int index, T1[] array, int arrayIndex, int count)
            {
                _innerList.CopyTo(index, array, arrayIndex, count);
            }

            //
            // Summary:
            //     Determines whether the System.Collections.Generic.List<T> contains elements
            //     that match the conditions defined by the specified predicate.
            //
            // Parameters:
            //   match:
            //     The System.Predicate<T> delegate that defines the conditions of the elements
            //     to search for.
            //
            // Returns:
            //     true if the System.Collections.Generic.List<T> contains one or more elements
            //     that match the conditions defined by the specified predicate; otherwise,
            //     false.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     match is null.
            public override bool Exists(Predicate<T1> match)
            {
                return _innerList.Exists(match);
            }

            //
            // Summary:
            //     Searches for an element that matches the conditions defined by the specified
            //     predicate, and returns the first occurrence within the entire System.Collections.Generic.List<T>.
            //
            // Parameters:
            //   match:
            //     The System.Predicate<T> delegate that defines the conditions of the element
            //     to search for.
            //
            // Returns:
            //     The first element that matches the conditions defined by the specified predicate,
            //     if found; otherwise, the default value for type T.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     match is null.
            public override T1 Find(Predicate<T1> match)
            {
                return _innerList.Find(match);
            }

            //
            // Summary:
            //     Retrieves all the elements that match the conditions defined by the specified
            //     predicate.
            //
            // Parameters:
            //   match:
            //     The System.Predicate<T> delegate that defines the conditions of the elements
            //     to search for.
            //
            // Returns:
            //     A System.Collections.Generic.List<T> containing all the elements that match
            //     the conditions defined by the specified predicate, if found; otherwise, an
            //     empty System.Collections.Generic.List<T>.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     match is null.
            public override List<T1> FindAll(Predicate<T1> match)
            {
                return _innerList.FindAll(match);
            }

            //
            // Summary:
            //     Searches for an element that matches the conditions defined by the specified
            //     predicate, and returns the zero-based index of the first occurrence within
            //     the entire System.Collections.Generic.List<T>.
            //
            // Parameters:
            //   match:
            //     The System.Predicate<T> delegate that defines the conditions of the element
            //     to search for.
            //
            // Returns:
            //     The zero-based index of the first occurrence of an element that matches the
            //     conditions defined by match, if found; otherwise, –1.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     match is null.
            public override int FindIndex(Predicate<T1> match)
            {
                return _innerList.FindIndex(match);
            }

            //
            // Summary:
            //     Searches for an element that matches the conditions defined by the specified
            //     predicate, and returns the zero-based index of the first occurrence within
            //     the range of elements in the System.Collections.Generic.List<T> that extends
            //     from the specified index to the last element.
            //
            // Parameters:
            //   startIndex:
            //     The zero-based starting index of the search.
            //
            //   match:
            //     The System.Predicate<T> delegate that defines the conditions of the element
            //     to search for.
            //
            // Returns:
            //     The zero-based index of the first occurrence of an element that matches the
            //     conditions defined by match, if found; otherwise, –1.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     match is null.
            //
            //   System.ArgumentOutOfRangeException:
            //     startIndex is outside the range of valid indexes for the System.Collections.Generic.List<T>.
            public override int FindIndex(int startIndex, Predicate<T1> match)
            {
                return _innerList.FindIndex(startIndex, match);
            }

            //
            // Summary:
            //     Searches for an element that matches the conditions defined by the specified
            //     predicate, and returns the zero-based index of the first occurrence within
            //     the range of elements in the System.Collections.Generic.List<T> that starts
            //     at the specified index and contains the specified number of elements.
            //
            // Parameters:
            //   startIndex:
            //     The zero-based starting index of the search.
            //
            //   count:
            //     The number of elements in the section to search.
            //
            //   match:
            //     The System.Predicate<T> delegate that defines the conditions of the element
            //     to search for.
            //
            // Returns:
            //     The zero-based index of the first occurrence of an element that matches the
            //     conditions defined by match, if found; otherwise, –1.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     match is null.
            //
            //   System.ArgumentOutOfRangeException:
            //     startIndex is outside the range of valid indexes for the System.Collections.Generic.List<T>.
            //      -or- count is less than 0.  -or- startIndex and count do not specify a valid
            //     section in the System.Collections.Generic.List<T>.
            public override int FindIndex(int startIndex, int count, Predicate<T1> match)
            {
                return _innerList.FindIndex(startIndex, count, match);
            }

            //
            // Summary:
            //     Searches for an element that matches the conditions defined by the specified
            //     predicate, and returns the last occurrence within the entire System.Collections.Generic.List<T>.
            //
            // Parameters:
            //   match:
            //     The System.Predicate<T> delegate that defines the conditions of the element
            //     to search for.
            //
            // Returns:
            //     The last element that matches the conditions defined by the specified predicate,
            //     if found; otherwise, the default value for type T.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     match is null.
            public override T1 FindLast(Predicate<T1> match)
            {
                return _innerList.FindLast(match);
            }

            //
            // Summary:
            //     Searches for an element that matches the conditions defined by the specified
            //     predicate, and returns the zero-based index of the last occurrence within
            //     the entire System.Collections.Generic.List<T>.
            //
            // Parameters:
            //   match:
            //     The System.Predicate<T> delegate that defines the conditions of the element
            //     to search for.
            //
            // Returns:
            //     The zero-based index of the last occurrence of an element that matches the
            //     conditions defined by match, if found; otherwise, –1.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     match is null.
            public override int FindLastIndex(Predicate<T1> match)
            {
                return _innerList.FindLastIndex(match);
            }

            //
            // Summary:
            //     Searches for an element that matches the conditions defined by the specified
            //     predicate, and returns the zero-based index of the last occurrence within
            //     the range of elements in the System.Collections.Generic.List<T> that extends
            //     from the first element to the specified index.
            //
            // Parameters:
            //   startIndex:
            //     The zero-based starting index of the backward search.
            //
            //   match:
            //     The System.Predicate<T> delegate that defines the conditions of the element
            //     to search for.
            //
            // Returns:
            //     The zero-based index of the last occurrence of an element that matches the
            //     conditions defined by match, if found; otherwise, –1.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     match is null.
            //
            //   System.ArgumentOutOfRangeException:
            //     startIndex is outside the range of valid indexes for the System.Collections.Generic.List<T>.
            public override int FindLastIndex(int startIndex, Predicate<T1> match)
            {
                return _innerList.FindLastIndex(startIndex, match);
            }

            //
            // Summary:
            //     Searches for an element that matches the conditions defined by the specified
            //     predicate, and returns the zero-based index of the last occurrence within
            //     the range of elements in the System.Collections.Generic.List<T> that contains
            //     the specified number of elements and ends at the specified index.
            //
            // Parameters:
            //   startIndex:
            //     The zero-based starting index of the backward search.
            //
            //   count:
            //     The number of elements in the section to search.
            //
            //   match:
            //     The System.Predicate<T> delegate that defines the conditions of the element
            //     to search for.
            //
            // Returns:
            //     The zero-based index of the last occurrence of an element that matches the
            //     conditions defined by match, if found; otherwise, –1.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     match is null.
            //
            //   System.ArgumentOutOfRangeException:
            //     startIndex is outside the range of valid indexes for the System.Collections.Generic.List<T>.
            //      -or- count is less than 0.  -or- startIndex and count do not specify a valid
            //     section in the System.Collections.Generic.List<T>.
            public override int FindLastIndex(int startIndex, int count, Predicate<T1> match)
            {
                return FindLastIndex(startIndex, count, match);
            }

            //
            // Summary:
            //     Performs the specified action on each element of the System.Collections.Generic.List<T>.
            //
            // Parameters:
            //   action:
            //     The System.Action<T> delegate to perform on each element of the System.Collections.Generic.List<T>.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     action is null.
            public override void ForEach(Action<T1> action)
            {
                _innerList.ForEach(action);
            }

            //
            // Summary:
            //     Returns an enumerator that iterates through the System.Collections.Generic.List<T>.
            //
            // Returns:
            //     A System.Collections.Generic.List<T>.Enumerator for the System.Collections.Generic.List<T>.
            public override IEnumerator<T1> GetEnumerator()
            {
                return _innerList.GetEnumerator();
            }

            //
            // Summary:
            //     Creates a shallow copy of a range of elements in the source System.Collections.Generic.List<T>.
            //
            // Parameters:
            //   index:
            //     The zero-based System.Collections.Generic.List<T> index at which the range
            //     starts.
            //
            //   count:
            //     The number of elements in the range.
            //
            // Returns:
            //     A shallow copy of a range of elements in the source System.Collections.Generic.List<T>.
            //
            // Exceptions:
            //   System.ArgumentOutOfRangeException:
            //     index is less than 0.  -or- count is less than 0.
            //
            //   System.ArgumentException:
            //     index and count do not denote a valid range of elements in the System.Collections.Generic.List<T>.
            public override List<T1> GetRange(int index, int count)
            {
                return _innerList.GetRange(index, count);
            }

            //
            // Summary:
            //     Searches for the specified object and returns the zero-based index of the
            //     first occurrence within the entire System.Collections.Generic.List<T>.
            //
            // Parameters:
            //   item:
            //     The object to locate in the System.Collections.Generic.List<T>. The value
            //     can be null for reference types.
            //
            // Returns:
            //     The zero-based index of the first occurrence of item within the entire System.Collections.Generic.List<T>,
            //     if found; otherwise, –1.
            public override int IndexOf(T1 item)
            {
                return _innerList.IndexOf(item);
            }

            //
            // Summary:
            //     Searches for the specified object and returns the zero-based index of the
            //     first occurrence within the range of elements in the System.Collections.Generic.List<T>
            //     that extends from the specified index to the last element.
            //
            // Parameters:
            //   item:
            //     The object to locate in the System.Collections.Generic.List<T>. The value
            //     can be null for reference types.
            //
            //   index:
            //     The zero-based starting index of the search.
            //
            // Returns:
            //     The zero-based index of the first occurrence of item within the range of
            //     elements in the System.Collections.Generic.List<T> that extends from index
            //     to the last element, if found; otherwise, –1.
            //
            // Exceptions:
            //   System.ArgumentOutOfRangeException:
            //     index is outside the range of valid indexes for the System.Collections.Generic.List<T>.
            public override int IndexOf(T1 item, int index)
            {
                return _innerList.IndexOf(item, index);
            }

            //
            // Summary:
            //     Searches for the specified object and returns the zero-based index of the
            //     first occurrence within the range of elements in the System.Collections.Generic.List<T>
            //     that starts at the specified index and contains the specified number of elements.
            //
            // Parameters:
            //   item:
            //     The object to locate in the System.Collections.Generic.List<T>. The value
            //     can be null for reference types.
            //
            //   index:
            //     The zero-based starting index of the search.
            //
            //   count:
            //     The number of elements in the section to search.
            //
            // Returns:
            //     The zero-based index of the first occurrence of item within the range of
            //     elements in the System.Collections.Generic.List<T> that starts at index and
            //     contains count number of elements, if found; otherwise, –1.
            //
            // Exceptions:
            //   System.ArgumentOutOfRangeException:
            //     index is outside the range of valid indexes for the System.Collections.Generic.List<T>.
            //      -or- count is less than 0.  -or- index and count do not specify a valid
            //     section in the System.Collections.Generic.List<T>.
            public override int IndexOf(T1 item, int index, int count)
            {
                return _innerList.IndexOf(item, index, count);
            }

            //
            // Summary:
            //     Inserts an element into the System.Collections.Generic.List<T> at the specified
            //     index.
            //
            // Parameters:
            //   index:
            //     The zero-based index at which item should be inserted.
            //
            //   item:
            //     The object to insert. The value can be null for reference types.
            //
            // Exceptions:
            //   System.ArgumentOutOfRangeException:
            //     index is less than 0.  -or- index is greater than System.Collections.Generic.List<T>.Count.
            public override void Insert(int index, T1 item)
            {
                CheckFixed();
            }

            //
            // Summary:
            //     Inserts the elements of a collection into the System.Collections.Generic.List<T>
            //     at the specified index.
            //
            // Parameters:
            //   index:
            //     The zero-based index at which the new elements should be inserted.
            //
            //   collection:
            //     The collection whose elements should be inserted into the System.Collections.Generic.List<T>.
            //     The collection itself cannot be null, but it can contain elements that are
            //     null, if type T is a reference type.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     collection is null.
            //
            //   System.ArgumentOutOfRangeException:
            //     index is less than 0.  -or- index is greater than System.Collections.Generic.List<T>.Count.
            public override void InsertRange(int index, IEnumerable<T1> collection)
            {
                if (collection == null) return;

                CheckFixed();
            }

            //
            // Summary:
            //     Searches for the specified object and returns the zero-based index of the
            //     last occurrence within the entire System.Collections.Generic.List<T>.
            //
            // Parameters:
            //   item:
            //     The object to locate in the System.Collections.Generic.List<T>. The value
            //     can be null for reference types.
            //
            // Returns:
            //     The zero-based index of the last occurrence of item within the entire the
            //     System.Collections.Generic.List<T>, if found; otherwise, –1.
            public override int LastIndexOf(T1 item)
            {
                return _innerList.LastIndexOf(item);
            }

            //
            // Summary:
            //     Searches for the specified object and returns the zero-based index of the
            //     last occurrence within the range of elements in the System.Collections.Generic.List<T>
            //     that extends from the first element to the specified index.
            //
            // Parameters:
            //   item:
            //     The object to locate in the System.Collections.Generic.List<T>. The value
            //     can be null for reference types.
            //
            //   index:
            //     The zero-based starting index of the backward search.
            //
            // Returns:
            //     The zero-based index of the last occurrence of item within the range of elements
            //     in the System.Collections.Generic.List<T> that extends from the first element
            //     to index, if found; otherwise, –1.
            //
            // Exceptions:
            //   System.ArgumentOutOfRangeException:
            //     index is outside the range of valid indexes for the System.Collections.Generic.List<T>.
            public override int LastIndexOf(T1 item, int index)
            {
                return _innerList.LastIndexOf(item, index);
            }

            //
            // Summary:
            //     Searches for the specified object and returns the zero-based index of the
            //     last occurrence within the range of elements in the System.Collections.Generic.List<T>
            //     that contains the specified number of elements and ends at the specified
            //     index.
            //
            // Parameters:
            //   item:
            //     The object to locate in the System.Collections.Generic.List<T>. The value
            //     can be null for reference types.
            //
            //   index:
            //     The zero-based starting index of the backward search.
            //
            //   count:
            //     The number of elements in the section to search.
            //
            // Returns:
            //     The zero-based index of the last occurrence of item within the range of elements
            //     in the System.Collections.Generic.List<T> that contains count number of elements
            //     and ends at index, if found; otherwise, –1.
            //
            // Exceptions:
            //   System.ArgumentOutOfRangeException:
            //     index is outside the range of valid indexes for the System.Collections.Generic.List<T>.
            //      -or- count is less than 0.  -or- index and count do not specify a valid
            //     section in the System.Collections.Generic.List<T>.
            public override int LastIndexOf(T1 item, int index, int count)
            {
                return _innerList.LastIndexOf(item, index, count);
            }

            //
            // Summary:
            //     Removes the first occurrence of a specific object from the System.Collections.Generic.List<T>.
            //
            // Parameters:
            //   item:
            //     The object to remove from the System.Collections.Generic.List<T>. The value
            //     can be null for reference types.
            //
            // Returns:
            //     true if item is successfully removed; otherwise, false. This method also
            //     returns false if item was not found in the System.Collections.Generic.List<T>.
            public override bool Remove(T1 item)
            {
                CheckFixed();
                return false;
            }

            //
            // Summary:
            //     Removes the all the elements that match the conditions defined by the specified
            //     predicate.
            //
            // Parameters:
            //   match:
            //     The System.Predicate<T> delegate that defines the conditions of the elements
            //     to remove.
            //
            // Returns:
            //     The number of elements removed from the System.Collections.Generic.List<T>
            //     .
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     match is null.
            public override int RemoveAll(Predicate<T1> match)
            {
                CheckFixed();
                return -1;
            }

            //
            // Summary:
            //     Removes the element at the specified index of the System.Collections.Generic.List<T>.
            //
            // Parameters:
            //   index:
            //     The zero-based index of the element to remove.
            //
            // Exceptions:
            //   System.ArgumentOutOfRangeException:
            //     index is less than 0.  -or- index is equal to or greater than System.Collections.Generic.List<T>.Count.
            public override void RemoveAt(int index)
            {
                CheckFixed();
            }

            //
            // Summary:
            //     Removes a range of elements from the System.Collections.Generic.List<T>.
            //
            // Parameters:
            //   index:
            //     The zero-based starting index of the range of elements to remove.
            //
            //   count:
            //     The number of elements to remove.
            //
            // Exceptions:
            //   System.ArgumentOutOfRangeException:
            //     index is less than 0.  -or- count is less than 0.
            //
            //   System.ArgumentException:
            //     index and count do not denote a valid range of elements in the System.Collections.Generic.List<T>.
            public override void RemoveRange(int index, int count)
            {
                CheckFixed();
            }

            //
            // Summary:
            //     Reverses the order of the elements in the entire System.Collections.Generic.List<T>.
            public override void Reverse()
            {
                _innerList.Reverse();
            }

            //
            // Summary:
            //     Reverses the order of the elements in the specified range.
            //
            // Parameters:
            //   index:
            //     The zero-based starting index of the range to reverse.
            //
            //   count:
            //     The number of elements in the range to reverse.
            //
            // Exceptions:
            //   System.ArgumentOutOfRangeException:
            //     index is less than 0.  -or- count is less than 0.
            //
            //   System.ArgumentException:
            //     index and count do not denote a valid range of elements in the System.Collections.Generic.List<T>.
            public override void Reverse(int index, int count)
            {
                _innerList.Reverse(index, count);
            }

            //
            // Summary:
            //     Sorts the elements in the entire System.Collections.Generic.List<T> using
            //     the default comparer.
            //
            // Exceptions:
            //   System.InvalidOperationException:
            //     The default comparer System.Collections.Generic.Comparer<T>.Default cannot
            //     find an implementation of the System.IComparable<T> generic interface or
            //     the System.IComparable interface for type T.
            public override void Sort()
            {
                _innerList.Sort();
            }

            //
            // Summary:
            //     Sorts the elements in the entire System.Collections.Generic.List<T> using
            //     the specified System.Comparison<T>.
            //
            // Parameters:
            //   comparison:
            //     The System.Comparison<T> to use when comparing elements.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     comparison is null.
            //
            //   System.ArgumentException:
            //     The implementation of comparison caused an error during the sort. For example,
            //     comparison might not return 0 when comparing an item with itself.
            public override void Sort(Comparison<T1> comparison)
            {
                _innerList.Sort(comparison);
            }

            //
            // Summary:
            //     Sorts the elements in the entire System.Collections.Generic.List<T> using
            //     the specified comparer.
            //
            // Parameters:
            //   comparer:
            //     The System.Collections.Generic.IComparer<T> implementation to use when comparing
            //     elements, or null to use the default comparer System.Collections.Generic.Comparer<T>.Default.
            //
            // Exceptions:
            //   System.InvalidOperationException:
            //     comparer is null, and the default comparer System.Collections.Generic.Comparer<T>.Default
            //     cannot find implementation of the System.IComparable<T> generic interface
            //     or the System.IComparable interface for type T.
            //
            //   System.ArgumentException:
            //     The implementation of comparer caused an error during the sort. For example,
            //     comparer might not return 0 when comparing an item with itself.
            public override void Sort(IComparer<T1> comparer)
            {
                _innerList.Sort(comparer);
            }

            //
            // Summary:
            //     Sorts the elements in a range of elements in System.Collections.Generic.List<T>
            //     using the specified comparer.
            //
            // Parameters:
            //   index:
            //     The zero-based starting index of the range to sort.
            //
            //   count:
            //     The length of the range to sort.
            //
            //   comparer:
            //     The System.Collections.Generic.IComparer<T> implementation to use when comparing
            //     elements, or null to use the default comparer System.Collections.Generic.Comparer<T>.Default.
            //
            // Exceptions:
            //   System.ArgumentOutOfRangeException:
            //     index is less than 0.  -or- count is less than 0.
            //
            //   System.ArgumentException:
            //     index and count do not specify a valid range in the System.Collections.Generic.List<T>.
            //      -or- The implementation of comparer caused an error during the sort. For
            //     example, comparer might not return 0 when comparing an item with itself.
            //
            //   System.InvalidOperationException:
            //     comparer is null, and the default comparer System.Collections.Generic.Comparer<T>.Default
            //     cannot find implementation of the System.IComparable<T> generic interface
            //     or the System.IComparable interface for type T.
            public override void Sort(int index, int count, IComparer<T1> comparer)
            {
                _innerList.Sort(index, count, comparer);
            }

            //
            // Summary:
            //     Copies the elements of the System.Collections.Generic.List<T> to a new array.
            //
            // Returns:
            //     An array containing copies of the elements of the System.Collections.Generic.List<T>.
            public override T1[] ToArray()
            {
                return _innerList.ToArray();
            }

            //
            // Summary:
            //     Sets the capacity to the actual number of elements in the System.Collections.Generic.List<T>,
            //     if that number is less than a threshold value.
            public override void TrimExcess()
            {
                _innerList.TrimExcess();
            }

            //
            // Summary:
            //     Determines whether every element in the System.Collections.Generic.List<T>
            //     matches the conditions defined by the specified predicate.
            //
            // Parameters:
            //   match:
            //     The System.Predicate<T> delegate that defines the conditions to check against
            //     the elements.
            //
            // Returns:
            //     true if every element in the System.Collections.Generic.List<T> matches the
            //     conditions defined by the specified predicate; otherwise, false. If the list
            //     has no elements, the return value is true.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     match is null.
            public override bool TrueForAll(Predicate<T1> match)
            {
                return _innerList.TrueForAll(match);
            }


            #endregion List Overloads

            #region ICloneable Members

            public override object Clone()
            {
                return new ChoFixedList<T1>(base.Clone() as ChoList<T1>);
            }

            #endregion

            #region Instance Members (Private)

            private void CheckFixed()
            {
                if (!_silent)
                    throw new ApplicationException("Collection is fixed size.");
            }

            #endregion Instance Members (Private)
        }
    }
}
