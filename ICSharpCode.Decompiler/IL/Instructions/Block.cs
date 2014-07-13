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
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	partial class Block : ILInstruction
	{
		public readonly InstructionCollection<ILInstruction> Instructions;
		
		public int IncomingEdgeCount;
		
		public Block() : base(OpCode.Block)
		{
			this.Instructions = new InstructionCollection<ILInstruction>(this);
		}
		
		public override StackType ResultType {
			get {
				return StackType.Void;
			}
		}
		
		/// <summary>
		/// Gets the name of this block.
		/// </summary>
		public string Label
		{
			get { return Disassembler.DisassemblerHelpers.OffsetToString(this.ILRange.Start); }
		}

		public override void WriteTo(ITextOutput output)
		{
			output.Write("Block ");
			output.WriteDefinition(Label, this);
			if (Parent is BlockContainer)
				output.Write(" (incoming: {0})", IncomingEdgeCount);
			output.WriteLine(" {");
			output.Indent();
			foreach (var inst in Instructions) {
				inst.WriteTo(output);
				output.WriteLine();
			}
			output.Unindent();
			output.Write("}");
		}
		
		public override IEnumerable<ILInstruction> Children {
			get { return Instructions; }
		}
		
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			for (int i = 0; i < Instructions.Count; i++) {
				Instructions[i] = Instructions[i].AcceptVisitor(visitor);
			}
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			var flags = InstructionFlags.None;
			foreach (var inst in Instructions)
				flags |= inst.Flags;
			return flags;
			
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			if (Instructions.Count > 0)
				Instructions[0] = Instructions[0].Inline(flagsBefore, instructionStack, out finished);
			else
				finished = true;
			return this;
		}
	}
}
