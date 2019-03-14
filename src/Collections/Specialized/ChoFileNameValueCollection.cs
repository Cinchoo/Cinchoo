namespace eSquare.Core.Collections.Specialized
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Xml;
    using System.Text;
    using System.Collections;
    using System.Configuration;
    using System.Collections.Specialized;

    using eSquare.Core.Xml;
    using eSquare.Core.Configuration;
    using eSquare.Core.Configuration.Storage;

    #endregion

    [Serializable]
    public sealed class ChoFileNameValueCollection : ChoConfigNameValueCollection
    {
        #region Instance Data Members

        private IChoConfigurationChangeWatcher _configurationChangeWatcher;

        #endregion

        #region Constructors

        public ChoFileNameValueCollection()
        {
        }

        public ChoFileNameValueCollection(XmlNode node)
        {
            ConfigSectionName = node.Name;
            _configPath = ChoConfigurationManager.GetConfigFile(node);

            if (_configPath == null || !File.Exists(_configPath))
                ErrMsg = String.Format("{0} not exists.", _configPath);
        }

        public ChoFileNameValueCollection(NameValueCollection c)
            : base(c)
        {
        }

        #endregion Constructors

        #region Instance Properties

        private string _configPath;
        public string ConfigPath
        {
            get { return _configPath; }
            //set  {  _configPath = value; }
        }

        #endregion 

        #region IChoConfigSection Members

        public override void Persist(ChoConfigSectionObjectMap configSectionObjectMap)
        {
            StringBuilder config = new StringBuilder();
            Hashtable keyValues = configSectionObjectMap.ToHashtable();
            foreach (string key in keyValues.Keys)
            {
                if (keyValues[key] != null)
                    config.AppendFormat("<add key=\"{0}\" value=\"{1}\" />{2}", key, keyValues[key].ToString(), Environment.NewLine);
            }

            File.WriteAllText(_configPath, ChoXmlDocument.SetInnerXml(_configPath, ConfigSectionName, config.ToString()));
        }

        public override void StartWatching()
        {
            _configurationChangeWatcher.StartWatching();
        }

        public override void StopWatching()
        {
            _configurationChangeWatcher.StopWatching();
        }

        public override IChoConfigurationChangeWatcher ConfigurationChangeWatcher
        {
            get 
            {
                lock (this)
                {
                    if (_configurationChangeWatcher == null)
                        _configurationChangeWatcher = new ChoConfigurationChangeFileWatcher(ConfigPath, ConfigSectionName);

                    return _configurationChangeWatcher;
                }
            }
        }

        //public override void Init(ChoConfigSectionObjectMap configSectionObjectMap)
        //{
        //    base.Init(configSectionObjectMap);
        //    configSectionObjectMap.ConfigFilePath = ConfigPath;
        //}

        #endregion
    }
}
