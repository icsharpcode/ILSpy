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

#pragma warning disable 1998
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	// The Pretty fixture of the same name pins how awaits are printed; this one pins what they
	// have to mean: copy semantics of struct awaitables, and the evaluation order around the
	// suspension point.
	public class AsyncAwaitPatterns
	{
		public struct CountingAwaitable
		{
			public int Counter;

			public TaskAwaiter<int> GetAwaiter()
			{
				Counter++;
				Console.WriteLine("  GetAwaiter, Counter is now " + Counter);
				return Task.FromResult(0).GetAwaiter();
			}
		}

		public class Holder
		{
			public CountingAwaitable Mutable;
			public readonly CountingAwaitable ReadOnly;

			public CountingAwaitable Property {
				get { return Mutable; }
			}
		}

		private int[] array = new int[4];
		private int index;
		private int field;

		public static void Main()
		{
			new AsyncAwaitPatterns().Run().Wait();
		}

		public async Task Run()
		{
			await MutableStructField();
			await ReadOnlyStructField();
			await StructProperty();
			await CompoundAssignmentToArrayElement();
			await AssignmentAfterAwait();
			await ArgumentEvaluationOrder();
			await RefArgumentEvaluationOrder();
			await AwaitInLoop();
			await AwaitInTernary(true);
			await AwaitInTernary(false);
#if CS60
			await AwaitInCatchAndFinally();
#endif
			Console.WriteLine("done");
		}

		private Task<int> Value(int v)
		{
			Console.WriteLine("  Value(" + v + ")");
			return Task.FromResult(v);
		}

		private int Index(string tag)
		{
			Console.WriteLine("  Index(" + tag + ") -> " + index);
			return index;
		}

		private int[] Array(string tag)
		{
			Console.WriteLine("  Array(" + tag + ")");
			return array;
		}

		private int Side()
		{
			Console.WriteLine("  Side()");
			return 100;
		}

		private static string Combine(int a, int b, int c)
		{
			return a + "/" + b + "/" + c;
		}

		private static void AddTo(ref int slot, int addend)
		{
			Console.WriteLine("  AddTo(" + slot + ", " + addend + ")");
			slot += addend;
		}

		// GetAwaiter is called on the field itself, so its mutation sticks.
		public async Task MutableStructField()
		{
			Console.WriteLine("MutableStructField");
			Holder holder = new Holder();
			await holder.Mutable;
			await holder.Mutable;
			Console.WriteLine("  Counter = " + holder.Mutable.Counter);
		}

		// A readonly field is defensively copied, so the mutation is discarded.
		public async Task ReadOnlyStructField()
		{
			Console.WriteLine("ReadOnlyStructField");
			Holder holder = new Holder();
			await holder.ReadOnly;
			await holder.ReadOnly;
			Console.WriteLine("  Counter = " + holder.ReadOnly.Counter);
		}

		// A property returns a copy, so the mutation is discarded as well.
		public async Task StructProperty()
		{
			Console.WriteLine("StructProperty");
			Holder holder = new Holder();
			await holder.Property;
			await holder.Property;
			Console.WriteLine("  Counter = " + holder.Mutable.Counter);
		}

		// Target and index are evaluated before the await, not after it.
		public async Task CompoundAssignmentToArrayElement()
		{
			Console.WriteLine("CompoundAssignmentToArrayElement");
			array = new int[4];
			index = 0;
			Array("lhs")[Index("lhs")] += await Value(5);
			index = 1;
			Console.WriteLine("  array = " + string.Join(",", array));
		}

		public async Task AssignmentAfterAwait()
		{
			Console.WriteLine("AssignmentAfterAwait");
			array = new int[4];
			index = 2;
			int[] target = Array("target");
			int i = Index("i");
			index = 3;
			target[i] = await Value(7);
			Console.WriteLine("  array = " + string.Join(",", array));
		}

		public async Task ArgumentEvaluationOrder()
		{
			Console.WriteLine("ArgumentEvaluationOrder");
			Console.WriteLine("  " + Combine(await Value(1), Side(), await Value(2)));
		}

		public async Task RefArgumentEvaluationOrder()
		{
			Console.WriteLine("RefArgumentEvaluationOrder");
			field = 0;
			AddTo(ref field, await Value(6));
			Console.WriteLine("  field = " + field);
		}

		public async Task AwaitInLoop()
		{
			Console.WriteLine("AwaitInLoop");
			for (int i = 0; i < 4; i++)
			{
				if (i == 1)
				{
					continue;
				}
				if (i == 3)
				{
					break;
				}
				Console.WriteLine("  loop " + await Value(i));
			}
		}

		public async Task AwaitInTernary(bool condition)
		{
			Console.WriteLine("AwaitInTernary(" + condition + ")");
			Console.WriteLine("  " + (condition ? await Value(1) : await Value(2)));
		}

#if CS60
		public async Task AwaitInCatchAndFinally()
		{
			Console.WriteLine("AwaitInCatchAndFinally");
			try
			{
				await Value(1);
				throw new InvalidOperationException("boom");
			}
			catch (InvalidOperationException ex)
			{
				Console.WriteLine("  caught " + ex.Message);
				await Value(2);
			}
			finally
			{
				Console.WriteLine("  finally");
				await Value(3);
			}
		}
#endif
	}
}
