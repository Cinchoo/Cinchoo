using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core.AutoUpdater
{
    public class ChoAutoUpdater
    {
        public bool CheckForUpdates()
        {
            return false;
        }

        public bool IsCriticalUpdate()
        {
            return false;
        }

        public bool Download()
        {
            return true;
        }

        public bool StartApp()
        {
            return true;
        }
    }
}
