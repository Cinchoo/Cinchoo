namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
using Cinchoo.Core.Collections.Generic.Dictionary;
using System.Xml;
using Cinchoo.Core.IO;
	using Cinchoo.Core.Xml;
	using System.IO;

	#endregion NameSpaces

	public delegate void ChoConfigurationMetaDataChangeEventHandler(XmlNode node);

	internal class ChoConfigurationMetaDataState : ChoSyncDisposableObject
	{
		#region Instance Data Members (Public)

		public string ConfigSectionName;
		public string MetaDataFileName;
		public event ChoConfigurationMetaDataChangeEventHandler ConfigurationMetaDataChangedEvent;
		public ChoFileWatcher FileWatcher;
		
		#endregion Instance Data Members (Public)

		#region Instance Methods (Public)

		public void OnConfigurationMetaDataChanged()
		{
			ChoConfigurationMetaDataChangeEventHandler configurationMetaDataChangeEvent = ConfigurationMetaDataChangedEvent;
			if (configurationMetaDataChangeEvent != null)
			{
				using (ChoXmlDocument document = new ChoXmlDocument(MetaDataFileName, false, true))
				{
					configurationMetaDataChangeEvent(document.XmlDocument.SelectSingleNode(@"//{0}".FormatString(ConfigSectionName)));
				}
			}
		}

		#endregion Instance Methods (Public)

		#region IDisposable Members

		protected override void Dispose(bool finalize)
		{
			if (FileWatcher != null)
				FileWatcher.Dispose();
		}

		#endregion IDisposable Members
	}

	internal static class ChoConfigurationMetaDataManager
	{
		#region Shared Data Members (Private)

		private readonly static ChoDictionary<string, ChoConfigurationMetaDataState> _configMetaData = ChoDictionary<string, ChoConfigurationMetaDataState>.Synchronized(new ChoDictionary<string, ChoConfigurationMetaDataState>());

		#endregion Shared Data Members (Private)

		#region Shared Members (Public)

		public static ChoConfigurationMetaDataState Register(string configSectionName, string metaDataFileName)
		{
			ChoGuard.ArgumentNotNullOrEmpty(configSectionName, "ConfigSectionName");
			ChoGuard.ArgumentNotNullOrEmpty(metaDataFileName, "MetaDataFileName");

			lock (_configMetaData.SyncRoot)
			{
				if (_configMetaData.ContainsKey(configSectionName))
					_configMetaData.Remove(configSectionName);

				ChoConfigurationMetaDataState configurationMetaDataState = new ChoConfigurationMetaDataState();
				_configMetaData.Add(configSectionName, configurationMetaDataState);

				configurationMetaDataState.ConfigSectionName = configSectionName;
				configurationMetaDataState.MetaDataFileName = metaDataFileName;
				configurationMetaDataState.FileWatcher = new ChoFileWatcher(metaDataFileName);
				configurationMetaDataState.FileWatcher.FileChanged += (target, e) => configurationMetaDataState.OnConfigurationMetaDataChanged();

				configurationMetaDataState.FileWatcher.StartWatching();

				return configurationMetaDataState;
			}
		}

		public static void Unregister(ChoConfigurationMetaDataState configurationMetaDataState)
		{
			ChoGuard.ArgumentNotNull(configurationMetaDataState, "ConfigurationMetaDataState");
			Unregister(configurationMetaDataState.ConfigSectionName);
		}

		public static void Unregister(string configSectionName)
		{
			lock (_configMetaData.SyncRoot)
			{
				if (_configMetaData.ContainsKey(configSectionName))
				{
					_configMetaData[configSectionName].Dispose();
					_configMetaData.Remove(configSectionName);
				}
			}
		}

		#endregion Shared Members (Public)

	}
}
