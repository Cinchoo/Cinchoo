namespace eSquare.Core.Diagnostics.Logging.Loggers
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public class ChoConsoleLogger : ChoLoggerImpl
    {
        #region IChoLogger Members

        public override void Log(object value)
        {
            if (value == null) return;

            Console.WriteLine(Format(value).ToString());
        }

        #endregion
    }
}
