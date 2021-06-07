#nullable enable
// Copyright (c) 2017 Daniel Grunwald
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
	// Note: The comp instruction also supports three-valued logic via ComparisonLiftingKind.ThreeValuedLogic.
	// comp.i4.lifted[3VL](x == ldc.i4 0) is used to represent a lifted logic.not.

	partial class ThreeValuedBoolAnd : ILiftableInstruction
	{
		bool ILiftableInstruction.IsLifted => true;
		StackType ILiftableInstruction.UnderlyingResultType => StackType.I4;

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(Left.ResultType == StackType.I4 || Left.ResultType == StackType.O);
		}
	}

	partial class ThreeValuedBoolOr : ILiftableInstruction
	{
		bool ILiftableInstruction.IsLifted => true;
		StackType ILiftableInstruction.UnderlyingResultType => StackType.I4;

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(Left.ResultType == StackType.I4 || Left.ResultType == StackType.O);
		}
	}

	partial class UserDefinedLogicOperator
	{
		protected override InstructionFlags ComputeFlags()
		{
			// left is always executed; right only sometimes
			return DirectFlags | left.Flags
				| SemanticHelper.CombineBranches(InstructionFlags.None, right.Flags);
		}

		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayThrow | InstructionFlags.SideEffect | InstructionFlags.ControlFlow;
			}
		}
	}
}
