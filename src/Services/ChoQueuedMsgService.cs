namespace Cinchoo.Core.Services
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Threading;
    using System.Reflection;
    using System.Diagnostics;
    using System.Collections.Generic;

    using Cinchoo.Core.Collections;
    using Cinchoo.Core.Diagnostics;

    #endregion NameSpaces

    //TODO: Need to run the queued items in timely manner
    //TODO: Throttle mechanism
    [DebuggerDisplay("Name = {_name}, HashCode = { GetHashCode() }")]
    public class ChoQueuedMsgService<T> : ChoSyncDisposableObject
    {
        #region Delegates

        public delegate void ChoQMessageHandler(IChoQueuedMsgServiceObject<T> msgObject);

        #endregion Delegates

        #region Instance Data Members (Private)

        private readonly string _name;
        private readonly IChoQueuedMsgServiceObject<T> _shutdownMsg;
        private readonly object _padLock = new object();
        private readonly ChoQueue _queue = ChoQueue.BlockingQueue(new ChoQueue());
        //private readonly ChoPriorityQueue _queue = ChoPriorityQueue.BlockingQueue(new ChoPriorityQueue());
        private readonly ChoQMessageHandler _queueMessageHandler;
        private bool _stoppingService = false;
        private readonly bool _autoShutdown = false;
        private Thread _queueProcessingThread;

        #endregion Instance Data Members (Private)

        private int _maxSize = 10000;
        public int MaxSize
        {
            get { return _maxSize; }
            set
            {
                if (_maxSize > 0)
                    _maxSize = value;
            }
        }

        #region Constructors

        public ChoQueuedMsgService(string name, IChoQueuedMsgServiceObject<T> shutdownMsg)
            : this(name, shutdownMsg, true, false, null)
        {
        }

        public ChoQueuedMsgService(string name, IChoQueuedMsgServiceObject<T> shutdownMsg, bool autoStart, bool autoShutdown)
            : this(name, shutdownMsg, autoStart, autoShutdown, null)
        {
        }

        public ChoQueuedMsgService(string name, IChoQueuedMsgServiceObject<T> shutdownMsg, bool autoStart, bool autoShutdown, ChoQMessageHandler queueMessageHandler)
        {
            ChoGuard.ArgumentNotNull(name, "Name");
            ChoGuard.ArgumentNotNull(shutdownMsg, "ShutdownMsg");

            _name = name;
            _shutdownMsg = shutdownMsg;
            _queueMessageHandler = queueMessageHandler;
            _autoShutdown = autoShutdown;
            if (autoStart)
                Start();
        }

        #endregion Constructors

        #region Instance Members (Public)

        public virtual void Start()
        {
            ChoGuard.NotDisposed(this);

            if (_queueProcessingThread != null && _queueProcessingThread.IsAlive)
                return;

            lock (_padLock)
            {
                if (_queueProcessingThread != null && _queueProcessingThread.IsAlive) return;

                try
                {
                    _queueProcessingThread = new Thread(new ParameterizedThreadStart(QueueProcessingThreadCallback));
                    _queueProcessingThread.Name = "{0}_{1}Thread".FormatString(_name, this.GetType().Name);
                    _queueProcessingThread.IsBackground = _autoShutdown;
                    _queueProcessingThread.Start();
                }
                catch
                {
                    _queueProcessingThread = null;
                    throw;
                }
            }
        }

        public virtual void Stop()
        {
            Stop(false);
        }

        public virtual void Enqueue(IChoQueuedMsgServiceObject<T> msgQObject)
        {
            if (_stoppingService)
                return;

            ChoGuard.NotDisposed(this);
            CheckState();

            if (_queue.Count >= _maxSize)
                return;

            //_queue.Enqueue(0, msgQObject);
            _queue.Enqueue(msgQObject);
        }

        #endregion Instance Members (Public)

        #region Instance Members (Protected)

        protected virtual void QueueMessageHandler(IChoQueuedMsgServiceObject<T> msgObject)
        {
            if (_queueMessageHandler != null)
                _queueMessageHandler(msgObject);
            else
                throw new NotImplementedException();
        }

        #endregion Instance Members (Protected)

        #region Instance Members (Private)

        private void Stop(bool silent)
        {
            if (ChoGuard.IsDisposed(this)) return;

            _stoppingService = true;

            //if (!silent)
            //    CheckState();

            lock (_padLock)
            {
                if (_queueProcessingThread != null || _queue.Count > 0)
                {
                    //_queue.Enqueue(0, _shutdownMsg);
                    _queue.Enqueue(_shutdownMsg);
                    int noOfRetry = 0;
                    while (true)
                    {
                        //Enqueue(_endOfMsg);
                        if (ChoTraceSwitch.Switch.TraceVerbose)
                            Trace.WriteLine("{0}: Stopping thread...".FormatString(_name));

                        if (_queueProcessingThread == null || !_queueProcessingThread.IsAlive || _queueProcessingThread.Join(1000))
                        {
                            if (ChoTraceSwitch.Switch.TraceVerbose)
                                Trace.WriteLine("{0}: Stopped thread...".FormatString(_name));

                            _queueProcessingThread = null;
                            //_stoppingService = false;
                            break;
                        }
                        else
                            Thread.Sleep(10);

                        noOfRetry++;
                        if (noOfRetry >= 5)
                        {
                            Trace.WriteLine("{0}: Aborting thread...".FormatString(_name));

                            try
                            {
                                _queueProcessingThread.AbortThread();
                            }
                            catch (Exception ex)
                            {
                                Trace.WriteLine(ex.ToString());
                            }

                            break;
                        }
                    }
                    if (_queueProcessingThread != null /*&& !_queueProcessingThread.IsAlive */)
                    {
                        _queueProcessingThread = null;
                    }
                }
            }
        }

        private void CheckState()
        {
            lock (_padLock)
            {
                if (_stoppingService)
                    throw new ChoQueuedMsgServiceException("Queued Message Service is in middle of stopping the service.");
                //else if (_queueProcessingThread == null)
                //    throw new ChoQueuedMsgServiceException("Queued Message Service is not running.");
            }
        }

        private void QueueProcessingThreadCallback(object state)
        {
            IChoQueuedMsgServiceObject<T> msgQObject = null;
            while (true)
            {
                try
                {
                    msgQObject = _queue.Dequeue() as IChoQueuedMsgServiceObject<T>;
                    if (msgQObject == null) continue;
                    if (msgQObject.IsQuitServiceMsg) break;

                    QueueMessageHandler(msgQObject);
                }
                catch (ChoFatalApplicationException fex)
                {
                    ChoEnvironment.Exit(-1, fex);
                }
                catch (Exception ex)
                {
                    ChoApplication.WriteToEventLog(ex.ToString(), EventLogEntryType.Error);
                    if (msgQObject != null)
                        ChoApplication.WriteToEventLog(ChoObject.ToString(msgQObject));
                    //ChoProfile.DefaultContext.Append(ex);
                    //if (!EventLog.SourceExists(ChoAssembly.GetEntryAssembly().GetName().Name))
                    //    EventLog.CreateEventSource(ChoAssembly.GetEntryAssembly().GetName().Name, "Application");
                    //EventLog.WriteEntry(ChoAssembly.GetEntryAssembly().GetName().Name, ChoApplicationException.ToString(ex));
                    //throw;
                }
            }
        }

        #endregion Instance Members (Private)

        #region ChoDisposableObject Overrides

        protected override void Dispose(bool finalize)
        {
            try
            {
                if (_queueProcessingThread != null)
                    Stop(true);
                else if (ChoTraceSwitch.Switch.TraceVerbose)
                    Trace.WriteLine("{0}: Thread not started.".FormatString(_name));
            }
            finally
            {
                IsDisposed = true;
            }
        }

        #endregion ChoDisposableObject Overrides
    }
}
