namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Diagnostics;
    using System.Reflection;
    using Cinchoo.Core;
    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Threading;
    using System.Collections.Generic;

    #endregion NameSpaces

    [ChoAppDomainEventsRegisterableType]
    public class ChoProfile : ChoProfileContainer, IChoProfile, IChoTrace
	{
		#region Readonly members

        internal const string GLOBAL_PROFILE_NAME = "GLOBAL";
        internal const string NULL_PROFILE_NAME = "NULL_PROFILE";
        internal const string DEFAULT_PROFILE_NAME = "__DUMMY_PROFILE__";
        internal const string CURRENT_CONTEXT_PROFILE = "__CONTEXT_PROFILE__";

        public static readonly IChoProfile Default = new ChoStreamProfile(ChoProfile.GLOBAL_PROFILE_NAME, String.Empty, null);
        public static readonly IChoProfile NULL = new ChoStreamProfile(ChoProfile.NULL_PROFILE_NAME, String.Empty, null);

		#endregion ReadOnly members

        #region Shared Data Members (Private)

		//private static readonly List<ChoDictionary<string, IChoProfile>> _allProfilesDict = new List<ChoDictionary<string, IChoProfile>>();

		//[ThreadStatic]
		private static ChoDictionary<string, IChoProfile> MemberProfileCache = ChoDictionary<string, IChoProfile>.Synchronized(new ChoDictionary<string, IChoProfile>());

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Private)

        private int _indent = 0;
		//private bool IsDisposed = false;
        private DateTime _startTime = DateTime.Now;
        private string _name = ChoRandom.NextRandom().ToString();
        private bool _condition = ChoTraceSwitch.Switch.TraceVerbose;

        #endregion

        #region Constructors

		static ChoProfile()
		{
			//ChoStreamProfileAttribute memberProfileAttribute = new ChoStreamProfileAttribute(ChoTraceSwitch.Switch.TraceVerbose, "Application Logs...");
			//memberProfileAttribute.Name = ChoApplicationSettings.Me.ApplicationId;
			//TODO:
			//memberProfileAttribute.Directory = null;
			//memberProfileAttribute.FileName = ChoPath.AddExtension(ChoApplicationSettings.Me.ApplicationId);
			//_globalProfile = memberProfileAttribute.ConstructProfile(null, null);
			//SetAsNotDisposed(_globalProfile, false);
			//_globalProfile = new ChoStreamProfile("Application Logs...");
            //MemberProfileCache.Add(GLOBAL_PROFILE_NAME, ChoStreamProfile.GlobalProfile);
		}

        public ChoProfile(string msg)
            : this(ChoTraceSwitch.Switch.TraceVerbose, msg)
        {
        }

        public ChoProfile(string msg, ChoProfile outerProfile)
            : this(ChoTraceSwitch.Switch.TraceVerbose, msg, outerProfile)
        {
        }

        public ChoProfile(bool condition, string msg)
            : this(condition, msg, null)
        {
        }

        public ChoProfile(bool condition, string msg, ChoProfile outerProfile)
        {
            _condition = condition;
            if (outerProfile != null)
                _indent = outerProfile._indent + 1;
            if (String.IsNullOrEmpty(msg))
                msg = GetDefaultMsg(ChoStackTrace.GetStackFrame());

            if (_condition)
            {
                if (ChoTraceSettings.Me.IndentProfiling)
                    ChoTrace.WriteLineIf(_condition, ChoStackTrace.GetStackFrame(typeof(ChoProfile)), String.Format("{0} {{", msg).Indent(_indent));
                else
                    ChoTrace.WriteLineIf(_condition, ChoStackTrace.GetStackFrame(typeof(ChoProfile)), String.Format("{0} {{", msg));
            }

			//if (outerProfile is ChoProfileContainer)
			//    ((ChoProfileContainer)outerProfile).Add(this);
        }

        #endregion

		//#region IDisposable Members (Public)

		//public void Dispose()
		//{
		//    Dispose(false);
		//}

		//#endregion

		//#region Finalizaer

		//~ChoProfile()
		//{
		//    Dispose(true);
		//}

		//#endregion

		//#region ChoProfileContainer Overrides

		//internal override IChoProfile OuterProfile
		//{
		//    get { return null; }
		//    set { }
		//}

		//#endregion ChoProfileContainer Overrides

        #region IChoProfile Members (Public)

        public string ProfilerName
        {
            get { return _name; }
        }

        public string AppendLine(string msg)
        {
            if (msg == null) return msg;
            return AppendLineIf(_condition, msg);
        }

        public string AppendLineIf(bool condition, string msg)
        {
            if (msg == null) return msg;
            return AppendIf(condition, msg + Environment.NewLine);
        }

        public string Append(string msg)
        {
            return AppendIf(_condition, msg);
        }

        public string AppendIf(bool condition, string msg)
        {
            if (msg == null || !condition) return msg;

            _Write(msg);
            return msg;
        }

        public void AppendIf(bool condition, Exception ex)
        {
            if (condition) AppendLine(ChoApplicationException.ToString(ex));
        }

        public void Append(Exception ex)
        {
            AppendIf(_condition, ex);
        }

        public string AppendIf(bool condition, string format, params object[] args)
        {
            return AppendIf(condition, String.Format(format, args));
        }

        public string Append(string format, params object[] args)
        {
            return Append(String.Format(format, args));
        }

        public string AppendLineIf(bool condition, string format, params object[] args)
        {
            return AppendLineIf(condition, String.Format(format, args));
        }

        public string AppendLine(string format, params object[] args)
        {
            return AppendLine(String.Format(format, args));
        }

		public TimeSpan ElapsedTimeTaken
        {
            get { return DateTime.Now - _startTime; }
        }

        public int Indent
        {
            get { return _indent; }
        }

        #endregion IChoProfile Members (Public)

        #region Instance Members (Private)

        protected override void Dispose(bool finalize)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                Clear();

                if (_condition)
                {
                    if (ChoTraceSettings.Me.IndentProfiling)
                        ChoTrace.WriteLineIf(_condition, ChoStackTrace.GetStackFrame(typeof(ChoProfile)), String.Format("}} [{0}] <---", Convert.ToString(DateTime.Now - _startTime)).Indent(_indent));
                    else
                        ChoTrace.WriteLineIf(_condition, ChoStackTrace.GetStackFrame(typeof(ChoProfile)), String.Format("}} [{0}] <---", Convert.ToString(DateTime.Now - _startTime)));
                }

                if (!finalize)
                    GC.SuppressFinalize(this);
            }
        }

        private void _WriteLine(string msg)
        {
            _WriteLineIf(_condition, msg);
        }

        private void _WriteLineIf(bool condition, string msg)
        {
            if (ChoTraceSettings.Me.IndentProfiling)
                msg = msg.Indent(_indent + 1);

            ChoTrace.WriteLineIf(condition, ChoStackTrace.GetStackFrame(typeof(ChoProfile)), msg);
        }

        private void _Write(string msg)
        {
            _WriteIf(_condition, msg);
        }

        private void _WriteIf(bool condition, string msg)
        {
            if (!condition) return;

            if (ChoTraceSettings.Me.IndentProfiling)
                msg = msg.Indent(_indent + 1);

            ChoTrace.WriteIf(condition, ChoStackTrace.GetStackFrame(typeof(ChoProfile)), msg);
        }

        #endregion Instance Members (Private)

        #region IChoTrace Members

        public void Debug(object message)
        {
            if (message != null)
                AppendLineIf(ChoTraceSwitch.Switch.TraceVerbose, message.ToString());
        }

        public void Debug(Exception exception)
        {
            AppendIf(ChoTraceSwitch.Switch.TraceVerbose, exception);
        }

        public void Debug(object message, Exception exception)
        {
            Debug(message);
            Debug(exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            AppendLineIf(ChoTraceSwitch.Switch.TraceVerbose, String.Format(format, args));
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            AppendLineIf(ChoTraceSwitch.Switch.TraceVerbose, String.Format(provider, format, args));
        }

        public void Error(object message)
        {
            if (message != null)
                AppendLineIf(ChoTraceSwitch.Switch.TraceError, message.ToString());
        }

        public void Error(Exception exception)
        {
            AppendIf(ChoTraceSwitch.Switch.TraceError, exception);
        }

        public void Error(object message, Exception exception)
        {
            Error(message);
            Error(exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            AppendLineIf(ChoTraceSwitch.Switch.TraceError, String.Format(format, args));
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            AppendLineIf(ChoTraceSwitch.Switch.TraceError, String.Format(provider, format, args));
        }

        public void Info(object message)
        {
            if (message != null)
                AppendLineIf(ChoTraceSwitch.Switch.TraceInfo, message.ToString());
        }

        public void Info(Exception exception)
        {
            AppendIf(ChoTraceSwitch.Switch.TraceInfo, exception);
        }

        public void Info(object message, Exception exception)
        {
            Info(message);
            Info(exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            AppendLineIf(ChoTraceSwitch.Switch.TraceInfo, String.Format(format, args));
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            AppendLineIf(ChoTraceSwitch.Switch.TraceInfo, String.Format(provider, format, args));
        }

        public void Warn(object message)
        {
            if (message != null)
                AppendLineIf(ChoTraceSwitch.Switch.TraceWarning, message.ToString());
        }

        public void Warn(Exception exception)
        {
            AppendIf(ChoTraceSwitch.Switch.TraceWarning, exception);
        }

        public void Warn(object message, Exception exception)
        {
            Warn(message);
            Warn(exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            AppendLineIf(ChoTraceSwitch.Switch.TraceWarning, String.Format(format, args));
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            AppendLineIf(ChoTraceSwitch.Switch.TraceWarning, String.Format(provider, format, args));
        }

        #endregion

		#region Shared Members (Private)

        private static readonly object _profileCacheLock = new object();
        private static readonly ChoConcurrentDictionary<string, ChoBaseProfile> _profileCache = new ChoConcurrentDictionary<string, ChoBaseProfile>();
        [ThreadStatic]
        private static Stack<ChoBaseProfile> _profileStack = null;

        internal static void Register(ChoBaseProfile profile)
        {
            if (profile == null || profile.ProfilerName.IsNullOrWhiteSpace()) return;
            if (profile.ProfilerName == ChoProfile.GLOBAL_PROFILE_NAME ||
                profile.ProfilerName == ChoProfile.NULL_PROFILE_NAME) return;

            lock (_profileCacheLock)
            {
                if (!_profileCache.ContainsKey(profile.ProfilerName))
                    _profileCache.AddOrUpdate(profile.ProfilerName, profile, (k, v) => v);

                if (_profileStack == null)
                    _profileStack = new Stack<ChoBaseProfile>();

                _profileStack.Push(profile);
                _contextProfile = profile;
                if (_contextProfile == null)
                    _contextProfile = ChoProfile.Default;
            }

        }

        internal static void Unregister(ChoBaseProfile profile)
        {
            if (profile == null || profile.ProfilerName.IsNullOrWhiteSpace()) return;
            if (profile.ProfilerName == ChoProfile.GLOBAL_PROFILE_NAME ||
                profile.ProfilerName == ChoProfile.NULL_PROFILE_NAME) return;

            lock (_profileCacheLock)
            {
                ChoBaseProfile x = null;
                if (_profileCache.ContainsKey(profile.ProfilerName))
                    _profileCache.TryRemove(profile.ProfilerName, out x);

                if (_profileStack != null)
                {
                    if (_profileStack.Count > 0)
                        _profileStack.Pop();

                    if (_profileStack.Count > 0)
                        _contextProfile = _profileStack.Peek();
                    else
                        _contextProfile = null;
                }

                if (_contextProfile == null)
                    _contextProfile = ChoProfile.Default;
            }
        }

        [ThreadStatic]
        private static IChoProfile _contextProfile = null;
        [ThreadStatic]
        private static ChoProfileAttribute _profileAttribute = null;
        
        internal static IChoProfile GetDefaultContext(StackFrame frame)
        {
            IChoProfile contextProfile = GetContext(frame);
            return contextProfile == null ? ChoProfile.Default : contextProfile;
        }

        internal static IChoProfile GetContext(StackFrame frame)
        {
            string name = null;
            MemberInfo method = frame.GetMethod();
            if (method != null)
            {
                name = method.Name;
                ChoProfileAttribute profileAttribute = ChoType.GetMemberAttributeByBaseType<ChoProfileAttribute>(method);
                if (profileAttribute == null)
                {
                    var type = method.DeclaringType;
                    name = type.Name;
                    profileAttribute = ChoType.GetAttribute(type, typeof(ChoProfileAttribute)) as ChoProfileAttribute;
                }
                if (profileAttribute != null)
                {
                    if (_profileAttribute == profileAttribute)
                        return _contextProfile;
                    else
                        _profileAttribute = profileAttribute;

                    if (profileAttribute.Name.IsNullOrWhiteSpace())
                        profileAttribute.Name = name;

                    if (profileAttribute.OuterProfileName.IsNullOrWhiteSpace())
                        _contextProfile = profileAttribute.ConstructProfile(null, null);
                    else
                        _contextProfile = profileAttribute.ConstructProfile(null, GetProfile(profileAttribute.OuterProfileName));
                }
            }

            return _contextProfile;
        }

        public static IChoProfile DefaultContext
        {
            get { return GetDefaultContext(new StackFrame(1)); }
        }

        public static bool TryGetProfile(string name, ref IChoProfile profile, Func<ChoBaseProfile> profileConstructFactoryMethod)
        {
            if (name.IsNullOrWhiteSpace()) return false;

            if (profileConstructFactoryMethod != null)
            {
                if (!_profileCache.ContainsKey(name))
                {
                    lock (_profileCacheLock)
                    {
                        if (!_profileCache.ContainsKey(name))
                            _profileCache.AddOrUpdate(name, profileConstructFactoryMethod(), (k, v) => v);
                    }
                }
            }

            if (_profileCache.ContainsKey(name))
            {
                profile = _profileCache[name];
                return true;
            }

            return false;
        }

        public static IChoProfile GetProfile(string name)
        {
            if (name == ChoProfile.GLOBAL_PROFILE_NAME) return ChoProfile.Default;
            if (name == ChoProfile.NULL_PROFILE_NAME) return ChoProfile.NULL;
            if (name.IsNullOrWhiteSpace()) return ChoProfile.Default;

            if (_profileCache.ContainsKey(name))
                return _profileCache[name];
            else if (name == CURRENT_CONTEXT_PROFILE)
                return _contextProfile == null ? ChoProfile.Default : _contextProfile;
            else 
                return ChoProfile.Default;
        }

		//private static void SetAsNotDisposed(IChoProfile profile, bool dispose)
		//{
		//    if (profile == null)
		//        return;

		//    if (profile is ChoBufferProfile)
		//        ((ChoBufferProfile)profile).CanDispose = dispose;
		//    else if (profile is ChoStreamProfile)
		//        ((ChoStreamProfile)profile).CanDispose = dispose;
		//}

		private static string GetProfileName(string name, out MemberInfo memberInfo, out string typeProfileFileName,
			out ChoProfileAttribute memberProfileAttribute, out ChoProfileAttribute typeProfileAttribute)
		{
			typeProfileFileName = null;

			StackFrame stackFrame = ChoStackTrace.GetStackFrame(typeof(ChoProfile).Namespace);
			memberInfo = stackFrame.GetMethod();

			memberProfileAttribute = null;
			foreach (ChoProfileAttribute profileAttribute in ChoType.GetMemberAttributesByBaseType<ChoProfileAttribute>(memberInfo))
			{
				if (profileAttribute.Name == name)
				{
					memberProfileAttribute = profileAttribute;
					break;
				}
			}

			ChoProfileAttribute emptyTypeProfileAttribute = null;
			typeProfileAttribute = null;
			foreach (ChoProfileAttribute profileAttribute in ChoType.GetAttributes<ChoProfileAttribute>(memberInfo.ReflectedType))
			{
				if (String.IsNullOrEmpty(profileAttribute.Name))
					emptyTypeProfileAttribute = profileAttribute;
				if (profileAttribute.Name == name)
				{
					typeProfileAttribute = profileAttribute;
					break;
				}
			}

			if (typeProfileAttribute == null)
			{
				if (emptyTypeProfileAttribute == null)
					typeProfileFileName = GLOBAL_PROFILE_NAME;
				else
					typeProfileFileName = "{0}_{1}_{2}_{3}".FormatString(name.IsNullOrEmpty() ? "Default" : name, "Type", ChoThreadLocalStorage.Target == null ? 0 : ChoThreadLocalStorage.Target.GetHashCode(),
						ChoPropertyManager.ExpandProperties(ChoThreadLocalStorage.Target, emptyTypeProfileAttribute.Name));
			}
			else
				typeProfileFileName = "{0}_{1}_{2}_{3}".FormatString(name.IsNullOrEmpty() ? "Default" : name, "Type", ChoThreadLocalStorage.Target == null ? 0 : ChoThreadLocalStorage.Target.GetHashCode(),
					ChoPropertyManager.ExpandProperties(ChoThreadLocalStorage.Target, typeProfileAttribute.Name));

			if (memberProfileAttribute != null)
				return "{0}_{1}_{2}_{3}".FormatString(name.IsNullOrEmpty() ? "Default" : name, memberInfo.Name, ChoThreadLocalStorage.Target == null ? 0 : ChoThreadLocalStorage.Target.GetHashCode(),
					ChoPropertyManager.ExpandProperties(ChoThreadLocalStorage.Target, memberProfileAttribute.Name));
			else
				return typeProfileFileName;
		}

		private static IChoProfile Register(string name, string profileName, MemberInfo memberInfo, string typeProfileName,
			ChoProfileAttribute memberProfileAttribute, ChoProfileAttribute typeProfileAttribute)
		{
			lock (MemberProfileCache.SyncRoot)
			{
				IChoProfile profile = null;
				if (MemberProfileCache.TryGetValue(profileName, out profile))
					return profile;

				if (!String.IsNullOrEmpty(typeProfileName) && !MemberProfileCache.ContainsKey(typeProfileName))
				{
					if (typeProfileAttribute != null)
					{
						IChoProfile profile1 = typeProfileAttribute.ConstructProfile(ChoThreadLocalStorage.Target, null);
						//SetAsNotDisposed(profile1, false);
						MemberProfileCache.Add(typeProfileName, profile1);
					}
					else
						MemberProfileCache.Add(typeProfileName, Default);
				}

				if (memberProfileAttribute == null)
					return MemberProfileCache[typeProfileName];
				else
				{
					IChoProfile profile1 = memberProfileAttribute.ConstructProfile(ChoThreadLocalStorage.Target, MemberProfileCache[typeProfileName]);
					//SetAsNotDisposed(profile1, false);
					MemberProfileCache.Add(profileName, profile1);
					return MemberProfileCache[profileName];
				}
			}
		}

		#endregion Shared Members (Private)

        #region Shared Properties (Public)

		[ThreadStatic]
		private static string _contextName = null;

		internal static void ResetContext()
		{
			_contextName = null;
		}

		public static ChoContextProfiler UsingContext(object target, string name)
		{
			_contextName = name;
			return new ChoContextProfiler(target);
		}

		public static IChoProfile GetContext(object target, string name)
		{
			ChoThreadLocalStorage.Register(target);
			return GetContext(name);
		}

        public static IChoProfile GetContext(string name)
        {
			string typeProfileName = null;
			MemberInfo memberInfo = null;
			ChoProfileAttribute memberAttribute = null;
			ChoProfileAttribute typeAttribute = null;

			string profileName = GetProfileName(name, out memberInfo, out typeProfileName, out memberAttribute, out typeAttribute);

			IChoProfile profile = null;
			if (MemberProfileCache.TryGetValue(profileName, out profile))
				return profile;

			return Register(name, profileName, memberInfo, typeProfileName, memberAttribute, typeAttribute);
        }

		public static IChoProfile GetDefaultContext(object target)
		{
			ChoThreadLocalStorage.Register(target);
			//return DefaultContext;
            return DefaultContext;
        }

        //public static IChoProfile DefaultContext
        //{
        //    get { return GetContext(_contextName); }
        //}

        #endregion Shared Properties (Public)

        #region Shared Members (Internal)

        internal static string GetDefaultMsg(StackFrame stackFrame)
        {
            if (stackFrame.GetMethod().Name == ".ctor" || stackFrame.GetMethod().Name == ".cctor")
                return String.Format("Elapsed time taken by `{0}` caller...", stackFrame.GetMethod().ReflectedType.FullName);
            else
                return String.Format("Elapsed time taken by `{0}[{1}]` caller...", stackFrame.GetMethod().ToString(), stackFrame.GetMethod().ReflectedType.FullName);
        }

        internal static void Register(string name, IChoProfile profile, StackFrame stackFrame, bool enableCallTrace)
        {
			//MethodBase methodBase = stackFrame.GetMethod();

			//if (!(methodBase is MemberInfo))
			//    throw new ChoApplicationException("Context object cannot be accessed in this context.");

			//lock (MemberProfileCache.SyncRoot)
			//{
			//    if (profile is ChoProfileContainer)
			//    {
			//        ((ChoProfileContainer)profile).Owner = methodBase;
			//        ChoProfileContainer profileContainer = profile as ChoProfileContainer;
			//        //if (profileContainer.OuterProfile == null && _profileCache.ContainsKey(methodBase))
			//        //    Dispose(_profileCache[methodBase]);
			//        if (profileContainer.OuterProfile == null)
			//        {
			//            StackFrame parentStackFrame = ChoStackTrace.GetParentStackFrame(stackFrame);
			//            if (parentStackFrame != null)
			//            {
			//                if (parentStackFrame.GetMethod().DeclaringType != stackFrame.GetMethod().DeclaringType
			//                    && IsParentObjectProfileDefined(parentStackFrame))
			//                    profileContainer.OuterProfile = GetParentObjectProfile(name, parentStackFrame);
			//                else if (MemberProfileCache.ContainsKey(new ThreadProfileContext(name, parentStackFrame)))
			//                    profileContainer.OuterProfile = MemberProfileCache[new ThreadProfileContext(name, parentStackFrame)].Profile;
			//            }
			//        }
			//    }

			//    ThreadProfileContext context = new ThreadProfileContext(name, methodBase);
			//    if (MemberProfileCache.ContainsKey(context))
			//    {
			//        if (MemberProfileCache[context].Profile != null)
			//            MemberProfileCache[context].Profile.Dispose();

			//        MemberProfileCache[context] = new ChoProfileEntry(profile, enableCallTrace);
			//    }
			//    else
			//        MemberProfileCache.Add(context, new ChoProfileEntry(profile, enableCallTrace));
			//}
        }
        
        internal static void Unregister(object owner)
        {
            Unregister(null, owner);
        }

        internal static void Unregister(string name, object owner)
        {
			//if (owner == null) return;
			//lock (MemberProfileCache.SyncRoot)
			//{
			//    ThreadProfileContext threadProfileContext = new ThreadProfileContext(name, owner);
			//    if (MemberProfileCache.ContainsKey(threadProfileContext))
			//        MemberProfileCache.Remove(threadProfileContext);
			//}
        }

        #endregion Shared Members (Internal)

        #region RegisterIfNotExists Overloads

		public static void RegisterIfNotExists(IChoProfile profile)
		{
			RegisterIfNotExists(null, profile);
		}

        public static void RegisterIfNotExists(Lazy<IChoProfile> profile)
        {
            RegisterIfNotExists(null, profile);
        }

		public static void RegisterIfNotExists(string name, IChoProfile profile)
		{
			Register(name, profile);
		}

		public static void RegisterIfNotExists(string name, Lazy<IChoProfile> profile)
        {
			Register(name, profile);
        }

        #endregion RegisterIfNotExists Overloads

		#region Register Overloads

		public static void Register(IChoProfile profile)
		{
            Register(profile.ProfilerName, profile);
		}

        //public static void Register(Lazy<IChoProfile> profile)
        //{
        //    Register(null, profile);
        //}

		public static void Register(string name, IChoProfile profile)
		{
			Register(name, profile, true);
		}

		public static void Register(string name, Lazy<IChoProfile> profile)
		{
			Register(name, profile.Value, true);
		}

		private static void Register(string name, IChoProfile profile, bool force)
		{
			string typeProfileName = null;
			MemberInfo memberInfo = null;
			ChoProfileAttribute memberAttribute = null;
			ChoProfileAttribute typeAttribute = null;

			string profileName = GetProfileName(name, out memberInfo, out typeProfileName, out memberAttribute, out typeAttribute);

			if (force)
				MemberProfileCache.AddOrUpdate(profileName, profile);
			else
				MemberProfileCache.GetOrAdd(profileName, profile);
		}

		#endregion Register Overloads

        #region Dispose Members (Public)

        //[ChoAppDomainUnloadMethod("Disposing all profile objects...")]
        public static void DisposeAll()
        {
			lock (MemberProfileCache.SyncRoot)
            {
				foreach (IChoProfile profile in MemberProfileCache.ToValuesArray())
				{
					//SetAsNotDisposed(profile, true);

					if (profile is IDisposable)
					{
						((IDisposable)profile).Dispose();
					}
				}
				MemberProfileCache.Clear();
            }
            lock (_profileCacheLock)
            {
                if (_profileCache != null)
                {
                    _profileCache.ForEach((keyValue) => keyValue.Value.Dispose());
                    _profileCache.Clear();
                }
            }
        }

        public static void Dispose(IChoProfile profile)
        {
            if (profile == null) return;
            if (profile is IDisposable)
                ((IDisposable)profile).Dispose();
        }

        #endregion Dispose Members (Public)

        #region WriteNewLine Overloads

        public static void WriteNewLine()
        {
            IChoProfile profile = GetDefaultContext(new StackFrame(1));
            if (profile != null)
                profile.AppendIf(true, Environment.NewLine);
            else
                ChoTrace.WriteLine(Environment.NewLine);
        }

        public static void WriteNewLineIf(bool condition)
        {
            IChoProfile profile = GetDefaultContext(new StackFrame(1));
            if (profile != null)
                profile.AppendIf(condition, Environment.NewLine);
            else
                ChoTrace.WriteLineIf(condition, Environment.NewLine);
        }

        #endregion WriteNewLine Overloads

        #region Write Overloads

        public static void Write(string msg)
        {
            IChoProfile profile = GetDefaultContext(new StackFrame(1));
            if (profile != null)
                profile.AppendIf(true, msg);
            else
                ChoTrace.Write(msg);
        }

        public static void Write(string format, params object[] args)
        {
            IChoProfile profile = GetDefaultContext(new StackFrame(1));
            if (profile != null)
                profile.AppendIf(true, String.Format(format, args));
            else
                ChoTrace.Write(String.Format(format, args));
        }

        public static void WriteIf(bool condition, string format, params object[] args)
        {
            IChoProfile profile = GetDefaultContext(new StackFrame(1));
            if (profile != null)
                profile.AppendIf(condition, String.Format(format, args));
            else
                ChoTrace.WriteIf(condition, String.Format(format, args));
        }

        public static void WriteIf(bool condition, string msg)
        {
            IChoProfile profile = GetDefaultContext(new StackFrame(1));
            if (profile != null)
                profile.AppendIf(condition, msg);
            else
                ChoTrace.WriteIf(condition, msg);
        }

        #endregion Write Overloads

        #region WriteLine Overloads

        public static void WriteLine(string msg)
        {
            IChoProfile profile = GetDefaultContext(new StackFrame(1));
            if (profile != null)
                profile.AppendLineIf(true, msg);
            else
                ChoTrace.WriteLine(msg);
        }

        public static void WriteLine(string format, params object[] args)
        {
            IChoProfile profile = GetDefaultContext(new StackFrame(1));
            if (profile != null)
                profile.AppendLineIf(true, String.Format(format, args));
            else
                ChoTrace.WriteLine(String.Format(format, args));
        }

        public static void WriteLineIf(bool condition, string format, params object[] args)
        {
            IChoProfile profile = GetDefaultContext(new StackFrame(1));
            if (profile != null)
                profile.AppendLineIf(condition, String.Format(format, args));
            else
                ChoTrace.WriteLineIf(condition, String.Format(format, args));
        }

        public static void WriteLineIf(bool condition, string msg)
        {
            IChoProfile profile = GetDefaultContext(new StackFrame(1));
            if (profile != null)
                profile.AppendLineIf(condition, msg);
            else
                ChoTrace.WriteLineIf(condition, msg);
        }

        #endregion WriteLine Overloads

        #region WriteSeparator Overloads

        public static void WriteSeparator()
        {
            IChoProfile profile = GetDefaultContext(new StackFrame(1));
            if (profile != null)
                profile.AppendLineIf(true, ChoTrace.SEPARATOR);
            else
                ChoTrace.WriteLine(ChoTrace.SEPARATOR);
        }

        public static void WriteSeparatorIf(bool condition)
        {
            IChoProfile profile = GetDefaultContext(new StackFrame(1));
            if (profile != null)
                profile.AppendLineIf(condition, ChoTrace.SEPARATOR);
            else
                ChoTrace.WriteLineIf(condition, ChoTrace.SEPARATOR);
        }

        #endregion WriteSeparator Overloads

        #region Write Exception Overloads

        public static bool Write(Exception ex)
        {
            IChoProfile profile = GetDefaultContext(new StackFrame(1));
            if (profile != null)
                profile.Append(ex);
            else
                ChoTrace.Error(ex.ToString());
            return true;
        }

        public static bool WriteNThrow(Exception ex)
        {
            IChoProfile profile = GetDefaultContext(new StackFrame(1));
            if (profile != null)
                profile.Append(ex);
            else
                ChoTrace.Error(ex.ToString());
            throw new ChoApplicationException("Failed to write.", ex);
        }

        #endregion Write Exception Overloads
	}
}
