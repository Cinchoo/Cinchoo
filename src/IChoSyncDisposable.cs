namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public interface IChoSyncDisposable : IChoDisposable
    {
        object DisposableLockObj
        {
            get;
        }
    }
}
