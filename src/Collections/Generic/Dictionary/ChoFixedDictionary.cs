﻿namespace Cinchoo.Core.Collections.Generic
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Runtime.Serialization;

	#endregion NameSpaces

	public partial class ChoDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDictionary<TKey, TValue>, ICloneable
	{
		#region ChoFixedDictionary Class

		private class ChoFixedDictionary<TKey1, TValue1> : ChoDictionary<TKey1, TValue1>, IDictionary<TKey1, TValue1>, ICloneable
		{
			#region Instance Data Members (Private)

			private readonly ChoDictionary<TKey1, TValue1> _dictionary;

			#endregion Instance Data Members (Private)

			#region Constructors

			internal ChoFixedDictionary(ChoDictionary<TKey1, TValue1> dictionary)
			{
				_dictionary = dictionary;
			}

			#endregion Constructors

			#region Instance Properties (Public)

			public override bool IsUnique
			{
				get { return _dictionary.IsUnique; }
			}

			public override bool IsFixedSize
			{
				get { return true; }
			}

			public override bool IsReadOnly
			{
				get { return _dictionary.IsReadOnly; }
			}

			public override bool IsSynchronized
			{
				get { return _dictionary.IsSynchronized; }
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
				get { return _dictionary.Keys; }
			}

			public override Dictionary<TKey1, TValue1>.ValueCollection Values
			{
				get { return _dictionary.Values; }
			}

			public override object SyncRoot
			{
				get { return _dictionary.SyncRoot; }
			}

			#endregion Instance Properties (Public)

			#region Indexers (Public)

			public override TValue1 this[TKey1 key]
			{
				get { return _dictionary[key]; }
				set { _dictionary[key] = value; }
			}

			#endregion Indexers (Public)

			#region Instance Members (Public)

			public override void Add(TKey1 key, TValue1 value)
			{
				throw new NotSupportedException("NotSupported_FixedSizeDictionary");
			}

			public override void Clear()
			{
				throw new NotSupportedException("NotSupported_FixedSizeDictionary");
			}

			public override bool ContainsKey(TKey1 key)
			{
				return _dictionary.ContainsKey(key);
			}

			public override bool ContainsValue(TValue1 value)
			{
				return _dictionary.ContainsValue(value);
			}

			public override IEnumerator<KeyValuePair<TKey1, TValue1>> GetEnumerator()
			{
				return _dictionary.GetEnumerator();
			}

			public override void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				_dictionary.GetObjectData(info, context);
			}

			public override void OnDeserialization(object sender)
			{
				_dictionary.OnDeserialization(sender);
			}

			public override bool Remove(TKey1 key)
			{
				throw new NotSupportedException("NotSupported_FixedSizeDictionary");
			}

			public override bool TryGetValue(TKey1 key, out TValue1 value)
			{
				return _dictionary.TryGetValue(key, out value);
			}

			#endregion Instance Members (Public)

			#region ToArray Members

			public override TValue1[] ToValuesArray()
			{
				return _dictionary.ToValuesArray();
			}

			public override TKey1[] ToKeysArray()
			{
				return _dictionary.ToKeysArray();
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
				return new ChoFixedDictionary<TKey1, TValue1>(_dictionary.Clone() as ChoDictionary<TKey1, TValue1>);
			}

			#endregion
		}

		#endregion ChoFixedDictionary Class
	}
}
