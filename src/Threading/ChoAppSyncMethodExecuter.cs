namespace Cinchoo.Core.Threading
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Threading;
    using System.Collections.Generic;

    #endregion NameSpaces

    #region Delegates

    public delegate object ChoCallback(object state);

    #endregion

    public class ChoAppSyncMethodExecuter : IDisposable
    {
        #region Instance Data Members (Private)

        private static ManualResetEvent _isLoading = new ManualResetEvent(true);
        private static bool _isExecuting = false;
        private object _retValue = null;

        #endregion

        #region Constructors

        public ChoAppSyncMethodExecuter(ChoCallback waitCallback)
            : this(waitCallback, null)
        {
        }

        public ChoAppSyncMethodExecuter(ChoCallback waitCallback, object state)
        {
            if (_isExecuting)
                _isLoading.WaitOne();
            else
            {
                _isExecuting = true;
                _isLoading.Reset();

                if (waitCallback != null)
                    _retValue = waitCallback(state);
            }
        }

        #endregion Constructors

        #region Instance Properties (Public)

        public object RetValue
        {
            get { return _retValue; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _isExecuting = false;
            _isLoading.Set();
        }

        #endregion

        #region Destructors

        ~ChoAppSyncMethodExecuter()
        {
            Dispose();
        }

        #endregion
    }
}
