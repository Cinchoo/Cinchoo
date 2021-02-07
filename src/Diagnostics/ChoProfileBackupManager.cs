using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core.Diagnostics
{
    internal static class ChoProfileBackupManager
    {
        private static readonly object _padLock = new object();
        private static string _backupDir;

        public static void Reset()
        {
            lock (_padLock)
            {
                _backupDir = null;
            }
        }

        public static void Register(string backupDir)
        {
            lock (_padLock)
            {
                _backupDir = backupDir;
            }
        }

        public static string GetBackupDir()
        {
            lock (_padLock)
            {
                return _backupDir;
            }
        }
    }
}
