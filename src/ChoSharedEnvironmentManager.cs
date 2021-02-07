namespace Cinchoo.Core
{
	#region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinchoo.Core.IO;
    using System.Diagnostics;
    using System.Linq;
    using Cinchoo.Core.Reflection;
    using Cinchoo.Core.Text.RegularExpressions;
    using System.Net;
    using Cinchoo.Core.Xml;
    using System.Configuration;
    using Cinchoo.Core.Configuration;

	#endregion NameSpaces

	public class ChoEnvironmentDetails
	{
		#region Instance Data Members (Public)

		[XmlAttribute("name")]
		public string Name
        {
            get;
            set;
        }

        [XmlAttribute("comment")]
        public string Comment
        {
            get;
            set;
        }

		[XmlAttribute("freeze")]
		public bool Freeze;

		[XmlAttribute("appFrxFilePath")]
		public string AppFrxFilePath;

		[XmlElement("machine")]
		public string[] Machines;

        [ChoIgnoreMemberFormatter]
        [XmlIgnore]
        internal ChoSharedEnvironmentManager Parent;

        [ChoIgnoreMemberFormatter]
        [XmlIgnore]
        private IEnumerable<ChoWildcard> _wildCards;

		#endregion Instance Data Members (Public)

		#region Instance Properties (Public)

		public bool IsValid
		{
			get { return !String.IsNullOrEmpty(Name); }
		}

		#endregion Instance Properties (Public)

		#region Instance Members (Public)

		public bool ContainsMachine(string machineName)
		{
            if (Machines == null) return false;

            if (_wildCards == null)
            {
                _wildCards = from machine in Machines
                             select new ChoWildcard(machine, RegexOptions.IgnoreCase);
            }

            return _wildCards.FirstOrDefault((wildCard) => wildCard.IsMatch(machineName)) != null;
		}

		#endregion Instance Members (Public)

		#region Object Overrides

		public override int GetHashCode()
		{
			if (String.IsNullOrEmpty(Name))
				return base.GetHashCode();
			else
				return Name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			return obj.GetHashCode() == GetHashCode();
		}

		#endregion Object Overrides
	}

	[XmlRoot("sharedEnvironment")]
	public class ChoSharedEnvironmentManager : IDisposable
	{
		#region Instance Data Members (Public)

		[XmlAttribute("baseAppConfigDirectory")]
		public string BaseAppConfigDirectory;

		[XmlAttribute("defaultEnvironment")]
		public string DefaultEnvironment;

		[XmlElement("environment")]
		public ChoEnvironmentDetails[] EnvironmentDetails;

		#endregion Instance Data Members (Public)

        #region Constructors

        public ChoSharedEnvironmentManager(string sharedEnvironmentConfigFilePath = null)
        {
            Load(sharedEnvironmentConfigFilePath);
        }

        #endregion Constructors

        #region Object Overrides

        public override string ToString()
		{
			return ChoObject.ToString(this);
		}

		#endregion Object Overrides

		#region Instance Members (Internal)
        
        internal static string SharedEnvironmentConfigFilePath
        {
            get
            {
                string sharedEnvironmentConfigFilePath = ChoPath.GetFullPath(ChoAppFrxSettings.Me.SharedEnvironmentConfgiFilePath);
                if (sharedEnvironmentConfigFilePath.IsNullOrWhiteSpace())
                {
                    string defaultSharedEnvironmentConfigFilePath = ChoPath.GetFullPath(ChoReservedFileName.SharedEnvironmentConfigFileName);
                    //if (File.Exists(defaultSharedEnvironmentConfigFilePath))
                        sharedEnvironmentConfigFilePath = defaultSharedEnvironmentConfigFilePath;
                }

                return sharedEnvironmentConfigFilePath;
            }
        }

        internal static string AppFrxFilePath
        {
            get
            {
                string appFrxFilePath = ChoPath.GetFullPath(ChoAppFrxSettings.Me.AppFrxFilePath);
                if (appFrxFilePath.IsNullOrWhiteSpace())
                {
                    string defaultAppFrxFilePath = ChoPath.GetFullPath(ChoReservedFileName.CoreFrxConfigFileName);
                    appFrxFilePath = defaultAppFrxFilePath;
                }

                return appFrxFilePath;
            }
        }

		internal ChoEnvironmentDetails GetEnvironmentDetails()
		{
            string hostName = GetHostName();
            ChoEnvironmentDetails environmentDetails = null;

            environmentDetails = GetEnvironmentDetails(hostName);
            if (environmentDetails != null)
                return environmentDetails;
            else
            {
                return (from ipAddr in GetIPAddresses()
                        let env = GetEnvironmentDetails(ipAddr)
                        where env != null
                        select env).FirstOrDefault();
            }
        }

		internal ChoEnvironmentDetails GetEnvironmentDetails(string machineName)
		{
			if (EnvironmentDetails != null)
			{
				foreach (ChoEnvironmentDetails environmentDetail in EnvironmentDetails)
				{
					if (environmentDetail == null) continue;
					if (environmentDetail.ContainsMachine(machineName))
						return environmentDetail;
				}
			}

			return null;
		}

		#endregion Instance Members (Private)

		#region Instance Members (Public)

        public string GetEnvironment()
		{
            string hostName = GetHostName();
            string environment = null;

			environment = GetEnvironment(hostName);
            if (!environment.IsNullOrWhiteSpace())
                return environment;
            else
            {
                return (from ipAddr in GetIPAddresses()
                       let env = GetEnvironment(ipAddr) 
                       where !env.IsNullOrWhiteSpace()
                       select env).FirstOrDefault();
            }
		}

		public string GetEnvironment(string machineName)
		{
			ChoEnvironmentDetails environmentDetails = GetEnvironmentDetails(machineName);

			return environmentDetails != null ? environmentDetails.Name : DefaultEnvironment;
		}

		public ChoEnvironmentDetails GetEnvironmentDetailsByEnvironment()
		{
            return GetEnvironmentDetailsByEnvironment(GetEnvironment());
		}

		public ChoEnvironmentDetails GetEnvironmentDetailsByEnvironment(string environment)
		{
			if (EnvironmentDetails != null)
			{
				foreach (ChoEnvironmentDetails environmentDetail in EnvironmentDetails)
				{
					if (environmentDetail == null) continue;
					if (environmentDetail.Name == environment)
						return environmentDetail;
				}
			}

			return null;
		}

		#endregion Instance Properties (Public)

		#region Shared Properties (Public)

        private static string GetHostName()
        {
            string hostName = null;

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
            if (hostName.IsNullOrWhiteSpace())
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

        private static IEnumerable<string> GetIPAddresses()
        {
            return from ipAddress in Dns.GetHostAddresses(GetHostName()) select ipAddress.ToString();
        }

        private void Load(string sharedEnvironmentConfigFilePath)
        {
            if (sharedEnvironmentConfigFilePath.IsNullOrWhiteSpace())
                sharedEnvironmentConfigFilePath = SharedEnvironmentConfigFilePath;

            BaseAppConfigDirectory = null;
            DefaultEnvironment = null;
            EnvironmentDetails = null;
            string backupSharedEnvironmentConfigFilePath = null;

            XmlDocument doc = new XmlDocument();
            if (!String.IsNullOrEmpty(sharedEnvironmentConfigFilePath)
                && File.Exists(sharedEnvironmentConfigFilePath))
            {
                ChoApplication.Trace(true, "Using shared environment config file: {0}".FormatString(sharedEnvironmentConfigFilePath));
                //backupSharedEnvironmentConfigFilePath = "{0}.{1}".FormatString(sharedEnvironmentConfigFilePath, ChoReservedFileExt.Cho);

                try
                {
                    //if (File.Exists(backupSharedEnvironmentConfigFilePath))
                    //    File.SetAttributes(backupSharedEnvironmentConfigFilePath, FileAttributes.Archive);
                    using (ChoXmlDocument xmlDoc = new ChoXmlDocument(sharedEnvironmentConfigFilePath))
                    {
                        doc = xmlDoc.XmlDocument;
                    }
                    //doc.Load(sharedEnvironmentConfigFilePath);
                    //doc.Save(backupSharedEnvironmentConfigFilePath);
                    //if (File.Exists(backupSharedEnvironmentConfigFilePath))
                    //    File.SetAttributes(backupSharedEnvironmentConfigFilePath, FileAttributes.Hidden);
                }
                catch (Exception ex)
                {
                    ChoApplication.Trace(true, "Error loading shared environment config file: {0}.".FormatString(sharedEnvironmentConfigFilePath));
                    ChoApplication.Trace(true, ex.ToString());

                    //doc = LoadBackupSharedEnvironmentConfigFile(backupSharedEnvironmentConfigFilePath);
                }
            }
            else if (ChoApplication.GetSharedEnvironmentConfigXml != null)
            {
                string xml = ChoApplication.GetSharedEnvironmentConfigXml();
                backupSharedEnvironmentConfigFilePath = ChoPath.ChangeExtension(ChoReservedFileName.SharedEnvironmentConfigFileName, ChoReservedFileExt.Cho);

                if (!xml.IsNullOrWhiteSpace())
                {
                    ChoApplication.Trace(true, "Using shared environment xml:");
                    ChoApplication.Trace(true, xml);

                    try
                    {
                        doc.LoadXml(xml);
                        doc.Save(backupSharedEnvironmentConfigFilePath);
                    }
                    catch (Exception ex)
                    {
                        ChoApplication.Trace(true, "Error loading shared environment config xml.");
                        ChoApplication.Trace(true, ex.ToString());

                        doc = LoadBackupSharedEnvironmentConfigFile(backupSharedEnvironmentConfigFilePath);
                    }
                }
            }
            LoadXml(doc);
        }

        private static XmlDocument LoadBackupSharedEnvironmentConfigFile(string backupSharedEnvironmentConfigFilePath)
        {
            XmlDocument doc = new XmlDocument();

            if (!backupSharedEnvironmentConfigFilePath.IsNullOrEmpty()
                && File.Exists(backupSharedEnvironmentConfigFilePath))
            {
                ChoApplication.Trace(true, "Loading backup shared environment config file: {0}".FormatString(backupSharedEnvironmentConfigFilePath));
                doc.Load(backupSharedEnvironmentConfigFilePath);
            }
            else
                ChoApplication.Trace(true, "No backup shared environment config file found.");

            return doc;
        }

        private void LoadXml(XmlDocument doc)
        {
            if (doc == null)
                return;

            XmlNode rootNode = doc.SelectSingleNode("//sharedEnvironment");
            if (rootNode != null)
            {
                if (rootNode.Attributes["baseAppSharedConfigDirectory"] != null)
                    BaseAppConfigDirectory = ChoPath.GetFullPath(ChoString.ExpandProperties(rootNode.Attributes["baseAppSharedConfigDirectory"].Value, ChoEnvironmentVariablePropertyReplacer.Instance));
                if (BaseAppConfigDirectory.IsNullOrWhiteSpace())
                    BaseAppConfigDirectory = ChoPath.AssemblyBaseDirectory;
                    
                if (rootNode.Attributes["defaultEnvironment"] != null)
                    DefaultEnvironment = rootNode.Attributes["defaultEnvironment"].Value;

                XmlNodeList envNodes = rootNode.SelectNodes("environment");
                if (envNodes != null)
                {
                    List<ChoEnvironmentDetails> environmentDetailList = new List<ChoEnvironmentDetails>();
                    foreach (XmlNode envNode in envNodes)
                    {
                        ChoEnvironmentDetails environmentDetails = new ChoEnvironmentDetails();

                        if (envNode.Attributes["name"] != null)
                        {
                            environmentDetails.Name = envNode.Attributes["name"].Value;
                            if (envNode.Attributes["comment"] != null)
                                environmentDetails.Comment = envNode.Attributes["comment"].Value;
                            if (environmentDetails.Comment.IsNullOrWhiteSpace())
                            {
                                environmentDetails.Comment = "{0} Environment".FormatString(environmentDetails.Name);
                            }

                            if (!environmentDetails.Name.IsNullOrWhiteSpace())
                            {
                                if (envNode.Attributes["freeze"] != null)
                                    Boolean.TryParse(envNode.Attributes["freeze"].Value, out environmentDetails.Freeze);

                                if (envNode.Attributes["appFrxFilePath"] != null)
                                {
                                    environmentDetails.AppFrxFilePath = envNode.Attributes["appFrxFilePath"].Value;
                                    if (!environmentDetails.AppFrxFilePath.IsNullOrWhiteSpace())
                                    {
                                        environmentDetails.AppFrxFilePath = ChoString.ExpandProperties(environmentDetails.AppFrxFilePath, ChoEnvironmentVariablePropertyReplacer.Instance);
                                        if (!Path.IsPathRooted(environmentDetails.AppFrxFilePath))
                                            environmentDetails.AppFrxFilePath = Path.Combine(BaseAppConfigDirectory, environmentDetails.AppFrxFilePath);
                                     
                                        if (ChoPath.IsDirectory(environmentDetails.AppFrxFilePath)
                                            || !Path.HasExtension((environmentDetails.AppFrxFilePath)))
                                            environmentDetails.AppFrxFilePath = Path.Combine(environmentDetails.AppFrxFilePath, ChoReservedFileName.CoreFrxConfigFileName);
                                    }
                                    else
                                        environmentDetails.AppFrxFilePath = Path.Combine(BaseAppConfigDirectory, environmentDetails.Name, ChoReservedFileName.CoreFrxConfigFileName);
                                }
                                else
                                {
                                    environmentDetails.AppFrxFilePath = Path.Combine(BaseAppConfigDirectory, environmentDetails.Name, ChoReservedFileName.CoreFrxConfigFileName);
                                }

                                XmlNodeList machineNodes = envNode.SelectNodes("machine");
                                if (machineNodes != null)
                                {
                                    List<string> machines = new List<string>();
                                    foreach (XmlNode machineNode in machineNodes)
                                    {
                                        machines.Add(machineNode.InnerText);
                                    }
                                    environmentDetails.Machines = machines.ToArray();
                                }

                                environmentDetailList.Add(environmentDetails);
                            }
                        }
                    }

                    EnvironmentDetails = environmentDetailList.ToArray();

                    foreach (ChoEnvironmentDetails environmentDetail in EnvironmentDetails)
                        environmentDetail.Parent = this;
                }
            }
        }

		#endregion Shared Properties (Public)

        public void Dispose()
        {
        }
    }
}
