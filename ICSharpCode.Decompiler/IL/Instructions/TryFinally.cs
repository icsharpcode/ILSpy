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
namespace ICSharpCode.Decompiler.IL
{
	partial class TryFinally
	{
		public override void WriteTo(ITextOutput output)
		{
			output.Write("try ");
			tryBlock.WriteTo(output);
			output.Write(" finally ");
			finallyBlock.WriteTo(output);
		}

		public override StackType ResultType {
			get {
				return tryBlock.ResultType;
			}
		}

		protected override InstructionFlags ComputeFlags()
		{
			// if the endpoint of either the try or the finally is unreachable, the endpoint of the try-finally will be unreachable
			return tryBlock.Flags | finallyBlock.Flags;
		}

		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			finished = false;
			return this;
		}
	}
	
	partial class TryFault
	{
		public override void WriteTo(ITextOutput output)
		{
			output.Write("try ");
			tryBlock.WriteTo(output);
			output.Write(" fault ");
			faultBlock.WriteTo(output);
		}
		
		public override StackType ResultType {
			get { return tryBlock.ResultType; }
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			// The endpoint of the try-fault is unreachable only if both endpoints are unreachable
			return IfInstruction.CombineFlags(tryBlock.Flags, faultBlock.Flags);
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			finished = false;
			return this;
		}
	}
}


