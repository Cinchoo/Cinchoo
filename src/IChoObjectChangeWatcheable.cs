namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion NameSpaces

    public interface IChoObjectChangeWatcheable
    {
        event EventHandler ObjectChanged;
        void OnObjectChanged(object target, EventArgs e);
    }
}
