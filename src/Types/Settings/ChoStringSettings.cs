namespace Cinchoo.Core
{
	#region NameSpaces

    using Cinchoo.Core.Configuration;

    #endregion NameSpaces

    [ChoTypeFormatter("String Settings")]
	[ChoConfigurationSection("cinchoo/stringSettings")]
	public class ChoStringSettings : ChoConfigurableObject
	{
		#region Instance Data Members (Public)

		[ChoPropertyInfo("Invariants")]
		[ChoTypeConverter(typeof(ChoToStringArrayConverter))]
		[ChoMemberFormatter("Invariants", Formatter = typeof(ChoArrayToStringFormatter))]
		public string[] Invariants = { "alias", "news" };

		[ChoPropertyInfo("Abbrs", DefaultValue = "ESC, RAJ")]
		[ChoTypeConverter(typeof(ChoToUpperCaseConverter), Priority = 1)]
		[ChoTypeConverter(typeof(ChoToStringArrayConverter), Priority = 0)]
		[ChoMemberFormatter("Abbrs", Formatter = typeof(ChoStringToArrayFormatter))]
		public string[] Abbrs;

		#endregion Instance Data Members (Public)

		#region Shared Properties

		public static ChoStringSettings Me
		{
			get { return ChoConfigurationManagementFactory.CreateInstance<ChoStringSettings>(); }
		}

		#endregion
	}
}
