namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Data.Odbc;
    using System.Xml;
    using System.Xml.XPath;
    using Cinchoo.Core.Services;
    using Cinchoo.Core.Xml;

    #endregion NameSpaces

    public sealed class ChoODBCNameValueConfigStorage : ChoFileNameValueConfigStorage
	{
		#region Instance Data Members (Private)

		private IChoConfigurationChangeWatcher _configurationChangeWatcher;
		private ChoODBCSectionInfo _odbcSectionInfo;
		private readonly object _padLock = new object();

		#endregion Instance Data Members (Private)

        public override object Load(ChoBaseConfigurationElement configElement, XmlNode node)
		{
            base.Load(configElement, node);

			LoadODBCInfoFromXml(ConfigNode);

			if (_odbcSectionInfo != null)
				return _odbcSectionInfo.GetData();

			return null;
		}

		public override void Persist(object data, ChoDictionaryService<string, object> stateInfo)
		{
			ChoGuard.ArgumentNotNull(data, "Config Data Object");

            string configXml = ToXml(data);
            if (configXml.IsNullOrEmpty())
                return;

			lock (_padLock)
			{
				ConfigurationChangeWatcher.StopWatching();

				//Save the data
				if (_odbcSectionInfo != null)
                    _odbcSectionInfo.SaveData(configXml);

				ConfigurationChangeWatcher.StartWatching();
			}
		}

		public override IChoConfigurationChangeWatcher ConfigurationChangeWatcher
		{
			get
			{
				lock (SyncRoot)
				{
					if (_configurationChangeWatcher == null && _odbcSectionInfo != null)
					{
						if (_odbcSectionInfo != null)
							_configurationChangeWatcher = new ChoConfigurationChangeODBCWatcher(ConfigSectionName, _odbcSectionInfo.ConnectionString,
                                _odbcSectionInfo.TableName, "LAST_UPDATE_TIME");
					}

					return _configurationChangeWatcher;
				}
			}
		}

		private void LoadODBCInfoFromXml(XmlNode node)
		{
			_odbcSectionInfo = new ChoODBCSectionInfo(node, ConfigElement);
			_odbcSectionInfo.Validate();
        }
	}

	internal class ChoODBCSectionInfo
	{
        #region Constants

        public const string CONNECTION_STRING = "ConnectionString";
        public const string TABLE_NAME = "TableName";
        public const string CONFIG_DATA_COLUMN_SIZE = "ConfigDataColumnSize";

        private const string ConnectionStringToken = "cinchoo:connectionString";
		private const string TableNameToken = "cinchoo:tableName";
        private const string ConfigDataColumnSizeToken = "cinchoo:configDataColumnSize";

		#endregion Constants

		#region Instance Data Members (Private)

		public readonly string ConnectionString;
		public readonly string TableName;
        public readonly int ConfigDataColumnSize = 200;
		public bool IsTableExists = false;
        public bool HasRows = true;

		#endregion Instance Data Members (Private)

		public ChoODBCSectionInfo(XmlNode node, ChoBaseConfigurationElement configElement)
		{
            int configDataColumnSize = 0;
			if (node != null)
			{
				XPathNavigator navigator = node.CreateNavigator();

				ConnectionString = (string)navigator.Evaluate(String.Format("string(@{0})", ConnectionStringToken), ChoXmlDocument.NameSpaceManager);
				TableName = (string)navigator.Evaluate(String.Format("string(@{0})", TableNameToken), ChoXmlDocument.NameSpaceManager);
                Int32.TryParse((string)navigator.Evaluate(String.Format("string(@{0})", ConfigDataColumnSizeToken), ChoXmlDocument.NameSpaceManager), out configDataColumnSize);
			}

			if (ConnectionString.IsNullOrWhiteSpace())
				ConnectionString = configElement[CONNECTION_STRING] as string;
			if (TableName.IsNullOrWhiteSpace())
				TableName = configElement[TABLE_NAME] as string;
            if (configDataColumnSize <= 0)
                Int32.TryParse(configElement[CONFIG_DATA_COLUMN_SIZE] as string, out configDataColumnSize);

            if (configDataColumnSize > 0)
                ConfigDataColumnSize = configDataColumnSize;

            //Normalize the ConnectionString
            ConnectionString = ConnectionString.Replace("'", "");
		}

        private bool CreateTableIfNotExists()
		{
            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();
                try
                {
                    HasRows = ContainsRows(connection);
                    return true;
                }
                catch
                {
                    string sql = "CREATE TABLE {0} (CONFIG_DATA VARCHAR({1}), LAST_UPDATE_TIME DATETIME)".FormatString(TableName, ConfigDataColumnSize);
                    using (OdbcCommand com = new OdbcCommand(sql, connection))
                    {
                        using (OdbcDataReader reader = com.ExecuteReader())
                        {
                        }
                    }
                    return true;
                }

            }
		}

		public void Validate()
		{
			if (ConnectionString.IsNullOrWhiteSpace())
				throw new ChoConfigurationException("Missing ODBC ConnectionString.");

			if (TableName.IsNullOrWhiteSpace())
				throw new ChoConfigurationException(String.Format("Missing TableName for '{0}'.", ConnectionString));

            IsTableExists = CreateTableIfNotExists();
		}

        private bool ContainsRows(OdbcConnection connection)
        {
            string sql = "SELECT COUNT(*) FROM {0}".FormatString(TableName);
            using (OdbcCommand com = new OdbcCommand(sql, connection))
            {
                return (int)com.ExecuteScalar() > 0;
            }
        }

		internal Dictionary<string, object> GetData()
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();
			using (OdbcConnection connection = new OdbcConnection(ConnectionString))
			{
				connection.Open();
				using (OdbcCommand com = new OdbcCommand("SELECT * FROM {0}".FormatString(TableName), connection))
				{
					using (OdbcDataReader reader = com.ExecuteReader())
					{
						while (reader.Read())
						{
							//Load column names
							for (int index = 0; index < reader.FieldCount; index++)
								dict.Add(reader.GetName(index), reader[index] == DBNull.Value ? null : reader[index]);

							return dict;
						}
					}
				}
			}
			return dict;
		}

        internal void SaveData(string configXml)
		{
            if (!IsTableExists)
                return;
            if (configXml.IsNullOrEmpty())
                return;

            if (configXml.Length > ConfigDataColumnSize)
                throw new ChoConfigurationException("Size [{0}] of Configuration Data is larger than column size [{1}].".FormatString(configXml.Length, ConfigDataColumnSize));

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                if (ContainsRows(connection))
                {
                    string sql = "UPDATE {0} SET CONFIG_DATA = ?, LAST_UPDATE_TIME = ?".FormatString(TableName);
                    using (OdbcCommand com = new OdbcCommand(sql, connection))
                    {
                        com.Parameters.Add("@CONFIG_DATA", OdbcType.VarChar, ConfigDataColumnSize, "CONFIG_DATA").Value = configXml;
                        com.Parameters.Add("@LAST_UPDATE_TIME", OdbcType.Date, 0, "LAST_UPDATE_TIME").Value = DateTime.Now;
                        com.ExecuteNonQuery();
                    }
                }
                else
                {
                    string sql = "INSERT INTO {0} (CONFIG_DATA, LAST_UPDATE_TIME) VALUES (?, ?)".FormatString(TableName);
                    using (OdbcCommand com = new OdbcCommand(sql, connection))
                    {
                        com.Parameters.Add("@CONFIG_DATA", OdbcType.VarChar, ConfigDataColumnSize, "CONFIG_DATA").Value = configXml;
                        com.Parameters.Add("@LAST_UPDATE_TIME", OdbcType.Date, 0, "LAST_UPDATE_TIME").Value = DateTime.Now;
                        com.ExecuteNonQuery();
                    }
                }
            }
        }
	}
}
