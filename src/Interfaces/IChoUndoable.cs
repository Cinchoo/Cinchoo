using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public interface IChoUndoable<T>
    {
        T Capture();
        void Undo(T source);
    }
}
