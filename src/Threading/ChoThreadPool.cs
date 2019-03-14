namespace Cinchoo.Core.Threading
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Security;
    using System.Threading;
    using System.Reflection;
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.Security.Principal;
    using System.Runtime.Remoting.Contexts;

    using Cinchoo.Core.Text;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.Collections;

    #endregion NameSpaces

    #region ChoCallerThreadContext class

    /// <summary>
    /// This class stores the caller thread context in order to restore
    /// it when the work item is executed in the context of the thread 
    /// from the pool.
    /// Note that we can't store the thread's CompressedStack, because 
    /// it throws a security exception
    /// </summary>
    public class ChoCallerThreadContext
    {
        private CultureInfo _culture = null;
        private CultureInfo _cultureUI = null;
        private IPrincipal _principal;
        private Context _context;

        private static FieldInfo _fieldInfo = GetFieldInfo();

        private static FieldInfo GetFieldInfo()
        {
            Type threadType = typeof(Thread);
            return threadType.GetField(
                "_context",
                BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        private ChoCallerThreadContext()
        {
        }

        /// <summary>
        /// Captures the current thread context
        /// </summary>
        /// <returns></returns>
        public static ChoCallerThreadContext Capture()
        {
            ChoCallerThreadContext callerThreadContext = new ChoCallerThreadContext();

            Thread thread = Thread.CurrentThread;
            callerThreadContext._culture = thread.CurrentCulture;
            callerThreadContext._cultureUI = thread.CurrentUICulture;
            callerThreadContext._principal = Thread.CurrentPrincipal;
            callerThreadContext._context = Thread.CurrentContext;
            return callerThreadContext;
        }

        /// <summary>
        /// Applies the thread context stored earlier
        /// </summary>
        /// <param name="callerThreadContext"></param>
        public static void Apply(ChoCallerThreadContext callerThreadContext)
        {
            Thread thread = Thread.CurrentThread;
            thread.CurrentCulture = callerThreadContext._culture;
            thread.CurrentUICulture = callerThreadContext._cultureUI;
            Thread.CurrentPrincipal = callerThreadContext._principal;

            // Uncomment the following block to enable the Thread.CurrentThread
            /*
                        if (null != _fieldInfo)
                        {
                            _fieldInfo.SetValue(
                                Thread.CurrentThread, 
                                callerThreadContext._context);
                        }
            */
        }
    }

    #endregion

    public sealed class ChoThreadPool : IDisposable
    {
        #region Instance Data Members (Public)

        private string _name; // = ChoAppSettings.ApplicationId;
        private int _maxWorkerThreads = ChoThreadPoolSettings.MaxWorkerThreads;
        private int _minWorkerThreads = ChoThreadPoolSettings.MinWorkerThreads;
        private int _idleTimeout = ChoThreadPoolSettings.IdleTimeout;
        private bool _disposeOfStateObjects = ChoThreadPoolSettings.DisposeOfStateObjects;
        private bool _blockIfPoolBusy = ChoThreadPoolSettings.BlockIfPoolBusy;
        private bool _useCallerContext = ChoThreadPoolSettings.UseCallerContext;
        private CallToPostExecute _callToPostExecute = ChoThreadPoolSettings.CallToPostExecute;
        private PostExecuteWorkItemCallback _postExecuteWorkItemCallback = ChoThreadPoolSettings.PostExecuteWorkItemCallback;
        private bool _canQueueWorkItems = ChoQueueSettings.TurnOn;

        /// <summary>
        /// Queue of work items.
        /// </summary>
        private ChoWorkItemsQueue _workItemsQueue = new ChoWorkItemsQueue();

        private ChoQueue _workItemsStateQueue = ChoQueue.Synchronized(new ChoQueue(ChoQueueSettings.MaxCapacity, ChoQueueSettings.StorageDirectory));

        /// <summary>
        /// Hashtable of all the threads in the thread pool.
        /// </summary>
        private Hashtable _workerThreads = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// A flag to indicate the threads to quit.
        /// </summary>
        private bool _shutdown = false;

        /// <summary>
        /// A flag to indicate all threads should complete before quit.
        /// </summary>
        private bool _waitOnDispose = false;

        /// <summary>
        /// Number of threads that currently work (not idle).
        /// </summary>
        private int _inUseWorkerThreads = 0;

        /// <summary>
        /// Counts the threads created in the pool.
        /// It is used to name the threads.
        /// </summary>
        private int _threadCounter = 0;

        /// <summary>
        /// Indicate that the SmartThreadPool has been disposed
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Total number of work items that are stored in the work items queue 
        /// plus the work items that the threads in the pool are working on.
        /// </summary>
        private int _currentWorkItemsCount = 0;

        /// <summary>
        /// Signaled when the thread pool is idle, i.e. no thread is busy
        /// and the work items queue is empty
        /// </summary>
        private ManualResetEvent _isIdleWaitHandle = new ManualResetEvent(true);

        /// <summary>
        /// Signaled when the thread pool reach down to the MinWorkerThreads level
        /// </summary>
        private ManualResetEvent _canAcceptWaitHandle = new ManualResetEvent(true);

        /// <summary>
        /// Signaled when the thread pool reach up to the MaxWorkerThreads level
        /// </summary>
        private ManualResetEvent _tooBusyWaitHandle = new ManualResetEvent(true);

        /// <summary>
        /// An event to signal all the threads to quit immediately.
        /// </summary>
        private ManualResetEvent _shuttingDownEvent = new ManualResetEvent(false);

        #endregion

        #region Constructors (Public)

        public ChoThreadPool(string name)
        {
            _name = name;
            Start();
        }

        public ChoThreadPool(string name, bool waitOnDispose)
            : this(name)
        {
            _waitOnDispose = waitOnDispose;
        }

        public ChoThreadPool(string name, int idleTimeout)
        {
            _name = name;
            _idleTimeout = idleTimeout;

            Start();
        }

        public ChoThreadPool(string name, int idleTimeout, bool waitOnDispose)
            : this(name, idleTimeout)
        {
            _waitOnDispose = waitOnDispose;
        }

        public ChoThreadPool(string name, int idleTimeout, int maxWorkerThreads)
        {
            _name = name;
            _idleTimeout = idleTimeout;
            _maxWorkerThreads = maxWorkerThreads;

            Start();
        }

        public ChoThreadPool(string name, int idleTimeout, int maxWorkerThreads, int minWorkerThreads)
        {
            _name = name;
            _idleTimeout = idleTimeout;
            _maxWorkerThreads = maxWorkerThreads;
            _minWorkerThreads = minWorkerThreads;

            Start();
        }

        public ChoThreadPool(string name, int idleTimeout, int maxWorkerThreads, int minWorkerThreads, bool canQueueWorkItems)
            : this(name, idleTimeout, maxWorkerThreads, minWorkerThreads)
        {
            _canQueueWorkItems = canQueueWorkItems;
        }


        #endregion

        #region Instance Members (Private)

        private void Start()
        {
            if (_minWorkerThreads >= _maxWorkerThreads)
                throw new ChoThreadPoolException("MinWorkerThread should be less than MaxWorkerThreads.");

			//ChoStreamProfile.WriteLine(ChoReservedDirectoryName.Settings, String.Format("{0}_{1}", Name, GetType().Name), ToString(), true);
            ChoTrace.WriteLine(String.Format("Starting `{2}` Threadpool with Min ({0}) - Max ({1}) threads.",
                _minWorkerThreads, _maxWorkerThreads, _name));

            StartThreads(_minWorkerThreads);
        }

        /// <summary>
        /// Starts new threads
        /// </summary>
        /// <param name="threadsCount">The number of threads to start</param>
        private void StartThreads(int threadsCount)
        {
            lock (_workerThreads.SyncRoot)
            {
                // Don't start threads on shut down
                if (_shutdown) return;

                for (int i = 0; i < threadsCount; ++i)
                {
                    // Don't create more threads then the upper limit
                    if (_workerThreads.Count >= _maxWorkerThreads)
                        return;

                    // Create a new thread
                    Thread workerThread = new Thread(new ThreadStart(ProcessQueuedItems));

                    // Configure the new thread and start it
                    workerThread.Name = "RITThread #" + _workerThreads.Count;
                    //ChoTrace.WriteLine(String.Format("Creating '{0}' thread ({1} >= {2})...", workerThread.Name, 
                    //	_workerThreads.Count, _maxWorkerThreads));
                    workerThread.IsBackground = true;
                    workerThread.Start();
                    ++_threadCounter;

                    // Add the new thread to the hashtable and update its creation
                    // time.
                    _workerThreads[workerThread] = DateTime.Now;

                    //ChoTrace.WriteLine(String.Format("{0} Threads in Pool.", _workerThreads.Count));
                }
            }
        }

        /// <summary>
        /// A worker thread method that processes work items from the work items queue.
        /// </summary>
        private void ProcessQueuedItems()
        {
            try
            {
                bool bInUseWorkerThreadsWasIncremented = false;

                // Process until shutdown.
                while (!_shutdown)
                {
                    // Update the last time this thread was seen alive.
                    // It's good for debugging.
                    _workerThreads[Thread.CurrentThread] = DateTime.Now;

                    // Wait for a work item, shutdown, or timeout
                    WorkItem workItem = Dequeue();

                    // Update the last time this thread was seen alive.
                    // It's good for debugging.
                    _workerThreads[Thread.CurrentThread] = DateTime.Now;

                    // On timeout or shut down.
                    if (null == workItem)
                    {
                        // Double lock for quit.
                        if (_workerThreads.Count > _minWorkerThreads)
                        {
                            lock (_workerThreads.SyncRoot)
                            {
                                if (_workerThreads.Count > _minWorkerThreads)
                                {
                                    // Inform that the thread is quiting and then quit.
                                    // This method must be called within this lock or else
                                    // more threads will quit and the thread pool will go
                                    // below the lower limit.
                                    InformCompleted();
                                    break;
                                }
                            }
                        }
                    }

                    // If we didn't quit then skip to the next iteration.
                    if (null == workItem)
                    {
                        continue;
                    }

                    try
                    {
                        // Initialize the value to false
                        bInUseWorkerThreadsWasIncremented = false;

                        // Change the state of the work item to 'in progress' if possible.
                        // We do it here so if the work item has been canceled we won't 
                        // increment the _inUseWorkerThreads.
                        // The cancel mechanism doesn't delete items from the queue,  
                        // it marks the work item as canceled, and when the work item
                        // is dequeued, we just skip it.
                        // If the post execute of work item is set to always or to
                        // call when the work item is canceled then the StartingWorkItem()
                        // will return true, so the post execute can run.
                        if (!workItem.StartingWorkItem())
                        {
                            continue;
                        }

                        // Execute the callback.  Make sure to accurately
                        // record how many callbacks are currently executing.
                        Interlocked.Increment(ref _inUseWorkerThreads);

                        // Mark that the _inUseWorkerThreads incremented, so in the finally{}
                        // statement we will decrement it correctly.
                        bInUseWorkerThreadsWasIncremented = true;

                        workItem.Execute();
                    }
                    finally
                    {
                        if (_disposeOfStateObjects)
                        {
                            workItem.Dispose();
                        }

                        // Decrement the _inUseWorkerThreads only if we had 
                        // incremented it. Note the cancelled work items don't
                        // increment _inUseWorkerThreads.
                        if (bInUseWorkerThreadsWasIncremented)
                        {
                            Interlocked.Decrement(ref _inUseWorkerThreads);
                        }

                        DecrementWorkItemsCount();
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // Handle the abort exception gracfully.
                Thread.ResetAbort();
            }
            catch (Exception e)
            {
                Debug.Assert(null != e);
            }
            finally
            {
                InformCompleted();
            }
        }

        #endregion

        #region ThreadPool functions (Public)

        /// <summary>
        /// Queues a user work item to the thread pool.
        /// </summary>
        /// <param name="callback">
        /// A WaitCallback representing the delegate to invoke when the thread in the 
        /// thread pool picks up the work item.
        /// </param>
        public IWorkItemResult QueueWorkItem(WorkItemCallback callback)
        {
            // Queue the delegate with no state
            return QueueWorkItem(callback, null);
        }

        /// <summary>
        /// Queues a user work item to the thread pool.
        /// </summary>
        /// <param name="callback">
        /// A WaitCallback representing the delegate to invoke when the thread in the 
        /// thread pool picks up the work item.
        /// </param>
        /// <param name="state">
        /// The object that is passed to the delegate when serviced from the thread pool.
        /// </param>
        public IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state)
        {
            ValidateNotDisposed();
            // Create a work item that contains the delegate and its state.
            WorkItem workItem = new WorkItem(
                callback,
                state,
                _useCallerContext,
                _postExecuteWorkItemCallback,
                _callToPostExecute);
            Enqueue(workItem);
            return workItem.GetWorkItemResult();
        }

        /// <summary>
        /// Queues a user work item to the thread pool.
        /// </summary>
        /// <param name="callback">
        /// A WaitCallback representing the delegate to invoke when the thread in the 
        /// thread pool picks up the work item.
        /// </param>
        /// <param name="state">
        /// The object that is passed to the delegate when serviced from the thread pool.
        /// </param>
        /// <param name="postExecuteWorkItemCallback">
        /// A delegate to call after the callback completion
        /// </param>
        /// <returns>Work item result</returns>
        public IWorkItemResult QueueWorkItem(
            WorkItemCallback callback,
            object state,
            PostExecuteWorkItemCallback postExecuteWorkItemCallback)
        {
            ValidateNotDisposed();

            // Create a work item that contains the delegate and its state.
            WorkItem workItem = new WorkItem(
                callback,
                state,
                _useCallerContext,
                postExecuteWorkItemCallback,
                _callToPostExecute);
            Enqueue(workItem);
            return workItem.GetWorkItemResult();
        }

        /// <summary>
        /// Queues a user work item to the thread pool.
        /// </summary>
        /// <param name="callback">
        /// A WaitCallback representing the delegate to invoke when the thread in the 
        /// thread pool picks up the work item.
        /// </param>
        /// <param name="state">
        /// The object that is passed to the delegate when serviced from the thread pool.
        /// </param>
        /// <param name="postExecuteWorkItemCallback">
        /// A delegate to call after the callback completion
        /// </param>
        /// <param name="callToPostExecute">Indicates on which cases to call to the post execute callback</param>
        /// <returns>Work item result</returns>
        public IWorkItemResult QueueWorkItem(
            WorkItemCallback callback,
            object state,
            PostExecuteWorkItemCallback postExecuteWorkItemCallback,
            CallToPostExecute callToPostExecute)
        {
            ValidateNotDisposed();

            // Create a work item that contains the delegate and its state.
            WorkItem workItem = new WorkItem(
                callback,
                state,
                _useCallerContext,
                postExecuteWorkItemCallback,
                callToPostExecute);
            Enqueue(workItem);
            return workItem.GetWorkItemResult();
        }

        #endregion ThreadPool functions (Public)

        #region Thread Processing functions (Private)

        /// <summary>
        /// Waits on the queue for a work item, shutdown, or timeout.
        /// </summary>
        /// <returns>
        /// Returns the WaitingCallback or null in case of timeout or shutdown.
        /// </returns>
        private WorkItem Dequeue()
        {
            WorkItem workItem =
                _workItemsQueue.DequeueWorkItem(_idleTimeout, _shuttingDownEvent);

            if (workItem != null && CanQueueWorkItems)
            {
                workItem.State = _workItemsStateQueue.Dequeue();
            }

            return workItem;
        }

        /// <summary>
        /// Put a new work item in the queue
        /// </summary>
        /// <param name="workItem">A work item to queue</param>
        private void Enqueue(WorkItem workItem)
        {
            if (!CanQueueWorkItems)
                WaitForRelax();

            // Make sure the workItem is not null
            Debug.Assert(null != workItem);

            IncrementWorkItemsCount();

            if (workItem != null && CanQueueWorkItems)
            {
                _workItemsStateQueue.Enqueue(workItem.State);
                workItem.State = null;
            }

            _workItemsQueue.EnqueueWorkItem(workItem);

            // If all the threads are busy then try to create a new one
            if ((InUseThreads + WaitingCallbacks) > _workerThreads.Count)
                StartThreads(1);
        }

        #endregion

        #region Other Helper functions (Private)

        private void IncrementWorkItemsCount()
        {
            int count = Interlocked.Increment(ref _currentWorkItemsCount);
            if (count == 1)
            {
                _isIdleWaitHandle.Reset();
            }
            if (count == _minWorkerThreads + 1)
            {
                _canAcceptWaitHandle.Reset();
            }
            if (_blockIfPoolBusy && InUseThreads >= _maxWorkerThreads - 1)
                _tooBusyWaitHandle.Reset();

            //			if (count >= _maxWorkerThreads)
            //			{
            //				_tooBusyWaitHandle.Reset();
            //			}
        }

        private void DecrementWorkItemsCount()
        {
            int count = Interlocked.Decrement(ref _currentWorkItemsCount);
            if (count == 0)
            {
                _isIdleWaitHandle.Set();
            }
            if (count == _minWorkerThreads)
            {
                //ChoTrace.WriteLine(String.Format("{0} : Can accept now.", count));
                _canAcceptWaitHandle.Set();
            }
            if (_blockIfPoolBusy && InUseThreads < _maxWorkerThreads - 1)
                _tooBusyWaitHandle.Set();

            //			if (count < _maxWorkerThreads)
            //			{
            //				_tooBusyWaitHandle.Set();
            //			}
        }

        /// <summary>
        /// Inform that the current thread is about to quit or quiting.
        /// The same thread may call this method more than once.
        /// </summary>
        private void InformCompleted()
        {
            try
            {
                // There is no need to lock the two methods together 
                // since only the current thread removes itself
                // and the _workerThreads is a synchronized hashtable
                if (_workerThreads.Contains(Thread.CurrentThread))
                {
                    _workerThreads.Remove(Thread.CurrentThread);
                }
            }
            catch (ThreadAbortException)
            {
                // Handle the abort exception gracfully.
                Thread.ResetAbort();
            }
        }

        #endregion Other Helper functions (Private)

        #region WaitToAccept functions

        /// <summary>
        /// Wait for the thread pool having MinWorkerThreads limit
        /// </summary>
        public void WaitToAccept()
        {
            WaitToAccept(Timeout.Infinite);
        }

        /// <summary>
        /// Wait for the thread pool having MinWorkerThreads limit
        /// </summary>
        public bool WaitToAccept(TimeSpan timeout)
        {
            return WaitToAccept((int)timeout.TotalMilliseconds);
        }

        /// <summary>
        /// Wait for the thread pool having MinWorkerThreads limit
        /// </summary>
        public bool WaitToAccept(int millisecondsTimeout)
        {
            return _canAcceptWaitHandle.WaitOne(millisecondsTimeout, false);
        }

        #endregion WaitForIdle functions

        #region WaitForIdle functions

        /// <summary>
        /// Wait for the thread pool to be idle
        /// </summary>
        public void WaitForIdle()
        {
            WaitForIdle(Timeout.Infinite);
        }

        /// <summary>
        /// Wait for the thread pool to be idle
        /// </summary>
        public bool WaitForIdle(TimeSpan timeout)
        {
            return WaitForIdle((int)timeout.TotalMilliseconds);
        }

        /// <summary>
        /// Wait for the thread pool to be idle
        /// </summary>
        public bool WaitForIdle(int millisecondsTimeout)
        {
            return _isIdleWaitHandle.WaitOne(millisecondsTimeout, false);
        }

        #endregion WaitForIdle functions

        #region WaitForRelax functions

        /// <summary>
        /// Wait for the thread pool to be idle
        /// </summary>
        public void WaitForRelax()
        {
            WaitForRelax(Timeout.Infinite);
        }

        /// <summary>
        /// Wait for the thread pool to be idle
        /// </summary>
        public bool WaitForRelax(TimeSpan timeout)
        {
            return WaitForRelax((int)timeout.TotalMilliseconds);
        }

        /// <summary>
        /// Wait for the thread pool to be idle
        /// </summary>
        public bool WaitForRelax(int millisecondsTimeout)
        {
            if (_blockIfPoolBusy)
                return _tooBusyWaitHandle.WaitOne(millisecondsTimeout, false);
            else
                return true;
        }

        #endregion WaitForIdle functions

        #region WaitAll functions

        /// <summary>
        /// Wait for all work items to complete
        /// </summary>
        /// <param name="workItemResults">Array of work item result objects</param>
        /// <returns>
        /// true when every work item in workItemResults has completed; otherwise false.
        /// </returns>
        public static bool WaitAll(
            IWorkItemResult[] workItemResults)
        {
            return WaitAll(workItemResults, Timeout.Infinite, true);
        }

        /// <summary>
        /// Wait for all work items to complete
        /// </summary>
        /// <param name="workItemResults">Array of work item result objects</param>
        /// <param name="timeout">The number of milliseconds to wait, or a TimeSpan that represents -1 milliseconds to wait indefinitely. </param>
        /// <param name="exitContext">
        /// true to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it; otherwise, false. 
        /// </param>
        /// <returns>
        /// true when every work item in workItemResults has completed; otherwise false.
        /// </returns>
        public static bool WaitAll(
            IWorkItemResult[] workItemResults,
            TimeSpan timeout,
            bool exitContext)
        {
            return WaitAll(workItemResults, (int)timeout.TotalMilliseconds, exitContext);
        }

        /// <summary>
        /// Wait for all work items to complete
        /// </summary>
        /// <param name="workItemResults">Array of work item result objects</param>
        /// <param name="timeout">The number of milliseconds to wait, or a TimeSpan that represents -1 milliseconds to wait indefinitely. </param>
        /// <param name="exitContext">
        /// true to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it; otherwise, false. 
        /// </param>
        /// <param name="cancelWaitHandle">A cancel wait handle to interrupt the wait if needed</param>
        /// <returns>
        /// true when every work item in workItemResults has completed; otherwise false.
        /// </returns>
        public static bool WaitAll(
            IWorkItemResult[] workItemResults,
            TimeSpan timeout,
            bool exitContext,
            WaitHandle cancelWaitHandle)
        {
            return WaitAll(workItemResults, (int)timeout.TotalMilliseconds, exitContext, cancelWaitHandle);
        }

        /// <summary>
        /// Wait for all work items to complete
        /// </summary>
        /// <param name="workItemResults">Array of work item result objects</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Timeout.Infinite (-1) to wait indefinitely.</param>
        /// <param name="exitContext">
        /// true to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it; otherwise, false. 
        /// </param>
        /// <returns>
        /// true when every work item in workItemResults has completed; otherwise false.
        /// </returns>
        public static bool WaitAll(
            IWorkItemResult[] workItemResults,
            int millisecondsTimeout,
            bool exitContext)
        {
            return WorkItem.WaitAll(workItemResults, millisecondsTimeout, exitContext, null);
        }

        /// <summary>
        /// Wait for all work items to complete
        /// </summary>
        /// <param name="workItemResults">Array of work item result objects</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Timeout.Infinite (-1) to wait indefinitely.</param>
        /// <param name="exitContext">
        /// true to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it; otherwise, false. 
        /// </param>
        /// <param name="cancelWaitHandle">A cancel wait handle to interrupt the wait if needed</param>
        /// <returns>
        /// true when every work item in workItemResults has completed; otherwise false.
        /// </returns>
        public static bool WaitAll(
            IWorkItemResult[] workItemResults,
            int millisecondsTimeout,
            bool exitContext,
            WaitHandle cancelWaitHandle)
        {
            return WorkItem.WaitAll(workItemResults, millisecondsTimeout, exitContext, cancelWaitHandle);
        }

        #endregion WaitAll functions

        #region WaitAny functions

        public static void WaitAllEx(
            IWorkItemResult[] passedWorkItemResults)
        {
            ArrayList workItemResults = new ArrayList(passedWorkItemResults);
            ArrayList pendingWorkItemResults = new ArrayList();

            int prevCount = workItemResults.Count;
            while (workItemResults.Count > 0)
            {
                foreach (IWorkItemResult workItemResult in workItemResults)
                {
                    if (!workItemResult.IsCompleted)
                        pendingWorkItemResults.Add(workItemResult);
                }
                workItemResults = new ArrayList(pendingWorkItemResults);
                pendingWorkItemResults.Clear();

                if (workItemResults.Count == 0) return;
                if (prevCount != workItemResults.Count)
                {
                    prevCount = workItemResults.Count;
                    ChoTrace.WriteLineIf(ChoTrace.ChoSwitch.TraceVerbose, String.Format("Waiting for {0} thread(s) to complete...", workItemResults.Count));
                    Thread.Sleep(1000);
                }
                else
                    Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// Waits for any of the work items in the specified array to complete, cancel, or timeout
        /// </summary>
        /// <param name="workItemResults">Array of work item result objects</param>
        /// <returns>
        /// The array index of the work item result that satisfied the wait, or WaitTimeout if any of the work items has been canceled.
        /// </returns>
        public static int WaitAny(
            IWorkItemResult[] workItemResults)
        {
            return WaitAny(workItemResults, Timeout.Infinite, true);
        }

        /// <summary>
        /// Waits for any of the work items in the specified array to complete, cancel, or timeout
        /// </summary>
        /// <param name="workItemResults">Array of work item result objects</param>
        /// <param name="timeout">The number of milliseconds to wait, or a TimeSpan that represents -1 milliseconds to wait indefinitely. </param>
        /// <param name="exitContext">
        /// true to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it; otherwise, false. 
        /// </param>
        /// <returns>
        /// The array index of the work item result that satisfied the wait, or WaitTimeout if no work item result satisfied the wait and a time interval equivalent to millisecondsTimeout has passed or the work item has been canceled.
        /// </returns>
        public static int WaitAny(
            IWorkItemResult[] workItemResults,
            TimeSpan timeout,
            bool exitContext)
        {
            return WaitAny(workItemResults, (int)timeout.TotalMilliseconds, exitContext);
        }

        /// <summary>
        /// Waits for any of the work items in the specified array to complete, cancel, or timeout
        /// </summary>
        /// <param name="workItemResults">Array of work item result objects</param>
        /// <param name="timeout">The number of milliseconds to wait, or a TimeSpan that represents -1 milliseconds to wait indefinitely. </param>
        /// <param name="exitContext">
        /// true to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it; otherwise, false. 
        /// </param>
        /// <param name="cancelWaitHandle">A cancel wait handle to interrupt the wait if needed</param>
        /// <returns>
        /// The array index of the work item result that satisfied the wait, or WaitTimeout if no work item result satisfied the wait and a time interval equivalent to millisecondsTimeout has passed or the work item has been canceled.
        /// </returns>
        public static int WaitAny(
            IWorkItemResult[] workItemResults,
            TimeSpan timeout,
            bool exitContext,
            WaitHandle cancelWaitHandle)
        {
            return WaitAny(workItemResults, (int)timeout.TotalMilliseconds, exitContext, cancelWaitHandle);
        }

        /// <summary>
        /// Waits for any of the work items in the specified array to complete, cancel, or timeout
        /// </summary>
        /// <param name="workItemResults">Array of work item result objects</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Timeout.Infinite (-1) to wait indefinitely.</param>
        /// <param name="exitContext">
        /// true to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it; otherwise, false. 
        /// </param>
        /// <returns>
        /// The array index of the work item result that satisfied the wait, or WaitTimeout if no work item result satisfied the wait and a time interval equivalent to millisecondsTimeout has passed or the work item has been canceled.
        /// </returns>
        public static int WaitAny(
            IWorkItemResult[] workItemResults,
            int millisecondsTimeout,
            bool exitContext)
        {
            return WorkItem.WaitAny(workItemResults, millisecondsTimeout, exitContext, null);
        }

        /// <summary>
        /// Waits for any of the work items in the specified array to complete, cancel, or timeout
        /// </summary>
        /// <param name="workItemResults">Array of work item result objects</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Timeout.Infinite (-1) to wait indefinitely.</param>
        /// <param name="exitContext">
        /// true to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it; otherwise, false. 
        /// </param>
        /// <param name="cancelWaitHandle">A cancel wait handle to interrupt the wait if needed</param>
        /// <returns>
        /// The array index of the work item result that satisfied the wait, or WaitTimeout if no work item result satisfied the wait and a time interval equivalent to millisecondsTimeout has passed or the work item has been canceled.
        /// </returns>
        public static int WaitAny(
            IWorkItemResult[] workItemResults,
            int millisecondsTimeout,
            bool exitContext,
            WaitHandle cancelWaitHandle)
        {
            return WorkItem.WaitAny(workItemResults, millisecondsTimeout, exitContext, cancelWaitHandle);
        }

        #endregion

        #region Shutdown functions

        /// <summary>
        /// Force the SmartThreadPool to shutdown
        /// </summary>
        public void Shutdown()
        {
            Shutdown(true, 0);
        }

        public void Shutdown(bool forceAbort, TimeSpan timeout)
        {
            Shutdown(forceAbort, (int)timeout.TotalMilliseconds);
        }

        /// <summary>
        /// Empties the queue of work items and abort the threads in the pool.
        /// </summary>
        public void Shutdown(bool forceAbort, int millisecondsTimeout)
        {
            if (_workerThreads == null) return;

            using (ChoBufferProfileEx profile = new ChoBufferProfileEx(true, String.Format("Shutting down `{0}` Threadpool...", _name)))
            {
                ValidateNotDisposed();
                Thread[] threads = null;
                lock (_workerThreads.SyncRoot)
                {
                    // Shutdown the work items queue
                    _workItemsQueue.Cleanup(); //.Dispose();

                    // Signal the threads to exit
                    _shutdown = true;
                    _shuttingDownEvent.Set();

                    // Make a copy of the threads' references in the pool
                    threads = new Thread[_workerThreads.Count];
                    _workerThreads.Keys.CopyTo(threads, 0);
                }

                int millisecondsLeft = millisecondsTimeout;
                DateTime start = DateTime.Now;
                bool waitInfinitely = (Timeout.Infinite == millisecondsTimeout);
                bool timeout = false;

                // Each iteration we update the time left for the timeout.
                foreach (Thread thread in threads)
                {
                    // Join don't work with negative numbers
                    if (!waitInfinitely && (millisecondsLeft < 0))
                    {
                        timeout = true;
                        break;
                    }

                    bool success = thread.Join(millisecondsLeft);
                    if (!success)
                    {
                        timeout = true;
                        break;
                    }

                    if (!waitInfinitely)
                    {
                        // Update the time left to wait
                        TimeSpan ts = DateTime.Now - start;
                        millisecondsLeft = millisecondsTimeout - (int)ts.TotalMilliseconds;
                    }
                }

                if (timeout && forceAbort)
                {
                    // Abort the threads in the pool
                    foreach (Thread thread in threads)
                    {
                        if ((thread != null) && thread.IsAlive)
                        {
                            try
                            {
                                thread.Abort("Shutdown");
                            }
                            catch (SecurityException)
                            {
                            }
                            catch (ThreadStateException)
                            {
                                // In case the thread has been terminated 
                                // after the check if it is alive.
                            }
                        }
                    }
                }
                _isIdleWaitHandle.Set();
                if (_blockIfPoolBusy)
                    _tooBusyWaitHandle.Set();
            }
        }

        #endregion Shutdown functions

        #region Properties

        public bool CanQueueWorkItems
        {
            get { return _canQueueWorkItems; }
            set { _canQueueWorkItems = value; }
        }


        public string Name
        {
            get { return _name; }
        }

        public bool DisposeOfStateObjects
        {
            get { return _disposeOfStateObjects; }
            set { _disposeOfStateObjects = value; }
        }

        public bool BlockIfPoolBusy
        {
            get { return _blockIfPoolBusy; }
            set { _blockIfPoolBusy = value; }
        }

        public bool UseCallerContext
        {
            get { return _useCallerContext; }
            set { _useCallerContext = value; }
        }

        public CallToPostExecute CallToPostExecute
        {
            get { return _callToPostExecute; }
            set { _callToPostExecute = value; }
        }

        public PostExecuteWorkItemCallback PostExecuteWorkItemCallback
        {
            get { return _postExecuteWorkItemCallback; }
            set { _postExecuteWorkItemCallback = value; }
        }

        /// <summary>
        /// Get the lower limit of threads in the pool.
        /// </summary>
        public int MinThreads
        {
            get
            {
                ValidateNotDisposed();
                return _minWorkerThreads;
            }
        }

        /// <summary>
        /// Get the upper limit of threads in the pool.
        /// </summary>
        public int MaxThreads
        {
            get
            {
                ValidateNotDisposed();
                return _maxWorkerThreads;
            }
        }
        /// <summary>
        /// Get the number of threads in the thread pool.
        /// Should be between the lower and the upper limits.
        /// </summary>
        public int ActiveThreads
        {
            get
            {
                ValidateNotDisposed();
                return _workerThreads.Count;
            }
        }

        /// <summary>
        /// Get the number of busy (not idle) threads in the thread pool.
        /// </summary>
        public int InUseThreads
        {
            get
            {
                ValidateNotDisposed();
                return _inUseWorkerThreads;
            }
        }

        /// <summary>
        /// Get the number of work items in the queue.
        /// </summary>
        public int WaitingCallbacks
        {
            get
            {
                ValidateNotDisposed();
                return _workItemsQueue.Count;
            }
        }

        #endregion

        #region IDisposable Members

        ~ChoThreadPool()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_waitOnDispose)
                {
                    WaitForIdle();
                }
                if (!_shutdown)
                {
                    Shutdown();
                }
                if (null != _shuttingDownEvent)
                {
                    _shuttingDownEvent.Close();
                    _shuttingDownEvent = null;
                }
                _workerThreads.Clear();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        private void ValidateNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().ToString(), "The SmartThreadPool has been shutdown");
            }
        }
        #endregion

        #region Object overrides

        public new string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("{0} ChoThreadPool Settings", Name);

            msg.AppendNewLine();
            msg.AppendLine("MaxWorkerThreads: {0}", _maxWorkerThreads);
            msg.AppendLine("MinWorkerThreads: {0}", _minWorkerThreads);
            msg.AppendLine("IdleTimeout: {0}", _idleTimeout);
            msg.AppendLine("DisposeOfStateObjects: {0}", _disposeOfStateObjects);
            msg.AppendLine("UseCallerContext: {0}", _useCallerContext);
            msg.AppendLine("PostExecuteWorkItemCallback: {0}", _postExecuteWorkItemCallback);
            msg.AppendLine("CallToPostExecute: {0}", _callToPostExecute);
            msg.AppendLine("BlockIfPoolBusy: {0}", _blockIfPoolBusy);
            msg.AppendNewLine();

            return msg.ToString();
        }

        #endregion
    }
}
