namespace Cinchoo.Core.Services
{
    #region NameSpaces

    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.ComponentModel.Design;

    using Cinchoo.Core;
    using Cinchoo.Core.Services;

    #endregion NameSpaces

    [Flags]
    public enum ChoWeakDictionaryServiceClean
    {
        Never = 0x00,
        WhenKeyObjectDeadOnly = 0x01,
        WhenValueObjectDeadOnly = 0x02,
        All = WhenKeyObjectDeadOnly | WhenValueObjectDeadOnly
    }

    [DebuggerDisplay("Name = {_name}")]
    public class ChoWeakDictionaryService : ChoWeakDictionaryService<object, object>
    {
        public ChoWeakDictionaryService(string name)
            : base(name)
        {
        }
    }

    [DebuggerDisplay("Name = {_name}")]
    public class ChoWeakDictionaryService<TKey, TValue> : ChoSyncDisposableObject, IChoDictionaryService<TKey, TValue>
        where TKey : class
        where TValue : class
    {
        #region Instance Data Members (Private)

        private readonly string _name;
        private readonly object _padLock = new object();
        private readonly ChoWeakDictionaryServiceClean _weakDictionaryServiceClean = ChoWeakDictionaryServiceClean.WhenKeyObjectDeadOnly;
        private readonly Dictionary<ChoWeakReference<TKey>, TValue> _cacheKeyValues = new Dictionary<ChoWeakReference<TKey>, TValue>();
        private readonly Dictionary<TValue, ChoWeakReference<TKey>> _cacheValueKeys = new Dictionary<TValue,ChoWeakReference<TKey>>();
        private readonly ChoTimerService<object> _timerService;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoWeakDictionaryService(string name) : this(name, 1000, 5000)
        {
        }

        public ChoWeakDictionaryService(string name, ChoWeakDictionaryServiceClean weakDictionaryServiceClean)
            : this(name, 1000, 5000, weakDictionaryServiceClean)
        {
        }

        public ChoWeakDictionaryService(string name, int dueTime, int period) 
            : this(name, dueTime, period, ChoWeakDictionaryServiceClean.WhenKeyObjectDeadOnly)
        {
        }

        public ChoWeakDictionaryService(string name, int dueTime, int period, ChoWeakDictionaryServiceClean weakDictionaryServiceClean)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

            _name = name;

            if (_weakDictionaryServiceClean != ChoWeakDictionaryServiceClean.Never)
            {
                _timerService = new ChoTimerService<object>(String.Format("{0}_WeakDictionaryServiceTimer", name),
                    OnTimerServiceCallback, null, period, true);

                _timerService.Silent = true;
                _timerService.Timeout = System.Threading.Timeout.Infinite;
            }
        }

        #endregion Constructors

        #region ChoTimerService Callback Method

        private void OnTimerServiceCallback(object state)
        {
            lock (_padLock)
            {
                if ((_weakDictionaryServiceClean & ChoWeakDictionaryServiceClean.WhenKeyObjectDeadOnly) == ChoWeakDictionaryServiceClean.WhenKeyObjectDeadOnly)
                {
                    List<ChoWeakReference<TKey>> listKeyItemsToBeRemoved = new List<ChoWeakReference<TKey>>();
                    foreach (ChoWeakReference<TKey> wrKey in _cacheKeyValues.Keys)
                    {
                        if (wrKey.IsAlive) continue;

                        if (_cacheValueKeys.ContainsKey(_cacheKeyValues[wrKey]))
                            _cacheValueKeys.Remove(_cacheKeyValues[wrKey]);

                        listKeyItemsToBeRemoved.Add(wrKey);
                    }

                    _cacheKeyValues.RemoveAllMatchingKeyItems(listKeyItemsToBeRemoved);
                }
                if ((_weakDictionaryServiceClean & ChoWeakDictionaryServiceClean.WhenValueObjectDeadOnly) == ChoWeakDictionaryServiceClean.WhenValueObjectDeadOnly)
                {
                    List<TValue> listValueItemsToBeRemoved = new List<TValue>();
                    foreach (ChoWeakReference<TValue> wrValue in _cacheValueKeys.Keys)
                    {
                        if (wrValue.IsAlive) continue;

                        if (_cacheKeyValues.ContainsKey(_cacheValueKeys[wrValue]))
                            _cacheKeyValues.Remove(_cacheValueKeys[wrValue]);

                        listValueItemsToBeRemoved.Add(wrValue);
                    }
                    _cacheValueKeys.RemoveAllMatchingKeyItems(listValueItemsToBeRemoved);
                }
            }
        }

        #endregion ChoTimerService Callback Method

        #region IDictionaryService Members

		public bool ContainsKey(TKey key)
		{
			ChoGuard.ArgumentNotNull(key, "Key");
			ValidateInputs(key, null);

			ChoWeakReference<TKey> wrKey = new ChoWeakReference<TKey>(key);

			lock (_padLock)
			{
				return _cacheKeyValues.ContainsKey(wrKey);
			}
		}

		public bool ContainsValue(TValue value)
		{
			ChoGuard.ArgumentNotNull(value, "Value");
			ValidateInputs(null, value);

			ChoWeakReference<TValue> wrValue = new ChoWeakReference<TValue>(value);

			lock (_padLock)
			{
				if (_cacheValueKeys.ContainsKey(wrValue))
				{
					if (_cacheValueKeys[wrValue].IsAlive)
						return true;
				}
			}

			return false;
		}

        /// <summary>
        /// Gets the key corresponding to the specified value.
        /// </summary>
        /// <param name="value">The value to look up in the dictionary.</param>
        /// <returns>The associated key, or null if no key exists.</returns>
        public TKey GetKey(TValue value)
        {
            ChoGuard.ArgumentNotNull(value, "Value");
            ValidateInputs(null, value);

            ChoWeakReference<TValue> wrValue = new ChoWeakReference<TValue>(value);

            lock (_padLock)
            {
                if (_cacheValueKeys.ContainsKey(wrValue))
                {
                    if (_cacheValueKeys[wrValue].IsAlive)
                        return _cacheValueKeys[wrValue].Target;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the value corresponding to the specified key.
        /// </summary>
        /// <param name="key">The key to look up the value for.</param>
        /// <returns>The associated value, or null if no value exists.</returns>
        public TValue GetValue(TKey key)
        {
            ChoGuard.ArgumentNotNull(key, "Key");
            ValidateInputs(key, null);

            ChoWeakReference<TKey> wrKey = new ChoWeakReference<TKey>(key);

            lock (_padLock)
            {
                if (_cacheKeyValues.ContainsKey(wrKey))
                {
					return _cacheKeyValues[wrKey];
                }
            }

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

            ValidateInputs(key, value);

            ChoWeakReference<TKey> wrKey = new ChoWeakReference<TKey>(key);
            ChoWeakReference<TValue> wrValue = new ChoWeakReference<TValue>(value);

            lock (_padLock)
            {
                if (_cacheKeyValues.ContainsKey(wrKey))
                {
                    if (_cacheValueKeys.ContainsKey(_cacheKeyValues[key]))
                        _cacheValueKeys.Remove(_cacheKeyValues[key]);

                    _cacheKeyValues[wrKey] = wrValue;
                }
                else
                    _cacheKeyValues.Add(wrKey, wrValue);

                if (_cacheValueKeys.ContainsKey(wrValue))
                    _cacheValueKeys[wrValue] = wrKey;
                else
                    _cacheValueKeys.Add(wrValue, wrKey);
            }
        }

        public void RemoveValue(TKey key)
        {
            ChoGuard.ArgumentNotNull(key, "Key");
            
            ValidateInputs(key, null);
            
            ChoWeakReference<TKey> wrKey = new ChoWeakReference<TKey>(key);

            lock (_padLock)
            {
                if (_cacheKeyValues.ContainsKey(wrKey))
                {
                    if (_cacheValueKeys.ContainsKey(_cacheKeyValues[wrKey]))
                        _cacheValueKeys.Remove(_cacheKeyValues[wrKey]);

                    _cacheKeyValues.Remove(wrKey);
                }
            }
        }

        public object SyncRoot
        {
            get { return _padLock; }
        }

        #endregion

        #region Instance Members (Private)

        private static void ValidateInputs(TKey key, TValue value)
        {
            if (key != null && key.GetType().IsValueType)
                throw new ApplicationException("ValueType");

            if ((key != null && key.GetType() == typeof(WeakReference))
                || (value != null && value.GetType() == typeof(WeakReference)))
                throw new ApplicationException("Invalid");
        }

        #endregion Instance Members (Private)

		protected override void Dispose(bool finalize)
		{
			if (_timerService != null)
			{
				_timerService.Dispose();
			}
		}
	}
}
