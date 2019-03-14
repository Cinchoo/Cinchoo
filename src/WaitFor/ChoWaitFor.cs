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

    #region ChoWaitFor Class

    public sealed class ChoWaitFor : ChoBaseWaitFor
    {
        #region Instance Data Members (Private)

        private readonly Delegate _func;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoWaitFor(Delegate func)
            : base(func)
        {
            _func = func;
        }

        #endregion Constructors

        #region Run Overloads

        public object Run(object[] args)
        {
            return Run(args, Timeout.Infinite);
        }

        public object Run(object[] args, int timeout)
        {
            return Run(args, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public object Run(object[] args, int timeout, int maxNoOfRetry)
        {
            return Run(args, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public object Run(object[] args, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            ChoList<Exception> aggExceptions = maxNoOfRetry != 0 ? new ChoList<Exception>() : null;

            while (true)
            {
                try
                {
                    if (timeout == Timeout.Infinite)
                        return _func.DynamicInvoke(args);
                    else
                    {
                        Thread threadToKill = null;
                        Func<Thread, object> _wrappedFunc = (@thread) =>
                        {
                            @thread = Thread.CurrentThread;
                            return _func.DynamicInvoke(args);
                        };
                        IAsyncResult result = _wrappedFunc.BeginInvoke(threadToKill, null, null);
                        object retValue = null;

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
}
