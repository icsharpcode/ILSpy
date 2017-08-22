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

using System;
using System.Collections.Generic;
using System.Threading;
using ICSharpCode.Decompiler.IL.Transforms;
using Mono.Cecil;
using ICSharpCode.Decompiler.Disassembler;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using System.Diagnostics;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Null coalescing operator expression. <c>if.notnull(valueInst, fallbackInst)</c></summary>
	partial class NullCoalescingInstruction
	{
		public NullCoalescingInstruction(ILInstruction valueInst, ILInstruction fallbackInst) : base(OpCode.NullCoalescingInstruction)
		{
			this.ValueInst = valueInst;
			this.FallbackInst = fallbackInst;
		}

		public override StackType ResultType {
			get {
				return CommonResultType(valueInst.ResultType, fallbackInst.ResultType);
			}
		}

		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.ControlFlow;
			}
		}

		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.ControlFlow | SemanticHelper.CombineBranches(valueInst.Flags, fallbackInst.Flags);
		}

		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(" (");
			valueInst.WriteTo(output);
			output.Write(", ");
			fallbackInst.WriteTo(output);
			output.Write(")");

		}
	}
}
