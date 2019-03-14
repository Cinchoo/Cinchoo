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

	public static class ChoActionWaitForEx
	{
        #region Action Overloads

        public static void Run(this Action action)
        {
            Run(action, Timeout.Infinite);
        }

        public static void Run(this Action action, int timeout)
        {
            Run(action, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run(this Action action, int timeout, int maxNoOfRetry)
        {
            Run(action, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run(this Action action, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor waitFor = new ChoActionWaitFor(action))
                waitFor.Run(timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action Overloads

        #region Action<T> Overloads

        public static void Run<T>(this Action<T> action, T arg)
        {
            Run(action, arg, Timeout.Infinite);
        }

        public static void Run<T>(this Action<T> action, T arg, int timeout)
        {
            Run(action, arg, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run<T>(this Action<T> action, T arg, int timeout, int maxNoOfRetry)
        {
            Run(action, arg, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run<T>(this Action<T> action, T arg, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor<T> waitFor = new ChoActionWaitFor<T>(action))
                waitFor.Run(arg, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action<T> Overloads

        #region Action<T1, T2> Overloads

        public static void Run<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            Run(action, arg1, arg2, Timeout.Infinite);
        }

        public static void Run<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2, int timeout)
        {
            Run(action, arg1, arg2, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2, int timeout, int maxNoOfRetry)
        {
            Run(action, arg1, arg2, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor<T1, T2> waitFor = new ChoActionWaitFor<T1, T2>(action))
                waitFor.Run(arg1, arg2, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action<T> Overloads

        #region Action<T1, T2, T3> Overloads

        public static void Run<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            Run(action, arg1, arg2, arg3, Timeout.Infinite);
        }

        public static void Run<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, int timeout)
        {
            Run(action, arg1, arg2, arg3, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, int timeout, int maxNoOfRetry)
        {
            Run(action, arg1, arg2, arg3, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor<T1, T2, T3> waitFor = new ChoActionWaitFor<T1, T2, T3>(action))
                waitFor.Run(arg1, arg2, arg3, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action<T1, T2, T3> Overloads

        #region Action<T1, T2, T3, T4> Overloads

        public static void Run<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Run(action, arg1, arg2, arg3, arg4, Timeout.Infinite);
        }

        public static void Run<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout)
        {
            Run(action, arg1, arg2, arg3, arg4, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout, int maxNoOfRetry)
        {
            Run(action, arg1, arg2, arg3, arg4, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor<T1, T2, T3, T4> waitFor = new ChoActionWaitFor<T1, T2, T3, T4>(action))
                waitFor.Run(arg1, arg2, arg3, arg4, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action<T1, T2, T3, T4> Overloads

        #region Action<T1, T2, T3, T4, T5> Overloads

        public static void Run<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, Timeout.Infinite);
        }

        public static void Run<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout, int maxNoOfRetry)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor<T1, T2, T3, T4, T5> waitFor = new ChoActionWaitFor<T1, T2, T3, T4, T5>(action))
                waitFor.Run(arg1, arg2, arg3, arg4, arg5, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action<T1, T2, T3, T4, T5> Overloads

        #region Action<T1, T2, T3, T4, T5, T6> Overloads

        public static void Run<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, Timeout.Infinite);
        }

        public static void Run<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout, int maxNoOfRetry)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor<T1, T2, T3, T4, T5, T6> waitFor = new ChoActionWaitFor<T1, T2, T3, T4, T5, T6>(action))
                waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action<T1, T2, T3, T4, T5, T6> Overloads

        #region Action<T1, T2, T3, T4, T5, T6, T7> Overloads

        public static void Run<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, Timeout.Infinite);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout, int maxNoOfRetry)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7> waitFor = new ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7>(action))
                waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action<T1, T2, T3, T4, T5, T6, T7> Overloads

        #region Action<T1, T2, T3, T4, T5, T6, T7, T8> Overloads

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, Timeout.Infinite);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout, int maxNoOfRetry)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8> waitFor = new ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8>(action))
                waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action<T1, T2, T3, T4, T5, T6, T7, T8> Overloads

        #region Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> Overloads

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, Timeout.Infinite);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout, int maxNoOfRetry)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9> waitFor = new ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9>(action))
                waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> Overloads

        #region Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Overloads

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, Timeout.Infinite);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout, int maxNoOfRetry)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> waitFor = new ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(action))
                waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Overloads

        #region Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Overloads

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, Timeout.Infinite);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout, int maxNoOfRetry)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> waitFor = new ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(action))
                waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Overloads

        #region Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Overloads

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, Timeout.Infinite);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout, int maxNoOfRetry)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> waitFor = new ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(action))
                waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Overloads

        #region Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Overloads

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, Timeout.Infinite);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout, int maxNoOfRetry)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> waitFor = new ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(action))
                waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Overloads

        #region Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Overloads

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, Timeout.Infinite);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout, int maxNoOfRetry)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> waitFor = new ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(action))
                waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Overloads

        #region Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Overloads

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, Timeout.Infinite);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout, int maxNoOfRetry)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> waitFor = new ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(action))
                waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Overloads

        #region Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Overloads

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, Timeout.Infinite);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout, int maxNoOfRetry)
        {
            Run(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> waitFor = new ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(action))
                waitFor.Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Overloads
    }
}
