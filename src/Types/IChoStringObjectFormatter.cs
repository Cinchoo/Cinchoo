using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public interface IChoStringObjectFormatter
    {
        string ToFormattedString();
        string GetHelpText();
    }

    public interface IChoStringObjectFormatter<T> : IChoStringObjectFormatter
    {
        T Value { get; }
    }
}
