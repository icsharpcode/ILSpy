#nullable enable
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

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	partial class DeconstructResultInstruction
	{
		public int Index { get; }

		public override StackType ResultType { get; }

		public DeconstructResultInstruction(int index, StackType resultType, ILInstruction argument)
			: base(OpCode.DeconstructResultInstruction, argument)
		{
			Debug.Assert(index >= 0);
			Index = index;
			ResultType = resultType;
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			output.Write(Index.ToString());
			output.Write('(');
			this.Argument.WriteTo(output, options);
			output.Write(')');
		}

		MatchInstruction? FindMatch()
		{
			for (ILInstruction? inst = this; inst != null; inst = inst.Parent)
			{
				if (inst.Parent is MatchInstruction match && inst != match.TestedOperand)
					return match;
			}
			return null;
		}

		void AdditionalInvariants()
		{
			var matchInst = FindMatch();
			Debug.Assert(matchInst != null && (matchInst.IsDeconstructCall || matchInst.IsDeconstructTuple));
			Debug.Assert(Argument.MatchLdLoc(matchInst!.Variable));
			var outParamType = matchInst.GetDeconstructResultType(this.Index);
			Debug.Assert(outParamType.GetStackType() == ResultType);
		}
	}
}
