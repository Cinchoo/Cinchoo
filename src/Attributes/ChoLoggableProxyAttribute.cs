namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.Remoting.Proxies;
    using Cinchoo.Core.Diagnostics;
    using System.IO;
    using Cinchoo.Core.IO;

    #endregion NameSpaces

    public abstract class ChoLoggableProxyAttribute : ChoProxyAttribute
    {
        #region Instance Properties (Public)

        //protected readonly object _padLock = new object();

        private string _logDirectory;
        public string LogDirectory
        {
            get { return _logDirectory == null ? ChoReservedDirectoryName.Settings : _logDirectory; }
            set
            {
                if (value != null && value.Trim().Length > 0)
                    _logDirectory = value.Trim();
            }
        }

        private string _logFileName;
        public string LogFileName
        {
            get { return _logFileName; }
            set
            {
                if (value != null && value.Trim().Length > 0)
                    _logFileName = Path.GetFileName(value.Trim());
            }
        }

        private bool _logCondition = true;
        public bool LogCondition
        {
            get { return _logCondition; }
            set { _logCondition = value; }
        }

        public string LogTimeStampFormat
        {
            get;
            set;
        }

        #endregion Instance Properties (Public)
    }
}
