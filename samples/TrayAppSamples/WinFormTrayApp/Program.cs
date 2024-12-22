using Cinchoo.Core;
using Cinchoo.Core.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormTrayApp
{
    [ChoApplicationHost]
    public class ChoAppHost : ChoApplicationHost
    {
        protected override void ApplyGlobalApplicationSettingsOverrides(ChoGlobalApplicationSettings obj)
        {
            obj.TrayApplicationBehaviourSettings.TurnOn = true;
        }

        public override object MainWindowObject
        {
            get
            {
                return new MainForm();
            }
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
