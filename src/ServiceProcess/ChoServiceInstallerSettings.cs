namespace Cinchoo.Core.ServiceProcess
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Serialization;
    using System.ServiceProcess;
    using Cinchoo.Core.Text;
    using Cinchoo.Core.Configuration;
    using System.IO;
    using Cinchoo.Core.IO;

    #endregion NameSpaces

    [XmlRoot("serviceInstallerSettings")]
    public class ChoServiceInstallerSettings
    {
        #region Shared Data Members (Private)

        private static readonly object _padLock = new object();
        private static ChoServiceInstallerSettings _instance;

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Public)

        [XmlAttribute("delayedAutoStart")]
        public bool DelayedAutoStart = false;

        [XmlElement("description")]
        public string Description = String.Empty;

        [XmlAttribute("displayName")]
        public string DisplayName = String.Empty;

        [XmlAttribute("serviceName")]
        public string ServiceName = String.Empty;

        [XmlAttribute("serviceStartMode")]
        public ServiceStartMode ServiceStartMode = ServiceStartMode.Automatic;

        #endregion

        #region Object Overrides

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("{0} Settings", GetType().Name);

            msg.AppendFormatLine("DelayedAutoStart: {0}", DelayedAutoStart);
            msg.AppendFormatLine("Description: {0}", Description);
            msg.AppendFormatLine("DisplayName: {0}", DisplayName);
            msg.AppendFormatLine("ServiceName: {0}", ServiceName);
            msg.AppendFormatLine("ServiceStartMode: {0}", ServiceStartMode);

            return msg.ToString();
        }

        #endregion Object Overrides

        #region Factory Methods

        public static ChoServiceInstallerSettings Me
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_padLock)
                {
                    if (_instance == null)
                        _instance = ChoCoreFrxConfigurationManager.Register<ChoServiceInstallerSettings>();
                }

                return _instance;
            }
        }

        #endregion Factory Methods
    }
}
