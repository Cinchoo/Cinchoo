using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinchoo.Core.Diagnostics;
using System.Threading.Tasks;
using Cinchoo.Core.Services;
using System.IO;
using Cinchoo.Core.Xml;
using System.Xml;
using System.Windows.Forms;
using System.Windows;
using System.Threading;
using System.Security.Permissions;

namespace Cinchoo.Core
{
    public class ChoPlugInsRuntime : ChoSyncDisposableObject
    {
        #region Instance Members (Private)

        private ChoPlugIn _topPlugIn = null;
        private ChoPlugIn _nextPlugIn = null;
        private IChoAbortableAsyncResult _result;
        private ChoPlugInManagerSettings _plugInManagerSettings = new ChoPlugInManagerSettings();
        public ChoPlugIn ActivePlugIn
        {
            get;
            private set;
        }
        private volatile bool _isRunning = false;

        public event EventHandler<ChoEventArgs<IChoAbortableAsyncResult>> RunComplete;
        public event EventHandler<ChoPlugInRunEventArgs> BeforePlugInRun;
        public event EventHandler<ChoPlugInRunEventArgs> AfterPlugInRun;

        #endregion Instance Members (Private)

        #region Constructors

        public ChoPlugInsRuntime()
        {
        }

        public ChoPlugInsRuntime(IEnumerable<ChoPlugInBuilder> plugInBuilders)
        {
            Load(plugInBuilders);
        }

        #endregion Constructors

        #region Instance Members (Private)

        private void AddPlugIn(ChoPlugIn plugIn)
        {
            if (plugIn == null) return;
            Verify();

            //lock (_padLock)
            //{
                if (_topPlugIn == null)
                    _topPlugIn = _nextPlugIn = plugIn;
                else
                    _nextPlugIn = _nextPlugIn.ContinueWith(plugIn);

                //Point next plugin to the last item

                while (_nextPlugIn.Next != null)
                    _nextPlugIn = _nextPlugIn.Next;
            //}
        }

        private void Verify()
        {
            if (_isRunning)
                throw new ChoPlugInException("PlugIn is currently running.");
        }

        #endregion Instance Members (Private)

        #region Instance Members (Public)

        public void Load(IEnumerable<ChoPlugInBuilder> plugInBuilders)
        {
            //ChoGuard.ArgumentNotNullOrEmpty(plugInBuilders, "PlugInBuilders");
            Reset();

            foreach (ChoPlugInBuilder plugInBuilder in plugInBuilders)
            {
                if (plugInBuilder == null) continue;
                AddPlugIn(plugInBuilder.CreatePlugIn());
            }
        }

        public IEnumerable<ChoPlugIn> AsEnumerable()
        {
            if (_topPlugIn == null)
                yield break;

            foreach (ChoPlugIn p in _topPlugIn.AsEnumerable())
                yield return p;
        }

        public void Add(ChoPlugIn plugIn)
        {
            AddPlugIn(plugIn);
        }

        public void Reset()
        {
            Verify();
            _topPlugIn = _nextPlugIn = null;
            _result = null;
            _isRunning = false;
        }
        
        public void RunAsync(object value, object contextInfo = null)
        {
            RunAsync(value, contextInfo, OnPlugInRunComplete);
        }

        private void RunAsync(object value, object contextInfo = null, ChoAbortableAsyncCallback callback = null)
        {
            Verify();

            if (_topPlugIn == null) return;
            
            ActivePlugIn = null;
            _result = ChoAbortableQueuedExecutionService.Global.Enqueue(() =>
                {
                    if (_topPlugIn == null) return null;

                    _isRunning = true;
                    
                    //lock (_padLock)
                    //{
                        try
                        {
                            _topPlugIn.ContextInfo = contextInfo;
                            _topPlugIn.BeforeRun += OnBeforePlugInRun;
                            _topPlugIn.AfterRun += OnAfterPlugInRun;

                            return _topPlugIn.Run(value);
                        }
                        finally
                        {
                            //_topPlugIn.BeforeRun -= BeforePlugInRun;
                        }
                    //}
                },
                callback, null, -1);
        }

        private void OnPlugInRunComplete(IChoAbortableAsyncResult result)
        {
            _isRunning = false;
            RunComplete.Raise(this, new ChoEventArgs<IChoAbortableAsyncResult>(result));
        }

        public object Run(object value, object contextInfo = null)
        {
            RunAsync(value, contextInfo, null);

            try
            {
                IChoAbortableAsyncResult result = _result;
                if (result != null)
                    return result.EndInvoke();
            }
            finally
            {
                OnPlugInRunComplete(_result);
                _result = null;
            }

            return null;
        }

        private void OnBeforePlugInRun(object sender, ChoPlugInRunEventArgs e)
        {
            ActivePlugIn = sender as ChoPlugIn;
            BeforePlugInRun.Raise(sender, e);
        }

        private void OnAfterPlugInRun(object sender, ChoPlugInRunEventArgs e)
        {
            AfterPlugInRun.Raise(sender, e);
        }

        public void Cancel()
        {
            if (!_isRunning) return;

            try
            {
                IChoAbortableAsyncResult r1 = _result;
                if (r1 == null) return;

                if (_topPlugIn != null)
                    _topPlugIn.Stop();

                Thread.Sleep(_plugInManagerSettings.StopRequestTimeout);
                r1.Abort();
            }
            finally
            {
                _result = null;
            }
        }

        #endregion Instance Members (Public)

        #region IDisposable Member

        protected override void Dispose(bool finalize)
        {
        }

        #endregion IDisposable Member
    }
}
