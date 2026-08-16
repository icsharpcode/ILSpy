// Copyright (c) 2026 Siegfried Pammer
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

// Await shapes that still decompile to code that does not compile are tracked as #4017 (type
// parameter with an interface constraint), #4018 (static dynamic call) and #4019 (with
// expression); they are absent here because they have no correct output to pin yet.

#pragma warning disable 1998
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncAwait
{
	public class AwaitableContainer
	{
		public class NestedAwaitable
		{
			public TaskAwaiter GetAwaiter()
			{
				return default(TaskAwaiter);
			}
		}
	}

	/// <summary>
	/// The context the await sits in: how the translated expression has to be parenthesized, and
	/// where the async state machine splits the surrounding statement.
	/// </summary>
	public class AwaitContexts
	{
#if CS80 && !NET40
		private sealed class AsyncDisposable : IAsyncDisposable
		{
			public ValueTask DisposeAsync()
			{
				return default(ValueTask);
			}
		}
#endif

		private static Task<int> Get()
		{
			return Task.FromResult(1);
		}

		private static Task<string> GetString()
		{
			return Task.FromResult("s");
		}

		private static Task<Exception> GetException()
		{
			return Task.FromResult(new Exception());
		}

		public async Task Statement()
		{
			await Get();
		}

		public async Task Argument()
		{
			Console.WriteLine(await Get());
		}

		public async Task BinaryOperator()
		{
#if ROSLYN2 || OPT
			Console.WriteLine(await Get() + await Get());
#else
			int value = await Get() + await Get();
			Console.WriteLine(value);
#endif
		}

		public async Task UnaryOperator()
		{
#if ROSLYN2 || OPT
			Console.WriteLine(-(await Get()));
#else
			int value = -(await Get());
			Console.WriteLine(value);
#endif
		}

		public async Task MemberAccessOnResult()
		{
#if ROSLYN2 || OPT
			Console.WriteLine((await GetString()).Length);
#else
			int length = (await GetString()).Length;
			Console.WriteLine(length);
#endif
		}

		public async Task IndexerOnResult()
		{
#if ROSLYN2 || OPT
			Console.WriteLine((await GetString())[0]);
#else
			char value = (await GetString())[0];
			Console.WriteLine(value);
#endif
		}

		public async Task CoalesceOnResult()
		{
#if ROSLYN2 || OPT
			Console.WriteLine((await GetString()) ?? "null");
#else
			string value = (await GetString()) ?? "null";
			Console.WriteLine(value);
#endif
		}

		public async Task ThrowAwaitedException()
		{
			throw await GetException();
		}

		public async Task Checked()
		{
#if ROSLYN2 || OPT
			Console.WriteLine(checked(await Get() + 1));
#else
			int value = checked(await Get() + 1);
			Console.WriteLine(value);
#endif
		}

#if CS60
		public async Task TryFinally()
		{
			try
			{
				await Get();
			}
			finally
			{
				await Get();
			}
		}
#endif

		public async Task Using()
		{
			using (new Disposable())
			{
				await Get();
			}
		}

#if CS60
		public async Task ConditionalAccessOnResult()
		{
#if ROSLYN2 || OPT
			Console.WriteLine((await GetString())?.Length);
#else
			object value = (await GetString())?.Length;
			Console.WriteLine(value);
#endif
		}

		public async Task CatchWithFilter()
		{
			try
			{
				await Get();
			}
			catch (Exception ex) when (ex.Message.Length > 2)
			{
				await Get();
			}
		}
#endif

#if CS70 && !NET40
		public async Task AwaitInTupleLiteral()
		{
			Console.WriteLine((await Get(), await GetString()));
		}
#endif

#if CS80 && !NET40
		public async Task AwaitUsing()
		{
			await using (new AsyncDisposable())
			{
				await Get();
			}
		}

		public async Task AwaitForeach(IAsyncEnumerable<int> source)
		{
			await foreach (int item in source)
			{
				Console.WriteLine(item);
			}
		}

		public async Task AwaitForeachConfigured(IAsyncEnumerable<int> source)
		{
			await foreach (int item in source.ConfigureAwait(continueOnCapturedContext: false))
			{
				Console.WriteLine(item);
			}
		}

		public async IAsyncEnumerable<int> AsyncIterator()
		{
			yield return await Get();
			await Task.Yield();
			yield return 2;
		}

		public async IAsyncEnumerable<int> AsyncIteratorWithFinally()
		{
			try
			{
				yield return await Get();
			}
			finally
			{
				Console.WriteLine("cleanup");
			}
		}

		public async Task LocalFunction()
		{
			Console.WriteLine(await Local());
			static async Task<int> Local()
			{
				return await Get();
			}
		}
#endif

		public async Task AwaitInGenericMethod<T>(Task<T> task)
		{
#if ROSLYN2 || OPT
			Console.WriteLine(await task);
#else
			object value = await task;
			Console.WriteLine(value);
#endif
		}
	}

	public static class AwaiterExtensions
	{
		public static TaskAwaiter GetAwaiter(this IAwaitableMarker marker)
		{
			return default(TaskAwaiter);
		}

		public static TaskAwaiter GetAwaiter(this int millisecondsDelay)
		{
			return Task.Delay(millisecondsDelay).GetAwaiter();
		}

		public static TaskAwaiter GetAwaiter(this Action action)
		{
			return default(TaskAwaiter);
		}

		public static TaskAwaiter<T[]> GetAwaiter<T>(this IEnumerable<Task<T>> tasks)
		{
			return default(TaskAwaiter<T[]>);
		}

#if CS70 && !NET40
		public static TaskAwaiter<T> GetAwaiter<T>(this (Task<T>, string) taggedTask)
		{
			return taggedTask.Item1.GetAwaiter();
		}
#endif

#if CS72
		public static TaskAwaiter GetAwaiter(this in ByRefReceiver receiver)
		{
			return receiver.Self();
		}
#endif
	}

	/// <summary>
	/// The operand side: the shape of the expression the await is applied to.
	/// </summary>
	public class AwaitOperands
	{
		private Task<int> taskField;

		private StructAwaitable structField;

		private readonly StructAwaitable readonlyStructField;

		private Task<int> Property {
			get {
				Console.WriteLine("get_Property");
				return taskField;
			}
		}

		private Task<int> this[int index] {
			get {
				Console.WriteLine("get_Item");
				return taskField;
			}
		}

		private static Task<int> Get()
		{
			return Task.FromResult(1);
		}

		public async Task DefaultOfStruct()
		{
			await default(StructAwaitable);
		}

		public async Task Ternary(bool condition, Task first, Task second)
		{
			await (condition ? first : second);
		}

		public async Task Coalesce(Task first, Task second)
		{
			await (first ?? second);
		}

#if CS60
		public async Task NullConditional(List<Task> tasks)
		{
			await (tasks?[0]);
		}
#endif

		public async Task Cast(object obj)
		{
			await (Task)obj;
		}

		/// <summary>
		/// A null literal has no type, so the cast is what makes the operand awaitable. Note that
		/// <c>default(Task)</c> compiles to the same `ldnull` and therefore decompiles to this same
		/// cast; the two are indistinguishable in IL.
		/// </summary>
		public async Task NullLiteral()
		{
			await (Task)null;
		}

		public async Task AsOperator(object obj)
		{
			await (obj as Task);
		}

		public async Task FieldAccess()
		{
			Console.WriteLine(await taskField);
		}

		public async Task PropertyAccess()
		{
			Console.WriteLine(await Property);
		}

		public async Task IndexerAccess()
		{
			Console.WriteLine(await this[0]);
		}

		public async Task StructField()
		{
			await structField;
		}

		public async Task ReadOnlyStructField()
		{
			await readonlyStructField;
		}

		public async Task StructArrayElement(StructAwaitable[] awaitables)
		{
			await awaitables[0];
		}

		public async Task MethodCall()
		{
			Console.WriteLine(await Get());
		}

		public async Task DelegateInvocation(Func<Task<int>> factory)
		{
			Console.WriteLine(await factory());
		}

		public async Task ArrayElement(Task<int>[] tasks)
		{
			Console.WriteLine(await tasks[0]);
		}

		public async Task TernaryOfTasks(bool condition)
		{
			Console.WriteLine(await (condition ? Get() : Get()));
		}
	}

	/// <summary>
	/// The receiver ("expected type") side of ExpressionBuilder.VisitAwait: the awaited expression
	/// is converted to the declaring type of the resolved GetAwaiter, or to its first parameter
	/// type when GetAwaiter is an extension method.
	/// </summary>
	public class AwaitReceivers
	{
		public async Task InstanceAwaiterOnSelf(ClassAwaitable awaitable)
		{
			await awaitable;
		}

		public async Task AwaiterInheritedFromBaseClass(DerivedAwaitable awaitable)
		{
			await awaitable;
		}

		public async Task AwaiterThroughInterface(IAwaitable awaitable)
		{
			await awaitable;
		}

		public async Task AwaiterThroughBaseInterface(IDerivedAwaitable awaitable)
		{
			await awaitable;
		}

		/// <summary>
		/// The cast carries the operand to the interface that declares GetAwaiter; without it the
		/// explicit implementation is not accessible. A conversion that is merely implicit must not
		/// be dropped here, even though a boxing conversion exists.
		/// </summary>
		public async Task ExplicitInterfaceImplementationOnStruct(ExplicitStructAwaitable awaitable)
		{
			await (IAwaitable)awaitable;
		}

		/// <summary>
		/// The same shape on a class, i.e. it is not specific to the boxing conversion.
		/// </summary>
		public async Task ExplicitInterfaceImplementationOnClass(ExplicitClassAwaitable awaitable)
		{
			await (IAwaitable)awaitable;
		}

		/// <summary>
		/// The await pattern does not apply user-defined conversions, so the cast that invokes
		/// op_Implicit has to survive.
		/// </summary>
		public async Task UserDefinedConversionToAwaitable(ConvertsToAwaitable awaitable)
		{
			await (ClassAwaitable)awaitable;
		}

		public async Task ExtensionAwaiterOnClass(MarkerClass marker)
		{
			await marker;
		}

		public async Task ExtensionAwaiterOnStruct(MarkerStruct marker)
		{
			await marker;
		}

		public async Task ExtensionAwaiterOnPrimitive()
		{
			await 100;
		}

		public async Task ExtensionAwaiterOnDelegate(Action action)
		{
			await action;
		}

#if CS72
		/// <summary>
		/// An extension GetAwaiter taking its receiver by 'in' makes the expected type a
		/// ByReferenceType. Stripping the 'ref' must not leave a conversion that reaches the
		/// managed reference back through a pointer.
		/// </summary>
		public async Task InReceiverExtensionAwaiter(ByRefReceiver receiver)
		{
			await receiver;
		}
#endif

		public async Task ExtensionAwaiterOverTaskArray(Task<int>[] tasks)
		{
#if ROSLYN2 || OPT
			Console.WriteLine((await tasks)[0]);
#else
			int value = (await tasks)[0];
			Console.WriteLine(value);
#endif
		}

		public async Task ExtensionAwaiterOverTaskList(List<Task<int>> tasks)
		{
#if ROSLYN2 || OPT
			Console.WriteLine((await tasks)[0]);
#else
			int value = (await tasks)[0];
			Console.WriteLine(value);
#endif
		}

		public async Task GenericAwaitableType(GenericAwaitable<string> awaitable)
		{
			Console.WriteLine(await awaitable);
		}

		public async Task NestedAwaitableType(AwaitableContainer.NestedAwaitable awaitable)
		{
			await awaitable;
		}

		public async Task TypeParameterWithClassConstraint<T>(T awaitable) where T : ClassAwaitable
		{
			await awaitable;
		}

		public async Task TypeParameterWithStructConstraint<T>(T awaitable) where T : struct, IAwaitable
		{
			await awaitable;
		}

		public async Task ConfiguredTaskAwaitable(Task<int> task)
		{
#if ROSLYN2
			Console.WriteLine(await task.ConfigureAwait(continueOnCapturedContext: false));
#else
			Console.WriteLine(await task.ConfigureAwait(false));
#endif
		}

#if CS70 && !NET40
		public async Task ExtensionAwaiterOnTuple(Task<int> task)
		{
			Console.WriteLine(await (task, "tag"));
		}
#endif

#if CS80 && !NET40
		public async Task ValueTaskAwaitable(ValueTask<int> task)
		{
			Console.WriteLine(await task);
		}

		public async Task ConfiguredValueTaskAwaitable(ValueTask<int> task)
		{
			Console.WriteLine(await task.ConfigureAwait(continueOnCapturedContext: false));
		}
#endif

#if NET80
		public async Task ConfigureAwaitWithOptions(Task task)
		{
			await task.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
		}
#endif

		public async Task AwaitOfAwait(Task<Task<int>> task)
		{
			Console.WriteLine(await (await task));
		}
	}

#if CS72
	public struct ByRefReceiver
	{
		public long A;

		public long B;

		public TaskAwaiter Self()
		{
			return default(TaskAwaiter);
		}
	}
#endif

	public class ClassAwaitable : IAwaitable
	{
		public TaskAwaiter GetAwaiter()
		{
			return default(TaskAwaiter);
		}
	}

	public class ConvertsToAwaitable
	{
		public static implicit operator ClassAwaitable(ConvertsToAwaitable value)
		{
			return new ClassAwaitable();
		}
	}

	public class DerivedAwaitable : ClassAwaitable
	{
	}

	public class Disposable : IDisposable
	{
		public void Dispose()
		{
		}
	}

	public class ExplicitClassAwaitable : IAwaitable
	{
		TaskAwaiter IAwaitable.GetAwaiter()
		{
			return default(TaskAwaiter);
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ExplicitStructAwaitable : IAwaitable
	{
		TaskAwaiter IAwaitable.GetAwaiter()
		{
			return default(TaskAwaiter);
		}
	}

	public class GenericAwaitable<T>
	{
		public TaskAwaiter<T> GetAwaiter()
		{
			return default(TaskAwaiter<T>);
		}
	}

	public interface IAwaitable
	{
		TaskAwaiter GetAwaiter();
	}

	public interface IAwaitableMarker
	{
	}

	public interface IBaseAwaitable
	{
		TaskAwaiter GetAwaiter();
	}

	public interface IDerivedAwaitable : IBaseAwaitable
	{
	}

	public class MarkerClass : IAwaitableMarker
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct MarkerStruct : IAwaitableMarker
	{
	}

	public struct StructAwaitable : IAwaitable
	{
		public int Counter;

		public TaskAwaiter GetAwaiter()
		{
			Counter++;
			return default(TaskAwaiter);
		}
	}
}
