namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Xml;
    using System.Text;
    using System.Linq;
    using System.Collections;
    using System.Configuration;
    using System.Collections.Generic;

    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.Configuration;
    using System.Threading;
    using System.Diagnostics;
    using System.IO;
    using Cinchoo.Core.Reflection;
    using System.Reflection;
    using System.Drawing;
    using System.Windows.Forms;

    #endregion

    public enum FrameworkFamily { Microsoft, Mono }
    public enum Platform { Win32, Unix, MacOS, Linux }

    public static class ChoEnvironment
    {
        static ChoEnvironment()
        {
            CommandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();

            //ChoFrameworkCmdLineArgs frameworkCmdLineArgs = new ChoFrameworkCmdLineArgs();
            //if (!frameworkCmdLineArgs.CommandLineArgsFile.IsNullOrWhiteSpace()
            //    && File.Exists(frameworkCmdLineArgs.CommandLineArgsFile))
            //{
            //    try
            //    {
            //        CommandLineArgs = File.ReadAllText(frameworkCmdLineArgs.CommandLineArgsFile).SplitNTrim();
            //    }
            //    catch { }
            //}
        }

        public static bool AtLeastVista()
        {
            return (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6);
        }

        public static string DomainUserName
        {
            get { return Environment.UserDomainName + "\\" + Environment.UserName; }
        }

        public static string ToDomainUserName(string userName)
        {
            if (userName.Contains(@"\"))
                return userName;
            else
                return @"{0}\{1}".FormatString(Environment.UserDomainName, userName);
        }

        private static string[] _commandLineArgs = new string[] { };
        public static string[] CommandLineArgs
        {
            get { return _commandLineArgs; }
            set
            {
                if (value == null)
                    _commandLineArgs = new string[] { };
                else
                    _commandLineArgs = value;
            }
        }

        public static string ToFullyQualifiedCommandLineArgs()
        {
            if (CommandLineArgs != null)
                return "{0} {1}".FormatString(ChoAssembly.GetEntryAssembly().Location, String.Join(" ", 
                    CommandLineArgs.Select((x) => x.Contains(' ') ? "\"{0}\"".FormatString(x) : x)));
            else
                return "{0} {1}".FormatString(ChoAssembly.GetEntryAssembly().Location, String.Empty);
        }

        public static void Exit(int exitCode, Exception ex)
        {
            Exit(exitCode, ex == null ? "Fatal application error occured." : ChoApplicationException.ToString(ex));
        }

        public static void Exit(int exitCode, string errMsg)
        {
            ChoApplication.WriteToEventLog(errMsg, EventLogEntryType.Error);
            ChoApplication.OnFatalApplicationException(exitCode, errMsg);
        }
        // Return a bitmap containing the UAC shield.
        private static Bitmap _shieldBmp = null;
        public static Bitmap GetUACShieldImage()
        {
            if (_shieldBmp != null) return _shieldBmp;

            const int WID = 50;
            const int HGT = 50;
            const int MARGIN = 4;

            // Make the button. For some reason, it must
            // have text or the UAC shield won't appear.
            Button btn = new Button();
            btn.Text = " ";
            btn.Size = new Size(WID, HGT);
            btn.SetUACShield();

            // Draw the button onto a bitmap.
            Bitmap bm = new Bitmap(WID, HGT);
            btn.Refresh();
            btn.DrawToBitmap(bm, new Rectangle(0, 0, WID, HGT));

            // Find the part containing the shield.
            int min_x = WID, max_x = 0, min_y = HGT, max_y = 0;

            // Fill on the left.
            for (int y = MARGIN; y < HGT - MARGIN; y++)
            {
                // Get the leftmost pixel's color.
                Color target_color = bm.GetPixel(MARGIN, y);

                // Fill in with this color as long as we see the target.
                for (int x = MARGIN; x < WID - MARGIN; x++)
                {
                    // See if this pixel is part of the shield.
                    if (bm.GetPixel(x, y).Equals(target_color))
                    {
                        // It's not part of the shield.
                        // Clear the pixel.
                        bm.SetPixel(x, y, Color.Transparent);
                    }
                    else
                    {
                        // It's part of the shield.
                        if (min_y > y) min_y = y;
                        if (min_x > x) min_x = x;
                        if (max_y < y) max_y = y;
                        if (max_x < x) max_x = x;
                    }
                }
            }

            // Clip out the shield part.
            int shield_wid = max_x - min_x + 1;
            int shield_hgt = max_y - min_y + 1;
            _shieldBmp = new Bitmap(shield_wid, shield_hgt);
            Graphics shield_gr = Graphics.FromImage(_shieldBmp);
            shield_gr.DrawImage(bm, 0, 0,
                new Rectangle(min_x, min_y, shield_wid, shield_hgt),
                GraphicsUnit.Pixel);

            // Return the shield.
            return _shieldBmp;
        }
    }

    //    /// <summary>
    //    /// Provides .NET / System environment settings information
    //    /// </summary>
    //    public static class ChoEnvironment
    //    {
    //        #region Shared Data Members (Private)

    //        private static FrameworkFamily _frameworkFamily = FrameworkFamily.Microsoft;
    //        private static Platform _platform = Platform.Win32;

    //        #endregion Shared Data Members (Private)

    //        #region Constructor

    //        static ChoEnvironment()
    //        {
    //            _frameworkFamily = Type.GetType("System.MonoType", false) != null ? FrameworkFamily.Mono : FrameworkFamily.Microsoft;
    //            _platform = (int)Environment.OSVersion.Platform != 4 && (int)Environment.OSVersion.Platform != 128 ? Platform.Win32 : Platform.Unix;
    //        }

    //        #endregion

    //        #region Shared Members (Public)

    //        private static ChoFramework GetRuntimeFramework()
    //        {
    //#if NET_2_0
    //            XmlNode eSquareNode = (XmlNode)ChoConfigurationManager.GetSection("Cinchoo");
    //#else
    //            XmlNode eSquareNode = null; // (XmlNode)ChoConfigurationManager.GetSection("Cinchoo");
    //#endif
    //            if (eSquareNode == null)
    //            {
    //                ChoTrace.WarnFormat("The \"Cinchoo\" section in the ESC Engine"
    //                    + " configuration file ({0}) is not available.",
    //                    AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
    //                return null;
    //            }

    //            XmlElement frameworkNode = (XmlElement)eSquareNode.SelectSingleNode("frameworks/platform[@name='" + Platform + "']/framework[@family='" + FrameworkFamily + "' and @clrversion='" + Environment.Version.ToString(3) + "']");
    //            if (frameworkNode == null)
    //            {
    //                System.Console.WriteLine("The Cinchoo configuration file ({0})"
    //                    + " does not have a <framework> node for the current"
    //                    + " runtime framework.",
    //                    AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
    //                System.Console.WriteLine(string.Empty);
    //                System.Console.WriteLine("Please add a <framework> node"
    //                    + " with family '{0}' and clrversion '{1}' under the"
    //                    + " '{2}' platform node.", FrameworkFamily,
    //                    Environment.Version.ToString(3), Platform);
    //                return null;
    //            }

    //            string frameworkVersion = frameworkNode.GetAttribute("version");

    //            XmlNodeList includeNodes = frameworkNode.SelectNodes("runtime/probing-paths/directory");
    //            ArrayList includes = new ArrayList(includeNodes.Count);
    //            foreach (XmlNode node in includeNodes)
    //            {
    //                XmlElement includeNode = (XmlElement)node;
    //                string name = includeNode.GetAttribute("name");
    //                includes.Add(name);
    //            }

    //            string[] probePaths = new string[includes.Count];
    //            includes.CopyTo(probePaths, 0);
    //            return new ChoFramework(frameworkVersion, probePaths);
    //        }

    //        #endregion Shared Members (Public)

    //        #region Shared Members (Public)

    //        public static FrameworkFamily FrameworkFamily
    //        {
    //            get { return _frameworkFamily; }
    //        }

    //        public static Platform Platform
    //        {
    //            get { return _platform; }
    //        }

    //        #endregion Shared Members (Public)
    //    }
}
