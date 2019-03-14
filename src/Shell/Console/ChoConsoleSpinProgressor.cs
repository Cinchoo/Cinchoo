namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Collections.Generic;
    
    using Cinchoo.Core.Drawing;
    using System.Security;
    using Cinchoo.Core.Services;

    #endregion NameSpaces

    public delegate bool ChoConsoleProgressorStart(ChoConsoleSpinProgressor sender, object state);

    public sealed class ChoConsoleSpinProgressor : ChoSyncDisposableObject
    {
        #region Instance Data Members (Private)

        private ChoPoint _location;
        private ChoPoint _statusMsgLocation;
        private string _msg;
        private IChoAbortableAsyncResult _result;
        private Thread _threadToKill = null;
        private int _stopRequested = 0;
        private int _isStarted = 0;
        private int _index = 0;

        private readonly ConsoleColor _foregroundColor;
        private readonly ConsoleColor _backgroundColor;
        private readonly char[] _spinChars = new char[] { '/', '-', '\\', '|' };
        private readonly ChoConsolePercentageProgressorSettings _consolePercentageProgressorSettings;

        #endregion Instance Data Members (Private)

        #region Events

        public event EventHandler<ChoExceptionEventArgs> ErrorOccured;

        #endregion Events

        #region Constructors

        static ChoConsoleSpinProgressor()
        {
            ChoConsole.Initialize();
        }

        public ChoConsoleSpinProgressor()
            : this(null)
        {
        }

        public ChoConsoleSpinProgressor(string msg)
            : this(msg, Console.ForegroundColor, Console.BackgroundColor, null)
        {
        }

        public ChoConsoleSpinProgressor(string msg, ConsoleColor foregroundColor)
            : this(msg, foregroundColor, Console.BackgroundColor, null)
        {
        }

        public ChoConsoleSpinProgressor(string msg, ConsoleColor foregroundColor, ConsoleColor backgroundColor, ChoConsolePercentageProgressorSettings consolePercentageProgressorSettings)
        {
            _msg = NormalizeMethod(msg);
            _foregroundColor = foregroundColor;
            _backgroundColor = backgroundColor;
            if (consolePercentageProgressorSettings == null)
                _consolePercentageProgressorSettings = new ChoConsolePercentageProgressorSettings();
            else
                _consolePercentageProgressorSettings = ChoObject.Clone(consolePercentageProgressorSettings) as ChoConsolePercentageProgressorSettings;
        }

        #endregion Constructors

        #region Instance Members (Public)

        #region Start Overloads

        public void Start(ChoConsoleProgressorStart consoleProgressorStart)
        {
            Start(consoleProgressorStart, Timeout.Infinite);
        }

        public void Start(ChoConsoleProgressorStart consoleProgressorStart, int timeout)
        {
            Start(consoleProgressorStart, null, null, timeout);
        }

        public void Start(ChoConsoleProgressorStart consoleProgressorStart, ChoAsyncCallback callback, object state)
        {
            Start(consoleProgressorStart, callback, state, Timeout.Infinite);
        }

        public void Start(ChoConsoleProgressorStart consoleProgressorStart, ChoAsyncCallback callback, object state, int timeout)
        {
            ChoGuard.NotDisposed(this);

            int isStarted = Interlocked.CompareExchange(ref _isStarted, 1, 0);
            if (isStarted == 1)
                return;

            Interlocked.CompareExchange(ref _stopRequested, 0, 1);

            lock (ChoConsole.SyncRoot)
            {
                ChoConsole.Clear();
                ChoConsole.ClearKeys();

                IChoAsyncResult result = ChoConsole.OutputQueuedExecutionService.Enqueue(() =>
                    {
                        _location = WriteNSavePosition(_msg + " ");
                        _statusMsgLocation = new ChoPoint(_consolePercentageProgressorSettings.ProgressBarMarginX, _location.Y + 1);
                        WriteSpinner("/");
                    });
                result.AsyncWaitHandle.WaitOne();
            }

            Action<ChoConsoleSpinProgressor> wrappedFunc = delegate
            {
                _threadToKill = Thread.CurrentThread;

                try
                {
                    while (true)
                    {
                        if (_stopRequested == 1)
                            break;

                        ChoConsole.WriteLine(GetNextSpinChar().ToString(), _location, _foregroundColor, _backgroundColor);
                        bool retValue = consoleProgressorStart(this, state);
                        if (!retValue)
                            break;
                    }
                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                }
            };

            _result = ChoAbortableQueuedExecutionService.Global.Enqueue<ChoConsoleSpinProgressor>(wrappedFunc, this);
        }

        #endregion Start Overloads

        #region Stop Overloads

        public void Stop()
        {
            Interlocked.Exchange(ref _stopRequested, 1);
        }

        #endregion Stop Overloads

        #region Abort Overloads

        public void Abort()
        {
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

        private char GetNextSpinChar()
        {
            _index = _index % 4;
            return _spinChars[_index++];
        }

        private string NormalizeMethod(string msg)
        {
            if (msg.IsNullOrEmpty())
                msg = "Processing...";
            else if (msg.EndsWith(Environment.NewLine))
                msg = msg.Substring(0, msg.LastIndexOf(Environment.NewLine));
            return msg;
        }

        private ChoPoint WriteNSavePosition(string msg)
        {
            //Save the previous settings
            ConsoleColor prevForegroundColor = Console.ForegroundColor;
            ConsoleColor prevBackgroundColor = Console.BackgroundColor;

            //Set the passed settings
            Console.ForegroundColor = _foregroundColor;
            Console.BackgroundColor = _backgroundColor;

            Console.SetCursorPosition(Console.CursorLeft + _consolePercentageProgressorSettings.ProgressBarMarginX,
                Console.CursorTop + _consolePercentageProgressorSettings.ProgressBarMarginY);

            Console.Write(msg);
            ChoPoint currentLocation = new ChoPoint(Console.CursorLeft, Console.CursorTop);

            //Reset to old settings
            Console.ForegroundColor = prevForegroundColor;
            Console.BackgroundColor = prevBackgroundColor;

            return currentLocation;
        }

        private void WriteSpinner(string msg)
        {
            //Save the previous settings
            ConsoleColor prevForegroundColor = Console.ForegroundColor;
            ConsoleColor prevBackgroundColor = Console.BackgroundColor;

            //Set the passed settings
            Console.ForegroundColor = _foregroundColor;
            Console.BackgroundColor = _backgroundColor;

            Console.SetCursorPosition(_location.X, _location.Y);

            Console.Write(msg + Environment.NewLine);

            //Reset to old settings
            Console.ForegroundColor = prevForegroundColor;
            Console.BackgroundColor = prevBackgroundColor;
        }

        private void SetStatusMsg(string statusMsg)
        {
            if (statusMsg != null && statusMsg.Length > _consolePercentageProgressorSettings.ProgressBarStatusMsgSize)
                statusMsg = statusMsg.Substring(0, _consolePercentageProgressorSettings.ProgressBarStatusMsgSize);

            ChoConsole.Write(statusMsg.PadRight(_consolePercentageProgressorSettings.ProgressBarStatusMsgSize), _statusMsgLocation, _consolePercentageProgressorSettings.ProgressBarStatusMsgForegroundColor,
                _consolePercentageProgressorSettings.ProgressBarStatusMsgBackgroundColor);
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
                            
                            Console.SetCursorPosition(0, _statusMsgLocation.Y + 1);
                            Console.WriteLine();
                        });

                    result.AsyncWaitHandle.WaitOne();
                }
            }
            
            //Console.WriteLine();
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