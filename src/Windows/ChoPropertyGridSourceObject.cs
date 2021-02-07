using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cinchoo.Core.Windows
{
    public interface IChoPropertyGridSourceObject
    {
        void SetParams(string cmdParams);
        string GetParams();
    }

    public class ChoPropertyGridSourceObject : IChoPropertyGridSourceObject
    {
        public virtual void SetParams(string cmdParams)
        {
            foreach (Tuple<string, string> tuple in ChoStringEx.ToKeyValuePairs(cmdParams, ';', '='))
            {
                MemberInfo memberInfo = ChoType.GetMemberInfo(this.GetType(), tuple.Item1);
                if (memberInfo != (MemberInfo)null && ChoType.GetAttribute<BrowsableAttribute>(memberInfo, false) == null)
                    ChoType.ConvertNSetMemberValue((object)this, tuple.Item1, (object)DenormalizeString(tuple.Item2));
            }
        }

        public virtual string GetParams()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Tuple<string, object> tuple in ChoObjectEx.AsDictionary((object)this, false, false))
            {
                if (stringBuilder.Length > 0)
                    stringBuilder.Append(";");
                MemberInfo memberInfo = ChoType.GetMemberInfo(this.GetType(), tuple.Item1);
                if (memberInfo != (MemberInfo)null && ChoType.GetAttribute<BrowsableAttribute>(memberInfo, false) == null)
                    stringBuilder.AppendFormat(ChoStringEx.FormatString("{0}={1}", (object)tuple.Item1, NormalizeString(tuple.Item2 != null ? tuple.Item2.ToString() : string.Empty)));
            }
            return stringBuilder.ToString();
        }

        private string DenormalizeString(string inText)
        {
            if (inText.IsNullOrWhiteSpace() || !inText.StartsWith("'") || !inText.EndsWith("'"))
                return inText;
            return inText.Substring(1, inText.Length - 2);
        }

        private string NormalizeString(string inText)
        {
            if (!inText.IsNullOrWhiteSpace() && inText.Contains(';'))
                return "'{0}'".FormatString(inText);

            return inText;
        }
    }
}
