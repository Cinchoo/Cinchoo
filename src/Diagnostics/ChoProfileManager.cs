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
    internal static class ChoProfileManager
    {
        #region Constants

        private const string PROF_DATA_KEY = "profiles-configurations";
        private const string PROF_DATA_FILE_WATCHER_KEY = "profiles-file-watcher";

        #endregion Constants

        #region Shared Data Members (Private)

        private static XmlNode _rootNode;
        private static string[] _includeFiles;
        private static string _profDataFilepath;
        private static ChoAppConfigurationChangeFileWatcher _profDataChangeWatcher;

        private static readonly object _padLock = new object();
        private static readonly object _pbsDataChangeWatcherLock = new object();
        private static readonly Dictionary<string, string> _profileCache = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _profileStopActionsCache = new Dictionary<string, string>();

        #endregion Shared Data Members (Private)

        #region Constructors

        static ChoProfileManager()
        {
            Refresh();
            ChoEnvironmentSettings.SetEnvironmentChangedEventHandlerNoCall(PROF_DATA_KEY,
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
                    _profDataFilepath = ChoMetaDataFilePathSettings.Me.OverridenPBSDataFilePath;

                ChoXmlDocument.CreateXmlFileIfEmpty(_profDataFilepath);
                _profDataChangeWatcher = new ChoAppConfigurationChangeFileWatcher(PROF_DATA_KEY, _profDataFilepath, _includeFiles);
                _profDataChangeWatcher.SetConfigurationChangedEventHandler(PROF_DATA_FILE_WATCHER_KEY,
                    (sender, e) =>
                    {
                        Refresh();
                    });

                if (!_profDataFilepath.IsNullOrWhiteSpace() && File.Exists(_profDataFilepath))
                {
                    using (ChoXmlDocument xmlDocument = new ChoXmlDocument(_profDataFilepath))
                    {
                        _rootNode = xmlDocument.XmlDocument.DocumentElement;
                        _includeFiles = xmlDocument != null ? xmlDocument.IncludeFiles : null;
                    }
                }
            }
            catch (Exception ex)
            {
                ChoTrace.Error(ex.ToString());
                throw;
            }
        }

        public static IChoProfileBackingStore GetProfile(string name, string startActions, string stopActions)
        {
            IChoProfileBackingStore profileBackingStore = null;
            if (TryGetProfile(name, startActions, stopActions, ref profileBackingStore))
                return profileBackingStore;
            else
                return null;
        }

        public static bool TryGetProfile(string name, string startActions, string stopActions, ref IChoProfileBackingStore profileBackingStore)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

            if (_profileBackingStoreCache.ContainsKey(name))
            {
                profileBackingStore = _profileBackingStoreCache[name];
                return true;
            }

            lock (_padLock)
            {
                if (!_profileBackingStoreCache.ContainsKey(name))
                {
                    try
                    {
                        profileBackingStore = ConstructProfileBackingStore(name, ref startActions, ref stopActions);
                        if (profileBackingStore == null)
                        {
                            if (name != ChoProfile.GLOBAL_PROFILE_NAME)
                            {
                                Trace.TraceInformation("Failed to create '{0}' profile backingstore, using default one.", name);
                            }
                            profileBackingStore = _defaultProfileBackingStore.Value;
                        }

                        if (_profileBackingStoreCache.ContainsKey(name))
                            Trace.TraceError("Duplicate '{0}' profile backingstore found. {1}", name, Environment.NewLine);
                        else
                        {
                            _profileBackingStoreCache.Add(name, profileBackingStore);
                            _profileBackingStopActionsCache.Add(name, stopActions);
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("Failed to create '{0}' profile backingstore, using default one. {2}{1}", name, ex.ToString(), Environment.NewLine);
                        _profileBackingStoreCache.Add(name, _defaultProfileBackingStore.Value);
                    }
                }
                profileBackingStore = _profileBackingStoreCache[name];
            }

            return true;
        }

        private static IChoProfileBackingStore ConstructProfileBackingStore(string name, ref string startActions, ref string stopActions)
        {
            string xpath = @"/profileBackingStores/profileBackingStore[contains(@name,'{0}')]".FormatString(name);
            XmlNode node = null;

            if (_rootNode != null)
                node = _rootNode.SelectSingleNode(xpath);

            if (node != null)
            {
                ChoProfileBackingStore profileBackingStore = node.ToObject(typeof(ChoProfileBackingStore)) as ChoProfileBackingStore;
                return profileBackingStore.Construct(ref startActions, ref stopActions) as IChoProfileBackingStore;
            }
            return null;
        }

        [ChoAppDomainUnloadMethod("Shutting down ProfileBackingStore Manager...")]
        private static void Shutdown()
        {
            if (_profDataChangeWatcher != null)
                _profDataChangeWatcher.Dispose();
            _profDataChangeWatcher = null;

            _profileBackingStoreCache.ForEach((keyValuePair) =>
                {
                    keyValuePair.Value.Stop(_profileBackingStopActionsCache[keyValuePair.Key]);
                });

            _profileBackingStoreCache.Clear();
            _profileBackingStopActionsCache.Clear();

            ChoProfile.Default.Dispose();
        }

        #endregion Shared Members (Private)
    }

    public class ChoProfileBackingStoreDef
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("classType")]
        public string ClassTypeName;

        [XmlElement("parameters")]
        public ChoCDATA Parameters;

        private Type _classType;

        internal object Construct()
        {
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

            if (paramsObj != null)
                return Activator.CreateInstance(_classType, paramsObj);
            else
                return Activator.CreateInstance(_classType);
        }
    }

    public class ChoProfileBackingStore
    {
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
            if (ClassTypeName.IsNullOrWhiteSpace())
                throw new ChoApplicationException("Missing class type name.");

            _classType = ChoType.GetType(ClassTypeName);
            if (_classType == null)
                throw new ChoApplicationException("Can't find {0} class type.".FormatString(ClassTypeName));

            if (StartActions != null && !StartActions.Value.IsNullOrWhiteSpace())
                startActions = ChoString.ExpandPropertiesEx(StartActions.Value);

            if (StopActions != null && !StopActions.Value.IsNullOrWhiteSpace())
                stopActions = ChoString.ExpandPropertiesEx(StopActions.Value);

            string parameters = null;
            if (Parameters != null)
                parameters = ChoString.ExpandPropertiesEx(Parameters.Value);

            object[] paramsObj = new object[] { };
            if (!parameters.IsNullOrWhiteSpace())
                paramsObj = ChoString.Split2Objects(parameters);

            if (paramsObj != null)
                return Activator.CreateInstance(_classType, paramsObj);
            else
                return Activator.CreateInstance(_classType);
        }
    }
}
