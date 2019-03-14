namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml;
    using System.Xml.XPath;
    using Cinchoo.Core;
    using Cinchoo.Core.Services;
    using Cinchoo.Core.Win32;
    using Cinchoo.Core.Xml;
    using Microsoft.Win32;

	#endregion NameSpaces

	public sealed class ChoRegistryConfigStorage : ChoConfigStorage, IChoDictionaryConfigStorage
	{
		#region Instance Data Members (Private)

		private ChoRegistrySectionInfo _registrySectionInfo;

		#endregion Instance Data Members (Private)

		#region IChoConfigStorage Overrides

        public override object Load(ChoBaseConfigurationElement configElement, XmlNode node)
		{
            base.Load(configElement, node);

            _registrySectionInfo = new ChoRegistrySectionInfo(ConfigNode, ConfigObjectType, ConfigElement);
            _registrySectionInfo.Validate();
            return _registrySectionInfo.LoadRegistryInfo();
		}

        public override void PostLoad(ChoBaseConfigurationElement configElement)
        {
            _registrySectionInfo.PostLoad(configElement);
        }

        protected override void PersistConfigData(object data, ChoDictionaryService<string, object> stateInfo)
        {
            ChoGuard.ArgumentNotNull(data, "Config Data Object");

            PersistAsNameSpaceAwareXml(data, stateInfo);

            //Write actual section data
            if (_registrySectionInfo != null)
                _registrySectionInfo.SaveData(data);
        }

		protected override string ToXml(object data)
		{
			return _registrySectionInfo != null ? _registrySectionInfo.ToXml(ConfigSectionName) : null;
		}

		public override IChoConfigurationChangeWatcher ConfigurationChangeWatcher
		{
			get
			{
                //Main app configuration
                List<IChoConfigurationChangeWatcher> _watchers = new List<IChoConfigurationChangeWatcher>();
                _watchers.Add(base.ConfigurationChangeWatcher);

                if (_registrySectionInfo != null && !_registrySectionInfo.RegistryKey.IsNullOrWhiteSpace())
                {
					ChoConfigurationChangeRegistryWatcher configurationChangeRegistryWatcher = new ChoConfigurationChangeRegistryWatcher(ConfigSectionName, _registrySectionInfo.RegistryKey);
                    _watchers.Add(configurationChangeRegistryWatcher);
                }

                return new ChoConfigurationChangeCompositeWatcher(ConfigSectionName, _watchers.ToArray());
			}
		}

        public override object PersistableState
        {
            get { return ChoObject.ToPersistableDictionaryCollection(ConfigElement, typeof(Object)); }
        }

		#endregion IChoConfigStorage Overrides
    }

	internal class ChoRegistrySectionInfo : ChoEquatableObject<ChoRegistrySectionInfo>
	{
		#region Constants

		private const string RegistryKeyToken = "cinchoo:registryKey";
        private const string EXPAND_KEY_POSTFIX = "_ES";

		#endregion Constants

		#region Instance Data Members (Public)

		public readonly string RegistryKey;
		[ChoIgnoreEqual]
		public readonly Type ConfigObjectType;
		[ChoIgnoreEqual]
		public readonly Dictionary<string, RegistryValueKind> RegistryValueKindDict = new Dictionary<string, RegistryValueKind>();
        public readonly Dictionary<string, RegistryValueKind> RegistryValueKindDictEx = new Dictionary<string, RegistryValueKind>();
        public readonly Dictionary<string, string> RegistryKeyMap = new Dictionary<string, string>();

		#endregion Instance Data Members (Public)

		public ChoRegistrySectionInfo(XmlNode node, Type configObjectType, ChoBaseConfigurationElement configElement)
		{
			ConfigObjectType = configObjectType;
            if (configElement.ConfigObject is ChoConfigurableObject)
                ((ChoConfigurableObject)configElement.ConfigObject).PropertyChanged += ChoRegistrySectionInfo_PropertyChanged;

            if (node != null)
			{
				XPathNavigator navigator = node.CreateNavigator();
                RegistryKey = (string)navigator.Evaluate(String.Format("string(@{0})", RegistryKeyToken), ChoXmlDocument.NameSpaceManager);
			}

            if (RegistryKey.IsNullOrWhiteSpace())
                RegistryKey = configElement[ChoRegistryConfigurationSectionAttribute.REG_KEY] as string;
            else
                configElement[ChoRegistryConfigurationSectionAttribute.REG_KEY] = RegistryKey;
		}

        public string ToXml(string configSectionName)
		{
			return @"<{0} {1}=""{2}"" {3}=""{4}"" />".FormatString(configSectionName, RegistryKeyToken, RegistryKey, ChoXmlDocument.CinchoNSToken, ChoXmlDocument.CinchooNSURI);
		}

		public void Validate()
		{
			if (RegistryKey.IsNullOrWhiteSpace())
				throw new ChoConfigurationException("Missing registry key.");

			if (ConfigObjectType == null)
				throw new ChoConfigurationException("Missing configuration object type.");

			//Load RegistryValueKinds
			foreach (MemberInfo memberInfo in ChoType.GetMemberInfos(ConfigObjectType, typeof(ChoMemberRegistryInfoAttribute)))
			{
				ChoMemberRegistryInfoAttribute memberRegistryInfoAttribute = memberInfo.GetCustomAttribute<ChoMemberRegistryInfoAttribute>();
				string memberName = ChoType.GetMemberName(memberInfo);

				RegistryValueKindDict.Add(memberName, memberRegistryInfoAttribute.RegistryValueKind);
                RegistryValueKindDictEx.Add(memberInfo.Name, memberRegistryInfoAttribute.RegistryValueKind);
                RegistryKeyMap.Add(memberInfo.Name, memberName);
            }
		}

        public object LoadRegistryInfo()
        {
            if (ConfigObjectType == null)
                return null;

            using (ChoRegistryKey registryKey = new ChoRegistryKey(RegistryKey, false, true))
            {
                if (registryKey.RegistrySubKey == null)
                    return null;
                else
                {
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    foreach (string registryValueName in registryKey.GetValueNames())
                    {
                        dict.Add(registryValueName, registryKey.GetValue(registryValueName, RegistryValueOptions.DoNotExpandEnvironmentNames));
                    }
                    return dict;
                }
            }
        }

        internal bool IsRegistryKeyFound()
        {
            using (ChoRegistryKey registryKey = new ChoRegistryKey(RegistryKey, false, true))
            {
                return registryKey.RegistrySubKey != null;
            }
        }

        internal void SaveData(object data)
        {
            if (!(data is Dictionary<string, object>))
                throw new ChoConfigurationException("Data object is not Dictionary<string, object> object.");

            Dictionary<string, object> dict = data as Dictionary<string, object>;

            if (!RegistryKey.IsNullOrWhiteSpace())
            {
                using (ChoRegistryKey registryKey = new ChoRegistryKey(RegistryKey, true))
                {
                    foreach (string registryValueName in dict.Keys)
                    {
                        if (registryValueName.EndsWith(EXPAND_KEY_POSTFIX)) continue;

                        if (dict[registryValueName] != null)
                        {
                            if (RegistryValueKindDict.ContainsKey(registryValueName))
                                registryKey.SetValue(registryValueName, dict[registryValueName], RegistryValueKindDict[registryValueName]);
                            else
                                registryKey.SetValue(registryValueName, dict[registryValueName]);
                        }
                    }
                }
            }
        }

        public void PostLoad(ChoBaseConfigurationElement configElement)
        {
        }

        void ChoRegistrySectionInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (RegistryValueKindDictEx.ContainsKey(e.PropertyName) && RegistryValueKindDictEx[e.PropertyName] == RegistryValueKind.ExpandString)
            {
                string expandedStringMemberName = e.PropertyName + EXPAND_KEY_POSTFIX;
                MemberInfo memberInfo = ChoType.GetMemberInfo(sender.GetType(), expandedStringMemberName);

                if (memberInfo != null)
                {
                    object expandString = ChoType.GetMemberValue(sender, e.PropertyName); ;

                    if (expandString != null && expandString is string)
                        ChoType.SetMemberValue(sender, expandedStringMemberName, Environment.ExpandEnvironmentVariables(expandString as string));
                    else
                        ChoType.SetMemberValue(sender, expandedStringMemberName, null);
                }

                //Console.WriteLine(sender.ToString());
            }
        }
    }
}
