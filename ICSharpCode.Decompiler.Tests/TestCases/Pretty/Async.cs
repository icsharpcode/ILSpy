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

		private static bool True()
		{
			return true;
		}

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
			if (await SimpleBoolTaskMethod())
			{
				await Task.Delay(TimeSpan.FromSeconds(1.0));
			}
			Console.WriteLine("After");
		}

		public async void AwaitInLoopCondition()
		{
			while (await SimpleBoolTaskMethod())
			{
				Console.WriteLine("Body");
			}
		}

#if CS60
		public async Task AwaitInCatch(bool b, Task<int> task1, Task<int> task2)
		{
			try
			{
				Console.WriteLine("Start try");
				await task1;
				Console.WriteLine("End try");
			}
			catch (Exception)
			{
				if (!b)
				{
					await task2;
				}
				else
				{
					Console.WriteLine("No await");
				}
			}
		}

		public async Task AwaitInFinally(bool b, Task<int> task1, Task<int> task2)
		{
			try
			{
				Console.WriteLine("Start try");
				await task1;
				Console.WriteLine("End try");
			}
			finally
			{
				if (!b)
				{
					await task2;
				}
				else
				{
					Console.WriteLine("No await");
				}
			}
		}

		public async Task AnonymousThrow()
		{
			try
			{
				await Task.Delay(0);
			}
			catch
			{
				await Task.Delay(0);
				throw;
			}
		}

		public async Task DeclaredException()
		{
			try
			{
				await Task.Delay(0);
			}
			catch (Exception)
			{
				await Task.Delay(0);
				throw;
			}
		}

		public async Task RethrowDeclared()
		{
			try
			{
				await Task.Delay(0);
			}
			catch (Exception ex)
			{
				await Task.Delay(0);
				throw ex;
			}
		}

		public async Task RethrowDeclaredWithFilter()
		{
			try
			{
				await Task.Delay(0);
			}
			catch (Exception ex) when (ex.GetType().FullName.Contains("asdf"))
			{
				await Task.Delay(0);
				throw;
			}
		}

		public async Task ComplexCatchBlock()
		{
			try
			{
				await Task.Delay(0);
			}
			catch (Exception ex)
			{
				if (ex.GetHashCode() != 0)
				{
					throw;
				}
				await Task.Delay(0);
			}
		}

		public async Task ComplexCatchBlockWithFilter()
		{
			try
			{
				await Task.Delay(0);
			}
			catch (Exception ex) when (ex.GetType().FullName.Contains("asdf"))
			{
				if (ex.GetHashCode() != 0)
				{
					throw;
				}
				await Task.Delay(0);
			}
		}

		public async Task LoadsToCatch(int i)
		{
			try
			{
				throw null;
			}
			catch (Exception ex2) when (i == 0)
			{
				Console.WriteLine("First!");
				if (i == 1)
				{
					throw;
				}
				await Task.Yield();
				Console.WriteLine(ex2.StackTrace);
			}
			catch (Exception ex3) when (True())
			{
				Console.WriteLine("Second!");
				if (i == 1)
				{
					throw;
				}
				await Task.Yield();
				Console.WriteLine(ex3.StackTrace);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Third!");
				if (i == 1)
				{
					throw;
				}
				await Task.Yield();
				Console.WriteLine(ex.StackTrace);
			}
			catch when (i == 0)
			{
				Console.WriteLine("Fourth!");
				if (i == 1)
				{
					throw;
				}
				await Task.Yield();
			}
			catch when (True())
			{
				Console.WriteLine("Fifth!");
				if (i == 1)
				{
					throw;
				}
				await Task.Yield();
			}
			catch
			{
				Console.WriteLine("Sixth!");
				if (i == 1)
				{
					throw;
				}
				await Task.Yield();
			}
		}
#endif

		public static async Task<int> GetIntegerSumAsync(IEnumerable<int> items)
		{
			await Task.Delay(100);
			int num = 0;
			foreach (int item in items)
			{
				num += item;
			}
			return num;
		}

		public static Func<Task<int>> AsyncLambda()
		{
			return async () => await GetIntegerSumAsync(new int[3] { 1, 2, 3 });
		}

		public static Func<Task<int>> AsyncDelegate()
		{
			return async delegate {
				await Task.Delay(10);
				return 2;
			};
		}

		public static async Task AlwaysThrow()
		{
			throw null;
		}

		public static async Task InfiniteLoop()
		{
			while (true)
			{
			}
		}

		public static async Task InfiniteLoopWithAwait()
		{
			while (true)
			{
				await Task.Delay(10);
			}
		}

		public async Task AsyncWithLocalVar()
		{
			object a = new object();
#if CS70
			(object, string) tuple = (new object(), "abc");
#endif
			await UseObj(a);
			await UseObj(a);
#if CS70
			await UseObj(tuple);
#endif
		}

		public static async Task UseObj(object a)
		{
		}

#if CS70
		public static async Task<int> AsyncLocalFunctions()
		{
			return await Nested(1) + await Nested(2);

#if CS80
			static async Task<int> Nested(int i)
#else
			async Task<int> Nested(int i)
#endif
			{
				await Task.Delay(i);
				return i;
			}
		}
#endif
	}

	public struct AsyncInStruct
	{
		private int i;

		public async Task<int> Test(AsyncInStruct xx)
		{
			xx.i++;
			i++;
			await Task.Yield();
			return i + xx.i;
		}
	}

	public struct HopToThreadPoolAwaitable : INotifyCompletion
	{
		public bool IsCompleted { get; set; }

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
