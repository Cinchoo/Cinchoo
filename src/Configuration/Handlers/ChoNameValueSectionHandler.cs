namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
    using System.Xml;

    #endregion

    public sealed class ChoNameValueSectionHandler : IChoStandardConfigurationSectionHandler
	{
		public ChoConfigSection Create(Type configObjectType, XmlNode section, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
		{
			return new ChoNameValueSection(configObjectType, section, contextNodes, configElement);
		}
	}
}
