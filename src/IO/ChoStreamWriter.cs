namespace eSquare.Core.IO
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public abstract class ChoStreamWriter
    {
        //
        // Summary:
        //     Writes the text representation of a Boolean value to the text stream.
        //
        // Parameters:
        //   value:
        //     The Boolean to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void Write(bool value)
        {
        }

        //
        // Summary:
        //     Writes a character to the text stream.
        //
        // Parameters:
        //   value:
        //     The character to write to the text stream.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void Write(char value)
        {
        }

        //
        // Summary:
        //     Writes a character array to the text stream.
        //
        // Parameters:
        //   buffer:
        //     The character array to write to the text stream.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void Write(char[] buffer)
        {
        }

        //
        // Summary:
        //     Writes the text representation of a decimal value followed by a line terminator
        //     to the text stream.
        //
        // Parameters:
        //   value:
        //     The decimal value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void Write(decimal value)
        {
        }

        //
        // Summary:
        //     Writes the text representation of an 8-byte floating-point value to the text
        //     stream.
        //
        // Parameters:
        //   value:
        //     The 8-byte floating-point value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void Write(double value)
        {
        }

        //
        // Summary:
        //     Writes the text representation of a 4-byte floating-point value to the text
        //     stream.
        //
        // Parameters:
        //   value:
        //     The 4-byte floating-point value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void Write(float value)
        {
        }

        //
        // Summary:
        //     Writes the text representation of a 4-byte signed integer to the text stream.
        //
        // Parameters:
        //   value:
        //     The 4-byte signed integer to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void Write(int value)
        {
        }

        //
        // Summary:
        //     Writes the text representation of an 8-byte signed integer to the text stream.
        //
        // Parameters:
        //   value:
        //     The 8-byte signed integer to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void Write(long value)
        {
        }

        //
        // Summary:
        //     Writes the text representation of an object to the text stream by calling
        //     ToString on that object.
        //
        // Parameters:
        //   value:
        //     The object to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void Write(object value)
        {
        }

        //
        // Summary:
        //     Writes a string to the text stream.
        //
        // Parameters:
        //   value:
        //     The string to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void Write(string value)
        {
        }

        //
        // Summary:
        //     Writes the text representation of a 4-byte unsigned integer to the text stream.
        //
        // Parameters:
        //   value:
        //     The 4-byte unsigned integer to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        [CLSCompliant(false)]
        public virtual void Write(uint value)
        {
        }

        //
        // Summary:
        //     Writes the text representation of an 8-byte unsigned integer to the text
        //     stream.
        //
        // Parameters:
        //   value:
        //     The 8-byte unsigned integer to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        [CLSCompliant(false)]
        public virtual void Write(ulong value)
        {
        }

        //
        // Summary:
        //     Writes out a formatted string, using the same semantics as System.String.Format(System.String,System.Object).
        //
        // Parameters:
        //   arg0:
        //     An object to write into the formatted string.
        //
        //   format:
        //     The formatting string.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ArgumentNullException:
        //     format is null.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        //
        //   System.FormatException:
        //     The format specification in format is invalid.-or- The number indicating
        //     an argument to be formatted is less than zero, or larger than or equal to
        //     the number of provided objects to be formatted.
        public virtual void Write(string format, object arg0)
        {
        }

        //
        // Summary:
        //     Writes out a formatted string, using the same semantics as System.String.Format(System.String,System.Object).
        //
        // Parameters:
        //   arg:
        //     The object array to write into the formatted string.
        //
        //   format:
        //     The formatting string.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        //
        //   System.FormatException:
        //     The format specification in format is invalid.-or- The number indicating
        //     an argument to be formatted is less than zero, or larger than or equal to
        //     arg. Length.
        //
        //   System.ArgumentNullException:
        //     format or arg is null.
        public virtual void Write(string format, params object[] arg)
        {
        }

        //
        // Summary:
        //     Writes a subarray of characters to the text stream.
        //
        // Parameters:
        //   count:
        //     The number of characters to write.
        //
        //   buffer:
        //     The character array to write data from.
        //
        //   index:
        //     Starting index in the buffer.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ArgumentOutOfRangeException:
        //     index or count is negative.
        //
        //   System.ArgumentException:
        //     The buffer length minus index is less than count.
        //
        //   System.ArgumentNullException:
        //     The buffer parameter is null.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void Write(char[] buffer, int index, int count)
        {
        }

        //
        // Summary:
        //     Writes out a formatted string, using the same semantics as System.String.Format(System.String,System.Object).
        //
        // Parameters:
        //   arg0:
        //     An object to write into the formatted string.
        //
        //   arg1:
        //     An object to write into the formatted string.
        //
        //   format:
        //     The formatting string.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ArgumentNullException:
        //     format is null.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        //
        //   System.FormatException:
        //     The format specification in format is invalid.-or- The number indicating
        //     an argument to be formatted is less than zero, or larger than or equal to
        //     the number of provided objects to be formatted.
        public virtual void Write(string format, object arg0, object arg1)
        {
        }

        //
        // Summary:
        //     Writes out a formatted string, using the same semantics as System.String.Format(System.String,System.Object).
        //
        // Parameters:
        //   arg2:
        //     An object to write into the formatted string.
        //
        //   arg0:
        //     An object to write into the formatted string.
        //
        //   arg1:
        //     An object to write into the formatted string.
        //
        //   format:
        //     The formatting string.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ArgumentNullException:
        //     format is null.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        //
        //   System.FormatException:
        //     The format specification in format is invalid.-or- The number indicating
        //     an argument to be formatted is less than zero, or larger than or equal to
        //     the number of provided objects to be formatted.
        public virtual void Write(string format, object arg0, object arg1, object arg2)
        {
        }

        //
        // Summary:
        //     Writes a line terminator to the text stream.
        //
        // Returns:
        //     The default line terminator is a carriage return followed by a line feed
        //     ("\r\n"), but this value can be changed using the System.IO.TextWriter.NewLine
        //     property.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void WriteLine()
        {
        }

        //
        // Summary:
        //     Writes the text representation of a Boolean followed by a line terminator
        //     to the text stream.
        //
        // Parameters:
        //   value:
        //     The Boolean to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void WriteLine(bool value)
        {
        }

        //
        // Summary:
        //     Writes a character followed by a line terminator to the text stream.
        //
        // Parameters:
        //   value:
        //     The character to write to the text stream.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void WriteLine(char value)
        {
        }

        //
        // Summary:
        //     Writes an array of characters followed by a line terminator to the text stream.
        //
        // Parameters:
        //   buffer:
        //     The character array from which data is read.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void WriteLine(char[] buffer)
        {
        }

        //
        // Summary:
        //     Writes the text representation of a decimal value followed by a line terminator
        //     to the text stream.
        //
        // Parameters:
        //   value:
        //     The decimal value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void WriteLine(decimal value)
        {
        }

        //
        // Summary:
        //     Writes the text representation of a 8-byte floating-point value followed
        //     by a line terminator to the text stream.
        //
        // Parameters:
        //   value:
        //     The 8-byte floating-point value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void WriteLine(double value)
        {
        }

        //
        // Summary:
        //     Writes the text representation of a 4-byte floating-point value followed
        //     by a line terminator to the text stream.
        //
        // Parameters:
        //   value:
        //     The 4-byte floating-point value to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void WriteLine(float value)
        {
        }

        //
        // Summary:
        //     Writes the text representation of a 4-byte signed integer followed by a line
        //     terminator to the text stream.
        //
        // Parameters:
        //   value:
        //     The 4-byte signed integer to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void WriteLine(int value)
        {
        }

        //
        // Summary:
        //     Writes the text representation of an 8-byte signed integer followed by a
        //     line terminator to the text stream.
        //
        // Parameters:
        //   value:
        //     The 8-byte signed integer to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void WriteLine(long value)
        {
        }

        //
        // Summary:
        //     Writes the text representation of an object by calling ToString on this object,
        //     followed by a line terminator to the text stream.
        //
        // Parameters:
        //   value:
        //     The object to write. If value is null, only the line termination characters
        //     are written.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void WriteLine(object value)
        {
        }

        //
        // Summary:
        //     Writes a string followed by a line terminator to the text stream.
        //
        // Parameters:
        //   value:
        //     The string to write. If value is null, only the line termination characters
        //     are written.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void WriteLine(string value)
        {
        }

        //
        // Summary:
        //     Writes the text representation of a 4-byte unsigned integer followed by a
        //     line terminator to the text stream.
        //
        // Parameters:
        //   value:
        //     The 4-byte unsigned integer to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        [CLSCompliant(false)]
        public virtual void WriteLine(uint value)
        {
        }

        //
        // Summary:
        //     Writes the text representation of an 8-byte unsigned integer followed by
        //     a line terminator to the text stream.
        //
        // Parameters:
        //   value:
        //     The 8-byte unsigned integer to write.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        [CLSCompliant(false)]
        public virtual void WriteLine(ulong value)
        {
        }

        //
        // Summary:
        //     Writes out a formatted string and a new line, using the same semantics as
        //     System.String.Format(System.String,System.Object).
        //
        // Parameters:
        //   arg0:
        //     The object to write into the formatted string.
        //
        //   format:
        //     The formatted string.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ArgumentNullException:
        //     format is null.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        //
        //   System.FormatException:
        //     The format specification in format is invalid.-or- The number indicating
        //     an argument to be formatted is less than zero, or larger than or equal to
        //     the number of provided objects to be formatted.
        public virtual void WriteLine(string format, object arg0)
        {
        }

        //
        // Summary:
        //     Writes out a formatted string and a new line, using the same semantics as
        //     System.String.Format(System.String,System.Object).
        //
        // Parameters:
        //   arg:
        //     The object array to write into format string.
        //
        //   format:
        //     The formatting string.
        //
        // Exceptions:
        //   System.FormatException:
        //     The format specification in format is invalid.-or- The number indicating
        //     an argument to be formatted is less than zero, or larger than or equal to
        //     arg.Length.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        //
        //   System.ArgumentNullException:
        //     A string or object is passed in as null.
        public virtual void WriteLine(string format, params object[] arg)
        {
        }

        //
        // Summary:
        //     Writes a subarray of characters followed by a line terminator to the text
        //     stream.
        //
        // Parameters:
        //   count:
        //     The maximum number of characters to write.
        //
        //   buffer:
        //     The character array from which data is read.
        //
        //   index:
        //     The index into buffer at which to begin reading.
        //
        // Returns:
        //     Characters are read from buffer beginning at index and ending at index +
        //     count.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ArgumentOutOfRangeException:
        //     index or count is negative.
        //
        //   System.ArgumentException:
        //     The buffer length minus index is less than count.
        //
        //   System.ArgumentNullException:
        //     The buffer parameter is null.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        public virtual void WriteLine(char[] buffer, int index, int count)
        {
        }

        //
        // Summary:
        //     Writes out a formatted string and a new line, using the same semantics as
        //     System.String.Format(System.String,System.Object).
        //
        // Parameters:
        //   arg0:
        //     The object to write into the format string.
        //
        //   arg1:
        //     The object to write into the format string.
        //
        //   format:
        //     The formatting string.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ArgumentNullException:
        //     format is null.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        //
        //   System.FormatException:
        //     The format specification in format is invalid.-or- The number indicating
        //     an argument to be formatted is less than zero, or larger than or equal to
        //     the number of provided objects to be formatted.
        public virtual void WriteLine(string format, object arg0, object arg1)
        {
        }

        //
        // Summary:
        //     Writes out a formatted string and a new line, using the same semantics as
        //     System.String.Format(System.String,System.Object).
        //
        // Parameters:
        //   arg2:
        //     The object to write into the format string.
        //
        //   arg0:
        //     The object to write into the format string.
        //
        //   arg1:
        //     The object to write into the format string.
        //
        //   format:
        //     The formatting string.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ArgumentNullException:
        //     format is null.
        //
        //   System.ObjectDisposedException:
        //     The System.IO.TextWriter is closed.
        //
        //   System.FormatException:
        //     The format specification in format is invalid.-or- The number indicating
        //     an argument to be formatted is less than zero, or larger than or equal to
        //     the number of provided objects to be formatted.
        public virtual void WriteLine(string format, object arg0, object arg1, object arg2)
        {
        }
    }

    [Serializable]
    public class ChoFileStreamWriter : ChoStreamWriter
    {
        #region Instance Members (Private)

        private string _path;
        private bool _append;
        private Encoding _encoding;
        private int _bufferSize;

        #endregion Instance Members (Private)

        #region Constructors

        public ChoFileStreamWriter(string path)
            : base(path)
        {
            Close();
        }

        public ChoFileStreamWriter(string path, bool append)
            : base(path, append)
        {
            Close();
        }

        public ChoFileStreamWriter(string path, bool append, Encoding encoding)
            : base(path, append, encoding)
        {
            Close();
        }

        public ChoFileStreamWriter(string path, bool append, Encoding encoding, int bufferSize)
            : base(path, append, encoding, bufferSize)
        {
            Close();
        }

        #endregion Constructors
    }
}
