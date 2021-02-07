using Cinchoo.Core.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Cinchoo.Core.Windows.Forms
{
    [DefaultEvent("MouseDoubleClick")]
    [DefaultProperty("Text")]
    [Designer("System.Windows.Forms.Design.NotifyIconDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [ToolboxItemFilter("System.Windows.Forms")]
    public sealed class ChoNotifyIcon : ChoNotifyPropertyChangedObject, IDisposable
    {
        #region Instance Data Members (Private)

        private NotifyIcon _notifyIcon = null;
        private Font _font = null;
        private Color _color = Color.Black;
        private Icon[] _animationIcons;
        private Timer _timer;
        private int _currIndex = 0;
        private int _loopCount = 0;
        private Icon _defaultIcon;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoNotifyIcon() : this(null)
        {
        }

        public ChoNotifyIcon(IContainer container)
        {
            if (container == null)
                _notifyIcon = new NotifyIcon();
            else
                _notifyIcon = new NotifyIcon(container);

            _font = new Font(ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.FontSettings.FontName, ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.FontSettings.FontSize);
            _color = ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.FontSettings.FontColor;
            Assembly entryAssembly = ChoAssembly.GetEntryAssembly();
            if (entryAssembly != null)
                _defaultIcon = Icon.ExtractAssociatedIcon(entryAssembly.Location);

            _timer = new Timer();
            _timer.Interval = 100;
            _timer.Tick += new System.EventHandler(this.timerTick);
        }

        #endregion Constructors

        #region Properties (Public)

        public NotifyIcon Handle
        {
            get { return _notifyIcon; }
        }

        public ToolTipIcon BalloonTipIcon { get; set; }
        //
        // Summary:
        //     Gets or sets the text to display on the balloon tip associated with the System.Windows.Forms.NotifyIcon.
        //
        // Returns:
        //     The text to display on the balloon tip associated with the System.Windows.Forms.NotifyIcon.
        [DefaultValue("")]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(true)]
        public string BalloonTipText 
        {
            get { return _notifyIcon.BalloonTipText; }
            set 
            {
                RaisePropertyChanging("BalloonTipText");
                _notifyIcon.BalloonTipText = value;
                RaisePropertyChanged("BalloonTipText");
            }
        }
        //
        // Summary:
        //     Gets or sets the title of the balloon tip displayed on the System.Windows.Forms.NotifyIcon.
        //
        // Returns:
        //     The text to display as the title of the balloon tip.
        [DefaultValue("")]
        [Localizable(true)]
        public string BalloonTipTitle
        {
            get { return _notifyIcon.BalloonTipTitle; }
            set
            {
                RaisePropertyChanging("BalloonTipTitle");
                _notifyIcon.BalloonTipTitle = value;
                RaisePropertyChanged("BalloonTipTitle");
            }
        }

        //
        // Summary:
        //     Gets or sets the shortcut menu for the icon.
        //
        // Returns:
        //     The System.Windows.Forms.ContextMenu for the icon. The default value is null.
        [Browsable(false)]
        [DefaultValue("")]
        public ContextMenu ContextMenu
        {
            get { return _notifyIcon.ContextMenu; }
            set
            {
                RaisePropertyChanging("ContextMenu");
                _notifyIcon.ContextMenu = value;
                RaisePropertyChanged("ContextMenu");
            }
        }

        //
        // Summary:
        //     Gets or sets the shortcut menu associated with the System.Windows.Forms.NotifyIcon.
        //
        // Returns:
        //     The System.Windows.Forms.ContextMenuStrip associated with the System.Windows.Forms.NotifyIcon
        [DefaultValue("")]
        public ContextMenuStrip ContextMenuStrip
        {
            get { return _notifyIcon.ContextMenuStrip; }
            set
            {
                RaisePropertyChanging("ContextMenuStrip");
                _notifyIcon.ContextMenuStrip = value;
                RaisePropertyChanged("ContextMenuStrip");
            }
        }

        //
        // Summary:
        //     Gets or sets the current icon.
        //
        // Returns:
        //     The System.Drawing.Icon displayed by the System.Windows.Forms.NotifyIcon
        //     component. The default value is null.
        [DefaultValue("")]
        [Localizable(true)]
        public Icon Icon
        {
            get { return _notifyIcon.Icon; }
            set
            {
                RaisePropertyChanging("Icon");
                _defaultIcon = _notifyIcon.Icon = value;
                RaisePropertyChanged("Icon");
            }
        }

        //
        // Summary:
        //     Gets or sets an object that contains data about the System.Windows.Forms.NotifyIcon.
        //
        // Returns:
        //     The System.Object that contains data about the System.Windows.Forms.NotifyIcon.
        [Bindable(true)]
        [DefaultValue("")]
        [Localizable(false)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag
        {
            get { return _notifyIcon.Tag; }
            set
            {
                RaisePropertyChanging("Tag");
                _notifyIcon.Tag = value;
                RaisePropertyChanged("Tag");
            }
        }

        //
        // Summary:
        //     Gets or sets the ToolTip text displayed when the mouse pointer rests on a
        //     notification area icon.
        //
        // Returns:
        //     The ToolTip text displayed when the mouse pointer rests on a notification
        //     area icon.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     ToolTip text is more than 63 characters long.
        [DefaultValue("")]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(true)]
        public string Text
        {
            get { return _notifyIcon.Text; }
            set
            {
                RaisePropertyChanging("Text");
                _notifyIcon.Text = value;
                RaisePropertyChanged("Text");
            }
        }

        //
        // Summary:
        //     Gets or sets a value indicating whether the icon is visible in the notification
        //     area of the taskbar.
        //
        // Returns:
        //     true if the icon is visible in the notification area; otherwise, false. The
        //     default value is false.
        [DefaultValue(false)]
        [Localizable(true)]
        public bool Visible
        {
            get { return _notifyIcon.Visible; }
            set
            {
                RaisePropertyChanging("Visible");
                _notifyIcon.Visible = value;
                RaisePropertyChanged("Visible");
            }
        }

        #endregion Properties (Public)

        #region Event Handlers

        // Summary:
        //     Occurs when the balloon tip is clicked.
        public event EventHandler BalloonTipClicked
        {
            add { _notifyIcon.BalloonTipClicked += value; }
            remove { _notifyIcon.BalloonTipClicked -= value; }
        }
        //
        // Summary:
        //     Occurs when the balloon tip is closed by the user.
        public event EventHandler BalloonTipClosed
        {
            add { _notifyIcon.BalloonTipClosed += value; }
            remove { _notifyIcon.BalloonTipClosed -= value; }
        }

        //
        // Summary:
        //     Occurs when the balloon tip is displayed on the screen.
        public event EventHandler BalloonTipShown
        {
            add { _notifyIcon.BalloonTipShown += value; }
            remove { _notifyIcon.BalloonTipShown -= value; }
        }

        //
        // Summary:
        //     Occurs when the user clicks the icon in the notification area.
        public event EventHandler Click
        {
            add { _notifyIcon.Click += value; }
            remove { _notifyIcon.Click -= value; }
        }

        //
        // Summary:
        //     Occurs when the user double-clicks the icon in the notification area of the
        //     taskbar.
        public event EventHandler DoubleClick
        {
            add { _notifyIcon.DoubleClick += value; }
            remove { _notifyIcon.DoubleClick -= value; }
        }

        //
        // Summary:
        //     Occurs when the user clicks a System.Windows.Forms.NotifyIcon with the mouse.
        public event MouseEventHandler MouseClick
        {
            add { _notifyIcon.MouseClick += value; }
            remove { _notifyIcon.MouseClick -= value; }
        }

        //
        // Summary:
        //     Occurs when the user double-clicks the System.Windows.Forms.NotifyIcon with
        //     the mouse.
        public event MouseEventHandler MouseDoubleClick
        {
            add { _notifyIcon.MouseDoubleClick += value; }
            remove { _notifyIcon.MouseDoubleClick -= value; }
        }

        //
        // Summary:
        //     Occurs when the user presses the mouse button while the pointer is over the
        //     icon in the notification area of the taskbar.
        public event MouseEventHandler MouseDown
        {
            add { _notifyIcon.MouseDown += value; }
            remove { _notifyIcon.MouseDown -= value; }
        }

        //
        // Summary:
        //     Occurs when the user moves the mouse while the pointer is over the icon in
        //     the notification area of the taskbar.
        public event MouseEventHandler MouseMove
        {
            add { _notifyIcon.MouseMove += value; }
            remove { _notifyIcon.MouseMove -= value; }
        }

        //
        // Summary:
        //     Occurs when the user releases the mouse button while the pointer is over
        //     the icon in the notification area of the taskbar.
        public event MouseEventHandler MouseUp
        {
            add { _notifyIcon.MouseUp += value; }
            remove { _notifyIcon.MouseUp -= value; }
        }

        #endregion Event Handlers

        #region Disposable Overrides

        public void Dispose()
        {
            _notifyIcon.Dispose();
            if (_font != null)
            {
                _font.Dispose();
                _font = null;
            }
        }

        #endregion Disposable Overrides

        #region Instance Members (Public)

        #region ShowText Overloades

        /// <summary>
        /// Shows text instead of icon in the tray
        /// </summary>
        /// <param name="text">The text to be displayed on the tray. 
        ///                    Make this only 1 or 2 characters. E.g. "23"</param>
        public void ShowText(string text)
        {
            ShowText(text, _font, _color);
        }
        /// <summary>
        /// Shows text instead of icon in the tray
        /// </summary>
        /// <param name="text">Same as above</param>
        /// <param name="color">Color to be used to display the text in the tray</param>
        public void ShowText(string text, Color color)
        {
            ShowText(text, _font, color);
        }
        /// <summary>
        /// Shows text instead of icon in the tray
        /// </summary>
        /// <param name="text">Same as above</param>
        /// <param name="font">The default color will be used but in user given font</param>
        public void ShowText(string text, Font font)
        {
            ShowText(text, font, _color);
        }
        /// <summary>
        /// Shows text instead of icon in the tray
        /// </summary>
        /// <param name="text">the text to be displayed</param>
        /// <param name="font">The font to be used</param>
        /// <param name="color">The color to be used</param>
        public void ShowText(string text, Font font, Color color)
        {
            if (_notifyIcon == null) return;

            Bitmap bitmap = new Bitmap(16, 16);//, System.Drawing.Imaging.PixelFormat.Max);

            Brush brush = new SolidBrush(color);

            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.DrawString(text, _font, brush, 0, 0);

            IntPtr hIcon = bitmap.GetHicon();
            Icon icon = Icon.FromHandle(hIcon);
            _notifyIcon.Icon = icon;

        }

        #endregion ShowText Overloads

        #region SetAnimationClip Overloads

        /// <summary>
        /// Sets the animation clip that will be displayed in the system tray
        /// </summary>
        /// <param name="icons">The array of icons which forms each frame of the animation
        ///                     This'll work by showing one icon after another in the array.
        ///                     Each of the icons must be 16x16 pixels </param>
        public void SetAnimationClip(Icon[] icons)
        {
            if (ChoGuard.IsArgumentNotNullOrEmpty(icons)) return;

            _animationIcons = icons;
        }

        /// <summary>
        /// Sets the animation clip that will be displayed in the system tray
        /// </summary>
        /// <param name="icons">The array of bitmaps which forms each frame of the animation
        ///                     This'll work by showing one bitmap after another in the array.
        ///                     Each of the bitmaps must be 16x16 pixels  </param>
        public void SetAnimationClip(Bitmap[] bitmap)
        {
            if (ChoGuard.IsArgumentNotNullOrEmpty(bitmap)) return;

            _animationIcons = new Icon[bitmap.Length];
            for (int i = 0; i < bitmap.Length; i++)
            {
                _animationIcons[i] = Icon.FromHandle(bitmap[i].GetHicon());
            }
        }

        /// <summary>
        /// Sets the animation clip that will be displayed in the system tray
        /// </summary>
        /// <param name="icons">The bitmap strip that contains the frames of animation.
        ///                     This can be created by creating a image of size 16*n by 16 pixels
        ///                     Where n is the number of frames. Then in the first 16x16 pixel put
        ///                     first image and then from 16 to 32 pixel put the second image and so on</param>
        public void SetAnimationClip(Bitmap bitmapStrip)
        {
            if (ChoGuard.IsArgumentNotNullOrEmpty(bitmapStrip)) return;

            _animationIcons = new Icon[bitmapStrip.Width / 16];
            for (int i = 0; i < _animationIcons.Length; i++)
            {
                Rectangle rect = new Rectangle(i * 16, 0, 16, 16);
                Bitmap bmp = bitmapStrip.Clone(rect, bitmapStrip.PixelFormat);
                _animationIcons[i] = Icon.FromHandle(bmp.GetHicon());
            }
        }

        #endregion SetAnimationClip Overloads

        #region ShowBalloonTip Overloads

        //
        // Summary:
        //     Displays a balloon tip in the taskbar for the specified time period.
        //
        // Parameters:
        //   timeout:
        //     The time period, in milliseconds, the balloon tip should display.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     timeout is less than 0.
        public void ShowBalloonTip(int timeout)
        {
            _notifyIcon.ShowBalloonTip(timeout);
        }

        public void ShowBalloonTip(string msg, int timeout)
        {
            _notifyIcon.BalloonTipText = msg;
            _notifyIcon.ShowBalloonTip(timeout);
        }

        //
        // Summary:
        //     Displays a balloon tip with the specified title, text, and icon in the taskbar
        //     for the specified time period.
        //
        // Parameters:
        //   timeout:
        //     The time period, in milliseconds, the balloon tip should display.
        //
        //   tipTitle:
        //     The title to display on the balloon tip.
        //
        //   tipText:
        //     The text to display on the balloon tip.
        //
        //   tipIcon:
        //     One of the System.Windows.Forms.ToolTipIcon values.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     timeout is less than 0.
        //
        //   System.ArgumentException:
        //     tipText is null or an empty string.
        //
        //   System.ComponentModel.InvalidEnumArgumentException:
        //     tipIcon is not a member of System.Windows.Forms.ToolTipIcon.
        public void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon)
        {
            _notifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);
        }

        #endregion ShowBalloonTip Overloads

        #region Animation Helper Methods

        /// <summary>
        /// Start showing the animation. This needs to be called after 
        /// setting the clip using any of the above methods
        /// </summary>
        /// <param name="loop">whether to loop infinitely or stop after one iteration</param>
        /// <param name="interval">Interval in millisecond in between each frame. Typicall 100</param>
        public void StartAnimation(int interval, int loopCount)
        {
            if (_animationIcons == null)
                throw new ArgumentNullException("Animation clip not set with SetAnimationClip");

            _loopCount = loopCount;
            _timer.Interval = interval;
            _timer.Start();
        }

        /// <summary>
        /// Stop animation started with StartAnimation with loop = true
        /// </summary>
        public void StopAnimation()
        {
            _timer.Stop();
        }

        private void timerTick(object sender, EventArgs e)
        {
            if (_currIndex < _animationIcons.Length)
            {
                _notifyIcon.Icon = _animationIcons[_currIndex];
                _currIndex++;
            }
            else
            {
                _currIndex = 0;
                if (_loopCount <= 0)
                {
                    _timer.Stop();
                    _notifyIcon.Icon = _defaultIcon;
                }
                else
                {
                    --_loopCount;
                }
            }
        }

        #endregion Animation Helper Methods

        #endregion Instance Members (Public)
    }
}
