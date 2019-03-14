namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;

	#endregion NameSpaces

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ChoIniConfigurationSectionAttribute : ChoConfigurationSectionAttribute
	{
		#region Constants

		public const string INI_SECTION_NAME = "IniSectionName";
		public const string INI_FILE_PATH = "IniFilePath";
		
		#endregion Constants

		#region Instance Data Members (Private)

		private readonly string _iniSectionName;
		private readonly string _iniFilePath;

		#endregion Instance Data Members (Private)
		
		#region Constructors

		/// <summary>
		/// Initialize a new instance of the <see cref="ChoConfigurationSectionAttribute"/> class with the configuration element name.
		/// </summary>
		/// <param name="configElementPath">The <see cref="string"/> configuration element name.</param>
		public ChoIniConfigurationSectionAttribute(string configElementPath, string iniSectionName, string iniFilePath)
			: base(configElementPath)
		{
			ChoGuard.NotNullOrEmpty(iniSectionName, INI_SECTION_NAME);
			ChoGuard.NotNullOrEmpty(iniFilePath, INI_FILE_PATH);

			_iniSectionName = iniSectionName;
			_iniFilePath = iniFilePath;
		}

		#endregion

		#region Instance Properties (Public)

		private Type _configSectionHandlerType = typeof(ChoNameValueSectionHandler);
		public override Type ConfigSectionHandlerType
		{
			get { return _configSectionHandlerType; }
			protected set { _configSectionHandlerType = value; }
		}

		private Type _configStorageType = typeof(ChoIniConfigStorage);
		public override Type ConfigStorageType
		{
			get { return _configStorageType; }
			set { throw new NotSupportedException(); }
		}

		#endregion Instance Properties (Public)

		protected override void Initialize(ChoBaseConfigurationElement configElement)
		{
			base.Initialize(configElement);

			configElement[INI_SECTION_NAME] = _iniSectionName;
			configElement[INI_FILE_PATH] = _iniFilePath;
		}
	}
}
