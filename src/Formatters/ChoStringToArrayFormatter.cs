namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using Cinchoo.Core.Text;

    #endregion NameSpaces

    public class ChoStringToArrayFormatter : IChoMemberFormatter
    {
        #region IChoMemberFormatter Members

        public string Format(object value, bool indentMsg)
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder();
            msg.AppendLine();
            if (indentMsg)
            {
                msg.AppendLine("[");
                foreach (string token in value as string[])
                {
                    msg.AppendFormatLine(token);
                }
                msg.AppendLine("]");
            }
            else
            {
                foreach (string token in value as string[])
                {
                    msg.AppendLine(token);
                }
            }
            return msg.ToString();
        }

        public bool CanFormat(Type sourceType)
        {
            if (sourceType == typeof(string[]))
                return true;
            else
                return false;
        }

        #endregion
    }
}
