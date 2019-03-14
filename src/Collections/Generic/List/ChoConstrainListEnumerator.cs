namespace Cinchoo.Core.Collections.Generic.List
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    #endregion NameSpaces

    public class ChoConstrainListEnumerator<T> : ChoBaseListEnumerator<T>, IEnumerable
    {
        #region Instance Data Members (Private)

        private readonly int _count;
        private readonly int _startIndex;
        private readonly IList<T> _baseList;
        private readonly Predicate<T> _match;
        private readonly IEnumerable<T> _enumerable;
        private readonly int _listCount;
        private readonly int _maxNoOfIteration = Int32.MaxValue;

        private IEnumerator<T> _enumerator;
        private int _currentIndex;
        private int _counter;
        private int _currentIteration;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoConstrainListEnumerator(IEnumerable<T> enumerable, Predicate<T> match)
        {
            ChoGuard.ArgumentNotNull(enumerable, "enumerable");

            _enumerable = enumerable;
            _match = match;

            Reset();
        }

        public ChoConstrainListEnumerator(IList<T> list, Predicate<T> match)
            : this(list, ChoListEnumeratorConst.DefaultStartIndex, _defaultMatch)
        {
        }

        public ChoConstrainListEnumerator(IList<T> list, int startIndex, Predicate<T> match)
            : this(list, startIndex, ChoListEnumeratorConst.DefaultCount, match)
        {
        }

        public ChoConstrainListEnumerator(IList<T> list, int startIndex, int count, Predicate<T> match)
        {
            ChoGuard.ArgumentNotNull(list, "list");
            ChoGuard.ArgumentNotNull(match, "match");

            _baseList = list;
            _match = match;
            _listCount = _baseList.Count;
            _startIndex = startIndex == ChoListEnumeratorConst.DefaultStartIndex ? 0 : startIndex;
            _count = count == ChoListEnumeratorConst.DefaultCount ? _listCount : _count;

            list.CheckIndex<T>(_startIndex);
            list.CheckRange<T>(_listCount == 0 ? 0 : _startIndex, _count);

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
                if (_enumerator != null)
                {
                    if (!_enumerator.MoveNext())
                    {
                        if (_currentIteration == _maxNoOfIteration)
                            return false;
                        else
                            _currentIteration++;

                        _enumerator = _enumerable.GetEnumerator();
                        continue;
                    }

                    if (_match(_enumerator.Current))
                        return true;
                }
                else
                {
                    if (_listCount == 0)
                        return false;

                    if (_currentIndex == _listCount - 1 || _count != ChoListEnumeratorConst.DefaultCount && _counter == _count)
                    {
                        if (_currentIteration == _maxNoOfIteration)
                            return false;
                        else
                            _currentIteration++;

                        _currentIndex = -1;
                        _counter = 0;
                        continue;
                    }
                    else
                        _currentIndex++;

                    if (_currentIndex < _startIndex)
                        continue;

                    if (_match(Current))
                    {
                        if (_count != ChoListEnumeratorConst.DefaultCount)
                            _counter++;
                        return true;
                    }
                }
            }
        }

        public override void Reset()
        {
            _currentIteration = 1;

            if (_enumerable != null)
                _enumerator = _enumerable.GetEnumerator();
            else
            {
                _counter = 0;
                _currentIndex = -1;
            }
        }

        #endregion

        #region Instance Properties (Public)

        public int Iteration
        {
            get { return _currentIteration; }
        }

        #endregion Instance Properties (Public)

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        #endregion
    }
}
