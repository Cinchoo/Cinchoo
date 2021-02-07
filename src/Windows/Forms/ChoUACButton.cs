using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cinchoo.Core.Windows.Forms
{
    public class ChoUACButton : Button
    {
        public ChoUACButton()
        {
            ChoButtonEx.SetUACShield(this, true);
        }

        protected override void OnClick(EventArgs e)
        {
            if (ChoWindowsIdentity.IsAdministrator())
                base.OnClick(e);
            else
                ChoApplication.RestartAsAdmin();
        }
    }
}
