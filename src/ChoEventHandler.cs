namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public static class ChoEventHandler
    {
        public static void Copy<T>(EventHandler<T> srcEventHandler, EventHandler<T> destEventHandler) where T: EventArgs
        {
            if (srcEventHandler == null) return;

            foreach (Delegate delegateMember in srcEventHandler.GetInvocationList())
            {
                destEventHandler += delegateMember as EventHandler<T>;
            }
        }

        public static void Copy<T>(EventHandler<T> srcEventHandler, Action<Delegate> callback) where T : EventArgs
        {
            if (srcEventHandler == null) return;

            ChoGuard.ArgumentNotNull(callback, "Callback");

            foreach (Delegate delegateMember in srcEventHandler.GetInvocationList())
            {
                if (callback != null)
                    callback(delegateMember);
            }
        }
    }
}
