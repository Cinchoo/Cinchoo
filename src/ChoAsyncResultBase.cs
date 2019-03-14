namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Threading;
    using Cinchoo.Core.Diagnostics;

    #endregion NameSpaces

    public abstract class ChoAsyncResultBase : IChoAsyncResult
    {
        #region Shared Data Members (Private)

        private static readonly Exception _threadAbortedException = new ChoThreadAbortException();

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Private)

        // Fields set at construction which never change while 
        // operation is pending
        private readonly Object _asyncState;
        private readonly object _waitHandleLock = new object();

        // Fields set at construction which do change after 
        // operation completes
        private const Int32 StatePending = 0;
        private const Int32 StateCompletedSynchronously = 1;
        private const Int32 StateCompletedAsynchronously = 2;

        private Int32 _completedState = StatePending;

        // Field that may or may not get set depending on usage
        private ManualResetEvent _asyncWaitHandle;

        // Fields set when operation completes
        private Exception _exception;
        private object _result = null;
        private bool _isAborted = false;
        private bool _isTimedOut = false;

        #endregion Instance Data Members (Private)

        #region Constructors

        internal ChoAsyncResultBase(Object state)
        {
            _asyncState = state;
        }

        #endregion Constructors

        #region Instance Members (Internal)

        internal void SetAsSuccess(object result, bool completedSynchronously)
        {
            _result = result;
            SignalCompletion(completedSynchronously);
        }

        internal void SetAsFailed(Exception exception, bool completedSynchronously)
        {
            // Passing null for exception means no error occurred. 
            // This is the common case

            if (exception is ChoTimeoutException)
                _isTimedOut = true;

            _exception = exception;
            SignalCompletion(completedSynchronously);
        }

        internal void SetAsAborted(bool completedSynchronously)
        {
            if (_isAborted)
                return;

            // Passing null for exception means no error occurred. 
            // This is the common case
            _isAborted = true;
            _exception = new ChoThreadAbortException();

            //_exception = _threadAbortedException;
            SignalCompletion(completedSynchronously);
        }

        #endregion Instance Members (Internal)

        #region Instance Members (Private)

        protected abstract void InvokeCallback(object state);

        private void SignalCompletion(bool completedSynchronously)
        {
            // The m_CompletedState field MUST be set prior calling the callback
            Int32 prevState = Interlocked.Exchange(ref _completedState, completedSynchronously ? StateCompletedSynchronously :
               StateCompletedAsynchronously);
            if (prevState != StatePending)
                return;
            //throw new InvalidOperationException("You can set a result only once");

            // If the event exists, set it
            if (_asyncWaitHandle != null)
                _asyncWaitHandle.Set();

            // If a callback method was set, call it
            //ThreadPool.QueueUserWorkItem(InvokeCallback);
            InvokeCallback(null);
        }

        #endregion Instance Members (Private)

        #region Instance Members (Public)

        public object EndInvoke()
        {
            // This method assumes that only 1 thread calls EndInvoke 
            // for this object
            if (!IsCompleted)
            {
                // If the operation isn't done, wait for it
                AsyncWaitHandle.WaitOne();
                AsyncWaitHandle.Close();
                _asyncWaitHandle = null;  // Allow early GC
            }

            // Operation is done: if an exception occured, throw it
            if (_exception != null) throw _exception;

            return _result;
        }

        #endregion Instance Members (Public)

        #region Implementation of IAsyncResult

        public Object AsyncState
        {
            get { return _asyncState; }
        }

        public Boolean CompletedSynchronously
        {
            get { return Thread.VolatileRead(ref _completedState) == StateCompletedSynchronously; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (_asyncWaitHandle == null)
                {
                    Boolean done = IsCompleted;
                    ManualResetEvent mre = new ManualResetEvent(done);
                    if (Interlocked.CompareExchange(ref _asyncWaitHandle, mre, null) != null)
                    {
                        // Another thread created this object's event; dispose 
                        // the event we just created
                        mre.Close();
                    }
                    else
                    {
                        if (!done && IsCompleted)
                        {
                            // If the operation wasn't done when we created 
                            // the event but now it is done, set the event
                            _asyncWaitHandle.Set();
                        }
                    }
                }
                return _asyncWaitHandle;
            }
        }

        public Boolean IsCompleted
        {
            get { return Thread.VolatileRead(ref _completedState) != StatePending; }
        }

        #endregion

        #region IChoAsyncResult Members

        public bool IsTimedout
        {
            get { return _isTimedOut; }
        }

        public bool IsAborted
        {
            get { return _isAborted; }
        }

        #endregion

        #region Object Overrides

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendFormat("Status: {0}{1}", _exception != null ? "SUCCESS" : "FAILED", Environment.NewLine);
            msg.AppendFormat("Aborted: {0}{1}", _isAborted, Environment.NewLine);
            msg.AppendFormat("Timedout: {0}{1}", _isTimedOut, Environment.NewLine);
            msg.AppendFormat("State: {0}{1}", _completedState, Environment.NewLine);
            if (_exception == null)
                msg.AppendFormat("Result: {0}{1}", ChoString.ToString(_result), Environment.NewLine);
            else
                msg.AppendFormat("Exception: {0}{1}", ChoString.ToString(_exception), Environment.NewLine);
            msg.AppendFormat("AsyncState: {0}{1}", ChoString.ToString(_asyncState), Environment.NewLine);

            return msg.ToString();
        }

        #endregion Object Overrides
    }
}
