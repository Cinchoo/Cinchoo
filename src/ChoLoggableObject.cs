namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

    using System;
    using System.Diagnostics;
    using System.IO;
    using Cinchoo.Core.IO;
    using Cinchoo.Core.Diagnostics;

	#endregion NameSpaces

	public abstract class ChoLoggableObject : IDisposable
	{
		#region Instance Properties

		private string _logDirectory;
		public string LogDirectory
		{
			get { return _logDirectory; }
			set
			{
				if (!value.IsNullOrWhiteSpace())
				{
					_logDirectory = value.Trim();
					if (!Path.IsPathRooted(_logDirectory))
					{
						_logDirectory = Path.Combine(ChoApplication.ApplicationLogDirectory, _logDirectory);
                        if (XmlLogCondition)
                            Directory.CreateDirectory(_logDirectory);
					}
				}
				else
					_logDirectory = ChoReservedDirectoryName.Settings;
			}
		}

        private string _logFileName;
		public string LogFileName
		{
			get
			{
				return _logFileName;
			}
			set
			{
				if (!value.IsNullOrWhiteSpace())
					_logFileName = Path.GetFileName(value.Trim());
			}
		}

		public bool? LogCondition
		{
			get;
			set;
		}

        internal bool XmlLogCondition
        {
            get
            {
                if (LogCondition == null)
                    return ChoTraceSwitch.SettingsLogSwitch.TraceVerbose;
                else
                    return LogCondition.Value;
            }
        }

		private string _logTimeStampFormat;
		public string LogTimeStampFormat
		{
			get { return _logTimeStampFormat; }
			set
			{
				if (value.IsNullOrWhiteSpace())
                    _logTimeStampFormat = ChoGlobalApplicationSettings.Me.LogSettings.LogTimeStampFormat;
				else
					_logTimeStampFormat = value;
			}
		}

		#endregion Instance Properties

        #region Constructors

        static ChoLoggableObject()
        {
            ChoFramework.Initialize();
        }

        public ChoLoggableObject()
        {
            _logFileName = ChoPath.AddExtension(GetType().FullName, ChoReservedFileExt.Log);
        }

        #endregion Constructors

        public void Log(string msg)
		{
			Log(XmlLogCondition, msg);
		}

		public virtual void Log(bool condition, string msg)
		{
			if (condition)
			{
                try
                {
                    //TODO: Rework
                    ChoFile.WriteLine(Path.Combine(LogDirectory, LogFileName), String.Format("{1}{0}{2}", Environment.NewLine,
                        DateTime.Now.ToString(LogTimeStampFormat), msg.ToString()));
                }
                catch (Exception ex)
                {
                    ChoApplication.WriteToEventLog(ChoApplicationException.ToString(ex), EventLogEntryType.Error);
                }
			}
		}

        #region IDisposable Members

		protected virtual void Dispose(bool finalize)
        {
        }

		public virtual void Dispose()
        {
            Dispose(false);
        }

        #endregion

        #region Finalizer

        ~ChoLoggableObject()
        {
            Dispose(true);
        }

        #endregion Finalizer
	}
}
