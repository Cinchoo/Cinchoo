namespace Cinchoo.Core.Win32
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.InteropServices;

    #endregion

    public static class ChoMpr
    {
        public const int CONNECT_UPDATE_PROFILE = 0x1;
        private const string MbrDllName = "mpr.dll";

        [DllImport(MbrDllName)]
        public static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flags);
        [DllImport(MbrDllName)]
        public static extern int WNetCancelConnection2(string name, int flags, bool force);
    }
}
