namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using Cinchoo.Core.Configuration;
	using Cinchoo.Core.Configuration.Handlers;
	using Cinchoo.Core.IO;
	using Cinchoo.Core.Diagnostics;

	#endregion NameSpaces

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ChoODBCConfigurationSectionAttribute : ChoConfigurationSectionAttribute
	{
		#region Constants

		public const string CONNECTION_STRING = "ConnectionString";
		public const string TABLE_NAME = "TableName";
        //public const string LAST_WRITE_TIME_COLUMN_NAME = "LastWriteTimeColumnName";
        //public const string CREATE_TABLE_SQL = "CreateTableSQL";

		#endregion Constants

		#region Instance Data Members (Private)

		private ChoConfigurationElement _configurationElement;
		private readonly string _connectionString;
		private readonly string _tableName;
		private readonly string _lastWriteTimeColumnName;
		private string _configSectionHandlerType = typeof(ChoNameValueSectionHandler).FullName;

		#endregion Instance Data Members (Private)

		#region Constructors

		/// <summary>
		/// Initialize a new instance of the <see cref="ChoConfigurationSectionAttribute"/> class with the configuration element name.
		/// </summary>
		/// <param name="configElementPath">The <see cref="string"/> configuration element name.</param>
		public ChoODBCConfigurationSectionAttribute(string configElementPath, string connectionString, string tableName, string lastWriteTimeColumnName)
			: base(configElementPath)
		{
			ChoGuard.NotNullOrEmpty(connectionString, CONNECTION_STRING);
			ChoGuard.NotNullOrEmpty(tableName, TABLE_NAME);
            //ChoGuard.NotNullOrEmpty(lastWriteTimeColumnName, LAST_WRITE_TIME_COLUMN_NAME);

			_connectionString = connectionString;
			_tableName = tableName;
			_lastWriteTimeColumnName = lastWriteTimeColumnName;
		}

		#endregion

		#region Instance Properties (Public)

		public string CreateTableSQL
		{
			get;
			set;
		}

		public override string ConfigSectionHandlerType
		{
			get { return _configSectionHandlerType; }
			protected set { _configSectionHandlerType = value; }
		}

		#endregion Instance Properties (Public)

		public override ChoBaseConfigurationElement GetMe(Type type)
		{
			ChoGuard.ArgumentNotNull(type, "Type");

			if (_configurationElement == null)
			{
				lock (SyncRoot)
				{
					if (_configurationElement == null)
					{
						_configurationElement = new ChoConfigurationElement(ConfigElementPath, BindingMode, TraceOutputDirectory, TraceOutputFileName.IsNullOrEmpty() ? ChoPath.AddExtension(type.FullName, ChoReservedFileExt.Log) : TraceOutputFileName);
						_configurationElement.DefaultConfigSectionHandlerType = ConfigSectionHandlerType;
                        _configurationElement.Defaultable = Defaultable;
                        _configurationElement.ConfigStorageType = ConfigStorageType;
                        _configurationElement.LogCondition = LogCondition;
						_configurationElement.LogTimeStampFormat = LogTimeStampFormat;
                        _configurationElement.ConfigFilePath = ConfigFilePath;
                        _configurationElement[CONNECTION_STRING] = _connectionString;
						_configurationElement[TABLE_NAME] = _tableName;
                        //_configurationElement[LAST_WRITE_TIME_COLUMN_NAME] = _lastWriteTimeColumnName;
                        //if (!CreateTableSQL.IsNullOrWhiteSpace())
                        //    _configurationElement[CREATE_TABLE_SQL] = CreateTableSQL;
                        LoadParameters(_configurationElement);
                    }
				}
			}

			return _configurationElement;
		}
	}
}
