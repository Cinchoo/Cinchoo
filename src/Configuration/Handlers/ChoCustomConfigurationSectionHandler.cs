namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
    using System.Xml;

    #endregion NameSpaces

    public abstract class ChoCustomConfigurationSectionHandler : IChoCustomConfigurationSectionHandler
	{
		#region Instance Data Members (Private)

		private string _parameters;

		#endregion Instance Data Members (Private)

		#region IChoCustomConfigurationSectionHandler Members

		public string Parameters
		{
			get { return _parameters; }
			set { _parameters = value; }
		}

		#endregion

		#region IChoConfigurationSectionHandler Members

		public abstract ChoConfigSection Create(Type configObjectType, XmlNode section, XmlNode[] contextNodes, ChoBaseConfigurationElement configElement);

		#endregion
	}
}
