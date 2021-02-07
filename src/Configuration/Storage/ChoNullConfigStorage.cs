namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

    using System.Xml;
    using Cinchoo.Core.Services;

	#endregion NameSpaces

    public class ChoNullConfigStorage : ChoBaseConfigStorage, IChoObjectConfigStorage
	{
		public override object Load(ChoBaseConfigurationElement configElement, XmlNode node)
		{
			return null;
		}

		public override bool CanPersist(object data, ChoDictionaryService<string, object> stateInfo)
		{
			return false;
		}

		public override void Persist(object data, ChoDictionaryService<string, object> stateInfo)
		{
		}

        public override object PersistableState 
        {
            get { return null; }
        }

		public override IChoConfigurationChangeWatcher ConfigurationChangeWatcher
		{
			get { return null; }
		}
	}
}
