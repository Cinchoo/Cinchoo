namespace Cinchoo.Core.Collections.Generic.List
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Collections;

	#endregion NameSpaces

	public struct ChoListEnumeratorConst
	{
		#region Constants

		public const int DefaultStartIndex = Int32.MinValue;
		public const int DefaultCount = Int32.MaxValue;

		#endregion Constants
	}

    public abstract class ChoBaseListEnumerator<T> : ChoSyncDisposableObject, IEnumerator<T>, IEnumerable
	{
		#region Shared Data Members (Private)

		protected static readonly Predicate<T> _defaultMatch = item => true;

		#endregion Shared Data Members (Private)

		#region IEnumerator Members (Public)

		public abstract T Current
		{
			get;
		}

		object IEnumerator.Current
		{
			get
			{
				ChoGuard.NotDisposed(this);

				return Current;
			}
		}

		public abstract bool MoveNext();
		public abstract void Reset();

		#endregion IEnumerator Members (Public)

		#region ChoSyncDisposableObject Overrides

		protected override void Dispose(bool finalize)
		{
			Reset();
		}

		#endregion ChoSyncDisposableObject Overrides

		#region Instance Members (Public)

		public IEnumerable<T> Values
		{
			get
			{
				Reset();
				while (MoveNext())
				{
					yield return Current;
				}
			}
		}

		#endregion Instance Members (Public)

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        #endregion
    }
}
