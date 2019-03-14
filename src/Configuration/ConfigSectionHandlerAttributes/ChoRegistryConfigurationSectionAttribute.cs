namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;

	#endregion NameSpaces

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ChoRegistryConfigurationSectionAttribute : ChoConfigurationSectionAttribute
	{
		#region Constants

		public const string REG_KEY = "RegistryKey";
		
		#endregion Constants

		#region Instance Data Members (Private)

		private readonly string _registryKey;

		#endregion Instance Data Members (Private)
		
		#region Constructors

		/// <summary>
		/// Initialize a new instance of the <see cref="ChoConfigurationSectionAttribute"/> class with the configuration element name.
		/// </summary>
		/// <param name="configElementPath">The <see cref="string"/> configuration element name.</param>
		public ChoRegistryConfigurationSectionAttribute(string configElementPath, string registryKey)
			: base(configElementPath)
		{
			ChoGuard.NotNullOrEmpty(registryKey, REG_KEY);

			_registryKey = registryKey;
		}

		#endregion

		#region Instance Properties (Public)

		private Type _configSectionHandlerType = typeof(ChoDictionarySectionHandler);
		public override Type ConfigSectionHandlerType
		{
			get { return _configSectionHandlerType; }
			protected set { _configSectionHandlerType = value; }
		}

		private Type _configStorageType = typeof(ChoRegistryConfigStorage);
		public override Type ConfigStorageType
		{
			get { return _configStorageType; }
			set { throw new NotSupportedException(); }
		}

		#endregion Instance Properties (Public)

		protected override void Initialize(ChoBaseConfigurationElement configElement)
		{
			base.Initialize(configElement);
			configElement.StateInfo[REG_KEY] = _registryKey;
		}
	}
}
