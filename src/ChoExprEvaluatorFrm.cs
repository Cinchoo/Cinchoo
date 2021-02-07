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
    public partial class ChoExprEvaluatorFrm : Form
    {
        public event EventHandler<ChoExprEvaluateEventArgs> Evaluate;

        public ChoExprEvaluatorFrm(string contextInfo = null)
        {
            InitializeComponent();

            ChoFramework.Initialize();
            if (!contextInfo.IsNull())
                txtContextInfo.Text = contextInfo;
            txtHelp.Text = ChoPropertyManagerSettings.Me.GetHelpText();
        }

        private void btnEval_Click(object sender, EventArgs e)
        {
            txtOutput.Text = null;
            try
            {
                string t = txtExpr.Text;
                if (t.IsNullOrWhiteSpace()) return;

                string ctx = txtContextInfo.Text;

                ChoExprEvaluateEventArgs args = new ChoExprEvaluateEventArgs();
                args.ExprText = t;
                args.ContextText = ctx;
                args.SingleExpr = chkSingleExpr.Checked;

                Evaluate.Raise(this, args);

                if (chkSingleExpr.Checked)
                {
                    if (!args.Handled)
                        txtOutput.Text = args.ExprText.ExpandProperties(args.ContextText);
                    else
                        txtOutput.Text = args.ContextText;
                }
                else
                {
                    if (!args.Handled)
                        txtOutput.Text = args.ExprText.SplitNExpandProperties(args.ContextText);
                    else
                        txtOutput.Text = args.ContextText;
                }
            }
            catch (Exception ex)
            {
                ChoMessageBox.Show(ex, "Error evaluating expression.", this.Text, "OK", null);
            }
        }

        public string ExpressionText
        {
            get { return txtExpr.Text; }
            set { txtExpr.Text = value; }
        }

        private void ChoExprEvaluatorFrm_Load(object sender, EventArgs e)
        {
        }

        private bool _isEscaped = false;
        private void btnEscapeText_Click(object sender, EventArgs e)
        {
            _isEscaped = !_isEscaped;
            txtExpr.Text = _isEscaped ? txtExpr.Text.EscapeSourceCode() : txtExpr.Text.UnescapeSourceCode();
            btnEscapeText.Text = !_isEscaped ? "Escape" : "Unescape";
        }

        public static void Show(string exprText = null)
        {
            ChoExprEvaluatorFrm frm = new ChoExprEvaluatorFrm();
            frm.txtExpr.Text = exprText;
            frm.ShowDialog();
        }
    }

    public class ChoExprEvaluateEventArgs : EventArgs
    {
        public string ExprText;
        public string ContextText;
        public bool Handled;
        public bool SingleExpr;
    }
}
