namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using Cinchoo.Core.Services;
    using System.Reflection;

    #endregion NameSpaces

    internal static class ChoConfigurationObjectErrorManagerService
    {
        #region ChoConfigurationObjectErrorInfo Class

        private class ChoConfigurationObjectErrorInfo
        {
            #region Instance Data Members (Public)

            public string ObjectErrMsg;
            public Dictionary<string, string> ObjectMemberErrMsgs = new Dictionary<string, string>();

            #endregion Instance Data Members (Public)

            #region Instance Members (Public)

            public void SetMemberErrorMsg(string memberName, string errMsg)
            {
                ChoGuard.ArgumentNotNullOrEmpty(memberName, "MemberName");

                if (!ObjectMemberErrMsgs.ContainsKey(memberName))
                    ObjectMemberErrMsgs.Add(memberName, errMsg);
                else
                    ObjectMemberErrMsgs[memberName] = errMsg;

            //    if (ObjectErrMsg.IsNullOrWhiteSpace())
            //        ObjectErrMsg = "Found error in configuration object member '{0}'.".FormatString(memberName);
            }

            public string GetMemberErrorMsg(string memberName)
            {
                ChoGuard.ArgumentNotNullOrEmpty(memberName, "MemberName");

                if (ObjectMemberErrMsgs.ContainsKey(memberName))
                    return ObjectMemberErrMsgs[memberName];
                else
                    return null;
            }

            public void ResetMemberErrorMsg(string memberName)
            {
                ChoGuard.ArgumentNotNullOrEmpty(memberName, "MemberName");

                if (ObjectMemberErrMsgs.ContainsKey(memberName))
                    ObjectMemberErrMsgs.Remove(memberName);
            }

            #endregion Instance Members (Public)
        }

        #endregion ChoConfigurationObjectErrorInfo Class

        #region Instance Data Members (Private)

        //private readonly static ChoWeakDictionaryService<object, ChoConfigurationObjectErrorInfo> _weakDictionaryService = 
        //    new ChoWeakDictionaryService<object,ChoConfigurationObjectErrorInfo>("ConfigurationObjectErrorManager");

        private readonly static ChoDictionaryService<object, ChoConfigurationObjectErrorInfo> _weakDictionaryService =
            new ChoDictionaryService<object, ChoConfigurationObjectErrorInfo>("ConfigurationObjectErrorManager");

        #endregion Instance Data Members (Private)

        #region Instance Members (Internal)

        internal static bool ContainsObjectError(object target)
        {
            lock (_weakDictionaryService.SyncRoot)
            {
                ChoConfigurationObjectErrorInfo errorInfo = GetConfigurationErrorInfo(target);
                return errorInfo.ObjectMemberErrMsgs.Count != 0 || !errorInfo.ObjectErrMsg.IsNullOrEmpty();
            }
        }

        internal static void SetObjectError(object target, string errMsg)
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            lock (_weakDictionaryService.SyncRoot)
            {
                ChoConfigurationObjectErrorInfo errorInfo = GetConfigurationErrorInfo(target);
                errorInfo.ObjectErrMsg = errMsg;
            }
        }

        internal static string GetObjectError(object target)
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            lock (_weakDictionaryService.SyncRoot)
            {
                ChoConfigurationObjectErrorInfo errorInfo = GetConfigurationErrorInfo(target);
                return errorInfo.ObjectErrMsg;
            }
        }

        internal static void ResetObjectErrors(object target)
        {
            ChoGuard.ArgumentNotNull(target, "Target");

			if (_weakDictionaryService != null)
			{
				lock (_weakDictionaryService.SyncRoot)
					_weakDictionaryService.RemoveValue(target);
			}
        }

        internal static void SetObjectMemberError(object target, string memberName, string errMsg)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            ChoGuard.ArgumentNotNullOrEmpty(memberName, "MemberName");

            lock (_weakDictionaryService.SyncRoot)
            {
                ChoConfigurationObjectErrorInfo errorInfo = GetConfigurationErrorInfo(target);
                errorInfo.SetMemberErrorMsg(memberName, errMsg.Replace(Environment.NewLine, " "));
            }
        }

        internal static string GetObjectMemberError(object target, string memberName)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            ChoGuard.ArgumentNotNullOrEmpty(memberName, "MemberName");

            lock (_weakDictionaryService.SyncRoot)
            {
                ChoConfigurationObjectErrorInfo errorInfo = GetConfigurationErrorInfo(target);
                return errorInfo.GetMemberErrorMsg(memberName);
            }
        }

        internal static void ResetObjectMemberError(object target, string memberName)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            ChoGuard.ArgumentNotNullOrEmpty(memberName, "MemberName");

            lock (_weakDictionaryService.SyncRoot)
            {
                ChoConfigurationObjectErrorInfo errorInfo = GetConfigurationErrorInfo(target);
                errorInfo.ResetMemberErrorMsg(memberName);
            }
        }

        #endregion Instance Members (Internal)

        #region Instance Members (Private)

        private static ChoConfigurationObjectErrorInfo GetConfigurationErrorInfo(object target)
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            ChoConfigurationObjectErrorInfo errorInfo = _weakDictionaryService.GetValue(target);
            if (errorInfo == null)
            {
                errorInfo = new ChoConfigurationObjectErrorInfo();
                _weakDictionaryService.SetValue(target, errorInfo);
            }

            return errorInfo;
        }

        #endregion Instance Members (Private)

		[ChoAppDomainUnloadMethod("Stopping global asynchronous execution service...")]
		private static void StopGlobalQueuedExecutionService()
		{
			if (_weakDictionaryService != null)
			{
				//_weakDictionaryService.Dispose();
			}
		}

    }
}
