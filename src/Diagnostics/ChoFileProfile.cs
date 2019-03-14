namespace eSquare.Core.Diagnostics
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Collections;
    using System.Diagnostics;
    using System.Configuration;
    using System.Collections.Specialized;

    //using eSquare.Core.IO;
    //using eSquare.Core.LoadBalancer;
    //using eSquare.Core.Configuration;
    using eSquare.Core.Types;
    using eSquare.Core.Exceptions;

    #endregion NameSpaces

    #region Enum

    public class Ext
    {
        public const string Err = "Err";
    }

    public class LogDirectories
    {
        #region Constants

        public readonly static string Caches = "Caches";
        public readonly static string DuplicateItemsCache = @"Caches\DuplicateItems";
        public readonly static string InvalidItemsCache = @"Caches\InvalidItems";
        public readonly static string ConversionFailedCache = @"Caches\ConversionFailed";
        public readonly static string CacheItemBulkOpSuccess = "CacheItemBulkOpSuccess";
        public readonly static string CacheItemBulkOpFailed = "CacheItemBulkOpFailed";
        public readonly static string CacheItemLoadSuccess = "CacheItemLoadSuccess";
        public readonly static string CacheItemLoadFailed = "CacheItemLoadFailed";
        public readonly static string CacheItemLoadSuccessNoRowsFound = "CacheItemLoadSuccessNoRowsFound";
        public readonly static string CacheItemInsertSuccess = "CacheItemInsertSuccess";
        public readonly static string CacheItemInsertFailed = "CacheItemInsertFailed";
        public readonly static string CacheItemDeleteSuccess = "CacheItemDeleteSuccess";
        public readonly static string CacheItemDeleteFailed = "CacheItemDeleteFailed";
        public readonly static string CacheItemUpdateSuccess = "CacheItemUpdateSuccess";
        public readonly static string CacheItemUpdateFailed = "CacheItemUpdateFailed";
        public readonly static string CacheItemFlushSuccess = "CacheItemFlushSuccess";
        public readonly static string CacheItemFlushFailed = "CacheItemFlushFailed";
        public readonly static string Settings = "Settings";
        public readonly static string CacheGroupSettings = "CacheGroupSettings";
        public readonly static string AddOnGroupSettings = "AddOnGroupSettings";
        public readonly static string PositionGeneratorGroupSettings = "PositionGeneratorGroupSettings";
        public readonly static string WorkflowExecutionPlanGroupSettings = "WorkflowExecutionPlanGroupSettings";
        public readonly static string Others = "Others";

        #endregion Constants

        #region Constructors

        static LogDirectories()
        {
            Caches = GetLogDirectoryByBatch(Caches);
            DuplicateItemsCache = GetLogDirectoryByBatch(DuplicateItemsCache);
            InvalidItemsCache = GetLogDirectoryByBatch(InvalidItemsCache);
            ConversionFailedCache = GetLogDirectoryByBatch(ConversionFailedCache);
            CacheItemBulkOpSuccess = GetLogDirectoryByBatch(CacheItemBulkOpSuccess);
            CacheItemBulkOpFailed = GetLogDirectoryByBatch(CacheItemBulkOpFailed);
            CacheItemLoadSuccess = GetLogDirectoryByBatch(CacheItemLoadSuccess);
            CacheItemLoadFailed = GetLogDirectoryByBatch(CacheItemLoadFailed);
            CacheItemLoadSuccessNoRowsFound = GetLogDirectoryByBatch(CacheItemLoadSuccessNoRowsFound);
            CacheItemInsertSuccess = GetLogDirectoryByBatch(CacheItemInsertSuccess);
            CacheItemInsertFailed = GetLogDirectoryByBatch(CacheItemInsertFailed);
            CacheItemDeleteSuccess = GetLogDirectoryByBatch(CacheItemDeleteSuccess);
            CacheItemDeleteFailed = GetLogDirectoryByBatch(CacheItemDeleteFailed);
            CacheItemUpdateSuccess = GetLogDirectoryByBatch(CacheItemUpdateSuccess);
            CacheItemUpdateFailed = GetLogDirectoryByBatch(CacheItemUpdateFailed);
            CacheItemFlushSuccess = GetLogDirectoryByBatch(CacheItemFlushSuccess);
            CacheItemFlushFailed = GetLogDirectoryByBatch(CacheItemFlushFailed);
            Settings = GetLogDirectoryByBatch(Settings);
            CacheGroupSettings = GetLogDirectoryByBatch(CacheGroupSettings);
            AddOnGroupSettings = GetLogDirectoryByBatch(AddOnGroupSettings);
            PositionGeneratorGroupSettings = GetLogDirectoryByBatch(PositionGeneratorGroupSettings);
            WorkflowExecutionPlanGroupSettings = GetLogDirectoryByBatch(WorkflowExecutionPlanGroupSettings);
            Others = GetLogDirectoryByBatch(Others);
        }

        #endregion

        #region Shared Members (Public)

        public static string GetLogDirectoryByBatch(string logDirectory)
        {
            return logDirectory;
            //BatchSettings.BatchType == null || BatchSettings.BatchType == BatchSettings.UnknownBatchType || logDirectory.EndsWith(BatchSettings.BatchType) ?
            //     logDirectory : Path.Combine(logDirectory, BatchSettings.BatchType);
        }

        #endregion
    }

    public class LogFiles
    {
        #region Constants

        public readonly static string AllFailedCacheLookup = "AllFailedCacheLookup";
        public readonly static string Bcp = "Bcp";
        public readonly static string SerializationIssues = "SerializationIssues";
        public readonly static string ConfigurationErrors = "ConfigurationErrors";

        #endregion Constants

        #region Constructors

        static LogFiles()
        {
            AllFailedCacheLookup = GetLogFileByProcessId(AllFailedCacheLookup);
            Bcp = GetLogFileByProcessId(Bcp);
            SerializationIssues = GetLogFileByProcessId(SerializationIssues);
            ConfigurationErrors = GetLogFileByProcessId(ConfigurationErrors);
        }

        #endregion

        #region Shared Members (Public)

        public static string GetLogFileByProcessId(string logFile)
        {
            return logFile;
            
            //return BatchSettings.BatchType == null /* || BatchSettings.BatchType == BatchSettings.UnknownBatchType */ ?
            //    String.Format("{0}_{1}", logFile, Process.GetCurrentProcess().Id)
            //    : logFile;
        }

        #endregion
    }

    #endregion

    #region ChoFileProfileSettings Class (Public Sealed)

    internal sealed class ChoFileProfileSettings //: IConfigSettingsHandler
    {
        #region Shared Data Members (Public)

        public static bool TurnOn = true;
        public static bool IndentProfiling = true;
        public static bool ForwardMsgToConsole = true;
        public static string LogDirectory = @"C:\Log";
        public static bool MachineDependent = false;

        #endregion Shared Data Members (Public)

        internal const string CompleteLogDirectory = "Completed";
        internal const string FailedLogDirectory = "Failed";

        #region Shared Constructors

        static ChoFileProfileSettings()
        {
            //try
            //{
            //    if (ChoAppSettings.UseEnterpriseLib4Log)
            //    {
            //        LoggingConfigurationView loggingConfigurationView = new LoggingConfigurationView(ConfigurationManager.GetCurrentContext());
            //        FlatFileSinkData flatFileSinkData = loggingConfigurationView.GetDistributorSettings().SinkDataCollection["Flat File Sink"] as FlatFileSinkData;

            //        if (((flatFileSinkData != null) && (flatFileSinkData.FileName != null)) && (flatFileSinkData.FileName.Length > 0))
            //            ChoFileProfileSettings.LogDirectory = Path.GetDirectoryName(flatFileSinkData.FileName);
            //    }
            //    else
            //    {
            //        ChoTextTraceListener textTraceListener = ChoTextTraceListener.GetFirst();
            //        if (textTraceListener != null)
            //            ChoFileProfileSettings.LogDirectory = Path.GetDirectoryName(textTraceListener.FileName);
            //    }
            //}
            //catch
            //{
            //    try
            //    {
            //        ChoTextTraceListener textTraceListener = ChoTextTraceListener.GetFirst();
            //        if (textTraceListener != null)
            //            ChoFileProfileSettings.LogDirectory = Path.GetDirectoryName(textTraceListener.FileName);
            //    }
            //    catch { }
            //}

            //ChoConfigSettings.Initialize("riskIt/fileProfileSettings", new ChoFileProfileSettings());
        }

        #endregion

        #region IConfigSettingsHandler Members

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder();

            msg.AppendFormat(Environment.NewLine);

            msg.AppendFormat("-- ChoFileProfile Settings --{0}", Environment.NewLine);
            msg.AppendFormat("\tTurnOn: {0}{1}", TurnOn, Environment.NewLine);
            msg.AppendFormat("\tIndentProfiling: {0}{1}", IndentProfiling, Environment.NewLine);
            msg.AppendFormat("\tForwardMsgToConsole: {0}{1}", ForwardMsgToConsole, Environment.NewLine);
            msg.AppendFormat("\tLogDirectory: {0}{1}", LogDirectory, Environment.NewLine);
            msg.AppendFormat("\tMachineDependent: {0}{1}", MachineDependent, Environment.NewLine);

            msg.AppendFormat(Environment.NewLine);

            return msg.ToString();
        }

        public void HandleConfigSettings(NameValueCollection nameValues)
        {
            if (nameValues != null)
            {
                try
                {
                    TurnOn = Boolean.Parse(nameValues["TurnOn"]);
                }
                catch
                {
                }

                try
                {
                    IndentProfiling = Boolean.Parse(nameValues["IndentProfiling"]);
                }
                catch
                {
                }

                try
                {
                    ForwardMsgToConsole = Boolean.Parse(nameValues["ForwardMsgToConsole"]);
                }
                catch
                {
                }

                try
                {
                    MachineDependent = Boolean.Parse(nameValues["MachineDependent"]);
                }
                catch
                {
                }

                if (nameValues["LogDirectory"] != null)
                {
                    LogDirectory = nameValues["LogDirectory"];
                    //LogDirectory = new ChoReservedParameters().Format(LogDirectory);
                    if (LogDirectory == null || LogDirectory.Trim() == String.Empty)
                        throw new NullReferenceException("LogDirectory can't be null.");
                }
            }

            if (LogDirectory.EndsWith("\\")) LogDirectory = LogDirectory.Substring(0, LogDirectory.Length - 1);

            //Attach Machine name to it
            if (MachineDependent)
            {
                if (!ChoFileProfileSettings.LogDirectory.EndsWith(Environment.MachineName))
                    LogDirectory = LogDirectory + "\\" + System.Environment.MachineName;
            }

            if (TurnOn)
            {
                if (!Directory.Exists(LogDirectory))
                    Directory.CreateDirectory(LogDirectory);
            }
        }

        #endregion
    }

    #endregion ChoFileProfileSettings Class (Public Sealed)

    public sealed class ChoFileProfile : IChoProfile, IDisposable
    {
        #region FileProfileItem Struct

        private enum FileOperation { Clean, WriteLine, Write, Move, Flush }

        private struct FileProfileItem
        {
            public FileOperation Operation;
            public string FileName;
            public string Msg;
            public string TargetFileName;

            public FileProfileItem(string fileName, string msg)
            {
                FileName = fileName;
                Msg = msg;
                TargetFileName = null;
                Operation = FileOperation.WriteLine;
            }

            public FileProfileItem(string fileName, string msg, FileOperation operation)
            {
                FileName = fileName;
                Msg = msg;
                TargetFileName = null;
                Operation = operation;
            }

            public FileProfileItem(string fileName, string msg, string targetFileName, FileOperation operation)
            {
                FileName = fileName;
                Msg = msg;
                TargetFileName = targetFileName;
                Operation = operation;
            }

            public static FileProfileItem FlushMsg
            {
                get { return new FileProfileItem(null, null, FileOperation.Flush); }
            }

            public bool IsFlushMsg
            {
                get { return Operation == FileOperation.Flush; }
            }
        }

        #endregion

        #region Constants

        public const string Seperator = "------------------------------------------------------";

        #endregion

        #region Instance Data Members (Private)

        private int _indent = 0;
        private bool _condition = false;
        private string _fileName = String.Empty;
        private bool _disposed = false;

        private string _msg = "Elapsed time taken";
        private DateTime _startTime = DateTime.Now;
        private StringBuilder _formattedMsg = new StringBuilder();
        private static Thread _fileWriterThread = null;
        private static Queue _messageQ = Queue.Synchronized(new Queue());
        private static AutoResetEvent _newMsgArrived = new AutoResetEvent(false);

        #endregion Instance Data Members (Private)

        #region Constrctors

        static ChoFileProfile()
        {
            try
            {
                _fileWriterThread = new Thread(new ParameterizedThreadStart(SyncFileWriter));
                _fileWriterThread.IsBackground = true;
                _fileWriterThread.Start();
            }
            catch (Exception ex)
            {
                ChoTrace.Write(ex);
            }
        }

        internal ChoFileProfile(bool trash)
        {
        }

        public ChoFileProfile(bool condition, string fileName, string msg)
        {
            if (fileName == null) throw new NullReferenceException("Log filename can't be null.");
            if (msg == null) throw new NullReferenceException("Message can't be null.");

            _fileName = LogFileName(fileName);
            Initialize(condition, msg);
        }

        public ChoFileProfile(bool condition, string fileName, string msg, ChoFileProfile outerProfile)
        {
            if (fileName == null) throw new NullReferenceException("Log filename can't be null.");
            if (msg == null) throw new NullReferenceException("Message can't be null.");
            if (outerProfile == null) throw new NullReferenceException("Outer profile object can't be null.");

            _fileName = LogFileName(fileName);
            _indent = outerProfile._indent + 1;
            Initialize(condition, msg);
        }

        public ChoFileProfile(bool condition, string subDirectoryName, string fileName, string msg)
        {
            if (fileName == null) throw new NullReferenceException("Log filename can't be null.");
            if (msg == null) throw new NullReferenceException("Message can't be null.");

            _fileName = LogFileName(subDirectoryName, fileName);
            Initialize(condition, msg);
        }

        public ChoFileProfile(bool condition, string subDirectoryName, string fileName, string msg, ChoFileProfile outerProfile)
        {
            if (fileName == null) throw new NullReferenceException("Log filename can't be null.");
            if (msg == null) throw new NullReferenceException("Message can't be null.");
            if (outerProfile == null) throw new NullReferenceException("Outer profile object can't be null.");

            _fileName = LogFileName(subDirectoryName, fileName);
            _indent = outerProfile._indent + 1;
            Initialize(condition, msg);
        }

        ~ChoFileProfile()
        {
            Dispose(true);
        }

        #endregion Constrctors

        #region IDisposable Overrides

        private void Dispose(bool finalize)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_condition)
                {
                    if (ChoFileProfileSettings.IndentProfiling)
                    {
                        for (int index = 0; index < _indent - 1; index++)
                            _formattedMsg.Append("\t");

                        _formattedMsg.Append("} [" + Convert.ToString(DateTime.Now - _startTime) + "] <---");

                        _indent--;
                    }
                    else
                        _formattedMsg.Append(_msg + ": " +
                            Convert.ToString(DateTime.Now - _startTime) + "<---");
                    Flush();
                }

                if (!finalize) GC.SuppressFinalize(this);
            }
        }

        public void Dispose()
        {
            Dispose(false);
        }

        #endregion IDisposable Overrides

        #region Instance Methods (Public)

        public void Append(string msg)
        {
            if (_condition)
            {
                if (ChoFileProfileSettings.IndentProfiling)
                {
                    for (int index = 0; index < _indent; index++)
                        _formattedMsg.Append("\t");
                }
                _formattedMsg.Append(msg);

                Flush(FileOperation.Write);
            }
        }

        #endregion

        #region Instance Properties (Public)

        public string ElapsedTimeTaken
        {
            get { return Convert.ToString(DateTime.Now - _startTime); }
        }

        #endregion

        #region Shared Members (Public)

        public static void Shutdown()
        {
            if (_fileWriterThread == null) return;

            while (true)
            {
                if (ChoTrace.ChoTraceSwitch.TraceVerbose)
                    Console.WriteLine("ChoFileProfile.Shutdown...");

                Push2Queue(null, null, FileOperation.Flush);
                if (_fileWriterThread.Join(1000)) break;
            }

            _fileWriterThread = null;
        }

        public static void Backup()
        {
            if (!ChoFileProfileSettings.TurnOn) return;

            string logDirectory = ChoFileProfileSettings.LogDirectory;

            try
            {
                //Has any files
                if (!HasAnyFiles(logDirectory)) return;

                //Backup the directory
                string newLogDirectory = logDirectory + "_" + DateTime.Now.ToString("yyyyMMdd");

                int index = 0;
                while (Directory.Exists(newLogDirectory))
                {
                    newLogDirectory = logDirectory + "_" + DateTime.Now.ToString("yyyyMMdd") + "." + index.ToString("D3");
                    index++;
                }

                Directory.CreateDirectory(newLogDirectory);

                //Move files
                foreach (string file in Directory.GetFiles(logDirectory))
                {
                    File.Move(file, newLogDirectory + "\\" + Path.GetFileName(file));
                }

                //Move Directories
                foreach (string directory in Directory.GetDirectories(logDirectory))
                {
                    Directory.Move(directory, newLogDirectory + "\\" + Path.GetFileName(directory));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ChoApplicationException.ToString(ex));
            }
        }

        public static void WriteLine(string fileName, string msg)
        {
            WriteLineIf(true, fileName, msg);
        }

        public static void WriteLine(string subDirectoryName, string fileName, string msg)
        {
            WriteLineIf(true, subDirectoryName, fileName, msg);
        }

        public static void WriteLineIf(bool condition, string fileName, string msg)
        {
            if (msg == null || !condition) return;
            if (!ChoFileProfileSettings.TurnOn) return;
            if (fileName == null) throw new NullReferenceException("Log filename can't be null.");

            Push2Queue(LogFileName(fileName), msg, FileOperation.WriteLine);
        }

        public static void WriteLineIf(bool condition, string subDirectoryName, string fileName, string msg)
        {
            if (msg == null || !condition) return;
            if (!ChoFileProfileSettings.TurnOn) return;
            if (fileName == null) throw new NullReferenceException("Log filename can't be null.");
            if (subDirectoryName == null) throw new NullReferenceException("SubDirectoryName can't be null.");

            Push2Queue(LogFileName(subDirectoryName, fileName), msg, FileOperation.WriteLine);
        }

        public static void WriteNewLine(string fileName)
        {
            WriteLineIf(true, fileName, Environment.NewLine, false);
        }

        public static void WriteNewLine(string subDirectoryName, string fileName)
        {
            WriteLineIf(true, subDirectoryName, fileName, Environment.NewLine, false);
        }

        public static void WriteNewLineIf(bool condition, string fileName)
        {
            WriteLineIf(condition, fileName, Environment.NewLine);
        }

        public static void WriteNewLineIf(bool condition, string subDirectoryName, string fileName)
        {
            WriteLineIf(condition, subDirectoryName, fileName, Environment.NewLine);
        }

        public static void WriteLine(string fileName, string msg, bool clearContent)
        {
            WriteLineIf(true, fileName, msg, clearContent);
        }

        public static void WriteLine(string subDirectoryName, string fileName, string msg, bool clearContent)
        {
            WriteLineIf(true, subDirectoryName, fileName, msg, clearContent);
        }

        public static void WriteLineIf(bool condition, string fileName, string msg, bool clearContent)
        {
            if (clearContent)
                Clean(fileName);
            WriteLineIf(condition, fileName, msg);
        }

        public static void WriteLineIf(bool condition, string subDirectoryName, string fileName, string msg, bool clearContent)
        {
            if (clearContent)
                Clean(subDirectoryName, fileName);
            WriteLineIf(condition, subDirectoryName, fileName, msg);
        }

        public static void Write(string fileName, string msg)
        {
            WriteIf(true, fileName, msg);
        }

        public static void Write(string subDirectoryName, string fileName, string msg)
        {
            WriteIf(true, subDirectoryName, fileName, msg);
        }

        public static void WriteIf(bool condition, string fileName, string msg)
        {
            if (msg == null || !condition) return;
            if (!ChoFileProfileSettings.TurnOn) return;
            if (fileName == null) throw new NullReferenceException("Log filename can't be null.");

            Push2Queue(LogFileName(fileName), msg, FileOperation.Write);
        }

        public static void WriteIf(bool condition, string subDirectoryName, string fileName, string msg)
        {
            if (msg == null || !condition) return;
            if (!ChoFileProfileSettings.TurnOn) return;
            if (fileName == null) throw new NullReferenceException("Log filename can't be null.");
            if (subDirectoryName == null) throw new NullReferenceException("SubDirectoryName can't be null.");

            Push2Queue(LogFileName(subDirectoryName, fileName), msg, FileOperation.Write);
        }

        public static void Write(string fileName, string msg, bool clearContent)
        {
            WriteIf(true, fileName, msg, clearContent);
        }

        public static void Write(string fileName, string subDirectoryName, string msg, bool clearContent)
        {
            WriteIf(true, subDirectoryName, fileName, msg, clearContent);
        }

        public static void WriteIf(bool condition, string fileName, string msg, bool clearContent)
        {
            if (clearContent)
                Clean(fileName);
            WriteIf(condition, fileName, msg);
        }

        public static void WriteIf(bool condition, string subDirectoryName, string fileName, string msg, bool clearContent)
        {
            if (clearContent)
                Clean(subDirectoryName, fileName);
            WriteIf(condition, subDirectoryName, fileName, msg);
        }

        public static void WriteLineWithTimeStamp(string fileName, string msg)
        {
            WriteLineWithTimeStampIf(true, fileName, msg);
        }

        public static void WriteLineWithTimeStamp(string subDirectoryName, string fileName, string msg)
        {
            WriteLineWithTimeStampIf(true, subDirectoryName, fileName, msg);
        }

        public static void WriteLineWithTimeStampIf(bool condition, string fileName, string msg)
        {
            if (msg == null || !condition) return;
            if (!ChoFileProfileSettings.TurnOn) return;
            if (fileName == null) throw new NullReferenceException("Log filename can't be null.");

            Push2Queue(LogFileName(fileName), String.Format("{0} <--- {1}", msg, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")), FileOperation.WriteLine);
        }

        public static void WriteLineWithTimeStampIf(bool condition, string subDirectoryName, string fileName, string msg)
        {
            if (msg == null || !condition) return;
            if (!ChoFileProfileSettings.TurnOn) return;
            if (fileName == null) throw new NullReferenceException("Log filename can't be null.");
            if (subDirectoryName == null) throw new NullReferenceException("SubDirectoryName can't be null.");

            Push2Queue(LogFileName(subDirectoryName, fileName), String.Format("{0} <--- {1}", msg, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")), FileOperation.WriteLine);
        }

        public static void SetComplete(string fileName)
        {
            if (!ChoFileProfileSettings.TurnOn) return;
            if (fileName == null) throw new NullReferenceException("Log filename can't be null.");
            Push2Queue(LogFileName(fileName), String.Empty, LogFileName(ChoFileProfileSettings.CompleteLogDirectory, fileName), FileOperation.Move);
        }

        public static void SetComplete(string subDirectoryName, string fileName)
        {
            if (!ChoFileProfileSettings.TurnOn) return;
            if (fileName == null) throw new NullReferenceException("Log filename can't be null.");
            if (subDirectoryName == null) throw new NullReferenceException("SubDirectoryName can't be null.");
            Push2Queue(LogFileName(subDirectoryName, fileName), String.Empty, LogFileName(subDirectoryName + "\\" + ChoFileProfileSettings.CompleteLogDirectory, fileName), FileOperation.Move);
        }

        public static void SetFailed(string fileName)
        {
            if (!ChoFileProfileSettings.TurnOn) return;
            if (fileName == null) throw new NullReferenceException("Log filename can't be null.");
            Push2Queue(LogFileName(fileName), String.Empty, LogFileName(ChoFileProfileSettings.FailedLogDirectory, fileName), FileOperation.Move);
        }

        public static void SetFailed(string subDirectoryName, string fileName)
        {
            if (!ChoFileProfileSettings.TurnOn) return;
            if (fileName == null) throw new NullReferenceException("Log filename can't be null.");
            if (subDirectoryName == null) throw new NullReferenceException("SubDirectoryName can't be null.");
            Push2Queue(LogFileName(subDirectoryName, fileName), String.Empty, LogFileName(subDirectoryName + "\\" + ChoFileProfileSettings.FailedLogDirectory, fileName), FileOperation.Move);
        }

        public static void Clean(string fileName)
        {
            if (!ChoFileProfileSettings.TurnOn) return;
            if (fileName == null) throw new NullReferenceException("Log filename can't be null.");
            Push2Queue(LogFileName(fileName), String.Empty, FileOperation.Clean);
        }

        public static void Clean(string subDirectoryName, string fileName)
        {
            if (!ChoFileProfileSettings.TurnOn) return;
            if (fileName == null) throw new NullReferenceException("Log filename can't be null.");
            if (subDirectoryName == null) throw new NullReferenceException("SubDirectoryName can't be null.");
            Push2Queue(LogFileName(subDirectoryName, fileName), String.Empty, FileOperation.Clean);
        }

        #endregion Other Helper Methods (Public)

        #region Shared Methods (Private)

        private static bool HasAnyFiles(string directory)
        {
            if (!Directory.Exists(directory)) return false;
            if (Directory.GetFiles(directory).Length > 0) return true;

            //Move Directories
            foreach (string subDirectory in Directory.GetDirectories(directory))
                if (Directory.GetFiles(subDirectory).Length > 0) return true;

            return false;
        }

        private static void Push2Queue(string fileName, string msg, FileOperation operation)
        {
            _messageQ.Enqueue(new FileProfileItem(fileName, msg, operation));
            _newMsgArrived.Set();
        }

        private static void Push2Queue(string fileName, string msg, string targetFileName, FileOperation operation)
        {
            _messageQ.Enqueue(new FileProfileItem(fileName, msg, targetFileName, operation));
            _newMsgArrived.Set();
        }

        private static void SyncFileWriter(object state)
        {
            FileProfileItem item;
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

                    item = (FileProfileItem)queueObject;
                    if (item.IsFlushMsg) break;

                    switch (item.Operation)
                    {
                        case FileOperation.WriteLine:
                            {
                                using (StreamWriter Writer = new StreamWriter(new FileStream(item.FileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
                                    Writer.WriteLine(item.Msg);
                                break;
                            }
                        case FileOperation.Write:
                            {
                                using (StreamWriter Writer = new StreamWriter(new FileStream(item.FileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
                                    Writer.Write(item.Msg);
                                break;
                            }
                        case FileOperation.Clean:
                            if (File.Exists(item.FileName))
                            {
                                using (StreamWriter logFile = new StreamWriter(item.FileName))
                                {
                                }
                            }
                            break;
                        case FileOperation.Move:
                            if (File.Exists(item.TargetFileName))
                                File.Delete(item.TargetFileName);
                            if (File.Exists(item.FileName))
                                File.Move(item.FileName, item.TargetFileName);
                            break;
                    }
                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                }
                catch (Exception ex)
                {
                    ChoTrace.Write(ex);
                }
            }
        }

        #endregion Shared Methods (Private)

        #region Instance Members (Private)

        private void Flush()
        {
            Flush(FileOperation.WriteLine);
        }

        private void Flush(FileOperation fileOperation)
        {
            if (!_condition || !ChoFileProfileSettings.TurnOn) return;

            Push2Queue(_fileName, _formattedMsg.ToString(), fileOperation);
            if (_formattedMsg.Length > 0) _formattedMsg.Remove(0, _formattedMsg.Length);
        }

        private void Initialize(bool condition, string msg)
        {
            if (_msg != null && _msg.Length > 0)
                _msg = msg;

            _condition = condition;

            if (ChoFileProfileSettings.ForwardMsgToConsole && _condition) Console.WriteLine(_msg);

            if (_condition)
            {
                if (ChoFileProfileSettings.IndentProfiling)
                {
                    for (int index = 0; index < _indent; index++)
                        _formattedMsg.Append("\t");

                    _formattedMsg.AppendFormat("{0} {{ [{1}]", _msg, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));

                    _indent++;
                }
                else
                    _formattedMsg.AppendFormat("{0}", _msg);

                Flush();
            }
        }

        internal static string LogFileName(string fileName)
        {
            if (Path.IsPathRooted(fileName))
                return fileName;
            else
            {
                string tmpFileName = fileName.ToLower();
                if (!Path.HasExtension(fileName)) fileName = fileName + ".log";

                if (!Directory.Exists(ChoFileProfileSettings.LogDirectory))
                    Directory.CreateDirectory(ChoFileProfileSettings.LogDirectory);

                return ChoFileProfileSettings.LogDirectory + "\\" + fileName;
            }
        }

        internal static string LogFileName(string directory, string fileName)
        {
            if (Path.IsPathRooted(fileName))
                return fileName;
            else
            {
                string tmpFileName = fileName.ToLower();
                if (!Path.HasExtension(fileName)) fileName = fileName + ".log";

                if (!directory.StartsWith(ChoFileProfileSettings.LogDirectory))
                    directory = ChoFileProfileSettings.LogDirectory + "\\" + directory;

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                return directory + "\\" + fileName;
            }
        }

        private static string LogDirectory(string subDirectoryName)
        {
            if (!Directory.Exists(ChoFileProfileSettings.LogDirectory + "\\" + subDirectoryName))
                Directory.CreateDirectory(ChoFileProfileSettings.LogDirectory + "\\" + subDirectoryName);

            return ChoFileProfileSettings.LogDirectory + "\\" + subDirectoryName;
        }

        #endregion Instance Members (Private)

        #region IChoProfile Members

        public new string ToString()
        {
            return String.Empty;
        }

        void eSquare.Core.Diagnostics.IChoProfile.Append(bool condition, Exception ex)
        {
            if (condition) ((IChoProfile)this).Append(ex);
        }

        void eSquare.Core.Diagnostics.IChoProfile.Append(Exception ex)
        {
            if (ex == null) return;
            Append(ChoApplicationException.ToString(ex));
        }

        string eSquare.Core.Diagnostics.IChoProfile.Append(bool condition, string msg)
        {
            if (condition) return ((IChoProfile)this).Append(msg);
            return msg;
        }

        string eSquare.Core.Diagnostics.IChoProfile.Append(string msg)
        {
            if (_condition)
            {
                _formattedMsg.Append(ChoString.IndentMsg(_indent - 1, msg));
                Flush(FileOperation.Write);
            }
            return msg;
        }

        public string AppendWithNewLineHandling(bool condition, string msg)
        {
            msg = ChoString.HandleNewLine(msg);
            if (condition) return ((IChoProfile)this).Append(msg);

            return msg;
        }

        string eSquare.Core.Diagnostics.IChoProfile.AppendWithNewLineHandling(string msg)
        {
            return AppendWithNewLineHandling(true, msg);
        }

        #endregion

        #region IChoProfile Members

        public int Indent
        {
            get { return _indent; }
        }

        #endregion
    }
}
