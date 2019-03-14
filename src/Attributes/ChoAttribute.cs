namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public abstract class ChoAttribute : Attribute
    {
        #region Shared Members (Public)

        public static T GetMe<T>(object target) where T: Attribute
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            return GetMe<T>(target.GetType());
        }

        public static T GetMe<T>(Type type) where T: Attribute
        {
            ChoGuard.ArgumentNotNull(type, "Type");
            return ChoType.GetAttribute<T>(type);
        }

        #endregion Shared Members (Public)
    }
}
