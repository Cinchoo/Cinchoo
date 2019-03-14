namespace eSquare.Core.Xml.Serialization
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
    public sealed class ChoFileXmlSerializer : ChoConfigXmlSerializer, IChoFileConfigSectionable
    {
        #region Instance Data Members

        private IChoConfigurationChangeWatcher _configurationChangeWatcher;

        #endregion

        #region Constructors

        public ChoFileXmlSerializer(XmlNode node) : base(node)
        {
            ConfigSectionName = node.Name;
            _configPath = ChoConfigurationManager.GetConfigFile(node);

            if (_configPath == null || !File.Exists(_configPath))
                ErrMsg = String.Format("{0} not exists.", _configPath);
        }

        #endregion Constructors

        #region Instance Properties

        private object _configObject;
        public object ConfigObject
        {
            get { return _configObject; }
            set { _configObject = ChoInterceptingProxy.Instance(value); }
        }

        private string _configPath;
        public string ConfigPath
        {
            get { return _configPath; }
            //set { _configPath = value; }
        }

        #endregion

        #region IChoConfigSection Members

        public override void Persist(ChoConfigSectionObjectMap configSectionObjectMap)
        {
            File.WriteAllText(_configPath, ChoXmlDocument.SetOuterXml(_configPath, ConfigSectionName,
                ChoObject.ToXmlString(configSectionObjectMap.ConfigObject)));
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

        public override void Init(ChoConfigSectionObjectMap configSectionObjectMap)
        {
            base.Init(configSectionObjectMap);
            //configSectionObjectMap.ConfigFilePath = ConfigPath;
            configSectionObjectMap.ConfigObject = ConfigObject;
        }

        #endregion
    }
}
