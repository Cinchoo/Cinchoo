namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public interface IChoObjectCopyable : IObjectCopyable<object>
    {
    }

    public interface IObjectCopyable<TObject> where TObject : class
    {
        void Copy(TObject obj);
    }
}
