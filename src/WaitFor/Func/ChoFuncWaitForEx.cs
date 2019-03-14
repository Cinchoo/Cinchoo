namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Services;

    #endregion NameSpaces

    public static class ChoFuncWaitForEx
    {
        #region ChoFuncWaitFor<TResult> Overloads

        public static TResult Run<TResult>(this Func<TResult> func)
        {
            return Run(func, Timeout.Infinite);
        }

        public static TResult Run<TResult>(this Func<TResult> func, int timeout)
        {
            return Run(func, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<TResult>(this Func<TResult> func, int timeout, int maxNoOfRetry)
        {
            return Run(func, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<TResult>(this Func<TResult> func, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<TResult> waitFor = new ChoFuncWaitFor<TResult>(func))
                return waitFor.Run(timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<TResult> Overloads

        #region ChoFuncWaitFor<T, TResult> Overloads

        public static TResult Run<T, TResult>(this Func<T, TResult> func, T arg)
        {
            return Run(func, arg, Timeout.Infinite);
        }

        public static TResult Run<T, TResult>(this Func<T, TResult> func, T arg, int timeout)
        {
            return Run(func, arg, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<T, TResult>(this Func<T, TResult> func, T arg, int timeout, int maxNoOfRetry)
        {
            return Run(func, arg, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<T, TResult>(this Func<T, TResult> func, T arg, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<T, TResult> waitFor = new ChoFuncWaitFor<T, TResult>(func))
                return waitFor.Run(arg, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<T, TResult> Overloads

        #region ChoFuncWaitFor<T1, T2, TResult> Overloads

        public static TResult Run<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1, T2 arg2)
        {
            return Run(func, arg1, arg2, Timeout.Infinite);
        }

        public static TResult Run<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1, T2 arg2, int timeout)
        {
            return Run(func, arg1, arg2, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1, T2 arg2, int timeout, int maxNoOfRetry)
        {
            return Run(func, arg1, arg2, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1, T2 arg2, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<T1, T2, TResult> waitFor = new ChoFuncWaitFor<T1, T2, TResult>(func))
                return waitFor.Run(arg1, arg2, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<T1, T2, TResult> Overloads

        #region ChoFuncWaitFor<T1, T2, T3, TResult> Overloads

        public static TResult Run<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3)
        {
            return Run(func, arg1, arg2, arg3, Timeout.Infinite);
        }

        public static TResult Run<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, int timeout)
        {
            return Run(func, arg1, arg2, arg3, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, int timeout, int maxNoOfRetry)
        {
            return Run(func, arg1, arg2, arg3, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<T1, T2, T3, TResult> waitFor = new ChoFuncWaitFor<T1, T2, T3, TResult>(func))
                return waitFor.Run(arg1, arg2, arg3, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<T1, T2, T3, TResult> Overloads

        #region ChoFuncWaitFor<T1, T2, T3, T4, TResult> Overloads

        public static TResult Run<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return Run(func, arg1, arg2, arg3, arg4, Timeout.Infinite);
        }

        public static TResult Run<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout)
        {
            return Run(func, arg1, arg2, arg3, arg4, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout, int maxNoOfRetry)
        {
            return Run(func, arg1, arg2, arg3, arg4, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<T1, T2, T3, T4, TResult> waitFor = new ChoFuncWaitFor<T1, T2, T3, T4, TResult>(func))
                return waitFor.Run(arg1, arg2, arg3, arg4, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<T1, T2, T3, T4, TResult> Overloads

        #region ChoFuncWaitFor<T1, T2, T3, T4, T5, TResult> Overloads

        public static TResult Run<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, Timeout.Infinite);
        }

        public static TResult Run<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout, int maxNoOfRetry)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<T1, T2, T3, T4, T5, TResult> waitFor = new ChoFuncWaitFor<T1, T2, T3, T4, T5, TResult>(func))
                return waitFor.Run(arg1, arg2, arg3, arg4, arg5, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, TResult> Overloads

        #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, TResult> Overloads

        public static TResult Run<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, Timeout.Infinite);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout, int maxNoOfRetry)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, TResult> waitFor = new ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, TResult>(func))
                return waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, TResult> Overloads

        #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, TResult> Overloads

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, Timeout.Infinite);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout, int maxNoOfRetry)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, TResult> waitFor = new ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, TResult>(func))
                return waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, TResult> Overloads

        #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Overloads

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, Timeout.Infinite);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout, int maxNoOfRetry)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, TResult> waitFor = new ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func))
                return waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Overloads

        #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> Overloads

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, Timeout.Infinite);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout, int maxNoOfRetry)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> waitFor = new ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func))
                return waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> Overloads

        #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> Overloads

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, Timeout.Infinite);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout, int maxNoOfRetry)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> waitFor = new ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func))
                return waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> Overloads

        #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> Overloads

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, Timeout.Infinite);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout, int maxNoOfRetry)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> waitFor = new ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func))
                return waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> Overloads

        #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> Overloads

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, Timeout.Infinite);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout, int maxNoOfRetry)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> waitFor = new ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func))
                return waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> Overloads

        #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> Overloads

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, Timeout.Infinite);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout, int maxNoOfRetry)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> waitFor = new ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func))
                return waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> Overloads

        #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> Overloads

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, Timeout.Infinite);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout, int maxNoOfRetry)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> waitFor = new ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func))
                return waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> Overloads

        #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> Overloads

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, Timeout.Infinite);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout, int maxNoOfRetry)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> waitFor = new ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func))
                return waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> Overloads

        #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> Overloads

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, Timeout.Infinite);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout, int maxNoOfRetry)
        {
            return Run(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static TResult Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> waitFor = new ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func))
                return waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> Overloads
    }
}
