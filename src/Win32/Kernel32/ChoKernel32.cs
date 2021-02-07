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
    using System.Text;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Runtime.ConstrainedExecution;
    using Microsoft.Win32.SafeHandles;

    #endregion NameSpaces

    // A delegate type to be used as the handler routine 
    // for SetConsoleCtrlHandler.
    public delegate Boolean ConsoleCtrlMessageHandler(CtrlTypes CtrlType);

    public static class ChoKernel32
    {
        private const string Kernel32DllName = "kernel32.dll";

        [DllImport(Kernel32DllName)]
        public static extern bool AllocConsole();

        [DllImport(Kernel32DllName)]
        public static extern bool FreeConsole();

        [DllImport(Kernel32DllName)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport(Kernel32DllName)]
        public static extern int GetConsoleOutputCP();

        // Gets the current process handle
        [DllImport(Kernel32DllName)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport(Kernel32DllName)]
        public static extern int GetLastError();

        [DllImport(Kernel32DllName)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        // Methods
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport(Kernel32DllName, CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr handle);
        
        [DllImport(Kernel32DllName, CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool CloseHandle(HandleRef handle);
        
        [DllImport(Kernel32DllName, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern SafeWaitHandle CreateSemaphore(SECURITY_ATTRIBUTES lpSecurityAttributes, int initialCount, int maximumCount, string name);
        
        [DllImport("perfcounter.dll", CharSet = CharSet.Auto)]
        public static extern int FormatFromRawValue(uint dwCounterType, uint dwFormat, ref long pTimeBase, PDH_RAW_COUNTER pRawValue1, PDH_RAW_COUNTER pRawValue2, PDH_FMT_COUNTERVALUE pFmtValue);
        
        [DllImport(Kernel32DllName, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int FormatMessage(int dwFlags, HandleRef lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, IntPtr arguments);
        
        [DllImport(Kernel32DllName, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int FormatMessage(int dwFlags, SafeHandle lpSource, uint dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, IntPtr[] arguments);
        
        [DllImport(Kernel32DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool FreeLibrary(HandleRef hModule);
        
        [DllImport(Kernel32DllName, CharSet = CharSet.Auto)]
        public static extern bool GetComputerName(StringBuilder lpBuffer, int[] nSize);
        
        [DllImport(Kernel32DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr LoadLibrary(string libFilename);
        
        [DllImport(Kernel32DllName, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern SafeWaitHandle OpenSemaphore(int desiredAccess, bool inheritHandle, string name);
        
        [DllImport(Kernel32DllName, CharSet = CharSet.Auto)]
        public static extern void OutputDebugString(string message);
        
        [DllImport(Kernel32DllName)]
        public static extern bool QueryPerformanceCounter(out long value);
        
        [DllImport(Kernel32DllName)]
        public static extern bool QueryPerformanceFrequency(out long value);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport(Kernel32DllName, SetLastError = true)]
        public static extern bool ReleaseSemaphore(SafeWaitHandle handle, int releaseCount, out int previousCount);

        [DllImport(Kernel32DllName, SetLastError = true)]
        public static extern bool SetConsoleCtrlHandler(ConsoleCtrlMessageHandler consoleCtrlRoutine, bool Add);

        [DllImport(Kernel32DllName)]
        public static extern int RegisterApplicationRestart(
            [MarshalAs(UnmanagedType.BStr)] string commandLineArgs,
            int flags);

        [DllImport(Kernel32DllName)]
        public static extern int UnregisterApplicationRestart();

        [DllImport(Kernel32DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetApplicationRestartSettings(
            IntPtr process,
            IntPtr commandLine,
            ref uint size,
            out uint flags);

        [DllImport(Kernel32DllName)]
        public static extern uint GetShortPathName(string lpszLongPath,
            [Out] StringBuilder lpszShortPath, uint cchBuffer);
    }
}
