namespace Cinchoo.Core.Logging
{
    #region NameSpaces

    using System;
    using System.Xml.Serialization;
    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Common;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.IO;

    #endregion NameSpaces

    [Serializable]
    [ChoTypeFormatter("Log Managers")]
    [ChoConfigurationSection("cinchoo/logManagerSettings", Defaultable = false)]
    [XmlRoot("logManagerSettings")]
    public class ChoLogManagerSettings : IChoObjectInitializable
    {
        #region Instance Data Members (Public)

        [XmlAttribute("turnOn")]
        public bool TurnOn = true;

        [XmlElement("logManager", typeof(ChoObjConfigurable))]
        [ChoIgnoreMemberFormatter]
        [ChoPropertyInfo(Persistable = false)]
        public ChoObjConfigurable[] LogManagerTypes = new ChoObjConfigurable[0];

        #endregion

        #region Instance Data Members (Private)

        private ChoDictionary<string, IChoLogManager> _logManagers;

        #endregion

        #region Shared Data Members (Private)

        private static string _buildInLogManagerType = typeof(ChoLogManager).AssemblyQualifiedName;

        #endregion Shared Data Members (Private)

        #region Instance Properties (Public)

        [XmlIgnore]
        [ChoIgnoreMemberFormatter]
        public IChoLogManager[] LoggerManagers
        {
            get { return _logManagers.ToValuesArray(); }
        }

        [XmlIgnore]
        [ChoMemberFormatter("Avail LogManagers", Formatter = typeof(ChoArrayToStringFormatter))]
        internal string[] LoggerManagerKeys
        {
            get { return _logManagers.ToKeysArray(); }
        }

        #endregion

        #region Shared Properties

        public static ChoLogManagerSettings Me
        {
            get { return ChoConfigurationManagementFactory.CreateInstance<ChoLogManagerSettings>(); }
        }

        private static ChoDictionary<string, IChoLogManager> _defaultLogManagers;
        public static ChoDictionary<string, IChoLogManager> DefaultLogManagers
        {
            get
            {
                if (_defaultLogManagers == null)
                {
                    //ChoStreamProfile.Clean(ChoReservedDirectoryName.Settings, ChoPath.AddExtension(typeof(ChoLogManagerSettings).FullName, ChoReservedFileExt.Err));

                    _defaultLogManagers = new ChoDictionary<string, IChoLogManager>();
                    ChoObjConfigurable.Load<IChoLogManager>(ChoPath.AddExtension(typeof(ChoLogManagerSettings).FullName, ChoReservedFileExt.Log), _buildInLogManagerType, _defaultLogManagers, null);
                }

                return _defaultLogManagers;
            }
        }

        #endregion

        #region IChoObjectInitializable Members

        public bool Initialize(bool beforeFieldInit, object state)
        {
            _logManagers = new ChoDictionary<string, IChoLogManager>(DefaultLogManagers);
            if (!beforeFieldInit)
                ChoObjConfigurable.Load<IChoLogManager>(ChoType.GetLogFileName(typeof(ChoLogManagerSettings)), ChoType.GetTypes(typeof(ChoLogManagerAttribute)),
                    _logManagers, LogManagerTypes);

            return false;
        }

        #endregion
    }
}
