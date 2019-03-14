namespace Cinchoo.Core.Collections.Generic
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Diagnostics;
	using System.Threading;

	#endregion NameSpaces

	[Serializable]
	[DebuggerDisplay("Count = {Count}")]
	public partial class ChoQueue<T> : Queue<T>, ICloneable, IDisposable
	{
		#region Instance Data Members (Private)

		private readonly object _syncRoot = new object();

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoQueue()
			: base()
		{
		}

		public ChoQueue(IEnumerable<T> collection) : base(collection)
		{
		}

		public ChoQueue(int capacity)
			: base(capacity)
		{
		}

		#endregion Constructors

		#region Instance Properties (Public)

		public virtual int Size
		{
			get { return Int32.MaxValue; }
		}

		public virtual object SyncRoot
		{
			get { return _syncRoot; }
		}

		public new virtual int Count 
		{
			get { return base.Count; }
		}

		public virtual bool IsSynchronized
		{
			get { return false; }
		}

		public virtual bool IsBlockingQueue
		{
			get { return false; }
		}

		#endregion Instance Properties (Public)

		#region Instance Members (Public)

		public new virtual void Clear()
		{
			base.Clear();
		}

		public new virtual bool Contains(T item)
		{
			return base.Contains(item);
		}

		public new virtual void CopyTo(T[] array, int arrayIndex)
		{
			base.CopyTo(array, arrayIndex);
		}

		public new virtual T Dequeue()
		{
			return Dequeue(Timeout.Infinite);
		}
		
		public virtual T Dequeue(int millisecondsTimeout)
		{
			return base.Dequeue();
		}

		public virtual bool TryDequeue(out T item)
		{
			if (Count == 0)
			{
				item = default(T);
				return false;
			}
			else
			{
				item = Dequeue();
				return true;
			}
		}

		public new virtual void Enqueue(T item)
		{
			Enqueue(item, Timeout.Infinite);
		}
		
		public virtual void Enqueue(T item, int millisecondsTimeout)
		{
			base.Enqueue(item);
		}

		public virtual bool TryEnqueue(T item)
		{
			Enqueue(item);
			return true;
		}

		public new virtual IEnumerator<T> GetEnumerator()
		{
			return base.GetEnumerator(); 
		}

		public new virtual T Peek()
		{
			return base.Peek();
		}
		
		public virtual bool TryPeek(out T item)
		{
			if (Count == 0)
			{
				item = default(T);
				return false;
			}
			else
			{
				item = Peek();
				return true;
			}
		}

		public new virtual T[] ToArray()
		{
			return base.ToArray();
		}

		public new virtual void TrimExcess()
		{
			base.TrimExcess();
		}
	
		#endregion Instance Members (Public)

		#region ICloneable Members

		public virtual object Clone()
		{
			return null;
		}

		#endregion

		#region IDisposable Members

		public virtual void Dispose()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool finalize)
		{
		}

		#endregion

		#region Finalizer

		~ChoQueue()
		{
			Dispose(true);
		}

		#endregion Finalizer

		#region Shared Members (Public)

		public static ChoQueue<T> Synchronized(ChoQueue<T> queue)
		{
			ChoGuard.ArgumentNotNull(queue, "Queue");

			return new ChoQueue<T>.ChoSyncronizedQueue<T>(queue);
		}

		public static ChoQueue<T> Synchronized(ChoQueue<T> queue, object syncObject)
		{
			ChoGuard.ArgumentNotNull(queue, "Queue");
			ChoGuard.ArgumentNotNull(syncObject, "SyncObject");

			return new ChoQueue<T>.ChoSyncronizedQueue<T>(queue, syncObject);
		}

		public static ChoQueue<T> BlockingQueue(ChoQueue<T> queue)
		{
			ChoGuard.ArgumentNotNull(queue, "Queue");

			return new ChoQueue<T>.ChoBlockingQueue<T>(queue);
		}

		public static ChoQueue<T> BlockingQueue(ChoQueue<T> queue, int size)
		{
			ChoGuard.ArgumentNotNull(queue, "Queue");
			if (size <= 0)
				throw new ArgumentException("Size must be positive.");
			if (size < queue.Count)
				throw new ArgumentException("Size must be greater than queue count.");

			return new ChoQueue<T>.ChoBlockingQueue<T>(queue, size);
		}

		#endregion Shared Members (Public)
	}
}
