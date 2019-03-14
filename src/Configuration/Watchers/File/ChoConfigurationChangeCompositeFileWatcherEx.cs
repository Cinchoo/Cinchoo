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
    using Cinchoo.Core.IO;

	#endregion NameSpaces

	public class ChoConfigurationChangeCompositeFileWatcherEx : ChoConfigurationChangeWatcher
	{
		#region Instance Data Members (Private)

		private readonly ChoDictionaryService<string, DateTime> _fileWatchersLastWriteTimeCache = new ChoDictionaryService<string, DateTime>(String.Format("FileWatchersLastWriteTimeCache_{0}", ChoRandom.NextRandom()));
		private string _configFilePath;
		private string[] _includeFileList;
		private List<string> _modifiedFileList = null;
		private readonly object _padLock = new object();

		#endregion Instance Data Members (Private)

		#region Constructors

		/// <summary>
		/// <para>Initialize a new <see cref="ChoConfigurationChangeCompositeFileWatcherEx"/> class with the path to the configuration file and the name of the section</para>
		/// </summary>
		/// <param name="_configFilePath">
		/// <para>The full path to the configuration file.</para>
		/// </param>
		/// <param name="_configurationSectionName">
		/// <para>The name of the configuration section to watch.</para>
		/// </param>
		public ChoConfigurationChangeCompositeFileWatcherEx(string configurationSectionName, string configFilePath, string[] includeFileList)
            : base(configurationSectionName)
		{
			if (string.IsNullOrEmpty(configFilePath)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "configFilePath");

			_configFilePath = configFilePath;
			_includeFileList = includeFileList;
		}

		#endregion Constructors

		public void Reset(string configFilePath, string[] includeFileList)
		{
			lock (_padLock)
			{
				_configFilePath = configFilePath;
				_includeFileList = includeFileList;
			}
		}

		#region ChoConfigurationChangeWatcher Overrides

		/// <summary>
		/// <para>Returns the <see cref="DateTime"/> of the last change of the information watched</para>
		/// <para>The information is retrieved using the watched file modification timestamp</para>
		/// </summary>
		/// <returns>The <see cref="DateTime"/> of the last modificaiton, or <code>DateTime.MinValue</code> if the information can't be retrieved</returns>
        protected override DateTime GetCurrentLastWriteTime()
		{
			if (File.Exists(_configFilePath) == true)
			{
				lock (_padLock)
				{
					List<string> modifiedFileList = new List<string>();
					DateTime storedLastWriteTime = _fileWatchersLastWriteTimeCache.GetValue(_configFilePath);

					DateTime lastWriteTime = File.GetLastWriteTime(_configFilePath);
					if (storedLastWriteTime != DateTime.MinValue && storedLastWriteTime < lastWriteTime)
						modifiedFileList.Add(_configFilePath);

					if (_includeFileList != null)
					{
						DateTime includeFileLastWriteTime;
						foreach (string includeFilePath in _includeFileList)
						{
							if (!File.Exists(includeFilePath))
								continue;

							includeFileLastWriteTime = File.GetLastWriteTime(includeFilePath);
							if (includeFileLastWriteTime > lastWriteTime)
								lastWriteTime = includeFileLastWriteTime;

							if (storedLastWriteTime != DateTime.MinValue && storedLastWriteTime < includeFileLastWriteTime)
								modifiedFileList.Add(includeFilePath);
						}
					}

					_modifiedFileList = modifiedFileList;
					_fileWatchersLastWriteTimeCache.SetValue(_configFilePath, lastWriteTime);

					return lastWriteTime;
				}
			}
			else
				return DateTime.MinValue;
		}

		/// <summary>
		/// Builds the change event data, including the full path of the watched file
		/// </summary>
		/// <returns>The change event information</returns>
		public override ChoConfigurationChangedEventArgs EventData
		{
			get
			{
				lock (_padLock)
				{
                    return new ChoConfigurationCompositeFileChangedEventArgs(this.SectionName, ChoPath.GetFullPath(_configFilePath), _includeFileList, _modifiedFileList == null ? null : _modifiedFileList.ToArray(), LastUpdatedTimeStamp);
				}
			}
		}

		#endregion ChoConfigurationChangeWatcher Overrides
	}
}
