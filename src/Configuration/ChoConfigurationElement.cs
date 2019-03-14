namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;

    using Cinchoo.Core.Services;

	#endregion NameSpaces

	public enum ChoConfigurationBindingMode
	{
		TwoWay, OneWay, OneWayToSource, OneTime
	}

	public class ChoConfigurationElement : ChoBaseConfigurationElement
	{
		#region Constructors

		public ChoConfigurationElement(string configElementPath)
			: base(configElementPath)
		{
		}

		#endregion Constructors

		#region ChoConfigurationElement Members

		public override void GetConfig(Type configObjectType, bool refresh)
		{
			ConfigSection = ChoConfigurationManager.GetConfig(configObjectType, ConfigElementPath, DefaultConfigSectionHandlerType, this) as ChoConfigSection;
		}

		public override void PersistConfig(ChoDictionaryService<string, object> stateInfo)
		{
			if (ConfigSection == null) return;

			ConfigSection.Persist(ConfigElementPath, stateInfo);
		}

		#endregion

		protected override void ApplyConfigMetaData(ChoBaseConfigurationMetaDataInfo configurationMetaDataInfo)
		{
			if (configurationMetaDataInfo == null || !(configurationMetaDataInfo is ChoStandardConfigurationMetaDataInfo))
				return;

			ChoStandardConfigurationMetaDataInfo standardConfigurationMetaDataInfo = configurationMetaDataInfo as ChoStandardConfigurationMetaDataInfo;
			BindingMode = standardConfigurationMetaDataInfo.BindingMode;
			Silent = standardConfigurationMetaDataInfo.Silent;
            Defaultable = standardConfigurationMetaDataInfo.Defaultable;
            IgnoreCase = standardConfigurationMetaDataInfo.IgnoreCase;
            if (standardConfigurationMetaDataInfo.ConfigurationMetaDataLogInfo != null)
			{
				LogCondition = standardConfigurationMetaDataInfo.ConfigurationMetaDataLogInfo.LogCondition;
				LogTimeStampFormat = standardConfigurationMetaDataInfo.ConfigurationMetaDataLogInfo.LogTimeStampFormat;
				LogDirectory = standardConfigurationMetaDataInfo.ConfigurationMetaDataLogInfo.LogDirectory;
				LogFileName = standardConfigurationMetaDataInfo.ConfigurationMetaDataLogInfo.LogFileName;
			}
		}
	}
}
