using Cinchoo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WPFTrayApp
{
    [ChoApplicationHost]
    public class ChoAppHost : ChoApplicationHost
    {
        protected override void ApplyGlobalApplicationSettingsOverrides(ChoGlobalApplicationSettings obj)
        {
            obj.TrayApplicationBehaviourSettings.TurnOn = true;
            obj.TrayApplicationBehaviourSettings.ContextMenuSettings.DisplayShowInTaskbarMenuItem = false;
            obj.TrayApplicationBehaviourSettings.FontSettings.FontSize = 9;
        }

        public override object MainWindowObject
        {
            get
            {
                return new MainWindow();
            }
        }

        protected override void AfterNotifyIconConstructed(Cinchoo.Core.Windows.Forms.ChoNotifyIcon ni)
        {
            ni.Text = "AfterConstruct";
            ni.ShowText("R");
        }

        public override void OnTrayAppAboutMenuClicked(object sender, EventArgs e)
        {
            MessageBox.Show("About Clicked");
        }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main(string[] args)
        {
            ChoApplication.Run(args);
        }
    }
}
