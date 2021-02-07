namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
    using System.Xml.Serialization;
    using Cinchoo.Core.Diagnostics;

    #endregion NameSpaces

    public abstract class ChoBaseConfigurationMetaDataInfo : ChoEquatableObject<ChoBaseConfigurationMetaDataInfo>, IChoMergeable<ChoBaseConfigurationMetaDataInfo>,
        ICloneable<ChoBaseConfigurationMetaDataInfo>
	{
		[XmlElement("configStorage")]
		public string ConfigStorageType;

        [ChoIgnoreMemberFormatter]
        [XmlIgnore]
        public IChoConfigStorage ConfigStorage
        {
            get
            {
                if (!ConfigStorageType.IsNullOrEmpty())
                {
                    try
                    {
                        Type configStorageType = ChoType.GetType(ConfigStorageType);
                        if (configStorageType != null && typeof(IChoConfigStorage).IsAssignableFrom(configStorageType))
                            return ChoType.CreateInstance(configStorageType) as IChoConfigStorage;
                    }
                    catch (Exception ex)
                    {
                        ChoTrace.Error(ex.ToString());
                    }
                }
                return null;
            }
            internal set
            {
                if (value != null)
                    ConfigStorageType = value.GetType().SimpleQualifiedName();
            }
        }

        public override bool Equals(ChoBaseConfigurationMetaDataInfo obj)
        {
            if (object.ReferenceEquals(obj, null))
                return false;

            if (obj.ConfigStorageType != ConfigStorageType)
                return false;

            return true;
        }

		protected abstract void Initialize();

        #region IChoMergeable<ChoBaseConfigurationMetaDataInfo> Members

        public virtual void Merge(ChoBaseConfigurationMetaDataInfo source)
        {
            if (ConfigStorageType.IsNullOrEmpty())
                ConfigStorageType = source.ConfigStorageType;
        }

        #endregion

        #region IChoMergeable Members

        public void Merge(object source)
        {
            Merge((ChoBaseConfigurationMetaDataInfo)source);
        }

        #endregion

        public abstract ChoBaseConfigurationMetaDataInfo Clone();

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
