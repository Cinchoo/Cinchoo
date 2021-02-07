using Cinchoo.Core.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public class ChoCBTEventArgs : EventArgs
    {
        public IntPtr Handle;        // Win32 handle of the window 
        public string Title;         // caption of the window 
        public string ClassName;     // class of the window 
        public bool IsDialogWindow;  // whether it's a popup dialog 
        public bool Handled;

        public static ChoCBTEventArgs New(int wParam, IntPtr lParam)
        {
            ChoCBTEventArgs e = new ChoCBTEventArgs();

            // Cache the window's class name 
            StringBuilder sb1 = new StringBuilder();
            sb1.Capacity = 40;
            ChoCore32.GetClassName((IntPtr)wParam, sb1, 40);
            e.ClassName = sb1.ToString();

            // Cache the window's title bar 
            StringBuilder sb2 = new StringBuilder();
            //sb2.Capacity = 256;
            //GetWindowText(m_hwnd, sb2, 256);
            //m_title = sb2.ToString();

            //// Cache the dialog flag 
            //m_isDialog = (m_class == "#32770");
            
            return e;
        }
    } 

}
