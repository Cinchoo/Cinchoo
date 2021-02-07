using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Cinchoo.Core.Collections
{
    public sealed class ChoEmptyEnumerable : IEnumerable
    {
        public static readonly IEnumerable Instance = new ChoEmptyEnumerable();

        private ChoEmptyEnumerable()
        {
        }

        public IEnumerator GetEnumerator()
        {
            return ChoEmptyEnumerator.Instance;
        }
    }

    public sealed class ChoEmptyEnumerator : IEnumerator
    {
        public static readonly IEnumerator Instance = new ChoEmptyEnumerator();

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
    }
}