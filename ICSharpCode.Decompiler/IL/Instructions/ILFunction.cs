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
using Mono.Cecil;
using ICSharpCode.Decompiler.Disassembler;

namespace ICSharpCode.Decompiler.IL
{
	partial class ILFunction
	{
		public readonly MethodDefinition Method;
		public readonly IList<ILVariable> Variables = new List<ILVariable>();
		
		public ILFunction(MethodDefinition method, ILInstruction body) : base(OpCode.ILFunction)
		{
			this.Body = body;
			this.Method = method;
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Method.WriteTo(output);
			output.WriteLine(" {");
			output.Indent();
			
			foreach (var variable in Variables) {
				variable.WriteDefinitionTo(output);
				output.WriteLine();
			}
			output.WriteLine();
			body.WriteTo(output);
			
			output.Unindent();
			output.WriteLine("}");
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			// Creating a lambda may throw OutOfMemoryException
			return InstructionFlags.MayThrow;
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			// To the outside, lambda creation looks like a constant
			finished = true;
			return this;
		}
	}
}
