namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Runtime.Remoting;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Diagnostics;

    #endregion NameSpaces

    public static class ChoActivator
    {
        public static object CreateInstance(Type type)
        {
            try
            {
                object obj = Activator.CreateInstance(type);
                if (obj != null)
                    Initialize(obj, true);

                return obj;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }
        }

        public static object CreateInstance(Type type, params object[] args)
        {
            try
            {
                object obj = Activator.CreateInstance(type, args);
                if (obj != null)
                    Initialize(obj);

                return obj;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }
        }

        public static T CreateInstance<T>(params object[] args)
        {
            try
            {
                T obj = (T)Activator.CreateInstance(typeof(T), args);
                Initialize(obj);

                return obj;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }
        }

        public static T CreateInstance<T>()
        {
            try
            {
                T obj = Activator.CreateInstance<T>();
                Initialize(obj, true);

                return obj;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }
        }

        private static void Initialize(object obj, bool resetObj = false)
        {
            if (obj == null) return;

            if (resetObj)
                ChoObject.ResetObject(obj);

            if (obj is IChoInitializable)
                ((IChoInitializable)obj).Initialize();
        }
    }
}
