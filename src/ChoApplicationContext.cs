using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Cinchoo.Core.Diagnostics;
using Cinchoo.Core.Windows.Forms;
using Cinchoo.Core.Reflection;
using System.Reflection;
using System.Windows.Interop;
using System.Collections.Generic;

namespace Cinchoo.Core
{
    //public class ChoWPFDefaultApplication : System.Windows.Application
    //{
    //    public static readonly ChoWPFDefaultApplication Default = new ChoWPFDefaultApplication();
    //}

    /// <summary>
    /// CalendarApplicationContext.
    /// This class has several jobs:
    ///		- Create the NotifyIcon UI
    ///		- Manage the CalendarForm that pops up
    ///		- Determines when the Application should exit
    /// </summary>
    public class ChoApplicationContext : ApplicationContext
    {
        private System.ComponentModel.IContainer _components;						// a list of components to dispose when the context is disposed
        private System.Windows.Forms.ToolStripMenuItem _aboutContextMenuItem;			// about menu command for context menu 
        private System.Windows.Forms.ToolStripMenuItem _helpContextMenuItem;			// help menu command for context menu 
        private System.Windows.Forms.ToolStripMenuItem _exitContextMenuItem;			// exit menu command for context menu 
        private System.Windows.Forms.ToolStripMenuItem _showMainWndMenuItem;			// open menu command for context menu 	
        private System.Windows.Forms.ToolStripMenuItem _alwaysOnTopContextMenuItem;
        private System.Windows.Forms.ToolStripSeparator _sepContextMenuItem1;
        private System.Windows.Forms.ToolStripSeparator _sepContextMenuItem2;
        private System.Windows.Forms.ToolStripSeparator _sepContextMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem _runAtStartupContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _showInTaskbarContextMenuItem;
        private System.Windows.Forms.Form _mainFormWindow;						// the current form we're displaying
        private System.Windows.Window _mainWPFWindow;						// the current form we're displaying
        private ContextMenuStrip _defaultContextMenu;
        private bool _hasWindow = false;
        private object _mainForm = null;
        private bool _isMainWindowOpen = false;
        private string _defaultTrayTipMsg;

        internal readonly ChoNotifyIcon NotifyIcon;				// the icon that sits in the system tray
        internal readonly ChoApplicationHost _appHost = null;

        /// <summary>
        /// This class should be created and passed into Application.Run( ... )
        /// </summary>
        public ChoApplicationContext(ChoApplicationHost appHost)
        {
            ChoGuard.ArgumentNotNull(appHost, "ApplicationHost");
            _appHost = appHost;
            _mainForm = appHost.GetMainWindowObject();
            _hasWindow = appHost.IsWindowApp;
            BuildDefaultContextMenu();
            this._components = new System.ComponentModel.Container();
            this.NotifyIcon = new ChoNotifyIcon(this._components);
            PreInitializeContext(appHost);
            ChoApplication.RaiseAfterNotifyIconConstructed(NotifyIcon);
            InitializeContext(appHost);
            _defaultTrayTipMsg = "{0} is running...".FormatString(ChoGlobalApplicationSettings.Me.ApplicationName);
        }

        public ChoApplicationContext()
            : this(null)
        {
        }

        internal bool Visible
        {
            get { return NotifyIcon != null ? NotifyIcon.Visible : false; }
            set { if (NotifyIcon != null) NotifyIcon.Visible = value; }
        }

        private void PreInitializeContext(ChoApplicationHost appHost)
        {
            if (appHost != null)
            {
                //if (ChoFrameworkCmdLineArgs.GetApplicationMode() != null
                //    && ChoFrameworkCmdLineArgs.GetApplicationMode().Value == ChoApplicationMode.Console
                if (ChoApplication.ApplicationMode == ChoApplicationMode.Console
                    && !ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn
                    /*&& _mainForm != null */)
                {
                    _mainForm = null;
                    //ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn = false;
                    return;
                }

                if (!ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.DisableDefaultDoubleClickEvent)
                    this.NotifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
                if (!ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.DisableDefaultClickEvent)
                    this.NotifyIcon.Click += new System.EventHandler(this.notifyIcon_Click);

                //System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(ChoApplication.Application_ThreadException);
                if (_mainForm is Form)
                {
                    ChoApplication.WindowsAppType = ChoWindowsAppType.WinForms;
                    this._mainFormWindow = (Form)_mainForm;
                }
                else if (_mainForm is Window)
                {
                    ChoApplication.WindowsAppType = ChoWindowsAppType.WPF;
                    this._mainWPFWindow = (Window)_mainForm;

                    //System.Windows.Application app = appHost.ApplicationObject as System.Windows.Application;
                    //if (app == null)
                    //    app = new ChoWPFDefaultApplication();

                    //app.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(ChoApplication.Current_DispatcherUnhandledException);
                    //app.Run(_mainWPFWindow);
                }
                //else
                //    throw new ApplicationException("MainWindow object is not a valid object. Must be either Form or Window type.");
            }

            //this.NotifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);

            //this.NotifyIcon.Icon = windowApp != null ? windowApp.TrayIcon : null;
            if (this.NotifyIcon.Icon == null)
            {
                Assembly entryAssembly = ChoAssembly.GetEntryAssembly();
                if (entryAssembly != null)
                    this.NotifyIcon.Icon = Icon.ExtractAssociatedIcon(entryAssembly.Location);
            }

            //this.NotifyIcon.Text = windowApp != null ? (windowApp.TooltipText.IsNullOrWhiteSpace() ? ChoGlobalApplicationSettings.Me.ApplicationName : windowApp.TooltipText) : ChoGlobalApplicationSettings.Me.ApplicationName;
            if (this.NotifyIcon.Text.IsNullOrWhiteSpace())
                this.NotifyIcon.Text = ChoGlobalApplicationSettings.Me.ApplicationName;

            if (this.NotifyIcon.BalloonTipText.IsNullOrWhiteSpace())
                this.NotifyIcon.BalloonTipText = _defaultTrayTipMsg;

            if (this.NotifyIcon.ContextMenu == null)
            {
                this.NotifyIcon.ContextMenuStrip = _defaultContextMenu;
            }
        }

        public void Run()
        {
            AttachResources();
            if (this._appHost.WinApp == null)
                System.Windows.Forms.Application.Run(this);
            else
            {
                _appHost.WinApp.Run(_mainWPFWindow);
            }
        }

        private void AttachResources()
        {
            try
            {
                if (System.Windows.Application.Current != null && System.Windows.Application.Current.Resources != null && _mainWPFWindow != null)
                {
                    foreach (var y in System.Windows.Application.Current.Resources.MergedDictionaries)
                        _mainWPFWindow.Resources.MergedDictionaries.Add(y);
                }
            }
            catch { }
        }

        /// <summary>
        /// Create the NotifyIcon UI, the ContextMenu for the NotifyIcon and an Exit menu item. 
        /// </summary>
        private void InitializeContext(ChoApplicationHost appHost)
        {
            //if (!_hasWindow)
            //    ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn = true;

            //if (appHost != null)
            //{
            //    //if (ChoFrameworkCmdLineArgs.GetApplicationMode() != null
            //    //    && ChoFrameworkCmdLineArgs.GetApplicationMode().Value == ChoApplicationMode.Console
            //    if (ChoApplication.ApplicationMode == ChoApplicationMode.Console
            //        && !ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn
            //        /*&& _mainForm != null */)
            //    {
            //        _mainForm = null;
            //        //ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn = false;
            //        return;
            //    }

            //    this.NotifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            //    //System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //    System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(ChoApplication.Application_ThreadException);
            //    if (_mainForm is Form)
            //    {
            //        ChoApplication.WindowsAppType = ChoWindowsAppType.WinForms;
            //        this._mainFormWindow = (Form)_mainForm;
            //    }
            //    else if (_mainForm is Window)
            //    {
            //        ChoApplication.WindowsAppType = ChoWindowsAppType.WPF;
            //        this._mainWPFWindow = (Window)_mainForm;

            //        //System.Windows.Application app = appHost.ApplicationObject as System.Windows.Application;
            //        //if (app == null)
            //        //    app = new ChoWPFDefaultApplication();

            //        //app.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(ChoApplication.Current_DispatcherUnhandledException);
            //        //app.Run(_mainWPFWindow);
            //    }
            //    //else
            //    //    throw new ApplicationException("MainWindow object is not a valid object. Must be either Form or Window type.");
            //}

            ////this.NotifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);

            ////this.NotifyIcon.Icon = windowApp != null ? windowApp.TrayIcon : null;
            //if (this.NotifyIcon.Icon == null)
            //{
            //    Assembly entryAssembly = ChoAssembly.GetEntryAssembly();
            //    if (entryAssembly != null)
            //        this.NotifyIcon.Icon = Icon.ExtractAssociatedIcon(entryAssembly.Location);
            //}

            ////this.NotifyIcon.Text = windowApp != null ? (windowApp.TooltipText.IsNullOrWhiteSpace() ? ChoGlobalApplicationSettings.Me.ApplicationName : windowApp.TooltipText) : ChoGlobalApplicationSettings.Me.ApplicationName;
            //if (this.NotifyIcon.Text.IsNullOrWhiteSpace())
            //    this.NotifyIcon.Text = ChoGlobalApplicationSettings.Me.ApplicationName;

            //if (this.NotifyIcon.BalloonTipText.IsNullOrWhiteSpace())
            //    this.NotifyIcon.BalloonTipText = _defaultTrayTipMsg;

            //if (this.NotifyIcon.ContextMenu == null)
            //{
            //    this.NotifyIcon.ContextMenuStrip = _defaultContextMenu;
            //}
            //this.NotifyIcon.Visible = ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn;

            if (this._mainFormWindow != null)
            {
                _mainFormWindow.Show();
                ChoWindowsManager.HideConsoleWindow();
                ChoWindowsManager.MainWindowHandle = this._mainFormWindow.Handle;
                _mainFormWindow.Closed += new EventHandler(mainForm_Closed);
                _mainFormWindow.Closing += new System.ComponentModel.CancelEventHandler(mainForm_Closing);
                _mainFormWindow.Resize += new EventHandler(mainForm_Resize);
            }
            else if (this._mainWPFWindow != null)
            {
                System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(_mainWPFWindow);
                if (_appHost.WinApp == null)
                {
                    //_appHost.WinApp = ChoWPFDefaultApplication.Default;
                    //_appHost.WinApp.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(ChoApplication.Current_DispatcherUnhandledException);

                    _mainWPFWindow.Show();
                }
                ChoWindowsManager.HideConsoleWindow();
                WindowInteropHelper windowInteropHelper = new WindowInteropHelper(_mainWPFWindow);
                ChoWindowsManager.MainWindowHandle = windowInteropHelper.Handle;
                _mainWPFWindow.SourceInitialized += mainWPFWindow_SourceInitialized;
                _mainWPFWindow.StateChanged += new EventHandler(mainWPFWindow_StateChanged);
                _mainWPFWindow.Closed += new EventHandler(mainWPFWindow_Closed);
                _mainWPFWindow.Closing += new System.ComponentModel.CancelEventHandler(mainWPFWindow_Closing);
            }
            else
            {
                ChoWindowsManager.HideConsoleWindow();
            }

            if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
            {
                if (_appHost != null)
                {
                    if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.HideMainWindowAtStartup)
                    {
                        //this._showMainWndMenuItem.Checked = false;
                        _isMainWindowOpen = false;

                        ToggleShowContextMenuItem();
                    }
                }
            }
            else
            {
                if (_appHost != null)
                {
                    ShowMainWindow();
                }
            }

            ChoGlobalApplicationSettings.Me.ObjectChanged += new EventHandler(Me_ConfigChanged);

            Me_ConfigChanged(null, null);
            if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
            {
                if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.HideMainWindowAtStartup)
                    HideMainWindow();
                else
                {
                    //HideMainWindow();
                    //ShowMainWindow();
                }
            }
        }

        void mainWPFWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e.Cancel) return;

            OnMainWindowClosing(e);
        }

        private void mainForm_Closed(object sender, EventArgs e)
        {
        }

        void mainWPFWindow_Closed(object sender, EventArgs e)
        {
        }

        void mainWPFWindow_SourceInitialized(object sender, EventArgs e)
        {
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(_mainWPFWindow);
            ChoWindowsManager.MainWindowHandle = windowInteropHelper.Handle;
        }

        void Me_ConfigChanged(object sender, EventArgs e)
        {
            if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
            {
                if (!ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TooltipText.IsNullOrWhiteSpace())
                    this.NotifyIcon.Text = ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TooltipText;
                if (!ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.BalloonTipText.IsNullOrWhiteSpace())
                    this.NotifyIcon.BalloonTipText = ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.BalloonTipText;

                if (!ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TrayIcon.IsNullOrWhiteSpace()
                    && File.Exists(ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TrayIcon))
                {
                    try
                    {
                        NotifyIcon.Icon = Icon.ExtractAssociatedIcon(ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TrayIcon);
                    }
                    catch (Exception ex)
                    {
                        ChoTrace.Error(ex.ToString());
                    }
                }
            }
            else
            {
            }

            if (_hasWindow)
            {
                this._alwaysOnTopContextMenuItem.Checked = ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.AlwaysOnTop;
                AlwaysOnTop();
            }

            if (_hasWindow)
            {
                this._showInTaskbarContextMenuItem.Checked = ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.ShowInTaskbar;
                ShowInTaskbar();
            }

            this.NotifyIcon.Visible = ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn;

            if (_hasWindow)
            {
                if (!ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
                {
                    //_showMainWndMenuItem.Checked = !ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.HideWindow;
                    _isMainWindowOpen = !ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.HideWindow;
                    ToggleShowContextMenuItem();
                }
                else
                {
                    //_showMainWndMenuItem.Checked = !ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.HideMainWindowAtStartup;
                    _isMainWindowOpen = !ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.HideMainWindowAtStartup;
                    ToggleShowContextMenuItem();

                    if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.HideTrayIconWhenMainWindowShown)
                        this.NotifyIcon.Visible = false;
                }
            }
        }

        private void BuildDefaultContextMenu()
        {
            _defaultContextMenu = new ContextMenuStrip();
            // 
            // showContextMenuItem
            // 
            this._showMainWndMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._showMainWndMenuItem.Text = "&Open"; //Show Main Window";
            _showMainWndMenuItem.Font = new Font(_showMainWndMenuItem.Font, _showMainWndMenuItem.Font.Style | System.Drawing.FontStyle.Bold);
            this._showMainWndMenuItem.Click += new System.EventHandler(this.showContextMenuItem_Click);

            this._sepContextMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this._sepContextMenuItem1.Text = "-";
            // 
            // alwaysOnTopContextMenuItem
            // 
            this._alwaysOnTopContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._alwaysOnTopContextMenuItem.Checked = ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.AlwaysOnTop;
            this._alwaysOnTopContextMenuItem.Text = "&Always on top";
            this._alwaysOnTopContextMenuItem.Click += new System.EventHandler(this.alwaysOnTopContextMenuItem_Click);

            this._runAtStartupContextMenuItem = new ChoUACToolStripMenuItem();
            this._runAtStartupContextMenuItem.Checked = ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.RunOnceAtStartup;
            this._runAtStartupContextMenuItem.Text = "Run at Systems &Startup";
            this._runAtStartupContextMenuItem.Click += new System.EventHandler(this.runAtStartupContextMenuItem_Click);
            _runAtStartupContextMenuItem.SetUACShield(true);

            this._showInTaskbarContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._showInTaskbarContextMenuItem.Checked = ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.ShowInTaskbar;
            this._showInTaskbarContextMenuItem.Text = "Show in &Taskbar";
            this._showInTaskbarContextMenuItem.Click += new System.EventHandler(this.showInTaskbarContextMenuItem_Click);

            this._sepContextMenuItem2 = new System.Windows.Forms.ToolStripSeparator();

            // 
            // helpContextMenuItem
            // 
            this._aboutContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._aboutContextMenuItem.Text = "Abo&ut";
            this._aboutContextMenuItem.Click += new System.EventHandler(this.aboutContextMenuItem_Click);

            this._sepContextMenuItem3 = new System.Windows.Forms.ToolStripSeparator();

            // 
            // helpContextMenuItem
            // 
            this._helpContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._helpContextMenuItem.Text = "&Help";
            this._helpContextMenuItem.Click += new System.EventHandler(this.helpContextMenuItem_Click);
            // 
            // exitContextMenuItem
            // 
            this._exitContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._exitContextMenuItem.Text = "E&xit";
            this._exitContextMenuItem.Click += new System.EventHandler(this.exitContextMenuItem_Click);

            List<ToolStripItem> menuItems = new List<ToolStripItem>();
            if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings != null
                || ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.ContextMenuSettings != null)
            {
                if (_hasWindow)
                {
                    if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.ContextMenuSettings.DisplayShowMainWndMenuItem)
                    {
                        menuItems.Add(_showMainWndMenuItem);
                        menuItems.Add(_sepContextMenuItem1);
                    }
                }
                else if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.ContextMenuSettings.DisplayShowMainWndMenuItem)
                {
                    menuItems.Add(_showMainWndMenuItem);
                    menuItems.Add(_sepContextMenuItem1);
                }

                bool addSeperator = false;
                if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.ContextMenuSettings.DisplayAlwaysOnTopMenuItem)
                {
                    if (_hasWindow)
                    {
                        addSeperator = true;
                        menuItems.Add(_alwaysOnTopContextMenuItem);
                    }
                }
                if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.ContextMenuSettings.DisplayRunAtSystemsStartupMenuItem)
                {
                    addSeperator = true;
                    menuItems.Add(_runAtStartupContextMenuItem);
                }
                if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.ContextMenuSettings.DisplayShowInTaskbarMenuItem)
                {
                    if (_hasWindow)
                    {
                        addSeperator = true;
                        menuItems.Add(_showInTaskbarContextMenuItem);
                    }
                }
                if (addSeperator)
                    menuItems.Add(_sepContextMenuItem2);

                if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.ContextMenuSettings.DisplayAboutMenuItem)
                {
                    menuItems.Add(_aboutContextMenuItem);
                    menuItems.Add(_sepContextMenuItem3);
                }
                if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.ContextMenuSettings.DisplayHelpMenuItem)
                    menuItems.Add(_helpContextMenuItem);
                if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.ContextMenuSettings.DisplayExitMenuItem)
                    menuItems.Add(_exitContextMenuItem);
            }

            _defaultContextMenu.Items.AddRange(menuItems.ToArray());
            _defaultContextMenu.Opening += _defaultContextMenu_Opening;
        }

        void _defaultContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.ContextMenuSettings.DisplayRunAtSystemsStartupMenuItem)
            {
                if (ChoWindowsIdentity.IsAdministrator())
                    _runAtStartupContextMenuItem.Checked = ChoApplication.IsAppRunAtSystemStartup("{0}_I".FormatString(ChoGlobalApplicationSettings.Me.ApplicationNameWithoutExtension));
                else
                {
                    //_runAtStartupContextMenuItem.Text = ChoApplication.IsAppRunAtSystemStartup() ? "Remove Run at Systems &Startup" : "Run at Systems &Startup";
                }
            }
        }

        /// <summary>
        /// When the application context is disposed, dispose things like the notify icon.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                {
                    _components.Dispose();
                }
            }
        }

        /// <summary>
        /// When the open menu item is clicked, make a call to Show the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showContextMenuItem_Click(object sender, EventArgs e)
        {
            if (_appHost == null) return;
            _appHost.OnTrayAppOpenMenuClicked(sender, e);
        }

        internal void ShowMenuItemClicked()
        {
            //this._showMainWndMenuItem.Checked = !this._showMainWndMenuItem.Checked;
            _isMainWindowOpen = !_isMainWindowOpen;

            ToggleShowContextMenuItem();
        }

        private void ToggleShowContextMenuItem()
        {
            if (_isMainWindowOpen /*this._showMainWndMenuItem.Checked*/)
            {
                //this._showMainWndMenuItem.Text = "&Hide Main Window";
                ShowMainWindow();
            }
            else
            {
                //this._showMainWndMenuItem.Text = "&Show Main Window";
                HideMainWindow();
            }
        }

        private void HideMainWindow()
        {
            ChoWindowsManager.Hide();
            if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.HideTrayIconWhenMainWindowShown)
                NotifyIcon.Visible = true;
        }

        private void ShowMainWindow()
        {
            ChoWindowsManager.Show();
            ChoWindowsManager.AlwaysOnTop(this._alwaysOnTopContextMenuItem.Checked);
            ShowInTaskbar();
            if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.HideTrayIconWhenMainWindowShown)
                NotifyIcon.Visible = false;
            if (ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.BringWindowToTop)
                ChoWindowsManager.BringWindowToTop();
            else
                ChoWindowsManager.SetTop();
        }

        private void alwaysOnTopContextMenuItem_Click(object sender, EventArgs e)
        {
            this._alwaysOnTopContextMenuItem.Checked = !this._alwaysOnTopContextMenuItem.Checked;
            AlwaysOnTop();
        }

        private void AlwaysOnTop()
        {
            if (_isMainWindowOpen /*_showMainWndMenuItem.Checked*/)
            {
                ChoWindowsManager.AlwaysOnTop(this._alwaysOnTopContextMenuItem.Checked);
                //ToggleShowContextMenuItem();
            }
        }

        private void runAtStartupContextMenuItem_Click(object sender, EventArgs e)
        {
            if (ChoWindowsIdentity.IsAdministrator())
            {
                this._runAtStartupContextMenuItem.Checked = !this._runAtStartupContextMenuItem.Checked;
                RunAtStartup();
            }
        }

        private void RunAtStartup()
        {
            try
            {
                ChoApplication.RunAtSystemStartup("{0}_I".FormatString(ChoGlobalApplicationSettings.Me.ApplicationNameWithoutExtension),
                    ChoAssembly.GetEntryAssembly().Location, !this._runAtStartupContextMenuItem.Checked);
            }
            catch { }
        }

        private void showInTaskbarContextMenuItem_Click(object sender, EventArgs e)
        {
            this._showInTaskbarContextMenuItem.Checked = !this._showInTaskbarContextMenuItem.Checked;
            ShowInTaskbar();
        }

        private void ShowInTaskbar()
        {
            if (_isMainWindowOpen /*_showMainWndMenuItem.Checked*/)
                ChoWindowsManager.ShowInTaskbar(this._showInTaskbarContextMenuItem.Checked);
        }

        /// <summary>
        /// When the exit menu item is clicked, make a call to terminate the ApplicationContext.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitContextMenuItem_Click(object sender, EventArgs e)
        {
            //ChoFramework.Shutdown();
            if (_appHost == null) return;
            _appHost.OnTrayAppExitMenuClicked(sender, e);
        }

        private void helpContextMenuItem_Click(object sender, EventArgs e)
        {
            if (_appHost == null) return;
            _appHost.OnTrayAppHelpMenuClicked(sender, e);
        }

        private void aboutContextMenuItem_Click(object sender, EventArgs e)
        {
            if (_appHost == null) return;
            _appHost.OnTrayAppAboutMenuClicked(sender, e);
        }

        /// <summary>
        /// When the notify icon is double clicked in the system tray, bring up a form with a calendar on it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_DoubleClick(object sender, System.EventArgs e)
        {
            ////_showMainWndMenuItem.Checked = true;
            //_isMainWindowOpen = true;
            //ToggleShowContextMenuItem();
            showContextMenuItem_Click(sender, e);
        }

        private void notifyIcon_Click(object sender, System.EventArgs e)
        {
            try
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                if (mi != null)
                    mi.Invoke(NotifyIcon.Handle, null);
            }
            catch { }
        }

        void mainWPFWindow_StateChanged(object sender, EventArgs e)
        {
            OnResizeMainWindow(_mainWPFWindow.WindowState == WindowState.Minimized ? FormWindowState.Minimized :
                _mainWPFWindow.WindowState == WindowState.Maximized ? FormWindowState.Maximized : FormWindowState.Normal);
        }

        private void mainForm_Resize(object sender, EventArgs e)
        {
            OnResizeMainWindow(this._mainFormWindow.WindowState);
        }

        private void OnResizeMainWindow(FormWindowState windowState)
        {
            if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
            {
                if (FormWindowState.Minimized == windowState)
                {
                    if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOnMode == ChoTrayAppTurnOnMode.OnMinimize
                        || ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOnMode == ChoTrayAppTurnOnMode.OnMinimizeOrClose)
                    {
                        this.NotifyIcon.Visible = true;

                        try
                        {
                            ChoApplication.Host.OnWindowMinimizeInternal(NotifyIcon);
                            if (!this.NotifyIcon.BalloonTipText.IsNull())
                                this.NotifyIcon.ShowBalloonTip(500);
                        }
                        catch { }

                        //_showMainWndMenuItem.Checked = false;
                        _isMainWindowOpen = false;
                        ToggleShowContextMenuItem();
                    }
                }
                //else if (FormWindowState.Normal == this._mainForm.WindowState)
                //{
                //    NotifyIcon.Visible = false;
                //}
            }
        }

        private void mainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnMainWindowClosing(e);
        }

        private void OnMainWindowClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (ChoFramework.ShutdownRequested)
                return;

            // null out the main form so we know to create a new one.
            //this._mainForm = null;
            if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
            {
                if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOnMode == ChoTrayAppTurnOnMode.OnClose
                    || ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOnMode == ChoTrayAppTurnOnMode.OnMinimizeOrClose)
                {
                    this.NotifyIcon.Visible = true;

                    try
                    {
                        ChoApplication.Host.OnWindowMinimizeInternal(NotifyIcon);

                        if (!this.NotifyIcon.BalloonTipText.IsNull())
                            this.NotifyIcon.ShowBalloonTip(500);
                    }
                    catch { }

                    //_showMainWndMenuItem.Checked = false;
                    _isMainWindowOpen = false;
                    ToggleShowContextMenuItem();
                    e.Cancel = true;
                }
                else
                {
                    ExitThread();
                }
            }
            else
            {
                ExitThread();
            }
        }

        /// <summary>
        /// If we are presently showing a mainForm, clean it up.
        /// </summary>
        protected override void ExitThreadCore()
        {
            ChoWindowsManager.ShowConsoleWindow();

            ChoFramework.Shutdown();

            if (_mainFormWindow != null)
            {
                // before we exit, give the main form a chance to clean itself up.
                _mainFormWindow.Close();
            }
            else
                Environment.Exit(0);

            if (System.Windows.Application.Current != null)
                System.Windows.Application.Current.Shutdown();

            base.ExitThreadCore();
        }

    }
}
