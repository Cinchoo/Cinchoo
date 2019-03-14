namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Cinchoo.Core;
    using Cinchoo.Core.Factory;
    using Cinchoo.Core.IO;
    using Cinchoo.Core.Services;
    using Cinchoo.Core.Xml;
    using System.Configuration;

    #endregion NameSpaces

    public static class ChoConfigurationConstants
	{
		#region Constants

		public const string TAG = "#TAG#";
		public const string CMD_INSTRUCTION = "#CMD_INSTRUCTION#";
		public const string FORCE_PERSIST = "#FORCE_PERSIST#";
        public const string PERSIST_TIME_STAMP = "#PERSIST_TIME_STAMP#";

		#endregion
	}

	[ChoAppDomainEventsRegisterableType]
	public static class ChoConfigurationManager
	{
		#region Constants

		internal const string PathToken = "cinchoo:path";

		#endregion Constants

		#region Shared Data Members (Private)

		private static readonly object _syncRoot = new object();
		private static string[] _appIncludeConfigFilePaths; 
		private static ChoXmlDocument _appXmlDocument;
        private static ChoAppConfigurationChangeFileWatcher _configurationChangeWatcher;
		private static ChoConfigurationChangeFileWatcher _systemConfigurationChangeWatcher;
		private static ChoConfiguration _configuration;
		private static string _appConfigPath;
        private static readonly object _key = new object();

		#endregion Shared Data Members (Private)

		#region Constructors

		static ChoConfigurationManager()
		{
		}

		#endregion Constructors

        #region Shared Members (Public)

        internal static void Initialize()
		{
            OpenExeConfiguration(true);
        }

        #endregion Shared Members (Public)

        #region Shared Properties

        public static ChoAppConfigurationChangeFileWatcher ConfigurationChangeWatcher
		{
			get { return _configurationChangeWatcher; }
		}

        internal static ChoConfigurationChangeFileWatcher SystemConfigurationChangeWatcher
		{
			get { return _systemConfigurationChangeWatcher; }
		}

		public static object SyncRoot
		{
			get { return _syncRoot; }
		}

		public static ChoXmlDocument AppXmlDocument
		{
			get { return _appXmlDocument; }
		}

		#endregion Shared Properties

		#region Shared Members (Public)

		internal static bool IsAppConfigFilePath(string configFilePath)
		{
            return String.Compare(configFilePath, ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath, false) == 0;
		}

		public static string GetConfigFile(XmlNode section)
		{
            string configPath = ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath;

			if (section != null)
			{
				if (section.Attributes[PathToken] != null)
					configPath = section.Attributes[PathToken].Value;

                //configPath = ChoPath.GetFullPath(configPath);
			}

			return configPath;
		}

		public static string[] AppIncludeConfigFilePaths
		{
			get { return _appIncludeConfigFilePaths; }
		}

		public static string GetConfigSectionName(Type type)
		{
			ChoGuard.ArgumentNotNull(type, "Type");

			string sectionName = type.Name;
			XmlRootAttribute attribute = ChoType.GetAttribute(type, typeof(XmlRootAttribute)) as XmlRootAttribute;
			if (attribute != null && !String.IsNullOrEmpty(attribute.ElementName))
				sectionName = attribute.ElementName;

			ChoConfigurationSectionAttribute configAttribute = ChoType.GetAttribute(type, typeof(ChoConfigurationSectionAttribute)) as ChoConfigurationSectionAttribute;
			if (configAttribute != null && !String.IsNullOrEmpty(configAttribute.ConfigElementPath))
				sectionName = configAttribute.ConfigElementPath;

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
            return GetConfigFromConfigFile(type, ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath);
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

		public static ChoConfigurationSection GetConfigSection(string sectionName)
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

        public static void PersistConfig(object configObject)
        {
            ChoGuard.ArgumentNotNull(configObject, "Config Object.");

            ChoConfigurationSectionAttribute configurationElementAttribute = ChoType.GetAttribute(configObject.GetType(), typeof(ChoConfigurationSectionAttribute)) 
                as ChoConfigurationSectionAttribute;

            if (configurationElementAttribute == null)
                return;

            configurationElementAttribute.GetMe(configObject.GetType()).Persist(true, null);
        }

        public static void RefreshConfig(object configObject)
        {
            ChoGuard.ArgumentNotNull(configObject, "Config Object.");

            ChoConfigurationSectionAttribute configurationElementAttribute = ChoType.GetAttribute(configObject.GetType(), typeof(ChoConfigurationSectionAttribute))
                as ChoConfigurationSectionAttribute;

            if (configurationElementAttribute == null)
                return;

            configurationElementAttribute.GetMe(configObject.GetType()).Refresh(true);
        }

		#endregion Shared Members (Public)

		#region Shared Members (Private)

		[ChoAppDomainUnloadMethod("Restore the Application configuration to original format...")]
		internal static void RestoreAppConfig()
		{
            if (File.Exists(ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath))
			{
				if (_appXmlDocument != null && _appXmlDocument.HasIncludeComments)
                    ChoXmlDocument.ContractNSave(_appXmlDocument, ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath);
			}
		}

		private static bool ContainSection(string sectionName, ChoConfigurationSectionGroup sectionGroup)
		{
			if (_configuration == null) return true;

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

		private static ChoConfigurationSection GetConfigSection(string sectionName, ChoConfigurationSectionGroup sectionGroup)
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
					{
						if (_configuration.SectionGroups.ContainsKey(sectionGroupName))
							sectionGroup = _configuration.SectionGroups[sectionGroupName];
					}
					else
					{
						if (sectionGroup.SectionGroups.ContainsKey(sectionGroupName))
							sectionGroup = sectionGroup.SectionGroups[sectionGroupName];
					}

					if (sectionGroup == null) return null;

					return GetConfigSection(sectionName, sectionGroup);
				}
			}
			else
			{
				if (sectionGroup == null)
				{
					if (_configuration.Sections.ContainsKey(sectionName))
						return _configuration.Sections[sectionName];
					else
						return null;
				}
				else
				{
					if (sectionGroup.Sections.ContainsKey(sectionName))
						return sectionGroup.Sections[sectionName];
					else
						return null;
				}

				//return sectionGroup == null ? _configuration.Sections[sectionName] : sectionGroup.Sections[sectionName];
			}
		}

        public static bool IsConfigXmlModified(string configSectionPath, XmlNode oldNode)
        {
            XmlNode node = ChoXmlDocument.GetXmlNode(configSectionPath, _configuration.RestOfXmlDocumentElements);
            
            if (object.ReferenceEquals(node, oldNode))
                return false;
            if (object.ReferenceEquals(oldNode, null))
                return true;
            if (object.ReferenceEquals(node, null))
                return true;
            return node.OuterXml != oldNode.OuterXml;
        }

		internal static object GetConfig(Type configObjectType, string configSectionPath, Type defaultConfigSectionHandlerType, 
            ChoBaseConfigurationElement configElement)
		{
			if (_configuration == null)
				return null;

			ChoConfigurationSection configurationSection = ChoConfigurationManager.GetConfigSection(configSectionPath);
			XmlNode[] restOfXmlElements = null;

            Type configSectionHandlerType = defaultConfigSectionHandlerType;
			if (configurationSection != null)
			{
				restOfXmlElements = configurationSection.RestOfXmlElements;
                if (!configurationSection.Type.IsNullOrWhiteSpace())
                {
                    configSectionHandlerType = ChoType.GetType(configurationSection.Type);
                }
			}

            configElement.ConfigSectionHandlerType = configSectionHandlerType;
            if (!typeof(IChoConfigurationSectionHandler).IsAssignableFrom(configSectionHandlerType))
                throw new ChoConfigurationException("Configuration section handler type should be of IChoConfigurationSectionHandler type.");

			if (typeof(IChoNullConfigurationSectionableHandler).IsAssignableFrom(configSectionHandlerType))
			{
				return ChoType.CreateInstance<IChoConfigurationSectionHandler>(configSectionHandlerType).Create(configObjectType, null, restOfXmlElements, configElement);
			}
			else
			{
                XmlNode node = ChoXmlDocument.GetXmlNode(configSectionPath, _configuration.RestOfXmlDocumentElements);
				//if (node != null)
				return ChoType.CreateInstance<IChoConfigurationSectionHandler>(configSectionHandlerType).Create(configObjectType, node, restOfXmlElements, configElement);
			}
			//return new ChoDefaultApplicationConfigSection(configObjectType);
		}
		
		public static void OpenConfiguration(string appConfigPath)
		{
			OpenConfiguration(ChoPath.GetFullPath(appConfigPath), true);
		}

		internal static void OpenExeConfiguration(bool doBackup)
		{
			string prevAppConfigPath = _appConfigPath;

            _appConfigPath = ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath;
            _appConfigPath = ChoPath.GetFullPath(_appConfigPath);

            if (_appConfigPath != prevAppConfigPath)
                Trace.TraceInformation("Using AppConfigPath: {0}".FormatString(_appConfigPath));

            if (!File.Exists(_appConfigPath))
			{
				if (!prevAppConfigPath.IsNullOrWhiteSpace() && File.Exists(prevAppConfigPath))
					File.Copy(prevAppConfigPath, _appConfigPath, true);
//                else
//                {
////					if (ChoEnvironmentSettings.CreateConfigFileIfNotExists)
////					{
//                        Directory.CreateDirectory(Path.GetDirectoryName(_appConfigPath));
//                        using (File.CreateText(_appConfigPath))
//                        { }
////					}
//                }
				OpenConfiguration(_appConfigPath, false);
			}
			else
				OpenConfiguration(_appConfigPath, doBackup);
		}

		private static void OpenConfiguration(string appConfigPath, bool doBackup)
		{
            if (_appConfigPath != appConfigPath)
                Trace.TraceInformation("Using AppConfigPath: {0}".FormatString(appConfigPath));

			_appConfigPath = appConfigPath;

			if (_appConfigPath.IsNullOrWhiteSpace())
			{
				Trace.TraceError("Empty AppConfigPath passed.");
				return;
			}

			ChoFile.SetReadOnly(_appConfigPath, false);

			if (_configurationChangeWatcher != null)
			{
				RestoreAppConfig();
				//_configurationChangeWatcher.StopWatching();
			}
			//if (_systemConfigurationChangeWatcher != null)
			//{
			//    _systemConfigurationChangeWatcher.StopWatching();
			//    _systemConfigurationChangeWatcher = null;
			//}

            ChoXmlDocument.CreateXmlFileIfEmpty(_appConfigPath);

			//backup the current configuration
            string backupConfigFilePath = String.Format("{0}.{1}", _appConfigPath, ChoReservedFileExt.Cho);

            try
            {
                LoadConfigurationFile();

                if (doBackup)
                {
                    if (File.Exists(backupConfigFilePath))
                        File.SetAttributes(backupConfigFilePath, FileAttributes.Archive);
                    File.Copy(_appConfigPath, backupConfigFilePath, true);
                    if (File.Exists(backupConfigFilePath))
                        File.SetAttributes(backupConfigFilePath, FileAttributes.Hidden);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                if (_appXmlDocument != null)
                {
                    _appXmlDocument.Dispose();
                    _appXmlDocument = null;
                }

                try
                {
                    //Rollback the configuration file
                    if (doBackup)
                        File.Copy(backupConfigFilePath, _appConfigPath, true);

                    LoadConfigurationFile();
                }
                catch
                {
                }
            }
			finally
			{
				if (_configurationChangeWatcher != null)
					_configurationChangeWatcher.StartWatching();
				if (_systemConfigurationChangeWatcher != null)
					_systemConfigurationChangeWatcher.StartWatching();
			}
		}

        private static void LoadConfigurationFile()
        {
            if (_appXmlDocument != null)
            {
                _appXmlDocument.Dispose();
                _appXmlDocument = null;
            }

            _appXmlDocument = new ChoXmlDocument(_appConfigPath);
            _appIncludeConfigFilePaths = _appXmlDocument.IncludeFiles;

            if (_appXmlDocument != null && _appIncludeConfigFilePaths != null && _appIncludeConfigFilePaths.Length > 0)
                _appXmlDocument.XmlDocument.Save(_appConfigPath);

            if (_configurationChangeWatcher == null)
            {
                _configurationChangeWatcher = new ChoAppConfigurationChangeFileWatcher("configurations", _appConfigPath, _appIncludeConfigFilePaths);
                _configurationChangeWatcher.SetConfigurationChangedEventHandler(_key, new ChoConfigurationChangedEventHandler(_configurationChangeWatcher_ConfigurationChanged));
                ChoEnvironmentSettings.SetEnvironmentChangedEventHandlerNoCall("configurations", (sender, e) => 
                    {
                        ChoAppConfigurationChangeFileWatcher configurationChangeWatcher = _configurationChangeWatcher;
                        _configurationChangeWatcher = null;
                        _configurationChangeWatcher_ConfigurationChanged(null, null);
                        configurationChangeWatcher.OnConfigurationChanged();
                        configurationChangeWatcher.Dispose();
                        configurationChangeWatcher = null;
                    });
            }
            else
                _configurationChangeWatcher.Reset(_appConfigPath, _appIncludeConfigFilePaths);

            if (_systemConfigurationChangeWatcher == null)
            {
                try
                {
                    _systemConfigurationChangeWatcher = new ChoConfigurationChangeFileWatcher("systemConfigurations", AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                    _systemConfigurationChangeWatcher.SetConfigurationChangedEventHandler(_key, new ChoConfigurationChangedEventHandler(_systemConfigurationChangeWatcher_ConfigurationChanged));
                }
                catch (Exception ex)
                {
                    ChoApplication.Trace(ex.ToString());
                }
            }

            //Remove namespaces
            if (_appXmlDocument != null && _appIncludeConfigFilePaths != null && _appIncludeConfigFilePaths.Length > 0)
            {
                XDocument doc = XDocument.Load(_appConfigPath, LoadOptions.PreserveWhitespace);
                doc.Descendants().Attributes().Where(a => a.IsNamespaceDeclaration).Remove();
                doc.Save(_appConfigPath, SaveOptions.DisableFormatting);
            }
            _configuration = _appXmlDocument.XmlDocument.DocumentElement.ToObject<ChoConfiguration>();
            _configuration.Initialize();
        }


		private static void _configurationChangeWatcher_ConfigurationChanged(object sender, ChoConfigurationChangedEventArgs e)
		{
			//OpenExeConfiguration(false);
			ChoQueuedExecutionService.Global.Enqueue(() => OpenExeConfiguration(false));
		}

		private static void _systemConfigurationChangeWatcher_ConfigurationChanged(object sender, ChoConfigurationChangedEventArgs e)
		{
			ChoQueuedExecutionService.Global.Enqueue(() =>
				{
					ChoAppDomain.Refresh();
					OpenExeConfiguration(false);
				}
				);
		}

        [ChoAppDomainUnloadMethod("Shutting down Configuration Manager...")]
		private static void StopGlobalQueuedExecutionService()
		{
            if (_configurationChangeWatcher != null)
                _configurationChangeWatcher.Dispose();
            _configurationChangeWatcher = null;

            if (_systemConfigurationChangeWatcher != null)
                _systemConfigurationChangeWatcher.Dispose();
            _systemConfigurationChangeWatcher = null;

			if (_appXmlDocument != null)
			{
				_appXmlDocument.Dispose();
				_appXmlDocument = null;
			}
		}

        #endregion
    }

    //public class ChoCoreFrxSettings
    //{
    //    #region Constants

    //    private const string CORE_FRX_NODE_NAME = "cinchooCoreFrx";

    //    #endregion Constants

    //    #region Shared Data Members (Private)

    //    private static readonly ChoCoreFrxSettings _instance = new ChoCoreFrxSettings();
    //    private static readonly XmlNode _coreFrxNode;
    //    private static readonly XPathNavigator _navigator;

    //    #endregion Shared Data Members (Private)

    //    #region Constructors

    //    static ChoCoreFrxSettings()
    //    {
    //        if (ChoConfigurationManager.AppXmlDocument != null)
    //        {
    //            XmlNode rootNode = ChoConfigurationManager.AppXmlDocument.XmlDocument;
    //            if (rootNode != null)
    //            {
    //                _coreFrxNode = rootNode.SelectSingleNode("//{0}".FormatString(CORE_FRX_NODE_NAME));
    //                if (_coreFrxNode != null)
    //                    _navigator = _coreFrxNode.CreateNavigator();
    //            }
    //        }
    //    }

    //    private ChoCoreFrxSettings()
    //    {
    //    }

    //    #region Constructors

    //    #region Instance Members (Public)

    //    public string this[string name]
    //    {
    //        get
    //        {
    //            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");
    //            if (_coreFrxNode != null)
    //            {
    //                (string)_navigator.Evaluate(String.Format("string(@{0})", ConnectionStringToken), ChoXmlDocument.NameSpaceManager);

    //                _coreFrxNode.SelectSingleNode("logFolder

    //            }
    //            return null;
    //        }
    //    }

    //    #endregion Instance Members (Public)

    //    #region Shared Members (Public)

    //    internal ChoCoreFrxSettings Me
    //    {
    //        get { return _instance; }
    //    }

    //    #endregion Shared Members (Public)
    //}
}
