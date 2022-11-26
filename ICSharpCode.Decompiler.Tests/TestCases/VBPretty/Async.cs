using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.VBPretty
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
#if LEGACY_VBC || (OPTIMIZE && !ROSLYN4)
			YieldAwaitable yieldAwaitable = default(YieldAwaitable);
			YieldAwaitable yieldAwaitable2 = yieldAwaitable;
			await yieldAwaitable2;
#else
			await default(YieldAwaitable);
#endif
		}

		public async void AwaitDefaultHopToThreadPool()
		{
#if LEGACY_VBC || (OPTIMIZE && !ROSLYN4)
			HopToThreadPoolAwaitable hopToThreadPoolAwaitable = default(HopToThreadPoolAwaitable);
			HopToThreadPoolAwaitable hopToThreadPoolAwaitable2 = hopToThreadPoolAwaitable;
			await hopToThreadPoolAwaitable2;
#else
			await default(HopToThreadPoolAwaitable);
#endif
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

		public async Task Issue2366a()
		{
			while (true)
			{
				try
				{
					await Task.CompletedTask;
				}
				catch (Exception projectError)
				{
					ProjectData.SetProjectError(projectError);
					ProjectData.ClearProjectError();
				}
			}
		}

		public static async Task<int> GetIntegerSumAsync(IEnumerable<int> items)
		{
			await Task.Delay(100);
			int num = 0;
			foreach (int item in items)
			{
				num = checked(num + item);
			}
			return num;
		}

		public async Task AsyncCatch(bool b, Task<int> task1, Task<int> task2)
		{
			try
			{
				Console.WriteLine("Start try");
				await task1;
				Console.WriteLine("End try");
			}
			catch (Exception projectError)
			{
				ProjectData.SetProjectError(projectError);
				Console.WriteLine("No await");
				ProjectData.ClearProjectError();
			}
		}

		public async Task AsyncCatchThrow(bool b, Task<int> task1, Task<int> task2)
		{
			try
			{
				Console.WriteLine("Start try");
				await task1;
				Console.WriteLine("End try");
			}
			catch (Exception projectError)
			{
				ProjectData.SetProjectError(projectError);
				Console.WriteLine("No await");
				throw;
			}
		}

		public async Task AsyncFinally(bool b, Task<int> task1, Task<int> task2)
		{
			try
			{
				Console.WriteLine("Start try");
				await task1;
				Console.WriteLine("End try");
			}
			finally
			{
				Console.WriteLine("No await");
			}
		}

		public static async Task AlwaysThrow()
		{
			throw new Exception();
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
			object objectValue = RuntimeHelpers.GetObjectValue(new object());
			await UseObj(RuntimeHelpers.GetObjectValue(objectValue));
			await UseObj(RuntimeHelpers.GetObjectValue(objectValue));
		}

		public static async Task UseObj(object a)
		{
		}
	}

	public struct AsyncInStruct
	{
		private int i;

		public async Task<int> Test(AsyncInStruct xx)
		{
			checked
			{
				xx.i++;
				i++;
				await Task.Yield();
				return i + xx.i;
			}
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

		void INotifyCompletion.OnCompleted(Action continuation)
		{
			//ILSpy generated this explicit interface implementation from .override directive in OnCompleted
			this.OnCompleted(continuation);
		}

		public void GetResult()
		{
		}
	}
}
