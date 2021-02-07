namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using Cinchoo.Core.Services;

	#endregion NameSpaces

	public class ChoCustomConfigurationElement : ChoBaseConfigurationElement
	{
		#region Instance Data Members (Private)

		private readonly IChoCustomConfigurationSectionHandler _configurationSectionHandler;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoCustomConfigurationElement(IChoCustomConfigurationSectionHandler configurationSectionHandler, string parameters)
			: base("UNKNOWN")
		{
			ChoGuard.ArgumentNotNull(configurationSectionHandler, "configurationSectionHandler");

			_configurationSectionHandler = configurationSectionHandler;
			_configurationSectionHandler.Parameters = parameters;
		}

		#endregion Constructors

		#region ChoConfigurationElement Members

		public override void GetConfig(Type configObjectType, bool refresh)
		{
			ConfigSection = _configurationSectionHandler.Create(configObjectType, null, null, this) as ChoConfigSection;
		}

		public override void PersistConfig(ChoDictionaryService<string, object> stateInfo)
		{
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
            ConfigFilePath = standardConfigurationMetaDataInfo.ConfigFilePath;
            LoadParameters(standardConfigurationMetaDataInfo.Parameters);

            if (standardConfigurationMetaDataInfo.ConfigurationMetaDataLogInfo != null)
			{
                LogCondition = standardConfigurationMetaDataInfo.ConfigurationMetaDataLogInfo.XmlLogCondition;
				LogTimeStampFormat = standardConfigurationMetaDataInfo.ConfigurationMetaDataLogInfo.LogTimeStampFormat;
				LogDirectory = standardConfigurationMetaDataInfo.ConfigurationMetaDataLogInfo.LogDirectory;
				LogFileName = standardConfigurationMetaDataInfo.ConfigurationMetaDataLogInfo.LogFileName;
			}
		}
	}
}
