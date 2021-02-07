using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public interface IChoScriptExtensionObject
    {
        event EventHandler<EventArgs> ScriptTextChanged;
        string ScriptText
        {
            get;
            set;
        }
        bool IsScriptReadonly
        {
            get;
        }
    }
}
