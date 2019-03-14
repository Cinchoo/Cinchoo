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

    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, in T17>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17);

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

    #region ChoActionWaitFor<T1, T2, T3, T4, T5> Class

    public sealed class ChoActionWaitFor<T1, T2, T3, T4, T5> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Action<T1, T2, T3, T4, T5> _action;
        private readonly Action<Thread, T1, T2, T3, T4, T5> _wrappedAction;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoActionWaitFor(Action<T1, T2, T3, T4, T5> action)
            : base(action)
        {
            _action = action;
            _wrappedAction = (@thread, @T1, @T2, @T3, @T4, @T5) =>
            {
                @thread = Thread.CurrentThread;
                _action(@T1, @T2, @T3, @T4, @T5);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Run(arg1, arg2, arg3, arg4, arg5, Timeout.Infinite);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout)
        {
            Run(arg1, arg2, arg3, arg4, arg5, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout, int maxNoOfRetry)
        {
            Run(arg1, arg2, arg3, arg4, arg5, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        _action(arg1, arg2, arg3, arg4, arg5);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, null, null);

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

    #endregion ChoActionWaitFor<T1, T2, T3, T4, T5> Class

    #region ChoActionWaitFor<T1, T2, T3, T4, T5, T6> Class

    public sealed class ChoActionWaitFor<T1, T2, T3, T4, T5, T6> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Action<T1, T2, T3, T4, T5, T6> _action;
        private readonly Action<Thread, T1, T2, T3, T4, T5, T6> _wrappedAction;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoActionWaitFor(Action<T1, T2, T3, T4, T5, T6> action)
            : base(action)
        {
            _action = action;
            _wrappedAction = (@thread, @T1, @T2, @T3, @T4, @T5, @T6) =>
            {
                @thread = Thread.CurrentThread;
                _action(@T1, @T2, @T3, @T4, @T5, @T6);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, Timeout.Infinite);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout, int maxNoOfRetry)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        _action(arg1, arg2, arg3, arg4, arg5, arg6);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, null, null);

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

    #endregion ChoActionWaitFor<T1, T2, T3, T4, T5, T6> Class

    #region ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7> Class

    public sealed class ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Action<T1, T2, T3, T4, T5, T6, T7> _action;
        private readonly Action<Thread, T1, T2, T3, T4, T5, T6, T7> _wrappedAction;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoActionWaitFor(Action<T1, T2, T3, T4, T5, T6, T7> action)
            : base(action)
        {
            _action = action;
            _wrappedAction = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7) =>
            {
                @thread = Thread.CurrentThread;
                _action(@T1, @T2, @T3, @T4, @T5, @T6, @T7);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, Timeout.Infinite);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout, int maxNoOfRetry)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        _action(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, null, null);

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

    #endregion ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7> Class

    #region ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8> Class

    public sealed class ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Action<T1, T2, T3, T4, T5, T6, T7, T8> _action;
        private readonly Action<Thread, T1, T2, T3, T4, T5, T6, T7, T8> _wrappedAction;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoActionWaitFor(Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
            : base(action)
        {
            _action = action;
            _wrappedAction = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8) =>
            {
                @thread = Thread.CurrentThread;
                _action(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, Timeout.Infinite);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout, int maxNoOfRetry)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        _action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, null, null);

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

    #endregion ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8> Class

    #region ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9> Class

    public sealed class ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> _action;
        private readonly Action<Thread, T1, T2, T3, T4, T5, T6, T7, T8, T9> _wrappedAction;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoActionWaitFor(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
            : base(action)
        {
            _action = action;
            _wrappedAction = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9) =>
            {
                @thread = Thread.CurrentThread;
                _action(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, Timeout.Infinite);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout, int maxNoOfRetry)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        _action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, null, null);

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

    #endregion ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9> Class

    #region ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Class

    public sealed class ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> _action;
        private readonly Action<Thread, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> _wrappedAction;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoActionWaitFor(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action)
            : base(action)
        {
            _action = action;
            _wrappedAction = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10) =>
            {
                @thread = Thread.CurrentThread;
                _action(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, Timeout.Infinite);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout, int maxNoOfRetry)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        _action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, null, null);

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

    #endregion ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Class

    #region ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Class

    public sealed class ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> _action;
        private readonly Action<Thread, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> _wrappedAction;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoActionWaitFor(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action)
            : base(action)
        {
            _action = action;
            _wrappedAction = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11) =>
            {
                @thread = Thread.CurrentThread;
                _action(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, Timeout.Infinite);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout, int maxNoOfRetry)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        _action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, null, null);

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

    #endregion ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Class

    #region ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Class

    public sealed class ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> _action;
        private readonly Action<Thread, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> _wrappedAction;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoActionWaitFor(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action)
            : base(action)
        {
            _action = action;
            _wrappedAction = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12) =>
            {
                @thread = Thread.CurrentThread;
                _action(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, Timeout.Infinite);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout, int maxNoOfRetry)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        _action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, null, null);

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

    #endregion ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Class

    #region ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Class

    public sealed class ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> _action;
        private readonly Action<Thread, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> _wrappedAction;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoActionWaitFor(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action)
            : base(action)
        {
            _action = action;
            _wrappedAction = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12, @T13) =>
            {
                @thread = Thread.CurrentThread;
                _action(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12, @T13);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, Timeout.Infinite);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout, int maxNoOfRetry)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        _action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, null, null);

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

    #endregion ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Class

    #region ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Class

    public sealed class ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> _action;
        private readonly Action<Thread, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> _wrappedAction;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoActionWaitFor(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action)
            : base(action)
        {
            _action = action;
            _wrappedAction = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12, @T13, @T14) =>
            {
                @thread = Thread.CurrentThread;
                _action(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12, @T13, @T14);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, Timeout.Infinite);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout, int maxNoOfRetry)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        _action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, null, null);

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

    #endregion ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Class

    #region ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Class

    public sealed class ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> _action;
        private readonly Action<Thread, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> _wrappedAction;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoActionWaitFor(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action)
            : base(action)
        {
            _action = action;
            _wrappedAction = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12, @T13, @T14, @T15) =>
            {
                @thread = Thread.CurrentThread;
                _action(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12, @T13, @T14, @T15);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, Timeout.Infinite);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout, int maxNoOfRetry)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        _action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, null, null);

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

    #endregion ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Class

    #region ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Class

    public sealed class ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> _action;
        private readonly Action<Thread, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> _wrappedAction;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoActionWaitFor(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action)
            : base(action)
        {
            _action = action;
            _wrappedAction = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12, @T13, @T14, @T15, @T16) =>
            {
                @thread = Thread.CurrentThread;
                _action(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12, @T13, @T14, @T15, @T16);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, Timeout.Infinite);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout, int maxNoOfRetry)
        {
            Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        _action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedAction.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, null, null);

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

    #endregion ChoActionWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Class
}
