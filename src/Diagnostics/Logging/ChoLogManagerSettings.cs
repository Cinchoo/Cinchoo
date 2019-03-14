namespace eSquare.Core.Diagnostics.Logging
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
    using eSquare.Core.Common;

    #endregion NameSpaces

    [Serializable]
    [ChoTypeFormatter("Log Managers")]
    [ChoNameValueConfigurationElementMap("eSquare/logManagerSettings", WatchChange = true, IgnoreError = true, Defaultable = true)]
    [XmlRoot("logManagerSettings")]
    public class ChoLogManagerSettings : IChoObjectInitializable
    {
        #region Instance Data Members (Public)

        [XmlAttribute("turnOn")]
        public bool TurnOn = true;

        [XmlElement("logManager", typeof(ChoObjConfigurable))]
        [ChoMemberFormatterIgnore]
        public ChoObjConfigurable[] LogManagerTypes = new ChoObjConfigurable[0];

        #endregion

        #region Instance Data Members (Private)

        private ChoDictionary<string, IChoLogManager> _logManagers;

        #endregion

        #region Shared Data Members (Private)

        private static string _buildInLogManagerType = "eSquare.Core.Diagnostics.Logging.ChoLogManager, eSquare.Core";

        #endregion Shared Data Members (Private)

        #region Instance Properties (Public)

        [XmlIgnore]
        [ChoMemberFormatterIgnore]
        public IChoLogManager[] LoggerManagers
        {
            get { return _logManagers.ToValuesArray(); }
        }

        [XmlIgnore]
        [ChoMemberFormatter("Avail LogManagers", FormatterType = typeof(ChoArrayToStringFormatter))]
        internal string[] LoggerManagerKeys
        {
            get { return _logManagers.ToKeysArray(); }
        }

        #endregion

        #region Shared Properties

        public static ChoLogManagerSettings Me
        {
            get { return ChoConfigurationManagementFactory.CreateInstance(typeof(ChoLogManagerSettings)) as ChoLogManagerSettings; }
        }

        private static ChoDictionary<string, IChoLogManager> _defaultLogManagers;
        public static ChoDictionary<string, IChoLogManager> DefaultLogManagers
        {
            get
            {
                if (_defaultLogManagers == null)
                {
                    ChoStreamProfile.Clean(ChoLogDirectories.Settings, Path.ChangeExtension(typeof(ChoLogManagerSettings).Name, ChoExt.Err));

                    _defaultLogManagers = new ChoDictionary<string, IChoLogManager>();
                    ChoObjConfigurable.Load<IChoLogManager>(typeof(ChoLogManagerSettings).Name, _buildInLogManagerType, _defaultLogManagers, null);
                }

                return _defaultLogManagers;
            }
        }

        #endregion

        #region IChoObjectInitializable Members

        public void Initialize(bool beforeInit)
        {
            _logManagers = new ChoDictionary<string, IChoLogManager>(DefaultLogManagers);
            if (!beforeInit)
                ChoObjConfigurable.Load<IChoLogManager>(typeof(ChoLogManagerSettings).Name, ChoType.GetTypes(typeof(ChoLogManagerAttribute)),
                    _logManagers, LogManagerTypes);
        }

        #endregion
    }
}
