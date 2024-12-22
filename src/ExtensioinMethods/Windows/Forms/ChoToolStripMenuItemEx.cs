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
using System.Windows.Forms;

namespace Cinchoo.Core
{
    public static class ChoToolStripMenuItemEx
    {
        public static void SetUACShield(this ToolStripMenuItem @this, bool showShield = true)
        {
            if (!ChoWindowsIdentity.IsAdministrator() && ChoEnvironment.AtLeastVista())
            {
                //SHSTOCKICONINFO iconResult = new SHSTOCKICONINFO();
                //iconResult.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(iconResult);

                //ChoShell32.SHGetStockIconInfo(
                //    SHSTOCKICONID.SIID_SHIELD,
                //    SHGSI.SHGSI_ICON | SHGSI.SHGSI_SMALLICON,
                //    ref iconResult);

                //@this.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                //@this.Image = Bitmap.FromHicon(iconResult.hIcon);
                @this.ImageScaling = ToolStripItemImageScaling.None;
                @this.Image = ChoEnvironment.GetUACShieldImage();
            }
        }
    }
}
