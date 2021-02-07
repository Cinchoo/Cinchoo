namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;
	using Cinchoo.Core.Collections.Specialized;
	using System.Collections;
	using Cinchoo.Core.Services;

	#endregion NameSpaces

	public class ChoFileDictionaryConfigStorage : ChoConfigStorage, IChoDictionaryConfigStorage
	{
		public override object Load(ChoBaseConfigurationElement configElement, XmlNode node)
		{
			base.Load(configElement, node);

			return ConfigNode.ToDictionary();
		}

		protected override string ToXml(object data)
		{
			if (!(data is IDictionary))
				throw new ChoConfigurationException("Data object is not IDictionary object.");

			return ((IDictionary)data).ToXml(ConfigSectionName);
		}

		public override object PersistableState
		{
			get { return ChoObject.ToPersistableDictionaryCollection(ConfigElement); }
		}
	}
}
