using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Cinchoo.Core.Win32
{
    public static class ChoGDI32
    {
        private const string GDI32DllName = "gdi32.dll";

        [DllImport(GDI32DllName, CharSet = CharSet.Auto)]
        public static extern IntPtr GetStockObject(int nIndex);

        [DllImport(GDI32DllName, CharSet = CharSet.Auto)]
        public static extern bool GetTextMetrics(HandleRef hDC, [In, Out] TEXTMETRIC tm);
    }
}
