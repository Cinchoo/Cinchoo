namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.IO;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using Cinchoo.Core.Properties;
	using Cinchoo.Core.Services;

	#endregion NameSpaces

	public class ChoConfigurationChangeCompositeWatcher : IChoConfigurationChangeWatcher
	{
		#region Instance Data Members (Private)

		private readonly string _threadName;
		private readonly string _configurationSectionName;
		private List<IChoConfigurationChangeWatcher> _configurationChangeWatchers = new List<IChoConfigurationChangeWatcher>();

		#endregion Instance Data Members (Private)

		#region Constructors

		/// <summary>
		/// <para>Initialize a new <see cref="ChoConfigurationChangeCompositeWatcher"/> class with the path to the configuration file and the name of the section</para>
		/// </summary>
		/// <param name="_configFilePath">
		/// <para>The full path to the configuration file.</para>
		/// </param>
		/// <param name="_configurationSectionName">
		/// <para>The name of the configuration section to watch.</para>
		/// </param>
		public ChoConfigurationChangeCompositeWatcher(string configurationSectionName, IChoConfigurationChangeWatcher[] configurationChangeWatchers)
		{
			ChoGuard.ArgumentNotNullOrEmpty(configurationChangeWatchers, "ConfigurationChangeWatchers");
			ChoGuard.ArgumentNotNullOrEmpty(configurationSectionName, "ConfigurationSectionName");

			foreach (IChoConfigurationChangeWatcher  configurationChangeWatcher in configurationChangeWatchers)
			{
				if (configurationChangeWatcher == null)
					continue;

				_configurationChangeWatchers.Add(configurationChangeWatcher);
			}

			_configurationSectionName = configurationSectionName;
			_threadName = "{0} : {1}".FormatString(typeof(ChoConfigurationChangeCompositeWatcher).Name, _configurationSectionName);
		}

		#endregion Constructors

		#region Destructors

		/// <summary>
		/// <para>Allows an <see cref="ChoConfigurationChangeCompositeWatcher"/> to attempt to free resources and perform other cleanup operations before the <see cref="ChoConfigurationChangeCompositeWatcher"/> is reclaimed by garbage collection.</para>
		/// </summary>
		~ChoConfigurationChangeCompositeWatcher()
		{
			Disposing(false);
		}

		#endregion Destructors

		#region ChoConfigurationChangeWatcher Overrides

		/// <summary>
		/// <para>Releases the unmanaged resources used by the <see cref="ConfigurationChangeFileWatcher"/> and optionally releases the managed resources.</para>
		/// </summary>
		public void Dispose()
		{
			Disposing(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// <para>Releases the unmanaged resources used by the <see cref="Cinchoo.Core.Configuration.ConfigurationChangeFileWatcher"/> and optionally releases the managed resources.</para>
		/// </summary>
		/// <param name="isDisposing">
		/// <para><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</para>
		/// </param>
		protected virtual void Disposing(bool isDisposing)
		{
            StopWatching();

            List<IChoConfigurationChangeWatcher> configurationChangeWatchers = _configurationChangeWatchers;
            _configurationChangeWatchers = null;
            if (configurationChangeWatchers != null)
                configurationChangeWatchers.Clear();
		}

		#endregion ChoConfigurationChangeWatcher Overrides

		#region IChoConfigurationChangeWatcher Members

		public void SetConfigurationChangedEventHandler(object key, ChoConfigurationChangedEventHandler ConfigurationChanged)
		{
			List<IChoConfigurationChangeWatcher> configurationChangeWatchers = _configurationChangeWatchers;
			if (configurationChangeWatchers == null || configurationChangeWatchers.Count == 0)
				return;
			
			foreach (IChoConfigurationChangeWatcher configurationChangeWatcher in configurationChangeWatchers)
			{
				configurationChangeWatcher.SetConfigurationChangedEventHandler(key, ConfigurationChanged);
			}
		}

		public void StartWatching()
		{
			List<IChoConfigurationChangeWatcher> configurationChangeWatchers = _configurationChangeWatchers;
			if (configurationChangeWatchers == null || configurationChangeWatchers.Count == 0)
				return;

			foreach (IChoConfigurationChangeWatcher configurationChangeWatcher in configurationChangeWatchers)
			{
				configurationChangeWatcher.StartWatching();
			}
		}

		public void StopWatching()
		{
			List<IChoConfigurationChangeWatcher> configurationChangeWatchers = _configurationChangeWatchers;
			if (configurationChangeWatchers == null || configurationChangeWatchers.Count == 0)
				return;

			foreach (IChoConfigurationChangeWatcher configurationChangeWatcher in configurationChangeWatchers)
			{
				configurationChangeWatcher.StopWatching();
			}
		}

		public void RestartWatching()
		{
			List<IChoConfigurationChangeWatcher> configurationChangeWatchers = _configurationChangeWatchers;
			if (configurationChangeWatchers == null || configurationChangeWatchers.Count == 0)
				return;

			foreach (IChoConfigurationChangeWatcher configurationChangeWatcher in configurationChangeWatchers)
			{
				configurationChangeWatcher.RestartWatching();
			}
		}

		public void ResetWatching()
		{
			List<IChoConfigurationChangeWatcher> configurationChangeWatchers = _configurationChangeWatchers;
			if (configurationChangeWatchers == null || configurationChangeWatchers.Count == 0)
				return;

			foreach (IChoConfigurationChangeWatcher configurationChangeWatcher in configurationChangeWatchers)
			{
				configurationChangeWatcher.ResetWatching();
			}
		}

		public void OnConfigurationChanged()
		{
			List<IChoConfigurationChangeWatcher> configurationChangeWatchers = _configurationChangeWatchers;
			if (configurationChangeWatchers == null || configurationChangeWatchers.Count == 0)
				return;

			foreach (IChoConfigurationChangeWatcher configurationChangeWatcher in configurationChangeWatchers)
			{
				configurationChangeWatcher.OnConfigurationChanged();
				break;
			}
		}

		public ChoConfigurationChangedEventArgs EventData
		{
			get { throw new NotSupportedException(); }
		}

		public string SectionName
		{
			get { return _configurationSectionName; }
		}

		#endregion
	}
}
