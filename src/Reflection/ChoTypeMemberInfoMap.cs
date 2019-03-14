namespace eSquare.Core.Reflection
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;

    #endregion NameSpaces

    public static class ChoTypeMemberInfoMap
    {
        private static Dictionary<Type, Dictionary<string, System.Reflection.MemberInfo>> _typeMemberInfoMap = new Dictionary<Type, Dictionary<string, MemberInfo>>();
        private static readonly object _padLock = new object();

        public Dictionary<string, System.Reflection.MemberInfo> GetMembers(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Type");

            if (_typeMemberInfoMap.ContainsKey(Type))
                return _typeMemberInfoMap[Type];
            else
            {
                lock (_padLock)
                {
                    Dictionary<string, MemberInfo> memberInfo = new Dictionary<string, MemberInfo>();

                    _typeMemberInfoMap.Add(type, memberInfo);
                }
            }
        }
    }
}
