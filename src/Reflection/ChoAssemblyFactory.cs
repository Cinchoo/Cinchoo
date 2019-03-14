namespace Cinchoo.Core.Reflection
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;
    using Cinchoo.Core.Collections.Generic;

    #endregion NameSpaces

    [ChoObjectFactory(ChoObjectConstructionType.Singleton)]
    [ChoTypeFormatter("Global Assembly Factory")]
    public class ChoGlobalAssemblyFactory : IChoObjectInitializable
    {
        #region Instance Data Members (Private)

        private ChoDictionary<string, Assembly> _loadedAssemblies = ChoDictionary<string, Assembly>.Synchronized(new ChoDictionary<string, Assembly>());

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

            foreach (Assembly assembly in ChoAssembly.GetAssemblies(directories))
            {
                if (_loadedAssemblies.ContainsKey(assembly.FullName)) continue;
                _loadedAssemblies.Add(assembly.FullName, assembly);
            }
        }

        #endregion Instance Members (Public)

        #region Instance Properties (Public)

        public Assembly[] Assemblies
        {
            get { return _loadedAssemblies.ToValuesArray(); }
        }

        #endregion Instance Properties (Public)

        #region Shared Properties

        public static ChoGlobalAssemblyFactory Me
        {
            get { return ChoObjectManagementFactory.CreateInstance(typeof(ChoGlobalAssemblyFactory)) as ChoGlobalAssemblyFactory; }
        }

        #endregion Shared Properties

        #region IChoObjectInitializable Members

        public bool Initialize(bool beforeFieldInit, object state)
        {
            if (beforeFieldInit) return false;

            //Load Default Assemblies
            foreach (Assembly assembly in ChoAssembly.GetLoadedAssemblies())
            {
                if (_loadedAssemblies.ContainsKey(assembly.FullName)) continue;
                _loadedAssemblies.Add(assembly.FullName, assembly);
            }

            LoadAssemblies(ChoCodeBase.Me.Paths);

            return false;
        }

        #endregion
    }
}
