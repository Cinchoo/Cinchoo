namespace Cinchoo.Core.Collections.Generic
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Runtime.Serialization;
	using System.Threading;
	using System.Collections;

	#endregion NameSpaces

	public partial class ChoDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDictionary<TKey, TValue>, ICloneable
	{
		#region ChoSynchronizedDictionary Class
		
		private class ChoSynchronizedDictionary<TKey1, TValue1> : ChoDictionary<TKey1, TValue1>, IDictionary<TKey1, TValue1>, ICloneable
		{
			#region Instance Data Members (Private)

			private readonly ChoDictionary<TKey1, TValue1> _dictionary;

			#endregion Instance Data Members (Private)

			#region Constructors

			internal ChoSynchronizedDictionary(ChoDictionary<TKey1, TValue1> dictionary)
				: this(dictionary, dictionary.SyncRoot)
			{
			}

			internal ChoSynchronizedDictionary(ChoDictionary<TKey1, TValue1> dictionary, object syncObject)
			{
				this._dictionary = dictionary;
				this._syncRoot = syncObject;
			}

			#endregion Constructors

			#region Instance Properties (Public)

			public override bool IsUnique
			{
				get { return _dictionary.IsUnique; }
			}

			public override bool IsFixedSize
			{
				get { return _dictionary.IsFixedSize; }
			}

			public override bool IsReadOnly
			{
				get { return _dictionary.IsReadOnly; }
			}

			public override bool IsSynchronized
			{
				get { return true; }
			}

			public override IEqualityComparer<TKey1> Comparer
			{
				get { return _dictionary.Comparer; }
			}

			public override int Count
			{
				get { return _dictionary.Count; }
			}

			public override Dictionary<TKey1, TValue1>.KeyCollection Keys
			{
				get
				{
					throw new NotSupportedException("Cannot get keys on a threadsafe dictionary.  Instead, use ToKeysArray()");
				}
			}

			public override Dictionary<TKey1, TValue1>.ValueCollection Values
			{
				get
				{
					throw new NotSupportedException("Cannot get values on a threadsafe dictionary.  Instead, use ToValuesArray()");
				}
			}

			public override object SyncRoot
			{
				get { return _syncRoot; }
			}

			#endregion Instance Properties (Public)

			#region Indexers (Public)

			public override TValue1 this[TKey1 key]
			{
				get
				{
					lock (_syncRoot)
					{
						return _dictionary[key];
					}
				}
				set
				{
					lock (_syncRoot)
					{
						_dictionary[key] = value;
					}
				}
			}

			#endregion Indexers (Public)

			#region Instance Members (Public)

			public override void AddOrUpdate(TKey1 key, TValue1 value)
			{
				ChoGuard.ArgumentNotNull(key, "Key");
				lock (_syncRoot)
				{
					_dictionary.AddOrUpdate(key, value);
				}
			}

			public override TValue1 GetOrAdd(TKey1 key, TValue1 value)
			{
				ChoGuard.ArgumentNotNull(key, "Key");
				lock (_syncRoot)
				{
					return _dictionary.GetOrAdd(key, value);
				}
			}

			public override void Add(TKey1 key, TValue1 value)
			{
				lock (_syncRoot)
				{
					_dictionary.Add(key, value);
				}
			}

			public override void Clear()
			{
				lock (_syncRoot)
				{
					_dictionary.Clear();
				}
			}

			public override bool ContainsKey(TKey1 key)
			{
				lock (_syncRoot)
				{
					return _dictionary.ContainsKey(key);
				}
			}

			public override bool ContainsValue(TValue1 value)
			{
				lock (_syncRoot)
				{
					return _dictionary.ContainsValue(value);
				}
			}

			public override IEnumerator<KeyValuePair<TKey1, TValue1>> GetEnumerator()
			{
				return new ChoSynchronizedEnumerator<KeyValuePair<TKey1, TValue1>>(_dictionary, _dictionary.SyncRoot);
			}

			public override void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				lock (_syncRoot)
				{
					_dictionary.GetObjectData(info, context);
				}
			}

			public override void OnDeserialization(object sender)
			{
				lock (_syncRoot)
				{
					_dictionary.OnDeserialization(sender);
				}
			}

			public override bool Remove(TKey1 key)
			{
				lock (_syncRoot)
				{
					return _dictionary.Remove(key);
				}
			}

			public override bool TryGetValue(TKey1 key, out TValue1 value)
			{
				lock (_syncRoot)
				{
					return _dictionary.TryGetValue(key, out value);
				}
			}

			#endregion Instance Members (Public)

			#region ToArray Members

			public override TValue1[] ToValuesArray()
			{
				lock (_syncRoot)
				{
					return _dictionary.ToValuesArray();
				}
			}

			public override TKey1[] ToKeysArray()
			{
				lock (_syncRoot)
				{
					return _dictionary.ToKeysArray();
				}
			}

			#endregion ToArray Members

			#region IDictionary<TKey1,TValue1> Members

			void IDictionary<TKey1, TValue1>.Add(TKey1 key, TValue1 value)
			{
				Add(key, value);
			}

			bool IDictionary<TKey1, TValue1>.ContainsKey(TKey1 key)
			{
				return ContainsKey(key);
			}

			ICollection<TKey1> IDictionary<TKey1, TValue1>.Keys
			{
				get { return Keys; }
			}

			bool IDictionary<TKey1, TValue1>.Remove(TKey1 key)
			{
				return Remove(key);
			}

			bool IDictionary<TKey1, TValue1>.TryGetValue(TKey1 key, out TValue1 value)
			{
				return TryGetValue(key, out value);
			}

			ICollection<TValue1> IDictionary<TKey1, TValue1>.Values
			{
				get { return Values; }
			}

			TValue1 IDictionary<TKey1, TValue1>.this[TKey1 key]
			{
				get { return this[key]; }
				set { this[key] = value; }
			}

			#endregion

			#region ICollection<KeyValuePair<TKey1,TValue1>> Members

			void ICollection<KeyValuePair<TKey1, TValue1>>.Add(KeyValuePair<TKey1, TValue1> item)
			{
				ChoGuard.ArgumentNotNull(item, "Item");
				Add(item.Key, item.Value);
			}

			void ICollection<KeyValuePair<TKey1, TValue1>>.Clear()
			{
				Clear();
			}

			bool ICollection<KeyValuePair<TKey1, TValue1>>.Contains(KeyValuePair<TKey1, TValue1> item)
			{
				ChoGuard.ArgumentNotNull(item, "Item");
				return ContainsKey(item.Key) && ContainsValue(item.Value);
			}

			void ICollection<KeyValuePair<TKey1, TValue1>>.CopyTo(KeyValuePair<TKey1, TValue1>[] array, int arrayIndex)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			int ICollection<KeyValuePair<TKey1, TValue1>>.Count
			{
				get { return Count; }
			}

			bool ICollection<KeyValuePair<TKey1, TValue1>>.IsReadOnly
			{
				get { return IsReadOnly; }
			}

			bool ICollection<KeyValuePair<TKey1, TValue1>>.Remove(KeyValuePair<TKey1, TValue1> item)
			{
				ChoGuard.ArgumentNotNull(item, "Item");
				return Remove(item.Key);
			}

			#endregion

			#region IEnumerable<KeyValuePair<TKey1,TValue1>> Members

			IEnumerator<KeyValuePair<TKey1, TValue1>> IEnumerable<KeyValuePair<TKey1, TValue1>>.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			#endregion

			#region ICloneable Members

			public override object Clone()
			{
				return new ChoSynchronizedDictionary<TKey1, TValue1>(_dictionary.Clone() as ChoDictionary<TKey1, TValue1>);
			}

			#endregion
		}

		#endregion ChoSynchronizedDictionary Class
	}
}
