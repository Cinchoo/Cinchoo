namespace Cinchoo.Core.Services
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using Cinchoo.Core.Services;

	#endregion NameSpaces

	public static class ChoAPM
	{
		#region Constants

        internal static readonly int DEF_SLEEP_BETWEEN_RETRY = ChoAPMSettings.Me.SleepBetweenRetry;
        internal static readonly int DEF_MAX_NO_OF_RETRY = ChoAPMSettings.Me.MaxNoOfRetry;

		#endregion Constants

		#region InvokeMethod Overloads

		public static object InvokeMethod(Delegate func)
		{
			return InvokeMethod(func, Timeout.Infinite);
		}

		public static object InvokeMethod(Delegate func, int timeout)
		{
			return InvokeMethod(func, timeout, DEF_MAX_NO_OF_RETRY);
		}

		public static object InvokeMethod(Delegate func, int timeout, int maxNoOfRetry)
		{
			return InvokeMethod(func, timeout, maxNoOfRetry, DEF_SLEEP_BETWEEN_RETRY);
		}

		public static object InvokeMethod(Delegate func, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return InvokeMethod(func, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public static object InvokeMethod(Delegate func, object[] parameters)
		{
			return InvokeMethod(func, parameters, Timeout.Infinite);
		}

		public static object InvokeMethod(Delegate func, object[] parameters, int timeout)
		{
			return InvokeMethod(func, parameters, timeout, DEF_MAX_NO_OF_RETRY);
		}

		public static object InvokeMethod(Delegate func, object[] parameters, int timeout, int maxNoOfRetry)
		{
			return InvokeMethod(func, parameters, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static object InvokeMethod(Delegate func, object[] parameters, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			ChoGuard.ArgumentNotNull(func, "Function");

			if (timeout != Timeout.Infinite && timeout < 0)
				throw new ArgumentOutOfRangeException("Timeout");

            return func.WaitFor(parameters, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion InvokeMethod Overloads

		#region BeginInvokeMethod Overloads

		public static IChoAbortableAsyncResult BeginInvokeMethod(Delegate func)
		{
			return BeginInvokeMethod(func, Timeout.Infinite);
		}

		public static IChoAbortableAsyncResult BeginInvokeMethod(Delegate func, int timeout)
		{
			return BeginInvokeMethod(func, timeout, DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAbortableAsyncResult BeginInvokeMethod(Delegate func, int timeout, int maxNoOfRetry)
		{
			return BeginInvokeMethod(func, timeout, maxNoOfRetry, DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAbortableAsyncResult BeginInvokeMethod(Delegate func, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(func, (object[])null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public static IChoAbortableAsyncResult BeginInvokeMethod(Delegate func, object[] parameters)
		{
			return BeginInvokeMethod(func, parameters, Timeout.Infinite);
		}

		public static IChoAbortableAsyncResult BeginInvokeMethod(Delegate func, object[] parameters, int timeout)
		{
			return BeginInvokeMethod(func, parameters, timeout, DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAbortableAsyncResult BeginInvokeMethod(Delegate func, object[] parameters, int timeout, int maxNoOfRetry)
		{
			return BeginInvokeMethod(func, parameters, timeout, maxNoOfRetry, DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAbortableAsyncResult BeginInvokeMethod(Delegate func, object[] parameters, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(func, parameters, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public static IChoAbortableAsyncResult BeginInvokeMethod(Delegate func, object[] parameters, ChoAsyncCallback callback, object state)
		{
			return BeginInvokeMethod(func, parameters, callback, state, Timeout.Infinite);
		}
		
		public static IChoAbortableAsyncResult BeginInvokeMethod(Delegate func, object[] parameters, ChoAsyncCallback callback, object state, int timeout)
		{
			return BeginInvokeMethod(func, parameters, callback, state, timeout, DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAbortableAsyncResult BeginInvokeMethod(Delegate func, object[] parameters, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return BeginInvokeMethod(func, parameters, callback, state, timeout, maxNoOfRetry, DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAbortableAsyncResult BeginInvokeMethod(Delegate func, object[] parameters, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			ChoGuard.ArgumentNotNull(func, "Function");

			if (timeout != Timeout.Infinite && timeout < 0)
				throw new ArgumentOutOfRangeException("Timeout");

			return ChoAbortableQueuedExecutionService.Me.EnqueueMethod(func, parameters, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry) as IChoAbortableAsyncResult;
		}

		#endregion BeginInvokeMethod Overloads

        #region Func Overloads

        #region Invoke<Func<TResult>> Overloads

        public static TResult Invoke<TResult>(Func<TResult> func)
		{
			return Invoke<TResult>(func, Timeout.Infinite);
		}

		public static TResult Invoke<TResult>(Func<TResult> func, int timeout)
		{
			return Invoke<TResult>(func, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static TResult Invoke<TResult>(Func<TResult> func, int timeout, int maxNoOfRetry)
		{
			return Invoke<TResult>(func, timeout, maxNoOfRetry, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static TResult Invoke<TResult>(Func<TResult> func, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return (TResult)InvokeMethod(func, (object[])null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Invoke<Func<TResult>> Overloads

		#region BeginInvoke<Func<TResult>> Overloads

		public static IChoAsyncResult BeginInvoke<TResult>(Func<TResult> func)
		{
			return BeginInvoke<TResult>(func, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<TResult>(Func<TResult> func, int timeout)
		{
			return BeginInvoke<TResult>(func, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<TResult>(Func<TResult> func, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<TResult>(func, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<TResult>(Func<TResult> func, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvoke<TResult>(func, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public static IChoAsyncResult BeginInvoke<TResult>(Func<TResult> func, ChoAsyncCallback callback, object state)
		{
			return BeginInvoke<TResult>(func, callback, state, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<TResult>(Func<TResult> func, ChoAsyncCallback callback, object state, int timeout)
		{
			return BeginInvoke<TResult>(func, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<TResult>(Func<TResult> func, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<TResult>(func, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<TResult>(Func<TResult> func, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(func, (object[])null, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion BeginInvoke<Func<TResult>> Overloads

		#region Invoke<Func<T, TResult>> Overloads

		public static TResult Invoke<T, TResult>(Func<T, TResult> func, T arg)
		{
			return Invoke<T, TResult>(func, arg, Timeout.Infinite);
		}

		public static TResult Invoke<T, TResult>(Func<T, TResult> func, T arg, int timeout)
		{
			return Invoke<T, TResult>(func, arg, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static TResult Invoke<T, TResult>(Func<T, TResult> func, T arg, int timeout, int maxNoOfRetry)
		{
			return Invoke<T, TResult>(func, arg, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static TResult Invoke<T, TResult>(Func<T, TResult> func, T arg, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return (TResult)InvokeMethod(func, new object[] { arg }, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Invoke<Func<T, TResult>> Overloads

		#region BeginInvoke<Func<T, TResult>> Overloads

		public static IChoAsyncResult BeginInvoke<T, TResult>(Func<T, TResult> func, T arg)
		{
			return BeginInvoke<T, TResult>(func, arg, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<T, TResult>(Func<T, TResult> func, T arg, int timeout)
		{
			return BeginInvoke<T, TResult>(func, arg, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T, TResult>(Func<T, TResult> func, T arg, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<T, TResult>(func, arg, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T, TResult>(Func<T, TResult> func, T arg, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvoke<T, TResult>(func, arg, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public static IChoAsyncResult BeginInvoke<T, TResult>(Func<T, TResult> func, T arg, ChoAsyncCallback callback, object state)
		{
			return BeginInvoke<T, TResult>(func, arg, callback, state, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<T, TResult>(Func<T, TResult> func, T arg, ChoAsyncCallback callback, object state, int timeout)
		{
			return BeginInvoke<T, TResult>(func, arg, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T, TResult>(Func<T, TResult> func, T arg, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<T, TResult>(func, arg, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T, TResult>(Func<T, TResult> func, T arg, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(func, new object[] { arg }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion BeginInvoke<Func<TResult>> Overloads

		#region Invoke<Func<T1, T2, TResult>> Overloads

		public static TResult Invoke<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2)
		{
			return Invoke<T1, T2, TResult>(func, arg1, arg2, Timeout.Infinite);
		}

		public static TResult Invoke<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2, int timeout)
		{
			return Invoke<T1, T2, TResult>(func, arg1, arg2, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static TResult Invoke<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2, int timeout, int maxNoOfRetry)
		{
			return Invoke<T1, T2, TResult>(func, arg1, arg2, timeout, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static TResult Invoke<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return (TResult)InvokeMethod(func, new object[] { arg1, arg2 }, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Invoke<Func<T1, T2, TResult>> Overloads

		#region BeginInvoke<Func<T1, T2, TResult>> Overloads

		public static IChoAsyncResult BeginInvoke<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2)
		{
			return BeginInvoke<T1, T2, TResult>(func, arg1, arg2, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2, int timeout)
		{
			return BeginInvoke<T1, T2, TResult>(func, arg1, arg2, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<T1, T2, TResult>(func, arg1, arg2, timeout, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvoke<T1, T2, TResult>(func, arg1, arg2, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2, ChoAsyncCallback callback, object state)
		{
			return BeginInvoke<T1, T2, TResult>(func, arg1, arg2, callback, state, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2, ChoAsyncCallback callback, object state, int timeout)
		{
			return BeginInvoke<T1, T2, TResult>(func, arg1, arg2, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<T1, T2, TResult>(func, arg1, arg2, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(func, new object[] { arg1, arg2 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion BeginInvoke<Func<T1, T2, TResult>> Overloads

		#region Invoke<Func<T1, T2, T3, TResult>> Overloads

		public static TResult Invoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3)
		{
			return Invoke<T1, T2, T3, TResult>(func, arg1, arg2, arg3, Timeout.Infinite);
		}

		public static TResult Invoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, int timeout)
		{
			return Invoke<T1, T2, T3, TResult>(func, arg1, arg2, arg3, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static TResult Invoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, int timeout, int maxNoOfRetry)
		{
			return Invoke<T1, T2, T3, TResult>(func, arg1, arg2, arg3, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static TResult Invoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return (TResult)InvokeMethod(func, new object[] { arg1, arg2, arg3 }, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Invoke<Func<T1, T2, T3, TResult>> Overloads

		#region BeginInvoke<Func<T1, T2, T3, TResult>> Overloads

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3)
		{
			return BeginInvoke<T1, T2, T3, TResult>(func, arg1, arg2, arg3, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, int timeout)
		{
			return BeginInvoke<T1, T2, T3, TResult>(func, arg1, arg2, arg3, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<T1, T2, T3, TResult>(func, arg1, arg2, arg3, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvoke<T1, T2, T3, TResult>(func, arg1, arg2, arg3, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, ChoAsyncCallback callback, object state)
		{
			return BeginInvoke<T1, T2, T3, TResult>(func, arg1, arg2, arg3, callback, state, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, ChoAsyncCallback callback, object state, int timeout)
		{
			return BeginInvoke<T1, T2, T3, TResult>(func, arg1, arg2, arg3, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<T1, T2, T3, TResult>(func, arg1, arg2, arg3, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(func, new object[] { arg1, arg2, arg3 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion BeginInvoke<Func<T1, T2, T3, TResult>> Overloads

		#region Invoke<Func<T1, T2, T3, T4, TResult>> Overloads

		public static TResult Invoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			return Invoke<T1, T2, T3, T4, TResult>(func, arg1, arg2, arg3, arg4, Timeout.Infinite);
		}

		public static TResult Invoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout)
		{
			return Invoke<T1, T2, T3, T4, TResult>(func, arg1, arg2, arg3, arg4, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static TResult Invoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout, int maxNoOfRetry)
		{
			return Invoke<T1, T2, T3, T4, TResult>(func, arg1, arg2, arg3, arg4, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static TResult Invoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return (TResult)InvokeMethod(func, new object[] { arg1, arg2, arg3, arg4 }, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Invoke<Func<T1, T2, T3, T4, TResult>> Overloads

		#region BeginInvoke<Func<T1, T2, T3, T4, TResult>> Overloads

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			return BeginInvoke<T1, T2, T3, T4, TResult>(func, arg1, arg2, arg3, arg4, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout)
		{
			return BeginInvoke<T1, T2, T3, T4, TResult>(func, arg1, arg2, arg3, arg4, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<T1, T2, T3, T4, TResult>(func, arg1, arg2, arg3, arg4, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvoke<T1, T2, T3, T4, TResult>(func, arg1, arg2, arg3, arg4, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, ChoAsyncCallback callback, object state)
		{
			return BeginInvoke<T1, T2, T3, T4, TResult>(func, arg1, arg2, arg3, arg4, callback, state, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, ChoAsyncCallback callback, object state, int timeout)
		{
			return BeginInvoke<T1, T2, T3, T4, TResult>(func, arg1, arg2, arg3, arg4, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<T1, T2, T3, T4, TResult>(func, arg1, arg2, arg3, arg4, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(func, new object[] { arg1, arg2, arg3, arg4 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion BeginInvoke<Func<T1, T2, T3, T4, TResult>> Overloads

        #region Invoke<Func<T1, T2, T3, T4, T5, TResult>> Overloads

        public static TResult Invoke<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return Invoke<T1, T2, T3, T4, T5, TResult>(func, arg1, arg2, arg3, arg4, arg5, Timeout.Infinite);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout)
        {
            return Invoke<T1, T2, T3, T4, T5, TResult>(func, arg1, arg2, arg3, arg4, arg5, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout, int maxNoOfRetry)
        {
            return Invoke<T1, T2, T3, T4, T5, TResult>(func, arg1, arg2, arg3, arg4, arg5, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return (TResult)InvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Func<T1, T2, T3, T4, T5, TResult>> Overloads

        #region BeginInvoke<Func<T1, T2, T3, T4, T5, TResult>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, TResult>(func, arg1, arg2, arg3, arg4, arg5, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, TResult>(func, arg1, arg2, arg3, arg4, arg5, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, TResult>(func, arg1, arg2, arg3, arg4, arg5, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, TResult>(func, arg1, arg2, arg3, arg4, arg5, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, TResult>(func, arg1, arg2, arg3, arg4, arg5, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, TResult>(func, arg1, arg2, arg3, arg4, arg5, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, TResult>(func, arg1, arg2, arg3, arg4, arg5, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Func<T1, T2, T3, T4, TResult>> Overloads

        #region Invoke<Func<T1, T2, T3, T4, T5, T6, TResult>> Overloads

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, Timeout.Infinite);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout, int maxNoOfRetry)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return (TResult)InvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Func<T1, T2, T3, T4, T5, T6, TResult>> Overloads

        #region BeginInvoke<Func<T1, T2, T3, T4, T5, T6, TResult>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Func<T1, T2, T3, T4, TResult>> Overloads

        #region Invoke<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> Overloads

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, Timeout.Infinite);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout, int maxNoOfRetry)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return (TResult)InvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> Overloads

        #region BeginInvoke<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Func<T1, T2, T3, T4, TResult>> Overloads

        #region Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> Overloads

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, Timeout.Infinite);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout, int maxNoOfRetry)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return (TResult)InvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> Overloads

        #region BeginInvoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Func<T1, T2, T3, T4, TResult>> Overloads

        #region Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> Overloads

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, Timeout.Infinite);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout, int maxNoOfRetry)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return (TResult)InvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> Overloads

        #region BeginInvoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Func<T1, T2, T3, T4, TResult>> Overloads

        #region Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> Overloads

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, Timeout.Infinite);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout, int maxNoOfRetry)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return (TResult)InvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> Overloads

        #region BeginInvoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Func<T1, T2, T3, T4, TResult>> Overloads

        #region Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> Overloads

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, Timeout.Infinite);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout, int maxNoOfRetry)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return (TResult)InvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> Overloads

        #region BeginInvoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Func<T1, T2, T3, T4, TResult>> Overloads

        #region Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> Overloads

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, Timeout.Infinite);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout, int maxNoOfRetry)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return (TResult)InvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> Overloads

        #region BeginInvoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Func<T1, T2, T3, T4, TResult>> Overloads

        #region Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> Overloads

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, Timeout.Infinite);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout, int maxNoOfRetry)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return (TResult)InvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> Overloads

        #region BeginInvoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Func<T1, T2, T3, T4, TResult>> Overloads

        #region Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> Overloads

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, Timeout.Infinite);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout, int maxNoOfRetry)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return (TResult)InvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> Overloads

        #region BeginInvoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Func<T1, T2, T3, T4, TResult>> Overloads

        #region Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> Overloads

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, Timeout.Infinite);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout, int maxNoOfRetry)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return (TResult)InvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> Overloads

        #region BeginInvoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Func<T1, T2, T3, T4, TResult>> Overloads

        #region Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> Overloads

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, Timeout.Infinite);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout, int maxNoOfRetry)
        {
            return Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return (TResult)InvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> Overloads

        #region BeginInvoke<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(func, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Func<T1, T2, T3, T4, TResult>> Overloads

        #endregion Func Overloads

        #region Action Overloads

        #region Invoke<Action> Overloads

        public static void Invoke(Action action)
		{
			Invoke(action, Timeout.Infinite);
		}

		public static void Invoke(Action action, int timeout)
		{
			Invoke(action, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static void Invoke(Action action, int timeout, int maxNoOfRetry)
		{
			Invoke(action, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static void Invoke(Action action, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			InvokeMethod(action, (object[])null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Invoke<Action> Overloads

		#region BeginInvoke<Action> Overloads

		public static IChoAsyncResult BeginInvoke(Action action)
		{
			return BeginInvoke(action, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke(Action action, int timeout)
		{
			return BeginInvoke(action, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke(Action action, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke(action, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke(Action action, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(action, (object[])null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public static IChoAsyncResult BeginInvoke(Action action, ChoAsyncCallback callback, object state)
		{
			return BeginInvoke(action, callback, state, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke(Action action, ChoAsyncCallback callback, object state, int timeout)
		{
			return BeginInvoke(action, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke(Action action, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke(action, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke(Action action, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(action, (object[])null, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion BeginInvoke<Action> Overloads

		#region Invoke<Action<T>> Overloads

		public static void Invoke<T>(Action<T> action, T arg)
		{
			Invoke<T>(action, arg, Timeout.Infinite);
		}

		public static void Invoke<T>(Action<T> action, T arg, int timeout)
		{
			Invoke<T>(action, arg, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static void Invoke<T>(Action<T> action, T arg, int timeout, int maxNoOfRetry)
		{
			Invoke<T>(action, arg, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static void Invoke<T>(Action<T> action, T arg, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			InvokeMethod(action, new object[] { arg }, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Invoke<Action<T>> Overloads

		#region BeginInvoke<Action<T>> Overloads

		public static IChoAsyncResult BeginInvoke<T>(Action<T> action, T arg)
		{
			return BeginInvoke<T>(action, arg, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<T>(Action<T> action, T arg, int timeout)
		{
			return BeginInvoke<T>(action, arg, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T>(Action<T> action, T arg, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<T>(action, arg, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T>(Action<T> action, T arg, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(action, new object[] { arg }, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public static IChoAsyncResult BeginInvoke<T>(Action<T> action, T arg, ChoAsyncCallback callback, object state)
		{
			return BeginInvoke<T>(action, arg, callback, state, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<T>(Action<T> action, T arg, ChoAsyncCallback callback, object state, int timeout)
		{
			return BeginInvoke<T>(action, arg, callback, state, Timeout.Infinite, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T>(Action<T> action, T arg, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<T>(action, arg, callback, state, Timeout.Infinite, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T>(Action<T> action, T arg, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(action, new object[] { arg }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion BeginInvoke<Action<T>> Overloads

		#region Invoke<Action<T1, T2>> Overloads

		public static void Invoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
		{
			Invoke<T1, T2>(action, arg1, arg2, Timeout.Infinite);
		}

		public static void Invoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, int timeout)
		{
			Invoke<T1, T2>(action, arg1, arg2, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static void Invoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, int timeout, int maxNoOfRetry)
		{
			Invoke<T1, T2>(action, arg1, arg2, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static void Invoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			InvokeMethod(action, new object[] { arg1, arg2 }, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Invoke<Action<T1, T2>> Overloads

		#region BeginInvoke<Action<T1, T2>> Overloads

		public static IChoAsyncResult BeginInvoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
		{
			return BeginInvoke<T1, T2>(action, arg1, arg2, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, int timeout)
		{
			return BeginInvoke<T1, T2>(action, arg1, arg2, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<T1, T2>(action, arg1, arg2, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(action, new object[] { arg1, arg2 }, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, ChoAsyncCallback callback, object state)
		{
			return BeginInvoke<T1, T2>(action, arg1, arg2, callback, state, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, ChoAsyncCallback callback, object state, int timeout)
		{
			return BeginInvoke<T1, T2>(action, arg1, arg2, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<T1, T2>(action, arg1, arg2, callback, state, timeout, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(action, new object[] { arg1, arg2 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion BeginInvoke<Action<T1, T2>> Overloads

		#region Invoke<Action<T1, T2, T3>> Overloads

		public static void Invoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
		{
			Invoke<T1, T2, T3>(action, arg1, arg2, arg3, Timeout.Infinite);
		}

		public static void Invoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, int timeout)
		{
			Invoke<T1, T2, T3>(action, arg1, arg2, arg3, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static void Invoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, int timeout, int maxNoOfRetry)
		{
			Invoke<T1, T2, T3>(action, arg1, arg2, arg3, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static void Invoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			InvokeMethod(action, new object[] { arg1, arg2, arg3 }, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Invoke<Action<T1, T2, T3>> Overloads

		#region BeginInvoke<Action<T1, T2, T3>> Overloads

		public static IChoAsyncResult BeginInvoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
		{
			return BeginInvoke<T1, T2, T3>(action, arg1, arg2, arg3, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, int timeout)
		{
			return BeginInvoke<T1, T2, T3>(action, arg1, arg2, arg3, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<T1, T2, T3>(action, arg1, arg2, arg3, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3 }, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, ChoAsyncCallback callback, object state)
		{
			return BeginInvoke<T1, T2, T3>(action, arg1, arg2, arg3, callback, state, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, ChoAsyncCallback callback, object state, int timeout)
		{
			return BeginInvoke<T1, T2, T3>(action, arg1, arg2, arg3, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<T1, T2, T3>(action, arg1, arg2, arg3, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Invoke<Action<T1, T2, T3>> Overloads

		#region Invoke<Action<T1, T2, T3, T4>> Overloads

		public static void Invoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			Invoke<T1, T2, T3, T4>(action, arg1, arg2, arg3, arg4, Timeout.Infinite);
		}

		public static void Invoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout)
		{
			Invoke<T1, T2, T3, T4>(action, arg1, arg2, arg3, arg4, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static void Invoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout, int maxNoOfRetry)
		{
			Invoke<T1, T2, T3, T4>(action, arg1, arg2, arg3, arg4, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static void Invoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			InvokeMethod(action, new object[] { arg1, arg2, arg3, arg4 }, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Invoke<Action<T1, T2, T3, T4>> Overloads

		#region BeginInvoke<Action<T1, T2, T3, T4>> Overloads

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			return BeginInvoke<T1, T2, T3, T4>(action, arg1, arg2, arg3, arg4, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout)
		{
			return BeginInvoke<T1, T2, T3, T4>(action, arg1, arg2, arg3, arg4, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<T1, T2, T3, T4>(action, arg1, arg2, arg3, arg4, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4 }, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, ChoAsyncCallback callback, object state)
		{
			return BeginInvoke<T1, T2, T3, T4>(action, arg1, arg2, arg3, arg4, callback, state, Timeout.Infinite);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, ChoAsyncCallback callback, object state, int timeout)
		{
			return BeginInvoke<T1, T2, T3, T4>(action, arg1, arg2, arg3, arg4, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return BeginInvoke<T1, T2, T3, T4>(action, arg1, arg2, arg3, arg4, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
		}

		public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion BeginInvoke<Action<T1, T2, T3, T4>> Overloads

        #region Invoke<Action<T1, T2, T3, T4, T5>> Overloads

        public static void Invoke<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Invoke<T1, T2, T3, T4, T5>(action, arg1, arg2, arg3, arg4, arg5, Timeout.Infinite);
        }

        public static void Invoke<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout)
        {
            Invoke<T1, T2, T3, T4, T5>(action, arg1, arg2, arg3, arg4, arg5, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout, int maxNoOfRetry)
        {
            Invoke<T1, T2, T3, T4, T5>(action, arg1, arg2, arg3, arg4, arg5, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            InvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Action<T1, T2, T3, T4, T5>> Overloads

        #region BeginInvoke<Action<T1, T2, T3, T4, T5>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return BeginInvoke<T1, T2, T3, T4, T5>(action, arg1, arg2, arg3, arg4, arg5, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5>(action, arg1, arg2, arg3, arg4, arg5, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5>(action, arg1, arg2, arg3, arg4, arg5, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5>(action, arg1, arg2, arg3, arg4, arg5, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5>(action, arg1, arg2, arg3, arg4, arg5, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5>(action, arg1, arg2, arg3, arg4, arg5, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Action<T1, T2, T3, T4, T5>> Overloads

        #region Invoke<Action<T1, T2, T3, T4, T5, T6>> Overloads

        public static void Invoke<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Invoke<T1, T2, T3, T4, T5, T6>(action, arg1, arg2, arg3, arg4, arg5, arg6, Timeout.Infinite);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout)
        {
            Invoke<T1, T2, T3, T4, T5, T6>(action, arg1, arg2, arg3, arg4, arg5, arg6, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout, int maxNoOfRetry)
        {
            Invoke<T1, T2, T3, T4, T5, T6>(action, arg1, arg2, arg3, arg4, arg5, arg6, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            InvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Action<T1, T2, T3, T4, T5, T6>> Overloads

        #region BeginInvoke<Action<T1, T2, T3, T4, T5, T6>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6>(action, arg1, arg2, arg3, arg4, arg5, arg6, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6>(action, arg1, arg2, arg3, arg4, arg5, arg6, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6>(action, arg1, arg2, arg3, arg4, arg5, arg6, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6>(action, arg1, arg2, arg3, arg4, arg5, arg6, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6>(action, arg1, arg2, arg3, arg4, arg5, arg6, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6>(action, arg1, arg2, arg3, arg4, arg5, arg6, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Action<T1, T2, T3, T4, T5, T6>> Overloads

        #region Invoke<Action<T1, T2, T3, T4, T5, T6, T7>> Overloads

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, Timeout.Infinite);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout, int maxNoOfRetry)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            InvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Action<T1, T2, T3, T4, T5, T6, T7>> Overloads

        #region BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7>> Overloads

        #region Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8>> Overloads

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, Timeout.Infinite);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout, int maxNoOfRetry)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            InvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8>> Overloads

        #region BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8>> Overloads

        #region Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>> Overloads

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, Timeout.Infinite);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout, int maxNoOfRetry)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            InvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>> Overloads

        #region BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>> Overloads

        #region Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> Overloads

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, Timeout.Infinite);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout, int maxNoOfRetry)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            InvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> Overloads

        #region BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> Overloads

        #region Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> Overloads

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, Timeout.Infinite);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout, int maxNoOfRetry)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            InvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> Overloads

        #region BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> Overloads

        #region Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> Overloads

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, Timeout.Infinite);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout, int maxNoOfRetry)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            InvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> Overloads

        #region BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> Overloads

        #region Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> Overloads

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, Timeout.Infinite);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout, int maxNoOfRetry)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            InvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> Overloads

        #region BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> Overloads

        #region Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> Overloads

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, Timeout.Infinite);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout, int maxNoOfRetry)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            InvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> Overloads

        #region BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> Overloads

        #region Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> Overloads

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, Timeout.Infinite);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout, int maxNoOfRetry)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            InvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> Overloads

        #region BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> Overloads

        #region Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> Overloads

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, Timeout.Infinite);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout, int maxNoOfRetry)
        {
            Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            InvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion Invoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> Overloads

        #region BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> Overloads

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16 }, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, ChoAsyncCallback callback, object state)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, callback, state, Timeout.Infinite);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, ChoAsyncCallback callback, object state, int timeout)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, callback, state, timeout, ChoAPM.DEF_MAX_NO_OF_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
        {
            return BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, callback, state, timeout, maxNoOfRetry, ChoAPM.DEF_SLEEP_BETWEEN_RETRY);
        }

        public static IChoAsyncResult BeginInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            return BeginInvokeMethod(action, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion BeginInvoke<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> Overloads

        #endregion Action Overloads
    }
}
