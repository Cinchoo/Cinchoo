namespace Cinchoo.Core.Diagnostics
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
    using System.Diagnostics;
    using Cinchoo.Core.Xml.Serialization;

    #endregion NameSpaces

    [ChoAppDomainEventsRegisterableType]
    internal static class ChoProfileBackingStoreManager
    {
        #region Constants

        private const string PBS_DATA_KEY = "profile-backing-store-configurations";
        private const string PBS_DATA_FILE_WATCHER_KEY = "pbs-file-watcher";

        #endregion Constants

        #region Shared Data Members (Private)

        private static XmlNode _rootNode;
        private static string[] _includeFiles;
        private static string _pbsDataFilepath;
        private static ChoAppConfigurationChangeFileWatcher _pbsDataChangeWatcher;

        private static readonly object _pbsStoreLock = new object();
        private static readonly Dictionary<string, ChoProfileBackingStoreDef> _pbsStoreDef = new Dictionary<string, ChoProfileBackingStoreDef>();
        private static readonly Dictionary<string, IChoProfileBackingStore> _pbsStore = new Dictionary<string, IChoProfileBackingStore>();
        private static readonly object _padLock = new object();
        private static readonly object _pbsDataChangeWatcherLock = new object();
        private static readonly Dictionary<string, IChoProfileBackingStore> _profilerCache = new Dictionary<string, IChoProfileBackingStore>();
        private static readonly Dictionary<string, string> _profileBackingStopActionsCache = new Dictionary<string, string>();
        private static readonly Lazy<IChoProfileBackingStore> _defaultProfileBackingStore = new Lazy<IChoProfileBackingStore>(() =>
        {
            IChoProfileBackingStore defaultProfileBackingStore = new ChoCompositeProfileBackingStore(new IChoProfileBackingStore[] { new ChoConsoleProfileBackingStore(), new ChoTraceProfileBackingStore() });
            defaultProfileBackingStore.Start(null);

            return defaultProfileBackingStore;
        });

        #endregion Shared Data Members (Private)

        #region Constructors

        static ChoProfileBackingStoreManager()
        {
            Refresh();
            ChoEnvironmentSettings.SetEnvironmentChangedEventHandlerNoCall(PBS_DATA_KEY,
                (sender, e) =>
                {
                    Refresh();
                });
        }

        #endregion Constructors

        #region Shared Members (Private)

        private static void Refresh()
        {
            try
            {
                lock (_padLock)
                {
                    //_propDict.Clear();
                }

                if (ChoMetaDataFilePathSettings.Me != null)
                    _pbsDataFilepath = ChoMetaDataFilePathSettings.Me.OverridenPBSDataFilePath;

                if (!ChoAppFrxSettings.Me.DisableMetaDataConfig)
                {
                    ChoXmlDocument.CreateXmlFileIfEmpty(_pbsDataFilepath);
                    _pbsDataChangeWatcher = new ChoAppConfigurationChangeFileWatcher(PBS_DATA_KEY, _pbsDataFilepath, _includeFiles);
                    _pbsDataChangeWatcher.SetConfigurationChangedEventHandler(PBS_DATA_FILE_WATCHER_KEY,
                        (sender, e) =>
                        {
                            Refresh();
                        });
                }

                if (!_pbsDataFilepath.IsNullOrWhiteSpace() && File.Exists(_pbsDataFilepath))
                {
                    using (ChoXmlDocument xmlDocument = new ChoXmlDocument(_pbsDataFilepath))
                    {
                        _rootNode = xmlDocument.XmlDocument.DocumentElement;
                        _includeFiles = xmlDocument != null ? xmlDocument.IncludeFiles : null;
                    }
                }

                BuildProfileBackingStores();
            }
            catch (Exception ex)
            {
                ChoTrace.Error(ex.ToString());
                //throw;
            }
        }

        private static void BuildProfileBackingStores()
        {
            string xpath = @"//configuration/loggers/profileBackingStores/profileBackingStore";

            if (_rootNode == null) return;

            lock (_pbsStoreLock)
            {
                _pbsStoreDef.Clear();
                foreach (XmlNode pbsNode in _rootNode.SelectNodes(xpath))
                {
                    if (pbsNode == null) continue;
                    try
                    {
                        ChoProfileBackingStoreDef profileBackingStoreDef;
                        profileBackingStoreDef = pbsNode.ToObject(typeof(ChoProfileBackingStoreDef)) as ChoProfileBackingStoreDef;

                        if (!_pbsStoreDef.ContainsKey(profileBackingStoreDef.Name))
                        {
                            _pbsStoreDef.Add(profileBackingStoreDef.Name, profileBackingStoreDef);
                        }
                        else
                            throw new ApplicationException("{0}: Duplicate profile backing store found.".FormatString(profileBackingStoreDef.Name));
                    }
                    catch (Exception ex)
                    {
                        ChoTrace.Error(ex);
                    }
                }
            }
        }

        private static IChoProfileBackingStore ConstructProfileBackingStore(ChoProfileBackingStoreDef profileBackingStoreDef, out string startActions, out string stopActions)
        {
            if (profileBackingStoreDef.Name.IsNullOrWhiteSpace())
                throw new ApplicationException("{0}: Node does not contain profile backing store object.".FormatString(profileBackingStoreDef.Name));

            startActions = null;
            stopActions = null;
            IChoProfileBackingStore pbsObject = profileBackingStoreDef.Construct(ref startActions, ref stopActions) as IChoProfileBackingStore;
            if (pbsObject == null)
                throw new ApplicationException("{0}: Node does not contain profile backing store object.".FormatString(profileBackingStoreDef.Name));

            return pbsObject;
        }

        public static IChoProfileBackingStore GetProfileBackingStore(string name, string startActions, string stopActions)
        {
            IChoProfileBackingStore profileBackingStore = null;
            if (TryGetProfileBackingStore(name, startActions, stopActions, ref profileBackingStore))
                return profileBackingStore;
            else
                return null;
        }

        public static bool TryGetProfileBackingStore(string name, string startActions, string stopActions, ref IChoProfileBackingStore profileBackingStore)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

            if (_profilerCache.ContainsKey(name))
            {
                profileBackingStore = _profilerCache[name];
                return true;
            }

            lock (_padLock)
            {
                if (!_profilerCache.ContainsKey(name))
                {
                    try
                    {
                        profileBackingStore = ConstructProfiler(name);
                        if (profileBackingStore == null)
                        {
                            if (name == ChoProfile.GLOBAL_PROFILE_NAME || name == ChoProfile.NULL_PROFILE_NAME || name == ChoProfile.DEFAULT_PROFILE_NAME)
                            {
                            }
                            else
                                Trace.TraceInformation("Failed to find profile backingstore for '{0}' logger, using default one.", name);
                            profileBackingStore = _defaultProfileBackingStore.Value;
                        }

                        if (_profilerCache.ContainsKey(name))
                            Trace.TraceError("Duplicate '{0}' logger found. {1}", name, Environment.NewLine);
                        else
                            _profilerCache.Add(name, profileBackingStore);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("Failed to create profile backingstore for '{0}' logger, using default one. {2}{1}", name, ex.ToString(), Environment.NewLine);
                        _profilerCache.Add(name, _defaultProfileBackingStore.Value);
                    }
                }
                profileBackingStore = _profilerCache[name];
            }

            return true;
        }

        private static IChoProfileBackingStore ConstructProfiler(string name)
        {
            if (_pbsStore.ContainsKey(name)) return _pbsStore[name];

            string xpath = @"//configuration/loggers/logger[contains(@name,'{0}')]".FormatString(name);
            XmlNode node = null;

            if (_rootNode != null)
                node = _rootNode.SelectSingleNode(xpath);

            lock (_pbsStoreLock)
            {
                if (_pbsStore.ContainsKey(name)) return _pbsStore[name];
                if (node != null)
                {
                    ChoProfiler profiler = node.ToObject(typeof(ChoProfiler)) as ChoProfiler;
                    if (!profiler.ProfileBackingStoreName.IsNullOrWhiteSpace())
                    {
                        if (!_pbsStoreDef.ContainsKey(name)) return null;

                        ChoProfileBackingStoreDef pbsStoreDef = _pbsStoreDef[name];

                        string startActions;
                        string stopActions;
                        IChoProfileBackingStore pbsObject;

                        pbsObject = ConstructProfileBackingStore(pbsStoreDef, out startActions, out stopActions);
                        pbsObject.Start(startActions);
                        if (!_profileBackingStopActionsCache.ContainsKey(name))
                            _profileBackingStopActionsCache.Add(name, stopActions);

                        if (!_pbsStore.ContainsKey(name))
                            _pbsStore.Add(name, pbsObject);

                        return pbsObject;
                    }
                }
            }
            return null;
        }

        [ChoAppDomainUnloadMethod("Shutting down ProfileBackingStore Manager...")]
        private static void Shutdown()
        {
            ChoProfile.DisposeAll();
            if (_pbsDataChangeWatcher != null)
                _pbsDataChangeWatcher.Dispose();
            _pbsDataChangeWatcher = null;

            ChoProfile.Default.Dispose();
            _pbsStore.ForEach((keyValuePair) =>
                {
                    if (_profileBackingStopActionsCache.ContainsKey(keyValuePair.Key))
                        keyValuePair.Value.Stop(_profileBackingStopActionsCache[keyValuePair.Key]);
                });

            //_profilerCache.Clear();

            _profileBackingStopActionsCache.Clear();
        }

        #endregion Shared Members (Private)
    }

    [XmlRoot("profileBackingStore")]
    public class ChoProfileBackingStoreDef
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("classType")]
        public string ClassTypeName;

        [XmlElement("parameters")]
        public ChoCDATA Parameters;

        [XmlElement("startActions")]
        public ChoCDATA StartActions;

        [XmlElement("stopActions")]
        public ChoCDATA StopActions;

        private Type _classType;

        internal object Construct(ref string startActions, ref string stopActions)
        {
            if (Name.IsNullOrWhiteSpace())
                throw new ChoApplicationException("Missing Name.");

            if (ClassTypeName.IsNullOrWhiteSpace())
                throw new ChoApplicationException("Missing class type name.");

            _classType = ChoType.GetType(ClassTypeName);
            if (_classType == null)
                throw new ChoApplicationException("Can't find {0} class type.".FormatString(ClassTypeName));

            string parameters = null;
            if (Parameters != null)
                parameters = ChoString.ExpandPropertiesEx(Parameters.Value);

            object[] paramsObj = new object[] { };
            if (!parameters.IsNullOrWhiteSpace())
                paramsObj = ChoString.Split2Objects(parameters);

            if (StartActions != null && !StartActions.Value.IsNullOrWhiteSpace())
                startActions = ChoString.ExpandPropertiesEx(StartActions.Value);

            if (StopActions != null && !StopActions.Value.IsNullOrWhiteSpace())
                stopActions = ChoString.ExpandPropertiesEx(StopActions.Value);

            if (paramsObj != null)
                return Activator.CreateInstance(_classType, paramsObj);
            else
                return Activator.CreateInstance(_classType);
        }
    }

    [XmlRoot("logger")]
    public class ChoProfiler
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("traceLevel")]
        public TraceLevel TraceLevel = TraceLevel.Verbose;

        [XmlAttribute("profileBackingStoreName")]
        public string ProfileBackingStoreName;

        internal void Validate()
        {
            if (Name.IsNullOrWhiteSpace())
                throw new ChoApplicationException("Missing Name.");

            if (ProfileBackingStoreName.IsNullOrWhiteSpace())
                throw new ChoApplicationException("Missing ProfileBackingStoreName.");
        }
    }
}
