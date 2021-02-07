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
    public partial class ChoFatalErrorMessageBoxFrm : Form
    {
        public string DetailMsg;

        public ChoFatalErrorMessageBoxFrm(string errMsg, string detailMsg = null)
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
            Clipboard.SetText(DetailMsg);
        }

        private void btnEmail_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "mailto:support?subject=Application Error&body={0}".FormatString(DetailMsg);
            proc.Start();
        }
    }
}
