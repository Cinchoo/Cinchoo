namespace eSquare.Core.Diagnostics.Logging
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Diagnostics;
    using System.Collections.Generic;

    using eSquare.Core.Collections.Generic;
    using eSquare.Core.Diagnostics.Logging.Formatters;
    using eSquare.Core.Diagnostics.Logging.LogListeners;

    #endregion NameSoaces

    [ChoLogManager("default")]
    public class ChoLogManager : IChoLogManager
    {
        #region IChoLoggerManager Members

        public void Log(ChoLogEntry message)
        {
            ChoDictionary<string, ChoLogListener[]> logListeners = ChoLoggerSettings.Me.Find(message.Categories);
            foreach (string key in logListeners.Keys)
            {
                if (logListeners[key] != null)
                {
                    foreach (ChoLogListener logListener in logListeners[key])
                        logListener.TraceData(new TraceEventCache(), key, message.Severity, 100, message.Message);
                }
            }
        }

        #endregion
    }
}
