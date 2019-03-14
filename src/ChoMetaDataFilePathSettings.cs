namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Serialization;
    using Cinchoo.Core.Text;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.IO;
    using System.IO;

    #endregion NameSpaces

    [XmlRoot("metaDataFilePathSettings")]
    public class ChoMetaDataFilePathSettings : IChoInitializable
    {
        #region Shared Members (Private)

        private static readonly object _padLock = new object();
        private static ChoMetaDataFilePathSettings _instance;

        #endregion Shared Members (Private)

        #region Instance Data Members (Public)

        [XmlElement("configurationMetaDataFilePath")]
        public string ConfigurationMetaDataFilePath = String.Empty;

        [XmlElement("pcMetaDataFilePath")]
        public string PCMetaDataFilePath = String.Empty;

        [XmlElement("etlMetaDataFilePath")]
        public string ETLMetaDataFilePath = String.Empty;

        [XmlIgnore]
        internal string OverridenConfigurationMetaDataFilePath = String.Empty;

        [XmlIgnore]
        internal string OverridenPCMetaDataFilePath = String.Empty;

        [XmlIgnore]
        public string OverridenETLMetaDataFilePath = String.Empty;

        #endregion

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("{0} Settings", GetType().Name);

            msg.AppendFormatLine("ConfigurationMetaDataFilePath: {0}", OverridenConfigurationMetaDataFilePath);
            msg.AppendFormatLine("PCMetaDataFilePath: {0}", OverridenPCMetaDataFilePath);
            msg.AppendFormatLine("ETLMetaDataFilePath: {0}", OverridenETLMetaDataFilePath);
            
            return msg.ToString();
        }

        public static ChoMetaDataFilePathSettings Me
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_padLock)
                {
                    if (_instance == null)
                        _instance = ChoCoreFrxConfigurationManager.Register<ChoMetaDataFilePathSettings>();
                }

                return _instance;
            }
        }

        #region IChoInitializable Members

        public void Initialize()
        {
            if (ConfigurationMetaDataFilePath.IsNullOrWhiteSpace())
                OverridenConfigurationMetaDataFilePath = ChoPath.AddExtension(Path.Combine(Path.GetDirectoryName(ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath), ChoReservedDirectoryName.Meta,
                    Path.GetFileNameWithoutExtension(ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath)), ChoReservedFileExt.MetaData);
            else
                OverridenConfigurationMetaDataFilePath = ConfigurationMetaDataFilePath;

            if (PCMetaDataFilePath.IsNullOrWhiteSpace())
                OverridenPCMetaDataFilePath = ChoPath.AddExtension(Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath), ChoReservedDirectoryName.Meta,
                    Path.GetFileNameWithoutExtension(ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath)), ChoReservedFileExt.Perf),
                    ChoReservedFileExt.MetaData);
            else
                OverridenPCMetaDataFilePath = PCMetaDataFilePath;

            if (ETLMetaDataFilePath.IsNullOrWhiteSpace())
                OverridenETLMetaDataFilePath = ChoPath.AddExtension(Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath), ChoReservedDirectoryName.Meta,
                    Path.GetFileNameWithoutExtension(ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath)), ChoReservedFileExt.ETL),
                    ChoReservedFileExt.MetaData);
            else
                OverridenETLMetaDataFilePath = ETLMetaDataFilePath;

            if (OverridenConfigurationMetaDataFilePath.IsNullOrWhiteSpace())
                throw new ApplicationException("ConfigurationMetaDataFilePath can not be null.");

            if (OverridenPCMetaDataFilePath.IsNullOrWhiteSpace())
                throw new ApplicationException("PCMetaDataFilePath can not be null.");

            if (OverridenETLMetaDataFilePath.IsNullOrWhiteSpace())
                throw new ApplicationException("ETLMetaDataFilePath can not be null.");
        }

        #endregion
    }
}
