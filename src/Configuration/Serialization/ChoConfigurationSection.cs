namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml.Serialization;
	using System.Xml;

	#endregion NameSpaces

	[Serializable]
	[XmlType("section")]
	public class ChoConfigurationSection
	{
		#region Instance Data Members (Public)

		[XmlAttribute("name")]
		public string Name;

		[XmlAttribute("type")]
		public string Type;

		[XmlAnyElement]
		public XmlNode[] RestOfXmlElements;

		#endregion Instance Data Members (Public)
	}
}
