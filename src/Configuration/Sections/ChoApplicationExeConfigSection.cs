namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
    using System.Xml;

	#endregion NameSpaces

	internal class ChoApplicationExeConfigSection : ChoNameValueSection
	{
		#region Constructors

		public ChoApplicationExeConfigSection(Type configObjectType, XmlNode node, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
			: base(configObjectType, node, contextNodes, configElement)
		{
		}

		#endregion Constructors

		#region ChoConfigSection Overrides

		public override object this[string name]
		{
			get { throw new NotImplementedException(); }
		}

		#endregion ChoConfigSection Overrides

		#region IChoConfigSection Members

		protected override IChoConfigStorage DefaultConfigStorage
		{
			get { return new ChoApplicationExeConfigStorage(); }
		}

		#endregion
	}
}
