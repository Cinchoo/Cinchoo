namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Xml;
    using System.Text;
    using System.Collections;
    using System.Configuration;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    using Cinchoo.Core.IO;
    using Cinchoo.Core.Types;
    using Cinchoo.Core.Exceptions;
    using Cinchoo.Core.Attributes;
    using Cinchoo.Core.Xml;
    using Cinchoo.Core.Factory;
    using Cinchoo.Core.Configuration.Sections;
    using Cinchoo.Core.Configuration.Handlers;
	using System.Linq;
	using System.Xml.Linq;
	using Cinchoo.Core.Configuration.Watchers.File;

    #endregion NameSpaces

    [ChoAppDomainEventsRegisterableType]
    public static class ChoConfigurationManager
	{
		#region Constants

		private const string PathToken = "cinchoo:path";

		#endregion Constants

		#region Shared Data Members (Private)

		private static readonly object _syncRoot = new object();
        private static Configuration _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private static string[] _appIncludeConfigFilePaths;
        private static readonly ChoApplicationExeConfigSettings applicationExeConfigSettings;
		private static ChoXmlDocument _appXmlDocument;
		private static ChoAppConfigurationChangeFileWatcher _configurationChangeWatcher;

        #endregion Shared Data Members (Private)

        #region Constructors

        static ChoConfigurationManager()
        {
            OpenExeConfiguration();
            applicationExeConfigSettings = ChoApplicationExeConfigSettings.Me;
        }

        #endregion Constructors

        public static void Initialize()
        {
        }

        #region Shared Properties

		public static ChoAppConfigurationChangeFileWatcher ConfigurationChangeWatcher
		{
			get { return _configurationChangeWatcher; }
		}

        public static object SyncRoot
        {
            get { return _syncRoot; }
        }

        public static Configuration Configuration
        {
            get { return _configuration; }
        }

        public static KeyValueConfigurationCollection AppSettings
        {
            get { return _configuration.AppSettings.Settings; }
        }

		public static ChoXmlDocument AppXmlDocument
		{
			get { return _appXmlDocument; }
		}

        #endregion Shared Properties

        #region Shared Members (Public)

        public static bool IsAppConfigFilePath(string configFilePath)
        {
            return String.Compare(configFilePath, ChoConfigurationManager.AppConfigFilePath, false) == 0;
        }

        public static string GetConfigFile(XmlNode section)
        {
            string configPath = ChoConfigurationManager.AppConfigFilePath;

            if (section.Attributes[PathToken] != null)
                configPath = section.Attributes[PathToken].Value;

            configPath = ChoPath.GetFullPath(configPath);

            return configPath;
        }

        public static string[] AppIncludeConfigFilePaths
        {
            get { return _appIncludeConfigFilePaths; }
        }

        public static string AppConfigFilePath
        {
            get { return _configuration.FilePath; }
        }

        public static string GetConfigSectionName(Type type)
        {
            ChoGuard.ArgumentNotNull(type, "Type");

            string sectionName = type.Name;
            XmlRootAttribute attribute = ChoType.GetAttribute(type, typeof(XmlRootAttribute)) as XmlRootAttribute;
            if (attribute != null && !String.IsNullOrEmpty(attribute.ElementName))
                sectionName = attribute.ElementName;

            ChoConfigurationSectionAttribute configAttribute = ChoType.GetAttribute(type, typeof(ChoConfigurationSectionAttribute)) as ChoConfigurationSectionAttribute;
            if (configAttribute != null && !String.IsNullOrEmpty(configAttribute.ConfigElementName))
                sectionName = configAttribute.ConfigElementName;

            return sectionName;
        }

        public static object GetConfigFromXml(Type type, string xml)
        {
            ChoGuard.ArgumentNotNullOrEmpty(xml, "Xml");

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            return GetConfig(type, document.DocumentElement);
        }

        public static object GetConfig(Type type)
        {
            return GetConfigFromConfigFile(type, ChoConfigurationManager.AppConfigFilePath);
        }

        public static object GetConfigFromConfigFile(Type type, string configFileName, bool initialize)
        {
            ChoGuard.ArgumentNotNullOrEmpty(configFileName, "ConfigFileName");

            XmlDocument document = new XmlDocument();
            document.Load(configFileName);

            if (initialize)
                return GetConfig(type, document.DocumentElement);
            else
                return GetConfigOnly(type, document.DocumentElement);
        }

        public static object GetConfigFromConfigFile(Type type, string configFileName)
        {
            return GetConfigFromConfigFile(type, configFileName, true);
        }

        public static object GetConfig(Type type, XmlNode node)
        {
            XmlSerializer serializer = new XmlSerializer(type);

            XmlRootAttribute xmlRootAttribute = ChoType.GetAttribute<XmlRootAttribute>(type);
            if (xmlRootAttribute == null)
                throw new NullReferenceException(String.Format("Missing XmlRootAttribute in {0} type.", type.FullName));

            if (String.IsNullOrEmpty(xmlRootAttribute.ElementName))
                throw new NullReferenceException(String.Format("Missing XmlRootAttribute(ElementName) in {0} type.", type.FullName));

            node = node.SelectSingleNode(String.Format("//{0}", xmlRootAttribute.ElementName));

            object configObject = null;
            if (node != null)
                configObject = serializer.Deserialize(new XmlNodeReader(node));
            else
                configObject = ChoObjectManagementFactory.CreateInstance(type);

            ChoObjectInitializer.Initialize(configObject);

            return configObject;
        }

        public static object GetConfigOnly(Type type, XmlNode node)
        {
            XmlSerializer serializer = new XmlSerializer(type);

            XmlRootAttribute xmlRootAttribute = ChoType.GetAttribute<XmlRootAttribute>(type);
            if (xmlRootAttribute == null)
                throw new NullReferenceException(String.Format("Missing XmlRootAttribute in {0} type.", type.FullName));

            if (String.IsNullOrEmpty(xmlRootAttribute.ElementName))
                throw new NullReferenceException(String.Format("Missing XmlRootAttribute(ElementName) in {0} type.", type.FullName));

            node = node.SelectSingleNode(String.Format("//{0}", xmlRootAttribute.ElementName));

            object configObject = null;
            if (node != null)
                configObject = serializer.Deserialize(new XmlNodeReader(node));
            else
                configObject = Activator.CreateInstance(type);

            return configObject;
        }

        public static ConfigurationSection GetConfigSection(string sectionName)
        {
            return GetConfigSection(sectionName, null);
        }

        public static bool ContainSection(string sectionName)
        {
            return GetConfigSection(sectionName) != null;
        }

		public static string GetAppSetting(string configEntryFileName, string keyName)
		{
			return GetAppSetting(configEntryFileName, keyName, null);
		}

		public static string GetAppSetting(string configEntryFileName, string keyName, string defaultValue)
		{
			if (configEntryFileName == null 
				|| configEntryFileName == String.Empty
				|| keyName == null
				|| keyName == String.Empty
				)
				return defaultValue;

			if (!File.Exists(configEntryFileName))
				return String.Empty;

			XmlDocument xmlDocument = new XmlDocument();

			xmlDocument.Load(configEntryFileName);

			XmlNodeList appSettingsElements  =  xmlDocument.SelectNodes("//configuration/appSettings/add");

			if (appSettingsElements.Count == 0)
				return defaultValue;

			foreach (XmlNode appSettingsElement in appSettingsElements)
			{
                //Console.WriteLine(appSettingsElement.Attributes["key"].Value);
				if (appSettingsElement.Attributes["key"].Value == keyName)
					return appSettingsElement.Attributes["value"].Value;
			}

			return defaultValue;
		}

        public static IDictionary GetDictionary(XmlNode region)
        {
            return GetDictionary(null, region, "key", "value");
        }

        public static IDictionary GetDictionary(IDictionary prev,
                               XmlNode region,
                               string nameAtt,
                               string valueAtt)
        {
            ChoGuard.ArgumentNotNull(region, "region");
            
            Hashtable hashtable;
            if (prev == null)
                hashtable = new Hashtable();
            else
            {
                Hashtable aux = (Hashtable)prev;
                hashtable = (Hashtable)aux.Clone();
            }

            ChoCollectionWrapper result = new ChoCollectionWrapper(hashtable);
            result = Read(result, region, nameAtt, valueAtt);
            if (result == null)
                return null;

            return result.UnWrap() as IDictionary;
        }

        public static NameValueCollection GetNameValues(XmlNode region)
        {
            return GetNameValues(null, region, "key", "value");
        }

        public static NameValueCollection GetNameValues(NameValueCollection prev,
                                        XmlNode region,
                                        string nameAtt,
                                        string valueAtt)
        {
            ChoGuard.ArgumentNotNull(region, "region");
            NameValueCollection coll =

                    new NameValueCollection();

            if (prev != null)
                coll.Add(prev);

            ChoCollectionWrapper result = new ChoCollectionWrapper(coll);
            result = Read(result, region, nameAtt, valueAtt);
            if (result == null)
                return null;

            return result.UnWrap() as NameValueCollection;
        }

        public static NameValueCollection GetNameValuesFromAttributes(XmlNode region)
        {
            return GetNameValuesFromAttributes(null, region);
        }

        public static NameValueCollection GetNameValuesFromAttributes(NameValueCollection prev,
                                        XmlNode region)
        {
            ChoGuard.ArgumentNotNull(region, "region");
            
            NameValueCollection coll =
                    new NameValueCollection();

            if (prev != null)
                coll.Add(prev);

            ChoCollectionWrapper result = new ChoCollectionWrapper(coll);
            result = ReadAttributes(result, region);
            if (result == null)
                return null;

            return result.UnWrap() as NameValueCollection;
        }

        #endregion Shared Members (Public)

        #region Shared Members (Private)

        [ChoAppDomainUnloadMethod("Restore the Application configuration to original format...")]
        public static void RestoreAppConfig()
        {
			if (File.Exists(AppConfigFilePath))
			{
				if (_appXmlDocument != null && _appXmlDocument.HasIncludeComments)
					ChoXmlDocument.ContractNSave(_appXmlDocument, AppConfigFilePath);
			}
        }

        private static bool ContainSection(string sectionName, ConfigurationSectionGroup sectionGroup)
        {
            if (_configuration == null || !_configuration.HasFile) return true;

            int index = sectionName.IndexOf('/');
            if (index >= 0)
            {
                string sectionGroupName = sectionName.Substring(0, index).Trim();
                if (String.IsNullOrEmpty(sectionGroupName))
                    return false;

                sectionGroup = sectionGroup == null ? _configuration.SectionGroups[sectionGroupName] : sectionGroup.SectionGroups[sectionGroupName];
                if (sectionGroup == null) return false;

                return ContainSection(sectionName.Substring(index + 1), sectionGroup);
            }
            else
            {
                return sectionGroup.Sections[sectionName] != null;
            }
        }

        private static ConfigurationSection GetConfigSection(string sectionName, ConfigurationSectionGroup sectionGroup)
        {
            if (sectionName.IsNullOrEmpty()) return null;

            sectionName = sectionName.Trim();

            int pos = sectionName.IndexOf('/');
            if (pos >= 0)
            {
                string sectionGroupName = sectionName.Substring(0, pos);
                sectionName = sectionName.Substring(pos + 1, sectionName.Length - (pos + 1));

                if (sectionGroupName.IsNullOrEmpty())
                    return GetConfigSection(sectionName, sectionGroup);
                else
                {
                    if (sectionGroup == null)
                        sectionGroup = _configuration.SectionGroups[sectionGroupName];
                    else
                        sectionGroup = sectionGroup.SectionGroups[sectionGroupName];

                    if (sectionGroup == null) return null;

                    return GetConfigSection(sectionName, sectionGroup);
                }
            }
            else
            {
                return sectionGroup == null ? _configuration.Sections[sectionName] : sectionGroup.Sections[sectionName];
            }
        }

		private static readonly object _key = new object();

        internal static void OpenExeConfiguration()
        {
            //Expand Application configuration file
            string appConfigPath = AppConfigFilePath; // AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            if (!File.Exists(appConfigPath))
                return;

			ChoFile.SetReadOnly(appConfigPath, false);

			if (_configurationChangeWatcher != null)
			{
				RestoreAppConfig();
				_configurationChangeWatcher.StopWatching();
				_configurationChangeWatcher = null;
			}

			//backup the current configuration
			string backupConfigFilePath = String.Format("{0}.cho", appConfigPath);
			File.Copy(appConfigPath, backupConfigFilePath, true);

			try
			{
				_appXmlDocument = new ChoXmlDocument(appConfigPath, false, true);
				_appIncludeConfigFilePaths = _appXmlDocument.IncludeFiles;

				if (_appXmlDocument != null && _appIncludeConfigFilePaths != null && _appIncludeConfigFilePaths.Length > 0)
					_appXmlDocument.XmlDocument.Save(appConfigPath);

				_configurationChangeWatcher = new ChoAppConfigurationChangeFileWatcher(ChoConfigurationManager.AppConfigFilePath, "Configurations");
				_configurationChangeWatcher.SetConfigurationChangedEventHandler(_key, new ChoConfigurationChangedEventHandler(_configurationChangeWatcher_ConfigurationChanged));

				//Remove namespaces
				if (_appXmlDocument != null && _appIncludeConfigFilePaths != null && _appIncludeConfigFilePaths.Length > 0)
				{
					XDocument doc = XDocument.Load(appConfigPath, LoadOptions.PreserveWhitespace);
					doc.Descendants().Attributes().Where(a => a.IsNamespaceDeclaration).Remove();
					doc.Save(appConfigPath, SaveOptions.DisableFormatting);
				}
				_configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

				//TODO: use XmlDocument to expand
				if (ChoEnvironmentSettings.HasAppConfigPathSpecified())
				{
					appConfigPath = ChoEnvironmentSettings.GetAppConfigPath();
					if (!String.IsNullOrEmpty(appConfigPath))
						_configuration = ConfigurationManager.OpenExeConfiguration(appConfigPath);
				}
			}
			catch
			{
				//Rollback the configuration file
				File.Copy(backupConfigFilePath, appConfigPath, true);

				throw;
			}
        }

		static void _configurationChangeWatcher_ConfigurationChanged(object sender, ChoConfigurationChangedEventArgs e)
		{
			_appXmlDocument = new ChoXmlDocument(AppConfigFilePath, false, true);
			_appIncludeConfigFilePaths = _appXmlDocument.IncludeFiles;
		}

        private static ChoCollectionWrapper ReadAttributes(ChoCollectionWrapper result,
                                XmlNode region)
        {
            if (region.Attributes != null && region.Attributes.Count != 0)
            {
                foreach (XmlAttribute attribute in region.Attributes)
                {
                    if (attribute == null) continue;
                    result[attribute.Name] = attribute.Value;
                }
            }

            return result;
        }

        private static ChoCollectionWrapper Read(ChoCollectionWrapper result,
                                XmlNode region,
                                string nameAtt,
                                string valueAtt)
        {
            //if (region.Attributes != null && region.Attributes.Count != 0)
            //    throw new ChoConfigurationException("Unknown attribute", region);

            XmlNode keyNode;
            XmlNode valueNode;
            XmlNodeList childs = region.ChildNodes;
            foreach (XmlNode node in childs)
            {
                XmlNodeType ntype = node.NodeType;
                if (ntype == XmlNodeType.Whitespace || ntype == XmlNodeType.Comment)
                    continue;

                if (ntype != XmlNodeType.Element)
                    throw new ChoConfigurationException("Only XmlElement allowed", node);

                string nodeName = node.Name;
                if (nodeName == "clear")
                {
                    if (node.Attributes != null && node.Attributes.Count != 0)
                        throw new ChoConfigurationException("Unknown attribute", node);

                    result.Clear();
                }
                else if (nodeName == "remove")
                {
                    keyNode = null;
                    if (node.Attributes != null)
                        keyNode = node.Attributes.RemoveNamedItem(nameAtt);

                    if (keyNode == null)
                        throw new ChoConfigurationException("Required attribute not found",
                                          node);
                    if (keyNode.Value == String.Empty)
                        throw new ChoConfigurationException("Required attribute is empty",
                                          node);

                    if (node.Attributes.Count != 0)
                        throw new ChoConfigurationException("Unknown attribute", node);

                    result.Remove(keyNode.Value);
                }
                else if (nodeName == "add")
                {
                    keyNode = null;
                    if (node.Attributes != null)
                        keyNode = node.Attributes.RemoveNamedItem(nameAtt);

                    if (keyNode == null)
                        throw new ChoConfigurationException("Required attribute not found",
                                          node);
                    if (keyNode.Value == String.Empty)
                        throw new ChoConfigurationException("Required attribute is empty",
                                          node);

                    valueNode = node.Attributes.RemoveNamedItem(valueAtt);
                    if (valueNode == null)
                        throw new ChoConfigurationException("Required attribute not found",
                                          node);

                    if (node.Attributes.Count != 0)
                        throw new ChoConfigurationException("Unknown attribute", node);

                    result[keyNode.Value] = valueNode.Value;
                }
                else
                {
                    //throw new ChoConfigurationException("Unknown element", node);
                }
            }

            return result;
        }

        #endregion

        #region CollectionWrapper Class

        private class ChoCollectionWrapper
        {
            IDictionary _dictionary;
            NameValueCollection _collection;
            bool _isDictionary;

            public ChoCollectionWrapper(IDictionary dictionary)
            {
                this._dictionary = dictionary;
                _isDictionary = true;
            }

            public ChoCollectionWrapper(NameValueCollection collection)
            {
                this._collection = collection;
                _isDictionary = false;
            }

            public void Remove(string s)
            {
                if (_isDictionary)
                    _dictionary.Remove(s);
                else
                    _collection.Remove(s);
            }

            public void Clear()
            {
                if (_isDictionary)
                    _dictionary.Clear();
                else
                    _collection.Clear();
            }

            public string this[string key]
            {
                set
                {
                    if (_isDictionary)
                        _dictionary[key] = value;
                    else
                        _collection[key] = value;
                }
            }

            public object UnWrap()
            {
                if (_isDictionary)
                    return _dictionary;
                else
                    return _collection;
            }
        }

        #endregion
    }
}
