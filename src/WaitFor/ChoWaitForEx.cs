namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Services;

    #endregion NameSpaces

    public static class ChoWaitForEx
    {
        #region ChoFuncWaitFor<TResult> Overloads

        public static object Run(this Delegate func, object[] args)
        {
            return Run(func, args, Timeout.Infinite);
        }

        public static object Run(this Delegate func, object[] args, int timeout)
        {
            return Run(func, args, timeout, ChoAPMSettings.Me.MaxNoOfRetry);
        }

        public static object Run(this Delegate func, object[] args, int timeout, int maxNoOfRetry)
        {
            return Run(func, args, timeout, maxNoOfRetry, ChoAPMSettings.Me.SleepBetweenRetry);
        }

        public static object Run(this Delegate func, object[] args, int timeout, int maxNoOfRetry, int sleepBetweenRetry)
        {
            using (ChoWaitFor waitFor = new ChoWaitFor(func))
                return waitFor.Run(args, timeout, maxNoOfRetry, sleepBetweenRetry);
        }

        #endregion ChoFuncWaitFor<TResult> Overloads
    }
}
