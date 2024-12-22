using Cinchoo.Core;
using Cinchoo.Core.Configuration;
using Cinchoo.Core.Shell;

namespace HelloWorld
{
    [ChoNameValueConfigurationSection("appSettings")]
    public class AppSettings : ChoConfigurableObject
    {
        [ChoPropertyInfo("name", DefaultValue = "Mark")]
        public string Name;

        [ChoPropertyInfo("address", DefaultValue = "100, Madison Avenue, New York, NY 10010")]
        public string Address;
    }

    class Program
    {
        static void Main(string[] args)
        {
            AppSettings appSettings = new AppSettings();

            //Update the name to 'Raj', see it is updating to source
            appSettings.Name = "Raj";

            ChoConsole.PauseLine();
        }
    }
}
