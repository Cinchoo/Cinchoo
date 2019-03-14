namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion NameSpaces

    public interface IChoAbortableAsyncResult : IChoAsyncResult
    {
        void Abort();
        bool IsAborted { get; }
    }
}
