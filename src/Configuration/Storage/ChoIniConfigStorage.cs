namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.IO;
	using System.Xml;
	using System.Xml.XPath;
	using Cinchoo.Core.Ini;
	using Cinchoo.Core.Services;
	using Cinchoo.Core.Xml;

	#endregion NameSpaces

	public sealed class ChoIniConfigStorage : ChoConfigStorage, IChoNameValueConfigStorage
	{
		#region Instance Data Members (Private)

		private ChoIniSectionInfo _iniSectionInfo;

		#endregion Instance Data Members (Private)

		public override object Load(ChoBaseConfigurationElement configElement, XmlNode node)
		{
			base.Load(configElement, node);

			_iniSectionInfo = new ChoIniSectionInfo(ConfigNode, ConfigElement);
			_iniSectionInfo.Validate();
			return _iniSectionInfo.GetData();
		}

		protected override void PersistConfigData(object data, ChoDictionaryService<string, object> stateInfo)
		{
			ChoGuard.ArgumentNotNull(data, "Config Data Object");

            PersistAsNameSpaceAwareXml(data, stateInfo);

			//Write actual section data
			if (_iniSectionInfo != null)
				_iniSectionInfo.SaveData(data);
		}

		protected override string ToXml(object data)
		{
			return _iniSectionInfo != null ? _iniSectionInfo.AsConfigSectionXml(ConfigSectionName) : null;
		}

		public override IChoConfigurationChangeWatcher ConfigurationChangeWatcher
		{
			get
			{
				//Main app configuration
				List<IChoConfigurationChangeWatcher> _watchers = new List<IChoConfigurationChangeWatcher>();
				_watchers.Add(base.ConfigurationChangeWatcher);

				if (_iniSectionInfo != null && !_iniSectionInfo.IniFilePath.IsNullOrWhiteSpace())
				{
					IChoConfigurationChangeWatcher configurationChangeWatcher = new ChoConfigurationChangeFileWatcher(ConfigSectionName, _iniSectionInfo.IniFilePath);
					configurationChangeWatcher.SetConfigurationChangedEventHandler("{0}_INI_WatcherHandler".FormatString(ConfigElement.ConfigElementPath),
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

		public override object PersistableState
		{
			get { return ChoObject.ToPersistableNameValueCollection(ConfigElement.ConfigObject); }
		}
	}

	internal class ChoIniSectionInfo : ChoEquatableObject<ChoIniSectionInfo>
	{
		#region Constants

		private const string IniSectionToken = "cinchoo:iniSectionName";
		private const string IniPathToken = "cinchoo:iniFilePath";

		#endregion Constants

		#region Instance Data Members (Private)

		public string IniFilePath;
		public readonly string IniSectionName;

		#endregion Instance Data Members (Private)

		#region Instance Data Members (Public)

		public ChoIniSectionInfo(XmlNode node, ChoBaseConfigurationElement configElement)
		{
			if (node != null)
			{
				XPathNavigator navigator = node.CreateNavigator();

				IniSectionName = (string)navigator.Evaluate(String.Format("string(@{0})", IniSectionToken), ChoXmlDocument.NameSpaceManager);
				IniFilePath = (string)navigator.Evaluate(String.Format("string(@{0})", IniPathToken), ChoXmlDocument.NameSpaceManager);
			}

			if (IniSectionName.IsNullOrWhiteSpace())
				IniSectionName = configElement[ChoIniConfigurationSectionAttribute.INI_SECTION_NAME] as string;
			else
				configElement[ChoIniConfigurationSectionAttribute.INI_SECTION_NAME] = IniSectionName;

			if (IniFilePath.IsNullOrWhiteSpace())
				IniFilePath = configElement[ChoIniConfigurationSectionAttribute.INI_FILE_PATH] as string;
			else
				configElement[ChoIniConfigurationSectionAttribute.INI_FILE_PATH] = IniFilePath;
		}

		public string AsConfigSectionXml(string configSectionName)
		{
			return @"<{0} {1}=""{2}"" {3}=""{4}"" {5}=""{6}"" />".FormatString(configSectionName, IniSectionToken, IniSectionName, IniPathToken, IniFilePath, ChoXmlDocument.CinchoNSToken, ChoXmlDocument.CinchooNSURI);
		}

		public void Validate()
		{
			if (IniSectionName.IsNullOrWhiteSpace())
				throw new ChoConfigurationException("Missing INI section name.");

			if (IniFilePath.IsNullOrWhiteSpace())
				throw new ChoConfigurationException(String.Format("Missing INI filepath for '{0}' ini section name.", IniSectionName));

			IniFilePath = ChoString.ExpandProperties(IniFilePath);
		}

		#endregion

		internal object GetData()
		{
			if (!IniFilePath.IsNullOrWhiteSpace() && File.Exists(IniFilePath))
			{
				using (ChoIniDocument iniDocument = ChoIniDocument.Load(IniFilePath))
				{
					ChoIniSectionNode iniSectionNode = iniDocument.GetSection(IniSectionName);
					if (iniSectionNode != null)
						return iniSectionNode.ToNameValueCollection();
				}
			}

			return null;
		}

		internal void SaveData(object data)
		{
			if (!(data is NameValueCollection))
				throw new ChoConfigurationException("Data object is not NameValueCollection object.");

			NameValueCollection nameValueCollection = data as NameValueCollection;

			if (!IniFilePath.IsNullOrWhiteSpace())
			{
				if (!File.Exists(IniFilePath))
				{
					using (File.CreateText(IniFilePath))
					{ }
				}

				using (ChoIniDocument iniDocument = new ChoIniDocument(IniFilePath))
				{
					ChoIniSectionNode sectionNode = iniDocument.GetSection(IniSectionName);
					if (sectionNode == null)
                        sectionNode = iniDocument.AddSection(IniSectionName);

					sectionNode.ClearNodes();
					foreach (string key in nameValueCollection.AllKeys)
						sectionNode.AddNameValueNode(key, nameValueCollection[key] == null ? String.Empty : nameValueCollection[key].ToString());

					sectionNode.AppendNewLine();

					iniDocument.Save();
				}
			}
		}
	}
}
