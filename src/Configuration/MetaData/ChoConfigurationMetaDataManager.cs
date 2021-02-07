namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

    using System;
    using System.IO;
    using System.Xml;
    using System.Linq;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using Cinchoo.Core;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.IO;
    using Cinchoo.Core.Services;
    using Cinchoo.Core.Xml;
using System.Reflection;
    using Cinchoo.Core.Xml.Serialization;
    
	#endregion NameSpaces

    [ChoAppDomainEventsRegisterableType]
    internal static class ChoConfigurationMetaDataManager
	{
		#region Shared Data Members (Private)

        private static XmlNode _rootNode;
        private static string[] _includeFiles;
        private static string _metaDataFilepath;
        private static ChoAppConfigurationChangeFileWatcher _configurationChangeWatcher;

        private static readonly object _padLock = new object();
        private static readonly object _configurationChangeWatcherLock = new object();
        private static readonly Dictionary<string, ChoPropertyInfos> _propDict = new Dictionary<string, ChoPropertyInfos>();
        private static readonly ChoDictionaryService<string, ChoBaseConfigurationMetaDataInfo> _defaultMetaDataCache = new ChoDictionaryService<string, ChoBaseConfigurationMetaDataInfo>("DefaultMetaDataInfoCache");

		#endregion Shared Data Members (Private)

		#region Constructors

        static ChoConfigurationMetaDataManager()
        {
            ChoEnvironmentSettings.SetEnvironmentChangedEventHandlerNoCall("meta-data_configurations",
                (sender, e) =>
                {
                    Refresh();
                });
        }

        #endregion Constructors

        #region Shared Members (Public)

        public static void SetWatcher(ChoBaseConfigurationElement configElement)
        {
            if (configElement == null)
                return;

            //if (configElement.WatchChange)
            //{
            if (ConfigurationChangeWatcher != null)
            {
                ConfigurationChangeWatcher.SetConfigurationChangedEventHandler("{0}_MetaData_WatcherHandler".FormatString(configElement.ConfigElementPath),
                    (sender, e) =>
                    {
                        if (ChoConfigurationMetaDataManager.IsMetaDataModified(configElement))
                        {
                            configElement.Refresh();
                        }
                    });
            }
            //}
        }

        public static void DisposeWatcher(ChoBaseConfigurationElement configElement)
        {
            if (configElement == null)
                return;

            if (ConfigurationChangeWatcher != null)
                ConfigurationChangeWatcher.SetConfigurationChangedEventHandler("{0}_MetaData_WatcherHandler".FormatString(configElement.ConfigElementPath), NullConfigurationChangedEventHandler);
        }

        public static ChoBaseConfigurationMetaDataInfo GetMetaDataSection(ChoBaseConfigurationElement configElement)
		{
            if (configElement == null)
                return null;

            Type configurationMetaDataType = configElement.ConfigurationMetaDataType;
            string configElementPath = configElement.ConfigElementPath;

			XmlNode node = null;
            if (_rootNode != null && !configElementPath.IsNullOrWhiteSpace())
			{
				node = _rootNode.SelectSingleNode(@"//{0}".FormatString(configElementPath));
			}
            if (node != null)
            {
                XmlAttributeOverrides overrides = new XmlAttributeOverrides();
                XmlAttributes attr = new XmlAttributes();
                attr.XmlRoot = new XmlRootAttribute(node.Name);
                overrides.Add(configurationMetaDataType, attr);

                ChoBaseConfigurationMetaDataInfo metaDataInfo = node.ToObject(configurationMetaDataType, overrides) as ChoBaseConfigurationMetaDataInfo;
                return metaDataInfo;
            }

            //configElement.ForcePersist(true);
            return null;
		}

        public static void SetMetaDataSection(ChoBaseConfigurationElement configElement)
        {
            if (configElement == null)
                return;
            if (ChoObject.Equals<ChoBaseConfigurationMetaDataInfo>(configElement.MetaDataInfo, GetMetaDataSection(configElement)))
                return;

            string configElementPath = configElement.ConfigElementPath;
            ChoBaseConfigurationMetaDataInfo configurationMetaDataInfo = configElement.MetaDataInfo;
            configurationMetaDataInfo = ChoObject.Merge<ChoBaseConfigurationMetaDataInfo>(configurationMetaDataInfo, GetDefaultMetaDataInfo(configElement));

            if (configurationMetaDataInfo == null)
                return;

            using (ChoXmlDocument xmlDocument = new ChoXmlDocument(_metaDataFilepath))
            {
                if (configElementPath.IsNullOrEmpty())
                    return;

                XmlNode node = xmlDocument.XmlDocument.SelectSingleNode(configElementPath) ;
                if (node == null)
                    node = xmlDocument.XmlDocument.MakeXPath(configElementPath);
             
                if (node != null)
                {
                    ChoXmlDocument.SetOuterXml(node, configurationMetaDataInfo.ToXml());
                    xmlDocument.XmlDocument.InnerXml = ChoXmlDocument.AppendToInnerXml(node, GetPropertyInfos(configElement).ToXml());

                    xmlDocument.Save();
                }
            }
        }

        #endregion Shared Members (Public)

        #region Shared Members (Private)

        private static ChoAppConfigurationChangeFileWatcher ConfigurationChangeWatcher
        {
            get
            {
                if (_configurationChangeWatcher != null)
                    return _configurationChangeWatcher;

                lock (_configurationChangeWatcherLock)
                {
                    if (_configurationChangeWatcher == null)
                    {
                        Refresh();
                    }
                    return _configurationChangeWatcher;
                }
            }
        }

        private static void NullConfigurationChangedEventHandler(object sender, ChoConfigurationChangedEventArgs e)
        {
        }

        private static void Refresh()
        {
            try
            {
                lock (_padLock)
                {
                    _propDict.Clear();
                }

                if (ChoMetaDataFilePathSettings.Me != null)
                    _metaDataFilepath = ChoMetaDataFilePathSettings.Me.OverridenConfigurationMetaDataFilePath;

                if (!ChoAppFrxSettings.Me.DisableMetaDataConfig)
                {
                    ChoXmlDocument.CreateXmlFileIfEmpty(_metaDataFilepath);
                    _configurationChangeWatcher = new ChoAppConfigurationChangeFileWatcher("meta-data_configurations", _metaDataFilepath, _includeFiles);
                    _configurationChangeWatcher.SetConfigurationChangedEventHandler("ChoMetaDataManager_Watcher",
                        (sender, e) =>
                        {
                            Refresh();
                        });
                }

                if (!_metaDataFilepath.IsNullOrWhiteSpace() && File.Exists(_metaDataFilepath))
                {
                    using (ChoXmlDocument xmlDocument = new ChoXmlDocument(_metaDataFilepath))
                    {
                        _rootNode = xmlDocument.XmlDocument.DocumentElement;
                        _includeFiles = xmlDocument != null ? xmlDocument.IncludeFiles : null;
                    }
                }
            }
            catch (Exception ex)
            {
                ChoTrace.Error(ex.ToString());
                //throw;
            }
        }

        private static ChoPropertyInfos ConstructPropertyInfos(ChoBaseConfigurationElement configElement)
        {
            if (configElement != null && configElement.ConfigbObjectType != null)
            {
                Dictionary<string, ChoPropertyInfos> propDict = _propDict;
                string configElementPath = configElement.ConfigElementPath;

                if (configElementPath.IsNullOrWhiteSpace())
                    return ChoPropertyInfos.Default;

                if (!propDict.ContainsKey(configElementPath))
                {
                    lock (_padLock)
                    {
                        if (!propDict.ContainsKey(configElementPath))
                        {
                            MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(configElement.ConfigbObjectType);
                            if (memberInfos != null && memberInfos.Length > 0)
                            {
                                //Set member values
                                List<ChoPropertyInfoMetaData> propertyInfoList = new List<ChoPropertyInfoMetaData>();
                                ChoPropertyInfoAttribute memberInfoAttribute = null;

                                foreach (MemberInfo memberInfo in memberInfos)
                                {
                                    if (memberInfo.GetCustomAttribute<ChoIgnorePropertyAttribute>() != null)
                                        continue;

                                    memberInfoAttribute = (ChoPropertyInfoAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoPropertyInfoAttribute));
                                    ChoPropertyInfoMetaData propInfo = new ChoPropertyInfoMetaData();

                                    propInfo.Name = ChoType.GetMemberName(memberInfo);

                                    object defaultValue = memberInfo.GetDefaultValue();
                                    if (defaultValue != null)
                                        propInfo.DefaultValue = new ChoCDATA(defaultValue.ToString());

                                    if (memberInfoAttribute != null)
                                    {
                                        propInfo.FallbackValue = new ChoCDATA(memberInfoAttribute.FallbackValue);
                                        propInfo.SourceType = memberInfoAttribute.SourceType;
                                        propInfo.IsExpression = memberInfoAttribute.IsExpression;
                                        propInfo.IsDefaultValueSpecified = memberInfoAttribute.IsDefaultValueSpecified;
                                        propInfo.IsFallbackValueSpecified = memberInfoAttribute.IsFallbackValueSpecified;
                                    }

                                    propertyInfoList.Add(propInfo);
                                }

                                if (propertyInfoList.Count > 0)
                                {
                                    ChoPropertyInfos propertyInfos = new ChoPropertyInfos();
                                    propertyInfos.PropertyInfoArr = propertyInfoList.ToArray();
                                    return propertyInfos;
                                }
                            }
                        }
                    }
                }
            }

            return ChoPropertyInfos.Default;
        }

        [ChoAppDomainUnloadMethod("Shutting down MetaDataManager...")]
        private static void Shutdown()
        {
            if (_configurationChangeWatcher != null)
                _configurationChangeWatcher.Dispose();
            _configurationChangeWatcher = null;
        }

        internal static bool IsMetaDataModified(ChoBaseConfigurationElement configElement)
        {
            if (configElement == null)
                return false;

            ChoBaseConfigurationMetaDataInfo newConfigurationMetaDataInfo = GetMetaDataSection(configElement);
            return !ChoObject.Equals<ChoBaseConfigurationMetaDataInfo>(newConfigurationMetaDataInfo, configElement.MetaDataInfo);
        }

        internal static void SetDefaultMetaDataInfo(ChoBaseConfigurationElement configElement)
        {
            if (configElement == null)
                return;

            _defaultMetaDataCache.SetValue(configElement.ConfigElementPath, configElement.MetaDataInfo);
        }

        internal static ChoBaseConfigurationMetaDataInfo GetDefaultMetaDataInfo(ChoBaseConfigurationElement configElement)
        {
            if (configElement == null)
                return null;

            return _defaultMetaDataCache.GetValue(configElement.ConfigElementPath);
        }


        internal static bool TryGetFallbackValue(ChoBaseConfigurationElement configElement, string propName,
            ChoPropertyInfoAttribute memberInfoAttribute, out string configFallbackValue)
        {
            configFallbackValue = null;

            ChoGuard.ArgumentNotNull(configElement, "ConfigElement");
            ChoGuard.ArgumentNotNull(propName, "PropertyName");

            ChoPropertyInfos propertyInfo = GetPropertyInfos(configElement);
            if (propertyInfo != null)
            {
                ChoPropertyInfoMetaData propInfo = propertyInfo[propName];
                if (propInfo == null)
                {
                    if (memberInfoAttribute != null && memberInfoAttribute.IsFallbackValueSpecified)
                    {
                        configFallbackValue = memberInfoAttribute.FallbackValue;
                        return memberInfoAttribute.IsFallbackValueSpecified;
                    }
                }

                configFallbackValue = propInfo != null && propInfo.FallbackValue != null ? propInfo.FallbackValue.Value : null;
                return propInfo != null ? propInfo.IsFallbackValueSpecified : false;
            }
            else if (memberInfoAttribute != null && memberInfoAttribute.IsFallbackValueSpecified)
            {
                configFallbackValue = memberInfoAttribute.FallbackValue;
                return memberInfoAttribute.IsFallbackValueSpecified;
            }

            return false;
        }

        internal static bool TryGetDefaultValue(ChoBaseConfigurationElement configElement, string propName,
            ChoPropertyInfoAttribute memberInfoAttribute, out string configDefaultValue)
        {
            configDefaultValue = null;

            ChoGuard.ArgumentNotNull(configElement, "ConfigElement");
            ChoGuard.ArgumentNotNull(propName, "PropertyName");

            ChoPropertyInfos propertyInfo = GetPropertyInfos(configElement);
            if (propertyInfo != null)
            {
                ChoPropertyInfoMetaData propInfo = propertyInfo[propName];
                if (propInfo == null)
                {
                    if (memberInfoAttribute != null && memberInfoAttribute.IsDefaultValueSpecified)
                    {
                        configDefaultValue = memberInfoAttribute.DefaultValue;
                        return memberInfoAttribute.IsDefaultValueSpecified;
                    }
                }

                configDefaultValue = propInfo != null && propInfo.DefaultValue != null ? propInfo.DefaultValue.Value : null;
                return propInfo != null ? propInfo.IsDefaultValueSpecified : false;
            }
            else if (memberInfoAttribute != null && memberInfoAttribute.IsDefaultValueSpecified)
            {
                configDefaultValue = memberInfoAttribute.DefaultValue;
                return memberInfoAttribute.IsDefaultValueSpecified;
            }

            return false;
        }

        internal static Type GetSourceType(ChoBaseConfigurationElement configElement, string propName,
            ChoPropertyInfoAttribute memberInfoAttribute)
        {
            ChoGuard.ArgumentNotNull(configElement, "ConfigElement");
            ChoGuard.ArgumentNotNull(propName, "PropertyName");

            ChoPropertyInfos propertyInfo = GetPropertyInfos(configElement);
            if (propertyInfo != null)
            {
                ChoPropertyInfoMetaData propInfo = propertyInfo[propName];
                if (propInfo != null && propInfo.SourceType != null)
                    return propInfo.SourceType;
            }

            if (memberInfoAttribute != null && memberInfoAttribute.SourceType != null)
                return memberInfoAttribute.SourceType;

            return null;
        }

        internal static bool IsExpressionProperty(ChoBaseConfigurationElement configElement, string propName,
            ChoPropertyInfoAttribute memberInfoAttribute)
        {
            ChoGuard.ArgumentNotNull(configElement, "ConfigElement");
            ChoGuard.ArgumentNotNull(propName, "PropertyName");

            ChoPropertyInfos propertyInfo = GetPropertyInfos(configElement);
            if (propertyInfo != null)
            {
                ChoPropertyInfoMetaData propInfo = propertyInfo[propName];
                if (propInfo != null)
                    return propInfo.IsExpression;
            }

            if (memberInfoAttribute != null)
                return memberInfoAttribute.IsExpression;

            return false;
        }


        private static ChoPropertyInfos GetPropertyInfos(ChoBaseConfigurationElement configElement)
        {
            string configElementPath = configElement.ConfigElementPath;
            if (configElementPath.IsNullOrWhiteSpace()) return null;

            Dictionary<string, ChoPropertyInfos> propDict = _propDict;
            if (!propDict.ContainsKey(configElementPath))
            {
                lock (_padLock)
                {
                    if (!propDict.ContainsKey(configElementPath))
                    {
                        string xpath = @"//{0}/propertyInfos".FormatString(configElementPath);
                        XmlNode node = null;
                        
                        if (_rootNode != null)
                            node = _rootNode.SelectSingleNode(xpath);

                        if (node != null)
                        {
                            propDict.Add(configElementPath, ChoPropertyInfos.Default);

                            ChoPropertyInfos propertyInfos = node.ToObject(typeof(ChoPropertyInfos)) as ChoPropertyInfos;
                            propertyInfos.Initialize();
                            propDict[configElementPath] = propertyInfos;
                        }
                        else
                            propDict[configElementPath] = ConstructPropertyInfos(configElement);
                    }
                }
            }

            return propDict.ContainsKey(configElementPath) ? propDict[configElementPath] : null;
        }

        #endregion Shared Members (Private)
    }

    public class ChoPropertyInfoMetaData
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlElement("defaultValue")]
        public ChoCDATA DefaultValue;
        [XmlElement("fallbackValue")]
        public ChoCDATA FallbackValue;
        [XmlAttribute("sourceType")]
        public string SourceTypeText;
        [XmlAttribute("isExpression")]
        public bool IsExpression;

        private Type _sourceType;
        [XmlIgnore]
        public Type SourceType
        {
            get { return _sourceType; }
            set
            {
                _sourceType = value;
                if (_sourceType != null)
                    SourceTypeText = _sourceType.ToString();
            }
        }

        [XmlIgnore]
        public bool IsDefaultValueSpecified;
        [XmlIgnore]
        public bool IsFallbackValueSpecified;

        internal void Initialize()
        {
            IsDefaultValueSpecified = DefaultValue != null;
            IsFallbackValueSpecified = FallbackValue != null;

            if (!SourceTypeText.IsNullOrWhiteSpace())
            {
                try
                {
                    SourceType = Type.GetType(SourceTypeText);
                }
                catch { }
            }
        }
    }

    [XmlRoot("propertyInfos")]
    public class ChoPropertyInfos
    {
        public static readonly ChoPropertyInfos Default = new ChoPropertyInfos();

        [XmlElement("propertyInfo", typeof(ChoPropertyInfoMetaData))]
        public ChoPropertyInfoMetaData[] PropertyInfoArr;

        [XmlIgnore]
        public ChoPropertyInfoMetaData this[string propName]
        {
            get
            {
                if (PropertyInfoArr != null)
                {
                    return (from propInfo in PropertyInfoArr 
                            where propInfo.Name == propName
                                select propInfo).FirstOrDefault();
                }

                return null;
            }
        }

        internal void Initialize()
        {
            if (PropertyInfoArr != null)
            {
                foreach (ChoPropertyInfoMetaData propertyInfo in PropertyInfoArr)
                    propertyInfo.Initialize();
            }
        }
    }
}
