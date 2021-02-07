namespace Cinchoo.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoDbGenericKeyValueConfigurationSectionAttribute : ChoDictionaryAdapterConfigurationSectionAttribute
    {

        public ChoDbGenericKeyValueConfigurationSectionAttribute(string configElementPath, string connectionStringOrName, string tableName, string provideName = "System.Data.SqlClient")
            : base(configElementPath, @"CONNECTION_STRING='{0}';TABLE_NAME={1};PROVIDER_NAMESPACE={2}".FormatString(connectionStringOrName, tableName, provideName))
        {
        }

        public ChoDbGenericKeyValueConfigurationSectionAttribute(string configElementPath, string configObjectAdapterParams) :
            base(configElementPath, typeof(ChoDbGenericKeyValueConfigStorage), configObjectAdapterParams)
        {
        }
    }
}
