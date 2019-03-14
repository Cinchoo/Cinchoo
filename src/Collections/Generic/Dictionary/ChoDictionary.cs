namespace Cinchoo.Core.Collections.Generic
{
	#region NameSpaces

	using System;
	using System.Text;
	using System.Diagnostics;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Runtime.Serialization;
	using Cinchoo.Core;

	#endregion NameSpaces

	// Summary:
	//     Represents a collection of keys and values.
	[Serializable]
	[ComVisible(false)]
	[DebuggerDisplay("Count = {Count}")]
	public partial class ChoDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDictionary<TKey, TValue>, ICloneable
	{
		#region Instance Data Members (Private)

		private object _syncRoot = new ChoNameableObject();

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoDictionary()
		{
		}

		public ChoDictionary(IDictionary<TKey, TValue> dictionary)
			: base(dictionary)
		{
		}

		public ChoDictionary(IEqualityComparer<TKey> comparer)
			: base(comparer)
		{
		}

		public ChoDictionary(int capacity)
			: base(capacity)
		{
		}

		public ChoDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
			: base(dictionary, comparer)
		{
		}

		public ChoDictionary(int capacity, IEqualityComparer<TKey> comparer)
			: base(capacity, comparer)
		{
		}

		protected ChoDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion Constructors

		#region Instance Properties (Public)

		public virtual bool IsUnique
		{
			get { return false; }
		}

		public virtual bool IsFixedSize
		{
			get { return false; }
		}

		public virtual bool IsReadOnly
		{
			get { return false; }
		}

		public virtual bool IsSynchronized
		{
			get { return false; }
		}

		public new virtual IEqualityComparer<TKey> Comparer
		{
			get { return base.Comparer; }
		}

		public new virtual int Count
		{
			get { return base.Count; }
		}

		public new virtual Dictionary<TKey, TValue>.KeyCollection Keys
		{
			get { return base.Keys; }
		}

		public new virtual Dictionary<TKey, TValue>.ValueCollection Values
		{
			get { return base.Values; }
		}

		public virtual object SyncRoot
		{
			get { return _syncRoot; }
		}

		#endregion Instance Properties (Public)

		#region Indexers (Public)

		public new virtual TValue this[TKey key]
		{
			get { return base[key]; }
			set { base[key] = value; }
		}

		#endregion Indexers (Public)

		#region Instance Members (Public)

		public virtual void AddOrUpdate(TKey key, TValue value)
		{
			ChoGuard.ArgumentNotNull(key, "Key");

			if (ContainsKey(key))
				this[key] = value;
			else
				Add(key, value);
		}

		public virtual TValue GetOrAdd(TKey key, TValue value)
		{
			ChoGuard.ArgumentNotNull(key, "Key");

			if (!ContainsKey(key))
				Add(key, value);

			return this[key];
		}

		public new virtual void Add(TKey key, TValue value)
		{
			base.Add(key, value);
		}

		public new virtual void Clear()
		{
			base.Clear();
		}

		public new virtual bool ContainsKey(TKey key)
		{
			return base.ContainsKey(key);
		}

		public new virtual bool ContainsValue(TValue value)
		{
			return base.ContainsValue(value);
		}

		public new virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return base.GetEnumerator();
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		public override void OnDeserialization(object sender)
		{
			base.OnDeserialization(sender);
		}

		public new virtual bool Remove(TKey key)
		{
			return base.Remove(key);
		}

		public new virtual bool TryGetValue(TKey key, out TValue value)
		{
			return base.TryGetValue(key, out value);
		}

		#endregion Instance Members (Public)

		#region ToArray Members

		public virtual TValue[] ToValuesArray()
		{
			return new List<TValue>(Values).ToArray();
		}

		public virtual TKey[] ToKeysArray()
		{
			return new List<TKey>(Keys).ToArray();
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

		//void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		//{
		//    throw new Exception("The method or operation is not implemented.");
		//}

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

		#region Synchronized Dictionary Members

		public static ChoDictionary<TKey, TValue> Synchronized(ChoDictionary<TKey, TValue> dictionary)
		{
			ChoGuard.ArgumentNotNull(dictionary, "Dictionary");
			return new ChoSynchronizedDictionary<TKey, TValue>(dictionary);
		}

		public static ChoDictionary<TKey, TValue> Synchronized(ChoDictionary<TKey, TValue> dictionary, object syncObject)
		{
			ChoGuard.ArgumentNotNull(dictionary, "Dictionary");
			ChoGuard.ArgumentNotNull(syncObject, "SyncObject");

			return new ChoSynchronizedDictionary<TKey, TValue>(dictionary, syncObject);
		}

		#endregion Synchronized Dictionary Members

		#region Unique Dictionary Members

		public static ChoDictionary<TKey, TValue> Unique(ChoDictionary<TKey, TValue> dictionary)
		{
			ChoGuard.ArgumentNotNull(dictionary, "Dictionary");
			return new ChoUniqueDictionary<TKey, TValue>(dictionary);
		}

		#endregion Unique Dictionary Members

		#region Fixed Dictionary Members

		public static ChoDictionary<TKey, TValue> Fixed(ChoDictionary<TKey, TValue> dictionary)
		{
			ChoGuard.ArgumentNotNull(dictionary, "Dictionary");
			return new ChoFixedDictionary<TKey, TValue>(dictionary);
		}

		#endregion Fixed Dictionary Members

		#region Fixed Dictionary Members

		public static ChoDictionary<TKey, TValue> ReadOnly(ChoDictionary<TKey, TValue> dictionary)
		{
			ChoGuard.ArgumentNotNull(dictionary, "Dictionary");
			return new ChoReadOnlyDictionary<TKey, TValue>(dictionary);
		}

		#endregion Fixed Dictionary Members

		#region ICloneable Members

		public virtual object Clone()
		{
			lock (this.SyncRoot)
			{
				ChoDictionary<TKey, TValue> clonedObject = new ChoDictionary<TKey, TValue>(this);
				return clonedObject;
			}
		}

		#endregion
	}
}
