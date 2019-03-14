namespace eSquare.Core.Property
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using eSquare.Core.Common;

    #endregion NameSpaces

    public class ChoGlobalPropertyHandlerReplacer : IChoPropertyReplacer
    {
        #region Constants

        private const string ApplicationName = "APPLICATION_NAME";
        private const string MachineName = "MACHINE_NAME";
        private const string ProcessId = "PROCESS_ID";
        private const string ThreadId = "THREAD_ID";
        private const string ThreadName = "THREAD_NAME";
        private const string RandomNo = "RANDOM_NO";
        private const string DateTime = "DATE_TIME";
        private const string Date = "DATE";
        private const string Time = "TIME";

        #endregion Constants

        #region IChoPropertyReplacer Members

        public bool Contains(string propertyName)
        {
            return false;
        }

        public string Replace(string propertyName, string format)
        {
            if (String.IsNullOrEmpty(propertyName)) return propertyName;

            switch (propertyName)
            {
                case ApplicationName:
                    return ChoObject.Format(ChoAppSettings.Me.ApplicationId, format);
                case MachineName:
                    return ChoObject.Format(Environment.MachineName, format);
                case ProcessId:
                    return ChoObject.Format(ChoApplicationInfo.ProcessId, format);
                case ThreadId:
                    return ChoObject.Format(ChoApplicationInfo.GetThreadId(), format);
                case ThreadName:
                    return ChoObject.Format(ChoApplicationInfo.GetThreadName(), format);
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
                    return ChoObject.Format(propertyName, format);
            }
        }

        #endregion
    }
}
