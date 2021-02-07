namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public class ChoConsoleProfileBackingStore : IChoProfileBackingStore
    {
        #region IChoProfileBackingStore Members

        public void Start(string actionCmds)
        {
            //using (ChoConsoleSession session = new ChoConsoleSession(ConsoleColor.DarkGreen))
            //    session.WriteLine("Starting...");
        }

        public void Stop(string actionCmds)
        {
            //using (ChoConsoleSession session = new ChoConsoleSession(ConsoleColor.DarkBlue))
            //    session.WriteLine("Stopping...");
        }

        public void Write(string msg, object tag)
        {
            Console.Write(msg);
        }

        #endregion
    }
}
