using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cinchoo.Core.Forms
{
    public class ChoWaitCursor : IDisposable
    {
        private Cursor _previousCursor;

        public ChoWaitCursor()
        {
            _previousCursor = Cursor.Current;

            Cursor.Current = Cursors.WaitCursor;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Cursor.Current = _previousCursor;
        }

        #endregion
    }
}
