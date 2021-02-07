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
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;

    using Cinchoo.Core;

    #endregion NameSpaces

    public class ChoAESCryptography : IDisposable
    {
        #region Instance Data Members (Private)

        private ICryptoTransform _encryptTransform;
        private ICryptoTransform _decryptTransform;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoAESCryptography(string key = null, string vector = null) 
            : this(key.IsNullOrWhiteSpace() ? ChoAESCryptoGraphySettings.Me.Key.ToByteArray() : key.ToByteArray(), 
                vector.IsNullOrWhiteSpace() ? ChoAESCryptoGraphySettings.Me.Vector.ToByteArray() : vector.ToByteArray())
        {
        }

        public ChoAESCryptography(byte[] key, byte[] vector)
        {
            ChoGuard.ArgumentNotNull(key, "Key");
            ChoGuard.ArgumentNotNull(vector, "Vector");

            //This is our encryption method
            RijndaelManaged rm = new RijndaelManaged();

            //Create an encryptor and a decryptor using our encryption method, key, and vector.
            _encryptTransform = rm.CreateEncryptor(key, vector);
            _decryptTransform = rm.CreateDecryptor(key, vector);
            Encoding = Encoding.UTF8;
        }

        public Encoding Encoding
        {
            get;
            set;
        }

        #endregion Constructors

        #region Instance Members (Public)

        /// Encrypt some text and return an encrypted byte array.
        public string Encrypt(string text)
        {
            //Translates our text value into a byte array.
            Byte[] bytes = Encoding.GetBytes(text);

            //Used to stream the data in and out of the CryptoStream.
            using (MemoryStream memoryStream = new MemoryStream())
            {
                //Write the decrypted value to the encryption stream
                using (CryptoStream cs = new CryptoStream(memoryStream, _encryptTransform, CryptoStreamMode.Write))
                {
                    cs.Write(bytes, 0, bytes.Length);
                    cs.FlushFinalBlock();

                    //Read encrypted value back out of the stream
                    memoryStream.Position = 0;

                    byte[] encrypted = new byte[memoryStream.Length];
                    memoryStream.Read(encrypted, 0, encrypted.Length);

                    return ChoByte.ToString(encrypted);
                }
            }
        }

        /// Decryption when working with byte arrays.    
        public string Decrypt(string text)
        {
            byte[] encrypted = text.ToByteArray();

            //Write the encrypted value to the decryption stream
            using (MemoryStream encryptedStream = new MemoryStream())
            {
                using (CryptoStream decryptStream = new CryptoStream(encryptedStream, _decryptTransform, CryptoStreamMode.Write))
                {
                    decryptStream.Write(encrypted, 0, encrypted.Length);
                    decryptStream.FlushFinalBlock();

                    //Read the decrypted value from the stream.
                    encryptedStream.Position = 0;
                    Byte[] decryptedBytes = new Byte[encryptedStream.Length];
                    encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);

                    return Encoding.GetString(decryptedBytes);
                }
            }
        }

        #endregion Instance Members (Public)

        #region Shared Members (Public)

        public static byte[] GenerateEncryptionKey()
        {
            //Generate a Key.
            RijndaelManaged rm = new RijndaelManaged();
            rm.GenerateKey();
            return rm.Key;
        }

        public static byte[] GenerateEncryptionVector()
        {
            //Generate a Vector
            RijndaelManaged rm = new RijndaelManaged();
            rm.GenerateIV();
            return rm.IV;
        }

        public static string GenerateEncryptionKeyString()
        {
            return ChoByte.ToString(GenerateEncryptionKey());
        }

        public static string GenerateEncryptionVectorString()
        {
            return ChoByte.ToString(GenerateEncryptionVector());
        }

        #endregion Shared Members (Public)

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
