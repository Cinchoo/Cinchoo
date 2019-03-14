#region Header

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Copyright (c) 2009-2010 Raj Nagalingam.
 *    All rights reserved.
 *
 * This program and the accompanying materials are made available under
 * the terms of the Common Public License v1.0 which accompanies this
 * distribution.
 *
 * Redistribution and use in source and binary forms, with or
 * without modification, are permitted provided that the following
 * conditions are met:
 *
 * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in
 * the documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
 * OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 *<author>Raj Nagalingam</author>
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace Cinchoo.Core.Win32
{
    #region NameSpaces

    using System;
    using System.Runtime.InteropServices;

    #endregion NameSpaces

    public static partial class ChoAdvAPI32
    {
        // Win32 function to open the service control manager
        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string machineName, string databaseName, uint dwAccess);

        // Win32 function to open a service instance
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        // Win32 function to lock the service database to perform write operations.
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr LockServiceDatabase(IntPtr hSCManager);

        // Win32 function to change the service config for the failure actions.
        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2", SetLastError=true, CharSet=CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceFailureActions(IntPtr hService, int dwInfoLevel,
            [MarshalAs(UnmanagedType.Struct)] ref SERVICE_FAILURE_ACTIONS lpInfo);
        
        [DllImport("advapi32.dll", SetLastError=true, CharSet=CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfig2(IntPtr hService, int dwInfoLevel, IntPtr lpInfo);

        // Win32 function to change the service config for the service description
        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceDescription(IntPtr hService, int dwInfoLevel, [MarshalAs(UnmanagedType.Struct)] ref SERVICE_DESCRIPTION lpInfo);

        [DllImport("advapi32.dll", EntryPoint = "QueryServiceConfig2")]
        public static extern int QueryServiceConfig2(IntPtr hService, SERVCE_CONFIG_INFO_LEVEL dwInfoLevel, IntPtr lpBuffer, int cbBufSize, out int pcbBytesNeeded);

        // Win32 function to close a service related handle.
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseServiceHandle(IntPtr hSCObject);

        // Win32 function to unlock the service database.
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnlockServiceDatabase(IntPtr hSCManager);

        // This method adjusts privileges for this process which is needed when
        // specifying the reboot option for a service failure recover action.
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges,
            [MarshalAs(UnmanagedType.Struct)] ref TOKEN_PRIVILEGES NewState, int BufferLength, [MarshalAs(UnmanagedType.Struct)] ref TOKEN_PRIVILEGES PreviousState, ref int ReturnLength);

        // Looks up the privilege code for the privilege name
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, ref LUID lpLuid);

        // Opens the security/privilege token for a process handle
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);

        public static bool GrandShutdownPrivilege()
        {
            // This code mimics the MSDN defined way to adjust privilege for shutdown
            // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/sysinfo/base/shutting_down.asp

            IntPtr tokenHandle = IntPtr.Zero;
            try
            {
                if (!OpenProcessToken(ChoKernel32.GetCurrentProcess(), (int)Win32Common.TOKEN_ADJUST_PRIVILEGES | (int)Win32Common.TOKEN_QUERY, ref tokenHandle))
                    return false;

                LUID luid = new LUID();
                LookupPrivilegeValue(null, Win32Common.SE_SHUTDOWN_NAME, ref luid);

                TOKEN_PRIVILEGES tokenPrivileges = new TOKEN_PRIVILEGES();
                tokenPrivileges.PrivilegeCount = 1;
                tokenPrivileges.Privileges = new LUID_AND_ATTRIBUTES[] { new LUID_AND_ATTRIBUTES(luid, (int)Win32Common.SE_PRIVILEGE_ENABLED) };

                TOKEN_PRIVILEGES tokenPrivileges1 = new TOKEN_PRIVILEGES();

                int retLen = 0;
                bool result = AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivileges, 0, ref tokenPrivileges1, ref retLen);

                if (ChoKernel32.GetLastError() != 0)
                    throw new Exception("Failed to grant shutdown privilege.");

                return true;
            }
            finally
            {
                if (tokenHandle != IntPtr.Zero)
                    ChoKernel32.CloseHandle(tokenHandle);
            }
        }
    }
}
