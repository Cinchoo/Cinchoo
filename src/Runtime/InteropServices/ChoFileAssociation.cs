using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Cinchoo.Core.Runtime.InteropServices
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class ChoFileAssociationAttribute : Attribute
    {
        public readonly string Extension;
        public readonly string ProgID;
        public string Description;
        public string Icon;
        public string Application;

        public ChoFileAssociationAttribute(string extension, string progID)
        {
            ChoGuard.ArgumentNotNullOrEmpty(extension, "Extension");
            ChoGuard.ArgumentNotNullOrEmpty(progID, "ProgID");

            Extension = extension;
            ProgID = progID;
        }
    }

    public class ChoFileAssociation
    {
        public static void Associate(ChoFileAssociationAttribute attr)
        {
            if (attr == null) return;
            Associate(attr.Extension, attr.ProgID, attr.Description, attr.Icon, attr.Application);
        }

        // Associate file extension with progID, description, icon and application
        public static void Associate(string extension,
               string progID, string description = null, string icon = null, string application = null)
        {
            Registry.ClassesRoot.CreateSubKey(extension).SetValue("", progID);
            if (progID != null && progID.Length > 0)
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(progID))
                {
                    if (description != null)
                        key.SetValue("", description);
                    if (icon != null)
                        key.CreateSubKey("DefaultIcon").SetValue("", ToShortPathName(icon));
                    if (application != null)
                        key.CreateSubKey(@"Shell\Open\Command").SetValue("",
                                    ToShortPathName(application) + " \"%1\"");
                }
        }

        // Return true if extension already associated in registry
        public static bool IsAssociated(string extension)
        {
            return (Registry.ClassesRoot.OpenSubKey(extension, false) != null);
        }

        [DllImport("Kernel32.dll")]
        private static extern uint GetShortPathName(string lpszLongPath,
            [Out] StringBuilder lpszShortPath, uint cchBuffer);

        // Return short path format of a file name
        private static string ToShortPathName(string longName)
        {
            StringBuilder s = new StringBuilder(1000);
            uint iSize = (uint)s.Capacity;
            uint iRet = GetShortPathName(longName, s, iSize);
            return s.ToString();
        }
    }
}
