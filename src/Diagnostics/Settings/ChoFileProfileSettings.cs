namespace Cinchoo.Core.Diagnostics
{
	#region NameSpaces

	using System;
	using System.IO;
	using System.Text;
	using System.Threading;
	using System.Collections;
	using System.Diagnostics;
	using System.Configuration;
	using System.Xml.Serialization;
	using System.Collections.Specialized;

	using Cinchoo.Core.Configuration;
	using Cinchoo.Core.IO;

	#endregion NameSpaces

	[Serializable]
	[XmlRoot("fileProfile")]
	[ChoTypeFormatter("File Profile Settings")]
	[ChoConfigurationSection("cinchoo/fileProfile", Defaultable = false)]
	public sealed class ChoFileProfileSettings : IChoObjectInitializable
	{
		#region Instance Data Members (Public)

		[XmlAttribute("turnOn")]
		public bool TurnOn = true;

		[XmlAttribute("logDirectory")]
		public string LogDirectory;

		#endregion Instance Data Members (Public)

		#region Shared Members (Public)

		public string DefaultLogDirectory
		{
			get { return ChoApplication.ApplicationLogDirectory; } // ChoString.Replace(@"C:\Log\{APPLICATION_NAME}", ChoPropertyManagerSettings.DefaultPropertyReplacers.ToValuesArray()); }
		}

		#endregion Shared Members (Public)

		#region Constants

		internal const string CompleteLogDirectory = "Completed";
		internal const string FailedLogDirectory = "Failed";

		#endregion Constants

		#region IChoObjectInitializable Members

		public bool Initialize(bool beforeFieldInit, object state)
		{
			if (beforeFieldInit || String.IsNullOrEmpty(LogDirectory))
				LogDirectory = DefaultLogDirectory;
			else
				LogDirectory = ChoPropertyManager.ExpandProperties(this, LogDirectory);

			if (TurnOn)
			{
				if (!Directory.Exists(LogDirectory))
                    Directory.CreateDirectory(LogDirectory);
			}

			return false;
		}

		#endregion

		#region Shared Properties

		public static ChoFileProfileSettings Me
		{
			get { return ChoConfigurationManagementFactory.CreateInstance<ChoFileProfileSettings>(); }
		}

		public static string GetFullPath(string fileName)
		{
			return GetFullPath(null, fileName);
		}

		public static string GetFullPath(string directory, string fileName)
		{
			ChoGuard.ArgumentNotNull(fileName, "Filename");

			if (Path.IsPathRooted(fileName))
				return fileName;
			else
			{
				if (!Path.HasExtension(fileName)) fileName = Path.ChangeExtension(fileName, ChoReservedFileExt.Log);

				if (String.IsNullOrEmpty(directory))
					directory = ChoFileProfileSettings.Me.LogDirectory;
				else if (!String.IsNullOrEmpty(ChoFileProfileSettings.Me.LogDirectory) && !directory.StartsWith(ChoFileProfileSettings.Me.LogDirectory))
					directory = Path.Combine(ChoFileProfileSettings.Me.LogDirectory, directory);

				if (!String.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

				return ChoPath.GetFullPath(Path.Combine(directory, fileName));
			}
		}

		#endregion
	}
}
