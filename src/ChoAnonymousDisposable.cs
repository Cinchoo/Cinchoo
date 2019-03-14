using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    class ChoAnonymousDisposable : IDisposable
    {
        private readonly Action _dispose;

        public ChoAnonymousDisposable(Action dispose)
        {
            ChoGuard.ArgumentNotNull(dispose, "Dispose");

            this._dispose = dispose;
        }

        public void Dispose()
        {
            _dispose();
        }
    }
}