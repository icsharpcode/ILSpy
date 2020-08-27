// Copyright (c) 2014 Daniel Grunwald
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


namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// A simple instruction that does not have any arguments.
	/// </summary>
	public abstract partial class SimpleInstruction : ILInstruction
	{
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			// the non-custom WriteTo would add useless parentheses
		}
	}

	public enum NopKind
	{
		Normal,
		Pop
	}

	partial class Nop
	{
		public string Comment;

		public NopKind Kind;

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			if (Kind != NopKind.Normal)
			{
				output.Write("." + Kind.ToString().ToLowerInvariant());
			}
			if (!string.IsNullOrEmpty(Comment))
			{
				output.Write(" // " + Comment);
			}
		}
	}

	partial class InvalidBranch : SimpleInstruction
	{
		public string Message;
		public StackType ExpectedResultType = StackType.Void;

		public InvalidBranch(string message) : this()
		{
			this.Message = message;
		}

		public override StackType ResultType {
			get { return ExpectedResultType; }
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			if (!string.IsNullOrEmpty(Message))
			{
				output.Write("(\"");
				output.Write(Message);
				output.Write("\")");
			}
		}
	}

	partial class InvalidExpression : SimpleInstruction
	{
		public string Message;
		public StackType ExpectedResultType = StackType.Unknown;

		public InvalidExpression(string message) : this()
		{
			this.Message = message;
		}

		public override StackType ResultType {
			get { return ExpectedResultType; }
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			if (!string.IsNullOrEmpty(Message))
			{
				output.Write("(\"");
				output.Write(Message);
				output.Write("\")");
			}
		}
	}
}
