namespace Cinchoo.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoSqlServerConfigurationSectionAttribute : ChoDbGenericKeyValueConfigurationSectionAttribute
    {
        public ChoSqlServerConfigurationSectionAttribute(string configElementPath, string serverName, string databaseName, string tableName)
            : this(configElementPath, @"Server={0};Database={1};Trusted_Connection=True;".FormatString(serverName, databaseName), tableName)
        {
        }
        public ChoSqlServerConfigurationSectionAttribute(string configElementPath, string serverName, string databaseName, string userName, string password, string tableName)
            : this(configElementPath, @"Server={0};Database={1};User Id={2};Password={3};".FormatString(serverName, databaseName, userName, password), tableName)
        {
        }

        public ChoSqlServerConfigurationSectionAttribute(string configElementPath, string connectionStringOrName, string tableName)
            : base(configElementPath, @"CONNECTION_STRING='{0}';TABLE_NAME={1};PROVIDER_NAMESPACE=System.Data.SqlClient".FormatString(connectionStringOrName, tableName))
        {
        }
    }
}
