namespace Cinchoo.Core.Reflection
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;
    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Configuration;

    #endregion NameSpaces

    //[ChoObjectFactory(ChoObjectConstructionType.Singleton)]
    [ChoTypeFormatter("Global Assembly Factory")]
    public class ChoGlobalAssemblyFactory : IChoInitializable
    {
        #region Shared Data Members (Private)

        private static readonly object _padLock = new object();
        private static ChoGlobalAssemblyFactory _instance;

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Private)

        private readonly Dictionary<string, Assembly> _loadedAssemblies = new Dictionary<string, Assembly>();
        public event EventHandler<ChoEventArgs<Assembly>> AssemblyLoaded;

        #endregion Instance Data Members (Private)

        #region Instance Members (Public)

        public void LoadAssemblies(string directory)
        {
            if (String.IsNullOrEmpty(directory)) return;

            LoadAssemblies(new string[] { directory });
        }

        public void LoadAssemblies(string[] directories)
        {
            if (!ChoGuard.IsArgumentNotNullOrEmpty(directories)) return;

            lock (_padLock)
            {
                foreach (Assembly assembly in ChoAssembly.GetAssemblies(directories))
                {
                    if (_loadedAssemblies.ContainsKey(assembly.FullName)) continue;
                    _loadedAssemblies.Add(assembly.FullName, assembly);
                    AssemblyLoaded.Raise(null, new ChoEventArgs<Assembly>(assembly));
                }
            }
        }

        #endregion Instance Members (Public)

        #region Instance Properties (Public)

        public IEnumerable<Assembly> Assemblies
        {
            get { return _loadedAssemblies.Values; }
        }

        #endregion Instance Properties (Public)

        #region Shared Properties

        public static ChoGlobalAssemblyFactory Me
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_padLock)
                {
                    if (_instance == null)
                    {
                        _instance = new ChoGlobalAssemblyFactory(); // ChoCoreFrxConfigurationManager.Register<ChoGlobalAssemblyFactory>();
                        _instance.Initialize();
                    }
                }

                return _instance;
            }
        }

        #endregion Shared Properties

        #region IChoInitializable Members

        public void Initialize()
        {
            //Load Default Assemblies
            foreach (Assembly assembly in ChoAssembly.GetLoadedAssemblies())
            {
                if (_loadedAssemblies.ContainsKey(assembly.FullName)) continue;
                _loadedAssemblies.Add(assembly.FullName, assembly);
            }

            LoadAssemblies(ChoCodeBase.Me.Paths);
        }

        #endregion
    }
}
