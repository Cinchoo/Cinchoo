namespace Cinchoo.Core.Logging
{
	#region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Common;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.Factory;

    #endregion NameSpaces

    [ChoObject("ChoLogSource")]
	public class ChoLogSource : ChoObjConfigurable, IChoObjectInitializable
	{
		public readonly static ChoLogSource DefaultLogSource = new ChoLogSource("Default", "ChoTextFormatter", "ChoConsoleLogListener");

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
		[ChoMemberFormatter(ChoNull.NullString, Formatter = typeof(ChoArrayToStringFormatter))]
		public ChoLogListener[] LogListeners
		{
			get { return _logListeners; }
		}

		#endregion Instance Data Members (Private)

		#region Instance Members (Public)

		public bool Initialize(bool beforeFieldInit, object state)
		{
			if (_logListeners != null) return false;

			if (!String.IsNullOrEmpty(FormatterName) && CSVLogListenerNames != null && CSVLogListenerNames.Length > 0)
			{
				IChoLogFormatter logFormatter = ChoLogFormatterSettings.Me[FormatterName];

				List<ChoLogListener> logListeners = new List<ChoLogListener>();
				ChoLogListener logListener = null;
				if (logFormatter != null)
				{
					foreach (string logListenerName in CSVLogListenerNames.SplitNTrim())
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

			return false;
		}

		#endregion Instance Members (Public)
	}

	[Serializable]
	[ChoTypeFormatter("Loggers")]
	[ChoConfigurationSection("cinchoo/loggerSettings", Defaultable = false)]
	[XmlRoot("loggerSettings")]
	public class ChoLoggerSettings : IChoObjectInitializable
	{
        private static readonly ChoLoggerSettings _instance = new ChoLoggerSettings();

		#region Instance Data Members (Public)

		[XmlAttribute("turnOn")]
		public bool TurnOn = true;

		[XmlElement("logger", typeof(ChoLogSource))]
		[ChoNotNullValidator]
		[ChoIgnoreMemberFormatter]
        [ChoPropertyInfo(Persistable=false)]
		public ChoLogSource[] LoggerTypes = new ChoLogSource[0];

		#endregion

		#region Instance Data Members (Private)

		private ChoDictionary<string, ChoLogListener[]> _logSources;

		#endregion

		#region Instance Properties (Public)

		[XmlIgnore]
		[ChoIgnoreMemberFormatter]
        [ChoPropertyInfo(Persistable = false)]
        public ChoDictionary<string, ChoLogListener[]> LogSources
		{
			get { return _logSources; }
		}

		[XmlIgnore]
		[ChoMemberFormatter("Avail Log Sources", Formatter = typeof(ChoArrayToStringFormatter))]
        [ChoPropertyInfo(Persistable = false)]
        internal string[] LogSourcesKeys
		{
			get { return _logSources.ToKeysArray(); }
		}

		#endregion

		#region Shared Properties

		public static ChoLoggerSettings Me
		{
            get { return _instance; }
		}

		private static ChoDictionary<string, ChoLogListener[]> _defaultLogSources;
		public static ChoDictionary<string, ChoLogListener[]> DefaultLogSources
		{
			get
			{
				if (_defaultLogSources == null)
				{
					//ChoStreamProfile.Clean(ChoReservedDirectoryName.Settings, ChoPath.AddExtension(typeof(ChoLoggerSettings).FullName, ChoReservedFileExt.Err));

					_defaultLogSources = ChoDictionary<string, ChoLogListener[]>.Unique(new ChoDictionary<string, ChoLogListener[]>());
					try
					{
						ChoLogSource loggerType = ChoLogSource.DefaultLogSource; // ChoObjectManagementFactory.CreateInstance(typeof(ChoLogSource)) as ChoLogSource;
						_defaultLogSources.Add(loggerType.Category, loggerType.LogListeners);
					}
					catch (Exception)
					{
						//ChoStreamProfile.WriteLine(ChoReservedDirectoryName.Settings, ChoPath.AddExtension(typeof(ChoLoggerSettings).FullName, ChoReservedFileExt.Err),
						//    String.Format("Failed to create {0} object. {1}", typeof(ChoLogSource).FullName, ex.Message));
					}
				}

				return _defaultLogSources;
			}
		}

		#endregion

		#region IChoObjectInitializable Members

		public bool Initialize(bool beforeFieldInit, object state)
		{
			_logSources = ChoDictionary<string, ChoLogListener[]>.Unique(new ChoDictionary<string, ChoLogListener[]>(DefaultLogSources));
			if (!beforeFieldInit)
			{
				if (LoggerTypes == null)
					return false;

				foreach (ChoLogSource loggerType in LoggerTypes)
				{
					try
					{
						ChoValidation.Validate(loggerType);
						_logSources.Add(loggerType.Category, loggerType.LogListeners);
					}
					catch (Exception)
					{
						//ChoStreamProfile.WriteLine(ChoReservedDirectoryName.Settings, ChoPath.AddExtension(typeof(ChoLoggerSettings).FullName, ChoReservedFileExt.Err),
						//    String.Format("Failed to initialize '{0}' object. {1}", loggerType.Category, ex.Message));
					}
				}
			}

			return false;
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
