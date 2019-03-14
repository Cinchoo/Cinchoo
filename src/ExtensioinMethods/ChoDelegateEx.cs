namespace System
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion NameSpaces

    public static class ChoDelegateEx
    {
        public static T ConvertTo<T>(this Delegate source)
            where T : class //, Delegate
        {
            if (source.GetInvocationList().Length > 1)
                throw new ArgumentException("Cannot safely convert MulticastDelegate");

            return Delegate.CreateDelegate(typeof(T), source.Target, source.Method) as T;
        }

        public static string ToString(this Delegate source)
        {
            if (source == null) return null;

            return String.Format("Target: {0}, Method: {1}", source.Target == null ? String.Empty : source.Target.ToString(), source.Method.ToString());
        }
    }
}
