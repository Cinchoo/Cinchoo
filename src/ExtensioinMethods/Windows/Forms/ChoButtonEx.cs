using Cinchoo.Core;
using Cinchoo.Core.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms
{
    public static class ChoButtonEx
    {
        public static void SetUACShield(this Button @this, bool showShield = true)
        {
            if (!ChoWindowsIdentity.IsAdministrator() && ChoEnvironment.AtLeastVista())
            {
                //Note: make sure the button FlatStyle = FlatStyle.System
                @this.FlatStyle = FlatStyle.System;
                // BCM_SETSHIELD = 0x0000160C
                ChoUser32.SendMessage(@this.Handle, ChoUser32.BCM_SETSHIELD, 0, showShield ? 0xFFFFFFFF : 0);
            }
        }
    }
}
