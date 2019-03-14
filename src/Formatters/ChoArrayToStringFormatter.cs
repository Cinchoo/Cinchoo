namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using Cinchoo.Core.Text;
    using Cinchoo.Core;

    #endregion NameSpaces

    [ChoStringObjectFormattable]
    public class ChoArrayToStringFormatter : IChoMemberFormatter
    {
        #region IChoMemberFormatter Members

        public string Format(object value, bool indentMsg)
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder();
            if (((Array)value).Length == 0) 
                msg.AppendLine(ChoString.EmptyString);
            else
            {
                if (indentMsg)
                {
                    msg.AppendLine("[");
                    foreach (object token in (Array)value)
                    {
                        msg.AppendFormatLine(ChoObject.ToString(token));
                    }
                    msg.AppendLine("]");
                }
                else
                {
                    foreach (object token in (Array)value)
                    {
                        msg.AppendLine(ChoObject.ToString(token));
                    }
                }
            }

            return msg.ToString();
        }

        public bool CanFormat(Type sourceType)
        {
            if (typeof(Array).IsAssignableFrom(sourceType))
                return true;
            else
                return false;
        }

        #endregion

        private readonly static ChoArrayToStringFormatter _arrayToStringFormatter = new ChoArrayToStringFormatter();

        [ChoCanObjectFormattable]
        public static bool CanFormat(object value)
        {
            if (value == null) return false;
            return value is Array;
        }

        [ChoObjectFormatter]
        public static string Format(object value, string format)
        {
            if (value == null) return null;

            if (!CanFormat(value))
                throw new ArgumentException(String.Format("Failed to format object of {0} type.", value.GetType().FullName));

            return _arrayToStringFormatter.Format(value, false);
        }
    }
}
