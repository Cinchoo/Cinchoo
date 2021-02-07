using Cinchoo.Core.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cinchoo.Core.Shell
{
    public static class ChoShellFileAssociation
    {
        public static void Register()
        {
            object[] attrArr = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(ChoShellFileAssociationAttribute), false);
            if (attrArr == null) return;

            foreach (object attr in attrArr)
            {
                if (attr is ChoShellFileAssociationAttribute)
                {
                    ((ChoShellFileAssociationAttribute)attr).Register();
                }
            }
            ChoShell32.SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }

        public static void Unregister()
        {
            object[] attrArr = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(ChoShellFileAssociationAttribute), false);
            if (attrArr == null) return;

            foreach (object attr in attrArr)
            {
                if (attr is ChoShellFileAssociationAttribute)
                {
                    ((ChoShellFileAssociationAttribute)attr).Unregister();
                }
            }
            ChoShell32.SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
