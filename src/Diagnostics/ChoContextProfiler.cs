namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;

    using Cinchoo.Core;

    #endregion NameSpaces

    public class ChoContextProfiler : ChoSyncDisposableObject, IChoProfile, IChoTrace
	{
		#region Instance Data Members (Private)

		private object _instance;

		#endregion Instance Data Members (Private)

		#region Constructors

		protected ChoContextProfiler()
        {
			_instance = this;
        }

		internal ChoContextProfiler(object target)
		{
			_instance = target;
		}

        #endregion Constructors

        #region IChoProfile Members

		[ChoIgnoreMemberFormatter]
        public string ProfilerName
        {
			get { throw new NotSupportedException(); }
        }

		[ChoIgnoreMemberFormatter]
		public int Indent
        {
			get { throw new NotSupportedException(); }
		}

		[ChoIgnoreMemberFormatter]
		public TimeSpan ElapsedTimeTaken
        {
			get { throw new NotSupportedException(); }
		}

        public void AppendIf(bool condition, Exception ex)
        {
            ChoProfile.GetDefaultContext(_instance).AppendIf(condition, ex);
        }

        public void Append(Exception ex)
        {
            ChoProfile.GetDefaultContext(_instance).Append(ex);
        }

        public string AppendIf(bool condition, string msg)
        {
            return ChoProfile.GetDefaultContext(_instance).AppendIf(condition, msg);
        }

        public string AppendIf(bool condition, string format, params object[] args)
        {
            return ChoProfile.GetDefaultContext(_instance).AppendIf(condition, format, args);
        }

        public string Append(string msg)
        {
            return ChoProfile.GetDefaultContext(_instance).Append(msg);
        }

        public string Append(string format, params object[] args)
        {
            return ChoProfile.GetDefaultContext(_instance).Append(format, args);
        }

        public string AppendLineIf(bool condition, string msg)
        {
            return ChoProfile.GetDefaultContext(_instance).AppendLineIf(condition, msg);
        }

        public string AppendLineIf(bool condition, string format, params object[] args)
        {
            return ChoProfile.GetDefaultContext(_instance).AppendLineIf(condition, format, args);
        }

        public string AppendLine(string msg)
        {
            return ChoProfile.GetDefaultContext(_instance).AppendLine(msg);
        }

        public string AppendLine(string format, params object[] args)
        {
            return ChoProfile.GetDefaultContext(_instance).AppendLine(format, args);
        }

        #endregion

        #region IChoTrace Members

        public void Debug(object message)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).Debug(message);
        }

        public void Debug(Exception exception)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).Debug(exception);
        }

        public void Debug(object message, Exception exception)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).Debug(message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).DebugFormat(format, args);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).DebugFormat(provider, format, args);
        }

        public void Error(object message)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).Error(message);
        }

        public void Error(Exception exception)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).Error(exception);
        }

        public void Error(object message, Exception exception)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).Error(message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).ErrorFormat(format, args);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).ErrorFormat(provider, format, args);
        }

        public void Info(object message)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).Info(message);
        }

        public void Info(Exception exception)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).Info(exception);
        }

        public void Info(object message, Exception exception)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).Info(message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).InfoFormat(format, args);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).InfoFormat(provider, format, args);
        }

        public void Warn(object message)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).Warn(message);
        }

        public void Warn(Exception exception)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).Warn(exception);
        }

        public void Warn(object message, Exception exception)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).Warn(message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).WarnFormat(format, args);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            ((IChoTrace)ChoProfile.GetDefaultContext(_instance)).WarnFormat(provider, format, args);
        }

        #endregion

		protected override void Dispose(bool finalize)
		{
			ChoProfile.ResetContext();
		}
	}
}
