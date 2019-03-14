namespace Cinchoo.Core.Logging
{
	#region NameSpaces

    using System;
    using System.Xml.Serialization;
    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Common;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.IO;

	#endregion NameSpaces

	[Serializable]
	[ChoTypeFormatter("Log Formatters")]
	[ChoConfigurationSection("cinchoo/logFormatterSettings")]
	[XmlRoot("logFormatterSettings")]
	public class ChoLogFormatterSettings : IChoObjectInitializable
	{
		#region Instance Data Members (Public)

		[XmlAttribute("turnOn")]
		public bool TurnOn = true;

		[XmlElement("logFormatter", typeof(ChoObjConfigurable))]
		[ChoIgnoreMemberFormatter]
		public ChoObjConfigurable[] LogFormatterTypes = new ChoObjConfigurable[0];

		#endregion

		#region Instance Data Members (Private)

		private ChoDictionary<string, IChoLogFormatter> _logFormatters;

		#endregion

		#region Shared Data Members (Private)

        private static string _buildInLogFormatterType = typeof(ChoTextLogFormatter).AssemblyQualifiedName;

		#endregion Shared Data Members (Private)

		#region Instance Properties (Public)

		[XmlIgnore]
		[ChoIgnoreMemberFormatter]
		public IChoLogFormatter[] LogFormatters
		{
			get { return _logFormatters.ToValuesArray(); }
		}

		[XmlIgnore]
		[ChoMemberFormatter("Avail Log Formatters", Formatter = typeof(ChoArrayToStringFormatter))]
		internal string[] LogFormatterKeys
		{
			get { return _logFormatters.ToKeysArray(); }
		}

		#endregion

		#region Indexers

		public IChoLogFormatter this[string name]
		{
			get { return _logFormatters != null && _logFormatters.ContainsKey(name) ? _logFormatters[name] : null; }
		}

		#endregion Indexers

		#region Shared Properties

		public static ChoLogFormatterSettings Me
		{
			get { return ChoConfigurationManagementFactory.CreateInstance<ChoLogFormatterSettings>(); }
		}

		private static ChoDictionary<string, IChoLogFormatter> _defaultLogFormatters;
		public static ChoDictionary<string, IChoLogFormatter> DefaultLogFormatters
		{
			get
			{
				if (_defaultLogFormatters == null)
				{
					//ChoStreamProfile.Clean(ChoReservedDirectoryName.Settings, ChoPath.AddExtension(typeof(ChoLogFormatterSettings).FullName, ChoReservedFileExt.Err));

					_defaultLogFormatters = new ChoDictionary<string, IChoLogFormatter>();
					ChoObjConfigurable.Load<IChoLogFormatter>(ChoPath.AddExtension(typeof(ChoLogFormatterSettings).FullName, ChoReservedFileExt.Log), _buildInLogFormatterType, _defaultLogFormatters, null);
				}

				return _defaultLogFormatters;
			}
		}

		#endregion

		#region IChoObjectInitializable Members

		public bool Initialize(bool beforeFieldInit, object state)
		{
			//Create the default/built-in objects
			_logFormatters = new ChoDictionary<string, IChoLogFormatter>(DefaultLogFormatters);

			ChoObjConfigurable.Load<IChoLogFormatter>(ChoType.GetLogFileName(typeof(ChoLogFormatterSettings)), ChoType.GetTypes(typeof(ChoLogFormatterAttribute)),
				_logFormatters, LogFormatterTypes);

			return false;
		}

		#endregion
	}
}
