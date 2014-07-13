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
		ILInstruction returnValue;
		
		/// <summary>
		/// The value to return. Null if this return statement is within a void method.
		/// </summary>
		public ILInstruction ReturnValue {
			get { return returnValue; }
			set { 
				if (value != null)
					ValidateArgument(value);
				SetChildInstruction(ref returnValue, value);
			}
		}
		
		public Return() : base(OpCode.Return)
		{
		}
		
		public Return(ILInstruction argument) : base(OpCode.Return)
		{
			this.ReturnValue = argument;
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			if (returnValue != null) {
				output.Write('(');
				returnValue.WriteTo(output);
				output.Write(')');
			}
		}
		
		public override IEnumerable<ILInstruction> Children {
			get {
				if (returnValue != null)
					yield return returnValue;
			}
		}
		
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			if (returnValue != null)
				this.ReturnValue = returnValue.AcceptVisitor(visitor);
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			if (returnValue != null)
				this.ReturnValue = returnValue.Inline(flagsBefore, instructionStack, out finished);
			else
				finished = true;
			return this;
		}
	}
}
