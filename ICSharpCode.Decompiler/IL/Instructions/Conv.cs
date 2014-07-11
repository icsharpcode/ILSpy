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
		public readonly StackType FromType;
		public readonly PrimitiveType ToType;
		public readonly OverflowMode ConvMode;
		
		public Conv(StackType fromType, PrimitiveType toType, OverflowMode convMode) : base(OpCode.Conv)
		{
			this.FromType = fromType;
			this.ToType = toType;
			this.ConvMode = convMode;
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.WriteSuffix(ConvMode);
			output.Write(' ');
			output.Write(FromType);
			output.Write("->");
			output.Write(ToType);
			output.Write('(');
			Argument.WriteTo(output);
			output.Write(')');
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			var flags = base.ComputeFlags();
			if ((ConvMode & OverflowMode.Ovf) != 0)
				flags |= InstructionFlags.MayThrow;
			return flags;
		}
	}
}
