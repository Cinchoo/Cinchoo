using Cinchoo.Core.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public class ChoWindowsCBTHooks : ChoWindowsHooks
    {
        #region Key Events

        private event EventHandler<ChoCBTEventArgs> _activate;
        public event EventHandler<ChoCBTEventArgs> Activate
        {
            add
            {
                Subscribe();
                _activate += value;
            }

            remove
            {
                _activate -= value;
                Unsubscribe();
            }
        }

        private event EventHandler<ChoCBTEventArgs> _createWindow;
        public event EventHandler<ChoCBTEventArgs> CreateWindow
        {
            add
            {
                Subscribe();
                _createWindow += value;
            }

            remove
            {
                _createWindow -= value;
                Unsubscribe();
            }
        }

        private event EventHandler<ChoCBTEventArgs> _destroyWindow;
        public event EventHandler<ChoCBTEventArgs> DestroyWindow
        {
            add
            {
                Subscribe();
                _destroyWindow += value;
            }

            remove
            {
                _destroyWindow -= value;
                Unsubscribe();
            }
        }
        
        private event EventHandler<ChoCBTEventArgs> _minMax;
        public event EventHandler<ChoCBTEventArgs> MinMax
        {
            add
            {
                Subscribe();
                _minMax += value;
            }

            remove
            {
                _minMax -= value;
                Unsubscribe();
            }
        }
                
        private event EventHandler<ChoCBTEventArgs> _setFocus;
        public event EventHandler<ChoCBTEventArgs> SetFocus
        {
            add
            {
                Subscribe();
                _setFocus += value;
            }

            remove
            {
                _setFocus -= value;
                Unsubscribe();
            }
        }
             
        private event EventHandler<ChoCBTEventArgs> _sysCommand;
        public event EventHandler<ChoCBTEventArgs> SysCommand
        {
            add
            {
                Subscribe();
                _sysCommand += value;
            }

            remove
            {
                _sysCommand -= value;
                Unsubscribe();
            }
        }

        private event EventHandler<ChoCBTEventArgs> _QS;
        public event EventHandler<ChoCBTEventArgs> QS
        {
            add
            {
                Subscribe();
                _QS += value;
            }

            remove
            {
                _QS -= value;
                Unsubscribe();
            }
        }

        private event EventHandler<ChoCBTEventArgs> _keySkipped;
        public event EventHandler<ChoCBTEventArgs> KeySkipped
        {
            add
            {
                Subscribe();
                _keySkipped += value;
            }

            remove
            {
                _keySkipped -= value;
                Unsubscribe();
            }
        }

        #endregion Key Events

        #region Constructors

        public ChoWindowsCBTHooks()
            : base(ChoHookType.CBT)
        {
        }

        #endregion Constructors

        #region ChoWindowsHooks Overrides

        private void UnsubscribeKeyEvents()
        {
            if (_activate == null &&
                _createWindow == null &&
                _destroyWindow == null &&
                _minMax == null &&
                _setFocus == null &&
                _sysCommand == null &&
                _QS == null &&
                _keySkipped == null
                )
            {
                base.Unsubscribe();
            }
        }

        protected override bool HooksCallbackProc(int code, int wParam, IntPtr lParam)
        {
            //indicates if any of underlaing events set e.Handled flag
            bool handled = false;

            ChoCBTEventArgs e = ChoCBTEventArgs.New(wParam, lParam);

            switch (code)
            {
                case ChoUser32.HCBT_ACTIVATE:
                    _activate.Raise(this, e);
                    break;
                case ChoUser32.HCBT_CREATEWND:
                    _createWindow.Raise(this, e);
                    break;
                case ChoUser32.HCBT_DESTROYWND:
                    _destroyWindow.Raise(this, e);
                    break;
                case ChoUser32.HCBT_MINMAX:
                    _minMax.Raise(this, e);
                    break;
                case ChoUser32.HCBT_SETFOCUS:
                    _setFocus.Raise(this, e);
                    break;
                case ChoUser32.HCBT_SYSCOMMAND:
                    _sysCommand.Raise(this, e);
                    break;
                case ChoUser32.HCBT_QS:
                    _QS.Raise(this, e);
                    break;
                case ChoUser32.HCBT_KEYSKIPPED:
                    _keySkipped.Raise(this, e);
                    break;
            }

            return e.Handled;
        }

        #endregion ChoWindowsHooks Overrides
    }
}
