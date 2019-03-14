namespace Cinchoo.Core.ServiceProcess
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Serialization;
    using Cinchoo.Core.Configuration;

    #endregion NameSpaces

    [XmlRoot("serviceSettings")]
    public class ChoServiceSettings
    {
        #region Shared Data Members (Private)

        private static readonly object _padLock = new object();
        private static ChoServiceSettings _instance;

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Public)

        [XmlAttribute("autoLog")]
        public bool AutoLog = true;

        [XmlAttribute("applicationId")]
        public string ApplicationId;

        [XmlAttribute("eventLogSourceName")]
        public string EventLogSourceName;

        [XmlAttribute("useApplicationDataFolderAsLogFolder")]
        public bool UseApplicationDataFolderAsLogFolder = false;

        [XmlElement("logFolder")]
        public string LogFolder;

        [XmlAttribute("appEnvironment")]
        public string AppEnvironment;

        [XmlElement("sharedEnvironmentConfigFilePath")]
        public string SharedEnvironmentConfigFilePath;

        [XmlElement("appConfigPath")]
        public string ApplicationConfigFilePath;

        [XmlIgnore]
        internal string ApplicationConfigDirectory;

        [XmlAttribute("hostName")]
        public string HostName;

        [XmlElement("logTimeStampFormat")]
        public string LogTimeStampFormat;

        #endregion

        #region Object Overrides

        #region Factory Methods

        public static ChoServiceSettings Me
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_padLock)
                {
                    if (_instance == null)
                    {
                        using (ChoCoreFrxConfigurationManager coreFrxConfigurationManager = new ChoCoreFrxConfigurationManager(typeof(ChoServiceSettings).FullName))
                            _instance = coreFrxConfigurationManager.ToObject<ChoServiceSettings>();

                        if (_instance == null)
                            _instance = new ChoServiceSettings();
                    }
                }

                return _instance;
            }
        }

        #endregion Factory Methods
    }
}
