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
    public partial class ChoMessageBox : Form
    {
        private string _detailErrMsg;
        private string _errMsg;
        private string _title;

        public ChoMessageBox(Exception ex, string errMsg = null, string title = null)
        {
            ChoGuard.ArgumentNotNullOrEmpty(ex, "Exception");

            InitializeComponent();
            Initialize(ChoApplicationException.ToString(ex), errMsg, title);
        }

        public ChoMessageBox(string detailErrMsg, string errMsg = null, string title = null)
        {
            ChoGuard.ArgumentNotNullOrEmpty(detailErrMsg, "DetailErrMsg");
            InitializeComponent();
            Initialize(detailErrMsg, errMsg, title);
        }

        private void Initialize(string detailErrMsg, string errMsg, string title)
        {
            _detailErrMsg = detailErrMsg;
            _errMsg = errMsg;
            _title = title;
            this.Text = _title;
            if (!errMsg.IsNullOrWhiteSpace())
                lblErrMsg.Text = errMsg;
            txtDetailErrMsg.Text = detailErrMsg;
        }

        private void btnEmail_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "mailto:support?subject=Application Error&body={0}".FormatString(_detailErrMsg);
                proc.Start();
            }
            catch { }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(_detailErrMsg);
            }
            catch { }
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ChoMessageBox_Load(object sender, EventArgs e)
        {

        }

        public static DialogResult Show(string detailErrMsg, string errMsg = null, string title = null)
        {
            ChoMessageBox frm = new ChoMessageBox(detailErrMsg, errMsg, title);
            return frm.ShowDialog();
        }

        public static DialogResult Show(Exception ex, string errMsg = null, string title = null, string btnYesText = "Yes", string btnNoText = "No")
        {
            ChoMessageBox frm = new ChoMessageBox(ex, errMsg, title);
            if (!btnNoText.IsNullOrWhiteSpace())
            {
                frm.btnNo.Text = btnNoText;
                frm.btnNo.Visible = true;
            }
            else
                frm.btnNo.Visible = false;

            if (!btnYesText.IsNullOrWhiteSpace())
            {
                frm.btnYes.Text = btnYesText;
                frm.btnYes.Visible = true;
            }
            else
                frm.btnYes.Visible = false;

            return frm.ShowDialog();
        }
    }
}
