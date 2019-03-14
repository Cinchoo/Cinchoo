namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Xml;
	using Cinchoo.Core.IO;
	using Cinchoo.Core.Services;
	using Cinchoo.Core.Xml;

	#endregion NameSoaces

	public abstract class ChoConfigStorage : ChoBaseConfigStorage
    {
        #region Constants

        private const string UNDERLYING_CONFIG_PATH = "#UNDERLYING_CONFIG_PATH#";
        private const string PREV_UNDERLYING_CONFIG_PATH = "#PREV_UNDERLYING_CONFIG_PATH#";

        #endregion Constants

        #region Instance Data Members (Protected)

        protected string UnderlyingConfigFilePath;
		protected string ConfigFilePath;
		protected string[] IncludeFilePaths = null;

		#endregion Instance Data Members (Protected)

		#region IChoConfigStorage Overrides

		protected string GetFullPath(string filePath)
		{
			if (filePath.IsNullOrEmpty())
				return filePath;
			if (!Path.IsPathRooted(filePath))
				filePath = Path.Combine(Path.GetDirectoryName(ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath), ChoPath.ChangeExtension(filePath, ChoReservedFileExt.Xml));

			filePath = ChoPath.GetFullPath(filePath);

			return filePath;
		}

		public override object Load(ChoBaseConfigurationElement configElement, XmlNode node)
		{
			base.Load(configElement, node);

            //if (configElement.ConfigFilePath.IsNullOrWhiteSpace())
            //    UnderlyingConfigFilePath = ChoConfigurationManager.GetConfigFile(node);
            //else
            //    UnderlyingConfigFilePath = configElement.ConfigFilePath;

            UnderlyingConfigFilePath = ChoConfigurationManager.GetConfigFile(node);
            if (UnderlyingConfigFilePath.IsNullOrWhiteSpace())
            {
                configElement[ChoConfigurationConstants.FORCE_PERSIST] = true;
                UnderlyingConfigFilePath = configElement.ConfigFilePath;
            }

            ConfigElement[UNDERLYING_CONFIG_PATH] = UnderlyingConfigFilePath;

            ConfigFilePath = GetFullPath(UnderlyingConfigFilePath);
            
            if (IsAppConfigFile)
			{
				if (!configElement.ConfigFilePath.IsNullOrWhiteSpace())
				{
					UnderlyingConfigFilePath = configElement.ConfigFilePath;
					ConfigFilePath = GetFullPath(UnderlyingConfigFilePath);
				}
			}

			if (node != null && !IsAppConfigFile)
			{
				if (configElement.Persistable)
					ChoXmlDocument.CreateXmlFileIfEmpty(ConfigFilePath);

				using (ChoXmlDocument document = new ChoXmlDocument(ConfigFilePath))
				{
					string nodeName = configElement.ConfigElementPath;
					IncludeFilePaths = document.IncludeFiles;
					if (document.XmlDocument != null && document.XmlDocument.DocumentElement != null)
					{
						ChoConfiguration configuration = document.XmlDocument.DocumentElement.ToObject<ChoConfiguration>();
						if (configuration != null)
						{
							ConfigNode = ChoXmlDocument.GetXmlNode(nodeName, configuration.RestOfXmlDocumentElements);
						}
					}
				}
			}

			return null;
		}

		public override void Persist(object data, ChoDictionaryService<string, object> stateInfo)
		{
			ChoXmlDocument.CreateXmlFileIfEmpty(ConfigFilePath);

			string backupConfigFilePath = String.Format("{0}.{1}", ConfigFilePath, ChoReservedFileExt.Cho);

			try
			{
				//Write meta-data info
				ChoConfigurationMetaDataManager.SetMetaDataSection(ConfigElement);
				if (!IsAppConfigFile)
				{
					if (File.Exists(backupConfigFilePath))
						File.SetAttributes(backupConfigFilePath, FileAttributes.Archive);
					File.Copy(ConfigFilePath, backupConfigFilePath, true);
					if (File.Exists(backupConfigFilePath))
						File.SetAttributes(backupConfigFilePath, FileAttributes.Hidden);
				}
			}
			catch (ChoFatalApplicationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				ConfigElement.Log(ex.ToString());
			}

            try
            {
                //if seperate config file maintained, make a link in the application configuration
                if (IsConfigReferenceDataChanged(stateInfo))
                {
                    using (ChoXmlDocument xmlDocument = new ChoXmlDocument(ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath))
                    {
                        XmlNode configNode = xmlDocument.XmlDocument.MakeXPath(ConfigElement.ConfigElementPath);
                        if (configNode != null)
                        {
                            if (!IsAppConfigFile)
                            {
                                string configXml = @"<{0} {1}=""{2}"" {3}=""{4}"" />".FormatString(ConfigSectionName, ChoConfigurationManager.PathToken, UnderlyingConfigFilePath, ChoXmlDocument.CinchoNSToken, ChoXmlDocument.CinchooNSURI);
                                ChoXmlDocument.SetNamespaceAwareOuterXml(configNode, configXml, ChoXmlDocument.CinchooNSURI);
                            }
                        }
                    }
                }

                PersistConfigData(data, stateInfo);
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ConfigElement.Log(ex.ToString());

                if (!IsAppConfigFile)
                    File.Copy(backupConfigFilePath, ConfigFilePath, true);
            }
		}

        private bool IsConfigReferenceDataChanged(ChoDictionaryService<string, object> stateInfo)
        {
            if (stateInfo[PREV_UNDERLYING_CONFIG_PATH] == null)
                stateInfo[PREV_UNDERLYING_CONFIG_PATH] = stateInfo[UNDERLYING_CONFIG_PATH];

            if (stateInfo != null && ((bool)stateInfo[ChoConfigurationConstants.FORCE_PERSIST]
                || (string)stateInfo[UNDERLYING_CONFIG_PATH] != (string)stateInfo[PREV_UNDERLYING_CONFIG_PATH]))
            {
                ConfigElement[PREV_UNDERLYING_CONFIG_PATH] = stateInfo[UNDERLYING_CONFIG_PATH];
                return true;
            }

            return false;
        }

		public override IChoConfigurationChangeWatcher ConfigurationChangeWatcher
		{
			get
			{
				List<IChoConfigurationChangeWatcher> _watchers = new List<IChoConfigurationChangeWatcher>();
				_watchers.Add(ChoConfigurationManager.ConfigurationChangeWatcher);

				//Setup section specific configuration watcher
				if (!IsAppConfigFile)
				{
					IChoConfigurationChangeWatcher sectionConfigurationChangeWatcher = new ChoConfigurationChangeCompositeFileWatcher(ConfigSectionName, ConfigFilePath, IncludeFilePaths);
					_watchers.Add(sectionConfigurationChangeWatcher);
				}
				return new ChoConfigurationChangeCompositeWatcher(ConfigSectionName, _watchers.ToArray());
			}
		}

		#endregion IChoConfigStorage Overrides

		#region Instance Members (Protected)

		protected abstract string ToXml(object data);

		protected virtual void PersistConfigData(object data, ChoDictionaryService<string, object> stateInfo)
		{
			PersistAsPlainXml(data, stateInfo);
		}

        protected void PersistAsPlainXml(object data, ChoDictionaryService<string, object> stateInfo)
		{
            using (ChoXmlDocument xmlDocument = new ChoXmlDocument(stateInfo[UNDERLYING_CONFIG_PATH] as string))
			{
				PersistConfigSectionDefinition(xmlDocument.XmlDocument, data);
				XmlNode configNode = xmlDocument.XmlDocument.MakeXPath(ConfigElement.ConfigElementPath);
				if (configNode != null)
				{
					string configXml = ToXml(data);
					if (configXml.IsNullOrEmpty())
						return;

					ChoXmlDocument.SetOuterXml(configNode, configXml);
				}
			}
		}

        protected void PersistAsNameSpaceAwareXml(object data, ChoDictionaryService<string, object> stateInfo)
		{
            using (ChoXmlDocument xmlDocument = new ChoXmlDocument(stateInfo[UNDERLYING_CONFIG_PATH] as string))
			{
				PersistConfigSectionDefinition(xmlDocument.XmlDocument, data);
				XmlNode configNode = xmlDocument.XmlDocument.MakeXPath(ConfigElement.ConfigElementPath);
				if (configNode != null)
				{
					string configXml = ToXml(data);
					if (configXml.IsNullOrEmpty())
						return;

					ChoXmlDocument.SetNamespaceAwareOuterXml(configNode, configXml, ChoXmlDocument.CinchooNSURI);
				}
			}
		}

		protected virtual void PersistConfigSectionDefinition(XmlDocument xmlDocument, object data)
		{
			XmlNode configSectionsNode = xmlDocument.SelectSingleNode("configSections");
			if (configSectionsNode == null)
				configSectionsNode = xmlDocument.MakeXPath("configSections");

			if (configSectionsNode == null)
				return;

			XmlNode sectionNode = MakeSectionDefinition(xmlDocument, configSectionsNode, ConfigElement.ConfigElementPath, ConfigElement.ConfigSectionHandlerType.AssemblyQualifiedName);
			ChoConfigurationManager.OpenExeConfiguration(false);
		}

		#endregion Instance Members (Protected)

		#region Instance Properties (Protected)

		protected bool IsAppConfigFile
		{
			get { return ConfigFilePath == ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath; }
		}

		#endregion Instance Properties (Protected)

		#region Instance Members (Private)

		private static XmlNode MakeSectionDefinition(XmlDocument doc, XmlNode parent, string xpath, string typeName)
		{
			// grab the next node name in the xpath; or return parent if empty
			string[] partsOfXPath = xpath.Trim('/').Split('/');
			string nextNodeInXPath = partsOfXPath.First();
			if (string.IsNullOrEmpty(nextNodeInXPath))
				return parent;

			if (partsOfXPath.Length >= 2)
			{
				//Create section group
				XmlNode sectionGroupNode = parent.SelectSingleNode("sectionGroup[@name='{0}']".FormatString(nextNodeInXPath));
				if (sectionGroupNode == null)
				{
					sectionGroupNode = parent.AppendChild(doc.CreateElement("sectionGroup"));
					sectionGroupNode.Attributes.Append(doc.CreateAttribute("name"));
					sectionGroupNode.Attributes["name"].Value = nextNodeInXPath;
				}
				string rest = String.Join("/", partsOfXPath.Skip(1).ToArray());
				return MakeSectionDefinition(doc, sectionGroupNode, rest, typeName);
			}
			else
			{
				//Create section
				//Create section group
				XmlNode sectionNode = parent.SelectSingleNode("section[@name='{0}']".FormatString(nextNodeInXPath));
				if (sectionNode == null)
				{
					sectionNode = parent.AppendChild(doc.CreateElement("section"));

					sectionNode.Attributes.Append(doc.CreateAttribute("name"));
					sectionNode.Attributes["name"].Value = nextNodeInXPath;

					sectionNode.Attributes.Append(doc.CreateAttribute("type"));
					sectionNode.Attributes["type"].Value = typeName;
				}
				return sectionNode;
			}
		}

		#endregion Instance Members (Private)
	}
}
