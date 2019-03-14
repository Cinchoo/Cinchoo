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

	//public delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

	#region ChoActionWaitFor Class

	public sealed class ChoActionWaitFor : ChoBaseWaitFor
	{
		#region Instance Data Members (Private)

		private readonly Action _action;
		private readonly Action<Thread> _wrappedAction;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoActionWaitFor(Action action)
			: base(action)
		{
			_action = action;
			_wrappedAction = (@thread) =>
			{
				@thread = Thread.CurrentThread;
				_action();
			};
		}

		#endregion Constructors

		#region Run Overloads

		public void Run()
		{
			Run(Timeout.Infinite);
		}

		public void Run(int timeout)
		{
			Run(timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public void Run(int timeout, int maxNoOfRetry)
		{
			Run(timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public void Run(int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

			int noOfRetry = 0;
			ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

			while (true)
			{
				try
				{
					if (timeout == Timeout.Infinite)
						_action();
					else
					{
						Thread threadToKill = null;
						IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, null, null);

						if (!result.AsyncWaitHandle.WaitOne(timeout, true))
						{
							if (threadToKill != null)
								threadToKill.Abort();
							throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
						}
						else
						{
							_wrappedAction.EndInvoke(result);
							result.AsyncWaitHandle.Close();
						}
					}
				}
				catch (Exception ex)
				{
					if (maxNoOfRetry != 0)
						noOfRetry = HandleException(maxNoOfRetry, sleepBetweenRetry, noOfRetry, aggExceptions, ex);
					else
						throw;
				}
			}
		}

		#endregion Run Overloads
	}

	#endregion ChoActionWaitFor Class

	#region ChoActionWaitFor<T> Class

	public sealed class ChoActionWaitFor<T> : ChoBaseWaitFor
	{
		#region Instance Data Members (Private)

		private readonly Action<T> _action;
		private readonly Action<Thread, T> _wrappedAction;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoActionWaitFor(Action<T> action)
			: base(action)
		{
			_action = action;
			_wrappedAction = (@thread, @T) =>
			{
				@thread = Thread.CurrentThread;
				_action(@T);
			};
		}

		#endregion Constructors

		#region Run Overloads

		public void Run(T arg)
		{
			Run(arg, Timeout.Infinite);
		}

		public void Run(T arg, int timeout)
		{
			Run(arg, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

        public void Run(T arg, int timeout, int maxNoOfRetry)
		{
			Run(arg, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public void Run(T arg, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

			int noOfRetry = 0;
			ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

			while (true)
			{
				try
				{
					if (timeout == Timeout.Infinite)
						_action(arg);
					else
					{
						Thread threadToKill = null;
						IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, arg, null, null);

						if (!result.AsyncWaitHandle.WaitOne(timeout, true))
						{
							if (threadToKill != null)
								threadToKill.Abort();
							throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
						}
						else
						{
							_wrappedAction.EndInvoke(result);
							result.AsyncWaitHandle.Close();
						}
					}
				}
				catch (Exception ex)
				{
					if (maxNoOfRetry != 0)
						noOfRetry = HandleException(maxNoOfRetry, sleepBetweenRetry, noOfRetry, aggExceptions, ex);
					else
						throw;
				}
			}
		}

		#endregion Run Overloads
	}

	#endregion ChoActionWaitFor<T> Class

	#region ChoActionWaitFor<T1, T2> Class

	public sealed class ChoActionWaitFor<T1, T2> : ChoBaseWaitFor
	{
		#region Instance Data Members (Private)

		private readonly Action<T1, T2> _action;
		private readonly Action<Thread, T1, T2> _wrappedAction;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoActionWaitFor(Action<T1, T2> action)
			: base(action)
		{
			_action = action;
			_wrappedAction = (@thread, @T1, @T2) =>
			{
				@thread = Thread.CurrentThread;
				_action(@T1, @T2);
			};
		}

		#endregion Constructors

		#region Run Overloads

		public void Run(T1 arg1, T2 arg2)
		{
			Run(arg1, arg2, Timeout.Infinite);
		}

		public void Run(T1 arg1, T2 arg2, int timeout)
		{
			Run(arg1, arg2, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public void Run(T1 arg1, T2 arg2, int timeout, int maxNoOfRetry)
		{
			Run(arg1, arg2, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public void Run(T1 arg1, T2 arg2, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

			int noOfRetry = 0;
			ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

			while (true)
			{
				try
				{
					if (timeout == Timeout.Infinite)
						_action(arg1, arg2);
					else
					{
						Thread threadToKill = null;
						IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, arg1, arg2, null, null);

						if (!result.AsyncWaitHandle.WaitOne(timeout, true))
						{
							if (threadToKill != null)
								threadToKill.Abort();
							throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
						}
						else
						{
							_wrappedAction.EndInvoke(result);
							result.AsyncWaitHandle.Close();
						}
					}
				}
				catch (Exception ex)
				{
					if (maxNoOfRetry != 0)
						noOfRetry = HandleException(maxNoOfRetry, sleepBetweenRetry, noOfRetry, aggExceptions, ex);
					else
						throw;
				}
			}
		}

		#endregion Run Overloads
	}

	#endregion ChoActionWaitFor<T1, T2> Class

	#region ChoActionWaitFor<T1, T2, T3> Class

	public sealed class ChoActionWaitFor<T1, T2, T3> : ChoBaseWaitFor
	{
		#region Instance Data Members (Private)

		private readonly Action<T1, T2, T3> _action;
		private readonly Action<Thread, T1, T2, T3> _wrappedAction;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoActionWaitFor(Action<T1, T2, T3> action)
			: base(action)
		{
			_action = action;
			_wrappedAction = (@thread, @T1, @T2, @T3) =>
			{
				@thread = Thread.CurrentThread;
				_action(@T1, @T2, @T3);
			};
		}

		#endregion Constructors

		#region Run Overloads

		public void Run(T1 arg1, T2 arg2, T3 arg3)
		{
			Run(arg1, arg2, arg3, Timeout.Infinite);
		}

		public void Run(T1 arg1, T2 arg2, T3 arg3, int timeout)
		{
			Run(arg1, arg2, arg3, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public void Run(T1 arg1, T2 arg2, T3 arg3, int timeout, int maxNoOfRetry)
		{
			Run(arg1, arg2, arg3, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public void Run(T1 arg1, T2 arg2, T3 arg3, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

			int noOfRetry = 0;
			ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

			while (true)
			{
				try
				{
					if (timeout == Timeout.Infinite)
						_action(arg1, arg2, arg3);
					else
					{
						Thread threadToKill = null;
						IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, arg1, arg2, arg3, null, null);

						if (!result.AsyncWaitHandle.WaitOne(timeout, true))
						{
							if (threadToKill != null)
								threadToKill.Abort();
							throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
						}
						else
						{
							_wrappedAction.EndInvoke(result);
							result.AsyncWaitHandle.Close();
						}
					}
				}
				catch (Exception ex)
				{
					if (maxNoOfRetry != 0)
						noOfRetry = HandleException(maxNoOfRetry, sleepBetweenRetry, noOfRetry, aggExceptions, ex);
					else
						throw;
				}
			}
		}

		#endregion Run Overloads
	}

	#endregion ChoActionWaitFor<T1, T2, T3> Class

	#region ChoActionWaitFor<T1, T2, T3, T4> Class

	public sealed class ChoActionWaitFor<T1, T2, T3, T4> : ChoBaseWaitFor
	{
		#region Instance Data Members (Private)

		private readonly Action<T1, T2, T3, T4> _action;
		private readonly Action<Thread, T1, T2, T3, T4> _wrappedAction;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoActionWaitFor(Action<T1, T2, T3, T4> action)
			: base(action)
		{
			_action = action;
			_wrappedAction = (@thread, @T1, @T2, @T3, @T4) =>
			{
				@thread = Thread.CurrentThread;
				_action(@T1, @T2, @T3, @T4);
			};
		}

		#endregion Constructors

		#region Run Overloads

		public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			Run(arg1, arg2, arg3, arg4, Timeout.Infinite);
		}

		public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout)
		{
			Run(arg1, arg2, arg3, arg4, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout, int maxNoOfRetry)
		{
			Run(arg1, arg2, arg3, arg4, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

			int noOfRetry = 0;
			ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

			while (true)
			{
				try
				{
					if (timeout == Timeout.Infinite)
						_action(arg1, arg2, arg3, arg4);
					else
					{
						Thread threadToKill = null;
						IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, null, null);

						if (!result.AsyncWaitHandle.WaitOne(timeout, true))
						{
							if (threadToKill != null)
								threadToKill.Abort();
							throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
						}
						else
						{
							_wrappedAction.EndInvoke(result);
							result.AsyncWaitHandle.Close();
						}
					}
				}
				catch (Exception ex)
				{
					if (maxNoOfRetry != 0)
						noOfRetry = HandleException(maxNoOfRetry, sleepBetweenRetry, noOfRetry, aggExceptions, ex);
					else
						throw;
				}
			}
		}

		#endregion Run Overloads
	}

	#endregion ChoActionWaitFor<T1, T2, T3, T4> Class
}
