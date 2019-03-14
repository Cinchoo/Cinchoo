namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion NameSpaces

	public class ChoConfigurationChangeNullWatcher : IChoConfigurationChangeWatcher
	{
		private string _configurationSectionName;

		public ChoConfigurationChangeNullWatcher()
		{
		}

		public ChoConfigurationChangeNullWatcher(string configurationSectionName)
		{
			_configurationSectionName = configurationSectionName;
		}

		public string SectionName
		{
			get { return _configurationSectionName; }
		}

		protected DateTime GetCurrentLastWriteTime()
		{
			return DateTime.MinValue;
		}

		protected string ThreadName
		{
			get { return "ConfigurationNullWatcherThread : " + _configurationSectionName; }
		}

		public ChoConfigurationChangedEventArgs EventData
		{
			get { return null; }
		}

		#region IChoConfigurationChangeWatcher Members

		public void SetConfigurationChangedEventHandler(object key, ChoConfigurationChangedEventHandler ConfigurationChanged)
		{
		}

		public void StartWatching()
		{
		}

		public void StopWatching()
		{
		}

		public void RestartWatching()
		{
		}

		public void ResetWatching()
		{
		}

		public void OnConfigurationChanged()
		{
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
