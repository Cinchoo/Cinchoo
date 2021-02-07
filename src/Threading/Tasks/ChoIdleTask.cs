namespace Cinchoo.Core.Threading.Tasks
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    #endregion NameSpaces

    public class ChoIdleTask : ChoSyncDisposableObject
    {
        #region Instance Data Members (Private)

        private readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private readonly Thread _idleThread;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoIdleTask() : this(null)
        {
        }

        public ChoIdleTask(string name)
        {
            _idleThread = new Thread(() => _waitHandle.WaitOne());
            _idleThread.Name = name.IsNullOrWhiteSpace() ? "ChoIdleTask_{0}".FormatString(ChoRandom.NextRandom()) : name;
            _idleThread.IsBackground = false;
        }

        #endregion Constructors

        #region Instance Members (Public)

        public void Start()
        {
            ChoGuard.NotDisposed(this);

            _idleThread.Start();
        }

        public void Stop()
        {
            ChoGuard.NotDisposed(this);

            _waitHandle.Set();
            Thread.Sleep(100);
            if (_idleThread.IsAlive)
            {
                _idleThread.AbortThread();
            }
        }

        #endregion Instance Members (Public)

        #region IDisposable Overrides

        protected override void Dispose(bool finalize)
        {
            try
            {
                if (_idleThread.IsAlive)
                {
                    _idleThread.AbortThread();
                }
            }
            catch { }
        }

        #endregion IDisposable Overrides
    }
}
