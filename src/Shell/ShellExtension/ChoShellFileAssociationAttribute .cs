using Cinchoo.Core.IO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;

namespace Cinchoo.Core.Shell
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class ChoShellFileAssociationAttribute : Attribute
    {
        public readonly string Extension;
        public string AdditionalArguments;
        public string DefaultArgSwitch;

        private int _defaultIconIndex;
        public int DefaultIconIndex
        {
            get { return _defaultIconIndex; }
            set
            {
                if (value >= 0)
                    _defaultIconIndex = value;
            }
        }

        private string _progID;
        public string ProgID
        {
            get { return _progID.IsNull() ? "{0}File".FormatString(Extension.Substring(1)) : _progID; }
            set { _progID = value; }
        }

        private string _defaultIcon;
        public string DefaultIcon
        {
            get { return _defaultIcon; }
            set { _defaultIcon = value; }
        }

        private string _description;
        public string Description
        {
            get { return _description.IsNull() ? "{0} Document".FormatString(Extension.Substring(1).ToUpper()) : _description; }
            set { _description = value; }
        }

        private string _application;
        public string Application
        {
            get 
            { 
                return _application.IsNullOrWhiteSpace() ? ChoApplication.EntryAssemblyLocation : _application; 
            }
            set { _application = ChoPath.ToShortFileName(value); }
        }

        public ChoShellFileAssociationAttribute(string extension)
        {
            ChoGuard.ArgumentNotNullOrEmpty(extension, "Extension");
            if (!extension.StartsWith("."))
                Extension = ".{0}".FormatString(extension);
            else
                Extension = extension;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrators")]
        public bool IsAssociated()
        {
            return (Registry.ClassesRoot.OpenSubKey(Extension, false) != null);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrators")]
        public void Register()
        {
            if (DefaultIcon.IsNullOrWhiteSpace())
            {
                DefaultIcon = "{0},{1}".FormatString(ChoPath.ToShortFileName(ChoApplication.EntryAssemblyLocation), DefaultIconIndex);
            }

            Registry.ClassesRoot.CreateSubKey(Extension, RegistryKeyPermissionCheck.ReadWriteSubTree).SetValue("", ProgID);

            if (!ProgID.IsNullOrWhiteSpace())
            {
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(ProgID))
                {
                    if (Description != null)
                        key.SetValue("", Description);
                    if (DefaultIcon != null)
                        key.CreateSubKey("DefaultIcon").SetValue("", DefaultIcon);
                    if (Application != null)
                        key.CreateSubKey(@"Shell\Open\Command").SetValue("",
                                    "{0} {1}\"%1\" {2}".FormatString(Application, DefaultArgSwitch, AdditionalArguments));
                }
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrators")]
        public void Unregister()
        {
            Registry.ClassesRoot.DeleteSubKeyTree(Extension, false);
            if (!ProgID.IsNullOrWhiteSpace())
                Registry.ClassesRoot.DeleteSubKeyTree(ProgID, false);
        }
    }
}
