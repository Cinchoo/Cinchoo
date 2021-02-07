using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public class ChoWindowsHooksEventProvider : Component
    {
        #region Instance Data Members (Private)

        private Lazy<ChoWindowsMouseHooks> _mouseHooksInstance = new Lazy<ChoWindowsMouseHooks>(() => new ChoWindowsMouseHooks(), true);
        private Lazy<ChoWindowsKeyboardHooks> _keyHooksInstance = new Lazy<ChoWindowsKeyboardHooks>(() => new ChoWindowsKeyboardHooks(), true);

        #endregion Instance Data Members (Private)

        #region Mouse Events

        /// <summary>
        /// Occurs when the mouse pointer is moved. 
        /// </summary>
        public event EventHandler<ChoMouseEventArgs> MouseMove
        {
            add
            {
                _mouseHooksInstance.Value.MouseMove += value;
            }

            remove
            {
                _mouseHooksInstance.Value.MouseMove -= value;
            }
        }

        public event EventHandler<ChoMouseEventArgs> MouseUp
        {
            add
            {
                _mouseHooksInstance.Value.MouseUp += value;
            }

            remove
            {
                _mouseHooksInstance.Value.MouseUp -= value;
            }
        }

        public event EventHandler<ChoMouseEventArgs> MouseDown
        {
            add
            {
                _mouseHooksInstance.Value.MouseDown += value;
            }

            remove
            {
                _mouseHooksInstance.Value.MouseDown -= value;
            }
        }

        public event EventHandler<ChoMouseEventArgs> MouseClick
        {
            add
            {
                _mouseHooksInstance.Value.MouseClick += value;
            }

            remove
            {
                _mouseHooksInstance.Value.MouseClick -= value;
            }
        }

        public event EventHandler<ChoMouseEventArgs> MouseDblClick
        {
            add
            {
                _mouseHooksInstance.Value.MouseDblClick += value;
            }

            remove
            {
                _mouseHooksInstance.Value.MouseDblClick -= value;
            }
        }

        public event EventHandler<ChoMouseEventArgs> MouseWheel
        {
            add
            {
                _mouseHooksInstance.Value.MouseWheel += value;
            }

            remove
            {
                _mouseHooksInstance.Value.MouseWheel -= value;
            }
        }

        #endregion Mouse Events

        #region Key Events

        public event EventHandler<ChoKeyPressEventArgs> KeyPress
        {
            add
            {
                _keyHooksInstance.Value.KeyPress += value;
            }

            remove
            {
                _keyHooksInstance.Value.KeyPress -= value;
            }
        }

        public event EventHandler<ChoKeyEventArgs> KeyDown
        {
            add
            {
                _keyHooksInstance.Value.KeyDown += value;
            }

            remove
            {
                _keyHooksInstance.Value.KeyDown -= value;
            }
        }

        public event EventHandler<ChoKeyEventArgs> KeyUp
        {
            add
            {
                _keyHooksInstance.Value.KeyUp += value;
            }

            remove
            {
                _keyHooksInstance.Value.KeyUp -= value;
            }
        }

        #endregion Key Events
    }
}
