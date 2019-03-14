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

namespace Cinchoo.Core.Security.Cryptography
{
    #region NameSpaces

    using System;
    using System.Xml.Serialization;

    using Cinchoo.Core.Configuration;
    using System.Xml;
    using Cinchoo.Core.Xml;
    using System.IO;
    using Cinchoo.Core;


    #endregion NameSpaces

    [Serializable]
    [ChoTypeFormatter("Cryptography Settings")]
    [ChoConfigurationSection("cinchoo/AESCryptoSettings")]
    public sealed class ChoAESCryptoGraphySettings : ChoConfigurableObject, IChoObjectInitializable
    {
        #region Shared Data Members (Private)

        private static readonly byte[] _defaultKey = { 123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };
        private static readonly byte[] _defaultVector = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 25, 21, 112, 79, 32, 114, 156 };

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Public)

        [XmlAttribute("key")]
        public string Key;

        [XmlAttribute("vector")]
        public string Vector;

        #endregion Instance Data Members (Public)

        #region Shared Members (Public)

        public static ChoAESCryptoGraphySettings Me
        {
			get { return ChoConfigurationManagementFactory.CreateInstance<ChoAESCryptoGraphySettings>(); }
		}

        #endregion Shared Members (Public)

        #region IChoObjectInitializable Members

        public bool Initialize(bool beforeFieldInit, object state)
        {
            if (beforeFieldInit) return false;

            if (Key.IsNullOrEmpty()) Key = ChoByte.ToString(_defaultKey);
            if (Vector.IsNullOrEmpty()) Vector = ChoByte.ToString(_defaultVector);

            return true;
        }

        #endregion
    }
}
