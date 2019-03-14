namespace Cinchoo.Core.Collections.Generic.List
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Collections;

	#endregion NameSpaces

	public class ChoReverseListEnumerator<T> : ChoBaseListEnumerator<T>, IEnumerable
	{
		#region Instance Data Members (Private)

		private readonly int _count;
		private readonly int _startIndex;
		private readonly IList<T> _baseList;
		private readonly Predicate<T> _match;

		private int _currentIndex;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoReverseListEnumerator(IList<T> list)
			: this(list, ChoListEnumeratorConst.DefaultStartIndex)
		{
		}

		public ChoReverseListEnumerator(IList<T> list, Predicate<T> match)
			: this(list, ChoListEnumeratorConst.DefaultStartIndex, ChoListEnumeratorConst.DefaultCount, match)
		{
		}

		public ChoReverseListEnumerator(IList<T> list, int startIndex)
			: this(list, startIndex, ChoListEnumeratorConst.DefaultCount)
		{
		}

		public ChoReverseListEnumerator(IList<T> list, int startIndex, Predicate<T> match)
			: this(list, startIndex, ChoListEnumeratorConst.DefaultCount, match)
		{
		}

		public ChoReverseListEnumerator(IList<T> list, int startIndex, int count)
			: this(list, startIndex, count, _defaultMatch)
		{
		}

		public ChoReverseListEnumerator(IList<T> list, int startIndex, int count, Predicate<T> match)
		{
			ChoGuard.ArgumentNotNull(list, "list");
			ChoGuard.ArgumentNotNull(match, "match");

			_baseList = list;
			_match = match;

			int listCount = _baseList.Count;
			_startIndex = startIndex == ChoListEnumeratorConst.DefaultStartIndex ? (listCount == 0 ? 0 : listCount - 1) : startIndex;
			_count = count == ChoListEnumeratorConst.DefaultCount ? _startIndex : count;

			list.CheckIndex<T>(_startIndex);
			list.CheckRange<T>(listCount == 0 ? 0 : listCount - _startIndex - 1, _count);

			Reset();
		}

		#endregion Constructors

		#region IEnumerator<T> Members

		public override T Current
		{
			get
			{
				ChoGuard.NotDisposed(this);

				return _baseList[_currentIndex];
			}
		}

		public override bool MoveNext()
		{
			ChoGuard.NotDisposed(this);

			while (true)
			{
				if (_currentIndex == 0)
					return false;

				if (_currentIndex == _startIndex - _count - 1)
					return false;

				_currentIndex--;

				if (_match(Current))
					return true;
			}
		}

		public override void Reset()
		{
			_currentIndex = _startIndex == 0 ? 0 : _startIndex + 1;
		}

		#endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        #endregion
    }
}
