using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class IChoCollectionEx
    {
        public static bool IsNullOrEmpty(this ICollection @this)
        {
            return @this == null || @this.Count == 0;
        }
    }
}
