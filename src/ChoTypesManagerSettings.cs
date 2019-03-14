namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Xml;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using Cinchoo.Core.Configuration;

    #endregion NameSpaces

    [ChoConfigurationSection("cinchoo/typeManagerSettings", Defaultable = false)]
    public class ChoTypesManagerSettings : IChoObjectInitializable
    {
        #region Instance Data Members (Public)

        [XmlArray("excludedTypeNames")]
        [XmlArrayItem("typeName", Type=typeof(string))]
        [ChoPropertyInfo(Persistable = false)]
        public string[] ExcludedTypeNames;

        #endregion Instance Data Members (Public)

        #region Shared Properties

        public static ChoTypesManagerSettings Me
        {
            get { return ChoConfigurationManagementFactory.CreateInstance<ChoTypesManagerSettings>(); }
        }

        #endregion

        #region IChoObjectInitializable Members

        public bool Initialize(bool beforeFieldInit, object state)
        {
            if (ExcludedTypeNames == null) ExcludedTypeNames = new string[] {};

            return false;
        }

        #endregion

        #region Shared Members (Public)

        public static bool IsExcludedType(string typeName)
        {
            if (typeName.IsNullOrEmpty()) return false;

            if (ChoTypesManagerSettings.Me.ExcludedTypeNames != null)
            {
                foreach (string lTypeName in ChoTypesManagerSettings.Me.ExcludedTypeNames)
                {
                    if (typeName == lTypeName) return true;
                }

            }
            return false;
        }

        public static bool IsExcludedType(Type type)
        {
            if (type == null) return false;

            if (ChoTypesManagerSettings.Me.ExcludedTypeNames != null)
            {
                foreach (string lTypeName in ChoTypesManagerSettings.Me.ExcludedTypeNames)
                {
                    if (type.FullName == lTypeName
                        || type.AssemblyQualifiedName == lTypeName)
                        return true;
                }
            }

            return false;
        }

        #endregion Shared Members (Public)
    }
}
