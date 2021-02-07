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
    public class ChoSplitter : Splitter
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

        public ChoSplitter()
        {
        }
 
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!SpliterLineVisible) return;
            var control = this;
            //paint the three dots'
            Point[] points = new Point[3];
            var w = control.Location.X;
            var h = control.Location.Y;
            var d = h; // control.SplitterDistance;
            var sW = w; // control.SplitterWidth;
            Pen controlDark2Px = new Pen(SystemColors.ControlDark, 2);

            //calculate the position of the points'
            if (control.Dock == DockStyle.Bottom ||
                control.Dock == DockStyle.Top)
            {
                points[0] = new Point(control.Size.Width / 2, control.Location.Y / 2);
                points[1] = new Point(control.Size.Width / 2 - 10, control.Location.Y / 2 - 10);
                points[2] = new Point(control.Size.Width / 2 + 10, control.Location.Y / 2 + 10);
            }
            else if (control.Dock == DockStyle.Left ||
                control.Dock == DockStyle.Right)
            {
                points[0] = new Point(control.Location.X, control.Size.Height / 2);
                points[1] = new Point(control.Location.X, control.Size.Height / 2 - 10);
                points[2] = new Point(control.Location.X, control.Size.Height / 2 + 10);
            }

            foreach (Point p in points)
            {
                //p.Offset(-2, -2);
                e.Graphics.FillEllipse(SystemBrushes.ControlDark,
                    new Rectangle(p, new Size(3, 3)));

                //p.Offset(1, 1);
                //e.Graphics.FillEllipse(SystemBrushes.ControlLight,
                //    new Rectangle(p, new Size(3, 3)));
            }


            //Point bottomLeft = new Point(0, this.Height);
            //Point topRight = new Point(this.Width, 0);
            //Pen controlDark = SystemPens.ControlDark;
            //Pen controlLightLight = SystemPens.ControlLightLight;
            //Pen controlDark2Px = new Pen(SystemColors.ControlDark, 2);
            //Point bottomRight = new Point(this.Width, this.Height);
            ////e.Graphics.DrawLine(
            ////    controlLightLight,
            ////    bottomLeft.X,
            ////    bottomLeft.Y - 2,
            ////    bottomRight.X,
            ////    bottomRight.Y - 2);
            ////e.Graphics.DrawLine(controlDark, bottomLeft, topRight);
            ////e.Graphics.DrawLine(
            ////    controlLightLight,
            ////    bottomLeft.X + 1,
            ////    bottomLeft.Y,
            ////    topRight.X,
            ////    topRight.Y + 1);
            ////e.Graphics.DrawLine(controlDark2Px, bottomLeft, bottomRight);
            //e.Graphics.DrawLine(controlDark2Px, bottomRight, topRight);
            ////int xNumberOfGripDots = this.Width / 4;
            ////for (int x = 1; x < xNumberOfGripDots; x++)
            ////{
            ////    for (int y = 1; y < 5 - x; y++)
            ////    {
            ////        DrawGripDot(e.Graphics, new Point(
            ////            this.Width - (y * 4), this.Height - (x * 4) - 1));
            ////    }
            ////}
        }
        private static void DrawGripDot(Graphics g, Point location)
        {
            g.FillRectangle(
                SystemBrushes.ControlLightLight, location.X + 1, location.Y + 1, 2, 2);
            g.FillRectangle(SystemBrushes.ControlDark, location.X, location.Y, 2, 2);
        }

        protected override void OnResize(EventArgs e)
        {
            this.SetRegion();
            base.OnResize(e);
        }

        private void SetRegion()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(new Point[] 
                    { 
    	                new Point(this.Location.X, this.Location.Y), 
    	                new Point(this.Location.X + this.Size.Width, this.Location.Y), 
    	                new Point(this.Location.X + this.Size.Width, this.Location.Y + this.Size.Height), 
    	                new Point(this.Location.X, this.Location.Y + this.Size.Height), 
                    });
            this.Region = new Region(path);
        }
    }
}
