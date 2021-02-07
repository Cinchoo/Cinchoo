using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cinchoo.Core.Win32;
using Cinchoo.Core.ServiceProcess;

namespace Cinchoo.Core
{
    public partial class ChoChooseEnvironmentFrm : Form
    {
        private BindingSource _bs = new BindingSource();

        public ChoChooseEnvironmentFrm()
        {
            //ChoApplication.ApplicationMode = ChoApplicationMode.Windows;
            InitializeComponent();
        }

        private void ChoChooseEnvironmentFrm_Load(object sender, EventArgs e)
        {
            ChoEnvironmentDetails environmentDetails = null;
            this.ActiveControl = cmbEnvironments;

            ChoSharedEnvironmentManager sharedEnvironmentManager = new ChoSharedEnvironmentManager();
            environmentDetails = ChoEnvironmentSettings.GetEnvironmentDetails();
            if (environmentDetails != null)
            {
                if (environmentDetails.Freeze)
                {
                    btnOK_Click(this, null);
                    return;
                }
            }
            
            if (sharedEnvironmentManager.EnvironmentDetails != null
                && sharedEnvironmentManager.EnvironmentDetails.Length >= 1)
            {
                _bs.DataSource = new List<ChoEnvironmentDetails>(sharedEnvironmentManager.EnvironmentDetails);
                cmbEnvironments.DataSource = _bs;
                cmbEnvironments.DisplayMember = "Name";
                cmbEnvironments.ValueMember = "Name";

                txtEnvComment.DataBindings.Add("Text", _bs, "Comment");

                if (sharedEnvironmentManager.EnvironmentDetails.Length == 1)
                    btnOK_Click(this, null);

                if (environmentDetails != null)
                    cmbEnvironments.SelectedItem = environmentDetails.Name;
            }
            else
                btnOK_Click(this, null);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cmbEnvironments.SelectedValue != null)
                ChoAppFrxSettings.Me.AppEnvironment = cmbEnvironments.SelectedValue.ToString();

            //ChoFramework.Initialize();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private static bool _isShown = false;
        public static void ShowWnd()
        {
            if (_isShown)
                return;

            _isShown = true;
            if (Environment.UserInteractive)
            {
                if (!ChoServiceCommandLineArgs.HasServiceParams())
                {
                    if (ChoApplication.ApplicationMode == ChoApplicationMode.Windows
                        || ChoApplication.ApplicationMode == ChoApplicationMode.Console)
                    {
                        if (!ChoAppFrxSettings.Me.DoNotShowEnvSelectionWnd)
                        {
                            ChoEnvironmentDetails environmentDetails = null;

                            ChoSharedEnvironmentManager sharedEnvironmentManager = new ChoSharedEnvironmentManager();
                            environmentDetails = ChoEnvironmentSettings.GetEnvironmentDetails();
                            if (environmentDetails != null)
                            {
                                if (environmentDetails.Freeze)
                                    return;
                            }

                            if (sharedEnvironmentManager.EnvironmentDetails != null
                                && sharedEnvironmentManager.EnvironmentDetails.Length >= 1)
                            {
                                if (sharedEnvironmentManager.EnvironmentDetails.Length == 1)
                                    return;

                                ChoChooseEnvironmentFrm frm1 = new ChoChooseEnvironmentFrm();
                                if (frm1.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                                    Environment.Exit(-101);
                            }
                        }
                    }
                }
            }
        }
    }
}
