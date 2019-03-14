namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
    using System.Xml;

    #endregion NameSpaces

    public sealed class ChoXmlSerializerSectionHandler : IChoStandardConfigurationSectionHandler
	{
		public ChoConfigSection Create(Type configObjectType, XmlNode section, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
		{
			return new ChoXmlSerializerSection(configObjectType, section, contextNodes, configElement);
		}
	}
}
