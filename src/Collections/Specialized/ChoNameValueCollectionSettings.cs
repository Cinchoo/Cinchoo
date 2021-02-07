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
	public class ChoNameValueCollectionSettings //: ChoConfigurableObject
	{
        private static readonly ChoNameValueCollectionSettings _instance = new ChoNameValueCollectionSettings();

		#region Instance Data Members (Public)

		[ChoPropertyInfo("NameValueSeparator")]
		public string NameValueSeparator = "=";

		[ChoPropertyInfo("NameValuePairSeparator")]
		public string NameValuePairSeparator = ";";

		#endregion

		#region Shared Properties

		public static ChoNameValueCollectionSettings Me
		{
            get { return _instance; }
		}

		#endregion
	}
}
