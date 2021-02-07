using Cinchoo.Core.Collections;
using Cinchoo.Core.Diagnostics;
using Cinchoo.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Cinchoo.Core
{
    public class ChoPlugInRunEventArgs : EventArgs
    {
        public object Value;
        public bool Cancel;
        public bool IsLastPlugIn;
    }

    public abstract class ChoPlugIn : ChoSyncDisposableObject
    {
        private ChoPlugIn _nextPlugIn = null;
        private bool _stopRequested = false;

        public string Name;
        public string Description;
        public bool DoPropertyResolve;
        public bool Enabled = true;

        private object _contextInfo;
        public object ContextInfo
        {
            get { return _contextInfo; }
            set
            {
                _contextInfo = value;
                if (Next != null)
                    Next._contextInfo = value;
            }
        }

        private event EventHandler<ChoPlugInRunEventArgs> _beforeRun;
        public event EventHandler<ChoPlugInRunEventArgs> BeforeRun
        {
            add
            {
                _beforeRun += value;
                if (Next != null)
                    Next.BeforeRun += value;
            }
            remove
            {
                _beforeRun -= value;
                if (Next != null)
                    Next.BeforeRun -= value;
            }
        }
        private event EventHandler<ChoPlugInRunEventArgs> _afterRun;
        public event EventHandler<ChoPlugInRunEventArgs> AfterRun
        {
            add
            {
                _afterRun += value;
                if (Next != null)
                    Next.AfterRun += value;
            }
            remove
            {
                _afterRun -= value;
                if (Next != null)
                    Next.AfterRun -= value;
            }
        }

        protected virtual void Validate()
        {
            ChoGuard.ArgumentNotNullOrEmpty(Name, "Name");
        }

        protected string ResolveText(string text)
        {
            if (DoPropertyResolve)
            {
                if (!text.IsNullOrWhiteSpace())
                    return text.ExpandProperties();
            }

            return text;
        }

        protected string ResolveFileText(ref string filePath)
        {
            if (DoPropertyResolve)
            {
                string tmpFileName = Path.ChangeExtension(ChoPath.GetTempFileName(), Path.GetExtension(filePath));
                File.WriteAllText(tmpFileName, File.ReadAllText(filePath).ExpandProperties());
                filePath = tmpFileName;
                ChoTrace.DebugFormat("{0}: Temp file created at '{1}'", Name, tmpFileName);
                return tmpFileName;
            }

            return null;
        }

        protected abstract object Execute(object value, out bool cancel);
        public object Run(object value)
        {
            try
            {
                if (Enabled)
                {
                    ChoPlugInRunEventArgs e = new ChoPlugInRunEventArgs() { Value = value, IsLastPlugIn = Next == null };
                    _beforeRun.Raise(this, e);
                    if (e.Cancel)
                        return e.Value;

                    value = e.Value;

                    if (_stopRequested)
                        throw new ChoPlugInStopRequestedException();

                    bool cancel = false;

                    Validate();
                    value = Execute(value, out cancel);
                    if (cancel) return value;

                    e = new ChoPlugInRunEventArgs() { Value = value, IsLastPlugIn = Next == null };
                    _afterRun.Raise(this, e);
                    if (cancel) return e.Value;

                    value = e.Value;
                }

                if (Next != null)
                    return Next.Run(value);
                else
                    return value;
            }
            finally
            {
                if (Enabled)
                {
                    CleanUp();
                }
            }
        }

        protected virtual void CleanUp()
        {
        }

        public ChoPlugIn ContinueWith(ChoPlugIn plugIn)
        {
            _nextPlugIn = plugIn;
            return _nextPlugIn;
        }

        public ChoPlugIn Next
        {
            get { return _nextPlugIn; }
        }

        public IEnumerable<ChoPlugIn> AsEnumerable()
        {
            ChoPlugIn next = this;
            yield return next;

            while (next.Next != null)
            {
                next = next.Next;
                yield return next;
            }
               
            yield break;
        }

        public virtual ChoPlugInBuilder CreateBuilder()
        {
            Type type = ChoType.GetTypeFromXmlSectionName(GetType().Name);
            if (type == null)
                throw new ChoPlugInException("Can't find builder for '{0}' plugin.".FormatString(GetType().Name));

            ChoPlugInBuilder builder = ChoActivator.CreateInstance(type) as ChoPlugInBuilder;
            if (builder == null)
                throw new ChoPlugInException("Can't find builder for '{0}' plugin.".FormatString(GetType().Name));

            InitializeBuilder(builder);
            return builder;
        }

        public virtual void InitializeBuilder(ChoPlugInBuilder builder)
        {
            if (builder == null) return;

            builder.Name = Name;
            builder.Description = Description;
            builder.DoPropertyResolve = DoPropertyResolve;
            builder.Enabled = Enabled;
        }

        public bool IsStopRequested()
        {
            return _stopRequested;
        }

        public void Stop()
        {
            _stopRequested = true;

            if (Next != null)
                Next.Stop();
        }

        protected override void Dispose(bool finalize)
        {
            CleanUp();
        }
    }
}
