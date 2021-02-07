using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cinchoo.Core
{
    /// <summary>
    /// Provides data for the MouseClickExt and MouseMoveExt events. It also provides a property Handled.
    /// Set this property to <b>true</b> to prevent further processing of the event in other applications.
    /// </summary>
    public class ChoMouseEventArgs : MouseEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the MouseEventArgs class. 
        /// </summary>
        /// <param name="button">One of the MouseButtons values indicating which mouse button was pressed.</param>
        /// <param name="clicks">The number of times a mouse button was pressed.</param>
        /// <param name="x">The x-coordinate of a mouse click, in pixels.</param>
        /// <param name="y">The y-coordinate of a mouse click, in pixels.</param>
        /// <param name="delta">A signed count of the number of detents the wheel has rotated.</param>
        public ChoMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
            : base(button, clicks, x, y, delta)
        { }

        /// <summary>
        /// Set this property to <b>true</b> inside your event handler to prevent further processing of the event in other applications.
        /// </summary>
        public bool Handled
        {
            get;
            set;
        }
    }
}
