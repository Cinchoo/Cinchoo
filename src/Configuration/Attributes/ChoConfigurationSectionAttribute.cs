namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Runtime.Remoting.Proxies;
	using Cinchoo.Core.IO;
	using Cinchoo.Core.Runtime.Remoting;
	using Cinchoo.Core.Services;

	#endregion NameSpaces

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class ChoConfigurationSectionAttribute : ChoLoggableProxyAttribute, IDisposable
	{
		#region Shared Data Members (Private)

		private readonly static ChoDictionaryService<Type, RealProxy> _dictService = new ChoDictionaryService<Type, RealProxy>(typeof(ChoConfigurationSectionAttribute).Name);

		#endregion Shared Data Members (Private)

		#region Instance Data Members (Private)

		private ChoConfigurationElement _configurationElement;

		#endregion Instance Data Members (Private)

		#region Instance Properties

		private string _configElementPath;
		public virtual string ConfigElementPath
		{
			get { return _configElementPath; }
			protected set
			{
				if (String.IsNullOrEmpty(value))
					throw new NullReferenceException("ConfigElementPath");

				_configElementPath = value;
			}
		}

		private Type _configSectionHandlerType = typeof(ChoNameValueSectionHandler);
		public virtual Type ConfigSectionHandlerType
		{
			get { return _configSectionHandlerType; }
			protected set { _configSectionHandlerType = value; }
		}

		public virtual ChoConfigurationBindingMode BindingMode
		{
			get;
			set;
		}

		private bool _silent = true;
		public virtual bool Silent
		{
			get { return _silent; }
			set { _silent = value; }
		}

		private bool _defaultable = true;
		public virtual bool Defaultable
		{
			get { return _defaultable; }
			set { _defaultable = value; }
		}

        private bool _ignoreCase = true;
        public virtual bool IgnoreCase
        {
            get { return _ignoreCase; }
            set { _ignoreCase = value; }
        }

		public virtual Type ConfigStorageType
		{
			get;
			set;
		}

		public virtual Type ConfigFileNameFromTypeName
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				if (value == null)
					return;

				ConfigFilePath = value.Name;
			}
		}

		public virtual Type ConfigFileNameFromTypeFullName
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				if (value == null)
					return;

				ConfigFilePath = value.FullName;
			}
		}

		public virtual string ConfigFilePath
		{
			get;
			set;
		}

		public string Parameters
		{
			get;
			set;
		}

		#endregion Instance Properties

		#region Constructors

		/// <summary>
		/// Initialize a new instance of the <see cref="ChoConfigurationSectionAttribute"/> class with the configuration element name.
		/// </summary>
		/// <param name="configElementPath">The <see cref="string"/> configuration element name.</param>
		public ChoConfigurationSectionAttribute(string configElementPath)
		{
			ConfigElementPath = configElementPath;
		}

		public ChoConfigurationSectionAttribute(string configElementPath, Type configSectionHandlerType)
		{
			ChoGuard.ArgumentNotNull(configSectionHandlerType, "ConfigSectionHandlerType");

			ConfigElementPath = configElementPath;
			ConfigSectionHandlerType = configSectionHandlerType;
		}

		#endregion

		#region ChoConfigurationElementAttribute Overrides

		internal ChoBaseConfigurationElement GetMe(Type type)
		{
			ChoGuard.ArgumentNotNull(type, "Type");

			if (_configurationElement == null)
			{
				lock (SyncRoot)
				{
					if (_configurationElement == null)
					{
						_configurationElement = new ChoConfigurationElement(ConfigElementPath);
						InitializeConfigElement(_configurationElement, type);
						Initialize(_configurationElement);
					}
				}
			}

			return _configurationElement;
		}
		
		protected virtual void Initialize(ChoBaseConfigurationElement configElement)
		{
		}

		public override MarshalByRefObject CreateInstance(Type configObjType)
		{
			if (_dictService.ContainsKey(configObjType))
				return (MarshalByRefObject)_dictService.GetValue(configObjType).GetTransparentProxy();

			lock (_dictService.SyncRoot)
			{
				if (_dictService.ContainsKey(configObjType))
					return (MarshalByRefObject)_dictService.GetValue(configObjType).GetTransparentProxy();
				else
				{
					RealProxy proxy = new ChoStandardConfigurationObjectProxy(base.CreateInstance(configObjType), configObjType);
					_dictService.SetValue(configObjType, proxy);
					return (MarshalByRefObject)proxy.GetTransparentProxy();
				}
			}
		}

		#endregion ChoConfigurationElementAttribute Overrides

		#region Instance Members (Private)

		private void InitializeConfigElement(ChoBaseConfigurationElement configElement, Type type)
		{
			if (configElement == null)
				return;

			configElement.DefaultConfigSectionHandlerType = ConfigSectionHandlerType;
			configElement.ConfigFilePath = ConfigFilePath;

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
            standardConfigurationMetaDataInfo.IgnoreCase = IgnoreCase;
            standardConfigurationMetaDataInfo.Silent = Silent;
			configElement.MetaDataInfo = standardConfigurationMetaDataInfo;

			LoadParameters(configElement);
			ChoConfigurationMetaDataManager.SetDefaultMetaDataInfo(configElement);
		}

		private void LoadParameters(ChoBaseConfigurationElement configurationElement)
		{
			if (configurationElement == null)
				return;

			if (Parameters.IsNullOrEmpty())
				return;

			Parameters = Parameters.ExpandProperties();
			foreach (Tuple<string, string> keyValue in Parameters.ToKeyValuePairs())
			{
				configurationElement[keyValue.Item1] = keyValue.Item2;
			}
		}

		#endregion Instance Members (Private)

		#region IDisposable Members

		protected virtual void Dispose(bool finalize)
		{
		}

		public virtual void Dispose()
		{
			Dispose(false);
		}

		#endregion

		#region Finalizer

		~ChoConfigurationSectionAttribute()
		{
			Dispose(true);
		}

		#endregion Finalizer
}
}
