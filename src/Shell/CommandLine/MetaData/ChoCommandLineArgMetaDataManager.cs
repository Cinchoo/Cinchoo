namespace Cinchoo.Core.Shell
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
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.Xml.Serialization;
    
	#endregion NameSpaces

    [ChoAppDomainEventsRegisterableType]
    internal static class ChoCmdLineArgMetaDataManager
    {
        #region Constants

        private const string META_DATA_KEY = "meta-data-cmdline-configurations";
        private const string META_DATA_FILE_WATCHER_KEY = "cmdline-file-watcher";

        #endregion Constants

        #region Shared Data Members (Private)

        private static XmlNode _rootNode;
        private static string[] _includeFiles;
        private static string _metaDataFilepath;
        private static ChoAppConfigurationChangeFileWatcher _metaDataChangeWatcher;

        private static readonly object _padLock = new object();
        private static readonly object _metaDataChangeWatcherLock = new object();
        private static readonly Dictionary<string, ChoPropertyInfos> _propDict = new Dictionary<string, ChoPropertyInfos>();
        private static readonly Dictionary<string, ChoObjectInfo> _objDict = new Dictionary<string, ChoObjectInfo>();
        private static readonly ChoDictionaryService<string, ChoBaseConfigurationMetaDataInfo> _defaultMetaDataCache = new ChoDictionaryService<string, ChoBaseConfigurationMetaDataInfo>("DefaultMetaDataInfoCache");

		#endregion Shared Data Members (Private)

		#region Constructors

        static ChoCmdLineArgMetaDataManager()
        {
            //ChoFramework.Initialize();
            //Refresh();
            //ChoEnvironmentSettings.SetEnvironmentChangedEventHandlerNoCall(META_DATA_KEY,
            //    (sender, e) =>
            //    {
            //        Refresh();
            //    });
        }

        #endregion Constructors

        #region Shared Members (Private)

        private static void SetMetaDataSection(ChoCommandLineArgObject cmdLineArgObject, ChoPropertyInfos propertyInfos)
        {
            return;
            if (cmdLineArgObject == null)
                return;

            string elementPath = cmdLineArgObject.GetType().Name;

            using (ChoXmlDocument xmlDocument = new ChoXmlDocument(_metaDataFilepath))
            {
                if (elementPath.IsNullOrEmpty())
                    return;

                XmlNode node = xmlDocument.XmlDocument.SelectSingleNode(elementPath);
                if (node == null)
                    node = xmlDocument.XmlDocument.MakeXPath(elementPath);

                if (node != null)
                {
                    xmlDocument.XmlDocument.InnerXml = ChoXmlDocument.AppendToInnerXml(node, propertyInfos.ToXml());

                    xmlDocument.Save();
                }
            }
        }

        private static void SetMetaDataSection(Type cmdLineObjType, ChoObjectInfo objectInfo)
        {
            if (cmdLineObjType == null)
                return;

            string elementPath = cmdLineObjType.Name;

            using (ChoXmlDocument xmlDocument = new ChoXmlDocument(_metaDataFilepath))
            {
                if (elementPath.IsNullOrEmpty())
                    return;

                XmlNode node = xmlDocument.XmlDocument.SelectSingleNode(elementPath);
                if (node == null)
                    node = xmlDocument.XmlDocument.MakeXPath(elementPath);

                if (node != null)
                {
                    xmlDocument.XmlDocument.InnerXml = ChoXmlDocument.AppendToInnerXml(node, objectInfo.ToXml());

                    xmlDocument.Save();
                }
            }
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
                    _metaDataFilepath = ChoMetaDataFilePathSettings.Me.OverridenCmdLineArgMetaDataFilePath;

                if (!ChoAppFrxSettings.Me.DisableMetaDataConfig)
                {
                    ChoXmlDocument.CreateXmlFileIfEmpty(_metaDataFilepath);
                    _metaDataChangeWatcher = new ChoAppConfigurationChangeFileWatcher(META_DATA_KEY, _metaDataFilepath, _includeFiles);
                    _metaDataChangeWatcher.SetConfigurationChangedEventHandler(META_DATA_FILE_WATCHER_KEY,
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
         
        private static ChoPropertyInfos ConstructPropertyInfos(ChoCommandLineArgObject cmdLineArgObject)
        {
            if (cmdLineArgObject != null)
            {
                Dictionary<string, ChoPropertyInfos> propDict = _propDict;
                string elementPath = cmdLineArgObject.GetType().Name;

                if (elementPath.IsNullOrWhiteSpace())
                    return ChoPropertyInfos.Default;

                if (!propDict.ContainsKey(elementPath))
                {
                    lock (_padLock)
                    {
                        if (!propDict.ContainsKey(elementPath))
                        {
                            MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(cmdLineArgObject.GetType());
                            if (memberInfos != null && memberInfos.Length > 0)
                            {
                                //Set member values
                                List<ChoPropertyInfo> propertyInfoList = new List<ChoPropertyInfo>();
                                ChoDefaultCommandLineArgAttribute memberInfoAttribute = null;

                                foreach (MemberInfo memberInfo in memberInfos)
                                {
                                    memberInfoAttribute = (ChoDefaultCommandLineArgAttribute)ChoType.GetMemberAttributeByBaseType(memberInfo, typeof(ChoDefaultCommandLineArgAttribute));
                                    if (memberInfoAttribute == null)
                                        continue;

                                    ChoPropertyInfo propInfo = new ChoPropertyInfo();

                                    propInfo.Name = ChoType.GetMemberName(memberInfo);

                                    if (ChoType.GetMemberType(memberInfo) != typeof(bool))
                                    {
                                        propInfo.DefaultValue = new ChoCDATA(memberInfoAttribute.DefaultValue);
                                        propInfo.FallbackValue = new ChoCDATA(memberInfoAttribute.FallbackValue);
                                        propInfo.IsDefaultValueSpecified = memberInfoAttribute.IsDefaultValueSpecified;
                                        propInfo.IsFallbackValueSpecified = memberInfoAttribute.IsFallbackValueSpecified;
                                        propertyInfoList.Add(propInfo);
                                    }
                                }

                                if (propertyInfoList != null)
                                {
                                    ChoPropertyInfos propertyInfos = new ChoPropertyInfos();
                                    propertyInfos.PropertyInfoArr = propertyInfoList.ToArray();

                                    SetMetaDataSection(cmdLineArgObject, propertyInfos);
                                    return propertyInfos;
                                }
                            }
                        }
                    }
                }
            }

            return ChoPropertyInfos.Default;
        }

        private static ChoObjectInfo ConstructObjectInfos(Type cmdLineObjType)
        {
            if (cmdLineObjType != null)
            {
                Dictionary<string, ChoObjectInfo> objDict = _objDict;
                string elementPath = cmdLineObjType.Name;

                if (elementPath.IsNullOrWhiteSpace())
                    return ChoObjectInfo.Default;

                if (!objDict.ContainsKey(elementPath))
                {
                    lock (_padLock)
                    {
                        if (!objDict.ContainsKey(elementPath))
                        {
                            ChoCommandLineArgObjectAttribute commandLineArgObjectAttribute =
                                ChoType.GetAttribute<ChoCommandLineArgObjectAttribute>(cmdLineObjType);

                            if (commandLineArgObjectAttribute != null)
                            {
                                ChoObjectInfo objInfo = new ChoObjectInfo();
                                objInfo.ApplicationName = commandLineArgObjectAttribute.ApplicationName;
                                objInfo.Copyright = commandLineArgObjectAttribute.Copyright;
                                objInfo.Description = new ChoCDATA(commandLineArgObjectAttribute.Description);
                                objInfo.Version = commandLineArgObjectAttribute.Version;
                                objInfo.AdditionalInfo = new ChoCDATA(commandLineArgObjectAttribute.AdditionalInfo);
                                objInfo.ShowUsageIfEmpty = commandLineArgObjectAttribute.ShowUsageIfEmptyInternal;
                                objInfo.DisplayDefaultValue = commandLineArgObjectAttribute.DisplayDefaultValueInternal;
                                objInfo.DoNotShowUsageDetail = commandLineArgObjectAttribute.DoNotShowUsageDetail;
                                SetMetaDataSection(cmdLineObjType, objInfo);

                                return objInfo;
                            }
                        }
                    }
                }
            }

            return ChoObjectInfo.Default;
        }

        [ChoAppDomainUnloadMethod("Shutting down CmdLineArg MetaDataManager...")]
        private static void Shutdown()
        {
            if (_metaDataChangeWatcher != null)
                _metaDataChangeWatcher.Dispose();
            _metaDataChangeWatcher = null;
        }

        internal static void LoadFromConfig(Type cmdLineObjType, ChoCommandLineArgObjectAttribute attr)
        {
            if (attr == null) return;
                        
            ChoObjectInfo objInfo = GetObjectInfo(cmdLineObjType);
            if (objInfo != null)
            {
                if (!objInfo.ApplicationName.IsNullOrEmpty())
                    attr.ApplicationName = objInfo.ApplicationName;
                if (objInfo.AdditionalInfo != null && !objInfo.AdditionalInfo.Value.IsNullOrEmpty())
                    attr.AdditionalInfo = objInfo.AdditionalInfo.Value;
                if (!objInfo.Copyright.IsNullOrEmpty())
                    attr.Copyright = objInfo.Copyright;
                if (objInfo.Description != null && !objInfo.Description.Value.IsNullOrEmpty())
                    attr.Description = objInfo.Description.Value;
                attr.DoNotShowUsageDetail = objInfo.DoNotShowUsageDetail;
                if (objInfo.ShowUsageIfEmpty.HasValue)
                    attr.ShowUsageIfEmptyInternal = objInfo.ShowUsageIfEmpty.Value;
                if (objInfo.DisplayDefaultValue.HasValue)
                    attr.DisplayDefaultValueInternal = objInfo.DisplayDefaultValue.Value;
                if (!objInfo.Version.IsNullOrEmpty())
                    attr.Version = objInfo.Version;
            }
        }

        internal static bool TryGetFallbackValue(ChoCommandLineArgObject target, string propName,
            ChoDefaultCommandLineArgAttribute memberInfoAttribute, out string fallbackValue)
        {
            fallbackValue = null;

            ChoGuard.ArgumentNotNull(target, "CommandLineArgObject");
            ChoGuard.ArgumentNotNull(propName, "PropertyName");

            ChoPropertyInfos propertyInfo = GetPropertyInfos(target);
            if (propertyInfo != null)
            {
                ChoPropertyInfo propInfo = propertyInfo[propName];
                if (propInfo == null)
                {
                    if (memberInfoAttribute != null && memberInfoAttribute.IsFallbackValueSpecified)
                    {
                        fallbackValue = memberInfoAttribute.FallbackValue;
                        return memberInfoAttribute.IsFallbackValueSpecified;
                    }
                }

                fallbackValue = propInfo != null && propInfo.FallbackValue != null ? propInfo.FallbackValue.Value : null;
                return propInfo != null ? propInfo.IsFallbackValueSpecified : false;
            }
            else if (memberInfoAttribute != null && memberInfoAttribute.IsFallbackValueSpecified)
            {
                fallbackValue = memberInfoAttribute.FallbackValue;
                return memberInfoAttribute.IsFallbackValueSpecified;
            }

            return false;
        }

        internal static bool TryGetDefaultValue(ChoCommandLineArgObject target, string propName,
            ChoDefaultCommandLineArgAttribute memberInfoAttribute, out string defaultValue)
        {
            defaultValue = null;

            ChoGuard.ArgumentNotNull(target, "CommandLineArgObject");
            ChoGuard.ArgumentNotNull(propName, "PropertyName");

            ChoPropertyInfos propertyInfo = GetPropertyInfos(target);
            if (propertyInfo != null)
            {
                ChoPropertyInfo propInfo = propertyInfo[propName];
                if (propInfo == null)
                {
                    if (memberInfoAttribute != null && memberInfoAttribute.IsDefaultValueSpecified)
                    {
                        defaultValue = memberInfoAttribute.DefaultValue;
                        return memberInfoAttribute.IsDefaultValueSpecified;
                    }
                }

                defaultValue = propInfo != null && propInfo.DefaultValue != null ? propInfo.DefaultValue.Value : null;
                return propInfo != null ? propInfo.IsDefaultValueSpecified : false;
            }
            else if (memberInfoAttribute != null && memberInfoAttribute.IsDefaultValueSpecified)
            {
                defaultValue = memberInfoAttribute.DefaultValue;
                return memberInfoAttribute.IsDefaultValueSpecified;
            }

            return false;
        }

        private static ChoPropertyInfos GetPropertyInfos(ChoCommandLineArgObject cmdLineArgObject)
        {
            string elementPath = cmdLineArgObject.GetType().Name;
            if (elementPath.IsNullOrWhiteSpace()) return null;

            Dictionary<string, ChoPropertyInfos> propDict = _propDict;
            if (!propDict.ContainsKey(elementPath))
            {
                lock (_padLock)
                {
                    if (!propDict.ContainsKey(elementPath))
                    {
                        string xpath = @"//{0}/propertyInfos".FormatString(elementPath);
                        XmlNode node = null;
                        
                        if (_rootNode != null)
                            node = _rootNode.SelectSingleNode(xpath);

                        if (node != null)
                        {
                            propDict.Add(elementPath, ChoPropertyInfos.Default);

                            ChoPropertyInfos propertyInfos = node.ToObject(typeof(ChoPropertyInfos)) as ChoPropertyInfos;
                            propertyInfos.Initialize();
                            propDict[elementPath] = propertyInfos;
                        }
                        else
                            propDict[elementPath] = ConstructPropertyInfos(cmdLineArgObject);
                    }
                }
            }

            return propDict.ContainsKey(elementPath) ? propDict[elementPath] : null;
        }

        private static ChoObjectInfo GetObjectInfo(Type cmdLineObjType)
        {
            string elementPath = cmdLineObjType.Name;
            if (elementPath.IsNullOrWhiteSpace()) return null;

            Dictionary<string, ChoObjectInfo> objDict = _objDict;
            if (!objDict.ContainsKey(elementPath))
            {
                lock (_padLock)
                {
                    if (!objDict.ContainsKey(elementPath))
                    {
                        string xpath = @"//{0}/objectInfo".FormatString(elementPath);
                        XmlNode node = null;

                        if (_rootNode != null)
                            node = _rootNode.SelectSingleNode(xpath);

                        if (node != null)
                        {
                            objDict.Add(elementPath, ChoObjectInfo.Default);

                            ChoObjectInfo objectInfo = node.ToObject(typeof(ChoObjectInfo)) as ChoObjectInfo;
                            objectInfo.Initialize();
                            objDict[elementPath] = objectInfo;
                        }
                        else
                            objDict[elementPath] = ConstructObjectInfos(cmdLineObjType);
                    }
                }
            }

            return objDict.ContainsKey(elementPath) ? objDict[elementPath] : null;
        }

        #endregion Shared Members (Private)
    }

    [XmlRoot("objectInfo")]
    public class ChoObjectInfo
    {
        public static readonly ChoObjectInfo Default = new ChoObjectInfo();

        [XmlAttribute("applicationName")]
        public string ApplicationName;
        [XmlAttribute("copyright")]
        public string Copyright;
        [XmlElement("description")]
        public ChoCDATA Description;
        [XmlAttribute("version")]
        public string Version;
        [XmlElement("additionalInfo")]
        public ChoCDATA AdditionalInfo;
        [XmlAttribute("doNotShowUsageDetail")]
        public bool DoNotShowUsageDetail;
        [XmlElement("showUsageIfEmpty")]
        public ChoNullable<bool> ShowUsageIfEmpty;
        [XmlElement("displayDefaultValue")]
        public ChoNullable<bool> DisplayDefaultValue;

        internal void Initialize()
        {
        }
    }

    public class ChoPropertyInfo
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlElement("defaultValue")]
        public ChoCDATA DefaultValue;
        [XmlElement("fallbackValue")]
        public ChoCDATA FallbackValue;

        [XmlIgnore]
        public bool IsDefaultValueSpecified;
        [XmlIgnore]
        public bool IsFallbackValueSpecified;

        internal void Initialize()
        {
            IsDefaultValueSpecified = DefaultValue != null && !DefaultValue.Value.IsNullOrEmpty();
            IsFallbackValueSpecified = FallbackValue != null && !FallbackValue.Value.IsNullOrEmpty();
        }
    }

    [XmlRoot("propertyInfos")]
    public class ChoPropertyInfos
    {
        public static readonly ChoPropertyInfos Default = new ChoPropertyInfos();

        [XmlElement("propertyInfo", typeof(ChoPropertyInfo))]
        public ChoPropertyInfo[] PropertyInfoArr;

        [XmlIgnore]
        public ChoPropertyInfo this[string propName]
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
                foreach (ChoPropertyInfo propertyInfo in PropertyInfoArr)
                    propertyInfo.Initialize();
            }
        }
    }
}
