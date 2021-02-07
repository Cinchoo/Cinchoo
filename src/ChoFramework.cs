namespace Cinchoo.Core
{
    #region NameSpaces

    #endregion NameSpaces

    public class ChoFramework
    {
        private readonly string _version;
        private readonly string[] _probePaths;
        internal static bool ShutdownRequested = false;

        internal ChoFramework(string version, string[] probePaths)
        {
            _version = version;
            _probePaths = probePaths;
        }

        public string Version
        {
            get { return _version; }
        }

        public string[] ProbePaths
        {
            get { return _probePaths; }
        }

        public static void Shutdown()
        {
            if (ChoApplication.Host != null)
            {
                if (ChoApplication.ApplicationMode == ChoApplicationMode.Windows
                    || ChoApplication.ApplicationMode == ChoApplicationMode.Console)
                {
                    //if (!ChoApplication.ServiceInstallation)
                        ChoApplication.Host.OnStopService();
                }
            }
            else
                ChoAppDomain.Exit();
        }

        public static void Initialize()
        {
            ChoAppDomain.Initialize();
        }
    }

}
