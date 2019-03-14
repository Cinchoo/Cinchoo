//namespace Cinchoo.Core.Diagnostics
//{
//    #region NameSpaces

//    using System;
//    using System.Text;
//    using System.Collections.Generic;

//    using Cinchoo.Core.Diagnostics.Attributes;
//    using Cinchoo.Core.Exceptions;
//    using Cinchoo.Core.Collections.Generic;
//    using System.Threading;
//    using Cinchoo.Core.Threading;
//    using Cinchoo.Core.Attributes;

//    #endregion NameSpaces

//    public abstract class ChoContextProfiler : ChoSyncDisposableObject, IChoProfile, IChoTrace
//    {
//        #region Instance Data Members (Private)

//        private Lazy<IChoProfile> _lzProfiler;
//        private Lazy<IChoTrace> _lzTracer;

//        #endregion Instance Data Members (Private)

//        #region Constructors

//        public ChoContextProfiler()
//        {
//            _lzProfiler = new Lazy<IChoProfile>(() => ChoProfile.GetDefaultContext(this), true);
//            _lzTracer = new Lazy<IChoTrace>(() => _lzProfiler.Value as IChoTrace, true);
//        }

//        #endregion Constructors

//        #region IChoProfile Members

//        [ChoMemberFormatterIgnore]
//        public string ProfilerName
//        {
//            get { return _lzProfiler.Value != null ? _lzProfiler.Value.ProfilerName : null; }
//        }

//        [ChoMemberFormatterIgnore]
//        public int Indent
//        {
//            get { return _lzProfiler.Value != null ? _lzProfiler.Value.Indent : -1; }
//        }

//        [ChoMemberFormatterIgnore]
//        public string ElapsedTimeTaken
//        {
//            get { return _lzProfiler.Value != null ? _lzProfiler.Value.ElapsedTimeTaken : null; }
//        }

//        public IChoProfile Profile
//        {
//            get { return ChoProfile.GetDefaultContext(this); }
//        }

//        public void AppendIf(bool condition, Exception ex)
//        {
//            _lzProfiler.Value.AppendIf(condition, ex);
//        }

//        public void Append(Exception ex)
//        {
//            _lzProfiler.Value.Append(ex);
//        }

//        public string AppendIf(bool condition, string msg)
//        {
//            return _lzProfiler.Value.AppendIf(condition, msg);
//        }

//        public string AppendIf(bool condition, string format, params object[] args)
//        {
//            return _lzProfiler.Value.AppendIf(condition, format, args);
//        }

//        public string Append(string msg)
//        {
//            return _lzProfiler.Value.Append(msg);
//        }

//        public string Append(string format, params object[] args)
//        {
//            return _lzProfiler.Value.Append(format, args);
//        }

//        public string AppendLineIf(bool condition, string msg)
//        {
//            return _lzProfiler.Value.AppendLineIf(condition, msg);
//        }

//        public string AppendLineIf(bool condition, string format, params object[] args)
//        {
//            return _lzProfiler.Value.AppendLineIf(condition, format, args);
//        }

//        public string AppendLine(string msg)
//        {
//            return _lzProfiler.Value.AppendLine(msg);
//        }

//        public string AppendLine(string format, params object[] args)
//        {
//            return _lzProfiler.Value.AppendLine(format, args);
//        }

//        #endregion

//        #region IChoTrace Members

//        public void Debug(object message)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.Debug(message);
//        }

//        public void Debug(Exception exception)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.Debug(exception);
//        }

//        public void Debug(object message, Exception exception)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.Debug(message, exception);
//        }

//        public void DebugFormat(string format, params object[] args)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.DebugFormat(format, args);
//        }

//        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.DebugFormat(provider, format, args);
//        }

//        public void Error(object message)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.Error(message);
//        }

//        public void Error(Exception exception)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.Error(exception);
//        }

//        public void Error(object message, Exception exception)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.Error(message, exception);
//        }

//        public void ErrorFormat(string format, params object[] args)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.ErrorFormat(format, args);
//        }

//        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.ErrorFormat(provider, format, args);
//        }

//        public void Info(object message)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.Info(message);
//        }

//        public void Info(Exception exception)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.Info(exception);
//        }

//        public void Info(object message, Exception exception)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.Info(message, exception);
//        }

//        public void InfoFormat(string format, params object[] args)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.InfoFormat(format, args);
//        }

//        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.InfoFormat(provider, format, args);
//        }

//        public void Warn(object message)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.Warn(message);
//        }

//        public void Warn(Exception exception)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.Warn(exception);
//        }

//        public void Warn(object message, Exception exception)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.Warn(message, exception);
//        }

//        public void WarnFormat(string format, params object[] args)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.WarnFormat(format, args);
//        }

//        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
//        {
//            if (_lzTracer.Value != null)
//                _lzTracer.Value.WarnFormat(provider, format, args);
//        }

//        #endregion

//        protected override void Dispose(bool finalize)
//        {
//            if (_lzProfiler.IsValueCreated)
//                _lzProfiler.Value.Dispose();
//            //ChoProfile.Dispose(_lzProfiler.Value);
//        }
//    }
//}
