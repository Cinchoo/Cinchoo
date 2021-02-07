namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.Text;

    #endregion NameSpaces

    [Serializable]
	public abstract class ChoFormattableObject : ChoDisposableObject, IFormattable, IDisposable
    {
        #region Constants

        public const string DefaultFormatName = "$$ToString$$";
        public const string ExtendedFormatName = "$$ToExtendedString$$";

        #endregion Constants

        #region Shared Data Members (Private)

        //private static readonly object _padLock = new object();
        private static readonly Regex regex = new Regex(@"(?<premsg>.*)\[Count: (?<count>\d+)\](?<msg>.*)", RegexOptions.Compiled | RegexOptions.Singleline);

        #endregion Shared Data Members (Private)

        #region Instance Members (Public)

        [ChoHiddenMember]
		public virtual string ToString(string formatName)
        {
            return ToString(this, formatName);
        }

        #endregion Instance Members (Public)

        #region Object Overrides

		[ChoHiddenMember]
		public override string ToString()
        {
            return ToString(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.FlattenHierarchy);
        }

		[ChoHiddenMember]
		public string ToString(BindingFlags bindingFlags)
		{
			return ToString(this, bindingFlags);
		}

        #endregion Object Overrides

        #region Shared Members (Public)

        private static bool HasFormatterSpecified(object target)
        {
            if (target == null)
                return false;

            ChoTypeFormatterAttribute objectFormatter = ChoType.GetAttribute(target.GetType(), typeof(ChoTypeFormatterAttribute)) as ChoTypeFormatterAttribute;
            return objectFormatter == null ? false : objectFormatter.HasFormatSpecified;
        }

		[ChoHiddenMember]
		public static string ToString(object target, string formatName)
        {
            if (target == null) return String.Empty;

            if (!HasFormatterSpecified(target))
            {
                Func<object, string> customFormatter = ChoGlobalObjectFormatters.GetObjectFormatHandler(target.GetType(), formatName);
                if (customFormatter != null)
                    return customFormatter(target);
                else
                {
                    ICustomFormatter formatter = ChoFormatProvider.Instance.GetFormat(target.GetType()) as ICustomFormatter;

                    ICustomFormatter tmpFormatter = formatter;
                    if (ChoApplication.OnObjectFormatterResolve(target.GetType(), out tmpFormatter))
                        formatter = tmpFormatter;

                    if (formatter != null)
                        return formatter.Format(formatName, target, null);
                    else if (formatName.IsNullOrWhiteSpace())
                        return ToString(target);
                    else
                        return ChoObject.Format(target, formatName);
                }
            }
            else
                return ToString(target);
        }

		[ChoHiddenMember]
		public static string ToString(object target)
        {
            return ToString(target, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.FlattenHierarchy);
        }

		[ChoHiddenMember]
		internal static string ToString(object target, BindingFlags bindingFlags)
		{
			return ToString(target, false, null, null, bindingFlags);
		}

        public static string ToString<T>(T target, Func<T, string> formatHandler)
        {
            if (target == null)
                return String.Empty;
            if (formatHandler == null)
                return ToString(target);
            else
                return formatHandler(target);
        }

        public static string ToString(object target, string format, IFormatProvider formatProvider)
        {
            if (target == null)
                return String.Empty;

            if (formatProvider != null)
            {
                ICustomFormatter formatter = formatProvider.GetFormat(target.GetType()) as ICustomFormatter;

                if (formatter != null)
                    return formatter.Format(format, target, formatProvider);
            }

            if (format == null)
                return ToString(target);

            return ToString(target, format);
        }

        #endregion Shared Members (Public)

        #region Shared Members (Private)

		[ChoHiddenMember]
		private static void GetErrorMsgs(object target, ChoStringMsgBuilder msg)
        {
            MemberInfo[] memberInfos = ChoType.GetMembers(target.GetType()); //, typeof(ChoMemberInfoAttribute));
            List<string> errMsgs = new List<string>();
            string errMsg;
            if (memberInfos != null && memberInfos.Length >= 0)
            {
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    errMsg = ChoConfigurationObjectErrorManagerService.GetObjectMemberError(target, memberInfo.Name);
                    if (errMsg != null)
                        errMsgs.Add(String.Format("{0}: {1}", memberInfo.Name, errMsg));
                }
            }

            if (errMsgs.Count > 0)
            {
				msg.AppendLine();
                ChoStringMsgBuilder errReport = new ChoStringMsgBuilder("Following errors occurred while construction");

                foreach (string errMsg1 in errMsgs)
                    errReport.AppendFormatLine(errMsg1);

                msg.AppendLine(errReport.ToString().Indent(1));
            }
            else 
            {
                errMsg = ChoConfigurationObjectErrorManagerService.GetObjectError(target);

                if (!errMsg.IsNullOrEmpty())
                {
					msg.AppendLine();
                    ChoStringMsgBuilder errReport = new ChoStringMsgBuilder("Following errors occurred while construction");
                    errReport.AppendFormatLine(errMsg);

                    msg.AppendLine(errReport.ToString().Indent(1));
                }
            }
        }

        [ChoHiddenMember]
        private static bool GetCountFromMsg(ref string msg, ref int count)
        {
            if (msg.IsNullOrEmpty())
                return false;

            Match match = regex.Match(msg);
            if (!match.Success)
                return false;

            msg = match.Groups["premsg"].ToString() + match.Groups["msg"].ToString();
            Int32.TryParse(match.Groups["count"].ToString(), out count);

            return true;
        }

		[ChoHiddenMember]
		private static string ToString(object target, bool collectErrMsgs, ChoMemberFormatterAttribute memberFormaterAttribute1, 
            ChoMemberItemFormatterAttribute memberItemFormaterAttribute1, BindingFlags bindingFlags)
        {
            if (target == null) return String.Empty;

            if (memberFormaterAttribute1 != null && memberFormaterAttribute1.CanFormat())
            {
                if (target.GetType().IsSimple())
                    return memberFormaterAttribute1.FormatObject(target);
                else
                {
                    ChoStringMsgBuilder msg = new ChoStringMsgBuilder();
					//msg.AppendLineIfNoNL(memberFormaterAttribute1.FormatObject(target));
                    msg.Append(memberFormaterAttribute1.FormatObject(target));
                    //msg.AppendNewLine();
                    if (collectErrMsgs) GetErrorMsgs(target, msg);
                    return msg.ToString();
                }
            }
            else if (target.GetType().IsSimple() || target.GetType().IsMethodImplemented("ToString"))
                return target.ToString();
            else if (target is Delegate)
				return String.Format("{0}.{1}", ((Delegate)target).Target == null ? "[Static]" : ((Delegate)target).Target.GetType().FullName, ((Delegate)target).Method.Name);
            else if (target is IEnumerable)
            {
                StringBuilder arrMsg = new StringBuilder();

                int count = 0;
                foreach (object item in (IEnumerable)target)
                {
                    count++;
                    arrMsg.AppendFormat("{0}{1}", ToString(item, collectErrMsgs, memberItemFormaterAttribute1, null, bindingFlags), Environment.NewLine);
                }

                return "[Count: {0}]{1}{2}".FormatString(count, Environment.NewLine, arrMsg.ToString());
            }
            else
            {
                bool foundMatchingFormatter = false;
                string retValue = ChoObject.Format(target, null, out foundMatchingFormatter);

                if (foundMatchingFormatter)
                {
                    if (target.GetType().IsSimple())
                        return retValue;
                    else
                    {
                        ChoStringMsgBuilder msg = new ChoStringMsgBuilder();
                        msg.AppendFormat(retValue);
                        msg.AppendNewLine();
                        if (collectErrMsgs) GetErrorMsgs(target, msg);
                        return msg.ToString();
                    }
                }
                else
                {
                    ChoInterceptableObject interceptableObject = null;
                    try
                    {
                        if (ChoType.IsRealProxyObject(target.GetType()))
                            interceptableObject = ChoInterceptableObject.Silentable(target as ChoInterceptableObject);

                        ChoTypeFormatterAttribute objectFormatter = ChoType.GetAttribute(target.GetType(), typeof(ChoTypeFormatterAttribute)) as ChoTypeFormatterAttribute;
                        if (objectFormatter == null)
                        {
                            ChoStringMsgBuilder msg = new ChoStringMsgBuilder(memberFormaterAttribute1 == null ? String.Format("{0} State", target.GetType().FullName) 
                                : memberFormaterAttribute1.Name);

							//MemberInfo[] memberInfos = target.GetType().GetMembers(bindingFlags /*BindingFlags.Public | BindingFlags.Instance /*| BindingFlags.DeclaredOnly*/ /*| BindingFlags.GetField | BindingFlags.GetProperty*/);
                            IEnumerable<MemberInfo> memberInfos = ChoType.GetGetFieldsNProperties(target.GetType(), bindingFlags);
                            if (memberInfos == null || memberInfos.Count() == 0)
                                msg.AppendFormatLine(ChoStringMsgBuilder.Empty);
                            else
                            {
                                foreach (MemberInfo memberInfo in memberInfos)
                                {
                                    if (!ChoType.IsValidObjectMember(memberInfo))
                                        continue;

                                    ChoIgnoreMemberFormatterAttribute memberFormatterIgnoreAttribute = ChoType.GetMemberAttribute(memberInfo, typeof(ChoIgnoreMemberFormatterAttribute)) as ChoIgnoreMemberFormatterAttribute;
                                    if (memberFormatterIgnoreAttribute != null)
                                        continue;

                                    string memberText = GetNFormatMemberValue(target, memberInfo, collectErrMsgs, null, null, bindingFlags);

                                    int count = 0;
                                    if (GetCountFromMsg(ref memberText, ref count))
                                        msg.AppendFormatLine("{0} [Length: {2}]: {1}", memberInfo.Name, memberText, count);
                                    else if (!memberText.ContainsHeader())
                                        msg.AppendFormatLine("{0}: {1}", memberInfo.Name, memberText);
                                    else
                                        msg.AppendFormatLine("{0}: {1}", memberInfo.Name, memberText);
                                }
                            }
                            msg.AppendNewLine();

                            if (collectErrMsgs) GetErrorMsgs(target, msg);

                            return msg.ToString();
                        }
                        else
                        {
                            ChoStringMsgBuilder msg = new ChoStringMsgBuilder(ChoString.ExpandProperties(target, objectFormatter.Header));;
                            if (objectFormatter.HasFormatSpecified)
                                msg.AppendFormat(objectFormatter.FormatObject(target));
                            else
                            {
								//MemberInfo[] memberInfos = target.GetType().GetMembers(bindingFlags /*BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance /*| BindingFlags.DeclaredOnly*/ /*| BindingFlags.GetField | BindingFlags.GetProperty*/);
                                IEnumerable<MemberInfo> memberInfos = ChoType.GetGetFieldsNProperties(target.GetType(), bindingFlags);
                                
                                if (memberInfos == null || memberInfos.Count() == 0)
                                    msg.AppendFormatLine(ChoStringMsgBuilder.Empty);
                                else
                                {
									bool isPublicMember = true;
                                    foreach (MemberInfo memberInfo in memberInfos)
                                    {
                                        if (!ChoType.IsValidObjectMember(memberInfo))
                                            continue;
										
                                        isPublicMember = true;
                                        
                                        ChoIgnoreMemberFormatterAttribute memberFormatterIgnoreAttribute = ChoType.GetMemberAttribute(memberInfo, typeof(ChoIgnoreMemberFormatterAttribute)) as ChoIgnoreMemberFormatterAttribute;
                                        if (memberFormatterIgnoreAttribute != null) continue;

										ChoMemberFormatterAttribute memberFormaterAttribute = ChoType.GetMemberAttribute(memberInfo, typeof(ChoMemberFormatterAttribute)) as ChoMemberFormatterAttribute;
                                        ChoMemberItemFormatterAttribute memberItemFormaterAttribute = ChoType.GetMemberAttribute(memberInfo, typeof(ChoMemberItemFormatterAttribute)) as ChoMemberItemFormatterAttribute;

                                        if (memberInfo is PropertyInfo)
                                        {
                                            MethodInfo getMethod = ((PropertyInfo)memberInfo).GetGetMethod(true);
                                            isPublicMember = getMethod != null && getMethod.IsPublic;
                                        }
                                        else if (memberInfo is FieldInfo)
                                            isPublicMember = ((FieldInfo)memberInfo).IsPublic;
                                        else
                                            continue;

                                        if (isPublicMember || (!isPublicMember && memberFormaterAttribute != null))
                                        {
                                            object memberValue = ChoType.GetMemberValue(target, memberInfo);
                                            if (memberValue == target)
                                                return null;

                                            string memberText = GetNFormatMemberValue(target, memberInfo, collectErrMsgs, memberFormaterAttribute, memberItemFormaterAttribute, bindingFlags);
                                            string memberName = memberFormaterAttribute == null || memberFormaterAttribute.Name.IsNullOrEmpty() ? memberInfo.Name : ChoPropertyManager.ExpandProperties(target, memberFormaterAttribute.Name);

                                            if (memberFormaterAttribute == null || !memberFormaterAttribute.CanFormat())
                                            {
                                                int count = 0;
                                                if (GetCountFromMsg(ref memberText, ref count))
                                                    msg.AppendFormatLine("{0} [Length: {2}]: {1}", memberName, memberText, count);
                                                else if (memberText.ContainsHeader())
                                                    msg.AppendFormatLine("{0}: {1}", memberName, memberText);
                                                else
                                                    msg.AppendFormatLine("{0}: {1}", memberName, memberText);
                                            }
                                            else if (memberFormaterAttribute.Name == ChoNull.NullString)
                                                msg.Append(memberFormaterAttribute.PostFormat(memberName, memberText));
                                            else
                                                msg.AppendFormat(memberFormaterAttribute.PostFormat(memberName, memberText));
                                        }
                                    }
                                }
                            }
                            //msg.AppendNewLine();
                            if (collectErrMsgs) GetErrorMsgs(target, msg);

                            return msg.ToString();
                        }
                    }
                    finally
                    {
                        if (interceptableObject != null)
                            interceptableObject.Dispose();
                    }
                }
            }
        }

		[ChoHiddenMember]
		private static string GetNFormatMemberValue(object target, MemberInfo memberInfo, bool collectErrMsgs, ChoMemberFormatterAttribute memberFormaterAttribute, 
            ChoMemberItemFormatterAttribute memberItemFormaterAttribute, BindingFlags bindingFlags)
        {
            try
            {
                string memberText = null;
                object memberValue = ChoType.GetMemberValue(target, memberInfo);
                if (memberValue == target)
                    return null;

                //if (memberFormaterAttribute == null)
                //{
                //    object[] typeConverters = ChoTypeDescriptor.GetTypeConverters(memberInfo);
                //    if (typeConverters != null && typeConverters.Length > 0)
                //    {
                //        if (typeConverters.Length == 1 && typeConverters[0].GetType() == typeof(System.ComponentModel.TypeConverter))
                //        {
                //        }
                //        else
                //        {
                //            object convertedValue = ChoConvert.ConvertTo(target, memberInfo, typeof(string)); //, typeConverters, ChoTypeDescriptor.GetTypeConverterParams(memberInfo), null);
                //            return convertedValue == null ? String.Empty : convertedValue.ToString();
                //            //return memberText;
                //        }
                //    }
                //}
                memberText = ChoFormattableObject.ToString(memberValue, collectErrMsgs, memberFormaterAttribute, memberItemFormaterAttribute, bindingFlags);
                if (memberText.ContainsMultiLines())
                    memberText = Environment.NewLine + memberText.Indent();
                return memberText;
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ChoApplicationException(memberInfo.Name, ex);
            }
        }

        [ChoHiddenMember]
        internal static string ToExtendedString(object target)
        {
            return ToString(target, true, null, null, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty);
        }

        #endregion Shared Members (Private)

        #region Instance Properties (Private)

        //private string DefaultFormatName
        //{
        //    get { return null; }
        //    set
        //    {
        //        ChoGuard.ArgumentNotNullOrEmpty(value, "DefaultFormatName");

        //        if (_gObjectFormatHandlers.ContainsKey(value))
        //            throw new ChoApplicationException(String.Format("{0} format routine not found.", value));

        //        _gDefaultFormatName = value;
        //    }
        //}

        #endregion Instance Properties (Private)

        #region IFormattable Members

		[ChoHiddenMember]
		public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider != null)
            {
                ICustomFormatter formatter = formatProvider.GetFormat(this.GetType()) as ICustomFormatter;

                if (formatter != null)
                    return formatter.Format(format, this, formatProvider);
            }

            if (format == null) return ToString();
            return ToString(format);
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool finalize)
		{
        }

        #endregion IDisposable Members
    }
}
