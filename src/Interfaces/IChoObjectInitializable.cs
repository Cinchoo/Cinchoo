namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public interface IChoInitializable
    {
        void Initialize();
    }

    public interface IChoObjectInitializable
    {
        bool Initialize(bool beforeFieldInit, object state);
    }
}
