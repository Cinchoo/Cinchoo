#region Header

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Copyright (c) 2007-2008 Raj Nagalingam.
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

namespace Cinchoo.Core.Pattern.Singleton
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;

    using Cinchoo.Core.Pattern.Singleton.Exceptions;
    using Cinchoo.Core.Pattern.Singleton.Attributes;

    #endregion NameSpaces

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
        private static T _instance;

        #endregion Instance Data Members (Private)

        #region Shared Members (Public)

        /// <summary>
        /// Method to create or get the instance of the class T.
        /// </summary>
        /// <param name="args">Constructor parameters, if any</param>
        /// <returns>Instance of the class.</returns>
        public static T Instance(params object[] args)
        {
            if (_instance != null) return _instance;

            lock (_padLock)
            {
                if (_instance != null) return _instance;

                _instance = new ChoSingletonHelper<T>().Instance(false, args);
                return _instance;
            }
        }

        #endregion Shared Members (Public)
    }
}


