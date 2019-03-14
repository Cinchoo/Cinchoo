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
	using Cinchoo.Core.Collections;

	#endregion NameSpaces

	public class ChoConfigurationChangeCompositeFileWatcher : IChoConfigurationChangeWatcher
	{
		#region Instance Data Members (Private)

		private readonly string _threadName;
		private readonly string _configurationSectionName;
		private readonly string _configFilePath;
		private readonly string[] _includeFileList;
		private List<IChoConfigurationChangeWatcher> _configurationChangeWatchers = new List<IChoConfigurationChangeWatcher>();
		private readonly object _padLock = new object();

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
		public ChoConfigurationChangeCompositeFileWatcher(string configurationSectionName, string configFilePath, string[] includeFileList)
		{
			if (configFilePath.IsNullOrWhiteSpace()) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "configFilePath");
			if (configurationSectionName.IsNullOrWhiteSpace()) throw new ArgumentNullException("configurationSectionName");

			_configurationSectionName = configurationSectionName;
			_configFilePath = configFilePath;
			_includeFileList = includeFileList;
			_threadName = "{0} : {1}".FormatString(typeof(ChoConfigurationChangeCompositeFileWatcher).Name, _configurationSectionName);

			_configurationChangeWatchers.Add(new ChoConfigurationChangeFileWatcher(configurationSectionName, configFilePath));
            if (includeFileList != null)
            {
                foreach (string includeFilePath in includeFileList)
                {
                    if (includeFilePath.IsNullOrWhiteSpace())
                        continue;

                    _configurationChangeWatchers.Add(new ChoConfigurationChangeFileWatcher(configurationSectionName, includeFilePath));
                }
            }
		}

		#endregion Constructors

		#region Destructors

		/// <summary>
		/// <para>Allows an <see cref="ChoConfigurationChangeCompositeWatcher"/> to attempt to free resources and perform other cleanup operations before the <see cref="ChoConfigurationChangeCompositeWatcher"/> is reclaimed by garbage collection.</para>
		/// </summary>
		~ChoConfigurationChangeCompositeFileWatcher()
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

		public void SetConfigurationChangedEventHandler(object key, ChoConfigurationChangedEventHandler configurationChanged)
		{
			ChoGuard.ArgumentNotNull(key, "key");
			ChoGuard.ArgumentNotNull(configurationChanged, "configurationChanged");

			lock (_padLock)
			{
				List<IChoConfigurationChangeWatcher> configurationChangeWatchers = _configurationChangeWatchers;
				if (configurationChangeWatchers == null || configurationChangeWatchers.Count == 0)
					return;

				foreach (IChoConfigurationChangeWatcher configurationChangeWatcher in configurationChangeWatchers)
				{
					configurationChangeWatcher.SetConfigurationChangedEventHandler(key, configurationChanged);
				}
			}
		}

		public virtual void StartWatching()
		{
			List<IChoConfigurationChangeWatcher> configurationChangeWatchers = _configurationChangeWatchers;
			if (configurationChangeWatchers == null || configurationChangeWatchers.Count == 0)
				return;

			lock (_padLock)
			{
				foreach (IChoConfigurationChangeWatcher configurationChangeWatcher in configurationChangeWatchers)
				{
					configurationChangeWatcher.StartWatching();
				}
			}
		}

		public virtual void StopWatching()
		{
			List<IChoConfigurationChangeWatcher> configurationChangeWatchers = _configurationChangeWatchers;
			if (configurationChangeWatchers == null || configurationChangeWatchers.Count == 0)
				return;

			lock (_padLock)
			{
				foreach (IChoConfigurationChangeWatcher configurationChangeWatcher in configurationChangeWatchers)
				{
					configurationChangeWatcher.StopWatching();
				}
			}
		}

		public virtual void RestartWatching()
		{
			List<IChoConfigurationChangeWatcher> configurationChangeWatchers = _configurationChangeWatchers;
			if (configurationChangeWatchers == null || configurationChangeWatchers.Count == 0)
				return;

			lock (_padLock)
			{
				foreach (IChoConfigurationChangeWatcher configurationChangeWatcher in configurationChangeWatchers)
				{
					configurationChangeWatcher.RestartWatching();
				}
			}
		}

		public virtual void ResetWatching()
		{
			List<IChoConfigurationChangeWatcher> configurationChangeWatchers = _configurationChangeWatchers;
			if (configurationChangeWatchers == null || configurationChangeWatchers.Count == 0)
				return;

			lock (_padLock)
			{
				foreach (IChoConfigurationChangeWatcher configurationChangeWatcher in configurationChangeWatchers)
				{
					configurationChangeWatcher.ResetWatching();
				}
			}
		}

		public virtual void OnConfigurationChanged()
		{
			List<IChoConfigurationChangeWatcher> configurationChangeWatchers = _configurationChangeWatchers;
			if (configurationChangeWatchers == null || configurationChangeWatchers.Count == 0)
				return;

			lock (_padLock)
			{
				foreach (IChoConfigurationChangeWatcher configurationChangeWatcher in configurationChangeWatchers)
				{
					configurationChangeWatcher.OnConfigurationChanged();
					break;
				}
			}
		}

		public ChoConfigurationChangedEventArgs EventData
		{
			get { return new ChoConfigurationCompositeChangedEventArgs(_configurationSectionName); }
		}

		public string SectionName
		{
			get { return _configurationSectionName; }
		}

		#endregion
	}
}
