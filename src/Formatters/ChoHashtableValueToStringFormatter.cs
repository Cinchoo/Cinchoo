namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using Cinchoo.Core.Text;
    using System.Collections;

    #endregion NameSpaces

    public class ChoHashtableValueToStringFormatter : IChoMemberFormatter
    {
        #region IChoMemberFormatter Members

        public string Format(object value, bool indentMsg)
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder();
            if (((Hashtable)value).Count == 0)
                msg.Append(ChoString.EmptyString);
            else
            {
                msg.AppendLine();
                if (indentMsg)
                {
                    msg.AppendLine("[");
                    foreach (object token in ((Hashtable)value).Values)
                        msg.AppendFormatLine(ChoObject.ToString(token));
                    msg.Append("]");
                }
                else
                {
                    foreach (object token in ((Hashtable)value).Values)
                        msg.AppendLine(ChoObject.ToString(token));
                }
            }

            return msg.ToString();
        }

        public bool CanFormat(Type sourceType)
        {
            if (typeof(Hashtable).IsAssignableFrom(sourceType))
                return true;
            else
                return false;
        }

        #endregion
    }
}
