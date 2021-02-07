namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections;
    using System.Xml.Serialization;
    using Cinchoo.Core;
    using Cinchoo.Core.Configuration;

    #endregion NameSpaces

    [XmlRoot("objectPool")]
    public class ChoObjectPool
    {
        [XmlAttribute("name")]
        public string Name;

        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable]
    [ChoTypeFormatter("Object Pools")]
    [ChoConfigurationSection("cinchoo/objectPoolSettings")]
    [XmlRoot("objectPoolSettings")]
    public class ChoObjectPoolSettings : ChoConfigurableObject
    {
        private static readonly ChoObjectPoolSettings _instance = new ChoObjectPoolSettings();

        #region Instance Data Members (Public)

        [XmlAttribute("defaultPool")]
        public string DefaultPoolName;

        [ChoIgnoreMemberFormatter]
        [XmlElement("objectPoolFile", typeof(string))]
        public string[] ObjectPoolFiles = new string[0];

        [XmlIgnore]
        [ChoTypeConverter(typeof(ChoFilesToObjectHashtableConverter), Parameters = new object[] { "ObjectPoolFiles", typeof(ChoObjectPool), "Name" })]
        [ChoMemberFormatter("Avail Object Pools", Formatter = typeof(ChoHashtableKeyToStringFormatter))]
        [CLSCompliant(false)]
        public Hashtable ObjectPools = new Hashtable();

        #endregion

        #region Shared Properties

        public static ChoObjectPoolSettings Me
        {
            get { return _instance; }
        }

        #endregion
    }
}
