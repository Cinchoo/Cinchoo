using System;
using System.Collections.Generic;
using System.Text;

namespace Cinchoo.Core
{
    public abstract class ChoSyncDisposableObject : ChoDisposableObject, IChoSyncDisposable
    {
        #region Instance Data Members (Private)

        private readonly object _disposableLockObj = new object();

        #endregion Instance Data Members (Private)

        #region IChoSyncDisposable Members

        public object DisposableLockObj
        {
            get { return _disposableLockObj; }
        }

        #endregion
    }

    public abstract class ChoSyncDisposableObject<T> : ChoDisposableObject<T>, IChoSyncDisposable
    {
        #region Instance Data Members (Private)

        private readonly object _disposableLockObj = new object();

        #endregion Instance Data Members (Private)

        #region IChoSyncDisposable Members

        public object DisposableLockObj
        {
            get { return _disposableLockObj; }
        }

        #endregion
    }
}
