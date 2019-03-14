namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Configuration;
	using System.IO;
	using System.Linq;
	using System.Xml;
	using System.Xml.Linq;
	using System.Xml.XPath;
	using Cinchoo.Core.IO;
	using Cinchoo.Core.Services;

	#endregion NameSpaces

	public sealed class ChoStandardAppSettingsConfigStorage : ChoConfigStorage, IChoNameValueConfigStorage
	{
		#region Constants

		private const string APP_SETTINGS_SECTION_NAME = "appSettings";

		#endregion Constants

		#region Instance Data Members (Private)

		private readonly Configuration _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		private string _appSettingsFilePath = null;
		private bool _hasAppSettingsFilePath = false;

		#endregion Instance Data Members (Private)

		public override object Load(ChoBaseConfigurationElement configElement, XmlNode node)
		{
			base.Load(configElement, node);

			ConfigurationManager.RefreshSection(APP_SETTINGS_SECTION_NAME);
			ConfigFilePath = _configuration.FilePath;
			_appSettingsFilePath = ChoPath.GetFullPath(GetAppSettingsFilePathIfAnySpecified());
			if (!_appSettingsFilePath.IsNullOrWhiteSpace())
				_hasAppSettingsFilePath = true;

			ConfigSectionName = APP_SETTINGS_SECTION_NAME;
			return ConfigurationManager.AppSettings;
		}
		
		public override void Persist(object data, ChoDictionaryService<string, object> stateInfo)
		{
			if (!(data is NameValueCollection))
				throw new ChoConfigurationException("Data object is not NameValueCollection object.");
			try
			{
				//Write meta-data info
				ChoConfigurationMetaDataManager.SetMetaDataSection(ConfigElement);
			}
			catch (ChoFatalApplicationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				ConfigElement.Log(ex.ToString());
			}
   
			NameValueCollection nameValueCollection = ((NameValueCollection)data);

			foreach (string key1 in nameValueCollection.Keys)
			{
				if (_configuration.AppSettings.Settings.AllKeys.Contains(key1))
					_configuration.AppSettings.Settings[key1].Value = nameValueCollection[key1];
				else
					_configuration.AppSettings.Settings.Add(new KeyValueConfigurationElement(key1, nameValueCollection[key1]));

			}

			_configuration.Save(ConfigurationSaveMode.Modified);
		}

		public override IChoConfigurationChangeWatcher ConfigurationChangeWatcher
		{
			get
			{
				List<IChoConfigurationChangeWatcher> _watchers = new List<IChoConfigurationChangeWatcher>();
				if (ChoConfigurationManager.SystemConfigurationChangeWatcher != null)
					_watchers.Add(ChoConfigurationManager.SystemConfigurationChangeWatcher);

				if (_hasAppSettingsFilePath)
				{
					IChoConfigurationChangeWatcher configurationChangeWatcher = new ChoConfigurationChangeFileWatcher(ConfigSectionName, _appSettingsFilePath);
					configurationChangeWatcher.SetConfigurationChangedEventHandler("{0}_AppSettings_WatcherHandler".FormatString(ConfigElement.ConfigElementPath),
						(sender, e) =>
						{
							if (e is ChoConfigurationFileChangedEventArgs &&
								(((ChoConfigurationFileChangedEventArgs)e).ConfigurationChangeAction == ChoConfigurationChangeAction.Deleted
								|| ((ChoConfigurationFileChangedEventArgs)e).ConfigurationChangeAction == ChoConfigurationChangeAction.Created))
							{
								ConfigElement.Persist(true, null);
							}
						});

					_watchers.Add(configurationChangeWatcher);
				}

				return new ChoConfigurationChangeCompositeWatcher(ConfigSectionName, _watchers.ToArray());
			}
		}

		private string GetAppSettingsFilePathIfAnySpecified()
		{
			if (!File.Exists(ConfigFilePath))
				return null;

			XDocument xDoc = XDocument.Load(ConfigFilePath);
			var appSettingsFileAtttibute = (from myConfig in xDoc.XPathSelectElements(@"configuration/{0}".FormatString(APP_SETTINGS_SECTION_NAME))
					select myConfig.Attributes("file")).FirstOrDefault();

			return appSettingsFileAtttibute.Count() == 0 ? null : appSettingsFileAtttibute.First().Value;
		}

		public override object PersistableState
		{
			get { return ChoObject.ToPersistableNameValueCollection(ConfigElement.ConfigObject); }
		}

		public override bool IsConfigSectionDefined
		{
			get
			{
				return ConfigurationManager.AppSettings.Count > 0;
			}
		}

		protected override string ToXml(object data)
		{
			throw new NotImplementedException();
		}
	}
}
