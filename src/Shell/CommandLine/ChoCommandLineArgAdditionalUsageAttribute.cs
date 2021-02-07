using System;
using System.Collections.Generic;
using System.Text;

namespace Cinchoo.Core.Shell
{
    /// <summary>
    /// Allows control of command line parsing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ChoCommandLineArgAdditionalUsageAttribute : Attribute
    {
        #region Public Instance Constructors

        public ChoCommandLineArgAdditionalUsageAttribute(string additionalUsageText)
        {
            AdditionalUsageText = additionalUsageText;
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties

        public string AdditionalUsageText
        {
            get;
            private set;
        }

        #endregion Public Instance Properties
    }
}
