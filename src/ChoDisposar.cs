namespace eSquare.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public static class ChoDisposar
    {
        public static void Dispose(IDisposable target, bool finalize)
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            if (target is IChoDisposable)
            {
                IChoDisposable disposableObj = target as IChoDisposable;
                if (disposableObj is IChoSyncDisposable && ((IChoSyncDisposable)disposableObj).DisposableLockObj != null)
                {
                    lock (((IChoSyncDisposable)disposableObj).DisposableLockObj)
                    {
                        Dispose(finalize, disposableObj);
                    }
                }
                else
                    Dispose(finalize, disposableObj);
            }
            else
                target.Dispose();
        }

        private static void Dispose(bool finalize, IChoDisposable disposableObj)
        {
            if (disposableObj.IsDisposed) return;
            disposableObj.IsDisposed = true;

            if (ChoType.HasMethod(disposableObj, "Dispose", new object[] { finalize }))
                ChoType.InvokeMethod(disposableObj, "Dispose", new object[] { finalize });

            //disposableObj.Dispose(finalize);
        }
    }
}
