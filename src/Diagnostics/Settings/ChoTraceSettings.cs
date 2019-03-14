namespace Cinchoo.Core.Diagnostics
{
	#region Namespaces

    using Cinchoo.Core.Configuration;

    #endregion

    [ChoTypeFormatter("Trace Settings")]
    [ChoConfigurationSection("cinchoo/traceSettings", Defaultable = false)]
	public class ChoTraceSettings //: ChoConfigurableObject
	{
		#region Shared Data Members (public)

		[ChoPropertyInfo("IndentProfiling")]
		public bool IndentProfiling = true;

		#endregion

		#region Shared Properties

		public static ChoTraceSettings Me
		{
			get { return ChoConfigurationManagementFactory.CreateInstance<ChoTraceSettings>(); }
		}

		#endregion
	}
}
