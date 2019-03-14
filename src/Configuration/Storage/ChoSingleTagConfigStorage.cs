namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;
	using Cinchoo.Core.Collections.Specialized;
    using Cinchoo.Core.Shell;
    using System.Collections.Specialized;

	#endregion NameSpaces

    public sealed class ChoSingleTagConfigStorage : ChoConfigStorage, IChoNameValueConfigStorage
	{
        public override object Load(ChoBaseConfigurationElement configElement, XmlNode node)
		{
            base.Load(configElement, node);

			return ConfigNode.ToNameValuesFromAttributes();
		}

		protected override string ToXml(object data)
		{
            if (!(data is NameValueCollection))
                throw new ChoConfigurationException("Data object is not NameValueCollection object.");

            return ((NameValueCollection)data).ToSingleTagXml(ConfigSectionName);
		}

        public override object PersistableState
        {
            get { return ChoObject.ToPersistableNameValueCollection(ConfigElement.ConfigObject); }
        }
    }
}
