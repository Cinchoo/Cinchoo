namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;
    using System.Collections.Specialized;
    using System.Collections;
    using Cinchoo.Core.Configuration;

    #endregion NameSpaces

    public static class ChoTypeMembersCache
    {
        #region Constants

        private static readonly MemberInfo[] EmptyMembers = new MemberInfo[] { };
        private static readonly FieldInfo[] EmptyFields = new FieldInfo[] { };
        private static readonly PropertyInfo[] EmptyProperties = new PropertyInfo[] { };

        #endregion Constants

        #region Shared Data Members (Private)

        private static readonly object _typeMembersDictionaryCacheLockObject = new object();

        private static readonly Dictionary<Type, Dictionary<string, MemberInfo>> _typeMembersDictionaryDictionaryCache = new Dictionary<Type, Dictionary<string, MemberInfo>>();
        private static readonly Dictionary<Type, Dictionary<string, FieldInfo>> _typeFieldsDictionaryDictionaryCache = new Dictionary<Type, Dictionary<string, FieldInfo>>();
        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _typePropertiesDictionaryDictionaryCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        private static readonly Dictionary<Type, MemberInfo[]> _typeMembersDictionaryCache = new Dictionary<Type, MemberInfo[]>();
        private static readonly Dictionary<Type, FieldInfo[]> _typeFieldsDictionaryCache = new Dictionary<Type, FieldInfo[]>();
        private static readonly Dictionary<Type, PropertyInfo[]> _typeProperiesDictionaryCache = new Dictionary<Type, PropertyInfo[]>();

        private static readonly Dictionary<Type, MemberInfo[]> _typeNonReadOnlyMembersDictionaryCache = new Dictionary<Type, MemberInfo[]>();
        private static readonly Dictionary<Type, FieldInfo[]> _typeNonReadOnlyFieldsDictionaryCache = new Dictionary<Type, FieldInfo[]>();
        private static readonly Dictionary<Type, PropertyInfo[]> _typeNonReadOnlyProperiesDictionaryCache = new Dictionary<Type, PropertyInfo[]>();

        #endregion Shared Data Members (Private)

        #region Shared Members (Public)

        public static MemberInfo GetMemberInfo(Type type, string memberName)
        {
            ChoGuard.ArgumentNotNull(type, "Type");
            ChoGuard.ArgumentNotNull(memberName, "MemberName");

            LoadMemberInfos(type);

            if (_typeMembersDictionaryDictionaryCache.ContainsKey(type) && _typeMembersDictionaryDictionaryCache[type].ContainsKey(memberName))
                return _typeMembersDictionaryDictionaryCache[type][memberName];
            else
                return null;
        }

        public static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            ChoGuard.ArgumentNotNull(type, "Type");
            ChoGuard.ArgumentNotNull(fieldName, "FieldName");

            LoadMemberInfos(type);

            if (_typeFieldsDictionaryDictionaryCache.ContainsKey(type) && _typeFieldsDictionaryDictionaryCache[type].ContainsKey(fieldName))
                return _typeFieldsDictionaryDictionaryCache[type][fieldName];
            else
                return null;
        }

        public static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            ChoGuard.ArgumentNotNull(type, "Type");
            ChoGuard.ArgumentNotNull(propertyName, "PropertyName");

            LoadMemberInfos(type);

            if (_typePropertiesDictionaryDictionaryCache.ContainsKey(type) && _typePropertiesDictionaryDictionaryCache[type].ContainsKey(propertyName))
                return _typePropertiesDictionaryDictionaryCache[type][propertyName];
            else
                return null;
        }

        public static MemberInfo[] GetAllMemberInfos(Type type)
        {
            ChoGuard.ArgumentNotNull(type, "Type");

            LoadMemberInfos(type);

            if (_typeMembersDictionaryCache.ContainsKey(type))
                return _typeMembersDictionaryCache[type];
            else
                return EmptyMembers;
        }

        public static FieldInfo[] GetAllFieldInfos(Type type, Type attributeType)
        {
            ChoGuard.ArgumentNotNull(type, "Type");

            LoadMemberInfos(type);

            if (_typeFieldsDictionaryCache.ContainsKey(type))
                return _typeFieldsDictionaryCache[type];
            else
                return EmptyFields;
        }

        public static PropertyInfo[] GetAllPropertyInfos(Type type, Type attributeType)
        {
            ChoGuard.ArgumentNotNull(type, "Type");

            LoadMemberInfos(type);

            if (_typeProperiesDictionaryCache.ContainsKey(type))
                return _typeProperiesDictionaryCache[type];
            else
                return EmptyProperties;
        }

        public static MemberInfo[] GetNonReadOnlyMemberInfos(Type type)
        {
            ChoGuard.ArgumentNotNull(type, "Type");

            LoadMemberInfos(type);

            if (_typeNonReadOnlyMembersDictionaryCache.ContainsKey(type))
                return _typeNonReadOnlyMembersDictionaryCache[type];
            else
                return EmptyMembers;
        }

        public static FieldInfo[] GetNonReadOnlyFieldInfos(Type type)
        {
            ChoGuard.ArgumentNotNull(type, "Type");

            LoadMemberInfos(type);

            if (_typeNonReadOnlyFieldsDictionaryCache.ContainsKey(type))
                return _typeNonReadOnlyFieldsDictionaryCache[type];
            else
                return EmptyFields;
        }

        public static PropertyInfo[] GetNonReadOnlyPropertyInfos(Type type)
        {
            ChoGuard.ArgumentNotNull(type, "Type");

            LoadMemberInfos(type);

            if (_typeNonReadOnlyProperiesDictionaryCache.ContainsKey(type))
                return _typeNonReadOnlyProperiesDictionaryCache[type];
            else
                return EmptyProperties;
        }

        #endregion Shared Members (Public)

        #region Shared Members (Private)

        private static void LoadMemberInfos(Type type)
        {
            if (type == null)
                throw new NullReferenceException("type");

            if (_typeMembersDictionaryCache.ContainsKey(type))
                return;

            lock (_typeMembersDictionaryCacheLockObject)
            {
                if (_typeMembersDictionaryCache.ContainsKey(type))
                    return;

                InitDicts(type);

                OrderedDictionary myMemberInfos = new OrderedDictionary();
                OrderedDictionary myFieldInfos = new OrderedDictionary();
                OrderedDictionary myPropertyInfos = new OrderedDictionary();

                OrderedDictionary myNonReadOnlyMemberInfos = new OrderedDictionary();
                OrderedDictionary myNonReadOnlyFieldInfos = new OrderedDictionary();
                OrderedDictionary myNonReadOnlyPropertyInfos = new OrderedDictionary();

                foreach (MemberInfo memberInfo in type.GetMembers(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (!(memberInfo is PropertyInfo)
                        && !(memberInfo is FieldInfo)
                        )
                        continue;

                    //if (memberInfo.GetCustomAttribute<ChoIgnorePropertyAttribute>() != null)
                    //    continue;

                    if (!ChoType.IsReadOnlyMember(memberInfo) && ChoType.IsReadableMember(memberInfo))
                    {
                        myMemberInfos.Add(memberInfo.Name, memberInfo);

                        if (memberInfo is FieldInfo)
                            myFieldInfos.Add(memberInfo.Name, memberInfo);
                        else
                            myPropertyInfos.Add(memberInfo.Name, memberInfo);
                    }

                    if (!ChoType.IsReadOnlyMember(memberInfo))
                    {
                        myNonReadOnlyMemberInfos.Add(memberInfo.Name, memberInfo);

                        if (memberInfo is FieldInfo)
                            myNonReadOnlyFieldInfos.Add(memberInfo.Name, memberInfo);
                        else
                            myNonReadOnlyPropertyInfos.Add(memberInfo.Name, memberInfo);
                    }
                }

                _typeMembersDictionaryDictionaryCache[type] = (new ArrayList(myMemberInfos.Values).ToArray(typeof(MemberInfo)) as MemberInfo[]).ToDictionary(member => member.Name);
                _typeFieldsDictionaryDictionaryCache[type] = (new ArrayList(myFieldInfos.Values).ToArray(typeof(FieldInfo)) as FieldInfo[]).ToDictionary(field => field.Name);
                _typePropertiesDictionaryDictionaryCache[type] = (new ArrayList(myPropertyInfos.Values).ToArray(typeof(PropertyInfo)) as PropertyInfo[]).ToDictionary(property => property.Name);

                _typeMembersDictionaryCache[type] = new ArrayList(myMemberInfos.Values).ToArray(typeof(MemberInfo)) as MemberInfo[];
                _typeFieldsDictionaryCache[type] = new ArrayList(myFieldInfos.Values).ToArray(typeof(FieldInfo)) as FieldInfo[];
                _typeProperiesDictionaryCache[type] = new ArrayList(myPropertyInfos.Values).ToArray(typeof(PropertyInfo)) as PropertyInfo[];

                _typeNonReadOnlyMembersDictionaryCache[type] = new ArrayList(myNonReadOnlyMemberInfos.Values).ToArray(typeof(MemberInfo)) as MemberInfo[];
                _typeNonReadOnlyFieldsDictionaryCache[type] = new ArrayList(myNonReadOnlyFieldInfos.Values).ToArray(typeof(FieldInfo)) as FieldInfo[];
                _typeNonReadOnlyProperiesDictionaryCache[type] = new ArrayList(myNonReadOnlyPropertyInfos.Values).ToArray(typeof(PropertyInfo)) as PropertyInfo[];
            }
        }

        private static void InitDicts(Type type)
        {
            if (!_typeMembersDictionaryDictionaryCache.ContainsKey(type))
                _typeMembersDictionaryDictionaryCache.Add(type, null);
            if (!_typeFieldsDictionaryDictionaryCache.ContainsKey(type))
                _typeFieldsDictionaryDictionaryCache.Add(type, null);
            if (!_typePropertiesDictionaryDictionaryCache.ContainsKey(type))
                _typePropertiesDictionaryDictionaryCache.Add(type, null);

            if (!_typeMembersDictionaryCache.ContainsKey(type))
                _typeMembersDictionaryCache.Add(type, EmptyMembers);
            if (!_typeFieldsDictionaryCache.ContainsKey(type))
                _typeFieldsDictionaryCache.Add(type, EmptyFields);
            if (!_typeProperiesDictionaryCache.ContainsKey(type))
                _typeProperiesDictionaryCache.Add(type, EmptyProperties);

            if (!_typeNonReadOnlyMembersDictionaryCache.ContainsKey(type))
                _typeNonReadOnlyMembersDictionaryCache.Add(type, EmptyMembers);
            if (!_typeNonReadOnlyFieldsDictionaryCache.ContainsKey(type))
                _typeNonReadOnlyFieldsDictionaryCache.Add(type, EmptyFields);
            if (!_typeNonReadOnlyProperiesDictionaryCache.ContainsKey(type))
                _typeNonReadOnlyProperiesDictionaryCache.Add(type, EmptyProperties);
        }

        #endregion Shared Members (Private)
    }
}
