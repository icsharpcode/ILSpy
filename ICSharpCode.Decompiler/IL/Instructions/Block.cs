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
			output.WriteDefinition("Block " + Label, this);
			output.WriteLine(" {");
			output.Indent();
			foreach (var inst in Instructions) {
				inst.WriteTo(output);
				output.WriteLine();
			}
			output.Unindent();
			output.WriteLine("}");
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
