namespace Cinchoo.Core.ServiceProcess
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Configuration.Install;
    using System.ServiceProcess;
    using System.ComponentModel;

    #endregion NameSpaces

    [RunInstaller(true)]
    public sealed class ChoServiceInstaller : Installer
    {
        public ChoServiceInstaller()
		{
            ChoService.ServiceInstallerInitialization(this, new ChoServiceInstallerEventArgs(ChoServiceProcessInstallerSettings.Me, ChoServiceInstallerSettings.Me));
            ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();

            ChoApplication.WriteToEventLog(ChoServiceProcessInstallerSettings.Me.ToString());
            processInstaller.Account = ChoServiceProcessInstallerSettings.Me.Account;
            if (ChoServiceProcessInstallerSettings.Me.Account == ServiceAccount.User)
            {
                processInstaller.Username = ChoServiceProcessInstallerSettings.Me.UserName;
                processInstaller.Password = ChoServiceProcessInstallerSettings.Me.Password;
            }

            ChoApplication.WriteToEventLog(ChoServiceInstallerSettings.Me.ToString());
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            serviceInstaller.DisplayName = ChoServiceInstallerSettings.Me.DisplayName;
            serviceInstaller.StartType = ChoServiceInstallerSettings.Me.ServiceStartMode;
            serviceInstaller.ServiceName = ChoServiceInstallerSettings.Me.ServiceName;
            serviceInstaller.DelayedAutoStart = ChoServiceInstallerSettings.Me.DelayedAutoStart;
            serviceInstaller.Description = ChoServiceInstallerSettings.Me.Description;

			this.Installers.Add(processInstaller);
			this.Installers.Add(serviceInstaller);
		}

        public override string HelpText
        {
            get { return ChoServiceProcessInstallerSettings.Me.HelpText; }
        }
    }
}
