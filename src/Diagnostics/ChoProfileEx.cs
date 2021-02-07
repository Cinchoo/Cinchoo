//namespace Cinchoo.Core.Diagnostics
//{
//    #region NameSpaces

//    using System;
//    using System.IO;
//    using System.Text;
//    using System.Reflection;
//    using System.Diagnostics;

//    using Cinchoo.Core.Types;
//    using Cinchoo.Core.Exceptions;
//    using Cinchoo.Core.Attributes;
//    using Cinchoo.Core.Diagnostics.Settings;
//    using Cinchoo.Core.Collections.Generic;
//    using Cinchoo.Core.Diagnostics.Attributes;
//    using Cinchoo.Core.IO;
//    using Cinchoo.Core.Collections.Generic.Dictionary;
//    using Cinchoo.Core.Threading;

//    #endregion NameSpaces

//    [ChoAppDomainEventsRegisterableType]
//    public class ChoProfile : ChoProfileContainer, IChoProfile, IChoTrace
//    {
//        #region ChoProfileEntry

//        private struct ChoProfileEntry
//        {
//            public IChoProfile Profile;
//            public bool EnableCallTrace;

//            public ChoProfileEntry(IChoProfile profile, bool enableCallTrace)
//            {
//                Profile = profile;
//                EnableCallTrace = enableCallTrace;
//            }

//            public ChoProfileEntry(IChoProfile profile) : this(profile, true)
//            {
//            }
//        }

//        #endregion ChoProfileEntry

//        #region ThreadProfileContext Struct

//        [DebuggerDisplay("Name = {_name}, HashCode = {_hashCode}")]
//        private struct ThreadProfileContext
//        {
//            #region Instance Data Members (Public)

//            private readonly int _hashCode;
//            private readonly string _name;

//            #endregion Instance Data Members (Public)

//            #region Constructors

//            public ThreadProfileContext(StackFrame stackFrame)
//                : this(null, stackFrame)
//            {
//            }

//            public ThreadProfileContext(string name, StackFrame stackFrame)
//            {
//                ChoGuard.ArgumentNotNull(stackFrame, "StackFrame");

//                string threadId = null; //ChoApplicationInfo.UnmanagedCodePermissionAvailable ? ChoApplicationInfo.GetThreadId() : null; //				ChoRandom.NextRandom().ToString();
//                object owner = stackFrame.GetMethod();
//                object target = ChoThreadLocalStorage.Target;
//                //_name = name + " SF- " + (owner != null ? owner.GetHashCode() : 0) + " " + (target != null ? target.GetHashCode() : 0);
//                _name = name + " SF- " + stackFrame.GetMethod().Name + " " + (target != null ? target.ToString(): String.Empty); //				target != null ? target.GetType().Name : "UNKNOWN";

//                _hashCode = (name != null ? name.GetHashCode() : 0) ^ (threadId != null ? threadId.GetHashCode() : 0) ^ (target != null ? target.GetHashCode() : 0) ^ (owner != null ? owner.GetHashCode() : 0);
//            }

//            public ThreadProfileContext(string name, object owner)
//            {
//                string threadId = null; // ChoApplicationInfo.UnmanagedCodePermissionAvailable ? ChoApplicationInfo.GetThreadId() : null; // ChoRandom.NextRandom().ToString();
//                object target = ChoThreadLocalStorage.Target;
//                _name = name + " Owner- " + (owner != null ? owner.ToString() : String.Empty); //				target != null ? target.GetType().Name : "UNKNOWN";

//                _hashCode = (name != null ? name.GetHashCode() : 0) ^ (threadId != null ? threadId.GetHashCode() : 0) ^ (target != null ? target.GetHashCode() : 0) ^ (owner != null ? owner.GetHashCode() : 0);
//            }

//            #endregion Constructors

//            public override bool Equals(object obj)
//            {
//                if (obj == null || !(obj is ThreadProfileContext)) return false;

//                ThreadProfileContext threadProfileContext = (ThreadProfileContext)obj;

//                return threadProfileContext.GetHashCode() == GetHashCode();
//                //return threadProfileContext.Name == Name && threadProfileContext.ThreadId == ThreadId && threadProfileContext.Owner == Owner;
//            }

//            public override int GetHashCode()
//            {
//                return _hashCode;
//            }

//            public override string ToString()
//            {
//                return _name + " " + _hashCode.ToString();
//            }
//        }

//        #endregion ThreadProfileContext Struct

//        #region Shared Data Members (Private)

//        private static ChoDictionary<ThreadProfileContext, ChoProfileEntry> MemberProfileCache = ChoDictionary<ThreadProfileContext, ChoProfileEntry>.Synchronized(new ChoDictionary<ThreadProfileContext, ChoProfileEntry>());

//        #endregion Shared Data Members (Private)

//        #region Instance Data Members (Private)

//        private int _indent = 0;
//        private bool _disposed = false;
//        private DateTime _startTime = DateTime.Now;
//        private string _name = ChoRandom.NextRandom().ToString();
//        private bool _condition = ChoTraceSwitch.Switch.TraceVerbose;

//        #endregion

//        #region Constructors

//        public ChoProfile(string msg)
//            : this(ChoTraceSwitch.Switch.TraceVerbose, msg)
//        {
//        }

//        public ChoProfile(string msg, ChoProfile outerProfile)
//            : this(ChoTraceSwitch.Switch.TraceVerbose, msg, outerProfile)
//        {
//        }

//        public ChoProfile(bool condition, string msg)
//            : this(condition, msg, null)
//        {
//        }

//        public ChoProfile(bool condition, string msg, ChoProfile outerProfile)
//        {
//            _condition = condition;
//            if (outerProfile != null)
//                _indent = outerProfile._indent + 1;
//            if (String.IsNullOrEmpty(msg))
//                msg = GetDefaultMsg(ChoStackTrace.GetStackFrame());

//            if (_condition)
//            {
//                if (ChoTraceSettings.Me.IndentProfiling)
//                    ChoTrace.WriteLineIf(_condition, ChoStackTrace.GetStackFrame(typeof(ChoProfile)), String.Format("{0} {{", msg).Indent(_indent));
//                else
//                    ChoTrace.WriteLineIf(_condition, ChoStackTrace.GetStackFrame(typeof(ChoProfile)), String.Format("{0} {{", msg));
//            }

//            if (outerProfile is ChoProfileContainer)
//                ((ChoProfileContainer)outerProfile).Add(this);
//        }

//        #endregion

//        #region IDisposable Members (Public)

//        public void Dispose()
//        {
//            Dispose(false);
//        }

//        #endregion

//        #region Finalizaer

//        ~ChoProfile()
//        {
//            Dispose(true);
//        }

//        #endregion

//        #region ChoProfileContainer Overrides

//        internal override IChoProfile OuterProfile
//        {
//            get { return null; }
//            set { }
//        }

//        #endregion ChoProfileContainer Overrides

//        #region IChoProfile Members (Public)

//        public string ProfilerName
//        {
//            get { return _name; }
//        }

//        public string AppendLine(string msg)
//        {
//            if (msg == null) return msg;
//            return AppendLineIf(_condition, msg);
//        }

//        public string AppendLineIf(bool condition, string msg)
//        {
//            if (msg == null) return msg;
//            return AppendIf(condition, msg + Environment.NewLine);
//        }

//        public string Append(string msg)
//        {
//            return AppendIf(_condition, msg);
//        }

//        public string AppendIf(bool condition, string msg)
//        {
//            if (msg == null || !condition) return msg;

//            _Write(msg);
//            return msg;
//        }

//        public void AppendIf(bool condition, Exception ex)
//        {
//            if (condition) AppendLine(ChoApplicationException.ToString(ex));
//        }

//        public void Append(Exception ex)
//        {
//            AppendIf(_condition, ex);
//        }

//        public string AppendIf(bool condition, string format, params object[] args)
//        {
//            return AppendIf(condition, String.Format(format, args));
//        }

//        public string Append(string format, params object[] args)
//        {
//            return Append(String.Format(format, args));
//        }

//        public string AppendLineIf(bool condition, string format, params object[] args)
//        {
//            return AppendLineIf(condition, String.Format(format, args));
//        }

//        public string AppendLine(string format, params object[] args)
//        {
//            return AppendLine(String.Format(format, args));
//        }

//        public string ElapsedTimeTaken
//        {
//            get { return Convert.ToString(DateTime.Now - _startTime); }
//        }

//        public int Indent
//        {
//            get { return _indent; }
//        }

//        #endregion IChoProfile Members (Public)

//        #region Instance Members (Private)

//        private void Dispose(bool finalize)
//        {
//            if (!_disposed)
//            {
//                _disposed = true;

//                Clear();

//                if (_condition)
//                {
//                    if (ChoTraceSettings.Me.IndentProfiling)
//                        ChoTrace.WriteLineIf(_condition, ChoStackTrace.GetStackFrame(typeof(ChoProfile)), String.Format("}} [{0}] <---", Convert.ToString(DateTime.Now - _startTime)).Indent(_indent));
//                    else
//                        ChoTrace.WriteLineIf(_condition, ChoStackTrace.GetStackFrame(typeof(ChoProfile)), String.Format("}} [{0}] <---", Convert.ToString(DateTime.Now - _startTime)));
//                }

//                if (!finalize)
//                    GC.SuppressFinalize(this);
//            }
//        }

//        private void _WriteLine(string msg)
//        {
//            _WriteLineIf(_condition, msg);
//        }

//        private void _WriteLineIf(bool condition, string msg)
//        {
//            if (ChoTraceSettings.Me.IndentProfiling)
//                msg = msg.Indent(_indent + 1);

//            ChoTrace.WriteLineIf(condition, ChoStackTrace.GetStackFrame(typeof(ChoProfile)), msg);
//        }

//        private void _Write(string msg)
//        {
//            _WriteIf(_condition, msg);
//        }

//        private void _WriteIf(bool condition, string msg)
//        {
//            if (!condition) return;

//            if (ChoTraceSettings.Me.IndentProfiling)
//                msg = msg.Indent(_indent + 1);

//            ChoTrace.WriteIf(condition, ChoStackTrace.GetStackFrame(typeof(ChoProfile)), msg);
//        }

//        #endregion Instance Members (Private)

//        #region Instance Properties (Internal)

//        internal override object Owner
//        {
//            get { return null; }
//            set { }
//        }

//        #endregion Instance Properties (Internal)

//        #region IChoTrace Members

//        public void Debug(object message)
//        {
//            if (message != null)
//                AppendLineIf(ChoTraceSwitch.Switch.TraceVerbose, message.ToString());
//        }

//        public void Debug(Exception exception)
//        {
//            AppendIf(ChoTraceSwitch.Switch.TraceVerbose, exception);
//        }

//        public void Debug(object message, Exception exception)
//        {
//            Debug(message);
//            Debug(exception);
//        }

//        public void DebugFormat(string format, params object[] args)
//        {
//            AppendLineIf(ChoTraceSwitch.Switch.TraceVerbose, String.Format(format, args));
//        }

//        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
//        {
//            AppendLineIf(ChoTraceSwitch.Switch.TraceVerbose, String.Format(provider, format, args));
//        }

//        public void Error(object message)
//        {
//            if (message != null)
//                AppendLineIf(ChoTraceSwitch.Switch.TraceError, message.ToString());
//        }

//        public void Error(Exception exception)
//        {
//            AppendIf(ChoTraceSwitch.Switch.TraceError, exception);
//        }

//        public void Error(object message, Exception exception)
//        {
//            Error(message);
//            Error(exception);
//        }

//        public void ErrorFormat(string format, params object[] args)
//        {
//            AppendLineIf(ChoTraceSwitch.Switch.TraceError, String.Format(format, args));
//        }

//        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
//        {
//            AppendLineIf(ChoTraceSwitch.Switch.TraceError, String.Format(provider, format, args));
//        }

//        public void Info(object message)
//        {
//            if (message != null)
//                AppendLineIf(ChoTraceSwitch.Switch.TraceInfo, message.ToString());
//        }

//        public void Info(Exception exception)
//        {
//            AppendIf(ChoTraceSwitch.Switch.TraceInfo, exception);
//        }

//        public void Info(object message, Exception exception)
//        {
//            Info(message);
//            Info(exception);
//        }

//        public void InfoFormat(string format, params object[] args)
//        {
//            AppendLineIf(ChoTraceSwitch.Switch.TraceInfo, String.Format(format, args));
//        }

//        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
//        {
//            AppendLineIf(ChoTraceSwitch.Switch.TraceInfo, String.Format(provider, format, args));
//        }

//        public void Warn(object message)
//        {
//            if (message != null)
//                AppendLineIf(ChoTraceSwitch.Switch.TraceWarning, message.ToString());
//        }

//        public void Warn(Exception exception)
//        {
//            AppendIf(ChoTraceSwitch.Switch.TraceWarning, exception);
//        }

//        public void Warn(object message, Exception exception)
//        {
//            Warn(message);
//            Warn(exception);
//        }

//        public void WarnFormat(string format, params object[] args)
//        {
//            AppendLineIf(ChoTraceSwitch.Switch.TraceWarning, String.Format(format, args));
//        }

//        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
//        {
//            AppendLineIf(ChoTraceSwitch.Switch.TraceWarning, String.Format(provider, format, args));
//        }

//        #endregion

//        #region Shared Properties (Public)

//        public static IChoProfile GetContext(string name)
//        {
//            StackFrame stackFrame = ChoStackTrace.GetStackFrame(typeof(ChoProfile).Namespace);
//            ThreadProfileContext context = new ThreadProfileContext(stackFrame);

//            if (ContainsMemberProfile(name, stackFrame))
//                return MemberProfileCache[context].Profile;

//            lock (MemberProfileCache.SyncRoot)
//            {
//                if (ContainsMemberProfile(name, stackFrame))
//                    return MemberProfileCache[context].Profile;
				
//                Register(name, stackFrame);

//                return MemberProfileCache[context].Profile;
//            }
//        }

//        public static IChoProfile GetContext(object target, string name)
//        {
//            lock (MemberProfileCache.SyncRoot)
//            {
//                ChoThreadLocalStorage.Register(target);
//                return GetContext(name);
//            }
//        }

//        public static IChoProfile GetDefaultContext(object target)
//        {
//            lock (MemberProfileCache.SyncRoot)
//            {
//                ChoThreadLocalStorage.Register(target);
//                return DefaultContext;
//            }
//        }

//        public static IChoProfile DefaultContext
//        {
//            get
//            {
//                StackFrame stackFrame = ChoStackTrace.GetStackFrame(typeof(ChoProfile).Namespace);
//                ThreadProfileContext context = new ThreadProfileContext(stackFrame);

//                if (ContainsMemberProfile(null, stackFrame))
//                    return MemberProfileCache[context].Profile;
				
//                lock (MemberProfileCache.SyncRoot)
//                {
//                    if (ContainsMemberProfile(null, stackFrame))
//                        return MemberProfileCache[context].Profile;

//                    Register(stackFrame);

//                    return MemberProfileCache[context].Profile;
//                }
//            }
//        }

//        #endregion Shared Properties (Public)

//        #region Shared Members (Internal)

//        internal static string GetDefaultMsg(StackFrame stackFrame)
//        {
//            if (stackFrame.GetMethod().Name == ".ctor" || stackFrame.GetMethod().Name == ".cctor")
//                return String.Format("Elapsed time taken by `{0}` caller...", stackFrame.GetMethod().ReflectedType.FullName);
//            else
//                return String.Format("Elapsed time taken by `{0}[{1}]` caller...", stackFrame.GetMethod().ToString(), stackFrame.GetMethod().ReflectedType.FullName);
//        }

//        internal static bool ContainsMemberProfile(string name, StackFrame stackFrame)
//        {
//            MethodBase methodBase = stackFrame.GetMethod();

//            if (!(methodBase is MemberInfo))
//                throw new ChoApplicationException("Context object cannot be accessing in this context.");

//            return MemberProfileCache.ContainsKey(new ThreadProfileContext(name, methodBase as MemberInfo));
//        }

//        //TODO: to be removed
//        internal static void Register(IChoProfile profile, StackFrame stackFrame)
//        {
//            //Register(profile, stackFrame, true);
//            Register(null, profile, stackFrame, true);
//        }
        
//        //internal static void Register(IChoProfile profile, StackFrame stackFrame, bool enableCallTrace)
//        //{
//        //    Register(null, profile, stackFrame, null, enableCallTrace);
//        //}

//        internal static void Register(string name, IChoProfile profile, StackFrame stackFrame, bool enableCallTrace)
//        {
//            MethodBase methodBase = stackFrame.GetMethod();

//            if (!(methodBase is MemberInfo))
//                throw new ChoApplicationException("Context object cannot be accessed in this context.");

//            lock (MemberProfileCache.SyncRoot)
//            {
//                if (profile is ChoProfileContainer)
//                {
//                    ((ChoProfileContainer)profile).Owner = methodBase;
//                    ChoProfileContainer profileContainer = profile as ChoProfileContainer;
//                    //if (profileContainer.OuterProfile == null && _profileCache.ContainsKey(methodBase))
//                    //    Dispose(_profileCache[methodBase]);
//                    if (profileContainer.OuterProfile == null)
//                    {
//                        StackFrame parentStackFrame = ChoStackTrace.GetParentStackFrame(stackFrame);
//                        if (parentStackFrame != null)
//                        {
//                            if (parentStackFrame.GetMethod().DeclaringType != stackFrame.GetMethod().DeclaringType
//                                && IsParentObjectProfileDefined(parentStackFrame))
//                                profileContainer.OuterProfile = GetParentObjectProfile(name, parentStackFrame);
//                            else if (MemberProfileCache.ContainsKey(new ThreadProfileContext(name, parentStackFrame)))
//                                profileContainer.OuterProfile = MemberProfileCache[new ThreadProfileContext(name, parentStackFrame)].Profile;
//                        }
//                    }
//                }

//                ThreadProfileContext context = new ThreadProfileContext(name, methodBase);
//                if (MemberProfileCache.ContainsKey(context))
//                {
//                    if (MemberProfileCache[context].Profile != null)
//                        MemberProfileCache[context].Profile.Dispose();

//                    MemberProfileCache[context] = new ChoProfileEntry(profile, enableCallTrace);
//                }
//                else
//                    MemberProfileCache.Add(context, new ChoProfileEntry(profile, enableCallTrace));
//            }
//        }

//        internal static void Register(StackFrame stackFrame, bool enableCallTrace)
//        {
//            lock (MemberProfileCache.SyncRoot)
//            {
//                _Register(null, stackFrame, enableCallTrace);
//            }
//        }

//        internal static void Register(StackFrame stackFrame)
//        {
//            lock (MemberProfileCache.SyncRoot)
//            {
//                _Register(null, stackFrame, null);
//            }
//        }

//        internal static void Register(string name, StackFrame stackFrame)
//        {
//            lock (MemberProfileCache.SyncRoot)
//            {
//                _Register(name, stackFrame, null);
//            }
//        }

//        private static void _Register(string name, StackFrame stackFrame, bool? overrideEnableCallTrace)
//        {
//            if (ContainsMemberProfile(name, stackFrame))
//                return;

//            MemberInfo memberInfo = stackFrame.GetMethod() as MemberInfo;
//            ConstructProfiles(memberInfo);

//            IChoProfile parentProfile = null;
//            bool enableCallTrace = true;

//            StackFrame parentStackFrame = ChoStackTrace.GetParentStackFrame(stackFrame);
//            if (parentStackFrame != null && !ContainsMemberProfile(name, parentStackFrame))
//            {
//                if (parentStackFrame.GetMethod().DeclaringType != stackFrame.GetMethod().DeclaringType
//                    && IsParentObjectProfileDefined(stackFrame))
//                    parentProfile = GetParentObjectProfile(name, stackFrame, out enableCallTrace);

//                if (parentProfile == null)
//                    _Register(name, parentStackFrame, overrideEnableCallTrace);
//            }

//            if (parentStackFrame != null && MemberProfileCache.ContainsKey(new ThreadProfileContext(name, parentStackFrame)))
//            {
//                parentProfile = MemberProfileCache[new ThreadProfileContext(name, parentStackFrame)].Profile;
//                enableCallTrace = MemberProfileCache[new ThreadProfileContext(name, parentStackFrame)].EnableCallTrace;
//            }

//            IChoProfile memberProfile = null;
//            MemberInfo memberInfo = stackFrame.GetMethod() as MemberInfo;
//            if (enableCallTrace)
//            {
//                ChoProfileAttribute memberProfileAttribute = null;
//                foreach (ChoProfileAttribute profileAttribute in ChoType.GetMemberAttributesByBaseType<ChoProfileAttribute>(memberInfo))
//                {
//                    if (profileAttribute.Name == name)
//                    {
//                        memberProfileAttribute = profileAttribute;
//                        break;
//                    }
//                }

//                if (memberProfileAttribute == null)
//                {
//                    if (parentStackFrame == null) //Main Entry
//                    {
//                        memberProfileAttribute = new ChoBufferProfileAttribute(ChoTraceSwitch.Switch.TraceVerbose, String.Format("Calling {0}.{1} method...", memberInfo.ReflectedType.FullName, memberInfo.Name));
//                        memberProfileAttribute.Directory = null;
//                        memberProfileAttribute.FileName = ChoPath.AddExtension(ChoApplicationSettings.Me.ApplicationId);
//                    }
//                    else
//                        memberProfileAttribute = new ChoBufferProfileAttribute(ChoTraceSwitch.Switch.TraceVerbose, String.Format("Calling {0}.{1} method...", memberInfo.ReflectedType.FullName, memberInfo.Name));
//                }

//                //REVISIT
//                memberProfile = memberProfileAttribute.ConstructProfile(ChoThreadLocalStorage.Target, parentProfile);
//            }
//            else
//                memberProfile = parentProfile;

//            if (memberProfile is ChoProfileContainer)
//                ((ChoProfileContainer)memberProfile).Owner = memberInfo;

//            MemberProfileCache.Add(new ThreadProfileContext(name, memberInfo), new ChoProfileEntry(memberProfile, overrideEnableCallTrace.HasValue ? overrideEnableCallTrace.Value : enableCallTrace));
//        }

//        private static 
//        private static void ConstructProfiles(MemberInfo memberInfo)
//        {
//            throw new NotImplementedException();
//        }

//        //private static void _Register(string name, StackFrame stackFrame, bool? overrideEnableCallTrace)
//        //{
//        //    if (ContainsMemberProfile(name, stackFrame)) return;

//        //    IChoProfile parentProfile = null;
//        //    bool enableCallTrace = true;

//        //    StackFrame parentStackFrame = ChoStackTrace.GetParentStackFrame(stackFrame);
//        //    if (parentStackFrame != null && !ContainsMemberProfile(name, parentStackFrame))
//        //    {
//        //        if (parentStackFrame.GetMethod().DeclaringType != stackFrame.GetMethod().DeclaringType
//        //            && IsParentObjectProfileDefined(stackFrame))
//        //            parentProfile = GetParentObjectProfile(name, stackFrame, out enableCallTrace);

//        //        if (parentProfile == null)
//        //            _Register(name, parentStackFrame, overrideEnableCallTrace);
//        //    }

//        //    if (parentStackFrame != null && MemberProfileCache.ContainsKey(new ThreadProfileContext(name, parentStackFrame)))
//        //    {
//        //        parentProfile = MemberProfileCache[new ThreadProfileContext(name, parentStackFrame)].Profile;
//        //        enableCallTrace = MemberProfileCache[new ThreadProfileContext(name, parentStackFrame)].EnableCallTrace;
//        //    }

//        //    IChoProfile memberProfile = null;
//        //    MemberInfo memberInfo = stackFrame.GetMethod() as MemberInfo;
//        //    if (enableCallTrace)
//        //    {
//        //        ChoProfileAttribute memberProfileAttribute = null;
//        //        foreach (ChoProfileAttribute profileAttribute in ChoType.GetMemberAttributesByBaseType<ChoProfileAttribute>(memberInfo))
//        //        {
//        //            if (profileAttribute.Name == name)
//        //            {
//        //                memberProfileAttribute = profileAttribute;
//        //                break;
//        //            }
//        //        }

//        //        if (memberProfileAttribute == null)
//        //        {
//        //            if (parentStackFrame == null) //Main Entry
//        //            {
//        //                memberProfileAttribute = new ChoBufferProfileAttribute(ChoTraceSwitch.Switch.TraceVerbose, String.Format("Calling {0}.{1} method...", memberInfo.ReflectedType.FullName, memberInfo.Name));
//        //                memberProfileAttribute.Directory = null;
//        //                memberProfileAttribute.FileName = ChoPath.AddExtension(ChoApplicationSettings.Me.ApplicationId);
//        //            }
//        //            else
//        //                memberProfileAttribute = new ChoBufferProfileAttribute(ChoTraceSwitch.Switch.TraceVerbose, String.Format("Calling {0}.{1} method...", memberInfo.ReflectedType.FullName, memberInfo.Name));
//        //        }

//        //        //REVISIT
//        //        memberProfile = memberProfileAttribute.ConstructProfile(ChoThreadLocalStorage.Target, parentProfile);
//        //    }
//        //    else
//        //        memberProfile = parentProfile;

//        //    if (memberProfile is ChoProfileContainer)
//        //        ((ChoProfileContainer)memberProfile).Owner = memberInfo;

//        //    MemberProfileCache.Add(new ThreadProfileContext(name, memberInfo), new ChoProfileEntry(memberProfile, overrideEnableCallTrace.HasValue ? overrideEnableCallTrace.Value : enableCallTrace));
//        //}

//        internal static bool IsParentObjectProfileDefined(StackFrame stackFrame)
//        {
//            MemberInfo memberInfo = stackFrame.GetMethod();
//            ChoProfileAttribute classProfileAttribute = ChoType.GetAttribute<ChoProfileAttribute>(memberInfo.DeclaringType);
//            return classProfileAttribute != null;
//        }

//        internal static IChoProfile GetParentObjectProfile(string name, StackFrame stackFrame)
//        {
//            bool enableCallTrace = true;
//            return GetParentObjectProfile(name, stackFrame, out enableCallTrace);
//        }

//        internal static IChoProfile GetParentObjectProfile(string name, StackFrame stackFrame, out bool enableCallTrace)
//        {
//            enableCallTrace = true;
//            MemberInfo memberInfo = stackFrame.GetMethod();

//            #region Identify and Build Class level profile

//            ThreadProfileContext callerProfileContext = new ThreadProfileContext(name, memberInfo.DeclaringType);
//            IChoProfile classProfile = null;

//            if (MemberProfileCache.ContainsKey(callerProfileContext))
//                classProfile = MemberProfileCache[callerProfileContext].Profile;
//            else
//            {
//                ChoProfileAttribute classProfileAttribute = ChoType.GetAttribute<ChoProfileAttribute>(memberInfo.DeclaringType);
//                if (classProfileAttribute == null)
//                {
//                    classProfileAttribute = new ChoStreamProfileAttribute(String.Empty);
//                    classProfileAttribute.FileName = String.Format("{0}.{1}", memberInfo.DeclaringType.FullName, ChoReservedFileExt.Log);
//                }

//                enableCallTrace = classProfileAttribute.EnableCallTrace;
//                //REVISIT
//                classProfile = classProfileAttribute.ConstructProfile(ChoThreadLocalStorage.Target, null);
//                if (classProfile is ChoProfileContainer)
//                    ((ChoProfileContainer)classProfile).Owner = callerProfileContext;

//                MemberProfileCache.Add(callerProfileContext, new ChoProfileEntry(classProfile, enableCallTrace));
//            }

//            #endregion Identify and Build Class level profile

//            return classProfile;
//        }
        
//        internal static void Unregister(object owner)
//        {
//            Unregister(null, owner);
//        }

//        internal static void Unregister(string name, object owner)
//        {
//            if (owner == null) return;
//            lock (MemberProfileCache.SyncRoot)
//            {
//                ThreadProfileContext threadProfileContext = new ThreadProfileContext(name, owner);
//                if (MemberProfileCache.ContainsKey(threadProfileContext))
//                    MemberProfileCache.Remove(threadProfileContext);
//            }
//        }

//        #endregion Shared Members (Internal)

//        #region RegisterIfNotExists Overloads

//        public static void RegisterIfNotExists(IChoProfile profile)
//        {
//            RegisterIfNotExists(null, profile);
//        }

//        public static void RegisterIfNotExists(string name, IChoProfile profile)
//        {
//            RegisterIfNotExists(name, profile, true);
//        }

//        public static void RegisterIfNotExists(IChoProfile profile, bool enableStackTrace)
//        {
//            RegisterIfNotExists(null, profile, enableStackTrace);
//        }

//        public static void RegisterIfNotExists(string name, IChoProfile profile, bool enableStackTrace)
//        {
//            Register(name, profile, enableStackTrace, false);
//        }

//        #endregion RegisterIfNotExists Overloads
        
//        #region Register Overloads

//        public static void Register(IChoProfile profile)
//        {
//            Register(null, profile);
//        }

//        public static void Register(string name, IChoProfile profile)
//        {
//            Register(name, profile, true);
//        }

//        public static void Register(IChoProfile profile, bool enableStackTrace)
//        {
//            Register(null, profile, enableStackTrace);
//        }

//        public static void Register(string name, IChoProfile profile, bool enableStackTrace)
//        {
//            Register(name, profile, enableStackTrace, true);
//        }

//        private static void Register(string name, IChoProfile profile, bool enableStackTrace, bool force)
//        {
//            if (!force)
//            {
//                if (MemberProfileCache.ContainsKey(new ThreadProfileContext(name, ChoStackTrace.GetStackFrame(typeof(ChoProfile)))))
//                    return;
//            }

//            Register(name, profile, ChoStackTrace.GetStackFrame(typeof(ChoProfile)), enableStackTrace);
//        }

//        #endregion Register Overloads

//        #region Dispose Members (Public)

//        [ChoAppDomainUnloadMethod("Disposing all profile objects...")]
//        public static void DisposeAll()
//        {
//            lock (MemberProfileCache.SyncRoot)
//            {
//                foreach (ChoProfileEntry profileEntry in MemberProfileCache.ToValuesArray())
//                {
//                    if (profileEntry.Profile is IDisposable)
//                    {
//                        ((IDisposable)profileEntry.Profile).Dispose();
//                    }
//                }
//                MemberProfileCache.Clear();
//            }
//        }

//        public static void Dispose(IChoProfile profile)
//        {
//            if (profile == null) return;
//            if (profile is IDisposable)
//                ((IDisposable)profile).Dispose();
//        }

//        #endregion Dispose Members (Public)

//        #region WriteNewLine Overloads

//        public static void WriteNewLine()
//        {
//            WriteNewLineIf(true);
//        }

//        public static void WriteNewLineIf(bool condition)
//        {
//            DefaultContext.AppendIf(condition, Environment.NewLine);
//        }

//        #endregion WriteNewLine Overloads

//        #region Write Overloads

//        public static void Write(string msg)
//        {
//            WriteIf(true, msg);
//        }

//        public static void Write(string format, params object[] args)
//        {
//            Write(String.Format(format, args));
//        }

//        public static void WriteIf(bool condition, string format, params object[] args)
//        {
//            WriteIf(condition, String.Format(format, args));
//        }

//        public static void WriteIf(bool condition, string msg)
//        {
//            DefaultContext.AppendIf(condition, msg);
//        }

//        #endregion Write Overloads

//        #region WriteLine Overloads

//        public static void WriteLine(string msg)
//        {
//            WriteLineIf(true, msg);
//        }

//        public static void WriteLine(string format, params object[] args)
//        {
//            WriteLine(String.Format(format, args));
//        }

//        public static void WriteLineIf(bool condition, string format, params object[] args)
//        {
//            WriteLineIf(condition, String.Format(format, args));
//        }

//        public static void WriteLineIf(bool condition, string msg)
//        {
//            DefaultContext.AppendLineIf(condition, msg);
//        }

//        #endregion WriteLine Overloads

//        #region WriteSeparator Overloads

//        public static void WriteSeparator()
//        {
//            WriteSeparatorIf(true);
//        }

//        public static void WriteSeparatorIf(bool condition)
//        {
//            DefaultContext.AppendLineIf(condition, ChoTrace.SEPARATOR);
//        }

//        #endregion WriteSeparator Overloads

//        #region Write Exception Overloads

//        public static bool Write(Exception ex)
//        {
//            DefaultContext.Append(ex);
//            return true;
//        }

//        public static bool WriteNThrow(Exception ex)
//        {
//            Write(ex);
//            throw new ChoApplicationException("Failed to write.", ex);
//        }

//        #endregion Write Exception Overloads

//        #region InitializeProfile Overloads

//        //public static void InitializeProfile(object target)
//        //{
//        //    ChoGuard.ArgumentNotNull(target, "Target");

//        //    ChoProfileAttribute classProfileAttribute = ChoType.GetAttribute<ChoProfileAttribute>(target.GetType());
//        //    if (classProfileAttribute != null)
//        //        classProfileAttribute.Initialize(target);
//        //}

//        //REVIST
//        //public static void InitializeProfile(Type type)
//        //{
//        //    ChoGuard.ArgumentNotNull(type, "Type");

//        //    ChoProfileAttribute classProfileAttribute = ChoType.GetAttribute<ChoProfileAttribute>(type);
//        //    if (classProfileAttribute != null)
//        //        classProfileAttribute.Initialize(null);
//        //}

//        #endregion InitializeProfile Overloads
//    }
//}
