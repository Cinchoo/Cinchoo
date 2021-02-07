using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Cinchoo.Core.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public POINT(System.Drawing.Point pt) : this(pt.X, pt.Y) { }

        public static implicit operator System.Drawing.Point(POINT p)
        {
            return new System.Drawing.Point(p.X, p.Y);
        }

        public static implicit operator POINT(System.Drawing.Point p)
        {
            return new POINT(p.X, p.Y);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEHOOKSTRUCT
    {
        public POINT Point;
        public IntPtr WndHandle;
        public uint HitTestCode;
        public IntPtr ExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSELLHOOKSTRUCT
    {
        public POINT Point;
        public int MouseData;
        public int Flags;
        public int Time;
        public int ExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBOARDHOOKSTRUCT
    {
        public int VKCode;
        public int ScanCode;
        public int Flags;
        public int Time;
        public int ExtraInfo;
    }
}
