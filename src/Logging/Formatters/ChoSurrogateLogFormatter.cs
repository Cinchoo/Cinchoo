using System;
using System.Collections.Generic;
using System.Text;

namespace eSquare.Core.Diagnostics.Logging.Formatters
{
    public class ChoSurrogateLogFormatter : IChoLogFormatter
    {
        #region IChoLogFormatter Members

        public string Format(ChoLogEntry logMessage)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
