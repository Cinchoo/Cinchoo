namespace Cinchoo.Core.Logging
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Security;
    using System.Threading;
    using System.Diagnostics;
    using System.Globalization;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using System.Security.Permissions;

    using Cinchoo.Core.Factory;
    using System.Reflection;
    using Cinchoo.Core.Diagnostics;

    #endregion NameSpaces

    [Serializable]
    [XmlRoot("logEntry")]
    public class ChoLogEntry : ICloneable, IChoObjectInitializable
    {
        #region Instance Data Members (Public)

        private DateTime _timeStamp = DateTime.Now;
        private string _message;
        private ICollection<string> _categories = new List<string>(new string[] { "Default" });
        private TraceEventType _severity = TraceEventType.Information;

        //private TraceEventType
        private IDictionary<string, object> _additionalProperties;

        #endregion Instance Data Members (Public)

        #region Instance Properties

        private string _machineName;
        public string MachineName
        {
            get { return _machineName; }
            set { _machineName = value; }
        }

        private string _appDomainName;
        public string AppDomainName
        {
            get { return _appDomainName; }
            set { _appDomainName = value; }
        }

        private string _processId;
        public string ProcessId
        {
            get { return _processId; }
            set { _processId = value; }
        }

        private string _processName;
        public string ProcessName
        {
            get { return _processName; }
            set { _processName = value; }
        }

        private string _hostName;
        public string HostName
        {
            get { return _hostName; }
            set { _hostName = value; }
        }

        private string _applicationName;
        public string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        private string _threadId;
        public string ThreadId
        {
            get { return _threadId; }
            set { _threadId = value; }
        }

        private string _threadName;
        public string ThreadName
        {
            get { return _threadName; }
            set { _threadName = value; }
        }

        private int _fileColumnNumber;
        public int FileColumnNumber
        {
            get { return _fileColumnNumber; }
            set { _fileColumnNumber = value; }
        }

        private int _fileLineNumber;
        public int FileLineNumber
        {
            get { return _fileLineNumber; }
            set { _fileLineNumber = value; }
        }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private int _ilOffset;
        public int ILOffset
        {
            get { return _ilOffset; }
            set { _ilOffset = value; }
        }

        private MethodBase _method;
        public MethodBase Method
        {
            get { return _method; }
            set { _method = value; }
        }

        private int _nativeOffset;
        public int NativeOffset
        {
            get { return _nativeOffset; }
            set { _nativeOffset = value; }
        }

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
        }

        /// <summary>
        /// Message body to log.  Value from ToString() method from message object.
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        /// <summary>
        /// Category name used to route the log entry to a one or more trace listeners.
        /// </summary>
        public ICollection<string> Categories
        {
            get { return _categories; }
            set { _categories = value; }
        }

        /// <summary>
        /// Log entry severity as a <see cref="Severity"/> enumeration. (Unspecified, Information, Warning or Error).
        /// </summary>
        public TraceEventType Severity
        {
            get { return _severity; }
            set { _severity = value; }
        }

        private int _eventId;
        public int EventId
        {
            get { return _eventId; }
            set { _eventId = value; }
        }

        /// <summary>
        /// <para>Gets the string representation of the <see cref="Severity"/> enumeration.</para>
        /// </summary>
        /// <value>
        /// <para>The string value of the <see cref="Severity"/> enumeration.</para>
        /// </value>
        public string SeverityString
        {
            get { return _severity.ToString(); }
        }

        /// <summary>
        /// Dictionary of key/value pairs to record.
        /// </summary>
        public IDictionary<string, object> AdditionalProperties
        {
            get
            {
                if (_additionalProperties == null)
                {
                    _additionalProperties = new Dictionary<string, object>();
                }
                return _additionalProperties;
            }
            set { _additionalProperties = value; }
        }

        #endregion Instance Properties

        #region ICloneable Members

        public object Clone()
        {
            ChoLogEntry logEntry = new ChoLogEntry();

            logEntry.Message = Message;
            logEntry.Severity = Severity;
            logEntry.Categories = new List<string>(Categories);

            // clone extended properties
            if (_additionalProperties != null)
                logEntry.AdditionalProperties = new Dictionary<string, object>(_additionalProperties);

            return logEntry;
        }

        #endregion

        #region IChoObjectInitializable Members

        public bool Initialize(bool beforeFieldInit, object state)
        {
            MachineName = ChoSystemInfo.MachineName;
			AppDomainName = ChoApplication.AppDomainName;
			ProcessId = ChoApplication.ProcessId.ToString();
			ProcessName = ChoApplication.ProcessFilePath;
            HostName = ChoSystemInfo.HostName;
            ApplicationName = ChoGlobalApplicationSettings.Me.ApplicationName;
			ThreadId = ChoApplication.GetThreadId().ToString();
			ThreadName = ChoApplication.GetThreadName();

            Enrich(ChoStackTrace.GetStackFrame());

            return false;
        }

        #endregion

        #region Instance Members (Public)

        public void Enrich(StackFrame stackFrame)
        {
            if (stackFrame != null)
            {
                FileColumnNumber = stackFrame.GetFileColumnNumber();
                FileLineNumber = stackFrame.GetFileLineNumber();
                FileName = stackFrame.GetFileName();
                ILOffset = stackFrame.GetILOffset();
                Method = stackFrame.GetMethod();
                NativeOffset = stackFrame.GetNativeOffset();
            }
        }

        #endregion Instance Members (Public)

        #region Shared Members (Public)

        public static ChoLogEntry New(string message)
        {
            ChoLogEntry logEntry = new ChoLogEntry();
            logEntry.Message = message;

            return logEntry;
        }

        public static ChoLogEntry New(string[] categories, string message)
        {
            ChoLogEntry logEntry = new ChoLogEntry();
            logEntry.Categories = categories;
            logEntry.Message = message;

            return logEntry;
        }

        #endregion Shared Members (Public)
    }
}
