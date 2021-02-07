namespace Cinchoo.Core.Reflection
{
	#region NameSpaces

	using System;
	using System.Text;
	using System.Collections.Generic;

	using Cinchoo.Core.Configuration;
	using Cinchoo.Core.Text.RegularExpressions;
    using System.Xml.Serialization;

	#endregion NameSpaces

	[ChoTypeFormatter("Assembly Settings")]
    [ChoConfigurationSection("cinchoo/assemblySettings", Defaultable = false)]
    public class ChoAssemblySettings : /* ChoConfigurableObject, */ IChoInitializable
	{
		#region Constants

		private const string DefaultExcludeAssemblies = "mscorlib*;System*";
		private const string DefaultIncludeAssemblies = null;

		#endregion Constants

        #region Shared Data Members (Private)

        private static readonly object _padLock = new object();
        private static ChoAssemblySettings _instance;

        #endregion Shared Data Members (Private)

		#region Instance Data Members (Public)

		[ChoPropertyInfo("excludeAssemblies")]
		public string ExcludeAssemblies = DefaultExcludeAssemblies;

		[ChoPropertyInfo("includeAssemblies")]
		public string IncludeAssemblies = DefaultIncludeAssemblies;

		#endregion

		#region Instance Data Members (Private)

		private List<ChoWildcard> _excludeAssembliesWildCard;
		private List<ChoWildcard> _includeAssembliesWildCard;

		#endregion Instance Data Members (Private)

		#region Instance Properties (Public)

		[ChoIgnoreMemberFormatter]
        [XmlIgnore]
        public List<ChoWildcard> ExcludeAssembliesWildCard
		{
			get { return _excludeAssembliesWildCard; }
		}

		[ChoIgnoreMemberFormatter]
        [XmlIgnore]
        public List<ChoWildcard> IncludeAssembliesWildCard
		{
			get { return _includeAssembliesWildCard; }
		}

		#endregion Instance Properties (Public)

		#region Shared Properties

        public static ChoAssemblySettings Me
        {
            //get { return ChoConfigurationManagementFactory.CreateInstance<ChoAssemblySettings>(); }
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_padLock)
                {
                    if (_instance == null)
                    {
                        _instance = new ChoAssemblySettings(); // ChoCoreFrxConfigurationManager.Register<ChoAssemblySettings>();
                        _instance.Initialize(false, null);
                    }
                }

                return _instance;
            }
        }

		#endregion

        public void AddExcludeAssemblyWildCard(string expr)
        {
            ExcludeAssembliesWildCard.Add(new ChoWildcard(expr, System.Text.RegularExpressions.RegexOptions.Compiled));
        }

        public void AddIncludeAssemblyWildCard(string expr)
        {
            IncludeAssembliesWildCard.Add(new ChoWildcard(expr, System.Text.RegularExpressions.RegexOptions.Compiled));
        }

		#region IChoObjectInitializable Members

		public bool Initialize(bool beforeFieldInit, object state)
		{
			if (beforeFieldInit) return false;

			#region Exclude Assemblies

			if (String.IsNullOrEmpty(ExcludeAssemblies))
				ExcludeAssemblies = String.Empty;

			List<ChoWildcard> wildcards = new List<ChoWildcard>();
			foreach (string excludeAssembly in ExcludeAssemblies.SplitNTrim())
			{
				if (String.IsNullOrEmpty(excludeAssembly)) continue;
				wildcards.Add(new ChoWildcard(excludeAssembly, System.Text.RegularExpressions.RegexOptions.Compiled));
			}

			_excludeAssembliesWildCard = wildcards;

			#endregion Exclude Assemblies

			#region Include Assemblies

			if (String.IsNullOrEmpty(IncludeAssemblies))
				IncludeAssemblies = String.Empty;

			wildcards = new List<ChoWildcard>();
			foreach (string includeAssembly in IncludeAssemblies.SplitNTrim())
			{
				if (String.IsNullOrEmpty(includeAssembly)) continue;
				wildcards.Add(new ChoWildcard(includeAssembly, System.Text.RegularExpressions.RegexOptions.Compiled));
			}

			_includeAssembliesWildCard = wildcards;

			#endregion Include Assemblies

			return false;
		}

		#endregion

        public void Initialize()
        {
            #region Exclude Assemblies

            if (String.IsNullOrEmpty(ExcludeAssemblies))
                ExcludeAssemblies = String.Empty;

            List<ChoWildcard> wildcards = new List<ChoWildcard>();
            foreach (string excludeAssembly in ExcludeAssemblies.SplitNTrim())
            {
                if (String.IsNullOrEmpty(excludeAssembly)) continue;
                wildcards.Add(new ChoWildcard(excludeAssembly, System.Text.RegularExpressions.RegexOptions.Compiled));
            }

            _excludeAssembliesWildCard = wildcards;

            #endregion Exclude Assemblies

            #region Include Assemblies

            if (String.IsNullOrEmpty(IncludeAssemblies))
                IncludeAssemblies = String.Empty;

            wildcards = new List<ChoWildcard>();
            foreach (string includeAssembly in IncludeAssemblies.SplitNTrim())
            {
                if (String.IsNullOrEmpty(includeAssembly)) continue;
                wildcards.Add(new ChoWildcard(includeAssembly, System.Text.RegularExpressions.RegexOptions.Compiled));
            }

            _includeAssembliesWildCard = wildcards;

            #endregion Include Assemblies
        }
    }
}
