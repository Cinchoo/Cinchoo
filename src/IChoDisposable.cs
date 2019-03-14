namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Threading;
    using System.Diagnostics;
    using System.Collections.Generic;

    #endregion NameSpaces

    public interface IChoDisposable : IDisposable
    {
        bool IsDisposed
        {
            get;
        }

        StackTrace ObjectCreationStackTrace
        {
            get;
        }

        void Dispose(bool finalize);
        //bool Dispose(WaitHandle notifyObject);
    }
}
