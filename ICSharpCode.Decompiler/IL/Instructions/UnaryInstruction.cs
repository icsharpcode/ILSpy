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
using System.Linq;

namespace ICSharpCode.Decompiler.IL
{
	public abstract class UnaryInstruction : ILInstruction
	{
		ILInstruction argument = Pop;
		
		public ILInstruction Argument {
			get { return argument; }
			set {
				Debug.Assert(value.ResultType != StackType.Void);
				argument = value;
				InvalidateFlags();
			}
		}
		
		protected UnaryInstruction(OpCode opCode) : base(opCode)
		{
		}

		internal override void CheckInvariant()
		{
			base.CheckInvariant();
			Argument.CheckInvariant();
			Debug.Assert(Argument.ResultType != StackType.Void);
		}
		
		//public sealed override bool IsPeeking { get { return Operand.IsPeeking; } }

		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write('(');
			Argument.WriteTo(output);
			output.Write(')');
		}
		
		public sealed override TAccumulate AggregateChildren<TSource, TAccumulate>(TAccumulate initial, ILVisitor<TSource> visitor, Func<TAccumulate, TSource, TAccumulate> func)
		{
			return func(initial, Argument.AcceptVisitor(visitor));
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			return Argument.Flags;
		}
		
		/*
		public override void TransformChildren(Func<ILInstruction, ILInstruction> transformFunc)
		{
			Operand = transformFunc(Operand);
		}

		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			Operand = Operand.Inline(flagsBefore, instructionStack, out finished);
			return this;
		}*/
	}

	/*
	class VoidInstruction() : UnaryInstruction(OpCode.Void)
	{
		public override bool NoResult { get { return true; } }

		public override InstructionFlags Flags
		{
			get { return Operand.Flags; }
		}
	}

	class LogicNotInstruction() : UnaryInstruction(OpCode.LogicNot)
	{
		public override InstructionFlags Flags
		{
			get { return Operand.Flags; }
		}
	}

	class UnaryNumericInstruction(OpCode opCode, StackType opType) : UnaryInstruction(opCode)
	{
		public readonly StackType OpType = opType;

		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			output.Write(OpType);
			output.Write('(');
			Operand.WriteTo(output);
			output.Write(')');
		}

		public override InstructionFlags Flags
		{
			get { return Operand.Flags; }
		}
	}

	class IsInst(public readonly TypeReference Type) : UnaryInstruction(OpCode.IsInst)
	{
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Type.WriteTo(output);
			output.Write('(');
			Operand.WriteTo(output);
			output.Write(')');
		}

		public override InstructionFlags Flags
		{
			get { return Operand.Flags; }
		}
	}

	class ConvInstruction(
		public readonly StackType FromType, public readonly PrimitiveType ToType, public readonly OverflowMode ConvMode
	) : UnaryInstruction(OpCode.Conv)
	{
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.WriteSuffix(ConvMode);
			output.Write(' ');
			output.Write(FromType);
			output.Write("->");
			output.Write(ToType);
			output.Write('(');
			Operand.WriteTo(output);
			output.Write(')');
		}

		public override InstructionFlags Flags
		{
			get { return Operand.Flags | InstructionFlags.MayThrow; }
		}
	}*/
}
 