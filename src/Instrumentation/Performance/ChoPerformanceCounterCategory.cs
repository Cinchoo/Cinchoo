namespace Cinchoo.Core.Instrumentation
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    public abstract class ChoPerformanceCounterCategory
    {
        static ChoPerformanceCounterCategory()
        {
            ChoAppDomain.Initialize();
        }

        public ChoPerformanceCounterCategory()
        {
            Type type = GetType();

            bool recreate = ChoPCMetaDataManager.Me.RecreateCounters(type);
            bool selfInstall = ChoPCMetaDataManager.Me.SelfInstallCounters(type);

            // create writable performance counter instances
            if (selfInstall)
            {
                if (ChoPerformanceCounterInstaller.Install(type, recreate))
                    ChoPerformanceCounterFactory.CreateCounters(this);
            }
            else
                ChoPerformanceCounterFactory.CreateCounters(this);
        }
    }

}
