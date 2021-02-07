namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Cinchoo.Core.Configuration;
    using System.Diagnostics;

    #endregion NameSpaces

    public sealed class ChoCommandLineArgObjectDirector //: ChoLoggableObject
    {
        public static void Load(object target)
        {
            Load(target, ChoEnvironment.CommandLineArgs);
        }

        public static void Load(object target, string[] commandLineArgs)
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            ChoCommandLineArgObject commandLineArgObject = target as ChoCommandLineArgObject;
            if (commandLineArgObject == null)
                throw new ChoApplicationException("Target is not ChoCommandLineArgObject.");

            ChoCommandLineArgObjectAttribute commandLineArgumentsObjectAttribute = commandLineArgObject.GetType().GetCustomAttribute(typeof(ChoCommandLineArgObjectAttribute)) as ChoCommandLineArgObjectAttribute;
            if (commandLineArgumentsObjectAttribute == null)
                throw new ChoApplicationException("Missing ChoCommandLineArgObjectAttribute. Must be specified.");

            ChoCommandLineParserSettings commandLineParserSettings = ChoCommandLineParserSettings.Me;

            bool isUsageAvail = !commandLineArgumentsObjectAttribute.Silent;

            bool showUsageIfEmpty = commandLineArgumentsObjectAttribute.GetShowUsageIfEmpty();

            if (commandLineArgs.IsNullOrEmpty() && showUsageIfEmpty)
            {
                if (isUsageAvail)
                    throw new ChoCommandLineArgUsageException(commandLineArgObject.GetUsage());
            }

            Exception exception = null;

            using (ChoCommandLineArgParser commandLineArgParser = new ChoCommandLineArgParser())
            {
                commandLineArgParser.UnrecognizedCommandLineArgFound += ((sender, eventArgs) =>
                {
                    if (commandLineArgObject != null)
                        commandLineArgObject.RaiseUnrecognizedCommandLineArgFound(eventArgs);
                });

                commandLineArgParser.Parse(commandLineArgs);

                if (commandLineArgObject != null)
                    commandLineArgObject.CommandLineArgs = commandLineArgs;

                if ((commandLineArgParser.IsUsageArgSpecified && commandLineArgumentsObjectAttribute.UsageSwitch.IsNullOrWhiteSpace())
                    || (!commandLineArgumentsObjectAttribute.UsageSwitch.IsNullOrWhiteSpace() && HasUsageSwitchSpecified(commandLineArgParser, commandLineArgumentsObjectAttribute))
                    )
                {
                    if (isUsageAvail)
                        throw new ChoCommandLineArgUsageException(commandLineArgObject.GetUsage());
                }

                string cmdLineSwitch = null;
                if (commandLineArgObject == null || !commandLineArgObject.RaiseBeforeCommandLineArgObjectLoaded(commandLineArgs))
                {
                    MemberInfo[] memberInfos = ChoType.GetMemberInfos(target.GetType(), typeof(ChoCommandLineArgAttribute));
                    MemberInfo[] memberInfos1 = ChoType.GetMemberInfos(target.GetType(), typeof(ChoPositionalCommandLineArgAttribute));

                    memberInfos = memberInfos1.Concat(memberInfos).ToArray();
                    if (memberInfos != null && memberInfos.Length > 0)
                    {
                        foreach (MemberInfo memberInfo in memberInfos)
                        {
                            cmdLineSwitch = GetCmdLineSwitch(memberInfo);
                            exception = ExtractNPopulateValue(commandLineArgObject, memberInfo, commandLineArgParser);

                            if (isUsageAvail)
                            {
                                if (exception != null)
                                    break;
                            }
                        }
                    }
                }

                if (commandLineArgObject != null)
                {
                    if (exception != null)
                    {
                        if (!commandLineArgObject.RaiseCommandLineArgObjectLoadError(commandLineArgs, exception))
                        {
                            if (isUsageAvail)
                            {
                                if (exception is ChoCommandLineArgException)
                                    throw exception;
                                else
                                {
                                    throw new ChoCommandLineArgException("Found exception while loading `{2}` command line argument. {0}{0}{1}".FormatString(
                                        Environment.NewLine, exception.Message, cmdLineSwitch), commandLineArgObject.GetUsage(), exception);
                                }
                            }
                        }
                    }
                    else
                        commandLineArgObject.RaiseAfterCommandLineArgObjectLoaded(commandLineArgs);
                }
            }
        }

        private static string GetCmdLineSwitch(MemberInfo memberInfo)
        {
            string cmdLineSwitch = null;
            ChoCommandLineArgAttribute commandLineArgumentAttribute = (ChoCommandLineArgAttribute)memberInfo.GetCustomAttribute<ChoCommandLineArgAttribute>(true);
            if (commandLineArgumentAttribute != null)
                cmdLineSwitch = commandLineArgumentAttribute.CommandLineSwitch;
            else
            {
                ChoPositionalCommandLineArgAttribute posCommandLineArgumentAttribute = (ChoPositionalCommandLineArgAttribute)memberInfo.GetCustomAttribute<ChoPositionalCommandLineArgAttribute>(true);
                if (posCommandLineArgumentAttribute != null)
                    cmdLineSwitch = posCommandLineArgumentAttribute.ShortName;
                if (cmdLineSwitch.IsNullOrWhiteSpace())
                    cmdLineSwitch = posCommandLineArgumentAttribute.Position.ToString();
            }

            return cmdLineSwitch;
        }

        private static bool HasUsageSwitchSpecified(ChoCommandLineArgParser commandLineArgParser, ChoCommandLineArgObjectAttribute commandLineArgumentsObjectAttribute)
        {
            if (commandLineArgumentsObjectAttribute == null
                || commandLineArgumentsObjectAttribute.UsageSwitch.IsNullOrWhiteSpace())
                return false;

            foreach (string usageSwitch in commandLineArgumentsObjectAttribute.UsageSwitch.SplitNTrim())
            {
                if (commandLineArgParser.Contains(usageSwitch))
                    return true;
            }

            return false;
        }

        //private static void AssignToDefaultValues(ChoCommandLineArgObject commandLineArgObject)
        //{
        //    object newCmdLineArgValue = null;

        //    string name = null;
        //    string defaultValue = null;
        //    bool isDefaultValueSpecified;
        //    MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(commandLineArgObject.GetType());
        //    if (memberInfos != null && memberInfos.Length > 0)
        //    {
        //        ChoCommandLineArgAttribute defaultCommandLineArgAttribute = null;
        //        foreach (MemberInfo memberInfo in memberInfos)
        //        {
        //            defaultCommandLineArgAttribute = (ChoCommandLineArgAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoCommandLineArgAttribute));
        //            if (defaultCommandLineArgAttribute == null) continue;

        //            name = ChoType.GetMemberName(memberInfo);
        //            defaultValue = null;
        //            if (ChoType.GetMemberType(memberInfo) == typeof(bool))
        //                continue;

        //            isDefaultValueSpecified = ChoCmdLineArgMetaDataManager.TryGetDefaultValue(commandLineArgObject, name, defaultCommandLineArgAttribute, out defaultValue);
        //            if (!isDefaultValueSpecified)
        //                continue;
        //            try
        //            {
        //                defaultValue = ChoString.ExpandPropertiesEx(defaultValue);
        //                object newConvertedValue = ChoConvert.ConvertFrom(defaultValue, memberInfo, commandLineArgObject);

        //                //object newConvertedValue = ChoConvert.ConvertFrom(commandLineArgObject, defaultValue, ChoType.GetMemberType(memberInfo),
        //                //    ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo));
        //                ChoType.SetMemberValue(commandLineArgObject, memberInfo, newCmdLineArgValue);
        //            }
        //            catch //(Exception ex)
        //            {
        //            }
        //        }
        //    }
        //}

        private static Exception ExtractNPopulateValue(ChoCommandLineArgObject commandLineArgObject, MemberInfo memberInfo, ChoCommandLineArgParser commandLineArgParser)
        {
            ChoDefaultCommandLineArgAttribute defaultCommandLineArgAttribute = null;
            ChoCommandLineArgAttribute commandLineArgumentAttribute = null;
            ChoPositionalCommandLineArgAttribute posCommandLineArgAttribute = null;

            if (ChoType.IsReadOnlyMember(memberInfo))
                return null;

            commandLineArgumentAttribute = null;
            posCommandLineArgAttribute = null;

            defaultCommandLineArgAttribute = commandLineArgumentAttribute = (ChoCommandLineArgAttribute)memberInfo.GetCustomAttribute<ChoCommandLineArgAttribute>(true);
            if (commandLineArgumentAttribute == null)
            {
                defaultCommandLineArgAttribute = posCommandLineArgAttribute = (ChoPositionalCommandLineArgAttribute)memberInfo.GetCustomAttribute<ChoPositionalCommandLineArgAttribute>(true);
                if (posCommandLineArgAttribute == null)
                    return null;
            }

            bool containsCmdLineArg = false;
            string cmdLineArgValue = null;
            object newCmdLineArgValue = null;
            string defaultValue = null;
            bool isDefaultValueSpecified = false;
            bool isFallbackValueSpecified = false;
            string name = null;
            string fallbackValue = null;
            object fallbackValueObj = null;
            object defaultValueObj = null;

            name = ChoType.GetMemberName(memberInfo);

            try
            {
                if (posCommandLineArgAttribute != null)
                {
                    if (!commandLineArgParser.IsSwitchSpecified(posCommandLineArgAttribute.Position))
                        commandLineArgObject.RaiseCommandLineArgNotFound(posCommandLineArgAttribute.Position.ToString(), ref cmdLineArgValue);
                    cmdLineArgValue = commandLineArgParser[posCommandLineArgAttribute.Position];
                }
                else if (commandLineArgumentAttribute != null)
                {
                    if (!commandLineArgParser.IsSwitchSpecified(commandLineArgumentAttribute.CommandLineSwitch))
                        commandLineArgObject.RaiseCommandLineArgNotFound(commandLineArgumentAttribute.CommandLineSwitch, ref cmdLineArgValue);
                    
                    if (ChoType.GetMemberType(memberInfo) == typeof(bool))
                    {
                        containsCmdLineArg = IsSwitchSpecified(commandLineArgParser, commandLineArgumentAttribute.CommandLineSwitch, commandLineArgumentAttribute.Aliases);
                        if (containsCmdLineArg)
                        {
                            cmdLineArgValue = "True";
                            //cmdLineArgValue = GetCmdLineArgValue(commandLineArgParser, commandLineArgumentAttribute.CommandLineSwitch, commandLineArgumentAttribute.Aliases);
                            //if (cmdLineArgValue.IsNullOrWhiteSpace())
                            //    cmdLineArgValue = "True";
                        }
                        else
                        {
                            containsCmdLineArg = IsSwitchSpecified(commandLineArgParser, "{0}-".FormatString(commandLineArgumentAttribute.CommandLineSwitch), commandLineArgumentAttribute.Aliases);
                            if (containsCmdLineArg)
                                cmdLineArgValue = "False";
                        }
                    }
                    //else if (ChoType.GetMemberType(memberInfo).IsEnum)
                    //{
                    //    containsCmdLineArg = IsSwitchSpecified(commandLineArgParser, Enum.GetNames(ChoType.GetMemberType(memberInfo)));
                    //    if (containsCmdLineArg)
                    //        cmdLineArgValue = GetCmdLineArgValue(commandLineArgParser, Enum.GetNames(ChoType.GetMemberType(memberInfo)));
                    //    else
                    //        cmdLineArgValue = GetCmdLineArgValue(commandLineArgParser, commandLineArgumentAttribute.CommandLineSwitch, commandLineArgumentAttribute.Aliases);
                    //}
                    else
                    {
                        cmdLineArgValue = GetCmdLineArgValue(commandLineArgParser, commandLineArgumentAttribute.CommandLineSwitch, commandLineArgumentAttribute.Aliases);
                    }
                }
                else
                    return null;

                //if (ChoType.GetMemberType(memberInfo) != typeof(bool))
                //{
                    isDefaultValueSpecified = ChoCmdLineArgMetaDataManager.TryGetDefaultValue(commandLineArgObject, name, defaultCommandLineArgAttribute, out defaultValue);
                    isFallbackValueSpecified = ChoCmdLineArgMetaDataManager.TryGetFallbackValue(commandLineArgObject, name, defaultCommandLineArgAttribute, out fallbackValue);
                //}

                try
                {
                    if (isFallbackValueSpecified)
                    {
    //                    fallbackValueObj = ChoConvert.ConvertFrom(commandLineArgObject, ChoString.ExpandPropertiesEx(fallbackValue),
    //ChoType.GetMemberType(memberInfo),
    //ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo));

                        fallbackValueObj = ChoConvert.ConvertFrom(ChoString.ExpandPropertiesEx(fallbackValue), memberInfo, commandLineArgObject);
                    }
                }
                catch
                {
                }

                try
                {
                    if (isDefaultValueSpecified)
                    {
                        //defaultValueObj = ChoConvert.ConvertFrom(commandLineArgObject, ChoString.ExpandPropertiesEx(defaultValue), ChoType.GetMemberType(memberInfo),
                        //    ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo));
                        defaultValueObj = ChoConvert.ConvertFrom(ChoString.ExpandPropertiesEx(defaultValue), memberInfo, commandLineArgObject);
                    }
                }
                catch
                {
                }

                if (commandLineArgObject != null && !commandLineArgObject.RaiseBeforeCommandLineArgLoaded(memberInfo.Name, ref cmdLineArgValue, defaultValueObj, fallbackValueObj))
                {
                    if (!cmdLineArgValue.IsNull())
                    {
                        newCmdLineArgValue = ChoConvert.ConvertFrom(ChoString.ExpandPropertiesEx(cmdLineArgValue), memberInfo, commandLineArgObject);

                        //newCmdLineArgValue = ChoConvert.ConvertFrom(commandLineArgObject, ChoString.ExpandPropertiesEx(cmdLineArgValue),
                        //    ChoType.GetMemberType(memberInfo),
                        //    ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo));
                    }

                    if (newCmdLineArgValue == null && defaultCommandLineArgAttribute.IsRequired)
                    {
                        if (ChoType.GetMemberType(memberInfo) != typeof(bool))
                        {
                            if (commandLineArgumentAttribute != null)
                                throw new ChoCommandLineArgException("Missing arg value for '{0}' required command line switch.".FormatString(
                                    commandLineArgumentAttribute == null ? ChoCommandLineArgObject.DefaultCmdLineSwitch : commandLineArgumentAttribute.CommandLineSwitch),
                                    commandLineArgObject.GetUsage());
                            else if (posCommandLineArgAttribute != null)
                            {
                                if (posCommandLineArgAttribute.ShortName.IsNull())
                                    throw new ChoCommandLineArgException("Missing positional arg value at '{0}' position.".FormatString(
                                        posCommandLineArgAttribute == null ? ChoCommandLineArgObject.DefaultCmdLineSwitch : posCommandLineArgAttribute.Position.ToString()), commandLineArgObject.GetUsage());
                                else
                                    throw new ChoCommandLineArgException("Missing '{0}' argument.".FormatString(posCommandLineArgAttribute.ShortName), commandLineArgObject.GetUsage());
                            }
                            else
                                throw new ChoCommandLineArgException("Missing arg value at '{0}' position.".FormatString(ChoCommandLineArgObject.DefaultCmdLineSwitch), commandLineArgObject.GetUsage());
                        }
                    }
                    else
                    {
                        if (newCmdLineArgValue == null)
                        {
                            if (isDefaultValueSpecified)
                            {
                                //if (ChoType.GetMemberType(memberInfo) != typeof(bool))
                                newCmdLineArgValue = defaultValueObj;
                                //else
                                //    newCmdLineArgValue = false;
                            }
                        }

                        ChoType.SetMemberValue(commandLineArgObject, memberInfo, newCmdLineArgValue);
                        if (commandLineArgObject != null)
                            commandLineArgObject.RaiseAfterCommandLineArgLoaded(memberInfo.Name, newCmdLineArgValue);
                    }
                }
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (commandLineArgObject != null && commandLineArgObject.RaiseCommandLineArgLoadError(memberInfo.Name, cmdLineArgValue, ex))
                {
                }
                else
                {
                    if (defaultCommandLineArgAttribute.IsRequired)
                        return ex;

                    if (fallbackValueObj != null)
                        ChoType.SetMemberValue(commandLineArgObject, memberInfo, fallbackValueObj);
                    else
                        return ex;
                }
            }
            return null;
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

        private static bool IsSwitchSpecified(ChoCommandLineArgParser commandLineArgParser, string[] commandLineSwitches)
        {
            return commandLineArgParser.IsSwitchSpecified(commandLineSwitches);
        }

        private static bool IsSwitchSpecified(ChoCommandLineArgParser commandLineArgParser, string commandLineSwitch, string aliases)
        {
            return commandLineArgParser.IsSwitchSpecified(commandLineSwitch, aliases);
        }

        private static bool IsSwitchSpecified(ChoCommandLineArgParser commandLineArgParser, int cmdLineParamPos = 0)
        {
            return commandLineArgParser.IsSwitchSpecified(cmdLineParamPos);
        }

        private static string GetCmdLineArgValue(ChoCommandLineArgParser commandLineArgParser, string[] switches)
        {
            foreach (string alias in switches)
            {
                if (commandLineArgParser.Contains(alias))
                    return alias;
            }

            return null;
        }

        private static string GetCmdLineArgValue(ChoCommandLineArgParser commandLineArgParser, string switchString, string aliases)
        {
            if (commandLineArgParser.Contains(switchString))
                return commandLineArgParser[switchString];

            if (!aliases.IsNullOrWhiteSpace())
            {
                foreach (string alias in aliases.SplitNTrim())
                {
                    if (commandLineArgParser.Contains(alias))
                        return commandLineArgParser[alias];
                }
            }

            return null;
        }
    }
}
