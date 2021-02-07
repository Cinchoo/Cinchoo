namespace Cinchoo.Core.Diagnostics
{
	#region NameSpaces

	using System;
    using System.Diagnostics;
using System.Collections.Generic;

    #endregion NameSpaces

	[Serializable]
	[DebuggerDisplay("Name={_name}")]
	public abstract class ChoBaseProfile : ChoProfileContainer, IChoProfile, IChoTrace, IChoSyncDisposable
    {
        #region Constants

		#endregion Constants

		#region Instance Data Members (Private)

		private readonly int _indent = 0;
		private readonly string _msg = "Elapsed time taken by `{0}` profile:";
		private readonly string _name;
        private readonly IChoProfile _outerProfile = null;
		private readonly IChoProfileBackingStore _profileBackingStore;
		private readonly bool _condition = ChoTraceSwitch.Switch.TraceVerbose;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly object _padLock = new object();
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly object _disposableLock = new object();

		private bool _started = false;
		//private bool IsDisposed = false;
		private DateTime _startTime = DateTime.Now;
		private DateTime _endTime = DateTime.Now;

		private bool _delayedStartProfile = false;

		#endregion Instance Data Members (Private)

		#region Shared Data Members (Private)

        //private static readonly bool ChoTraceSwitch.Switch.TraceVerbose = ChoTraceSwitch.Switch.TraceVerbose;

		#endregion Shared Data Members (Private)

		#region Constrctors

		static ChoBaseProfile()
		{
		}

		public ChoBaseProfile(string msg)
			: this(msg, (ChoBaseProfile)null)
		{
		}

        public ChoBaseProfile(string msg, IChoProfile outerProfile)
			: this(ChoTraceSwitch.Switch.TraceVerbose, ChoProfile.DEFAULT_PROFILE_NAME, msg, outerProfile)
		{
		}

		public ChoBaseProfile(string name, string msg)
			: this(ChoTraceSwitch.Switch.TraceVerbose, name, msg)
		{
		}

        public ChoBaseProfile(string name, string msg, IChoProfile outerProfile)
            : this(ChoTraceSwitch.Switch.TraceVerbose, name, msg, outerProfile)
        {
        }

		public ChoBaseProfile(bool condition, string name, string msg)
			: this(condition, name, msg, null)
		{
		}

        public ChoBaseProfile(bool condition, string name, string msg, IChoProfile outerProfile)
			: this(condition, name, msg, outerProfile, false, null, null)
		{
		}

        private readonly IChoProfile _parentProfile = null;
        private readonly bool _registered = false;

        internal ChoBaseProfile(bool condition, string name, string msg, IChoProfile outerProfile, bool delayedStartProfile, string startActions, string stopActions)
		{
			_condition = condition;
			_name = name.IsNullOrWhiteSpace() || name == ChoProfile.DEFAULT_PROFILE_NAME ? String.Format("Profile_{0}".FormatString(ChoRandom.NextRandom())) : name;
            if (outerProfile != ChoProfile.NULL)
                _outerProfile = outerProfile;
            if (_outerProfile == null)
				_profileBackingStore = ChoProfileBackingStoreManager.GetProfileBackingStore(name, startActions, stopActions);
			_delayedStartProfile = delayedStartProfile;
			if (_condition)
			{
				if (!msg.IsNullOrEmpty())
					_msg = msg;
				else if (!_name.IsNullOrEmpty())
					_msg = String.Format(_msg, _name);
				else
					_msg = ChoProfile.GetDefaultMsg(ChoStackTrace.GetStackFrame(GetType().Namespace));
			}

			if (ChoTraceSettings.Me.IndentProfiling)
			{
                if (outerProfile != null)
                {
                    if (outerProfile.ProfilerName != ChoProfile.GLOBAL_PROFILE_NAME && outerProfile.ProfilerName != ChoProfile.NULL_PROFILE_NAME)
                        _indent = outerProfile.Indent + 1;
                }
			}
			if (!_delayedStartProfile)
				StartIfNotStarted();

			if (_outerProfile is ChoProfileContainer)
            {
				((ChoProfileContainer)_outerProfile).Add(this);
                _parentProfile = outerProfile;
            }

            if (name != ChoProfile.GLOBAL_PROFILE_NAME && name != ChoProfile.NULL_PROFILE_NAME && name != ChoProfile.CURRENT_CONTEXT_PROFILE /* && outerProfile != null */)
            {
                ChoProfile.Register(this);
                _registered = true;
            }
		}

		internal ChoBaseProfile(string name, string msg, bool dummy)
		{
			_condition = true;
			_name = name;
			_msg = msg;
		}

		#endregion Constrctors

		#region IChoProfile Members (Public)

		public string ProfilerName
		{
			get { return _name; }
		}

		public string AppendLine(string msg)
		{
			return AppendLineIf(_condition, msg);
		}

		public string AppendLineIf(bool condition, string msg)
		{
			ChoGuard.IsDisposed(this);

			if (!condition)
				return msg;
			if (msg == null)
				return AppendIf(condition, Environment.NewLine);
			else
				return AppendIf(condition, msg + Environment.NewLine);
		}

		public string Append(string msg)
		{
			return AppendIf(_condition, msg);
		}

		public string AppendIf(bool condition, string msg)
		{
			ChoGuard.IsDisposed(this);

			if (!condition)
				return msg;

			StartIfNotStarted();

			Write(msg, _indent + 1);

			return msg;
		}

		public void AppendIf(bool condition, Exception ex)
		{
			if (condition)
				AppendLineIf(condition, ChoApplicationException.ToString(ex));
		}

		public void Append(Exception ex)
		{
			AppendIf(_condition, ex);
		}

		public string AppendIf(bool condition, string format, params object[] args)
		{
			return AppendIf(condition, String.Format(format, args));
		}

		public string Append(string format, params object[] args)
		{
			return Append(String.Format(format, args));
		}

		public string AppendLineIf(bool condition, string format, params object[] args)
		{
			return AppendLineIf(condition, String.Format(format, args));
		}

		public string AppendLine(string format, params object[] args)
		{
			return AppendLine(String.Format(format, args));
		}

		public TimeSpan ElapsedTimeTaken
		{
			get { return (_endTime - _startTime); }
		}

		public int Indent
		{
			get { return _indent; }
		}

		#endregion IChoProfile Members (Public)

		#region Instance Members (Protected)

		protected abstract void Flush();
		protected abstract void Write(string msg);
        //protected virtual void CloseBackingStore()
        //{
        //    if (_profileBackingStore != null)
        //        _profileBackingStore.Stop();
        //}

		protected override void Dispose(bool finalize)
		{
			try
			{
				StartIfNotStarted();

				Clear();
                if (_condition)
                {
                    if (_name != ChoProfile.GLOBAL_PROFILE_NAME && _name != ChoProfile.NULL_PROFILE_NAME)
                        Write(String.Format("}} [{0}] <---{1}", Convert.ToString(DateTime.Now - _startTime), Environment.NewLine), _indent);
                }

				Flush();

                if (_registered)
                    ChoProfile.Unregister(this);

                //CloseBackingStore();
			}
			catch (Exception ex)
			{
				ChoTrace.Write(ex);
			}
			finally
			{
				IsDisposed = true;
			}
		}

		protected void WriteToBackingStore(string msg)
		{
			if (_outerProfile != null)
				((ChoBaseProfile)_outerProfile).Write(msg);
			else if (_profileBackingStore != null)
                _profileBackingStore.Write(msg, Tag);
			else
				Trace.Write(msg);
		}

		#endregion Instance Members (Protected)

		#region Instance Members (Private)

		private void Write(string msg, int indent)
		{
			if (ChoTraceSettings.Me.IndentProfiling)
				msg = msg.Indent(indent);

			Write(msg);
		}

		private void StartIfNotStarted()
		{
			if (_started)
				return;

			lock (_padLock)
			{
				if (_started)
					return;

				_startTime = DateTime.Now;
				_started = true;

                if (_condition)
                {
                    if (_name != ChoProfile.GLOBAL_PROFILE_NAME && _name != ChoProfile.NULL_PROFILE_NAME)
                        Write(String.Format("{0} {{ [{1}]{2}", _msg, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"), Environment.NewLine), _indent);
                }
			}
		}

		#endregion Instance Members (Private)

		#region IChoTrace Members

		public void Debug(object message)
		{
			if (message != null)
				AppendLineIf(ChoTraceSwitch.Switch.TraceVerbose, message.ToString());
		}

		public void Debug(Exception exception)
		{
			AppendIf(ChoTraceSwitch.Switch.TraceVerbose, exception);
		}

		public void Debug(object message, Exception exception)
		{
			Debug(message);
			Debug(exception);
		}

		public void DebugFormat(string format, params object[] args)
		{
			AppendLineIf(ChoTraceSwitch.Switch.TraceVerbose, String.Format(format, args));
		}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			AppendLineIf(ChoTraceSwitch.Switch.TraceVerbose, String.Format(provider, format, args));
		}

		public void Error(object message)
		{
			if (message != null)
				AppendLineIf(ChoTraceSwitch.Switch.TraceError, message.ToString());
		}

		public void Error(Exception exception)
		{
			AppendIf(ChoTraceSwitch.Switch.TraceError, exception);
		}

		public void Error(object message, Exception exception)
		{
			Error(message);
			Error(exception);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			AppendLineIf(ChoTraceSwitch.Switch.TraceError, String.Format(format, args));
		}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			AppendLineIf(ChoTraceSwitch.Switch.TraceError, String.Format(provider, format, args));
		}

		public void Info(object message)
		{
			if (message != null)
				AppendLineIf(ChoTraceSwitch.Switch.TraceInfo, message.ToString());
		}

		public void Info(Exception exception)
		{
			AppendIf(ChoTraceSwitch.Switch.TraceInfo, exception);
		}

		public void Info(object message, Exception exception)
		{
			Info(message);
			Info(exception);
		}

		public void InfoFormat(string format, params object[] args)
		{
			AppendLineIf(ChoTraceSwitch.Switch.TraceInfo, String.Format(format, args));
		}

		public void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			AppendLineIf(ChoTraceSwitch.Switch.TraceInfo, String.Format(provider, format, args));
		}

		public void Warn(object message)
		{
			if (message != null)
				AppendLineIf(ChoTraceSwitch.Switch.TraceWarning, message.ToString());
		}

		public void Warn(Exception exception)
		{
			AppendIf(ChoTraceSwitch.Switch.TraceWarning, exception);
		}

		public void Warn(object message, Exception exception)
		{
			Warn(message);
			Warn(exception);
		}

		public void WarnFormat(string format, params object[] args)
		{
			AppendLineIf(ChoTraceSwitch.Switch.TraceWarning, String.Format(format, args));
		}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			AppendLineIf(ChoTraceSwitch.Switch.TraceWarning, String.Format(provider, format, args));
		}

		#endregion

		#region IChoSyncDisposable Members

		public object DisposableLockObj
		{
			get { return _disposableLock; }
		}

		#endregion

        public object Tag
        {
            get;
            set;
        }
	}
}
