namespace Cinchoo.Core.IO
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
using System.IO;

    #endregion NameSpaces

    /// <summary>
    /// Encapsulates a stream writer which does not close the underlying stream.
    /// </summary>
    public class ChoNoCloseStreamReader : StreamReader
    {
        /// <summary>
        /// Creates a new stream writer object.
        /// </summary>
        /// <param name="stream">The underlying stream to write to.</param>
        /// <param name="encoding">The encoding for the stream.</param>
        public ChoNoCloseStreamReader(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
        }

        /// <summary>
        /// Creates a new stream writer object using default encoding.
        /// </summary>
        /// <param name="stream">The underlying stream to write to.</param>
        /// <param name="encoding">The encoding for the stream.</param>
        public ChoNoCloseStreamReader(Stream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// Disposes of the stream writer.
        /// </summary>
        /// <param name="disposing">True to dispose managed objects.</param>
        protected override void Dispose(bool disposeManaged)
        {
            // Dispose the stream writer but pass false to the dispose
            // method to stop it from closing the underlying stream
            base.Dispose(false);
        }
    }
}
