using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Cinchoo.Core
{
    public static class ChoWindowsIdentity
    {
        public static bool IsAdministrator()
        {
            // Check administrative role
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator));
        }
    }
}
