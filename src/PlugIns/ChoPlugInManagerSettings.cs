using Cinchoo.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    [ChoNameValueConfigurationSection("plugInManagerSettings")]
    public class ChoPlugInManagerSettings : ChoConfigurableObject
    {
        [ChoPropertyInfo("stopRequestTimeout", DefaultValue = "5000")]
        [Description("Timeout period (ms) the plugin manager will wait after stop request placed.")]
        public int StopRequestTimeout;

        protected override void OnAfterConfigurationObjectLoaded()
        {
            if (StopRequestTimeout < -1)
                StopRequestTimeout = 5000;
        }
    }
}
