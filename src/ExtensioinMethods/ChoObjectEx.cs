#region Copyright & License
//
// Author: Raj Nagalingam (dotnetfm@google.com)
// Copyright (c) 2007-2008, NAG Groups LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

namespace System
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using System.Xml;
    using System.Reflection;
    using Cinchoo.Core;
    using System.Collections;

    #endregion NameSpaces

    public interface ICloneable<out T> : ICloneable
    {
        new T Clone();
    }

    public interface IVisitorContext
    {
    }

    public interface IVisitor<T>
    {
        void Visit(T target, IVisitorContext context);
    }

    public static partial class ChoObjectEx
    {
        #region Shared Data Members (Private)

        private static readonly XmlWriterSettings _xws = new XmlWriterSettings();

        #endregion

        static ChoObjectEx()
        {
            _xws.OmitXmlDeclaration = true;
            _xws.ConformanceLevel = ConformanceLevel.Auto;
            _xws.Indent = true;
        }

        public static bool IsNull(this object target)
        {
            return target == null;
        }

        public static bool IsNullOrDbNull(this object target)
        {
            return target == null || target == DBNull.Value;
        }

        public static T Clone<T>(this T target)
        {
            if (target != null)
            {
                if (target is ICloneable)
                    return (T)((ICloneable)target).Clone();
                else
                    return CloneObject(target);
            }
            else
                return default(T);
        }

        public static object CloneObject(this object target)
        {
            if (target != null)
            {
                using (MemoryStream buffer = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    formatter.Serialize(buffer, target);
                    buffer.Seek(0, SeekOrigin.Begin);
                    return formatter.Deserialize(buffer);
                }
            }
            else
                return null;
        }

        public static T CloneObject<T>(this T target)
        {
            if (target != null)
            {
                using (MemoryStream buffer = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    formatter.Serialize(buffer, target);
                    buffer.Seek(0, SeekOrigin.Begin);
                    return (T)formatter.Deserialize(buffer);
                }
            }
            else
                return default(T);
        }

        public static void TryDispose(this object target)
        {
            TryDispose(target, false);
        }

        public static void TryDispose(this object target, bool silent)
        {
            IDisposable disposable = target as IDisposable;
            if (disposable == null) return;

            try
            {
                disposable.Dispose();
            }
            catch (Exception)
            {
                if (!silent)
                    throw;
            }
        }

        public static bool In<T>(this T value, IEnumerable<T> values)
        {
            if (values == null) throw new ArgumentNullException("values");

            return values.Contains(value);
        }

        public static bool In<T>(this T value, params T[] values)
        {
            if (values == null) throw new ArgumentNullException("values");

            return values.Contains(value);
        }

        public static void Accept<T, V>(this T visitable, V visitor, IVisitorContext context)
              where V : IVisitor<T>
        {
            visitor.Visit(visitable, context);
        }

        public static R Pipe<T, R>(this T o, Func<T, R> func)
        {
            if (func == null) throw new ArgumentNullException("func", "'func' can't be null.");
            T buffer = o;
            return func(buffer);
        }

        public static T Pipe<T>(this T o, Action<T> action)
        {
            if (action == null) throw new ArgumentNullException("action", "'action' can't be null.");
            T buffer = o;
            action(buffer);
            return buffer;
        }

        public static bool Equals<T, TResult>(this T obj, object obj1, Func<T, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector", "'selector' can't be null.");

            return obj1 is T && selector(obj).Equals(selector((T)obj1));
        }

        public static void CheckRange(this object o, int idx, int count, int size)
        {
            if (idx < 0)
                throw new ArgumentOutOfRangeException("index");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            if ((uint)idx + (uint)count > (uint)size)
                throw new ArgumentException("index and count exceed length of list");
        }

        public static string ToNullNSXml(this object obj)
        {
            return ToNullNSXml(obj, _xws);
        }

        public static string ToNullNSXml(this object obj, XmlWriterSettings xws, XmlAttributeOverrides xmlAttributeOverrides = null)
        {
            if (obj == null)
                return null;

            StringBuilder xmlString = new StringBuilder();
            using (XmlWriter xtw = XmlTextWriter.Create(xmlString, xws ?? _xws))
            {
                ChoNullNSXmlSerializer serializer = xmlAttributeOverrides != null ? new ChoNullNSXmlSerializer(obj.GetType(), xmlAttributeOverrides) : new ChoNullNSXmlSerializer(obj.GetType());
                serializer.Serialize(xtw, obj);

                xtw.Flush();

                return xmlString.ToString();
            }
        }

        public static string ToXml(this object obj)
        {
            return ToXml(obj, _xws);
        }

        public static string ToXml(this object obj, XmlWriterSettings xws)
        {
            if (obj == null)
                return null;

            StringBuilder xmlString = new StringBuilder();
            using (XmlWriter xtw = XmlTextWriter.Create(xmlString, xws ?? new XmlWriterSettings()))
            {
                ChoNullNSXmlSerializer serializer = new ChoNullNSXmlSerializer(obj.GetType());
                serializer.Serialize(xtw, obj);

                xtw.Flush();

                return xmlString.ToString();
            }

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    XmlSerializer serializer = new XmlSerializer(obj.GetType());
            //    serializer.Serialize(ms, obj);
            //    ms.Seek(0, SeekOrigin.Begin);
            //    using (StreamReader sr = new StreamReader(ms))
            //    {
            //        // return the formatted string to caller
            //        return sr.ReadToEnd();
            //    }
            //}
        }
        #region AsDictionary Overloads

        public static IEnumerable<Tuple<string, object>> AsDictionary(this object target, bool flattenhierarchy = false, bool expandCollection = false)
        {
            if (target != null)
            {
                Type type = target.GetType();

                foreach (MemberInfo memberInfo in ChoTypeMembersCache.GetAllMemberInfos(type))
                {
                    object memberValue = ChoType.GetMemberValue(target, memberInfo);

                    if (!flattenhierarchy)
                        yield return new Tuple<string, object>(memberInfo.Name, memberValue);
                    else
                    {
                        Type memberType = ChoType.GetMemberType(memberInfo);
                        if (memberType.IsSimple())
                            yield return new Tuple<string, object>(memberInfo.Name, memberValue);
                        else if (typeof(ICollection).IsAssignableFrom(memberType))
                        {
                            if (!expandCollection || memberValue == null)
                                yield return new Tuple<string, object>(memberInfo.Name, memberValue);
                            else
                            {
                                if (((ICollection)memberValue).Count == 0)
                                    yield return new Tuple<string, object>(memberInfo.Name, null);
                                else
                                {
                                    foreach (object value in (ICollection)memberValue)
                                        yield return new Tuple<string, object>(memberInfo.Name, value);
                                }
                            }
                        }
                        else
                        {
                            foreach (Tuple<string, object> subItem in AsDictionary(memberValue, flattenhierarchy))
                            {
                                yield return subItem;
                            }
                        }
                    }
                }
            }

            yield break;
        }

        public static IEnumerable<Tuple<MemberInfo, object>> AsDictionaryWithMemberInfo(this object target, bool flattenhierarchy = false, bool expandCollection = false)
        {
            if (target != null)
            {
                Type type = target.GetType();

                foreach (MemberInfo memberInfo in ChoTypeMembersCache.GetAllMemberInfos(type))
                {
                    object memberValue = ChoType.GetMemberValue(target, memberInfo);
                    if (!flattenhierarchy)
                        yield return new Tuple<MemberInfo, object>(memberInfo, memberValue);
                    else
                    {
                        Type memberType = ChoType.GetMemberType(memberInfo);

                        if (memberType.IsSimple())
                            yield return new Tuple<MemberInfo, object>(memberInfo, memberValue);
                        else if (typeof(ICollection).IsAssignableFrom(memberType))
                        {
                            if (!expandCollection || memberValue == null)
                                yield return new Tuple<MemberInfo, object>(memberInfo, memberValue);
                            else
                            {
                                if (((ICollection)memberValue).Count == 0)
                                    yield return new Tuple<MemberInfo, object>(memberInfo, null);
                                else
                                {
                                    foreach (object value in (ICollection)memberValue)
                                        yield return new Tuple<MemberInfo, object>(memberInfo, value);
                                }
                            }
                        }
                        else
                        {
                            foreach (Tuple<MemberInfo, object> subItem in AsDictionaryWithMemberInfo(memberValue, flattenhierarchy))
                            {
                                yield return subItem;
                            }
                        }
                    }
                }
            }

            yield break;
        }

        #endregion AsDictionary Overloads

        #region AsList Overloads

        public static IEnumerable<object> AsList(this object target, bool flattenhierarchy = false, bool expandCollection = false)
        {
            if (target != null)
            {
                Type type = target.GetType();

                foreach (MemberInfo memberInfo in ChoTypeMembersCache.GetAllMemberInfos(type))
                {
                    object memberValue = ChoType.GetMemberValue(target, memberInfo);
                    if (!flattenhierarchy)
                        yield return memberValue;
                    else
                    {
                        Type memberType = ChoType.GetMemberType(memberInfo);

                        if (memberType.IsSimple())
                            yield return memberValue;
                        else if (typeof(ICollection).IsAssignableFrom(memberType))
                        {
                            if (!expandCollection || memberValue == null)
                                yield return memberValue;
                            else
                            {
                                if (((ICollection)memberValue).Count == 0)
                                    yield return null;
                                else
                                {
                                    foreach (object value in (ICollection)memberValue)
                                        yield return value;
                                }
                            }
                        }
                        else
                        {
                            foreach (Tuple<string, object> subItem in AsDictionary(memberValue, flattenhierarchy))
                            {
                                yield return subItem;
                            }
                        }
                    }
                }
            }

            yield break;
        }


        #endregion AsList Overloads
    }
}
