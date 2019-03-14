namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using Cinchoo.Core.Collections.Generic.Dictionary;
	using Cinchoo.Core.APM;
	using Cinchoo.Core.Diagnostics;
using Cinchoo.Core.Services;
	using System.Threading;

	#endregion NameSpaces

	public delegate void ChoConfigurationStateChangedEventHandler(object sender, ChoConfigurationStateChangedEventArgs e);

	public class ChoConfigurationElementStateManager
	{
		#region Instance Data Members (Private)

		private readonly string _configsectionName;
		private readonly ChoDictionary<object, ChoConfigurationStateChangedEventHandler> _eventHandlerList = ChoDictionary<object, ChoConfigurationStateChangedEventHandler>.Synchronized(new ChoDictionary<object, ChoConfigurationStateChangedEventHandler>());
		private Type _configSectionHandlerType;

		#endregion

		#region Shared Data Members (Private)

		private static readonly ChoDictionaryService<string, ChoConfigurationElementStateManager> _configurationElementStateManagerDictionaryService = new ChoDictionaryService<string,ChoConfigurationElementStateManager>("ChoConfigurationElementStateManagerDictionaryService");

		#endregion Shared Data Members (Private)

		#region Constructors & Destructors

		/// <summary>
		/// <para>Initialize a new <see cref="ChoConfigurationChangeWatcher"/> class</para>
		/// </summary>
		private ChoConfigurationElementStateManager(string configsectionName)
		{
			ChoGuard.ArgumentNotNullOrEmpty(configsectionName, "configsectionName");
			_configsectionName = configsectionName;
		}

		/// <summary>
		/// <para>Allows an <see cref="Cinchoo.Core.Configuration.ChoConfigurationChangeFileWatcher"/> to attempt to free resources and perform other cleanup operations before the <see cref="Cinchoo.Core.Configuration.ConfigurationChangeFileWatcher"/> is reclaimed by garbage collection.</para>
		/// </summary>
		~ChoConfigurationElementStateManager()
		{
			Disposing(false);
		}

		#endregion

		#region Instance Members (Public)

		public void SetConfigurationStateChangeEventHandler(object key, ChoConfigurationStateChangedEventHandler configurationStateChanged)
		{
			ChoGuard.ArgumentNotNull(key, "key");
			ChoGuard.ArgumentNotNull(configurationStateChanged, "configurationStateChanged");

			_eventHandlerList.AddOrUpdate(key, configurationStateChanged);
		}

		//public object Tag
		//{
		//    get { return _tag; }
		//    set { _tag = value; }
		//}

		public virtual void OnConfigurationStateChanged()
		{
			OnConfigurationStateChanged(null);
		}
		
		public virtual void OnConfigurationStateChanged(object cmdInstruction)
		{
			OnConfigurationStateChanged(cmdInstruction, null);
		}
		
		public virtual void OnConfigurationStateChanged(object cmdInstruction, object tag)
		{
			try
			{
				if (_eventHandlerList != null)
				{
					int counter = 0;
					WaitHandle[] handles = new WaitHandle[_eventHandlerList.Count];
					ChoConfigurationStateChangedEventArgs eventData = EventData;
					foreach (ChoConfigurationStateChangedEventHandler callback in _eventHandlerList.ToValuesArray())
					{
						if (callback != null)
						{
							handles[counter++] = callback.BeginInvoke(this, new ChoConfigurationStateChangedEventArgs(_configsectionName, cmdInstruction, tag), null, null).AsyncWaitHandle;
						}
					}
					WaitHandle.WaitAll(handles);
				}
			}
			catch (Exception ex)
			{
				ChoTrace.Write(ex);
			}
		}

		public void SetConfigSectionHandlerType(Type configSectionHandlerType)
		{
			if (_configSectionHandlerType == configSectionHandlerType)
				return;

			if (_configSectionHandlerType == null)
				_configSectionHandlerType = configSectionHandlerType;
			else
			{
				_configSectionHandlerType = configSectionHandlerType;
				OnConfigurationStateChanged();
			}
		}
		
		/// <summary>
		/// <para>Releases the unmanaged resources used by the <see cref="ConfigurationChangeFileWatcher"/> and optionally releases the managed resources.</para>
		/// </summary>
		public void Dispose()
		{
			Disposing(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region Instance Properties (Public)

		public virtual ChoConfigurationStateChangedEventArgs EventData
		{
			get { return new ChoConfigurationStateChangedEventArgs(_configsectionName, null, null); }
		}

		#endregion

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
				_eventHandlerList.Clear();
			}
		}

		public static bool ContainsConfigurationElementStateManager(string configSectionName)
		{
			lock (_configurationElementStateManagerDictionaryService.SyncRoot)
			{
				return _configurationElementStateManagerDictionaryService.ContainsKey(configSectionName);
			}
		}

		public static ChoConfigurationElementStateManager GetConfigurationElementStateManager(string configSectionName)
		{
			lock (_configurationElementStateManagerDictionaryService.SyncRoot)
			{
				if (!_configurationElementStateManagerDictionaryService.ContainsKey(configSectionName))
					_configurationElementStateManagerDictionaryService.SetValue(configSectionName, new ChoConfigurationElementStateManager(configSectionName));
			}
			return _configurationElementStateManagerDictionaryService.GetValue(configSectionName);
		}
	}
}
