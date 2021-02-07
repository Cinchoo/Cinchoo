namespace Cinchoo.Core.Reflection
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Collections;

    using Cinchoo.Core.IO;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.Collections;
    using System.Collections.Generic;
    using Cinchoo.Core.Text.RegularExpressions;
    using System.Web;
    using System.Diagnostics;

    #endregion NameSpaces

    internal sealed class ChoExcludedAssembly
    {
    }

    internal sealed class ChoCodeBaseAssembly
    {
    }

    public static class ChoAssembly
    {
        #region Constants

        private const string AspNetNamespace = "ASP";

        #endregion Constants

        #region Instance Data Members (Private)

        private static string[] _paths; // = ChoCodeBase.Me.Paths; // WorkflowManager.Paths;

        #endregion

        #region Constructors

        static ChoAssembly()
        {
            ChoAssemblyResolver.Attach();
            //if (ChoCodeBase.Me != null)
            //    _paths = ChoCodeBase.Me.Paths;

            //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        #endregion

        #region Shared Members (Public)

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //if (_paths == null || _paths.Length == 0) return null;

            //string assmeblyFileName = null;
            //string[] asms = args.Name.Split(new char[] { ',' });

            //bool fileFound = false;
            //if (ChoAssemblyResolver.Contains(asms[0]))
            //{
            //    fileFound = true;
            //    assmeblyFileName = ChoAssemblyResolver.GetAssemblyFileLocation(asms[0]);
            //}
            //else
            //{
            //    foreach (string path in _paths)
            //    {
            //        if (path == null || path.Trim().Length == 0) continue;

            //        assmeblyFileName = Path.Combine(path, asms[0] + ".dll");

            //        if (File.Exists(assmeblyFileName))
            //        {
            //            fileFound = true;
            //            ChoAssemblyResolver.Add(asms[0], assmeblyFileName);
            //            break;
            //        }
            //    }
            //}

            //if (fileFound && assmeblyFileName != null)
            //{
            //    if (ChoCodeBase.Me.LoadAssemblyFromFile)
            //        return Assembly.LoadFile(assmeblyFileName);
            //    else
            //        return Assembly.Load(ChoFile.ReadAllBytes(assmeblyFileName));
            //}
            //else
            //    ChoAssemblyResolver.AddMissingAssembly(assmeblyFileName);

            return null;
        }

        #endregion Shared Members (Public)

        #region Shared Members (Public)

        internal static void Initialize()
        {
        }

        private static readonly object _entryAssemblyLock = new object();
        private static Assembly _entryAssembly;

        public static string GetAssemblyCopyright()
        {
            if (GetEntryAssembly() == null) return null;

            var attributes = GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributes.Length > 0)
            {
                var titleAttribute = (AssemblyCopyrightAttribute)attributes[0];
                if (titleAttribute != null && !titleAttribute.Copyright.IsNullOrWhiteSpace())
                    return titleAttribute.Copyright;
            }
            return null; // System.IO.Path.GetFileNameWithoutExtension(GetEntryAssembly().CodeBase);
        }

        public static string GetAssemblyTitle()
        {
            if (GetEntryAssembly() == null) return null;

            var attributes = GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attributes.Length > 0)
            {
                var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                if (titleAttribute != null && !titleAttribute.Title.IsNullOrWhiteSpace())
                    return titleAttribute.Title;
            }
            return null; // System.IO.Path.GetFileNameWithoutExtension(GetEntryAssembly().CodeBase);
        }

        public static string GetAssemblyDescription()
        {
            if (GetEntryAssembly() == null) return null;

            var attributes = GetEntryAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                var titleAttribute = (AssemblyDescriptionAttribute)attributes[0];
                if (titleAttribute != null && !titleAttribute.Description.IsNullOrWhiteSpace())
                    return titleAttribute.Description;
            }
            return null; // System.IO.Path.GetFileNameWithoutExtension(GetEntryAssembly().CodeBase);
        }

        public static string GetEntryAssemblyLocation()
        {
            HttpContext ctx = HttpContext.Current;
            if (ctx == null)
                return ChoAssembly.GetEntryAssembly().Location;
            else if (!HttpContext.Current.Request.PhysicalApplicationPath.IsNullOrEmpty())
            {
                if (HttpContext.Current.Request.PhysicalApplicationPath.EndsWith(@"\"))
                    return HttpContext.Current.Request.PhysicalApplicationPath.Substring(0, HttpContext.Current.Request.PhysicalApplicationPath.Length - 1);
                else
                    return HttpContext.Current.Request.PhysicalApplicationPath;
            }
            else
                return Assembly.GetExecutingAssembly().Location;
        }

        public static Assembly GetEntryAssembly()
        {
            if (_entryAssembly != null)
                return _entryAssembly;

            lock (_entryAssemblyLock)
            {
                if (_entryAssembly != null)
                    return _entryAssembly;

                // Try the EntryAssembly, this doesn't work for ASP.NET classic pipeline (untested on integrated)
                Assembly assembly = Assembly.GetEntryAssembly();

                // Look for web application assembly
                HttpContext ctx = HttpContext.Current;
                if (ctx != null)
                    assembly = GetWebApplicationAssembly(ctx);

                // Fallback to executing assembly
                _entryAssembly = assembly ?? (Assembly.GetExecutingAssembly());
                return _entryAssembly;
            }
        }

        #region GetAssemblies Overloads

        private readonly static object _loadedAssemblyLock = new object();
        private static List<Assembly> _loadedAssemblies = null;

        internal static void AddToLoadedAssembly(Assembly assembly)
        {
            if (assembly == null) return;

            lock (_loadedAssemblyLock)
            {
                _loadedAssemblies.Add(assembly);
            }
        }

        public static Assembly[] GetLoadedAssemblies()
        {
            if (_loadedAssemblies != null)
                return _loadedAssemblies.ToArray();

            lock (_loadedAssemblyLock)
            {
                if (_loadedAssemblies != null)
                    return _loadedAssemblies.ToArray();

                _loadedAssemblies = new List<Assembly>();

                //using (ChoBufferProfileEx profile = new ChoBufferProfileEx(ChoType.GetLogFileName(typeof(ChoExcludedAssembly)), String.Format("Below are the excluded assemblies...")))
                //{
                    Assembly[] loadedAssemblies = null;
                    try
                    {
                        LoadReferencedAssemblies();
                        loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                    }
                    catch (System.Security.SecurityException)
                    {
                        // Insufficient permissions to get the list of loaded assemblies
                    }

                    if (loadedAssemblies != null)
                    {
                        // Search the loaded assemblies for the type
                        foreach (Assembly assembly in loadedAssemblies)
                        {
                            DiscoverNLoadAssemblies(null, assembly, _loadedAssemblies);
                        }
                    }

                    return _loadedAssemblies.ToArray();
                //}
            }
        }

        private static void LoadReferencedAssemblies()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select((a) =>
                {
                    if (!a.IsDynamic)
                    {
                        try
                        {
                            return a.Location;
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.ToString());
                        }
                    }
                    return String.Empty;
                }).ToArray();

            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.AllDirectories);
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
            toLoad.ForEach(path => 
                {
                    try
                    {
                        if (!IsExcludedAssembly(path))
                        {
                            Assembly ass = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path));
                            loadedAssemblies.Add(ass);
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.ToString());
                    }
                });
        }

        private static void DiscoverNLoadAssemblies(ChoBufferProfileEx profile, Assembly assembly, List<Assembly> assemblies)
        {
            if (IsExcludedAssembly(assembly.FullName))
            {
                //profile.Info(assembly.FullName);
            }
            else
            {
                assemblies.Add(assembly);
            }
        }

        public static Assembly[] GetAssemblies(string directory)
        {
            return GetAssemblies(new string[] { directory });
        }

        public static Assembly[] GetAssemblies(string[] directories)
        {
            List<Assembly> assemblies = new List<Assembly>();

            //using (ChoBufferProfileEx fileProfile = new ChoBufferProfileEx(ChoType.GetLogFileName(typeof(ChoCodeBaseAssembly)), String.Format("Below are the assemblies loaded from files...")))
            //{
                foreach (string directory in directories)
                {
                    if (directory == null) continue;
                    foreach (string file in Directory.GetFiles(directory, "*.dll")) //TODO: Filter needs to be configurable
                    {
                        if (file == null) continue;
                        if (IsExcludedAssembly(file)) continue;

                        try
                        {
                            Assembly assembly = Assembly.LoadFile(file);
                            if (assembly != null)
                            {
                                DiscoverNLoadAssemblies(null, assembly, assemblies);
                                //fileProfile.Info(file);
                            }
                        }
                        catch (ChoFatalApplicationException)
                        {
                            throw;
                        }
                        catch (Exception) // ex)
                        {
                            //fileProfile.ErrorFormat("Error [{0}]: {1}", file, ex.Message);
                        }
                    }
                }
            //}
            return assemblies.ToArray();
        }

        #endregion GetAssemblies Overloads

        /// <summary>
        /// Gets the assembly location path for the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly to get the location for.</param>
        /// <returns>The location of the assembly.</returns>
        /// <remarks>
        /// <para>
        /// This method does not guarantee to return the correct path
        /// to the assembly. If only tries to give an indication as to
        /// where the assembly was loaded from.
        /// </para>
        /// </remarks>
        public static string GetAssemblyLocationInfo(Assembly assembly)
        {
            if (assembly.GlobalAssemblyCache)
            {
                return "Global Assembly Cache";
            }
            else
            {
                try
                {
                    // This call requires FileIOPermission for access to the path
                    // if we don't have permission then we just ignore it and
                    // carry on.
                    return assembly.Location;
                }
                catch (System.Security.SecurityException)
                {
                    return "Location Permission Denied";
                }
            }
        }

        /// <summary>
        /// Gets the fully qualified name of the <see cref="Type" />, including 
        /// the name of the assembly from which the <see cref="Type" /> was 
        /// loaded.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to get the fully qualified name for.</param>
        /// <returns>The fully qualified name for the <see cref="Type" />.</returns>
        /// <remarks>
        /// <para>
        /// This is equivalent to the <c>Type.AssemblyQualifiedName</c> property,
        /// but this method works on the .NET Compact Framework 1.0 as well as
        /// the full .NET runtime.
        /// </para>
        /// </remarks>
        public static string AssemblyQualifiedName(Type type)
        {
            return type.FullName + ", " + type.Assembly.FullName;
        }

        /// <summary>
        /// Gets the short name of the <see cref="Assembly" />.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly" /> to get the name for.</param>
        /// <returns>The short name of the <see cref="Assembly" />.</returns>
        /// <remarks>
        /// <para>
        /// The short name of the assembly is the <see cref="Assembly.FullName" /> 
        /// without the version, culture, or public key. i.e. it is just the 
        /// assembly's file name without the extension.
        /// </para>
        /// <para>
        /// Use this rather than <c>Assembly.GetName().Name</c> because that
        /// is not available on the Compact Framework.
        /// </para>
        /// <para>
        /// Because of a FileIOPermission security demand we cannot do
        /// the obvious Assembly.GetName().Name. We are allowed to get
        /// the <see cref="Assembly.FullName" /> of the assembly so we 
        /// start from there and strip out just the assembly name.
        /// </para>
        /// </remarks>
        public static string AssemblyShortName(Assembly assembly)
        {
            string name = assembly.FullName;
            int offset = name.IndexOf(',');
            if (offset > 0)
            {
                name = name.Substring(0, offset);
            }
            return name.Trim();
        }

        /// <summary>
        /// Gets the file name portion of the <see cref="Assembly" />, including the extension.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly" /> to get the file name for.</param>
        /// <returns>The file name of the assembly.</returns>
        /// <remarks>
        /// <para>
        /// Gets the file name portion of the <see cref="Assembly" />, including the extension.
        /// </para>
        /// </remarks>
        public static string AssemblyFileName(Assembly assembly)
        {
            return System.IO.Path.GetFileName(assembly.Location);
        }

        public static string GetAssemblyName()
        {
            return GetAssemblyName(Assembly.GetCallingAssembly().FullName);
        }

        public static string GetAssemblyName(object assemblyObject)
        {
            if (assemblyObject == null) return null;
            return GetAssemblyName(assemblyObject.GetType().Assembly.FullName);
        }

        public static string GetAssemblyName(string assemblyFullName)
        {
            if (assemblyFullName == null) return null;
            if (assemblyFullName.IndexOf(",") < 0) return assemblyFullName;
            return assemblyFullName.Substring(0, assemblyFullName.IndexOf(","));
        }

        #endregion

        #region Shared Members (Private)

        private static Assembly GetWebApplicationAssembly(HttpContext context)
        {
            ChoGuard.ArgumentNotNull(context, "context");

            IHttpHandler handler = context.CurrentHandler;
            if (handler == null)
                return null;

            Type type = handler.GetType();
            while (type != null && type != typeof(object) && type.Namespace == AspNetNamespace)
                type = type.BaseType;

            return type.Assembly;
        }

        private static bool IsExcludedAssembly(string assmeblyName)
        {
            if (ChoAssemblySettings.Me.IncludeAssembliesWildCard != null)
            {
                foreach (ChoWildcard wildcardEx in ChoAssemblySettings.Me.IncludeAssembliesWildCard)
                {
                    if (wildcardEx.IsMatch(assmeblyName))
                        return false;
                }
            }

            if (ChoAssemblySettings.Me.ExcludeAssembliesWildCard != null)
            {
                foreach (ChoWildcard wildcardEx in ChoAssemblySettings.Me.ExcludeAssembliesWildCard)
                {
                    if (wildcardEx.IsMatch(assmeblyName))
                        return true;
                }
            }

            return false;
        }

        #endregion Shared Members (Private)
    }
}
