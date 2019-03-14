namespace Cinchoo.Core.Shell
{
	#region NameSpaces

    using System;
    using System.Security;
    using Cinchoo.Core.Drawing;

	#endregion NameSpaces

	public class ChoConsoleSession : ChoDisposableObject
    {
        #region Instance Data Members (Private)

        private readonly int? _cursorLeft;
		private readonly int? _cursorTop;
		private readonly ConsoleColor _foregroundColor;
		private readonly ConsoleColor _backgroundColor;

		#endregion Instance Data Members (Private)

		#region Constructors

        static ChoConsoleSession()
        {
            ChoConsole.Initialize();
        }

        public ChoConsoleSession()
            : this(null, null, ChoConsole.DefaultConsoleForegroundColor, ChoConsole.DefaultConsoleBackgroundColor)
        {
        }

		public ChoConsoleSession(int cursorLeft, int cursorTop)
            : this(cursorLeft, cursorTop, ChoConsole.DefaultConsoleForegroundColor, ChoConsole.DefaultConsoleBackgroundColor)
		{
		}

		public ChoConsoleSession(ChoPoint cursorLocation)
			: this(cursorLocation.X, cursorLocation.Y)
		{
		}

		public ChoConsoleSession(ChoPoint cursorLocation, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
			: this(cursorLocation.X, cursorLocation.Y, Console.ForegroundColor, Console.BackgroundColor)
		{
		}

		public ChoConsoleSession(int cursorLeft, int cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
			: this((int?)cursorLeft, (int?)cursorTop, foregroundColor, backgroundColor)
		{
		}

		public ChoConsoleSession(ConsoleColor foregroundColor)
			: this(foregroundColor, Console.BackgroundColor)
		{
		}

		public ChoConsoleSession(ConsoleColor foregroundColor, ConsoleColor backgroundColor)
			: this(null, null, foregroundColor, backgroundColor)
		{
		}

		private ChoConsoleSession(int? cursorLeft, int? cursorTop, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
		{
			_cursorLeft = cursorLeft;
			_cursorTop = cursorTop;
			_foregroundColor = foregroundColor;
			_backgroundColor = backgroundColor;
		}

		#endregion Constructors

		#region Instance Members (Public)

        #region Clear Overloads

        public void Clear()
        {
            ChoConsole.Clear();
        }

        #endregion Clear Overloads

        #region Pause Overloads

        /// <summary>
		/// Writes the default string value, followed by the current line terminator, to the standard output stream.
		/// Reads the next character from the standard input stream.
		/// </summary>
		/// <returns>The next character from the input stream, or negative one (-1) if there are currently no more characters to be read.</returns>
		public int Pause()
		{
			return Pause(null);
		}

		/// <summary>
		/// Writes the specified string value, followed by the current line terminator, to the standard output stream.
		/// Reads the next character from the standard input stream.
		/// </summary>
		/// <param name="msg">The string message to be written to the console.</param>
		/// <returns>The next character from the input stream, or negative one (-1) if there are currently no more characters to be read.</returns>
		public int Pause(string msg)
		{
            return ChoConsole.Pause(msg);
		}

		#endregion Pause Overloads

		#region PauseLine Overloads

		/// <summary>
		/// Writes the default string value, followed by the current line terminator, to the standard output stream.
		/// Reads the next line of characters from the standard input stream.
		/// </summary>
		/// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
		public string PauseLine()
		{
			return PauseLine(null);
		}

		/// <summary>
		/// Writes the default string value, followed by the current line terminator, to the standard output stream.
		/// Reads the next line of characters from the standard input stream.
		/// </summary>
		/// <param name="msg">The string message to be written to the console.</param>
		/// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
		public string PauseLine(string msg)
		{
            return ChoConsole.PauseLine(msg);
		}

		#endregion PauseLine Overloads

		#region ReadLine Overloads

		/// <summary>
		/// Reads the next line of characters from the standard input stream.
		/// </summary>
		public string ReadLine()
		{
			return ChoConsole.ReadLine();
		}

		/// <summary>
		/// Reads the next line of characters from the standard input stream within the specified timeout period.
		/// </summary>
		/// <param name="timeoutInMilliSeconds">A System.TimeSpan that represents the number of milliseconds to wait, or a System.TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
		/// <param name="defaultValue">A default value will be returned from this call when the timeout period elapsed while reading from console.</param>
		/// <param name="errMsg">A error message used to throw ChoTimeoutException when timeout happens while reading line from console and default value is not specified.</param>
		/// <returns>The next line of characters from the input stream, or null if no more lines</returns>
		public string ReadLine(int timeoutInMilliSeconds)
		{
			return ChoConsole.ReadLine(timeoutInMilliSeconds);
		}

		/// <summary>
		/// Reads the next line of characters from the standard input stream within the specified timeout period.
		/// </summary>
		/// <param name="timeoutInMilliSeconds">A System.TimeSpan that represents the number of milliseconds to wait, or a System.TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
		/// <param name="defaultValue">A default value will be returned from this call when the timeout period elapsed while reading from console.</param>
		/// <returns>The next line of characters from the input stream, or null if no more lines</returns>
		public string ReadLine(int timeoutInMilliSeconds, string defaultValue)
		{
			return ChoConsole.ReadLine(timeoutInMilliSeconds, defaultValue);
		}

		/// <summary>
		/// Reads the next line of characters from the standard input stream within the specified timeout period.
		/// </summary>
		/// <param name="timeoutInMilliSeconds">A System.TimeSpan that represents the number of milliseconds to wait, or a System.TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
		/// <param name="defaultValue">A default value will be returned from this call when the timeout period elapsed while reading from console.</param>
		/// <param name="errMsg">A error message used to throw ChoTimeoutException when timeout happens while reading line from console and default value is not specified.</param>
		/// <returns>The next line of characters from the input stream, or null if no more lines</returns>
		public string ReadLine(int timeoutInMilliSeconds, string defaultValue, string errMsg)
		{
			return ChoConsole.ReadLine(timeoutInMilliSeconds, defaultValue, errMsg);
		}

		#endregion ReadLine Overloads

		#region Read Overloads

		/// <summary>
		/// Reads the next character from the standard input stream.
		/// </summary>
		public int Read()
		{
			return ChoConsole.Read();
		}

		/// <summary>
		/// Reads the next character from the standard input stream within the specified timeout period.
		/// </summary>
		/// <param name="timeoutInMilliSeconds">A System.TimeSpan that represents the number of milliseconds to wait, or a System.TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
		/// <param name="defaultValue">A default value will be returned from this call when the timeout period elapsed while reading from console.</param>
		/// <param name="errMsg">A error message used to throw ChoTimeoutException when timeout happens while reading line from console and default value is not specified.</param>
		/// <returns>The next line of characters from the input stream, or null if no more lines</returns>
		public int Read(int timeoutInMilliSeconds)
		{
			return ChoConsole.Read(timeoutInMilliSeconds);
		}

		/// <summary>
		/// Reads the next character from the standard input stream within the specified timeout period.
		/// </summary>
		/// <param name="timeoutInMilliSeconds">A System.TimeSpan that represents the number of milliseconds to wait, or a System.TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
		/// <param name="defaultValue">A default value will be returned from this call when the timeout period elapsed while reading from console.</param>
		/// <returns>The next line of characters from the input stream, or null if no more lines</returns>
		public int Read(int timeoutInMilliSeconds, int? defaultValue)
		{
			return ChoConsole.Read(timeoutInMilliSeconds, defaultValue);
		}

		/// <summary>
		/// Reads the next character from the standard input stream within the specified timeout period.
		/// </summary>
		/// <param name="timeoutInMilliSeconds">A System.TimeSpan that represents the number of milliseconds to wait, or a System.TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
		/// <param name="defaultValue">A default value will be returned from this call when the timeout period elapsed while reading from console.</param>
		/// <param name="errMsg">A error message used to throw ChoTimeoutException when timeout happens while reading line from console and default value is not specified.</param>
		/// <returns>The next line of characters from the input stream, or null if no more lines</returns>
		public int Read(int timeoutInMilliSeconds, int? defaultValue, string errMsg)
		{
			return ChoConsole.Read(timeoutInMilliSeconds, defaultValue, errMsg);
		}

		#endregion Read Overloads

		#region ReadKey Overloads

		/// <summary>
		/// Reads the next character from the standard input stream.
		/// </summary>
		/// <returns>
		///     A System.ConsoleKeyInfo object that describes the System.ConsoleKey constant and Unicode character, if any, that correspond to the pressed console key. 
		///     The System.ConsoleKeyInfo object also describes, in a bitwise combination of System.ConsoleModifiers values, whether one or more SHIFT, ALT, or 
		///     CTRL modifier keys was pressed simultaneously with the console key.
		/// </returns>
		public ConsoleKeyInfo ReadKey()
		{
			return ChoConsole.ReadKey();
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
		public ConsoleKeyInfo ReadKey(int timeoutInMilliSeconds)
		{
			return ChoConsole.ReadKey(timeoutInMilliSeconds);
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
		public ConsoleKeyInfo ReadKey(int timeoutInMilliSeconds, ConsoleKeyInfo? defaultValue)
		{
			return ChoConsole.ReadKey(timeoutInMilliSeconds, defaultValue);
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
		public ConsoleKeyInfo ReadKey(int timeoutInMilliSeconds, ConsoleKeyInfo? defaultValue, string errMsg)
		{
			return ChoConsole.ReadKey(timeoutInMilliSeconds, defaultValue, errMsg);
		}

		#endregion ReadKey Overloads

		#region ReadPassword Overloads

		public string ReadPassword()
		{
			return ChoConsole.ReadPassword('*');
		}

		public string ReadPassword(int maxLength)
		{
			return ChoConsole.ReadPassword(maxLength);
		}

		public string ReadPassword(char maskChar, int maxLength)
		{
			return ChoConsole.ReadPassword(maskChar, maxLength);
		}

		#endregion ReadPassword Overloads

		#region ClearKeys Overloads

		public static ConsoleKeyInfo[] ClearKeys()
		{
			return ChoConsole.ClearKeys();
		}

		public static ConsoleKeyInfo[] ClearKeys(bool intercept)
		{
			return ChoConsole.ClearKeys(intercept);
		}

		#endregion ClearKeys Overloads

		#region Write Overloads

		public void Write()
		{
			Write((string)null);
		}

		public void Write(string msg)
		{
			if (HasPositionSpecified)
				ChoConsole.Write(msg, _cursorLeft.Value, _cursorTop.Value, _foregroundColor, _backgroundColor);
			else
				ChoConsole.Write(msg, _foregroundColor, _backgroundColor);
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

		#region WriteLine Overloads

		public void WriteLine(string msg)
		{
			Write(String.Format("{0}{1}", msg, Environment.NewLine));
		}

		public void WriteLine()
		{
			Write(Environment.NewLine);
		}
        //
        // Summary:
        //     Writes the text representation of the specified Boolean value, followed by
        //     the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void WriteLine(bool value)
        {
            Write(String.Format("{0}{1}", value, Environment.NewLine));
        }

        //
        // Summary:
        //     Writes the specified Unicode character, followed by the current line terminator,
        //     value to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void WriteLine(char value)
        {
            Write(String.Format("{0}{1}", value, Environment.NewLine));
        }

        //
        // Summary:
        //     Writes the specified array of Unicode characters, followed by the current
        //     line terminator, to the standard output stream.
        //
        // Parameters:
        //   buffer:
        //     A Unicode character array.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void WriteLine(char[] buffer)
        {
            Write(String.Format("{0}{1}", new string(buffer), Environment.NewLine));
        }

        //
        // Summary:
        //     Writes the text representation of the specified System.Decimal value, followed
        //     by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void WriteLine(decimal value)
        {
            Write(String.Format("{0}{1}", value, Environment.NewLine));
        }

        //
        // Summary:
        //     Writes the text representation of the specified double-precision floating-point
        //     value, followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void WriteLine(double value)
        {
            Write(String.Format("{0}{1}", value, Environment.NewLine));
        }

        //
        // Summary:
        //     Writes the text representation of the specified single-precision floating-point
        //     value, followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void WriteLine(float value)
        {
            Write(String.Format("{0}{1}", value, Environment.NewLine));
        }

        //
        // Summary:
        //     Writes the text representation of the specified 32-bit signed integer value,
        //     followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void WriteLine(int value)
        {
            Write(String.Format("{0}{1}", value, Environment.NewLine));
        }

        //
        // Summary:
        //     Writes the text representation of the specified 64-bit signed integer value,
        //     followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void WriteLine(long value)
        {
            Write(String.Format("{0}{1}", value, Environment.NewLine));
        }

        //
        // Summary:
        //     Writes the text representation of the specified object, followed by the current
        //     line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        public void WriteLine(object value)
        {
            Write(String.Format("{0}{1}", value, Environment.NewLine));
        }

        //
        // Summary:
        //     Writes the text representation of the specified 32-bit unsigned integer value,
        //     followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        [CLSCompliant(false)]
        public void WriteLine(uint value)
        {
            Write(String.Format("{0}{1}", value, Environment.NewLine));
        }

        //
        // Summary:
        //     Writes the text representation of the specified 64-bit unsigned integer value,
        //     followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        [CLSCompliant(false)]
        public void WriteLine(ulong value)
        {
            Write(String.Format("{0}{1}", value, Environment.NewLine));
        }

        //
        // Summary:
        //     Writes the text representation of the specified object, followed by the current
        //     line terminator, to the standard output stream using the specified format
        //     information.
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
        public void WriteLine(string format, object arg0)
        {
            Write(String.Format("{0}{1}", String.Format(format, arg0), Environment.NewLine));
        }

        //
        // Summary:
        //     Writes the text representation of the specified array of objects, followed
        //     by the current line terminator, to the standard output stream using the specified
        //     format information.
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
        public void WriteLine(string format, params object[] arg)
        {
            Write(String.Format("{0}{1}", String.Format(format, arg), Environment.NewLine));
        }

        //
        // Summary:
        //     Writes the specified subarray of Unicode characters, followed by the current
        //     line terminator, to the standard output stream.
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
        public void WriteLine(char[] buffer, int index, int count)
        {
            Write(String.Format("{0}{1}", new string(buffer, index, count), Environment.NewLine));
        }

        //
        // Summary:
        //     Writes the text representation of the specified objects, followed by the
        //     current line terminator, to the standard output stream using the specified
        //     format information.
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
        public void WriteLine(string format, object arg0, object arg1)
        {
            Write(String.Format("{0}{1}", String.Format(format, arg0, arg1), Environment.NewLine));
        }

        //
        // Summary:
        //     Writes the text representation of the specified objects, followed by the
        //     current line terminator, to the standard output stream using the specified
        //     format information.
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
        public void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            Write(String.Format("{0}{1}", String.Format(format, arg0, arg1, arg2), Environment.NewLine));
        }

        //
        // Summary:
        //     Writes the text representation of the specified objects and variable-length
        //     parameter list, followed by the current line terminator, to the standard
        //     output stream using the specified format information.
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
        [CLSCompliant(false)]
        [SecuritySafeCritical]
        public void WriteLine(string format, object arg0, object arg1, object arg2, object arg3)
        {
            Write(String.Format("{0}{1}", String.Format(format, arg0, arg1, arg2, arg3), Environment.NewLine));
        }

		#endregion WriteLine Overloads

		#endregion Instance Members (Public)

        #region Instance Properties (Private)

        private bool HasPositionSpecified
		{
			get { return _cursorTop.HasValue && _cursorLeft.HasValue; }
		}

		#endregion Instance Properties (Private)

		#region ChoDisposableObject Overrides

		protected override void Dispose(bool finalize)
		{
		}

		#endregion ChoDisposableObject Overrides
    }
}
