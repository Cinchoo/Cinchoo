using System;
using System.Collections.Generic;
using System.Text;

namespace Cinchoo.Core.Shell
{
    /// <summary>
    /// Allows control of command line parsing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ChoCommandLineArgAttribute : ChoDefaultCommandLineArgAttribute
    {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChoCommandLineArgAttribute" /> class
        /// with the specified argument type.
        /// </summary>
        /// <param name="argumentType">Specifies the checking to be done on the argument.</param>
        public ChoCommandLineArgAttribute(string commandLineSwitch)
        {
            ChoGuard.ArgumentNotNullOrEmpty(commandLineSwitch, "CommandLine Switch");

            CommandLineSwitch = commandLineSwitch;
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties

        public string CommandLineSwitch
        {
            get;
            private set;
        }

        #endregion Public Instance Properties
   }
}
