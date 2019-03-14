﻿#region Header

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

namespace Cinchoo.Core.Pattern
{
    /// <summary>
    /// Class used to restrict instantiation of a class to one object. This class allow the type to have constructors modifiers as Private only.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ChoSingleton<T>
        where T : class
    {
        #region Shared Data Members (Private)

        /// <summary>
        /// Synchronization object
        /// </summary>
        private static readonly object _padLock = new object();

        /// <summary>
        /// Instance of an object.
        /// </summary>
        private volatile static T _instance;

        private static bool _isInitizlied = false;

        #endregion Instance Data Members (Private)

        #region Shared Members (Public)

        public static void Initialize(params object[] args)
        {
            Initialize(SingletonTypeValidationRules.DoNotAllowAll, args);
        }

        public static void Initialize(SingletonTypeValidationRules singletonTypeValidationRules, params object[] args)
        {
            T obj = GetInstance(singletonTypeValidationRules, args);
        }

        public static T GetInstance(params object[] args)
        {
            return GetInstance(SingletonTypeValidationRules.DoNotAllowAll, args);
        }

        /// <summary>
        /// Method to create or get the instance of the class T.
        /// </summary>
        /// <param name="args">Constructor parameters, if any</param>
        /// <returns>Instance of the class.</returns>
        public static T GetInstance(SingletonTypeValidationRules singletonTypeValidationRules, params object[] args)
        {
            if (_instance != null) return _instance;

            lock (_padLock)
            {
                if (_instance != null) return _instance;

                _instance = new ChoSingletonHelper<T>().GetInstance(false, args, singletonTypeValidationRules);
                _isInitizlied = true;

                return _instance;
            }
        }

        public static T Instance
        {
            get
            {
                if (!IsInitialized)
                    throw new ChoSingletonException(string.Format("Singleton<{0}> is not initialized. Please call either GetInstance() or Initialize() methods first.",
                        typeof(T).FullName));

                return _instance;
            }
        }

        public static bool IsInitialized
        {
            get { return _isInitizlied; }
        }

        #endregion Shared Members (Public)

        #region Object Overrides (Public)

        public new static string ToString()
        {
            return ChoObject.ToString(_instance);
        }

        #endregion Object Overrides (Public)
    }
}


