namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    #endregion NameSpaces

    public class ChoAbortableAsyncResult : ChoAsyncResultBase, IChoAbortableAsyncResult
    {
        #region Instance Data Members (Private)

        private Thread _threadToKill;
        private readonly ChoAsyncCallback _asyncCallback;

        #endregion Instance Data Members (Private)

        #region Constructors

        internal ChoAbortableAsyncResult(ChoAsyncCallback asyncCallback, Object state)
            : base(state)
        {
            _asyncCallback = asyncCallback;
        }

        #endregion Constructors

        protected override void InvokeCallback(object state)
        {
            if (_asyncCallback != null)
                _asyncCallback.DynamicInvoke(new object[] { this });
        }

        #region Object Overrides

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder();
            msg.Append(base.ToString());
            msg.AppendFormat("Callback: {0}{1}", ChoString.ToString(_asyncCallback), Environment.NewLine);
            return msg.ToString();
        }

        #endregion Object Overrides

        #region Instance Properties (Internal)

        internal bool AbortRequested
        {
            get;
            set;
        }

        internal Thread ThreadToKill
        {
            get { return _threadToKill; }
            set { _threadToKill = value; }
        }

        #endregion Instance Properties (Internal)

        #region IChoAbortableAsyncResult Members

        public void Abort()
        {
            AbortRequested = true;
            if (_threadToKill != null && _threadToKill.IsAlive)
            {
                _threadToKill.AbortThread();
                SetAsAborted(true);
            }
        }

        #endregion IChoAbortableAsyncResult Members
    }
}
