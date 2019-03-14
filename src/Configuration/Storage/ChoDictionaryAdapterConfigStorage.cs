namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System.Collections.Generic;
    using System.Xml;
    using Cinchoo.Core.Services;
    using System.Xml.XPath;
    using Cinchoo.Core.Xml;
    using System;
    using System.Collections;

    #endregion NameSpaces

    public class ChoDictionaryAdapterConfigStorage : ChoConfigStorage, IChoDictionaryConfigStorage
    {
        protected ChoDictionaryAdapterSectionInfo SectionInfo;

        public override object Load(ChoBaseConfigurationElement configElement, XmlNode node)
        {
            base.Load(configElement, node);

            SectionInfo = configElement.StateInfo["DICT_ADAPTER"] as ChoDictionaryAdapterSectionInfo;
            ChoDictionaryAdapterSectionInfo sectionInfo = new ChoDictionaryAdapterSectionInfo(ConfigNode, configElement);
            if (SectionInfo != sectionInfo)
            {
                configElement.StateInfo["DICT_ADAPTER"] = SectionInfo = sectionInfo;
                SectionInfo.Initialize(configElement);
            }
            return SectionInfo.GetData();
        }

        protected override void PersistConfigData(object data, ChoDictionaryService<string, object> stateInfo)
        {
            ChoGuard.ArgumentNotNull(data, "Config Data Object");

            //Write section info
            PersistAsNameSpaceAwareXml(data, stateInfo);

            //Write actual section data
            if (SectionInfo != null)
                SectionInfo.SaveData(data);
        }

        protected override string ToXml(object data)
        {
            return SectionInfo != null ? SectionInfo.AsConfigSectionXml(ConfigSectionName) : null;
        }

        public override IChoConfigurationChangeWatcher ConfigurationChangeWatcher
        {
            get
            {
                //Main app configuration
                List<IChoConfigurationChangeWatcher> _watchers = new List<IChoConfigurationChangeWatcher>();
                _watchers.Add(base.ConfigurationChangeWatcher);

                if (SectionInfo != null)
                {
                    IChoConfigurationChangeWatcher configurationChangeWatcher = new ChoConfigurationAdapterChangeWatcher(ConfigSectionName, SectionInfo.ConfigObjectAdapter);
                    _watchers.Add(configurationChangeWatcher);
                }

                return new ChoConfigurationChangeCompositeWatcher(ConfigSectionName, _watchers.ToArray());
            }
        }

        public override object PersistableState
        {
            get { return ChoObject.ToPersistableDictionaryCollection(ConfigElement); }
        }
    }

    public class ChoDictionaryAdapterSectionInfo : ChoEquatableObject<ChoDictionaryAdapterSectionInfo>
    {
        #region Constants

        private const string ConfigObjectAdapterTypeToken = "cinchoo:configObjectAdapterType";
        private const string ConfigObjectAdapterParamsToken = "cinchoo:configObjectAdapterParams";

        #endregion Constants

        #region Instance Data Members (Private)

        public readonly string ConfigObjectAdapterTypeString = null;
        public readonly string ConfigObjectAdapterParamsString = null;

        [ChoIgnoreEqual]
        public IChoDictionaryConfigObjectAdapter ConfigObjectAdapter = null;

        #endregion Instance Data Members (Private)

        #region Instance Data Members (Public)

        public ChoDictionaryAdapterSectionInfo(XmlNode node, ChoBaseConfigurationElement configElement)
        {
            if (node != null)
            {
                XPathNavigator navigator = node.CreateNavigator();

                ConfigObjectAdapterTypeString = (string)navigator.Evaluate(String.Format("string(@{0})", ConfigObjectAdapterTypeToken), ChoXmlDocument.NameSpaceManager);
                XPathNavigator paramsNode = navigator.SelectSingleNode(String.Format("//{0}", ConfigObjectAdapterParamsToken), ChoXmlDocument.NameSpaceManager);
                if (paramsNode != null)
                    ConfigObjectAdapterParamsString = paramsNode.InnerXml.Trim();
            }

            if (ConfigObjectAdapterTypeString.IsNullOrWhiteSpace())
                ConfigObjectAdapterTypeString = configElement[ChoDictionaryAdapterConfigurationSectionAttribute.CONFIG_OBJECT_ADAPTER_TYPE] as string;
            else
                configElement[ChoDictionaryAdapterConfigurationSectionAttribute.CONFIG_OBJECT_ADAPTER_TYPE] = ConfigObjectAdapterTypeString;

            if (ConfigObjectAdapterParamsString.IsNullOrWhiteSpace())
                ConfigObjectAdapterParamsString = configElement[ChoDictionaryAdapterConfigurationSectionAttribute.CONFIG_OBJECT_ADAPTER_PARAMS] as string;
            else
                configElement[ChoDictionaryAdapterConfigurationSectionAttribute.CONFIG_OBJECT_ADAPTER_PARAMS] = ConfigObjectAdapterParamsString;
        }

        public string AsConfigSectionXml(string configSectionName)
        {
            if (!ConfigObjectAdapterParamsString.IsNullOrWhiteSpace())
                return @"<{0} {1}=""{2}"" {3}=""{4}""><{5}><![CDATA[{6}]]></{5}></{0}>".FormatString(configSectionName, ConfigObjectAdapterTypeToken, ConfigObjectAdapterTypeString,
                    ChoXmlDocument.CinchoNSToken, ChoXmlDocument.CinchooNSURI, ConfigObjectAdapterParamsToken, ConfigObjectAdapterParamsString);
            else
                return @"<{0} {1}=""{2}"" {3}=""{4}"" />".FormatString(configSectionName, ConfigObjectAdapterTypeToken, ConfigObjectAdapterTypeString,
                    ChoXmlDocument.CinchoNSToken, ChoXmlDocument.CinchooNSURI);
        }

        public void Initialize(ChoBaseConfigurationElement configElement)
        {
            if (ConfigObjectAdapterTypeString.IsNullOrWhiteSpace())
                throw new ChoConfigurationException("Missing ConfigObjectAdapterType.");

            Type ConfigObjectAdapterType = ChoType.GetType(ConfigObjectAdapterTypeString);
            if (ConfigObjectAdapterType == null)
                throw new ChoConfigurationException(String.Format("Failed to discover ConfigObjectAdapterType ({0}).", ConfigObjectAdapterTypeString));

            if (!typeof(IChoDictionaryConfigObjectAdapter).IsAssignableFrom(ConfigObjectAdapterType))
                throw new ChoConfigurationException(String.Format("ConfigObjectAdapter ({0}) is not of IChoDictionaryConfigObjectAdapter type.", ConfigObjectAdapterTypeString));

            ConfigObjectAdapter = ChoActivator.CreateInstance(ConfigObjectAdapterType) as IChoDictionaryConfigObjectAdapter;
            ConfigObjectAdapter.Init(configElement, ConfigObjectAdapterParamsString.ToKeyValuePairs());
        }

        #endregion

        internal object GetData()
        {
            if (ConfigObjectAdapter != null)
            {
                return ConfigObjectAdapter.GetData();
            }

            return null;
        }

        internal void SaveData(object data)
        {
            if (!(data is IDictionary))
                throw new ChoConfigurationException("Data object is not IDictionary object.");

            IDictionary dict = data as IDictionary;

            if (dict != null)
            {
                ConfigObjectAdapter.Save(dict);
            }
        }
    }
}
