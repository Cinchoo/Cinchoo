namespace Cinchoo.Core.Services
{
    #region NameSpaces

    using System;
    using System.Diagnostics;
    using System.Threading;

    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.Reflection;

    #endregion NameSpaces

    [ChoAppDomainEventsRegisterableType]
    public class ChoQueuedExecutionService : ChoExecutionServiceBase
    {
        #region Global QueuedExecutionService

        private readonly static ChoDictionary<string, ChoQueuedExecutionService> _globalQueuedExecutionServices = ChoDictionary<string, ChoQueuedExecutionService>.Synchronized(new ChoDictionary<string, ChoQueuedExecutionService>());

        #endregion Global QueuedExecutionService

        #region Instance Data Members (Private)

        private readonly ChoQueuedMsgService<ChoExecutionServiceData> _queuedMsgService;

        #endregion Instance Data Members (Private)

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

        #region Constructors

        public ChoQueuedExecutionService(string name)
            : this(name, true)
        {
        }

        public ChoQueuedExecutionService(string name, bool autoStart)
            : base(name)
        {
            _queuedMsgService = new ChoQueuedMsgService<ChoExecutionServiceData>("{0}_{1}".FormatString(name, typeof(ChoQueuedExecutionService).Name),
                ChoStandardQueuedMsgObject<ChoExecutionServiceData>.QuitMsg, autoStart, true, QueueMessageHandler);
        }

        #endregion Constructors

        #region Enqueue Overloads

        public override IChoAsyncResult EnqueueMethod(Delegate func, object[] parameters, ChoAsyncCallback callback, object state, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            ChoGuard.ArgumentNotNull(func, "Function");
            CheckTimeoutArg(timeout);

            if (ChoGuard.IsDisposed(this))
                return null;

            timeout = timeout > _timeout ? _timeout : timeout;
            maxNoOfRetry = maxNoOfRetry > _maxNoOfRetry ? _maxNoOfRetry : maxNoOfRetry;
            sleepBetweenRetry = sleepBetweenRetry > _sleepBetweenRetry ? _sleepBetweenRetry : sleepBetweenRetry;

            ChoExecutionServiceData data = new ChoExecutionServiceData(func, parameters, timeout, new ChoAsyncResult(callback, state), maxNoOfRetry, sleepBetweenRetry);
            _queuedMsgService.Enqueue(ChoStandardQueuedMsgObject<ChoExecutionServiceData>.New(data));

            return data.Result;
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
            _queuedMsgService.Dispose();
        }

        #endregion Instance Members (Public)

        #region Shared Members (Private)

        private void QueueMessageHandler(IChoQueuedMsgServiceObject<ChoExecutionServiceData> msgObject)
        {
            if (msgObject == null
                || !ChoGuard.IsArgumentNotNullOrEmpty(msgObject.State)
                )
                return;

            ChoAsyncResult asyncResult = msgObject.State.Result as ChoAsyncResult;
            try
            {
                object retValue = msgObject.State.Func.Run(msgObject.State.Parameters, msgObject.State.Timeout, msgObject.State.MaxNoOfRetry, msgObject.State.SleepBetweenRetry);
                asyncResult.SetAsSuccess(retValue, true);
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
                ChoTrace.Error("Thread aborted." + msgObject.State.ToString());
                asyncResult.SetAsAborted(true);
            }
            catch (ChoFatalApplicationException fex)
            {
                ChoTrace.Error(fex);
                ChoTrace.Error(msgObject.State.ToString());
                asyncResult.SetAsFailed(fex, true);
                ChoEnvironment.Exit(-1, fex);
            }
            catch (Exception ex)
            {
                ChoTrace.Error(ex);
                ChoTrace.Error(msgObject.State.ToString());
                asyncResult.SetAsFailed(ex, true);
            }
        }

        private static void CheckTimeoutArg(int timeout)
        {
            if (timeout != -1 && timeout < 0)
                throw new ArgumentOutOfRangeException("Timeout");
        }

        #endregion Shared Members (Private)

        #region Shared Members (Public)

        public static ChoQueuedExecutionService GetService(string name)
        {
            ChoQueuedExecutionService queuedExecutionService = null;
            if (_globalQueuedExecutionServices.TryGetValue(name, out queuedExecutionService))
                return queuedExecutionService;

            lock (_globalQueuedExecutionServices.SyncRoot)
            {
                if (_globalQueuedExecutionServices.TryGetValue(name, out queuedExecutionService))
                    return queuedExecutionService;

                queuedExecutionService = new ChoQueuedExecutionService(name, true);
                _globalQueuedExecutionServices.Add(name, queuedExecutionService);
                return queuedExecutionService;
            }
        }

        #endregion Shared Members (Public)

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

        #region Shared Properties (Public)

        public static ChoQueuedExecutionService Global
        {
            get { return GetService(ChoGlobalApplicationSettings.Me.ApplicationName); }
        }

        #endregion Shared Properties (Public)

        [ChoAppDomainUnloadMethod("Stopping global queued execution services...")]
        private static void StopGlobalQueuedExecutionService()
        {
            lock (_globalQueuedExecutionServices.SyncRoot)
            {
                foreach (string name in _globalQueuedExecutionServices.ToKeysArray())
                {
                    if (ChoTraceSwitch.Switch.TraceVerbose)
                        Trace.WriteLine("Stopping {0} Q execution service...".FormatString(name));

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
