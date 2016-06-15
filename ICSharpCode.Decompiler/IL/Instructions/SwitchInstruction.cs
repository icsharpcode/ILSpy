// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Description of SwitchInstruction.
	/// </summary>
	partial class SwitchInstruction
	{
		public static readonly SlotInfo ValueSlot = new SlotInfo("Value", canInlineInto: true);
		public static readonly SlotInfo SectionSlot = new SlotInfo("Section", isCollection: true);
		
		public SwitchInstruction(ILInstruction value)
			: base(OpCode.SwitchInstruction)
		{
			this.Value = value;
			this.Sections = new InstructionCollection<SwitchSection>(this, 1);
		}
		
		ILInstruction value;
		public ILInstruction Value {
			get { return this.value; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.value, value, 0);
			}
		}
		
		public readonly InstructionCollection<SwitchSection> Sections;
		
		protected override InstructionFlags ComputeFlags()
		{
			var sectionFlags = InstructionFlags.ControlFlow;
			foreach (var section in Sections) {
				sectionFlags = IfInstruction.CombineFlags(sectionFlags, section.Flags);
			}
			return value.Flags | sectionFlags;
		}
		
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.ControlFlow;
			}
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write("switch (");
			value.WriteTo(output);
			output.WriteLine(") {");
			output.Indent();
			foreach (var section in this.Sections) {
				section.WriteTo(output);
				output.WriteLine();
			}
			output.Unindent();
			output.Write('}');
		}
		
		protected override int GetChildCount()
		{
			return 1 + Sections.Count;
		}
		
		protected override ILInstruction GetChild(int index)
		{
			if (index == 0)
				return value;
			return Sections[index - 1];
		}
		
		protected override void SetChild(int index, ILInstruction value)
		{
			if (index == 0)
				Value = value;
			else
				Sections[index - 1] = (SwitchSection)value;
		}
		
		protected override SlotInfo GetChildSlot(int index)
		{
			if (index == 0)
				return ValueSlot;
			return SectionSlot;
		}
		
		public override ILInstruction Clone()
		{
			var clone = new SwitchInstruction(value.Clone());
			clone.ILRange = this.ILRange;
			clone.Value = value.Clone();
			clone.Sections.AddRange(this.Sections.Select(h => (SwitchSection)h.Clone()));
			return clone;
		}
	}
	
	partial class SwitchSection
	{
		public SwitchSection()
			: base(OpCode.SwitchSection)
		{
			
		}

		public LongSet Labels { get; set; }
		
		protected override InstructionFlags ComputeFlags()
		{
			return body.Flags;
		}
		
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.None;
			}
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write("case ");
			output.Write(Labels.ToString());
			output.Write(": ");
			
			body.WriteTo(output);
		}
	}
}
