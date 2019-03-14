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
    }

    public abstract class ChoQueuedMsgServiceObjectBase<T> : IChoQueuedMsgServiceObject<T>
    {
        #region Constructors

        public ChoQueuedMsgServiceObjectBase()
        {
        }

        public ChoQueuedMsgServiceObjectBase(T state)
        {
            _state = state;
        }

        #endregion Constructors

        #region Instance Data Members (Private)

        private T _state;

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
    }
}
