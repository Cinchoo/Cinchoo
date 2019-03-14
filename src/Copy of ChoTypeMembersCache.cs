namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;
    using System.Collections.Specialized;
    using System.Collections;

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
        private static readonly Dictionary<Type, Dictionary<Type, MemberInfo[]>> _typeMembersDictionaryCache = new Dictionary<Type, Dictionary<Type, MemberInfo[]>>();
        private static readonly Dictionary<Type, Dictionary<Type, FieldInfo[]>> _typeFieldsDictionaryCache = new Dictionary<Type, Dictionary<Type, FieldInfo[]>>();
        private static readonly Dictionary<Type, Dictionary<Type, PropertyInfo[]>> _typeProperiesDictionaryCache = new Dictionary<Type, Dictionary<Type, PropertyInfo[]>>();

        private static readonly Dictionary<Type, Dictionary<Type, MemberInfo[]>> _typeNonReadOnlyMembersDictionaryCache = new Dictionary<Type, Dictionary<Type, MemberInfo[]>>();
        private static readonly Dictionary<Type, Dictionary<Type, FieldInfo[]>> _typeNonReadOnlyFieldsDictionaryCache = new Dictionary<Type, Dictionary<Type, FieldInfo[]>>();
        private static readonly Dictionary<Type, Dictionary<Type, PropertyInfo[]>> _typeNonReadOnlyProperiesDictionaryCache = new Dictionary<Type, Dictionary<Type, PropertyInfo[]>>();

        #endregion Shared Data Members (Private)

        #region Shared Members (Public)

        public static MemberInfo[] GetMembers(Type type, Type attributeType)
        {
            LoadMembers(type, attributeType);

            if (_typeMembersDictionaryCache[type].ContainsKey(attributeType))
                return _typeMembersDictionaryCache[type][attributeType];
            else
                return EmptyMembers;
        }

        public static PropertyInfo[] GetProperties(Type type, Type attributeType)
        {
            LoadMembers(type, attributeType);

            if (_typeProperiesDictionaryCache[type].ContainsKey(attributeType))
                return _typeProperiesDictionaryCache[type][attributeType];
            else
                return EmptyProperties;
        }

        public static FieldInfo[] GetFields(Type type, Type attributeType)
        {
            LoadMembers(type, attributeType);

            if (_typeFieldsDictionaryCache[type].ContainsKey(attributeType))
                return _typeFieldsDictionaryCache[type][attributeType];
            else
                return EmptyFields;
        }

        public static MemberInfo[] GetNonReadOnlyMembers(Type type, Type attributeType)
        {
            LoadMembers(type, attributeType);

            if (_typeNonReadOnlyMembersDictionaryCache[type].ContainsKey(attributeType))
                return _typeNonReadOnlyMembersDictionaryCache[type][attributeType];
            else
                return EmptyMembers;
        }

        public static PropertyInfo[] GetNonReadOnlyProperties(Type type, Type attributeType)
        {
            LoadMembers(type, attributeType);

            if (_typeNonReadOnlyProperiesDictionaryCache[type].ContainsKey(attributeType))
                return _typeNonReadOnlyProperiesDictionaryCache[type][attributeType];
            else
                return EmptyProperties;
        }

        public static FieldInfo[] GetNonReadOnlyFields(Type type, Type attributeType)
        {
            LoadMembers(type, attributeType);

            if (_typeNonReadOnlyFieldsDictionaryCache[type].ContainsKey(attributeType))
                return _typeNonReadOnlyFieldsDictionaryCache[type][attributeType];
            else
                return EmptyFields;
        }

        #endregion Shared Members (Public)

        #region Shared Members (Private)

        public static void LoadMembers(Type type, Type attributeType)
        {
            if (type == null)
                throw new NullReferenceException("type");
            if (attributeType == null)
                throw new NullReferenceException("attributeType");

            if (_typeMembersDictionaryCache.ContainsKey(type)
                && _typeMembersDictionaryCache[type] != null
                && _typeMembersDictionaryCache[type].ContainsKey(attributeType))
                return;

            lock (_typeMembersDictionaryCacheLockObject)
            {
                if (_typeMembersDictionaryCache.ContainsKey(type)
                    && _typeMembersDictionaryCache[type] != null
                    && _typeMembersDictionaryCache[type].ContainsKey(attributeType))
                    return;

                InitDicts(type, attributeType);

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

                    object memberAttribute = memberInfo.GetCustomAttribute(attributeType);
                    if (memberAttribute == null)
                        continue;

                    myMemberInfos.Add(memberInfo.Name, memberInfo);

                    if (memberInfo is FieldInfo)
                        myFieldInfos.Add(memberInfo.Name, memberInfo);
                    else
                        myPropertyInfos.Add(memberInfo.Name, memberInfo);

                    if (!ChoType.IsReadOnlyMember(memberInfo))
                    {
                        myNonReadOnlyMemberInfos.Add(memberInfo.Name, memberInfo);

                        if (memberInfo is FieldInfo)
                            myNonReadOnlyFieldInfos.Add(memberInfo.Name, memberInfo);
                        else
                            myNonReadOnlyPropertyInfos.Add(memberInfo.Name, memberInfo);
                    }
                }

                _typeMembersDictionaryCache[type][attributeType] = new ArrayList(myMemberInfos.Values).ToArray(typeof(MemberInfo)) as MemberInfo[];
                _typeFieldsDictionaryCache[type][attributeType] = new ArrayList(myFieldInfos.Values).ToArray(typeof(FieldInfo)) as FieldInfo[];
                _typeProperiesDictionaryCache[type][attributeType] = new ArrayList(myPropertyInfos.Values).ToArray(typeof(PropertyInfo)) as PropertyInfo[];

                _typeNonReadOnlyMembersDictionaryCache[type][attributeType] = new ArrayList(myNonReadOnlyMemberInfos.Values).ToArray(typeof(MemberInfo)) as MemberInfo[];
                _typeNonReadOnlyFieldsDictionaryCache[type][attributeType] = new ArrayList(myNonReadOnlyFieldInfos.Values).ToArray(typeof(FieldInfo)) as FieldInfo[];
                _typeNonReadOnlyProperiesDictionaryCache[type][attributeType] = new ArrayList(myNonReadOnlyPropertyInfos.Values).ToArray(typeof(PropertyInfo)) as PropertyInfo[];
            }
        }

        private static void InitDicts(Type type, Type attributeType)
        {
            if (!_typeMembersDictionaryCache.ContainsKey(type) || _typeMembersDictionaryCache[type] == null)
            {
                _typeMembersDictionaryCache[type] = new Dictionary<Type, MemberInfo[]>();
                _typeMembersDictionaryCache[type].Add(attributeType, null);
            }
            if (!_typeFieldsDictionaryCache.ContainsKey(type) || _typeFieldsDictionaryCache[type] == null)
            {
                _typeFieldsDictionaryCache[type] = new Dictionary<Type, FieldInfo[]>();
                _typeFieldsDictionaryCache[type].Add(attributeType, null);
            }
            if (!_typeProperiesDictionaryCache.ContainsKey(type) || _typeProperiesDictionaryCache[type] == null)
            {
                _typeProperiesDictionaryCache[type] = new Dictionary<Type, PropertyInfo[]>();
                _typeProperiesDictionaryCache[type].Add(attributeType, null);
            }

            if (!_typeNonReadOnlyMembersDictionaryCache.ContainsKey(type) || _typeNonReadOnlyMembersDictionaryCache[type] == null)
            {
                _typeNonReadOnlyMembersDictionaryCache[type] = new Dictionary<Type, MemberInfo[]>();
                _typeNonReadOnlyMembersDictionaryCache[type].Add(attributeType, null);
            }
            if (!_typeNonReadOnlyFieldsDictionaryCache.ContainsKey(type) || _typeNonReadOnlyFieldsDictionaryCache[type] == null)
            {
                _typeNonReadOnlyFieldsDictionaryCache[type] = new Dictionary<Type, FieldInfo[]>();
                _typeNonReadOnlyFieldsDictionaryCache[type].Add(attributeType, null);
            }
            if (!_typeNonReadOnlyProperiesDictionaryCache.ContainsKey(type) || _typeNonReadOnlyProperiesDictionaryCache[type] == null)
            {
                _typeNonReadOnlyProperiesDictionaryCache[type] = new Dictionary<Type, PropertyInfo[]>();
                _typeNonReadOnlyProperiesDictionaryCache[type].Add(attributeType, null);
            }
        }

        #endregion Shared Members (Private)
    }
}
