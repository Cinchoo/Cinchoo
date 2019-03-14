using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Threading
{
    public static class ChoThreadEx
    {
        public static void AbortThread(this Thread thread)
        {
            if (thread == null ||
                !thread.IsAlive)
                return;

            try
            {
                thread.Abort();
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
        }
    }
}
