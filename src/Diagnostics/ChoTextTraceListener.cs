namespace Cinchoo.Core.Diagnostics
{
    #region Namespaces

    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Reflection;
    using System.Diagnostics;
    using System.Collections;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;

    using Cinchoo.Core.Storage;
    using Cinchoo.Core.Exceptions;
    using Cinchoo.Core.Property;
	using Cinchoo.Core.IO;
    using Cinchoo.Core.Reflection;

    #endregion

    /// <summary>
    /// ChoTextTraceListener is a plug-in trace listener component. It logs entries into log file with autoback feature. It
	/// backup the log file once it reaches the limit, create a new log file to log entries. Also it backup all the 
	/// log files once the process restarts. (you have option to turn of all these features by settings AUTOBACKUP as false.
	/// </summary>
	public class ChoTextTraceListener : TextWriterTraceListener
	{
		#region TextTraceMsg Struct

		private enum FileOperation { CreateNew, Rename, WriteLine, Write, Flush }

		private struct TextTraceMsg
		{
			public FileOperation Operation;
			public string Msg;
            public string LogFileName;
            public string PrevLogFileName; 

			public TextTraceMsg(string logFileName, string msg, FileOperation operation)
			{
                LogFileName = logFileName;
				Msg = msg;
				Operation = operation;
				PrevLogFileName = null;
			}

            public TextTraceMsg(string logFileName, string msg, string prevLogFileName)
			{
                LogFileName = logFileName;
                Msg = msg;
				Operation = FileOperation.Rename;
				PrevLogFileName = prevLogFileName;
			}

            public static TextTraceMsg FlushTraceMsg
            {
                get { return new TextTraceMsg(null, null, FileOperation.Flush); }
            }

            public bool IsFlushMsg
            {
                get { return Operation == FileOperation.Flush; }
            }
		}

		#endregion

        #region Instance Data Members (Private)

        private Queue _messageQ = Queue.Synchronized(new Queue());
		private AutoResetEvent _newMsgArrived = new AutoResetEvent(false);
        private Thread _fileWriterThread;

		/// <summary>
		/// Cyclic flag
		/// </summary>
		bool _cyclic = true;	
		/// <summary>
		/// Max log file size.
		/// </summary>
		long _maxFileSize = ChoStorageScale.MB;	
		/// <summary>
		/// Max log files to be generated. if the files crossed over limit, will be autoBackup and restart from 0.
		/// </summary>
		int	_maxFileCount = 100;
		/// <summary>
		/// Directory location where log files will be created.
		/// </summary>
		string _directoryName = ChoApplicationInfo.Me.ApplicationLogDirectory;
		/// <summary>
		/// Log file name pattern.
		/// </summary>
		string _baseFileName = ChoApplicationInfo.Me.ApplicationNameWithoutExtension;
		/// <summary>
		/// Log file extension.
		/// </summary>
		string _fileExt = ChoReservedFileExt.Log;				
		/// <summary>
		/// It contains list of fully qualified class names whose log entried to be logged to file.
		/// </summary>
		string[] _includeList = new string[0];
		/// <summary>
		/// It contains list of fully qualified class names whose log entried shouldn't be logged to file.
		/// </summary>
		string[] _excludeList = new string[0];
		/// <summary>
		/// If this flag is off, only one log file will be created and appended thereafter.
		/// </summary>
		bool _autoBackup = false;			

		bool _allowSplitMsg = false;
		bool _timeStamp = false;
		bool _processInfo = false;
		bool _callerInfo = false;
		string _seperator=",";

		#endregion

		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ChoTextTraceListener()
		{
            _baseFileName = ChoAssembly.GetEntryAssembly().GetName().Name;
            Init();
        }

		/// <summary>
		/// Constructor with Initialize data
		/// </summary>
		/// <param name="initializeData">Semi colon seperated Initialize information</param>
		public ChoTextTraceListener(string initializeData)
		{
			string [] initDatas=initializeData.Split(';');

			foreach (string initData in initDatas)
			{
				string [] nameValues=initData.Split('=');
				if (nameValues.Length==2)
				{
					switch (nameValues.GetValue(0).ToString().ToUpper().Trim())
					{
						case "CYCLIC":
							try
							{
								_cyclic = Convert.ToBoolean(nameValues.GetValue(1).ToString().Trim());
							}
							catch
							{
							}
							break;
						case "DIRECTORYNAME":
							_directoryName = nameValues.GetValue(1).ToString().Trim();
							break;
						case "BASEFILENAME":
							_baseFileName = GetBaseFileName(nameValues.GetValue(1).ToString().Trim());
							break;
						case "MAXFILECOUNT":
							try
							{
								_maxFileCount = Convert.ToInt32(nameValues.GetValue(1));
							}
							catch 
							{
							}
							break;
						case "FILEEXT":
							FileExt = nameValues.GetValue(1).ToString().Trim();
							break;
						case "MAXFILESIZE":
							try
							{
								_maxFileSize = ChoStorageScale.Parse(nameValues.GetValue(1).ToString().Trim());
							}
							catch 
							{
							}
							break;
						case "INCLUDE":
							_includeList = nameValues.GetValue(1).ToString().Trim().Split(',');
							break;
						case "EXCLUDE":
							_excludeList = nameValues.GetValue(1).ToString().Trim().Split(',');
							break;
						case "AUTOBACKUP":
							try
							{
								_autoBackup = Convert.ToBoolean(nameValues.GetValue(1).ToString().Trim());
							}
							catch
							{
							}
							break;
						case "ALLOWSPLITMSG":
							try
							{
								_allowSplitMsg = Convert.ToBoolean(nameValues.GetValue(1).ToString().Trim());
							}
							catch
							{
							}
							break;
						case "TIMESTAMP":
							try
							{
								_timeStamp = Convert.ToBoolean(nameValues.GetValue(1).ToString().Trim());
							}
							catch
							{
							}
							break;
						case "PROCESSINFO":
							try
							{
								_processInfo = Convert.ToBoolean(nameValues.GetValue(1).ToString().Trim());
							}
							catch
							{
							}
							break;
						case "CALLERINFO":
							try
							{
								_callerInfo = Convert.ToBoolean(nameValues.GetValue(1).ToString().Trim());
							}
							catch
							{
							}
							break;
						case "SEPERATOR":
							_seperator = nameValues.GetValue(1).ToString().Trim();
							break;
					}
				}
			}

            Init();
        }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="directoryName">Log directory.</param>
		/// <param name="maxFileSize">Max size of each log file.</param>
		/// <param name="baseFileName">Log file name.</param>
		/// <param name="fileExt">Log file extension.</param>
		/// <param name="maxFileCount">Max log file count. Once it reach the top count, it will automatically backup all the files and restart from 0</param>
		/// <param name="includeList">It contains list of fully qualified class names whose log entried to be logged to file.</param>
		/// <param name="excludeList">It contains list of fully qualified class names whose log entried shouldn't logged to file.</param>
		/// <param name="cyclic"></param>
		/// <param name="autoBackup">If this flag is off, only one log file will be created and appended thereafter.</param>
		/// <param name="allowSplitMsg"></param>
		/// <param name="seperator"></param>
		public ChoTextTraceListener(string directoryName,
			long maxFileSize,
			string baseFileName,
			string fileExt,
			int maxFileCount,
			string[] includeList,
			string[] excludeList,
			bool cyclic,
			bool autoBackup,
			bool allowSplitMsg, 
			string seperator
			)
		{
			_directoryName = directoryName;
			_maxFileSize = maxFileSize;
			_baseFileName = GetBaseFileName(baseFileName);
			FileExt = fileExt;
			_maxFileCount = maxFileCount;
			_includeList = includeList;
			_excludeList = excludeList;
			_cyclic = cyclic;
			_autoBackup = autoBackup;
			_seperator = seperator;

            Init();
        }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="directoryName">Log directory.</param>
		/// <param name="maxFileSize">Max size of each log file.</param>
		/// <param name="baseFileName">Log file name.</param>
		/// <param name="fileExt">Log file extension.</param>
		/// <param name="maxFileCount">Max log file count. Once it reach the top count, it will automatically backup all the files and restart from 0</param>
		/// <param name="includeList">It contains list of fully qualified class names whose log entried to be logged to file.</param>
		/// <param name="excludeList">It contains list of fully qualified class names whose log entried shouldn't logged to file.</param>
		/// <param name="cyclic"></param>
		/// <param name="autoBackup">If this flag is off, only one log file will be created and appended thereafter.</param>
		/// <param name="allowSplitMsg"></param>
		/// <param name="seperator"></param>
		/// <param name="timeStamp"></param>
		/// <param name="processInfo"></param>
		/// <param name="callerInfo"></param>
		public ChoTextTraceListener(string directoryName,
			long maxFileSize,
			string baseFileName,
			string fileExt,
			int maxFileCount,
			string[] includeList,
			string[] excludeList,
			bool cyclic,
			bool autoBackup,
			bool allowSplitMsg,
			string seperator,
			bool timeStamp,
			bool processInfo,
			bool callerInfo
			)
		{
			_directoryName = directoryName;
			_maxFileSize = maxFileSize;
			_baseFileName = GetBaseFileName(baseFileName);
			FileExt = fileExt;
			_maxFileCount = maxFileCount;
			_includeList = includeList;
			_excludeList = excludeList;
			_cyclic = cyclic;
			_autoBackup = autoBackup;
			_timeStamp = timeStamp;
			_processInfo = processInfo;
			_callerInfo = callerInfo;
			_seperator = seperator;

            Init();
        }

        private void Init()
        {
            Directory.CreateDirectory(_directoryName);
            Start();
        }

		#endregion

		#region TextWriterTraceListener Overrides

		/// <summary>
		/// Writes a message to this instance's Writer
		/// </summary>
		/// <param name="message"></param>
		public override void Write(string message) 
		{	
			Trace.AutoFlush = false;
			if (message != null && message.StartsWith(ChoTrace.BACKUP))
				Push2Queue(String.Empty, FileOperation.CreateNew);
			else if (Traceable)
				Push2Queue(FormatMessage(message), FileOperation.Write);
		}

		/// <summary>
		/// Writes a message to this instance's Writer followed by a line terminator.
		/// </summary>
		/// <param name="message"></param>
		public override void WriteLine(string message) 
		{	
			Trace.AutoFlush = false;
			if (message != null && message.StartsWith(ChoTrace.BACKUP))
				Push2Queue(String.Empty, FileOperation.CreateNew);
			else if (Traceable)
				Push2Queue(FormatMessage(message), FileOperation.WriteLine);
		} 

		#endregion

		#region Instance Memeber function (Private)

		private string FormatMessage(string message)
		{
			StringBuilder formattedMessage = new StringBuilder();

			if (_timeStamp)
                formattedMessage.AppendFormat("{0}{1} ", DateTime.Now.ToString("G", ChoGlobalAttributes.CultureInfo), _seperator);
			if (_processInfo)
				formattedMessage.AppendFormat("{0}/{1}{2} ", Process.GetCurrentProcess().Id, 
					Thread.CurrentThread.GetHashCode(), _seperator);

			if (_callerInfo)
				formattedMessage.AppendFormat("{0}{1} ", CallerMethod(), _seperator);

			formattedMessage.Append(message);

			return formattedMessage.ToString();
		}

		private string CallerMethod()
		{
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame;
			MethodBase stackFrameMethod;

			string typeName;
			for (int frameCount=0; frameCount < stackTrace.FrameCount; frameCount++)
			{
				stackFrame = stackTrace.GetFrame(frameCount);
				stackFrameMethod = stackFrame.GetMethod();
				typeName = stackFrameMethod.ReflectedType.FullName;

				if (!typeName.StartsWith(typeof(ChoTextTraceListener).Namespace) 
					&& !typeName.StartsWith("System"))
					return stackFrameMethod.ReflectedType.FullName + "." + stackFrameMethod.Name;
			} 

			return String.Empty;
		}

		private string GetBaseFileName(string baseFileName)
		{
			if (baseFileName == null || baseFileName.Length == 0) return String.Empty;
			return ChoPropertyManager.ExpandProperties(this, baseFileName);
        }

        #region Push2Queue Overloads

        private void Push2Queue(TextTraceMsg traceMsg)
        {
            Start();
            _messageQ.Enqueue(traceMsg);
            _newMsgArrived.Set();
        }

		private void Push2Queue(string msg, FileOperation operation)
		{
            Start();
            _messageQ.Enqueue(new TextTraceMsg(FileName, msg, operation));
			_newMsgArrived.Set();
		}

		private void Push2Queue(string msg, string prevLogFileName)
		{
            Start();
            _messageQ.Enqueue(new TextTraceMsg(FileName, msg, prevLogFileName));
			_newMsgArrived.Set();
        }

        #endregion Push2Queue Overloads

        private void Start()
        {
            lock (typeof(ChoTextTraceListener))
            {
                if (_fileWriterThread != null && _fileWriterThread.IsAlive) return;

                try
                {
                    _fileWriterThread = new Thread(new ThreadStart(SyncFileWriter));
                    _fileWriterThread.IsBackground = true;
                    _fileWriterThread.Start();
                }
                catch (Exception ex)
                {
                    ChoTrace.WriteNThrow(ex);
                }
            }
        }

        private void SyncFileWriter()
		{
			while (true)
			{
				try
				{
					object queueObject = null;
					if (_messageQ.Count > 0)
						queueObject = _messageQ.Dequeue();
					else
					{
						_newMsgArrived.WaitOne();
						continue;
					}

					TextTraceMsg item = (TextTraceMsg)queueObject;
                    if (item.IsFlushMsg) break;

					switch (item.Operation)
					{
						case FileOperation.WriteLine:
							if (base.Writer == null)
								base.Writer = new StreamWriter(new ChoFileStreamWithBackup(FileName, 
									_maxFileSize, _maxFileCount, FileMode.Append, _cyclic, _autoBackup, _allowSplitMsg));
							base.Writer.WriteLine(item.Msg);
							base.Writer.Flush();
							break;
						case FileOperation.Write:
							if (base.Writer == null)
								base.Writer = new StreamWriter(new ChoFileStreamWithBackup(FileName, 
									_maxFileSize, _maxFileCount, FileMode.Append, _cyclic, _autoBackup, _allowSplitMsg));
							base.Writer.Write(item.Msg);
							base.Writer.Flush();
							break;
						case FileOperation.CreateNew:
							base.Writer.Flush();
							base.Writer.Close();
							base.Writer = new StreamWriter(new ChoFileStreamWithBackup(FileName, 
								_maxFileSize, _maxFileCount, FileMode.Append, _cyclic, _autoBackup, _allowSplitMsg));
							break;
						case FileOperation.Rename:
							base.Writer.Flush();
							base.Writer.Close();
							base.Writer = new StreamWriter(new ChoFileStreamWithBackup(Path.Combine(_directoryName, Path.ChangeExtension(_baseFileName, _fileExt)), _maxFileSize, _maxFileCount, FileMode.Append, _cyclic, _autoBackup, _allowSplitMsg));
							if (File.Exists(Path.Combine(_directoryName, Path.ChangeExtension(item.PrevLogFileName, _fileExt))))
							{
								using (StreamReader reader = File.OpenText(Path.Combine(_directoryName, Path.ChangeExtension(item.PrevLogFileName, _fileExt))))
									base.Writer.WriteLine(reader.ReadToEnd());

                                File.Delete(Path.Combine(_directoryName, Path.ChangeExtension(item.PrevLogFileName, _fileExt)));
							}
							break;
					}
				}
				catch (Exception ex)
				{
					ChoApplication.WriteToEventLog(ChoApplicationException.ToString(ex), EventLogEntryType.Error);
				}
			}
		}

		#endregion

		#region Instance Properties (Public)

		public string FileName
		{
			get { return Path.Combine(_directoryName, Path.ChangeExtension(_baseFileName,  _fileExt)); }
		}

		#endregion

		#region Instance Properties (Private)

		/// <summary>
		/// Property tells you the whether the message is eligible to write to log file or not.
		/// </summary>
		private bool Traceable
		{
			get 
			{
				if (_includeList.Length==0 && _excludeList.Length==0 ) return true;

				StackTrace stackTrace = new StackTrace();
				StackFrame stackFrame;
				MethodBase stackFrameMethod;

				string typeName;
				for (int frameCount=0; frameCount < stackTrace.FrameCount; frameCount++)
				{
					stackFrame = stackTrace.GetFrame(frameCount);
					stackFrameMethod = stackFrame.GetMethod();
					typeName = stackFrameMethod.ReflectedType.FullName;

					//First check to see the calling method class exists in the ExcludeList
					foreach (string excludeItem in _excludeList)
					{
						if (typeName.StartsWith(excludeItem))
							return false;
					}

					//Next check to see the calling method class exists in the IncludeList
					foreach (string includeItem in _includeList)
					{
						if (typeName.StartsWith(includeItem))
							return true;
					}
				} 

				return false;
			}
		}

		/// <summary>
		/// Property to validate and set the log file extension 
		/// </summary>
		private string FileExt
		{
			get { return _fileExt; }
			set 
			{
                if (String.IsNullOrEmpty(value)) return;
				_fileExt = value.Replace(".", String.Empty);
			}
		}

		#endregion

		#region Events

		public delegate string GetCustomLogFileNameEventHandler();
		public event GetCustomLogFileNameEventHandler OnGetCustomLogFileName
		{
			add
			{
				string prevBaseFileName = _baseFileName;

				string customBaseFileName = null;
				if (value != null) customBaseFileName = value();
				if (customBaseFileName != null && customBaseFileName.Length > 0) _baseFileName = GetBaseFileName(customBaseFileName);
				Push2Queue(String.Empty, prevBaseFileName);
			}
			remove
			{
			}
		}

		#endregion

		#region Instance Members (Public)

		public override void Flush()
		{
			while (true)
			{
                if (ChoTrace.ChoSwitch.TraceVerbose)
					Trace.WriteLine("Trace.Flush...");
				
                Push2Queue(TextTraceMsg.FlushTraceMsg);
                if (_fileWriterThread.Join(1000)) break;
			}
			base.Flush();
		}

		#endregion

		#region Shared Members (Public)

		public static ChoTextTraceListener GetFirst()
		{
			foreach (TraceListener traceListener in Trace.Listeners)
			{
				if (traceListener == null) continue;
				if (traceListener is ChoTextTraceListener) return traceListener as ChoTextTraceListener;
			}
			return null;
		}

		public static ChoTextTraceListener Get(string name)
		{
			foreach (TraceListener traceListener in Trace.Listeners)
			{
				if (traceListener == null) continue;
				if (traceListener is ChoTextTraceListener && traceListener.Name == name ) return traceListener as ChoTextTraceListener;
			}
			return null;
		}

		public static ChoTextTraceListener[] GetAll()
		{
			ArrayList traceListeners = new ArrayList();
			foreach (TraceListener traceListener in Trace.Listeners)
			{
				if (traceListener == null) continue;
				if (traceListener is ChoTextTraceListener)
					traceListeners.Add(traceListener);
			}
			return traceListeners.ToArray(typeof(ChoTextTraceListener)) as ChoTextTraceListener[];
		}

		#endregion
	}
}
