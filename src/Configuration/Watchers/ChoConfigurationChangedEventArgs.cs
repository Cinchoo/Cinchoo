namespace Cinchoo.Core.Configuration
{
	#region NameSpaces
	
	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion NameSpaces

	/// <summary>
	/// Event handler called after a configuration has changed.
	/// </summary>
	/// <param name="sender">
	/// <para>The Configuration of the event.</para>
	/// </param>
	/// <param name="e">
	/// <para>A <see cref="ConfigurationChangedEventArgs"/> that contains the event data.</para>
	/// </param>
	public delegate void ChoConfigurationChangedEventHandler(object sender, ChoConfigurationChangedEventArgs e);

	/// <summary>
	/// </summary>
	[Serializable]
	public class ChoConfigurationChangedEventArgs : EventArgs
	{
		private readonly string _configurationSectionName;
        private readonly DateTime _lastUpdatedTimeStamp;

		/// <summary>
		/// <para>Initialize a new instance of the <see cref="ConfigurationChangingEventArgs"/> class with the section name</para>
		/// </summary
		/// <param name="configurationSectionName"><para>The section name of the changes.</para></param>
        public ChoConfigurationChangedEventArgs(string configurationSectionName, DateTime lastUpdatedTimeStamp)
		{
			this._configurationSectionName = configurationSectionName;
            _lastUpdatedTimeStamp = lastUpdatedTimeStamp;
		}

		/// <summary>
		/// <para>Gets the section name where the changes occurred.</para>
		/// </summary>
		/// <value>
		/// <para>The section name where the changes occurred.</para>
		/// </value>
		public string SectionName
		{
			get { return _configurationSectionName; }
		}

        public DateTime LastUpdatedTimeStamp
        {
            get { return _lastUpdatedTimeStamp; }
        }
    }
}
