namespace Cinchoo.Core.Memory
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
using System.Threading;

	#endregion NameSpaces

	public partial class ChoObjectPool<T> where T: struct
	{
		#region Constants

		private const int DEF_BUFFER_SIZE = 1024; // size of the buffers
		private const int DEF_POOL_BLOCK_SIZE = 512; // initial size of the pool
		private const int DEF_NO_OF_BLOCKS = 1;

		#endregion Constants

		#region Instance Data Members (Private)

		private readonly object _padLock = new object();
		private readonly int _bufferSize;
		private readonly int _poolBlockSize;
		private readonly int _noOfBlocks;
		private readonly Stack<T[]> _freeBuffers;
		private readonly ManualResetEvent _bufferEmptyEvent = new ManualResetEvent(false);

		#endregion Instance Data Members (Private)

		#region Instance Data Members (Private)

		private int _noOfBlocksCounter = 0;
		private int _poolSize = 0;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoObjectPool()
			: this(DEF_BUFFER_SIZE)
		{
		}

		public ChoObjectPool(int bufferSize)
			: this(bufferSize, DEF_POOL_BLOCK_SIZE)
		{
		}

		public ChoObjectPool(int bufferSize, int poolBlockSize)
			: this(bufferSize, poolBlockSize, DEF_NO_OF_BLOCKS)
		{
		}

		public ChoObjectPool(int bufferSize, int poolBlockSize, int noOfBlocks)
		{
			if (bufferSize <= 0)
				throw new ArgumentException("bufferSize must be > 0");
			if (poolBlockSize <= 0)
				throw new ArgumentException("poolBlockSize must be > 0");
			if (noOfBlocks < 0)
				throw new ArgumentException("noOfBlocks must be >= 0");

			_bufferSize = bufferSize;
			_poolBlockSize = poolBlockSize;
			_noOfBlocks = noOfBlocks;
			_freeBuffers = new Stack<T[]>(poolBlockSize);

			for (int i = 0; i < _poolBlockSize; i++)
				_freeBuffers.Push(new T[_bufferSize]);

			_poolSize += _poolBlockSize;
			_noOfBlocksCounter++;
		}

		#endregion Constructors

		#region Instance Members (Public)

		public virtual T[] New()
		{
			while (true)
			{
				lock (_padLock)
				{
					if (_freeBuffers.Count == 0)
					{
						if (_freeBuffers.Count == 0 && _noOfBlocksCounter == _noOfBlocks)
						{
						}
						else
						{
							_noOfBlocksCounter++;
							for (int i = 0; i < _poolBlockSize; i++)
								_freeBuffers.Push(new T[_bufferSize]);
							_poolSize += _poolBlockSize;
						}
					}

					if (_freeBuffers.Count != 0)
						return _freeBuffers.Pop();
				}

				_bufferEmptyEvent.WaitOne();
			}
		}

		public virtual void Release(T[] buffer)
		{
			lock (_padLock)
			{
				_freeBuffers.Push(buffer);
				//Console.WriteLine("Releasing...{0}/{1}", _freeBuffers.Count, _poolSize);

				_bufferEmptyEvent.Set();
			}
		}

		#endregion Instance Members (Public)

		public struct ChoObjectContext<T1> : IDisposable where T1 : struct
		{
			#region Instance Data Members (Private)

			private readonly T1[] _buffer;
			private readonly ChoObjectPool<T1> _objectPool;

			#endregion Instance Data Members (Private)

			#region Constructors

			public ChoObjectContext(T1[] buffer, ChoObjectPool<T1> objectPool)
			{
				ChoGuard.ArgumentNotNull(objectPool, "objectPool");
				ChoGuard.ArgumentNotNull(buffer, "buffer");

				_buffer = buffer;
				_objectPool = objectPool;
			}

			#endregion Construcors

			#region Instance Members (Public)

			public void Dispose()
			{
				_objectPool.Release(_buffer);
			}

			public void Release()
			{
				Dispose();
			}

			#endregion Instance Members (Public)

			#region Instance Properties (Public)

			public T1[] Buffer
			{
				get { return _buffer; }
			}

			#endregion Instance Properties (Public)
		}
	}
}
