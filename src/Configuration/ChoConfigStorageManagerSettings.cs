namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

    using System;
    using System.Xml.Serialization;
    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Common;
    using Cinchoo.Core.IO;

	#endregion NameSpaces

	[Serializable]
	[ChoTypeFormatter("Config Sources")]
	[ChoConfigurationSection("cinchoo/configStorageSettings")]
	[XmlRoot("configStorageSettings")]
	public class ChoConfigStorageManagerSettings : IChoObjectInitializable
	{
		#region Instance Data Members (Public)

		[XmlElement("configStorage", typeof(ChoObjConfigurable))]
		[ChoIgnoreMemberFormatter]
		public ChoObjConfigurable[] ConfigStorageTypes = new ChoObjConfigurable[0];

		#endregion

		#region Instance Data Members (Private)

		private object _padLock = new object();
		private ChoDictionary<string, IChoConfigStorage> _configStorages;

		#endregion

		#region Shared Data Members (Private)

        private static string _buildInConfigStorageType = typeof(ChoStandardAppSettingsConfigStorage).AssemblyQualifiedName;

		#endregion Shared Data Members (Private)

		#region Instance Properties (Public)

		[XmlIgnore]
		[ChoIgnoreMemberFormatter]
		public IChoConfigStorage[] ConfigStorages
		{
			get { return _configStorages.ToValuesArray(); }
		}

		[XmlIgnore]
		[ChoMemberFormatter("Avail Config Storages", Formatter = typeof(ChoArrayToStringFormatter))]
		internal string[] ConfigStorageKeys
		{
			get { return _configStorages.ToKeysArray(); }
		}

		#endregion

		#region Shared Properties

		public static ChoConfigStorageManagerSettings Me
		{
			get { return ChoConfigurationManagementFactory.CreateInstance<ChoConfigStorageManagerSettings>(); }
		}

		private static ChoDictionary<string, IChoConfigStorage> _defaultConfigStorages;
		public static ChoDictionary<string, IChoConfigStorage> DefaultConfigStorages
		{
			get
			{
				if (_defaultConfigStorages == null)
				{
					//ChoStreamProfile.Clean(ChoReservedDirectoryName.Settings, ChoPath.AddExtension(typeof(ChoConfigStorageManagerSettings).FullName, ChoReservedFileExt.Err));

					_defaultConfigStorages = new ChoDictionary<string, IChoConfigStorage>();
					ChoObjConfigurable.Load<IChoConfigStorage>(ChoPath.AddExtension(typeof(ChoConfigStorageManagerSettings).FullName, ChoReservedFileExt.Log), _buildInConfigStorageType, _defaultConfigStorages, null);
				}

				return _defaultConfigStorages;
			}
		}

		#endregion

		#region IChoObjectInitializable Members

		public bool Initialize(bool beforeFieldInit, object state)
		{
			lock (_padLock)
			{
				_configStorages = new ChoDictionary<string, IChoConfigStorage>(DefaultConfigStorages);
                //if (!beforeFieldInit)
                //    ChoObjConfigurable.Load<IChoConfigStorage>(ChoType.GetLogFileName(typeof(ChoConfigStorageManagerSettings)), ChoType.GetTypes(typeof(ChoConfigStorageAttribute)),
                //        _configStorages, ConfigStorageTypes);

				return false;
			}
		}

		#endregion

		#region Instance Members (Public)

		public IChoConfigStorage GetConfigStorage(string configStorageName)
		{
			ChoGuard.ArgumentNotNullOrEmpty(configStorageName, "Config Storage Name");

			lock (_padLock)
			{
				if (_configStorages != null && _configStorages.ContainsKey(configStorageName))
				{
					return _configStorages[configStorageName];
				}
			}

			return null;
		}

		public IChoConfigStorage GetDefaultConfigStorage()
		{
			if (_defaultConfigStorages != null && _defaultConfigStorages.Count > 0)
			{
				foreach (string keys in _defaultConfigStorages.Keys)
					return _defaultConfigStorages[keys];
			}

			return null;
		}

		#endregion Instance Members (Public)
	}
}
