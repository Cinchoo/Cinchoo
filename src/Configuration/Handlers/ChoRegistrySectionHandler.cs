namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
    using System.Xml;

    #endregion

    public sealed class ChoRegistrySectionHandler : IChoStandardConfigurationSectionHandler
	{
		public ChoConfigSection Create(Type configObjectType, XmlNode section, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
		{
			return new ChoRegistrySection(configObjectType, section, contextNodes, configElement);
		}
	}
}
