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
    using Cinchoo.Core.Xml.Serialization;
    using System.Text.RegularExpressions;

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

        public static string ToNString(this object target)
        {
            if (target == null) return null;
            return target.ToString();
        }

        public static bool IsNull(this object target)
        {
            return target == null;
        }

        public static bool IsNullOrDbNull(this object target)
        {
            return target == null || target == DBNull.Value;
        }

        public static object ToDbValue(this object target)
        {
            return target == null ? DBNull.Value : target;
        }

        public static object ToDbValue<T>(this T target)
        {
            if (typeof(T) == typeof(string))
                return (target as string).IsNullOrWhiteSpace() ? DBNull.Value : (object)target;
            else
                return EqualityComparer<T>.Default.Equals(target, default(T)) ? DBNull.Value : (object)target;
        }

        public static object ToDbValue<T>(this T target, T defaultValue)
        {
            return EqualityComparer<T>.Default.Equals(target, defaultValue) ? DBNull.Value : (object)target;
        }

        public static T ChangeType<T>(this object target)
        {
            return target == null || target == DBNull.Value ? default(T) : (T)Convert.ChangeType(target, typeof(T));
        }

        public static T CastTo<T>(this object target)
        {
            return target == null || target == DBNull.Value ? default(T) : (T)Convert.ChangeType(target, typeof(T));
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

        public static T MemberwiseClone<T>(this T target)
            where T: new()
        {
            T obj = Activator.CreateInstance<T>();

            if (target != null)
            {
                foreach (PropertyInfo info in ChoType.GetProperties(typeof(T)))
                {
                    if (ChoType.GetAttribute<ChoIgnorePropertyAttribute>(info, false) == null)
                        ChoType.SetPropertyValue(obj, info, ChoType.GetPropertyValue(target, info));
                }
            }

            return obj;
        }

        public static object MemberwiseClone<T>(this object target)
            where T : new()
        {
            if (target == null)
            {
                object obj = Activator.CreateInstance(target.GetType());

                foreach (PropertyInfo info in ChoType.GetProperties(target.GetType()))
                {
                    if (ChoType.GetAttribute<ChoIgnorePropertyAttribute>(info, false) == null)
                        ChoType.SetPropertyValue(obj, info, ChoType.GetPropertyValue(target, info));
                }

                return obj;
            }
            else
                return null;
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

        public static string ToNullNSXml(this object obj, XmlWriterSettings xws = null, XmlAttributeOverrides xmlAttributeOverrides = null)
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

        public static string ToNullNSXmlWithType(this object obj, ChoTypeNameSpecifier? typeNameSpecifier = null, XmlWriterSettings xws = null, XmlAttributeOverrides xmlAttributeOverrides = null)
        {
            if (obj == null)
                return null;

            StringBuilder xmlString = new StringBuilder();
            using (XmlWriter xtw = XmlTextWriter.Create(xmlString, xws ?? _xws))
            {
                ChoNullNSXmlSerializer serializer = xmlAttributeOverrides != null ? new ChoNullNSXmlSerializer(obj.GetType(), xmlAttributeOverrides) : new ChoNullNSXmlSerializer(obj.GetType());
                serializer.Serialize(xtw, obj);

                xtw.Flush();
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString.ToString());
            xmlDoc.DocumentElement.Attributes.Append(xmlDoc.CreateAttribute("_type_")).Value = typeNameSpecifier != null ? obj.GetType().GetName(typeNameSpecifier.Value) : obj.GetTypeName();
            return xmlDoc.InnerXml;
        }

        public static string ToXml(this object obj, XmlWriterSettings xws = null)
        {
            if (obj == null)
                return null;

            StringBuilder xmlString = new StringBuilder();
            using (XmlWriter xtw = XmlTextWriter.Create(xmlString, xws ?? _xws))
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

        #region Load Overloads

        public static void Load(this object obj, string keyValueText, bool throwExIfMemberNotFound = false)
        {
            if (obj == null) return;
            if (keyValueText.IsNullOrWhiteSpace()) return;

            Load(obj, keyValueText.ToKeyValuePairs(), throwExIfMemberNotFound);
        }

        public static void Load(this object obj, IEnumerable<Tuple<string, string>> keyValuePairs, bool throwExIfMemberNotFound = false)
        {
            if (obj == null) return;
            if (keyValuePairs == null) return;

            string value = null;
            foreach (Tuple<string, string> keyValue in keyValuePairs)
            {
                if (keyValue.Item1.IsNullOrWhiteSpace()) continue;

                value = keyValue.Item2;
                if (!value.IsNullOrWhiteSpace())
                {
                    if (value.StartsWith("'") && value.EndsWith("'"))
                        value = value.Substring(1, value.Length - 2);
                }

                if (throwExIfMemberNotFound)
                    ChoObject.SetObjectMemberConvertedValue(obj, keyValue.Item1, value);
                else
                {
                    if (ChoType.HasMember(obj.GetType(), keyValue.Item1))
                        ChoObject.SetObjectMemberConvertedValue(obj, keyValue.Item1, value);
                }
            }
        }

        #endregion Load Overloads

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

        public static object[] Expand(object target)
        {
            if (target == null) return new object[] {};

            if (target is Array)
                return ((Array)target).Cast<object>().ToArray();
            else
                return new object[] { target };
        }

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

        public static string ToString(this object anObject, string aFormat)
        {
            return ChoObjectEx.ToString(anObject, aFormat, null);
        }

        public static string ToString(this object anObject, string aFormat, IFormatProvider formatProvider)
        {
            if (anObject == null) return null;

            StringBuilder sb = new StringBuilder();
            Type type = anObject.GetType();
            Regex reg = new Regex(@"({)([^}]+)(})", RegexOptions.IgnoreCase);
            MatchCollection mc = aFormat.IsNullOrWhiteSpace() ? null : reg.Matches(aFormat);
            int startIndex = 0;
            if (mc != null && mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    Group g = m.Groups[2]; //it's second in the match between { and }
                    int length = g.Index - startIndex - 1;
                    sb.Append(aFormat.Substring(startIndex, length));

                    string toGet = String.Empty;
                    string toFormat = String.Empty;
                    int formatIndex = g.Value.IndexOf(":"); //formatting would be to the right of a :
                    if (formatIndex == -1) //no formatting, no worries
                    {
                        toGet = g.Value;
                    }
                    else //pickup the formatting
                    {
                        toGet = g.Value.Substring(0, formatIndex);
                        toFormat = g.Value.Substring(formatIndex + 1);
                    }

                    //first try properties
                    PropertyInfo retrievedProperty = type.GetProperty(toGet);
                    Type retrievedType = null;
                    object retrievedObject = null;
                    if (retrievedProperty != null)
                    {
                        retrievedType = retrievedProperty.PropertyType;
                        retrievedObject = retrievedProperty.GetValue(anObject, null);
                    }
                    else //try fields
                    {
                        FieldInfo retrievedField = type.GetField(toGet);
                        if (retrievedField != null)
                        {
                            retrievedType = retrievedField.FieldType;
                            retrievedObject = retrievedField.GetValue(anObject);
                        }
                    }

                    if (retrievedType != null) //Cool, we found something
                    {
                        string result = String.Empty;
                        if (toFormat == String.Empty) //no format info
                        {
                            result = retrievedType.InvokeMember("ToString",
                                BindingFlags.Public | BindingFlags.NonPublic |
                                BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase
                                , null, retrievedObject, null) as string;
                        }
                        else //format info
                        {
                            result = retrievedType.InvokeMember("ToString",
                                BindingFlags.Public | BindingFlags.NonPublic |
                                BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase
                                , null, retrievedObject, new object[] { toFormat, formatProvider }) as string;
                        }
                        sb.Append(result);
                    }
                    else //didn't find a property with that name, so be gracious and put it back
                    {
                        sb.Append("{");
                        sb.Append(g.Value);
                        sb.Append("}");
                    }
                    startIndex = g.Index + g.Length + 1;
                }
                if (startIndex < aFormat.Length) //include the rest (end) of the string
                {
                    sb.Append(aFormat.Substring(startIndex));
                }
            }
            else
            {
                if (formatProvider == null)
                {
                    if (!aFormat.IsNullOrWhiteSpace())
                        sb.Append(String.Format("{0:" + aFormat + "}", anObject));
                    else
                        sb.Append(anObject.ToString());
                }
                else
                {
                    if (!aFormat.IsNullOrWhiteSpace())
                        sb.Append(String.Format(formatProvider, "{0:" + aFormat + "}", anObject));
                    else
                        sb.Append(anObject.ToString());
                }
            }
            return sb.ToString();
        }

        public static string GetTypeName(this object @this)
        {
            if (@this == null) return null;

            ChoSerializableAttribute attr = ChoType.GetAttribute<ChoSerializableAttribute>(@this.GetType());
            if (attr == null)
                return @this.GetType().GetName(ChoTypeNameSpecifier.SimpleQualifiedName);
            else
                return @this.GetType().GetName(attr.TypeNameSpecifier);
        }
    }
}
