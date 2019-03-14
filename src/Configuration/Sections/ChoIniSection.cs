namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
    using System.Xml;

    #endregion

    [Serializable]
	public sealed class ChoIniSection : ChoNameValueSection
	{
		#region Constructors

		public ChoIniSection(Type configObjectType, XmlNode node, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
			: base(configObjectType, node, contextNodes, configElement)
		{
		}

		#endregion Constructors

		#region IChoConfigSection Members

		protected override IChoConfigStorage DefaultConfigStorage
		{
			get { return new ChoIniNameValueConfigStorage(); }
		}

		#endregion
	}
}
