using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinchoo.Core.Configuration;
using System.Xml.Serialization;
using System.Configuration;

namespace Cinchoo.Core.Compiler
{
    public class ChoCompilerParametersSettings : ConfigurationSection //, IChoInitializable
    {
        #region Shared Data Members (Internal)

        private const string SECTION_NAME = "compilerParametersSettings";
        private static readonly object _padLock = new object();
        private static readonly Lazy<ChoCompilerParametersSettings> _defaultInstance = new Lazy<ChoCompilerParametersSettings>(() =>
        {
            ChoCompilerParametersSettings instance = new ChoCompilerParametersSettings();
            instance.GenerateExecutable = false;
            instance.GenerateInMemory = true;
            instance.IncludeDebugInformation = false;
            instance.WarningLevel = -1;
            instance.RefAssemblies = String.Empty; // "PresentationFramework.dll;WindowsBase.dll;PresentationCore.dll;UIAutomationProvider.dll;Microsoft.VisualStudio.Debugger.Runtime.dll";
            instance.ExcludeRefAssemblies = String.Empty;
            instance.Namespaces = String.Empty;

            return instance;
        });
        private static ChoCompilerParametersSettings _instance = null;

        #endregion Shared Data Members (Internal)

        [ConfigurationProperty("generateExecutable")]
        public bool GenerateExecutable
        {
            get { return (bool)this["generateExecutable"]; }
            set { this["generateExecutable"] = value; }
        }

        [ConfigurationProperty("generateInMemory")]
        public bool GenerateInMemory
        {
            get { return (bool)this["generateInMemory"]; }
            set { this["generateInMemory"] = value; }
        }

        [ConfigurationProperty("includeDebugInformation")]
        public bool IncludeDebugInformation
        {
            get { return (bool)this["includeDebugInformation"]; }
            set { this["includeDebugInformation"] = value; }
        }

        [ConfigurationProperty("treatWarningsAsErrors")]
        public bool TreatWarningsAsErrors
        {
            get { return (bool)this["treatWarningsAsErrors"]; }
            set { this["treatWarningsAsErrors"] = value; }
        }

        [ConfigurationProperty("warningLevel")]
        public int WarningLevel
        {
            get { return (int)this["warningLevel"]; }
            set 
            { 
                if (value <= -1)
                    this["warningLevel"] = -1; 
                else
                    this["warningLevel"] = value; 
            }
        }

        [ConfigurationProperty("refAssemblies")]
        public string RefAssemblies
        {
            get { return (string)this["refAssemblies"]; }
            set { this["refAssemblies"] = value; }
        }

        [ConfigurationProperty("excludeRefAssemblies")]
        public string ExcludeRefAssemblies
        {
            get { return (string)this["excludeRefAssemblies"]; }
            set { this["excludeRefAssemblies"] = value; }
        }

        [ConfigurationProperty("namespaces")]
        public string Namespaces
        {
            get { return (string)this["namespaces"]; }
            set { this["namespaces"] = value; }
        }

        #region Factory Methods

        public static ChoCompilerParametersSettings Me
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_padLock)
                {
                    if (_instance == null)
                        _instance = ChoConfigurationManager.GetSection(SECTION_NAME, _defaultInstance.Value);
                }

                return _instance == null ? _defaultInstance.Value : _instance;
            }
        }

        #endregion Factory Methods

        //public void Initialize()
        //{
        //    if (ExcludeRefAssemblies.IsNullOrWhiteSpace())
        //        ExcludeRefAssemblies = "PresentationFramework.dll;WindowsBase.dll;PresentationCore.dll;UIAutomationProvider.dll;Microsoft.VisualStudio.Debugger.Runtime.dll";
        //    if (WarningLevel <= -1)
        //        WarningLevel = -1;
        //}
    }
}
