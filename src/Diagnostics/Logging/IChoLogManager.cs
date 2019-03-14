namespace eSquare.Core.Diagnostics.Logging
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Diagnostics;
    using System.Collections.Generic;

    #endregion NameSpaces

    public interface IChoLogManager
    {
        void Log(ChoLogEntry message);
    }
}
