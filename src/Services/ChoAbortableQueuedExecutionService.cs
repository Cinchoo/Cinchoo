namespace Cinchoo.Core.Services
{
    #region NameSpaces

    using System;
    using System.Threading;

    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    #endregion NameSpaces

    [ChoAppDomainEventsRegisterableType]
    [ChoBufferProfile(ChoString.Empty, NameFromTypeFullName = typeof(ChoAbortableQueuedExecutionService))]
    public class ChoAbortableQueuedExecutionService : ChoAbortableExecutionServiceBase
    {
        //#region Global QueuedExecutionService

        //private static ChoAbortableQueuedExecutionService _globalAbortableQueuedExecutionService = new ChoAbortableQueuedExecutionService(ChoGlobalApplicationSettings.Me.ApplicationName, true);

        //#endregion Global QueuedExecutionService

        #region Global QueuedExecutionService

        private readonly static ChoDictionary<string, ChoAbortableQueuedExecutionService> _globalQueuedExecutionServices = ChoDictionary<string, ChoAbortableQueuedExecutionService>.Synchronized(new ChoDictionary<string, ChoAbortableQueuedExecutionService>());

        #endregion Global QueuedExecutionService

        #region Instance Data Members (Private)

        private readonly ChoQueuedMsgService<ChoExecutionServiceData> _queuedMsgService;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoAbortableQueuedExecutionService(string name)
            : this(name, true)
        {
        }

        public ChoAbortableQueuedExecutionService(string name, bool autoStart)
            : base(name)
        {
            _queuedMsgService = new ChoQueuedMsgService<ChoExecutionServiceData>("{0}_{1}".FormatString(name, typeof(ChoAbortableQueuedExecutionService).Name),
                ChoStandardQueuedMsgObject<ChoExecutionServiceData>.QuitMsg, autoStart, true, QueueMessageHandler);
        }

        #endregion Constructors

        #region Enqueue Overloads

        public override IChoAbortableAsyncResult EnqueueMethod(Delegate func, object[] parameters, ChoAbortableAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            ChoGuard.ArgumentNotNull(func, "Function");
            CheckTimeoutArg(timeout);

            ChoExecutionServiceData data = new ChoExecutionServiceData(func, parameters, timeout, new ChoAbortableAsyncResult(callback, state), maxNoOfRetry, sleepBetweenRetry);
            _queuedMsgService.Enqueue(ChoStandardQueuedMsgObject<ChoExecutionServiceData>.New(data));

            return data.Result as IChoAbortableAsyncResult;
        }

        #endregion Enqueue Overloads

        #region Instance Members (Public)

        public override void Start()
        {
            ChoGuard.NotDisposed(this);
            _queuedMsgService.Start();
        }

        public override void Stop()
        {
            ChoGuard.NotDisposed(this);
            //Console.WriteLine(String.Format("Stopping {0} asynchronous execution service...", Name));
            _queuedMsgService.Dispose();
        }

        #endregion Instance Members (Public)

        private int _timeout = 60000;
        public int Timeout
        {
            get { return _timeout; }
            set
            {
                if (_timeout > 0 && _timeout != System.Threading.Timeout.Infinite)
                    _timeout = value;
            }
        }

        private int _maxNoOfRetry;
        public int MaxNoOfRetry
        {
            get { return _maxNoOfRetry; }
            set
            {
                if (_maxNoOfRetry > 0)
                    _maxNoOfRetry = value;
            }
        }

        private int _sleepBetweenRetry;
        public int SleepBetweenRetry
        {
            get { return _sleepBetweenRetry; }
            set
            {
                if (_sleepBetweenRetry > 0)
                    _sleepBetweenRetry = value;
            }
        }

        #region Shared Members (Private)

        private void QueueMessageHandler(IChoQueuedMsgServiceObject<ChoExecutionServiceData> msgObject)
        {
            if (msgObject == null || !ChoGuard.IsArgumentNotNullOrEmpty(msgObject.State))
                return;

            ChoAbortableAsyncResult asyncResult = msgObject.State.Result as ChoAbortableAsyncResult;
            if (msgObject.State.Result != null)
            {
                if (msgObject.State.Result is ChoAbortableAsyncResult && ((ChoAbortableAsyncResult)msgObject.State.Result).AbortRequested)
                {
                    asyncResult.SetAsAborted(true);
                    return;
                }
            }
            WaitFor(msgObject.State.Func, msgObject.State.Parameters, msgObject.State.Timeout, msgObject.State.MaxNoOfRetry, msgObject.State.SleepBetweenRetry, msgObject.State);
        }

        private void WaitFor(Delegate func, object[] parameters, int timeout, int maxNoOfRetry, int sleepBetweenRetry, ChoExecutionServiceData data)
        {
            ChoGuard.ArgumentNotNull(func, "Function");

            ChoAbortableAsyncResult asyncResult = data.Result as ChoAbortableAsyncResult;

            if (_timeout > 0)
                timeout = timeout > _timeout ? _timeout : timeout;
            if (_maxNoOfRetry > 0)
                maxNoOfRetry = maxNoOfRetry > _maxNoOfRetry ? _maxNoOfRetry : maxNoOfRetry;
            if (_sleepBetweenRetry > 0)
                sleepBetweenRetry = sleepBetweenRetry > _sleepBetweenRetry ? _sleepBetweenRetry : sleepBetweenRetry;

            if (maxNoOfRetry > 0 && sleepBetweenRetry <= 0)
                sleepBetweenRetry = 1000;

            //ChoWaitFor.CheckParams(timeout, maxNoOfRetry, sleepBetweenRetry);

            int noOfRetry = 0;
            object retValue = null;
            AutoResetEvent _event = new AutoResetEvent(false);
            ChoList<Exception> aggExceptions = new ChoList<Exception>();
            Func<object> wrappedFunc = delegate
            {
                if (asyncResult.AbortRequested)
                {
                    //asyncResult.SetAsAborted(true);
                    return null;
                }

                asyncResult.ThreadToKill = Thread.CurrentThread;
                _event.Set();
                return func.DynamicInvoke(parameters);
            };

            while (true)
            {
                if (asyncResult.AbortRequested)
                {
                    //asyncResult.SetAsAborted(true);
                    break;
                }

                try
                {
                    if (timeout == System.Threading.Timeout.Infinite)
                        retValue = wrappedFunc.Invoke();
                    else
                    {
                        _event.Reset();
                        IAsyncResult result = wrappedFunc.BeginInvoke(null, null);
                        _event.WaitOne();
                        //Thread.Sleep(1000);
                        //using (result.AsyncWaitHandle)
                        //{
                        if (!result.AsyncWaitHandle.WaitOne(timeout, true))
                        {
                            if (asyncResult.ThreadToKill != null && asyncResult.ThreadToKill.IsAlive)
                            {
                                try
                                {
                                    asyncResult.ThreadToKill.Abort();
                                }
                                catch (ThreadAbortException)
                                {
                                    Thread.ResetAbort();
                                }
                            }
                            throw new ChoTimeoutException(String.Format("The method failed to execute within {0} sec.", timeout));

                            //ChoWaitFor.RaiseTimeoutException(func, timeout);
                        }
                        else
                        {
                            try
                            {
                                retValue = wrappedFunc.EndInvoke(result);
                            }
                            catch (ThreadAbortException)
                            {
                                //Thread.ResetAbort();
                                asyncResult.SetAsAborted(true);
                                return;
                            }
                        }
                        //}
                    }
                    asyncResult.SetAsSuccess(retValue, true);
                    return;
                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                    //ChoTrace.Debug("Thread Aborted.");
                    //ChoTrace.Debug(data.ToString());
                    asyncResult.SetAsAborted(true);
                    break;
                }
                catch (ChoFatalApplicationException fEx)
                {
                    asyncResult.SetAsFailed(fEx, true);
                    throw;
                }
                catch (Exception ex)
                {
                    if (maxNoOfRetry != 0)
                    {
                        if (noOfRetry == maxNoOfRetry)
                        {
                            asyncResult.SetAsFailed(new ChoAggregateException(String.Format("The method failed to execute after {0} retries.", maxNoOfRetry), aggExceptions), true);
                            return;
                        }

                        noOfRetry++;
                        if (ex is TargetInvocationException)
                        {
                            aggExceptions.Add(ex.InnerException);
                            asyncResult.SetAsFailedWithRetry(ex.InnerException, noOfRetry);
                        }
                        else
                        {
                            aggExceptions.Add(ex);
                            asyncResult.SetAsFailedWithRetry(ex, noOfRetry);
                        }

                        Thread.Sleep((int)sleepBetweenRetry);
                    }
                    else
                    {
                        //ChoProfile.WriteLine(ex.ToString());
                        //ChoProfile.WriteLine(data.ToString());
                        if (ex is TargetInvocationException)
                            asyncResult.SetAsFailed(ex.InnerException != null ? ex.InnerException : ex, true);
                        else
                            asyncResult.SetAsFailed(ex, true);

                        return;
                    }
                }
            }
        }
        private static void CheckTimeoutArg(int timeout)
        {
            if (timeout != -1 && timeout < 0)
                throw new ArgumentOutOfRangeException("Timeout");
        }

        #endregion Shared Members (Private)

        #region ChoSyncDisposableObject Overrides

        protected override void Dispose(bool finalize)
        {
            try
            {
                Stop();
            }
            finally
            {
                IsDisposed = true;
            }
        }

        #endregion ChoSyncDisposableObject Overrides

        #region Shared Members (Public)

        public static ChoAbortableQueuedExecutionService GetService(string name)
        {
            ChoAbortableQueuedExecutionService queuedExecutionService = null;
            if (_globalQueuedExecutionServices.TryGetValue(name, out queuedExecutionService))
                return queuedExecutionService;

            lock (_globalQueuedExecutionServices.SyncRoot)
            {
                if (_globalQueuedExecutionServices.TryGetValue(name, out queuedExecutionService))
                    return queuedExecutionService;

                queuedExecutionService = new ChoAbortableQueuedExecutionService(name, true);
                _globalQueuedExecutionServices.Add(name, queuedExecutionService);
                return queuedExecutionService;
            }
        }

        #endregion Shared Members (Public)

        #region Shared Properties (Public)

        public static ChoAbortableQueuedExecutionService Global
        {
            get { return GetService(ChoGlobalApplicationSettings.Me.ApplicationName); }
        }

        #endregion Shared Properties (Public)

        [ChoAppDomainUnloadMethod("Stopping global abortable queued execution service...")]
        private static void StopGlobalQueuedExecutionService()
        {
            //if (_globalAbortableQueuedExecutionService != null)
            //{
            //    _globalAbortableQueuedExecutionService.Dispose();
            //    _globalAbortableQueuedExecutionService = null;
            //}
            lock (_globalQueuedExecutionServices.SyncRoot)
            {
                foreach (string name in _globalQueuedExecutionServices.ToKeysArray())
                {
                    if (ChoTraceSwitch.Switch.TraceVerbose)
                        Trace.WriteLine("Stopping {0} abortable Q execution service...".FormatString(name));

                    try
                    {
                        _globalQueuedExecutionServices[name].Dispose();
                    }
                    catch (ThreadAbortException)
                    {
                        Thread.ResetAbort();
                    }
                    catch (Exception)
                    {
                    }
                }

                _globalQueuedExecutionServices.Clear();
            }
        }
    }
}
