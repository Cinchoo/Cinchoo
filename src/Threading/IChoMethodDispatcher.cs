namespace Cinchoo.Core.Threading
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public interface IChoMethodDispatcher
    {
        ChoMethodDispatcherResult Invoke(params object[] inputs);
    }
}
