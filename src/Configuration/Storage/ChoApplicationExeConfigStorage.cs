namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;

	#endregion NameSpaces

	internal class ChoApplicationExeConfigStorage : ChoConfigStorage
	{
        public override object Load(ChoBaseConfigurationElement configElement, XmlNode node)
		{
            base.Load(configElement, node);

			return null;
		}

		protected override string ToXml(object data)
		{
			return null;
		}

        public override object PersistableState
        {
            get { return null; }
        }

		public override IChoConfigurationChangeWatcher ConfigurationChangeWatcher
		{
			get
			{
				return ChoConfigurationManager.ConfigurationChangeWatcher;
			}
		}
	}
}
