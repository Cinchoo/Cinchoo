namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Xml;



	#endregion

	public class ChoDictionarySectionHandler : IChoStandardConfigurationSectionHandler
	{
		public ChoConfigSection Create(Type configObjectType, XmlNode section, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
		{
			return new ChoDictionarySection(configObjectType, section, contextNodes, configElement);
		}
	}
}
