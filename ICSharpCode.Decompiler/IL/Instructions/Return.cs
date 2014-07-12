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

namespace ICSharpCode.Decompiler.IL
{
	partial class Return
	{
		ILInstruction argument;
		
		/// <summary>
		/// The value to return. Null if this return statement is within a void method.
		/// </summary>
		public ILInstruction Argument {
			get { return argument; }
			set { 
				if (value != null)
					ValidateArgument(value);
				SetChildInstruction(ref argument, value);
			}
		}
		
		public Return() : base(OpCode.Return)
		{
		}
		
		public Return(ILInstruction argument) : base(OpCode.Return)
		{
			this.Argument = argument;
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			if (Argument != null) {
				output.Write('(');
				Argument.WriteTo(output);
				output.Write(')');
			}
		}
		
		public override TAccumulate AggregateChildren<TSource, TAccumulate>(TAccumulate initial, ILVisitor<TSource> visitor, Func<TAccumulate, TSource, TAccumulate> func)
		{
			TAccumulate value = initial;
			if (argument != null)
				value = func(value, argument.AcceptVisitor(visitor));
			return value;
		}
		
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			if (argument != null)
				this.Argument = argument.AcceptVisitor(visitor);
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			if (argument != null)
				this.Argument = argument.Inline(flagsBefore, instructionStack, out finished);
			else
				finished = true;
			return this;
		}
	}
}
