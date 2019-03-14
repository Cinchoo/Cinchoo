using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Cinchoo.Core.Collections
{
    public class ChoEnumerable : IEnumerable
    {
        private readonly IEnumerator _enumerator;

        public ChoEnumerable(IEnumerator enumerator)
        {
            ChoGuard.ArgumentNotNull(enumerator, "Enumerator");

            _enumerator = enumerator;
        }

        public IEnumerator GetEnumerator()
        {
            return _enumerator;
        }
    }

    public class ChoEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public ChoEnumerable(IEnumerator<T> enumerator)
        {
            ChoGuard.ArgumentNotNull(enumerator, "Enumerator");

            _enumerator = enumerator;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
