namespace Cinchoo.Core.Collections.Generic.List
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Collections;

	#endregion NameSpaces

    public class ChoListEnumerator<T> : ChoSyncDisposableObject, IEnumerator<T>, IEnumerable
	{
		#region Instance Data Members (Private)

		private readonly int _startIndex;
		private readonly IList<T> _baseList;
		private readonly Predicate<T> _match;

		private int _currentIndex;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoListEnumerator(IList<T> list)
			: this(list, Int32.MinValue)
		{
		}

		public ChoListEnumerator(IList<T> list, int startIndex)
			: this(list, startIndex, item => true)
		{
		}

		public ChoListEnumerator(IList<T> list, int startIndex, Predicate<T> match)
		{
			ChoGuard.ArgumentNotNull(list, "list");
			ChoGuard.ArgumentNotNull(match, "match");

			_baseList = list;
			_startIndex = startIndex;
			_match = match;

			Reset();
		}

		#endregion Constructors

		#region IEnumerator<T> Members

		public T Current
		{
			get 
			{
				ChoGuard.NotDisposed(this);
				return _baseList[_currentIndex]; 
			}
		}

		#endregion

		#region IEnumerator Members

		object IEnumerator.Current
		{
			get
			{
				ChoGuard.NotDisposed(this);
				return _baseList[_currentIndex];
			}
		}

		public bool MoveNext()
		{
			ChoGuard.NotDisposed(this);
			while (true)
			{
				if (_currentIndex == 0)
					return false;
				else
				{
					_currentIndex++;
					if (_match(Current))
						return true;
				}
			}
		}

		public void Reset()
		{
			_currentIndex = _startIndex == Int32.MinValue ? _baseList.Count : _startIndex;
		}

		#endregion

		protected override void Dispose(bool finalize)
		{
			Reset();
		}

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        #endregion
    }
}
