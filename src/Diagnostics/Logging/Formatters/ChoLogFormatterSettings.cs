namespace eSquare.Core.Diagnostics.Logging.Formatters
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
    [ChoTypeFormatter("Log Formatters")]
    [ChoNameValueConfigurationElementMap("eSquare/logFormatterSettings", WatchChange = true, IgnoreError = true, Defaultable = true)]
    [XmlRoot("logFormatterSettings")]
    public class ChoLogFormatterSettings : IChoObjectInitializable
    {
        #region Instance Data Members (Public)

        [XmlAttribute("turnOn")]
        public bool TurnOn = true;

        [XmlElement("logFormatter", typeof(ChoObjConfigurable))]
        [ChoMemberFormatterIgnore]
        public ChoObjConfigurable[] LogFormatterTypes = new ChoObjConfigurable[0];

        #endregion

        #region Instance Data Members (Private)

        private ChoDictionary<string, IChoLogFormatter> _logFormatters;

        #endregion

        #region Shared Data Members (Private)

        private static string _buildInLogFormatterType = "eSquare.Core.Diagnostics.Logging.Formatters.ChoTextLogFormatter, eSquare.Core";

        #endregion Shared Data Members (Private)

        #region Instance Properties (Public)

        [XmlIgnore]
        [ChoMemberFormatterIgnore]
        public IChoLogFormatter[] LogFormatters
        {
            get { return _logFormatters.ToValuesArray(); }
        }

        [XmlIgnore]
        [ChoMemberFormatter("Avail Log Formatters", FormatterType = typeof(ChoArrayToStringFormatter))]
        internal string[] LogFormatterKeys
        {
            get { return _logFormatters.ToKeysArray(); }
        }

        #endregion

        #region Indexers

        public IChoLogFormatter this[string name]
        {
            get { return _logFormatters != null && _logFormatters.ContainsKey(name) ? _logFormatters[name] : null; }
        }

        #endregion Indexers

        #region Shared Properties

        public static ChoLogFormatterSettings Me
        {
            get { return ChoConfigurationManagementFactory.CreateInstance(typeof(ChoLogFormatterSettings)) as ChoLogFormatterSettings; }
        }

        private static ChoDictionary<string, IChoLogFormatter> _defaultLogFormatters;
        public static ChoDictionary<string, IChoLogFormatter> DefaultLogFormatters
        {
            get
            {
                if (_defaultLogFormatters == null)
                {
                    ChoStreamProfile.Clean(ChoLogDirectories.Settings, Path.ChangeExtension(typeof(ChoLogFormatterSettings).Name, ChoExt.Err));

                    _defaultLogFormatters = new ChoDictionary<string, IChoLogFormatter>();
                    ChoObjConfigurable.Load<IChoLogFormatter>(typeof(ChoLogFormatterSettings).Name, _buildInLogFormatterType, _defaultLogFormatters, null);
                }

                return _defaultLogFormatters;
            }
        }

        #endregion

        #region IChoObjectInitializable Members

        public void Initialize(bool beforeInit)
        {
            //Create the default/built-in objects
            _logFormatters = new ChoDictionary<string, IChoLogFormatter>(DefaultLogFormatters);

            ChoObjConfigurable.Load<IChoLogFormatter>(typeof(ChoLogFormatterSettings).Name, ChoType.GetTypes(typeof(ChoLogFormatterAttribute)),
                _logFormatters, LogFormatterTypes);
        }

        #endregion
    }
}
