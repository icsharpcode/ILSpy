// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#pragma warning disable 1998
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class Async
	{
		private int memberField;

		public async void SimpleVoidMethod()
		{
			Console.WriteLine("Before");
			await Task.Delay(TimeSpan.FromSeconds(1.0));
			Console.WriteLine("After");
		}

		public async void VoidMethodWithoutAwait()
		{
			Console.WriteLine("No Await");
		}

		public async void EmptyVoidMethod()
		{
		}

		public async void AwaitYield()
		{
			await Task.Yield();
		}

		public async void AwaitDefaultYieldAwaitable()
		{
			await default(YieldAwaitable);
		}

		public async void AwaitDefaultHopToThreadPool()
		{
			// unlike YieldAwaitable which implements ICriticalNotifyCompletion,
			// the HopToThreadPoolAwaitable struct only implements
			// INotifyCompletion, so this results in different codegen
			await default(HopToThreadPoolAwaitable);
		}

		public async Task SimpleVoidTaskMethod()
		{
			Console.WriteLine("Before");
			await Task.Delay(TimeSpan.FromSeconds(1.0));
			Console.WriteLine("After");
		}

		public async Task TaskMethodWithoutAwait()
		{
			Console.WriteLine("No Await");
		}

		public async Task CapturingThis()
		{
			await Task.Delay(memberField);
		}

		public async Task CapturingThisWithoutAwait()
		{
			Console.WriteLine(memberField);
		}

		public async Task<bool> SimpleBoolTaskMethod()
		{
			Console.WriteLine("Before");
			await Task.Delay(TimeSpan.FromSeconds(1.0));
			Console.WriteLine("After");
			return true;
		}

		public async void TwoAwaitsWithDifferentAwaiterTypes()
		{
			Console.WriteLine("Before");
			if (await SimpleBoolTaskMethod()) {
				await Task.Delay(TimeSpan.FromSeconds(1.0));
			}
			Console.WriteLine("After");
		}

		public async void AwaitInLoopCondition()
		{
			while (await SimpleBoolTaskMethod()) {
				Console.WriteLine("Body");
			}
		}

#if CS60
		public async Task AwaitInCatch(bool b, Task<int> task1, Task<int> task2)
		{
			try {
				Console.WriteLine("Start try");
				await task1;
				Console.WriteLine("End try");
			} catch (Exception) {
				if (!b) {
					await task2;
				} else {
					Console.WriteLine("No await");
				}
			}
		}

		public async Task AwaitInFinally(bool b, Task<int> task1, Task<int> task2)
		{
			try {
				Console.WriteLine("Start try");
				await task1;
				Console.WriteLine("End try");
			} finally {
				if (!b) {
					await task2;
				} else {
					Console.WriteLine("No await");
				}
			}
		}
#endif

		public static async Task<int> GetIntegerSumAsync(IEnumerable<int> items)
		{
			await Task.Delay(100);
			int num = 0;
			foreach (int item in items) {
				num += item;
			}
			return num;
		}

		public static Func<Task<int>> AsyncLambda()
		{
			return async () => await GetIntegerSumAsync(new int[3] {
				1,
				2,
				3
			});
		}

		public static Func<Task<int>> AsyncDelegate()
		{
			return async delegate {
				await Task.Delay(10);
				return 2;
			};
		}

#if CS70
		public static async Task<int> AsyncLocalFunctions()
		{
			return await Nested(1) + await Nested(2);

			async Task<int> Nested(int i)
			{
				await Task.Delay(i);
				return i;
			}
		}
#endif
	}

	public struct HopToThreadPoolAwaitable : INotifyCompletion
	{
		public bool IsCompleted {
			get;
			set;
		}

		public HopToThreadPoolAwaitable GetAwaiter()
		{
			return this;
		}

		public void OnCompleted(Action continuation)
		{
			Task.Run(continuation);
		}

		public void GetResult()
		{
		}
	}
}
