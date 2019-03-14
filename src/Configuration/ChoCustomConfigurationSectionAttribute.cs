namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.IO;
	using System.Text;
	using System.Reflection;
	using System.Collections;
	using System.Configuration;
	using System.Collections.Generic;
	using System.Collections.Specialized;

	using Cinchoo.Core.Properties;
	using Cinchoo.Core.Exceptions;
	using Cinchoo.Core.Diagnostics;
	using Cinchoo.Core.Configuration;
	using Cinchoo.Core.Collections.Specialized;
	using Cinchoo.Core.Configuration.Sections;
	using Cinchoo.Core.Configuration.Handlers;
	using Cinchoo.Core.IO;
    using Cinchoo.Core.Configuration.MetaData;

	#endregion NameSpaces

	#region ChoCustomConfigurationSectionAttribute Class

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ChoCustomConfigurationSectionAttribute : ChoBaseConfigurationSectionAttribute
	{
		#region Instance Data Members (Private)

		private ChoCustomConfigurationElement _configurationElement;
		private readonly Type _configurationSectionHandlerType;
		private readonly string _parameters;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoCustomConfigurationSectionAttribute(Type configurationSectionHandlerType) : this(configurationSectionHandlerType, null)
		{
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="ChoCustomConfigurationSectionAttribute"/> class with the configuration element name.
		/// </summary>
		/// <param name="configElementPath">The <see cref="string"/>configuration element path.</param>
		public ChoCustomConfigurationSectionAttribute(Type configurationSectionHandlerType, string parameters)
		{
			ChoGuard.ArgumentNotNull(configurationSectionHandlerType, "configurationSectionHandlerType");
			_configurationSectionHandlerType = configurationSectionHandlerType;
			_parameters = parameters;
		}

		#endregion

		#region ChoConfigurationElementAttribute Overrides

		public override ChoBaseConfigurationElement GetMe(Type type)
		{
			ChoGuard.ArgumentNotNull(type, "Type");

			if (_configurationElement == null)
			{
				lock (SyncRoot)
				{
					if (_configurationElement == null)
					{
						object customConfigurationSectionHandler = Activator.CreateInstance(_configurationSectionHandlerType) as IChoCustomConfigurationSectionHandler;
						if (customConfigurationSectionHandler == null)
							throw new ChoConfigurationConstructionException(String.Format("Missing {0} custom configuration section handler.", _configurationSectionHandlerType.FullName));
						if (!(customConfigurationSectionHandler is IChoCustomConfigurationSectionHandler))
							throw new ChoConfigurationConstructionException(String.Format("Mismatched {0} custom configuration section handler type specified.", _configurationSectionHandlerType.FullName));

						_configurationElement = new ChoCustomConfigurationElement(customConfigurationSectionHandler as IChoCustomConfigurationSectionHandler,
							_parameters);
                        _configurationElement.DefaultConfigSectionHandlerType = ConfigSectionHandlerType;
                        _configurationElement.ConfigFilePath = ConfigFilePath;

                        ChoStandardConfigurationMetaDataInfo standardConfigurationMetaDataInfo = new ChoStandardConfigurationMetaDataInfo();
                        standardConfigurationMetaDataInfo.BindingMode = BindingMode;
                        if (ConfigStorageType != null)
                            standardConfigurationMetaDataInfo.ConfigStorageType = ConfigStorageType.AssemblyQualifiedName;
                        standardConfigurationMetaDataInfo.ConfigurationMetaDataLogInfo = new ChoConfigurationMetaDataLogInfo();
                        standardConfigurationMetaDataInfo.ConfigurationMetaDataLogInfo.LogCondition = LogCondition;
                        standardConfigurationMetaDataInfo.ConfigurationMetaDataLogInfo.LogTimeStampFormat = LogTimeStampFormat;
                        standardConfigurationMetaDataInfo.ConfigurationMetaDataLogInfo.LogDirectory = LogDirectory;
                        standardConfigurationMetaDataInfo.ConfigurationMetaDataLogInfo.LogFileName = LogFileName.IsNullOrEmpty() ? ChoPath.AddExtension(type.FullName, ChoReservedFileExt.Log) : LogFileName;
                        standardConfigurationMetaDataInfo.Defaultable = Defaultable;
                        _configurationElement.MetaDataInfo = standardConfigurationMetaDataInfo;

                        LoadParameters(_configurationElement);
                    }
				}
			}

			return _configurationElement;
		}

		#endregion ChoConfigurationElementAttribute Overrides

		public override MarshalByRefObject CreateInstance(Type proxiedType)
		{
			throw new NotImplementedException();
		}

		public override string GetConfigXml()
		{
			throw new NotImplementedException();
		}
	}

	#endregion ChoCustomConfigurationSectionAttribute Class
}
