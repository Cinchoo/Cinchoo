namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using Cinchoo.Core.Configuration;

	#endregion NameSpaces

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ChoStandardAppSettingsConfigurationSectionAttribute : ChoConfigurationSectionAttribute
	{
		#region Constructors

		/// <summary>
		/// Initialize a new instance of the <see cref="ChoConfigurationSectionAttribute"/> class with the configuration element name.
		/// </summary>
		/// <param name="configElementPath">The <see cref="string"/> configuration element name.</param>
		public ChoStandardAppSettingsConfigurationSectionAttribute()
			: base("appSettings")
		{
		}

		#endregion

		#region Instance Properties (Public)

		private Type _configSectionHandlerType = typeof(ChoNameValueSectionHandler);
		public override Type ConfigSectionHandlerType
		{
			get { return _configSectionHandlerType; }
			protected set { _configSectionHandlerType = value; }
		}

		private Type _configStorageType = typeof(ChoStandardAppSettingsConfigStorage);
		public override Type ConfigStorageType
		{
			get { return _configStorageType; }
			set { throw new NotSupportedException(); }
		}

		#endregion Instance Properties (Public)
	}
}
