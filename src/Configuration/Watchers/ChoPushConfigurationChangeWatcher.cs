namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Specialized;
	using System.Threading;
	using Cinchoo.Core.Diagnostics;
	using Cinchoo.Core.Services;

	#endregion NameSpaces

	/// <summary>
	/// <para>Represents an <see cref="IChoConfigurationChangeWatcher"/> that watches a file.</para>
	/// </summary>
	public abstract class ChoPushConfigurationChangeWatcher : IChoConfigurationChangeWatcher
	{
		private static readonly object _configurationChangedKey = new object();

		private readonly object _padLockObj = new object();
		private readonly object _eventHandlerListLock = new object();
		private readonly OrderedDictionary _eventHandlerList = new OrderedDictionary();
		private readonly string _configurationSectionName;

        private DateTime _lastUpdatedTimeStamp = DateTime.MinValue;
        private bool _firstTime = true;

		/// <summary>
		/// <para>Initialize a new <see cref="ChoConfigurationChangeWatcher"/> class</para>
		/// </summary>
		public ChoPushConfigurationChangeWatcher(string configurationSectionName)
		{
			ChoGuard.ArgumentNotNullOrEmpty(configurationSectionName, "ConfigurationSectionName");

			_configurationSectionName = configurationSectionName;
		}

		/// <summary>
		/// <para>Allows an <see cref="Cinchoo.Core.Configuration.ChoConfigurationChangeFileWatcher"/> to attempt to free resources and perform other cleanup operations before the <see cref="Cinchoo.Core.Configuration.ConfigurationChangeFileWatcher"/> is reclaimed by garbage collection.</para>
		/// </summary>
        ~ChoPushConfigurationChangeWatcher()
		{
			Disposing(false);
		}

		/// <summary>
		/// Event raised when the underlying persistence mechanism for configuration notices that
		/// the persistent representation of configuration information has changed.
		/// </summary>
		//public event ChoConfigurationChangedEventHandler ConfigurationChanged;
		public void SetConfigurationChangedEventHandler(object key, ChoConfigurationChangedEventHandler configurationChanged)
		{
			ChoGuard.ArgumentNotNull(key, "key");
			ChoGuard.ArgumentNotNull(configurationChanged, "configurationChanged");

			OrderedDictionary eventHandlerList = _eventHandlerList;
			if (eventHandlerList != null)
			{
				lock (_eventHandlerListLock)
				{
					if (eventHandlerList.Contains(key))
						eventHandlerList[key] = configurationChanged;
					else
						eventHandlerList.Add(key, configurationChanged);
				}
			}
		}

		/// <summary>
		/// <para>Starts watching the configuration file.</para>
		/// </summary>
		public abstract void StartWatching();

		/// <summary>
		/// <para>Stops watching the configuration file.</para>
		/// </summary>
        public abstract void StopWatching();

		public virtual void ResetWatching()
		{
            _firstTime = true;
            _lastUpdatedTimeStamp = DateTime.MinValue;
        }

		public virtual void RestartWatching()
		{
			StopWatching();
			StartWatching();
		}

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
			if (isDisposing)
			{
				StopWatching();

				OrderedDictionary eventHandlerList = _eventHandlerList;
				//_eventHandlerList = null;
				if (eventHandlerList != null)
				{
					lock (_eventHandlerListLock)
					{
						eventHandlerList.Clear();
					}
				}
			}
		}

		/// <summary>
		/// <para>Raises the <see cref="ConfigurationChanged"/> event.</para>
		/// </summary>
		public virtual void OnConfigurationChanged()
		{
			try
			{
				ChoConfigurationChangedEventArgs eventData = EventData;
				OrderedDictionary eventHandlerList = _eventHandlerList;
				if (eventHandlerList != null)
				{
					lock (_eventHandlerListLock)
					{
						//int counter = 0;
						//WaitHandle[] handles = new WaitHandle[_eventHandlerList.Count];
						foreach (ChoConfigurationChangedEventHandler callback in eventHandlerList.Values)
						{
							if (callback != null)
							{
								callback(this, eventData);

								//handles[counter++] = callback.BeginInvoke(this, eventData, null, null).AsyncWaitHandle;
							}
						}
						//WaitHandle.WaitAll(handles);
					}
				}
			}
			catch (ChoFatalApplicationException)
			{
                throw;
            }
			catch (Exception ex)
			{
				ChoTrace.Write(ex);
			}
		}

		/// <summary>
		/// <para>Gets the name of the configuration section being watched.</para>
		/// </summary>
		/// <value>
		/// <para>The name of the configuration section being watched.</para>
		/// </value>
		public virtual string SectionName
		{
			get { return _configurationSectionName; }
		}

		/// <summary>
		/// Builds the change event data, in a suitable way for the specific watcher implementation
		/// </summary>
		/// <returns>The change event information</returns>
		public virtual ChoConfigurationChangedEventArgs EventData
		{
            get { return new ChoConfigurationChangedEventArgs(_configurationSectionName, _lastUpdatedTimeStamp); }
		}

		/// <summary>
		/// Returns the source name to use when logging events
		/// </summary>
		/// <returns>The event source name</returns>
		protected virtual string EventSourceName
		{
			get { return ChoGlobalApplicationSettings.Me.EventLogSourceName; }
		}

        protected bool IsFirstTime
        {
            get { return _firstTime; }
        }

        protected void SetLastUpdatedTimeStamp(DateTime lastUpdatedTimeStamp)
        {
            _lastUpdatedTimeStamp = lastUpdatedTimeStamp;
        }
	}
}
