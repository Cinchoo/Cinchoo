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
    using Cinchoo.Core.Types;

    #endregion NameSpaces

    /// <summary>
    /// Class used to restrict instantiation of a class to one object. This class allow the type to have constructors modifiers either Private or Protected.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class ChoSingletonHelper<T> where T : class
    {
        #region Instance Members (Internal)

        /// <summary>
        /// Method to create or get the instance of the class T.
        /// </summary>
        /// <param name="args">Constructor parameters, if any</param>
        /// <returns>Instance of the class.</returns>
        internal T Instance(bool strict, object[] args)
        {
            T _instance = default(T);

            VerifySingtonType(strict);

            if (IsCustomSingleType())
            {
                ConstructorInfo constructorInfo = GetSingletonContructor();
                if (constructorInfo != null)
                {
                    try
                    {
                        _instance = constructorInfo.Invoke(args) as T;
                    }
                    catch (Exception innerEx)
                    {
                        throw new ChoSingletonException(String.Format("Failed to invoke constructor [{0}] in `{1}` type due mismatch parametes.",
                            constructorInfo, typeof(T).FullName), innerEx);
                    }
                }
                else
                    _instance = CreateInstance(args);
            }
            else
            {
                _instance = ChoType.CreateInstance(typeof(T), args) as T;
            }

            if (_instance == null)
                throw new ChoSingletonException("Instance object is null.");

            return _instance;
        }

        #endregion Instance Members (Protected)

        #region Instance Members (Private)

        /// <summary>
        /// Helper method to verify the singleton type
        /// </summary>
        private void VerifySingtonType(bool strict)
        {
            //Make sure the type is sealed
            if (!typeof(T).IsSealed)
                throw new ChoSingletonException(String.Format("The '{0}' type must be sealed class.", typeof(T).FullName));

            //Make sure there is no static members other than self create instance
            MemberInfo[] memberInfos = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (memberInfos.Length > 0)
            {
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    if (memberInfo.MemberType == MemberTypes.Method &&
                        memberInfo.GetCustomAttributes(typeof(ChoSingletonFactoryMethodAttribute), false).Length > 0) continue;

                    throw new ChoSingletonException(String.Format("The '{0}' type must not have STATIC [{1}] members other than self create instance STATIC method.", typeof(T).FullName,
                        memberInfo.ToString()));
                }
            }
                
            ConstructorInfo[] constructorInfos = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (strict)
            {
                foreach (ConstructorInfo constructorInfo in constructorInfos)
                {
                    if (!constructorInfo.IsPrivate)
                        throw new ChoSingletonException(String.Format("One of constructor [{0}] in '{1}' type has modifier specified other than private. All the constructors must be private.",
                            constructorInfo.ToString(), typeof(T).FullName));
                }
            }
            else
            {
                foreach (ConstructorInfo constructorInfo in constructorInfos)
                {
                    if (!constructorInfo.IsPrivate && !constructorInfo.IsFamily)
                        throw new ChoSingletonException(String.Format("One of constructor [{0}] in '{1}' has modifier specified other than private / protected. All the constructors must be private/protected.",
                            constructorInfo.ToString(), typeof(T).FullName));
                }
            }

            //Make sure the type have only one constructor
            if (constructorInfos.Length > 1)
            {
                if (typeof(T).GetCustomAttributes(typeof(ChoCustomSingletonTypeAttribute), false).Length == 1)
                {
                    if (GetSingletonContructor() != null) return;
                }
                if (!HasFactoryMethodDefined())
                    throw new ChoSingletonException(String.Format("The '{0}' type must have only one private constructor. Or decorate a constructor with ChoSingetonConstructorAttribute.", typeof(T).FullName));
            }
        }

        /// <summary>
        /// Helper method to get the constructor decorated with ChoSingletonConstructorAttribute
        /// </summary>
        /// <returns></returns>
        private ConstructorInfo GetSingletonContructor()
        {
            foreach (ConstructorInfo constructorInfo in typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (constructorInfo.GetCustomAttributes(typeof(ChoSingletonConstructorAttribute), false).Length == 0) continue;

                return constructorInfo;
            }

            return null;
        }

        /// <summary>
        /// Helper method to check the class has ChoSingletonTypeAttribute declared.
        /// </summary>
        /// <returns>true, if the attribute is declared. Otherwise false.</returns>
        private bool IsCustomSingleType()
        {
            return typeof(T).GetCustomAttributes(typeof(ChoCustomSingletonTypeAttribute), true).Length > 0;
        }

        /// <summary>
        /// Helper method to inspect the type have Factory method defined or not.
        /// </summary>
        /// <returns></returns>
        private bool HasFactoryMethodDefined()
        {
            foreach (MethodInfo methodInfo in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (methodInfo.GetCustomAttributes(typeof(ChoSingletonFactoryMethodAttribute), false).Length == 0) continue;

                if (methodInfo.IsStatic)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Helper method to call self create instance method.
        /// </summary>
        /// <param name="args">List of parameters required by self create instance method.</param>
        /// <returns>Instance of the class return by self create instance method.</returns>
        private T CreateInstance(object[] args)
        {
            foreach (MethodInfo methodInfo in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (methodInfo.GetCustomAttributes(typeof(ChoSingletonFactoryMethodAttribute), false).Length == 0) continue;

                if (methodInfo.IsStatic)
                {
                    if (methodInfo.IsConstructor || methodInfo.IsGenericMethod || methodInfo.IsGenericMethodDefinition
                        && (typeof(T) != methodInfo.ReturnType || !typeof(T).IsSubclassOf(methodInfo.ReturnType)))
                        throw new ChoSingletonException(String.Format("Invalid Singleton Self Create Instance Method found in '{0}' type.",
                            typeof(T).FullName));

                    if (methodInfo.GetParameters().Length == 0)
                        return methodInfo.Invoke(null, null) as T;
                    else
                    {
                        try
                        {
                            return methodInfo.Invoke(null, args) as T;
                        }
                        catch (Exception innerEx)
                        {
                            throw new ChoSingletonException(String.Format("Can't find self create instance method with matching parameters [{0}] in '{1}' type.",
                                ChoString.Join(ChoType.ConvertToTypes(args))), innerEx);
                        }
                    }
                }
                break;
            }

            throw new ChoSingletonException("Missing Singleton Self Create Instance Method. It should be STATIC method.");
        }

        #endregion Instance Members (Private)
    }
}
