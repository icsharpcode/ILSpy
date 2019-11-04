using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class AsyncStreams
	{
		public static async IAsyncEnumerable<int> CountTo(int until)
		{
			for (int i = 0; i < until; i++) {
				yield return i;
				await Task.Delay(10);
			}
		}

		public static async IAsyncEnumerable<int> AlwaysThrow()
		{
			throw null;
			yield break;
		}

		public static async IAsyncEnumerator<int> InfiniteLoop()
		{
			while (true) {
			}
			yield break;
		}

		public static async IAsyncEnumerable<int> InfiniteLoopWithAwait()
		{
			while (true) {
				await Task.Delay(10);
			}
			yield break;
		}
	}
}
