namespace Cinchoo.Core.Services
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using Cinchoo.Core;
	
	using Cinchoo.Core.Threading;
using System.Diagnostics;

	#endregion NameSpaces

	[DebuggerDisplay("Name={Name}")]
	public abstract class ChoAbortableExecutionServiceBase : ChoSyncDisposableObject //, IChoExecutionService
	{
		#region Instance Data Members (Private)

		private readonly string _name;

		#endregion Instance Data Members (Private)
		
		#region Constructors

		public ChoAbortableExecutionServiceBase(string name)
		{
			ChoGuard.ArgumentNotNullOrEmpty(name, "Name");
			_name = name;
		}

		#endregion Constructors

		#region Instance Properties (Public)

		public virtual string Name
		{
			get { return _name; }
		}

		#endregion Instance Properties (Public)

		#region Instance Members (Public)

		public abstract void Start();
		public abstract void Stop();
		public abstract IChoAbortableAsyncResult EnqueueMethod(Delegate func, object[] parameters, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry);

		#endregion Instance Members (Public)

		#region Object Overrides

		public override string ToString()
		{
			return Name;
		}

		public override int GetHashCode()
		{
			return Name == null ? base.GetHashCode() : Name.GetHashCode();
		}

		#endregion Object Overrides

		#region Enqueue Overloads

		#region Func Overloads

		#region Enqueue<TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<TResult>(Func<TResult> func)
		{
			return Enqueue<TResult>(func, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<TResult>(Func<TResult> func, int timeout)
		{
			return Enqueue<TResult>(func, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<TResult>(Func<TResult> func, int timeout, int maxNoOfRetry)
		{
			return Enqueue<TResult>(func, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<TResult>(Func<TResult> func, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<TResult>(func, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<TResult>(Func<TResult> func, ChoAsyncCallback callback, object state)
		{
			return Enqueue<TResult>(func, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<TResult>(Func<TResult> func, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<TResult>(func, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<TResult>(Func<TResult> func, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<TResult>(func, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<TResult>(Func<TResult> func, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, (object[])null, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<TResult> Overloads

		#region Enqueue<T, TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<T, TResult>(Func<T, TResult> func, T param)
		{
			return Enqueue<T, TResult>(func, param, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T, TResult>(Func<T, TResult> func, T param, int timeout)
		{
			return Enqueue<T, TResult>(func, param, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T, TResult>(Func<T, TResult> func, T param, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T, TResult>(func, param, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T, TResult>(Func<T, TResult> func, T param, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T, TResult>(func, param, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T, TResult>(Func<T, TResult> func, T param, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T, TResult>(func, param, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T, TResult>(Func<T, TResult> func, T param, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T, TResult>(func, param, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T, TResult>(Func<T, TResult> func, T param, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T, TResult>(func, param, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T, TResult>(Func<T, TResult> func, T param, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T, TResult> Overloads

		#region Enqueue<T1, T2, TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2)
		{
			return Enqueue<T1, T2, TResult>(func, param1, param2, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2, int timeout)
		{
			return Enqueue<T1, T2, TResult>(func, param1, param2, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, TResult>(func, param1, param2, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, TResult>(func, param1, param2, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, TResult>(func, param1, param2, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, TResult>(func, param1, param2, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, TResult>(func, param1, param2, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, TResult> Overloads

		#region Enqueue<T1, T2, T3, TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3)
		{
			return Enqueue<T1, T2, T3, TResult>(func, param1, param2, param3, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3, int timeout)
		{
			return Enqueue<T1, T2, T3, TResult>(func, param1, param2, param3, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, TResult>(func, param1, param2, param3, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, TResult>(func, param1, param2, param3, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, TResult>(func, param1, param2, param3, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, TResult>(func, param1, param2, param3, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, TResult>(func, param1, param2, param3, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, TResult> Overloads

		#region Enqueue<T1, T2, T3, T4, TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			return Enqueue<T1, T2, T3, T4, TResult>(func, param1, param2, param3, param4, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, TResult>(func, param1, param2, param3, param4, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, TResult>(func, param1, param2, param3, param4, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, TResult>(func, param1, param2, param3, param4, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, TResult>(func, param1, param2, param3, param4, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, TResult>(func, param1, param2, param3, param4, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, TResult>(func, param1, param2, param3, param4, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, TResult> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
		{
			return Enqueue<T1, T2, T3, T4, T5, TResult>(func, param1, param2, param3, param4, param5, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, TResult>(func, param1, param2, param3, param4, param5, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, TResult>(func, param1, param2, param3, param4, param5, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, TResult>(func, param1, param2, param3, param4, param5, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, TResult>(func, param1, param2, param3, param4, param5, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, TResult>(func, param1, param2, param3, param4, param5, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, TResult>(func, param1, param2, param3, param4, param5, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, TResult> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, TResult>(func, param1, param2, param3, param4, param5, param6, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, TResult>(func, param1, param2, param3, param4, param5, param6, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, TResult>(func, param1, param2, param3, param4, param5, param6, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, TResult>(func, param1, param2, param3, param4, param5, param6, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, TResult>(func, param1, param2, param3, param4, param5, param6, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, TResult>(func, param1, param2, param3, param4, param5, param6, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, TResult>(func, param1, param2, param3, param4, param5, param6, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, TResult> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult>(func, param1, param2, param3, param4, param5, param6, param7, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult>(func, param1, param2, param3, param4, param5, param6, param7, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult>(func, param1, param2, param3, param4, param5, param6, param7, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult>(func, param1, param2, param3, param4, param5, param6, param7, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult>(func, param1, param2, param3, param4, param5, param6, param7, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult>(func, param1, param2, param3, param4, param5, param6, param7, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult>(func, param1, param2, param3, param4, param5, param6, param7, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, TResult> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9, param10 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> Overloads

		#endregion Func Overloads

		#region Action Overloads

		#region Enqueue Overloads

		public IChoAbortableAsyncResult Enqueue(Action func)
		{
			return Enqueue(func, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue(Action func, int timeout)
		{
			return Enqueue(func, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue(Action func, int timeout, int maxNoOfRetry)
		{
			return Enqueue(func, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue(Action func, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue(func, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue(Action func, ChoAsyncCallback callback, object state)
		{
			return Enqueue(func, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue(Action func, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue(func, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue(Action func, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue(func, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue(Action func, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, (object[])null, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue Overloads

		#region Enqueue<T> Overloads

		public IChoAbortableAsyncResult Enqueue<T>(Action<T> func, T param)
		{
			return Enqueue<T>(func, param, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T>(Action<T> func, T param, int timeout)
		{
			return Enqueue<T>(func, param, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T>(Action<T> func, T param, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T>(func, param, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T>(Action<T> func, T param, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T>(func, param, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T>(Action<T> func, T param, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T>(func, param, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T>(Action<T> func, T param, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T>(func, param, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T>(Action<T> func, T param, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T>(func, param, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T>(Action<T> func, T param, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T> Overloads

		#region Enqueue<T1, T2> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2)
		{
			return Enqueue<T1, T2>(func, param1, param2, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2, int timeout)
		{
			return Enqueue<T1, T2>(func, param1, param2, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2>(func, param1, param2, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2>(func, param1, param2, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2>(func, param1, param2, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2>(func, param1, param2, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2>(func, param1, param2, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2> Overloads

		#region Enqueue<T1, T2, T3> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3)
		{
			return Enqueue<T1, T2, T3>(func, param1, param2, param3, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3, int timeout)
		{
			return Enqueue<T1, T2, T3>(func, param1, param2, param3, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3>(func, param1, param2, param3, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3>(func, param1, param2, param3, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3>(func, param1, param2, param3, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3>(func, param1, param2, param3, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3>(func, param1, param2, param3, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3> Overloads

		#region Enqueue<T1, T2, T3, T4> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			return Enqueue<T1, T2, T3, T4>(func, param1, param2, param3, param4, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4, int timeout)
		{
			return Enqueue<T1, T2, T3, T4>(func, param1, param2, param3, param4, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4>(func, param1, param2, param3, param4, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4>(func, param1, param2, param3, param4, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4>(func, param1, param2, param3, param4, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4>(func, param1, param2, param3, param4, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4>(func, param1, param2, param3, param4, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4> Overloads

		#region Enqueue<T1, T2, T3, T4, T5> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
		{
			return Enqueue<T1, T2, T3, T4, T5>(func, param1, param2, param3, param4, param5, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5>(func, param1, param2, param3, param4, param5, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5>(func, param1, param2, param3, param4, param5, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5>(func, param1, param2, param3, param4, param5, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5>(func, param1, param2, param3, param4, param5, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5>(func, param1, param2, param3, param4, param5, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5>(func, param1, param2, param3, param4, param5, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6>(func, param1, param2, param3, param4, param5, param6, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6>(func, param1, param2, param3, param4, param5, param6, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6>(func, param1, param2, param3, param4, param5, param6, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6>(func, param1, param2, param3, param4, param5, param6, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6>(func, param1, param2, param3, param4, param5, param6, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6>(func, param1, param2, param3, param4, param5, param6, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6>(func, param1, param2, param3, param4, param5, param6, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7>(func, param1, param2, param3, param4, param5, param6, param7, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7>(func, param1, param2, param3, param4, param5, param6, param7, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7>(func, param1, param2, param3, param4, param5, param6, param7, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7>(func, param1, param2, param3, param4, param5, param6, param7, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7>(func, param1, param2, param3, param4, param5, param6, param7, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7>(func, param1, param2, param3, param4, param5, param6, param7, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7>(func, param1, param2, param3, param4, param5, param6, param7, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(func, param1, param2, param3, param4, param5, param6, param7, param8, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(func, param1, param2, param3, param4, param5, param6, param7, param8, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(func, param1, param2, param3, param4, param5, param6, param7, param8, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(func, param1, param2, param3, param4, param5, param6, param7, param8, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(func, param1, param2, param3, param4, param5, param6, param7, param8, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(func, param1, param2, param3, param4, param5, param6, param7, param8, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(func, param1, param2, param3, param4, param5, param6, param7, param8, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9, param10 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Overloads

		#region Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Overloads

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16, ChoAsyncCallback callback, object state)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16, ChoAsyncCallback callback, object state, int timeout)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(func, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16 }, callback, state, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		#endregion Enqueue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Overloads

		#endregion Action Overloads

		#region EnqueueMethod Overloads

		public IChoAbortableAsyncResult EnqueueMethod(Delegate func, object[] parameters)
		{
			return EnqueueMethod(func, parameters, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult EnqueueMethod(Delegate func, object[] parameters, int timeout)
		{
			return EnqueueMethod(func, parameters, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult EnqueueMethod(Delegate func, object[] parameters, int timeout, int maxNoOfRetry)
		{
			return EnqueueMethod(func, parameters, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public IChoAbortableAsyncResult EnqueueMethod(Delegate func, object[] parameters, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			return EnqueueMethod(func, parameters, null, null, timeout, maxNoOfRetry, sleepBetweenRetry);
		}

		public IChoAbortableAsyncResult EnqueueMethod(Delegate func, object[] parameters, ChoAsyncCallback callback, object state)
		{
			return EnqueueMethod(func, parameters, callback, state, Timeout.Infinite);
		}

		public IChoAbortableAsyncResult EnqueueMethod(Delegate func, object[] parameters, ChoAsyncCallback callback, object state, int timeout)
		{
			return EnqueueMethod(func, parameters, callback, state, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public IChoAbortableAsyncResult EnqueueMethod(Delegate func, object[] parameters, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry)
		{
			return EnqueueMethod(func, parameters, callback, state, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		#endregion EnqueueMethod Overloads

		#endregion Enqueue Overloads
	}
}
