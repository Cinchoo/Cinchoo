namespace Cinchoo.Core.Logging
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    //[ChoLogListener("ChoConsoleLogListener")]
    public class ChoConsoleLogListener : ChoLogListener
    {
        #region TraceListener Overrides

        public override void Write(string message)
        {
            Console.Write(message);
        }

        public override void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public override void TraceData(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id, object data)
        {
            if (data is ChoLogEntry && Formatter != null)
                WriteLine(this.Formatter.Format(data as ChoLogEntry));

            base.TraceData(eventCache, source, eventType, id, data);
        }

        #endregion TraceListener Overrides
    }
}
