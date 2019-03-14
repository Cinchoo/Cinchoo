namespace Cinchoo.Core.Threading
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.Remoting.Messaging;
    
    using System.Threading;
    using Cinchoo.Core.Services;

    #endregion NameSpaces

    public enum SyncGateMode
    {
        Exclusive,
        Shared
    }

    public class ChoSyncGate : ChoSyncDisposableObject
    {
        #region Instance Data Members (Private)

        private int _numReaders;
        private Queue<SyncGateAsyncResult> _qReadRequests;
        private Queue<SyncGateAsyncResult> _qWriteRequests;
        private SyncGateStates _state;
        private object _syncLock;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoSyncGate() : this(false)
        {
        }

        /// <summary>
        /// Constructs a SyncGate object
        /// </summary>
        /// <param name="blockReadersUntilFirstWriteCompletes">Pass true to have readers block until the first writer has created the data that is being protected by the ReaderWriterGate.</param>
        public ChoSyncGate(bool blockReadersUntilFirstWriteCompletes)
        {
            this._syncLock = new object();
            this._qWriteRequests = new Queue<SyncGateAsyncResult>();
            this._qReadRequests = new Queue<SyncGateAsyncResult>();
            this._state = blockReadersUntilFirstWriteCompletes ? SyncGateStates.ReservedForWriter : SyncGateStates.Free;
        }

        #endregion Constructors

        #region Instance Members (Public)

        /// <summary>
        /// Allows the caller to notify the SyncGate that it wants exclusive or shared access to a resource. 
        /// </summary>
        /// <param name="mode">Indicates if exclusive or shared access is desired.</param>
        /// <param name="asyncCallback">The callback method to invoke once access can be granted.</param>
        public void BeginRegion(SyncGateMode mode, ChoAsyncCallback asyncCallback)
        {
            BeginRegion(mode, asyncCallback, null);
        }

        /// <summary>
        /// Allows the caller to notify the SyncGate that it wants exclusive or shared access to a resource. 
        /// </summary>
        /// <param name="mode">Indicates if exclusive or shared access is desired.</param>
        /// <param name="asyncCallback">The callback method to invoke once access can be granted.</param>
        /// <param name="asyncState">Additional state to pass to the callback method.</param>
        public void BeginRegion(SyncGateMode mode, ChoAsyncCallback asyncCallback, object asyncState)
        {
            ChoGuard.ArgumentNotNull(asyncCallback, "asyncCallback");

            SyncGateAsyncResult ar = new SyncGateAsyncResult(mode, asyncCallback, asyncState);
            lock (_syncLock)
            {
                switch (mode)
                {
                    case SyncGateMode.Exclusive:
                        switch (_state)
                        {
                            case SyncGateStates.OwnedByReaders:
                            case SyncGateStates.OwnedByReadersAndWriterPending:
                                _state = SyncGateStates.OwnedByReadersAndWriterPending;
                                _qWriteRequests.Enqueue(ar);
                                break;
                            case SyncGateStates.OwnedByWriter:
                                _qWriteRequests.Enqueue(ar);
                                break;
                        }
                        break;

                    case SyncGateMode.Shared:
                        switch (_state)
                        {
                            case SyncGateStates.Free:
                            case SyncGateStates.OwnedByReaders:
                                _state = SyncGateStates.OwnedByReaders;
                                _numReaders++;
                                QueueCallback(ar);
                                break;
                            case SyncGateStates.OwnedByReadersAndWriterPending:
                            case SyncGateStates.OwnedByWriter:
                            case SyncGateStates.ReservedForWriter:
                                _qReadRequests.Enqueue(ar);
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Call this method after accessing the resource to notify the SyncGate that it can grant access to other code.
        /// </summary>
        /// <param name="result">The IAsyncResult object given to the callback method when access was granted.</param>
        public void EndRegion(IAsyncResult result)
        {
            ChoGuard.ArgumentNotNull(result, "result");

            SyncGateAsyncResult sgar = (SyncGateAsyncResult)result;
            sgar.EndInvoke();
            lock (_syncLock)
            {
                if ((sgar.Mode != SyncGateMode.Shared) || (--this._numReaders <= 0))
                {
                    if (_qWriteRequests.Count > 0)
                    {
                        _state = SyncGateStates.OwnedByWriter;
                        QueueCallback(_qWriteRequests.Dequeue());
                    }
                    else if (_qReadRequests.Count > 0)
                    {
                        _state = SyncGateStates.OwnedByReaders;
                        _numReaders = _qReadRequests.Count;
                        while (_qReadRequests.Count > 0)
                        {
                            QueueCallback(_qReadRequests.Dequeue());
                        }
                    }
                    else
                    {
                        _state = SyncGateStates.Free;
                    }
                }
            }
        }

        #endregion Instance Members (Public)

        #region Instance Members (Private)

        private static void InvokeCallback(object o)
        {
            ((SyncGateAsyncResult)o).SetAsSuccess(null, false);
        }

        private static void QueueCallback(SyncGateAsyncResult sgar)
        {
            ThreadPool.QueueUserWorkItem(InvokeCallback, sgar);
        }

        #endregion Instance Members (Private)

        #region SyncGateAsyncResult Class

        private sealed class SyncGateAsyncResult : ChoAsyncResult
        {
            private SyncGateMode _syncGateMode;

            internal SyncGateAsyncResult(SyncGateMode mode, ChoAsyncCallback asyncCallback, object state)
                : base(asyncCallback, state)
            {
                this._syncGateMode = mode;
            }

            internal SyncGateMode Mode
            {
                get { return _syncGateMode; }
            }
        }

        #endregion SyncGateAsyncResult Class

        #region SyncGateStates Class

        private enum SyncGateStates
        {
            Free,
            OwnedByReaders,
            OwnedByReadersAndWriterPending,
            OwnedByWriter,
            ReservedForWriter
        }

        #endregion SyncGateStates Class

        protected override void Dispose(bool finalize)
        {
        }
    }
}
