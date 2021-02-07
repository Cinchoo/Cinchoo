using Cinchoo.Core.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Cinchoo.Core
{
    public class ChoWindowsMouseHooks : ChoWindowsHooks
    {
        #region Enum

        private enum ChoMouseEventType
        {
            None,
            MouseDown,
            MouseUp,
            DoubleClick,
            MouseWheel,
            MouseMove
        }

        #endregion Enum


        #region Mouse Events

        private event EventHandler<ChoMouseEventArgs> _mouseMove;
        public event EventHandler<ChoMouseEventArgs> MouseMove
        {
            add
            {
                Subscribe();
                _mouseMove += value;
            }

            remove
            {
                _mouseMove -= value;
                Unsubscribe();
            }
        }

        private event EventHandler<ChoMouseEventArgs> _mouseUp;
        public event EventHandler<ChoMouseEventArgs> MouseUp
        {
            add
            {
                Subscribe();
                _mouseUp += value;
            }

            remove
            {
                _mouseUp -= value;
                Unsubscribe();
            }
        }

        private event EventHandler<ChoMouseEventArgs> _mouseDown;
        public event EventHandler<ChoMouseEventArgs> MouseDown
        {
            add
            {
                Subscribe();
                _mouseDown += value;
            }

            remove
            {
                _mouseDown -= value;
                Unsubscribe();
            }
        }

        private event EventHandler<ChoMouseEventArgs> _mouseClick;
        public event EventHandler<ChoMouseEventArgs> MouseClick
        {
            add
            {
                Subscribe();
                _mouseClick += value;
            }

            remove
            {
                _mouseClick -= value;
                Unsubscribe();
            }
        }

        private event EventHandler<ChoMouseEventArgs> _mouseDblClick;
        public event EventHandler<ChoMouseEventArgs> MouseDblClick
        {
            add
            {
                Subscribe();
                _mouseDblClick += value;
            }

            remove
            {
                _mouseDblClick -= value;
                Unsubscribe();
            }
        }

        private event EventHandler<ChoMouseEventArgs> _mouseWheel;
        public event EventHandler<ChoMouseEventArgs> MouseWheel
        {
            add
            {
                Subscribe();
                _mouseWheel += value;
            }

            remove
            {
                _mouseWheel -= value;
                Unsubscribe();
            }
        }

        #endregion Mouse Events

        #region Instance Data Members

        private int _oldMouseX;
        private int _oldMouseY;

        #endregion

        #region Constructors

        public ChoWindowsMouseHooks()
            : base(ChoHookType.MouseLL)
        {
        }

        #endregion Constructors

        #region ChoWindowsHooks Overrides

        public override void Unsubscribe()
        {
            if (_mouseWheel == null &&
                _mouseDblClick == null &&
                _mouseClick == null &&
                _mouseDown == null &&
                _mouseUp == null)
            {
                base.Unsubscribe();
            }
        }

        protected override bool HooksCallbackProc(int code, int wParam, IntPtr lParam)
        {
            if (code > -1)
            {
                //Marshall the data from callback.
                MOUSELLHOOKSTRUCT mouseHookStruct = (MOUSELLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MOUSELLHOOKSTRUCT));

                MouseButtons button = GetButton(wParam);
                ChoMouseEventType eventType = GetEventType(wParam);

                ChoMouseEventArgs e = new ChoMouseEventArgs(
                    button,
                    (eventType == ChoMouseEventType.DoubleClick ? 2 : 1),
                    mouseHookStruct.Point.X,
                    mouseHookStruct.Point.Y,
                    (eventType == ChoMouseEventType.MouseWheel ? (short)((mouseHookStruct.MouseData >> 16) & 0xffff) : 0));

                // Prevent multiple Right Click events (this probably happens for popup menus)
                if (button == MouseButtons.Right && mouseHookStruct.Flags != 0)
                    eventType = ChoMouseEventType.None;

                switch (eventType)
                {
                    case ChoMouseEventType.MouseDown:
                        _mouseDown.Raise(this, e);
                        break;
                    case ChoMouseEventType.MouseUp:
                        _mouseClick.Raise(this, e);
                        _mouseUp.Raise(this, e);
                        break;
                    case ChoMouseEventType.DoubleClick:
                        _mouseDblClick.Raise(this, e);
                        break;
                    case ChoMouseEventType.MouseWheel:
                        _mouseWheel.Raise(this, e);
                        break;
                    case ChoMouseEventType.MouseMove:
                        //If someone listens to move and there was a change in coordinates raise move event
                        if (_oldMouseX != mouseHookStruct.Point.X || _oldMouseY != mouseHookStruct.Point.Y)
                        {
                            _oldMouseX = mouseHookStruct.Point.X;
                            _oldMouseY = mouseHookStruct.Point.Y;
                            _mouseMove.Raise(this, e);
                        }
                        break;
                    default:
                        break;
                }

                return e.Handled;
            }

            return false;
        }

        #endregion ChoWindowsHooks Overrides

        #region Instance Members (Private)

        private MouseButtons GetButton(Int32 wParam)
        {
            switch (wParam)
            {
                case ChoUser32.WM_LBUTTONDOWN:
                case ChoUser32.WM_LBUTTONUP:
                case ChoUser32.WM_LBUTTONDBLCLK:
                    return MouseButtons.Left;
                case ChoUser32.WM_RBUTTONDOWN:
                case ChoUser32.WM_RBUTTONUP:
                case ChoUser32.WM_RBUTTONDBLCLK:
                    return MouseButtons.Right;
                case ChoUser32.WM_MBUTTONDOWN:
                case ChoUser32.WM_MBUTTONUP:
                case ChoUser32.WM_MBUTTONDBLCLK:
                    return MouseButtons.Middle;
                default:
                    return MouseButtons.None;
            }

        }

        private ChoMouseEventType GetEventType(Int32 wParam)
        {
            switch (wParam)
            {
                case ChoUser32.WM_LBUTTONDOWN:
                case ChoUser32.WM_RBUTTONDOWN:
                case ChoUser32.WM_MBUTTONDOWN:
                    return ChoMouseEventType.MouseDown;
                case ChoUser32.WM_LBUTTONUP:
                case ChoUser32.WM_RBUTTONUP:
                case ChoUser32.WM_MBUTTONUP:
                    return ChoMouseEventType.MouseUp;
                case ChoUser32.WM_LBUTTONDBLCLK:
                case ChoUser32.WM_RBUTTONDBLCLK:
                case ChoUser32.WM_MBUTTONDBLCLK:
                    return ChoMouseEventType.DoubleClick;
                case ChoUser32.WM_MOUSEWHEEL:
                    return ChoMouseEventType.MouseWheel;
                case ChoUser32.WM_MOUSEMOVE:
                    return ChoMouseEventType.MouseMove;
                default:
                    return ChoMouseEventType.None;
            }
        }

        #endregion Instance Members (Private)
    }
}
