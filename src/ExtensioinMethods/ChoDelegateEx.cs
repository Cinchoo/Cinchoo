namespace System
{
    #region NameSpaces

    using Cinchoo.Core;
    using Cinchoo.Core.Diagnostics;
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

        public static void InvokeEx<T>(this MulticastDelegate source, object target, T args,
            Action<Delegate> removeHandler = null)
            where T: EventArgs
        {
            MulticastDelegate handler = source;

            if (handler != null)
            {
                foreach (Delegate invokeHandler in handler.GetInvocationList())
                {
                    try
                    {
                        invokeHandler.DynamicInvoke(new object[] { target, args });
                    }
                    catch (ChoFatalApplicationException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        ChoTrace.Error(ex);
                        if (removeHandler != null)
                            removeHandler(invokeHandler);
                    }
                }
            }
        }
    }
}
