#region Header

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Copyright (c) 2009-2010 Raj Nagalingam.
 *    All rights reserved.
 *
 * This program and the accompanying materials are made available under
 * the terms of the Common Public License v1.0 which accompanies this
 * distribution.
 *
 * Redistribution and use in source and binary forms, with or
 * without modification, are permitted provided that the following
 * conditions are met:
 *
 * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in
 * the documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
 * OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 *<author>Raj Nagalingam</author>
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.IO;
    using Cinchoo.Core.Reflection;
    using Cinchoo.Core.Text;
    using Cinchoo.Core.Win32;
    using Microsoft.Win32;
    using Cinchoo.Core.ServiceProcess;
    using Cinchoo.Core.Services;
    using System.Collections.Generic;
    using Cinchoo.Core.Shell;
    using Cinchoo.Core.Configuration;
    using System.Web;
    using System.Windows;
    using System.Windows.Forms;
    using Cinchoo.Core.Windows.Forms;
    using System.Reflection;
    using Cinchoo.Core.Compiler;
    using System.Threading.Tasks;
    using System.ComponentModel;
    using System.Security.Principal;
    using System.Configuration;

    #endregion NameSpaces

    public enum ChoApplicationMode { NA, Console, Service, Windows, Web }
    public enum ChoWindowsAppType { NA, WinForms, WPF }

    public static class ChoExitCodes
    {
        public const int ActivateFirstInstanceNExit = -100;
    }

    public class ChoFatalErrorEventArgs : EventArgs
    {
        public readonly int ErrorCode;
        public readonly string ErrorMsg;
        public readonly Exception Exception;

        public ChoFatalErrorEventArgs(int errorCode, string errorMsg)
        {
            ErrorCode = errorCode;
            ErrorMsg = errorMsg;
        }

        public ChoFatalErrorEventArgs(int errorCode, Exception exception)
        {
            ErrorCode = errorCode;
            Exception = exception;
        }

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder(GetType().Name);

            msg.AppendFormatLine("ErrorCode: {0}".FormatString(ErrorCode));
            msg.AppendFormatLine("ErrorMsg: {0}".FormatString(ErrorMsg));
            msg.AppendFormatLine("Exception: ");
            if (Exception != null)
                msg.AppendFormatLine(Exception.ToString().Indent());

            return msg.ToString();
        }
    }

    public partial class ChoApplication
    {
        #region Constants

        private const string RegRunSubKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string RegRunOnceSubKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce";

        #endregion Constants

        #region Shared Data Members (Private)

        private static readonly Queue<Tuple<bool, string, string>> _queueTraceMsg = new Queue<Tuple<bool, string, string>>();
        private static readonly object _hostLock = new object();
        private static ChoApplicationHost _applicationHost;
        private static readonly object _padLock = new object();
        private static bool _isInitialized = false;
        private static EventLog _elApplicationEventLog;
        private static EventLog _elSystemEventLog;
        private static DateTime _logBackupDay;
        private static ChoTextWriterTraceListener _frxTextWriterTraceListener;

        internal static bool ServiceInstallation;

        /// <summary>
        /// Registry Key point to SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run
        /// </summary>
        private static RegistryKey _rkAppRun;

        /// <summary>
        /// Registry Key point to SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce
        /// </summary>
        private static RegistryKey _rkAppRunOnce;

        internal static bool IsInitialized
        {
            get { return _isInitialized; }
            set { _isInitialized = value; }
        }

        #endregion Shared Data Members (Private)

        #region Shared Data Members (Public)

        public static string AppEnvironment
        {
            get;
            internal set;
        }

        private static ChoApplicationMode _applicationMode = ChoApplicationMode.NA;
        public static ChoApplicationMode ApplicationMode
        {
            get 
            {
                ChoApplicationMode? applicationMode = ChoFrameworkCmdLineArgs.GetApplicationMode();
                if (applicationMode == null)
                    return _applicationMode;
                else
                    return applicationMode.Value;
            }
            set
            {
                //if (IsInitialized)
                //    throw new ChoFatalApplicationException("Can't set ApplicationMode now. Must be called before Cinchoo Framework intialized.");
                
                _applicationMode = value;
            }
        }

        public static ChoWindowsAppType WindowsAppType
        {
            get;
            internal set;
        }

        private static Mutex _singleInstanceMutex;

        private static string _appDomainName;
        public static string AppDomainName
        {
            get { return _appDomainName; }
            private set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "AppDomainName");
                _appDomainName = value;
            }
        }

        public static int ProcessId
        {
            get;
            private set;
        }

        private static string _processFilePath;
        public static string ProcessFilePath
        {
            get { return _processFilePath; }
            private set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "ProcessFilePath");
                _processFilePath = value;
            }
        }

        public static bool UnmanagedCodePermissionAvailable
        {
            get;
            private set;
        }

        private static string _entryAssemblyLocation;
        public static string EntryAssemblyLocation
        {
            get { return _entryAssemblyLocation; }
            private set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "EntryAssemblyLocation");
                _entryAssemblyLocation = value;
            }
        }

        private static string _entryAssemblyFileName;
        public static string EntryAssemblyFileName
        {
            get { return _entryAssemblyFileName; }
            private set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "EntryAssemblyFileName");
                _entryAssemblyFileName = value;
            }
        }

        public static string ApplicationBaseDirectory
        {
            get { return ChoPath.AssemblyBaseDirectory; }
        }

        public static string ApplicationConfigDirectory
        {
            get;
            private set;
        }

        private static string _applicationLogDirectory;
        public static string ApplicationLogDirectory
        {
            get { return _applicationLogDirectory; }
            private set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "ApplicationLogDirectory");
                _applicationLogDirectory = value;
            }
        }

        public static Func<bool> VerifyAnotherInstanceRunning
        {
            get;
            set;
        }

        public static Func<string> GetSharedEnvironmentConfigXml
        {
            get;
            set;
        }

        public static Action<ChoAppFrxSettings> OnInitialize
        {
            get;
            set;
        }

        #endregion Shared Data Members (Public)

        #region Events

        public static event EventHandler<ChoObjectFormaterEventArgs> ObjectFormatterResolve;

        public static event EventHandler<ChoUnknownProperyEventArgs> PropertyResolve;
        //internal static event EventHandler<ChoFrxParamsEventArgs> ApplyFrxParamsOverridesInternal;
        //public static event EventHandler<ChoFrxParamsEventArgs> ApplyFrxParamsOverrides
        //{
        //    add
        //    {
        //        if (IsInitialized)
        //            throw new ChoFatalApplicationException("Can't subscribe to ApplyFrxParamsOverrides event now. Must be called before Cinchoo Framework intialized.");
 
        //        ApplyFrxParamsOverridesInternal += value;
        //    }
        //    remove
        //    {
        //        ApplyFrxParamsOverridesInternal -= value;
        //    }
        //}

        internal static bool InternalEventsSubscriped = false;

        internal static bool IsAfterConfigurationManagerInitializedSubscribed = false;
        internal static event EventHandler AfterConfigurationManagerInitializedInternal;
        public static event EventHandler AfterConfigurationManagerInitialized
        {
            add
            {
                IsAfterConfigurationManagerInitializedSubscribed = true;

                if (IsInitialized)
                    throw new ChoFatalApplicationException("Can't subscribe to AfterConfigurationManagerInitialized event now. Must be called before Cinchoo Framework initialized.");

                AfterConfigurationManagerInitializedInternal = null;
                AfterConfigurationManagerInitializedInternal += value;
            }
            remove
            {
                AfterConfigurationManagerInitializedInternal -= value;
            }
        }

        internal static event EventHandler<ChoEventArgs<object>> AfterAppFrxSettingsLoadedInternal;
        public static event EventHandler<ChoEventArgs<object>> AfterAppFrxSettingsLoaded
        {
            add
            {
                if (IsInitialized)
                    throw new ChoFatalApplicationException("Can't subscribe to AfterAppFrxSettingsLoaded event now. Must be called before Cinchoo Framework initialized.");

                AfterAppFrxSettingsLoadedInternal += value;
            }
            remove
            {
                AfterAppFrxSettingsLoadedInternal -= value;
            }
        }

        internal static event EventHandler<ChoEventArgs<ChoGlobalApplicationSettings>> ApplyGlobalApplicationSettingsOverridesInternal;
        public static event EventHandler<ChoEventArgs<ChoGlobalApplicationSettings>> ApplyGlobalApplicationSettingsOverrides
        {
            add
            {
                if (IsInitialized)
                    throw new ChoFatalApplicationException("Can't subscribe to ApplyGlobalApplicationSettingsOverrides event now. Must be called before Cinchoo Framework intialized.");

                ApplyGlobalApplicationSettingsOverridesInternal += value;
            }
            remove
            {
                ApplyGlobalApplicationSettingsOverridesInternal -= value;
            }
        }
        internal static event EventHandler<ChoEventArgs<ChoMetaDataFilePathSettings>> ApplyMetaDataFilePathSettingsOverridesInternal;
        public static event EventHandler<ChoEventArgs<ChoMetaDataFilePathSettings>> ApplyMetaDataFilePathSettingsOverrides
        {
            add
            {
                if (IsInitialized)
                    throw new ChoFatalApplicationException("Can't subscribe to ApplyMetaDataFilePathSettingsOverrides event now. Must be called before Cinchoo Framework intialized.");

                ApplyMetaDataFilePathSettingsOverridesInternal += value;
            }
            remove
            {
                ApplyMetaDataFilePathSettingsOverridesInternal -= value;
            }
        }
        internal static event EventHandler<ChoEventArgs<ChoNotifyIcon>> AfterNotifyIconConstructedInternal;
        public static event EventHandler<ChoEventArgs<ChoNotifyIcon>> AfterNotifyIconConstructed
        {
            add
            {
                if (IsInitialized)
                    throw new ChoFatalApplicationException("Can't subscribe to AfterNotifyIconConstructed event now. Must be called before Cinchoo Framework intialized.");

                AfterNotifyIconConstructedInternal += value;
            }
            remove
            {
                AfterNotifyIconConstructedInternal -= value;
            }
        }
        public static event EventHandler<ChoFatalErrorEventArgs> FatalApplicationException;

        #endregion Events

        #region Constructors

        static ChoApplication()
        {
            try
            {
                AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            }
            catch { }
            try
            {
                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            }
            catch { }

            ChoAssembly.Initialize();
            //Hook unhandled exception handler
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            ApplicationMode = ChoWindowsManager.ApplicationMode;
        }

        #endregion Constructors

        #region Shared Members (Public)

        internal static void OnFatalApplicationException(int errorCode, string errorMsg)
        {
            FatalApplicationException.Raise(null, new ChoFatalErrorEventArgs(errorCode, errorMsg));

            DisplayMsg(errorMsg);

            Environment.FailFast(errorMsg);
        }

        private static string UnwrapException(Exception ex)
        {
            if (ex == null) return null;
            try
            {
                return ChoApplicationException.ToString(ex);
            }
            catch
            {
                return ex.ToString();
            }
        }

        public static void DisplayMsg(string msg, Exception ex = null, ConsoleColor foregroundColor = ConsoleColor.Gray, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            try
            {
                if (ChoApplication.ApplicationMode == ChoApplicationMode.Windows)
                {
                    try
                    {
                        if (!(ex is System.Windows.Markup.XamlParseException))
                            new ChoFatalErrorMessageBox(msg, ex != null ? ex.ToString() : null).ShowDialog();
                        else
                            ChoApplication.WriteToEventLog(ex != null ? UnwrapException(ex) : msg, EventLogEntryType.Error);
                    }
                    catch 
                    {
                        ChoApplication.WriteToEventLog(ex != null ? UnwrapException(ex) : msg, EventLogEntryType.Error);
                    }
                }
                else if (ChoApplication.ApplicationMode == ChoApplicationMode.Console)
                {
                    //try
                    //{
                    //    ChoConsole.WriteLine(ex != null ? UnwrapException(ex) : msg, foregroundColor, backgroundColor);
                    //}
                    //catch
                    //{
                        Console.WriteLine(ex != null ? UnwrapException(ex) : msg);
                    //}
                }
                else
                    ChoApplication.WriteToEventLog(ex != null ? UnwrapException(ex) : msg, EventLogEntryType.Error);
            }
            catch { }
        }

        internal static void OnFatalApplicationException(int errorCode, Exception exception)
        {
            FatalApplicationException.Raise(null, new ChoFatalErrorEventArgs(errorCode, exception));
            DisplayMsg(exception.Message, exception);
            Environment.FailFast("Fatal error occured.", exception);
        }

        public static void Trace(string msg, string logFileName = null)
        {
            if (!IsInitialized)
                _queueTraceMsg.Enqueue(new Tuple<bool, string, string>(ChoTraceSwitch.Switch.TraceVerbose, msg, logFileName));
            else
                Trace(ChoTraceSwitch.Switch.TraceVerbose, msg, logFileName);
        }

        public static void Trace(bool condition, string msg, string logFileName = null)
        {
            //if (ApplicationMode == ChoApplicationMode.Console && condition)
            //    Console.WriteLine(msg);

            if (!IsInitialized)
                _queueTraceMsg.Enqueue(new Tuple<bool, string, string>(condition, msg, logFileName));
            else
                TraceInternal(condition, msg, logFileName);
        }

        public static void TraceInternal(bool condition, string msg, string logFileName = null)
        {
            if (!condition) return;

            if (logFileName.IsNullOrWhiteSpace())
                System.Diagnostics.Trace.WriteLineIf(condition, msg);
            else
            {
                try
                {
                    if (Path.IsPathRooted(logFileName))
                    {
                        ChoFile.WriteLine(logFileName, msg);
                    }
                    else
                    {
                        ChoFile.WriteLine(Path.Combine(ApplicationLogDirectory, logFileName), msg);
                    }
                }
                catch (Exception ex)
                {
                    ChoApplication.WriteToEventLog(UnwrapException(ex), EventLogEntryType.Error);
                }

            }
        }

        private static bool _disabledEventLog = false;
        /// <summary>
        /// Writes an information type entry, with the given message text, to the event log.
        /// </summary>
        /// <param name="message">The string to write to the event log.</param>
        public static void WriteToEventLog(string message)
        {
            try
            {
                //Trace(message);

                try
                {
                    if (!_disabledEventLog)
                    {
                        if (_elApplicationEventLog != null)
                            _elApplicationEventLog.WriteEntry(message);
                        else if (_elSystemEventLog != null)
                            _elSystemEventLog.WriteEntry(message);
                    }
                }
                catch (SecurityException sEx)
                {
                    _disabledEventLog = true;
                    Trace(sEx.ToString());
                }
            }
            catch { } //If the event log is full or any other errors while writing to event log, we dont need to let the service to stop working or die
        }

        /// <summary>
        /// Writes an error, warning, information, success audit, or failure audit entry with the given message text to the event log.
        /// </summary>
        /// <param name="message">The string to write to the event log.</param>
        /// <param name="type">One of the System.Diagnostics.EventLogEntryType values.</param>
        public static void WriteToEventLog(string message, EventLogEntryType type)
        {
            try
            {
                //if (type == EventLogEntryType.Error ||
                //    type == EventLogEntryType.FailureAudit)
                //    Trace(true, message);
                //else
                //    Trace(message);

                try
                {
                    if (!_disabledEventLog)
                    {
                        if (_elApplicationEventLog != null)
                            _elApplicationEventLog.WriteEntry(message, type);
                        else if (_elSystemEventLog != null)
                            _elSystemEventLog.WriteEntry(message, type);
                    }
                }
                catch (SecurityException sEx)
                {
                    _disabledEventLog = true;
                    Trace(sEx.ToString());
                }
            }
            catch { } //If the event log is full or any other errors while writing to event log, we dont need to let the service to stop working or die
        }

        /// <summary>
        /// Writes an entry with the given message text and application-defined event identifier to the event log.
        /// </summary>
        /// <param name="message">The string to write to the event log.</param>
        /// <param name="type">One of the System.Diagnostics.EventLogEntryType values.</param>
        /// <param name="eventID">The application-specific identifier for the event.</param>
        public static void WriteToEventLog(string message, EventLogEntryType type, int eventID)
        {
            try
            {
                //if (type == EventLogEntryType.Error ||
                //    type == EventLogEntryType.FailureAudit)
                //    Trace(true, message);
                //else
                //    Trace(message);

                try
                {
                    if (!_disabledEventLog)
                    {
                        if (_elApplicationEventLog != null)
                            _elApplicationEventLog.WriteEntry(message, type, eventID);
                        else if (_elSystemEventLog != null)
                            _elSystemEventLog.WriteEntry(message, type, eventID);
                    }
                }
                catch (SecurityException sEx)
                {
                    _disabledEventLog = true;
                    Trace(sEx.ToString());
                }
            }
            catch { } //If the event log is full or any other errors while writing to event log, we dont need to let the service to stop working or die
        }

        /// <summary>
        /// Writes an entry with the given message text, application-defined event identifier,
        /// and application-defined category to the event log.
        /// </summary>
        /// <param name="message">The string to write to the event log.</param>
        /// <param name="type">One of the System.Diagnostics.EventLogEntryType values.</param>
        /// <param name="eventID">The application-specific identifier for the event.</param>
        /// <param name="category">The application-specific subcategory associated with the message.</param>
        public static void WriteToEventLog(string message, EventLogEntryType type, int eventID, short category)
        {
            try
            {
                //if (type == EventLogEntryType.Error ||
                //    type == EventLogEntryType.FailureAudit)
                //    Trace(true, message);
                //else
                //    Trace(message);

                try
                {
                    if (!_disabledEventLog)
                    {
                        if (_elApplicationEventLog != null)
                            _elApplicationEventLog.WriteEntry(message, type, eventID, category);
                        else if (_elSystemEventLog != null)
                            _elSystemEventLog.WriteEntry(message, type, eventID, category);
                    }
                }
                catch (SecurityException sEx)
                {
                    _disabledEventLog = true;
                    Trace(sEx.ToString());
                }
            }
            catch { } //If the event log is full or any other errors while writing to event log, we dont need to let the service to stop working or die
        }

        #endregion Shared Members (Public)

        #region Shared Members (Private)

        private static void CheckApplicationInitialized()
        {
            if (!IsInitialized)
                throw new ApplicationException("Application is not initalized. Please invoke ChoApplication.Initialize() method to initialize it.");
        }

        private static void TerminateApp(Exception ex)
        {
            string errorMsg = "An application error occurred. Please contact the adminstrator with the following information:\n\n";
            bool failFast = true;

            if (ex != null)
            {
                ChoDoNotShowStackTraceAttribute attr = ChoType.GetAttribute<ChoDoNotShowStackTraceAttribute>(ex.GetType());
                if (attr != null)
                {
                    errorMsg = ex.Message;
                    failFast = false;
                    ex = null;
                }
                else
                    errorMsg = UnwrapException(ex);
                ChoApplication.WriteToEventLog(errorMsg);
                //ex = null;
                //errorMsg += ex.Message + "\n\nStack Trace:\n" + ex.StackTrace;
                //ChoApplication.WriteToEventLog(errorMsg + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace, EventLogEntryType.Error);

                //if (Host != null)
                //    Host.TerminateService(ex);
            }
            else
                ChoApplication.WriteToEventLog(errorMsg + "Unknown exception occured.", EventLogEntryType.Error);

            DisplayMsg(ex != null ? ex.Message : errorMsg, ex);
            //Console.WriteLine(errorMsg);
            System.Diagnostics.Trace.WriteLine(errorMsg);

            ChoFramework.Shutdown();
            if (failFast) // || ChoApplication.ApplicationMode != ChoApplicationMode.Service)
                Environment.FailFast(errorMsg, ex);
            else
                Environment.Exit(-1);
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            TerminateApp(e.Exception);
        }

        /// <summary>
        /// Unhandled exception handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            TerminateApp((Exception)e.ExceptionObject);
        }

        internal static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            TerminateApp(e.Exception);
        }

        internal static void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            TerminateApp(e.Exception);
        }

        #endregion Shared Members (Private)

        #region RunAtSystemStartup Overloads

        /// <summary>
        /// Set/Remove the running executable to run at system startup
        /// </summary>
        /// <returns>true, if the operation is successfull. Otherwise, false.</returns>
        public static bool RunAtSystemStartup()
        {
            return RunAtSystemStartup(false);
        }

        /// <summary>
        /// Set/Remove the running executable to run at system startup
        /// </summary>
        /// <param name="remove">true, it remove the application from running at system startup.</param>
        /// <returns>true, if the operation is successfull. Otherwise, false.</returns>
        public static bool RunAtSystemStartup(bool remove)
        {
            if (HttpContext.Current == null)
            {
                string appLocation = ChoAssembly.GetEntryAssembly().Location;
                return RunAtSystemStartup(ChoGlobalApplicationSettings.Me.ApplicationNameWithoutExtension, appLocation, remove);
            }
            return true;
        }

        /// <summary>
        /// Set/Remove the running executable to run at system startup
        /// </summary>
        /// <param name="appName">A name to represent the application name in the registry</param>
        /// <returns>true, if the operation is successfull. Otherwise, false.</returns>
        public static bool RunAtSystemStartup(string appName, string appLocation)
        {
            return RunAtSystemStartup(appName, appLocation, false);
        }

        /// <summary>
        /// Set/Remove the running executable to run at system startup
        /// </summary>
        /// <param name="appName">A name to represent the application name in the registry</param>
        /// <param name="remove">true, it remove the application from running at system startup.</param>
        /// <returns>true, if the operation is successfull. Otherwise, false.</returns>
        public static bool RunAtSystemStartup(string appName, string appLocation, bool remove)
        {
            if (String.IsNullOrEmpty(appName))
                throw new ArgumentException("AppName is missing.");

            return SetValueToRegistry(_rkAppRun, appName, @"""{0}""".FormatString(appLocation), remove);
        }

        public static bool IsAppRunAtSystemStartup()
        {
            return IsAppRunAtSystemStartup(ChoGlobalApplicationSettings.Me.ApplicationNameWithoutExtension);
        }

        public static bool IsAppRunAtSystemStartup(string appName)
        {
            if (String.IsNullOrEmpty(appName))
                throw new ArgumentException("AppName is missing.");

            if (_rkAppRun != null)
                return !((string)_rkAppRun.GetValue(appName, null)).IsNullOrWhiteSpace();
            else
                return false;
        }

        #endregion RunAtSystemStartup Overloads

        #region RunOnceAtSystemStartup Overloads

        /// <summary>
        /// Set/Remove the running executable to run once at system startup
        /// </summary>
        /// <returns>true, if the operation is successfull. Otherwise, false.</returns>
        public static bool RunOnceAtSystemStartup()
        {
            return RunOnceAtSystemStartup(false);
        }

        /// <summary>
        /// Set/Remove the running executable to run once at system startup
        /// </summary>
        /// <param name="remove">true, it remove the application from running at system startup.</param>
        /// <returns>true, if the operation is successfull. Otherwise, false.</returns>
        public static bool RunOnceAtSystemStartup(bool remove)
        {
            if (HttpContext.Current == null)
            {
                string appLocation = ChoAssembly.GetEntryAssembly().Location;
                return RunOnceAtSystemStartup(ChoGlobalApplicationSettings.Me.ApplicationNameWithoutExtension, @"""{0}""".FormatString(appLocation), remove);
            }
            return true;
        }

        /// <summary>
        /// Set/Remove the running executable to run once at system startup
        /// </summary>
        /// <param name="appName">A name to represent the application name in the registry</param>
        /// <returns>true, if the operation is successfull. Otherwise, false.</returns>
        public static bool RunOnceAtSystemStartup(string appName, string appLocation)
        {
            return RunOnceAtSystemStartup(appName, appLocation, false);
        }

        /// <summary>
        /// Set/Remove the running executable to run once at system startup
        /// </summary>
        /// <param name="appName">A name to represent the application name in the registry</param>
        /// <param name="remove">true, it remove the application from running at system startup.</param>
        /// <returns>true, if the operation is successfull. Otherwise, false.</returns>
        public static bool RunOnceAtSystemStartup(string appName, string appLocation, bool remove)
        {
            if (String.IsNullOrEmpty(appName))
                throw new ArgumentException("AppName is missing.");

            return SetValueToRegistry(_rkAppRunOnce, String.Format("!{0}", appName), @"""{0}""".FormatString(appLocation), remove);
        }

        #region Shared Members (Public)

        public static int GetThreadId()
        {
            int threadId = 0;
            if (UnmanagedCodePermissionAvailable)
            {
                try
                {
                    threadId = ChoKernel32Core.GetCurrentThreadId();
                }
                catch (Exception ex)
                {
                    threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                    Trace(ChoTraceSwitch.Switch.TraceError, ex.ToString());
                }
            }
            else
            {
                threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                Trace(ChoTraceSwitch.Switch.TraceError, "Failed to retrieve value due to unmanaged code permission denied.");
            }

            return threadId;
        }

        public static string GetThreadName()
        {
            string threadName = null;
            try
            {
                threadName = Thread.CurrentThread.Name;
            }
            catch (Exception ex)
            {
                Trace(ChoTraceSwitch.Switch.TraceError, ex.ToString());
            }
            if (String.IsNullOrEmpty(threadName))
                threadName = String.Format("ChoThread {0}", GetThreadId());

            return threadName;
        }

        #endregion Shared Members (Public)

        #endregion RunOnceAtSystemStartup Overloads

        #region Shared Methods (Private)

        internal static void RaiseAfterConfigurationManagerInitialized()
        {
            AfterConfigurationManagerInitializedInternal.Raise(null, null);
        }

        internal static void RaiseGlobalApplicationSettingsOverrides(ChoGlobalApplicationSettings instance)
        {
            ApplyGlobalApplicationSettingsOverridesInternal.Raise(null, new ChoEventArgs<ChoGlobalApplicationSettings>(instance));
        }

        internal static void RaiseMetaDataFilePathSettingsOverrides(ChoMetaDataFilePathSettings instance)
        {
            ApplyMetaDataFilePathSettingsOverridesInternal.Raise(null, new ChoEventArgs<ChoMetaDataFilePathSettings>(instance));
        }

        internal static void RaiseAfterNotifyIconConstructed(ChoNotifyIcon instance)
        {
            AfterNotifyIconConstructedInternal.Raise(null, new ChoEventArgs<ChoNotifyIcon>(instance));
        }

        internal static void RaiseAfterAppFrxSettingsLoaded(object instance)
        {
            AfterAppFrxSettingsLoadedInternal.Raise(null, new ChoEventArgs<object>(instance));
        }

        //internal static void RaiseApplyFrxParamsOverrides(bool init = false)
        //{
        //    //ApplyFrxParamsOverridesInternal.Raise(null, new ChoFrxParamsEventArgs(
        //    //    ChoFrxParamsEventArgs.DefaultGlobalApplicationSettings, ChoFrxParamsEventArgs.DefaultMetaDataFilePathSettings));

        //    //ChoFrxParamsEventArgs.Initialize();

        //    if (init)
        //    {
        //        ChoEnvironmentSettings.CheckNChangeEnvironment();
        //        ChoGlobalApplicationSettings.Me.PostInitialize();
        //    }
        //    //ChoGlobalApplicationSettings.Me.Initialize();
        //    //ChoMetaDataFilePathSettings.Me.Initialize();
        //}

        /// <summary>
        /// Set/Remove the running executable to run at system startup
        /// </summary>
        /// <param name="appName">A name to represent the application name in the registry</param>
        /// <param name="appLocation">Full path of the application to be set.</param>
        /// <returns>true, if the operation is successfull. Otherwise, false.</returns>
        private static bool SetValueToRegistry(RegistryKey regKey, string appName, string appLocation, bool remove)
        {
            if (String.IsNullOrEmpty(appName))
                throw new ArgumentException("AppName is missing.");

            if (!remove && String.IsNullOrEmpty(appLocation))
                throw new ArgumentException("AppLocation is missing.");

            if (regKey == null)
                return false;

            if (remove)
                regKey.DeleteValue(appName, false);
            else
                regKey.SetValue(appName, appLocation);

            return true;
        }

        private static string GetProcessName()
        {
            StringBuilder buffer = new StringBuilder(1024);
            int length = ChoKernel32Core.GetModuleFileName(ChoKernel32Core.GetModuleHandle(null), buffer, buffer.Capacity);
            return buffer.ToString();
        }

        #endregion Shared Methods (Private)

        #region Shared Methods (Internal)

        internal static bool OnObjectFormatterResolve(Type type, out ICustomFormatter formatter)
        {
            formatter = null;
            EventHandler<ChoObjectFormaterEventArgs> objectFormatterResolve = ObjectFormatterResolve;
            if (objectFormatterResolve != null)
            {
                ChoObjectFormaterEventArgs objectFormatterEventArgs = new ChoObjectFormaterEventArgs(type);
                foreach (EventHandler<ChoObjectFormaterEventArgs> delegateMember in objectFormatterResolve.GetInvocationList())
                {
                    delegateMember(null, objectFormatterEventArgs);
                    if (!objectFormatterEventArgs.Resolved)
                        continue;

                    formatter = objectFormatterEventArgs.Formatter;
                    return true;
                }
            }

            if (type.GetEnumerableType() == typeof(char))
            {
                formatter = ChoCharFormatter.Instance;
                return true;
            }

            return false;
        }

        internal static bool OnPropertyResolve(string propertyName, string format, object context, out string propertyValue)
        {
            propertyValue = null;
            EventHandler<ChoUnknownProperyEventArgs> unknownPropertyFound = PropertyResolve;
            if (unknownPropertyFound != null)
            {
                ChoUnknownProperyEventArgs unknownProperyEventArgs = new ChoUnknownProperyEventArgs(propertyName, format) { Context = context };
                foreach (EventHandler<ChoUnknownProperyEventArgs> delegateMember in unknownPropertyFound.GetInvocationList())
                {
                    delegateMember(null, unknownProperyEventArgs);
                    if (!unknownProperyEventArgs.Resolved)
                        continue;

                    propertyValue = unknownProperyEventArgs.PropertyValue;
                    return true;
                }
            }
            return false;
        }

        internal static void Refresh()
        {
            if (_singleInstanceMutex != null)
                _singleInstanceMutex.Dispose();

            IsInitialized = false;
            Initialize();
        }

        private static string _logFileName;
        private static string _logDirectory;

        private static void Initialize()
        {
            if (IsInitialized)
                return;

            lock (_padLock)
            {
                if (IsInitialized)
                    return;

                _logBackupDay = DateTime.Today;

                InitializeAppInfo();

                if (!ServiceInstallation)
                {
                    if (ApplicationMode != ChoApplicationMode.Service
                        && ApplicationMode != ChoApplicationMode.Web)
                    {
                        if (ChoWindowsIdentity.IsAdministrator())
                        {
                            try
                            {
                                _rkAppRun = Registry.CurrentUser.OpenSubKey(RegRunSubKey, true);
                                if (_rkAppRun == null)
                                    _rkAppRun = Registry.CurrentUser.CreateSubKey(RegRunSubKey);

                                RunAtSystemStartup(!ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.RunAtStartup);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Trace.TraceError(ex.ToString());
                            }

                            try
                            {
                                _rkAppRunOnce = Registry.CurrentUser.OpenSubKey(RegRunOnceSubKey, true);
                                if (_rkAppRunOnce == null)
                                    _rkAppRunOnce = Registry.CurrentUser.CreateSubKey(RegRunOnceSubKey);

                                RunOnceAtSystemStartup(!ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.RunOnceAtStartup);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Trace.TraceError(ex.ToString());
                            }
                        }
                    }
                }

                ChoGuard.ArgumentNotNullOrEmpty(ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath, "Application Config Path");

                try
                {
                    _elApplicationEventLog = new EventLog("Application", Environment.MachineName, ChoGlobalApplicationSettings.Me.ApplicationName);
                    _elApplicationEventLog.Log = "Application";
                    _elApplicationEventLog.Source = ChoGlobalApplicationSettings.Me.EventLogSourceName;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(ex.ToString());
                }

                //ApplicationConfigDirectory = Path.GetDirectoryName(ChoPath.GetFullPath(ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath));
                //if (!ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.SingleInstanceApp
                //    || (ChoGlobalApplicationSettings.Me.LogSettings.DoAppendProcessIdToLogDir.HasValue
                //    && ChoGlobalApplicationSettings.Me.LogSettings.DoAppendProcessIdToLogDir.Value))
                //{
                //    ApplicationConfigDirectory = Path.Combine(ApplicationConfigDirectory, ProcessId.ToString());
                //}

                //Add default text trace listerner, if not defined in the configuration file
                if (ChoGlobalApplicationSettings.Me.LogSettings.TraceLevel != TraceLevel.Off)
                    Directory.CreateDirectory(ApplicationLogDirectory);

                try
                {
                    if (_logFileName != ChoGlobalApplicationSettings.Me.LogSettings.LogFileName
                        || _logDirectory != ApplicationLogDirectory /*ChoApplication.ApplicationLogDirectory */)
                    {
                        _logFileName = ChoGlobalApplicationSettings.Me.LogSettings.LogFileName;
                        _logDirectory = ApplicationLogDirectory; // ChoApplication.ApplicationLogDirectory;

                        if (ChoGlobalApplicationSettings.Me.LogSettings.TraceLevel != TraceLevel.Off)
                        {
                            ChoTextWriterTraceListener frxTextWriterTraceListener = new Cinchoo.Core.Diagnostics.ChoTextWriterTraceListener("Cinchoo",
                                String.Format("BASEFILENAME={0};DIRECTORYNAME={1};FILEEXT={2}", ChoGlobalApplicationSettings.Me.LogSettings.LogFileName,
                                ApplicationLogDirectory /*ChoApplication.ApplicationLogDirectory*/, ChoReservedFileExt.Txt));

                            if (_frxTextWriterTraceListener != null)
                                System.Diagnostics.Trace.Listeners.Remove(_frxTextWriterTraceListener);
                            else
                            {
                                ChoGlobalTimerServiceManager.Register("Logbackup", () =>
                                {
                                    if (DateTime.Today != _logBackupDay)
                                    {
                                        _logBackupDay = DateTime.Today;
                                        ChoTrace.Backup();
                                    }
                                }, 60000);
                            }

                            _frxTextWriterTraceListener = frxTextWriterTraceListener;
                            if (_frxTextWriterTraceListener != null)
                                System.Diagnostics.Trace.Listeners.Add(_frxTextWriterTraceListener);

                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(ex.ToString());
                }

                while (_queueTraceMsg.Count > 0)
                {
                    Tuple<bool, string, string> tuple = _queueTraceMsg.Dequeue();
                    TraceInternal(tuple.Item1, tuple.Item2, tuple.Item3);
                }
                //Debugger.Break();
                IsInitialized = true;

                ChoApplication.WriteToEventLog(ChoApplication.ToString());
                //ChoApplication.WriteToEventLog(ChoGlobalApplicationSettings.Me.ToString());

                //Initialize other Framework Settings
                //ChoAssembly.Initialize();
                ChoConsole.Initialize();
                ChoConfigurationManager.Initialize();
            }
        }

        private static void InitializeAppInfo()
        {
            //RaiseApplyFrxParamsOverrides(true);

            try
            {
                EntryAssemblyLocation = ChoAssembly.GetEntryAssembly().Location;
                EntryAssemblyFileName = System.IO.Path.GetFileName(EntryAssemblyLocation);
            }
            catch (System.Security.SecurityException ex)
            {
                // This security exception will occur if the caller does not have 
                // some undefined set of SecurityPermission flags.
                Trace(ChoTraceSwitch.Switch.TraceError, ex.ToString());
            }

            if (!ServiceInstallation)
            {
                if (ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.SingleInstanceApp)
                {
                    Func<bool> verifyAnotherInstanceRunning = VerifyAnotherInstanceRunning;
                    if (verifyAnotherInstanceRunning != null)
                    {
                        bool instanceExists = verifyAnotherInstanceRunning();
                        if (instanceExists)
                            RaiseErrorOrActivateFirstInstance();
                    }
                    else
                    {
                        bool createdNew = true;
                        _singleInstanceMutex = new Mutex(true, ChoGlobalApplicationSettings.Me.ApplicationName, out createdNew);
                        if (!createdNew)
                            RaiseErrorOrActivateFirstInstance();
                    }
                }

                if (!ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn)
                {
                    if (ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.HideWindow)
                        ChoWindowsManager.Hide();
                    else
                    {
                        ChoWindowsManager.Show();

                        ChoWindowsManager.AlwaysOnTop(ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.AlwaysOnTop);

                        if (!ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.AlwaysOnTop)
                        {
                            if (ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.BringWindowToTop)
                                ChoWindowsManager.BringWindowToTop();
                        }
                        //ChoWindowManager.ShowInTaskbar(ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.ShowInTaskbar);
                    }

                }

                if (ChoApplicationHost.ApplicationContext != null)
                {
                    ChoApplicationHost.ApplicationContext.Visible = ChoGlobalApplicationSettings.Me.TrayApplicationBehaviourSettings.TurnOn;
                }
            }

            #region Check for Unmanaged Code Permission Available

            // check whether the unmanaged code permission is available to avoid three potential stack walks
            SecurityPermission unmanagedCodePermission = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
            // avoid a stack walk by checking for the permission on the current assembly. this is safe because there are no
            // stack walk modifiers before the call.
            if (SecurityManager.IsGranted(unmanagedCodePermission))
            {
                try
                {
                    unmanagedCodePermission.Demand();
                    UnmanagedCodePermissionAvailable = true;
                }
                catch (SecurityException ex)
                {
                    Trace(ChoTraceSwitch.Switch.TraceError, ex.ToString());
                }
            }

            #endregion Check for Unmanaged Code Permission Available

            #region Get AppDomainName

            try
            {
                AppDomainName = AppDomain.CurrentDomain.FriendlyName;
            }
            catch (Exception ex)
            {
                Trace(ChoTraceSwitch.Switch.TraceError, ex.ToString());
            }

            #endregion Get AppDomainName

            #region Get ProcessId, ProcessName

            if (UnmanagedCodePermissionAvailable)
            {
                try
                {
                    ProcessId = ChoKernel32Core.GetCurrentProcessId();
                }
                catch (Exception ex)
                {
                    Trace(ChoTraceSwitch.Switch.TraceError, ex.ToString());

                    try
                    {
                        ProcessId = Process.GetCurrentProcess().Id;
                    }
                    catch { }
                }

                try
                {
                    ProcessFilePath = GetProcessName();
                }
                catch (Exception ex)
                {
                    Trace(ChoTraceSwitch.Switch.TraceError, ex.ToString());
                }
            }
            else
            {
                try
                {
                    ProcessId = Process.GetCurrentProcess().Id;
                }
                catch { }

                Trace(ChoTraceSwitch.Switch.TraceError, "Failed to retrieve value due to unmanaged code permission denied.");
            }

            #endregion Get ProcessId, ProcessName

            ApplicationConfigDirectory = Path.GetDirectoryName(ChoPath.GetFullPath(ChoGlobalApplicationSettings.Me.ApplicationConfigFilePath));
            ApplicationLogDirectory = ChoGlobalApplicationSettings.Me.ApplicationLogDirectory;

            if (!ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.SingleInstanceApp)
            {
                if (ChoGlobalApplicationSettings.Me.LogSettings.DoAppendProcessIdToLogDir == null 
                    || ChoGlobalApplicationSettings.Me.LogSettings.DoAppendProcessIdToLogDir.Value)
                {
                    ApplicationLogDirectory = Path.Combine(ApplicationLogDirectory, ProcessId.ToString());
                    ApplicationConfigDirectory = Path.Combine(ApplicationConfigDirectory, ProcessId.ToString());
                }
            }
            else
            {
                if (ChoGlobalApplicationSettings.Me.LogSettings.DoAppendProcessIdToLogDir != null
                    && ChoGlobalApplicationSettings.Me.LogSettings.DoAppendProcessIdToLogDir.Value)
                {
                    ApplicationLogDirectory = Path.Combine(ApplicationLogDirectory, ProcessId.ToString());
                    ApplicationConfigDirectory = Path.Combine(ApplicationConfigDirectory, ProcessId.ToString());
                }
            }
            //if (ChoGlobalApplicationSettings.Me.LogSettings.TraceLevel != TraceLevel.Off)
            //    Directory.CreateDirectory(ApplicationLogDirectory);

            ChoMetaDataFilePathSettings.Me.PostInitialize();
        }

        private static void RaiseErrorOrActivateFirstInstance()
        {
            if (ChoGlobalApplicationSettings.Me.ApplicationBehaviourSettings.ActivateFirstInstance)
            {
                if (ApplicationMode == ChoApplicationMode.Console
                    || ApplicationMode == ChoApplicationMode.Windows)
                {
                    string appProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                    if (!appProcessName.IsNullOrWhiteSpace())
                    {
                        int processId = Process.GetCurrentProcess().Id;
                        Process[] runningProcesses = Process.GetProcessesByName(appProcessName);
                        if (runningProcesses.Length != 1)
                        {
                            foreach (Process process in runningProcesses)
                            {
                                if (process.Id == processId) continue;

                                //ChoUser32.ShowWindowAsync(process.MainWindowHandle,
                                //    (int)SHOWWINDOW.SW_SHOWMINIMIZED);
                                ChoUser32.ShowWindowAsync(process.MainWindowHandle,
                                    (int)SHOWWINDOW.SW_RESTORE);
                                ChoUser32.ShowWindowAsync(process.MainWindowHandle,
                                    (int)SHOWWINDOW.SW_SHOWDEFAULT);
                                ChoWindowsManager.SetTop(process.MainWindowHandle);
                            }
                        }
                    }
                }

                Environment.Exit(ChoExitCodes.ActivateFirstInstanceNExit);
            }
            else
                ChoEnvironment.Exit(100, "Already another instance of this application running.");
        }

        #endregion Shared Methods (Internal)

        #region Object Overrides

        public static new string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("{0} Settings", typeof(ChoApplication).Name);

            msg.AppendFormatLine("AppEnvironment: {0}", AppEnvironment);
            msg.AppendFormatLine("ApplicationMode: {0}", ApplicationMode);
            msg.AppendFormatLine("AppDomainName: {0}", AppDomainName);
            msg.AppendFormatLine("ProcessId: {0}", ProcessId);
            msg.AppendFormatLine("ProcessFilePath: {0}", ProcessFilePath);
            msg.AppendFormatLine("UnmanagedCodePermissionAvailable: {0}", UnmanagedCodePermissionAvailable);
            msg.AppendFormatLine("EntryAssemblyLocation: {0}", EntryAssemblyLocation);
            msg.AppendFormatLine("EntryAssemblyFileName: {0}", EntryAssemblyFileName);
            msg.AppendFormatLine("ApplicationBaseDirectory: {0}", ApplicationBaseDirectory);
            msg.AppendFormatLine("ApplicationConfigDirectory: {0}", ApplicationConfigDirectory);
            msg.AppendFormatLine("ApplicationLogDirectory: {0}", ApplicationLogDirectory);

            return msg.ToString();
        }

        #endregion Object Overrides

        #region Shared Members (Public)

        public static void RunAsAdmin(string fileName, string args)
        {
            var processInfo = new ProcessStartInfo
            {
                Verb = "runas",
                FileName = fileName,
                Arguments = args,
            };

            try
            {
                Process.Start(processInfo);
            }
            catch (Win32Exception)
            {
                // Do nothing...
            }
        }

        public static void RestartAsAdmin()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = ChoApplication.EntryAssemblyLocation;
            startInfo.Arguments = String.Join(" ", ChoEnvironment.ToFullyQualifiedCommandLineArgs());
            startInfo.Verb = "runas";
            try
            {
                Process p = Process.Start(startInfo);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                return;
            }

            Exit();
        }

        public static void Restart()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = ChoApplication.EntryAssemblyLocation;
            startInfo.Arguments = String.Join(" ", ChoEnvironment.ToFullyQualifiedCommandLineArgs());
            if (ChoWindowsIdentity.IsAdministrator())
                startInfo.Verb = "runas";
            try
            {
                Process p = Process.Start(startInfo);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                return;
            }

            Exit();
        }

        public static void CheckNChangeEnvironment()
        {
            ChoEnvironmentSettings.CheckNChangeEnvironment();
        }

        public static void Exit()
        {
            System.Windows.Forms.Application.Exit();

            if (System.Windows.Application.Current != null)
                System.Windows.Application.Current.Shutdown();
        }

        //public static void Run(ChoApplicationHost host, string[] args)
        //{
        //    ChoGuard.ArgumentNotNull(host, "Host");

        //    if (_applicationHost != null)
        //        return;

        //    lock (_hostLock)
        //    {
        //        if (_applicationHost == null)
        //        {
        //            _applicationHost = host;
        //            _applicationHost.Args = args;
        //            ChoApplicationHost.IsApplicationHostUsed = true;

        //            ChoFramework.Initialize();
        //            ChoService.Initialize(_applicationHost);
        //        }
        //    }
        //}

        //public static void Run<T>(string[] args)
        //    where T : ChoApplicationHost
        //{
        //    if (_applicationHost != null)
        //        return;

        //    lock (_hostLock)
        //    {
        //        if (_applicationHost == null)
        //        {
        //            _applicationHost = Activator.CreateInstance<T>();
        //            _applicationHost.Args = args;
        //            ChoApplicationHost.IsApplicationHostUsed = true;

        //            ChoFramework.Initialize();
        //            ChoService.Initialize(_applicationHost);
        //        }
        //    }
        //}

        public static void Run(string[] args, ChoApplicationMode? applicationMode = null)
        {
            //if (_applicationHost != null)
            //    return;
            lock (_hostLock)
            {
                if (IsInitialized)
                    throw new ChoFatalApplicationException("Framework already initialized. When using ApplicationHost, explicit framework initialization must be avoided.");

                if (applicationMode != null)
                    ApplicationMode = applicationMode.Value;
                if (_applicationHost == null)
                {
                    _applicationHost = DiscoverAppHost();
                }
                ChoEnvironment.CommandLineArgs = args;
                ChoApplicationHost.IsApplicationHostUsed = true;

                ChoFramework.Initialize();
                ChoService.Initialize(_applicationHost);
            }
        }

        public static void ShowMainWindow(bool showAsDialog = true)
        {
            if (_applicationHost == null)
            {
                lock (_hostLock)
                {
                    _applicationHost = DiscoverAppHost();
                }
            }

            if (_applicationHost != null)
            {
                var app = _applicationHost.ApplicationObject;
                var wnd = _applicationHost.GetMainWindowObject();
                if (wnd is Window)
                {
                    ChoWindowsManager.HideConsoleWindow();
                    if (showAsDialog)
                        ((Window)wnd).ShowDialog();
                    else
                        ((Window)wnd).Show();
                }
                else if (wnd is Form)
                {
                    ChoWindowsManager.HideConsoleWindow();
                    if (showAsDialog)
                        ((Form)wnd).ShowDialog();
                    else
                        ((Form)wnd).Show();
                }
            }
        }

        private static ChoApplicationHost DiscoverAppHost()
        {
            ChoApplicationHost host = null;
            try
            {
                Type runInstallerType = null;
                if (ChoAppFrxSettings.Me.ApplicationHostType.IsNullOrWhiteSpace())
                {
                    //Dicover Service Installer from entry assembly
                    Assembly entryAssembly = ChoAssembly.GetEntryAssembly();

                    if (entryAssembly != null)
                    {
                        foreach (Type type in entryAssembly.GetTypes())
                        {
                            ChoApplicationHostAttribute runInstallerAttribute = type.GetCustomAttribute<ChoApplicationHostAttribute>();
                            if (runInstallerAttribute == null)
                                continue;

                            if (typeof(ChoApplicationHost).IsAssignableFrom(type))
                            {
                                runInstallerType = type;
                                break;
                            }
                        }
                    }

                    //Dicover Service Installer from all assemblies
                    if (runInstallerType == null)
                    {
                        Type[] types = ChoType.GetTypes<ChoApplicationHostAttribute>();
                        if (!types.IsNullOrEmpty())
                        {
                            foreach (Type type in types)
                            {
                                if (typeof(ChoApplicationHost).IsAssignableFrom(type))
                                {
                                    runInstallerType = type;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    runInstallerType = Type.GetType(ChoAppFrxSettings.Me.ApplicationHostType);
                }

                if (runInstallerType != null)
                    host = ChoActivator.CreateInstance(runInstallerType) as ChoApplicationHost;
                if (host == null)
                    throw new ChoFatalApplicationException("No type found with ChoApplicationHostAttribute in the all referenced assemblies.");
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (ChoCommandLineArgException)
            {
                throw;
                //ChoApplication.DisplayMsg(argEx.Message);
            }
            catch (ChoCommandLineArgUsageException)
            {
                throw;
                //ChoApplication.DisplayMsg(usageEx.Message);
            }
            catch (Exception ex)
            {
                //ChoApplication.WriteToEventLog(ex.ToString());
                throw new ChoFatalApplicationException("Application failed to discover ApplicationHost object.", ex);
            }

            return host;
        }

        public static ChoApplicationHost Host
        {
            get { return _applicationHost; }
        }

        public static ChoNotifyIcon NotifyIcon
        {
            get { return ChoApplicationHost.ApplicationContext != null ? ChoApplicationHost.ApplicationContext.NotifyIcon : null; }
        }

        #endregion Shared Members (Public)

        internal static IEnumerable<Dictionary<string, string>> GetPropertyHelpTexts()
        {
            EventHandler<ChoUnknownProperyEventArgs> propertyResolve = PropertyResolve;
            if (propertyResolve == null) yield break;

            foreach (EventHandler<ChoUnknownProperyEventArgs> x in PropertyResolve.GetInvocationList())
            {
                ChoUnknownProperyEventArgs args = new ChoUnknownProperyEventArgs();
                x(null, args);
                yield return args.HelpTextDict;
            }
        }
    }
}
