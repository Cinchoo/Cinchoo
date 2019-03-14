namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Security;
    using System.Threading;
    using System.Diagnostics;
    using System.Globalization;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using System.Security.Permissions;

    using Cinchoo.Core.Diagnostics;
    using System.IO;

    #endregion NameSpaces

    public static class ChoSystemInfo
    {
        #region Shared Data Members (Public)

        public readonly static string MachineName;
        public readonly static string HostName;

        #endregion Shared Data Members (Public)

        #region Constructors

        static ChoSystemInfo()
        {
            HostName = GetHostName();

            #region Get MachineName

            try
            {
                MachineName = Environment.MachineName;
            }
            catch (Exception e)
            {
                MachineName = String.Format("Error retrieving MachineName. {0}.", e.Message);
            }

            #endregion Get MachineName
        }

        #endregion Constructors

        #region Shared Members (Private)

        private static string GetHostName()
        {
            string hostName = ChoNull.NullString; ;

            // Get the DNS host name of the current machine
            try
            {
                // Lookup the host name
                hostName = System.Net.Dns.GetHostName();
            }
            catch (System.Net.Sockets.SocketException)
            {
            }
            catch (System.Security.SecurityException)
            {
                // We may get a security exception looking up the hostname
                // You must have Unrestricted DnsPermission to access resource
            }

            // Get the NETBIOS machine name of the current machine
            if (hostName == null || hostName.Length == 0)
            {
                try
                {
                    hostName = Environment.MachineName;
                }
                catch (InvalidOperationException)
                {
                }
                catch (System.Security.SecurityException)
                {
                    // We may get a security exception looking up the machine name
                    // You must have Unrestricted EnvironmentPermission to access resource
                }
            }

            return hostName;
        }

        #endregion Shared Members (Private)
    }
}
