namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Cinchoo.Core.Net;
    using System.Net;
    using System.IO;
    using System.Management;
using System.Threading;
using Cinchoo.Core.Services;
using System.Diagnostics;
    using Cinchoo.Core.IO;

    #endregion
    
    public class ChoWinRemoteExecState : IDisposable
    {
        #region Instance Data Members (Private)

        private readonly string _remoteMachine;
        private readonly NetworkCredential _networkCredential;
        private readonly object _padLock = new object();
        private bool _outputLoaded = false;
        private bool _errorLoaded = false;
        private string _output = null;
        private string _error = null;
        private bool _isDisposed = false;
        private uint _processId;
        #endregion

        #region Instance Data Members (Public)

        public readonly string RemoteShareName;
        public readonly string RemoteScriptBaseFilePath;

        public readonly string RemoteScriptFilePath;
        public readonly string RemoteScriptInnerFilePath;
        public readonly string RemoteScriptOutputFilePath;
        public readonly string RemoteScriptErrorFilePath;

        #endregion

        #region Constructors

        internal ChoWinRemoteExecState(string remoteMachine, NetworkCredential networkCredential)
        {
            _remoteMachine = remoteMachine;
            _networkCredential = networkCredential;

            RemoteShareName = @"\\{0}\admin$".FormatString(remoteMachine);
            RemoteScriptBaseFilePath = @"{0}\{1}".FormatString(RemoteShareName, Math.Abs(ChoRandom.NextRandom()));

            RemoteScriptFilePath = @"{0}.bat".FormatString(RemoteScriptBaseFilePath);
            RemoteScriptInnerFilePath = @"{0}_i.bat".FormatString(RemoteScriptBaseFilePath);
            RemoteScriptOutputFilePath = @"{0}.out".FormatString(RemoteScriptBaseFilePath);
            RemoteScriptErrorFilePath = @"{0}.err".FormatString(RemoteScriptBaseFilePath);
        }

        #endregion

        #region Instance Properties (Public)

        public string Output
        {
            get
            {
                if (_outputLoaded)
                    return _output;

                lock (_padLock)
                {
                    if (!_outputLoaded)
                    {
                        using (ChoNetworkShare networkShare = new ChoNetworkShare(RemoteShareName, _networkCredential))
                        {
                            if (File.Exists(RemoteScriptOutputFilePath))
                                _output = File.ReadAllText(RemoteScriptOutputFilePath);
                            else
                                Console.WriteLine("File not found.");
                        }

                        _outputLoaded = true;
                    }
                    return _output;
                }
            }
        }

        public string Error
        {
            get
            {
                if (_errorLoaded)
                    return _error;

                lock (_padLock)
                {
                    if (!_errorLoaded)
                    {
                        using (ChoNetworkShare networkShare = new ChoNetworkShare(RemoteShareName, _networkCredential))
                        {
                            if (File.Exists(RemoteScriptErrorFilePath))
                                _error = File.ReadAllText(RemoteScriptErrorFilePath);
                        }

                        _errorLoaded = true;
                    }
                    return _error;
                }
            }
        }

        [CLSCompliant(false)]
        public uint ProcessId
        {
            get { return _processId; }
            internal set
            {
                _processId = value;
                if (value != 0)
                {
                    while (true)
                    {
                        try
                        {
                            Process.GetProcessById((int)value, _remoteMachine);
                            Thread.Sleep(1000);
                        }
                        catch
                        {
                            break;                    
                        }
                    }
                }
            }
        }

        [CLSCompliant(false)]
        public uint ReturnValue
        {
            get;
            internal set;
        }

        #endregion
        
        #region Instance Members (Public)

        public void Clean()
        {
            using (ChoNetworkShare networkShare = new ChoNetworkShare(RemoteShareName, _networkCredential))
            {
                if (File.Exists(RemoteScriptFilePath))
                    File.Delete(RemoteScriptFilePath);

                if (File.Exists(RemoteScriptInnerFilePath))
                    File.Delete(RemoteScriptInnerFilePath);

                if (File.Exists(RemoteScriptOutputFilePath))
                    File.Delete(RemoteScriptOutputFilePath);

                if (File.Exists(RemoteScriptErrorFilePath))
                    File.Delete(RemoteScriptErrorFilePath);
            }
        }

        #endregion

        #region IDisposable Overrides

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                Clean();
                _isDisposed = true;
            }
        }

        #endregion
    
        #region Destructors

        ~ChoWinRemoteExecState() 
        { 
            Dispose(false); 
        }

        #endregion
    }
}
