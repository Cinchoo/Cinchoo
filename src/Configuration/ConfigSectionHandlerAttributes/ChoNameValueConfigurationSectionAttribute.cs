namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ChoNameValueConfigurationSectionAttribute : ChoConfigurationSectionAttribute
	{
        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <see cref="ChoConfigurationSectionAttribute"/> class with the configuration element name.
		/// </summary>
        /// <param name="configElementPath">The <see cref="string"/> configuration element name.</param>
		public ChoNameValueConfigurationSectionAttribute(string configElementPath)
			: base(configElementPath)
		{
        }

        #endregion

		#region Instance Properties (Public)

        private Type _configStorageType = typeof(ChoFileNameValueConfigStorage);
        public override Type ConfigStorageType
        {
            get { return _configStorageType; }
            set
            {
                if (value != null)
                {
                    if (!typeof(IChoNameValueConfigStorage).IsAssignableFrom(value))
                        throw new ChoConfigurationException("Configuration Storage Type should be of IChoNameValueConfigStorare");
                    _configStorageType = value;
                }
            }
        }

        private Type _configSectionHandlerType = typeof(ChoNameValueSectionHandler);
        public override Type ConfigSectionHandlerType
		{
			get { return _configSectionHandlerType; }
			protected set { _configSectionHandlerType = value; }
		}

		#endregion Instance Properties (Public)
	}
}
