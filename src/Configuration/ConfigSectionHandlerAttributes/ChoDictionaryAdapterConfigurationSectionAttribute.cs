namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
    using System.Xml.Serialization;

	#endregion NameSpaces

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class ChoDictionaryAdapterConfigurationSectionAttribute : ChoConfigurationSectionAttribute
	{
		#region Constants

		public const string CONFIG_OBJECT_ADAPTER_TYPE = "#ConfigObjectAdapterType#";
        public const string CONFIG_OBJECT_ADAPTER_PARAMS = "#ConfigObjectAdapterParams#";
		
		#endregion Constants

		#region Instance Data Members (Private)

		private readonly string _configObjectAdapterType;
        private readonly string _configObjectAdapterParams;

		#endregion Instance Data Members (Private)
		
		#region Constructors

		/// <summary>
		/// CustomDictionarytialize a new instance of the <see cref="ChoConfigurationSectionAttribute"/> class with the configuration element name.
		/// </summary>
		/// <param name="configElementPath">The <see cref="string"/> configuration element name.</param>
		public ChoDictionaryAdapterConfigurationSectionAttribute(string configElementPath, string configObjectAdapterType, string configObjectAdapterParams = null)
			: base(configElementPath)
		{
			ChoGuard.NotNullOrEmpty(configObjectAdapterType, CONFIG_OBJECT_ADAPTER_TYPE);

			_configObjectAdapterType = configObjectAdapterType;
            _configObjectAdapterParams = configObjectAdapterParams;
		}

        public ChoDictionaryAdapterConfigurationSectionAttribute(string configElementPath, Type configObjectAdapterType, string configObjectAdapterParams = null)
			: base(configElementPath)
		{
			ChoGuard.NotNull(configObjectAdapterType, CONFIG_OBJECT_ADAPTER_TYPE);

            _configObjectAdapterType = configObjectAdapterType.SimpleQualifiedName();
            _configObjectAdapterParams = configObjectAdapterParams;
        }

		#endregion

		#region Instance Properties (Public)

		private Type _configSectionHandlerType = typeof(ChoDictionarySectionHandler);
        [XmlIgnore]
        public override Type ConfigSectionHandlerType
		{
			get { return _configSectionHandlerType; }
			protected set { _configSectionHandlerType = value; }
		}

		private Type _configStorageType = typeof(ChoDictionaryAdapterConfigStorage);
		public override Type ConfigStorageType
		{
			get { return _configStorageType; }
			set { throw new NotSupportedException(); }
		}

		#endregion Instance Properties (Public)

		protected override void Initialize(ChoBaseConfigurationElement configElement)
		{
			base.Initialize(configElement);

			configElement[CONFIG_OBJECT_ADAPTER_TYPE] = _configObjectAdapterType;
            configElement[CONFIG_OBJECT_ADAPTER_PARAMS] = _configObjectAdapterParams;
        }
	}
}
