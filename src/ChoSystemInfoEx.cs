namespace eSquare.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using eSquare.Core.Diagnostics;

    #endregion

    static class ChoSystemInfo
    {
        #region Shared Data Members (Private)

        public readonly static string NullText = "(null)";
        public readonly static string NotAvailableText = "NOT AVAILABLE";
        public readonly static string HostName;
        public readonly static string ApplicationName;
        public readonly static DateTime ProcessStartTime = DateTime.Now;
        public readonly static int CurrentThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        public readonly static string EntryAssemblyLocation = System.Reflection.Assembly.GetEntryAssembly().Location;
        public readonly static string ConfigurationFileLocation = System.AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        public readonly static string ApplicationBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        #endregion

        #region Constructors

        static ChoSystemInfo()
        {
            HostName = GetHostName();
            ApplicationName = GetApplicationName();
        }

        #endregion

        #region Shared Members (Private)

        private static string GetApplicationName()
        {
            string applicatioName = NotAvailableText;
            try
            {
                applicatioName = AppDomain.CurrentDomain.FriendlyName;
            }
            catch (System.Security.SecurityException ex)
            {
                // This security exception will occur if the caller does not have 
                // some undefined set of SecurityPermission flags.
                ChoTrace.Debug("SystemInfo: Security exception while trying to get current domain friendly name. Error Ignored.", ex);

                try
                {
                    applicatioName = System.IO.Path.GetFileName(EntryAssemblyLocation);
                }
                catch (System.Security.SecurityException)
                {
                }
            }

            return applicatioName;
        }

        private static string GetHostName()
        {
            string hostName = NotAvailableText;

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

        #endregion

        #region Shared Members (Public)

        #endregion
    }
}
