namespace Cinchoo.Core.Services
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Threading;
    using System.Reflection;
    using System.Diagnostics;
    using System.Collections.Generic;

    using Cinchoo.Core.Collections;
    using Cinchoo.Core.Diagnostics;

    #endregion NameSpaces

    public class ChoTimerService<T> : ChoSyncDisposableObject
    {
        #region Delegate

        //public delegate void ChoTimerServiceCallback(T state);

        #endregion Delegate

        #region Constants (Private)

        private const int DEFAULT_PERIOD = 1000;
        private const int DEFAULT_TIMER_CALLBACK_TIMEOUT = -1; //5000;

        #endregion Constants (Private)

        #region Instance Data Members (Private)

        private bool _isPaused = false;
        private bool _isStopped = false;
        private int _timeout = DEFAULT_TIMER_CALLBACK_TIMEOUT;
        private bool _silent = false;

        private readonly Action<T> _timerServiceCallback;
        private readonly T _state;

        private readonly string _name;
        private readonly int _period;
        private readonly Thread _timerThread;
        private readonly AutoResetEvent _resumeEvent = new AutoResetEvent(false);

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoTimerService(string name) : this(name, null, true)
        {
        }
        
        public ChoTimerService(string name, bool autoStart) : this(name, null, autoStart)
        {
        }

        public ChoTimerService(string name, Action<T> timerServiceCallback)
            : this(name, timerServiceCallback, true)
        {
        }

        public ChoTimerService(string name, Action<T> timerServiceCallback, bool autoStart)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

            _name = name;
            _timerServiceCallback = timerServiceCallback;
            if (autoStart) Start();
        }

        public ChoTimerService(string name, Action<T> timerServiceCallback, T state, int period, bool autoStart)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

            _name = name;
            _timerServiceCallback = timerServiceCallback;
            _state = state;
            _period = period;
            _timerThread = new Thread(OnTimerCallback);
            _timerThread.IsBackground = true;
            if (autoStart) Start();
        }

        #endregion Constructors

        #region Instance Members (Private)

        private void OnTimerCallback(object state)
        {
            if (_isStopped || _isPaused)
                _resumeEvent.WaitOne();

            try
            {
                if (_timerServiceCallback != null)
                    _timerServiceCallback.Run((T)state, _timeout);
                //new Action<T>(OnTimerServiceCallback).WaitFor((T)state, _timeout);
            }
            catch (TimeoutException ex)
            {
                if (!_silent)
                    throw new ChoTimerServiceException(String.Format("{1}: Timeout [{0} ms] elapsed prior to completion of the method.", _timeout, _name), ex);
                else
                    ChoTrace.Error(String.Format("Timeout [{0} ms] elapsed prior to completion of the method.", _timeout));
            }

            Thread.Sleep(_period);
        }

        #endregion Instance Members (Private)

        #region Instance Members (Public)

        //public virtual void OnTimerServiceCallback(T state)
        //{
        //    if (_timerServiceCallback != null)
        //        _timerServiceCallback(state);
        //    else
        //        throw new NotSupportedException();
        //}

        public void Pause()
        {
            ChoGuard.NotDisposed(this);

            lock (DisposableLockObj)
            {
                _isPaused = true;
            }
        }

        public void Continue()
        {
            ChoGuard.NotDisposed(this);

            lock (DisposableLockObj)
            {
                _isPaused = false;
                _resumeEvent.Set();
            }
        }

        public void Start()
        {
            ChoGuard.NotDisposed(this);

            lock (DisposableLockObj)
            {
                if (!_timerThread.IsAlive)
                    _timerThread.Start();
                else
                {
                    _isStopped = false;
                    _resumeEvent.Set();
                }
            }
        }

        public void Stop()
        {
            if (ChoGuard.IsDisposed(this)) return;

            lock (DisposableLockObj)
            {
                _isStopped = true;
            }
        }

        #endregion Instance Members (Public)

        #region Instance Properties (Public)

        public string Name
        {
            get { return _name; }
        }

        public bool Silent
        {
            get { return _silent; }
            set { _silent = value; }
        }

        public int Timeout
        {
            get { return _timeout; }
            set 
            {
                if (value != System.Threading.Timeout.Infinite && value <= 0)
                    throw new ChoTimerServiceException("Timeout value should be > 0");

                _timeout = value; 
            }
        }

        #endregion Instance Properties (Public)

        #region ChoSyncDisposableObject Overrides

        protected override void Dispose(bool finalize)
        {
            try
            {
                if (!finalize)
                {
                }
            }
            finally
            {
                IsDisposed = true;
            }
        }

        #endregion ChoSyncDisposableObject Overrides
    }
}
