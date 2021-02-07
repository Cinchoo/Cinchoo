using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core.Threading
{
    public static class ChoMutexHelper
    {
        public static string GetName(string name)
        {
            return @"Global\{0}_Mutex".FormatString(name);
        }
    }
}
