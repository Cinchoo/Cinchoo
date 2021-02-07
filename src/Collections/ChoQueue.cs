namespace Cinchoo.Core.Collections
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Collections;

    using Cinchoo.Core.IO;

    #endregion NameSpaces

    #region ChoQueueSettings Class

    public sealed class ChoQueueSettings //: IConfigSettingsHandler
    {
        #region Shared Data Members (Public)

        public static bool TurnOn = false;
        public static int MaxCapacity = 100;
        public static string StorageDirectory = Path.Combine(ChoApplication.ApplicationBaseDirectory, ChoPath.GetRandomFileName());

        #endregion Shared Data Members (Public)

        #region Shared Constructors

        static ChoQueueSettings()
        {
            //ChoConfigSettings.Initialize("riskIt/queueSettings", new ChoQueueSettings());
        }

        #endregion

        //#region Shared Member Functions (Public)

        //public void HandleConfigSettings(NameValueCollection nameValues)
        //{
        //    if (nameValues != null)
        //    {
        //        try
        //        {
        //            TurnOn = Boolean.Parse(nameValues["TurnOn"]);
        //        }
        //        catch { }

        //        try
        //        {
        //            int maxCapacity = Int32.Parse(nameValues["MaxCapacity"]);
        //            MaxCapacity = maxCapacity < 0 ? -1 : maxCapacity;
        //        }
        //        catch { }

        //        if (nameValues["StorageDirectory"] != null)
        //            StorageDirectory = nameValues["StorageDirectory"];
        //    }
        //}

        //public override string ToString()
        //{
        //    StringBuilder msg = new StringBuilder();

        //    msg.AppendFormat(Environment.NewLine);

        //    msg.AppendFormat("-- ChoQueue Settings --{0}", Environment.NewLine);
        //    msg.AppendFormat("\tTurnOn: {0}{1}", TurnOn, Environment.NewLine);
        //    msg.AppendFormat("\tMaxCapacity: {0}{1}", MaxCapacity, Environment.NewLine);
        //    msg.AppendFormat("\tStorageDirectory: {0}{1}", StorageDirectory, Environment.NewLine);

        //    msg.Append(Environment.NewLine);

        //    return msg.ToString();
        //}

        //#endregion
    }

#endregion ChoQueueSettings Class

    [Serializable]
    public class ChoQueue : ICollection, IEnumerable, IDisposable
    {
        #region Instance Data Members (Private)

        private bool _disposed = false;
        private int _capacity = Int32.MaxValue;
        private Queue _internalQueue;
        private Queue _persistanceQueue = new Queue();
        private ArrayList _commonQueue = new ArrayList();
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
                _internalQueue = new Queue();
            else
            {
                _capacity = capacity;
                _internalQueue = new Queue(capacity);
            }

            if (storageDir == null)
                _storageDir = Path.Combine(ChoApplication.ApplicationBaseDirectory, ChoPath.GetRandomFileName());
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
                return ChoObject.Deserialize(Path.Combine(_storageDir, _commonQueue[index] as string));

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

        public virtual void Enqueue(object obj)
        {
            if (_internalQueue.Count < _capacity)
            {
                _internalQueue.Enqueue(obj);
                _commonQueue.Add(obj);
            }
            else
            {
                string filePath = ChoPath.GetRandomFileName(_storageDir);
                ChoObject.Serialize(filePath, obj);
                _persistanceQueue.Enqueue(filePath);
                _commonQueue.Add(filePath);
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
                        _internalQueue.Enqueue(ChoObject.Deserialize(Path.Combine(_storageDir, fileName)));
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

        public virtual bool IsBlockingQueue
        {
            get { return false; }
        }

        public virtual object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        public virtual IEnumerator GetEnumerator()
        {
            return new ChoQueueEnumerator(this);
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

            if (queue.IsBlockingQueue)
                throw new ArgumentException("Can't synchronize the blocking queue.");

            return new ChoSynchronizedQueue(queue);
        }

        public static ChoQueue Synchronized(ChoQueue queue, object syncObject)
        {
            if (queue == null)
                throw new ArgumentNullException("queue");

            if (queue.IsBlockingQueue)
                throw new ArgumentException("Can't synchronize the blocking queue.");

            ChoGuard.ArgumentNotNull(syncObject, "SyncObject");

            return new ChoSynchronizedQueue(queue, syncObject);
        }

        public static ChoQueue BlockingQueue(ChoQueue queue)
        {
            if (queue == null)
                throw new ArgumentNullException("queue");

            return new ChoBlockingQueue(queue);
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

        #region ChoSyncronizedQueue Class

        [Serializable]
        private class ChoSynchronizedQueue : ChoQueue
        {
            #region Instance Data Members (Private)

            private ChoQueue _queue;
            private object _syncRoot;

            #endregion Instance Data Members (Private)

            #region Constructors

            internal ChoSynchronizedQueue(ChoQueue q) : this(q, q.SyncRoot)
            {
            }

            internal ChoSynchronizedQueue(ChoQueue q, object syncObject)
            {
                _queue = q;
                _syncRoot = syncObject;
            }

            #endregion Constructors

            #region ChoQueue Overrides

            public override void Clear()
            {
                lock (_syncRoot)
                {
                    _queue.Clear();
                }
            }

            public override void CopyTo(Array array, int arrayIndex)
            {
                lock (_syncRoot)
                {
                    _queue.CopyTo(array, arrayIndex);
                }
            }

            public override object Dequeue()
            {
                lock (_syncRoot)
                {
                    return _queue.Dequeue();
                }
            }

            public override void Enqueue(object value)
            {
                lock (_syncRoot)
                {
                    _queue.Enqueue(value);
                }
            }

            public override IEnumerator GetEnumerator()
            {
                lock (_syncRoot)
                {
                    //return _queue.GetEnumerator();
                    throw new NotSupportedException("Cannot enumerate a threadsafe queue.  Instead, enumerate using ToArray() method.");
                }
            }

            public override object[] ToArray()
            {
                lock (_syncRoot)
                {
                    return _queue.ToArray();
                }
            }

            // Properties
            public override int Count
            {
                get
                {
                    lock (_syncRoot)
                    {
                        return _queue.Count;
                    }
                }
            }

            public override bool IsBlockingQueue
            {
                get { return _queue.IsBlockingQueue; }
            }

            public override bool IsSynchronized
            {
                get { return true; }
            }

            public override object SyncRoot
            {
                get { return _syncRoot; }
            }

            #endregion ChoQueue Overrides
        }

        #endregion ChoSyncronizedQueue Class

        #region ChoBlockingQueue Class

        [Serializable]
        private class ChoBlockingQueue : ChoQueue
        {
            #region Instance Data Members (Private)

            private readonly ChoQueue _queue;
            private readonly AutoResetEvent _newItemArrived = new AutoResetEvent(false);

            #endregion Instance Data Members (Private)

            #region Constructors

            internal ChoBlockingQueue(ChoQueue q)
            {
                _queue = q;
            }

            #endregion Constructors

            #region ChoQueue Overrides

            public override void Clear()
            {
                _queue.Clear();
            }

            public override void CopyTo(Array array, int arrayIndex)
            {
                _queue.CopyTo(array, arrayIndex);
            }

            public override object Dequeue()
            {
                while (_queue.Count == 0)
                {
                    _newItemArrived.WaitOne();
                }

                lock (_queue)
                {
                    return _queue.Dequeue();
                }
            }

            public override void Enqueue(object value)
            {
                lock (_queue)
                {
                    _queue.Enqueue(value);
                }
                _newItemArrived.Set();
            }

            public override IEnumerator GetEnumerator()
            {
                return _queue.GetEnumerator();
            }

            public override object[] ToArray()
            {
                return _queue.ToArray();
            }

            // Properties
            public override int Count
            {
                get { return _queue.Count; }
            }

            public override bool IsSynchronized
            {
                get { return _queue.IsSynchronized; }
            }

            public override bool IsBlockingQueue
            {
                get { return true; }
            }

            public override object SyncRoot
            {
                get { return _queue.SyncRoot; }
            }

            #endregion ChoQueue Overrides
        }

        #endregion ChoBlockingQueue Class
    }
}
