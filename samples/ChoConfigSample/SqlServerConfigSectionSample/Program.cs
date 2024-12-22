using Cinchoo.Core;
using Cinchoo.Core.Configuration;
using Cinchoo.Core.Shell;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServerConfigSectionSample
{
    ///Create a table in SqlServer databse using the sql script below
    /*
    CREATE TABLE APP_SETTINGS
    (
        [KEY] VARCHAR (50) NOT NULL,
        VALUE VARCHAR (100),
        PRIMARY KEY ([KEY]) 
    )  
    */

    [ChoSqlServerConfigurationSection("appSettings", @"Server=WNPCTDVZKN01\MSZKND01;Database=School;Trusted_Connection=True;", "APP_SETTINGS")]
    public class AppSettings : ChoConfigurableObject
    {
        [ChoPropertyInfo("name", DefaultValue = "Mark")]
        public string Name;

        [ChoPropertyInfo("address", DefaultValue = "100, Madison Avenue, New York, NY 10010")]
        public string Address;

        protected override void OnAfterConfigurationObjectLoaded()
        {
            Console.WriteLine(ToString());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            AppSettings appSettings = new AppSettings();

            ChoConsole.PauseLine();
        }
    }
}
