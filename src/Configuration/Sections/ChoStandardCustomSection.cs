namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
    using System.Xml;

    #endregion NameSpaces

    public abstract class ChoStandardCustomSection : ChoConfigSection, IChoCustomConfigSection
	{
		#region Constructors

		public ChoStandardCustomSection(Type configObjectType, XmlNode node, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
			: base(configObjectType, node, contextNodes, configElement)
		{
		}

		#endregion Constructors

		#region IChoCustomConfigSection Members

		public override object PersistableState
		{
			get { return ConfigObject; }
		}

		#endregion

		#region IChoCustomConfigSection Members

		public void ResetObject()
		{
			ChoObject.ResetObject(ConfigObject);
		}

		#endregion
	}
}
