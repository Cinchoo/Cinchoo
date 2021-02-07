namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;
    using System.IO;
    using Cinchoo.Core.Reflection;

    #endregion NameSpaces

    public abstract class ChoCommandLineArgBuilder : ChoInterceptableObject
    {
        private static readonly Dictionary<Type, Dictionary<string, Type>> _commandsCache = new Dictionary<Type, Dictionary<string, Type>>();
        private static readonly Dictionary<Type, List<ChoCommandLineArgBuilderCommandAttribute>> _commandsAttrCache = new Dictionary<Type, List<ChoCommandLineArgBuilderCommandAttribute>>();

        private static readonly object _commandsCacheLock = new object();

        #region Instance Data Members (Private)

        [ChoHiddenMember]
        private readonly ChoCommandLineArgBuilderAttribute _commandLineArgsObjectAttribute;

        #endregion Instance Data Members (Private)

        #region Instance Properties (Internal)

        internal string[] CommandLineArgs
        {
            get;
            set;
        }

        #endregion Instance Properties (Internal)

        #region Constructors

        [ChoHiddenMember]
        public ChoCommandLineArgBuilder()
        {
            _commandLineArgsObjectAttribute = GetType().GetCustomAttribute(typeof(ChoCommandLineArgBuilderAttribute)) as ChoCommandLineArgBuilderAttribute;
            if (_commandLineArgsObjectAttribute == null)
                throw new ChoFatalApplicationException("Missing ChoCommandLineArgBuilderAttribute defined for '{0}' type.".FormatString(GetType().Name));
        }
        
        #endregion Constructors

        #region Instance Members (Public)

        public virtual string GetUsage()
        {
            Type type = GetType();

            DiscoverCommands(type);

            StringBuilder builder = new StringBuilder();
            StringBuilder whereBuilder = new StringBuilder();

            builder.Append(Environment.NewLine);
            builder.Append("The syntax of this command is:");
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            builder.Append(Path.GetFileNameWithoutExtension(ChoApplication.EntryAssemblyFileName).ToUpper());

            if (_commandsAttrCache.ContainsKey(type))
            {
                foreach (ChoCommandLineArgBuilderCommandAttribute attr in _commandsAttrCache[type].ToArray())
                {
                    if (whereBuilder.Length == 0)
                        whereBuilder.Append(attr.Command);
                    else
                        whereBuilder.Append(" | {0}".FormatString(attr.Command));
                }
                if (whereBuilder.Length > 0)
                    builder.Append(" [{0}]".FormatString(whereBuilder.ToString()));
            }

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

            return builder.ToString();
        }

        private void DiscoverCommands(Type builderType)
        {
            if (_commandsCache.ContainsKey(builderType)) return;

            lock (_commandsCacheLock)
            {
                if (_commandsCache.ContainsKey(builderType)) return;

                Dictionary<string, Type> commands = new Dictionary<string, Type>();

                ChoCommandLineArgBuilderCommandAttribute[] attrs = null; // ChoType.GetAttributes<ChoCommandLineArgBuilderCommandAttribute>(builderType);
                ConstructorInfo constructorInfo = builderType.GetConstructor(new Type[] {});
                if (constructorInfo != null)
                    attrs = ChoType.GetAttributes<ChoCommandLineArgBuilderCommandAttribute>(constructorInfo);

                if (attrs != null)
                {
                    foreach (ChoCommandLineArgBuilderCommandAttribute attr in attrs)
                        commands.Add(attr.Command, attr.CommandType);
                }

                _commandsCache.Add(builderType, commands);

                List<ChoCommandLineArgBuilderCommandAttribute> attr1 = new List<ChoCommandLineArgBuilderCommandAttribute>(attrs);
                attr1.Sort((x, y) => x.Order.CompareTo(y.Order));
                _commandsAttrCache.Add(builderType, attr1);
            }
        }

        [ChoHiddenMember]
        public void Log(string msg)
        {
            if (_commandLineArgsObjectAttribute == null)
                return;
            _commandLineArgsObjectAttribute.GetMe(GetType()).Log(msg);
        }

        [ChoHiddenMember]
        public void Log(bool condition, string msg)
        {
            if (_commandLineArgsObjectAttribute == null)
                return;
            _commandLineArgsObjectAttribute.GetMe(GetType()).Log(condition, msg);
        }

        #endregion Instance Members (Public)

        #region Object Overrides

        public override string ToString()
        {
            return ChoObject.ToString(this);
        }

        #endregion Object Overrides

        public ChoCommandLineArgObject CommandLineArgObject { get; internal set; }

        public virtual Type GetCommandLineArgObjectType(string command)
        {
            Type builderType = GetType();
            DiscoverCommands(builderType);

            if (!_commandsCache.ContainsKey(builderType))
                return null;

            Dictionary<string, Type> commandTypes = _commandsCache[builderType];
            if (commandTypes == null || !commandTypes.ContainsKey(command))
                return null;

            return commandTypes[command];
        }

        [ChoHiddenMember]
        protected override void PostInvoke(ChoMemberInfo memberInfo)
        {
            if (memberInfo.Exception != null)
            {
                //if (RaiseCommandLineArgLoadError(memberInfo.Name, memberInfo.Value, memberInfo.Exception))
                //    memberInfo.Exception = null;
            }
        }
    }
}
