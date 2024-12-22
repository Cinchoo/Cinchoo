namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	#endregion NameSpaces

	public static class ChoFormEx
	{
		/// <summary>
		/// Default shortcut menu, if the form does not have context menu attached to it.
		/// </summary>
		private static ContextMenuStrip _defaultTrayShortcutMenu;

		/// <summary>
		/// Static Constructor
		/// </summary>
		static ChoFormEx()
		{
			_defaultTrayShortcutMenu = new ContextMenuStrip();
			_defaultTrayShortcutMenu.Items.Add("&Restore", null, (sender, e) =>
			{
				ToolStripItem stripItem = sender as ToolStripItem;
				if (stripItem != null)
				{
					Form mainForm = stripItem.Tag as Form;
					mainForm.RestoreFromTray();
				}
			});
			_defaultTrayShortcutMenu.Items.Add("E&xit", null, (sender, e) =>
			{
				ToolStripItem stripItem = sender as ToolStripItem;
				if (stripItem != null)
				{
					Form mainForm = stripItem.Tag as Form;
					NotifyIcon trayIcon = mainForm.Tag as NotifyIcon;
					if (trayIcon != null)
					{
						trayIcon.Dispose();
						mainForm.Tag = null;
					}
				}
				Application.Exit();
			});
		}

		/// <summary>
		/// Convert a Windows Application to Tray Application
		/// </summary>
		/// <param name="frm">A window form used to construct NotifyIcon object.</param>
		/// <returns>A component that creates an icon in the notification area.</returns>
		public static NotifyIcon HideToTray(this Form frm)
		{
			if (frm.ContextMenuStrip != null)
				return HideToTray(frm, frm.ContextMenuStrip);
			else
				return HideToTray(frm, frm.ContextMenu);
		}

		/// <summary>
		/// Convert a Windows Application to Tray Application
		/// </summary>
		/// <param name="frm">A window form used to construct NotifyIcon object.</param>
		/// <param name="menuStrip">A context menu strip used to set as shortcut menu for NotifyIcon object.</param>
		/// <returns>A component that creates an icon in the notification area.</returns>
		public static NotifyIcon HideToTray(this Form frm, ContextMenuStrip menuStrip)
		{
			return HideToTray(frm, menuStrip, null);
		}

		/// <summary>
		/// Convert a Windows Application to Tray Application
		/// </summary>
		/// <param name="frm">A window form used to construct NotifyIcon object.</param>
		/// <param name="menu">A context menu used to set as shortcut menu for NotifyIcon object.</param>
		/// <returns>A component that creates an icon in the notification area.</returns>
		public static NotifyIcon HideToTray(this Form frm, ContextMenu menu)
		{
			return HideToTray(frm, null, menu);
		}

		/// <summary>
		/// Restore the window application to normal from Tray
		/// </summary>
		/// <param name="frm">A window form used to construct NotifyIcon object.</param>
		public static void RestoreFromTray(this Form frm)
		{
			Form mainForm = Application.OpenForms.Count > 0 ? Application.OpenForms[0] : frm;
			if (mainForm != null)
			{
				mainForm.Visible = true;
				mainForm.ShowInTaskbar = true;
				mainForm.WindowState = FormWindowState.Normal;
				NotifyIcon trayIcon = mainForm.Tag as NotifyIcon;
				if (trayIcon != null)
				{
					trayIcon.Dispose();
					mainForm.Tag = null;
				}
			}
		}

		/// <summary>
		/// Helper method to convert application to Tray application. 
		/// menuStrip object / menu object / default tray menu will be used as Shortcut menu in the order it is specified. 
		/// </summary>
		/// <param name="frm">A window form used to construct NotifyIcon object.</param>
		/// <param name="menuStrip">A context menu strip used to set as shortcut menu for NotifyIcon object.</param>
		/// <param name="menu">A context menu used to set as shortcut menu for NotifyIcon object.</param>
		/// <returns>A component that creates an icon in the notification area.</returns>
		private static NotifyIcon HideToTray(Form frm, ContextMenuStrip menuStrip, ContextMenu menu)
		{
			Form mainForm = Application.OpenForms.Count > 0 ? Application.OpenForms[0] : frm;

			if (menuStrip == null && menu == null)
			{
				foreach (ToolStripItem tsi in _defaultTrayShortcutMenu.Items)
					tsi.Tag = mainForm;

				menuStrip = _defaultTrayShortcutMenu;
			}

			NotifyIcon trayIcon = new NotifyIcon();
			trayIcon.Text = frm.Text;
			trayIcon.Icon = frm.Icon;
			if (menuStrip != null)
				trayIcon.ContextMenuStrip = menuStrip;
			else
				trayIcon.ContextMenu = menu;
			trayIcon.Visible = true;
			trayIcon.Tag = frm;

			mainForm.Visible = false;
			mainForm.ShowInTaskbar = false;
			mainForm.Tag = trayIcon;

			return trayIcon;
		}
	}
}
