namespace eSquare.Core.Collections.Specialized
{
    #region NameSpaces

    using System;
    using System.Xml;
    using System.Collections;
    using System.Configuration;
    using System.Collections.Specialized;

    using eSquare.Core.Configuration;
    using eSquare.Core.Configuration.Storage;

    #endregion

    [Serializable]
    public abstract class ChoWatchableNameValueCollection : ChoNameValueCollection, IChoConfigSectionable
    {
        public ChoWatchableNameValueCollection()
        {
        }

        public ChoWatchableNameValueCollection(NameValueCollection c)
            : base(c)
        {
        }

        #region IChoConfigSection Members

        public Hashtable ToHashtable()
        {
            Hashtable hashTable = new Hashtable();
            foreach (string key in Keys)
                hashTable.Add(key, this[key]);

            return hashTable;
        }

        public abstract void Persist(ChoConfigSectionObjectMap configSectionObjectMap);

        #endregion

        #region IChoConfigSection Members

        private string _errMsg;
        public string ErrMsg
        {
            get { return _errMsg; }
            set { _errMsg = value; }
        }

        #endregion

        #region IChoConfigSection Members

        public abstract void StartWatching();
        public abstract void StopWatching();

        #endregion

        #region IChoConfigSectionable Members

        public abstract IChoConfigurationChangeWatcher ConfigurationChangeWatcher
        {
            get;
        }

        #endregion
    }
}
