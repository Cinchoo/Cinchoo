using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cinchoo.Core.Windows.Forms
{
    public class ChoLabel : Control
    {
        public ChoLabel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.CacheText
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.UserPaint, true);
        }
        protected override void OnTextChanged(EventArgs e) { base.OnTextChanged(e); Invalidate(); }
        protected override void OnFontChanged(EventArgs e) { base.OnFontChanged(e); Invalidate(); }

        static readonly StringFormat format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawString(Text, Font, SystemBrushes.ControlText, ClientRectangle, format);
        }
    }
}
