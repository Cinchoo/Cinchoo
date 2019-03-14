namespace Cinchoo.Core.Pattern
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.IO;
    using Cinchoo.Core.Services;
    using Cinchoo.Core.Xml;
    using Cinchoo.Core.Configuration;

    #endregion NameSpaces

    public abstract class ChoMetaDataManager<T, TMetaDataInfo> : ChoDisposableObject
        where T : IChoMetaDataObject<TMetaDataInfo>
        where TMetaDataInfo : class, IEquatable<TMetaDataInfo>, new()
    {
        #region Shared Data Members (Private)

        private static readonly object _metaDataManagersLock = new object();
        private static readonly List<ChoMetaDataManager<T, TMetaDataInfo>> _metaDataManagers = new List<ChoMetaDataManager<T, TMetaDataInfo>>();

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Private)

        protected XmlNode RootNode;
        private string[] _includeFiles;

        private bool _isInitialized = false;
        private readonly object _padLock = new object();
        private ChoAppConfigurationChangeFileWatcher _configurationChangeWatcher;
        private readonly object _configurationChangeWatcherLock = new object();
        private readonly ChoDictionaryService<int, TMetaDataInfo> _defaultMetaDataCache = new ChoDictionaryService<int, TMetaDataInfo>("PCMetaData_Dictionary_Service");
        private readonly Type _metaDataInfoType = typeof(TMetaDataInfo);

        #endregion Instance Data Members (Private)

        #region Constructors

        protected ChoMetaDataManager()
        {
            lock (_metaDataManagersLock)
            {
                Initialize();
                _metaDataManagers.Add(this);
            }
        }

        #endregion Constructors

        #region Instance Properties (Public)

        public abstract string MetaDataFilePath
        {
            get;
        }

        #endregion Instance Properties (Public)

        #region Instance Members (Public)

        public void SetWatcher(T pc)
        {
            if (pc == null)
                return;

            Initialize();

            SetMetaDataSection(pc);
            ConfigurationChangeWatcher.SetConfigurationChangedEventHandler("{0}_{1}_WatcherHandler".FormatString(pc.Name, _metaDataInfoType.Name),
                (sender, e) =>
                {
                    if (IsMetaDataModified(pc))
                    {
                        pc.SetMetaData(GetMetaDataSectionNSaveIfEmpty(pc));
                    }
                });
            pc.SetMetaData(GetMetaDataSection(pc));
        }

        public TMetaDataInfo GetMetaDataSection(T pc)
        {
            if (pc != null)
            {
                string xPath = pc.NodeLocateXPath;

                XmlNode node = null;
                if (RootNode != null)
                    node = RootNode.SelectSingleNode(xPath);

                if (node != null)
                {
                    XmlAttributeOverrides overrides = new XmlAttributeOverrides();
                    XmlAttributes attr = new XmlAttributes();
                    attr.XmlRoot = new XmlRootAttribute(node.Name);
                    overrides.Add(typeof(TMetaDataInfo), attr);

                    TMetaDataInfo metaDataInfo = (TMetaDataInfo)node.ToObject(typeof(TMetaDataInfo), overrides);
                    return metaDataInfo;
                }
            }

            return null;
        }

        public void SetMetaDataSection(T pc)
        {
            if (pc == null)
                return;
            
            TMetaDataInfo metaDataInfo = GetMetaDataSection(pc);
            if (metaDataInfo != null && ChoObject.Equals<TMetaDataInfo>(pc.MetaDataInfo, metaDataInfo))
                return;

            SaveMetaDataSection(pc);
        }

        public void ResetWatcher(T pc)
        {
            if (pc == null)
                return;

            ConfigurationChangeWatcher.SetConfigurationChangedEventHandler("{0}_{1}_WatcherHandler".FormatString(pc.Name, _metaDataInfoType.Name),
                (sender, e) =>
                {
                });
        }

        public bool IsMetaDataModified(T pc)
        {
            if (pc == null)
                return false;

            TMetaDataInfo newConfigurationMetaDataInfo = GetMetaDataSection(pc);
            return !ChoObject.Equals<TMetaDataInfo>(newConfigurationMetaDataInfo, pc.MetaDataInfo);
        }

        #endregion Instance Members (Public)

        #region Instance Members (Private)

        private void SaveMetaDataSection(T pc)
        {
            string xPath = pc.NodeLocateXPath;
            TMetaDataInfo metaDataInfo = pc.MetaDataInfo;

            using (ChoXmlDocument xmlDocument = new ChoXmlDocument(MetaDataFilePath))
            {
                if (metaDataInfo == null)
                    return;

                XmlNode node = xmlDocument.XmlDocument.SelectSingleNode(xPath);
                if (node == null)
                    node = xmlDocument.XmlDocument.MakeXPath(pc.NodeCreateXPath);

                if (node != null)
                {
                    ChoXmlDocument.SetOuterXml(node, metaDataInfo.ToXml());
                    xmlDocument.Save();
                }
            }
        }

        private ChoAppConfigurationChangeFileWatcher ConfigurationChangeWatcher
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
                            _configurationChangeWatcher = new ChoAppConfigurationChangeFileWatcher("ChoPCMetaDataManager_Watcher", MetaDataFilePath, _includeFiles);
                            _configurationChangeWatcher.SetConfigurationChangedEventHandler("ChoPCMetaDataManager_Watcher",
                                (sender, e) =>
                                {
                                    Refresh();
                                });

                            if (!MetaDataFilePath.IsNullOrWhiteSpace() && File.Exists(MetaDataFilePath))
                            {
                                Refresh();
                            }

                            _configurationChangeWatcher.StartWatching();
                        }
                        catch (ChoFatalApplicationException)
                        {
                            throw;
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

        private void Refresh()
        {
            try
            {
                LoadFile();
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ChoTrace.Error(ex);

                try
                {
                    string newFile = ChoPath.AddExtension(MetaDataFilePath, ChoReservedFileExt.Err);
                    if (File.Exists(newFile))
                        File.Delete(newFile);
                    File.Move(MetaDataFilePath, newFile);

                    ChoXmlDocument.CreateXmlFileIfEmpty(MetaDataFilePath);
                    LoadFile();
                }
                catch (Exception innerEx)
                {
                    ChoTrace.Error(innerEx);
                }
            }
        }

        private void LoadFile()
        {
            using (ChoXmlDocument xmlDocument = new ChoXmlDocument(MetaDataFilePath))
            {
                RootNode = xmlDocument.XmlDocument.DocumentElement;
                _includeFiles = xmlDocument != null ? xmlDocument.IncludeFiles : null;
            }
        }

        private void Initialize()
        {
            if (_isInitialized)
                return;

            lock (_padLock)
            {
                if (_isInitialized)
                    return;

                try
                {
                    ChoXmlDocument.CreateXmlFileIfEmpty(MetaDataFilePath);
                    Refresh();
                }
                finally
                {
                    _isInitialized = true;
                }
            }
        }

        private TMetaDataInfo GetMetaDataSectionNSaveIfEmpty(T pc)
        {
            TMetaDataInfo metaData = GetMetaDataSection(pc);
            if (metaData == null)
            {
                SaveMetaDataSection(pc);
                return pc.MetaDataInfo;
            }
            else
                return metaData;
        }

        #endregion Instance Members (Private)

        [ChoAppDomainUnloadMethod("Shutting down MetaDataManagers...")]
        private static void Shutdown()
        {
            lock (_metaDataManagersLock)
            {
                if (_metaDataManagers != null)
                {
                    foreach (IDisposable disposableObj in _metaDataManagers)
                    {
                        if (disposableObj == null)
                            continue;

                        disposableObj.Dispose();
                    }
                }
            }
        }

        #region IDisposable Overrides

        protected override void Dispose(bool finalize)
        {
            if (_configurationChangeWatcher != null)
                _configurationChangeWatcher.Dispose();
            _configurationChangeWatcher = null;
        }

        #endregion IDisposable Overrides
    }
}
