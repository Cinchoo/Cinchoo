using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;

namespace Cinchoo.Core.Forms
{
    public partial class ChoMainForm : Form
    {
        private readonly bool _visible = true;

        internal ChoMainForm(bool visible) : this()
        {
            _visible = visible;
        }

        public ChoMainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.ShowInTaskbar = ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.ShowInTaskbar;
            this.Visible = _visible;

            base.OnLoad(e);
        }
    }
}
