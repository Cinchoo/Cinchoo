namespace Cinchoo.Core.Collections.Generic
{
	#region NameSpaces

	using System;
	using System.Text;
	using System.Diagnostics;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Runtime.Serialization;
    using System.Collections;
	using Cinchoo.Core.Text;

	#endregion NameSpaces

	// Summary:
	//     Represents a collection of keys and values.
	[Serializable]
	[ComVisible(false)]
	[DebuggerDisplay("Count = {Count}")]
	public partial class ChoNestedDictionary<TKey, TValue> : /* ChoFormattableObject, */ IDictionary<TKey, TValue>, ICloneable
	{
		#region Instance Data Members (Private)

		private readonly object _syncRoot = new object();
		private readonly Dictionary<TKey, ChoTuple<TValue, IDictionary<TKey, TValue>>> _innerDict;
		private readonly IEqualityComparer<TKey> _keyComparer = EqualityComparer<TKey>.Default;
		private readonly IEqualityComparer<TValue> _valueComparer = EqualityComparer<TValue>.Default;
		private readonly TValue _defaultValue = default(TValue);

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoNestedDictionary()
		{
			_innerDict = new Dictionary<TKey, ChoTuple<TValue, IDictionary<TKey, TValue>>>();
		}

		public ChoNestedDictionary(IDictionary<TKey, TValue> dictionary)
		{
			_innerDict = new Dictionary<TKey, ChoTuple<TValue, IDictionary<TKey, TValue>>>();
			AddRange(dictionary);
		}

		public ChoNestedDictionary(IEqualityComparer<TKey> comparer)
		{
			_innerDict = new Dictionary<TKey, ChoTuple<TValue, IDictionary<TKey, TValue>>>(comparer);
			_keyComparer = comparer;
		}

		public ChoNestedDictionary(int capacity)
		{
			_innerDict = new Dictionary<TKey, ChoTuple<TValue, IDictionary<TKey, TValue>>>(capacity);
		}

		public ChoNestedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
			: this(comparer)
		{
			AddRange(dictionary);
		}

		public ChoNestedDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			_innerDict = new Dictionary<TKey, ChoTuple<TValue, IDictionary<TKey, TValue>>>(capacity, comparer);
			_keyComparer = comparer;
		}

		#endregion Constructors

		#region Instance Properties (Public)

		public bool IsUnique
		{
			get { return false; }
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public IEqualityComparer<TKey> Comparer
		{
			get { return _keyComparer; }
		}

		public int Count
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
			get { return _innerDict.Count; }
		}

		public KeyCollection Keys
		{
			get { return new ChoNestedDictionary<TKey, TValue>.KeyCollection(this); }
		}

		public ValueCollection Values
		{
			get { return new ChoNestedDictionary<TKey, TValue>.ValueCollection(this); }
		}

		public object SyncRoot
		{
			get { return _syncRoot; }
		}

		#endregion Instance Properties (Public)

		#region Indexers (Public)

		public TValue this[TKey key]
		{
			get
			{
				ChoGuard.ArgumentNotNull(key, "Key");

				TValue value = _defaultValue;
				if (TryGetValue(key, out value))
					return value;

				throw new KeyNotFoundException();
			}
			set
			{
				ChoGuard.ArgumentNotNull(key, "Key");
				
				if (!SetValue(_innerDict, key, value, null))
					Add(key, value);
			}
		}

		#endregion Indexers (Public)

		#region Instance Members (Public)

		#region AddOrUpdate Overloads

		public void AddOrUpdate(TKey key, TValue value)
		{
			this[key] = value;
		}

		public void AddOrUpdate(IDictionary<TKey, TValue> valueDict)
		{
			CheckCircularDependency(valueDict);

			TKey key = CheckDictForNonEmpty(valueDict);

			if (!SetValue(_innerDict, key, _defaultValue, valueDict))
				AddDictValue(key, valueDict);
		}

		#endregion AddOrUpdate Overloads

		#region GetOrAdd Overloads

		public ChoTuple<TValue, IDictionary<TKey, TValue>> GetOrAdd(TKey key, TValue value)
		{
			ChoGuard.ArgumentNotNull(key, "Key");

			if (!ContainsKey(key))
				Add(key, value);
			else
				_innerDict[key] = new ChoTuple<TValue,IDictionary<TKey,TValue>>(value, null);

			return _innerDict[key];
		}

		public ChoTuple<TValue, IDictionary<TKey, TValue>> GetOrAdd(IDictionary<TKey, TValue> valueDict)
		{
			CheckCircularDependency(valueDict);
			TKey key = CheckDictForNonEmpty(valueDict);

			if (!ContainsKey(key))
				AddDictValue(key, valueDict);
			else
				_innerDict[key] = new ChoTuple<TValue, IDictionary<TKey, TValue>>(default(TValue), valueDict);

			return _innerDict[key];
		}

		#endregion GetOrAdd Overloads

		#region Add Overloads

		public void Add(TKey key, TValue value)
		{
			_innerDict.Add(key, new ChoTuple<TValue, IDictionary<TKey, TValue>>(value, null));
		}

		public void Add(IDictionary<TKey, TValue> valueDict)
		{
			CheckCircularDependency(valueDict);
			TKey key = CheckDictForNonEmpty(valueDict);
			AddDictValue(key, valueDict);
		}

		#endregion Add Overloads

		#region Clear Overloads

		public void Clear()
		{
			_innerDict.Clear();
		}

		#endregion Clear Overloads

		#region Contains Overloads

		public bool Contains(IDictionary<TKey, TValue> dictValue)
		{
			ChoGuard.ArgumentNotNull(dictValue, "DictValue");

			using (ChoNestedDictionary<TKey, TValue>.ChoNestedCollectionDictionaryEnumerator<TKey, TValue> x1 = new ChoNestedDictionary<TKey, TValue>.ChoNestedCollectionDictionaryEnumerator<TKey, TValue>(this))
			{
				while (x1.MoveNext())
				{
					if (x1.Current == dictValue)
						return true;
				}
			}

			return false; 
		}

		public bool ContainsKey(TKey key)
		{
			foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
			{
				if (_keyComparer.Equals(key, keyValuePair.Key))
					return true;
			}

			return false;
		}

		public bool ContainsValue(TValue value)
		{
			foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
			{
				if (_valueComparer.Equals(value, keyValuePair.Value))
					return true;
			}

			return false;
		}

		#endregion Contains Overloads

		public ChoTuple<TValue, IDictionary<TKey, TValue>> GetItem(TKey key)
		{
			using (ChoNestedDictionary<TKey, TValue>.ChoNestedCollectionDictionaryEnumerator<TKey, TValue> x1 = new ChoNestedDictionary<TKey, TValue>.ChoNestedCollectionDictionaryEnumerator<TKey, TValue>(this))
			{
				while (x1.MoveNext())
				{
					if (_keyComparer.Equals(key, x1.Key))
					{
						return x1.ActualValue;
					}
				}
			}

			return ChoTuple<TValue, IDictionary<TKey, TValue>>.Default;
		}

		public bool IsItemValueDictionary(TKey key)
		{
			using (ChoNestedDictionary<TKey, TValue>.ChoNestedCollectionDictionaryEnumerator<TKey, TValue> x1 = new ChoNestedDictionary<TKey, TValue>.ChoNestedCollectionDictionaryEnumerator<TKey, TValue>(this))
			{
				while (x1.MoveNext())
				{
					if (_keyComparer.Equals(key, x1.Key))
						return true;
				}
			}

			return false;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return new ChoNestedCollectionEnumerator<TKey, TValue>(this);
		}

		public bool Remove(IDictionary<TKey, TValue> dictValue)
		{
			ChoGuard.ArgumentNotNull(dictValue, "DictValue");

			using (ChoNestedDictionary<TKey, TValue>.ChoNestedCollectionDictionaryEnumerator<TKey, TValue> x1 = new ChoNestedDictionary<TKey, TValue>.ChoNestedCollectionDictionaryEnumerator<TKey, TValue>(this))
			{
				while (x1.MoveNext())
				{
					if (x1.Current == dictValue)
					{
						if (x1.ParentDictionary is ChoNestedDictionary<TKey, TValue>)
							((ChoNestedDictionary<TKey, TValue>)x1.ParentDictionary)._innerDict.Remove(x1.Key);
						return true;
					}
				}
			}

			return false;
		}

		public bool Remove(TKey key)
		{
			bool found = false;
			bool isCollectionEmpty = false;

			using (ChoNestedCollectionEnumerator<TKey, TValue> enumerator = GetEnumerator() as ChoNestedCollectionEnumerator<TKey, TValue>)
			{
				while (enumerator.MoveNext())
				{
					if (_keyComparer.Equals(key, enumerator.Current.Key))
					{
						found = true;
						break;
					}
				}

				if (found)
				{
					if (enumerator.ContainedDictionary is ChoNestedDictionary<TKey, TValue>)
					{
						ChoNestedDictionary<TKey, TValue> dict = (ChoNestedDictionary<TKey, TValue>)enumerator.ContainedDictionary;
						dict._innerDict.Remove(key);
						isCollectionEmpty = dict._innerDict.Count == 0;
					}
					else
					{
						enumerator.ContainedDictionary.Remove(key);
						isCollectionEmpty = enumerator.ContainedDictionary.Count == 0;
					}
				}

				if (isCollectionEmpty)
				{
					if (enumerator.ParentDictionary is ChoNestedDictionary<TKey, TValue>)
						((ChoNestedDictionary<TKey, TValue>)enumerator.ParentDictionary).Remove(enumerator.ContainedDictionary);
				}
			}

			return found;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			value = _defaultValue;
			foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
			{
				if (_keyComparer.Equals(key, keyValuePair.Key))
				{
					value = keyValuePair.Value;
					return true;
				}
			}

			return false;
		}

		#endregion Instance Members (Public)

		#region ToArray Members

		public TValue[] ToValuesArray()
		{
			List<TValue> values = new List<TValue>(Values);
			return values.ToArray();
		}

		public TKey[] ToKeysArray()
		{
			List<TKey> keys = new List<TKey>(Keys);
			return keys.ToArray();
		}

		#endregion ToArray Members

		#region IDictionary<TKey,TValue> Members

		void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		{
			Add(key, value);
		}

		bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
		{
			return ContainsKey(key);
		}

		ICollection<TKey> IDictionary<TKey, TValue>.Keys
		{
			get { return Keys; }
		}

		bool IDictionary<TKey, TValue>.Remove(TKey key)
		{
			return Remove(key);
		}

		bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
		{
			return TryGetValue(key, out value);
		}

		ICollection<TValue> IDictionary<TKey, TValue>.Values
		{
			get { return Values; }
		}

		TValue IDictionary<TKey, TValue>.this[TKey key]
		{
			get { return this[key]; }
			set { this[key] = value; }
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			ChoGuard.ArgumentNotNull(item, "Item");
			Add(item.Key, item.Value);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		{
			Clear();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			ChoGuard.ArgumentNotNull(item, "Item");
			return ContainsKey(item.Key) && ContainsValue(item.Value);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		int ICollection<KeyValuePair<TKey, TValue>>.Count
		{
			get { return Count; }
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get { return IsReadOnly; }
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			ChoGuard.ArgumentNotNull(item, "Item");
			return Remove(item.Key);
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return Clone(this);
		}

		private IDictionary<TKey, TValue> Clone(IEnumerable<KeyValuePair<TKey,TValue>> inDict)
		{
			throw new NotImplementedException();
			//if (inDict is ChoNestedDictionary<TKey, TValue>)
			//{
			//    ChoNestedDictionary<TKey, TValue> outDict = new ChoNestedDictionary<TKey, TValue>();

			//    foreach (ChoTuple<TValue, IDictionary<TKey, TValue>> tuple in ((ChoNestedDictionary<TKey, TValue>)inDict)._innerDict)
			//    {
			//        if (tuple.Second == null)
			//        {
			//            if (tuple.First == null)
			//                outDict.Add(default(T));
			//            else if (tuple.First is ICloneable)
			//                outDict.Add((T)((ICloneable)tuple.First).Clone());
			//            else
			//                throw new ApplicationException("One of the list item is not a Clonable object.");
			//        }
			//        else
			//            outDict.Add(Clone(tuple.Second));
			//    }

			//    return outDict;
			//}
			//else if (inDict is IDictionary<TKey, TValue>)
			//{
			//    Dictionary<TKey, TValue> subOutList = new Dictionary<TKey, TValue>();

			//    foreach (KeyValuePair<TKey, TValue> item in inDict)
			//    {
			//        if (item is ICloneable)
			//            subOutList.Add((T)((ICloneable)item).Clone());
			//        else
			//            throw new ApplicationException("One of the list item is not a Clonable object.");
			//    }

			//    return subOutList;
			//}
			//else
			//    throw new ApplicationException("Unsupported list item found.");
		}
		#endregion

		#region Instance Members (Private)

		private static TKey CheckDictForNonEmpty(IDictionary<TKey, TValue> value)
		{
			ChoGuard.ArgumentNotNull(value, "Value");
			if (value.Count == 0)
				throw new ApplicationException("Dictionary can't be empty.");

			foreach (KeyValuePair<TKey, TValue> keyValuePair in value)
				return keyValuePair.Key;

			return default(TKey);
		}

		private void AddDictValue(TKey key, IDictionary<TKey, TValue> valueDict)
		{
			_innerDict.Add(key, new ChoTuple<TValue, IDictionary<TKey, TValue>>(_defaultValue, valueDict));
		}
		
		private void CheckCircularDependency(IDictionary<TKey, TValue> valueDict)
		{
			if (valueDict is ChoNestedDictionary<TKey, TValue>)
			{
				if (((ChoNestedDictionary<TKey, TValue>)valueDict).Contains(this))
					throw new InvalidOperationException("Can't add item to the dictionary, because it'll makes circular reference.");

				if (CheckCircularDependency(this, valueDict as ChoNestedDictionary<TKey, TValue>))
					throw new InvalidOperationException("Can't add item to the dictionary, because it'll makes circular reference.");
			}
		}

		private bool CheckCircularDependency(ChoNestedDictionary<TKey, TValue> parentDict, ChoNestedDictionary<TKey, TValue> valueDict)
		{
			return false;
		}

		private void AddRange(IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary is ChoNestedDictionary<TKey, TValue>)
			{
				foreach (KeyValuePair<TKey, ChoTuple<TValue, IDictionary<TKey, TValue>>> keyValuePair in ((ChoNestedDictionary<TKey, TValue>)dictionary)._innerDict)
					_innerDict.Add(keyValuePair.Key, keyValuePair.Value);
			}
			else
			{
				foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
					_innerDict.Add(keyValuePair.Key, new ChoTuple<TValue, IDictionary<TKey, TValue>>(keyValuePair.Value, null));
			}
		}

		private bool SetValue(Dictionary<TKey, ChoTuple<TValue, IDictionary<TKey, TValue>>> dict, TKey key, TValue value, IDictionary<TKey, TValue> dictValue)
		{
			if (dict.ContainsKey(key))
			{
				if (dictValue != null)
				{
					CheckCircularDependency(dictValue);

					dict[key].Second = dictValue;
					dict[key].First = default(TValue);
				}
				else
				{
					dict[key].Second = null;
					dict[key].First = value;
				}

				return true;
			}

			foreach (TKey key1 in dict.Keys)
			{
				if (dict[key1].Second == null)
					continue;

				if (dict[key1].Second is ChoNestedDictionary<TKey, TValue>)
				{
					if (SetValue(((ChoNestedDictionary<TKey, TValue>)dict[key1].Second)._innerDict, key, value, dictValue))
						return true;
				}
				else
				{
					IDictionary<TKey, TValue> subDict = (IDictionary<TKey, TValue>)dict[key1].Second;
					if (subDict != null && subDict.ContainsKey(key))
					{
						if (dictValue != null)
							throw new ApplicationException("Can't set dictionary to IDictionary.");
						else
							subDict[key] = value;

						return true;
					}
				}
			}

			return false;
		}

		#endregion Instance Members (Private)

		#region ChoNestedCollectionEnumerator Class

		private sealed class ChoNestedCollectionEnumerator<TKey1, TValue1> : IDisposable, IEnumerator<KeyValuePair<TKey1, TValue1>>
		{
			#region Instance Data Members (Private)

			private readonly Predicate<KeyValuePair<TKey1, TValue1>> _match = item => true;
			private KeyValuePair<TKey1, TValue1> _current;
			private KeyValuePair<TKey1, ChoTuple<TValue1, IDictionary<TKey1, TValue1>>> _actualValue;
			private ChoTuple<IDictionary<TKey1, TValue1>, IDictionary<TKey1, TValue1>> _collectionPair;
			private readonly Stack<ChoTuple<IDictionary<TKey1, TValue1>, IDictionary<TKey1, TValue1>>> _collectionPairs = new Stack<ChoTuple<IDictionary<TKey1, TValue1>, IDictionary<TKey1, TValue1>>>();
			private readonly Stack<ChoTuple<IEnumerator<KeyValuePair<TKey1, ChoTuple<TValue1, IDictionary<TKey1, TValue1>>>>, IEnumerator<KeyValuePair<TKey1, TValue1>>>> _collectionStack =
				new Stack<ChoTuple<IEnumerator<KeyValuePair<TKey1,ChoTuple<TValue1,IDictionary<TKey1,TValue1>>>>,IEnumerator<KeyValuePair<TKey1,TValue1>>>>();

			#endregion Instance Data Members (Private)
			
			#region Constructors

			public ChoNestedCollectionEnumerator(ChoNestedDictionary<TKey1, TValue1> nestedDictionary)
				: this(nestedDictionary, item => true)
			{
			}

			public ChoNestedCollectionEnumerator(ChoNestedDictionary<TKey1, TValue1> nestedDictionary, Predicate<KeyValuePair<TKey1, TValue1>> match)
			{
				ChoGuard.ArgumentNotNull(nestedDictionary, "NestedDictionary");
				
				if (match != null) _match = match;
				_collectionStack.Push(new ChoTuple<IEnumerator<KeyValuePair<TKey1,ChoTuple<TValue1,IDictionary<TKey1,TValue1>>>>,IEnumerator<KeyValuePair<TKey1,TValue1>>>(
					nestedDictionary._innerDict.GetEnumerator(), null));

				_collectionPairs.Push(new ChoTuple<IDictionary<TKey1, TValue1>, IDictionary<TKey1, TValue1>>(nestedDictionary as IDictionary<TKey1, TValue1>, null));
				_collectionPair = _collectionPairs.Peek();
			}

			#endregion Constructors

			#region IDisposable Overrides

			public void Dispose()
			{
			}

			#endregion IDisposable Overrides

			#region IEnumerator<T1> Members

			public KeyValuePair<TKey1, TValue1> Current
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
				foreach (ChoTuple<IEnumerator<KeyValuePair<TKey1, ChoTuple<TValue1, IDictionary<TKey1, TValue1>>>>, IEnumerator<KeyValuePair<TKey1, TValue1>>>
					tuple in _collectionStack)
				{
					if (tuple.First != null)
						tuple.First.Reset();
					if (tuple.Second != null)
						tuple.Second.Reset();
				}

				_collectionStack.Clear();
				_collectionPairs.Clear();
				_collectionPair = null;
				_current = default(KeyValuePair<TKey1, TValue1>);
			}

			#endregion

			#region Instance Members (Private)

			private bool Move2NextItem()
			{
				while (true)
				{
					if (_collectionStack.Count == 0)
					{
						_current = default(KeyValuePair<TKey1, TValue1>);
						return false;
					}
					else if (Move2NextItem(_collectionStack.Peek(), _collectionPairs.Peek()))
						return true;
				}
			}

			private bool Move2NextItem(ChoTuple<IEnumerator<KeyValuePair<TKey1, ChoTuple<TValue1, IDictionary<TKey1, TValue1>>>>, IEnumerator<KeyValuePair<TKey1, TValue1>>> tuple,
				ChoTuple<IDictionary<TKey1, TValue1>, IDictionary<TKey1, TValue1>> collectionPair)
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
						if (tuple.First.Current.Value != null && tuple.First.Current.Value.Second != null)
						{
							if (tuple.First.Current.Value.Second is ChoNestedDictionary<TKey1, TValue1>)
								_collectionStack.Push(new ChoTuple<IEnumerator<KeyValuePair<TKey1, ChoTuple<TValue1, IDictionary<TKey1, TValue1>>>>, IEnumerator<KeyValuePair<TKey1, TValue1>>>(
									((ChoNestedDictionary<TKey1, TValue1>)tuple.First.Current.Value.Second)._innerDict.GetEnumerator(), null));
							else if (tuple.First.Current.Value.Second is Dictionary<TKey1, TValue1>)
								_collectionStack.Push(new ChoTuple<IEnumerator<KeyValuePair<TKey1, ChoTuple<TValue1, IDictionary<TKey1, TValue1>>>>, IEnumerator<KeyValuePair<TKey1, TValue1>>>(
									null, ((Dictionary<TKey1, TValue1>)tuple.First.Current.Value.Second).GetEnumerator()));

							_collectionPairs.Push(new ChoTuple<IDictionary<TKey1, TValue1>, IDictionary<TKey1, TValue1>>(tuple.First.Current.Value.Second as IDictionary<TKey1, TValue1>, 
								collectionPair.First != null ? collectionPair.First : collectionPair.Second));
							_collectionPair = _collectionPairs.Peek();

							return Move2NextItem();
						}
						else
						{
							KeyValuePair<TKey1, TValue1> keyValuePair = new KeyValuePair<TKey1,TValue1>(tuple.First.Current.Key, tuple.First.Current.Value != null ? tuple.First.Current.Value.First : default(TValue1));
							if (_match(keyValuePair))
							{
								_current = keyValuePair;
								_actualValue = tuple.First.Current;
								return true;
							}
							else
								return false;
						}
					}
					else if (tuple.Second != null)
					{
						if (_match(tuple.Second.Current))
						{
							_current = tuple.Second.Current;
							_actualValue = new KeyValuePair<TKey1, ChoTuple<TValue1, IDictionary<TKey1, TValue1>>>(tuple.Second.Current.Key, new ChoTuple<TValue1, IDictionary<TKey1, TValue1>>(default(TValue1), _collectionPairs.Peek().Second as IDictionary<TKey1, TValue1>));
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

			public IDictionary<TKey1, TValue1> ContainedDictionary
			{
				get { return _collectionPair.First; }
			}

			public IDictionary<TKey1, TValue1> ParentDictionary
			{
				get { return _collectionPair.Second; }
			}

			public KeyValuePair<TKey1, ChoTuple<TValue1, IDictionary<TKey1, TValue1>>> ActualValue
			{
				get { return _actualValue; }
			}
		}

		#endregion ChoNestedCollectionEnumerator Class

		#region ChoNestedCollectionDictionaryEnumerator Class

		private sealed class ChoNestedCollectionDictionaryEnumerator<TKey1, TValue1> : IDisposable, IEnumerator<IDictionary<TKey1, TValue1>>
		{
			#region Instance Data Members (Private)

			private readonly Predicate<IDictionary<TKey1, TValue1>> _match = item => true;
			private IDictionary<TKey1, TValue1> _current;
			private ChoTuple<TValue1, IDictionary<TKey1, TValue1>> _actualValue;
			private TKey1 _key;
			private ChoTuple<IDictionary<TKey1, TValue1>, IDictionary<TKey1, TValue1>> _collectionPair;
			private readonly Stack<ChoTuple<IDictionary<TKey1, TValue1>, IDictionary<TKey1, TValue1>>> _collectionPairs = new Stack<ChoTuple<IDictionary<TKey1, TValue1>, IDictionary<TKey1, TValue1>>>();
			private readonly Stack<IEnumerator> _collectionStack = new Stack<IEnumerator>();

			#endregion Instance Data Members (Private)

			#region Constructors

			public ChoNestedCollectionDictionaryEnumerator(ChoNestedDictionary<TKey1, TValue1> nestedDictionary)
				: this(nestedDictionary, item => true)
			{
			}

			public ChoNestedCollectionDictionaryEnumerator(ChoNestedDictionary<TKey1, TValue1> nestedDictionary, Predicate<IDictionary<TKey1, TValue1>> match)
			{
				ChoGuard.ArgumentNotNull(nestedDictionary, "NestedDictionary");
				ChoGuard.ArgumentNotNull(match, "match");

				_match = match;
				_collectionStack.Push(nestedDictionary._innerDict.GetEnumerator());

				_collectionPairs.Push(new ChoTuple<IDictionary<TKey1, TValue1>, IDictionary<TKey1, TValue1>>(nestedDictionary as IDictionary<TKey1, TValue1>, null));
				_collectionPair = _collectionPairs.Peek();
			}

			#endregion Constructors

			#region IDisposable Overrides

			public void Dispose()
			{
			}

			#endregion IDisposable Overrides

			#region IEnumerator<T1> Members

			public IDictionary<TKey1, TValue1> Current
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
				foreach (IEnumerator collection in _collectionStack)
					collection.Reset();

				_collectionStack.Clear();
				_collectionPairs.Clear();
				_collectionPair = null;
				_current = default(IDictionary<TKey1, TValue1>);
			}

			#endregion

			#region Instance Members (Private)

			private bool Move2NextItem()
			{
				while (true)
				{
					if (_collectionStack.Count == 0)
					{
						_current = default(ChoNestedDictionary<TKey1, TValue1>);
						return false;
					}
					else if (Move2NextItem(_collectionStack.Peek(), _collectionPairs.Peek()))
						return true;
				}
			}

			private bool Move2NextItem(IEnumerator collection,
				ChoTuple<IDictionary<TKey1, TValue1>, IDictionary<TKey1, TValue1>> collectionPair)
			{
				bool retVal = false;

				while (true)
				{
					retVal = collection.MoveNext();
					if (!retVal)
						break;

					if (collection.Current is KeyValuePair<TKey1, TValue1>)
						break;

					KeyValuePair<TKey1, ChoTuple<TValue1, IDictionary<TKey1, TValue1>>> keyValuePair = (KeyValuePair<TKey1, ChoTuple<TValue1, IDictionary<TKey1, TValue1>>>)collection.Current;
					ChoTuple<TValue1, IDictionary<TKey1, TValue1>> collectionValue = keyValuePair.Value;

					if (collectionValue.Second != null && _match(collectionValue.Second))
					{
						retVal = true;
						_current = collectionValue.Second;
						_key = keyValuePair.Key;
						_actualValue = collectionValue;

						if (_current is ChoNestedDictionary<TKey1, TValue1>)
							_collectionStack.Push(((ChoNestedDictionary<TKey1, TValue1>)_current)._innerDict.GetEnumerator());
						else if (_current is IDictionary<TKey1, TValue1>)
							_collectionStack.Push(((IDictionary<TKey1, TValue1>)_current).GetEnumerator());

						_collectionPairs.Push(new ChoTuple<IDictionary<TKey1, TValue1>, IDictionary<TKey1, TValue1>>(_current,
							collectionPair.First != null ? collectionPair.First : collectionPair.Second));
						_collectionPair = _collectionPairs.Peek();

						break;
					}
				}

				if (!retVal)
				{
					_collectionStack.Pop();
					_collectionPairs.Pop();
					_collectionPair = _collectionPairs.Count > 0 ? _collectionPairs.Peek() : null;
					
					return Move2NextItem();
				}

				return retVal;
			}

			#endregion Instance Members (Private)

			public TKey1 Key
			{
				get { return _key; }
			}

			public ChoTuple<TValue1, IDictionary<TKey1, TValue1>> ActualValue
			{
				get { return _actualValue; }
			}

			public IDictionary<TKey1, TValue1> ContainedDictionary
			{
				get { return _collectionPair.First; }
			}

			public IDictionary<TKey1, TValue1> ParentDictionary
			{
				get { return _collectionPair.Second; }
			}
		}

		#endregion ChoNestedCollectionDictionaryEnumerator Class

		#region KeyCollection Class

		[DebuggerDisplay("Count = {Count}")]
		[Serializable()]
		public sealed class KeyCollection : ICollection<TKey>, ICollection
		{
			#region Instance Data Members (Private)

			private ChoNestedDictionary<TKey, TValue> _dictionary;

			#endregion Instance Data Members (Private)

			#region Constructors

			public KeyCollection(ChoNestedDictionary<TKey, TValue> dictionary)
			{
				ChoGuard.ArgumentNotNull(dictionary, "Dictionary");
				this._dictionary = dictionary;
			}

			#endregion Constructors

			#region Instance Members (Public)

			public Enumerator GetEnumerator()
			{
				return new Enumerator(_dictionary);
			}

			public void CopyTo(TKey[] array, int index)
			{
				ChoGuard.ArgumentNotNull(array, "Array");

				if (index < 0 || index > array.Length)
					throw new IndexOutOfRangeException("index");

				if (array.Length - index < _dictionary.Count)
					throw new ArgumentException("Array is small to copy");

				foreach (TKey key in _dictionary.Keys)
					array[index++] = key;
			}

			public int Count
			{
				get { return _dictionary.Count; }
			}

			bool ICollection<TKey>.IsReadOnly
			{
				get { return true; }
			}

			void ICollection<TKey>.Add(TKey item)
			{
				throw new NotSupportedException();
			}

			void ICollection<TKey>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<TKey>.Contains(TKey item)
			{
				return _dictionary.ContainsKey(item);
			}

			bool ICollection<TKey>.Remove(TKey item)
			{
				throw new NotSupportedException();
			}

			IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
			{
				return new Enumerator(_dictionary);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new Enumerator(_dictionary);
			}

			void ICollection.CopyTo(Array array, int index)
			{
				ChoGuard.ArgumentNotNull(array, "Array");

				if (array.Rank != 1)
					throw new ArgumentException("Multi-Dimentional array not supported.");

				if (array.GetLowerBound(0) != 0)
					throw new ArgumentException("Non-zero lower bound array not supported.");

				if (index < 0 || index > array.Length)
					throw new IndexOutOfRangeException("index");

				if (array.Length - index < _dictionary.Count)
					throw new ArgumentException("Array is small to copy");

				TKey[] keys = array as TKey[];
				if (keys != null)
				{
					CopyTo(keys, index);
				}
				else
				{
					object[] objects = array as object[];
					if (objects == null)
						throw new ArgumentException("Invalid array type passed.");

					foreach (TKey key in _dictionary.Keys)
						objects[index++] = key;
				}
			}

			bool ICollection.IsSynchronized
			{
				get { return false; }
			}

			Object ICollection.SyncRoot
			{
				get { return ((ICollection)_dictionary).SyncRoot; }
			}

			#endregion Instance Members (Public)

			#region Enumerator Struct

			[Serializable()]
			public struct Enumerator : IEnumerator<TKey>, System.Collections.IEnumerator
			{
				#region Instance Data Members (Private)

				private IEnumerator<KeyValuePair<TKey, TValue>> _enumerator;
				private TKey _currentKey;

				#endregion Instance Data Members (Private)

				#region Constructors

				internal Enumerator(ChoNestedDictionary<TKey, TValue> dictionary)
				{
					_enumerator = dictionary.GetEnumerator();
					_currentKey = default(TKey);
				}

				#endregion Constructors

				#region IDisposable Members

				public void Dispose()
				{
					_currentKey = default(TKey);
				}

				#endregion IDisposable Members

				#region IEnumerator Members

				public bool MoveNext() 
				{
					if (!_enumerator.MoveNext())
						return false;

					_currentKey = _enumerator.Current.Key;
                    return true;
                }

				public TKey Current
				{
					get { return _currentKey; }
				}

				Object System.Collections.IEnumerator.Current
				{
					get { return _currentKey; }
				}

				void System.Collections.IEnumerator.Reset()
				{
					Dispose();
				}

				#endregion IEnumerator Members
			}

			#endregion Enumerator Struct
		}

		#endregion KeyCollection Class

		#region ValueCollection Class

		[DebuggerDisplay("Count = {Count}")]
		[Serializable()]
		public sealed class ValueCollection : ICollection<TValue>, ICollection
		{
			#region Instance Data Members (Private)

			private ChoNestedDictionary<TKey, TValue> _dictionary;

			#endregion Instance Data Members (Private)

			public ValueCollection(ChoNestedDictionary<TKey, TValue> dictionary)
			{
				ChoGuard.ArgumentNotNull(dictionary, "Dictionary");
				this._dictionary = dictionary;
			}

			public Enumerator GetEnumerator()
			{
				return new Enumerator(_dictionary);
			}

			public void CopyTo(TValue[] array, int index)
			{
				ChoGuard.ArgumentNotNull(array, "Array");

				if (index < 0 || index > array.Length)
					throw new IndexOutOfRangeException("index");

				if (array.Length - index < _dictionary.Count)
					throw new ArgumentException("Array is small to copy");

				foreach (TValue value in _dictionary.Values)
					array[index++] = value;
			}

			public int Count
			{
				get { return _dictionary.Count; }
			}

			bool ICollection<TValue>.IsReadOnly
			{
				get { return true; }
			}

			void ICollection<TValue>.Add(TValue item)
			{
				throw new NotSupportedException();
			}

			bool ICollection<TValue>.Remove(TValue item)
			{
				throw new NotSupportedException();
			}

			void ICollection<TValue>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<TValue>.Contains(TValue item)
			{
				return _dictionary.ContainsValue(item);
			}

			IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
			{
				return new Enumerator(_dictionary);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new Enumerator(_dictionary);
			}

			void ICollection.CopyTo(Array array, int index)
			{
				ChoGuard.ArgumentNotNull(array, "Array");

				if (array.Rank != 1)
					throw new ArgumentException("Multi-Dimentional array not supported.");

				if (array.GetLowerBound(0) != 0)
					throw new ArgumentException("Non-zero lower bound array not supported.");

				if (index < 0 || index > array.Length)
					throw new IndexOutOfRangeException("index");

				if (array.Length - index < _dictionary.Count)
					throw new ArgumentException("Array is small to copy");


				TValue[] values = array as TValue[];
				if (values != null)
				{
					CopyTo(values, index);
				}
				else
				{
					object[] objects = array as object[];
					if (objects == null)
						throw new ArgumentException("Invalid array type passed.");

					foreach (TValue value in _dictionary.Values)
						objects[index++] = value;
				}
			}

			bool ICollection.IsSynchronized
			{
				get { return false; }
			}

			Object ICollection.SyncRoot
			{
				get { return ((ICollection)_dictionary).SyncRoot; }
			}

			#region Enumerator Struct

			[Serializable()]
			public struct Enumerator : IEnumerator<TValue>, System.Collections.IEnumerator
			{
				#region Instance Data Members (Private)

				private IEnumerator<KeyValuePair<TKey, TValue>> _enumerator;
				private TValue _currentValue;

				#endregion Instance Data Members (Private)

				internal Enumerator(ChoNestedDictionary<TKey, TValue> dictionary)
				{
					_enumerator = dictionary.GetEnumerator();
					_currentValue = default(TValue);
				}

				public void Dispose()
				{
					_currentValue = default(TValue);
				}

				public bool MoveNext()
				{
					if (!_enumerator.MoveNext())
						return false;

					_currentValue = _enumerator.Current.Value;
					return true;
				}

				public TValue Current
				{
					get { return _currentValue; }
				}

				Object System.Collections.IEnumerator.Current
				{
					get { return _currentValue; }
				}

				void System.Collections.IEnumerator.Reset()
				{
					Dispose();
				}
			}

			#endregion Enumerator Struct
		}

		#endregion ValueCollection Class

		#region Object Overrides (Public)

		public /* override */ string ToString(string formatName)
		{
			if (formatName == "F")
			{
				return FormattedString(this);
			}
			else
			{
				ChoStringMsgBuilder msg = new ChoStringMsgBuilder("ChoNestedDictionory Values");

				foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
					msg.AppendFormatLine("{0} - {1}", keyValuePair.Key, keyValuePair.Value);

				return msg.ToString();
			}
		}

		private string FormattedString(ChoNestedDictionary<TKey, TValue> dict)
		{
			ChoStringMsgBuilder msg = new ChoStringMsgBuilder();

			foreach (KeyValuePair<TKey, ChoTuple<TValue, IDictionary<TKey, TValue>>> KeyValuePair1 in dict._innerDict)
			{
				if (KeyValuePair1.Value.Second != null)
				{
					if (KeyValuePair1.Value.Second is ChoNestedDictionary<TKey, TValue>)
						msg.AppendFormatLine(FormattedString(KeyValuePair1.Value.Second as ChoNestedDictionary<TKey, TValue>));
					else
					{
						ChoStringMsgBuilder msg1 = new ChoStringMsgBuilder();
						foreach (KeyValuePair<TKey, TValue> keyValuePair in (IDictionary<TKey, TValue>)KeyValuePair1.Value.Second)
							msg1.AppendFormatLine("{0} - {1}", keyValuePair.Key, keyValuePair.Value);

						msg.AppendFormatLine(msg1.ToString());
					}
				}
				else
					msg.AppendFormatLine("{0} - {1}", KeyValuePair1.Key, KeyValuePair1.Value.First);
			}

			return msg.ToString();
		}

		public override string ToString()
		{
			return ToString(null);
		}

		#endregion Object Overrides (Public)
	}
}
