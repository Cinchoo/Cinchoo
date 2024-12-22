using Cinchoo.Core;
using Cinchoo.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public static class ChoConfigurationSectionEx
    {
        public static void Save(this ConfigurationSection section, string name, ConfigurationSaveMode mode = ConfigurationSaveMode.Modified)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "SectionName");
            ChoGuard.ArgumentNotNullOrEmpty(section, "Section");

            var config = ChoConfigurationManager.ApplicationConfiguration; // ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.Sections.Add(name, section);
            config.Save(mode);
        }
    }
}
