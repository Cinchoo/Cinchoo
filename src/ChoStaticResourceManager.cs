using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.ConstrainedExecution;
using Microsoft.Win32.SafeHandles;

namespace Cinchoo.Core
{
    /// 
    /// Ensures deterministic StreamWriter Flush, and File close at application exit for
    /// static facades. The cleanup is done after all normal finalizers have been called.
    /// 
    public class ChoStaticResourceManager : CriticalFinalizerObject
    {
        List<StreamWriter> writers = new List<StreamWriter>();

        public void AddStream(StreamWriter writer)
        {
            writers.Add(writer);
            FileStream fStream = GetIfFileStream(writer.BaseStream);
            if (fStream != null)
            {
                GC.SuppressFinalize(fStream); // prevent GC on FileStream
                // prevent file close at application exit before want to let it happen
                GC.SuppressFinalize(fStream.SafeFileHandle);
            }
        }

        static FileStream GetIfFileStream(Stream stream)
        {
            if (stream is FileStream)
                return (FileStream)stream;
            else
                return null;
        }

        /// 
        /// Deterministic cleanup of StreamWriters
        /// 1. StreamWriter Close -> FileStream -> Close -> possible Writes
        /// 2. FileHandle Close
        /// 
        ~ChoStaticResourceManager()
        {
            foreach (StreamWriter writer in writers)
            {
                FileStream fstream = GetIfFileStream(writer.BaseStream);
                SafeFileHandle handle = null;
                if (fstream != null)
                {
                    handle = fstream.SafeFileHandle;
                }
                writer.Close(); // Close StreamWriter first
                if (handle != null) // Close file handle now hurray
                    handle.Close();
            }
        }
    }
}