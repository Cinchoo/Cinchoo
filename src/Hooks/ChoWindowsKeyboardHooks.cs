using Cinchoo.Core.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Cinchoo.Core
{
    public class ChoWindowsKeyboardHooks : ChoWindowsHooks
    {
        #region Key Events

        private event EventHandler<ChoKeyPressEventArgs> _keyPress;
        public event EventHandler<ChoKeyPressEventArgs> KeyPress
        {
            add
            {
                Subscribe();
                _keyPress += value;
            }

            remove
            {
                _keyPress -= value;
                Unsubscribe();
            }
        }

        private event EventHandler<ChoKeyEventArgs> _keyDown;
        public event EventHandler<ChoKeyEventArgs> KeyDown
        {
            add
            {
                Subscribe();
                _keyDown += value;
            }

            remove
            {
                _keyDown -= value;
                Unsubscribe();
            }
        }

        private event EventHandler<ChoKeyEventArgs> _keyUp;
        public event EventHandler<ChoKeyEventArgs> KeyUp
        {
            add
            {
                Subscribe();
                _keyUp += value;
            }

            remove
            {
                _keyUp -= value;
                Unsubscribe();
            }
        }

        #endregion Key Events
        
        #region Constructors

        public ChoWindowsKeyboardHooks()
            : base(ChoHookType.KeyboardLL)
        {
        }

        #endregion Constructors

        #region ChoWindowsHooks Overrides

        public override void Unsubscribe()
        {
            if (_keyPress == null &&
                _keyDown == null &&
                _keyUp == null)
            {
                base.Unsubscribe();
            }
        }

        protected override bool HooksCallbackProc(int code, int wParam, IntPtr lParam)
        {
            //indicates if any of underlaing events set e.Handled flag
            bool handled = false;
            bool suppressKeyPress = false;

            if (code >= 0)
            {
                //read structure KeyboardHookStruct at lParam
                KEYBOARDHOOKSTRUCT keyboardHookStruct = (KEYBOARDHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KEYBOARDHOOKSTRUCT));

                //raise KeyDown
                if (wParam == ChoUser32.WM_KEYDOWN || wParam == ChoUser32.WM_SYSKEYDOWN)
                    handled = handled || RaiseKeyDownEvent(keyboardHookStruct, out suppressKeyPress);
                // raise KeyUp
                else if (wParam == ChoUser32.WM_KEYUP || wParam == ChoUser32.WM_SYSKEYUP)
                    handled = handled || RaiseKeyUpEvent(keyboardHookStruct, out suppressKeyPress);

                // raise KeyPress
                if (wParam == ChoUser32.WM_KEYDOWN && !handled && !suppressKeyPress)
                    handled = handled || RaiseKeyPressEvent(keyboardHookStruct);
            }

            return handled;
        }

        #endregion ChoWindowsHooks Overrides

        #region Instance Members (Private)

        private bool RaiseKeyDownEvent(KEYBOARDHOOKSTRUCT keyboardHookStruct, out bool suppressKeyPress)
        {
            suppressKeyPress = false;

            EventHandler<ChoKeyEventArgs> keyDown = _keyDown;
            if (keyDown == null) return false;

            ChoKeyEventArgs e = ChoKeyEventArgs.New(keyboardHookStruct);
            if (e == null) return false;

            keyDown.Raise(null, e);
            suppressKeyPress = e.SuppressKeyPress;
            return e.Handled;
        }

        private bool RaiseKeyPressEvent(KEYBOARDHOOKSTRUCT keyboardHookStruct)
        {
            EventHandler<ChoKeyPressEventArgs> keyPress = _keyPress;
            if (keyPress == null) return false;

            ChoKeyPressEventArgs e = ChoKeyPressEventArgs.New(keyboardHookStruct);
            if (e == null) return false;

            keyPress.Raise(null, e);
            return e.Handled;
        }

        private bool RaiseKeyUpEvent(KEYBOARDHOOKSTRUCT keyboardHookStruct, out bool suppressKeyPress)
        {
            suppressKeyPress = false;

            EventHandler<ChoKeyEventArgs> keyUp = _keyUp;
            if (keyUp == null) return false;

            ChoKeyEventArgs e = ChoKeyEventArgs.New(keyboardHookStruct);
            if (e == null) return false;

            keyUp.Raise(null, e);
            suppressKeyPress = e.SuppressKeyPress;
            return e.Handled;
        }

        #endregion Instance Members (Private)
    }
}
