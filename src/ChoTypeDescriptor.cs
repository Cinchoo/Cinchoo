namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using Cinchoo.Core.Collections;

    #endregion NameSpaces

    public class ChoTypeMemberConverterEventArgs : EventArgs
    {
        public MemberInfo MemberInfo;
        public List<object> TypeConverters
        {
            get;
            internal set;
        }
        public List<object> TypeConverterParams
        {
            get;
            internal set;
        }
        public bool IsParamsRequested;
        public bool Resolved;
    }

    public class ChoTypeConverterEventArgs : EventArgs
    {
        public Type Type;
        public List<object> TypeConverters
        {
            get;
            internal set;
        }
        public List<object> TypeConverterParams
        {
            get;
            internal set;
        }
        public bool IsParamsRequested;
        public bool Resolved;
    }

    public static class ChoTypeDescriptor
    {
        private static readonly Dictionary<MemberInfo, object[]> _typeMemberTypeConverterCache = new Dictionary<MemberInfo, object[]>();
        private static readonly object _typeMemberTypeConverterCacheLockObject = new object();
        private static readonly Dictionary<MemberInfo, object[]> _typeMemberTypeConverterParamsCache = new Dictionary<MemberInfo, object[]>();
        private static readonly Dictionary<Type, object[]> _typeTypeConverterCache = new Dictionary<Type, object[]>();
        private static readonly Dictionary<Type, object[]> _typeTypeConverterParamsCache = new Dictionary<Type, object[]>();
        private static readonly object[] EmptyParams = new object[0];
        private static readonly TypeConverter[] EmptyTypeConverters = new TypeConverter[0];

        public static event EventHandler<ChoTypeMemberConverterEventArgs> TypeMemberConverterResolve;
        public static event EventHandler<ChoTypeConverterEventArgs> TypeConverterResolve;

        public static void ClearConverters(Type type)
        {
            ChoGuard.ArgumentNotNull(type, "Type");
            lock (_typeMemberTypeConverterCacheLockObject)
            {
                if (_typeTypeConverterCache.ContainsKey(type))
                {
                    _typeTypeConverterCache.Remove(type);
                }
            }
        }

        public static object[] GetTypeConverters(Type type)
        {
            if (type == null)
                return EmptyTypeConverters;

            object[] typeConverters = GetTypeConvertersInternal(type);

            TryResolveTypeConverters(type, ref typeConverters);
            return typeConverters;
        }

        private static object[] GetTypeConvertersInternal(Type type)
        {
            if (type != null)
            {
                if (_typeTypeConverterCache.ContainsKey(type))
                    return _typeTypeConverterCache[type];

                lock (_typeMemberTypeConverterCacheLockObject)
                {
                    if (!_typeTypeConverterCache.ContainsKey(type))
                        Register(type);

                    return _typeTypeConverterCache[type];
                }
            }
            return EmptyTypeConverters;
        }

        public static object[] GetTypeConverterParams(Type type)
        {
            if (type == null)
                return EmptyParams;

            object[] typeConverterParams = _typeTypeConverterParamsCache.ContainsKey(type) ? _typeTypeConverterParamsCache[type] : EmptyParams;

            if (!TryResolveTypeConverterParams(type, ref typeConverterParams))
            {
                if (!_typeTypeConverterParamsCache.ContainsKey(type))
                    return EmptyParams;
                return _typeTypeConverterParamsCache[type];
            }
            return typeConverterParams;
        }

        public static void RegisterConverters(Type type, TypeConverter[] typeConverters)
        {
            ChoGuard.ArgumentNotNull(type, "Type");
            lock (_typeMemberTypeConverterCacheLockObject)
            {
                if (typeConverters == null)
                {
                    typeConverters = EmptyTypeConverters;
                }
                if (_typeTypeConverterCache.ContainsKey(type))
                {
                    _typeTypeConverterCache[type] = typeConverters;
                }
                else
                {
                    _typeTypeConverterCache.Add(type, typeConverters);
                }
            }
        }

        public static void ClearConverters(MemberInfo memberInfo)
        {
            ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");
            lock (_typeMemberTypeConverterCacheLockObject)
            {
                if (_typeMemberTypeConverterCache.ContainsKey(memberInfo))
                {
                    _typeMemberTypeConverterCache.Remove(memberInfo);
                }
            }
        }

        public static object[] GetTypeConverterParams(MemberInfo memberInfo)
        {
            if (memberInfo == null)
                return EmptyParams;

            object[] typeConverterParams = _typeMemberTypeConverterCache.ContainsKey(memberInfo) ? _typeMemberTypeConverterCache[memberInfo] : EmptyParams;

            if (!TryResolveTypeMemberConverterParams(memberInfo, ref typeConverterParams))
            {
                Type type;
                if (ChoType.TryGetMemberType(memberInfo, out type) && (type == null))
                    return EmptyParams;
                if (!_typeMemberTypeConverterCache.ContainsKey(memberInfo))
                    return EmptyParams;
                if ((_typeMemberTypeConverterCache[memberInfo] == EmptyTypeConverters) && _typeTypeConverterParamsCache.ContainsKey(type))
                    return _typeTypeConverterParamsCache[type];
                return _typeMemberTypeConverterParamsCache[memberInfo];
            }
            return typeConverterParams;
        }

        public static object GetTypeConverter(MemberInfo memberInfo)
        {
            if (memberInfo != null)
            {
                object[] typeConverters = GetTypeConverters(memberInfo);
                if ((typeConverters != null) && (typeConverters.Length != 0))
                {
                    return typeConverters[0];
                }
            }
            return null;
        }

        public static object[] GetTypeConverters(MemberInfo memberInfo)
        {
            if (memberInfo == null)
                return EmptyTypeConverters;

            object[] typeConverters = GetTypeConvertersInternal(memberInfo);

            TryResolveTypeMemberConverters(memberInfo, ref typeConverters);
            return typeConverters;
        }

        private static object[] GetTypeConvertersInternal(MemberInfo memberInfo)
        {
            if (memberInfo != null)
            {
                Type type;
                if (ChoType.TryGetMemberType(memberInfo, out type) && (type == null))
                    return EmptyTypeConverters;

                if (_typeMemberTypeConverterCache.ContainsKey(memberInfo))
                {
                    if ((_typeMemberTypeConverterCache[memberInfo] == EmptyTypeConverters) && _typeTypeConverterCache.ContainsKey(type))
                    {
                        return _typeTypeConverterCache[type];
                    }
                    return _typeMemberTypeConverterCache[memberInfo];
                }
                lock (_typeMemberTypeConverterCacheLockObject)
                {
                    if (!_typeMemberTypeConverterCache.ContainsKey(memberInfo))
                    {
                        _typeMemberTypeConverterCache[memberInfo] = EmptyTypeConverters;
                        _typeMemberTypeConverterParamsCache[memberInfo] = EmptyParams;
                        ChoPriorityQueue queue = new ChoPriorityQueue();
                        ChoPriorityQueue queue2 = new ChoPriorityQueue();
                        foreach (Attribute attribute in ChoType.GetMemberAttributesByBaseType(memberInfo, typeof(ChoTypeConverterAttribute)))
                        {
                            ChoTypeConverterAttribute attribute2 = (ChoTypeConverterAttribute)attribute;
                            if (attribute2 != null)
                            {
                                queue.Enqueue(attribute2.Priority, attribute2.CreateInstance());
                                queue2.Enqueue(attribute2.Priority, attribute2.Parameters);
                            }
                            if (queue.Count > 0)
                            {
                                _typeMemberTypeConverterCache[memberInfo] = queue.ToArray();
                                _typeMemberTypeConverterParamsCache[memberInfo] = queue2.ToArray();
                                return _typeMemberTypeConverterCache[memberInfo];
                            }
                        }
                        if ((queue.Count == 0) && !type.IsSimple())
                        {
                            if (!_typeTypeConverterCache.ContainsKey(type))
                            {
                                Register(type);
                            }
                            return _typeTypeConverterCache[type];
                        }
                    }
                    return (_typeMemberTypeConverterCache.ContainsKey(memberInfo) ? _typeMemberTypeConverterCache[memberInfo] : ((object[])EmptyTypeConverters));
                }
            }
            return EmptyTypeConverters;
        }

        public static void RegisterConverters(MemberInfo memberInfo, TypeConverter[] typeConverters)
        {
            ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");
            lock (_typeMemberTypeConverterCacheLockObject)
            {
                if (typeConverters == null)
                {
                    typeConverters = EmptyTypeConverters;
                }
                if (_typeMemberTypeConverterCache.ContainsKey(memberInfo))
                {
                    _typeMemberTypeConverterCache[memberInfo] = typeConverters;
                }
                else
                {
                    _typeMemberTypeConverterCache.Add(memberInfo, typeConverters);
                }
            }
        }

        private static bool TryResolveTypeMemberConverters(MemberInfo memberInfo, ref object[] typeConverters)
        {
            if (memberInfo != null)
            {
                ChoTypeMemberConverterEventArgs e = new ChoTypeMemberConverterEventArgs() { MemberInfo = memberInfo, TypeConverters = new List<object>(typeConverters), TypeConverterParams = new List<object>(EmptyParams) };
                TypeMemberConverterResolve.Raise(null, e);
                if (e.Resolved)
                    typeConverters = e.TypeConverters.ToNArray();
                return e.Resolved;
            }
            return false;
        }

        private static bool TryResolveTypeMemberConverterParams(MemberInfo memberInfo, ref object[] typeConverterParams)
        {
            if (memberInfo != null)
            {
                ChoTypeMemberConverterEventArgs e = new ChoTypeMemberConverterEventArgs() { MemberInfo = memberInfo, TypeConverterParams = new List<object>(typeConverterParams), TypeConverters = new List<object>(EmptyTypeConverters), IsParamsRequested = true };
                TypeMemberConverterResolve.Raise(null, e);
                if (e.Resolved)
                    typeConverterParams = e.TypeConverterParams.ToNArray();
                return e.Resolved;
            }
            return false;
        }

        private static bool TryResolveTypeConverters(Type type, ref object[] typeConverters)
        {
            if (type != null)
            {
                ChoTypeConverterEventArgs e = new ChoTypeConverterEventArgs() { Type = type, TypeConverters = new List<object>(typeConverters), TypeConverterParams = new List<object>(EmptyParams) };
                TypeConverterResolve.Raise(null, e);
                if (e.Resolved)
                    typeConverters = e.TypeConverters.ToNArray();
                return e.Resolved;
            }
            return false;
        }

        private static bool TryResolveTypeConverterParams(Type type, ref object[] typeConverterParams)
        {
            if (type != null)
            {
                ChoTypeConverterEventArgs e = new ChoTypeConverterEventArgs() { Type = type, TypeConverterParams = new List<object>(typeConverterParams), TypeConverters = new List<object>(EmptyTypeConverters), IsParamsRequested = true };
                TypeConverterResolve.Raise(null, e);
                if (e.Resolved)
                    typeConverterParams = e.TypeConverterParams.ToNArray();
                return e.Resolved;
            }
            return false;
        }

        private static void Register(Type type)
        {
            ChoTypeConverterAttribute customAttribute = type.GetCustomAttribute<ChoTypeConverterAttribute>();
            if (customAttribute != null)
            {
                _typeTypeConverterCache.Add(type, new object[] { customAttribute.CreateInstance() });
                _typeTypeConverterParamsCache.Add(type, new object[] { customAttribute.Parameters });
            }
            ChoPriorityQueue queue = new ChoPriorityQueue();
            ChoPriorityQueue queue2 = new ChoPriorityQueue();
            foreach (Attribute attribute in ChoType.GetAttributes(type, typeof(ChoTypeConverterAttribute)))
            {
                ChoTypeConverterAttribute attribute2 = (ChoTypeConverterAttribute)attribute;
                if (attribute2 != null)
                {
                    queue.Enqueue(attribute2.Priority, attribute2.CreateInstance());
                    queue2.Enqueue(attribute2.Priority, attribute2.Parameters);
                }
            }
            if (queue.Count > 0)
            {
                _typeTypeConverterCache[type] = queue.ToArray();
                _typeTypeConverterParamsCache[type] = queue2.ToArray();
                return;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(type);
            if (converter != null && converter.GetType() != typeof(TypeConverter))
            {
                _typeTypeConverterCache.Add(type, new object[] { converter });
                _typeTypeConverterParamsCache.Add(type, EmptyParams);
            }
            else
            {
                _typeTypeConverterCache.Add(type, EmptyTypeConverters);
                _typeTypeConverterParamsCache.Add(type, EmptyParams);
            }
        }
    }
}
