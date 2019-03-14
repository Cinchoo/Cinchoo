namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;

	#endregion NameSpaces

	public sealed class ChoApplicationExeSectionHandler : ChoCustomConfigurationSectionHandler
	{
		public override ChoConfigSection Create(Type configObjectType, XmlNode section, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
		{
			return new ChoApplicationExeConfigSection(configObjectType, section, contextNodes, configElement);
		}
	}
}
