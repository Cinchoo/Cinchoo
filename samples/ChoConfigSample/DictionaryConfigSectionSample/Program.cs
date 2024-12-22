using Cinchoo.Core;
using Cinchoo.Core.Configuration;
using Cinchoo.Core.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryConfigSectionSample
{
    [ChoDictionaryConfigurationSection("appSettings")]
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
