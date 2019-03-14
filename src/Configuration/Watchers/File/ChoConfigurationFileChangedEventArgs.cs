namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.IO;
	using System.Text;
	using System.Collections.Generic;

	using Cinchoo.Core.Properties;
	using System.Diagnostics;

	#endregion NameSpaces

	public enum ChoConfigurationChangeAction { Changed, Created, Deleted };

	[Serializable]
	public class ChoConfigurationFileChangedEventArgs : ChoConfigurationChangedEventArgs
	{
		private readonly string _configurationFile;
		private readonly ChoConfigurationChangeAction _configurationChangeAction;

		/// <summary>
		/// <para>Initialize a new instance of the <see cref="ConfigurationChangingEventArgs"/> class with the configuration file, the section name, the old value, and the new value of the changes.</para>
		/// </summary>
		/// <param name="configurationFile"><para>The configuration file where the change occured.</para></param>
		/// <param name="sectionName"><para>The section name of the changes.</para></param>
		public ChoConfigurationFileChangedEventArgs(string sectionName, string configurationFile, ChoConfigurationChangeAction configurationChangeAction, DateTime lastUpdatedTimeStamp)
            : base(sectionName, lastUpdatedTimeStamp)
		{
			_configurationFile = configurationFile;
			_configurationChangeAction = configurationChangeAction;
		}

		/// <summary>
		/// <para>Gets the configuration file of the data that is changing.</para>
		/// </summary>
		/// <value>
		/// <para>The configuration file of the data that is changing.</para>
		/// </value>
		public string ConfigurationFile
		{
			get { return _configurationFile; }
		}

		#region Object Overrides

		public override string ToString()
		{
			return String.Format("SectionName: {0}, Type: {1}, ConfigFile: {2}, Action: {3}", 
				this.SectionName, typeof(ChoConfigurationFileChangedEventArgs).FullName, this.ConfigurationFile, this.ConfigurationChangeAction);
		}

		public ChoConfigurationChangeAction ConfigurationChangeAction
		{
			get { return _configurationChangeAction; }
		}

		#endregion Object Overrides
	}
}
