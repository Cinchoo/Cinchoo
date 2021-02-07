namespace Cinchoo.Core.Services
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.IO;

    #endregion NameSpaces

    public enum ChoTimerMethodExecutionStatus { Success, Error, TimedOut }

    public struct ChoTimerMethodExecutionState
    {
        #region Instance Data Members (Private)

        private ChoTimerMethodExecutionStatus _status;
        private Exception _exception;
        private object _state;

        #endregion Instance Data Members (Private)

        #region Constructors

        internal ChoTimerMethodExecutionState(ChoTimerMethodExecutionStatus status, Exception exception, object state)
        {
            _status = status;
            _exception = exception;
            _state = state;
        }

        #endregion Constructors

        #region Instance Properties (Public)

        public ChoTimerMethodExecutionStatus Status
        {
            get { return _status; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        public object State
        {
            get { return _state; }
        }

        #endregion Instance Properties (Public)
    }

    //Uregister automatically when the callback is idle for 10 times
    [ChoAppDomainEventsRegisterableType]
    public static class ChoGlobalTimerServiceManager
    {
        private class ChoGlobalTimerServiceData
        {
            #region Instance Data Members

            public readonly string Name;
            public readonly long Period;
            public readonly Action TimerServiceStartMethod;
            public readonly Action<ChoTimerMethodExecutionState> TimerServiceCallbackMethod;

            private long _countdownPeriod;

            #endregion Instance Data Members

            #region Constructors

            public ChoGlobalTimerServiceData(string name, Action timerServiceStartMethod, long period)
                : this(name, timerServiceStartMethod, null, period)
            {
            }

            public ChoGlobalTimerServiceData(string name, Action timerServiceStartMethod,
                Action<ChoTimerMethodExecutionState> timerServiceCallback, long period)
            {
                Name = name;
                TimerServiceStartMethod = timerServiceStartMethod;
                TimerServiceCallbackMethod = timerServiceCallback;
                _countdownPeriod = Period = (long)Math.Round(period / (double)ChoGlobalTimerServiceManager.DEFAULT_PERIOD) * ChoGlobalTimerServiceManager.DEFAULT_PERIOD;
            }

            #endregion Constructors

            #region Instance Members (Public)

            public bool IsPeriodElapsed()
            {
                if (Period == System.Threading.Timeout.Infinite) return false;

                _countdownPeriod = _countdownPeriod - ChoGlobalTimerServiceManager.DEFAULT_PERIOD;
                
                bool periodElapsed = _countdownPeriod == 0;
                if (periodElapsed)
                    _countdownPeriod = Period;

                return periodElapsed;
            }

            #endregion Instance Members (Public)

            internal void Run()
            {
                if (TimerServiceCallbackMethod == null)
                    TimerServiceStartMethod();
                else
                {
                }
            }
        }

        #region Constants (Private)

        private const int DEFAULT_PERIOD = 100;

        #endregion Constants (Private)

        #region Shared Data Members (Private)

        private readonly static Dictionary<string, ChoGlobalTimerServiceData> _callbacks = new Dictionary<string, ChoGlobalTimerServiceData>();
        private readonly static object _padLock = new object();
        private static ChoTimerService<object> _timerService;

        #endregion Shared Data Members (Private)

        #region Constructors

        static ChoGlobalTimerServiceManager()
        {
            ChoFramework.Initialize();
        }

        #endregion Constructors

        #region Shared Members (Public)

        public static void Register(string name, Action timerServiceCallback, long period)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");
            ChoGuard.ArgumentNotNull(timerServiceCallback, "TimerServiceCallback");

            if (period != Timeout.Infinite && period <= 0)
                throw new ChoTimerServiceException("Period should be > 0");

            lock (_padLock)
            {
                if (_callbacks.ContainsKey(name))
                {
                    //TODO: log the duplicate entries
                    return;
                }
                
                _callbacks.Add(name, new ChoGlobalTimerServiceData(name, timerServiceCallback, period));
            }
        }

        public static bool Contains(string name)
        {
            lock (_padLock)
            {
                return _callbacks.ContainsKey(name);
            }
        }

        public static void Unregister(string name)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");
            
            lock (_padLock)
            {
                if (!_callbacks.ContainsKey(name)) return;
                _callbacks.Remove(name);
            }
        }

        [ChoAppDomainUnloadMethod("Shutting down the Global Timer Service...")]
        private static void Shutdown()
        {
            lock (_padLock)
            {
                if (_timerService != null)
                {
                    _timerService.Dispose();
                    _timerService = null;
                }
            }
        }

        #endregion Shared Members (Public)

        #region Shared Members (Private)

        [ChoAppDomainLoadMethod("Starting Global Timer Service....")]
        private static void Init()
        {
            _timerService = new ChoTimerService<object>("GlobalTimer", OnTimerServiceCallback, null, DEFAULT_PERIOD, false);
            _timerService.Silent = true;
            _timerService.Timeout = Timeout.Infinite;
            _timerService.Start();
        }

        private static void CheckState()
        {
            lock (_padLock)
            {
                if (_timerService == null)
                    throw new ChoTimerServiceException("Service is not running now.");
            }
        }

        private static void OnTimerServiceCallback(object state)
        {
            lock (_padLock)
            {
                try
                {
                    foreach (ChoGlobalTimerServiceData globalTimerServiceData in _callbacks.Values)
                    {
                        if (globalTimerServiceData.IsPeriodElapsed())
                        {
                            try
                            {
                                globalTimerServiceData.Run();
                            }
                            catch (ChoFatalApplicationException)
                            {
                                throw;
                            }
                            catch (Exception ex)
                            {
                                ChoTrace.Error(ex);
                                //ChoProfile.RegisterIfNotExists(globalTimerServiceData.Name, new ChoBufferProfileEx(ChoFileProfileSettings.GetFullPath(ChoReservedDirectoryName.Others,
                                //    ChoPath.AddExtension(typeof(ChoGlobalTimerServiceManager).FullName, ChoReservedFileExt.Err)),
                                //    "Errors found..."));
                                //ChoProfile.GetContext(globalTimerServiceData.Name).AppendIf(ChoTraceSwitch.Switch.TraceError, ex);
                            }
                        }
                    }
                }
                catch (TimeoutException tex)
                {
                    ChoTrace.Error(tex);
                }
            }
        }

        #endregion Shared Members (Private)
    }
}
