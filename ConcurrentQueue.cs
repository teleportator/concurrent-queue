using System;
using System.Collections;

namespace ConcurrentQueue
{
	public class ConcurrentQueue<T>
	{
		private readonly Queue _internalQueue;

		public ConcurrentQueue()
		{
			_internalQueue = new Queue();
		}

		public void Push(T item)
		{
			lock (_internalQueue.SyncRoot)
			{
				_internalQueue.Enqueue(item);
			}
		}

		public T Pop()
		{
			while (true)
			{
				try
				{
					return SyncronizedDequeue();
				}
				catch (InvalidOperationException)
				{
				}
			}
		}

		private T SyncronizedDequeue()
		{
			lock (_internalQueue.SyncRoot)
			{
				return (T) _internalQueue.Dequeue();
			}
		}
	}
}