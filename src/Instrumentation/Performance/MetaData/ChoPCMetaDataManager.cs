namespace Cinchoo.Core.Instrumentation
{
	#region NameSpaces

    using System;
    using System.IO;
    using System.Xml;
    using Cinchoo.Core;
    using Cinchoo.Core.IO;
    using Cinchoo.Core.Pattern;

	#endregion NameSpaces

    [ChoAppDomainEventsRegisterableType]
    public class ChoPCMetaDataManager : ChoMetaDataManager<ChoPerformanceCounter, ChoPCMetaDataInfo>
    {
        #region Instance Data Members (Private)

        private static ChoPCMetaDataManager _instance = new ChoPCMetaDataManager();

        #endregion Instance Data Members (Private)

        #region ChoMetaDataManager Overrides

        internal bool RecreateCounters(Type type)
        {
            bool recreate = true;
            if (type != null)
            {
                ChoPerformanceCounterCategoryAttribute performanceCounterCategoryAttribute = type.GetCustomAttribute<ChoPerformanceCounterCategoryAttribute>();
                XmlNode rootNode = RootNode;
                if (performanceCounterCategoryAttribute != null && rootNode != null)
                {
                    XmlNode categoryNode = rootNode.SelectSingleNode("//PerformanceCounterCategory[@name='{0}']".FormatString(performanceCounterCategoryAttribute.CategoryName));
                    if (categoryNode != null)
                    {
                        XmlAttribute attribute = categoryNode.Attributes["recreate"];
                        if (attribute != null)
                        {
                            bool.TryParse(attribute.Value, out recreate);
                        }
                    }
                }
            }
            return recreate;
        }

        internal bool SelfInstallCounters(Type type)
        {
            bool selfInstall = true;
            if (type != null)
            {
                ChoPerformanceCounterCategoryAttribute performanceCounterCategoryAttribute = type.GetCustomAttribute<ChoPerformanceCounterCategoryAttribute>();
                XmlNode rootNode = RootNode;
                if (performanceCounterCategoryAttribute != null && rootNode != null)
                {
                    XmlNode categoryNode = rootNode.SelectSingleNode("//PerformanceCounterCategory[@name='{0}']".FormatString(performanceCounterCategoryAttribute.CategoryName));
                    if (categoryNode != null)
                    {
                        XmlAttribute attribute = categoryNode.Attributes["selfInstall"];
                        if (attribute != null)
                        {
                            bool.TryParse(attribute.Value, out selfInstall);
                        }
                    }
                }
            }
            return selfInstall;
        }

        public override string MetaDataFilePath
        {
            get
            {
                return ChoMetaDataFilePathSettings.Me != null ? ChoMetaDataFilePathSettings.Me.OverridenPCMetaDataFilePath : null; 
            }
        }

        #endregion ChoMetaDataManager Overrides

        public static ChoPCMetaDataManager Me
        {
            get { return _instance; }
        }
    }
}
