namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    #endregion NameSpaces

    public class ChoAsyncResult : ChoAsyncResultBase
    {
        #region Instance Data Members (Private)

        private readonly ChoAsyncCallback _asyncCallback;

        #endregion Instance Data Members (Private)

        #region Constructors

        internal ChoAsyncResult(ChoAsyncCallback asyncCallback, Object state)
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
    }
}
