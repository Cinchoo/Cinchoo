namespace eSquare.Core.Diagnostics
{
    #region NameSpaces

    using System;
    using System.Reflection;
    using System.Diagnostics;

    #endregion NameSpaces

    public static class ChoStackTrace
    {
        #region Shared Members (Public)

        public static string GetMemberName()
        {
            StackFrame stackFrame = new StackTrace(true).GetFrame(2);
            if (stackFrame == null) return "Unknown";

            return stackFrame.GetMethod().Name;
        }

        public new static string ToString()
        {
            StackFrame stackFrame = new StackTrace(true).GetFrame(2);
            if (stackFrame == null) return "Unknown";

            return stackFrame.GetMethod().ToString();
        }

        #endregion
    }
}
