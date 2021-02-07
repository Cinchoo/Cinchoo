using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Cinchoo.Core.Win32
{
    public static class ChoCore32
    {
        private const string CoreDllName = "coredll.dll";

        [DllImport(CoreDllName)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder buf, int nMaxCount);
    }
}
