#pragma warning disable 1998

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class AsyncMethodBuilderOverride
	{
		private int memberField;

		[AsyncMethodBuilder(typeof(MyTaskMethodBuilder))]
		public async Task SimpleOverride()
		{
			Console.WriteLine("Before");
			await Task.Delay(TimeSpan.FromSeconds(1.0));
			Console.WriteLine("After");
		}

		[AsyncMethodBuilder(typeof(MyTaskMethodBuilder))]
		public async Task OverrideWithoutAwait()
		{
			Console.WriteLine("No Await");
		}

		[AsyncMethodBuilder(typeof(MyTaskMethodBuilder))]
		public async Task CapturingThis()
		{
			await Task.Delay(memberField);
		}

		[AsyncMethodBuilder(typeof(MyTaskMethodBuilder))]
		public async Task AwaitInLoop(int count)
		{
			for (int i = 0; i < count; i++)
			{
				await Task.Delay(i);
			}
		}

		[AsyncMethodBuilder(typeof(MyTaskMethodBuilder<>))]
		public async Task<bool> BoolOverride()
		{
			Console.WriteLine("Before");
			await Task.Delay(TimeSpan.FromSeconds(1.0));
			Console.WriteLine("After");
			return true;
		}

		[AsyncMethodBuilder(typeof(MyTaskMethodBuilder<>))]
		public async Task<T> GenericOverride<T>(T value)
		{
			await Task.Delay(100);
			return value;
		}

		[AsyncMethodBuilder(typeof(MyClassTaskMethodBuilder))]
		public async Task ClassBuilderOverride()
		{
			await Task.Delay(100);
		}

		[AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
		public async ValueTask PoolingOverride()
		{
			await Task.Delay(100);
		}

		[AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
		public async ValueTask<int> PoolingOverrideOfT()
		{
			await Task.Delay(100);
			return 42;
		}

		public static async Task<int> LocalFunctionOverride()
		{
			return await Nested(1) + await Nested(2);

			[AsyncMethodBuilder(typeof(MyTaskMethodBuilder<>))]
			static async Task<int> Nested(int i)
			{
				await Task.Delay(i);
				return i;
			}
		}

		public static Func<Task> LambdaOverride()
		{
			return [AsyncMethodBuilder(typeof(MyTaskMethodBuilder))] async Task () => await Task.Delay(100);
		}
	}

	public class MyClassTaskMethodBuilder
	{
		private AsyncTaskMethodBuilder builder = AsyncTaskMethodBuilder.Create();

		public Task Task => builder.Task;

		public static MyClassTaskMethodBuilder Create()
		{
			return new MyClassTaskMethodBuilder();
		}

		public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
		{
			builder.Start(ref stateMachine);
		}

		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			builder.SetStateMachine(stateMachine);
		}

		public void SetException(Exception exception)
		{
			builder.SetException(exception);
		}

		public void SetResult()
		{
			builder.SetResult();
		}

		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
		}

		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
		}
	}

	public struct MyTaskMethodBuilder
	{
		private AsyncTaskMethodBuilder builder;

		public Task Task => builder.Task;

		public static MyTaskMethodBuilder Create()
		{
			return new MyTaskMethodBuilder {
				builder = AsyncTaskMethodBuilder.Create()
			};
		}

		public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
		{
			builder.Start(ref stateMachine);
		}

		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			builder.SetStateMachine(stateMachine);
		}

		public void SetException(Exception exception)
		{
			builder.SetException(exception);
		}

		public void SetResult()
		{
			builder.SetResult();
		}

		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
		}

		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
		}
	}

	public struct MyTaskMethodBuilder<T>
	{
		private AsyncTaskMethodBuilder<T> builder;

		public Task<T> Task => builder.Task;

		public static MyTaskMethodBuilder<T> Create()
		{
			return new MyTaskMethodBuilder<T> {
				builder = AsyncTaskMethodBuilder<T>.Create()
			};
		}

		public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
		{
			builder.Start(ref stateMachine);
		}

		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			builder.SetStateMachine(stateMachine);
		}

		public void SetException(Exception exception)
		{
			builder.SetException(exception);
		}

		public void SetResult(T result)
		{
			builder.SetResult(result);
		}

		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
		}

		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
		}
	}
}
