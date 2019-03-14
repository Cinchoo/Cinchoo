namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

    using System;
    using System.Collections.Specialized;
    using System.Xml;

    #endregion NameSpaces

    public sealed class ChoStandardAppSettingsSection : ChoNameValueSection
	{
		#region Constructors

		public ChoStandardAppSettingsSection(Type configObjectType, XmlNode node, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
			: base(configObjectType, node, contextNodes, configElement)
		{
		}

		#endregion Constructors

		#region IChoConfigSection Members

		public override object this[string name]
		{
			get
			{
				ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

				if (!(ConfigData is NameValueCollection))
					throw new ChoConfigurationException("ConfigObject is not of NameValueCollection type.");

				return ((NameValueCollection)ConfigData)[name];
			}
		}

		#endregion

		#region ChoConfigSection Overrides

		protected override IChoConfigStorage DefaultConfigStorage
		{
			get { return new ChoStandardAppSettingsConfigStorage(); }
		}

		#endregion ChoConfigSection Overrides
	}
}
