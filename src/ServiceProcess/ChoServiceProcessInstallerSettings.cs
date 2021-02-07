namespace Cinchoo.Core.ServiceProcess
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Serialization;
    using Cinchoo.Core.Configuration;
    using System.ServiceProcess;
    using Cinchoo.Core.Text;
    using System.IO;
    using Cinchoo.Core.IO;
    using Cinchoo.Core.Security.Cryptography;

    #endregion NameSpaces

    [XmlRoot("serviceProcessInstallerSettings")]
    public class ChoServiceProcessInstallerSettings
    {
        #region Shared Data Members (Private)

        private static readonly object _padLock = new object();
        private static ChoServiceProcessInstallerSettings _instance;

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Public)

        [XmlAttribute("account")]
        public ServiceAccount Account = ServiceAccount.LocalSystem;

        [XmlElement("helpText")]
        public string HelpText = String.Empty;

        [XmlAttribute("userName")]
        public string UserName = String.Empty;

        [XmlAttribute("password")]
        public string Password = String.Empty;

        #endregion

        #region Object Overrides

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("{0} Settings", GetType().Name);

            msg.AppendFormatLine("Account: {0}", Account);
            msg.AppendFormatLine("UserName: {0}", UserName);
            msg.AppendFormatLine("HelpText: {0}", HelpText);

            return msg.ToString();
        }

        #endregion Object Overrides

        #region Factory Methods

        public static ChoServiceProcessInstallerSettings Me
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_padLock)
                {
                    if (_instance == null)
                        _instance = ChoCoreFrxConfigurationManager.Register<ChoServiceProcessInstallerSettings>();
                    if (_instance != null)
                    {
                        if (!_instance.Password.IsNullOrWhiteSpace())
                        {
                            using (ChoAESCryptography crypt = new ChoAESCryptography())
                                _instance.Password = crypt.Decrypt(_instance.Password);
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion Factory Methods
    }
}
