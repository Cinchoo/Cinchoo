namespace Cinchoo.Core.ConsoleUtils
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion

    #region CtrlTypes Enum

    // An enumerated type for the control messages
    // sent to the handler routine.
    public enum CtrlTypes
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT,
        CTRL_CLOSE_EVENT,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT
    }

    #endregion CtrlTypes Enum

    #region Delegate Definition

    // A delegate type to be used as the handler routine 
    // for SetConsoleCtrlHandler.
    public delegate void ConsoleCtrlHandler(CtrlTypes ctrlType);

    #endregion Delegate Definition

    class ChoConsoleApplication
    {
    }
}
