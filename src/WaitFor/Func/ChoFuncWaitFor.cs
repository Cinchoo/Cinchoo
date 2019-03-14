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

    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, in T17, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17);

	#region ChoFuncWaitFor<TResult> Class

	public sealed class ChoFuncWaitFor<TResult> : ChoBaseWaitFor
	{
		#region Instance Data Members (Private)

		private readonly Func<TResult> _func;
		private readonly Func<Thread, TResult> _wrappedFunc;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoFuncWaitFor(Func<TResult> func) : base(func)
		{
			_func = func;
			_wrappedFunc = (@thread) =>
			{
				@thread = Thread.CurrentThread;
				return _func();
			};
		}

		#endregion Constructors

		#region Run Overloads

		public TResult Run()
		{
			return Run(Timeout.Infinite);
		}

		public TResult Run(int timeout)
		{
			return Run(timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public TResult Run(int timeout, int maxNoOfRetry)
		{
			return Run(timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public TResult Run(int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

			int noOfRetry = 0;
			ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

			while (true)
			{
				try
				{
					if (timeout == Timeout.Infinite)
						return _func();
					else
					{
						Thread threadToKill = null;
						IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, null, null);
						TResult retValue = default(TResult);

						if (!result.AsyncWaitHandle.WaitOne(timeout, true))
						{
							if (threadToKill != null)
								threadToKill.Abort();
							throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
						}
						else
						{
							retValue = _wrappedFunc.EndInvoke(result);
							result.AsyncWaitHandle.Close();
						}

						return retValue;
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

	#endregion ChoFuncWaitFor<TResult> Class

	#region ChoFuncWaitFor<T, TResult> Class

	public sealed class ChoFuncWaitFor<T, TResult> : ChoBaseWaitFor
	{
		#region Instance Data Members (Private)

		private readonly Func<T, TResult> _func;
		private readonly Func<Thread, T, TResult> _wrappedFunc;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoFuncWaitFor(Func<T, TResult> func)
			: base(func)
		{
			_func = func;
			_wrappedFunc = (@thread, @T) =>
			{
				@thread = Thread.CurrentThread;
				return _func(@T);
			};
		}

		#endregion Constructors

		#region Run Overloads

		public TResult Run(T arg)
		{
			return Run(arg, Timeout.Infinite);
		}

		public TResult Run(T arg, int timeout)
		{
			return Run(arg, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public TResult Run(T arg, int timeout, int maxNoOfRetry)
		{
			return Run(arg, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public TResult Run(T arg, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

			int noOfRetry = 0;
			ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

			while (true)
			{
				try
				{
					if (timeout == Timeout.Infinite)
						return _func(arg);
					else
					{
						Thread threadToKill = null;
						IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, arg, null, null);
						TResult retValue = default(TResult);

						if (!result.AsyncWaitHandle.WaitOne(timeout, true))
						{
							if (threadToKill != null)
								threadToKill.Abort();
							throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
						}
						else
						{
							retValue = _wrappedFunc.EndInvoke(result);
							result.AsyncWaitHandle.Close();
						}

						return retValue;
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

	#endregion ChoFuncWaitFor<T, TResult> Class
	
	#region ChoFuncWaitFor<T1, T2, TResult> Class

	public sealed class ChoFuncWaitFor<T1, T2, TResult> : ChoBaseWaitFor
	{
		#region Instance Data Members (Private)

		private readonly Func<T1, T2, TResult> _func;
		private readonly Func<Thread, T1, T2, TResult> _wrappedFunc;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoFuncWaitFor(Func<T1, T2, TResult> func)
			: base(func)
		{
			_func = func;
			_wrappedFunc = (@thread, @T1, @T2) =>
			{
				@thread = Thread.CurrentThread;
				return _func(@T1, @T2);
			};
		}

		#endregion Constructors

		#region Run Overloads

		public TResult Run(T1 arg1, T2 arg2)
		{
			return Run(arg1, arg2, Timeout.Infinite);
		}

		public TResult Run(T1 arg1, T2 arg2, int timeout)
		{
			return Run(arg1, arg2, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public TResult Run(T1 arg1, T2 arg2, int timeout, int maxNoOfRetry)
		{
			return Run(arg1, arg2, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public TResult Run(T1 arg1, T2 arg2, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

			int noOfRetry = 0;
			ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

			while (true)
			{
				try
				{
					if (timeout == Timeout.Infinite)
						return _func(arg1, arg2);
					else
					{
						Thread threadToKill = null;
						IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, arg1, arg2, null, null);
						TResult retValue = default(TResult);

						if (!result.AsyncWaitHandle.WaitOne(timeout, true))
						{
							if (threadToKill != null)
								threadToKill.Abort();
							throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
						}
						else
						{
							retValue = _wrappedFunc.EndInvoke(result);
							result.AsyncWaitHandle.Close();
						}

						return retValue;
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

	#endregion ChoFuncWaitFor<T1, T2, TResult> Class

	#region ChoFuncWaitFor<T1, T2, T3, TResult> Class

	public sealed class ChoFuncWaitFor<T1, T2, T3, TResult> : ChoBaseWaitFor
	{
		#region Instance Data Members (Private)

		private readonly Func<T1, T2, T3, TResult> _func;
		private readonly Func<Thread, T1, T2, T3, TResult> _wrappedFunc;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoFuncWaitFor(Func<T1, T2, T3, TResult> func)
			: base(func)
		{
			_func = func;
			_wrappedFunc = (@thread, @T1, @T2, @T3) =>
			{
				@thread = Thread.CurrentThread;
				return _func(@T1, @T2, @T3);
			};
		}

		#endregion Constructors

		#region Run Overloads

		public TResult Run(T1 arg1, T2 arg2, T3 arg3)
		{
			return Run(arg1, arg2, arg3, Timeout.Infinite);
		}

		public TResult Run(T1 arg1, T2 arg2, T3 arg3, int timeout)
		{
			return Run(arg1, arg2, arg3, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public TResult Run(T1 arg1, T2 arg2, T3 arg3, int timeout, int maxNoOfRetry)
		{
			return Run(arg1, arg2, arg3, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public TResult Run(T1 arg1, T2 arg2, T3 arg3, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

			int noOfRetry = 0;
			ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

			while (true)
			{
				try
				{
					if (timeout == Timeout.Infinite)
						return _func(arg1, arg2, arg3);
					else
					{
						Thread threadToKill = null;
						IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, arg1, arg2, arg3, null, null);
						TResult retValue = default(TResult);

						if (!result.AsyncWaitHandle.WaitOne(timeout, true))
						{
							if (threadToKill != null)
								threadToKill.Abort();
							throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
						}
						else
						{
							retValue = _wrappedFunc.EndInvoke(result);
							result.AsyncWaitHandle.Close();
						}

						return retValue;
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

	#endregion ChoFuncWaitFor<T1, T2, T3, TResult> Class

	#region ChoFuncWaitFor<T1, T2, T3, T4, TResult> Class

	public sealed class ChoFuncWaitFor<T1, T2, T3, T4, TResult> : ChoBaseWaitFor
	{
		#region Instance Data Members (Private)

		private readonly Func<T1, T2, T3, T4, TResult> _func;
		private readonly Func<Thread, T1, T2, T3, T4, TResult> _wrappedFunc;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoFuncWaitFor(Func<T1, T2, T3, T4, TResult> func)
			: base(func)
		{
			_func = func;
			_wrappedFunc = (@thread, @T1, @T2, @T3, @T4) =>
			{
				@thread = Thread.CurrentThread;
				return _func(@T1, @T2, @T3, @T4);
			};
		}

		#endregion Constructors

		#region Run Overloads

		public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			return Run(arg1, arg2, arg3, arg4, Timeout.Infinite);
		}

		public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout)
		{
			return Run(arg1, arg2, arg3, arg4, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
		}

		public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout, int maxNoOfRetry)
		{
			return Run(arg1, arg2, arg3, arg4, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
		}

		public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
		{
			CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

			int noOfRetry = 0;
			ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

			while (true)
			{
				try
				{
					if (timeout == Timeout.Infinite)
						return _func(arg1, arg2, arg3, arg4);
					else
					{
						Thread threadToKill = null;
						IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, null, null);
						TResult retValue = default(TResult);

						if (!result.AsyncWaitHandle.WaitOne(timeout, true))
						{
							if (threadToKill != null)
								threadToKill.Abort();
							throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
						}
						else
						{
							retValue = _wrappedFunc.EndInvoke(result);
							result.AsyncWaitHandle.Close();
						}

						return retValue;
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

	#endregion ChoFuncWaitFor<T1, T2, T3, T4, TResult> Class

    #region ChoFuncWaitFor<T1, T2, T3, T4, T5, TResult> Class

    public sealed class ChoFuncWaitFor<T1, T2, T3, T4, T5, TResult> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Func<T1, T2, T3, T4, T5, TResult> _func;
        private readonly Func<Thread, T1, T2, T3, T4, T5, TResult> _wrappedFunc;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoFuncWaitFor(Func<T1, T2, T3, T4, T5, TResult> func)
            : base(func)
        {
            _func = func;
            _wrappedFunc = (@thread, @T1, @T2, @T3, @T4, @T5) =>
            {
                @thread = Thread.CurrentThread;
                return _func(@T1, @T2, @T3, @T4, @T5);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, Timeout.Infinite);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout, int maxNoOfRetry)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        return _func(arg1, arg2, arg3, arg4, arg5);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, null, null);
                        TResult retValue = default(TResult);

                        if (!result.AsyncWaitHandle.WaitOne(timeout, true))
                        {
                            if (threadToKill != null)
                                threadToKill.Abort();
                            throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
                        }
                        else
                        {
                            retValue = _wrappedFunc.EndInvoke(result);
                            result.AsyncWaitHandle.Close();
                        }

                        return retValue;
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

    #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, TResult> Class

    #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, TResult> Class

    public sealed class ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, TResult> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Func<T1, T2, T3, T4, T5, T6, TResult> _func;
        private readonly Func<Thread, T1, T2, T3, T4, T5, T6, TResult> _wrappedFunc;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoFuncWaitFor(Func<T1, T2, T3, T4, T5, T6, TResult> func)
            : base(func)
        {
            _func = func;
            _wrappedFunc = (@thread, @T1, @T2, @T3, @T4, @T5, @T6) =>
            {
                @thread = Thread.CurrentThread;
                return _func(@T1, @T2, @T3, @T4, @T5, @T6);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, Timeout.Infinite);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout, int maxNoOfRetry)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        return _func(arg1, arg2, arg3, arg4, arg5, arg6);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, null, null);
                        TResult retValue = default(TResult);

                        if (!result.AsyncWaitHandle.WaitOne(timeout, true))
                        {
                            if (threadToKill != null)
                                threadToKill.Abort();
                            throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
                        }
                        else
                        {
                            retValue = _wrappedFunc.EndInvoke(result);
                            result.AsyncWaitHandle.Close();
                        }

                        return retValue;
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

    #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, TResult> Class

    #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, TResult> Class

    public sealed class ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, TResult> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, TResult> _func;
        private readonly Func<Thread, T1, T2, T3, T4, T5, T6, T7, TResult> _wrappedFunc;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoFuncWaitFor(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func)
            : base(func)
        {
            _func = func;
            _wrappedFunc = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7) =>
            {
                @thread = Thread.CurrentThread;
                return _func(@T1, @T2, @T3, @T4, @T5, @T6, @T7);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, Timeout.Infinite);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout, int maxNoOfRetry)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        return _func(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, null, null);
                        TResult retValue = default(TResult);

                        if (!result.AsyncWaitHandle.WaitOne(timeout, true))
                        {
                            if (threadToKill != null)
                                threadToKill.Abort();
                            throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
                        }
                        else
                        {
                            retValue = _wrappedFunc.EndInvoke(result);
                            result.AsyncWaitHandle.Close();
                        }

                        return retValue;
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

    #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, TResult> Class

    #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Class

    public sealed class ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, TResult> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> _func;
        private readonly Func<Thread, T1, T2, T3, T4, T5, T6, T7, T8, TResult> _wrappedFunc;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoFuncWaitFor(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func)
            : base(func)
        {
            _func = func;
            _wrappedFunc = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8) =>
            {
                @thread = Thread.CurrentThread;
                return _func(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, Timeout.Infinite);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout, int maxNoOfRetry)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        return _func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, null, null);
                        TResult retValue = default(TResult);

                        if (!result.AsyncWaitHandle.WaitOne(timeout, true))
                        {
                            if (threadToKill != null)
                                threadToKill.Abort();
                            throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
                        }
                        else
                        {
                            retValue = _wrappedFunc.EndInvoke(result);
                            result.AsyncWaitHandle.Close();
                        }

                        return retValue;
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

    #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Class

    #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> Class

    public sealed class ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> _func;
        private readonly Func<Thread, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> _wrappedFunc;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoFuncWaitFor(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func)
            : base(func)
        {
            _func = func;
            _wrappedFunc = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9) =>
            {
                @thread = Thread.CurrentThread;
                return _func(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, Timeout.Infinite);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout, int maxNoOfRetry)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        return _func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, null, null);
                        TResult retValue = default(TResult);

                        if (!result.AsyncWaitHandle.WaitOne(timeout, true))
                        {
                            if (threadToKill != null)
                                threadToKill.Abort();
                            throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
                        }
                        else
                        {
                            retValue = _wrappedFunc.EndInvoke(result);
                            result.AsyncWaitHandle.Close();
                        }

                        return retValue;
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

    #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> Class

    #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> Class

    public sealed class ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> _func;
        private readonly Func<Thread, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> _wrappedFunc;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoFuncWaitFor(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func)
            : base(func)
        {
            _func = func;
            _wrappedFunc = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10) =>
            {
                @thread = Thread.CurrentThread;
                return _func(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, Timeout.Infinite);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout, int maxNoOfRetry)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        return _func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, null, null);
                        TResult retValue = default(TResult);

                        if (!result.AsyncWaitHandle.WaitOne(timeout, true))
                        {
                            if (threadToKill != null)
                                threadToKill.Abort();
                            throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
                        }
                        else
                        {
                            retValue = _wrappedFunc.EndInvoke(result);
                            result.AsyncWaitHandle.Close();
                        }

                        return retValue;
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

    #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> Class

    #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> Class

    public sealed class ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> _func;
        private readonly Func<Thread, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> _wrappedFunc;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoFuncWaitFor(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func)
            : base(func)
        {
            _func = func;
            _wrappedFunc = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11) =>
            {
                @thread = Thread.CurrentThread;
                return _func(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, Timeout.Infinite);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout, int maxNoOfRetry)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        return _func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, null, null);
                        TResult retValue = default(TResult);

                        if (!result.AsyncWaitHandle.WaitOne(timeout, true))
                        {
                            if (threadToKill != null)
                                threadToKill.Abort();
                            throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
                        }
                        else
                        {
                            retValue = _wrappedFunc.EndInvoke(result);
                            result.AsyncWaitHandle.Close();
                        }

                        return retValue;
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

    #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> Class

    #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> Class

    public sealed class ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> _func;
        private readonly Func<Thread, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> _wrappedFunc;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoFuncWaitFor(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func)
            : base(func)
        {
            _func = func;
            _wrappedFunc = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12) =>
            {
                @thread = Thread.CurrentThread;
                return _func(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, Timeout.Infinite);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout, int maxNoOfRetry)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        return _func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, null, null);
                        TResult retValue = default(TResult);

                        if (!result.AsyncWaitHandle.WaitOne(timeout, true))
                        {
                            if (threadToKill != null)
                                threadToKill.Abort();
                            throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
                        }
                        else
                        {
                            retValue = _wrappedFunc.EndInvoke(result);
                            result.AsyncWaitHandle.Close();
                        }

                        return retValue;
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

    #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> Class

    #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> Class

    public sealed class ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> _func;
        private readonly Func<Thread, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> _wrappedFunc;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoFuncWaitFor(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func)
            : base(func)
        {
            _func = func;
            _wrappedFunc = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12, @T13) =>
            {
                @thread = Thread.CurrentThread;
                return _func(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12, @T13);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, Timeout.Infinite);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout, int maxNoOfRetry)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        return _func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, null, null);
                        TResult retValue = default(TResult);

                        if (!result.AsyncWaitHandle.WaitOne(timeout, true))
                        {
                            if (threadToKill != null)
                                threadToKill.Abort();
                            throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
                        }
                        else
                        {
                            retValue = _wrappedFunc.EndInvoke(result);
                            result.AsyncWaitHandle.Close();
                        }

                        return retValue;
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

    #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> Class

    #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> Class

    public sealed class ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> _func;
        private readonly Func<Thread, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> _wrappedFunc;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoFuncWaitFor(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func)
            : base(func)
        {
            _func = func;
            _wrappedFunc = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12, @T13, @T14) =>
            {
                @thread = Thread.CurrentThread;
                return _func(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12, @T13, @T14);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, Timeout.Infinite);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout, int maxNoOfRetry)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        return _func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, null, null);
                        TResult retValue = default(TResult);

                        if (!result.AsyncWaitHandle.WaitOne(timeout, true))
                        {
                            if (threadToKill != null)
                                threadToKill.Abort();
                            throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
                        }
                        else
                        {
                            retValue = _wrappedFunc.EndInvoke(result);
                            result.AsyncWaitHandle.Close();
                        }

                        return retValue;
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

    #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> Class

    #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> Class

    public sealed class ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> _func;
        private readonly Func<Thread, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> _wrappedFunc;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoFuncWaitFor(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func)
            : base(func)
        {
            _func = func;
            _wrappedFunc = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12, @T13, @T14, @T15) =>
            {
                @thread = Thread.CurrentThread;
                return _func(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12, @T13, @T14, @T15);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, Timeout.Infinite);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout, int maxNoOfRetry)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        return _func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, null, null);
                        TResult retValue = default(TResult);

                        if (!result.AsyncWaitHandle.WaitOne(timeout, true))
                        {
                            if (threadToKill != null)
                                threadToKill.Abort();
                            throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
                        }
                        else
                        {
                            retValue = _wrappedFunc.EndInvoke(result);
                            result.AsyncWaitHandle.Close();
                        }

                        return retValue;
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

    #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> Class

    #region ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> Class

    public sealed class ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> _func;
        private readonly Func<Thread, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> _wrappedFunc;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoFuncWaitFor(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func)
            : base(func)
        {
            _func = func;
            _wrappedFunc = (@thread, @T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12, @T13, @T14, @T15, @T16) =>
            {
                @thread = Thread.CurrentThread;
                return _func(@T1, @T2, @T3, @T4, @T5, @T6, @T7, @T8, @T9, @T10, @T11, @T12, @T13, @T14, @T15, @T16);
            };
        }

        #endregion Constructors

        #region Run Overloads

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, Timeout.Infinite);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout, int maxNoOfRetry)
        {
            return Run(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public TResult Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        return _func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
                    else
                    {
                        Thread threadToKill = null;
                        IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, null, null);
                        TResult retValue = default(TResult);

                        if (!result.AsyncWaitHandle.WaitOne(timeout, true))
                        {
                            if (threadToKill != null)
                                threadToKill.Abort();
                            throw new TimeoutException(String.Format("Timeout [{0}] elapsed prior to completion of the method [{1}].", timeout, FuncSignature));
                        }
                        else
                        {
                            retValue = _wrappedFunc.EndInvoke(result);
                            result.AsyncWaitHandle.Close();
                        }

                        return retValue;
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

    #endregion ChoFuncWaitFor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> Class
}
