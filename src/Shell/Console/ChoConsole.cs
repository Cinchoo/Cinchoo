namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cinchoo.Core;
    using Cinchoo.Core.Drawing;
    using Cinchoo.Core.Services;
    using System.Security;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using System.IO;

    #endregion NameSpaces

    public static class ChoConsole
    {
        #region Shared Data Members

        public static readonly object SyncRoot = new object();
        public static ConsoleColor DefaultConsoleForegroundColor = ConsoleColor.White;
        public static ConsoleColor DefaultConsoleBackgroundColor;

        internal static readonly ChoQueuedExecutionService OutputQueuedExecutionService = ChoQueuedExecutionService.GetService("ChoConsoleOutput");
        internal static readonly ChoQueuedExecutionService InputQueuedExecutionService = ChoQueuedExecutionService.GetService("ChoConsoleInput");

        private static readonly Func<int> _consoleReadFunc = Console.Read;
        private static readonly Func<string> _consoleReadLineFunc = Console.ReadLine;
        private static readonly ChoFuncWaitFor<bool, ConsoleKeyInfo> _consoleReadKeyFuncWaitFor = new ChoFuncWaitFor<bool, ConsoleKeyInfo>(Console.ReadKey);

        private static readonly Action<string, int?, int?, ConsoleColor, ConsoleColor> _action = (msg, cursorLeft, cursorTop, foregroundColor, backgroundColor) =>
        {
            if (!HasConsoleWindow) return;

            bool resetCursorPosition = cursorLeft.HasValue && cursorTop.HasValue;

            //Save the previous settings
            ConsoleColor prevForegroundColor = Console.ForegroundColor;
            ConsoleColor prevBackgroundColor = Console.BackgroundColor;
            ChoPoint currentLocation = new ChoPoint(Console.CursorLeft, Console.CursorTop);

            //Set the passed settings
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;

            if (resetCursorPosition)
                Console.SetCursorPosition(cursorLeft.Value, cursorTop.Value);

            Console.Write(msg);

            //Reset to old settings
            Console.ForegroundColor = prevForegroundColor;
            Console.BackgroundColor = prevBackgroundColor;

            if (resetCursorPosition)
                Console.SetCursorPosition(currentLocation.X, currentLocation.Y);
        };

        #endregion Shared Data Members

        static ChoConsole()
        {
            ChoFramework.Initialize();
        }

        #region Initialize

        internal static void Initialize()
        {
            DefaultConsoleForegroundColor = ChoConsoleSettings.Me.ForegroundColor;
            DefaultConsoleBackgroundColor = ChoConsoleSettings.Me.BackgroundColor;
        }

        internal static bool HasConsoleWindow
        {
            get { return ChoWindowsManager.ConsoleWindowHandle != IntPtr.Zero; }
        }

        #endregion Initiialize

        #region Pause Overloads

        public static object Clear()
        {
            if (ChoApplication.ApplicationMode != ChoApplicationMode.Console)
                return null;

            AutoResetEvent taskDone = new AutoResetEvent(false);
            int retVal = 0;
            lock (ChoConsole.SyncRoot)
            {
                OutputQueuedExecutionService.Enqueue(() =>
                {
                    try
                    {
                        Console.Clear();
                    }
                    finally
                    {
                        taskDone.Set();
                    }
                });
            }

            taskDone.WaitOne();
            return retVal;
        }

        /// <summary>
        /// Writes the default string value to the standard output stream.
        /// Reads the next character from the standard input stream.
        /// </summary>
        /// <returns>The next character from the input stream, or negative one (-1) if there are currently no more characters to be read.</returns>
        public static int Pause()
        {
            return Pause(null);
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// Reads the next character from the standard input stream.
        /// </summary>
        /// <param name="msg">The string message to be written to the console.</param>
        /// <returns>The next character from the input stream, or negative one (-1) if there are currently no more characters to be read.</returns>
        public static int Pause(string msg)
        {
            if (ChoApplication.ApplicationMode != ChoApplicationMode.Console)
                return 0;

            AutoResetEvent taskDone = new AutoResetEvent(false);
            int retVal = 0;
            lock (ChoConsole.SyncRoot)
            {
                WriteLine(msg.IsNullOrEmpty() ? "Press any key to continue..." : msg);
                InputQueuedExecutionService.Enqueue(() =>
                {
                    try
                    {
                        retVal = Console.Read();
                    }
                    finally
                    {
                        taskDone.Set();
                    }
                });
            }

            taskDone.WaitOne();
            return retVal;
        }

        #endregion Pause Overloads

        #region PauseLine Overloads

        /// <summary>
        /// Writes the default string value, followed by the current line terminator, to the standard output stream.
        /// Reads the next line of characters from the standard input stream.
        /// </summary>
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        public static string PauseLine()
        {
            return PauseLine(null);
        }

        /// <summary>
        /// Writes the default string value, followed by the current line terminator, to the standard output stream.
        /// Reads the next line of characters from the standard input stream.
        /// </summary>
        /// <param name="msg">The string message to be written to the console.</param>
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        public static string PauseLine(string msg)
        {
            if (ChoApplication.ApplicationMode != ChoApplicationMode.Console)
                return String.Empty;

            AutoResetEvent taskDone = new AutoResetEvent(false);
            string retVal = null;
            lock (ChoConsole.SyncRoot)
            {
                WriteLine(msg.IsNullOrEmpty() ? "Press ENTER key to continue..." : msg, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
                InputQueuedExecutionService.Enqueue(() =>
                {
                    try
                    {
                        retVal = Console.ReadLine();
                    }
                    finally
                    {
                        taskDone.Set();
                    }
                });
            }

            taskDone.WaitOne();
            return retVal;
        }

        #endregion PauseLine Overloads

        #region Read Overloads

        /// <summary>
        /// Reads the next character from the standard input stream.
        /// </summary>
        public static int Read()
        {
            return Read(Timeout.Infinite, null, null);
        }

        /// <summary>
        /// Reads the next character from the standard input stream within the specified timeout period.
        /// </summary>
        /// <param name="timeoutInMilliSeconds">A System.TimeSpan that represents the number of milliseconds to wait, or a System.TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
        /// <param name="defaultValue">A default value will be returned from this call when the timeout period elapsed while reading from console.</param>
        /// <param name="errMsg">A error message used to throw ChoTimeoutException when timeout happens while reading line from console and default value is not specified.</param>
        /// <returns>The next line of characters from the input stream, or null if no more lines</returns>
        public static int Read(int timeoutInMilliSeconds)
        {
            return Read(timeoutInMilliSeconds, null, null);
        }

        /// <summary>
        /// Reads the next character from the standard input stream within the specified timeout period.
        /// </summary>
        /// <param name="timeoutInMilliSeconds">A System.TimeSpan that represents the number of milliseconds to wait, or a System.TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
        /// <param name="defaultValue">A default value will be returned from this call when the timeout period elapsed while reading from console.</param>
        /// <returns>The next line of characters from the input stream, or null if no more lines</returns>
        public static int Read(int timeoutInMilliSeconds, int? defaultValue)
        {
            return Read(timeoutInMilliSeconds, defaultValue, null);
        }

        /// <summary>
        /// Reads the next character from the standard input stream within the specified timeout period.
        /// </summary>
        /// <param name="timeoutInMilliSeconds">A System.TimeSpan that represents the number of milliseconds to wait, or a System.TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
        /// <param name="defaultValue">A default value will be returned from this call when the timeout period elapsed while reading from console.</param>
        /// <param name="errMsg">A error message used to throw ChoTimeoutException when timeout happens while reading line from console and default value is not specified.</param>
        /// <returns>The next line of characters from the input stream, or null if no more lines</returns>
        public static int Read(int timeoutInMilliSeconds, int? defaultValue, string errMsg)
        {
            if (ChoApplication.ApplicationMode != ChoApplicationMode.Console)
                return 0;

            AutoResetEvent taskDone = new AutoResetEvent(false);
            int retValue = 0;
            Exception retEx = null;

            lock (ChoConsole.SyncRoot)
            {
                InputQueuedExecutionService.Enqueue(() =>
                    {
                        try
                        {
                            if (timeoutInMilliSeconds == Timeout.Infinite)
                                retValue = Console.Read();
                            else
                            {
                                try
                                {
                                    retValue = _consoleReadFunc.Run(timeoutInMilliSeconds);
                                }
                                catch (TimeoutException ex)
                                {
                                    if (defaultValue == null || !defaultValue.HasValue)
                                        retEx = new ChoConsoleException(errMsg, ex);
                                    else
                                        retValue = defaultValue.Value;
                                }
                            }
                        }
                        finally
                        {
                            taskDone.Set();
                        }
                    }
                );
            }

            taskDone.WaitOne();
            if (retEx != null)
                throw retEx;
            else
                return retValue;
        }

        #endregion Read Overloads

        #region ReadLine Overloads

        /// <summary>
        /// Reads the next line of characters from the standard input stream.
        /// </summary>
        public static string ReadLine()
        {
            return ReadLine(Timeout.Infinite, null, null);
        }

        /// <summary>
        /// Reads the next line of characters from the standard input stream within the specified timeout period.
        /// </summary>
        /// <param name="timeoutInMilliSeconds">A System.TimeSpan that represents the number of milliseconds to wait, or a System.TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
        /// <param name="defaultValue">A default value will be returned from this call when the timeout period elapsed while reading from console.</param>
        /// <param name="errMsg">A error message used to throw ChoTimeoutException when timeout happens while reading line from console and default value is not specified.</param>
        /// <returns>The next line of characters from the input stream, or null if no more lines</returns>
        public static string ReadLine(int timeoutInMilliSeconds)
        {
            return ReadLine(timeoutInMilliSeconds, null, null);
        }

        /// <summary>
        /// Reads the next line of characters from the standard input stream within the specified timeout period.
        /// </summary>
        /// <param name="timeoutInMilliSeconds">A System.TimeSpan that represents the number of milliseconds to wait, or a System.TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
        /// <param name="defaultValue">A default value will be returned from this call when the timeout period elapsed while reading from console.</param>
        /// <returns>The next line of characters from the input stream, or null if no more lines</returns>
        public static string ReadLine(int timeoutInMilliSeconds, string defaultValue)
        {
            return ReadLine(timeoutInMilliSeconds, defaultValue, null);
        }

        /// <summary>
        /// Reads the next line of characters from the standard input stream within the specified timeout period.
        /// </summary>
        /// <param name="timeoutInMilliSeconds">A System.TimeSpan that represents the number of milliseconds to wait, or a System.TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
        /// <param name="defaultValue">A default value will be returned from this call when the timeout period elapsed while reading from console.</param>
        /// <param name="errMsg">A error message used to throw ChoTimeoutException when timeout happens while reading line from console and default value is not specified.</param>
        /// <returns>The next line of characters from the input stream, or null if no more lines</returns>
        public static string ReadLine(int timeoutInMilliSeconds, string defaultValue, string errMsg)
        {
            if (ChoApplication.ApplicationMode != ChoApplicationMode.Console)
                return String.Empty;

            AutoResetEvent taskDone = new AutoResetEvent(false);
            string retValue = null;
            Exception retEx = null;

            lock (ChoConsole.SyncRoot)
            {
                InputQueuedExecutionService.Enqueue(() =>
                    {
                        try
                        {
                            if (timeoutInMilliSeconds == Timeout.Infinite)
                                retValue = Console.ReadLine();
                            else
                            {
                                try
                                {
                                    retValue = _consoleReadLineFunc.Run(timeoutInMilliSeconds);
                                }
                                catch (TimeoutException ex)
                                {
                                    if (defaultValue == null)
                                        retEx = new ChoConsoleException(errMsg, ex);
                                    else
                                        retValue = defaultValue;
                                }
                            }
                        }
                        finally
                        {
                            taskDone.Set();
                        }
                    });
            }

            taskDone.WaitOne();
            if (retEx != null)
                throw retEx;
            else
                return retValue;
        }

        #endregion ReadLine Overloads

        #region ReadKey Overloads

        /// <summary>
        /// Reads the next character from the standard input stream.
        /// </summary>
        /// <returns>
        ///     A System.ConsoleKeyInfo object that describes the System.ConsoleKey constant and Unicode character, if any, that correspond to the pressed console key. 
        ///     The System.ConsoleKeyInfo object also describes, in a bitwise combination of System.ConsoleModifiers values, whether one or more SHIFT, ALT, or 
        ///     CTRL modifier keys was pressed simultaneously with the console key.
        /// </returns>
        public static ConsoleKeyInfo ReadKey()
        {
            return ReadKey(Timeout.Infinite, null, null);
        }

        /// <summary>
        /// Reads the next character from the standard input stream within the specified timeout period.
        /// </summary>
        /// <param name="timeoutInMilliSeconds">A System.TimeSpan that represents the number of milliseconds to wait, or a System.TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
        /// <param name="defaultValue">A default value will be returned from this call when the timeout period elapsed while reading from console.</param>
        /// <param name="errMsg">A error message used to throw ChoTimeoutException when timeout happens while reading line from console and default value is not specified.</param>
        /// <returns>
        ///     A System.ConsoleKeyInfo object that describes the System.ConsoleKey constant and Unicode character, if any, that correspond to the pressed console key. 
        ///     The System.ConsoleKeyInfo object also describes, in a bitwise combination of System.ConsoleModifiers values, whether one or more SHIFT, ALT, or 
        ///     CTRL modifier keys was pressed simultaneously with the console key.
        /// </returns>
        public static ConsoleKeyInfo ReadKey(int timeoutInMilliSeconds)
        {
            return ReadKey(timeoutInMilliSeconds, null, null);
        }

        /// <summary>
        /// Reads the next character from the standard input stream within the specified timeout period.
        /// </summary>
        /// <param name="timeoutInMilliSeconds">A System.TimeSpan that represents the number of milliseconds to wait, or a System.TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
        /// <param name="defaultValue">A default value will be returned from this call when the timeout period elapsed while reading from console.</param>
        /// <returns>
        ///     A System.ConsoleKeyInfo object that describes the System.ConsoleKey constant and Unicode character, if any, that correspond to the pressed console key. 
        ///     The System.ConsoleKeyInfo object also describes, in a bitwise combination of System.ConsoleModifiers values, whether one or more SHIFT, ALT, or 
        ///     CTRL modifier keys was pressed simultaneously with the console key.
        /// </returns>
        public static ConsoleKeyInfo ReadKey(int timeoutInMilliSeconds, ConsoleKeyInfo? defaultValue)
        {
            return ReadKey(timeoutInMilliSeconds, defaultValue, null);
        }

        /// <summary>
        /// Reads the next character from the standard input stream within the specified timeout period.
        /// </summary>
        /// <param name="timeoutInMilliSeconds">A System.TimeSpan that represents the number of milliseconds to wait, or a System.TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
        /// <param name="defaultValue">A default value will be returned from this call when the timeout period elapsed while reading from console.</param>
        /// <param name="errMsg">A error message used to throw ChoTimeoutException when timeout happens while reading line from console and default value is not specified.</param>
        /// <returns>
        ///     A System.ConsoleKeyInfo object that describes the System.ConsoleKey constant and Unicode character, if any, that correspond to the pressed console key. 
        ///     The System.ConsoleKeyInfo object also describes, in a bitwise combination of System.ConsoleModifiers values, whether one or more SHIFT, ALT, or 
        ///     CTRL modifier keys was pressed simultaneously with the console key.
        /// </returns>
        public static ConsoleKeyInfo ReadKey(int timeoutInMilliSeconds, ConsoleKeyInfo? defaultValue, string errMsg)
        {
            ConsoleKeyInfo retValue = default(ConsoleKeyInfo);
            if (ChoApplication.ApplicationMode != ChoApplicationMode.Console)
                return retValue;

            AutoResetEvent taskDone = new AutoResetEvent(false);
            Exception retEx = null;

            lock (ChoConsole.SyncRoot)
            {
                InputQueuedExecutionService.Enqueue(() =>
                    {
                        try
                        {
                            if (timeoutInMilliSeconds == Timeout.Infinite)
                                retValue = Console.ReadKey();
                            else
                            {
                                try
                                {
                                    retValue = _consoleReadKeyFuncWaitFor.Run(true, timeoutInMilliSeconds);
                                }
                                catch (TimeoutException ex)
                                {
                                    if (defaultValue == null || !defaultValue.HasValue)
                                        retEx = new ChoConsoleException(errMsg, ex);
                                    else
                                        retValue = defaultValue.Value;
                                }
                            }
                        }
                        finally
                        {
                            taskDone.Set();
                        }
                    });
            }

            taskDone.WaitOne();
            if (retEx != null)
                throw retEx;
            else
                return retValue;
        }

        #endregion ReadKey Overloads

        #region ReadPassword Overloads

        public static string ReadPassword()
        {
            return ReadPassword('*');
        }

        public static string ReadPassword(char maskChar)
        {
            return ReadPassword(maskChar, Int32.MaxValue);
        }

        public static string ReadPassword(int maxLength)
        {
            return ReadPassword('*', maxLength);
        }

        public static string ReadPassword(char maskChar, int maxLength)
        {
            if (ChoApplication.ApplicationMode != ChoApplicationMode.Console)
                return String.Empty;

            AutoResetEvent taskDone = new AutoResetEvent(false);
            string retValue = null;
            Exception retEx = null;

            lock (ChoConsole.SyncRoot)
            {
                if (HasConsoleWindow)
                {
                    InputQueuedExecutionService.Enqueue(() =>
                        {
                            try
                            {
                                if (maxLength <= 0)
                                    retEx = new ChoConsoleException(String.Format("MaxLength [{0}] value should be >= 0.", maxLength));

                                string password = String.Empty;
                                ConsoleKeyInfo info;
                                do
                                {
                                    info = Console.ReadKey(true);

                                    if (info.Key != ConsoleKey.Backspace)
                                    {
                                        if (!Char.IsControl(info.KeyChar))
                                            password += info.KeyChar;
                                        else if (info.Key.ToString().Length == 1)
                                            password += info.Key;
                                        else
                                            continue;

                                        Console.Write(maskChar);
                                    }
                                    else if (info.Key == ConsoleKey.Backspace)
                                    {
                                        if (!string.IsNullOrEmpty(password))
                                        {
                                            password = password.Substring(0, password.Length - 1);
                                            //rollback the cursor and write a space so it looks backspaced to the user
                                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                                            Console.Write(" ");
                                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                                        }
                                    }

                                    if (password.Length == maxLength)
                                        break;
                                }
                                while (info.Key != ConsoleKey.Enter);

                                ClearKeysInternal(true);

                                retValue = password;
                            }
                            finally
                            {
                                taskDone.Set();
                            }
                        });
                }
            }

            taskDone.WaitOne();
            Console.WriteLine();
            if (retEx != null)
                throw retEx;
            else
                return retValue;
        }

        #endregion ReadPassword Overloads

        #region ClearKeys Overloads

        public static ConsoleKeyInfo[] ClearKeys()
        {
            return ClearKeys(true);
        }

        private static ConsoleKeyInfo[] ClearKeysInternal(bool intercept)
        {
            List<ConsoleKeyInfo> keyInfos = new List<ConsoleKeyInfo>();
            while (Console.KeyAvailable)
                keyInfos.Add(Console.ReadKey(intercept));

            return keyInfos.ToArray();
        }

        public static ConsoleKeyInfo[] ClearKeys(bool intercept)
        {
            if (ChoApplication.ApplicationMode != ChoApplicationMode.Console)
                return null;

            AutoResetEvent taskDone = new AutoResetEvent(false);
            ConsoleKeyInfo[] retValue = null;

            lock (ChoConsole.SyncRoot)
            {
                InputQueuedExecutionService.Enqueue(() =>
                    {
                        try
                        {
                            return ClearKeysInternal(intercept);
                        }
                        finally
                        {
                            taskDone.Set();
                        }
                    });
            }

            taskDone.WaitOne();
            return retValue;
        }

        #endregion ClearKeys Overloads

        #region Write Overloads

        #region Write (string) Overloads

        public static void Write(string msg)
        {
            Write(msg, DefaultConsoleForegroundColor);
        }

        public static void Write(string msg, ConsoleColor foregroundColor)
        {
            Write(msg, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(string msg, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(msg, null, null, foregroundColor, backgroundColor);
        }

        public static void Write(string msg, ChoPoint cursorLocation)
        {
            Write(msg, cursorLocation.X, cursorLocation.Y);
        }

        public static void Write(string msg, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(msg, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void Write(string msg, int cursorLeft, int cursorTop)
        {
            Write(msg, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(string msg, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(msg, (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion Write (string) Overloads

        #region Write (bool) Overloads

        public static void Write(bool value)
        {
            Write(value, DefaultConsoleForegroundColor);
        }

        public static void Write(bool value, ConsoleColor foregroundColor)
        {
            Write(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(bool value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        public static void Write(bool value, ChoPoint cursorLocation)
        {
            Write(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void Write(bool value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void Write(bool value, int cursorLeft, int cursorTop)
        {
            Write(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(bool value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion Write (bool) Overloads

        #region Write (char) Overloads

        public static void Write(char value)
        {
            Write(value, DefaultConsoleForegroundColor);
        }

        public static void Write(char value, ConsoleColor foregroundColor)
        {
            Write(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(char value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        public static void Write(char value, ChoPoint cursorLocation)
        {
            Write(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void Write(char value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void Write(char value, int cursorLeft, int cursorTop)
        {
            Write(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(char value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion Write (char) Overloads

        #region Write (char[]) Overloads

        public static void Write(char[] buffer)
        {
            Write(buffer, DefaultConsoleForegroundColor);
        }

        public static void Write(char[] buffer, ConsoleColor foregroundColor)
        {
            Write(buffer, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(char[] buffer, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(new string(buffer), null, null, foregroundColor, backgroundColor);
        }

        public static void Write(char[] buffer, ChoPoint cursorLocation)
        {
            Write(buffer, cursorLocation.X, cursorLocation.Y);
        }

        public static void Write(char[] buffer, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(buffer, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void Write(char[] buffer, int cursorLeft, int cursorTop)
        {
            Write(buffer, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(char[] buffer, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(new string(buffer), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion Write (char[]) Overloads

        #region Write (decimal) Overloads

        public static void Write(decimal value)
        {
            Write(value, DefaultConsoleForegroundColor);
        }

        public static void Write(decimal value, ConsoleColor foregroundColor)
        {
            Write(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(decimal value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        public static void Write(decimal value, ChoPoint cursorLocation)
        {
            Write(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void Write(decimal value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void Write(decimal value, int cursorLeft, int cursorTop)
        {
            Write(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(decimal value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion Write (decimal) Overloads

        #region Write (double) Overloads

        public static void Write(double value)
        {
            Write(value, DefaultConsoleForegroundColor);
        }

        public static void Write(double value, ConsoleColor foregroundColor)
        {
            Write(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(double value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        public static void Write(double value, ChoPoint cursorLocation)
        {
            Write(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void Write(double value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void Write(double value, int cursorLeft, int cursorTop)
        {
            Write(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(double value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion Write (double) Overloads

        #region Write (float) Overloads

        public static void Write(float value)
        {
            Write(value, DefaultConsoleForegroundColor);
        }

        public static void Write(float value, ConsoleColor foregroundColor)
        {
            Write(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(float value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        public static void Write(float value, ChoPoint cursorLocation)
        {
            Write(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void Write(float value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void Write(float value, int cursorLeft, int cursorTop)
        {
            Write(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(float value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion Write (float) Overloads

        #region Write (int) Overloads

        public static void Write(int value)
        {
            Write(value, DefaultConsoleForegroundColor);
        }

        public static void Write(int value, ConsoleColor foregroundColor)
        {
            Write(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(int value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        public static void Write(int value, ChoPoint cursorLocation)
        {
            Write(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void Write(int value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void Write(int value, int cursorLeft, int cursorTop)
        {
            Write(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(int value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion Write (int) Overloads

        #region Write (long) Overloads

        public static void Write(long value)
        {
            Write(value, DefaultConsoleForegroundColor);
        }

        public static void Write(long value, ConsoleColor foregroundColor)
        {
            Write(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(long value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        public static void Write(long value, ChoPoint cursorLocation)
        {
            Write(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void Write(long value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void Write(long value, int cursorLeft, int cursorTop)
        {
            Write(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(long value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion Write (long) Overloads

        #region Write (object) Overloads

        public static void Write(object value)
        {
            Write(value, DefaultConsoleForegroundColor);
        }

        public static void Write(object value, ConsoleColor foregroundColor)
        {
            Write(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(object value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(ChoObject.ToString(value), null, null, foregroundColor, backgroundColor);
        }

        public static void Write(object value, ChoPoint cursorLocation)
        {
            Write(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void Write(object value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void Write(object value, int cursorLeft, int cursorTop)
        {
            Write(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(object value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(ChoObject.ToString(value), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion Write (object) Overloads

        #region Write (uint) Overloads

        [CLSCompliant(false)]
        public static void Write(uint value)
        {
            Write(value, DefaultConsoleForegroundColor);
        }

        [CLSCompliant(false)]
        public static void Write(uint value, ConsoleColor foregroundColor)
        {
            Write(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        [CLSCompliant(false)]
        public static void Write(uint value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        [CLSCompliant(false)]
        public static void Write(uint value, ChoPoint cursorLocation)
        {
            Write(value, cursorLocation.X, cursorLocation.Y);
        }

        [CLSCompliant(false)]
        public static void Write(uint value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        [CLSCompliant(false)]
        public static void Write(uint value, int cursorLeft, int cursorTop)
        {
            Write(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        [CLSCompliant(false)]
        public static void Write(uint value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion Write (uint) Overloads

        #region Write (ulong) Overloads

        [CLSCompliant(false)]
        public static void Write(ulong value)
        {
            Write(value, DefaultConsoleForegroundColor);
        }

        [CLSCompliant(false)]
        public static void Write(ulong value, ConsoleColor foregroundColor)
        {
            Write(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        [CLSCompliant(false)]
        public static void Write(ulong value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        [CLSCompliant(false)]
        public static void Write(ulong value, ChoPoint cursorLocation)
        {
            Write(value, cursorLocation.X, cursorLocation.Y);
        }

        [CLSCompliant(false)]
        public static void Write(ulong value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        [CLSCompliant(false)]
        public static void Write(ulong value, int cursorLeft, int cursorTop)
        {
            Write(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        [CLSCompliant(false)]
        public static void Write(ulong value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion Write (ulong) Overloads

        #region Write (string format, object arg0) Overloads

        public static void Write(string format, object arg0)
        {
            Write(format, arg0, DefaultConsoleForegroundColor);
        }

        public static void Write(string format, object arg0, ConsoleColor foregroundColor)
        {
            Write(format, arg0, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(string format, object arg0, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(String.Format(format, arg0), null, null, foregroundColor, backgroundColor);
        }

        public static void Write(string format, object arg0, ChoPoint cursorLocation)
        {
            Write(format, arg0, cursorLocation.X, cursorLocation.Y);
        }

        public static void Write(string format, object arg0, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(format, arg0, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void Write(string format, object arg0, int cursorLeft, int cursorTop)
        {
            Write(format, arg0, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(string format, object arg0, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(String.Format(format, arg0), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion Write (string format, object arg0) Overloads

        #region Write (string format, object arg0, object arg1) Overloads

        public static void Write(string format, object arg0, object arg1)
        {
            Write(format, arg0, arg1, DefaultConsoleForegroundColor);
        }

        public static void Write(string format, object arg0, object arg1, ConsoleColor foregroundColor)
        {
            Write(format, arg0, arg1, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(string format, object arg0, object arg1, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(String.Format(format, arg0, arg1), null, null, foregroundColor, backgroundColor);
        }

        public static void Write(string format, object arg0, object arg1, ChoPoint cursorLocation)
        {
            Write(format, arg0, arg1, cursorLocation.X, cursorLocation.Y);
        }

        public static void Write(string format, object arg0, object arg1, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(format, arg0, arg1, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void Write(string format, object arg0, object arg1, int cursorLeft, int cursorTop)
        {
            Write(format, arg0, arg1, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(string format, object arg0, object arg1, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(String.Format(format, arg0, arg1), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion Write (string format, object arg0, object arg1) Overloads

        #region Write (string format, object arg0, object arg1, object arg2) Overloads

        public static void Write(string format, object arg0, object arg1, object arg2)
        {
            Write(format, arg0, arg1, arg2, DefaultConsoleForegroundColor);
        }

        public static void Write(string format, object arg0, object arg1, object arg2, ConsoleColor foregroundColor)
        {
            Write(format, arg0, arg1, arg2, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(string format, object arg0, object arg1, object arg2, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(String.Format(format, arg0, arg1, arg2), null, null, foregroundColor, backgroundColor);
        }

        public static void Write(string format, object arg0, object arg1, object arg2, ChoPoint cursorLocation)
        {
            Write(format, arg0, arg1, arg2, cursorLocation.X, cursorLocation.Y);
        }

        public static void Write(string format, object arg0, object arg1, object arg2, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(format, arg0, arg1, arg2, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void Write(string format, object arg0, object arg1, object arg2, int cursorLeft, int cursorTop)
        {
            Write(format, arg0, arg1, arg2, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(string format, object arg0, object arg1, object arg2, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(String.Format(format, arg0, arg1, arg2), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion Write (string format, object arg0, object arg1, object arg2) Overloads

        #region Write (string format, object arg0, object arg1, object arg2, object arg3) Overloads

        public static void Write(string format, object arg0, object arg1, object arg2, object arg3)
        {
            Write(format, arg0, arg1, arg2, arg3, DefaultConsoleForegroundColor);
        }

        public static void Write(string format, object arg0, object arg1, object arg2, object arg3, ConsoleColor foregroundColor)
        {
            Write(format, arg0, arg1, arg2, arg3, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(string format, object arg0, object arg1, object arg2, object arg3, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(String.Format(format, arg0, arg1, arg2, arg3), null, null, foregroundColor, backgroundColor);
        }

        public static void Write(string format, object arg0, object arg1, object arg2, object arg3, ChoPoint cursorLocation)
        {
            Write(format, arg0, arg1, arg2, arg3, cursorLocation.X, cursorLocation.Y);
        }

        public static void Write(string format, object arg0, object arg1, object arg2, object arg3, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(format, arg0, arg1, arg2, arg3, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void Write(string format, object arg0, object arg1, object arg2, object arg3, int cursorLeft, int cursorTop)
        {
            Write(format, arg0, arg1, arg2, arg3, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void Write(string format, object arg0, object arg1, object arg2, object arg3, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(String.Format(format, arg0, arg1, arg2, arg3), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion Write (string format, object arg0, object arg1, object arg2, object arg3) Overloads

        #endregion Write Overloads

        #region WriteLine Overloads

        #region WriteLine (string) Overloads

        public static void WriteLine()
        {
            WriteLine(String.Empty, DefaultConsoleForegroundColor);
        }

        public static void WriteLine(string msg)
        {
            WriteLine(msg, DefaultConsoleForegroundColor);
        }

        public static void WriteLine(string msg, ConsoleColor foregroundColor)
        {
            WriteLine(msg, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(string msg, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(msg, null, null, foregroundColor, backgroundColor);
        }

        public static void WriteLine(string msg, ChoPoint cursorLocation)
        {
            WriteLine(msg, cursorLocation.X, cursorLocation.Y);
        }

        public static void WriteLine(string msg, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(msg, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void WriteLine(string msg, int cursorLeft, int cursorTop)
        {
            WriteLine(msg, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(string msg, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(msg, (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion WriteLine (string) Overloads

        #region WriteLine (bool) Overloads

        public static void WriteLine(bool value)
        {
            WriteLine(value, DefaultConsoleForegroundColor);
        }

        public static void WriteLine(bool value, ConsoleColor foregroundColor)
        {
            WriteLine(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(bool value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        public static void WriteLine(bool value, ChoPoint cursorLocation)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void WriteLine(bool value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void WriteLine(bool value, int cursorLeft, int cursorTop)
        {
            WriteLine(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(bool value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion WriteLine (bool) Overloads

        #region WriteLine (char) Overloads

        public static void WriteLine(char value)
        {
            WriteLine(value, DefaultConsoleForegroundColor);
        }

        public static void WriteLine(char value, ConsoleColor foregroundColor)
        {
            WriteLine(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(char value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        public static void WriteLine(char value, ChoPoint cursorLocation)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void WriteLine(char value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void WriteLine(char value, int cursorLeft, int cursorTop)
        {
            WriteLine(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(char value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion WriteLine (char) Overloads

        #region WriteLine (char[]) Overloads

        public static void WriteLine(char[] value)
        {
            WriteLine(value, DefaultConsoleForegroundColor);
        }

        public static void WriteLine(char[] value, ConsoleColor foregroundColor)
        {
            WriteLine(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(char[] value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(new string(value), null, null, foregroundColor, backgroundColor);
        }

        public static void WriteLine(char[] value, ChoPoint cursorLocation)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void WriteLine(char[] value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void WriteLine(char[] value, int cursorLeft, int cursorTop)
        {
            WriteLine(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(char[] value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(new string(value), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion WriteLine (char[]) Overloads

        #region WriteLine (decimal) Overloads

        public static void WriteLine(decimal value)
        {
            WriteLine(value, DefaultConsoleForegroundColor);
        }

        public static void WriteLine(decimal value, ConsoleColor foregroundColor)
        {
            WriteLine(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(decimal value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        public static void WriteLine(decimal value, ChoPoint cursorLocation)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void WriteLine(decimal value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void WriteLine(decimal value, int cursorLeft, int cursorTop)
        {
            WriteLine(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(decimal value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion WriteLine (decimal) Overloads

        #region WriteLine (double) Overloads

        public static void WriteLine(double value)
        {
            WriteLine(value, DefaultConsoleForegroundColor);
        }

        public static void WriteLine(double value, ConsoleColor foregroundColor)
        {
            WriteLine(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(double value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        public static void WriteLine(double value, ChoPoint cursorLocation)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void WriteLine(double value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void WriteLine(double value, int cursorLeft, int cursorTop)
        {
            WriteLine(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(double value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion WriteLine (double) Overloads

        #region WriteLine (float) Overloads

        public static void WriteLine(float value)
        {
            WriteLine(value, DefaultConsoleForegroundColor);
        }

        public static void WriteLine(float value, ConsoleColor foregroundColor)
        {
            WriteLine(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(float value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        public static void WriteLine(float value, ChoPoint cursorLocation)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void WriteLine(float value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void WriteLine(float value, int cursorLeft, int cursorTop)
        {
            WriteLine(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(float value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion WriteLine (float) Overloads

        #region WriteLine (int) Overloads

        public static void WriteLine(int value)
        {
            WriteLine(value, DefaultConsoleForegroundColor);
        }

        public static void WriteLine(int value, ConsoleColor foregroundColor)
        {
            WriteLine(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(int value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        public static void WriteLine(int value, ChoPoint cursorLocation)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void WriteLine(int value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void WriteLine(int value, int cursorLeft, int cursorTop)
        {
            WriteLine(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(int value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion WriteLine (int) Overloads

        #region WriteLine (long) Overloads

        public static void WriteLine(long value)
        {
            WriteLine(value, DefaultConsoleForegroundColor);
        }

        public static void WriteLine(long value, ConsoleColor foregroundColor)
        {
            WriteLine(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(long value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        public static void WriteLine(long value, ChoPoint cursorLocation)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void WriteLine(long value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void WriteLine(long value, int cursorLeft, int cursorTop)
        {
            WriteLine(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(long value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion WriteLine (long) Overloads

        #region WriteLine (object) Overloads

        public static void WriteLine(object value)
        {
            WriteLine(value, DefaultConsoleForegroundColor);
        }

        public static void WriteLine(object value, ConsoleColor foregroundColor)
        {
            WriteLine(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(object value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(ChoObject.ToString(value), null, null, foregroundColor, backgroundColor);
        }

        public static void WriteLine(object value, ChoPoint cursorLocation)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y);
        }

        public static void WriteLine(object value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void WriteLine(object value, int cursorLeft, int cursorTop)
        {
            WriteLine(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(object value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(ChoObject.ToString(value), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion WriteLine (object) Overloads

        #region WriteLine (uint) Overloads

        [CLSCompliant(false)]
        public static void WriteLine(uint value)
        {
            WriteLine(value, DefaultConsoleForegroundColor);
        }

        [CLSCompliant(false)]
        public static void WriteLine(uint value, ConsoleColor foregroundColor)
        {
            WriteLine(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        [CLSCompliant(false)]
        public static void WriteLine(uint value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        [CLSCompliant(false)]
        public static void WriteLine(uint value, ChoPoint cursorLocation)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y);
        }

        [CLSCompliant(false)]
        public static void WriteLine(uint value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        [CLSCompliant(false)]
        public static void WriteLine(uint value, int cursorLeft, int cursorTop)
        {
            WriteLine(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        [CLSCompliant(false)]
        public static void WriteLine(uint value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion WriteLine (uint) Overloads

        #region WriteLine (ulong) Overloads

        [CLSCompliant(false)]
        public static void WriteLine(ulong value)
        {
            WriteLine(value, DefaultConsoleForegroundColor);
        }

        [CLSCompliant(false)]
        public static void WriteLine(ulong value, ConsoleColor foregroundColor)
        {
            WriteLine(value, foregroundColor, DefaultConsoleBackgroundColor);
        }

        [CLSCompliant(false)]
        public static void WriteLine(ulong value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), null, null, foregroundColor, backgroundColor);
        }

        [CLSCompliant(false)]
        public static void WriteLine(ulong value, ChoPoint cursorLocation)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y);
        }

        [CLSCompliant(false)]
        public static void WriteLine(ulong value, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        [CLSCompliant(false)]
        public static void WriteLine(ulong value, int cursorLeft, int cursorTop)
        {
            WriteLine(value, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        [CLSCompliant(false)]
        public static void WriteLine(ulong value, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(value.ToString(), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion WriteLine (ulong) Overloads

        #region WriteLine (string format, object arg0) Overloads

        public static void WriteLine(string format, object arg0)
        {
            WriteLine(format, arg0, DefaultConsoleForegroundColor);
        }

        public static void WriteLine(string format, object arg0, ConsoleColor foregroundColor)
        {
            WriteLine(format, arg0, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(string format, object arg0, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(String.Format(format, arg0), null, null, foregroundColor, backgroundColor);
        }

        public static void WriteLine(string format, object arg0, ChoPoint cursorLocation)
        {
            WriteLine(format, arg0, cursorLocation.X, cursorLocation.Y);
        }

        public static void WriteLine(string format, object arg0, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(format, arg0, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void WriteLine(string format, object arg0, int cursorLeft, int cursorTop)
        {
            WriteLine(format, arg0, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(string format, object arg0, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(String.Format(format, arg0), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion WriteLine (string format, object arg0) Overloads

        #region WriteLine (string format, object arg0, object arg1) Overloads

        public static void WriteLine(string format, object arg0, object arg1)
        {
            WriteLine(format, arg0, arg1, DefaultConsoleForegroundColor);
        }

        public static void WriteLine(string format, object arg0, object arg1, ConsoleColor foregroundColor)
        {
            WriteLine(format, arg0, arg1, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(string format, object arg0, object arg1, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(String.Format(format, arg0, arg1), null, null, foregroundColor, backgroundColor);
        }

        public static void WriteLine(string format, object arg0, object arg1, ChoPoint cursorLocation)
        {
            WriteLine(format, arg0, arg1, cursorLocation.X, cursorLocation.Y);
        }

        public static void WriteLine(string format, object arg0, object arg1, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(format, arg0, arg1, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void WriteLine(string format, object arg0, object arg1, int cursorLeft, int cursorTop)
        {
            WriteLine(format, arg0, arg1, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(string format, object arg0, object arg1, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(String.Format(format, arg0, arg1), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion WriteLine (string format, object arg0, object arg1) Overloads

        #region WriteLine (string format, object arg0, object arg1, object arg2) Overloads

        public static void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            WriteLine(format, arg0, arg1, arg2, DefaultConsoleForegroundColor);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2, ConsoleColor foregroundColor)
        {
            WriteLine(format, arg0, arg1, arg2, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(String.Format(format, arg0, arg1, arg2), null, null, foregroundColor, backgroundColor);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2, ChoPoint cursorLocation)
        {
            WriteLine(format, arg0, arg1, arg2, cursorLocation.X, cursorLocation.Y);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(format, arg0, arg1, arg2, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2, int cursorLeft, int cursorTop)
        {
            WriteLine(format, arg0, arg1, arg2, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(String.Format(format, arg0, arg1, arg2), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion WriteLine (string format, object arg0, object arg1, object arg2) Overloads

        #region WriteLine (string format, object arg0, object arg1, object arg2, object arg3) Overloads

        public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3)
        {
            WriteLine(format, arg0, arg1, arg2, arg3, DefaultConsoleForegroundColor);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3, ConsoleColor foregroundColor)
        {
            WriteLine(format, arg0, arg1, arg2, arg3, foregroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(String.Format(format, arg0, arg1, arg2, arg3), null, null, foregroundColor, backgroundColor);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3, ChoPoint cursorLocation)
        {
            WriteLine(format, arg0, arg1, arg2, arg3, cursorLocation.X, cursorLocation.Y);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3, ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(format, arg0, arg1, arg2, arg3, cursorLocation.X, cursorLocation.Y, foregroundColor, backgroundColor);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3, int cursorLeft, int cursorTop)
        {
            WriteLine(format, arg0, arg1, arg2, arg3, cursorLeft, cursorTop, DefaultConsoleForegroundColor, DefaultConsoleBackgroundColor);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3, int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            WriteLine(String.Format(format, arg0, arg1, arg2, arg3), (int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor);
        }

        #endregion WriteLine (string format, object arg0, object arg1, object arg2, object arg3) Overloads

        #endregion WriteLine Overloads

        #region Shared Members (Private)

        private static void WriteLine(string msg, int? cursorLeft, int? cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(String.Format("{0}{1}", msg, Environment.NewLine), cursorLeft, cursorTop, foregroundColor, backgroundColor);
        }

        private static void Write(string msg, int? cursorLeft, int? cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            if (ChoApplication.ApplicationMode != ChoApplicationMode.Console)
                return;

            //lock (ChoConsole.SyncRoot)
            //{
                OutputQueuedExecutionService.Enqueue(_action, msg, cursorLeft, cursorTop, foregroundColor, backgroundColor);
            //}
        }

        #endregion Shared Members (Private)
    }
}
