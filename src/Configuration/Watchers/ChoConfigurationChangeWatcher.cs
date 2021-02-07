namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Specialized;
	using System.Threading;
	using Cinchoo.Core.Diagnostics;
	using Cinchoo.Core.Services;
    using System.Threading.Tasks;

	#endregion NameSpaces

	/// <summary>
	/// <para>Represents an <see cref="IChoConfigurationChangeWatcher"/> that watches a file.</para>
	/// </summary>
	public abstract class ChoConfigurationChangeWatcher : IChoConfigurationChangeWatcher
	{
		private static readonly object _configurationChangedKey = new object();

		private readonly object _padLockObj = new object();
		private readonly int _pollDelayInMilliseconds = 1000;
		private Thread _pollingThread;
		private DateTime _lastUpdatedTimeStamp = DateTime.MinValue;
		private PollingStatus _pollingStatus;
		private readonly object _eventHandlerListLock = new object();
		private readonly OrderedDictionary _eventHandlerList = new OrderedDictionary();
		private readonly string _configurationSectionName;
		private readonly string _threadName;
		private Action<int, PollingStatus> _pollNow = null;
		private bool _firstTime = true;
        private bool _stopWatching = false;

        private bool _doNotUseGlobalQueue = true;
		internal bool DoNotUseGlobalQueue
		{
            get { return _doNotUseGlobalQueue; }
            set { _doNotUseGlobalQueue = value; }
		}

		/// <summary>
		/// <para>Initialize a new <see cref="ChoConfigurationChangeWatcher"/> class</para>
		/// </summary>
		public ChoConfigurationChangeWatcher(string configurationSectionName)
		{
			ChoGuard.ArgumentNotNullOrEmpty(configurationSectionName, "ConfigurationSectionName");

			_configurationSectionName = configurationSectionName;
			_threadName = "{0} : {1}".FormatString(GetType().Name, _configurationSectionName);

			_pollNow = new Action<int, PollingStatus>((pollDelayInMilliseconds, pollingStatus) =>
			{
				Poller(pollingStatus);
                Thread.Sleep(pollDelayInMilliseconds);
            });
		}

		/// <summary>
		/// <para>Allows an <see cref="Cinchoo.Core.Configuration.ChoConfigurationChangeFileWatcher"/> to attempt to free resources and perform other cleanup operations before the <see cref="Cinchoo.Core.Configuration.ConfigurationChangeFileWatcher"/> is reclaimed by garbage collection.</para>
		/// </summary>
		~ChoConfigurationChangeWatcher()
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
		public virtual void StartWatching()
		{
            _stopWatching = false;
			Thread pollingThread = _pollingThread;
			if (pollingThread != null)
				return;

			lock (_padLockObj)
			{
				if (_pollingThread == null)
				{
					ResetWatching();
					_pollingStatus = new PollingStatus(true);
					_pollingThread = new Thread(new ParameterizedThreadStart(Poller));

                    if (!DoNotUseGlobalQueue)
                    {
                        Task.Factory.StartNew(() => PollNow());
                    }
                    else
                    {
                        _pollingThread.IsBackground = true;
                        _pollingThread.Name = ThreadName;
                        _pollingThread.Start(_pollingStatus);
                        Thread.Sleep(100);
                    }
				}
			}
		}

		private void PollNow()
		{
            PollingStatus pollingStatus = null;
            while (true)
            {
                pollingStatus = _pollingStatus;

                //if (!ChoFramework.ShutdownRequested)
                //    ChoQueuedExecutionService.Global.Enqueue<int, PollingStatus>(_pollNow, _pollDelayInMilliseconds, _pollingStatus);
                if (pollingStatus != null && pollingStatus.Polling)
                {
                    if (!ChoFramework.ShutdownRequested)
                    {
                        IAsyncResult result = ChoQueuedExecutionService.Global.Enqueue<int, PollingStatus>(_pollNow, _pollDelayInMilliseconds, _pollingStatus);
                        result.AsyncWaitHandle.WaitOne();
                        Thread.Sleep(_pollDelayInMilliseconds);
                    }
                    else
                        break;
                }
                else
                    break;
            }
		}

		/// <summary>
		/// <para>Stops watching the configuration file.</para>
		/// </summary>
		public virtual void StopWatching()
		{
            _stopWatching = true;
        }

        private void DisposeWatcher()
        {
            _stopWatching = true;
			Thread pollingThread = _pollingThread;
			if (pollingThread == null)
				return;

			lock (_padLockObj)
			{
				if (_pollingThread != null)
				{
					_pollingStatus.Polling = false;

					int noOfRetry = 0;
					while (_pollingThread.IsAlive)
					{
						noOfRetry++;
						Thread.Sleep(100);

						if (noOfRetry == 5)
						{
							_pollingThread.AbortThread();
							break;
						}
					}

					if (!_pollingThread.IsAlive)
					{
						_pollingStatus = null;
						_pollingThread = null;
					}
				}
			}
		}

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
				//StopWatching();
                DisposeWatcher();

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
		/// <para>Returns the <see cref="DateTime"/> of the last change of the information watched</para>
		/// </summary>
		/// <returns>The <see cref="DateTime"/> of the last modificaiton, or <code>DateTime.MinValue</code> if the information can't be retrieved</returns>
		protected abstract DateTime GetCurrentLastWriteTime();

		/// <summary>
		/// Returns the string that should be assigned to the thread used by the watcher
		/// </summary>
		/// <returns>The name for the thread</returns>
		protected virtual string ThreadName
		{
			get { return _threadName; }
		}

		protected virtual DateTime LastUpdatedTimeStamp
		{
			get { return _lastUpdatedTimeStamp; }
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

		private void Poller(object parameter)
		{
			DateTime currentLastWriteTime;
			PollingStatus pollingStatus = (PollingStatus)parameter;

			if (DoNotUseGlobalQueue)
			{
				while (pollingStatus.Polling)
				{
                    if (!_stopWatching)
                    {
                        currentLastWriteTime = GetCurrentLastWriteTime();
                        if (_firstTime)
                        {
                            _firstTime = false;
                            _lastUpdatedTimeStamp = currentLastWriteTime;
                        }
                        else
                        {
                            if (_lastUpdatedTimeStamp != currentLastWriteTime)
                            {
                                if (ChoApplication.IsInitialized)
                                {
                                    _lastUpdatedTimeStamp = currentLastWriteTime;
                                    OnConfigurationChanged();
                                }
                            }
                        }
                    }
					Thread.Sleep(_pollDelayInMilliseconds);
				}
			}
			else
			{
                if (!_stopWatching)
                {
                    currentLastWriteTime = GetCurrentLastWriteTime();
                    if (_firstTime)
                    {
                        _firstTime = false;
                        _lastUpdatedTimeStamp = currentLastWriteTime;
                    }
                    else
                    {
                        if (_lastUpdatedTimeStamp != currentLastWriteTime)
                        {
                            if (ChoApplication.IsInitialized)
                            {
                                _lastUpdatedTimeStamp = currentLastWriteTime;
                                OnConfigurationChanged();
                            }
                        }
                    }
                }
                //if (pollingStatus != null && pollingStatus.Polling)
                //    PollNow();
			}
		}

		private class PollingStatus
		{
			private volatile bool polling;

			public PollingStatus(bool polling)
			{
				this.polling = polling;
			}

			public bool Polling
			{
				get { return polling; }
				set { polling = value; }
			}
		}
	}
}
