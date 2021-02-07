using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cinchoo.Core.Windows.Forms
{
    public class ChoSplitContainer : SplitContainer
    {
        //private Pen _dashedPen = new Pen(Color.DarkRed, 1);

        private bool _splitterLineVisible;
        [Browsable(true)]
        [DisplayName("SplitterLineVisible")]
        [Description("Splitter line visible")]
        public bool SpliterLineVisible
        {
            get { return _splitterLineVisible; }
            set
            {
                _splitterLineVisible = value;
                if (DesignMode && Parent != null)
                    Parent.Refresh(); // Refreshes the client area of the parent control
            }
        }

        //[Browsable(true)]
        //[DisplayName("SplitterLineColor")]
        //[Description("Color of the splitter line")]
        //public Color SpliterLineColor
        //{
        //    get { return _dashedPen.Color; }
        //    set
        //    {
        //        _dashedPen = new Pen(value, 1);
        //        if (DesignMode && Parent != null)
        //            Parent.Refresh(); // Refreshes the client area of the parent control
        //    }
        //}

        //[Browsable(true)]
        //[DisplayName("SplitterLineStyle")]
        //[Description("Style of the splitter line")]
        //public DashStyle SpliterLineDashStyle
        //{
        //    get { return _dashedPen.DashStyle; }
        //    set
        //    {
        //        _dashedPen.DashStyle = value;
        //        if (DesignMode && Parent != null)
        //            Parent.Refresh(); // Refreshes the client area of the parent control
        //    }
        //}

        public ChoSplitContainer()
        {
        }
 
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!SpliterLineVisible) return;

            var control = this;
            //paint the three dots'
            Point[] points = new Point[3];
            var w = control.Width;
            var h = control.Height;
            var d = control.SplitterDistance;
            var sW = control.SplitterWidth;

            //calculate the position of the points'
            if (control.Orientation == Orientation.Horizontal)
            {
                points[0] = new Point((w / 2), d + (sW / 2));
                points[1] = new Point(points[0].X - 10, points[0].Y);
                points[2] = new Point(points[0].X + 10, points[0].Y);
            }
            else
            {
                points[0] = new Point(d + (sW / 2), (h / 2));
                points[1] = new Point(points[0].X, points[0].Y - 10);
                points[2] = new Point(points[0].X, points[0].Y + 10);
            }

            foreach (Point p in points)
            {
                p.Offset(-2, -2);
                e.Graphics.FillEllipse(SystemBrushes.ControlDark,
                    new Rectangle(p, new Size(3, 3)));

                p.Offset(1, 1);
                e.Graphics.FillEllipse(SystemBrushes.ControlLight,
                    new Rectangle(p, new Size(3, 3)));
            }
        }
    }
}
