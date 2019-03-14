namespace Cinchoo.Core.Collections.Specialized
{
	#region NameSpaces

	using System;
	using System.Text;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using Cinchoo.Core.Configuration;

	#endregion NameSpaces

	[ChoTypeFormatter("NameValueCollection Settings")]
	[ChoConfigurationSection("cinchoo/nameValueCollectionSettings")]
	public class ChoNameValueCollectionSettings : ChoConfigurableObject
	{
		#region Instance Data Members (Public)

		[ChoPropertyInfo("NameValueSeperator")]
		public string NameValueSeperator = "=";

		[ChoPropertyInfo("NameValuePairSeperator")]
		public string NameValuePairSeperator = ";";

		#endregion

		#region Shared Properties

		public static ChoNameValueCollectionSettings Me
		{
			get { return ChoConfigurationManagementFactory.CreateInstance<ChoNameValueCollectionSettings>(); }
		}

		#endregion
	}
}
