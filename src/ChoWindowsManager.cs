namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Security;
    using Cinchoo.Core.Win32;
    using System.Diagnostics;
    using System.IO;
    using System.Web;

    #endregion NameSpaces

    [SuppressUnmanagedCodeSecurity]
    public static class ChoWindowsManager
    {
        #region Shared Data Members (Private)

        internal static IntPtr MainWindowHandle = IntPtr.Zero;
        internal readonly static IntPtr ConsoleWindowHandle = IntPtr.Zero;
        internal readonly static ChoApplicationMode ApplicationMode = ChoApplicationMode.Console;

        private readonly static int _origExWindowStyle;
        
        #endregion Shared Data Members (Private)

        #region Constructors

        static ChoWindowsManager()
        {
            try
            {
                ConsoleWindowHandle = ChoKernel32.GetConsoleWindow();
                MainWindowHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                
                if (MainWindowHandle != IntPtr.Zero)
                    _origExWindowStyle = ChoUser32.GetWindowLong(MainWindowHandle, (int)GwlIndexEnum.GWL_EXSTYLE);

                if (Environment.UserInteractive)
                {
                    if (ConsoleWindowHandle != IntPtr.Zero)
                        ApplicationMode = ChoApplicationMode.Console;
                    else
                        ApplicationMode = ChoApplicationMode.Windows;
                }
                else
                {
                    ApplicationMode = ChoApplicationMode.Service;
                    if (HttpContext.Current != null)
                    {
                        ApplicationMode = ChoApplicationMode.Web;
                    }
                }
            }
            catch { }
        }

        #endregion Constructors

        #region Instance Properties

        public static bool HasWindow
        {
            get { return MainWindowHandle != IntPtr.Zero; }
        }

        #endregion Instance Properties

        #region Instance Members (Public)

        public static void AlwaysOnTop(bool set)
        {
            if (HasWindow)
            {
                if (set)
                    ChoUser32.SetWindowPos(MainWindowHandle, (IntPtr)SpecialWindowHandles.HWND_TOPMOST, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_SHOWWINDOW);
                else
                    ChoUser32.SetWindowPos(MainWindowHandle, (IntPtr)SpecialWindowHandles.HWND_NOTOPMOST, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_SHOWWINDOW);
            }
        }

        public static void SetTop(IntPtr? handle = null)
        {
            if (handle == null)
                handle = MainWindowHandle;

            ChoUser32.SetWindowPos(handle.Value, (IntPtr)SpecialWindowHandles.HWND_TOP, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_SHOWWINDOW);
        }

        private static bool _show = true;
        public static void ShowInTaskbar(bool show)
        {
            if (HasWindow)
            {
                if (!show)
                {
                    if (_show)
                    {
                        _show = false;
                        Hide();
                        ChoUser32.SetWindowLong(MainWindowHandle, (int)GwlIndexEnum.GWL_EXSTYLE, (int)((WindowStyles)_origExWindowStyle | WindowStyles.WS_EX_TOOLWINDOW));
                        ChoUser32.ShowWindow(MainWindowHandle, (int)SHOWWINDOW.SW_SHOWNA);
                    }
                }
                else
                {
                    if (!_show)
                    {
                        _show = true;
                        Hide();
                        ChoUser32.SetWindowLong(MainWindowHandle, (int)GwlIndexEnum.GWL_EXSTYLE, (int)_origExWindowStyle);
                        ChoUser32.ShowWindow(MainWindowHandle, (int)SHOWWINDOW.SW_SHOWNA);
                    }
                }
            }
        }

        public static void BringWindowToTop()
        {
            if (HasWindow)
            {
                SetTop();
                ChoUser32.SetForegroundWindow(MainWindowHandle);
            }
        }
        /// <summary>
        /// Creates a new console instance if the process is not attached to a console already.
        /// </summary>
        public static void Show()
        {
            if (HasWindow)
            {
                ChoUser32.ShowWindow(MainWindowHandle, (int)SHOWWINDOW.SW_SHOWNORMAL); //1 = SW_SHOWNORMA
                ChoWindowsManager.SetTop();
            }
        }

        /// <summary>
        /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
        /// </summary>
        public static void Hide()
        {
            if (HasWindow)
                ChoUser32.ShowWindow(MainWindowHandle, (int)SHOWWINDOW.SW_HIDE); //1 = SW_HIDE
        }

        public static void HideConsoleWindow()
        {
            if (ConsoleWindowHandle != IntPtr.Zero)
                ChoUser32.ShowWindow(ConsoleWindowHandle, (int)SHOWWINDOW.SW_HIDE); //1 = SW_HIDE
        }

        public static void ShowConsoleWindow()
        {
            if (ConsoleWindowHandle != IntPtr.Zero)
                ChoUser32.ShowWindow(ConsoleWindowHandle, (int)SHOWWINDOW.SW_SHOWNORMAL); //1 = SW_HIDE
        }

        #endregion Instance Members (Public)
    }
}
