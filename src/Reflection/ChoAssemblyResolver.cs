namespace Cinchoo.Core.Reflection
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Diagnostics;

    #endregion NameSpaces

    [ChoStreamProfile("Missing Assemblies", NameFromTypeFullName = typeof(ChoMissingAssemblies), StartActions = "Truncate")]
    [ChoObjectFactory(ChoObjectConstructionType.Singleton)]
    public class ChoMissingAssemblies : ChoContextProfiler
    {
        #region Shared Members (Public)

        public static ChoMissingAssemblies Me
        {
            get { return ChoObjectManagementFactory.CreateInstance<ChoMissingAssemblies>(); }
        }

        #endregion Shared Members (Public)
    }

	[ChoStreamProfile("Loaded Assemblies", NameFromTypeFullName = typeof(ChoLoadedAssemblies), StartActions = "Truncate")]
    [ChoObjectFactory(ChoObjectConstructionType.Singleton)]
    public class ChoLoadedAssemblies : ChoContextProfiler
    {
        #region Instance Members (Public)

        public void Add(Assembly assembly)
        {
            if (ChoTrace.ChoSwitch.TraceVerbose)
            {
                if (assembly.IsDynamic)
                    Trace.WriteLine(String.Format("Assembly: {0}, CodeBase: {1}, GAC: {2}, RuntimeVersion: {3}", assembly.FullName, "[Dynamic]", assembly.GlobalAssemblyCache, assembly.ImageRuntimeVersion));
                else
                    Trace.WriteLine(String.Format("Assembly: {0}, CodeBase: {1}, GAC: {2}, RuntimeVersion: {3}", assembly.FullName, assembly.CodeBase, assembly.GlobalAssemblyCache, assembly.ImageRuntimeVersion));
            }
			//ChoLoadedAssemblies.Me.AppendLine(String.Format("Assembly: {0}, CodeBase: {1}, GAC: {2}, RuntimeVersion: {3}", assembly.FullName, assembly.CodeBase, assembly.GlobalAssemblyCache, assembly.ImageRuntimeVersion));
		}

        #endregion Instance Members (Public)

        #region Shared Members (Public)

        public static ChoLoadedAssemblies Me
        {
            get { return ChoObjectManagementFactory.CreateInstance<ChoLoadedAssemblies>(); }
        }

        #endregion Shared Members (Public)
    }

    #region ChoAssemblyManager Class

    public static class ChoAssemblyManager
    {
        #region Shared Data Members (Private)

        /// <summary>
        /// Holds the loaded assemblies.
        /// </summary>
        private static ChoDictionary<string, Assembly> _assemblyCache = ChoDictionary<string, Assembly>.Synchronized(new ChoDictionary<string, Assembly>());

        /// <summary>
        /// Holds the missing assembly cache.
        /// </summary>
        private static List<string> _missingAssemblyCache = new List<string>();

        #endregion

        #region Constructors

        static ChoAssemblyManager()
        {
            Clear();
        }

        #endregion

        #region Shared Members (Public)

        public static void Clear()
        {
            lock (_assemblyCache.SyncRoot)
            {
                _assemblyCache.Clear();
                _missingAssemblyCache.Clear();

				//REVISIT
				//ChoMissingAssemblies.Me.Initialize();
				//ChoLoadedAssemblies.Me.Initialize();
            }
        }

        public static bool ContainsAssembly(string assemblyName)
        {
            ChoGuard.ArgumentNotNullOrEmpty(assemblyName, "AssemblyName");

            return _assemblyCache.ContainsKey(assemblyName);
        }

        public static bool ContainsAssembly(Assembly assembly)
        {
            ChoGuard.ArgumentNotNull(assembly, "Assembly");

            return _assemblyCache.ContainsKey(assembly.FullName);
        }

        public static void AddAssemblyToCache(Assembly assembly)
        {
            ChoGuard.ArgumentNotNull(assembly, "Assembly");

            lock (_assemblyCache.SyncRoot)
            {
                if (ContainsAssembly(assembly)) return;

                _assemblyCache.Add(assembly.FullName, assembly);
                //ChoLoadedAssemblies.Me.Add(assembly);
            }
        }

        public static Assembly GetAssemblyFromCache(string assemblyFileName)
        {
            ChoGuard.ArgumentNotNullOrEmpty(assemblyFileName, "AssemblyFileName");

            return _assemblyCache[assemblyFileName];
        }

        public static void AddMissingAssembly(string assemblyFileName)
        {
            ChoGuard.ArgumentNotNullOrEmpty(assemblyFileName, "Assembly File Name");

            lock (_assemblyCache.SyncRoot)
            {
                if (_missingAssemblyCache.Contains(assemblyFileName)) return;
                
                _missingAssemblyCache.Add(assemblyFileName);
                //ChoMissingAssemblies.Me.AppendLine(assemblyFileName);
            }
        }

        #endregion

        internal static bool ContainsAsMissingAssembly(string assemblyFileName)
        {
            lock (_assemblyCache.SyncRoot)
            {
                return _missingAssemblyCache.Contains(assemblyFileName);
            }
        }
    }

    #endregion ChoAssemblyManager Class

    public static class ChoAssemblyResolver
    {
        #region Shared Data Members (Private)

        private static string[] _paths = ChoCodeBase.Me.Paths;

        #endregion Shared Data Members (Private)

        #region Public Instance Constructors

        static ChoAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve +=
                new ResolveEventHandler(AssemblyResolve);

            AppDomain.CurrentDomain.AssemblyLoad +=
                new AssemblyLoadEventHandler(AssemblyLoad);
        }

        #endregion Public Instance Constructors

        #region Public Shared Methods

        /// <summary> 
        /// Installs the assembly resolver by hooking up to the 
        /// <see cref="AppDomain.AssemblyResolve" /> event.
        /// </summary>
        public static void Attach()
        {
        }

        /// <summary> 
        /// Uninstalls the assembly resolver.
        /// </summary>
        public static void Clear()
        {
            ChoAssemblyManager.Clear();

            AppDomain.CurrentDomain.AssemblyResolve -=
                new ResolveEventHandler(AssemblyResolve);

            AppDomain.CurrentDomain.AssemblyLoad -=
                new AssemblyLoadEventHandler(AssemblyLoad);
        }

        #endregion Public Instance Methods

        #region Private Shared Methods

        /// <summary> 
        /// Resolves an assembly not found by the system using the assembly 
        /// cache.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">A <see cref="ResolveEventArgs" /> that contains the event data.</param>
        /// <returns>
        /// The loaded assembly, or <see langword="null" /> if not found.
        /// </returns>
        private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            bool isFullName = args.Name.IndexOf("Version=") != -1;

            // first try to find an already loaded assembly
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (isFullName)
                {
                    if (assembly.FullName == args.Name)
                    {
                        // return assembly from AppDomain
                        return assembly;
                    }
                }
                else if (assembly.GetName(false).Name == args.Name)
                {
                    // return assembly from AppDomain
                    return assembly;
                }
            }

            if (ChoAssemblyManager.ContainsAsMissingAssembly(args.Name))
                return null;

            // find assembly in cache
            if (ChoAssemblyManager.ContainsAssembly(args.Name))
            {
                // return assembly from cache
                return (Assembly)ChoAssemblyManager.GetAssemblyFromCache(args.Name);
            }
            else
            {
                //String resourceName = "AssemblyLoadingAndReflection." + new AssemblyName(args.Name).Name + ".dll";

                //if (Assembly.GetExecutingAssembly().GetManifestResourceInfo(resourceName) != null)
                //{
                //    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                //    {
                //        Byte[] assemblyData = new Byte[stream.Length];

                //        stream.Read(assemblyData, 0, assemblyData.Length);

                //        return Assembly.Load(assemblyData);
                //    }
                //}

                string assmeblyFileName = null;
                string[] asms = args.Name.Split(new char[] { ',' });
                bool fileFound = false;
                foreach (string path in _paths)
                {
                    if (path == null || path.Trim().Length == 0) continue;

                    assmeblyFileName = Path.Combine(path, asms[0] + ".dll");

                    if (File.Exists(assmeblyFileName))
                    {
                        fileFound = true;
                        break;
                    }
                }

                if (fileFound)
                {
                    //if (ChoCodeBase.Me.LoadAssemblyFromFile)
                        return Assembly.LoadFile(assmeblyFileName);
                    //else
                    //    return Assembly.Load(ChoFile.ReadAllBytes(assmeblyFileName));
                }
                else if (!assmeblyFileName.IsNullOrEmpty())
                    ChoAssemblyManager.AddMissingAssembly(args.Name);
            }

            return null;
        }

        /// <summary>
        /// Occurs when an assembly is loaded. The loaded assembly is added 
        /// to the assembly cache.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">An <see cref="AssemblyLoadEventArgs" /> that contains the event data.</param>
        private static void AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            ChoAssemblyManager.AddAssemblyToCache(args.LoadedAssembly);
        }

        #endregion Private Instance Methods
    }
}
