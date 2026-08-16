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

// Every member of this file is an await shape whose decompilation does not compile today.
// The file is written as the SPEC: input == expected output == correct C#, so a fixed
// decompiler makes the test pass with no edits here. Each member names the output that is
// produced instead. The test is ignored until all of them are fixed.

#pragma warning disable 1998
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncAwaitBugs
{
	public class AwaitPatternsThatDoNotRoundTrip
	{
		private static Task<int> Get()
		{
			return Task.FromResult(1);
		}

		/// <summary>
		/// The cast carries the operand to the interface that declares GetAwaiter; without it the
		/// explicit implementation is not accessible. ConvertTo(allowImplicitConversion: true)
		/// drops it because a boxing conversion exists.
		/// Today: <c>await value;</c> -> CS1929.
		/// </summary>
		public async Task ExplicitInterfaceImplementationOnStruct(ExplicitStructAwaitable value)
		{
			await (IAwaitable)value;
		}

		/// <summary>
		/// Same defect on a class, i.e. it is not specific to the boxing conversion.
		/// Today: <c>await value;</c> -> CS1929.
		/// </summary>
		public async Task ExplicitInterfaceImplementationOnClass(ExplicitClassAwaitable value)
		{
			await (IAwaitable)value;
		}

		/// <summary>
		/// The await pattern does not apply user-defined conversions, so the cast that invokes
		/// op_Implicit has to survive.
		/// Today: <c>await value;</c> -> CS1929.
		/// </summary>
		public async Task UserDefinedConversionToAwaitable(ConvertsToAwaitable value)
		{
			await (ClassAwaitable)value;
		}

		/// <summary>
		/// A null literal has no type, so the cast is what makes the operand awaitable.
		/// Today: <c>await null;</c> -> CS4001 "Cannot await '&lt;null&gt;'".
		/// </summary>
		public async Task AwaitNullTask()
		{
			await (Task)null;
		}

		/// <summary>
		/// Today: <c>await null;</c> -> CS4001, i.e. <c>default(Task)</c> is lost the same way.
		/// </summary>
		public async Task AwaitDefaultTask()
		{
			await default(Task);
		}

		/// <summary>
		/// An extension GetAwaiter taking its receiver by 'in' makes the expected type a
		/// ByReferenceType; VisitAwait strips the DirectionExpression and ConvertTo then converts
		/// the value back to a managed reference through a pointer.
		/// Today: <c>public unsafe async Task ...</c> with <c>await (ref *(ByRefReceiver*)value);</c>
		/// -> CS1525.
		/// </summary>
		public async Task InReceiverExtensionAwaiter(ByRefReceiver value)
		{
			await value;
		}

		/// <summary>
		/// The constrained callvirt lowers to an LdObjIfRef that ExpressionBuilder has no case
		/// for, and the operand is dropped entirely.
		/// Today: <c>await (IAwaitable)/*OpCode not supported: LdObjIfRef*/;</c> -> CS0119.
		/// </summary>
		public async Task TypeParameterWithInterfaceConstraint<T>(T value) where T : IAwaitable
		{
			await value;
		}

		/// <summary>
		/// A dynamic call to a static method whose argument list contains an await: the
		/// typeof(TargetType) marker of the call site is materialized as the receiver.
		/// Today: <c>Type typeFromHandle = typeof(Console); typeFromHandle.WriteLine(...);</c>
		/// -> CS1061. Without the await (or for an instance call) the same code is correct.
		/// </summary>
		public async Task DynamicAwaitInStaticCall(dynamic value)
		{
			Console.WriteLine("x" + await value);
		}

		/// <summary>
		/// The await splits the assignment across a suspension point, which defeats the
		/// with-expression transform and leaves the raw clone call behind.
		/// Today: <c>Record record = value._003CClone_003E_0024();</c> -> uncompilable.
		/// Without the await the same expression round-trips.
		/// </summary>
		public async Task<Record> WithExpressionContainingAwait(Record value)
		{
			return value with {
				X = await Get()
			};
		}
	}
	public static class ByRefAwaiterExtensions
	{
		public static TaskAwaiter GetAwaiter(this in ByRefReceiver receiver)
		{
			return receiver.Self();
		}
	}
	public struct ByRefReceiver
	{
		public long A;

		public long B;

		public TaskAwaiter Self()
		{
			return default(TaskAwaiter);
		}
	}
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
	public interface IAwaitable
	{
		TaskAwaiter GetAwaiter();
	}

	public record Record(int X);
}
