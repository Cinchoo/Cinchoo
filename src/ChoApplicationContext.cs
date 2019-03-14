using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Cinchoo.Core.Diagnostics;
using Cinchoo.Core.Forms;
using Cinchoo.Core.Reflection;
using System.Reflection;
using System.Windows.Interop;

namespace Cinchoo.Core
{
    public interface IChoWinFormApp
    {
        Form MainFormWindow
        {
            get;
        }
        ContextMenu GetContextMenu(ContextMenu contextMenu);

        string TooltipText
        {
            get;
        }

        string BalloonTipText
        {
            get;
        }

        Icon TrayIcon
        {
            get;
        }
    }

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
        private System.Windows.Forms.ContextMenu _notifyIconContextMenu;	// the context menu for the notify icon
		private System.Windows.Forms.MenuItem _exitContextMenuItem;			// exit menu command for context menu 
		private System.Windows.Forms.MenuItem _showContextMenuItem;			// open menu command for context menu 	
        private System.Windows.Forms.MenuItem _alwaysOnTopContextMenuItem;
        private System.Windows.Forms.MenuItem _sepContextMenuItem1;
        private System.Windows.Forms.MenuItem _sepContextMenuItem2;
        private System.Windows.Forms.MenuItem _runAtStartupContextMenuItem;
        private System.Windows.Forms.MenuItem _showInTaskbarContextMenuItem;
        private System.Windows.Forms.Form _mainFormWindow;						// the current form we're displaying
        private System.Windows.Window _mainWPFWindow;						// the current form we're displaying
        private ContextMenu _defaultContextMenu;
        internal System.Windows.Forms.NotifyIcon NotifyIcon;				// the icon that sits in the system tray

		/// <summary>
		/// This class should be created and passed into Application.Run( ... )
		/// </summary>
        public ChoApplicationContext(IChoWinFormApp winFormApp)
        {
            BuildDefaultContextMenu();
            InitializeContext(winFormApp);
        }
        
        public ChoApplicationContext() : this(null)
		{
        }

        internal bool Visible
        {
            get { return NotifyIcon != null ? NotifyIcon.Visible : false; }
            set { if (NotifyIcon != null) NotifyIcon.Visible = value; }
        }

		/// <summary>
		/// Create the NotifyIcon UI, the ContextMenu for the NotifyIcon and an Exit menu item. 
		/// </summary>
        private void InitializeContext(IChoWinFormApp windowApp) 
		{
            this._components = new System.ComponentModel.Container();
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this._components);
            this._notifyIconContextMenu = GetContextMenu(windowApp);
            if (windowApp != null)
            {
                this._mainFormWindow = windowApp.MainFormWindow;
            }

            this.NotifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);

            this.NotifyIcon.Icon = windowApp != null ? windowApp.TrayIcon : null;
            if (this.NotifyIcon.Icon == null)
            {
                Assembly entryAssembly = ChoAssembly.GetEntryAssembly();
                if (entryAssembly != null)
                    this.NotifyIcon.Icon = Icon.ExtractAssociatedIcon(entryAssembly.Location);
            }

            this.NotifyIcon.Text = windowApp != null ? (windowApp.TooltipText.IsNullOrWhiteSpace() ? ChoGlobalApplicationSettings.Me.ApplicationName : windowApp.TooltipText) : ChoGlobalApplicationSettings.Me.ApplicationName;

            string defaultBaloonTipText = "{0} is running...".FormatString(ChoGlobalApplicationSettings.Me.ApplicationName);
            this.NotifyIcon.BalloonTipText = windowApp != null ? (windowApp.BalloonTipText.IsNullOrWhiteSpace() ? defaultBaloonTipText : windowApp.BalloonTipText) : defaultBaloonTipText;

            this.NotifyIcon.ContextMenu = _notifyIconContextMenu;
            this.NotifyIcon.Visible = true;

            if (this._mainFormWindow != null)
            {
                _mainWPFWindow.Show();
                ChoWindowsManager.HideConsoleWindow();
                ChoWindowsManager.MainWindowHandle = this._mainFormWindow.Handle;
                _mainFormWindow.Closed += new EventHandler(mainForm_Closed);
                _mainFormWindow.Closing += new System.ComponentModel.CancelEventHandler(mainForm_Closing);
                _mainFormWindow.Resize += new EventHandler(mainForm_Resize);
            }
            else if (this._mainWPFWindow != null)
            {
                ChoWindowsManager.HideConsoleWindow();
                _mainWPFWindow.Visibility = Visibility.Hidden;
                _mainWPFWindow.Show();
                WindowInteropHelper windowInteropHelper = new WindowInteropHelper(_mainWPFWindow);
                ChoWindowsManager.MainWindowHandle = windowInteropHelper.Handle;

                //_mainWPFWindow.Show();
                //_mainWPFWindow.SourceInitialized += new EventHandler(mainWPFWindow_SourceInitialized);
                //ChoWindowsManager.MainWindowHandle = this._mainWPFWindow;
                //_mainFormWindow.Closed += new EventHandler(mainForm_Closed);
                //_mainFormWindow.Closing += new System.ComponentModel.CancelEventHandler(mainForm_Closing);
                //_mainFormWindow.Resize += new EventHandler(mainForm_Resize);
            }

            if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
            {
                if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.HideMainWindowAtStartup)
                {
                    this._showContextMenuItem.Checked = false;
                    ToggleShowContextMenuItem();
                }
            }
            else
                ShowMainWindow();

            ChoGlobalApplicationSettings.Me.ObjectChanged += new EventHandler(Me_ConfigChanged);

            Me_ConfigChanged(null, null);
            if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
            {
                if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.HideMainWindowAtStartup)
                    HideMainWindow();
                else
                {
                    HideMainWindow();
                    ShowMainWindow();
                }
            }
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


            this._alwaysOnTopContextMenuItem.Checked = ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.AlwaysOnTop;
            AlwaysOnTop();

            this._runAtStartupContextMenuItem.Checked = ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.RunOnceAtStartup;
            RunAtStartup();

            this._showInTaskbarContextMenuItem.Checked = ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.ShowInTaskbar;
            ShowInTaskbar();

            this.NotifyIcon.Visible = ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn;

            if (!ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
            {
                _showContextMenuItem.Checked = !ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.HideWindow;
                ToggleShowContextMenuItem();
            }
            else
            {
                _showContextMenuItem.Checked = !ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.HideMainWindowAtStartup;
                ToggleShowContextMenuItem();

                if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.HideTrayIconWhenMainWindowShown)
                    this.NotifyIcon.Visible = false;
            }
        }

        private ContextMenu GetContextMenu(IChoWinFormApp windowApp)
        {
            ContextMenu contextMenu = windowApp != null ? windowApp.GetContextMenu(_defaultContextMenu) : _defaultContextMenu;

            return contextMenu;
        }

        private void BuildDefaultContextMenu()
        {
            _defaultContextMenu = new ContextMenu();
            // 
            // showContextMenuItem
            // 
            this._showContextMenuItem = new System.Windows.Forms.MenuItem();
            this._showContextMenuItem.Text = "Show Main Window";
            this._showContextMenuItem.DefaultItem = true;
            this._showContextMenuItem.Click += new System.EventHandler(this.showContextMenuItem_Click);

            this._sepContextMenuItem1 = new System.Windows.Forms.MenuItem();
            this._sepContextMenuItem1.Text = "-";
            // 
            // alwaysOnTopContextMenuItem
            // 
            this._alwaysOnTopContextMenuItem = new System.Windows.Forms.MenuItem();
            this._alwaysOnTopContextMenuItem.Checked = ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.AlwaysOnTop;
            this._alwaysOnTopContextMenuItem.Text = "Always on top";
            this._alwaysOnTopContextMenuItem.Click += new System.EventHandler(this.alwaysOnTopContextMenuItem_Click);

            this._runAtStartupContextMenuItem = new System.Windows.Forms.MenuItem();
            this._runAtStartupContextMenuItem.Checked = ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.RunOnceAtStartup;
            this._runAtStartupContextMenuItem.Text = "Run at Systems Startup";
            this._runAtStartupContextMenuItem.Click += new System.EventHandler(this.runAtStartupContextMenuItem_Click);

            this._showInTaskbarContextMenuItem = new System.Windows.Forms.MenuItem();
            this._showInTaskbarContextMenuItem.Checked = ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.ShowInTaskbar;
            this._showInTaskbarContextMenuItem.Text = "Show in Taskbar";
            this._showInTaskbarContextMenuItem.Click += new System.EventHandler(this.showInTaskbarContextMenuItem_Click);

            this._sepContextMenuItem2 = new System.Windows.Forms.MenuItem();
            this._sepContextMenuItem2.Text = "-";

            // 
            // exitContextMenuItem
            // 
            this._exitContextMenuItem = new System.Windows.Forms.MenuItem();
            this._exitContextMenuItem.Text = "Exit";
            this._exitContextMenuItem.Click += new System.EventHandler(this.exitContextMenuItem_Click);

            _defaultContextMenu.MenuItems.AddRange(new MenuItem[] { _showContextMenuItem, _sepContextMenuItem1, _alwaysOnTopContextMenuItem, _runAtStartupContextMenuItem, 
                _showInTaskbarContextMenuItem, _sepContextMenuItem2, _exitContextMenuItem });
        }

		/// <summary>
		/// When the application context is disposed, dispose things like the notify icon.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
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
            this._showContextMenuItem.Checked = !this._showContextMenuItem.Checked;

            ToggleShowContextMenuItem();
        }

        private void ToggleShowContextMenuItem()
        {
            if (this._showContextMenuItem.Checked)
            {
                this._showContextMenuItem.Text = "&Hide Main Window";
                ShowMainWindow();
            }
            else
            {
                this._showContextMenuItem.Text = "&Show Main Window";
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
        }

        private void alwaysOnTopContextMenuItem_Click(object sender, EventArgs e)
        {
            this._alwaysOnTopContextMenuItem.Checked = !this._alwaysOnTopContextMenuItem.Checked;
            AlwaysOnTop();
        }

        private void AlwaysOnTop()
        {
            if (_showContextMenuItem.Checked)
            {
                ChoWindowsManager.AlwaysOnTop(this._alwaysOnTopContextMenuItem.Checked);
                //ToggleShowContextMenuItem();
            }
        }

        private void runAtStartupContextMenuItem_Click(object sender, EventArgs e)
        {
            this._runAtStartupContextMenuItem.Checked = !this._runAtStartupContextMenuItem.Checked;
            RunAtStartup();
        }

        private void RunAtStartup()
        {
            try
            {
                ChoApplication.RunAtSystemStartup(!this._runAtStartupContextMenuItem.Checked);
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
            if (_showContextMenuItem.Checked)
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
            ExitThread();
        }
		
		/// <summary>
		/// When the notify icon is double clicked in the system tray, bring up a form with a calendar on it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void notifyIcon_DoubleClick(object sender,System.EventArgs e)
		{
            _showContextMenuItem.Checked = true;
            ToggleShowContextMenuItem();
		}

        private void mainForm_Resize(object sender, EventArgs e)
        {
            if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
            {
                if (FormWindowState.Minimized == this._mainFormWindow.WindowState)
                {
                    if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TrayAppTurnOnMode == ChoTrayAppTurnOnMode.OnMinimize
                        || ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TrayAppTurnOnMode == ChoTrayAppTurnOnMode.OnMinimizeOrClose)
                    {
                        this.NotifyIcon.Visible = true;

                        try
                        {
                            if (!this.NotifyIcon.BalloonTipText.IsNull())
                                this.NotifyIcon.ShowBalloonTip(500);
                        }
                        catch { }

                        _showContextMenuItem.Checked = false;
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
            if (ChoFramework.ShutdownRequested)
                return;

            // null out the main form so we know to create a new one.
            //this._mainForm = null;
            if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
            {
                if (ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TrayAppTurnOnMode == ChoTrayAppTurnOnMode.OnClose
                    || ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TrayAppTurnOnMode == ChoTrayAppTurnOnMode.OnMinimizeOrClose)
                {
                    _showContextMenuItem.Checked = false;
                    ToggleShowContextMenuItem();
                    this.NotifyIcon.Visible = true;
                    e.Cancel = true;
                }
                else
                {
                    //ChoFramework.Shutdown();
                    ExitThread();
                }
            }
            else
            {
                //ChoFramework.Shutdown();
                ExitThread();
            }
        }

		private void mainForm_Closed(object sender, EventArgs e) 
		{
		}

		/// <summary>
		/// If we are presently showing a mainForm, clean it up.
		/// </summary>
		protected override void ExitThreadCore()
		{
            ChoFramework.Shutdown();

            if (_mainFormWindow != null)
            {
                // before we exit, give the main form a chance to clean itself up.
                _mainFormWindow.Close();
            }
            else
                Environment.Exit(0);
			base.ExitThreadCore();
		}
		
	}
}
