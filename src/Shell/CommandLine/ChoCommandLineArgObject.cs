namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;
    using Cinchoo.Core.Collections;
    using System.IO;
    using Cinchoo.Core.Reflection;

    #endregion NameSpaces

    public abstract class ChoCommandLineArgObject : ChoInterceptableObject
    {
        internal const string DefaultCmdLineSwitch = "<default>";

        #region Instance Data Members (Private)

        [ChoHiddenMember]
        private readonly ChoCommandLineArgObjectAttribute _commandLineArgsObjectAttribute;

        #endregion Instance Data Members (Private)

        #region Instance Properties (Internal)

        public string[] CommandLineArgs;

        #endregion Instance Properties (Internal)

        #region Constructors

        [ChoHiddenMember]
        public ChoCommandLineArgObject()
        {
            _commandLineArgsObjectAttribute = GetType().GetCustomAttribute(typeof(ChoCommandLineArgObjectAttribute)) as ChoCommandLineArgObjectAttribute;
            if (_commandLineArgsObjectAttribute == null)
                throw new ChoFatalApplicationException("Missing ChoCommandLineArgObjectAttribute defined for '{0}' type.".FormatString(GetType().Name));
        }
        
        #endregion Constructors

        #region Instance Members (Public)

        private string GetDefaultValueText(MemberInfo memberInfo)
        {
            string name = null;
            string defaultValue = null;
            bool isDefaultValueSpecified;
            ChoCommandLineArgAttribute defaultCommandLineArgAttribute = null;

            defaultCommandLineArgAttribute = (ChoCommandLineArgAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoCommandLineArgAttribute));
            if (defaultCommandLineArgAttribute == null) return null;

            name = ChoType.GetMemberName(memberInfo);
            if (ChoType.GetMemberType(memberInfo) != typeof(bool))
            {
                isDefaultValueSpecified = ChoCmdLineArgMetaDataManager.TryGetDefaultValue(this, name, defaultCommandLineArgAttribute, out defaultValue);
                if (isDefaultValueSpecified)
                    return defaultValue;
            }

            return null;
        }

        public virtual string GetUsage()
        {
            Type type = GetType();

            StringBuilder builder = new StringBuilder();
            StringBuilder whereBuilder = new StringBuilder();

            ChoCommandLineParserSettings commandLineParserSettings = ChoCommandLineParserSettings.Me;
            char cmdLineValueSeparator = commandLineParserSettings.ValueSeparators != null && commandLineParserSettings.ValueSeparators.Length > 0 ? commandLineParserSettings.ValueSeparators[0] : ':';
            char cmdLineSwitchChar = commandLineParserSettings.SwitchChars != null && commandLineParserSettings.SwitchChars.Length > 0 ? commandLineParserSettings.SwitchChars[0] : '-';

            builder.Append(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location).ToUpper());
            MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(type);

            if (memberInfos != null && memberInfos.Length > 0)
            {
                ChoCommandLineArgAttribute commandLineArgumentAttribute = null;
                ChoDefaultCommandLineArgAttribute defaultCommandLineArgAttribute = null;
                ChoPositionalCommandLineArgAttribute posCommandLineArgAttribute = null;

                List<Tuple<ChoDefaultCommandLineArgAttribute, MemberInfo>> memberList = new List<Tuple<ChoDefaultCommandLineArgAttribute, MemberInfo>>();
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    commandLineArgumentAttribute = null;
                    defaultCommandLineArgAttribute = null;

                    defaultCommandLineArgAttribute = commandLineArgumentAttribute = (ChoCommandLineArgAttribute)memberInfo.GetCustomAttribute<ChoCommandLineArgAttribute>(true);
                    if (commandLineArgumentAttribute == null)
                    {
                        defaultCommandLineArgAttribute = posCommandLineArgAttribute = (ChoPositionalCommandLineArgAttribute)memberInfo.GetCustomAttribute<ChoPositionalCommandLineArgAttribute>(true);
                        if (posCommandLineArgAttribute == null)
                            continue;
                    }

                    memberList.Add(new Tuple<ChoDefaultCommandLineArgAttribute, MemberInfo>(defaultCommandLineArgAttribute, memberInfo));
                }

                bool isEmptyShortName;

                memberList.Sort((x, y) =>
                    x.Item1.Order.CompareTo(y.Item1.Order));

                MemberInfo memberInfo1 = null;
                foreach (Tuple<ChoDefaultCommandLineArgAttribute, MemberInfo> tuple in memberList)
                {
                    memberInfo1 = tuple.Item2;

                    commandLineArgumentAttribute = null;
                    defaultCommandLineArgAttribute = null;

                    defaultCommandLineArgAttribute = commandLineArgumentAttribute = (ChoCommandLineArgAttribute)memberInfo1.GetCustomAttribute<ChoCommandLineArgAttribute>(true);
                    if (commandLineArgumentAttribute == null)
                    {
                        defaultCommandLineArgAttribute = posCommandLineArgAttribute = (ChoPositionalCommandLineArgAttribute)memberInfo1.GetCustomAttribute<ChoPositionalCommandLineArgAttribute>(true);
                        if (posCommandLineArgAttribute == null)
                            continue;
                    }

                    isEmptyShortName = !defaultCommandLineArgAttribute.ShortName.IsNull()
                            && defaultCommandLineArgAttribute.ShortName.Length == 0;

                    if (!defaultCommandLineArgAttribute.IsRequired)
                        builder.Append(" [");
                    else
                        builder.Append(" ");

                    Type memberType = ChoType.GetMemberType(memberInfo1);
                    if (memberType.IsNullableType())
                        memberType = Nullable.GetUnderlyingType(memberType);

                    if (commandLineArgumentAttribute != null)
                        builder.Append("{0}{1}{2}".FormatString(cmdLineSwitchChar, commandLineArgumentAttribute.CommandLineSwitch, !isEmptyShortName ? cmdLineValueSeparator.ToString() : String.Empty));

                    string description = null;
                    if (commandLineArgumentAttribute != null)
                        description = commandLineArgumentAttribute.Description;
                    else if (posCommandLineArgAttribute != null)
                        description = posCommandLineArgAttribute.Description;
                    else
                        description = defaultCommandLineArgAttribute.Description;

                    if (_commandLineArgsObjectAttribute.GetDisplayDefaultValue())
                    {
                        string defaultValue = null;
                        defaultValue = GetDefaultValueText(memberInfo1);
                        if (defaultValue != null && memberType != typeof(bool))
                            description = "{0} [DEFAULT: {1}]".FormatString(description, defaultValue);
                    }

                    if (commandLineArgumentAttribute != null)
                        whereBuilder.AppendFormat("{1}{3}{2}{0}", Environment.NewLine, GetCmdLineSwitches(cmdLineSwitchChar, commandLineArgumentAttribute),
                        description.WrapLongLines(commandLineArgumentAttribute.DescriptionFormatLineSize, String.Empty,
                        commandLineArgumentAttribute.DescriptionFormatLineBreakChar, commandLineArgumentAttribute.NoOfTabsSwitchDescFormatSeparator),
                        "\t".Repeat(commandLineArgumentAttribute.NoOfTabsSwitchDescFormatSeparator));
                    else if (posCommandLineArgAttribute != null)
                        whereBuilder.AppendFormat("{1}{3}{2}{0}", Environment.NewLine,
                            defaultCommandLineArgAttribute.ShortName.IsNull() ? "Position{0}".FormatString(posCommandLineArgAttribute.Position) : defaultCommandLineArgAttribute.ShortName,
                        description.WrapLongLines(posCommandLineArgAttribute.DescriptionFormatLineSize, String.Empty,
                        posCommandLineArgAttribute.DescriptionFormatLineBreakChar, posCommandLineArgAttribute.NoOfTabsSwitchDescFormatSeparator),
                        "\t".Repeat(posCommandLineArgAttribute.NoOfTabsSwitchDescFormatSeparator));
                    else
                        whereBuilder.AppendFormat("{3}{2}{1}{0}", Environment.NewLine,
                            description.WrapLongLines(defaultCommandLineArgAttribute.DescriptionFormatLineSize, String.Empty,
                            defaultCommandLineArgAttribute.DescriptionFormatLineBreakChar, defaultCommandLineArgAttribute.NoOfTabsSwitchDescFormatSeparator),
                            "\t".Repeat(defaultCommandLineArgAttribute.NoOfTabsSwitchDescFormatSeparator), DefaultCmdLineSwitch);

                    if (memberType == typeof(int))
                    {
                        if (!defaultCommandLineArgAttribute.ShortName.IsNull())
                            builder.Append(defaultCommandLineArgAttribute.ShortName);
                        else
                            builder.Append("<int>");
                    }
                    else if (memberType == typeof(uint))
                    {
                        if (!defaultCommandLineArgAttribute.ShortName.IsNull())
                            builder.Append(defaultCommandLineArgAttribute.ShortName);
                        else
                            builder.Append("<uint>");
                    }
                    else if (memberType == typeof(bool))
                        builder.Remove(builder.Length - 1, 1);
                    else if (memberType == typeof(string))
                    {
                        if (!defaultCommandLineArgAttribute.ShortName.IsNull())
                            builder.Append(defaultCommandLineArgAttribute.ShortName);
                        else
                            builder.Append("<string>");
                    }
                    else if (memberType.IsEnum)
                    {
                        //builder.Append("{0}{1}{2}".FormatString(cmdLineSwitchChar, commandLineArgumentAttribute.CommandLineSwitch, !isEmptyShortName ? cmdLineValueSeparator.ToString() : String.Empty));
                        builder.Append("{");
                        bool first = true;
                        foreach (FieldInfo field in memberType.GetFields())
                        {
                            if (field.IsStatic)
                            {
                                if (first)
                                    first = false;
                                else
                                    builder.Append(" | ");

                                builder.Append(field.Name);
                            }
                        }
                        builder.Append('}');
                    }
                    else
                    {
                        if (!defaultCommandLineArgAttribute.ShortName.IsNull())
                            builder.Append(defaultCommandLineArgAttribute.ShortName);
                        else
                            builder.Append("<Unknown>");
                    }

                    if (!defaultCommandLineArgAttribute.IsRequired)
                        builder.Append("]");
                }
            }

            if (!_commandLineArgsObjectAttribute.DoNotShowUsageDetail)
            {
                builder.Append(Environment.NewLine);
                builder.Append(Environment.NewLine);
                builder.Append(whereBuilder.ToString().Indent());
                builder.Append(Environment.NewLine);

                foreach (ChoCommandLineArgAdditionalUsageAttribute commandLineArgAdditionalUsageAttribute in ChoType.GetAttributes<ChoCommandLineArgAdditionalUsageAttribute>(type))
                {
                    if (commandLineArgAdditionalUsageAttribute != null && !commandLineArgAdditionalUsageAttribute.AdditionalUsageText.IsNull())
                    {
                        builder.Append(commandLineArgAdditionalUsageAttribute.AdditionalUsageText);

                        builder.Append(Environment.NewLine);
                        builder.Append(Environment.NewLine);
                    }
                }
            }
            return builder.ToString();
        }

        private string GetCmdLineSwitches(char cmdLineSwitchChar, ChoCommandLineArgAttribute commandLineArgumentAttribute)
        {
            if (commandLineArgumentAttribute.Aliases.IsNullOrWhiteSpace())
                return "{0}{1}".FormatString(cmdLineSwitchChar, commandLineArgumentAttribute.CommandLineSwitch);

            StringBuilder msg = new StringBuilder("{0}{1}".FormatString(cmdLineSwitchChar, commandLineArgumentAttribute.CommandLineSwitch));
            foreach (string alias in commandLineArgumentAttribute.Aliases.SplitNTrim())
            {
                msg.Append(" {0}{1}".FormatString(cmdLineSwitchChar, alias));
            }

            return msg.ToString();
        }

        //[ChoHiddenMember]
        //public void Log(string msg)
        //{
        //    if (_commandLineArgsObjectAttribute == null)
        //        return;
        //    _commandLineArgsObjectAttribute.GetMe(GetType()).Log(msg);
        //}

        //[ChoHiddenMember]
        //public void Log(bool condition, string msg)
        //{
        //    if (_commandLineArgsObjectAttribute == null)
        //        return;
        //    _commandLineArgsObjectAttribute.GetMe(GetType()).Log(condition, msg);
        //}

        #endregion Instance Members (Public)

        #region Object Overrides

        public override string ToString()
        {
            return ChoObject.ToString(this);
        }

        #endregion Object Overrides

        #region Instance Members (Internal)

        protected virtual bool OnBeforeCommandLineArgObjectLoaded(string[] commandLineArgs)
        {
            return false;
        }

        [ChoHiddenMember]
        internal bool RaiseBeforeCommandLineArgObjectLoaded(string[] commandLineArgs)
        {
            if (OnBeforeCommandLineArgObjectLoaded(commandLineArgs))
                return true;

            return false;
        }

        protected virtual void OnAfterCommandLineArgObjectLoaded(string[] commandLineArgs)
        {
        }

        [ChoHiddenMember]
        internal void RaiseAfterCommandLineArgObjectLoaded(string[] commandLineArgs)
        {
            OnAfterCommandLineArgObjectLoaded(commandLineArgs);
        }

        protected virtual bool OnCommandLineArgObjectLoadError(string[] commandLineArgs, Exception ex)
        {
            return false;
        }

        [ChoHiddenMember]
        internal bool RaiseCommandLineArgObjectLoadError(string[] commandLineArgs, Exception ex)
        {
            if (OnCommandLineArgObjectLoadError(commandLineArgs, ex))
                return true;

            return false;
        }

        protected virtual bool OnBeforeCommandLineArgLoaded(string memberName, ref string value, object defaultValue, object fallbackValue)
        {
            return false;
        }

        [ChoHiddenMember]
        internal bool RaiseBeforeCommandLineArgLoaded(string memberName, ref string value, object defaultValue, object fallbackValue)
        {
            if (OnBeforeCommandLineArgLoaded(memberName, ref value, defaultValue, fallbackValue))
                return true;

            return false;
        }

        protected virtual void OnAfterCommandLineArgLoaded(string memberName, object value)
        {
        }

        [ChoHiddenMember]
        internal void RaiseAfterCommandLineArgLoaded(string memberName, object value)
        {
            OnAfterCommandLineArgLoaded(memberName, value);
        }

        protected virtual bool OnCommandLineArgLoadError(string memberName, object value, Exception ex)
        {
            return false;
        }

        [ChoHiddenMember]
        internal bool RaiseCommandLineArgLoadError(string memberName, object value, Exception ex)
        {
            if (OnCommandLineArgLoadError(memberName, value, ex))
                return true;

            return false;
        }

        protected virtual void OnCommandLineArgNotFound(string cmdLineArgSwitch, ref string cmdLineArgValue)
        {
        }

        [ChoHiddenMember]
        internal void RaiseCommandLineArgNotFound(string cmdLineArgSwitch, ref string cmdLineArgValue)
        {
            OnCommandLineArgNotFound(cmdLineArgSwitch, ref cmdLineArgValue);
        }

        //protected virtual void OnUnrecognizedCommandLineArgFound(string cmdLineArgValue)
        //{
        //}

        [ChoHiddenMember]
        internal void RaiseUnrecognizedCommandLineArgFound(ChoUnrecognizedCommandLineArgEventArg eventArgs)
        {
            if (eventArgs != null)
            {
                //OnUnrecognizedCommandLineArgFound(eventArgs.CmdLineArgValue);
            }
        }

        #endregion Instance Members (Internal)

        [ChoHiddenMember]
        protected override void PostInvoke(ChoMemberInfo memberInfo)
        {
            if (memberInfo.Exception != null)
            {
                if (RaiseCommandLineArgLoadError(memberInfo.Name, memberInfo.Value, memberInfo.Exception))
                    memberInfo.Exception = null;
            }
        }
    }
}
