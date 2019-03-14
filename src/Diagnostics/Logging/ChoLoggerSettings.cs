namespace eSquare.Core.Diagnostics.Logging
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections.Generic;

    using eSquare.Core.Types;
    using eSquare.Core.Factory;
    using eSquare.Core.Interfaces;
    using eSquare.Core.Attributes;
    using eSquare.Core.Diagnostics;
    using eSquare.Core.Formatters;
    using eSquare.Core.Exceptions;
    using eSquare.Core.Validation;
    using eSquare.Core.Configuration;
    using eSquare.Core.Collections.Generic;
    using eSquare.Core.Attributes.Validators;
    using eSquare.Core.Diagnostics.Logging.Formatters;
    using eSquare.Core.Diagnostics.Logging.LogListeners;
    using eSquare.Core.Common;

    #endregion NameSpaces

    [ChoObject("ChoLogSource")]
    public class ChoLogSource : ChoObjConfigurable, IChoObjectInitializable
    {
        #region Instance Data Members (Public)

        [ChoNotNullValidator]
        [XmlAttribute("category")]
        public string Category;

        [ChoNotNullValidator]
        [XmlAttribute("formatter")]
        public string FormatterName;

        [ChoNotNullValidator]
        [XmlAttribute("logListeners")]
        public string CSVLogListenerNames;

        #endregion Instance Data Members (Public)

        #region Constructors

        public ChoLogSource()
        {
        }

        [ChoObjectConstructor("Default, ChoTextFormatter, ChoConsoleLogListener")]
        public ChoLogSource(string category, string formatterName, string logListenerNames)
        {
            Category = category;
            FormatterName = formatterName;
            CSVLogListenerNames = logListenerNames;
        }

        #endregion Constructors

        #region Instance Data Members (Private)

        private ChoLogListener[] _logListeners;
        [ChoNotNullValidator]
        [ChoMemberFormatter(ChoNull.NullString, FormatterType = typeof(ChoArrayToStringFormatter))]
        public ChoLogListener[] LogListeners
        {
            get { return _logListeners; }
        }

        #endregion Instance Data Members (Private)

        #region Instance Members (Public)

        public void Initialize(bool beforeInit)
        {
            if (_logListeners != null) return;

            if (!String.IsNullOrEmpty(FormatterName) && CSVLogListenerNames != null && CSVLogListenerNames.Length > 0)
            {
                IChoLogFormatter logFormatter = ChoLogFormatterSettings.Me[FormatterName];

                List<ChoLogListener> logListeners = new List<ChoLogListener>();
                ChoLogListener logListener = null;
                if (logFormatter != null)
                {
                    foreach (string logListenerName in ChoString.SplitNTrim(CSVLogListenerNames))
                    {
                        logListener = ChoLogListenerSettings.Me[logListenerName];
                        if (logListener != null)
                        {
                            logListener.Formatter = logFormatter;
                            logListeners.Add(logListener);
                        }
                    }
                }
                _logListeners = logListeners.ToArray();
            }
        }

        #endregion Instance Members (Public)
    }

    [Serializable]
    [ChoTypeFormatter("Loggers")]
    [ChoNameValueConfigurationElementMap("eSquare/loggerSettings", WatchChange = true, IgnoreError = true, Defaultable = true)]
    [XmlRoot("loggerSettings")]
    public class ChoLoggerSettings : IChoObjectInitializable
    {
        #region Instance Data Members (Public)

        [XmlAttribute("turnOn")]
        public bool TurnOn = true;

        [XmlElement("logger", typeof(ChoLogSource))]
        [ChoNotNullValidator]
        [ChoMemberFormatterIgnore]
        public ChoLogSource[] LoggerTypes = new ChoLogSource[0];

        #endregion

        #region Instance Data Members (Private)

        private ChoDictionary<string, ChoLogListener[]> _logSources;

        #endregion

        #region Instance Properties (Public)

        [XmlIgnore]
        [ChoMemberFormatterIgnore]
        public ChoDictionary<string, ChoLogListener[]> LogSources
        {
            get { return _logSources; }
        }

        [XmlIgnore]
        [ChoMemberFormatter("Avail Log Sources", FormatterType = typeof(ChoArrayToStringFormatter))]
        internal string[] LogSourcesKeys
        {
            get { return _logSources.ToKeysArray(); }
        }

        #endregion

        #region Shared Properties

        public static ChoLoggerSettings Me
        {
            get { return ChoConfigurationManagementFactory.CreateInstance(typeof(ChoLoggerSettings)) as ChoLoggerSettings; }
        }

        private static ChoDictionary<string, ChoLogListener[]> _defaultLogSources;
        public static ChoDictionary<string, ChoLogListener[]> DefaultLogSources
        {
            get
            {
                if (_defaultLogSources == null)
                {
                    ChoStreamProfile.Clean(ChoLogDirectories.Settings, Path.ChangeExtension(typeof(ChoLoggerSettings).Name, ChoExt.Err));

                    _defaultLogSources = ChoDictionary<string, ChoLogListener[]>.Unique(new ChoDictionary<string, ChoLogListener[]>());
                    try
                    {
                        ChoLogSource loggerType = ChoObjectManagementFactory.CreateInstance(typeof(ChoLogSource)) as ChoLogSource;
                        _defaultLogSources.Add(loggerType.Category, loggerType.LogListeners);
                    }
                    catch (Exception ex)
                    {
                        ChoStreamProfile.WriteLine(ChoLogDirectories.Settings, Path.ChangeExtension(typeof(ChoLoggerSettings).Name, ChoExt.Err),
                            String.Format("Failed to create {0} object. {1}", typeof(ChoLogSource).FullName, ex.Message));
                    }
                }

                return _defaultLogSources;
            }
        }

        #endregion

        #region IChoObjectInitializable Members

        public void Initialize(bool beforeInit)
        {
            _logSources = ChoDictionary<string, ChoLogListener[]>.Unique(new ChoDictionary<string, ChoLogListener[]>(DefaultLogSources));
            if (!beforeInit)
            {
                if (LoggerTypes == null) return;

                foreach (ChoLogSource loggerType in LoggerTypes)
                {
                    try
                    {
                        ChoValidation.Validate(loggerType);
                        _logSources.Add(loggerType.Category, loggerType.LogListeners);
                    }
                    catch (Exception ex)
                    {
                        ChoStreamProfile.WriteLine(ChoLogDirectories.Settings, Path.ChangeExtension(typeof(ChoLoggerSettings).Name, ChoExt.Err),
                            String.Format("Failed to initialize '{0}' object. {1}", loggerType.Category, ex.Message));
                    }
                }
            }
        }

        #endregion IChoObjectInitializable Members

        #region Instance Members (Public)

        public IEnumerable<ChoLogListener[]> Find(string category)
        {
            ChoGuard.ArgumentNotNull(category, "Category");

            foreach (string key in _logSources.Keys)
            {
                if (key == category && _logSources.ContainsKey(key))
                    yield return _logSources[key];
            }
        }

        public ChoDictionary<string, ChoLogListener[]> Find(ICollection<string> categories)
        {
            ChoGuard.ArgumentNotNull(categories, "Categories");

            ChoDictionary<string, ChoLogListener[]> logListeners = new ChoDictionary<string, ChoLogListener[]>();
            foreach (string category in categories)
            {
                foreach (string key in _logSources.Keys)
                {
                    if (key == category && _logSources.ContainsKey(key))
                        logListeners.Add(key, _logSources[key]);
                }
            }

            return logListeners;
        }

        #endregion Instance Members (Public)
    }
}
