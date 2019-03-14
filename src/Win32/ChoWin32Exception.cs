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
    using System.IO;
    using System.Text;
    using System.Security;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Runtime.Serialization;
    using System.Runtime.InteropServices;

    using Cinchoo.Core;
    using Cinchoo.Core.Win32;

    #endregion NameSpaces

    [Serializable, SuppressUnmanagedCodeSecurity, HostProtection(SecurityAction.LinkDemand, SharedState = true)]
    public class ChoWin32Exception : ExternalException, ISerializable
    {
        #region ChoIntSecurity Class

        [HostProtection(SecurityAction.LinkDemand, SharedState = true)]
        private static class ChoIntSecurity
        {
            // Fields
            public static readonly CodeAccessPermission FullReflection = new ReflectionPermission(PermissionState.Unrestricted);
            public static readonly CodeAccessPermission UnmanagedCode = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);

            // Methods
            public static string UnsafeGetFullPath(string fileName)
            {
                string fullPath = fileName;
                new FileIOPermission(PermissionState.None) { AllFiles = FileIOPermissionAccess.PathDiscovery }.Assert();
                try
                {
                    fullPath = Path.GetFullPath(fileName);
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
                return fullPath;
            }
        }

        #endregion ChoIntSecurity Class

        // Fields
        private readonly int nativeErrorCode;

        // Methods
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public ChoWin32Exception()
            : this(Marshal.GetLastWin32Error())
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public ChoWin32Exception(int error)
            : this(error, GetErrorMessage(error), true)
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public ChoWin32Exception(string message)
            : this(Marshal.GetLastWin32Error(), GetErrorMessage(Marshal.GetLastWin32Error(), message), true)
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public ChoWin32Exception(int error, string message)
            : base(GetErrorMessage(error, message))
        {
            this.nativeErrorCode = error;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public ChoWin32Exception(string message, Exception innerException)
            : base(GetErrorMessage(Marshal.GetLastWin32Error(), message), innerException)
        {
            this.nativeErrorCode = Marshal.GetLastWin32Error();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private ChoWin32Exception(int error, string message, bool dummy)
            : base(message)
        {
            this.nativeErrorCode = error;
        }

        protected ChoWin32Exception(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ChoIntSecurity.UnmanagedCode.Demand();
            this.nativeErrorCode = info.GetInt32("NativeErrorCode");
        }

        private static string GetErrorMessage(int error)
        {
            return GetErrorMessage(error, null);
        }

        private static string GetErrorMessage(int error, string customErrMsg)
        {
            string lastErrMsg = null;
            customErrMsg = customErrMsg.Trim(); //.NTrim();

            try
            {
                StringBuilder lpBuffer = new StringBuilder(0x100);
                if (ChoKernel32.FormatMessage(0x3200, Win32Common.NullHandleRef, error, 0, lpBuffer, lpBuffer.Capacity + 1, IntPtr.Zero) == 0)
                    lastErrMsg = "Unknown error (0x" + Convert.ToString(error, 0x10) + ")";

                int length = lpBuffer.Length;
                while (length > 0)
                {
                    char ch = lpBuffer[length - 1];
                    if ((ch > ' ') && (ch != '.'))
                    {
                        break;
                    }
                    length--;
                }
                lastErrMsg = lpBuffer.ToString(0, length);
            }
            finally
            {
                if (!String.IsNullOrEmpty(customErrMsg))
                {
                    if (customErrMsg.EndsWith("."))
                        lastErrMsg = String.Format("{0} {1}.", customErrMsg, lastErrMsg);
                    else
                        lastErrMsg = String.Format("{0}. {1}.", customErrMsg, lastErrMsg);
                }
            }

            return lastErrMsg;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue("NativeErrorCode", this.nativeErrorCode);
            base.GetObjectData(info, context);
        }

        // Properties
        public int NativeErrorCode
        {
            get
            {
                return this.nativeErrorCode;
            }
        }
    }
}