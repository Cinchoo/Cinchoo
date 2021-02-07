namespace Cinchoo.Core.Services
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    public interface IChoQueuedMsgServiceObject<T>
    {
        T State { get; }
        bool IsQuitServiceMsg { get; }
        int Timeout { get; }
        int MaxNoOfRetry { get; }
    }

    public abstract class ChoQueuedMsgServiceObjectBase<T> : IChoQueuedMsgServiceObject<T>
    {
        #region Constructors

        public ChoQueuedMsgServiceObjectBase()
        {
        }

        public ChoQueuedMsgServiceObjectBase(T state, int timeout = 10000, int maxNoOfRetry = 0)
        {
            _state = state;
            _timeout = timeout;
            _maxNoOfRetry = maxNoOfRetry;
        }

        #endregion Constructors

        #region Instance Data Members (Private)

        private readonly T _state;
        private readonly int _timeout;
        private readonly int _maxNoOfRetry;

        #endregion Instance Data Members (Private)

        #region ISyncMsgQObject<T> Members

        public T State
        {
            get { return _state; }
        }

        public abstract bool IsQuitServiceMsg
        {
            get;
        }

        #endregion

        public int Timeout
        {
            get { return _timeout; }
        }

        public int MaxNoOfRetry
        {
            get { return _maxNoOfRetry; }
        }
    }
}
