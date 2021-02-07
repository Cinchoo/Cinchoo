using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Cinchoo.Core.Collections.Generic
{
    public sealed class ChoEmptyEnumerable<T> : IEnumerable<T>
    {
        public static readonly IEnumerable<T> Instance = new ChoEmptyEnumerable<T>();

        private ChoEmptyEnumerable()
        {
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ChoEmptyEnumerator<T>.Instance;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public sealed class ChoEmptyEnumerator<T> : IEnumerator<T>
    {
        public static readonly IEnumerator<T> Instance = new ChoEmptyEnumerator<T>();

        private ChoEmptyEnumerator()
        {
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }

        public object Current
        {
            get { throw new InvalidOperationException("No elements"); }
        }

        T IEnumerator<T>.Current
        {
            get { throw new InvalidOperationException("No elements"); }
        }

        public void Dispose()
        {
        }
    }
}