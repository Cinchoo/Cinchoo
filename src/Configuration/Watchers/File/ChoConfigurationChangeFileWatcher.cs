namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.IO;
	using System.Text;
	using System.Collections.Generic;

	using Cinchoo.Core.Properties;
	using System.Diagnostics;
    using Cinchoo.Core.IO;

	#endregion NameSpaces

	public class ChoConfigurationChangeFileWatcher : ChoConfigurationChangeWatcher ///, IChoConfigurationChangeWatcher
	{
		#region Instance Data Members (Private)

		private readonly string _configFilePath;
		private readonly object _padLock = new object();

		private ChoConfigurationChangeAction _configurationChangeAction = ChoConfigurationChangeAction.Changed;
		private DateTime _lastWriteTime = DateTime.MinValue;
        private DateTime _createdTime = DateTime.MinValue;
		private ChoConfigurationFileChangedEventArgs _configurationFileChangedEventArgs = null;

		#endregion Instance Data Members (Private)

		/// <summary>
		/// <para>Initialize a new <see cref="ChoConfigurationChangeFileWatcher"/> class with the path to the configuration file and the name of the section</para>
		/// </summary>
		/// <param name="_configFilePath">
		/// <para>The full path to the configuration file.</para>
		/// </param>
		/// <param name="_configurationSectionName">
		/// <para>The name of the configuration section to watch.</para>
		/// </param>
		public ChoConfigurationChangeFileWatcher(string configurationSectionName, string configFilePath)
            : base(configurationSectionName)
		{
			if (string.IsNullOrEmpty(configFilePath)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "configFilePath");

			this._configFilePath = configFilePath;
            _configurationFileChangedEventArgs = new ChoConfigurationFileChangedEventArgs(configurationSectionName, ChoPath.GetFullPath(_configFilePath), _configurationChangeAction, DateTime.MinValue);
		}

		/// <summary>
		/// <para>Returns the <see cref="DateTime"/> of the last change of the information watched</para>
		/// <para>The information is retrieved using the watched file modification timestamp</para>
		/// </summary>
		/// <returns>The <see cref="DateTime"/> of the last modificaiton, or <code>DateTime.MinValue</code> if the information can't be retrieved</returns>
        protected override DateTime GetCurrentLastWriteTime()
		{
			lock (_padLock)
			{
                DateTime lastWriteTime;
                DateTime createdTime;
                ChoConfigurationChangeAction configurationChangeAction = ChoConfigurationChangeAction.Changed;

                if (File.Exists(_configFilePath))
                {
                    lastWriteTime = File.GetLastWriteTime(_configFilePath);
                    createdTime = File.GetCreationTime(_configFilePath);
                }
                else
                {
                    lastWriteTime = DateTime.MinValue;
                    createdTime = DateTime.MinValue;
                }

                try
                {
                    if (lastWriteTime == DateTime.MinValue)
                    {
                        if (_lastWriteTime == DateTime.MinValue)
                        {
                        }
                        else
                        {
                            configurationChangeAction = ChoConfigurationChangeAction.Deleted;
                        }
                    }
                    else
                    {
                        if (_lastWriteTime == DateTime.MinValue)
                            configurationChangeAction = ChoConfigurationChangeAction.Created;
                        else
                        {
                            if (_lastWriteTime != lastWriteTime)
                                configurationChangeAction = ChoConfigurationChangeAction.Changed;
                        }
                    }
                }
                finally
                {
                    if (_configurationChangeAction != configurationChangeAction)
                    {
                        _configurationChangeAction = configurationChangeAction;
                        _configurationFileChangedEventArgs = new ChoConfigurationFileChangedEventArgs(this.SectionName, ChoPath.GetFullPath(_configFilePath), _configurationChangeAction, _lastWriteTime);
                    }
                    _lastWriteTime = lastWriteTime;
                    _createdTime = createdTime;
                }

				return _lastWriteTime;
			}
		}

		/// <summary>
		/// Builds the change event data, including the full path of the watched file
		/// </summary>
		/// <returns>The change event information</returns>
		public override ChoConfigurationChangedEventArgs EventData
		{
			get { return _configurationFileChangedEventArgs; }
		}
	}
}
