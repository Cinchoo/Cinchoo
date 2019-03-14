namespace Cinchoo.Core
{
	#region Namespaces

    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.IO;
    using System.Collections.Specialized;
    using Cinchoo.Core.Diagnostics;
    using System.Collections.Generic;

	#endregion

    [ChoAppDomainEventsRegisterableType]
    public static class ChoEnvironmentSettings
    {
        #region Shared Data Members (public)

        private static readonly object _padLock = new object();
        private static string _sharedEnvironmentConfigFilePath;
        private static ChoSharedEnvironmentManager _sharedEnvironmentManager;
        private static string _configFilePath = null;
        private static string _environment;
        private static string Environment
        {
            get { return _environment; }
            set { _environment = value; ChoApplication.AppEnvironment = value; }
        }

        private static readonly object _eventHandlerListLock = new object();
        private static readonly OrderedDictionary _eventHandlerList = new OrderedDictionary();

		#endregion

        #region Shared Data Members (Private)

        private static readonly ChoConfigurationChangeFileWatcher _appConfigFileWatcher = new ChoConfigurationChangeFileWatcher("AppConfigFileWatcher", AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        private static ChoConfigurationChangeFileWatcher _sharedEnvConfigFileWatcher = null;

        #endregion Shared Data Members (Private)

        #region Constructors

        static ChoEnvironmentSettings()
        {
            _appConfigFileWatcher.DoNotUseGlobalQueue = true;

            _appConfigFileWatcher.SetConfigurationChangedEventHandler("OnEnvironmentChanged", (sender, e) =>
                {
                    //ConfigurationManager.RefreshSection("appSettings");
                    ChoAppFrxSettings.RefreshSection();

                    bool envChanged = false;

                    string environment = ChoAppFrxSettings.Me.AppEnvironment.NTrim();
                    string sharedEnvironmentConfigFilePath = SharedEnvironmentConfigFilePath;

                    if (environment != Environment)
                    {
                        envChanged = true;
                        ChoApplication.Trace(true, "Environment changed from '{0}' to '{1}'.".FormatString(Environment, environment));
                        Environment = environment;
                    }
                    if (sharedEnvironmentConfigFilePath != _sharedEnvironmentConfigFilePath)
                    {
                        envChanged = true;
                        ChoApplication.Trace(true, "SharedEnvironmentConfgiFilePath changed from '{0}' to '{1}'.".FormatString(_sharedEnvironmentConfigFilePath, sharedEnvironmentConfigFilePath));
                        _sharedEnvironmentConfigFilePath = sharedEnvironmentConfigFilePath;
                    }

                    if (envChanged)
                    {
                        LoadSharedEnvironmentManager();
                        OnEnvironmentChanged();
                    }
                });

            Environment = ChoAppFrxSettings.Me.AppEnvironment.NTrim();
            _sharedEnvironmentConfigFilePath = SharedEnvironmentConfigFilePath;

            LoadSharedEnvironmentManager();
            _appConfigFileWatcher.StartWatching();
        }

        #endregion Constructors

        #region Shared Members (Public)

        public static void SetEnvironmentChangedEventHandler(object key, EventHandler eventHandler)
        {
            ChoGuard.ArgumentNotNull(key, "key");
            ChoGuard.ArgumentNotNull(eventHandler, "eventHandler");

            OrderedDictionary eventHandlerList = _eventHandlerList;
            if (eventHandlerList != null)
            {
                lock (_eventHandlerListLock)
                {
                    if (eventHandlerList.Contains(key))
                    {
                        //eventHandlerList[key] = eventHandler;
                        return;
                    }
                    else
                        eventHandlerList.Add(key, eventHandler);

                    eventHandler(null, null);
                }
            }
        }

        public static void SetEnvironmentChangedEventHandlerNoCall(object key, EventHandler eventHandler)
        {
            ChoGuard.ArgumentNotNull(key, "key");
            ChoGuard.ArgumentNotNull(eventHandler, "eventHandler");

            OrderedDictionary eventHandlerList = _eventHandlerList;
            if (eventHandlerList != null)
            {
                lock (_eventHandlerListLock)
                {
                    if (eventHandlerList.Contains(key))
                        eventHandlerList[key] = eventHandler;
                    else
                        eventHandlerList.Add(key, eventHandler);
                }
            }
        }

        public static void OnEnvironmentChanged()
        {
            try
            {
                OrderedDictionary eventHandlerList = _eventHandlerList;
                if (eventHandlerList != null)
                {
                    lock (_eventHandlerListLock)
                    {
                        //int counter = 0;
                        //WaitHandle[] handles = new WaitHandle[_eventHandlerList.Count];
                        foreach (EventHandler callback in eventHandlerList.Values)
                        {
                            if (callback != null)
                            {
                                callback(null, null);

                                //handles[counter++] = callback.BeginInvoke(this, eventData, null, null).AsyncWaitHandle;
                            }
                        }
                        //WaitHandle.WaitAll(handles);
                    }
                }
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ChoTrace.Write(ex);
            }
        }

        private static string SharedEnvironmentConfigFilePath
        {
            get
            {
                string sharedEnvironmentConfigFilePath = ChoPath.GetFullPath(ChoAppFrxSettings.Me.SharedEnvironmentConfgiFilePath);
                if (sharedEnvironmentConfigFilePath.IsNullOrWhiteSpace())
                {
                    string defaultSharedEnvironmentConfigFilePath = ChoPath.GetFullPath("SharedEnvironments.config");
                    //if (File.Exists(defaultSharedEnvironmentConfigFilePath))
                        sharedEnvironmentConfigFilePath = defaultSharedEnvironmentConfigFilePath;
                }

                return sharedEnvironmentConfigFilePath;
            }
        }

        private static void LoadSharedEnvironmentManager()
        {
            if (ChoAppFrxSettings.Me.AppFrxFilePath.IsNullOrEmpty())
            {
                if (_sharedEnvironmentManager != null)
                    _sharedEnvironmentManager.Dispose();

                _sharedEnvironmentManager = new ChoSharedEnvironmentManager(_sharedEnvironmentConfigFilePath);
                LoadFrxConfigPath();

                if (_sharedEnvConfigFileWatcher != null)
                    _sharedEnvConfigFileWatcher.Dispose();

                if (!_sharedEnvironmentConfigFilePath.IsNullOrWhiteSpace())
                {
                    _sharedEnvConfigFileWatcher = new ChoConfigurationChangeFileWatcher("SharedEnvConfigFileWatcher", _sharedEnvironmentConfigFilePath);
                    _sharedEnvConfigFileWatcher.DoNotUseGlobalQueue = true;
                    _sharedEnvConfigFileWatcher.SetConfigurationChangedEventHandler("OnSharedEnvironmentFileChanged", (sender, e) =>
                    {
                        if (_sharedEnvironmentManager != null)
                            _sharedEnvironmentManager.Dispose();

                        _sharedEnvironmentManager = new ChoSharedEnvironmentManager(_sharedEnvironmentConfigFilePath);
                        LoadFrxConfigPath();

                        OnEnvironmentChanged();
                    });

                    _sharedEnvConfigFileWatcher.StartWatching();
                }
            }
        }

        public static string GetConfigFilePath()
        {
            if (!_configFilePath.IsNullOrWhiteSpace())
                return _configFilePath;
            else
            {
                string configFilePath;
                string appFrxFilePath = ChoAppFrxSettings.Me.AppFrxFilePath;

                if (appFrxFilePath.IsNullOrWhiteSpace())
                    configFilePath = ChoPath.GetFullPath(Path.Combine(ChoReservedDirectoryName.Config, ChoReservedFileName.CoreFrxConfigFileName));
                else
                {
                    appFrxFilePath = ChoString.ExpandProperties(appFrxFilePath, ChoEnvironmentVariablePropertyReplacer.Instance);

                    if (ChoPath.IsDirectory(appFrxFilePath))
                        appFrxFilePath = Path.Combine(appFrxFilePath, ChoReservedFileName.CoreFrxConfigFileName);

                    configFilePath = appFrxFilePath;
                }
                return configFilePath;
            }
        }

        #endregion Instance Members (Public)

		#region Instance Members (Private)

        private static void LoadFrxConfigPath()
        {
            ChoEnvironmentDetails environmentDetails = null;

            lock (_padLock)
            {
                environmentDetails = _sharedEnvironmentManager.GetEnvironmentDetails();
                if (environmentDetails != null && environmentDetails.Freeze)
                {
                    Environment = environmentDetails.Name;
                    ChoApplication.Trace(true, "This host is Freezed to '{0}' environment.".FormatString(environmentDetails.Name));
                }
                else
                {
                    environmentDetails = _sharedEnvironmentManager.GetEnvironmentDetailsByEnvironment(Environment);
                    if (environmentDetails == null)
                    {
                        if (!Environment.IsNullOrWhiteSpace())
                            ChoApplication.Trace(true, "The '{0}' environment found in the configuration file.".FormatString(Environment));
                        environmentDetails = _sharedEnvironmentManager.GetEnvironmentDetailsByEnvironment();
                    }
                }
            }

            if (environmentDetails != null)
            {
                Environment = environmentDetails.Name;
                ChoApplication.Trace(true, "Using the '{0}' environment.".FormatString(Environment));
                _configFilePath = environmentDetails.AppFrxFilePath;
            }
            else
            {
                ChoApplication.Trace(true, "No Environment found for this host.");
                _configFilePath = null;
            }
        }

        #endregion Instance Members (Private)

        #region Cleanup

        [ChoAppDomainUnloadMethod("Disposing ChoEnvironmentSettings...")]
        public static void Cleanup()
        {
            if (_appConfigFileWatcher != null)
                _appConfigFileWatcher.StopWatching();

            if (_sharedEnvConfigFileWatcher != null)
                _sharedEnvConfigFileWatcher.StopWatching();
        }

        #endregion Cleanup
    }
}
