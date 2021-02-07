namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.Text;

    #endregion NameSpaces

    [ChoBufferProfile("Property Manager Errors", NameFromTypeFullName = typeof(ChoPropertyManager))]
    public static class ChoPropertyManager
    {
        #region Shared Members (Public)

        public static string ExpandProperties(object target, string inString)
        {
            if (String.IsNullOrEmpty(inString)) return inString;
            return ChoString.ExpandProperties(target, inString);
        }

        public static string FormatException(string expr, Exception ex)
        {
            //ChoProfile.WriteLine("Expression: {1}{0}Error: {2}", Environment.NewLine, expr, ex.ToString());
            return String.Format("{2} Error while evaluating `{0}' expression. {1}]", expr, ex.Message, ChoString.ExceptionStringToken);
        }

        #endregion Shared Members (Public)

        #region Object Overloads

        public new static string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("Available Properties");

            foreach (var propertyReplacer in ChoPropertyManagerSettings.Me.PropertyReplacers)
            {
                ChoStringMsgBuilder msg1 = new ChoStringMsgBuilder("{0} Property Replacer".FormatString(propertyReplacer.Name));

                bool found = false;
                if (propertyReplacer.AvailablePropeties != null)
                {
                    found = false;
                    foreach (KeyValuePair<string, string> keyValue in propertyReplacer.AvailablePropeties)
                    {
                        found = true;
                        msg1.AppendFormatLine("{0} - {1}".FormatString(keyValue.Key, keyValue.Value));
                    }
                }
                
                if (!found)
                    msg1.AppendFormatLine("No properties found. / Failed to enumerate properties.");

                msg.AppendFormatLine(msg1.ToString());
            }

            return msg.ToString();
        }

        #endregion
    }
}
