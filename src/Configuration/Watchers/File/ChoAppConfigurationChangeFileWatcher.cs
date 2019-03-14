namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion NameSpaces

	public class ChoAppConfigurationChangeFileWatcher : ChoConfigurationChangeCompositeFileWatcher
	{
		public ChoAppConfigurationChangeFileWatcher(string configSectionName, string configFilePath, string[] includeFileList)
			: base(configSectionName, configFilePath, includeFileList)
		{
			base.StartWatching();
		}

        public override void StartWatching()
        {
        }

        public override void StopWatching()
        {
            ResetWatching();
        }

        public override void RestartWatching()
        {
        }

        protected override void Disposing(bool isDisposing)
        {
            base.StopWatching();
            base.Disposing(isDisposing);
        }

		internal void Reset(string _appConfigPath, string[] _appIncludeConfigFilePaths)
		{
		}
	}
}
