namespace Cinchoo.Core.Threading
{
    #region NameSpaces

    using System;
    using System.Threading;
    using System.Diagnostics;

    using Cinchoo.Core.Diagnostics;

    #endregion NameSpaces

    #region CallToPostExecute enumerator

    [Flags]
    public enum CallToPostExecute
    {
        Never = 0x00,
        WhenWorkItemCanceled = 0x01,
        WhenWorkItemNotCanceled = 0x02,
        Always = WhenWorkItemCanceled | WhenWorkItemNotCanceled,
    }

    #endregion

    #region WorkItem Delegate

    /// <summary>
    /// A delegate that represents the method to run as the work item
    /// </summary>
    /// <param name="state">A state object for the method to run</param>
    public delegate object WorkItemCallback(object state);

    /// <summary>
    /// A delegate to call after the WorkItemCallback completed
    /// </summary>
    /// <param name="wir">The work item result object</param>
    public delegate void PostExecuteWorkItemCallback(IWorkItemResult wir);

    #endregion

    #region IWorkItemResult interface

    /// <summary>
    /// IWorkItemResult interface
    /// </summary>
    public interface IWorkItemResult
    {
        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits.
        /// </summary>
        /// <returns>The result of the work item</returns>
        object GetResult();

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits until timeout.
        /// </summary>
        /// <returns>The result of the work item</returns>
        /// On timeout throws WorkItemTimeoutException
        object GetResult(
            int millisecondsTimeout,
            bool exitContext);

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits until timeout.
        /// </summary>
        /// <returns>The result of the work item</returns>
        /// On timeout throws WorkItemTimeoutException
        object GetResult(
            TimeSpan timeout,
            bool exitContext);

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits until timeout or until the cancelWaitHandle is signaled.
        /// </summary>
        /// <param name="millisecondsTimeout">Timeout in milliseconds, or -1 for infinite</param>
        /// <param name="exitContext">
        /// true to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it; otherwise, false. 
        /// </param>
        /// <param name="cancelWaitHandle">A cancel wait handle to interrupt the blocking if needed</param>
        /// <returns>The result of the work item</returns>
        /// On timeout throws WorkItemTimeoutException
        /// On cancel throws WorkItemCancelException
        object GetResult(
            int millisecondsTimeout,
            bool exitContext,
            WaitHandle cancelWaitHandle);

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits until timeout or until the cancelWaitHandle is signaled.
        /// </summary>
        /// <returns>The result of the work item</returns>
        /// On timeout throws WorkItemTimeoutException
        /// On cancel throws WorkItemCancelException
        object GetResult(
            TimeSpan timeout,
            bool exitContext,
            WaitHandle cancelWaitHandle);

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits.
        /// </summary>
        /// <param name="e">Filled with the exception if one was thrown</param>
        /// <returns>The result of the work item</returns>
        object GetResult(out Exception e);

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits until timeout.
        /// </summary>
        /// <param name="e">Filled with the exception if one was thrown</param>
        /// <returns>The result of the work item</returns>
        /// On timeout throws WorkItemTimeoutException
        object GetResult(
            int millisecondsTimeout,
            bool exitContext,
            out Exception e);

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits until timeout.
        /// </summary>
        /// <param name="e">Filled with the exception if one was thrown</param>
        /// <returns>The result of the work item</returns>
        /// On timeout throws WorkItemTimeoutException
        object GetResult(
            TimeSpan timeout,
            bool exitContext,
            out Exception e);

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits until timeout or until the cancelWaitHandle is signaled.
        /// </summary>
        /// <param name="millisecondsTimeout">Timeout in milliseconds, or -1 for infinite</param>
        /// <param name="exitContext">
        /// true to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it; otherwise, false. 
        /// </param>
        /// <param name="cancelWaitHandle">A cancel wait handle to interrupt the blocking if needed</param>
        /// <param name="e">Filled with the exception if one was thrown</param>
        /// <returns>The result of the work item</returns>
        /// On timeout throws WorkItemTimeoutException
        /// On cancel throws WorkItemCancelException
        object GetResult(
            int millisecondsTimeout,
            bool exitContext,
            WaitHandle cancelWaitHandle,
            out Exception e);

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits until timeout or until the cancelWaitHandle is signaled.
        /// </summary>
        /// <returns>The result of the work item</returns>
        /// <param name="e">Filled with the exception if one was thrown</param>
        /// On timeout throws WorkItemTimeoutException
        /// On cancel throws WorkItemCancelException
        object GetResult(
            TimeSpan timeout,
            bool exitContext,
            WaitHandle cancelWaitHandle,
            out Exception e);

        /// <summary>
        /// Gets an indication whether the asynchronous operation has completed.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Gets an indication whether the asynchronous operation has been canceled.
        /// </summary>
        bool IsCanceled { get; }

        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
        /// </summary>
        object State { get; }

        /// <summary>
        /// Cancel the work item if it didn't start running yet.
        /// </summary>
        /// <returns>Returns true on success or false if the work item is in progress or already completed</returns>
        bool Cancel();
    }

    #endregion

    #region WorkItem class

    /// <summary>
    /// Holds a callback delegate and the state for that delegate.
    /// </summary>
    internal class WorkItem : IDisposable
    {
        #region WorkItemState enum

        /// <summary>
        /// Indicates the state of the work item in the thread pool
        /// </summary>
        private enum WorkItemState
        {
            InQueue,
            InProgress,
            Completed,
            Canceled,
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// Callback delegate for the callback.
        /// </summary>
        private WorkItemCallback _callback;

        /// <summary>
        /// Callback delegate for the the post execute.
        /// </summary>
        private PostExecuteWorkItemCallback _postExecuteWorkItemCallback;

        /// <summary>
        /// State with which to call the callback delegate.
        /// </summary>
        private object _state;

        /// <summary>
        /// Stores the caller's context
        /// </summary>
        private ChoCallerThreadContext _callerContext;

        /// <summary>
        /// Holds the result of the mehtod
        /// </summary>
        private object _result;

        /// <summary>
        /// Hold the exception if the method threw it
        /// </summary>
        private Exception _exception;

        /// <summary>
        /// Hold the state of the work item
        /// </summary>
        private WorkItemState _workItemState;

        /// <summary>
        /// A ManualResetEvent to indicate that the result is ready
        /// </summary>
        private ManualResetEvent _workItemCompleted;

        /// <summary>
        /// A reference count to the _workItemCompleted. 
        /// When it reaches to zero _workItemCompleted is Closed
        /// </summary>
        private int _workItemCompletedRefCount;

        /// <summary>
        /// Represents the result state of the work item
        /// </summary>
        private WorkItemResult _workItemResult;

        /// <summary>
        /// Indicates when to call to the post execute
        /// </summary>
        private CallToPostExecute _callToPostExecute;

        #endregion

        #region Construction

        /// <summary>
        /// Initialize the callback holding object.
        /// </summary>
        /// <param name="callback">Callback delegate for the callback.</param>
        /// <param name="state">State with which to call the callback delegate.</param>
        /// 
        /// We assume that the WorkItem object is created within the thread
        /// that meant to run the callback
        public WorkItem(
            WorkItemCallback callback,
            object state,
            bool useCallerContext,
            PostExecuteWorkItemCallback postExecuteWorkItemCallback,
            CallToPostExecute callToPostExecute)
        {
            if (useCallerContext)
            {
                _callerContext = ChoCallerThreadContext.Capture();
            }

            _postExecuteWorkItemCallback = postExecuteWorkItemCallback;
            _callToPostExecute = callToPostExecute;
            _callback = callback;
            _state = state;
            _workItemState = WorkItemState.InQueue;
            _workItemCompleted = null;
            _workItemCompletedRefCount = 0;
            _workItemResult = new WorkItemResult(this);
        }

        #endregion

        #region Properties

        internal object State
        {
            get { return _state; }
            set { _state = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Change the state of the work item to in progress if it wasn't canceled.
        /// </summary>
        /// <returns>
        /// Return true on success or false in case the work item was canceled.
        /// If the work item needs to run a post execute then the method will return true.
        /// </returns>
        public bool StartingWorkItem()
        {
            lock (this)
            {
                if (WorkItemState.Canceled == _workItemState)
                {
                    bool result = false;
                    if ((_postExecuteWorkItemCallback != null) &&
                        ((_callToPostExecute & CallToPostExecute.WhenWorkItemCanceled) == CallToPostExecute.WhenWorkItemCanceled))
                    {
                        result = true;
                    }

                    return result;
                }

                Debug.Assert(WorkItemState.InQueue == _workItemState);

                SetWorkItemState(WorkItemState.InProgress);
            }
            return true;
        }

        /// <summary>
        /// Execute the work item and the post execute
        /// </summary>
        public void Execute()
        {
            CallToPostExecute currentCallToPostExecute = 0;

            // Execute the work item if we are in the correct state
            switch (_workItemState)
            {
                case WorkItemState.InProgress:
                    currentCallToPostExecute |= CallToPostExecute.WhenWorkItemNotCanceled;
                    ExecuteWorkItem();
                    break;
                case WorkItemState.Canceled:
                    currentCallToPostExecute |= CallToPostExecute.WhenWorkItemCanceled;
                    break;
                default:
                    Debug.Assert(false);
                    throw new NotSupportedException();
            }

            // Run the post execute as needed
            if ((currentCallToPostExecute & _callToPostExecute) != 0)
            {
                PostExecute();
            }
        }

        /// <summary>
        /// Execute the work item
        /// </summary>
        private void ExecuteWorkItem()
        {
            ChoCallerThreadContext ctc = null;
            if (null != _callerContext)
            {
                ctc = ChoCallerThreadContext.Capture();
                ChoCallerThreadContext.Apply(_callerContext);
            }

            Exception exception = null;
            object result = null;

            try
            {
                result = _callback(_state);
            }
            catch (Exception e)
            {
                // Save the exception so we can rethrow it later
                ChoTrace.Write(e);
                exception = e;
            }

            if (null != _callerContext)
            {
                ChoCallerThreadContext.Apply(ctc);
            }

            SetResult(result, exception);
        }

        /// <summary>
        /// Runs the post execute callback
        /// </summary>
        private void PostExecute()
        {
            if (null != _postExecuteWorkItemCallback)
            {
                try
                {
                    _postExecuteWorkItemCallback(this._workItemResult);
                }
                catch (Exception e)
                {
                    Debug.Assert(null != e);
                }
            }
        }

        /// <summary>
        /// Set the result of the work item to return
        /// </summary>
        /// <param name="result">The result of the work item</param>
        internal void SetResult(object result, Exception exception)
        {
            _result = result;
            _exception = exception;
            SignalComplete(false);
        }

        /// <summary>
        /// Returns the work item result
        /// </summary>
        /// <returns>The work item result</returns>
        internal IWorkItemResult GetWorkItemResult()
        {
            return _workItemResult;
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
        internal static bool WaitAll(
            IWorkItemResult[] workItemResults,
            int millisecondsTimeout,
            bool exitContext,
            WaitHandle cancelWaitHandle)
        {
            bool success;
            WaitHandle[] waitHandles = new WaitHandle[workItemResults.Length]; ;
            GetWaitHandles(workItemResults, waitHandles);

            if ((null == cancelWaitHandle) && (waitHandles.Length <= 64))
            {
                success = WaitHandle.WaitAll(waitHandles, millisecondsTimeout, exitContext);
            }
            else
            {
                success = true;
                int millisecondsLeft = millisecondsTimeout;
                DateTime start = DateTime.Now;

                WaitHandle[] whs;
                if (null != cancelWaitHandle)
                {
                    whs = new WaitHandle[] { null, cancelWaitHandle };
                }
                else
                {
                    whs = new WaitHandle[] { null };
                }

                bool waitInfinitely = (Timeout.Infinite == millisecondsTimeout);
                // Iterate over the wait handles and wait for each one to complete.
                // We cannot use WaitHandle.WaitAll directly, because the cancelWaitHandle
                // won't affect it.
                // Each iteration we update the time left for the timeout.
                for (int i = 0; i < workItemResults.Length; ++i)
                {
                    // WaitAny don't work with negative numbers
                    if (!waitInfinitely && (millisecondsLeft < 0))
                    {
                        success = false;
                        break;
                    }

                    whs[0] = waitHandles[i];
                    int result = WaitHandle.WaitAny(whs, millisecondsLeft, exitContext);
                    if ((result > 0) || (WaitHandle.WaitTimeout == result))
                    {
                        success = false;
                        break;
                    }

                    if (!waitInfinitely)
                    {
                        // Update the time left to wait
                        TimeSpan ts = DateTime.Now - start;
                        millisecondsLeft = millisecondsTimeout - (int)ts.TotalMilliseconds;
                    }
                }
            }
            // Release the wait handles
            ReleaseWaitHandles(workItemResults);

            return success;
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
        internal static int WaitAny(
            IWorkItemResult[] workItemResults,
            int millisecondsTimeout,
            bool exitContext,
            WaitHandle cancelWaitHandle)
        {
            WaitHandle[] waitHandles = null;

            if (null != cancelWaitHandle)
            {
                waitHandles = new WaitHandle[workItemResults.Length + 1];
                GetWaitHandles(workItemResults, waitHandles);
                waitHandles[workItemResults.Length] = cancelWaitHandle;
            }
            else
            {
                waitHandles = new WaitHandle[workItemResults.Length];
                GetWaitHandles(workItemResults, waitHandles);
            }

            int result = WaitHandle.WaitAny(waitHandles, millisecondsTimeout, exitContext);

            // Treat cancel as timeout
            if (null != cancelWaitHandle)
            {
                if (result == workItemResults.Length)
                {
                    result = WaitHandle.WaitTimeout;
                }
            }

            ReleaseWaitHandles(workItemResults);

            return result;
        }

        /// <summary>
        /// Fill an array of wait handles with the work items wait handles.
        /// </summary>
        /// <param name="workItemResults">An array of work item results</param>
        /// <param name="waitHandles">An array of wait handles to fill</param>
        private static void GetWaitHandles(
            IWorkItemResult[] workItemResults,
            WaitHandle[] waitHandles)
        {
            for (int i = 0; i < workItemResults.Length; ++i)
            {
                WorkItemResult wir = workItemResults[i] as WorkItemResult;
                Debug.Assert(null != wir, "All workItemResults must be WorkItemResult objects");

                waitHandles[i] = wir.GetWorkItem().GetWaitHandle();
            }
        }

        /// <summary>
        /// Release the work items' wait handles
        /// </summary>
        /// <param name="workItemResults">An array of work item results</param>
        private static void ReleaseWaitHandles(IWorkItemResult[] workItemResults)
        {
            for (int i = 0; i < workItemResults.Length; ++i)
            {
                WorkItemResult wir = workItemResults[i] as WorkItemResult;

                wir.GetWorkItem().ReleaseWaitHandle();
            }
        }


        #endregion

        #region Private Members

        /// <summary>
        /// Sets the work item's state
        /// </summary>
        /// <param name="workItemState">The state to set the work item to</param>
        private void SetWorkItemState(WorkItemState workItemState)
        {
            lock (this)
            {
                //				if (Thread.CurrentThread != null)
                //					ChoTrace.WriteLine(String.Format("{0}: {1}", Thread.CurrentThread.Name, workItemState));
                _workItemState = workItemState;
            }
        }

        /// <summary>
        /// Signals that work item has been completed or canceled
        /// </summary>
        /// <param name="canceled">Indicates that the work item has been canceled</param>
        private void SignalComplete(bool canceled)
        {
            SetWorkItemState(canceled ? WorkItemState.Canceled : WorkItemState.Completed);
            lock (this)
            {
                // If someone is waiting then signal.
                if (null != _workItemCompleted)
                {
                    _workItemCompleted.Set();
                }
            }
        }

        #endregion

        #region Members exposed by WorkItemResult

        /// <summary>
        /// Cancel the work item if it didn't start running yet.
        /// </summary>
        /// <returns>Returns true on success or false if the work item is in progress or already completed</returns>
        private bool Cancel()
        {
            lock (this)
            {
                switch (_workItemState)
                {
                    case WorkItemState.Canceled:
                        //Debug.WriteLine("Work item already canceled");
                        return true;
                    case WorkItemState.Completed:
                    case WorkItemState.InProgress:
                        //Debug.WriteLine("Work item cannot be canceled");
                        return false;
                    case WorkItemState.InQueue:
                        // Signal to the wait for completion that the work
                        // item has been completed (canceled). There is no
                        // reason to wait for it to get out of the queue
                        SignalComplete(true);
                        //Debug.WriteLine("Work item canceled");
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits for the result, timeout, or cancel.
        /// In case of error the method throws and exception
        /// </summary>
        /// <returns>The result of the work item</returns>
        private object GetResult(
            int millisecondsTimeout,
            bool exitContext,
            WaitHandle cancelWaitHandle)
        {
            Exception e = null;
            object result = GetResult(millisecondsTimeout, exitContext, cancelWaitHandle, out e);
            if (null != e)
            {
                throw e;
            }
            return result;
        }

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits for the result, timeout, or cancel.
        /// In case of error the e argument is filled with the exception
        /// </summary>
        /// <returns>The result of the work item</returns>
        private object GetResult(
            int millisecondsTimeout,
            bool exitContext,
            WaitHandle cancelWaitHandle,
            out Exception e)
        {
            e = null;

            // Check for cancel
            if (WorkItemState.Canceled == _workItemState)
            {
                throw new ChoWorkItemCancelException("Work item canceled");
            }

            // Check for completion
            if (IsCompleted)
            {
                e = _exception;
                return _result;
            }

            // If no cancelWaitHandle is provided
            if (null == cancelWaitHandle)
            {
                WaitHandle wh = GetWaitHandle();

                bool timeout = !wh.WaitOne(millisecondsTimeout, exitContext);

                ReleaseWaitHandle();

                if (timeout)
                    throw new ChoWorkItemTimeoutException(String.Format("Work item timeout [Timeout (secs) : {0}]", millisecondsTimeout == -1 ? "Infinite" : (millisecondsTimeout / 1000).ToString()));
            }
            else
            {
                WaitHandle wh = GetWaitHandle();
                int result = WaitHandle.WaitAny(new WaitHandle[] { wh, cancelWaitHandle });
                ReleaseWaitHandle();

                switch (result)
                {
                    case 0:
                        // The work item signaled
                        // Note that the signal could be also as a result of canceling the 
                        // work item (not the get result)
                        break;
                    case 1:
                    case WaitHandle.WaitTimeout:
                        throw new ChoWorkItemTimeoutException(String.Format("Work item timeout [Timeout (secs) : {0}]", millisecondsTimeout == -1 ? "Infinite" : (millisecondsTimeout / 1000).ToString()));
                    default:
                        Debug.Assert(false);
                        break;

                }
            }

            // Check for cancel
            if (WorkItemState.Canceled == _workItemState)
            {
                throw new ChoWorkItemCancelException("Work item canceled");
            }

            Debug.Assert(IsCompleted);

            e = _exception;

            // Return the result
            return _result;
        }

        /// <summary>
        /// A wait handle to wait for completion, cancel, or timeout 
        /// </summary>
        private WaitHandle GetWaitHandle()
        {
            lock (this)
            {
                if (null == _workItemCompleted)
                {
                    _workItemCompleted = new ManualResetEvent(IsCompleted);
                }
                ++_workItemCompletedRefCount;
            }
            return _workItemCompleted;
        }

        private void ReleaseWaitHandle()
        {
            lock (this)
            {
                if (null != _workItemCompleted)
                {
                    --_workItemCompletedRefCount;
                    if (0 == _workItemCompletedRefCount)
                    {
                        _workItemCompleted.Close();
                        _workItemCompleted = null;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true when the work item has completed or canceled
        /// </summary>
        private bool IsCompleted
        {
            get
            {
                lock (this)
                {
                    return ((_workItemState == WorkItemState.Completed) ||
                        (_workItemState == WorkItemState.Canceled));
                }
            }
        }

        /// <summary>
        /// Returns true when the work item has canceled
        /// </summary>
        public bool IsCanceled
        {
            get
            {
                lock (this)
                {
                    return (_workItemState == WorkItemState.Canceled);
                }
            }
        }

        #endregion

        #region WorkItemResult class

        private class WorkItemResult : IWorkItemResult
        {
            /// <summary>
            /// A back reference to the work item
            /// </summary>
            private WorkItem _workItem;

            public WorkItemResult(WorkItem workItem)
            {
                _workItem = workItem;
            }

            internal WorkItem GetWorkItem()
            {
                return _workItem;
            }

            #region IWorkItemResult Members

            public bool IsCompleted
            {
                get
                {
                    return _workItem.IsCompleted;
                }
            }

            public bool IsCanceled
            {
                get
                {
                    return _workItem.IsCanceled;
                }
            }

            public object GetResult()
            {
                return _workItem.GetResult(Timeout.Infinite, true, null);
            }

            public object GetResult(int millisecondsTimeout, bool exitContext)
            {
                return _workItem.GetResult(millisecondsTimeout, exitContext, null);
            }

            public object GetResult(TimeSpan timeout, bool exitContext)
            {
                return _workItem.GetResult((int)timeout.TotalMilliseconds, exitContext, null);
            }

            public object GetResult(int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle)
            {
                return _workItem.GetResult(millisecondsTimeout, exitContext, cancelWaitHandle);
            }

            public object GetResult(TimeSpan timeout, bool exitContext, WaitHandle cancelWaitHandle)
            {
                return _workItem.GetResult((int)timeout.TotalMilliseconds, exitContext, cancelWaitHandle);
            }

            public object GetResult(out Exception e)
            {
                return _workItem.GetResult(Timeout.Infinite, true, null, out e);
            }

            public object GetResult(int millisecondsTimeout, bool exitContext, out Exception e)
            {
                return _workItem.GetResult(millisecondsTimeout, exitContext, null, out e);
            }

            public object GetResult(TimeSpan timeout, bool exitContext, out Exception e)
            {
                return _workItem.GetResult((int)timeout.TotalMilliseconds, exitContext, null, out e);
            }

            public object GetResult(int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle, out Exception e)
            {
                return _workItem.GetResult(millisecondsTimeout, exitContext, cancelWaitHandle, out e);
            }

            public object GetResult(TimeSpan timeout, bool exitContext, WaitHandle cancelWaitHandle, out Exception e)
            {
                return _workItem.GetResult((int)timeout.TotalMilliseconds, exitContext, cancelWaitHandle, out e);
            }

            public bool Cancel()
            {
                return _workItem.Cancel();
            }

            public object State
            {
                get
                {
                    return _workItem._state;
                }
            }

            #endregion
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            IDisposable disp = _state as IDisposable;
            if (null != disp)
            {
                disp.Dispose();
                _state = null;
            }
        }

        #endregion
    }
    #endregion
}
