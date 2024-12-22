using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public static class IChoSynchronizeInvokeEx
    {
        // Extension method.
        public static void SynchronizedInvoke(this ISynchronizeInvoke sync, Action action)
        {
            if (sync == null) return;

            // If the invoke is not required, then invoke here and get out.
            if (!sync.InvokeRequired)
            {
                // Execute action.
                if (action != null)
                    action();

                // Get out.
                return;
            }

            // Marshal to the required context.
            sync.Invoke(action, new object[] { });
        }
    }
}
