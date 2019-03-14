namespace Cinchoo.Core.Services
{
    #region NameSpaces

    using System;
    

    #endregion NameSpaces

    [ChoTypeFormatter("Execution Service Data")]
    internal class ChoExecutionServiceData : ChoObject
    {
        #region Instance Data Members (Public)

        public readonly string DelegateReadableName;
		public readonly Delegate Func;
        [ChoMemberFormatter("Parameters", Formatter = typeof(ChoArrayToStringFormatter))]
        public readonly object[] Parameters;
        public readonly int Timeout;
        public readonly IChoAsyncResult Result;
        public readonly int MaxNoOfRetry = ChoAPMSettings.Me.MaxNoOfRetry;
        public readonly int SleepBetweenRetry = ChoAPMSettings.Me.SleepBetweenRetry;

        #endregion Instance Data Members (Public)

        #region Constructors

        public ChoExecutionServiceData(Delegate func, object[] parameters, int timeout, ChoAsyncResultBase result, int maxNoOfRetry, int sleepBetweenRetry)
            : this(ChoDelegateEx.ToString(func), func, parameters, timeout, result, maxNoOfRetry, sleepBetweenRetry)
        {
        }

        public ChoExecutionServiceData(string delegateReadableName, Delegate func, object[] parameters, int timeout, ChoAsyncResultBase result, int maxNoOfRetry, int sleepBetweenRetry)
        {
            DelegateReadableName = delegateReadableName;
            Func = func;
            Parameters = parameters;
            Timeout = timeout;
            Result = result;
            MaxNoOfRetry = maxNoOfRetry;
            SleepBetweenRetry = sleepBetweenRetry;
        }

        #endregion Constructors
    }
}
