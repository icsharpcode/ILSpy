// Copyright (c) 2016 Siegfried Pammer
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

#nullable enable
using System;

namespace ICSharpCode.Decompiler.IL.Patterns
{
	partial class PatternInstruction : ILInstruction
	{
		public override void AcceptVisitor(ILVisitor visitor)
		{
			throw new NotSupportedException();
		}

		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			throw new NotSupportedException();
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			throw new NotSupportedException();
		}

		protected override InstructionFlags ComputeFlags()
		{
			throw new NotSupportedException();
		}

		public override InstructionFlags DirectFlags {
			get {
				throw new NotSupportedException();
			}
		}
	}

	partial class AnyNode : PatternInstruction
	{
		CaptureGroup? group;

		public AnyNode(CaptureGroup? group = null)
			: base(OpCode.AnyNode)
		{
			this.group = group;
		}

		protected internal override bool PerformMatch(ILInstruction? other, ref Match match)
		{
			if (other == null)
				return false;
			if (group != null)
				match.Add(group, other);
			return true;
		}
	}
}
