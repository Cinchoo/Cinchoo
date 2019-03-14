namespace Cinchoo.Core.Diagnostics
{
	#region NameSpaces

	using System;
    using System.Diagnostics;

    #endregion NameSpaces

	[Serializable]
	[DebuggerDisplay("Name={_name}")]
	public abstract class ChoBaseProfile : ChoProfileContainer, IChoProfile, IChoTrace, IChoSyncDisposable
	{
		#region Constants

		private const string DEFAULT_NAME = "$GLOBAL";

		#endregion Constants

		#region Instance Data Members (Private)

		private readonly int _indent = 0;
		private readonly string _msg = "Elapsed time taken by `{0}` profile:";
		private readonly string _name;
		private readonly ChoBaseProfile _outerProfile = null;
		private readonly IChoProfileBackingStore _profileBackingStore;
		private readonly bool _condition = _defaultCondition;
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

		private static readonly bool _defaultCondition = ChoTrace.GetChoSwitch().TraceVerbose;

		#endregion Shared Data Members (Private)

		#region Constrctors

		static ChoBaseProfile()
		{
		}

		public ChoBaseProfile(string msg)
			: this(msg, (ChoBaseProfile)null)
		{
		}

		public ChoBaseProfile(string msg, ChoBaseProfile outerProfile)
			: this(_defaultCondition, DEFAULT_NAME, msg, outerProfile)
		{
		}

		public ChoBaseProfile(string name, string msg)
			: this(_defaultCondition, name, msg)
		{
		}

		public ChoBaseProfile(bool condition, string name, string msg)
			: this(condition, name, msg, null)
		{
		}

		private ChoBaseProfile(bool condition, string name, string msg, ChoBaseProfile outerProfile)
			: this(condition, name, msg, outerProfile, false, null, null)
		{
		}

		internal ChoBaseProfile(bool condition, string name, string msg, ChoBaseProfile outerProfile, bool delayedStartProfile, string startActions, string stopActions)
		{
			_condition = condition;
			_name = name;
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
					_indent = outerProfile.Indent + 1;
			}
			if (!_delayedStartProfile)
				StartIfNotStarted();

			if (_outerProfile is ChoProfileContainer)
				((ChoProfileContainer)_outerProfile).Add(this);
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
		protected virtual void CloseBackingStore()
		{
			if (_profileBackingStore != null)
				_profileBackingStore.Stop();
		}

		protected override void Dispose(bool finalize)
		{
			try
			{
				StartIfNotStarted();

				Clear();
				if (_condition)
					Write(String.Format("}} [{0}] <---{1}", Convert.ToString(DateTime.Now - _startTime), Environment.NewLine), _indent);

				Flush();
				CloseBackingStore();
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
				_outerProfile.Write(msg);
			else if (_profileBackingStore != null)
				_profileBackingStore.Write(msg);
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
					Write(String.Format("{0} {{ [{1}]{2}", _msg, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"), Environment.NewLine), _indent);
			}
		}

		#endregion Instance Members (Private)

		#region IChoTrace Members

		public void Debug(object message)
		{
			if (message != null)
				AppendLineIf(ChoTrace.ChoSwitch.TraceVerbose, message.ToString());
		}

		public void Debug(Exception exception)
		{
			AppendIf(ChoTrace.ChoSwitch.TraceVerbose, exception);
		}

		public void Debug(object message, Exception exception)
		{
			Debug(message);
			Debug(exception);
		}

		public void DebugFormat(string format, params object[] args)
		{
			AppendLineIf(ChoTrace.ChoSwitch.TraceVerbose, String.Format(format, args));
		}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			AppendLineIf(ChoTrace.ChoSwitch.TraceVerbose, String.Format(provider, format, args));
		}

		public void Error(object message)
		{
			if (message != null)
				AppendLineIf(ChoTrace.ChoSwitch.TraceError, message.ToString());
		}

		public void Error(Exception exception)
		{
			AppendIf(ChoTrace.ChoSwitch.TraceError, exception);
		}

		public void Error(object message, Exception exception)
		{
			Error(message);
			Error(exception);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			AppendLineIf(ChoTrace.ChoSwitch.TraceError, String.Format(format, args));
		}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			AppendLineIf(ChoTrace.ChoSwitch.TraceError, String.Format(provider, format, args));
		}

		public void Info(object message)
		{
			if (message != null)
				AppendLineIf(ChoTrace.ChoSwitch.TraceInfo, message.ToString());
		}

		public void Info(Exception exception)
		{
			AppendIf(ChoTrace.ChoSwitch.TraceInfo, exception);
		}

		public void Info(object message, Exception exception)
		{
			Info(message);
			Info(exception);
		}

		public void InfoFormat(string format, params object[] args)
		{
			AppendLineIf(ChoTrace.ChoSwitch.TraceInfo, String.Format(format, args));
		}

		public void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			AppendLineIf(ChoTrace.ChoSwitch.TraceInfo, String.Format(provider, format, args));
		}

		public void Warn(object message)
		{
			if (message != null)
				AppendLineIf(ChoTrace.ChoSwitch.TraceWarning, message.ToString());
		}

		public void Warn(Exception exception)
		{
			AppendIf(ChoTrace.ChoSwitch.TraceWarning, exception);
		}

		public void Warn(object message, Exception exception)
		{
			Warn(message);
			Warn(exception);
		}

		public void WarnFormat(string format, params object[] args)
		{
			AppendLineIf(ChoTrace.ChoSwitch.TraceWarning, String.Format(format, args));
		}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			AppendLineIf(ChoTrace.ChoSwitch.TraceWarning, String.Format(provider, format, args));
		}

		#endregion

		#region IChoSyncDisposable Members

		public object DisposableLockObj
		{
			get { return _disposableLock; }
		}

		#endregion
	}
}
