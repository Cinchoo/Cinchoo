namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
    using System.Xml;

    #endregion

    [Serializable]
	public sealed class ChoRegistrySection : ChoDictionarySection
	{
		#region Constructors

		public ChoRegistrySection(Type configObjectType, XmlNode node, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
			: base(configObjectType, node, contextNodes, configElement)
		{
		}

		#endregion Constructors

		#region IChoConfigSection Members

		protected override IChoConfigStorage DefaultConfigStorage
		{
			get { return new ChoRegistryConfigStorage(); }
		}

        public override object PersistableState
        {
            get { return ChoObject.ToPersistableDictionaryCollection(ConfigObject, typeof(Object)); }
        }

		#endregion
	}
}
