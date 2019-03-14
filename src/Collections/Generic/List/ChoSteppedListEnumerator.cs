namespace Cinchoo.Core.Collections.Generic.List
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Collections;

	#endregion NameSpaces

	public class ChoSteppedListEnumerator<T> : ChoBaseListEnumerator<T>, IEnumerable
	{
		#region Instance Data Members (Private)

		private readonly int _count;
		private readonly int _startIndex;
		private readonly IList<T> _baseList;
		private readonly Predicate<T> _match;
		private readonly IEnumerable<T> _enumerable;
		private readonly int _listCount;
		private readonly int _step = 1;

		private IEnumerator<T> _enumerator;
		private int _currentIndex;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoSteppedListEnumerator(IEnumerable<T> enumerable)
			: this(enumerable, Int32.MaxValue)
		{
		}

		public ChoSteppedListEnumerator(IEnumerable<T> enumerable, int step)
			: this(enumerable, step, _defaultMatch)
		{
		}

		public ChoSteppedListEnumerator(IEnumerable<T> enumerable, int step, Predicate<T> match)
		{
			ChoGuard.ArgumentNotNull(enumerable, "enumerable");

			if (step <= 0)
				throw new ArgumentException("step must be positive.");

			_step = step;
			_enumerable = enumerable;
			_match = match;

			Reset();
		}

		public ChoSteppedListEnumerator(IList<T> list)
			: this(list, _defaultMatch)
		{
		}

		public ChoSteppedListEnumerator(IList<T> list, Predicate<T> match)
			: this(list, ChoListEnumeratorConst.DefaultStartIndex, _defaultMatch)
		{
		}

		public ChoSteppedListEnumerator(IList<T> list, int startIndex)
			: this(list, startIndex, _defaultMatch)
		{
		}

		public ChoSteppedListEnumerator(IList<T> list, int startIndex, Predicate<T> match)
			: this(list, startIndex, ChoListEnumeratorConst.DefaultCount, match)
		{
		}

		public ChoSteppedListEnumerator(IList<T> list, int startIndex, int count)
			: this(list, startIndex, count, ChoListEnumeratorConst.DefaultCount, _defaultMatch)
		{
		}

		public ChoSteppedListEnumerator(IList<T> list, int startIndex, int count, Predicate<T> match)
			: this(list, startIndex, count, ChoListEnumeratorConst.DefaultCount, match)
		{
		}

		public ChoSteppedListEnumerator(IList<T> list, int startIndex, int count, int step)
			: this(list, startIndex, count, step, _defaultMatch)
		{
		}

		public ChoSteppedListEnumerator(IList<T> list, int startIndex, int count, int step, Predicate<T> match)
		{
			ChoGuard.ArgumentNotNull(list, "list");
			ChoGuard.ArgumentNotNull(match, "match");

			if (step <= 0)
				throw new ArgumentException("step must be positive.");

			_baseList = list;
			_match = match;
			_listCount = _baseList.Count;
			_startIndex = startIndex == ChoListEnumeratorConst.DefaultStartIndex ? 0 : startIndex;
			_count = count == ChoListEnumeratorConst.DefaultCount ? _listCount : _count;
			_step = step;

			list.CheckIndex<T>(_startIndex);
			list.CheckRange<T>(_startIndex, _count);

			Reset();
		}

		#endregion Constructors

		#region IEnumerator<T> Members

		public override T Current
		{
			get
			{
				ChoGuard.NotDisposed(this);

				if (_enumerator != null)
					return _enumerator.Current;
				else
					return _baseList[_currentIndex];
			}
		}

		public override bool MoveNext()
		{
			ChoGuard.NotDisposed(this);

			while (true)
			{
				_currentIndex++;

				if (_enumerator != null)
				{
					if (!_enumerator.MoveNext())
						return false;
				}
				else
				{
					if (_listCount == 0)
						return false;

					if (_currentIndex == _listCount - 1)
						return false;

					if (_currentIndex < _startIndex)
						continue;
				}

				if ((_currentIndex - _startIndex) % _step == 0)
				{
					if (_match(Current))
						return true;
				}
			}
		}

		public override void Reset()
		{
			if (_enumerable == null)
				_currentIndex = -1;
			else
				_enumerator = _enumerable.GetEnumerator();
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
