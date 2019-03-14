namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections;
	using System.Xml;

	#endregion NameSpaces

	public class ChoDictionarySection : ChoConfigSection
	{
		#region Constructors

		public ChoDictionarySection(Type configObjectType, XmlNode node, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
			: base(configObjectType, node, contextNodes, configElement)
		{
		}

		#endregion Constructors

		#region ChoConfigSection Overrides

		protected override void CheckValidConfigStoragePassed(IChoConfigStorage configStorage)
		{
			if (!(ConfigStorage is IChoDictionaryConfigStorage))
			{
				throw new ChoConfigurationException("ConfigStorage [{0}] is not a IChoDictionaryConfigStorage type.".FormatString(ConfigStorage.GetType().FullName));
			}
		}

		public override object this[string name]
		{
			get
			{
				ChoGuard.ArgumentNotNull(name, "Name");
                foreach (string key in Collection.Keys)
                {
                    if (String.Compare(key, name, IgnoreCase) == 0)
                        return Collection[key];
                }
                return null;
			}
		}

		public override bool HasConfigMemberDefined(string key)
		{
			if (!IsValidCollection)
				return false;

            foreach (string key1 in Collection.Keys)
            {
                if (String.Compare(key1, key, IgnoreCase) == 0)
                    return true;
            }
            return false;
		}

		public override bool HasConfigSectionDefined
		{
			get
			{
				if (!IsValidCollection)
					return false;
				else
					return Collection != null && Collection.Count > 0;
			}
		}

        //public override object PersistableState
        //{
        //    get { return ChoObject.ToPersistableDictionaryCollection(ConfigObject); }
        //}

		#endregion ChoConfigSection Overrides

		#region IChoConfigSection Members

		protected override IChoConfigStorage DefaultConfigStorage
		{
			get { return new ChoFileDictionaryConfigStorage(); }
		}

		#endregion

		#region IEquatable Overrides

		public override bool Equals(ChoDisposableObject obj)
		{
			if (Object.ReferenceEquals(obj, null))
				return false;
			if (!(obj is ChoDictionarySection))
				return false;

			ChoDictionarySection newSection = (ChoDictionarySection)obj;
			if (this.Count == 0 && newSection.Count == 0)
				return true;

			if (this.Count != newSection.Count)
				return false;

			if (Object.ReferenceEquals(Collection, newSection.Collection))
				return true;
			if (Object.ReferenceEquals(Collection, null))
				return false;
			if (Object.ReferenceEquals(newSection.Collection, null))
				return false;

			foreach (DictionaryEntry keyValue in Collection)
			{
				if (!newSection.Collection.Contains(keyValue.Key))
					return false;

				if (!Object.Equals(keyValue.Value, newSection.Collection[keyValue.Key]))
					return false;
			}

			return true;
		}

		#endregion IEquatable Overrides

		#region Instance Members (Private)

        private bool IgnoreCase
        {
            get
            {
                if (ConfigElement != null && ConfigElement.MetaDataInfo != null
                    && ConfigElement.MetaDataInfo is ChoStandardConfigurationMetaDataInfo)
                    return ((ChoStandardConfigurationMetaDataInfo)ConfigElement.MetaDataInfo).IgnoreCase;
                return false;
            }
        }

		private bool IsValidCollection
		{
			get { return ConfigData == null || ConfigData is IDictionary; }
		}

		private IDictionary Collection
		{
			get
			{
				if (!IsValidCollection)
					throw new ChoConfigurationException("ConfigObject is not of IDictionary type.");
				else
					return ConfigData as IDictionary;
			}
		}

		private int Count
		{
			get
			{
				if (!base.HasConfigSectionDefined)
					return 0;

				if (!IsValidCollection)
					return 0;
				else
					return Collection != null ? Collection.Count : 0;
			}
		}
		#endregion
	}
}
