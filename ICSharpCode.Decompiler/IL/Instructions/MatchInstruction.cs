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

namespace ICSharpCode.Decompiler.IL
{
	partial class MatchInstruction : ILInstruction
	{
		public bool IsPattern(ILInstruction inst, out ILInstruction testedOperand)
		{
			switch (inst) {
				case MatchInstruction m:
					testedOperand = m.testedOperand;
					return true;
				case Comp comp:
					testedOperand = comp.Left;
					return IsConstant(comp.Right);
				case ILInstruction logicNot when logicNot.MatchLogicNot(out var operand):
					return IsPattern(operand, out testedOperand);
				default:
					testedOperand = null;
					return false;
			}
		}

		private static bool IsConstant(ILInstruction inst)
		{
			return inst.OpCode switch
			{
				OpCode.LdcDecimal => true,
				OpCode.LdcF4 => true,
				OpCode.LdcF8 => true,
				OpCode.LdcI4 => true,
				OpCode.LdcI8 => true,
				OpCode.LdNull => true,
				_ => false
			};
		}

		void AdditionalInvariants()
		{
			foreach (var subPattern in SubPatterns) {
				ILInstruction operand;
				Debug.Assert(IsPattern(subPattern, out operand));
			}
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			output.Write('(');
			Variable.WriteTo(output);
			output.Write(" = ");
			TestedOperand.WriteTo(output, options);
			output.Write(')');
		}
	}
}
