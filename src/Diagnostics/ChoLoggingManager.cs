namespace eSquare.Core.Logging
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Configuration;
    using System.Collections.Generic;

    using eSquare.Core.Attributes;
    using eSquare.Core.Exceptions;
    using eSquare.Core.Validation;

    #endregion NameSpaces

    public sealed class ChoLoggingManager : IChoLoggingManager
    {
        #region IChoLoggingManager Members

        public void Log(Type caller, object value)
        {
            //Broadcase them to all loggers simultaniously
            if (ChoLoggingManagerSettings.Me.LoggingManagers == null || ChoLoggingManagerSettings.Me.LoggingManagers.Length == 0)
                return;

            foreach (
        }

        #endregion
    }
}
