namespace Cinchoo.Core
{
    #region NameSpaces

    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.IO;

    #endregion NameSpaces

	[ChoStreamProfile("Configuration Errors", Name = ChoReservedFileName.ConfigurationErrors, StartActions = "Truncate")]
    [ChoObjectFactory(ChoObjectConstructionType.Singleton)]
    public class ChoConfigurationErrorsProfiler : ChoContextProfiler
    {
        #region Shared Members (Public)

        public static ChoConfigurationErrorsProfiler Me
        {
            get { return ChoObjectManagementFactory.CreateInstance<ChoConfigurationErrorsProfiler>(); }
        }

        #endregion Shared Members (Public)
    }
}
