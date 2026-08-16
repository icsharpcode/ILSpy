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

// Await shapes where the cast in front of the operand is load-bearing: dropping it either makes
// GetAwaiter unreachable or leaves the operand with no type at all. Each member here once
// decompiled to code that does not compile, so the file doubles as a regression test - it is
// written as the C# the decompiler has to produce, and a relapse shows up as a diff.
//
// Await shapes that still decompile to uncompilable code are tracked as #4017 (type parameter
// with an interface constraint), #4018 (static dynamic call) and #4019 (with expression); they
// are not covered here because they have no correct output to pin yet.

#pragma warning disable 1998
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncAwaitBugs
{
	public class AwaitPatternsThatDoNotRoundTrip
	{
		/// <summary>
		/// The cast carries the operand to the interface that declares GetAwaiter; without it the
		/// explicit implementation is not accessible. A conversion that is merely implicit must not
		/// be dropped here, even though a boxing conversion exists.
		/// </summary>
		public async Task ExplicitInterfaceImplementationOnStruct(ExplicitStructAwaitable value)
		{
			await (IAwaitable)value;
		}

		/// <summary>
		/// The same shape on a class, i.e. it is not specific to the boxing conversion.
		/// </summary>
		public async Task ExplicitInterfaceImplementationOnClass(ExplicitClassAwaitable value)
		{
			await (IAwaitable)value;
		}

		/// <summary>
		/// The await pattern does not apply user-defined conversions, so the cast that invokes
		/// op_Implicit has to survive.
		/// </summary>
		public async Task UserDefinedConversionToAwaitable(ConvertsToAwaitable value)
		{
			await (ClassAwaitable)value;
		}

		/// <summary>
		/// A null literal has no type, so the cast is what makes the operand awaitable. Note that
		/// <c>default(Task)</c> compiles to the same `ldnull` and therefore decompiles to this same
		/// cast; the two are indistinguishable in IL.
		/// </summary>
		public async Task AwaitNullTask()
		{
			await (Task)null;
		}

		/// <summary>
		/// An extension GetAwaiter taking its receiver by 'in' makes the expected type a
		/// ByReferenceType. Stripping the 'ref' must not leave a conversion that reaches the
		/// managed reference back through a pointer.
		/// </summary>
		public async Task InReceiverExtensionAwaiter(ByRefReceiver value)
		{
			await value;
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
}
