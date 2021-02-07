namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.ComponentModel;
    using System.Xml;
    using Cinchoo.Core.Services;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml.Serialization;

    #endregion NameSpaces

    [Serializable]
    public abstract class ChoConfigSection : ChoDisposableObject, INotifyPropertyChanged
    {
        internal bool IsMetaDataDefinitionChanged = false;

        #region Constructors

        protected ChoConfigSection()
        {
        }

        public ChoConfigSection(Type configObjectType, XmlNode node, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
            : this()
        {
            _configElement = configElement;

            if (configElement != null)
            {
                //Get and Set MetaDataFileName
                configElement.ConfigurationMetaDataType = ConfigurationMetaDataType;
                IChoConfigStorage attrDefinedConfigStorage = configElement.MetaDataInfo != null ? configElement.MetaDataInfo.ConfigStorage : null;

                ChoBaseConfigurationMetaDataInfo defaultMetaDataInfo = InitDefaultMetaDataInfo(configElement);

                configElement.MetaDataInfo = ChoObject.Merge<ChoBaseConfigurationMetaDataInfo>(ChoConfigurationMetaDataManager.GetMetaDataSection(configElement), defaultMetaDataInfo);

                ChoBaseConfigurationMetaDataInfo origMetaDataInfo = configElement.MetaDataInfo.Clone();
                InvokeOverrideConfigurationMetaDataInfo(configElement);
                configElement.ReapplyConfigMetaData();

                ConfigStorage = configElement.MetaDataInfo != null && configElement.MetaDataInfo.ConfigStorage != null ?
                    configElement.MetaDataInfo.ConfigStorage : attrDefinedConfigStorage;

                if (ConfigStorage == null)
                {
                    configElement.Log("Missing configuration storage, assigning to configSection default storage.");
                    ConfigStorage = DefaultConfigStorage;
                    if (ConfigStorage == null)
                    {
                        configElement.Log("Missing configuration storage, assigning to system default storage.");
                        ConfigStorage = ChoConfigStorageManagerSettings.Me.GetDefaultConfigStorage();
                    }
                }

                if (ConfigStorage != null)
                {
                    try
                    {
                        CheckValidConfigStoragePassed(ConfigStorage);
                    }
                    catch (ChoFatalApplicationException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        if (configElement.Silent)
                        {
                            ConfigElement.Log(ex.Message);
                            ConfigStorage = ChoConfigStorageManagerSettings.Me.GetDefaultConfigStorage();
                            if (ConfigStorage != null)
                                ConfigElement.Log("Using default [{0}] config storage.".FormatString(ConfigStorage.GetType().FullName));
                        }
                        else
                            throw;
                    }
                }

                if (ConfigStorage == null)
                    throw new ChoConfigurationException("Missing configuration storage.");

                IsMetaDataDefinitionChanged = !ChoObject.Equals<ChoBaseConfigurationMetaDataInfo>(ChoConfigurationMetaDataManager.GetMetaDataSection(configElement), origMetaDataInfo);
                configElement.MetaDataInfo.ConfigStorage = ConfigStorage;

                try
                {
                    ConfigData = ConfigStorage.Load(_configElement, node);
                }
                catch (ChoFatalApplicationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _configLoadExceptions.Add(ex);
                }
            }

            //if (ConfigStorage == null)
            //{
            //    try
            //    {
            //        ConfigStorage = DefaultConfigStorage;
            //        if (ConfigStorage == null)
            //            ConfigStorage = ChoConfigStorageManagerSettings.Me.GetDefaultConfigStorage();
            //    }
            //    catch (Exception ex)
            //    {
            //        _configLoadExceptions.Add(ex);
            //    }
            //}

            try
            {
                ConfigurationChangeWatcher = ConfigStorage.ConfigurationChangeWatcher;
                if (ConfigurationChangeWatcher != null)
                    ConfigurationChangeWatcher.StartWatching();
            }
            catch (Exception ex)
            {
                _configLoadExceptions.Add(ex);
            }
        }

        #endregion Constructors

        public virtual void Initialize()
        {
            if (ConfigStorage != null)
                ConfigStorage.PostLoad(_configElement);

        }

        #region IChoConfigSectionable Members

        protected virtual void CheckValidConfigStoragePassed(IChoConfigStorage configStorage)
        {
        }

        public IChoConfigurationChangeWatcher ConfigurationChangeWatcher
        {
            get;
            set;
        }

        private readonly ChoBaseConfigurationElement _configElement;
        public ChoBaseConfigurationElement ConfigElement
        {
            get { return _configElement; }
        }

        private List<Exception> _configLoadExceptions = new List<Exception>();

        public Exception ConfigLoadException
        {
            get { return _configLoadExceptions.Count == 0 ? null : new ChoAggregateException(_configLoadExceptions.ToArray()); }
        }

        private readonly object _padLock = new object();
        public object SyncRoot
        {
            get { return _padLock; }
        }

        private object _data;
        [XmlIgnore]
        public virtual object ConfigData
        {
            get { return _data; }
            protected set { _data = value; }
        }

        public abstract object this[string key]
        {
            get;
        }

        public abstract bool HasConfigMemberDefined(string key);

        public virtual bool HasConfigSectionDefined
        {
            get { return ConfigStorage.IsConfigSectionDefined; }
        }

        private object _configObject;
        public object ConfigObject
        {
            get { return _configObject; }
            set
            {
                _configObject = value;
                NotifyPropertyChanged("ConfigObject");
            }
        }

        public IChoConfigStorage ConfigStorage
        {
            get { return ConfigElement != null ? ConfigElement.ConfigStorage : null; }
            set
            {
                if (ConfigElement != null)
                    ConfigElement.ConfigStorage = value;
            }
        }

        public virtual Type ConfigurationMetaDataType
        {
            get { return typeof(ChoStandardConfigurationMetaDataInfo); }
        }

        public virtual object PersistableState
        {
            get { return ConfigStorage.PersistableState; }
        }

        public virtual void Persist(string configSectionFullPath, ChoDictionaryService<string, object> stateInfo)
        {
            ChoQueuedExecutionService.Global.Enqueue(() =>
            {
                object state = null;

                lock (_padLock)
                {
                    state = PersistableState;
                }

                if (state != null)
                {
                    IChoConfigStorage configStorage = ConfigStorage;
                    if (configStorage != null)
                    {
                        if (ConfigStorage.CanPersist(state, stateInfo))
                        {
                            StopWatching();

                            try
                            {
                                ConfigStorage.Persist(state, stateInfo);

                                //Call AfterConfigurationMemberPersist for each member
                                if (ConfigObject is ChoConfigurableObject)
                                    ((ChoConfigurableObject)ConfigObject).CallAfterConfigurationMemberPersist();

                                if (ConfigObject is ChoConfigurableObject)
                                    ((ChoConfigurableObject)ConfigObject).RaiseAfterConfigurationObjectPersisted();
                            }
                            finally
                            {
                                //Reset Watcher
                                ResetWatching();

                                StartWatching();
                            }
                        }
                    }
                }
            }
            );
        }

        public virtual void StartWatching()
        {
            if (ConfigurationChangeWatcher != null)
                ConfigurationChangeWatcher.StartWatching();
        }

        public virtual void StopWatching()
        {
            if (ConfigurationChangeWatcher != null)
                ConfigurationChangeWatcher.StopWatching();
        }

        public virtual void RestartWatching()
        {
            StopWatching();
            StartWatching();
        }

        public virtual void ResetWatching()
        {
            if (ConfigurationChangeWatcher != null)
                ConfigurationChangeWatcher.ResetWatching();
        }

        public virtual void SetWatcher(ChoConfigurationChangedEventHandler configurationChangedEventHandler)
        {
            if (ConfigurationChangeWatcher != null)
                ConfigurationChangeWatcher.SetConfigurationChangedEventHandler(_configElement.ConfigElementPath, configurationChangedEventHandler);
        }

        #endregion

        #region Abstract Members (Protected)

        protected abstract IChoConfigStorage DefaultConfigStorage
        {
            get;
        }

        #endregion Abstract Members (Protected)

        #region ChoDisposableObject Overrides

        protected override void Dispose(bool finalize)
        {
            //ChoQueuedExecutionService.GlobalApplicationQueuedExecutionService.Enqueue(() =>
            //    {

            //********* REVISIT ************* IMPORTANT
            //if (ConfigurationChangeWatcher != null)
            //    ConfigurationChangeWatcher.StopWatching();

            if (ConfigStorage != null)
            {
                ConfigStorage.Dispose();

                //ConfigStorage = null;
            }
            //});
        }

        #endregion ChoDisposableObject Overrides5

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        #endregion

        private void InvokeOverrideConfigurationMetaDataInfo(ChoBaseConfigurationElement configElement)
        {
            ChoBaseConfigurationMetaDataInfo standardConfigurationMetaDataInfo = configElement.MetaDataInfo;
            Type type = configElement.ConfigbObjectType;

            if (standardConfigurationMetaDataInfo == null || type == null) return;

            if (configElement.ConfigObject is ChoConfigurableObject)
                ((ChoConfigurableObject)configElement.ConfigObject).InvokeOverrideMetaDataInfo(standardConfigurationMetaDataInfo);
        }

        private ChoBaseConfigurationMetaDataInfo InitDefaultMetaDataInfo(ChoBaseConfigurationElement configElement)
        {
            ChoBaseConfigurationMetaDataInfo defaultMetaDataInfo = ChoConfigurationMetaDataManager.GetDefaultMetaDataInfo(configElement);
            if (defaultMetaDataInfo.ConfigStorageType.IsNullOrEmpty())
            {
                if (DefaultConfigStorage != null)
                    defaultMetaDataInfo.ConfigStorageType = DefaultConfigStorage.GetType().SimpleQualifiedName();
                else
                {
                    IChoConfigStorage configStorage = ChoConfigStorageManagerSettings.Me.GetDefaultConfigStorage();
                    if (configStorage != null)
                        defaultMetaDataInfo.ConfigStorageType = configStorage.GetType().SimpleQualifiedName();
                }
            }
            return defaultMetaDataInfo;
        }
    }
}
