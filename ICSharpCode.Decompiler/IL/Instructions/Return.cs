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
using System.Diagnostics;

namespace ICSharpCode.Decompiler.IL
{
	partial class Return
	{
		public static readonly SlotInfo ReturnValueSlot = new SlotInfo("ReturnValue", canInlineInto: true);
		
		readonly bool hasArgument;
		ILInstruction returnValue;
		
		/// <summary>
		/// The value to return. Null if this return statement is within a void method.
		/// </summary>
		public ILInstruction ReturnValue {
			get { return returnValue; }
			set {
				if (hasArgument) {
					ValidateChild(value);
					SetChildInstruction(ref returnValue, value, 0);
				} else {
					Debug.Assert(value == null);
				}
			}
		}
		
		public Return() : base(OpCode.Return)
		{
		}
		
		public Return(ILInstruction argument) : base(OpCode.Return)
		{
			this.hasArgument = true;
			this.ReturnValue = argument;
		}
		
		public override ILInstruction Clone()
		{
			Return clone = (Return)ShallowClone();
			if (hasArgument)
				clone.ReturnValue = returnValue.Clone();
			return clone;
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			InstructionFlags flags = InstructionFlags.MayBranch | InstructionFlags.EndPointUnreachable;
			if (hasArgument) {
				flags |= returnValue.Flags;
			}
			return flags;
		}
		
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayBranch | InstructionFlags.EndPointUnreachable;
			}
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			if (returnValue != null) {
				output.Write('(');
				returnValue.WriteTo(output);
				output.Write(')');
			}
		}
		
		protected override int GetChildCount()
		{
			return hasArgument ? 1 : 0;
		}
		
		protected override ILInstruction GetChild(int index)
		{
			if (index == 0 && hasArgument)
				return returnValue;
			else
				throw new IndexOutOfRangeException();
		}
		
		protected override void SetChild(int index, ILInstruction value)
		{
			if (index == 0 && hasArgument)
				ReturnValue = value;
			else
				throw new IndexOutOfRangeException();
		}
		
		protected override SlotInfo GetChildSlot(int index)
		{
			return ReturnValueSlot;
		}
	}
}
