namespace Cinchoo.Core.Reflection
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Diagnostics;

    #endregion NameSpaces

    [ChoStreamProfile("Missing Assemblies", NameFromTypeFullName = typeof(ChoMissingAssemblies))]
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

	[ChoStreamProfile("Loaded Assemblies", NameFromTypeFullName = typeof(ChoLoadedAssemblies))]
    [ChoObjectFactory(ChoObjectConstructionType.Singleton)]
    public class ChoLoadedAssemblies : ChoContextProfiler
    {
        #region Instance Members (Public)

        public void Add(Assembly assembly)
        {
            if (ChoTraceSwitch.Switch.TraceVerbose)
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

        private static readonly object _padLock = new object();
        /// <summary>
        /// Holds the loaded assemblies.
        /// </summary>
        private static readonly Dictionary<string, Assembly> _assemblyCache = new Dictionary<string, Assembly>();

        /// <summary>
        /// Holds the missing assembly cache.
        /// </summary>
        private static readonly List<string> _missingAssemblyCache = new List<string>();

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
            lock (_padLock)
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

            lock (_padLock)
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

            lock (_padLock)
            {
                if (_missingAssemblyCache.Contains(assemblyFileName)) return;
                
                _missingAssemblyCache.Add(assemblyFileName);
                //ChoMissingAssemblies.Me.AppendLine(assemblyFileName);
            }
        }

        #endregion

        internal static bool ContainsAsMissingAssembly(string assemblyFileName)
        {
            lock (_padLock)
            {
                return _missingAssemblyCache.Contains(assemblyFileName);
            }
        }
    }

    #endregion ChoAssemblyManager Class

    public static class ChoAssemblyResolver
    {
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
            Assembly assembly = DiscoverAssembly(sender, args);
            if (assembly != null)
                ChoAssembly.AddToLoadedAssembly(assembly);

            return assembly;
        }
         
        private static Assembly DiscoverAssembly(object sender, ResolveEventArgs args)
        {
            if (ChoAssemblyManager.ContainsAsMissingAssembly(args.Name))
                return null;

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
                int index = args.Name.IndexOf(',');
                var name = index < 0 ? args.Name + ".dll" : args.Name.Substring(0, index) + ".dll";

                Assembly resAssembly = LoadAssemblyFromResource(name);
                if (resAssembly != null)
                    return resAssembly;
                else
                {
                    resAssembly = ChoEmbeddedAssembly.Get(name);
                    if (resAssembly != null)
                        return resAssembly;
                }

                bool fileFound = false;
                foreach (string path in ChoCodeBase.Me.Paths)
                {
                    if (path == null || path.Trim().Length == 0) continue;

                    assmeblyFileName = Path.Combine(path, name);

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

        private static Assembly LoadAssemblyFromResource(string name)
        {
            //Assembly thisAssembly = Assembly.GetEntryAssembly();

            foreach (Assembly thisAssembly in ChoAssembly.GetLoadedAssemblies())
            {
                if (thisAssembly.IsDynamic) continue;
                try
                {
                    //Load form Embedded Resources - This Function is not called if the Assembly is in the Application Folder
                    var resources = thisAssembly.GetManifestResourceNames().Where(s => s.EndsWith(name));
                    if (resources.Count() > 0)
                    {
                        var resourceName = resources.First();
                        using (Stream stream = thisAssembly.GetManifestResourceStream(resourceName))
                        {
                            if (stream == null) return null;
                            var block = new byte[stream.Length];
                            stream.Read(block, 0, block.Length);
                            return Assembly.Load(block);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ChoTrace.Error(ex);
                }
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
