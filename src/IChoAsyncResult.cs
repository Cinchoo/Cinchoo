namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion NameSpaces

    public delegate void ChoAsyncCallback(IChoAsyncResult result);
    public delegate void ChoAbortableAsyncCallback(IChoAbortableAsyncResult result);
 
    public interface IChoAsyncResult : IAsyncResult
    {
        object Result { get; }
        bool IsTimedout { get; }
        object EndInvoke();
    }
}
