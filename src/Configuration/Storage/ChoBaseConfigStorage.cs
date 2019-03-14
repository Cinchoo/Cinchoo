namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

    using System;
    using System.Xml;
    using Cinchoo.Core;
    using Cinchoo.Core.Services;

    #endregion NameSpaces

    public abstract class ChoBaseConfigStorage : IChoConfigStorage
	{
		#region Instance Data Members (Protected)

		protected string ConfigSectionName = "UNKNOWN";
        protected XmlNode RootConfigNode;
        protected XmlNode ConfigNode;
		internal ChoBaseConfigurationElement ConfigElement;
		protected Type ConfigObjectType;
        private readonly object _syncRoot = new object();

		#endregion Instance Data Members (Protected)

		public virtual object Load(ChoBaseConfigurationElement configElement, XmlNode node)
		{
			ChoGuard.ArgumentNotNull(configElement, "ConfigElement");

			ConfigElement = configElement;
            
            //ConfigElement.LastLoadedTimeStamp = DateTime.Now;
            ConfigObjectType = ConfigElement.ConfigbObjectType;
            ConfigSectionName = configElement.ConfigSectionName;
            RootConfigNode = ConfigNode = node;

            //ChoMetaDataManager.SetWatcher(this);

			if (ConfigObjectType == null)
				throw new ChoConfigurationException("Missing configuration object type.");

			return null;
		}

        public virtual void PostLoad(ChoBaseConfigurationElement configElement)
        {
        }

		public abstract void Persist(object data, ChoDictionaryService<string, object> stateInfo);
		public virtual bool CanPersist(object data, ChoDictionaryService<string, object> stateInfo)
		{
            DateTime persistTimeStamp = GetPersistTimeStamp(stateInfo);

            if (persistTimeStamp < ConfigElement.LastLoadedTimeStamp)
            {
                ConfigElement.Log("SKIPPING Persist: Stale config data [ {0} - {1} ]".FormatString(persistTimeStamp.Ticks, ConfigElement.LastLoadedTimeStamp.Ticks));
                return false;
            }
            else
                return true;
		}

        private DateTime GetPersistTimeStamp(ChoDictionaryService<string, object> stateInfo)
        {
            object persistTimeStamp = stateInfo[ChoConfigurationConstants.PERSIST_TIME_STAMP];

            return persistTimeStamp != null ? (DateTime)persistTimeStamp : DateTime.MinValue;
        }

		public abstract IChoConfigurationChangeWatcher ConfigurationChangeWatcher
		{
			get;
		}

		public object SyncRoot
		{
			get { return _syncRoot; }
		}

        public virtual bool IsConfigSectionDefined
        {
            get { return ConfigNode != null; }
        }

        public abstract object PersistableState { get; }

		#region IDisposable Members

		protected virtual void Dispose(bool finalize)
		{
            //ChoMetaDataManager.DisposeWatcher(this);
		}

		public void Dispose()
		{
			Dispose(false);
		}

		#endregion

		#region Destructor

		~ChoBaseConfigStorage()
		{
			Dispose(true);
		}

		#endregion
   }
}
