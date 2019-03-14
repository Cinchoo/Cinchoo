namespace Cinchoo.Core.Instrumentation.Performance.MetaData
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using Cinchoo.Core.Diagnostics;
	using Cinchoo.Core.Xml;
	using System.IO;
	using System.Configuration;
	using Cinchoo.Core.IO;
    using Cinchoo.Core;
    using System.Xml.Serialization;
    using Cinchoo.Core.Configuration.Watchers.File;
using Cinchoo.Core.Services;
    using Cinchoo.Core.Configuration;

	#endregion NameSpaces

    [ChoAppDomainEventsRegisterableType]
    public static class ChoPCMetaDataManager
	{
		#region Constants

        private const string PC_METADATA_FILEPATH_TAG = "pcMetaDataFilePath";

		#endregion Constants

		#region Shared Data Members (Private)

        private static XmlNode _rootNode;
        private static string[] _includeFiles;

        private static bool _isInitialized = false;
        private static readonly object _padLock = new object();
        private static string MetaDataFilepath;
        private static ChoAppConfigurationChangeFileWatcher _configurationChangeWatcher;
        private static readonly object _configurationChangeWatcherLock = new object();
        private static readonly ChoDictionaryService<int, ChoPCMetaDataInfo> _defaultMetaDataCache = new ChoDictionaryService<int, ChoPCMetaDataInfo>("PCMetaData_Dictionary_Service");

		#endregion Shared Data Members (Private)

        static ChoPCMetaDataManager()
        {
            Initialize();
        }

        private static void Initialize()
        {
            if (_isInitialized)
                return;

            lock (_padLock)
            {
                if (_isInitialized)
                    return;

                try
                {
                    MetaDataFilepath = ConfigurationManager.AppSettings[PC_METADATA_FILEPATH_TAG];

                    if (MetaDataFilepath.IsNullOrWhiteSpace() || !File.Exists(MetaDataFilepath))
                        MetaDataFilepath = ChoPath.AddExtension(Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(ChoApplication.ApplicationConfigFilePath), Path.GetFileNameWithoutExtension(ChoApplication.ApplicationConfigFilePath)), "perf"),
                            ChoReservedFileExt.MetaData);

                    ChoXmlDocument.CreateXmlFileIfEmpty(MetaDataFilepath);
                    Refresh();
                }
                finally
                {
                    _isInitialized = true;
                }
            }
        }

        public static void SetWatcher(ChoPerformanceCounter pc)
        {
            if (pc == null)
                return;

            Initialize();

            ConfigurationChangeWatcher.SetConfigurationChangedEventHandler("{0}_PCMetaData_WatcherHandler".FormatString("{0}/{1}/{2}".FormatString(pc.CategoryName, pc.CounterName, pc.InstanceName)),
                (sender, e) =>
                {
                    if (ChoPCMetaDataManager.IsMetaDataModified(pc))
                    {
                        pc.RefreshMetaData(GetMetaDataSectionNSaveIfEmpty(pc));
                    }
                });
            pc.RefreshMetaData(GetMetaDataSection(pc));
        }

        public static void DisposeWatcher(ChoBaseConfigurationElement configElement)
        {
            if (configElement == null)
                return;

            ConfigurationChangeWatcher.SetConfigurationChangedEventHandler("{0}_PCMetaData_WatcherHandler".FormatString(configElement.ConfigElementPath), NullConfigurationChangedEventHandler);
        }
        
        public static ChoPCMetaDataInfo GetMetaDataSectionNSaveIfEmpty(ChoPerformanceCounter pc)
        {
            ChoPCMetaDataInfo metaData = GetMetaDataSection(pc);
            if (metaData == null)
            {
                SaveMetaDataSection(pc);
                return pc.MetaDataInfo;
            }
            else
                return metaData;
        }

        public static ChoPCMetaDataInfo GetMetaDataSection(ChoPerformanceCounter pc)
		{
            if (pc == null)
                return null;

            string xPath = GetXPath(pc);

			XmlNode node = null;
            if (_rootNode != null)
                node = _rootNode.SelectSingleNode(xPath);

            if (node != null)
            {
                XmlAttributeOverrides overrides = new XmlAttributeOverrides();
                XmlAttributes attr = new XmlAttributes();
                attr.XmlRoot = new XmlRootAttribute(node.Name);
                overrides.Add(typeof(ChoPCMetaDataInfo), attr);

                ChoPCMetaDataInfo metaDataInfo = node.ToObject(typeof(ChoPCMetaDataInfo), overrides) as ChoPCMetaDataInfo;
                return metaDataInfo;
            }
            else
                return null;
		}

        public static void SetMetaDataSection(ChoPerformanceCounter pc)
        {
            if (pc == null)
                return;

            if (ChoObject.Equals<ChoPCMetaDataInfo>(pc.MetaDataInfo, GetMetaDataSection(pc)))
                return;

            SaveMetaDataSection(pc);
        }

        [ChoAppDomainUnloadMethod("Shutting down MetaDataManager...")]
        private static void Shutdown()
        {
            if (_configurationChangeWatcher != null)
                _configurationChangeWatcher.Dispose();
            _configurationChangeWatcher = null;
        }

        public static bool IsMetaDataModified(ChoPerformanceCounter pc)
        {
            if (pc == null)
                return false;

            ChoPCMetaDataInfo newConfigurationMetaDataInfo = GetMetaDataSection(pc);
            return !ChoObject.Equals<ChoPCMetaDataInfo>(newConfigurationMetaDataInfo, pc.MetaDataInfo);
        }

        #region Instance Members (Private)

        private static string GetXPath(ChoPerformanceCounter pc)
        {
            if (pc.InstanceName.IsNullOrEmpty())
                return "//PerformanceCounterCategory[@name='{0}']/PerformanceCounter[@name='{1}']".FormatString(pc.CategoryName, pc.CounterName);
            else
                return "//PerformanceCounterCategory[@name='{0}']/PerformanceCounter[@name='{1}' and @instanceName='{2}']".FormatString(pc.CategoryName, pc.CounterName, pc.InstanceName);
        }

        private static void SaveMetaDataSection(ChoPerformanceCounter pc)
        {
            string xPath = GetXPath(pc);
            ChoPCMetaDataInfo metaDataInfo = pc.MetaDataInfo;

            using (ChoXmlDocument xmlDocument = new ChoXmlDocument(MetaDataFilepath, false, false))
            {
                if (metaDataInfo == null)
                    return;

                XmlNode node = xmlDocument.XmlDocument.SelectSingleNode(xPath);
                if (node == null)
                    node = xmlDocument.XmlDocument.MakeXPath(xPath);

                if (node != null)
                {
                    ChoXmlDocument.SetOuterXml(node, metaDataInfo.ToXml());
                    xmlDocument.Save();
                }
            }
        }

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
                        try
                        {
                            _configurationChangeWatcher = new ChoAppConfigurationChangeFileWatcher("PCmeta-data_configurations", MetaDataFilepath, _includeFiles);
                            _configurationChangeWatcher.SetConfigurationChangedEventHandler("ChoPCMetaDataManager_Watcher",
                                (sender, e) =>
                                {
                                    Refresh();
                                });

                            if (!MetaDataFilepath.IsNullOrWhiteSpace() && File.Exists(MetaDataFilepath))
                            {
                                Refresh();
                            }

                            _configurationChangeWatcher.StartWatching();
                        }
                        catch (Exception ex)
                        {
                            ChoTrace.Error(ex.ToString());
                        }
                        finally
                        {
                        }
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
                LoadFile();
            }
            catch (Exception ex)
            {
                ChoTrace.Error(ex);

                try
                {
                    string newFile = ChoPath.AddExtension(MetaDataFilepath, ChoReservedFileExt.Err);
                    if (File.Exists(newFile))
                        File.Delete(newFile);
                    File.Move(MetaDataFilepath, newFile);

                    ChoXmlDocument.CreateXmlFileIfEmpty(MetaDataFilepath);
                    LoadFile();
                }
                catch (Exception innerEx)
                {
                    ChoTrace.Error(innerEx);
                }
            }
        }

        private static void LoadFile()
        {
            using (ChoXmlDocument xmlDocument = new ChoXmlDocument(MetaDataFilepath, false, false))
            {
                _rootNode = xmlDocument.XmlDocument.DocumentElement;
                _includeFiles = xmlDocument != null ? xmlDocument.IncludeFiles : null;
            }
        }

        #endregion Instance Members (Private)
    }
}
