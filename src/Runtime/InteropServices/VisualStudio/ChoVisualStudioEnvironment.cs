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

namespace Cinchoo.Core.Runtime.InteropServices
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
using System.ComponentModel;
    using Microsoft.Win32;
    using Cinchoo.Core.Collections.Generic;
using Cinchoo.Core.ComponentModel;

    #endregion NameSpaces

    public class ChoVisualStudioEnvironment : ChoObject, IDisposable
    {
        #region Instance Members (Public)

        public readonly string EnvironmentDirectory;
        public readonly string EnvironmentPath;
        public readonly string ProductDir;
        public readonly string VS7CommonBinDir;
        public readonly string VS7CommonDir;
        public readonly string VS7EnvironmentLocation;
        public readonly string Locale;

        public readonly bool IsInstalled = false;

        #endregion Instance Members (Public)

        #region Constructors

        public ChoVisualStudioEnvironment(ChoVisualStudioVersion vsVersion) : this(ChoEnum.ToDescription(vsVersion))
        {
        }

        public ChoVisualStudioEnvironment(string vsVersion)
        {
            ChoGuard.ArgumentNotNullOrEmpty(vsVersion, "VisualStudio Version");

            using (RegistryKey regKeyVS = Registry.LocalMachine.OpenSubKey(String.Format(@"SOFTWARE\Microsoft\VisualStudio\{0}\Setup\VS", vsVersion)))
            {
                if (regKeyVS == null) return;

                IsInstalled = true;

                EnvironmentDirectory = regKeyVS.GetValue("EnvironmentDirectory").ToString();
                EnvironmentPath = regKeyVS.GetValue("EnvironmentPath").ToString();
                ProductDir = regKeyVS.GetValue("ProductDir").ToString();
                VS7CommonBinDir = regKeyVS.GetValue("VS7CommonBinDir").ToString();
                VS7CommonDir = regKeyVS.GetValue("VS7CommonDir").ToString();
                VS7EnvironmentLocation = regKeyVS.GetValue("VS7EnvironmentLocation").ToString();

                using (RegistryKey regKeyBuildNumber = Registry.LocalMachine.OpenSubKey(String.Format(@"SOFTWARE\Microsoft\VisualStudio\{0}\Setup\VS\BuildNumber", vsVersion)))
                {
                    foreach (string valueName in regKeyBuildNumber.GetValueNames())
                    {
                        if (String.IsNullOrEmpty(valueName)) continue;
                        Locale = valueName;
                        break;
                    }
                }
            }
        }

        #endregion Constructors
    }
}
