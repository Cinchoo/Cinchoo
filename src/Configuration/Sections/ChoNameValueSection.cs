namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Xml;


    #endregion NameSpaces

    public class ChoNameValueSection : ChoConfigSection
	{
		#region Constructors

		public ChoNameValueSection(Type configObjectType, XmlNode node, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement)
			: base(configObjectType, node, contextNodes, configElement)
		{
        }

		#endregion Constructors

		#region ChoConfigSection Overrides

        protected override void CheckValidConfigStoragePassed(IChoConfigStorage configStorage)
        {
            if (!(ConfigStorage is IChoNameValueConfigStorage))
            {
                throw new ChoConfigurationException("ConfigStorage [{0}] is not a IChoNameValueConfigStorage type.".FormatString(ConfigStorage.GetType().FullName));
            }
        }

		public override object this[string name]
		{
			get
			{
				ChoGuard.ArgumentNotNullOrEmpty(name, "Name");
                foreach (string key in Collection.AllKeys)
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

            foreach (string key1 in Collection.AllKeys)
            {
                if (String.Compare(key1, key, IgnoreCase) == 0)
                    return true;
            }
            return false;
            //return Collection.AllKeys.Contains(key, StringComparer.InvariantCultureIgnoreCase);
        }

		public override bool HasConfigSectionDefined
		{
			get 
			{
                //if (!base.HasConfigSectionDefined)
                //    return false;

                if (!IsValidCollection)
					return false;
				else
                    return Collection != null && Collection.Count > 0;
			}
		}

        //public override object PersistableState
        //{
        //    get { return ChoObject.ToPersistableNameValueCollection(ConfigObject); }
        //}

		#endregion ChoConfigSection Overrides

		#region IChoConfigSection Members

		protected override IChoConfigStorage DefaultConfigStorage
		{
			get { return new ChoFileNameValueConfigStorage(); }
		}

		#endregion

        #region IEquatable Overrides

        public override bool Equals(ChoDisposableObject obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return false;
            if (!(obj is ChoNameValueSection))
                return false;

            ChoNameValueSection newSection = obj as ChoNameValueSection;
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

            //Is all keys are same
            var diff = Collection.AllKeys.Except(newSection.Collection.AllKeys);
            if (diff.Count() != 0)
                return false;

            //Check the values are equal
            foreach (string key in Collection.AllKeys)
            {
                if (!ChoObject.Equals(this[key], newSection[key]))
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
            get { return ConfigData == null || ConfigData is NameValueCollection; }
        }

        private NameValueCollection Collection
        {
            get
            {
                if (!IsValidCollection)
                    throw new ChoConfigurationException("ConfigObject is not of NameValueCollection type.");
                else
                    return ConfigData as NameValueCollection;
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
