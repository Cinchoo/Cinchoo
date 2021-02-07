using Cinchoo.Core.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Cinchoo.Core
{
    public static class ChoWindowsHooksManager
    {
        #region Shared Data Members (Private)

        private static Lazy<ChoWindowsMouseHooks> _mouseHooksInstance = new Lazy<ChoWindowsMouseHooks>(() => new ChoWindowsMouseHooks(), true);
        private static Lazy<ChoWindowsKeyboardHooks> _keyHooksInstance = new Lazy<ChoWindowsKeyboardHooks>(() => new ChoWindowsKeyboardHooks(), true);

        private static int _oldMouseX;
        private static int _oldMouseY;

        #endregion Shared Data Members (Private)

        #region Mouse Events

        private static event EventHandler<ChoMouseEventArgs> _mouseMove;
        public static event EventHandler<ChoMouseEventArgs> MouseMove
        {
            add
            {
                SubscribeMouseEvents();
                _mouseMove += value;
            }

            remove
            {
                _mouseMove -= value;
                UnsubscribeMouseEvents();
            }
        }

        private static event EventHandler<ChoMouseEventArgs> _mouseUp;
        public static event EventHandler<ChoMouseEventArgs> MouseUp
        {
            add
            {
                SubscribeMouseEvents();
                _mouseUp += value;
            }

            remove
            {
                _mouseUp -= value;
                UnsubscribeMouseEvents();
            }
        }

        private static event EventHandler<ChoMouseEventArgs> _mouseDown;
        public static event EventHandler<ChoMouseEventArgs> MouseDown
        {
            add
            {
                SubscribeMouseEvents();
                _mouseDown += value;
            }

            remove
            {
                _mouseDown -= value;
                UnsubscribeMouseEvents();
            }
        }

        private static event EventHandler<ChoMouseEventArgs> _mouseClick;
        public static event EventHandler<ChoMouseEventArgs> MouseClick
        {
            add
            {
                SubscribeMouseEvents();
                _mouseClick += value;
            }

            remove
            {
                _mouseClick -= value;
                UnsubscribeMouseEvents();
            }
        }

        private static event EventHandler<ChoMouseEventArgs> _mouseDblClick;
        public static event EventHandler<ChoMouseEventArgs> MouseDblClick
        {
            add
            {
                SubscribeMouseEvents();
                _mouseDblClick += value;
            }

            remove
            {
                _mouseDblClick -= value;
                UnsubscribeMouseEvents();
            }
        }

        private static event EventHandler<ChoMouseEventArgs> _mouseWheel;
        public static event EventHandler<ChoMouseEventArgs> MouseWheel
        {
            add
            {
                SubscribeMouseEvents();
                _mouseWheel += value;
            }

            remove
            {
                _mouseWheel -= value;
                UnsubscribeMouseEvents();
            }
        }

        #endregion Mouse Events

        #region Key Events

        private static event EventHandler<ChoKeyPressEventArgs> _keyPress;
        public static event EventHandler<ChoKeyPressEventArgs> KeyPress
        {
            add
            {
                SubscribeKeyEvents();
                _keyPress += value;
            }

            remove
            {
                _keyPress -= value;
                UnsubscribeKeyEvents();
            }
        }

        private static event EventHandler<ChoKeyEventArgs> _keyDown;
        public static event EventHandler<ChoKeyEventArgs> KeyDown
        {
            add
            {
                SubscribeKeyEvents();
                _keyDown += value;
            }

            remove
            {
                _keyDown -= value;
                UnsubscribeKeyEvents();
            }
        }

        private static event EventHandler<ChoKeyEventArgs> _keyUp;
        public static event EventHandler<ChoKeyEventArgs> KeyUp
        {
            add
            {
                SubscribeKeyEvents();
                _keyUp += value;
            }

            remove
            {
                _keyUp -= value;
                UnsubscribeKeyEvents();
            }
        }

        #endregion Key Events

        #region Mouse Related Methods

        private static void SubscribeMouseEvents()
        {
            _mouseHooksInstance.Value.Subscribe();
        }

        private static void UnsubscribeMouseEvents()
        {
            if (_mouseWheel == null &&
                _mouseDblClick == null &&
                _mouseClick == null &&
                _mouseDown == null &&
                _mouseUp == null)
            {
                _mouseHooksInstance.Value.Unsubscribe();
            }
        }
        private static bool MouseHooksProc(int code, int wParam, IntPtr lParam)
        {
            bool handled = false;

            if (code >= 0)
            {
                //Marshall the data from callback.
                MOUSELLHOOKSTRUCT mouseHookStruct = (MOUSELLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MOUSELLHOOKSTRUCT));

                //detect button clicked
                MouseButtons button = MouseButtons.None;
                short mouseDelta = 0;
                int clickCount = 0;
                bool mouseDown = false;
                bool mouseUp = false;

                switch (wParam)
                {
                    case ChoUser32.WM_LBUTTONDOWN:
                        mouseDown = true;
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case ChoUser32.WM_LBUTTONUP:
                        mouseUp = true;
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case ChoUser32.WM_LBUTTONDBLCLK:
                        button = MouseButtons.Left;
                        clickCount = 2;
                        break;
                    case ChoUser32.WM_RBUTTONDOWN:
                        mouseDown = true;
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case ChoUser32.WM_RBUTTONUP:
                        mouseUp = true;
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case ChoUser32.WM_RBUTTONDBLCLK:
                        button = MouseButtons.Right;
                        clickCount = 2;
                        break;
                    case ChoUser32.WM_MOUSEWHEEL:
                        //If the message is WM_MOUSEWHEEL, the high-order word of MouseData member is the wheel delta. 
                        //One wheel click is defined as WHEEL_DELTA, which is 120. 
                        //(value >> 16) & 0xffff; retrieves the high-order word from the given 32-bit value
                        mouseDelta = (short)((mouseHookStruct.MouseData >> 16) & 0xffff);

                        //TODO: X BUTTONS (I havent them so was unable to test)
                        //If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP, 
                        //or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released, 
                        //and the low-order word is reserved. This value can be one or more of the following values. 
                        //Otherwise, MouseData is not used. 
                        break;
                }

                //generate event 
                ChoMouseEventArgs e = new ChoMouseEventArgs(
                                                   button,
                                                   clickCount,
                                                   mouseHookStruct.Point.X,
                                                   mouseHookStruct.Point.Y,
                                                   mouseDelta);

                //Mouse up
                if (mouseUp)
                    _mouseUp.Raise(null, e);
                 
                handled = handled || e.Handled;
                
                //Mouse down
                if (mouseDown)
                    _mouseDown.Raise(null, e);
                
                handled = handled || e.Handled;
                
                //If someone listens to click and a click is heppened
                if (clickCount > 0)
                    _mouseClick.Raise(null, e);
                
                handled = handled || e.Handled;
                
                //If someone listens to double click and a click is heppened
                if (clickCount == 2)
                    _mouseDblClick.Raise(null, e);
                
                handled = handled || e.Handled;
                
                //Wheel was moved
                if (mouseDelta != 0)
                    _mouseWheel.Raise(null, e);
                
                handled = handled || e.Handled;
                
                RaiseMouseMoveEvent(mouseHookStruct, e);
                
                handled = handled || e.Handled;
            }

            return handled;
        }

        private static void RaiseMouseMoveEvent(MOUSELLHOOKSTRUCT mouseHookStruct, ChoMouseEventArgs e)
        {
            EventHandler<ChoMouseEventArgs> mouseMove = _mouseMove;
            if (mouseMove == null) return;

            //If someone listens to move and there was a change in coordinates raise move event
            if (_oldMouseX != mouseHookStruct.Point.X || _oldMouseY != mouseHookStruct.Point.Y)
            {
                _oldMouseX = mouseHookStruct.Point.X;
                _oldMouseY = mouseHookStruct.Point.Y;
                mouseMove.Raise(null, e);
            }
        }

        #endregion Mouse Related Methods

        #region Key Related Methods
        
        private static void SubscribeKeyEvents()
        {
            _keyHooksInstance.Value.Subscribe();
        }

        private static void UnsubscribeKeyEvents()
        {
            if (_keyPress == null &&
                _keyDown == null &&
                _keyUp == null)
            {
                _keyHooksInstance.Value.Unsubscribe();
            }
        }

        private static bool RaiseKeyDownEvent(KEYBOARDHOOKSTRUCT keyboardHookStruct, out bool suppressKeyPress)
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

        private static bool RaiseKeyPressEvent(KEYBOARDHOOKSTRUCT keyboardHookStruct)
        {
            EventHandler<ChoKeyPressEventArgs> keyPress = _keyPress;
            if (keyPress == null) return false;

            ChoKeyPressEventArgs e = ChoKeyPressEventArgs.New(keyboardHookStruct);
            if (e == null) return false;
            
            keyPress.Raise(null, e);
            return e.Handled;
        }

        private static bool RaiseKeyUpEvent(KEYBOARDHOOKSTRUCT keyboardHookStruct, out bool suppressKeyPress)
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

        private static bool KeyHooksProc(int code, int wParam, IntPtr lParam)
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

        #endregion Key Related Methods
    }
}
