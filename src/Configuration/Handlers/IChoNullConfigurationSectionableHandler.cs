namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
    using System.Xml;


    #endregion NameSpaces

    // Summary:
	//     Handles the access to certain configuration sections.
	public interface IChoConfigurationSectionHandler
	{
		ChoConfigSection Create(Type configObjectType, XmlNode section, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement);
	}

	public interface IChoNullConfigurationSectionableHandler : IChoConfigurationSectionHandler
	{
	}
}
