namespace Cinchoo.Core.Diagnostics
{
	#region NameSpaces

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Cinchoo.Core.IO;

    #endregion NameSpaces

    [Serializable]
	[DebuggerDisplay("Name={_name}")]
	public class ChoBufferProfile : ChoBaseProfile
	{
		#region Instance Data Members (Private)

		private readonly StringBuilder _msg = new StringBuilder();

		#endregion Instance Data Members (Private)

		#region Constrctors

		static ChoBufferProfile()
		{
		}

		public ChoBufferProfile(string msg)
			: base(msg)
		{
		}

		public ChoBufferProfile(string msg, ChoBaseProfile outerProfile)
			: base(msg, outerProfile)
		{
		}

		public ChoBufferProfile(string name, string msg)
			: base(name, msg)
		{
		}

		public ChoBufferProfile(bool condition, string name, string msg)
			: base(condition, name, msg)
		{
		}

		internal ChoBufferProfile(bool condition, string name, string msg, ChoBaseProfile outerProfile, bool delayedStartProfile, string startActions, string stopActions)
			: base(condition, name, msg, outerProfile, delayedStartProfile, startActions, stopActions)
		{
		}

		#endregion Constrctors

		protected override void Flush()
		{
			WriteToBackingStore(_msg.ToString());
		}

		protected override void Write(string msg)
		{
			_msg.Append(msg);
		}
	}

	[Serializable]
	public class ChoBufferProfileEx : ChoProfileContainer, IChoProfile, IChoTrace, IChoDelayedStartProfile, IChoSyncDisposable
	{
		#region Instance Data Members (Private)

		private const ChoProfileIntializationAction DefaultProfileMode = ChoProfileIntializationAction.Truncate;

		private readonly object _padLock = new object();
		private bool _condition = ChoTrace.GetChoSwitch().TraceVerbose;
		private string _name = ChoRandom.NextRandom().ToString();
		private int _indent = 0;
		private DateTime _startTime = DateTime.Now;
		private bool _traceOnDispose = true;
		private IChoProfile _outerProfile = null;
		private StringBuilder _formattedMsg = new StringBuilder();
		private string _filePath;
		private ChoProfileIntializationAction _mode = DefaultProfileMode;
		private TextWriter _streamWriter;
		//private bool _canDispose = true;

		private bool _doneElapsedTime = false;
		private bool _delayedStartProfile = false;
		private bool _delayedAutoStart = false;
		private bool _started = false;

		#endregion

		#region Constructors

		protected ChoBufferProfileEx()
		{
		}

		public ChoBufferProfileEx(string msg)
			: this(ChoTrace.ChoSwitch.TraceVerbose, msg, (IChoProfile)null)
		{
		}

		public ChoBufferProfileEx(string filePath, string msg)
			: this(ChoTrace.ChoSwitch.TraceVerbose, filePath, msg, (IChoProfile)null)
		{
		}

		public ChoBufferProfileEx(TextWriter streamWriter, string msg)
			: this(ChoTrace.ChoSwitch.TraceVerbose, streamWriter, msg, (IChoProfile)null)
		{
		}

		public ChoBufferProfileEx(string format, params object[] args)
			: this(ChoTrace.ChoSwitch.TraceVerbose, String.Format(format, args), (IChoProfile)null)
		{
		}

		public ChoBufferProfileEx(string filePath, string format, params object[] args)
			: this(ChoTrace.ChoSwitch.TraceVerbose, filePath, String.Format(format, args), (IChoProfile)null)
		{
		}

		public ChoBufferProfileEx(TextWriter streamWriter, string format, params object[] args)
			: this(ChoTrace.ChoSwitch.TraceVerbose, streamWriter, String.Format(format, args), (IChoProfile)null)
		{
		}

		public ChoBufferProfileEx(bool condition, string msg)
			: this(condition, msg, (IChoProfile)null)
		{
		}

		public ChoBufferProfileEx(bool condition, string filePath, string msg)
			: this(condition, filePath, msg, (IChoProfile)null)
		{
		}

		public ChoBufferProfileEx(bool condition, TextWriter streamWriter, string msg)
			: this(condition, streamWriter, msg, (IChoProfile)null)
		{
		}

		public ChoBufferProfileEx(bool condition, string format, params object[] args)
			: this(condition, String.Format(format, args), (IChoProfile)null)
		{
		}

		public ChoBufferProfileEx(bool condition, string filePath, string format, params object[] args)
			: this(condition, filePath, String.Format(format, args), (IChoProfile)null)
		{
		}

		public ChoBufferProfileEx(bool condition, TextWriter streamWriter, string format, object[] args)
			: this(condition, streamWriter, String.Format(format, args), (IChoProfile)null)
		{
		}

		public ChoBufferProfileEx(string msg, IChoProfile outerProfile)
			: this(ChoTrace.ChoSwitch.TraceVerbose, msg, outerProfile)
		{
		}

		public ChoBufferProfileEx(string filePath, string msg, IChoProfile outerProfile)
			: this(ChoTrace.ChoSwitch.TraceVerbose, filePath, msg, outerProfile)
		{
		}

		public ChoBufferProfileEx(TextWriter streamWriter, string msg, IChoProfile outerProfile)
			: this(ChoTrace.ChoSwitch.TraceVerbose, streamWriter, msg, outerProfile)
		{
		}

		public ChoBufferProfileEx(string format, object arg, IChoProfile outerProfile)
			: this(ChoTrace.ChoSwitch.TraceVerbose, String.Format(format, new object[] { arg }), outerProfile)
		{
		}

		public ChoBufferProfileEx(string filePath, string format, object arg, IChoProfile outerProfile)
			: this(ChoTrace.ChoSwitch.TraceVerbose, filePath, String.Format(format, new object[] { arg }), outerProfile)
		{
		}

		public ChoBufferProfileEx(TextWriter streamWriter, string format, object arg, IChoProfile outerProfile)
			: this(ChoTrace.ChoSwitch.TraceVerbose, streamWriter, String.Format(format, new object[] { arg }), outerProfile)
		{
		}

		public ChoBufferProfileEx(string format, object[] args, IChoProfile outerProfile)
			: this(ChoTrace.ChoSwitch.TraceVerbose, String.Format(format, args), outerProfile)
		{
		}

		public ChoBufferProfileEx(string filePath, string format, object[] args, IChoProfile outerProfile)
			: this(ChoTrace.ChoSwitch.TraceVerbose, filePath, String.Format(format, args), outerProfile)
		{
		}

		public ChoBufferProfileEx(TextWriter streamWriter, string format, object[] args, IChoProfile outerProfile)
			: this(ChoTrace.ChoSwitch.TraceVerbose, streamWriter, String.Format(format, args), outerProfile)
		{
		}

		public ChoBufferProfileEx(bool condition, string format, object arg, IChoProfile outerProfile)
			: this(condition, String.Format(format, new object[] { arg }), outerProfile)
		{
		}

		public ChoBufferProfileEx(bool condition, string filePath, string format, object arg, IChoProfile outerProfile)
			: this(condition, filePath, String.Format(format, new object[] { arg }), outerProfile)
		{
		}

		public ChoBufferProfileEx(bool condition, TextWriter streamWriter, string format, object arg, IChoProfile outerProfile)
			: this(condition, streamWriter, String.Format(format, new object[] { arg }), outerProfile)
		{
		}

		public ChoBufferProfileEx(bool condition, string format, object[] args, IChoProfile outerProfile)
			: this(condition, String.Format(format, args), outerProfile)
		{
		}

		public ChoBufferProfileEx(bool condition, string filePath, string format, object[] args, IChoProfile outerProfile)
			: this(condition, filePath, String.Format(format, args), outerProfile)
		{
		}

		public ChoBufferProfileEx(bool condition, TextWriter streamWriter, string format, object[] args, IChoProfile outerProfile)
			: this(condition, streamWriter, String.Format(format, args), outerProfile)
		{
		}

		public ChoBufferProfileEx(bool condition, string msg, IChoProfile outerProfile)
			: this(condition, msg, outerProfile, false)
		{
		}

		public ChoBufferProfileEx(bool condition, string filePath, string msg, IChoProfile outerProfile)
			: this(condition, filePath, msg, outerProfile, false)
		{
		}

		public ChoBufferProfileEx(bool condition, TextWriter streamWriter, string msg, IChoProfile outerProfile)
			: this(condition, null, DefaultProfileMode, streamWriter, msg, outerProfile, false)
		{
		}

		internal ChoBufferProfileEx(bool condition, string name, TextWriter streamWriter, string msg, IChoProfile outerProfile, bool register)
			: this(condition, name, null, DefaultProfileMode, streamWriter, msg, outerProfile, register)
		{
		}

		internal ChoBufferProfileEx(bool condition, string msg, IChoProfile outerProfile, bool register)
			: this(condition, null, DefaultProfileMode, (TextWriter)null, msg, outerProfile, register)
		{
		}

		internal ChoBufferProfileEx(bool condition, string filePath, string msg, IChoProfile outerProfile, bool register)
			: this(condition, filePath, DefaultProfileMode, (TextWriter)null, msg, outerProfile, register)
		{
		}

		internal ChoBufferProfileEx(bool condition, string name, string filePath, ChoProfileIntializationAction mode, string msg, IChoProfile outerProfile, bool register)
			: this(condition, name, filePath, mode, (TextWriter)null, msg, null, register)
		{
		}
		
		internal ChoBufferProfileEx(bool condition, string filePath, ChoProfileIntializationAction mode, TextWriter streamWriter, string msg, IChoProfile outerProfile, bool register)
			: this(condition, null, filePath, mode, streamWriter, msg, outerProfile, register)
		{
		}

		internal ChoBufferProfileEx(bool condition, string name, string filePath, ChoProfileIntializationAction mode, TextWriter streamWriter, string msg, IChoProfile outerProfile, bool register)
		{
			_condition = condition;
			_mode = mode;
			FilePath = String.IsNullOrEmpty(filePath) ? filePath : ChoString.ExpandProperties(filePath);
			TextWriter = streamWriter;
			//OuterProfile = outerProfile;

			if (String.IsNullOrEmpty(msg))
				msg = ChoProfile.GetDefaultMsg(ChoStackTrace.GetStackFrame(GetType().Namespace));

			msg = ChoString.ExpandProperties(msg);

			if (_condition)
				_formattedMsg.AppendFormat("{0} {{{1}", msg, Environment.NewLine);

			if (register)
				ChoProfile.Register(name, this, ChoStackTrace.GetStackFrame(GetType().Namespace), false);

			//if (OuterProfile is ChoProfileContainer)
			//    ((ChoProfileContainer)OuterProfile).Add(this);
		}

		#endregion
		
		#region IDisposable Overrides

		public new virtual void Dispose()
		{
			ChoObjectDisposar.Dispose(this, false);
		}

		#endregion IDisposable Overrides

		#region Finalizers

		~ChoBufferProfileEx()
		{
			//Dispose(true);
			ChoObjectDisposar.Dispose(this, true);
		}

		#endregion Finalizers

		//#region ChoProfileContainer Overrides

		//internal override IChoProfile OuterProfile
		//{
		//    get { return _outerProfile; }
		//    set
		//    {
		//        if (value != null)
		//        {
		//            if (!String.IsNullOrEmpty(FilePath))
		//            {
		//                //if (value is ChoBufferProfile)
		//                //{
		//                //    ChoBufferProfile fileOuterProfile = value as ChoBufferProfile;
		//                //    if (fileOuterProfile.FilePath != FilePath) return;
		//                //}
		//                //else if (value is ChoBufferProfile)
		//                //{
		//                //    ChoBufferProfile outerBufferProfile = value as ChoBufferProfile;
		//                //    if (outerBufferProfile.FilePath != FilePath) return;
		//                //}
		//            }

		//            if (_outerProfile == null)
		//                _indent = value.Indent + 1;
		//            _outerProfile = value;
		//            _traceOnDispose = false;
		//        }
		//    }
		//}

		//#endregion ChoProfileContainer Overrides

		#region IChoProfile Members (Public)
		
		public virtual string ProfilerName
		{
			get { return _name; }
		}

		public virtual string AppendLine(string msg)
		{
			if (msg == null) return msg;
			return AppendLineIf(_condition, msg);
		}

		public virtual string AppendLineIf(bool condition, string msg)
		{
			if (msg == null || !condition) return msg;
			return AppendIf(condition, msg + Environment.NewLine);
		}

		public virtual string Append(string msg)
		{
			return AppendIf(_condition, msg);
		}

		public virtual string AppendIf(bool condition, string msg)
		{
			if (msg == null || !condition) return msg;

			lock (_padLock)
			{
				if (_delayedStartProfile)
				{
					if (_delayedAutoStart)
						Start();
					else if (!_started)
						return msg;
				}

				if (ChoTraceSettings.Me.IndentProfiling)
					_formattedMsg.Append(msg.Indent(1));
				else
					_formattedMsg.Append(msg);

				return msg;
			}
		}

		public virtual void AppendIf(bool condition, Exception ex)
		{
			if (condition) AppendLineIf(condition, ChoApplicationException.ToString(ex));
		}

		public virtual void Append(Exception ex)
		{
			AppendIf(_condition, ex);
		}

		public virtual string AppendIf(bool condition, string format, params object[] args)
		{
			return AppendIf(condition, String.Format(format, args));
		}

		public virtual string Append(string format, params object[] args)
		{
			return Append(String.Format(format, args));
		}

		public virtual string AppendLineIf(bool condition, string format, params object[] args)
		{
			return AppendLineIf(condition, String.Format(format, args));
		}

		public virtual string AppendLine(string format, params object[] args)
		{
			return AppendLine(String.Format(format, args));
		}

		public virtual TimeSpan ElapsedTimeTaken
		{
			get { return DateTime.Now - _startTime; }
		}

		public virtual int Indent
		{
			get { return _indent; }
		}

        [CLSCompliant(false)]
		public ChoTimeSpanFormat ElapsedTimeFormat
		{
			get;
			set;
		}

		public string ElapsedTimeFormatString
		{
			get;
			set;
		}

		#endregion IChoProfile Members (Public)

		#region Object Overrides (Public)

		public virtual new string ToString()
		{
			if (_delayedStartProfile && !_started) return String.Empty;

			if (!_doneElapsedTime)
			{
				_doneElapsedTime = true;
				//_traceOnDispose = false;

				if (_condition)
				{
					if (ElapsedTimeFormatString.IsNullOrEmpty())
					{
						if (ChoTraceSettings.Me.IndentProfiling)
							_formattedMsg.AppendFormat("}} [{0}] <---{1}", (DateTime.Now - _startTime).ToString(ElapsedTimeFormat), Environment.NewLine);
						else
							_formattedMsg.AppendFormat(": [{0}] <---{1}", (DateTime.Now - _startTime).ToString(ElapsedTimeFormat), Environment.NewLine);
					}
					else
					{
						if (ChoTraceSettings.Me.IndentProfiling)
							_formattedMsg.AppendFormat("}} [{0}] <---{1}", (DateTime.Now - _startTime).ToString(ElapsedTimeFormatString), Environment.NewLine);
						else
							_formattedMsg.AppendFormat(": [{0}] <---{1}", (DateTime.Now - _startTime).ToString(ElapsedTimeFormatString), Environment.NewLine);
					}
				}
			}

			return _formattedMsg.ToString();
		}

		#endregion Object Overrides (Public)

		#region Instance Members (Public)

		public virtual void Start()
		{
			if (_outerProfile != null && _outerProfile is IChoDelayedStartProfile)
				((IChoDelayedStartProfile)_outerProfile).Start();

			if (_started) return;
			if (_delayedStartProfile)
			{
				lock (this)
				{
					_startTime = DateTime.Now;
					_started = true;
				}
			}
		}

		public virtual void Clean()
		{
			if (_streamWriter == null || String.IsNullOrEmpty(FilePath)) return;
			ChoFile.Clean(FilePath);
		}

		#endregion Instance Members (Public)

		#region Properties (Public)

		public virtual bool IsDelayedStart
		{
			get { return false; }
		}

		public virtual bool IsDelayedAutoStart
		{
			get { return false; }
		}

		protected virtual string FilePath
		{
			get { return _filePath; }
			set 
			{
				_filePath = null;
				if (String.IsNullOrEmpty(value)) return;

				if (String.IsNullOrEmpty(Path.GetDirectoryName(value)))
					_filePath = ChoFileProfileSettings.GetFullPath(ChoReservedDirectoryName.Others, value);
				else
					_filePath = ChoFileProfileSettings.GetFullPath(value);
			}
		}

		protected virtual TextWriter TextWriter
		{
			get { return _streamWriter; }
			private set
			{
				if (value != null && value is StreamWriter && ((StreamWriter)value).BaseStream is FileStream)
					_filePath = (((StreamWriter)value).BaseStream as FileStream).Name;
				else
					_streamWriter = value;
			}
		}

		#endregion Properties (Public)

		#region Instance Properties (Internal)

		internal virtual bool TraceOnDispose
		{
			get { return _traceOnDispose; }
		}

		internal virtual bool Condition
		{
			get { return _condition; }
		}

		internal virtual bool DelayedStartProfile
		{
			get { return _delayedStartProfile; }
		}

		internal virtual bool Started
		{
			get { return _started; }
		}

		internal virtual ChoProfileIntializationAction Mode
		{
			get { return _mode; }
		}

		#endregion Instance Properties (Internal)

		#region IChoTrace Members

		public virtual void Debug(object message)
		{
			if (message != null)
				AppendLineIf(ChoTrace.ChoSwitch.TraceVerbose, message.ToString());
		}

		public virtual void Debug(Exception exception)
		{
			AppendIf(ChoTrace.ChoSwitch.TraceVerbose, exception);
		}

		public virtual void Debug(object message, Exception exception)
		{
			Debug(message);
			Debug(exception);
		}

		public virtual void DebugFormat(string format, params object[] args)
		{
			AppendLineIf(ChoTrace.ChoSwitch.TraceVerbose, String.Format(format, args));
		}

		public virtual void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			AppendLineIf(ChoTrace.ChoSwitch.TraceVerbose, String.Format(provider, format, args));
		}

		public virtual void Error(object message)
		{
			if (message != null)
				AppendLineIf(ChoTrace.ChoSwitch.TraceError, message.ToString());
		}

		public virtual void Error(Exception exception)
		{
			AppendIf(ChoTrace.ChoSwitch.TraceError, exception);
		}

		public virtual void Error(object message, Exception exception)
		{
			Error(message);
			Error(exception);
		}

		public virtual void ErrorFormat(string format, params object[] args)
		{
			AppendLineIf(ChoTrace.ChoSwitch.TraceError, String.Format(format, args));
		}

		public virtual void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			AppendLineIf(ChoTrace.ChoSwitch.TraceError, String.Format(provider, format, args));
		}

		public virtual void Info(object message)
		{
			if (message != null)
				AppendLineIf(ChoTrace.ChoSwitch.TraceInfo, message.ToString());
		}

		public virtual void Info(Exception exception)
		{
			AppendIf(ChoTrace.ChoSwitch.TraceInfo, exception);
		}

		public virtual void Info(object message, Exception exception)
		{
			Info(message);
			Info(exception);
		}

		public virtual void InfoFormat(string format, params object[] args)
		{
			AppendLineIf(ChoTrace.ChoSwitch.TraceInfo, String.Format(format, args));
		}

		public virtual void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			AppendLineIf(ChoTrace.ChoSwitch.TraceInfo, String.Format(provider, format, args));
		}

		public virtual void Warn(object message)
		{
			if (message != null)
				AppendLineIf(ChoTrace.ChoSwitch.TraceWarning, message.ToString());
		}

		public virtual void Warn(Exception exception)
		{
			AppendIf(ChoTrace.ChoSwitch.TraceWarning, exception);
		}

		public virtual void Warn(object message, Exception exception)
		{
			Warn(message);
			Warn(exception);
		}

		public virtual void WarnFormat(string format, params object[] args)
		{
			AppendLineIf(ChoTrace.ChoSwitch.TraceWarning, String.Format(format, args));
		}

		public virtual void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			AppendLineIf(ChoTrace.ChoSwitch.TraceWarning, String.Format(provider, format, args));
		}

		#endregion

		#region IChoSyncDisposable Members

		private readonly object _syncRoot = new object();
		public virtual object DisposableLockObj
		{
			get { return _syncRoot; }
		}

		//private bool IsDisposed = false;
		public new virtual bool IsDisposed
		{
			get { return base.IsDisposed; }
			set { /*IsDisposed = value; */}
		}

		protected override void Dispose(bool finalize)
		{
			if (!finalize)
				return;

			try
			{
				Clear();
				if (!DelayedStartProfile || (DelayedStartProfile && Started))
				{
					if (TraceOnDispose)
					{
						if (Condition)
						{
							string msg = Environment.NewLine + ToString().Indent(Indent - 1);
							if (!String.IsNullOrEmpty(FilePath))
							{
								//if (Mode == ChoProfileIntializationAction.Roll)
								//    ChoBufferProfile.RollNWriteLine(FilePath, msg);
								//else if (Mode == ChoProfileIntializationAction.Truncate)
								//    ChoBufferProfile.CleanNWriteLine(FilePath, msg);
								//else
								//    ChoBufferProfile.WriteLine(FilePath, msg);
							}
							else if (TextWriter != null)
							{
								TextWriter.Write(msg);
								TextWriter.Flush();
							}
							else
								ChoTrace.WriteIf(Condition, ChoStackTrace.GetStackFrame(GetType()), msg);
						}
					}
					//else if (OuterProfile != null)
					//    OuterProfile.Append(ToString());
				}

				//ChoProfile.Unregister(CanDispose);

				//if (!finalize)
				//    GC.SuppressFinalize(this);
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

		#endregion

		#region Shared Members (Public)

		public static ChoBufferProfileEx DelayedStart(ChoBufferProfileEx bufferProfile)
		{
			return new ChoDelayedStartBufferProfile(bufferProfile);
		}

		public static ChoBufferProfileEx DelayedAutoStart(ChoBufferProfileEx bufferProfile)
		{
			return new ChoDelayedAutoStartBufferProfile(bufferProfile);
		}

		#endregion Shared Members (Public)

		#region ChoDelayedStartBufferProfile Class

		private class ChoDelayedStartBufferProfile : ChoBufferProfileEx, IDisposable
		{
			#region Instance Data Members (Private)

			private ChoBufferProfileEx _bufferProfile;

			#endregion Instance Data Members (Private)

			#region Constructors

			public ChoDelayedStartBufferProfile(ChoBufferProfileEx profile)
			{
				ChoGuard.ArgumentNotNull(profile, "Profile");
				_bufferProfile = profile;
				_bufferProfile._delayedStartProfile = true;
			}

			#endregion Constructors

			//#region ChoProfileContainer Overrides

			//internal override IChoProfile OuterProfile
			//{
			//    get { return _bufferProfile.OuterProfile; }
			//    set { _bufferProfile.OuterProfile = value; }
			//}

			//#endregion ChoProfileContainer Overrides

			#region IChoProfile Members (Public)

			public override string ProfilerName
			{
				get { return _bufferProfile.ProfilerName; }
			}

			public override string AppendLine(string msg)
			{
				return _bufferProfile.AppendLine(msg);
			}

			public override string AppendLineIf(bool condition, string msg)
			{
				return _bufferProfile.AppendLineIf(condition, msg);
			}

			public override string Append(string msg)
			{
				return _bufferProfile.Append(msg);
			}

			public override string AppendIf(bool condition, string msg)
			{
				return _bufferProfile.AppendIf(condition, msg);
			}

			public override void AppendIf(bool condition, Exception ex)
			{
				_bufferProfile.AppendIf(condition, ex);
			}

			public override void Append(Exception ex)
			{
				_bufferProfile.Append(ex);
			}

			public override string AppendIf(bool condition, string format, params object[] args)
			{
				return AppendIf(condition, String.Format(format, args));
			}

			public override string Append(string format, params object[] args)
			{
				return Append(String.Format(format, args));
			}

			public override string AppendLineIf(bool condition, string format, params object[] args)
			{
				return AppendLineIf(condition, String.Format(format, args));
			}

			public override string AppendLine(string format, params object[] args)
			{
				return AppendLine(String.Format(format, args));
			}

			public override TimeSpan ElapsedTimeTaken
			{
				get { return _bufferProfile.ElapsedTimeTaken; }
			}

			public override int Indent
			{
				get { return _bufferProfile.Indent; }
			}

			#endregion IChoProfile Members (Public)

			#region Object Overrides (Public)

			public override string ToString()
			{
				return _bufferProfile.ToString();
			}

			#endregion Object Overrides (Public)

			#region Properties (Public)

			public override bool IsDelayedStart
			{
				get { return true; }
			}

			public override bool IsDelayedAutoStart
			{
				get { return _bufferProfile.IsDelayedAutoStart; }
			}

			protected override string FilePath
			{
				get { return _bufferProfile.FilePath; }
				set { _bufferProfile.FilePath = value; }
			}

			protected override TextWriter TextWriter
			{
				get { return _bufferProfile.TextWriter; }
			}

			#endregion Properties (Public)

			#region Instance Properties (Internal)

			internal override bool TraceOnDispose
			{
				get { return _bufferProfile.TraceOnDispose; }
			}

			internal override bool Condition
			{
				get { return _bufferProfile.Condition; }
			}

			internal override bool DelayedStartProfile
			{
				get { return _bufferProfile.DelayedStartProfile; }
			}

			internal override bool Started
			{
				get { return _bufferProfile.Started; }
			}

			internal override ChoProfileIntializationAction Mode
			{
				get { return _bufferProfile.Mode; }
			}

			public override object DisposableLockObj
			{
				get { return _bufferProfile.DisposableLockObj; }
			}

			public override bool IsDisposed
			{
				get { return _bufferProfile.IsDisposed; }
				set { _bufferProfile.IsDisposed = value; }
			}

			#endregion Instance Properties (Internal)

			#region IChoTrace Members

			public override void Debug(object message)
			{
				_bufferProfile.Debug(message);
			}

			public override void Debug(Exception exception)
			{
				_bufferProfile.Debug(exception);
			}

			public override void Debug(object message, Exception exception)
			{
				_bufferProfile.Debug(message, exception);
			}

			public override void DebugFormat(string format, params object[] args)
			{
				_bufferProfile.DebugFormat(format, args);
			}

			public override void DebugFormat(IFormatProvider provider, string format, params object[] args)
			{
				_bufferProfile.DebugFormat(provider, format, args);
			}

			public override void Error(object message)
			{
				_bufferProfile.Error(message);
			}

			public override void Error(Exception exception)
			{
				_bufferProfile.Error(exception);
			}

			public override void Error(object message, Exception exception)
			{
				_bufferProfile.Error(message, exception);
			}

			public override void ErrorFormat(string format, params object[] args)
			{
				_bufferProfile.ErrorFormat(format, args);
			}

			public override void ErrorFormat(IFormatProvider provider, string format, params object[] args)
			{
				_bufferProfile.ErrorFormat(provider, format, args);
			}

			public override void Info(object message)
			{
				_bufferProfile.Info(message);
			}

			public override void Info(Exception exception)
			{
				_bufferProfile.Info(exception);
			}

			public override void Info(object message, Exception exception)
			{
				_bufferProfile.Info(message, exception);
			}

			public override void InfoFormat(string format, params object[] args)
			{
				_bufferProfile.InfoFormat(format, args);
			}

			public override void InfoFormat(IFormatProvider provider, string format, params object[] args)
			{
				_bufferProfile.InfoFormat(provider, format, args);
			}

			public override void Warn(object message)
			{
				_bufferProfile.Warn(message);
			}

			public override void Warn(Exception exception)
			{
				_bufferProfile.Warn(exception);
			}

			public override void Warn(object message, Exception exception)
			{
				_bufferProfile.Warn(message, exception);
			}

			public override void WarnFormat(string format, params object[] args)
			{
				_bufferProfile.WarnFormat(format, args);
			}

			public override void WarnFormat(IFormatProvider provider, string format, params object[] args)
			{
				_bufferProfile.WarnFormat(provider, format, args);
			}

			#endregion

			#region Instance Members (Public)

			public override void Start()
			{
				_bufferProfile.Start();
			}

			public override void Clean()
			{
				_bufferProfile.Clean();
			}

			#endregion Instance Members (Public)

			#region ChoProfileContainer Overrides

			internal override void Add(IChoProfile profile)
			{
				_bufferProfile.Add(profile);
			}

			internal override void Clear()
			{
				_bufferProfile.Clear();
			}

			internal override void Remove(IChoProfile profile)
			{
				_bufferProfile.Remove(profile);
			}

			#endregion ChoProfileContainer Overrides

			#region IDisposable Members

			public override void Dispose()
			{
				_bufferProfile.Dispose();
			}

			protected override void Dispose(bool finalize)
			{
				_bufferProfile.Dispose(finalize);
			}

			#endregion

			#region Finalizer

			~ChoDelayedStartBufferProfile()
			{
				//_bufferProfile.Dispose(true);
				ChoObjectDisposar.Dispose(_bufferProfile, false);
			}

			#endregion Finalizer
		}

		#endregion ChoDelayedStartBufferProfile Class

		#region ChoDelayedAutoStartBufferProfile Class

		private class ChoDelayedAutoStartBufferProfile : ChoBufferProfileEx, IDisposable
		{
			#region Instance Data Members (Private)

			private ChoBufferProfileEx _bufferProfile;

			#endregion Instance Data Members (Private)

			#region Constructors

			public ChoDelayedAutoStartBufferProfile(ChoBufferProfileEx profile)
			{
				ChoGuard.ArgumentNotNull(profile, "Profile");
				_bufferProfile = profile;
				_bufferProfile._delayedStartProfile = true;
				_bufferProfile._delayedAutoStart = true;
			}

			#endregion Constructors

			//#region ChoProfileContainer Overrides

			//internal override IChoProfile OuterProfile
			//{
			//    get { return _bufferProfile.OuterProfile; }
			//    set { _bufferProfile.OuterProfile = value; }
			//}

			//#endregion ChoProfileContainer Overrides

			#region IChoProfile Members (Public)

			public override string ProfilerName
			{
				get { return _bufferProfile.ProfilerName; }
			}

			public override string AppendLine(string msg)
			{
				return _bufferProfile.AppendLine(msg);
			}

			public override string AppendLineIf(bool condition, string msg)
			{
				return _bufferProfile.AppendLineIf(condition, msg);
			}

			public override string Append(string msg)
			{
				return _bufferProfile.Append(msg);
			}

			public override string AppendIf(bool condition, string msg)
			{
				return _bufferProfile.AppendIf(condition, msg);
			}

			public override void AppendIf(bool condition, Exception ex)
			{
				_bufferProfile.AppendIf(condition, ex);
			}

			public override void Append(Exception ex)
			{
				_bufferProfile.Append(ex);
			}

			public override string AppendIf(bool condition, string format, params object[] args)
			{
				return AppendIf(condition, String.Format(format, args));
			}

			public override string Append(string format, params object[] args)
			{
				return Append(String.Format(format, args));
			}

			public override string AppendLineIf(bool condition, string format, params object[] args)
			{
				return AppendLineIf(condition, String.Format(format, args));
			}

			public override string AppendLine(string format, params object[] args)
			{
				return AppendLine(String.Format(format, args));
			}

			public override TimeSpan ElapsedTimeTaken
			{
				get { return _bufferProfile.ElapsedTimeTaken; }
			}

			public override int Indent
			{
				get { return _bufferProfile.Indent; }
			}

			#endregion IChoProfile Members (Public)

			#region Object Overrides (Public)

			public override string ToString()
			{
				return _bufferProfile.ToString();
			}

			#endregion Object Overrides (Public)

			#region Properties (Public)

			public override bool IsDelayedStart
			{
				get { return _bufferProfile.IsDelayedStart; }
			}

			public override bool IsDelayedAutoStart
			{
				get { return true; }
			}

			protected override string FilePath
			{
				get { return _bufferProfile.FilePath; }
				set { _bufferProfile.FilePath = value; }
			}

			protected override TextWriter TextWriter
			{
				get { return _bufferProfile.TextWriter; }
			}


			#endregion Properties (Public)

			#region Instance Properties (Internal)

			internal override bool TraceOnDispose
			{
				get { return _bufferProfile.TraceOnDispose; }
			}

			internal override bool Condition
			{
				get { return _bufferProfile.Condition; }
			}

			internal override bool DelayedStartProfile
			{
				get { return _bufferProfile.DelayedStartProfile; }
			}

			internal override bool Started
			{
				get { return _bufferProfile.Started; }
			}

			internal override ChoProfileIntializationAction Mode
			{
				get { return _bufferProfile.Mode; }
			}

			public override object DisposableLockObj
			{
				get { return _bufferProfile.DisposableLockObj; }
			}

			public override bool IsDisposed
			{
				get { return _bufferProfile.IsDisposed; }
				set { _bufferProfile.IsDisposed = value; }
			}

			#endregion Instance Properties (Internal)

			#region IChoTrace Members

			public override void Debug(object message)
			{
				_bufferProfile.Debug(message);
			}

			public override void Debug(Exception exception)
			{
				_bufferProfile.Debug(exception);
			}

			public override void Debug(object message, Exception exception)
			{
				_bufferProfile.Debug(message, exception);
			}

			public override void DebugFormat(string format, params object[] args)
			{
				_bufferProfile.DebugFormat(format, args);
			}

			public override void DebugFormat(IFormatProvider provider, string format, params object[] args)
			{
				_bufferProfile.DebugFormat(provider, format, args);
			}

			public override void Error(object message)
			{
				_bufferProfile.Error(message);
			}

			public override void Error(Exception exception)
			{
				_bufferProfile.Error(exception);
			}

			public override void Error(object message, Exception exception)
			{
				_bufferProfile.Error(message, exception);
			}

			public override void ErrorFormat(string format, params object[] args)
			{
				_bufferProfile.ErrorFormat(format, args);
			}

			public override void ErrorFormat(IFormatProvider provider, string format, params object[] args)
			{
				_bufferProfile.ErrorFormat(provider, format, args);
			}

			public override void Info(object message)
			{
				_bufferProfile.Info(message);
			}

			public override void Info(Exception exception)
			{
				_bufferProfile.Info(exception);
			}

			public override void Info(object message, Exception exception)
			{
				_bufferProfile.Info(message, exception);
			}

			public override void InfoFormat(string format, params object[] args)
			{
				_bufferProfile.InfoFormat(format, args);
			}

			public override void InfoFormat(IFormatProvider provider, string format, params object[] args)
			{
				_bufferProfile.InfoFormat(provider, format, args);
			}

			public override void Warn(object message)
			{
				_bufferProfile.Warn(message);
			}

			public override void Warn(Exception exception)
			{
				_bufferProfile.Warn(exception);
			}

			public override void Warn(object message, Exception exception)
			{
				_bufferProfile.Warn(message, exception);
			}

			public override void WarnFormat(string format, params object[] args)
			{
				_bufferProfile.WarnFormat(format, args);
			}

			public override void WarnFormat(IFormatProvider provider, string format, params object[] args)
			{
				_bufferProfile.WarnFormat(provider, format, args);
			}

			#endregion

			#region ChoProfileContainer Overrides

			internal override void Add(IChoProfile profile)
			{
				_bufferProfile.Add(profile);
			}

			internal override void Clear()
			{
				_bufferProfile.Clear();
			}

			internal override void Remove(IChoProfile profile)
			{
				_bufferProfile.Remove(profile);
			}

			#endregion ChoProfileContainer Overrides

			#region Instance Members (Public)

			public override void Start()
			{
				_bufferProfile.Start();
			}

			public override void Clean()
			{
				_bufferProfile.Clean();
			}

			#endregion Instance Members (Public)

			#region IDisposable Members

			public override void Dispose()
			{
				_bufferProfile.Dispose();
			}

			protected override void Dispose(bool finalize)
			{
				_bufferProfile.Dispose(finalize);
			}

			#endregion

			#region Finalizer

			~ChoDelayedAutoStartBufferProfile()
			{
				//_bufferProfile.Dispose(true);
				ChoObjectDisposar.Dispose(_bufferProfile, false);
			}

			#endregion Finalizer
		}

		#endregion ChoDelayedAutoStartBufferProfile Class
	}
}
