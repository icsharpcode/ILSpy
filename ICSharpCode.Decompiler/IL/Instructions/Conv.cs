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

using System;

namespace ICSharpCode.Decompiler.IL
{
	partial class Conv : UnaryInstruction
	{
		public readonly PrimitiveType TargetType;
		public readonly bool CheckForOverflow;
		
		/// <summary>
		/// Gets the sign of the input type.
		/// </summary>
		public readonly Sign Sign;
		
		public Conv(ILInstruction argument, PrimitiveType targetType, bool checkForOverflow, Sign sign) : base(OpCode.Conv, argument)
		{
			this.TargetType = targetType;
			this.CheckForOverflow = checkForOverflow;
			this.Sign = sign;
		}
		
		public override StackType ResultType {
			get { return TargetType.GetStackType(); }
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			if (CheckForOverflow)
				output.Write(".ovf");
			if (Sign == Sign.Unsigned)
				output.Write(".unsigned");
			else if (Sign == Sign.Signed)
				output.Write(".signed");
			output.Write(' ');
			output.Write(Argument.ResultType);
			output.Write("->");
			output.Write(TargetType);
			output.Write('(');
			Argument.WriteTo(output);
			output.Write(')');
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			var flags = base.ComputeFlags();
			if (CheckForOverflow)
				flags |= InstructionFlags.MayThrow;
			return flags;
		}
	}
}
