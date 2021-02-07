namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Cinchoo.Core.Configuration;

    #endregion NameSpaces

    public sealed class ChoCommandLineArgBuilderDirector : ChoLoggableObject
    {
        private const string DefaultCmdLineSwitch = "<default>";

        public static void Load(object target)
        {
            Load(target, ChoEnvironment.CommandLineArgs);
        }

        public static void Load(object target, string[] commandLineArgs)
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            ChoCommandLineArgBuilder commandLineArgBuilder = target as ChoCommandLineArgBuilder;
            if (commandLineArgBuilder == null)
                throw new ChoApplicationException("Target is not ChoCommandLineArgBuilder.");

            using (ChoCommandLineArgParser commandLineArgParser = new ChoCommandLineArgParser())
            {
                //commandLineArgParser.UnrecognizedCommandLineArgFound += ((sender, eventArgs) =>
                //{
                //    if (commandLineArgBuilder != null)
                //        commandLineArgBuilder.RaiseUnrecognizedCommandLineArgFound(eventArgs);
                //});

                commandLineArgParser.Parse(commandLineArgs);
                if (commandLineArgBuilder != null)
                    ChoEnvironment.CommandLineArgs = ChoEnvironment.CommandLineArgs.Skip(1).ToArray();
                
                bool isUsageAvail = true;
                ChoCommandLineArgObjectAttribute commandLineArgumentsObjectAttribute = commandLineArgBuilder.GetType().GetCustomAttribute(typeof(ChoCommandLineArgObjectAttribute)) as ChoCommandLineArgObjectAttribute;
                if (commandLineArgumentsObjectAttribute != null)
                    isUsageAvail = !commandLineArgumentsObjectAttribute.Silent;

                if (commandLineArgParser.PosArgs.Length == 0 && commandLineArgParser.Switches.Count == 0)
                {
                    if (isUsageAvail)
                        throw new ChoCommandLineArgUsageException(commandLineArgBuilder.GetUsage());
                }

                if (commandLineArgParser.PosArgs.Length == 0 && commandLineArgParser.IsUsageArgSpecified)
                {
                    if (isUsageAvail)
                        throw new ChoCommandLineArgUsageException(commandLineArgBuilder.GetUsage());
                }

                string command = commandLineArgParser.PosArgs[0];
                if (command.IsNullOrWhiteSpace())
                {
                    if (isUsageAvail)
                        throw new ChoCommandLineArgUsageException(commandLineArgBuilder.GetUsage());
                }

                Type commandLineArgObjectType = commandLineArgBuilder.GetCommandLineArgObjectType(command);

                if (commandLineArgObjectType == null)
                {
                    if (isUsageAvail)
                        throw new ChoCommandLineArgUsageException("Command '{1}' not found.{0}{0}{2}".FormatString(Environment.NewLine, command, commandLineArgBuilder.GetUsage()));
                }

                try
                {
                    ChoCommandLineArgObject argObj = ChoActivator.CreateInstance(commandLineArgObjectType) as ChoCommandLineArgObject;
                    commandLineArgBuilder.CommandLineArgObject = argObj;
                }
                catch (ChoCommandLineArgUsageException uEx)
                {
                    throw new ChoCommandLineArgUsageException(uEx.Message.Insert(uEx.Message.IndexOf(' '), " " + command));
                }
                catch (ChoCommandLineArgException aEx)
                {
                    throw new ChoCommandLineArgUsageException(
                        "{1}{0}{0}{2}".FormatString(Environment.NewLine, aEx.ErrorMessage, aEx.UsageMessage.Insert(aEx.UsageMessage.IndexOf(' '), " " + command)));
                }
            }
        }
    
        internal object Construct(object target)
        {
            return Construct(target, ChoEnvironment.CommandLineArgs);
        }

        internal object Construct(object target, string[] commandLineArgs)
        {
            Load(target, commandLineArgs);
            return target;
        }
    }
}
