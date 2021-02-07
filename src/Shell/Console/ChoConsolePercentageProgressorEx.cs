namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Collections.Generic;
    using System.Runtime.Remoting.Contexts;
    
	using Cinchoo.Core.Drawing;
    using System.Runtime.InteropServices;
    using System.Security;
    using Cinchoo.Core.Services;

    #endregion NameSpaces

    public delegate Tuple<int, string> ChoConsolePercentageProgressorStartEx(ChoConsolePercentageProgressorEx sender, int runningPercentage, object state);

    public enum StandardHandle
    {
        Input = -10,
        Output = -11,
        Error = -12
    }

    /// <summary>
    /// Summary description for ConsoleCoordinate.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ConsoleCoordinate
    {
        public readonly short X;
        public readonly short Y;

        public ConsoleCoordinate(short x, short y)
        {
            X = x;
            Y = y;
        }
    }

    public sealed class ChoConsolePercentageProgressorEx : ChoSyncDisposableObject
    {
        #region DllImports

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(int intHandleType);

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleTextAttribute(IntPtr handle, int wAttributes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleCursorPosition(IntPtr handle, ConsoleCoordinate inCoordCharacterAttributes);

        #endregion

        #region Constants
        #endregion Constants

        #region Instance Data Members (Private)

        private string _msg;
        private IChoAbortableAsyncResult _result;
        private Thread _threadToKill = null;
        private ConsoleCoordinate _consoleCoordinate = new ConsoleCoordinate(10, 2);
        private int _stopRequested = 0;
        private int _isStarted = 0;

        private readonly int _padLength = 0;
        private readonly object _padLock = new object();
        private readonly IntPtr _handle = GetStdHandle((int)StandardHandle.Output);
        private readonly ChoConsolePercentageProgressorSettings _consolePercentageProgressorSettings;

        #endregion Instance Data Members (Private)

        #region Instance Data Members (Public)

        public readonly int MinPercentage;
        public readonly int MaxPercentage;

        #endregion Instance Data Members (Public)

        #region Events

        public event EventHandler<ChoExceptionEventArgs> ErrorOccured;

        #endregion Events

        #region Constructors

        static ChoConsolePercentageProgressorEx()
        {
            ChoConsole.Initialize();
        }

        public ChoConsolePercentageProgressorEx() : this(null)
        {
        }

        public ChoConsolePercentageProgressorEx(string msg)
            : this(msg, 0, 100)
        {
        }
        
        public ChoConsolePercentageProgressorEx(string msg, int minPercentage, int maxPercentage)
            : this(msg, minPercentage, maxPercentage, new ChoConsolePercentageProgressorSettings())
        {
        }

        public ChoConsolePercentageProgressorEx(string msg, int minPercentage, int maxPercentage, ChoConsolePercentageProgressorSettings consolePercentageProgressorSettings)
        {
            if (minPercentage >= maxPercentage)
                throw new ArgumentException("MinPercentage should be < MaxPercentage.");

            MinPercentage = minPercentage;
            MaxPercentage = maxPercentage;

            if (consolePercentageProgressorSettings == null)
                _consolePercentageProgressorSettings = new ChoConsolePercentageProgressorSettings();
            else
                _consolePercentageProgressorSettings = ChoObject.Clone(consolePercentageProgressorSettings) as ChoConsolePercentageProgressorSettings;

            _padLength = MinPercentage.ToString().Length;
            if (MaxPercentage.ToString().Length > _padLength)
                _padLength = MaxPercentage.ToString().Length;

            _msg = NormalizeMethod(msg);
        }

        #endregion Constructors

        #region Instance Members (Public)

        #region Start Overloads

        public void Start(ChoConsolePercentageProgressorStartEx consolePercentageProgressorStart)
        {
            Start(consolePercentageProgressorStart, Timeout.Infinite);
        }

        public void Start(ChoConsolePercentageProgressorStartEx consolePercentageProgressorStart, int timeout)
        {
            Start(consolePercentageProgressorStart, null, null, timeout);
        }

        public void Start(ChoConsolePercentageProgressorStartEx consolePercentageProgressorStart, ChoAbortableAsyncCallback callback, object state)
        {
            Start(consolePercentageProgressorStart, callback, state, Timeout.Infinite);
        }

        public void Start(ChoConsolePercentageProgressorStartEx consolePercentageProgressorStart, ChoAbortableAsyncCallback callback, object state, int timeout)
        {
            if (ChoApplication.ApplicationMode != ChoApplicationMode.Console)
                return;
            
            ChoGuard.NotDisposed(this);

            int isStarted = Interlocked.CompareExchange(ref _isStarted, 1, 0);
            if (isStarted == 1)
                return;

            Interlocked.CompareExchange(ref _stopRequested, 0, 1);

            lock (ChoConsole.SyncRoot)
            {
                ChoConsole.Clear();
                ChoConsole.ClearKeys();

                _consoleCoordinate = new ConsoleCoordinate((short)(Console.CursorLeft + _consolePercentageProgressorSettings.ProgressBarMarginX),
                    (short)(Console.CursorTop + _consolePercentageProgressorSettings.ProgressBarMarginY));

                ShowProgress(MinPercentage, _msg);
            }

            Action<ChoConsolePercentageProgressorEx, int> wrappedFunc = delegate
            {
                _threadToKill = Thread.CurrentThread;

                try
                {
                    int percentage = MinPercentage;
                    int retPercentage = MinPercentage;
                    while (retPercentage < MaxPercentage)
                    {
                        if (_stopRequested == 1)
                            break;

                        Tuple<int, string> retValue = consolePercentageProgressorStart(this, percentage, state);
                        retPercentage = retValue.Item1;

                        if (percentage >= retPercentage)
                            throw new ChoConsoleException("Returned percentage '{0}' value <= running percentage '{1}' value. It may leads to infinite loop.".FormatString(retPercentage, percentage));
                        else
                            percentage = retPercentage;

                        ShowProgress(retPercentage, retValue.Item2);
                    }
                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                }
                finally
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.ResetColor();
                }
            };

            _result = ChoAbortableQueuedExecutionService.Global.Enqueue<ChoConsolePercentageProgressorEx, int>(wrappedFunc, this, MinPercentage, callback, state, timeout);
        }

        #endregion Start Overloads

        #region Stop Overloads

        public void Stop()
        {
            if (ChoApplication.ApplicationMode != ChoApplicationMode.Console)
                return;

            Interlocked.Exchange(ref _stopRequested, 1);
        }

        #endregion Stop Overloads

        #region Abort Overloads

        public void Abort()
        {
            if (ChoApplication.ApplicationMode != ChoApplicationMode.Console)
                return;

            if (_threadToKill != null)
            {
                if ((_threadToKill.ThreadState & (ThreadState.Aborted | ThreadState.Stopped)) == 0)
                {
                    _threadToKill.AbortThread();
                }
            }
        }

        #endregion Abort Overloads

        #endregion Instance Members (Public)

        #region Instance Members (Private)

        private void ShowProgress(int percentComplete, string statusMsg)
        {
            string maxDisplayValue = (MaxPercentage - MinPercentage).ToString();
            // Calculations
            int incrementPercent = (MaxPercentage - MinPercentage) / _consolePercentageProgressorSettings.ProgressBarSize;
            int completeLength = Convert.ToInt32((percentComplete - MinPercentage) / incrementPercent);

            // Constructing the full string making up the progressbar
            string statusText = ((MaxPercentage - MinPercentage) - (MaxPercentage - percentComplete)).ToString().PadLeft(maxDisplayValue.Length, ' ') + _consolePercentageProgressorSettings.UnitIndicator + " / " + maxDisplayValue + _consolePercentageProgressorSettings.UnitIndicator;
            int startPosOfStatusTextInProgressBar = (int)((float)_consolePercentageProgressorSettings.ProgressBarSize / 2.0 - (int)(statusText.Length / 2.0));
            statusText = statusText.PadLeft(startPosOfStatusTextInProgressBar + statusText.Length, ' ');
            statusText = statusText.PadRight(_consolePercentageProgressorSettings.ProgressBarSize, ' ');

            // Preparing the completed/incomplete progressbar text string
            string completeText = statusText.Length >= completeLength ? statusText.Substring(0, completeLength) : statusText;
            string incompleteText = statusText.Substring(completeText.Length, statusText.Length - completeText.Length);
            bool bResult = SetConsoleCursorPosition(_handle, _consoleCoordinate);

            //Printing to console and coloring
            SetConsoleTextAttribute(_handle, (int)_consolePercentageProgressorSettings.ProgressBarFontColor | ((int)_consolePercentageProgressorSettings.ProgressBarCompleteColor << 4));
            System.Console.Write(completeText);
            SetConsoleTextAttribute(_handle, (int)_consolePercentageProgressorSettings.ProgressBarFontColor | ((int)_consolePercentageProgressorSettings.ProgressBarIncompleteColor << 4));
            System.Console.Write(incompleteText);

            if (_consolePercentageProgressorSettings.DisplayScale)
            {
                SetConsoleTextAttribute(_handle, (int)_consolePercentageProgressorSettings.ProgressBarScaleForegroundColor | ((int)_consolePercentageProgressorSettings.ProgressBarScaleBackgroundColor << 4));
                bool bResult1 = SetConsoleCursorPosition(_handle, new ConsoleCoordinate(_consoleCoordinate.X, (short)(_consoleCoordinate.Y + 1)));
                System.Console.Write(MinPercentage.ToString() + _consolePercentageProgressorSettings.UnitIndicator);

                bool bResult2 = SetConsoleCursorPosition(_handle, new ConsoleCoordinate((short)(_consoleCoordinate.X + completeText.Length + incompleteText.Length - MaxPercentage.ToString().Length - _consolePercentageProgressorSettings.UnitIndicator.Length),
                    (short)(_consoleCoordinate.Y + 1)));
                Console.Write(MaxPercentage.ToString() + _consolePercentageProgressorSettings.UnitIndicator);
            }

            Write(statusMsg);
        }

        private void SetStatusMsg(string statusMsg)
        {
            if (ChoApplication.ApplicationMode != ChoApplicationMode.Console)
                return;

            if (statusMsg != null && statusMsg.Length > _consolePercentageProgressorSettings.ProgressBarStatusMsgSize)
                statusMsg = statusMsg.Substring(0, _consolePercentageProgressorSettings.ProgressBarStatusMsgSize);

            if (_consolePercentageProgressorSettings.DisplayScale)
                SetConsoleCursorPosition(_handle, new ConsoleCoordinate(_consoleCoordinate.X, (short)(_consoleCoordinate.Y + 3)));
            else
                SetConsoleCursorPosition(_handle, new ConsoleCoordinate(_consoleCoordinate.X, (short)(_consoleCoordinate.Y + 2)));

            // Printing to console and coloring
            SetConsoleTextAttribute(_handle, (int)_consolePercentageProgressorSettings.ProgressBarStatusMsgForegroundColor | ((int)_consolePercentageProgressorSettings.ProgressBarStatusMsgBackgroundColor << 4));
            System.Console.Write(statusMsg.PadRight(_consolePercentageProgressorSettings.ProgressBarStatusMsgSize));
        }

        private string NormalizeMethod(string msg)
        {
            if (msg.IsNullOrEmpty()) msg = "Processing...";
            else if (msg.EndsWith(Environment.NewLine))
                msg = msg.Substring(0, msg.LastIndexOf(Environment.NewLine));
            return msg;
        }

        #endregion Instance Members (Private)

        #region Dispose Methods

        protected override void Dispose(bool finalize)
        {
            if (_result != null)
            {
                try
                {
                    _result.EndInvoke();
                }
                catch (ChoFatalApplicationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    ErrorOccured.Raise(this, new ChoExceptionEventArgs(ex));
                }
                finally
                {
                    _result = null;

                    IChoAsyncResult result = ChoConsole.OutputQueuedExecutionService.Enqueue(() =>
                    {
                        //Reset to old settings
                        Console.ForegroundColor = ChoConsole.DefaultConsoleForegroundColor;
                        Console.BackgroundColor = ChoConsole.DefaultConsoleBackgroundColor;
                        
                        if (_consolePercentageProgressorSettings.DisplayScale)
                            Console.SetCursorPosition(0, _consoleCoordinate.Y + 4);
                        else
                            Console.SetCursorPosition(0, _consoleCoordinate.Y + 3);
                        Console.WriteLine();
                    });

                    result.AsyncWaitHandle.WaitOne();
                }
            }
        }

        #endregion Dispose Methods

        #region Write Overloads

        public void Write()
        {
            Write((string)null);
        }

        public void Write(string msg)
        {
            SetStatusMsg(msg);
        }

        //
        // Summary:
        //     Writes the text representation of the specified Boolean value to the standard
        //     output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void Write(bool value)
        {
            Write(value.ToString());
        }

        //
        // Summary:
        //     Writes the specified Unicode character value to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void Write(char value)
        {
            Write(value.ToString());
        }

        //
        // Summary:
        //     Writes the specified array of Unicode characters to the standard output stream.
        //
        // Parameters:
        //   buffer:
        //     A Unicode character array.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void Write(char[] buffer)
        {
            Write(new string(buffer));
        }

        //
        // Summary:
        //     Writes the text representation of the specified System.Decimal value to the
        //     standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void Write(decimal value)
        {
            Write(value.ToString());
        }

        //
        // Summary:
        //     Writes the text representation of the specified double-precision floating-point
        //     value to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void Write(double value)
        {
            Write(value.ToString());
        }

        //
        // Summary:
        //     Writes the text representation of the specified single-precision floating-point
        //     value to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void Write(float value)
        {
            Write(value.ToString());
        }

        //
        // Summary:
        //     Writes the text representation of the specified 32-bit signed integer value
        //     to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void Write(int value)
        {
            Write(value.ToString());
        }

        //
        // Summary:
        //     Writes the text representation of the specified 64-bit signed integer value
        //     to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void Write(long value)
        {
            Write(value.ToString());
        }

        //
        // Summary:
        //     Writes the text representation of the specified object to the standard output
        //     stream.
        //
        // Parameters:
        //   value:
        //     The value to write, or null.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void Write(object value)
        {
            Write(ChoObject.ToString(value));
        }

        //
        // Summary:
        //     Writes the text representation of the specified 32-bit unsigned integer value
        //     to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        [CLSCompliant(false)]
        public void Write(uint value)
        {
            Write(value.ToString());
        }

        //
        // Summary:
        //     Writes the text representation of the specified 64-bit unsigned integer value
        //     to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        [CLSCompliant(false)]
        public void Write(ulong value)
        {
            Write(value.ToString());
        }

        //
        // Summary:
        //     Writes the text representation of the specified object to the standard output
        //     stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string.
        //
        //   arg0:
        //     An object to write using format.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        //
        //   System.ArgumentNullException:
        //     format is null.
        //
        //   System.FormatException:
        //     The format specification in format is invalid.
        public void Write(string format, object arg0)
        {
            Write(String.Format(format, arg0));
        }

        //
        // Summary:
        //     Writes the text representation of the specified array of objects to the standard
        //     output stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string.
        //
        //   arg:
        //     An array of objects to write using format.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        //
        //   System.ArgumentNullException:
        //     format or arg is null.
        //
        //   System.FormatException:
        //     The format specification in format is invalid.
        public void Write(string format, params object[] arg)
        {
            Write(String.Format(format, arg));
        }

        //
        // Summary:
        //     Writes the specified subarray of Unicode characters to the standard output
        //     stream.
        //
        // Parameters:
        //   buffer:
        //     An array of Unicode characters.
        //
        //   index:
        //     The starting position in buffer.
        //
        //   count:
        //     The number of characters to write.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     buffer is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     index or count is less than zero.
        //
        //   System.ArgumentException:
        //     index plus count specify a position that is not within buffer.
        //
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void Write(char[] buffer, int index, int count)
        {
            Write(new string(buffer, index, count));
        }

        //
        // Summary:
        //     Writes the text representation of the specified objects to the standard output
        //     stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string.
        //
        //   arg0:
        //     The first object to write using format.
        //
        //   arg1:
        //     The second object to write using format.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        //
        //   System.ArgumentNullException:
        //     format is null.
        //
        //   System.FormatException:
        //     The format specification in format is invalid.
        public void Write(string format, object arg0, object arg1)
        {
            Write(String.Format(format, arg0, arg1));
        }

        //
        // Summary:
        //     Writes the text representation of the specified objects to the standard output
        //     stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string.
        //
        //   arg0:
        //     The first object to write using format.
        //
        //   arg1:
        //     The second object to write using format.
        //
        //   arg2:
        //     The third object to write using format.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        //
        //   System.ArgumentNullException:
        //     format is null.
        //
        //   System.FormatException:
        //     The format specification in format is invalid.
        public void Write(string format, object arg0, object arg1, object arg2)
        {
            Write(String.Format(format, arg0, arg1, arg2));
        }

        //
        // Summary:
        //     Writes the text representation of the specified objects and variable-length
        //     parameter list to the standard output stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string.
        //
        //   arg0:
        //     The first object to write using format.
        //
        //   arg1:
        //     The second object to write using format.
        //
        //   arg2:
        //     The third object to write using format.
        //
        //   arg3:
        //     The fourth object to write using format.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        //
        //   System.ArgumentNullException:
        //     format is null.
        //
        //   System.FormatException:
        //     The format specification in format is invalid.
        [SecuritySafeCritical]
        [CLSCompliant(false)]
        public void Write(string format, object arg0, object arg1, object arg2, object arg3)
        {
            Write(String.Format(format, arg0, arg1, arg2, arg3));
        }

        #endregion Write Overloads
    }
}
