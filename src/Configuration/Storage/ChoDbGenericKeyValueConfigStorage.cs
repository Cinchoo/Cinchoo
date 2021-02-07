using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Timers;
using System.Configuration;

namespace Cinchoo.Core.Configuration
{
    /*
    CREATE TABLE TEST_CONFIG
    (
        [KEY] VARCHAR (50) NOT NULL,
        VALUE VARCHAR (100),
        PRIMARY KEY ([KEY])
    )
    */

    public interface IChoDbGenericSqlGenerator
    {
        void Override(IDbCommand selectCmd, IDbCommand insertCmd, IDbCommand updateCmd);
    }

    internal class ChoDbGenericKeyValueConfigStorageParams : IChoInitializable
    {
        [ChoPropertyInfo("CONNECTION_STRING")]
        public string CONNECTION_STRING;
        [ChoPropertyInfo("TABLE_NAME")]
        public string TABLE_NAME;
        [ChoPropertyInfo("KEY_COLUMN_NAME", DefaultValue = "KEY")]
        public string KEY_COLUMN_NAME;
        [ChoPropertyInfo("VALUE_COLUMN_NAME", DefaultValue = "VALUE")]
        public string VALUE_COLUMN_NAME;
        [ChoPropertyInfo("LAST_UPDATE_DATETIME_KEY_NAME", DefaultValue = "")]
        public string LAST_UPDATE_DATETIME_KEY_NAME;
        [ChoPropertyInfo("PROVIDER_ASSEMBLY_FILE_PATH")]
        public string PROVIDER_ASSEMBLY_FILE_PATH;
        [ChoPropertyInfo("PARAM_PREFIX_CHAR", DefaultValue = "@")]
        public char PARAM_PREFIX_CHAR;
        [ChoPropertyInfo("PROVIDER_NAMESPACE", DefaultValue = "System.Data.SqlClient")]
        public string PROVIDER_NAMESPACE;
        [ChoPropertyInfo("TRUNCATE_VALUE_DATA", DefaultValue = "True")]
        public bool TRUNCATE_VALUE_DATA;
        [ChoPropertyInfo("SQL_GENERATOR_TYPE")]
        public string SQL_GENERATOR_TYPE;

        public ChoDbGenericKeyValueConfigStorageParams(IEnumerable<Tuple<string, string>> parameters)
        {
            ChoObject.ResetObject(this);
            ChoObjectEx.Load(this, parameters);
        }

        public void Initialize()
        {
            if (CONNECTION_STRING.IsNullOrWhiteSpace())
                throw new ChoConfigurationException("Missing ConnectionString.");
            if (TABLE_NAME.IsNullOrWhiteSpace())
                throw new ChoConfigurationException("Missing TableName.");
            if (PROVIDER_NAMESPACE.IsNullOrWhiteSpace())
                throw new ChoConfigurationException("Missing ProviderNamespace.");
            if (PARAM_PREFIX_CHAR.ToString().IsAlphaNumeric())
                throw new ChoConfigurationException("Parameter prefix character [{0}] can't be alpha numeric.".FormatString(PARAM_PREFIX_CHAR));

            ConnectionStringSettings col = ConfigurationManager.ConnectionStrings[CONNECTION_STRING];
            if (col != null)
            {
                CONNECTION_STRING = col.ConnectionString;
                if (!col.ProviderName.IsNullOrWhiteSpace())
                    PROVIDER_NAMESPACE = col.ProviderName;
                else
                    PROVIDER_NAMESPACE = "System.Data.SqlClient";
            }
        }
    }

    public class ChoDbGenericKeyValueConfigStorage : IChoDictionaryConfigObjectAdapter
    {
        #region Instance Data Members (Private)

        private bool IsValidState = false;

        private Assembly _providerAssembly = null;
        private IDbDataAdapter _daSelectAll;
        private IDbDataAdapter _daSave;

        private DataSet _dsSelectAll = new DataSet();
        private DataSet _dsSave = new DataSet();
        private DataSet _dsGetLastUpdateTimeStamp = new DataSet();

        private string _selectSQL = "SELECT [{0}], [{1}] FROM [{2}]";
        private string _insertSQL = "INSERT INTO [{2}] ([{0}], [{1}]) VALUES ({3}{0}, {3}{1})";
        private string _updateSQL = "UPDATE [{2}] SET [{1}] = {3}{1} WHERE [{0}] = {3}{0}";
        private string _getLastUpdateTimeStampSQL = "SELECT [{1}] FROM [{2}] WHERE [{0}] = '{3}'";
        private IDbCommand _getLastUpdateTimeStampCmd;

        private ChoBaseConfigurationElement _configElement;
        private DataTable _saveTable;
        private ChoDbGenericKeyValueConfigStorageParams _dbGenericKeyValueConfigStorageParams;

        private Type _dbConnectionType;
        private Type _dbCommandType;
        private Type _dbDataAdapterType;
        private Type _dataParameterType;
        
        private int _keyColumnLength;
        private int _valueColumnLength;
        private Type _sqlGeneratorType;
        private Dictionary<string, object> _dict = null;
        private DateTime _lastUpdateDateTime = DateTime.MinValue;

        #endregion Instance Data Members (Private)

        #region IChoDictionaryConfigObjectAdapter Overloads

        public IDictionary GetData()
        {
            Hashtable dict = null;

            if (IsValidState)
            {
                dict = new Hashtable();
                //Connect to the source, load the configuration object payload
                DataTable table = LoadConfigData(_dsSelectAll, _daSelectAll);

                foreach (DataRow row in table.Rows)
                {
                    if (row[_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME] == null
                        || row[_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME] == DBNull.Value
                        || String.Compare((string)row[_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME], _dbGenericKeyValueConfigStorageParams.LAST_UPDATE_DATETIME_KEY_NAME, true) == 0
                        ) continue;

                    dict.Add(row[_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME], row[_dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME] == DBNull.Value ? null : row[_dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME]);
                }
            }

            return dict;
        }

        public void Save(System.Collections.IDictionary dict)
        {
            if (!IsValidState)
                return;

            if (dict != null && dict.Count > 0)
            {
                //if (_saveTable == null)
                    _saveTable = LoadConfigData(_dsSave, _daSave);

                foreach (string key in dict.Keys)
                    AddOrUpdateRow(_saveTable, key, dict[key] as string);

                if (_configElement != null && _configElement.WatchChange)
                {
                    if (!_dbGenericKeyValueConfigStorageParams.LAST_UPDATE_DATETIME_KEY_NAME.IsNullOrWhiteSpace())
                        AddOrUpdateRow(_saveTable, _dbGenericKeyValueConfigStorageParams.LAST_UPDATE_DATETIME_KEY_NAME, DateTime.Now.ToString());
                }

                try
                {
                    _daSave.Update(_dsSave); //, _dbGenericKeyValueConfigStorageParams.TABLE_NAME);
                    _dsSave.AcceptChanges();
                }
                catch
                {
                    _dsSave.RejectChanges();
                    throw;
                }
            }
        }

        public DateTime LastUpdateDateTime
        {
            get
            {
                if (IsValidState)
                {
                    if (_configElement.WatchChange)
                    {
                        if (!_dbGenericKeyValueConfigStorageParams.LAST_UPDATE_DATETIME_KEY_NAME.IsNullOrWhiteSpace())
                        {
                            using (IDbConnection conn = Connection)
                            {
                                _getLastUpdateTimeStampCmd.Connection = Connection;
                                _getLastUpdateTimeStampCmd.Connection.Open();

                                DateTime value = _getLastUpdateTimeStampCmd.ExecuteScalar().ChangeType<DateTime>();
                                if (value == default(DateTime))
                                    this._configElement.Persist(false, null);
                                return value;
                            }
                        }
                        else
                            return HasDataChanged();
                    }
                }

                return DateTime.MinValue;
            }
        }

        public void Init(ChoBaseConfigurationElement configElement, IEnumerable<Tuple<string, string>> parameters)
        {
            string providerAssemblyFilePath = null;

            _configElement = configElement;

            if (parameters != null)
            {
                IsValidState = false;

                _dbGenericKeyValueConfigStorageParams = ChoActivator.CreateInstance<ChoDbGenericKeyValueConfigStorageParams>(parameters);
                if (_dbGenericKeyValueConfigStorageParams.TABLE_NAME.IsNullOrWhiteSpace())
                    _dbGenericKeyValueConfigStorageParams.TABLE_NAME = configElement.ConfigSectionName;
                providerAssemblyFilePath = _dbGenericKeyValueConfigStorageParams.PROVIDER_ASSEMBLY_FILE_PATH;

                if (!providerAssemblyFilePath.IsNullOrWhiteSpace())
                {
                    try
                    {
                        _providerAssembly = Assembly.Load(providerAssemblyFilePath);
                    }
                    catch
                    {
                        _providerAssembly = Assembly.LoadFrom(providerAssemblyFilePath);
                    }
                    if (_providerAssembly == null)
                        throw new ChoConfigurationException("Can't find '{0}' assembly.".FormatString(providerAssemblyFilePath));
                }

                string sqlGeneratorTypeName = _dbGenericKeyValueConfigStorageParams.SQL_GENERATOR_TYPE;
                if (!sqlGeneratorTypeName.IsNullOrWhiteSpace())
                {
                    try
                    {
                        _sqlGeneratorType = ChoType.GetType(sqlGeneratorTypeName);
                    }
                    catch (Exception ex)
                    {
                        throw new ChoConfigurationException("Can't find sql generator '{0}' type.".FormatString(sqlGeneratorTypeName), ex);
                    }

                    if (!typeof(IChoDbGenericSqlGenerator).IsAssignableFrom(_sqlGeneratorType))
                        throw new ChoConfigurationException("Type '{0}' is not assignable from '{1}'.".FormatString(sqlGeneratorTypeName, typeof(IChoDbGenericSqlGenerator)));
                }

                DiscoverProviderTypes();

                _selectSQL = _selectSQL.FormatString(_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME, _dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME, _dbGenericKeyValueConfigStorageParams.TABLE_NAME);
                _insertSQL = _insertSQL.FormatString(_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME, _dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME, _dbGenericKeyValueConfigStorageParams.TABLE_NAME, _dbGenericKeyValueConfigStorageParams.PARAM_PREFIX_CHAR);
                _updateSQL = _updateSQL.FormatString(_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME, _dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME, _dbGenericKeyValueConfigStorageParams.TABLE_NAME, _dbGenericKeyValueConfigStorageParams.PARAM_PREFIX_CHAR);
                _getLastUpdateTimeStampSQL = _getLastUpdateTimeStampSQL.FormatString(_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME, _dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME, _dbGenericKeyValueConfigStorageParams.TABLE_NAME, _dbGenericKeyValueConfigStorageParams.LAST_UPDATE_DATETIME_KEY_NAME);

                _daSelectAll = GetDataAdapter(_selectSQL, _insertSQL, _updateSQL);
                _daSave = GetDataAdapter(_selectSQL, _insertSQL, _updateSQL);

                _getLastUpdateTimeStampCmd = NewCommand;
                _getLastUpdateTimeStampCmd.Connection = Connection;
                _getLastUpdateTimeStampCmd.CommandText = _getLastUpdateTimeStampSQL;
                _getLastUpdateTimeStampCmd.CommandType = CommandType.Text; 
                
                DataTable table = LoadConfigData(_dsSelectAll, _daSelectAll, true);

                if (table != null)
                {
                    if (!table.Columns.Contains(_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME))
                        throw new ChoConfigurationException("Key column '{0}' not found in '{1}' table.".FormatString(_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME, _dbGenericKeyValueConfigStorageParams.TABLE_NAME));
                    if (table.Columns[_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME].DataType != typeof(String))
                        throw new ChoConfigurationException("Key column '{0}' is not of type string. Found as '{1}' type.".FormatString(_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME, table.Columns[_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME].DataType));

                    if (!table.Columns.Contains(_dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME))
                        throw new ChoConfigurationException("Value column '{0}' not found in '{1}' table.".FormatString(_dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME, _dbGenericKeyValueConfigStorageParams.TABLE_NAME));
                    if (table.Columns[_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME].DataType != typeof(String))

                        throw new ChoConfigurationException("Value column '{0}' is not of type string. Found as '{1}' type.".FormatString(_dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME, table.Columns[_dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME].DataType));
                    _keyColumnLength = table.Columns[_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME].MaxLength;
                    _valueColumnLength = table.Columns[_dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME].MaxLength;

                    if (_keyColumnLength < 50)
                        throw new ChoConfigurationException("Key column '{0}' must be of size 50. Found '{1}' size.".FormatString(_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME, _keyColumnLength));
                }
                else
                    throw new ChoConfigurationException("Value column '{0}' not found in '{1}' table.".FormatString(_dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME, _dbGenericKeyValueConfigStorageParams.TABLE_NAME));

                IsValidState = true;
            }
        }

        private DateTime HasDataChanged()
        {
            Dictionary<string, object> dict = ((Hashtable)GetData()).ToDictionary<string, object>();
            if (dict != null)
            {
                if (_dict == null || _dict.Count == 0)
                {
                    _dict = dict;
                    _lastUpdateDateTime = DateTime.Now;
                }
                else
                {
                    var diff = _dict.Except(dict).Concat(dict.Except(_dict));
                    if (diff.Count() > 0)
                    {
                        _dict = dict;
                        _lastUpdateDateTime = DateTime.Now;
                    }
                }
            }

            return _lastUpdateDateTime;
        }

        #endregion IChoDictionaryConfigObjectAdapter Overloads

        #region Other Members (Private)

        private void AddOrUpdateRow(DataTable table, string key, string value)
        {
            if (_dbGenericKeyValueConfigStorageParams.TRUNCATE_VALUE_DATA && !value.IsNullOrWhiteSpace())
            {
                if (value.Length > _valueColumnLength)
                    value = value.Substring(0, _valueColumnLength);
            } 
            
            DataRow row = table.Rows.Find(key);
            if (row == null)
                row = table.Rows.Add(key, value);
            else
                row[_dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME] = value;
        }

        private DataTable LoadConfigData(DataSet ds, IDbDataAdapter da, bool fillSchema = false)
        {
            ds.Clear();
            if (!fillSchema)
                da.Fill(ds);
            else
                da.FillSchema(ds, SchemaType.Source);

            ds.Tables[0].PrimaryKey = new DataColumn[] { ds.Tables[0].Columns[_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME] };

            return ds.Tables[0];
        }

        protected virtual IDbDataAdapter GetDataAdapter(string selectSql, string insertSql, string updateSql)
        {
            IDbDataAdapter adapter = NewDataAdapter;

            adapter.SelectCommand = NewCommand;
            adapter.SelectCommand.Connection = Connection;
            adapter.SelectCommand.CommandText = selectSql;
            adapter.SelectCommand.CommandType = CommandType.Text;

            adapter.InsertCommand = NewCommand;
            adapter.InsertCommand.Connection = Connection;
            adapter.InsertCommand.CommandText = insertSql;
            adapter.InsertCommand.CommandType = CommandType.Text;

            IDataParameter param1 = NewParameter;
            param1.ParameterName = ParameterName(_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME);
            param1.SourceColumn = _dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME;
            adapter.InsertCommand.Parameters.Add(param1);

            IDataParameter param2 = NewParameter;
            param2.ParameterName = ParameterName(_dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME);
            param2.SourceColumn = _dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME;
            adapter.InsertCommand.Parameters.Add(param2);

            adapter.UpdateCommand = NewCommand;
            adapter.UpdateCommand.Connection = Connection;
            adapter.UpdateCommand.CommandText = updateSql;
            adapter.UpdateCommand.CommandType = CommandType.Text;

            IDataParameter param3 = NewParameter;
            param3.ParameterName = ParameterName(_dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME);
            param3.SourceColumn = _dbGenericKeyValueConfigStorageParams.KEY_COLUMN_NAME;
            adapter.UpdateCommand.Parameters.Add(param3);

            IDataParameter param4 = NewParameter;
            param4.ParameterName = ParameterName(_dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME);
            param4.SourceColumn = _dbGenericKeyValueConfigStorageParams.VALUE_COLUMN_NAME;
            adapter.UpdateCommand.Parameters.Add(param4);

            if (_sqlGeneratorType != null)
            {
                IChoDbGenericSqlGenerator gen = Activator.CreateInstance(_sqlGeneratorType) as IChoDbGenericSqlGenerator;
                if (gen != null)
                    gen.Override(adapter.SelectCommand, adapter.InsertCommand, adapter.UpdateCommand);
            }

            return adapter;
        }

        protected virtual string ParameterName(string name)
        {
            return "{0}{1}".FormatString(_dbGenericKeyValueConfigStorageParams.PARAM_PREFIX_CHAR, name);
        }

        protected virtual IDbDataAdapter NewDataAdapter
        {
            get
            {
                IDbDataAdapter dataAdapter = Activator.CreateInstance(_dbDataAdapterType) as IDbDataAdapter;
                return dataAdapter;
            }
        }

        protected virtual IDbCommand NewCommand
        {
            get
            {
                IDbCommand command = Activator.CreateInstance(_dbCommandType) as IDbCommand;
                return command;
            }
        }

        protected virtual IDataParameter NewParameter
        {
            get
            {
                IDataParameter parameter = Activator.CreateInstance(_dataParameterType) as IDataParameter;
                return parameter;
            }
        }

        protected virtual IDbConnection Connection
        {
            get 
            {
                IDbConnection connection = Activator.CreateInstance(_dbConnectionType) as IDbConnection;
                connection.ConnectionString = _dbGenericKeyValueConfigStorageParams.CONNECTION_STRING;
                return connection;
            }
        }

        private void DiscoverProviderTypes()
        {
            foreach (Type type in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                   .Where(t => t.IsClass && t.IsPublic && !t.IsNested && t.Namespace == _dbGenericKeyValueConfigStorageParams.PROVIDER_NAMESPACE))
            {
                if (typeof(IDbConnection).IsAssignableFrom(type))
                    _dbConnectionType = type;
                else if (typeof(IDbCommand).IsAssignableFrom(type))
                    _dbCommandType = type;
                else if (typeof(IDbDataAdapter).IsAssignableFrom(type))
                    _dbDataAdapterType = type;
                else if (typeof(IDataParameter).IsAssignableFrom(type))
                    _dataParameterType = type;
            }

            if (_dataParameterType == null)
                throw new ChoConfigurationException("Missing connection type.");
            if (_dbCommandType == null)
                throw new ChoConfigurationException("Missing command type.");
            if (_dbDataAdapterType == null)
                throw new ChoConfigurationException("Missing data adapter type.");
            if (_dataParameterType == null)
                throw new ChoConfigurationException("Missing data parameter type.");
        }

        #endregion Other Members (Private)
    }
}
