using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cinchoo.Core
{
    public partial class ChoFatalErrorMessageBox : Form
    {
        public string DetailMsg;

        public ChoFatalErrorMessageBox(string errMsg, string title = null, string detailMsg = null)
        {
            InitializeComponent();
            txtErrMsg.Text = errMsg;
            DetailMsg = detailMsg.IsNullOrWhiteSpace() ? errMsg : detailMsg;
        }

        private void ChoFatalErrorMessageBox_Load(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(DetailMsg);
            }
            catch { }
        }

        private void btnEmail_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "mailto:support?subject=Application Error&body={0}".FormatString(DetailMsg);
                proc.Start();
            }
            catch { }
        }

        public static void Show(string errMsg, string detailMsg = null)
        {
            ChoFatalErrorMessageBox frm = new ChoFatalErrorMessageBox(errMsg, detailMsg);
            frm.ShowDialog();
        }
    }
}
