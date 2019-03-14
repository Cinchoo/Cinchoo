namespace Cinchoo.Core.Logging
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.IO;

    #endregion NameSpaces

    public static class ChoLogger
    {
        #region Public Constants

        public const string MachineName = "MachineName";
        public const string AppDomainName = "AppDomainName";
        public const string ProcessId = "ProcessId";
        public const string ProcessName = "ProcessName";
        public const string HostName = "HostName";
        public const string ApplicationName = "ApplicationName";
        public const string ThreadId = "ThreadId";
        public const string ThreadName = "ThreadName";

        public const string FileColumnNumber = "FileColumnNumber";
        public const string FileLineNumber = "FileLineNumber";
        public const string FileName = "FileName";
        public const string ILOffset = "ILOffset";
        public const string Method = "Method";
        public const string NativeOffset = "NativeOffset";

        #endregion Public Constants

        #region Other Data Members (Private)

        private static readonly ICollection<string> _emptyCategoriesList = new List<string>(0);
        private const TraceEventType DefaultSeverity = TraceEventType.Information;
        private const int DefaultEventId = 1;

        #endregion Other Data Members (Private)

        #region Constructors

        static ChoLogger()
        {
			//ChoStreamProfile.Clean(ChoReservedDirectoryName.Settings, ChoType.GetLogFileName(typeof(ChoLogger)));
        }

        #endregion Constructors

        #region Shared Members (Public)

        public static void Log(object message)
        {
            Log(message, _emptyCategoriesList);
        }

        public static void Log(object message, string category)
        {
            Log(message, category, DefaultSeverity);
        }

        public static void Log(object message, ICollection<string> categories)
        {
            Log(message, categories, DefaultSeverity);
        }

        public static void Log(object message, string category, TraceEventType severity)
        {
            Log(message, category, DefaultSeverity, DefaultEventId, null);
        }

        public static void Log(object message, ICollection<string> categories, TraceEventType severity)
        {
            Log(message, categories, DefaultSeverity, DefaultEventId, null);
        }

        public static void Log(object message, string category, TraceEventType severity, int eventId, IDictionary<string, object> additionalProperties)
        {
            Log(message, new string[] { category }, severity, eventId, additionalProperties);
        }

        public static void Log(object message, ICollection<string> categories, TraceEventType severity, int eventId, IDictionary<string, object> additionalProperties)
        {
            ChoLogEntry logEntry = ChoObjectManagementFactory.CreateInstance<ChoLogEntry>(typeof(ChoLogEntry));
            logEntry.AdditionalProperties = additionalProperties;
            logEntry.Categories = categories;
            logEntry.Message = message.ToString();
            logEntry.Severity = severity;
            logEntry.EventId = eventId;

            Log(logEntry);
        }

        public static void Log(ChoLogEntry log)
        {
            Log(ChoStackTrace.GetStackFrame(typeof(ChoLogger)), log);
        }

        #endregion Shared Members (Public)

        #region Shared Members (Internal)

        internal static void Log(StackFrame callerStackFrame, string message)
        {
            Log(callerStackFrame, message as object);
        }

        internal static void Log(StackFrame callerStackFrame, object message)
        {
            if (!ChoLogManagerSettings.Me.TurnOn) return;

            if (ChoLogManagerSettings.Me.LoggerManagers == null ||
                ChoLogManagerSettings.Me.LoggerManagers.Length == 0) return;

            ChoLogEntry logMessage;
            if (message is ChoLogEntry)
            {
                logMessage = message as ChoLogEntry;
            }
            else
            {
                logMessage = ChoObjectManagementFactory.CreateInstance<ChoLogEntry>(typeof(ChoLogEntry));
                logMessage.Message = message.ToString();
            }

            logMessage.Enrich(callerStackFrame);

            //TODO: Make call to individual logger parallel / simultanously
            foreach (IChoLogManager logManager in ChoLogManagerSettings.Me.LoggerManagers)
            {
                if (logManager == null) continue;

                try
                {
                    logManager.Log(logMessage);
                }
                catch (Exception)
                {
					//ChoStreamProfile.WriteLine(ChoReservedDirectoryName.Settings, ChoType.GetLogFileName(typeof(ChoLogger)), String.Format("[{0}]: {1}", logManager.GetType().FullName, ex.Message));
                }
            }
        }

        #endregion Shared Members (Internal)
    }
}
