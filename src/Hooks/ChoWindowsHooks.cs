using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinchoo.Core;
using System.Runtime.InteropServices;
using System.Reflection;
using Cinchoo.Core.Win32;
using System.Windows.Forms;

namespace Cinchoo.Core
{
    public enum ChoHookType : int
    {
        MsgFilter = -1,
        JournalRecord = 0,
        JournalPlayback = 1,
        Keyboard = 2,
        GetMessage = 3,
        CallWndProc = 4,
        CBT = 5,
        SysMsgFilter = 6,
        Mouse = 7,
        Debug = 9,
        Shell = 10,
        ForegroundIdle = 11,
        CallWndProcRet = 12,
        KeyboardLL = 13,
        MouseLL = 14
    }

    public delegate int ChoHooksProc(int code, int wParam, IntPtr lParam);

    public abstract class ChoWindowsHooks : ChoDisposableObject
    {
        #region Instance Data Members (Private)

        private readonly object _padLock = new object();
        private int _hooksHandle = 0;
        private ChoHookType _hookType = ChoHookType.MsgFilter;
        private ChoHooksProc _hooksProc;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoWindowsHooks(ChoHookType hookType)
        {
            _hookType = hookType;
            Application.ApplicationExit += Application_ApplicationExit;
        }

        #endregion Constructors

        #region Instance Members (Public)

        public virtual void Subscribe()
        {
            if (_hooksHandle != 0)
                return;

            lock (_padLock)
            {
                // install Mouse hook only if it is not installed and must be installed
                if (_hooksHandle == 0)
                {
                    _hooksProc = HooksProc;

                    //install hook
                    _hooksHandle = ChoUser32.SetWindowsHookEx(
                        (int)_hookType,
                        _hooksProc, IntPtr.Zero,
                        //Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
                        0);
                    //If SetWindowsHookEx fails.
                    if (_hooksHandle == 0)
                        ChoWin32Exception.CheckNThrowException();
                }
            }
        }

        public virtual void Unsubscribe()
        {
            if (_hooksHandle == 0)
                return;

            lock (_padLock)
            {
                // install Mouse hook only if it is not installed and must be installed
                if (_hooksHandle != 0)
                {
                    //install hook
                    int result = ChoUser32.UnhookWindowsHookEx(_hooksHandle);
                    _hooksHandle = 0;
                    //If SetWindowsHookEx fails.
                    if (result == 0)
                        ChoWin32Exception.CheckNThrowException();
                }

            }
        }

        #endregion Instance Members (Public)

        #region Instance Members (Private)

        protected virtual int HooksProc(int code, int wParam, IntPtr lParam)
        {
            if (HooksCallbackProc(code, wParam, lParam))
                return -1;

            int hooksHandle = _hooksHandle;
            if (hooksHandle != 0)
                return ChoUser32.CallNextHookEx(hooksHandle, code, wParam, lParam);
            else
                return -1;
        }

        protected abstract bool HooksCallbackProc(int code, int wParam, IntPtr lParam);

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            Unsubscribe();
        }

        #endregion Instance Members (Private)

        #region Dispose Overrides

        protected override void Dispose(bool finalize)
        {
            Unsubscribe();
        }

        #endregion Dispose Overrides
    }
}