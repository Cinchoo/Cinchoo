using Cinchoo.Core;
using Cinchoo.Core.Diagnostics;
using Cinchoo.Core.Shell;
using Cinchoo.Core.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowlessTrayApp
{
    [ChoApplicationHost]
    public class ChoAppHost : ChoApplicationHost
    {
        protected override void ApplyGlobalApplicationSettingsOverrides(ChoGlobalApplicationSettings obj)
        {
            obj.TrayApplicationBehaviourSettings.TurnOn = true;
            obj.TrayApplicationBehaviourSettings.TooltipText = "Hello World!";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ChoApplication.Run(args);
        }
    }
}
