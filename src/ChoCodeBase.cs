namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.IO;
	using System.Xml;
	using System.Text;
	using System.Collections;
	using System.Xml.Serialization;

	using Cinchoo.Core.Text;
	using Cinchoo.Core.Collections;
	using Cinchoo.Core.Diagnostics;
	using Cinchoo.Core.Configuration;
	using System.Collections.Generic;
	using Cinchoo.Core.Factory;
	using Cinchoo.Core.IO;

	#endregion NameSpaces

	public class CodeBasePath //: IChoObjectInitializable
	{
		#region Instance Data Members (Public)

		[XmlAttribute("includeSubDirectories")]
		public bool IncludeSubDirectories = false;

		[XmlAttribute("watchChange")]
		public bool WatchChange = false;

		[XmlAttribute("path")]
		public string Path;

		#endregion

		#region Instance Properties (Public)

		private string[] _directories;
		public string[] Directories
		{
			get { return _directories; }
		}

		#endregion Instance Properties (Public)

        private static readonly CodeBasePath _instance;

		#region Constructors

		static CodeBasePath()
		{
            _instance = new CodeBasePath();
            _instance.Initialize();
			//ChoStreamProfile.Clean(ChoReservedDirectoryName.Settings, ChoPath.AddExtension(typeof(CodeBasePath).FullName, ChoReservedFileExt.Err));
			//ChoStreamProfile.Clean(ChoReservedDirectoryName.Settings, ChoPath.AddExtension(typeof(CodeBasePath).FullName, ChoReservedFileExt.Log));
		}

		public CodeBasePath()
		{
			Path = ChoPath.AssemblyBaseDirectory;
			IncludeSubDirectories = true;
		}

		public CodeBasePath(string path, bool includeSubDirectories)
		{
			Path = path;
			IncludeSubDirectories = includeSubDirectories;
		}

		#endregion

		#region Instance Members (Private)

		private string[] GetDirectories()
		{
			return GetDirectories(Path);
		}

		private string[] GetDirectories(string path)
		{
			List<string> paths = new List<string>();
			if (!String.IsNullOrEmpty(path))
			{
				if (Directory.Exists(path))
				{
					paths.Add(path);

					if (IncludeSubDirectories)
					{
						try
						{
							foreach (string subDirectory in Directory.GetDirectories(path))
								paths.AddRange(GetDirectories(subDirectory));
						}
						catch (Exception)
						{
							//ChoStreamProfile.WriteLine(ChoReservedDirectoryName.Settings, ChoPath.AddExtension(GetType().FullName, ChoReservedFileExt.Err), ex.Message);
						}
					}
				}
				//else
				//    ChoStreamProfile.WriteLine(ChoReservedDirectoryName.Settings, ChoPath.AddExtension(GetType().FullName, ChoReservedFileExt.Err), String.Format("{0} directory not exists.", path));
			}

			return paths.ToArray();
		}

		#endregion Instance Members (Private)

		#region Object Overrides

		public override string ToString()
		{
			return String.Format("Path: {1} {0}", IncludeSubDirectories ? "[SubDirs Included]" : String.Empty, Path);
		}

		#endregion Object Overrides

		#region IChoObjectInitializable Members

		public bool Initialize()
		{
			_directories = GetDirectories();
			return false;
		}

		#endregion

        public static CodeBasePath Me
        {
            get { return _instance; }
        }
	}

	[Serializable]
	[XmlRoot("codeBase")]
	[ChoTypeFormatter("CodeBase Settings")]
	[ChoConfigurationSection("cinchoo/codeBase", Defaultable = false)]
	public class ChoCodeBase //: IChoObjectInitializable //, IChoObjectCleanable
	{
		#region Shared Data Members (Private)

		private static FileSystemWatcher _fileSystemWatcher = new FileSystemWatcher();

		#endregion Shared Data Members (Private)

		#region Instance Data Members (Public)

		[XmlAttribute("includeAppPaths")]
		public bool IncludeAppPaths = true;

		[XmlAttribute("loadAssemblyFromFile")]
		public bool LoadAssemblyFromFile = true;

		[XmlElement("codeBasePath", typeof(CodeBasePath))]
		[ChoIgnoreMemberFormatter]
		public CodeBasePath[] CodeBasePaths;

		#endregion

		#region Instance Data Members (Private)

		private List<string> _paths = new List<string>();

		#endregion

        static ChoCodeBase()
        {
            _instance = new ChoCodeBase();
            _instance.Initialize();
        }

        private ChoCodeBase()
        {
        }

		#region Shared Properties (Public)

		public static string[] AppPaths
		{
			get
			{
				return CodeBasePath.Me.Directories;
			}
		}

        private static ChoCodeBase _instance;

		public static ChoCodeBase Me
		{
            get { return _instance /*ChoConfigurationManagementFactory.CreateInstance<ChoCodeBase>() */; }
		}

		#endregion

		#region Instance Properties (Public)

		[ChoMemberFormatter("Paths", Formatter=typeof(ChoArrayToStringFormatter))]
		public string[] Paths
		{
			get { return _paths.ToArray(); }
		}

		#endregion

		#region IObjectInitializable Members

        public void Initialize()
        {
            if (IncludeAppPaths && ChoGuard.IsArgumentNotNullOrEmpty(AppPaths))
                _paths.AddRange(AppPaths);

            if (CodeBasePaths != null && CodeBasePaths.Length > 0)
            {
                foreach (CodeBasePath codeBasePath in CodeBasePaths)
                {
                    _paths.AddRange(codeBasePath.Directories);
                }
            }
        }

		#endregion

		#region IChoObjectCleanable Members

		public void Clean()
		{
			//ChoStreamProfile.Clean(ChoReservedDirectoryName.Settings, ChoType.GetLogFileName(typeof(CodeBasePath), ChoReservedFileExt.Err));
			_paths.Clear();
		}

		#endregion
	}
}
