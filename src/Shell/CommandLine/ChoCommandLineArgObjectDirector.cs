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

    public sealed class ChoCommandLineArgObjectDirector : ChoLoggableObject
    {
        private const string DefaultCmdLineSwitch = "<default>";

        public static void Load(object target)
        {
            Load(target, Environment.GetCommandLineArgs().Skip(1).ToArray());
        }

        public static void Load(object target, string[] commandLineArgs)
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            ChoCommandLineArgObject commandLineArgObject = target as ChoCommandLineArgObject;

            Exception exception = null;
            if (commandLineArgObject != null)
                commandLineArgObject.CommandLineArgs = commandLineArgs;

            using (ChoCommandLineArgParser commandLineArgParser = new ChoCommandLineArgParser(commandLineArgs))
            {
                commandLineArgParser.UnrecognizedCommandLineArgFound += ((sender, eventArgs) =>
                {
                    if (commandLineArgObject != null)
                        commandLineArgObject.OnUnrecognizedCommandLineArgFound(eventArgs);
                });

                commandLineArgParser.Parse();

                if (commandLineArgParser.IsUsageArgSpecified)
                    throw new ChoCommandLineArgUsageException(GetUsage(target));

                string cmdLineSwitch = null;
                if (commandLineArgObject == null || !commandLineArgObject.OnBeforeCommandLineArgObjectLoaded(commandLineArgs))
                {
                    MemberInfo[] memberInfos = ChoType.GetMemberInfos(target.GetType(), typeof(ChoCommandLineArgAttribute));
                    MemberInfo[] memberInfos1 = ChoType.GetMemberInfos(target.GetType(), typeof(ChoDefaultCommandLineArgAttribute));

                    ChoCommandLineParserSettings commandLineParserSettings = ChoCommandLineParserSettings.Me;
                    memberInfos = memberInfos.Concat(memberInfos1).ToArray();
                    if (memberInfos != null && memberInfos.Length > 0)
                    {
                        foreach (MemberInfo memberInfo in memberInfos)
                        {
                            ChoCommandLineArgAttribute commandLineArgumentAttribute = (ChoCommandLineArgAttribute)memberInfo.GetCustomAttribute<ChoCommandLineArgAttribute>(true);
                            if (commandLineArgumentAttribute == null)
                                continue;

                            cmdLineSwitch = commandLineArgumentAttribute.CommandLineSwitch;
                            bool isSwitchSpecified = IsSwitchSpecified(commandLineArgParser.Switches, cmdLineSwitch, commandLineParserSettings.IgnoreCase);

                            exception = ExtractNPopulateValue(target, memberInfo, cmdLineSwitch, commandLineArgParser, commandLineArgObject, isSwitchSpecified);

                            if (exception != null)
                                break;
                        }

                        if (exception == null)
                        {
                            cmdLineSwitch = DefaultCmdLineSwitch;
                            MemberInfo defaultMemberInfo = GetMemberForDefaultSwitch(memberInfos1);
                            exception = ExtractNPopulateValue(target, defaultMemberInfo, DefaultCmdLineSwitch, commandLineArgParser, commandLineArgObject, false);
                        }
                    }
                }

                if (commandLineArgObject != null)
                {
                    if (exception != null)
                    {
                        if (!commandLineArgObject.OnCommandLineArgObjectLoadError(commandLineArgs, exception))
                            throw new ChoCommandLineArgException("Found exception while loading `{3}` command line argument. {0}{0}{2}{0}{0}{1}".FormatString(
                                Environment.NewLine, GetUsage(target), exception.Message, cmdLineSwitch), exception);
                    }
                    else
                        commandLineArgObject.OnAfterCommandLineArgObjectLoaded(commandLineArgs);
                }
            }
        }

        private static Exception ExtractNPopulateValue(object target, MemberInfo memberInfo, string switchString, ChoCommandLineArgParser commandLineArgParser, 
            ChoCommandLineArgObject commandLineArgObject, bool isSwitchSpecified)
        {
            ChoCommandLineArgAttribute commandLineArgumentAttribute = null;
            ChoDefaultCommandLineArgAttribute defaultCommandLineArgAttribute = null;

            if (memberInfo == null)
            {
                if (commandLineArgObject != null)
                    commandLineArgObject.OnCommandLineArgMemberNotFound(switchString, commandLineArgParser[switchString]);
            }
            else
            {
                //if (ChoType.IsReadOnlyMember(memberInfo))
                //    return null;

                commandLineArgumentAttribute = null;
                defaultCommandLineArgAttribute = null;

                commandLineArgumentAttribute = (ChoCommandLineArgAttribute)memberInfo.GetCustomAttribute<ChoCommandLineArgAttribute>(true);
                if (commandLineArgumentAttribute == null)
                {
                    defaultCommandLineArgAttribute = (ChoDefaultCommandLineArgAttribute)memberInfo.GetCustomAttribute<ChoDefaultCommandLineArgAttribute>(true);
                    if (defaultCommandLineArgAttribute == null)
                        return null;
                }

                string cmdLineArgValue = null;
                object newCmdLineArgValue = null;

                try
                {
                    if (defaultCommandLineArgAttribute != null)
                        cmdLineArgValue = commandLineArgParser.DefaultArgs.Length > 0 ? commandLineArgParser.DefaultArgs[0] : null;
                    else if (commandLineArgumentAttribute != null)
                    {
                        cmdLineArgValue = commandLineArgParser[commandLineArgumentAttribute.CommandLineSwitch];
                        defaultCommandLineArgAttribute = commandLineArgumentAttribute;
                    }
                    else
                        return null;

                    object defaultCmdLineArgValue = defaultCommandLineArgAttribute.DefaultValue;

                    if (isSwitchSpecified && cmdLineArgValue == null && defaultCommandLineArgAttribute.FallbackValue != null)
                        defaultCmdLineArgValue = defaultCommandLineArgAttribute.FallbackValue;

                    if (commandLineArgObject == null || !commandLineArgObject.OnBeforeCommandLineArgLoaded(memberInfo.Name, cmdLineArgValue, defaultCommandLineArgAttribute.DefaultValue, defaultCommandLineArgAttribute.FallbackValue))
                    {
                        if (!cmdLineArgValue.IsNullOrWhiteSpace())
                        {
                            newCmdLineArgValue = ChoConvert.ConvertFrom(target, ChoString.ExpandPropertiesEx(cmdLineArgValue), 
                                ChoType.GetMemberType(memberInfo),
                                ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo));
                        }
                        else if (defaultCmdLineArgValue != null)
                        {
                            if (defaultCmdLineArgValue is string)
                                newCmdLineArgValue = ChoConvert.ConvertFrom(target, ChoString.ExpandPropertiesEx(defaultCmdLineArgValue as string), 
                                    ChoType.GetMemberType(memberInfo),
                                    ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo));
                            else
                                newCmdLineArgValue = ChoConvert.ConvertFrom(target, defaultCmdLineArgValue, ChoType.GetMemberType(memberInfo),
                                    ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo));
                        }

                        if (newCmdLineArgValue != null)
                        {
                            ChoType.SetMemberValue(target, memberInfo, newCmdLineArgValue);
                            if (commandLineArgObject != null)
                                commandLineArgObject.OnAfterCommandLineArgLoaded(memberInfo.Name, newCmdLineArgValue);
                        }
                        else if (defaultCommandLineArgAttribute.IsRequired)
                            throw new ChoCommandLineArgException("Missing arg value for '{0}' required command line switch.".FormatString(
                                commandLineArgumentAttribute == null ? DefaultCmdLineSwitch : commandLineArgumentAttribute.CommandLineSwitch));
                    }
                }
                catch (ChoFatalApplicationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    if (commandLineArgObject != null && commandLineArgObject.OnCommandLineArgLoadError(memberInfo.Name, cmdLineArgValue, ex))
                    {
                    }
                    else
                    {
                        return ex;
                    }
                }
            }

            return null;
        }

        public static string GetUsage(object target)
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            StringBuilder builder = new StringBuilder();
            StringBuilder whereBuilder = new StringBuilder();

            ChoCommandLineParserSettings commandLineParserSettings = ChoCommandLineParserSettings.Me;
            char cmdLineValueSeperator = commandLineParserSettings.ValueSeperators != null && commandLineParserSettings.ValueSeperators.Length > 0 ? commandLineParserSettings.ValueSeperators[0] : ':';
            char cmdLineSwitchChar = commandLineParserSettings.SwitchChars != null && commandLineParserSettings.SwitchChars.Length > 0 ? commandLineParserSettings.SwitchChars[0] : '-';

            builder.Append(ChoApplication.EntryAssemblyFileName);
            MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(target.GetType());

            if (memberInfos != null && memberInfos.Length > 0)
            {
                ChoCommandLineArgAttribute commandLineArgumentAttribute = null;
                ChoDefaultCommandLineArgAttribute defaultCommandLineArgAttribute = null;
                bool isEmptyShortName;

                foreach (MemberInfo memberInfo in memberInfos)
                {
                    //if (ChoType.IsReadOnlyMember(memberInfo))
                    //    continue;

                    commandLineArgumentAttribute = null;
                    defaultCommandLineArgAttribute = null;

                    commandLineArgumentAttribute = (ChoCommandLineArgAttribute)memberInfo.GetCustomAttribute<ChoCommandLineArgAttribute>(true);
                    if (commandLineArgumentAttribute == null)
                    {
                        defaultCommandLineArgAttribute = (ChoDefaultCommandLineArgAttribute)memberInfo.GetCustomAttribute<ChoDefaultCommandLineArgAttribute>(true);
                        if (defaultCommandLineArgAttribute == null)
                            continue;
                    }
                    else
                        defaultCommandLineArgAttribute = commandLineArgumentAttribute;

                    isEmptyShortName = !defaultCommandLineArgAttribute.ShortName.IsNull()
                            && defaultCommandLineArgAttribute.ShortName.Length == 0;

                    if (commandLineArgumentAttribute != null)
                        builder.Append(" {0}{1}{2}".FormatString(cmdLineSwitchChar, commandLineArgumentAttribute.CommandLineSwitch, !isEmptyShortName ? cmdLineValueSeperator.ToString() : String.Empty));
                    else
                        builder.Append(" ");

                    Type memberType = ChoType.GetMemberType(memberInfo);

                    if (defaultCommandLineArgAttribute.IsRequired)
                    {
                        if (!isEmptyShortName)
                            builder.Append("[");
                    }

                    if (commandLineArgumentAttribute != null)
                        whereBuilder.AppendFormat("{1}{2}{4}{3}{0}", Environment.NewLine, cmdLineSwitchChar, commandLineArgumentAttribute.CommandLineSwitch, 
                        commandLineArgumentAttribute.Description.WrapLongLines(commandLineArgumentAttribute.DescriptionFormatLineSize, String.Empty,
                        commandLineArgumentAttribute.DescriptionFormatLineBreakChar, commandLineArgumentAttribute.DescriptionFormatLineNoOfTabs),
                        commandLineArgumentAttribute.SwitchValueSeperator);
                    else
                        whereBuilder.AppendFormat("{3}{2}{1}{0}", Environment.NewLine,
                            defaultCommandLineArgAttribute.Description.WrapLongLines(defaultCommandLineArgAttribute.DescriptionFormatLineSize, String.Empty,
                            defaultCommandLineArgAttribute.DescriptionFormatLineBreakChar, defaultCommandLineArgAttribute.DescriptionFormatLineNoOfTabs),
                            defaultCommandLineArgAttribute.SwitchValueSeperator, DefaultCmdLineSwitch);

                    if (!defaultCommandLineArgAttribute.ShortName.IsNull())
                        builder.Append(defaultCommandLineArgAttribute.ShortName);
                    else
                    {
                        if (memberType == typeof(int))
                            builder.Append("<int>");
                        else if (memberType == typeof(uint))
                            builder.Append("<uint>");
                        else if (memberType == typeof(bool))
                            builder.Append("{True|False}");
                        else if (memberType == typeof(string))
                            builder.Append("<string>");
                        else if (memberType.IsEnum)
                        {
                            builder.Append("{");
                            bool first = true;
                            foreach (FieldInfo field in memberType.GetFields())
                            {
                                if (field.IsStatic)
                                {
                                    if (first)
                                        first = false;
                                    else
                                        builder.Append('|');
                                    builder.Append(field.Name);
                                }
                            }
                            builder.Append('}');
                        }
                        else
                            builder.Append("<Unknown>");
                    }
                    if (defaultCommandLineArgAttribute.IsRequired)
                    {
                        if (!isEmptyShortName)
                            builder.Append("]");
                    }
                }
            }

            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            builder.Append("Where");
            builder.Append(Environment.NewLine);
            builder.Append(whereBuilder.ToString().Indent());
            builder.Append(Environment.NewLine);

            if (target is ChoCommandLineArgObject)
            {
                string additionalUsageText = ((ChoCommandLineArgObject)target).AdditionalUsageText;
                if (!additionalUsageText.IsNullOrWhiteSpace())
                {
                    builder.Append(additionalUsageText);

                    builder.Append(Environment.NewLine);
                    builder.Append(Environment.NewLine);
                }
            }

            return builder.ToString();
        }

        internal object Construct(object target)
        {
            return Construct(target, Environment.GetCommandLineArgs().Skip(1).ToArray());
        }

        internal object Construct(object target, string[] commandLineArgs)
        {
            Load(target, commandLineArgs);
            return target;
        }

        private static bool IsSwitchSpecified(Dictionary<string, string>.KeyCollection switchKeyCollection, string commandLineSwitch, bool ignoreCase)
        {
            if (switchKeyCollection == null)
                return false;

            foreach (string switchString in switchKeyCollection)
            {
                if (String.Compare(commandLineSwitch, switchString, ignoreCase) == 0)
                    return true;
            }

            return false;
        }

        private static MemberInfo GetMemberForSwitch(MemberInfo[] memberInfos, string switchString, bool ignoreCase)
        {
            if (memberInfos.IsNullOrEmpty())
                return null;

            foreach (MemberInfo memberInfo in memberInfos)
            {
                ChoCommandLineArgAttribute commandLineArgumentAttribute = (ChoCommandLineArgAttribute)memberInfo.GetCustomAttribute<ChoCommandLineArgAttribute>(true);

                if (commandLineArgumentAttribute == null)
                    continue;

                if (String.Compare(commandLineArgumentAttribute.CommandLineSwitch, switchString, ignoreCase) == 0)
                    return memberInfo;
            }

            return null;
        }

        private static MemberInfo GetMemberForDefaultSwitch(MemberInfo[] memberInfos)
        {
            if (memberInfos.IsNullOrEmpty())
                return null;

            foreach (MemberInfo memberInfo in memberInfos)
            {
                ChoDefaultCommandLineArgAttribute commandLineArgumentAttribute = (ChoDefaultCommandLineArgAttribute)memberInfo.GetCustomAttribute<ChoDefaultCommandLineArgAttribute>(false);

                if (commandLineArgumentAttribute != null)
                    return memberInfo;
            }

            return null;
        }
    }
}
