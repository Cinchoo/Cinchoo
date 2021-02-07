namespace Cinchoo.Core.Ini
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Cinchoo.Core.Configuration;
    using System.Xml.Serialization;
    using Cinchoo.Core.Text;

    #endregion NameSpaces

    [XmlRoot("iniSettings")]
    public class ChoIniSettings : IChoInitializable
    {
        #region Shared Data Members (Private)

        private static readonly object _padLock = new object();
        private static ChoIniSettings _instance;

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Public)

        [XmlAttribute("nameValueSeparator")]
        public string NameValueSeparatorText = "=";

        [XmlIgnore]
        public char NameValueSeparator = '=';

        [XmlAttribute("commentChars")]
        public string CommentChars = ";#";

        [XmlAttribute("ignoreValueWhiteSpaces")]
        public bool IgnoreValueWhiteSpaces = false;

        #endregion Instance Data Members (Public)

        #region Object Overrides

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("INI Settings");

            msg.AppendFormatLine("NameValueDelimiter: {0}", NameValueSeparator);
            msg.AppendFormatLine("CommantChars: {0}", CommentChars);
            msg.AppendFormatLine("IgnoreValueWhiteSpaces: {0}", IgnoreValueWhiteSpaces);

            return msg.ToString();
        }

        #endregion Object Overrides

        #region Factory Methods

        public static ChoIniSettings Me
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_padLock)
                {
                    if (_instance == null)
                        _instance = ChoCoreFrxConfigurationManager.Register<ChoIniSettings>();
                }

                return _instance;
            }
        }

        #endregion Factory Methods

        #region IChoInitializable Members

        public void Initialize()
        {
            NameValueSeparatorText = NameValueSeparatorText.NTrim();
            if (NameValueSeparatorText != null)
                NameValueSeparator = NameValueSeparatorText[0];
        }

        #endregion
    }
}
