using System;
using System.Collections.Generic;
using System.Text;

namespace Cinchoo.Core
{
    public interface IChoMemberFormatter
    {
        string Format(object value, bool indentMsg);
        bool CanFormat(System.Type sourceType);
    }
}
