using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class AsyncForeach
	{
		public async Task<int> SumIntegers(IAsyncEnumerable<int> items, CancellationToken token)
		{
			int sum = 0;
			await foreach (int item in items.WithCancellation(token)) {
				if (token.IsCancellationRequested) {
					break;
				}
				sum += item;
			}
			return sum;
		}

		public async Task<int> MaxInteger(IAsyncEnumerable<int> items)
		{
			int max = int.MinValue;
			await foreach (int item in items) {
				if (item > max) {
					max = item;
				}
			}
			return max;
		}
	}
}
