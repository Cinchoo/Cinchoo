namespace Cinchoo.Core.Factory
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    public interface IChoCustomObjectFactory
    {
        object CreateInstance(Type type);
        object CreateInstance(string type);
    }
}
