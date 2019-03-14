namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public static class ChoSafeObjectFactory
    {
        #region Shared Data Members (Private)

        private static object _padLock = new object();

        #endregion Shared Data Members (Private)

        #region Shared Members (Public)

        public static object Construct(Type type)
        {
            return Construct<object>(type);
        }

        public static T Construct<T>(Type type) where T: new()
        {
            return Construct<T>(type, _padLock);
        }

        public static T Construct<T>(Type type, object padLock) where T : new()
        {
            ChoGuard.ArgumentNotNull(type, "Type");
            ChoGuard.ArgumentNotNull(padLock, "PadLock");

            return Construct<T>(type, _padLock);
        }

        #endregion Shared Members (Public)
    }
}
