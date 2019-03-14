namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Cinchoo.Core.Xml;
    using System.Xml;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.IO;
    using System.Configuration;
    using Cinchoo.Core.IO;
    using Cinchoo.Core.Services;

    #endregion NameSpaces

    [ChoAppDomainEventsRegisterableType]
    public static class ChoCoreFrxConfigurationManager
    {
        private static readonly Dictionary<Type, IChoConfigurationChangeWatcher> _dictService = new Dictionary<Type, IChoConfigurationChangeWatcher>();

        public static T Register<T>()
        {
            using (ChoCoreFrxConfigurationManager<T> coreFrxConfigurationManager = new ChoCoreFrxConfigurationManager<T>())
            {
                T instance = coreFrxConfigurationManager.ConfigObject;

                if (instance is IChoObjectChangeWatcheable)
                {
                    ChoConfigurationChangeFileWatcher fileWatcher = null;
                    fileWatcher = new ChoConfigurationChangeFileWatcher("{0}_FileWatcher".FormatString(typeof(T).Name), coreFrxConfigurationManager.ConfigFilePath);
                    fileWatcher.DoNotUseGlobalQueue = true;
                    fileWatcher.SetConfigurationChangedEventHandler("{0}_FileWatcher".FormatString(typeof(T).Name), (sender1, e1) =>
                    {
                        using (ChoCoreFrxConfigurationManager<T> coreFrxConfigurationManager1 = new ChoCoreFrxConfigurationManager<T>())
                        {
                            T instance1 = coreFrxConfigurationManager1.ConfigObject;
                            if (instance1 is IChoObjectChangeWatcheable)
                            {
                                ((IChoObjectChangeWatcheable)instance1).OnObjectChanged(instance1, null);
                            }
                        }
                    });

                    _dictService.Add(typeof(T), fileWatcher);

                    ChoEnvironmentSettings.SetEnvironmentChangedEventHandlerNoCall(typeof(T).Name, ((sender, e) =>
                    {
                        using (ChoCoreFrxConfigurationManager<T> coreFrxConfigurationManager1 = new ChoCoreFrxConfigurationManager<T>())
                        {
                            T instance1 = coreFrxConfigurationManager1.ConfigObject;
                            if (instance1 is IChoObjectChangeWatcheable)
                            {
                                ((IChoObjectChangeWatcheable)instance1).OnObjectChanged(instance1, null);
                            }
                        }
                    }));

                    if (fileWatcher != null)
                        fileWatcher.StartWatching();
                }

                return instance;
            }
        }

        #region Cleanup

        [ChoAppDomainUnloadMethod("Disposing ChoCoreFrxConfigurationManager...")]
        public static void Cleanup()
        {
            if (_dictService != null)
            {
                foreach (IChoConfigurationChangeWatcher configurationChangeWatcher in _dictService.Values)
                {
                    if (configurationChangeWatcher != null)
                        configurationChangeWatcher.StopWatching();
                }
                _dictService.Clear();
            }
        }

        #endregion Cleanup
    }

    public class ChoCoreFrxConfigurationManager<T> : IDisposable
    {
        #region Constants


        #endregion Constants 

        #region Shared Data Members (Private)

        private readonly object _padLock = new object();
        private XmlDocument _xmlDocument;
        private string _configFilePath;
        private T _configObject;
        private bool _configSectionExists = false;

        #endregion Shared Data Members (Private)

        #region Constructors

        public ChoCoreFrxConfigurationManager()
        {
            if (typeof(T) == typeof(ChoGlobalApplicationSettings) || typeof(T) == typeof(ChoMetaDataFilePathSettings))
                ChoGlobalApplicationSettings.AppFrxConfigFilePath = _configFilePath = ChoPath.GetFullPath(ChoEnvironmentSettings.GetConfigFilePath());
            else
                _configFilePath = Path.Combine(ChoGlobalApplicationSettings.Me.ApplicationConfigDirectory, ChoPath.AddExtension(typeof(T).Name, ChoReservedFileExt.Xml));

            Init();
        }

        #endregion Constructors

        #region Instance Members (Public)

        private T ToObject()
        {
            T obj = default(T);

            try
            {
                if (_xmlDocument != null)
                    obj = _xmlDocument.ToObject<T>();

                _configSectionExists = obj != null;
                if (obj == null)
                    obj = ChoActivator.CreateInstance<T>();
            }
            finally
            {
                if (obj != null)
                {
                    if (obj is IChoInitializable)
                        ((IChoInitializable)obj).Initialize();

                    _xmlDocument.DocumentElement.SaveAsChild(obj);

                    if (!_configSectionExists)
                        _xmlDocument.Save(_configFilePath);

                    //if (typeof(T) != typeof(ChoGlobalApplicationSettings))
                        ChoApplication.Trace(obj.ToString());
                }
            }
            return obj;
        }

        #endregion Instance Members (Public)

        #region Instance Members (Private)

        private void Init()
        {
            try
            {
                if (_configFilePath.IsNullOrWhiteSpace())
                    return;

                //_configFilePath = ChoPath.ChangeExtension(_configFilePath, ChoReservedFileExt.Xml);

                lock (_padLock)
                {
                    try
                    {
                        ChoXmlDocument.CreateXmlFileIfEmpty(_configFilePath);
                        if (File.Exists(_configFilePath))
                        {
                            _xmlDocument = new System.Xml.XmlDocument();
                            _xmlDocument.Load(_configFilePath);
                        }

                        _configObject = ToObject();
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.ToString());
                    }
                }
            }
            finally
            {
                if (_configObject == null)
                    _configObject = ChoActivator.CreateInstance<T>();
            }
        }

        #endregion Instance Members (Private)

        #region Instance Properties (Public)

        public string ConfigFilePath
        {
            get { return _configFilePath; }
        }

        public T ConfigObject
        {
            get { return _configObject; }
        }

        #endregion Instance Properties (Public)

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
