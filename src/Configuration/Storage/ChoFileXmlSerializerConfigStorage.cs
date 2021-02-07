namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinchoo.Core.Properties;
    using Cinchoo.Core.Services;

	#endregion NameSpaces

    public class ChoFileXmlSerializerConfigStorage : ChoConfigStorage, IChoObjectConfigStorage
    {
        #region Instance Data Members (Private)

        private static readonly object _padLock = new object();
        private static readonly Dictionary<Type, XmlAttributeOverrides> _typeXmlAttributeOverridesDict = new Dictionary<Type, XmlAttributeOverrides>();

        #endregion Instance Data Members (Private)

        public override object Load(ChoBaseConfigurationElement configElement, XmlNode node)
		{
			base.Load(configElement, node);

            node = ConfigNode;

			string typeName = null;
            Type objectType = ConfigObjectType;
            if (objectType == null)
                throw new ChoConfigurationException(String.Format(CultureInfo.InvariantCulture, Resources.ES1004, typeName));

            if (node == null)
                return null;

            XmlAttributeOverrides overrides = GetXmlAttributeOverrides(objectType, node.Name);

            XmlSerializer serializer = overrides != null ? new XmlSerializer(objectType, overrides) : XmlSerializer.FromTypes(new[] { objectType}).GetNValue(0);

            return serializer.Deserialize(new XmlNodeReader(node));
		}

		public override bool CanPersist(object data, ChoDictionaryService<string, object> stateInfo)
		{
			return data != null;
		}

		protected override string ToXml(object data)
		{
			return ChoXmlSerializerConfigHelper.GetObjectXmlString(data);
        }

        #region Instance Members (Private)

        private XmlAttributeOverrides GetXmlAttributeOverrides(Type type, string rootNodeName)
        {
            if (type == null)
                return null;

            if (_typeXmlAttributeOverridesDict.ContainsKey(type))
                return _typeXmlAttributeOverridesDict[type];

            lock (_padLock)
            {
                if (_typeXmlAttributeOverridesDict.ContainsKey(type))
                    return _typeXmlAttributeOverridesDict[type];

                if (type.GetCustomAttribute<XmlRootAttribute>() == null)
                {
                    XmlAttributeOverrides overrides = new XmlAttributeOverrides();
                    XmlAttributes attr = new XmlAttributes();
                    attr.XmlRoot = new XmlRootAttribute(rootNodeName);
                    overrides.Add(type, attr);

                    _typeXmlAttributeOverridesDict.Add(type, overrides);
                }
                else
                    _typeXmlAttributeOverridesDict.Add(type, null);

                return _typeXmlAttributeOverridesDict[type];
            }
        }

        #endregion Instance Members (Private)

        public override object PersistableState
        {
            get { return ConfigElement.ConfigObject; }
        }
    }
}
