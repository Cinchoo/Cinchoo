namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    public class ChoExceptionEventArgs : EventArgs
    {
        public readonly Exception Exception;

        public ChoExceptionEventArgs(Exception ex)
        {
            Exception = ex;
        }
    }
}
