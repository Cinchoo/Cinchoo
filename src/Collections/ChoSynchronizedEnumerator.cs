namespace Cinchoo.Core.Collections
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;
    using System.Collections;

	#endregion NameSpaces

	#region ChoSynchronizedEnumerator Class

    public sealed class ChoSynchronizedEnumerator : ChoBaseEnumerator
	{
		#region Instance Data Members (Private)

        private readonly IEnumerable _enumerable;
        private readonly object _syncObject;
        private IEnumerator _enumerator;

		#endregion Instance Data Members (Private)

		#region Constructors

		// Constructor.
		public ChoSynchronizedEnumerator(IEnumerable enumerable, object syncObject)
		{
            ChoGuard.ArgumentNotNull(enumerable, "Enumerable");
			ChoGuard.ArgumentNotNull(syncObject, "SyncObject");

			_syncObject = syncObject;

			Monitor.Enter(_syncObject);

			_enumerable = enumerable;
            Reset();
		}

		#endregion Constructors

		#region ChoSyncDisposableObject Overrides

		protected override void Dispose(bool finalize)
		{
            base.Dispose(finalize);
			Monitor.Exit(_syncObject);
		}

		#endregion ChoSyncDisposableObject Overrides

		#region IEnumerator Members

        public override object Current
		{
            get
            {
                ChoGuard.NotDisposed(this);
                return _enumerator.Current;
            }
		}

		#endregion

		#region IEnumerator Members

        public override bool MoveNext()
        {
            ChoGuard.NotDisposed(this);

            return _enumerator.MoveNext();
		}

        public override void Reset()
        {
            _enumerator = _enumerable.GetEnumerator();
        }

		#endregion
	};

	#endregion ChoSynchronizedEnumerator Class
}
