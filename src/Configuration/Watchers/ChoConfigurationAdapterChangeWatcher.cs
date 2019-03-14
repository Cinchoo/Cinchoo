namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System;

    #endregion NameSpaces

    public class ChoConfigurationAdapterChangeWatcher : ChoConfigurationChangeWatcher
    {
        #region Instance Data Members (Private)

        private readonly IChoConfigObjectAdapter _configObjectAdapter;

        #endregion Instance Data Members (Private)

        /// <summary>
        /// <para>Initialize a new <see cref="ChoConfigurationAdapterChangeWatcher"/> class with the path to the configuration file and the name of the section</para>
        /// </summary>
        /// <param name="_configFilePath">
        /// <para>The full path to the configuration file.</para>
        /// </param>
        /// <param name="_configurationSectionName">
        /// <para>The name of the configuration section to watch.</para>
        /// </param>
        public ChoConfigurationAdapterChangeWatcher(string configurationSectionName, IChoConfigObjectAdapter configObjectAdapter)
            : base(configurationSectionName)
        {
            ChoGuard.ArgumentNotNullOrEmpty(configObjectAdapter, "ConfigObjectAdapter");

            _configObjectAdapter = configObjectAdapter;
        }

        /// <summary>
        /// <para>Returns the <see cref="DateTime"/> of the last change of the information watched</para>
        /// <para>The information is retrieved using the watched file modification timestamp</para>
        /// </summary>
        /// <returns>The <see cref="DateTime"/> of the last modificaiton, or <code>DateTime.MinValue</code> if the information can't be retrieved</returns>
        protected override DateTime GetCurrentLastWriteTime()
        {
            return _configObjectAdapter != null ? _configObjectAdapter.LastUpdateDateTime : DateTime.MinValue;
        }
    }
}
