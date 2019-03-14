namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Runtime.Remoting;
    using System.Collections.Generic;

    #endregion NameSpaces

    public static class ChoActivator
    {
        public static object CreateInstance(Type type)
        {
            object obj = Activator.CreateInstance(type);

            if (obj is IChoInitializable)
                ((IChoInitializable)obj).Initialize();

            return obj;
        }

        public static T CreateInstance<T>()
        {
            T obj = Activator.CreateInstance<T>();

            if (obj is IChoInitializable)
                ((IChoInitializable)obj).Initialize();

            return obj;
        }
    }
}
