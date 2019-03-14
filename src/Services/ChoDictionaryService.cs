namespace Cinchoo.Core.Services
{
	#region NameSpaces

	using System;
	using System.Linq;
	using System.Text;
	using System.Diagnostics;
	using System.Collections.Generic;

	#endregion NameSpaces

	[DebuggerDisplay("Name = {_name}")]
	public class ChoDictionaryService : ChoDictionaryService<object, object>
	{
		public ChoDictionaryService(string name)
			: this(name, null, null)
		{
		}

		public ChoDictionaryService(string name, Func<object> defaultValueInstanceFactory)
			: this(name, null, defaultValueInstanceFactory)
		{
		}

		public ChoDictionaryService(string name, Func<object> defaultKeyInstanceFactory, Func<object> defaultValueInstanceFactory) 
			: base(name, defaultKeyInstanceFactory, defaultValueInstanceFactory)
		{
		}
	}

	[DebuggerDisplay("Name = {_name}")]
	public class ChoDictionaryService<TKey, TValue> : IChoDictionaryService<TKey, TValue>, ICloneable<ChoDictionaryService<TKey, TValue>>, IChoMergeable
	{
		#region Instance Data Members (Private)

		private readonly string _name;
		private readonly object _padLock = new object();
		private readonly Dictionary<TKey, TValue> _cacheKeyValues = new Dictionary<TKey, TValue>();
		private readonly Dictionary<TValue, TKey> _cacheValueKeys = new Dictionary<TValue, TKey>();
		private readonly Func<TKey> _defaultKeyInstanceFactory;
		private readonly Func<TValue> _defaultValueInstanceFactory;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoDictionaryService(string name) : this(name, null, null)
		{
		}

		public ChoDictionaryService(string name, Func<TValue> defaultValueInstanceFactory) : this(name, null, defaultValueInstanceFactory)
		{
		}

		public ChoDictionaryService(string name, Func<TKey> defaultKeyInstanceFactory, Func<TValue> defaultValueInstanceFactory)
		{
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

			_name = name;
			_defaultKeyInstanceFactory = defaultKeyInstanceFactory;
			_defaultValueInstanceFactory = defaultValueInstanceFactory;
		}

		#endregion Constructors

		#region IDictionaryService Members

		/// <summary>
		/// Gets the key corresponding to the specified value.
		/// </summary>
		/// <param name="value">The value to look up in the dictionary.</param>
		/// <returns>The associated key, or null if no key exists.</returns>
		public TKey GetKey(TValue value)
		{
			ChoGuard.ArgumentNotNull(value, "Value");

			if (_cacheValueKeys.ContainsKey(value))
				return _cacheValueKeys[value];
			else if (_defaultValueInstanceFactory != null)
			{
				SetValue(_defaultKeyInstanceFactory(), value);
				return _cacheValueKeys[value];
			}
			else
				return default(TKey);
		}

		public TValue this[TKey key]
		{
			get
			{
				return GetValue(key);
			}
			set
			{
				SetValue(key, value);
			}
		}

		/// <summary>
		/// Gets the value corresponding to the specified key.
		/// </summary>
		/// <param name="key">The key to look up the value for.</param>
		/// <returns>The associated value, or null if no value exists.</returns>
		public TValue GetValue(TKey key)
		{
			ChoGuard.ArgumentNotNull(key, "Key");
			
			if (_cacheKeyValues.ContainsKey(key))
				return _cacheKeyValues[key];
			else if (_defaultValueInstanceFactory != null)
			{
				SetValue(key, _defaultValueInstanceFactory());
				return _cacheKeyValues[key];
			}
			else
				return default(TValue);
		}

		/// <summary>
		/// Sets the specified key-value pair.
		/// </summary>
		/// <param name="key">An object to use as the key to associate the value with.</param>
		/// <param name="value">The value to store.</param>
		public void SetValue(TKey key, TValue value)
		{
			ChoGuard.ArgumentNotNull(key, "Key");

			lock (_padLock)
			{
				if (_cacheKeyValues.ContainsKey(key))
				{
					if (_cacheKeyValues[key] != null && _cacheValueKeys.ContainsKey(_cacheKeyValues[key]))
						_cacheValueKeys.Remove(_cacheKeyValues[key]);

					_cacheKeyValues[key] = value;
				}
				else
					_cacheKeyValues.Add(key, value);

				if (value != null)
				{
					if (_cacheValueKeys.ContainsKey(value))
						_cacheValueKeys[value] = key;
					else
						_cacheValueKeys.Add(value, key);
				}
			}
		}

		public void RemoveValue(TKey key)
		{
			ChoGuard.ArgumentNotNull(key, "Key");

			lock (_padLock)
			{
				if (_cacheKeyValues.ContainsKey(key))
				{
					if (_cacheValueKeys.ContainsKey(_cacheKeyValues[key]))
						_cacheValueKeys.Remove(_cacheKeyValues[key]);

					_cacheKeyValues.Remove(key);
				}
			}
		}

		public object SyncRoot
		{
			get { return _padLock; }
		}

		#endregion

		#region Other Members

		public bool ContainsKey(TKey key)
		{
			ChoGuard.ArgumentNotNull(key, "Key");
			return _cacheKeyValues.ContainsKey(key);
		}

		public bool ContainsValue(TValue value)
		{
			ChoGuard.ArgumentNotNull(value, "Value");
			return _cacheValueKeys.ContainsKey(value);
		}

		#endregion Other Members

		#region ICloneable<ChoDictionaryService<TKey,TValue>> Members

		public ChoDictionaryService<TKey, TValue> Clone()
		{
			return InternalClone();
		}

		#endregion

		#region ICloneable Members

		object ICloneable.Clone()
		{
			return InternalClone();
		}

		#endregion

		private ChoDictionaryService<TKey, TValue> InternalClone()
		{
			ChoDictionaryService<TKey, TValue> clonedObject = new ChoDictionaryService<TKey, TValue>("{0}_Clone".FormatString(_name));
			foreach (TKey key in this._cacheKeyValues.Keys)
				clonedObject.SetValue(key, this[key]);

			return clonedObject;
		}

		#region IMergeable Members

		public void Merge(object source)
		{
			if (source == null
				|| !(source is ChoDictionaryService<TKey, TValue>)
				|| Object.ReferenceEquals(this, source))
				return;

			ChoDictionaryService<TKey, TValue> stateInfo = source as ChoDictionaryService<TKey, TValue>;
			foreach (TKey key in stateInfo._cacheKeyValues.Keys)
				SetValue(key, stateInfo[key]);
		}

		#endregion
	}
}
