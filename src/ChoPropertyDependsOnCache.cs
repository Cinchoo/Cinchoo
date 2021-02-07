using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cinchoo.Core
{
    internal class ChoPropertyDependsOnCache
    {
        public static readonly ChoPropertyDependsOnCache Instance;
        public static readonly object _padLock = new object();
        public static readonly Dictionary<Type, Dictionary<string, HashSet<string>>> _cache = new Dictionary<Type, Dictionary<string, HashSet<string>>>();

        static ChoPropertyDependsOnCache()
        {
            Instance = new ChoPropertyDependsOnCache();
        }

        private void Register(Type type)
        {
            if (type == null) return;
            if (_cache.ContainsKey(type)) return;

            lock (_padLock)
            {
                if (_cache.ContainsKey(type)) return;

                Dictionary<string, HashSet<string>> dependsOnCache = new Dictionary<string, HashSet<string>>(StringComparer.InvariantCultureIgnoreCase);
                _cache.Add(type, dependsOnCache);

                string memberName = null;
                MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(type);
                if (memberInfos != null && memberInfos.Length > 0)
                {
                    ChoPropertyDependsOnAttribute memberInfoAttribute = null;
                    foreach (MemberInfo memberInfo in memberInfos)
                    {
                        memberName = memberInfo.Name;
                        memberInfoAttribute = (ChoPropertyDependsOnAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoPropertyDependsOnAttribute));
                        if (memberInfoAttribute == null) continue;

                        string[] dependsOn = memberInfoAttribute.DependsOn;
                        if (dependsOn.IsNullOrEmpty()) continue;

                        foreach (string item in dependsOn)
                        {
                            if (item.IsNullOrWhiteSpace()) continue;

                            if (!dependsOnCache.ContainsKey(item))
                                dependsOnCache.Add(item, new HashSet<string>(StringComparer.InvariantCultureIgnoreCase));

                            if (dependsOnCache[item].Contains(memberName))
                                continue;

                            dependsOnCache[item].Add(memberName);
                        }
                    }
                }
            }
        }

        public string[] GetDependsOn(Type type, string memberName)
        {
            ChoGuard.ArgumentNotNull(type, "Type");
            ChoGuard.ArgumentNotNullOrEmpty(memberName, "MemberName");

            Register(type);
            if (!_cache.ContainsKey(type)
                || !_cache[type].ContainsKey(memberName) 
                || _cache[type][memberName] == null) 
                return new String[] { };

            return _cache[type][memberName].ToArray();
        }
    }
}
