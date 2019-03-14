namespace eSquare.Core.Collections.Generic
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using eSquare.Core.IO;

    #endregion NameSpaces

    [Serializable]
    public class ChoQueue<T> : ICollection<T>, IEnumerable<T>, IDisposable
    {
        #region Instance Data Members (Private)

        private bool _disposed = false;
        private int _capacity = Int32.MaxValue;
        private Queue<T> _internalQueue;
        private Queue<string> _persistanceQueue = new Queue<string>();
        private List<string> _commonQueue = new List<string>();
        private string _storageDir;

        #endregion

        #region Constructors

        public ChoQueue()
            : this(0)
        {
        }

        public ChoQueue(int capacity)
            : this(capacity, null)
        {
        }

        public ChoQueue(int capacity, string storageDir)
        {
            if (capacity <= 0)
                _internalQueue = new Queue<T>();
            else
            {
                _capacity = capacity;
                _internalQueue = new Queue<T>(capacity);
            }

            if (storageDir == null)
                _storageDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ChoPath.GetRandomFileName());
            else
                _storageDir = Path.Combine(storageDir, ChoPath.GetRandomFileName());
        }

        #endregion

        #region Instance Members (Internal)

        internal virtual object GetElement(int index)
        {
            if (index < _internalQueue.Count)
                return _commonQueue[index];

            if (index - _internalQueue.Count < _persistanceQueue.Count)
                return ChoObject.Deserialize(_storageDir, _commonQueue[index] as string);

            return null;
        }

        internal void Dispose(bool finalize)
        {
            if (_disposed) return;

            _disposed = true;

            Clear();

            if (finalize)
                GC.SuppressFinalize(this);
        }

        #endregion

        #region Instance Members (Public)

        public virtual bool Contains(object obj)
        {
            int index = 0;
            int num2 = Count;
            while (num2-- > 0)
            {
                if (obj == null)
                {
                    if (GetElement(index) == null)
                        return true;
                }
                else if (obj.Equals(GetElement(index)))
                    return true;

                index = (index + 1) % Count;
            }

            return false;
        }

        public virtual object[] ToArray()
        {
            ArrayList itemsList = new ArrayList();
            for (int index = 0; index < Count; index++)
                itemsList.Add(GetElement(index));

            return itemsList.ToArray();
        }

        public virtual void Clear()
        {
            _internalQueue.Clear();
            _persistanceQueue.Clear();
            _commonQueue.Clear();

            if (Directory.Exists(_storageDir))
                Directory.Delete(_storageDir, true);
        }

        public virtual void Enqueue(T obj)
        {
            if (_internalQueue.Count < _capacity)
            {
                _internalQueue.Enqueue(obj);
                _commonQueue.Add(obj);
            }
            else
            {
                string fileName = ChoObject.Serialize(_storageDir, obj);
                _persistanceQueue.Enqueue(fileName);
                _commonQueue.Add(fileName);
            }
        }

        public virtual object Dequeue()
        {
            if (_internalQueue.Count == 1)
            {
                if (_persistanceQueue.Count > 0)
                {
                    int index = 0;
                    while (index < _capacity)
                    {
                        if (_persistanceQueue.Count == 0) break;
                        string fileName = _persistanceQueue.Dequeue() as string;
                        _internalQueue.Enqueue(ChoObject.Deserialize(_storageDir, fileName));
                        File.Delete(Path.Combine(_storageDir, fileName));
                        index++;
                    }
                }
            }

            _commonQueue.RemoveAt(0);
            return _internalQueue.Dequeue();
        }

        #endregion

        #region ICollection Members

        public virtual void CopyTo(Array array, int index)
        {
            ToArray().CopyTo(array, index);
        }

        public virtual int Count
        {
            get { return _internalQueue.Count + _persistanceQueue.Count; }
        }

        public virtual bool IsSynchronized
        {
            get { return false; }
        }

        public virtual object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        public virtual IEnumerator<T> GetEnumerator()
        {
            return null; // new ChoQueueEnumerator<T>(this);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(false);
        }

        #endregion

        #region Shared Members (Public)

        public static ChoQueue Synchronized(ChoQueue queue)
        {
            if (queue == null)
                throw new ArgumentNullException("queue");

            return new ChoSynchronizedQueue(queue);
        }

        #endregion Shared Members (Public)

        #region Finalizer

        ~ChoQueue()
        {
            Dispose(true);
        }

        #endregion

        #region ChoQueueEnumerator Class

        [Serializable]
        private class ChoQueueEnumerator : IEnumerator, ICloneable
        {
            // Fields
            private int _index;
            private ChoQueue _q;
            private object _currentElement;

            // Methods
            internal ChoQueueEnumerator(ChoQueue q)
            {
                _q = q;
                _index = q == null || q.Count == 0 ? -1 : 0;
            }

            public object Clone()
            {
                return base.MemberwiseClone();
            }

            public virtual bool MoveNext()
            {
                if (_index >= 0)
                {
                    _currentElement = _q.GetElement(_index);
                    _index++;
                    if (_index == _q.Count)
                        _index = -1;

                    return true;
                }
                else
                {
                    _currentElement = null;
                    return false;
                }
            }

            public virtual void Reset()
            {
                _index = _q == null || _q.Count == 0 ? -1 : 0;
                _currentElement = null;
            }

            // Properties
            public virtual object Current
            {
                get
                {
                    if (_currentElement != null)
                        return _currentElement;

                    if (this._index == 0)
                        throw new InvalidOperationException("InvalidOperation_EnumNotStarted");

                    throw new InvalidOperationException("InvalidOperation_EnumEnded");
                }
            }
        }

        #endregion ChoQueueEnumerator Class

        #region RITSyncronizedQueue Class

        [Serializable]
        private class ChoSynchronizedQueue : ChoQueue
        {
            #region Instance Data Members (Private)

            private ChoQueue _q;
            private object root;

            #endregion Instance Data Members (Private)

            #region Constructors

            internal ChoSynchronizedQueue(ChoQueue q)
            {
                _q = q;
                root = _q.SyncRoot;
            }

            #endregion Constructors

            #region ChoQueue Overrides

            public override void Clear()
            {
                lock (root)
                {
                    _q.Clear();
                }
            }

            public override void CopyTo(Array array, int arrayIndex)
            {
                lock (root)
                {
                    _q.CopyTo(array, arrayIndex);
                }
            }

            public override object Dequeue()
            {
                lock (root)
                {
                    return _q.Dequeue();
                }
            }

            public override void Enqueue(object value)
            {
                lock (root)
                {
                    _q.Enqueue(value);
                }
            }

            public override IEnumerator GetEnumerator()
            {
                lock (root)
                {
                    return _q.GetEnumerator();
                }
            }

            public override object[] ToArray()
            {
                lock (root)
                {
                    return _q.ToArray();
                }
            }

            // Properties
            public override int Count
            {
                get
                {
                    lock (root)
                    {
                        return _q.Count;
                    }
                }
            }

            public override bool IsSynchronized
            {
                get { return true; }
            }

            public override object SyncRoot
            {
                get { return root; }
            }

            #endregion ChoQueue Overrides
        }

        #endregion RITSyncronizedQueue Class

        #region ICollection<T> Members

        public void Add(T item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Contains(T item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsReadOnly
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool Remove(T item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
