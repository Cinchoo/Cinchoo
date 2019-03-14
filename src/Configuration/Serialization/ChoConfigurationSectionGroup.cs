namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
using System.Xml.Serialization;
	using System.Collections;

	#endregion NameSpaces

	[Serializable]
	[XmlType("sectionGroup")]
	public class ChoConfigurationSectionGroup
	{
		#region Instance Data Members (Private)

		private Dictionary<string, ChoConfigurationSectionGroup> _sectionGroupsDict = new Dictionary<string, ChoConfigurationSectionGroup>();
		private Dictionary<string, ChoConfigurationSection> _sectionsDict = new Dictionary<string, ChoConfigurationSection>();

		#endregion Instance Data Members (Private)
		
		#region Instance Data Members (Public)

		[XmlAttribute("name")]
		public string Name;

		[XmlElement(typeof(ChoConfigurationSection)), XmlElement(typeof(ChoConfigurationSectionGroup))]
		public ArrayList ConfigSectionsOrGroups;

		#endregion Instance Data Members (Public)

		#region Instance Properties (Public)

		[XmlIgnore]
		public Dictionary<string, ChoConfigurationSectionGroup> SectionGroups
		{
			get { return _sectionGroupsDict; }
		}

		[XmlIgnore]
		public Dictionary<string, ChoConfigurationSection> Sections
		{
			get { return _sectionsDict; }
		}

		#endregion Instance Properties (Public)

		#region Instance Members (Internal)

		internal void Initialize()
		{
			if (ConfigSectionsOrGroups != null)
			{
				foreach (object element in ConfigSectionsOrGroups)
				{
					if (element is ChoConfigurationSection)
					{
						if (!((ChoConfigurationSection)element).Name.IsNullOrWhiteSpace())
							_sectionsDict.Add(((ChoConfigurationSection)element).Name, element as ChoConfigurationSection);
					}
					else if (element is ChoConfigurationSectionGroup)
					{
						ChoConfigurationSectionGroup configurationSectionGroup = element as ChoConfigurationSectionGroup;
						if (!configurationSectionGroup.Name.IsNullOrWhiteSpace())
						{
							configurationSectionGroup.Initialize();
							_sectionGroupsDict.Add(configurationSectionGroup.Name, configurationSectionGroup);
						}
					}
				}
			}
		}

		#endregion Instance Members (Internal)

	}
}
