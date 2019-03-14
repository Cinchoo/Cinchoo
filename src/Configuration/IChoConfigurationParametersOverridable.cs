using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core.Configuration
{
    public interface IChoConfigurationParametersOverridable
    {
        void OverrideParameters(ChoBaseConfigurationElement configurationElement);
    }
}
