using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
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

		public async IAsyncEnumerable<int> AwaitInFinally()
		{
			try {
				Console.WriteLine("try");
				yield return 1;
				Console.WriteLine("end try");
			} finally {
				Console.WriteLine("finally");
				await Task.Yield();
				Console.WriteLine("end finally");
			}
		}

		public static async IAsyncEnumerable<int> SimpleCancellation([EnumeratorCancellation] CancellationToken cancellationToken)
		{
			yield return 1;
			await Task.Delay(100, cancellationToken);
			yield return 2;
		}
	}

	public struct TestStruct
	{
		private int i;

		public async IAsyncEnumerable<int> AwaitInStruct(TestStruct xx)
		{
			xx.i++;
			i++;
			await Task.Yield();
			yield return i;
			yield return xx.i;
		}
	}
}
