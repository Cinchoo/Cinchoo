using System;
using System.Collections.Generic;
using System.Text;

namespace Cinchoo.Core.Shell
{
    /// <summary>
    /// Allows control of command line parsing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ChoPositionalCommandLineArgAttribute : ChoDefaultCommandLineArgAttribute
    {
        #region Public Instance Constructors

        public ChoPositionalCommandLineArgAttribute(int position, string shortName)
        {
            if (position < 1)
                throw new ArgumentOutOfRangeException("Position must be >= 1");
            if (shortName.IsNullOrWhiteSpace())
                throw new ArgumentOutOfRangeException("Missing shortname.");

            Position = position;
            ShortName = shortName;
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties

        public int Position
        {
            get;
            private set;
        }
        
        #endregion Public Instance Properties
   }
}
