namespace eSquare.Core.Diagnostics.Logging.LogListeners
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
    using eSquare.Core.Collections.Generic;
    using eSquare.Core.Formatters;
    using eSquare.Core.Common;

    #endregion NameSpaces

    [Serializable]
    [ChoTypeFormatter("Log Listeners")]
    [ChoNameValueConfigurationElementMap("eSquare/logListenerSettings", WatchChange = true, IgnoreError = true, Defaultable = true)]
    [XmlRoot("logListenerSettings")]
    public class ChoLogListenerSettings : IChoObjectInitializable
    {
        #region Instance Data Members (Public)

        [XmlAttribute("turnOn")]
        public bool TurnOn = true;

        [XmlElement("logListener", typeof(ChoObjConfigurable))]
        [ChoMemberFormatterIgnore]
        public ChoObjConfigurable[] LogListenerTypes = new ChoObjConfigurable[0];

        #endregion

        #region Instance Data Members (Private)

        private ChoDictionary<string, ChoLogListener> _logListeners;

        #endregion

        #region Indexers

        public ChoLogListener this[string name]
        {
            get { return _logListeners != null && _logListeners.ContainsKey(name) ? _logListeners[name] : null; }
        }

        #endregion Indexers

        #region Shared Data Members (Private)

        private static string _buildInLogListenerType = "eSquare.Core.Diagnostics.Logging.LogListeners.ChoConsoleLogListener, eSquare.Core";

        #endregion Shared Data Members (Private)

        #region Instance Properties (Public)

        [XmlIgnore]
        [ChoMemberFormatterIgnore]
        public ChoLogListener[] LogListeners
        {
            get { return _logListeners.ToValuesArray(); }
        }

        [XmlIgnore]
        [ChoMemberFormatter("Avail Log Listeners", FormatterType = typeof(ChoArrayToStringFormatter))]
        internal string[] LogListenerKeys
        {
            get { return _logListeners.ToKeysArray(); }
        }

        #endregion

        #region Shared Properties

        public static ChoLogListenerSettings Me
        {
            get { return ChoConfigurationManagementFactory.CreateInstance(typeof(ChoLogListenerSettings)) as ChoLogListenerSettings; }
        }

        private static ChoDictionary<string, ChoLogListener> _defaultLogListeners;
        public static ChoDictionary<string, ChoLogListener> DefaultLogListeners
        {
            get
            {
                if (_defaultLogListeners == null)
                {
                    ChoStreamProfile.Clean(ChoLogDirectories.Settings, Path.ChangeExtension(typeof(ChoLogListenerSettings).Name, ChoExt.Err));

                    _defaultLogListeners = new ChoDictionary<string, ChoLogListener>();
                    ChoObjConfigurable.Load<ChoLogListener>(typeof(ChoLogListenerSettings).Name, _buildInLogListenerType, _defaultLogListeners, null);
                }

                return _defaultLogListeners;
            }
        }

        #endregion

        #region IChoObjectInitializable Members

        public void Initialize(bool beforeInit)
        {
            if (beforeInit)
                //Create the default/built-in objects
                _logListeners = new ChoDictionary<string, ChoLogListener>(DefaultLogListeners);
            else
                ChoObjConfigurable.Load<ChoLogListener>(typeof(ChoLogListenerSettings).Name, ChoType.GetTypes(typeof(ChoLogListenerAttribute)),
                    _logListeners, LogListenerTypes);
        }

        #endregion
    }
}
