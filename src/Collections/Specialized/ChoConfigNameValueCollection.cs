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
    public abstract class ChoConfigNameValueCollection : NameValueCollection, IDisposable, IChoConfigSectionable
    {
        #region Constructors

        public ChoConfigNameValueCollection()
        {
        }

        public ChoConfigNameValueCollection(NameValueCollection c)
            : base(c)
        {
        }

        #endregion Constructors

        #region IChoConfigSectionable Members

        public new object this[string key]
        {
            get 
            {
                ChoGuard.ArgumentNotNull(key, "key");
                return base[key];
            }
        }

        public abstract void Persist(ChoConfigSectionObjectMap configSectionObjectMap);

        private string _configSectionName;
        public string ConfigSectionName
        {
            get { return _configSectionName; }
            set { _configSectionName = value; }
        }

        private string _errMsg;
        public string ErrMsg
        {
            get { return _errMsg; }
            set { _errMsg = value; }
        }

        public abstract void StartWatching();
        public abstract void StopWatching();

        public abstract IChoConfigurationChangeWatcher ConfigurationChangeWatcher
        {
            get;
        }

        public virtual void Init(ChoConfigSectionObjectMap configSectionObjectMap)
        {
            if (configSectionObjectMap == null)
                throw new ArgumentNullException("configSectionObjectMap");

            configSectionObjectMap.ConfigSection = this;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region IChoConfigSectionable Members


        #endregion
    }
}
