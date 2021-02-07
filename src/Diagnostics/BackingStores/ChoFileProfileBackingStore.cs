namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.IO;
    using System.Diagnostics;
    using Cinchoo.Core.IO;

    #endregion NameSpaces

    public class ChoFileProfileBackingStore : IChoProfileBackingStore
    {
        private readonly string _filePath;
        //private StreamWriter _sw = null;
        private ChoTextWriterTraceListener _frxTextWriterTraceListener;
        private readonly object _padLock = new object();

        public ChoFileProfileBackingStore(string filePath)
        {
            ChoGuard.ArgumentNotNullOrEmpty(filePath, "FilePath");
            _filePath = ChoString.ExpandPropertiesEx(filePath);
        }

        #region IChoProfileBackingStore Members

        public void Start(string actionCmds)
        {
            try
            {
                _frxTextWriterTraceListener = new Cinchoo.Core.Diagnostics.ChoTextWriterTraceListener("Cinchoo",
                    String.Format("BASEFILENAME={0};DIRECTORYNAME={1};FILEEXT={2}", Path.GetFileNameWithoutExtension(_filePath),
                    ChoApplication.ApplicationLogDirectory, Path.GetExtension(_filePath)));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        public void Stop(string actionCmds)
        {
            lock (_padLock)
            {
                if (_frxTextWriterTraceListener != null)
                {
                    _frxTextWriterTraceListener.Close();
                    _frxTextWriterTraceListener = null;
                }
            }
        }

        public void Write(string msg, object tag)
        {
            if (_frxTextWriterTraceListener == null)
                Trace.WriteLine(msg);
            else
                _frxTextWriterTraceListener.WriteLine(msg);
        }

        private void RollfileIfExists()
        {
            _frxTextWriterTraceListener.Write(ChoTrace.BACKUP);
        }

        #endregion
    }
}
