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
	[ChoTypeFormatter("Log Listeners")]
	[ChoConfigurationSection("cinchoo/logListenerSettings")]
	[XmlRoot("logListenerSettings")]
	public class ChoLogListenerSettings : IChoObjectInitializable
	{
		#region Instance Data Members (Public)

		[XmlAttribute("turnOn")]
		public bool TurnOn = true;

		[XmlElement("logListener", typeof(ChoObjConfigurable))]
		[ChoIgnoreMemberFormatter]
		public ChoObjConfigurable[] LogListenerTypes = new ChoObjConfigurable[0];

		#endregion

		#region Instance Data Members (Private)

		private ChoDictionary<string, ChoLogListener> _logListeners;

		#endregion

		#region Indexers

		public ChoLogListener this[string name]
		{
			get { return _logListeners != null && _logListeners.ContainsKey(name) ? _logListeners[name] : null; }
		}

		#endregion Indexers

		#region Shared Data Members (Private)

        private static string _buildInLogListenerType = typeof(ChoConsoleLogListener).AssemblyQualifiedName;

		#endregion Shared Data Members (Private)

		#region Instance Properties (Public)

		[XmlIgnore]
		[ChoIgnoreMemberFormatter]
		public ChoLogListener[] LogListeners
		{
			get { return _logListeners.ToValuesArray(); }
		}

		[XmlIgnore]
		[ChoMemberFormatter("Avail Log Listeners", Formatter = typeof(ChoArrayToStringFormatter))]
		internal string[] LogListenerKeys
		{
			get { return _logListeners.ToKeysArray(); }
		}

		#endregion

		#region Shared Properties

		public static ChoLogListenerSettings Me
		{
			get { return ChoConfigurationManagementFactory.CreateInstance<ChoLogListenerSettings>(); }
		}

		private static ChoDictionary<string, ChoLogListener> _defaultLogListeners;
		public static ChoDictionary<string, ChoLogListener> DefaultLogListeners
		{
			get
			{
				if (_defaultLogListeners == null)
				{
					//ChoStreamProfile.Clean(ChoReservedDirectoryName.Settings, ChoPath.AddExtension(typeof(ChoLogListenerSettings).FullName, ChoReservedFileExt.Err));

					_defaultLogListeners = new ChoDictionary<string, ChoLogListener>();
					ChoObjConfigurable.Load<ChoLogListener>(ChoPath.AddExtension(typeof(ChoLogListenerSettings).FullName, ChoReservedFileExt.Log), _buildInLogListenerType, _defaultLogListeners, null);
				}

				return _defaultLogListeners;
			}
		}

		#endregion

		#region IChoObjectInitializable Members

		public bool Initialize(bool beforeFieldInit, object state)
		{
			if (beforeFieldInit)
				//Create the default/built-in objects
				_logListeners = new ChoDictionary<string, ChoLogListener>(DefaultLogListeners);
			else
				ChoObjConfigurable.Load<ChoLogListener>(ChoType.GetLogFileName(typeof(ChoLogListenerSettings)), ChoType.GetTypes(typeof(ChoLogListenerAttribute)),
					_logListeners, LogListenerTypes);

			return false;
		}

		#endregion
	}
}
