namespace Cinchoo.Core.Net
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.InteropServices;
    using System.Net;
    using Cinchoo.Core.Win32;
    using Cinchoo.Core.Diagnostics;

    #endregion

    public class ChoNetworkShare : IDisposable
    {
        #region Instance Data Members (Private)

        private readonly string _networkShareName;
        private bool _isDisposed = false;

        #endregion

        #region Constructors

        public ChoNetworkShare(string networkShareName, NetworkCredential credentials)
        {
            ChoGuard.ArgumentNotNullOrEmpty(networkShareName, "NetworkShareName");
            ChoGuard.ArgumentNotNull(credentials, "NetworkCredentials");

            _networkShareName = networkShareName;

            var netResource = new NetResource() { Scope = ResourceScope.Context, ResourceType = ResourceType.Disk, DisplayType = ResourceDisplaytype.Share, RemoteName = _networkShareName };
            var result = ChoMpr.WNetAddConnection2(netResource, credentials.Password, @"{1}\{0}".FormatString(credentials.UserName, credentials.Domain), ChoMpr.CONNECT_UPDATE_PROFILE);
            if (result != 0)
                throw new ChoWin32Exception(result, "Error connecting to '{0}' remote share".FormatString(_networkShareName));
        }

        #endregion

        #region Properties

        public bool TraceErrors
        {
            get;
            set;
        }

        #endregion

        #region Destructors

        ~ChoNetworkShare() 
        { 
            Dispose(false); 
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
            if (_isDisposed) return;

            _isDisposed = true;
            var result = ChoMpr.WNetCancelConnection2(_networkShareName, ChoMpr.CONNECT_UPDATE_PROFILE, true);
            if (result != 0 && TraceErrors)
                ChoTrace.Error(new ChoWin32Exception(result, "Error disconnecting to '{0}' remote share".FormatString(_networkShareName)));
        }

        #endregion
    }
}
