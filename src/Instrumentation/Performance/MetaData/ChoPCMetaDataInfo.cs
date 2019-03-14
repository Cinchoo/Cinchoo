namespace Cinchoo.Core.Instrumentation
{
	#region NameSpaces

    using System.Xml.Serialization;

    #endregion NameSpaces

    public class ChoPCMetaDataInfo : ChoEquatableObject<ChoPCMetaDataInfo>, IChoMergeable<ChoPCMetaDataInfo>
	{
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("instanceName")]
        public string InstanceName;

        [XmlAttribute("turnOn")]
		public bool TurnOn = true;

        public ChoPCMetaDataInfo()
        {
        }

        public ChoPCMetaDataInfo(string name, string instanceName)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");
            Name = name;

            if (instanceName != ChoPerformanceCounter.DefaultInstanceName)
                InstanceName = instanceName;
        }

        public override bool Equals(ChoPCMetaDataInfo obj)
        {
            if (object.ReferenceEquals(obj, null))
                return false;

            if (Name != obj.Name)
                return false;

            if (InstanceName != obj.InstanceName)
                return false;

            if (obj.TurnOn != TurnOn)
                return false;

            return true;
        }

        #region IChoMergeable<ChoPCMetaDataInfo> Members

        public virtual void Merge(ChoPCMetaDataInfo source)
        {
            TurnOn = source.TurnOn;
        }

        #endregion

        #region IChoMergeable Members

        public void Merge(object source)
        {
            Merge((ChoPCMetaDataInfo)source);
        }

        #endregion
    }
}
