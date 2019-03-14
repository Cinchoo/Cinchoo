namespace eSquare.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using eSquare.Core.Types;

    #endregion NameSpaces

    public class ChoPropertyManager
    {
        #region Shared Data Members (Private)

        private const string ApplicationName = "APPLICATION_NAME";
        private const string MachineName = "MACHINE_NAME";
        private const string ProcessId = "PROCESS_ID";
        private const string ThreadId = "THREAD_ID";
        private const string ThreadName = "THREAD_NAME";
        private const string RandomNo = "RANDOM_NO";
        private const string DateTime = "DATE_TIME";
        private const string Date = "DATE";
        private const string Time = "TIME";

        #endregion Shared Data Members (Private)

        #region Shared Members (Public)

        public static string Translate(string inString)
        {
            if (String.IsNullOrEmpty(inString)) return inString;

            return ChoString.Replace(inString, new ChoString.PropertyReplaceHandler(_Translate));
        }

        #endregion Shared Members (Public)

        private static string _Translate(string token, string format)
        {
            if (String.IsNullOrEmpty(token)) return token;

            switch (token)
            {
                case ApplicationName:
                    return ChoObject.Format(ChoAppSettings.Me.ApplicationId, format);
                case MachineName:
                    return ChoObject.Format(Environment.MachineName, format);
                case ProcessId:
                    return ChoObject.Format(ChoSystemInfo.ProcessId, format);
                case ThreadId:
                    return ChoObject.Format(ChoSystemInfo.GetThreadId(), format);
                case ThreadName:
                    return ChoObject.Format(ChoSystemInfo.GetThreadName(), format);
                case RandomNo:
                    return ChoObject.Format(ChoRandom.NextRandom().ToString(), format);
                case DateTime:
                    return ChoObject.Format(System.DateTime.Now, format);
                case Date:
                    if (String.IsNullOrEmpty(format))
                        return System.DateTime.Today.ToShortDateString();
                    else
                        return ChoObject.Format(System.DateTime.Today, format);
                case Time:
                    if (String.IsNullOrEmpty(format))
                        return System.DateTime.Now.ToShortTimeString();
                    else
                        return ChoObject.Format(System.DateTime.Now, format);
                default:
                    return ChoObject.Format(token, format);
            }
        }

    }
}
