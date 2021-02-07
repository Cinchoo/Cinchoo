using System;
using System.Collections.Generic;
using System.Text;
using Cinchoo.Core;

namespace Cinchoo.Core.Shell
{
    /// <summary>
    /// Allows control of command line parsing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
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

        private string _commandLineSwitch;
        public string CommandLineSwitch
        {
            get { return _commandLineSwitch; }
            private set
            {
                if (value.ContainsWhitespaces())
                    throw new ArgumentException("CommandLineSwitch [{0}] contains whitespaces.".FormatString(value));

                _commandLineSwitch = value;
            }
        }

        private string _aliases;
        public string Aliases
        {
            get { return _aliases; }
            set
            {
                if (!value.IsNull())
                {
                    foreach (string alias in value.SplitNTrim())
                    {
                        if (alias.ContainsWhitespaces())
                            throw new ArgumentException("CommandLineSwitch [{0}] alias contains whitespaces.".FormatString(alias));
                    }
                }

                _aliases = value;
            }
        }

        #endregion Public Instance Properties
   }
}
