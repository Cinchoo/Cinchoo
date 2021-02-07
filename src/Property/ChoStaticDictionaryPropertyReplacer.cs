namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    [Serializable]
    public class ChoStaticDictionaryPropertyReplacer : IChoKeyValuePropertyReplacer
    {
        #region Shared Data Members (Private)

        private readonly static ChoStaticDictionaryPropertyReplacer _instance;

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Private)

        private readonly object _padLock = new object();
        private readonly Dictionary<string, object> _dict = new Dictionary<string, object>();
        private readonly Dictionary<string, string> _dictDescription = new Dictionary<string, string>();

        #endregion Instance Data Members (Private)

        #region Constructors

        static ChoStaticDictionaryPropertyReplacer()
        {
            _instance = new ChoStaticDictionaryPropertyReplacer();
        }

        private ChoStaticDictionaryPropertyReplacer()
        {
        }

        #endregion Constructors

        #region IChoKeyValuePropertyReplacer Members

        public bool ContainsProperty(string propertyName, object context)
        {
            lock (_padLock)
            {
                return _dict.ContainsKey(propertyName);
            }
        }

        public string ReplaceProperty(string propertyName, string format, object context)
        {
            lock (_padLock)
            {
                return ChoObject.Format(_dict[propertyName], format);
            }
        }

        #endregion

        #region Instance Members (Public)

        public bool ContainsKey(string key)
        {
            lock (_padLock)
            {
                return _dict.ContainsKey(key);
            }
        }

        public void Add(string key, object value)
        {
            lock (_padLock)
            {
                _dict.Add(key, value);
            }
        }

        public void Add(string key, object value, string description)
        {
            lock (_padLock)
            {
                _dict.Add(key, value);
                _dictDescription.Add(key, description);
            }
        }

        public void AddOrReplace(string key, object value)
        {
            lock (_padLock)
            {
                if (_dict.ContainsKey(key))
                    _dict[key] = value;
                else
                    _dict.Add(key, value);
            }
        }

        public void AddOrReplace(string key, object value, string description)
        {
            lock (_padLock)
            {
                if (_dict.ContainsKey(key))
                    _dict[key] = value;
                else
                    _dict.Add(key, value);

                if (_dictDescription.ContainsKey(key))
                    _dictDescription[key] = description;
                else
                    _dictDescription.Add(key, description);
            }
        }

        public void Remove(string key)
        {
            lock (_padLock)
            {
                _dict.Remove(key);
                if (_dictDescription.ContainsKey(key))
                    _dictDescription.Remove(key);
            }
        }

        #endregion Instance Members (Public)

        #region Shared Members (Public)

        public static ChoStaticDictionaryPropertyReplacer Me
        {
            get { return _instance; }
        }

        #endregion Shared Members (Public)

        #region IChoPropertyReplacer Members

        public IEnumerable<KeyValuePair<string, string>> AvailablePropeties
        {
            get
            {
                foreach (string propertyName in _dict.Keys)
                    yield return new KeyValuePair<string, string>(propertyName, null);
            }
        }

        public string Name
        {
            get { return this.GetType().FullName; }
        }

        public string GetPropertyDescription(string propertyName)
        {
            if (_dictDescription.ContainsKey(propertyName))
                return _dictDescription[propertyName];
            else
                return null;
        }
        #endregion
    }
}
