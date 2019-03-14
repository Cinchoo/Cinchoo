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

        private Timer _timer;
        private bool _paused = false;
        private int _timeout = DEFAULT_TIMER_CALLBACK_TIMEOUT;
        private bool _silent = false;

        private readonly Action<T> _timerServiceCallback;
        private readonly T _state;

        private readonly string _name;
        private readonly int? _intDueTime;
        private readonly int? _intPeriod;
        private readonly uint? _uintDueTime;
        private readonly uint? _uintPeriod;
        private readonly long? _longDueTime;
        private readonly long? _longPeriod;
        private readonly TimeSpan? _tsDueTime;
        private readonly TimeSpan? _tsPeriod;

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

        public ChoTimerService(string name, Action<T> timerServiceCallback, T state, int dueTime, int period, bool autoStart)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

            _name = name;
            _timerServiceCallback = timerServiceCallback;
            _state = state;
            _intDueTime = dueTime;
            _intPeriod = period;
            if (autoStart) Start();
        }

        public ChoTimerService(string name, Action<T> timerServiceCallback, T state, long dueTime, long period, bool autoStart)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

            _name = name;
            _timerServiceCallback = timerServiceCallback;
            _state = state;
            _longDueTime = dueTime;
            _longPeriod = period;
            if (autoStart) Start();
        }

        public ChoTimerService(string name, Action<T> timerServiceCallback, T state, TimeSpan dueTime, TimeSpan period, bool autoStart)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

            _name = name;
            _timerServiceCallback = timerServiceCallback;
            _state = state;
            _tsDueTime = dueTime;
            _tsPeriod = period;
            if (autoStart) Start();
        }

        [CLSCompliant(false)]
        public ChoTimerService(string name, Action<T> timerServiceCallback, T state, uint dueTime, uint period, bool autoStart)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

            _name = name;
            _timerServiceCallback = timerServiceCallback;
            _state = state;
            _uintDueTime = dueTime;
            _uintPeriod = period;
            if (autoStart) Start();
        }

        #endregion Constructors

        #region Instance Members (Private)

        private void OnTimerCallback(object state)
        {
			//if (ChoGuard.IsDisposed(this))
			//    return;

			Pause();
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
                    ChoProfile.GetContext(Name).AppendIf(ChoTrace.ChoSwitch.TraceError, String.Format("Timeout [{0} ms] elapsed prior to completion of the method.", _timeout));
            }
			//if (ChoGuard.IsDisposed(this))
			//    return;
			Continue();
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
                if (_timer != null)
                {
                    _paused = true;
                    _timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                }
            }
        }

        public void Continue()
        {
            ChoGuard.NotDisposed(this);

            lock (DisposableLockObj)
            {
                if (_timer != null)
                {
                    if (_paused)
                    {
                        _paused = false;

                        if (_intDueTime.HasValue)
                            _timer.Change(0, _intPeriod.Value);
                        else if (_longDueTime.HasValue)
                            _timer.Change(0, _longPeriod.Value);
                        else if (_tsDueTime.HasValue)
                            _timer.Change(TimeSpan.Zero, _tsPeriod.Value);
                        else if (_uintDueTime.HasValue)
                            _timer.Change(0, _uintPeriod.Value);
                        else
                            _timer.Change(0, DEFAULT_PERIOD);
                    }
                }
            }
        }

        public void Start()
        {
            ChoGuard.NotDisposed(this);

            lock (DisposableLockObj)
            {
                if (_timer != null) return;

                if (_intDueTime.HasValue)
                    _timer = new Timer(new TimerCallback(OnTimerCallback), _state, _intDueTime.Value, _intPeriod.Value);
                else if (_longDueTime.HasValue)
                    _timer = new Timer(new TimerCallback(OnTimerCallback), _state, _longDueTime.Value, _longPeriod.Value);
                else if (_tsDueTime.HasValue)
                    _timer = new Timer(new TimerCallback(OnTimerCallback), _state, _tsDueTime.Value, _tsPeriod.Value);
                else if (_uintDueTime.HasValue)
                    _timer = new Timer(new TimerCallback(OnTimerCallback), _state, _uintDueTime.Value, _uintPeriod.Value);
                else
                    _timer = new Timer(new TimerCallback(OnTimerCallback), _state, 0, DEFAULT_PERIOD);
            }
        }

        public void Stop()
        {
            if (ChoGuard.IsDisposed(this)) return;

            lock (DisposableLockObj)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
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
                    if (_timer != null)
                    {
						AutoResetEvent notifyObject = new AutoResetEvent(false);
                        //_timer.Dispose(notifyObject);
                        //notifyObject.WaitOne();
						_timer.Dispose();
                        _timer = null;
                    }
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
