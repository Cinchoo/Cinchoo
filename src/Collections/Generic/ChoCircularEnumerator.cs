namespace Cinchoo.Core.Collections.Generic
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;

    #endregion NameSpaces

    public class ChoCircularEnumerator<T> : ChoBaseEnumerator<T>, IEnumerable
    {
        #region Instance Data Members (Private)

        private readonly IEnumerable<T> _enumerable;
        private readonly int _maxNoOfIteration = Int32.MaxValue;

        private IEnumerator<T> _enumerator;
        private int _currentIteration;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoCircularEnumerator(IEnumerable<T> enumerable, int maxNoOfIteration)
        {
            ChoGuard.ArgumentNotNull(enumerable, "enumerable");

            if (maxNoOfIteration <= 0)
                throw new ArgumentException("maxNoOfIteration must be positive.");

            _maxNoOfIteration = maxNoOfIteration;
            _enumerable = enumerable;

            Reset();
        }

        #endregion Constructors

        #region IEnumerator<T> Members

        public override T Current
        {
            get
            {
                ChoGuard.NotDisposed(this);
                return _enumerator.Current;
            }
        }

        public override bool MoveNext()
        {
            ChoGuard.NotDisposed(this);

            while (true)
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
            }
        }

        public override void Reset()
        {
            _currentIteration = 1;
            _enumerator = _enumerable.GetEnumerator();
        }

        #endregion

        #region Instance Properties (Public)

        public int Iteration
        {
            get { return _currentIteration; }
        }

        #endregion Instance Properties (Public)
    }
}
