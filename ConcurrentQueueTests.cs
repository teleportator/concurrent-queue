using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcurrentQueue
{
	[TestClass]
	public class ConcurrentQueueTests
	{
		[TestMethod, Timeout(5000)]
		public void ConcurrentQueue_ShouldBeFifo()
		{
			var sut = new ConcurrentQueue<int>();

			var count = 10;
			var sequence = Enumerable.Repeat(0, count).Select((n, i) => i).ToArray();

			foreach (var item in sequence)
			{
				sut.Push(item);
			}

			var actual = new int[10];
			for (int i = 0; i < count; i++)
			{
				actual[i] = sut.Pop();
			}

			actual.ShouldAllBeEquivalentTo(sequence);
		}

		[TestMethod, Timeout(5000)]
		public void ConcurrentQueue_Pop_ShouldWaitItemInQueue()
		{
			var sut = new ConcurrentQueue<int>();
			var manualResetEvent = new ManualResetEvent(false);
			var popTask = Task.Run(
				() =>
				{
					manualResetEvent.Set();
					return sut.Pop();
				});

			manualResetEvent.WaitOne();
			popTask.Wait(1000).Should().BeFalse("expected to wait for item in queue");

			sut.Push(1);
			popTask.Wait(1000).Should().BeTrue("expected item to be dequeue immediately");

			var actual = popTask.Result;
			actual.Should().Be(1);
		}
	}
}