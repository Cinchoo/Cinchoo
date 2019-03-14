namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.IO;
	using Cinchoo.Core.Win32;

	#endregion NameSpaces

	public class ChoConfigurationChangeRegistryWatcher : ChoConfigurationChangeWatcher
	{
		#region Instance Data Members (Private)

		private string _registryKey;
		private ChoRegistryChangeMonitor _registryChangeMonitor;
        private ChoConfigurationRegistryChangedEventArgs _eventArgs;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoConfigurationChangeRegistryWatcher(string configurationSectionName, string registryKey)
            : base(configurationSectionName)
		{
			ChoGuard.ArgumentNotNullOrEmpty(registryKey, "RegistryKey");

			_registryKey = registryKey;
            _eventArgs = new ChoConfigurationRegistryChangedEventArgs(configurationSectionName, _registryKey, DateTime.MinValue);

			_registryChangeMonitor = new ChoRegistryChangeMonitor(registryKey);
			_registryChangeMonitor.RegistryChanged += new EventHandler(RegistryChangeMonitor_RegistryChanged);
			_registryChangeMonitor.Error += new ErrorEventHandler(RegistryChangeMonitor_Error);
		}

		#endregion Constructors

		public override void StartWatching()
		{
			_registryChangeMonitor.Start();
		}

		public override void StopWatching()
		{
			_registryChangeMonitor.Stop();
		}

        protected override DateTime GetCurrentLastWriteTime()
		{
			throw new NotImplementedException();
		}

		public override ChoConfigurationChangedEventArgs EventData
		{
            get { return _eventArgs; }
		}

		#region Event Handlers

		private void RegistryChangeMonitor_Error(object sender, ErrorEventArgs e)
		{
		}

		private void RegistryChangeMonitor_RegistryChanged(object sender, EventArgs e)
		{
            _eventArgs = new ChoConfigurationRegistryChangedEventArgs(SectionName, _registryKey, DateTime.Now);
			OnConfigurationChanged();
		}

		#endregion Event Handlers
	}
}
