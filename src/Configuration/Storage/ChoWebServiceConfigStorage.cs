using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using Cinchoo.Core.Net;
using Cinchoo.Core.Xml.Serialization;

namespace Cinchoo.Core.Configuration
{
    public interface IChoConfigurationService
    {
        ChoCDATA GET_CONFIG_DATA(string configElementName);
        DateTime GET_LAST_UPDATE_DATETIME();
        void SAVE_CONFIG_DATA(string configElementName, ChoCDATA configData);
    }

    internal class ChoWebServiceConfigStorageParams : IChoInitializable
    {
        [ChoPropertyInfo("WS_CONFIG_SECTION_NAME")]
        public string WS_CONFIG_SECTION_NAME;
        [ChoPropertyInfo("WS_URL")]
        public string WS_URL;
        [ChoPropertyInfo("WS_GET_LAST_UPDATE_DATETIME_METHOD", DefaultValue = "GET_LAST_UPDATE_DATETIME")]
        public string WS_GET_LAST_UPDATE_DATETIME_METHOD;
        [ChoPropertyInfo("WS_GET_CONFIG_DATA_METHOD", DefaultValue = "GET_CONFIG_DATA")]
        public string WS_GET_CONFIG_DATA_METHOD;
        [ChoPropertyInfo("WS_SAVE_CONFIG_DATA_METHOD", DefaultValue = "SAVE_CONFIG_DATA")]
        public string WS_SAVE_CONFIG_DATA_METHOD;

        public ChoWebServiceConfigStorageParams(IEnumerable<Tuple<string, string>> parameters)
        {
            ChoObject.ResetObject(this);
            ChoObjectEx.Load(this, parameters);
        }

        public void Initialize()
        {
            if (WS_URL.IsNullOrWhiteSpace())
                throw new ChoConfigurationException("Missing WebService Url.");
            if (WS_GET_LAST_UPDATE_DATETIME_METHOD.IsNullOrWhiteSpace())
                throw new ChoConfigurationException("Missing last update datetime webmethod.");
            if (WS_GET_CONFIG_DATA_METHOD.IsNullOrWhiteSpace())
                throw new ChoConfigurationException("Missing get config data webmethod.");
            if (WS_SAVE_CONFIG_DATA_METHOD.ToString().IsAlphaNumeric())
                throw new ChoConfigurationException("Missing save config data webmethod.");
        }
    }

    //public class ChoWebServiceConfigStorage : IChoDictionaryConfigObjectAdapter
    //{
    //    #region Instance Data Members (Private)

    //    private bool IsValidState = false;

    //    private Assembly _providerAssembly = null;
    //    private IDbDataAdapter _daSelectAll;
    //    private IDbDataAdapter _daSave;

    //    private DataSet _dsSelectAll = new DataSet();
    //    private DataSet _dsSave = new DataSet();
    //    private DataSet _dsGetLastUpdateTimeStamp = new DataSet();

    //    private string _selectSQL = "SELECT [{0}], [{1}] FROM [{2}]";
    //    private string _insertSQL = "INSERT INTO [{2}] ([{0}], [{1}]) VALUES ({3}{0}, {3}{1})";
    //    private string _updateSQL = "UPDATE [{2}] SET [{1}] = {3}{1} WHERE [{0}] = {3}{0}";
    //    private string _getLastUpdateTimeStampSQL = "SELECT [{1}] FROM [{2}] WHERE [{0}] = '{3}'";
    //    private IDbCommand _getLastUpdateTimeStampCmd;

    //    private ChoBaseConfigurationElement _configElement;
    //    private DataTable _saveTable;
    //    private ChoWebServiceConfigStorageParams _webServiceConfigStorageParams;

    //    private Type _dbConnectionType;
    //    private Type _dbCommandType;
    //    private Type _dbDataAdapterType;
    //    private Type _dataParameterType;
        
    //    private int _keyColumnLength;
    //    private int _valueColumnLength;
    //    private Type _sqlGeneratorType;

    //    #endregion Instance Data Members (Private)

    //    #region IChoDictionaryConfigObjectAdapter Overloads

    //    public IDictionary GetData()
    //    {
    //        Hashtable dict = null;

    //        if (IsValidState)
    //        {
    //            dict = new Hashtable();
    //            //Connect to the source, load the configuration object payload
    //            DataTable table = LoadConfigData(_dsSelectAll, _daSelectAll);

    //            foreach (DataRow row in table.Rows)
    //            {
    //                if (row[_webServiceConfigStorageParams.KEY_COLUMN_NAME] == null
    //                    || row[_webServiceConfigStorageParams.KEY_COLUMN_NAME] == DBNull.Value
    //                    || String.Compare((string)row[_webServiceConfigStorageParams.KEY_COLUMN_NAME], _webServiceConfigStorageParams.LAST_UPDATE_DATETIME_KEY_NAME, true) == 0
    //                    ) continue;

    //                dict.Add(row[_webServiceConfigStorageParams.KEY_COLUMN_NAME], row[_webServiceConfigStorageParams.VALUE_COLUMN_NAME] == DBNull.Value ? null : row[_webServiceConfigStorageParams.VALUE_COLUMN_NAME]);
    //            }
    //        }
    //        return dict;
    //    }

    //    public void Save(System.Collections.IDictionary dict)
    //    {
    //        if (!IsValidState)
    //            return;

    //        if (dict != null && dict.Count > 0)
    //        {
    //            if (_saveTable == null)
    //                _saveTable = LoadConfigData(_dsSave, _daSave);

    //            foreach (string key in dict.Keys)
    //                AddOrUpdateRow(_saveTable, key, dict[key] as string);

    //            if (_configElement != null && _configElement.WatchChange)
    //                AddOrUpdateRow(_saveTable, _webServiceConfigStorageParams.LAST_UPDATE_DATETIME_KEY_NAME, DateTime.Now.ToString());

    //            _daSave.Update(_dsSave); //, _webServiceConfigStorageParams.TABLE_NAME);
    //        }
    //    }

    //    public DateTime LastUpdateDateTime
    //    {
    //        get
    //        {
    //            if (IsValidState)
    //            {
    //                if (_configElement.WatchChange)
    //                {
    //                    using (ChoWebService ws = new ChoWebService(_webServiceConfigStorageParams.WS_URL))
    //                    {
    //                        string ws.Execute(_dsGetLastUpdateTimeStamp);
    //                    }
    //                }
    //            }

    //            return DateTime.MinValue;
    //        }
    //    }

    //    public void Init(ChoBaseConfigurationElement configElement, IEnumerable<Tuple<string, string>> parameters)
    //    {
    //        string providerAssemblyFilePath = null;

    //        _configElement = configElement;

    //        if (parameters != null)
    //        {
    //            IsValidState = false;

    //            _webServiceConfigStorageParams = ChoActivator.CreateInstance<ChoWebServiceConfigStorageParams>(parameters);
    //            if (_webServiceConfigStorageParams.WS_CONFIG_SECTION_NAME.IsNullOrWhiteSpace())
    //                _webServiceConfigStorageParams.WS_CONFIG_SECTION_NAME = configElement.ConfigSectionName;
                
    //            DataTable table = LoadConfigData(_dsSelectAll, _daSelectAll, true);

    //            if (table != null)
    //            {
    //                if (!table.Columns.Contains(_webServiceConfigStorageParams.KEY_COLUMN_NAME))
    //                    throw new ChoConfigurationException("Key column '{0}' not found in '{1}' table.".FormatString(_webServiceConfigStorageParams.KEY_COLUMN_NAME, _webServiceConfigStorageParams.TABLE_NAME));
    //                if (table.Columns[_webServiceConfigStorageParams.KEY_COLUMN_NAME].DataType != typeof(String))
    //                    throw new ChoConfigurationException("Key column '{0}' is not of type string. Found as '{1}' type.".FormatString(_webServiceConfigStorageParams.KEY_COLUMN_NAME, table.Columns[_webServiceConfigStorageParams.KEY_COLUMN_NAME].DataType));

    //                if (!table.Columns.Contains(_webServiceConfigStorageParams.VALUE_COLUMN_NAME))
    //                    throw new ChoConfigurationException("Value column '{0}' not found in '{1}' table.".FormatString(_webServiceConfigStorageParams.VALUE_COLUMN_NAME, _webServiceConfigStorageParams.TABLE_NAME));
    //                if (table.Columns[_webServiceConfigStorageParams.KEY_COLUMN_NAME].DataType != typeof(String))

    //                    throw new ChoConfigurationException("Value column '{0}' is not of type string. Found as '{1}' type.".FormatString(_webServiceConfigStorageParams.VALUE_COLUMN_NAME, table.Columns[_webServiceConfigStorageParams.VALUE_COLUMN_NAME].DataType));
    //                _keyColumnLength = table.Columns[_webServiceConfigStorageParams.KEY_COLUMN_NAME].MaxLength;
    //                _valueColumnLength = table.Columns[_webServiceConfigStorageParams.VALUE_COLUMN_NAME].MaxLength;

    //                if (_keyColumnLength < 50)
    //                    throw new ChoConfigurationException("Key column '{0}' must be of size 50. Found '{1}' size.".FormatString(_webServiceConfigStorageParams.KEY_COLUMN_NAME, _keyColumnLength));
    //            }
    //            else
    //                throw new ChoConfigurationException("Value column '{0}' not found in '{1}' table.".FormatString(_webServiceConfigStorageParams.VALUE_COLUMN_NAME, _webServiceConfigStorageParams.TABLE_NAME));

    //            IsValidState = true;
    //        }
    //    }

    //    #endregion IChoDictionaryConfigObjectAdapter Overloads

    //    #region Other Members (Private)

    //    private void AddOrUpdateRow(DataTable table, string key, string value)
    //    {
    //        if (_webServiceConfigStorageParams.TRUNCATE_VALUE_DATA && !value.IsNullOrWhiteSpace())
    //        {
    //            if (value.Length > _valueColumnLength)
    //                value = value.Substring(0, _valueColumnLength);
    //        } 
            
    //        DataRow row = table.Rows.Find(key);
    //        if (row == null)
    //            row = table.Rows.Add(key, value);
    //        else
    //            row[_webServiceConfigStorageParams.VALUE_COLUMN_NAME] = value;
    //    }

    //    private DataTable LoadConfigData(DataSet ds, IDbDataAdapter da, bool fillSchema = false)
    //    {
    //        ds.Clear();
    //        if (!fillSchema)
    //            da.Fill(ds);
    //        else
    //            da.FillSchema(ds, SchemaType.Source);

    //        ds.Tables[0].PrimaryKey = new DataColumn[] { ds.Tables[0].Columns[_webServiceConfigStorageParams.KEY_COLUMN_NAME] };

    //        return ds.Tables[0];
    //    }

    //    protected virtual IDbDataAdapter GetDataAdapter(string selectSql, string insertSql, string updateSql)
    //    {
    //        IDbDataAdapter adapter = NewDataAdapter;

    //        adapter.SelectCommand = NewCommand;
    //        adapter.SelectCommand.Connection = Connection;
    //        adapter.SelectCommand.CommandText = selectSql;
    //        adapter.SelectCommand.CommandType = CommandType.Text;

    //        adapter.InsertCommand = NewCommand;
    //        adapter.InsertCommand.Connection = Connection;
    //        adapter.InsertCommand.CommandText = insertSql;
    //        adapter.InsertCommand.CommandType = CommandType.Text;

    //        IDataParameter param1 = NewParameter;
    //        param1.ParameterName = ParameterName(_webServiceConfigStorageParams.KEY_COLUMN_NAME);
    //        param1.SourceColumn = _webServiceConfigStorageParams.KEY_COLUMN_NAME;
    //        adapter.InsertCommand.Parameters.Add(param1);

    //        IDataParameter param2 = NewParameter;
    //        param2.ParameterName = ParameterName(_webServiceConfigStorageParams.VALUE_COLUMN_NAME);
    //        param2.SourceColumn = _webServiceConfigStorageParams.VALUE_COLUMN_NAME;
    //        adapter.InsertCommand.Parameters.Add(param2);

    //        adapter.UpdateCommand = NewCommand;
    //        adapter.UpdateCommand.Connection = Connection;
    //        adapter.UpdateCommand.CommandText = updateSql;
    //        adapter.UpdateCommand.CommandType = CommandType.Text;

    //        IDataParameter param3 = NewParameter;
    //        param3.ParameterName = ParameterName(_webServiceConfigStorageParams.KEY_COLUMN_NAME);
    //        param3.SourceColumn = _webServiceConfigStorageParams.KEY_COLUMN_NAME;
    //        adapter.UpdateCommand.Parameters.Add(param3);

    //        IDataParameter param4 = NewParameter;
    //        param4.ParameterName = ParameterName(_webServiceConfigStorageParams.VALUE_COLUMN_NAME);
    //        param4.SourceColumn = _webServiceConfigStorageParams.VALUE_COLUMN_NAME;
    //        adapter.UpdateCommand.Parameters.Add(param4);

    //        IChoDbGenericSqlGenerator gen = Activator.CreateInstance(_sqlGeneratorType) as IChoDbGenericSqlGenerator;
    //        if (gen != null)
    //            gen.Override(adapter.SelectCommand, adapter.InsertCommand, adapter.UpdateCommand);

    //        return adapter;
    //    }

    //    protected virtual string ParameterName(string name)
    //    {
    //        return "{0}{1}".FormatString(_webServiceConfigStorageParams.PARAM_PREFIX_CHAR, name);
    //    }

    //    protected virtual IDbDataAdapter NewDataAdapter
    //    {
    //        get
    //        {
    //            IDbDataAdapter dataAdapter = Activator.CreateInstance(_dbDataAdapterType) as IDbDataAdapter;
    //            return dataAdapter;
    //        }
    //    }

    //    protected virtual IDbCommand NewCommand
    //    {
    //        get
    //        {
    //            IDbCommand command = Activator.CreateInstance(_dbCommandType) as IDbCommand;
    //            return command;
    //        }
    //    }

    //    protected virtual IDataParameter NewParameter
    //    {
    //        get
    //        {
    //            IDataParameter parameter = Activator.CreateInstance(_dataParameterType) as IDataParameter;
    //            return parameter;
    //        }
    //    }

    //    protected virtual IDbConnection Connection
    //    {
    //        get 
    //        {
    //            IDbConnection connection = Activator.CreateInstance(_dbConnectionType) as IDbConnection;
    //            connection.ConnectionString = _webServiceConfigStorageParams.CONNECTION_STRING;
    //            return connection;
    //        }
    //    }

    //    private void DiscoverProviderTypes()
    //    {
    //        foreach (Type type in AppDomain.CurrentDomain.GetAssemblies()
    //            .SelectMany(t => t.GetTypes())
    //               .Where(t => t.IsClass && t.IsPublic && !t.IsNested && t.Namespace == _webServiceConfigStorageParams.PROVIDER_NAMESPACE))
    //        {
    //            if (typeof(IDbConnection).IsAssignableFrom(type))
    //                _dbConnectionType = type;
    //            else if (typeof(IDbCommand).IsAssignableFrom(type))
    //                _dbCommandType = type;
    //            else if (typeof(IDbDataAdapter).IsAssignableFrom(type))
    //                _dbDataAdapterType = type;
    //            else if (typeof(IDataParameter).IsAssignableFrom(type))
    //                _dataParameterType = type;
    //        }

    //        if (_dataParameterType == null)
    //            throw new ChoConfigurationException("Missing connection type.");
    //        if (_dbCommandType == null)
    //            throw new ChoConfigurationException("Missing command type.");
    //        if (_dbDataAdapterType == null)
    //            throw new ChoConfigurationException("Missing data adapter type.");
    //        if (_dataParameterType == null)
    //            throw new ChoConfigurationException("Missing data parameter type.");
    //    }

    //    #endregion Other Members (Private)
    //}
}
