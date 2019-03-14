namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
    using System.Xml;

    #endregion

    [Serializable]
    public sealed class ChoXmlSerializerSection : ChoStandardCustomSection
	{
		private bool _hasConfigData;
        private string _payload;

		#region Constructors

		public ChoXmlSerializerSection(Type configObjectType, XmlNode node, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
			: base(configObjectType, node, contextNodes, configElement)
		{
			_hasConfigData = this.ConfigData != null;
            _payload = node != null ? node.OuterXml : null;
		}

		#endregion Constructors

		#region IChoConfigSection Members

        protected override void CheckValidConfigStoragePassed(IChoConfigStorage configStorage)
        {
            if (!(ConfigStorage is IChoObjectConfigStorage))
            {
                throw new ChoConfigurationException("ConfigStorage [{0}] is not a IChoObjectConfigStorage type.".FormatString(ConfigStorage.GetType().FullName));
            }
        }

		public override object this[string name]
		{
			get
			{
				ChoGuard.ArgumentNotNullOrEmpty(name, "Name");
				return ChoObject.GetObjectMemberValue(ConfigObject, name);
			}
		}

        public override bool HasConfigMemberDefined(string key)
        {
            return true;
        }

		public override bool HasConfigSectionDefined
		{
			get { return _hasConfigData; }
		}

		#endregion

		protected override IChoConfigStorage DefaultConfigStorage
		{
			get { return new ChoFileXmlSerializerConfigStorage(); }
		}

        public override bool Equals(ChoDisposableObject other)
        {
            ChoXmlSerializerSection other1 = other as ChoXmlSerializerSection;
            return other1 == null ? false : _payload == other1._payload;
        }
    }
}
