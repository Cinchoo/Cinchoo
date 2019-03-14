namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;

    #endregion NameSpaces

    public class ChoUnrecognizedCommandLineArgEventArg : EventArgs
    {
        public readonly string CmdLineArgValue;

        public ChoUnrecognizedCommandLineArgEventArg(string cmdLineArgValue)
        {
            CmdLineArgValue = cmdLineArgValue;
        }
    }

    // Simple command line parsing utility class, provides easy access to command line arguments and switches
    // Valid parameters forms:
    //      {-,/}param{ ,=,:}((",')value(",'))
    // Examples: 
    //      -param1 value1 /param3:"Test-:-work" 
    //      /param4=happy -param5 '--=nice=--'
    public class ChoCommandLineArgParser : IDisposable, IEnumerable
    {
        #region Instance Data Members (Private)

        private readonly ChoCommandLineParserSettings _commandLineParserSettings;
        private readonly string[] _args;
        private readonly Dictionary<string, string> _argsDict;
        private readonly List<string> _unrecognizedArgList = new List<string>();
       
        private bool _isUsageArgSpecified = false;
        private List<string> _defaultArgs = new List<string>();

        #endregion

        #region Events

        public event EventHandler<ChoUnrecognizedCommandLineArgEventArg> UnrecognizedCommandLineArgFound;

        #endregion Events

        #region Constructors

        public ChoCommandLineArgParser()
            : this(Environment.GetCommandLineArgs().Skip(1).ToArray())
        {
        }

        public ChoCommandLineArgParser(string[] args)
            : this(args, new ChoCommandLineParserSettings())
        {
        }

        public ChoCommandLineArgParser(string[] args, ChoCommandLineParserSettings commandLineParserSettings)
        {
            if (commandLineParserSettings == null)
                throw new ArgumentNullException("commandLineParserSettings");

            _commandLineParserSettings = commandLineParserSettings;
            _args = args;

            if (_commandLineParserSettings.IgnoreCase)
                _argsDict = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            else
                _argsDict = new Dictionary<string, string>();
        }

        #endregion

        #region Instance Members (Public)

        public void Parse()
        {
            string[] args = NormalizeCmdLineArgs(_args);

            if (args == null || args.Length == 0)
                return;

            //bool first = true;
            foreach (string arg in args)
            {
                if (!arg.IsNullOrWhiteSpace())
                {
                    //if (first)
                    //{
                    //    if (arg.Contains(ChoApplication.EntryAssemblyLocation)
                    //        || arg.Contains("\"{0}\"".FormatString(ChoApplication.EntryAssemblyLocation))
                    //        || arg.Contains(ChoApplication.AppDomainName)
                    //        || arg.Contains("\"{0}\"".FormatString(ChoApplication.AppDomainName))
                    //    )
                    //        continue;
                    //}

                    string argSwitch = null;
                    string argValue = null;
                    if (ParseSwitchValue(arg, ref argSwitch, ref argValue))
                    {
                        if (IsUsageSwitch(argSwitch))
                        {
                            _isUsageArgSpecified = true;
                        }
                        else if (!_argsDict.ContainsKey(argSwitch))
                            _argsDict.Add(argSwitch, argValue);
                        else
                        {
                            _unrecognizedArgList.Add(arg);
                            OnUnrecognizedCommandLineArgFound(arg);
                        }
                    }
                    else
                    {
                        string defaultArg = arg;
                        if (defaultArg.StartsWith("\"") && defaultArg.EndsWith("\""))
                            defaultArg = arg.Substring(1, arg.Length - 2);

                        _defaultArgs.Add(defaultArg);
                    }
                }
                //first = false;
            }
        }

        #endregion Instance Members (Public)

        #region Instance Members (Private)

        protected void OnUnrecognizedCommandLineArgFound(string cmdLineArgValue)
        {
            UnrecognizedCommandLineArgFound.Raise(this, new ChoUnrecognizedCommandLineArgEventArg(cmdLineArgValue));
        }

        private string[] NormalizeCmdLineArgs(string[] args)
        {
            string argFile = null;
            bool foundFileArg = false;
            foreach (string arg in args)
            {
                if (!arg.IsNullOrWhiteSpace())
                {
                    foundFileArg = false;
                    foreach (string fileArgSwitch in _commandLineParserSettings.FileArgSwitches)
                    {
                        if (arg.StartsWith(fileArgSwitch))
                        {
                            foundFileArg = true;
                            argFile = arg.Substring(fileArgSwitch.Length);
                            break;
                        }
                    }

                    if (foundFileArg)
                    {
                        List<string> newArgs = new List<string>();

                        foreach (string line in System.IO.File.ReadLines(argFile))
                        {
                            if (!line.IsNullOrWhiteSpace())
                            {
                                newArgs.AddRange(line.SplitNTrim(' '));
                            }
                        }
                        if (newArgs.Count > 0)
                            return newArgs.ToArray();
                    }
                }
            }

            return args;
        }

        private bool IsUsageSwitch(string argSwitch)
        {
            if (_commandLineParserSettings.UsageSwitches.IsNullOrEmpty())
                return false;

            foreach (string usageSwitch in _commandLineParserSettings.UsageSwitches)
            {
                if (String.Compare(usageSwitch, argSwitch, _commandLineParserSettings.IgnoreCase) == 0)
                    return true;
            }

            return false;
        }

        private bool ParseSwitchValue(string arg, ref string argSwitch, ref string argValue)
        {
            string restArg;
            if (!IsSwitchKey(arg, out restArg))
                return false;

            int index = restArg.IndexOfAny(_commandLineParserSettings.ValueSeperators);
            if (index < 0)
                argSwitch = restArg;
            else
            {
                argSwitch = restArg.Substring(0, index);
                argValue = restArg.Substring(index + 1);
                if (argValue.IsNullOrWhiteSpace())
                    argValue = null;
                if (argValue != null && argValue.StartsWith("\"") && argValue.EndsWith("\""))
                    argValue = argValue.Substring(1, argValue.Length - 2);
            }

            return true;
        }

        private bool IsSwitchKey(string arg, out string restArg)
        {
            bool containsSwitchChar = false;
            foreach (char switchChar in _commandLineParserSettings.SwitchChars)
            {
                if (switchChar == arg[0])
                {
                    containsSwitchChar = true;
                    break;
                }
            }

            restArg = containsSwitchChar ? arg.Substring(1) : arg;
            return containsSwitchChar;
        }

        private string NormalizeKey(string key)
        {
            bool containsSwitchChar = false;
            foreach (char switchChar in _commandLineParserSettings.SwitchChars)
            {
                if (switchChar == key[0])
                {
                    containsSwitchChar = true;
                    break;
                }
            }

            return containsSwitchChar ? key.Substring(1) : key;
        }

        #endregion Instance Members (Private)

        #region Indexers

        public string this[string argSwitch]
        {
            get
            {
                if (argSwitch == null || argSwitch.Trim().Length == 0) return null;

                argSwitch = NormalizeKey(argSwitch);

                return _argsDict.ContainsKey(argSwitch) ? _argsDict[argSwitch] : null;
            }
            set
            {
                if (argSwitch == null || argSwitch.Trim().Length == 0) throw new NullReferenceException("Switch can't be null.");
                _argsDict[NormalizeKey(argSwitch)] = value;
            }
        }

        #endregion

        #region Instance Members (Public)

        public bool IsUsageArgSpecified
        {
            get { return _isUsageArgSpecified; }
        }

        public bool HasUnrecognizedArgs
        {
            get { return _unrecognizedArgList.Count > 0; }
        }

        public string[] UnrecognizedArgs
        {
            get { return _unrecognizedArgList.ToArray(); }
        }

        public string[] DefaultArgs
        {
            get { return _defaultArgs.ToArray(); }
        }

        public bool Contains(string argSwitch)
        {
            if (argSwitch == null || argSwitch.Trim().Length == 0) return false;

            return _argsDict.ContainsKey(NormalizeKey(argSwitch));
        }

        public Dictionary<string, string>.KeyCollection Switches
        {
            get { return _argsDict.Keys; }
        }

        public Dictionary<string, string>.ValueCollection Values
        {
            get { return _argsDict.Values; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _argsDict.GetEnumerator();
        }

        #endregion
    }
}
