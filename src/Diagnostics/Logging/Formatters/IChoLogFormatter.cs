namespace eSquare.Core.Diagnostics.Logging.Formatters
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Diagnostics;
    using System.Collections.Generic;

    #endregion NameSpaces
    
    public interface IChoLogFormatter
    {
        string Format(ChoLogEntry logEntry);
    }
}
