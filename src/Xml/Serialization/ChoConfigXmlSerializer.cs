namespace eSquare.Core.Xml.Serialization
{
    #region NameSpaces

    using System;
    using System.Xml;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using eSquare.Core.Configuration;
    using eSquare.Core.Configuration.Storage;
    using eSquare.Core.IO;

    #endregion NameSpaces

    [Serializable]
    public abstract class ChoConfigXmlSerializer : IDisposable, IChoConfigSectionable
    {
        #region Constructors

        public ChoConfigXmlSerializer(XmlNode node)
        {
        }

        #endregion Constructors

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region IChoConfigSectionable Members

        public object this[string key]
        {
            get { throw new NotSupportedException(); }
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
    }
}
