namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Xml;
    using System.Text;
    using System.Collections;
    using System.Configuration;
    using System.Collections.Generic;

    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.Configuration;
	using System.Threading;

    #endregion

    public enum FrameworkFamily { Microsoft, Mono }
    public enum Platform { Win32, Unix, MacOS, Linux }

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
