using Cinchoo.Core;
using Cinchoo.Core.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class ChoExceptionEx
    {
        public static void HandleException(this Exception ex, Exception newEx)
        {
            if (ex is ChoFatalApplicationException)
            {
                throw ex;
            }
            else if (ex is ChoCommandLineArgException
                || ex is ChoCommandLineArgUsageException)
            {
                ChoApplication.DisplayMsg(ex.Message);
            }
            else
            {
                if (newEx == null)
                {
                    ChoApplication.DisplayMsg(ex.Message, ex);
                    throw ex;
                }
                else
                {
                    throw newEx;
                }
            }
        }
    }
}
