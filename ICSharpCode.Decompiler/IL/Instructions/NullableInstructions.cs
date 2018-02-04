// Copyright (c) 2018 Daniel Grunwald
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
	partial class NullableUnwrap
	{
		public NullableUnwrap(StackType unwrappedType, ILInstruction argument)
			: base(OpCode.NullableUnwrap, argument)
		{
			this.ResultType = unwrappedType;
		}

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(Argument.ResultType == StackType.O, "nullable.unwrap expects nullable type as input");
			Debug.Assert(Ancestors.Any(a => a is NullableRewrap));
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			output.Write("nullable.unwrap ");
			output.Write(ResultType);
			output.Write('(');
			Argument.WriteTo(output, options);
			output.Write(')');
		}

		public override StackType ResultType { get; }
	}

	partial class NullableRewrap
	{
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(Argument.HasFlag(InstructionFlags.MayUnwrapNull));
		}

		public override InstructionFlags DirectFlags => InstructionFlags.ControlFlow;

		protected override InstructionFlags ComputeFlags()
		{
			// Convert MayUnwrapNull flag to ControlFlow flag.
			// Also, remove EndpointUnreachable flag, because the end-point is reachable through
			// the implicit nullable.unwrap branch.
			const InstructionFlags flagsToRemove = InstructionFlags.MayUnwrapNull | InstructionFlags.EndPointUnreachable;
			return (Argument.Flags & ~flagsToRemove) | InstructionFlags.ControlFlow;
		}

		public override StackType ResultType {
			get {
				if (Argument.ResultType == StackType.Void)
					return StackType.Void;
				else
					return StackType.O;
			}
		}
	}
}
