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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.NRefactory;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// A simple instruction that does not have any arguments.
	/// </summary>
	public abstract class SimpleInstruction : ILInstruction
	{
		protected SimpleInstruction(OpCode opCode) : base(opCode)
		{
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
		}
		
		protected override int GetChildCount()
		{
			return 0;
		}
		
		protected override ILInstruction GetChild(int index)
		{
			throw new IndexOutOfRangeException();
		}
		
		protected override void SetChild(int index, ILInstruction value)
		{
			throw new IndexOutOfRangeException();
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.None;
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			// Nothing to do, since we don't have arguments.
			return this;
		}

		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
		}
	}

	partial class Pop
	{
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.MayPop;
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			return context.Pop(flagsBefore) ?? this;
		}
	}

	partial class Peek
	{
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.MayPeek;
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			return context.Peek(flagsBefore) ?? this;
		}
	}
}
