namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
    using System.Xml;



    #endregion

    public sealed class ChoSingleTagSectionHandler : IChoStandardConfigurationSectionHandler
	{
		public ChoConfigSection Create(Type configObjectType, XmlNode section, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
		{
			return new ChoSingleTagSection(configObjectType, section, contextNodes, configElement);
		}
	}
}
