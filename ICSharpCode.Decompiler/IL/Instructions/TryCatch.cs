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
	partial class TryCatch
	{
		public readonly InstructionCollection<TryCatchHandler> CatchHandlers;
		
		public TryCatch(ILInstruction tryBlock) : base(OpCode.TryCatch)
		{
			this.TryBlock = tryBlock;
			this.CatchHandlers = new InstructionCollection<TryCatchHandler>(this);
		}
		
		ILInstruction tryBlock;
		public ILInstruction TryBlock {
			get { return this.tryBlock; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.tryBlock, value);
			}
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write("try ");
			tryBlock.WriteTo(output);
			foreach (var handler in CatchHandlers) {
				output.Write(' ');
				handler.WriteTo(output);
			}
		}
		
		public override StackType ResultType {
			get { return StackType.Void; }
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			var flags = tryBlock.Flags;
			foreach (var handler in CatchHandlers)
				flags = IfInstruction.CombineFlags(flags, handler.Flags);
			return flags;
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			finished = false;
			return this;
		}
		
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return tryBlock;
				foreach (var handler in CatchHandlers)
					yield return handler;
			}
		}
		
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.TryBlock = tryBlock.AcceptVisitor(visitor);
			for (int i = 0; i < CatchHandlers.Count; i++) {
				if (CatchHandlers[i].AcceptVisitor(visitor) != CatchHandlers[i])
					throw new InvalidOperationException("Cannot transform a TryCatchHandler");
			}
		}
	}
	
	partial class TryCatchHandler
	{
		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			finished = false;
			return this;
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(" catch ");
			if (variable != null) {
				output.WriteDefinition(variable.Name, variable);
				output.Write(" : ");
				Disassembler.DisassemblerHelpers.WriteOperand(output, variable.Type);
			}
			output.Write(" if (");
			filter.WriteTo(output);
			output.Write(')');
			output.Write(' ');
			body.WriteTo(output);
		}
	}
}
