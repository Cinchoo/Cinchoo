using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core.Collections
{
    public class ChoEnumerator<T> : IEnumerator<T> where T: class
    {
        private readonly Func<T, T> _moveNextFunc;
        private readonly T _startValue;
        private T _nextValue;

        public ChoEnumerator(Func<T, T> moveNextFunc, T startValue)
        {
            ChoGuard.ArgumentNotNull(moveNextFunc, "MoveNextFunc");

            _moveNextFunc = moveNextFunc;
            _startValue = startValue;
        }

        public T Current
        {
            get { return _nextValue; }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            if (_nextValue == null)
                _nextValue = _startValue;
            else
                _nextValue = _moveNextFunc(_nextValue);

            return _nextValue != null;
        }

        public void Reset()
        {
            _nextValue = null;
        }
    }
}
