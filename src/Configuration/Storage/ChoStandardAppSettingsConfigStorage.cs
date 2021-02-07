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
    using System.Text;

	#endregion NameSpaces

	public class ChoStandardAppSettingsConfigStorage : ChoConfigStorage, IChoNameValueConfigStorage
	{
		#region Constants

		private const string APP_SETTINGS_SECTION_NAME = "appSettings";

		#endregion Constants

		#region Instance Data Members (Private)

		#endregion Instance Data Members (Private)

		public override object Load(ChoBaseConfigurationElement configElement, XmlNode node)
		{
			base.Load(configElement, node);

            ConfigurationManager.RefreshSection(APP_SETTINGS_SECTION_NAME);
            ChoConfigurationManager.Refresh();
            ConfigFilePath = ChoConfigurationManager.ApplicationConfigurationFilePath;
			string appSettingsFilePath = GetAppSettingsFilePathIfAnySpecified();
            configElement[ChoConfigurationConstants.FORCE_PERSIST] = CanForcePersist(appSettingsFilePath);

			ConfigSectionName = APP_SETTINGS_SECTION_NAME;
            RefreshSection(appSettingsFilePath);
            return ChoConfigurationManager.ApplicationConfiguration.AppSettings.Settings.ToNameValueCollection();
		}

        private bool CanForcePersist(string appSettingsFilePath)
        {
            if (appSettingsFilePath.IsNullOrWhiteSpace()) return false;

            appSettingsFilePath = ChoConfigurationManager.GetFullPath(appSettingsFilePath);
            if (!File.Exists(appSettingsFilePath)) return true;

            //try
            //{
            //    XDocument doc = XDocument.Load(appSettingsFilePath);
            //    if (doc.Root == null
            //    || doc.Root.Name.LocalName != APP_SETTINGS_SECTION_NAME)
            //        return true;
            //}
            //catch
            //{
            //    return true;
            //}

            return false;
        }

        private void RefreshSection(string appSettingsFilePath)
        {
            if (!appSettingsFilePath.IsNullOrWhiteSpace())
            {
                if (ChoConfigurationManager.GetFullPath(ChoConfigurationManager.ApplicationConfiguration.AppSettings.File) != ChoConfigurationManager.GetFullPath(appSettingsFilePath))
                    ConfigElement[ChoConfigurationConstants.FORCE_PERSIST] = true;
            }

            if (!ChoConfigurationManager.ApplicationConfiguration.AppSettings.File.IsNullOrWhiteSpace())
            {
                if (!File.Exists(ChoConfigurationManager.GetFullPath(ChoConfigurationManager.ApplicationConfiguration.AppSettings.File)))
                    ConfigElement[ChoConfigurationConstants.FORCE_PERSIST] = true;
            }
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

            bool saveMainConfig = false;
            string appSettingsFilePath = this.GetAppSettingsFilePathIfAnySpecified();
            if (!appSettingsFilePath.IsNullOrWhiteSpace())
            {
                if (ChoConfigurationManager.GetFullPath(ChoConfigurationManager.ApplicationConfiguration.AppSettings.File) != ChoConfigurationManager.GetFullPath(appSettingsFilePath))
                {
                    saveMainConfig = true;
                    ChoConfigurationManager.ApplicationConfiguration.AppSettings.File = appSettingsFilePath;
                }
            }

            NameValueCollection nameValueCollection = ((NameValueCollection)data);
            if (ChoConfigurationManager.ApplicationConfiguration.AppSettings.File.IsNullOrWhiteSpace())
            {
                foreach (string key1 in nameValueCollection.Keys)
                {
                    if (ChoConfigurationManager.ApplicationConfiguration.AppSettings.Settings.AllKeys.Contains(key1))
                        ChoConfigurationManager.ApplicationConfiguration.AppSettings.Settings[key1].Value = nameValueCollection[key1];
                    else
                        ChoConfigurationManager.ApplicationConfiguration.AppSettings.Settings.Add(new KeyValueConfigurationElement(key1, nameValueCollection[key1]));

                }

                ChoConfigurationManager.ApplicationConfiguration.Save(ConfigurationSaveMode.Modified);
            }
            else
            {
                if (saveMainConfig)
                    ChoConfigurationManager.ApplicationConfiguration.Save(ConfigurationSaveMode.Modified);
                File.WriteAllText(ChoConfigurationManager.GetFullPath(ChoConfigurationManager.ApplicationConfiguration.AppSettings.File), GetAppSettingsText(nameValueCollection));
            }
            //ConfigurationManager.RefreshSection(APP_SETTINGS_SECTION_NAME);
            ChoConfigurationManager.Refresh();
        }

		public override IChoConfigurationChangeWatcher ConfigurationChangeWatcher
		{
			get
			{
                //ConfigurationManager.RefreshSection(APP_SETTINGS_SECTION_NAME);
                ChoConfigurationManager.Refresh();

                string appSettingsFilePath = this.GetAppSettingsFilePathIfAnySpecified();
                
                List<IChoConfigurationChangeWatcher> _watchers = new List<IChoConfigurationChangeWatcher>();
                IChoConfigurationChangeWatcher configurationChangeWatcher1 = new ChoConfigurationChangeFileWatcher("AppConfig_{0}".FormatString(ConfigSectionName), ChoConfigurationManager.ApplicationConfigurationFilePath);
                configurationChangeWatcher1.SetConfigurationChangedEventHandler("App_{0}_AppSettings_WatcherHandler".FormatString(ConfigElement.ConfigElementPath),
                    (sender, e) =>
                    {
                        //ConfigurationManager.RefreshSection(APP_SETTINGS_SECTION_NAME);
                        ChoConfigurationManager.Refresh();

                        string lAppSettingsFilePath = this.GetAppSettingsFilePathIfAnySpecified();
                        //Console.WriteLine("AppConfig changed. " + lAppSettingsFilePath);

                        if (e is ChoConfigurationFileChangedEventArgs &&
                            ((ChoConfigurationFileChangedEventArgs)e).ConfigurationChangeAction == ChoConfigurationChangeAction.Changed)
                        {
                            //ConfigElement.Persist(true, null, true);
                            if (!((string)ConfigElement[ChoConfigurationConstants.TAG]).IsNullOrWhiteSpace() &&
                                (string)ConfigElement[ChoConfigurationConstants.TAG] != lAppSettingsFilePath)
                            {
                                ConfigElement[ChoConfigurationConstants.TAG] = lAppSettingsFilePath;
                                ConfigElement.Persist(true, null, true);
                            }
                            else if (((string)ConfigElement[ChoConfigurationConstants.TAG]).IsNullOrWhiteSpace())
                                ConfigElement[ChoConfigurationConstants.TAG] = lAppSettingsFilePath;
                        }
                    });
                _watchers.Add(configurationChangeWatcher1);

                if (!appSettingsFilePath.IsNullOrWhiteSpace())
				{
                    IChoConfigurationChangeWatcher configurationChangeWatcher = new ChoConfigurationChangeFileWatcher("AppSettingsConfig_{0}".FormatString(ConfigElement.ConfigElementPath), ChoConfigurationManager.GetFullPath(appSettingsFilePath));
					configurationChangeWatcher.SetConfigurationChangedEventHandler("{0}_AppSettings_WatcherHandler".FormatString(ConfigElement.ConfigElementPath),
						(sender, e) =>
						{
							if (e is ChoConfigurationFileChangedEventArgs &&
								(((ChoConfigurationFileChangedEventArgs)e).ConfigurationChangeAction == ChoConfigurationChangeAction.Deleted
								|| ((ChoConfigurationFileChangedEventArgs)e).ConfigurationChangeAction == ChoConfigurationChangeAction.Created))
							{
								ConfigElement.Persist(true, null, true);
							}
						});

					_watchers.Add(configurationChangeWatcher);
				}

				return new ChoConfigurationChangeCompositeWatcher(ConfigSectionName, _watchers.ToArray());
			}
		}

        private string GetAppSettingsText(NameValueCollection nameValueCollection)
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendLine("<appSettings>");

            if (nameValueCollection != null)
            {
                foreach (string key in nameValueCollection.AllKeys)
                {
                    msg.AppendLine("<add key='{0}' value='{1}' />".FormatString(key, nameValueCollection[key]));
                }
            }
            msg.AppendLine("</appSettings>");
            return msg.ToString().IndentXml();
        }

		private string GetAppSettingsFilePathIfAnySpecified()
		{
            string filePath = null;

            System.Configuration.AppSettingsSection appSettings = ChoConfigurationManager.ApplicationConfiguration.AppSettings;
            if (!appSettings.File.IsNullOrEmpty())
                filePath = appSettings.File.NTrim();
            else
                filePath = ConfigElement.ConfigFilePath.NTrim();

            if (Directory.Exists(ChoConfigurationManager.GetFullPath(filePath)))
                return null;
            else
                return filePath;
        }

		public override object PersistableState
		{
			get { return ChoObject.ToPersistableNameValueCollection(ConfigElement.ConfigObject); }
		}

		public override bool IsConfigSectionDefined
		{
			get
			{
				return ChoConfigurationManager.ApplicationConfiguration.AppSettings.Settings.Count > 0;
			}
		}

		protected override string ToXml(object data)
		{
			throw new NotImplementedException();
		}
	}
}
