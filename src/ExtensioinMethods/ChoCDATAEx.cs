using Cinchoo.Core.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class ChoCDATAEx
    {
        public static bool IsNullOrWhiteSpace(this ChoCDATA data)
        {
            return data == null || data.Value.IsNullOrWhiteSpace();
        }

        public static string GetValue(this ChoCDATA data)
        {
            return data.IsNull() ? String.Empty : data.Value;
        }
    }

}
