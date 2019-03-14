namespace Cinchoo.Core.Shell
{
	#region NameSpaces

    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.IO;
    using System.IO;
    using System.Xml.Serialization;
    using Cinchoo.Core.Text;

    #endregion NameSpaces

    [XmlRoot("commandLineParserSettings")]
    public class ChoCommandLineParserSettings
	{
        #region Shared Data Members (Private)

        private static readonly object _padLock = new object();
        private static ChoCommandLineParserSettings _instance;

        #endregion Shared Data Members (Private)
        
        #region Instance Data Members (Public)

        //[ChoPropertyInfo("switchChars")]
        //[ChoTypeConverter(typeof(ChoCharArrayToStringConverter))]
        //[ChoMemberFormatter("SwitchChars", Formatter = typeof(ChoArrayToStringFormatter))]
        //[ChoEqualityComparer(typeof(ChoCharArrayEqualityComparer))]
        [XmlAttribute("switchChars")]
		public char[] SwitchChars = new char[] { '/', '-'};

        //[ChoPropertyInfo("valueSeperators")]
        //[ChoTypeConverter(typeof(ChoCharArrayToStringConverter))]
        //[ChoMemberFormatter("ValueSeperators", Formatter = typeof(ChoArrayToStringFormatter))]
        //[ChoEqualityComparer(typeof(ChoCharArrayEqualityComparer))]
        [XmlAttribute("valueSeperators")]
        public char[] ValueSeperators = new char[] { ':', '=' };

        //[ChoPropertyInfo("usageSwitches")]
        //[ChoTypeConverter(typeof(ChoArrayToStringConverter))]
        //[ChoMemberFormatter("UsageSwitches", Formatter = typeof(ChoArrayToStringFormatter))]
        //[ChoEqualityComparer(typeof(ChoStringArrayEqualityComparer))]
        [XmlAttribute("usageSwitches")]
        public string[] UsageSwitches = new string[] { "?", "h", "help" };

        //[ChoPropertyInfo("fileArgSwitches")]
        //[ChoTypeConverter(typeof(ChoArrayToStringConverter))]
        //[ChoMemberFormatter("FileArgSwitches", Formatter = typeof(ChoArrayToStringFormatter))]
        //[ChoEqualityComparer(typeof(ChoStringArrayEqualityComparer))]
        [XmlAttribute("fileArgSwitches")]
        public string[] FileArgSwitches = new string[] { "@" };

        //[ChoPropertyInfo("ignoreCase", DefaultValue = true)]
        [XmlAttribute("ignoreCase")]
        public bool IgnoreCase = true;

		#endregion

        #region Object Overrides

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("CommandLine Parser Settings");

            msg.AppendFormatLine("SwitchChars: {0}", new ChoArrayToStringFormatter().Format(SwitchChars, true));
            msg.AppendFormatLine("valueSeperators: {0}", new ChoArrayToStringFormatter().Format(ValueSeperators, true));
            msg.AppendFormatLine("UsageSwitches: {0}", new ChoArrayToStringFormatter().Format(UsageSwitches, true));
            msg.AppendFormatLine("FileArgSwitches: {0}", new ChoArrayToStringFormatter().Format(FileArgSwitches, true));
            msg.AppendFormatLine("IgnoreCase: {0}", IgnoreCase);

            return msg.ToString();
        }

        #endregion Object Overrides

        #region Factory Methods

        public static ChoCommandLineParserSettings Me
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_padLock)
                {
                    if (_instance == null)
                        _instance = ChoCoreFrxConfigurationManager.Register<ChoCommandLineParserSettings>();
                }

                return _instance;
            }
        }

        #endregion Factory Methods
    }
}
