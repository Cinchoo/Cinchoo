namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
    using System.Xml.Serialization;

	#endregion NameSpaces

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class ChoDictionaryConfigurationSectionAttribute : ChoConfigurationSectionAttribute
	{
		#region Constructors

		/// <summary>
		/// Initialize a new instance of the <see cref="ChoConfigurationSectionAttribute"/> class with the configuration element name.
		/// </summary>
		/// <param name="configElementPath">The <see cref="string"/> configuration element name.</param>
		public ChoDictionaryConfigurationSectionAttribute(string configElementPath)
			: base(configElementPath)
		{
		}

		#endregion

		#region Instance Properties (Public)

		private Type _configStorageType = typeof(ChoFileDictionaryConfigStorage);
		public override Type ConfigStorageType
		{
			get { return _configStorageType; }
			set
			{
				if (value != null)
				{
					if (!typeof(IChoDictionaryConfigStorage).IsAssignableFrom(value))
						throw new ChoConfigurationException("Configuration Storage Type should be of IChoDictionaryConfigStorage");
					_configStorageType = value;
				}
			}
		}

		private Type _configSectionHandlerType = typeof(ChoDictionarySectionHandler);
        [XmlIgnore]
        public override Type ConfigSectionHandlerType
		{
			get { return _configSectionHandlerType; }
			protected set { _configSectionHandlerType = value; }
		}

		#endregion Instance Properties (Public)
	}
}
