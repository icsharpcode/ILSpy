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
using System.Diagnostics;
using System.Linq;

namespace ICSharpCode.Decompiler.IL
{
	public abstract class UnaryInstruction : ILInstruction
	{
		ILInstruction argument;
		
		public ILInstruction Argument {
			get { return argument; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref argument, value);
			}
		}
		
		protected UnaryInstruction(OpCode opCode, ILInstruction argument) : base(opCode)
		{
			this.Argument = argument;
		}

		//public sealed override bool IsPeeking { get { return Operand.IsPeeking; } }

		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write('(');
			argument.WriteTo(output);
			output.Write(')');
		}
		
		public sealed override TAccumulate AggregateChildren<TSource, TAccumulate>(TAccumulate initial, ILVisitor<TSource> visitor, Func<TAccumulate, TSource, TAccumulate> func)
		{
			return func(initial, argument.AcceptVisitor(visitor));
		}
		
		public sealed override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Argument = argument.AcceptVisitor(visitor);
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			Argument = argument.Inline(flagsBefore, instructionStack, out finished);
			return this;
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			return argument.Flags;
		}
	}
	
	partial class BitNot
	{
		public override StackType ResultType {
			get {
				return Argument.ResultType;
			}
		}
	}
}
 