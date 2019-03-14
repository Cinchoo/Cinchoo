namespace eSquare.Core
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.Xml.Serialization;
    using System.Collections.Generic;

    using eSquare.Core.Types;
    using eSquare.Core.Interfaces;
    using eSquare.Core.Attributes;
    using eSquare.Core.Configuration;
    using eSquare.Core.Diagnostics;
    using eSquare.Core.Formatters;
    using eSquare.Core.Collections.Generic;

    #endregion NameSpaces

    public class ChoConfigNameValue : ChoObjTypeConfigurable
    {
    }

    [Serializable]
    [ChoTypeFormatter("Property Manager Settings")]
    [ChoObjectFactoryAttribute(ChoObjectConstructionType.Singleton)]
    [ChoNameValueConfigurationElementMap("eSquare/propertyManagersSettings", WatchChange = true, IgnoreError = true, Defaultable = true)]
    [XmlRoot("propertyManagerSettings")]
    public class ChoPropertyManagerSettings : IChoObjectInitializable
    {
        #region Instance Data Members (Public)

        [XmlElement("propertyManager", typeof(ChoObjTypeConfigurable))]
        [ChoMemberFormatterIgnore]
        public ChoObjTypeConfigurable[] PropertyManagerTypes = new ChoObjTypeConfigurable[0];

        #endregion

        #region Instance Data Members (Private)

        private ChoDictionary<string, IFormatProvider> _propertyManagers = new ChoDictionary<string, IFormatProvider>();

        #endregion

        #region Instance Properties (Public)

        [XmlIgnore]
        [ChoMemberFormatterIgnore]
        public IFormatProvider[] PropertyManagers
        {
            get { return _propertyManagers.ToValuesArray(); }
        }

        [XmlIgnore]
        [ChoMemberFormatter("Avail PropertyManagers", FormatterType = typeof(ChoArrayToStringFormatter))]
        internal string[] PropertyManagerKeys
        {
            get { return _propertyManagers.ToKeysArray(); }
        }

        #endregion

        #region Instance Members (Public)

        public bool TryGetValue(string formatterName, out IFormatProvider propertyManager)
        {
            return _propertyManagers.TryGetValue(formatterName, out propertyManager);
        }

        #endregion Instance Members (Public)

        #region Shared Properties

        public static ChoPropertyManagerSettings Me
        {
            get { return ChoConfigurationManagementFactory.CreateInstance(typeof(ChoPropertyManagerSettings)) as ChoPropertyManagerSettings; }
        }

        #endregion

        #region IChoObjectInitializable Members

        public void Initialize(bool beforeInit)
        {
            if (!beforeInit)
                ChoObjTypeConfigurable.Load<IFormatProvider>(typeof(ChoPropertyManagerSettings).Name, ChoType.GetTypes<IFormatProvider>(),
                    _propertyManagers, PropertyManagerTypes, ChoDefaultObjectKey.Name);
            else
                ChoStreamProfile.Clean(ChoLogDirectories.Settings, Path.ChangeExtension(typeof(ChoPropertyManagerSettings).Name, ChoExt.Err));
        }

        #endregion
    }
}
