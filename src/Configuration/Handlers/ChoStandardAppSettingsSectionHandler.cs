namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;

    using System.Xml;

	#endregion NameSpaces

	public sealed class ChoStandardAppSettingsSectionHandler : IChoNullConfigurationSectionableHandler
	{
		public ChoConfigSection Create(Type configObjectType, XmlNode section, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
		{
			return new ChoStandardAppSettingsSection(configObjectType, section, contextNodes, configElement);
		}
	}
}
