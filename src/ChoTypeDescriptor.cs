namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using Cinchoo.Core.Collections;

    #endregion NameSpaces

    public static class ChoTypeDescriptor
    {
        #region Constants

        private static readonly TypeConverter[] EmptyTypeConverters = new TypeConverter[] { };
        private static readonly object[] EmptyParams = new object[] { };

        #endregion Constants

        #region Shared Data Members (Private)

        private static readonly object _typeMemberTypeConverterCacheLockObject = new object();
        private static readonly Dictionary<MemberInfo, object[]> _typeMemberTypeConverterCache = new Dictionary<MemberInfo, object[]>();
        private static readonly Dictionary<Type, object[]> _typeTypeConverterCache = new Dictionary<Type, object[]>();
        
        private static readonly Dictionary<MemberInfo, object[]> _typeMemberTypeConverterParamsCache = new Dictionary<MemberInfo, object[]>();
        private static readonly Dictionary<Type, object[]> _typeTypeConverterParamsCache = new Dictionary<Type, object[]>();

        #endregion Shared Data Members (Private)

        #region Shared Members (Public)

        #region GetTypeConverters Overloads (Public)

        public static void RegisterConverters(MemberInfo memberInfo, TypeConverter[] typeConverters)
        {
            ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

            lock (_typeMemberTypeConverterCacheLockObject)
            {
                if (typeConverters == null)
                    typeConverters = EmptyTypeConverters;

                if (_typeMemberTypeConverterCache.ContainsKey(memberInfo))
                    _typeMemberTypeConverterCache[memberInfo] = typeConverters;
                else
                    _typeMemberTypeConverterCache.Add(memberInfo, typeConverters);
            }
        }

        public static void RegisterConverters(Type type, TypeConverter[] typeConverters)
        {
            ChoGuard.ArgumentNotNull(type, "Type");

            lock (_typeMemberTypeConverterCacheLockObject)
            {
                if (typeConverters == null)
                    typeConverters = EmptyTypeConverters;

                if (_typeTypeConverterCache.ContainsKey(type))
                    _typeTypeConverterCache[type] = typeConverters;
                else
                    _typeTypeConverterCache.Add(type, typeConverters);
            }
        }

        public static void ClearConverters(MemberInfo memberInfo)
        {
            ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

            lock (_typeMemberTypeConverterCacheLockObject)
            {
                if (_typeMemberTypeConverterCache.ContainsKey(memberInfo))
                    _typeMemberTypeConverterCache.Remove(memberInfo);
            }
        }

        public static void ClearConverters(Type type)
        {
            ChoGuard.ArgumentNotNull(type, "Type");

            lock (_typeMemberTypeConverterCacheLockObject)
            {
                if (_typeTypeConverterCache.ContainsKey(type))
                    _typeTypeConverterCache.Remove(type);
            }
        }

        #endregion GetTypeConverters Overloads (Public)

        #region GetTypeConverters Overloads (internal)

        public static object[] GetTypeConverterParams(MemberInfo memberInfo)
        {
            if (memberInfo == null)
                return EmptyParams;

            Type memberType;
            if (ChoType.TryGetMemberType(memberInfo, out memberType) && (memberType == null /*|| memberType.IsSimple() */))
                return EmptyParams;
            
            if (_typeMemberTypeConverterCache.ContainsKey(memberInfo))
            {
                if (_typeMemberTypeConverterCache[memberInfo] == EmptyTypeConverters)
                {
                    if (_typeTypeConverterParamsCache.ContainsKey(memberType))
                        return _typeTypeConverterParamsCache[memberType];
                }

                return _typeMemberTypeConverterParamsCache[memberInfo];
            }

            return EmptyParams;
        }

        public static object GetTypeConverter(MemberInfo memberInfo)
        {
            if (memberInfo == null)
                return null;

            object[] typeConverters = GetTypeConverters(memberInfo);
            if (typeConverters == null || typeConverters.Length == 0)
                return null;

            return typeConverters[0];
        }


        public static object[] GetTypeConverters(MemberInfo memberInfo)
        {
            if (memberInfo == null)
                return null;

            Type memberType;
            if (ChoType.TryGetMemberType(memberInfo, out memberType) && (memberType == null /*|| memberType.IsSimple() */))
                return null;

            if (_typeMemberTypeConverterCache.ContainsKey(memberInfo))
            {
                if (_typeMemberTypeConverterCache[memberInfo] == EmptyTypeConverters)
                {
                    if (_typeTypeConverterCache.ContainsKey(memberType))
                        return _typeTypeConverterCache[memberType];
                }
                   
                return _typeMemberTypeConverterCache[memberInfo];
            }
            else
            {
                lock (_typeMemberTypeConverterCacheLockObject)
                {
                    if (!_typeMemberTypeConverterCache.ContainsKey(memberInfo))
                    {
                        Type typeConverterAttribute = typeof(ChoTypeConverterAttribute);

                        _typeMemberTypeConverterCache[memberInfo] = EmptyTypeConverters;
                        _typeMemberTypeConverterParamsCache[memberInfo] = EmptyParams;

                        ChoPriorityQueue queue = new ChoPriorityQueue();
                        ChoPriorityQueue paramsQueue = new ChoPriorityQueue();
                        foreach (Attribute attribute in ChoType.GetAttributes(memberInfo, typeof(ChoTypeConverterAttribute), false))
                        {
                            ChoTypeConverterAttribute converterAttribute = (ChoTypeConverterAttribute)attribute;
                            if (converterAttribute != null)
                            {
                                queue.Enqueue(converterAttribute.Priority, converterAttribute.CreateInstance());
                                paramsQueue.Enqueue(converterAttribute.Priority, converterAttribute.Parameters);
                            }

                            if (queue.Count > 0)
                            {
                                _typeMemberTypeConverterCache[memberInfo] = queue.ToArray();
                                _typeMemberTypeConverterParamsCache[memberInfo] = paramsQueue.ToArray();

                                return _typeMemberTypeConverterCache[memberInfo];
                            }
                        }

                        if (queue.Count == 0 && !memberType.IsSimple())
                        {
                            if (!_typeTypeConverterCache.ContainsKey(memberType))
                            {
                                foreach (Type type in ChoType.GetTypes<ChoTypeConverterAttribute>())
                                {
                                    ChoTypeConverterAttribute converterAttribute = type.GetCustomAttribute<ChoTypeConverterAttribute>();
                                    if (converterAttribute != null && converterAttribute.ConverterType == memberType)
                                    {
                                        _typeTypeConverterCache.Add(memberType, new object[] { ChoType.CreateInstance(type) });
                                        _typeTypeConverterParamsCache.Add(memberType, new object[] { converterAttribute.Parameters });

                                        return _typeTypeConverterCache[memberType];
                                    }
                                }

                                TypeConverter converter = TypeDescriptor.GetConverter(memberType);
                                if (converter != null)
                                    _typeTypeConverterCache.Add(memberType, new object[] { converter });
                                else
                                    _typeTypeConverterCache.Add(memberType, EmptyTypeConverters);

                                _typeTypeConverterParamsCache.Add(memberType, EmptyParams);
                            }

                            return _typeTypeConverterCache[memberType];
                        }
                    }

                    return _typeMemberTypeConverterCache.ContainsKey(memberInfo) ? _typeMemberTypeConverterCache[memberInfo] : EmptyTypeConverters;
                }
            }
        }

        #endregion GetTypeConverter Overloads (internal)

        #endregion Shared Members (Public)

        #region Shared Members (Private)

        public static void LoadMembers(MemberInfo memberInfo)
        {
            if (memberInfo == null)
                throw new NullReferenceException("memberInfo");

            //if (_typeMembersDictionaryCache.ContainsKey(type))
            //    return;

            //lock (_typeMembersDictionaryCacheLockObject)
            //{
            //    if (_typeMembersDictionaryCache.ContainsKey(type))
            //        return;

            //    InitDicts(type);

            //    OrderedDictionary myMemberInfos = new OrderedDictionary();
            //    OrderedDictionary myFieldInfos = new OrderedDictionary();
            //    OrderedDictionary myPropertyInfos = new OrderedDictionary();

            //    OrderedDictionary myNonReadOnlyMemberInfos = new OrderedDictionary();
            //    OrderedDictionary myNonReadOnlyFieldInfos = new OrderedDictionary();
            //    OrderedDictionary myNonReadOnlyPropertyInfos = new OrderedDictionary();

            //    foreach (MemberInfo memberInfo in type.GetMembers(BindingFlags.Instance | BindingFlags.Public))
            //    {
            //        if (!(memberInfo is PropertyInfo)
            //            && !(memberInfo is FieldInfo)
            //            )
            //            continue;

            //        if (memberInfo.GetCustomAttribute<ChoIgnorePropertyAttribute>() != null)
            //            continue;

            //        myMemberInfos.Add(memberInfo.Name, memberInfo);

            //        if (memberInfo is FieldInfo)
            //            myFieldInfos.Add(memberInfo.Name, memberInfo);
            //        else
            //            myPropertyInfos.Add(memberInfo.Name, memberInfo);

            //        if (!ChoType.IsReadOnlyMember(memberInfo))
            //        {
            //            myNonReadOnlyMemberInfos.Add(memberInfo.Name, memberInfo);

            //            if (memberInfo is FieldInfo)
            //                myNonReadOnlyFieldInfos.Add(memberInfo.Name, memberInfo);
            //            else
            //                myNonReadOnlyPropertyInfos.Add(memberInfo.Name, memberInfo);
            //        }
            //    }

            //    _typeMembersDictionaryCache[type] = new ArrayList(myMemberInfos.Values).ToArray(typeof(MemberInfo)) as MemberInfo[];
            //    _typeFieldsDictionaryCache[type] = new ArrayList(myFieldInfos.Values).ToArray(typeof(FieldInfo)) as FieldInfo[];
            //    _typeProperiesDictionaryCache[type] = new ArrayList(myPropertyInfos.Values).ToArray(typeof(PropertyInfo)) as PropertyInfo[];

            //    _typeNonReadOnlyMembersDictionaryCache[type] = new ArrayList(myNonReadOnlyMemberInfos.Values).ToArray(typeof(MemberInfo)) as MemberInfo[];
            //    _typeNonReadOnlyFieldsDictionaryCache[type] = new ArrayList(myNonReadOnlyFieldInfos.Values).ToArray(typeof(FieldInfo)) as FieldInfo[];
            //    _typeNonReadOnlyProperiesDictionaryCache[type] = new ArrayList(myNonReadOnlyPropertyInfos.Values).ToArray(typeof(PropertyInfo)) as PropertyInfo[];
            //}
        }

        #endregion Shared Members (Private)
    }
}
