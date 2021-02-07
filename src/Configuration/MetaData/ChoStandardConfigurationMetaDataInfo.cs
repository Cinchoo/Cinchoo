namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

    using System.Xml.Serialization;
    using System;
    using Cinchoo.Core.Diagnostics;

    #endregion NameSpaces

    [ChoTypeFormatter("Log Information")]
    public class ChoConfigurationMetaDataLogInfo : ChoEquatableObject<ChoConfigurationMetaDataLogInfo>, ICloneable<ChoConfigurationMetaDataLogInfo>
	{
        [XmlElement("condition")]
        public ChoNullable<bool> LogCondition;

        [ChoIgnoreMemberFormatter]
        [XmlIgnore]
        public bool XmlLogCondition
        {
            get { return !LogCondition.HasValue ? ChoTraceSwitch.Switch.TraceVerbose : LogCondition.Value; }
            set { LogCondition = value; }
        }

		[XmlAttribute("timeStampFormat")]
		public string LogTimeStampFormat;

		[XmlAttribute("directory")]
		public string LogDirectory;

		[XmlAttribute("fileName")]
		public string LogFileName;

		internal void Initialize()
		{
            //LogDirectory = ChoString.ExpandProperties(LogDirectory);
            //LogFileName = ChoString.ExpandProperties(LogFileName);
		}

        #region IEquatable<ChoConfigurationMetaDataLogInfo> Members

        public override bool Equals(ChoConfigurationMetaDataLogInfo other)
        {
            if (object.ReferenceEquals(other, null))
                return false;

            if (LogCondition != other.LogCondition)
                return false;

            if (LogTimeStampFormat != other.LogTimeStampFormat)
                return false;

            if (LogDirectory != other.LogDirectory)
                return false;

            if (LogFileName != other.LogFileName)
                return false;

            return true;
        }

        #endregion

        public ChoConfigurationMetaDataLogInfo Clone()
        {
            ChoConfigurationMetaDataLogInfo info = new ChoConfigurationMetaDataLogInfo();
            info.LogCondition = LogCondition;
            info.LogTimeStampFormat = LogTimeStampFormat;
            info.LogDirectory = LogDirectory;
            info.LogFileName = LogFileName;

            return info;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }

	[ChoTypeFormatter("MetaData Information")]
	[XmlRoot("metaData")]
	public class ChoStandardConfigurationMetaDataInfo : ChoBaseConfigurationMetaDataInfo
	{
		[XmlAttribute("bindingMode")]
		public ChoConfigurationBindingMode BindingMode;

        [XmlAttribute("defaultable")]
        public bool Defaultable = true;

		[XmlAttribute("silent")]
		public bool Silent = true;

        [XmlAttribute("ignoreCase")]
        public bool IgnoreCase = true;

		[XmlElement("logInfo")]
		public ChoConfigurationMetaDataLogInfo ConfigurationMetaDataLogInfo;

        [XmlElement("parameters")]
        public string Parameters;

        [XmlElement("configFilePath")]
        public string ConfigFilePath;

		protected override void Initialize()
		{
			if (ConfigurationMetaDataLogInfo != null)
				ConfigurationMetaDataLogInfo.Initialize();
		}

        #region IEquatable<ChoStandardConfigurationMetaDataInfo> Members

        public override bool Equals(ChoBaseConfigurationMetaDataInfo obj)
        {
            if (object.ReferenceEquals(obj, null))
                return false;

            if (!(obj is ChoStandardConfigurationMetaDataInfo))
                return false;

            if (!base.Equals(obj))
                return false;

            ChoStandardConfigurationMetaDataInfo other = obj as ChoStandardConfigurationMetaDataInfo;
            if (other != null)
            {
                if (BindingMode != other.BindingMode)
                    return false;

                if (Defaultable != other.Defaultable)
                    return false;

                if (IgnoreCase != other.IgnoreCase)
                    return false;

                if (Silent != other.Silent)
                    return false;

                if (Parameters != other.Parameters)
                    return false;

                if (ConfigFilePath != other.ConfigFilePath)
                    return false;

                if (!ChoObject.Equals<ChoConfigurationMetaDataLogInfo>(ConfigurationMetaDataLogInfo, other.ConfigurationMetaDataLogInfo))
                    return false;
            }

            return true;
        }

        public override void Merge(ChoBaseConfigurationMetaDataInfo source)
        {
            base.Merge(source);

            ChoStandardConfigurationMetaDataInfo other = source as ChoStandardConfigurationMetaDataInfo;
            if (other != null)
            {
                ConfigurationMetaDataLogInfo = ChoObject.Merge(ConfigurationMetaDataLogInfo, other.ConfigurationMetaDataLogInfo);
            }

        }

        #endregion

        public override ChoBaseConfigurationMetaDataInfo Clone()
        {
            ChoStandardConfigurationMetaDataInfo info = new ChoStandardConfigurationMetaDataInfo();

            info.ConfigStorageType = ConfigStorageType;
            info.BindingMode = BindingMode;
            info.Defaultable = Defaultable;
            info.Silent = Silent;
            info.IgnoreCase = IgnoreCase;
            info.ConfigurationMetaDataLogInfo = ConfigurationMetaDataLogInfo != null ? ConfigurationMetaDataLogInfo.Clone() : null;
            info.Parameters = Parameters;
            info.ConfigFilePath = ConfigFilePath;

            return info;
        }
    }
}
