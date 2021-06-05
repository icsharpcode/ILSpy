#nullable enable
// Copyright (c) 2017 Siegfried Pammer
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

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Kind of null-coalescing operator.
	/// ILAst: <c>if.notnull(valueInst, fallbackInst)</c>
	/// C#: <c>value ?? fallback</c>
	/// </summary>
	public enum NullCoalescingKind
	{
		/// <summary>
		/// Both ValueInst and FallbackInst are of reference type.
		/// 
		/// Semantics: equivalent to "valueInst != null ? valueInst : fallbackInst",
		///            except that valueInst is evaluated only once.
		/// </summary>
		Ref,
		/// <summary>
		/// Both ValueInst and FallbackInst are of type Nullable{T}.
		/// 
		/// Semantics: equivalent to "valueInst.HasValue ? valueInst : fallbackInst",
		///            except that valueInst is evaluated only once.
		/// </summary>
		Nullable,
		/// <summary>
		/// ValueInst is Nullable{T}, but FallbackInst is non-nullable value type.
		/// 
		/// Semantics: equivalent to "valueInst.HasValue ? valueInst.Value : fallbackInst",
		///            except that valueInst is evaluated only once.
		/// </summary>
		NullableWithValueFallback
	}

	partial class NullCoalescingInstruction
	{
		public readonly NullCoalescingKind Kind;
		public StackType UnderlyingResultType = StackType.O;

		public NullCoalescingInstruction(NullCoalescingKind kind, ILInstruction valueInst, ILInstruction fallbackInst) : base(OpCode.NullCoalescingInstruction)
		{
			this.Kind = kind;
			this.ValueInst = valueInst;
			this.FallbackInst = fallbackInst;
		}

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(valueInst.ResultType == StackType.O); // lhs is reference type or nullable type
			Debug.Assert(fallbackInst.ResultType == StackType.O || Kind == NullCoalescingKind.NullableWithValueFallback);
			Debug.Assert(ResultType == UnderlyingResultType || Kind == NullCoalescingKind.Nullable);
		}

		public override StackType ResultType {
			get {
				return fallbackInst.ResultType;
			}
		}

		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.ControlFlow;
			}
		}

		protected override InstructionFlags ComputeFlags()
		{
			// valueInst is always executed; fallbackInst only sometimes
			return InstructionFlags.ControlFlow | valueInst.Flags
				| SemanticHelper.CombineBranches(InstructionFlags.None, fallbackInst.Flags);
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write("(");
			valueInst.WriteTo(output, options);
			output.Write(", ");
			fallbackInst.WriteTo(output, options);
			output.Write(")");
		}
	}
}
