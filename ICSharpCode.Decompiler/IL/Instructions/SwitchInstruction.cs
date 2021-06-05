#nullable enable
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

using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Generalization of IL switch-case: like a VB switch over integers, this instruction
	/// supports integer value ranges as labels.
	/// 
	/// The section labels are using 'long' as integer type.
	/// If the Value instruction produces StackType.I4 or I, the value is implicitly sign-extended to I8.
	/// </summary>
	partial class SwitchInstruction
	{
		public static readonly SlotInfo ValueSlot = new SlotInfo("Value", canInlineInto: true);
		public static readonly SlotInfo SectionSlot = new SlotInfo("Section", isCollection: true);

		/// <summary>
		/// If the switch instruction is lifted, the value instruction produces a value of type <c>Nullable{T}</c> for some
		/// integral type T. The section with <c>SwitchSection.HasNullLabel</c> is called if the value is null.
		/// </summary>
		public bool IsLifted;

		public SwitchInstruction(ILInstruction value)
			: base(OpCode.SwitchInstruction)
		{
			this.Value = value;
			this.Sections = new InstructionCollection<SwitchSection>(this, 1);
		}

		ILInstruction value = null!;
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
			var sectionFlags = InstructionFlags.EndPointUnreachable; // neutral element for CombineBranches()
			foreach (var section in Sections)
			{
				sectionFlags = SemanticHelper.CombineBranches(sectionFlags, section.Flags);
			}
			return value.Flags | InstructionFlags.ControlFlow | sectionFlags;
		}

		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.ControlFlow;
			}
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write("switch");
			if (IsLifted)
				output.Write(".lifted");
			output.Write(" (");
			value.WriteTo(output, options);
			output.Write(") ");
			output.MarkFoldStart("{...}");
			output.WriteLine("{");
			output.Indent();
			foreach (var section in this.Sections)
			{
				section.WriteTo(output, options);
				output.WriteLine();
			}
			output.Unindent();
			output.Write('}');
			output.MarkFoldEnd();
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
			clone.AddILRange(this);
			clone.Value = value.Clone();
			clone.Sections.AddRange(this.Sections.Select(h => (SwitchSection)h.Clone()));
			return clone;
		}

		StackType resultType = StackType.Void;

		public override StackType ResultType => resultType;

		public void SetResultType(StackType resultType)
		{
			this.resultType = resultType;
		}

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			bool expectNullSection = this.IsLifted;
			LongSet sets = LongSet.Empty;
			foreach (var section in Sections)
			{
				if (section.HasNullLabel)
				{
					Debug.Assert(expectNullSection, "Duplicate 'case null' or 'case null' in non-lifted switch.");
					expectNullSection = false;
				}
				Debug.Assert(!section.Labels.IsEmpty || section.HasNullLabel);
				Debug.Assert(!section.Labels.Overlaps(sets));
				Debug.Assert(section.Body.ResultType == this.ResultType);
				sets = sets.UnionWith(section.Labels);
			}
			Debug.Assert(sets.SetEquals(LongSet.Universe), "switch does not handle all possible cases");
			Debug.Assert(!expectNullSection, "Lifted switch is missing 'case null'");
			Debug.Assert(this.IsLifted ? (value.ResultType == StackType.O) : (value.ResultType == StackType.I4 || value.ResultType == StackType.I8));
		}

		public SwitchSection GetDefaultSection()
		{
			// Pick the section with the most labels as default section.
			IL.SwitchSection defaultSection = Sections.First();
			foreach (var section in Sections)
			{
				if (section.Labels.Count() > defaultSection.Labels.Count())
				{
					defaultSection = section;
				}
			}
			return defaultSection;
		}
	}

	partial class SwitchSection
	{
		public SwitchSection()
			: base(OpCode.SwitchSection)
		{
			this.Labels = LongSet.Empty;
		}

		/// <summary>
		/// If true, serves as 'case null' in a lifted switch.
		/// </summary>
		public bool HasNullLabel { get; set; }

		/// <summary>
		/// The set of labels that cause execution to jump to this switch section.
		/// </summary>
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

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.WriteLocalReference("case", this, isDefinition: true);
			output.Write(' ');
			if (HasNullLabel)
			{
				output.Write("null");
				if (!Labels.IsEmpty)
				{
					output.Write(", ");
					output.Write(Labels.ToString());
				}
			}
			else
			{
				output.Write(Labels.ToString());
			}
			output.Write(": ");

			body.WriteTo(output, options);
		}
	}
}
