using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.IO;
using System.Security.Policy;
using System.Threading;
using Cinchoo.Core;
using Cinchoo.Core.Configuration;

namespace System
{
    public static class ChoActionEx
    {
        public static void RunWithIgnoreError(this Action action)
        {
            if (action == null) return;

            try
            {
                action();
            }
            catch
            {
            }
        }

        public static void ExecuteInConstrainedRegion(this Action action)
        {
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                action();
            }
        }

        public static void ExecuteInAppDomain(this Action action, string domainName = null, string configurationFile = null)
        {
            domainName = domainName ?? Guid.NewGuid().ToString();

            var ads = new AppDomainSetup();

            ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            ads.DisallowBindingRedirects = false;

            if (!configurationFile.IsNullOrWhiteSpace())
            {
                if (!Path.IsPathRooted(configurationFile))
                {
                    var baseFile = new FileInfo(ChoConfigurationManager.ApplicationConfigurationFilePath);
                    var directory = baseFile.Directory;
                    var newFile = directory.GetFiles(configurationFile).FirstOrDefault();

                    if (newFile.Exists)
                        configurationFile = newFile.FullName;
                    else
                        throw new ArgumentException("Configuration file " + configurationFile + " does not exist.");
                }

                ads.ConfigurationFile = configurationFile;
            }
            else
                ads.ConfigurationFile = ChoConfigurationManager.ApplicationConfigurationFilePath;

            var baseEvidence = AppDomain.CurrentDomain.Evidence;
            var evidence = new Evidence(baseEvidence);
            var domain = AppDomain.CreateDomain(domainName, evidence, ads);

            try
            {
                var mbrtType = typeof(ChoMarshalByRefType);
                ChoMarshalByRefType mbrt = (ChoMarshalByRefType)domain.CreateInstanceAndUnwrap(mbrtType.Assembly.FullName, mbrtType.FullName);
                mbrt.Do(action);
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }
    }
}