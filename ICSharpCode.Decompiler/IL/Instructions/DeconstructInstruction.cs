// Copyright (c) 2020 Siegfried Pammer
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

using System.Diagnostics;
using System.Linq;

namespace ICSharpCode.Decompiler.IL
{
	partial class DeconstructInstruction
	{
		public static readonly SlotInfo InitSlot = new SlotInfo("Init", canInlineInto: true, isCollection: true);
		public static readonly SlotInfo DeconstructSlot = new SlotInfo("Deconstruct", canInlineInto: true);
		public static readonly SlotInfo ConversionsSlot = new SlotInfo("Conversions");
		public static readonly SlotInfo AssignmentsSlot = new SlotInfo("Assignments");

		public DeconstructInstruction()
			: base(OpCode.DeconstructInstruction)
		{
			this.Init = new InstructionCollection<StLoc>(this, 0);
		}

		public readonly InstructionCollection<StLoc> Init;

		ILInstruction deconstruct;
		public ILInstruction Deconstruct {
			get { return this.deconstruct; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.deconstruct, value, Init.Count);
			}
		}

		Block conversions;
		public Block Conversions {
			get { return this.conversions; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.conversions, value, Init.Count + 1);
			}
		}

		Block assignments;
		public Block Assignments {
			get { return this.assignments; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.assignments, value, Init.Count + 2);
			}
		}

		protected sealed override int GetChildCount()
		{
			return Init.Count + 3;
		}

		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index - Init.Count) {
				case 0:
					return this.deconstruct;
				case 1:
					return this.conversions;
				case 2:
					return this.assignments;
				default:
					return this.Init[index];
			}
		}

		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index - Init.Count) {
				case 0:
					this.Deconstruct = value;
					break;
				case 1:
					this.Conversions = (Block)value;
					break;
				case 2:
					this.Assignments = (Block)value;
					break;
				default:
					this.Init[index] = (StLoc)value;
					break;
			}
		}

		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index - Init.Count) {
				case 0:
					return DeconstructSlot;
				case 1:
					return ConversionsSlot;
				case 2:
					return AssignmentsSlot;
				default:
					return InitSlot;
			}
		}

		public sealed override ILInstruction Clone()
		{
			var clone = new DeconstructInstruction();
			clone.Init.AddRange(this.Init.Select(inst => (StLoc)inst.Clone()));
			clone.Deconstruct = this.deconstruct.Clone();
			clone.Conversions = (Block)this.conversions.Clone();
			clone.Assignments = (Block)this.assignments.Clone();
			return clone;
		}

		protected override InstructionFlags ComputeFlags()
		{
			var flags = InstructionFlags.None;
			foreach (var inst in Init) {
				flags |= inst.Flags;
			}
			flags |= deconstruct.Flags | conversions.Flags | assignments.Flags;
			return flags;
		}

		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.None;
			}
		}

		protected internal override void InstructionCollectionUpdateComplete()
		{
			base.InstructionCollectionUpdateComplete();
			if (deconstruct.Parent == this)
				deconstruct.ChildIndex = Init.Count;
			if (conversions.Parent == this)
				conversions.ChildIndex = Init.Count + 1;
			if (assignments.Parent == this)
				assignments.ChildIndex = Init.Count + 2;
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write("deconstruct");
			output.MarkFoldStart("{...}");
			output.WriteLine("{");
			output.Indent();
			output.WriteLine("init:");
			output.Indent();
			foreach (var inst in this.Init) {
				inst.WriteTo(output, options);
				output.WriteLine();
			}
			output.Unindent();
			output.WriteLine("deconstruct:");
			output.Indent();
			deconstruct.WriteTo(output, options);
			output.Unindent();
			output.Write("conversions:");
			conversions.WriteTo(output, options);
			output.Write("assignments: ");
			assignments.WriteTo(output, options);
			output.Unindent();
			output.Write('}');
			output.MarkFoldEnd();
		}

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			foreach (var init in this.Init) {
				Debug.Assert(init.Variable.IsSingleDefinition && init.Variable.LoadCount == 1);
				Debug.Assert(init.Variable.LoadInstructions[0].IsDescendantOf(assignments));
			}
		}
	}
}
