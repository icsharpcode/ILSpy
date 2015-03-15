// Copyright (c) 2014 Daniel Grunwald
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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace ICSharpCode.Decompiler.IL
{
	public class TransformStackIntoVariablesState : IInlineContext
	{
		public Stack<ILVariable> Variables { get; set; }
		public UnionFind<ILVariable> UnionFind { get; set; }
		public Dictionary<Block, ImmutableArray<ILVariable>> InitialVariables { get; set; }
		public IDecompilerTypeSystem TypeSystem { get; set; }

		public TransformStackIntoVariablesState()
		{
			Variables = new Stack<ILVariable>();
			UnionFind = new UnionFind<ILVariable>();
			InitialVariables = new Dictionary<Block, ImmutableArray<ILVariable>>();
		}

		public void MergeVariables(Stack<ILVariable> a, Stack<ILVariable> b)
		{
			Debug.Assert(a.Count == b.Count);
			var enum1 = a.GetEnumerator();
			var enum2 = b.GetEnumerator();
			while (enum1.MoveNext() && enum2.MoveNext())
				UnionFind.Merge(enum1.Current, enum2.Current);
		}

		ILInstruction IInlineContext.Peek(InstructionFlags flagsBefore)
		{
			return new LdLoc(Variables.Peek());
		}

		ILInstruction IInlineContext.Pop(InstructionFlags flagsBefore)
		{
			return new LdLoc(Variables.Pop());
		}
	}
	
	public static class Extensions
	{
		public static Stack<T> Clone<T>(this Stack<T> inst)
		{
			if (inst == null)
				throw new ArgumentNullException("inst");
			return inst.ToImmutableArray().ToStack();
		}

		public static Stack<T> ToStack<T>(this ImmutableArray<T> inst)
		{
			var copy = new Stack<T>();
			for (int i = inst.Length - 1; i >= 0; i--)
				copy.Push(inst[i]);
			return copy;
		}
	}
}


