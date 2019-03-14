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
	[XmlRoot("configuration")]
	public class ChoConfiguration
	{
		#region Instance Data Members (Public)

		[XmlElement("configSections")]
		public ChoConfigurationSectionGroup ConfigSection;

		[XmlAnyElement]
		public XmlNode[] RestOfXmlDocumentElements;

		#endregion Instance Data Members (Public)

		#region Instance Properties (Public)

		[XmlIgnore]	
		public Dictionary<string, ChoConfigurationSectionGroup> SectionGroups
		{
			get { return ConfigSection != null ? ConfigSection.SectionGroups : null; }
		}

		[XmlIgnore]
		public Dictionary<string, ChoConfigurationSection> Sections
		{
			get { return ConfigSection != null ? ConfigSection.Sections : null; }
		}

		#endregion Instance Properties (Public)

		#region Instance Members (Public)

		internal void Initialize()
		{
			if (ConfigSection != null)
				ConfigSection.Initialize();
			else
				ConfigSection = new ChoConfigurationSectionGroup();
		}

		#endregion Instance Members (Public)
	}
}
