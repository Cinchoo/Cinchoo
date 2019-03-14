namespace eSquare.Core.Formatters
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using eSquare.Core.Text;

    #endregion NameSpaces

    public class ChoStringArrayFormatter : IChoMemberFormatter
    {
        #region IChoMemberFormatter Members

        public string Format(object value)
        {
            if (!CanFormat(value.GetType()))
                return value.ToString();
            else
            {
                ChoStringMsgBuilder msg = new ChoStringMsgBuilder();
                msg.AppendLine();
                msg.AppendLine("[");
                foreach (string token in value as string[])
                {
                    msg.AppendFormatLine(token);
                }
                msg.AppendLine("]");

                return msg.ToString();
            }
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
