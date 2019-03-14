namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
    using System.Xml;

    #endregion

    public sealed class ChoIniSectionHandler : IChoStandardConfigurationSectionHandler
	{
		public ChoConfigSection Create(Type configObjectType, XmlNode section, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
		{
			return new ChoIniSection(configObjectType, section, contextNodes, configElement);
		}
	}
}
