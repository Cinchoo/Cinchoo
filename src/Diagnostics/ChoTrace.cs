namespace Cinchoo.Core.Diagnostics
{
	#region NameSpaces

    using System;
    using System.Diagnostics;
    using Cinchoo.Core;
    using Cinchoo.Core.Logging;

	#endregion NameSpaces

    public static class ChoTraceSwitch
    {
        public readonly static TraceSwitch Switch = new TraceSwitch("ChoSwitch", "Cinchoo Trace Switch", "Verbose");
        public readonly static TraceSwitch SettingsLogSwitch = new TraceSwitch("ChoSettingsLogSwitch", "Cinchoo Settings Log Switch", "Verbose");
    }

	[ChoAppDomainEventsRegisterableType]
	public class ChoTrace : IChoProfile, IChoTrace
	{
		#region Instance Data Members (Private)

		private string _name = ChoRandom.NextRandom().ToString();
		private bool _condition = ChoTraceSwitch.Switch.TraceVerbose;

		#endregion Instance Data Members (Private)

		#region Constants (Internal)

		internal const string BACKUP = "${{^BACKUP}}";
		internal const string SEPARATOR = "_______________________________________________";

		#endregion

		#region Constructors (Static)

		static ChoTrace()
		{
            ChoAppDomain.Initialize();
		}
		
		public ChoTrace(string msg)
			: this(ChoTraceSwitch.Switch.TraceVerbose, msg)
		{
		}

		public ChoTrace(bool condition, string msg)
		{
			ChoTrace.WriteLineIf(condition, msg);
		}

		#endregion

		#region Shared Trace Functions (Public)

		#region Backup Members

		public static void Backup()
		{
			try
			{
                ChoProfileBackupManager.Reset();
				Trace.Write(BACKUP);
			}
			catch (Exception ex)
			{
				ChoApplication.WriteToEventLog(ChoApplicationException.ToString(ex), EventLogEntryType.Error);
			}
		}

		#endregion Backup Members

		#region WriteNewLine Overloads

		public static void WriteNewLine()
		{
			WriteNewLineIf(true);
		}

		public static void WriteNewLineIf(bool condition)
		{
			WriteNewLineIf(condition, ChoStackTrace.GetStackFrame(typeof(ChoTrace)));
		}

		internal static void WriteNewLineIf(bool condition, StackFrame callerStackFrame)
		{
            //ChoLogger.Log(callerStackFrame, Environment.NewLine);
			try
			{
				Trace.WriteLineIf(condition, Environment.NewLine);
			}
			catch (Exception ex)
			{
				ChoApplication.WriteToEventLog(ChoApplicationException.ToString(ex), EventLogEntryType.Error);
			}
		}

		#endregion WriteNewLine Overloads

		#region Write Overloads

		public static void Write(string msg)
		{
			WriteIf(true, msg);
		}

		public static void Write(string format, params object[] args)
		{
			Write(String.Format(format, args));
		}

		public static void WriteIf(bool condition, string msg)
		{
			WriteIf(condition, ChoStackTrace.GetStackFrame(typeof(ChoTrace)), msg);
		}

		public static void WriteIf(bool condition, string format, params object[] args)
		{
			WriteIf(condition, String.Format(format, args));
		}

		internal static void WriteIf(bool condition, StackFrame callerStackFrame, string msg)
		{
			if (msg == null) return;
            //if (condition) ChoLogger.Log(callerStackFrame, msg);

			try
			{
				Trace.WriteIf(condition, msg);
			}
			catch (Exception ex)
			{
				ChoApplication.WriteToEventLog(ChoApplicationException.ToString(ex), EventLogEntryType.Error);
			}
		}

		internal static void WriteIf(bool condition, StackFrame callerStackFrame, string format, params object[] args)
		{
			WriteIf(condition, callerStackFrame, String.Format(format, args));
		}

		#endregion Write Overloads

		#region WriteLine Overloads

		public static void WriteLine(string msg)
		{
			WriteLineIf(true, msg);
		}

		public static void WriteLine(string format, params object[] args)
		{
			WriteLine(String.Format(format, args));
		}

		public static void WriteLineIf(bool condition, string msg)
		{
			WriteLineIf(condition, ChoStackTrace.GetStackFrame(typeof(ChoProfile)), msg);
		}

		public static void WriteLineIf(bool condition, string format, params object[] args)
		{
			WriteLineIf(condition, String.Format(format, args));
		}

		internal static void WriteLineIf(bool condition, string msg, StackFrame callerStackFrame)
		{
            //if (condition) ChoLogger.Log(callerStackFrame, msg + Environment.NewLine);
			try
			{
				Trace.WriteLineIf(condition, msg);
			}
			catch (Exception ex)
			{
				ChoApplication.WriteToEventLog(ChoApplicationException.ToString(ex), EventLogEntryType.Error);
			}
		}

		internal static void WriteLineIf(bool condition, StackFrame callerStackFrame, string format, params object[] args)
		{
			WriteLineIf(condition, args == null || args.Length == 0 ? format : String.Format(format, args), callerStackFrame);
		}

		#endregion WriteLine Overloads

		#region WriteSeparator Overloads

		public static void WriteSeparator()
		{
			WriteSeparatorIf(true);
		}

		public static void WriteSeparatorIf(bool condition)
		{
			WriteSeparatorIf(condition, ChoStackTrace.GetStackFrame(typeof(ChoTrace)));
		}

		internal static void WriteSeparatorIf(bool condition, StackFrame callerStackFrame)
		{
            //if (condition) ChoLogger.Log(callerStackFrame, SEPARATOR + Environment.NewLine);
			try
			{
				Trace.WriteLineIf(condition, SEPARATOR);
			}
			catch (Exception ex)
			{
				ChoApplication.WriteToEventLog(ChoApplicationException.ToString(ex), EventLogEntryType.Error);
			}
		}

		#endregion WriteSeparator Overloads

		#region Write Exception Overloads

		public static bool Write(Exception ex)
		{
        //    return Write(ex, ChoStackTrace.GetStackFrame(typeof(ChoTrace)));
        //}

        //internal static bool Write(Exception ex, StackFrame callerStackFrame)
        //{
			if (ex == null) return false;

			if (!ChoApplicationException.IsProcessed(ex))
			{
                //if (ChoTraceSwitch.Switch.TraceError)
                //    ChoLogger.Log(callerStackFrame, String.Format("[{0}]: {1}{2}", ChoStackTrace.GetCallerName(), ex.Message, Environment.NewLine));

				try
				{
					Trace.WriteLineIf(ChoTraceSwitch.Switch.TraceError, String.Format("[{0}]: {1}", ChoStackTrace.GetCallerName(), ChoApplicationException.ToString(ex)));
					return true;
				}
				catch (Exception exception)
				{
					ChoApplication.WriteToEventLog(ChoApplicationException.ToString(exception), EventLogEntryType.Error);
				}
				finally
				{
					ChoApplicationException.SetProcessed(ex);
				}
			}
			return false;
		}

		public static bool WriteNThrow(Exception ex)
		{
			if (ex == null) return false;

			Write(ex);
			throw new ChoApplicationException("Failed to write.", ex);
		}

		#endregion Write Exception Overloads

		#region Flush Members

		//[ChoAppDomainUnloadMethod("Flushing the trace...")]
		internal static void FlushAll()
		{
			foreach (TraceListener traceListener in Trace.Listeners)
			{
				if (traceListener == null) continue;
				traceListener.Flush();
			}
		}

		public static void Flush(string name)
		{
			foreach (TraceListener traceListener in Trace.Listeners)
			{
				if (traceListener == null) continue;
				if (traceListener.Name == name)
				{
					traceListener.Flush();
					break;
				}
			}
		}

		#endregion Flush Members

		#endregion

		#region IChoTrace Members

		public static void Debug(object message)
		{
			if (message != null)
				WriteLineIf(ChoTraceSwitch.Switch.TraceVerbose, message.ToString());
		}

		public static void Debug(Exception exception)
		{
			if (ChoTraceSwitch.Switch.TraceVerbose)
				Write(exception);
		}

		public static void Debug(object message, Exception exception)
		{
			Debug(message);
			Debug(exception);
		}

		public static void DebugFormat(string format, params object[] args)
		{
			WriteLineIf(ChoTraceSwitch.Switch.TraceVerbose, String.Format(format, args));
		}

		public static void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			WriteLineIf(ChoTraceSwitch.Switch.TraceVerbose, String.Format(provider, format, args));
		}

		public static void Error(object message)
		{
			if (message != null)
				WriteLineIf(ChoTraceSwitch.Switch.TraceError, message.ToString());
		}

		public static void Error(Exception exception)
		{
			if (ChoTraceSwitch.Switch.TraceError)
				Write(exception);
		}

		public static void Error(object message, Exception exception)
		{
			Error(message);
			Error(exception);
		}

		public static void ErrorFormat(string format, params object[] args)
		{
			WriteLineIf(ChoTraceSwitch.Switch.TraceError, String.Format(format, args));
		}

		public static void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			WriteLineIf(ChoTraceSwitch.Switch.TraceError, String.Format(provider, format, args));
		}

		public static void Info(object message)
		{
			if (message != null)
				WriteLineIf(ChoTraceSwitch.Switch.TraceInfo, message.ToString());
		}

		public static void Info(Exception exception)
		{
			if (ChoTraceSwitch.Switch.TraceInfo)
				Write(exception);
		}

		public static void Info(object message, Exception exception)
		{
			Info(message);
			Info(exception);
		}

		public static void InfoFormat(string format, params object[] args)
		{
			WriteLineIf(ChoTraceSwitch.Switch.TraceInfo, String.Format(format, args));
		}

		public static void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			WriteLineIf(ChoTraceSwitch.Switch.TraceInfo, String.Format(provider, format, args));
		}

		public static void Warn(object message)
		{
			if (message != null)
				WriteLineIf(ChoTraceSwitch.Switch.TraceWarning, message.ToString());
		}

		public static void Warn(Exception exception)
		{
			if (ChoTraceSwitch.Switch.TraceWarning)
				Write(exception);
		}

		public static void Warn(object message, Exception exception)
		{
			Warn(message);
			Warn(exception);
		}

		public static void WarnFormat(string format, params object[] args)
		{
			WriteLineIf(ChoTraceSwitch.Switch.TraceWarning, String.Format(format, args));
		}

		public static void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			WriteLineIf(ChoTraceSwitch.Switch.TraceWarning, String.Format(provider, format, args));
		}

		#endregion

		#region IChoProfile Members (Public)

		public string ProfilerName
		{
			get { return _name; }
		}

		public string AppendLine(string msg)
		{
			if (msg == null) return msg;
			return AppendLineIf(_condition, msg);
		}

		public string AppendLineIf(bool condition, string msg)
		{
			if (msg == null) return msg;
			return AppendIf(condition, msg + Environment.NewLine);
		}

		public string Append(string msg)
		{
			return AppendIf(_condition, msg);
		}

		public string AppendIf(bool condition, string msg)
		{
			if (msg == null || !condition) return msg;

			ChoTrace.WriteLine(msg);
			return msg;
		}

		public void AppendIf(bool condition, Exception ex)
		{
			if (condition) AppendLine(ChoApplicationException.ToString(ex));
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
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public int Indent
		{
			get { return 0; }
		}

		#endregion IChoProfile Members (Public)

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion

		#region IChoTrace Members

		void IChoTrace.Debug(object message)
		{
			if (message != null)
				AppendLineIf(ChoTraceSwitch.Switch.TraceVerbose, message.ToString());
		}

		void IChoTrace.Debug(Exception exception)
		{
			AppendIf(ChoTraceSwitch.Switch.TraceVerbose, exception);
		}

		void IChoTrace.Debug(object message, Exception exception)
		{
			Debug(message);
			Debug(exception);
		}

		void IChoTrace.DebugFormat(string format, params object[] args)
		{
			AppendLineIf(ChoTraceSwitch.Switch.TraceVerbose, String.Format(format, args));
		}

		void IChoTrace.DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			AppendLineIf(ChoTraceSwitch.Switch.TraceVerbose, String.Format(provider, format, args));
		}

		void IChoTrace.Error(object message)
		{
			if (message != null)
				AppendLineIf(ChoTraceSwitch.Switch.TraceError, message.ToString());
		}

		void IChoTrace.Error(Exception exception)
		{
			AppendIf(ChoTraceSwitch.Switch.TraceError, exception);
		}

		void IChoTrace.Error(object message, Exception exception)
		{
			Error(message);
			Error(exception);
		}

		void IChoTrace.ErrorFormat(string format, params object[] args)
		{
			AppendLineIf(ChoTraceSwitch.Switch.TraceError, String.Format(format, args));
		}

		void IChoTrace.ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			AppendLineIf(ChoTraceSwitch.Switch.TraceError, String.Format(provider, format, args));
		}

		void IChoTrace.Info(object message)
		{
			if (message != null)
				AppendLineIf(ChoTraceSwitch.Switch.TraceInfo, message.ToString());
		}

		void IChoTrace.Info(Exception exception)
		{
			AppendIf(ChoTraceSwitch.Switch.TraceInfo, exception);
		}

		void IChoTrace.Info(object message, Exception exception)
		{
			Info(message);
			Info(exception);
		}

		void IChoTrace.InfoFormat(string format, params object[] args)
		{
			AppendLineIf(ChoTraceSwitch.Switch.TraceInfo, String.Format(format, args));
		}

		void IChoTrace.InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			AppendLineIf(ChoTraceSwitch.Switch.TraceInfo, String.Format(provider, format, args));
		}

		void IChoTrace.Warn(object message)
		{
			if (message != null)
				AppendLineIf(ChoTraceSwitch.Switch.TraceWarning, message.ToString());
		}

		void IChoTrace.Warn(Exception exception)
		{
			AppendIf(ChoTraceSwitch.Switch.TraceWarning, exception);
		}

		void IChoTrace.Warn(object message, Exception exception)
		{
			Warn(message);
			Warn(exception);
		}

		void IChoTrace.WarnFormat(string format, params object[] args)
		{
			AppendLineIf(ChoTraceSwitch.Switch.TraceWarning, String.Format(format, args));
		}

		void IChoTrace.WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			AppendLineIf(ChoTraceSwitch.Switch.TraceWarning, String.Format(provider, format, args));
		}

		#endregion
	}
}
