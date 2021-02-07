using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core.Configuration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoAnyDbConfigurationSectionAttribute : ChoDbGenericKeyValueConfigurationSectionAttribute
    {
        public ChoAnyDbConfigurationSectionAttribute(string configElementPath, string connectionStringOrName, string tableName = null, string provideName = "System.Data.SqlClient")
            : this(configElementPath, ChoStringEx.FormatString("CONNECTION_STRING='{0}';TABLE_NAME={1};PROVIDER_NAMESPACE={2}", (object)connectionStringOrName, (object)tableName, (object)provideName))
        {
        }

        public ChoAnyDbConfigurationSectionAttribute(string configElementPath, string dbParams)
            : base(configElementPath, dbParams)
        {
        }
    }
}
