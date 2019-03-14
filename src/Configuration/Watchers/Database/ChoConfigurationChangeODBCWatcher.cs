namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.IO;
	using System.Text;
	using System.Data;
	using System.Data.Odbc;
	using System.Collections.Generic;

	using Cinchoo.Core.Properties;
	using System.Diagnostics;

	#endregion NameSpaces

	public class ChoConfigurationChangeODBCWatcher : ChoConfigurationChangeWatcher
	{
		#region Instance Data Members (Private)

		private readonly string _configurationSectionName;
		private readonly string _connectionString;
		private readonly string _threadName;
		private readonly string _lastWriteTimeTableName;
		private readonly string _lastWriteTimeColumnName;
		private readonly string _sql;

		#endregion Instance Data Members (Private)

		/// <summary>
		/// <para>Initialize a new <see cref="ChoConfigurationChangeODBCWatcher"/> class with the path to the configuration file and the name of the section</para>
		/// </summary>
		/// <param name="_configFilePath">
		/// <para>The full path to the configuration file.</para>
		/// </param>
		/// <param name="_configurationSectionName">
		/// <para>The name of the configuration section to watch.</para>
		/// </param>
		public ChoConfigurationChangeODBCWatcher(string configurationSectionName, string connectionString, string lastWriteTimeTableName, string lastWriteTimeColumnName)
		{
			ChoGuard.ArgumentNotNullOrEmpty(connectionString, "ConnectionString");
			ChoGuard.ArgumentNotNullOrEmpty(configurationSectionName, "ConfigurationSectionName");
			ChoGuard.ArgumentNotNullOrEmpty(lastWriteTimeTableName, "LastWriteTimeTableName");
			ChoGuard.ArgumentNotNullOrEmpty(lastWriteTimeColumnName, "LastWriteTimeColumnName");

			_configurationSectionName = configurationSectionName;
			_connectionString = connectionString;
			_threadName = "{0} : {1}".FormatString(typeof(ChoConfigurationChangeODBCWatcher).Name, _configurationSectionName);
			_lastWriteTimeTableName = lastWriteTimeTableName;
			_lastWriteTimeColumnName = lastWriteTimeColumnName;
			_sql = "SELECT {0} FROM {1}".FormatString(_lastWriteTimeColumnName, _lastWriteTimeTableName);
		}

		/// <summary>
		/// <para>Allows an <see cref="ChoConfigurationChangeODBCWatcher"/> to attempt to free resources and perform other cleanup operations before the <see cref="ChoConfigurationChangeODBCWatcher"/> is reclaimed by garbage collection.</para>
		/// </summary>
		~ChoConfigurationChangeODBCWatcher()
		{
			Disposing(false);
		}

		/// <summary>
		/// <para>Gets the name of the configuration section being watched.</para>
		/// </summary>
		/// <value>
		/// <para>The name of the configuration section being watched.</para>
		/// </value>
		public override string SectionName
		{
			get { return _configurationSectionName; }
		}

		/// <summary>
		/// <para>Returns the <see cref="DateTime"/> of the last change of the information watched</para>
		/// <para>The information is retrieved using the watched file modification timestamp</para>
		/// </summary>
		/// <returns>The <see cref="DateTime"/> of the last modificaiton, or <code>DateTime.MinValue</code> if the information can't be retrieved</returns>
        protected override DateTime GetCurrentLastWriteTime()
		{
			try
			{
				using (OdbcConnection connection = new OdbcConnection(_connectionString))
				{
					connection.Open();
					using (OdbcCommand com = new OdbcCommand(_sql))
					{
						using (OdbcDataReader reader = com.ExecuteReader())
						{
							while (reader.Read())
							{
								return reader.GetDateTime(0);
							}
						}
					}
				}
			}
			catch //(Exception ex)
			{
			}
			return DateTime.MinValue;
		}

		/// <summary>
		/// Returns the string that should be assigned to the thread used by the watcher
		/// </summary>
		/// <returns>The name for the thread</returns>
		protected override string ThreadName
		{
			get { return _threadName; }
		}

		/// <summary>
		/// Builds the change event data, including the full path of the watched file
		/// </summary>
		/// <returns>The change event information</returns>
		public override ChoConfigurationChangedEventArgs EventData
		{
			get { return new ChoConfigurationODBCChangedEventArgs(_configurationSectionName, _connectionString, _lastWriteTimeTableName, _lastWriteTimeColumnName); }
		}
	}
}
