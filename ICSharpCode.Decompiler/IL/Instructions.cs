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
using Mono.Cecil;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Enum representing the type of an <see cref="ILInstruction"/>.
	/// </summary>
	public enum OpCode
	{
		/// <summary>No operation. Takes 0 arguments and returns void.</summary>
		Nop,
		/// <summary>Pops the top of the evaluation stack and returns the value.</summary>
		Pop,
		/// <summary>Peeks at the top of the evaluation stack and returns the value. Corresponds to IL 'dup'.</summary>
		Peek,
		/// <summary>Ignore the arguments and produce void. Used to prevent the end result of an instruction from being pushed to the evaluation stack.</summary>
		Void,
		/// <summary>A container of IL blocks.</summary>
		BlockContainer,
		/// <summary>A block of IL instructions.</summary>
		Block,
		/// <summary>Unary operator that expects an input of type I4. Returns 1 (of type I4) if the input value is 0. Otherwise, returns 0 (of type I4).</summary>
		LogicNot,
		/// <summary>Adds two numbers.</summary>
		Add,
		/// <summary>Subtracts two numbers</summary>
		Sub,
		/// <summary>Multiplies two numbers</summary>
		Mul,
		/// <summary>Divides two numbers</summary>
		Div,
		/// <summary>Division remainder</summary>
		Rem,
		/// <summary>Bitwise AND</summary>
		BitAnd,
		/// <summary>Bitwise OR</summary>
		BitOr,
		/// <summary>Bitwise XOR</summary>
		BitXor,
		/// <summary>Bitwise NOT</summary>
		BitNot,
		/// <summary>Retrieves the RuntimeArgumentHandle.</summary>
		Arglist,
		/// <summary>Unconditional branch. <c>goto target;</c></summary>
		Branch,
		/// <summary>If statement / conditional expression. <c>if (condition) trueExpr else falseExpr</c></summary>
		IfInstruction,
		/// <summary>Breakpoint instruction</summary>
		DebugBreak,
		/// <summary>Compare equal. Returns 1 (of type I4) if two numbers or object references are equal; 0 otherwise.</summary>
		Ceq,
		/// <summary>Compare greater than. For integers, perform a signed comparison. For floating-point numbers, return 0 for unordered numbers.</summary>
		Cgt,
		/// <summary>Compare greater than (unordered/unsigned). For integers, perform a signed comparison. For floating-point numbers, return 1 for unordered numbers.</summary>
		Cgt_Un,
		/// <summary>Compare less than. For integers, perform a signed comparison. For floating-point numbers, return 0 for unordered numbers.</summary>
		Clt,
		/// <summary>Compare less than (unordered/unsigned). For integers, perform a signed comparison. For floating-point numbers, return 1 for unordered numbers.</summary>
		Clt_Un,
		/// <summary>Non-virtual method call.</summary>
		Call,
		/// <summary>Virtual method call.</summary>
		CallVirt,
		/// <summary>Checks that the float on top of the stack is not NaN or infinite.</summary>
		Ckfinite,
		/// <summary>Numeric cast.</summary>
		Conv,
		/// <summary>Loads the value of a local variable. (ldarg/ldloc)</summary>
		LdLoc,
		/// <summary>Loads the address of a local variable. (ldarga/ldloca)</summary>
		LdLoca,
		/// <summary>Stores a value into a local variable. (starg/stloc)</summary>
		StLoc,
		/// <summary>Loads a constant string.</summary>
		LdStr,
		/// <summary>Loads a constant 32-bit integer.</summary>
		LdcI4,
		/// <summary>Loads a constant 64-bit integer.</summary>
		LdcI8,
		/// <summary>Loads a constant floating-point number.</summary>
		LdcF,
		/// <summary>Loads the null reference.</summary>
		LdNull,
		/// <summary>Returns from the current method or lambda.</summary>
		Return,
		/// <summary>Shift left</summary>
		Shl,
		/// <summary>Shift right</summary>
		Shr,
		/// <summary>Load instance field</summary>
		LdFld,
		/// <summary>Load address of instance field</summary>
		LdFlda,
		/// <summary>Store value to instance field</summary>
		StFld,
		/// <summary>Load static field</summary>
		LdsFld,
		/// <summary>Load static field address</summary>
		LdsFlda,
		/// <summary>Store value to static field</summary>
		StsFld,
		/// <summary>Test if object is instance of class or interface.</summary>
		IsInst,
		/// <summary>Indirect load (ref/pointer dereference).</summary>
		LdObj,
		/// <summary>Unbox a value.</summary>
		UnboxAny,
		/// <summary>Creates an object instance and calls the constructor.</summary>
		NewObj,
		/// <summary>Throws an exception.</summary>
		Throw,
		/// <summary>Returns the length of an array as 'native unsigned int'.</summary>
		LdLen,
	}

	/// <summary>No operation. Takes 0 arguments and returns void.</summary>
	public sealed partial class Nop : SimpleInstruction
	{
		public Nop() : base(OpCode.Nop)
		{
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitNop(this);
		}
	}

	/// <summary>Pops the top of the evaluation stack and returns the value.</summary>
	public sealed partial class Pop : SimpleInstruction
	{
		public Pop(StackType resultType) : base(OpCode.Pop)
		{
			this.resultType = resultType;
		}
		StackType resultType;
		public override StackType ResultType { get { return resultType; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitPop(this);
		}
	}

	/// <summary>Peeks at the top of the evaluation stack and returns the value. Corresponds to IL 'dup'.</summary>
	public sealed partial class Peek : SimpleInstruction
	{
		public Peek(StackType resultType) : base(OpCode.Peek)
		{
			this.resultType = resultType;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.MayPeek;
		}
		StackType resultType;
		public override StackType ResultType { get { return resultType; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitPeek(this);
		}
	}

	/// <summary>Ignore the arguments and produce void. Used to prevent the end result of an instruction from being pushed to the evaluation stack.</summary>
	public sealed partial class Void : UnaryInstruction
	{
		public Void(ILInstruction argument) : base(OpCode.Void, argument)
		{
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitVoid(this);
		}
	}

	/// <summary>A container of IL blocks.</summary>
	public sealed partial class BlockContainer : ILInstruction
	{
		public override StackType ResultType { get { return StackType.Void; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBlockContainer(this);
		}
	}

	/// <summary>A block of IL instructions.</summary>
	public sealed partial class Block : ILInstruction
	{

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBlock(this);
		}
	}

	/// <summary>Unary operator that expects an input of type I4. Returns 1 (of type I4) if the input value is 0. Otherwise, returns 0 (of type I4).</summary>
	public sealed partial class LogicNot : UnaryInstruction
	{
		public LogicNot(ILInstruction argument) : base(OpCode.LogicNot, argument)
		{
		}
		public override StackType ResultType { get { return StackType.I4; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLogicNot(this);
		}
	}

	/// <summary>Adds two numbers.</summary>
	public sealed partial class Add : BinaryNumericInstruction
	{
		public Add(ILInstruction left, ILInstruction right, OverflowMode overflowMode) : base(OpCode.Add, left, right, overflowMode)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitAdd(this);
		}
	}

	/// <summary>Subtracts two numbers</summary>
	public sealed partial class Sub : BinaryNumericInstruction
	{
		public Sub(ILInstruction left, ILInstruction right, OverflowMode overflowMode) : base(OpCode.Sub, left, right, overflowMode)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitSub(this);
		}
	}

	/// <summary>Multiplies two numbers</summary>
	public sealed partial class Mul : BinaryNumericInstruction
	{
		public Mul(ILInstruction left, ILInstruction right, OverflowMode overflowMode) : base(OpCode.Mul, left, right, overflowMode)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitMul(this);
		}
	}

	/// <summary>Divides two numbers</summary>
	public sealed partial class Div : BinaryNumericInstruction
	{
		public Div(ILInstruction left, ILInstruction right, OverflowMode overflowMode) : base(OpCode.Div, left, right, overflowMode)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow;
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDiv(this);
		}
	}

	/// <summary>Division remainder</summary>
	public sealed partial class Rem : BinaryNumericInstruction
	{
		public Rem(ILInstruction left, ILInstruction right, OverflowMode overflowMode) : base(OpCode.Rem, left, right, overflowMode)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow;
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitRem(this);
		}
	}

	/// <summary>Bitwise AND</summary>
	public sealed partial class BitAnd : BinaryNumericInstruction
	{
		public BitAnd(ILInstruction left, ILInstruction right, OverflowMode overflowMode) : base(OpCode.BitAnd, left, right, overflowMode)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBitAnd(this);
		}
	}

	/// <summary>Bitwise OR</summary>
	public sealed partial class BitOr : BinaryNumericInstruction
	{
		public BitOr(ILInstruction left, ILInstruction right, OverflowMode overflowMode) : base(OpCode.BitOr, left, right, overflowMode)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBitOr(this);
		}
	}

	/// <summary>Bitwise XOR</summary>
	public sealed partial class BitXor : BinaryNumericInstruction
	{
		public BitXor(ILInstruction left, ILInstruction right, OverflowMode overflowMode) : base(OpCode.BitXor, left, right, overflowMode)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBitXor(this);
		}
	}

	/// <summary>Bitwise NOT</summary>
	public sealed partial class BitNot : UnaryInstruction
	{
		public BitNot(ILInstruction argument) : base(OpCode.BitNot, argument)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBitNot(this);
		}
	}

	/// <summary>Retrieves the RuntimeArgumentHandle.</summary>
	public sealed partial class Arglist : SimpleInstruction
	{
		public Arglist() : base(OpCode.Arglist)
		{
		}
		public override StackType ResultType { get { return StackType.O; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitArglist(this);
		}
	}

	/// <summary>Unconditional branch. <c>goto target;</c></summary>
	public sealed partial class Branch : SimpleInstruction
	{
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.EndPointUnreachable | InstructionFlags.MayBranch;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBranch(this);
		}
	}

	/// <summary>If statement / conditional expression. <c>if (condition) trueExpr else falseExpr</c></summary>
	public sealed partial class IfInstruction : ILInstruction
	{

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitIfInstruction(this);
		}
	}

	/// <summary>Breakpoint instruction</summary>
	public sealed partial class DebugBreak : SimpleInstruction
	{
		public DebugBreak() : base(OpCode.DebugBreak)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.SideEffect;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDebugBreak(this);
		}
	}

	/// <summary>Compare equal. Returns 1 (of type I4) if two numbers or object references are equal; 0 otherwise.</summary>
	public sealed partial class Ceq : BinaryComparisonInstruction
	{
		public Ceq(ILInstruction left, ILInstruction right) : base(OpCode.Ceq, left, right)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitCeq(this);
		}
	}

	/// <summary>Compare greater than. For integers, perform a signed comparison. For floating-point numbers, return 0 for unordered numbers.</summary>
	public sealed partial class Cgt : BinaryComparisonInstruction
	{
		public Cgt(ILInstruction left, ILInstruction right) : base(OpCode.Cgt, left, right)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitCgt(this);
		}
	}

	/// <summary>Compare greater than (unordered/unsigned). For integers, perform a signed comparison. For floating-point numbers, return 1 for unordered numbers.</summary>
	public sealed partial class Cgt_Un : BinaryComparisonInstruction
	{
		public Cgt_Un(ILInstruction left, ILInstruction right) : base(OpCode.Cgt_Un, left, right)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitCgt_Un(this);
		}
	}

	/// <summary>Compare less than. For integers, perform a signed comparison. For floating-point numbers, return 0 for unordered numbers.</summary>
	public sealed partial class Clt : BinaryComparisonInstruction
	{
		public Clt(ILInstruction left, ILInstruction right) : base(OpCode.Clt, left, right)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitClt(this);
		}
	}

	/// <summary>Compare less than (unordered/unsigned). For integers, perform a signed comparison. For floating-point numbers, return 1 for unordered numbers.</summary>
	public sealed partial class Clt_Un : BinaryComparisonInstruction
	{
		public Clt_Un(ILInstruction left, ILInstruction right) : base(OpCode.Clt_Un, left, right)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitClt_Un(this);
		}
	}

	/// <summary>Non-virtual method call.</summary>
	public sealed partial class Call : CallInstruction
	{
		public Call(MethodReference method) : base(OpCode.Call, method)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitCall(this);
		}
	}

	/// <summary>Virtual method call.</summary>
	public sealed partial class CallVirt : CallInstruction
	{
		public CallVirt(MethodReference method) : base(OpCode.CallVirt, method)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitCallVirt(this);
		}
	}

	/// <summary>Checks that the float on top of the stack is not NaN or infinite.</summary>
	public sealed partial class Ckfinite : SimpleInstruction
	{
		public Ckfinite() : base(OpCode.Ckfinite)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.MayPeek | InstructionFlags.MayThrow;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitCkfinite(this);
		}
	}

	/// <summary>Numeric cast.</summary>
	public sealed partial class Conv : UnaryInstruction
	{

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitConv(this);
		}
	}

	/// <summary>Loads the value of a local variable. (ldarg/ldloc)</summary>
	public sealed partial class LdLoc : SimpleInstruction
	{
		public LdLoc(ILVariable variable) : base(OpCode.LdLoc)
		{
			this.variable = variable;
		}
		readonly ILVariable variable;
		/// <summary>Returns the variable operand.</summary>
		public ILVariable Variable { get { return variable; } }
		public override StackType ResultType { get { return variable.Type.GetStackType(); } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdLoc(this);
		}
	}

	/// <summary>Loads the address of a local variable. (ldarga/ldloca)</summary>
	public sealed partial class LdLoca : SimpleInstruction
	{
		public LdLoca(ILVariable variable) : base(OpCode.LdLoca)
		{
			this.variable = variable;
		}
		public override StackType ResultType { get { return StackType.Ref; } }
		readonly ILVariable variable;
		/// <summary>Returns the variable operand.</summary>
		public ILVariable Variable { get { return variable; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdLoca(this);
		}
	}

	/// <summary>Stores a value into a local variable. (starg/stloc)</summary>
	public sealed partial class StLoc : UnaryInstruction
	{
		public StLoc(ILInstruction argument, ILVariable variable) : base(OpCode.StLoc, argument)
		{
			this.variable = variable;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		readonly ILVariable variable;
		/// <summary>Returns the variable operand.</summary>
		public ILVariable Variable { get { return variable; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitStLoc(this);
		}
	}

	/// <summary>Loads a constant string.</summary>
	public sealed partial class LdStr : SimpleInstruction
	{
		public LdStr(string value) : base(OpCode.LdStr)
		{
			this.Value = value;
		}
		public readonly string Value;
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
		public override StackType ResultType { get { return StackType.O; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdStr(this);
		}
	}

	/// <summary>Loads a constant 32-bit integer.</summary>
	public sealed partial class LdcI4 : SimpleInstruction
	{
		public LdcI4(int value) : base(OpCode.LdcI4)
		{
			this.Value = value;
		}
		public readonly int Value;
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
		public override StackType ResultType { get { return StackType.I4; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdcI4(this);
		}
	}

	/// <summary>Loads a constant 64-bit integer.</summary>
	public sealed partial class LdcI8 : SimpleInstruction
	{
		public LdcI8(long value) : base(OpCode.LdcI8)
		{
			this.Value = value;
		}
		public readonly long Value;
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
		public override StackType ResultType { get { return StackType.I8; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdcI8(this);
		}
	}

	/// <summary>Loads a constant floating-point number.</summary>
	public sealed partial class LdcF : SimpleInstruction
	{
		public LdcF(double value) : base(OpCode.LdcF)
		{
			this.Value = value;
		}
		public readonly double Value;
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
		public override StackType ResultType { get { return StackType.F; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdcF(this);
		}
	}

	/// <summary>Loads the null reference.</summary>
	public sealed partial class LdNull : SimpleInstruction
	{
		public LdNull() : base(OpCode.LdNull)
		{
		}
		public override StackType ResultType { get { return StackType.O; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdNull(this);
		}
	}

	/// <summary>Returns from the current method or lambda.</summary>
	public sealed partial class Return : ILInstruction
	{
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.MayBranch | InstructionFlags.EndPointUnreachable;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitReturn(this);
		}
	}

	/// <summary>Shift left</summary>
	public sealed partial class Shl : BinaryNumericInstruction
	{
		public Shl(ILInstruction left, ILInstruction right, OverflowMode overflowMode) : base(OpCode.Shl, left, right, overflowMode)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitShl(this);
		}
	}

	/// <summary>Shift right</summary>
	public sealed partial class Shr : BinaryNumericInstruction
	{
		public Shr(ILInstruction left, ILInstruction right, OverflowMode overflowMode) : base(OpCode.Shr, left, right, overflowMode)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitShr(this);
		}
	}

	/// <summary>Load instance field</summary>
	public sealed partial class LdFld : UnaryInstruction, ISupportsVolatilePrefix, ISupportsUnalignedPrefix
	{
		public LdFld(ILInstruction argument, FieldReference field) : base(OpCode.LdFld, argument)
		{
			this.field = field;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
		}
		/// <summary>Gets/Sets whether the memory access is volatile.</summary>
		public bool IsVolatile { get; set; }
		/// <summary>Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.</summary>
		public byte UnalignedPrefix { get; set; }
		readonly FieldReference field;
		/// <summary>Returns the field operand.</summary>
		public FieldReference Field { get { return field; } }
		public override StackType ResultType { get { return field.FieldType.GetStackType(); } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdFld(this);
		}
	}

	/// <summary>Load address of instance field</summary>
	public sealed partial class LdFlda : UnaryInstruction
	{
		public LdFlda(ILInstruction argument, FieldReference field) : base(OpCode.LdFlda, argument)
		{
			this.field = field;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow;
		}
		readonly FieldReference field;
		/// <summary>Returns the field operand.</summary>
		public FieldReference Field { get { return field; } }
		public override StackType ResultType { get { return StackType.Ref; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdFlda(this);
		}
	}

	/// <summary>Store value to instance field</summary>
	public sealed partial class StFld : BinaryInstruction, ISupportsVolatilePrefix, ISupportsUnalignedPrefix
	{
		public StFld(ILInstruction left, ILInstruction right, FieldReference field) : base(OpCode.StFld, left, right)
		{
			this.field = field;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
		}
		/// <summary>Gets/Sets whether the memory access is volatile.</summary>
		public bool IsVolatile { get; set; }
		/// <summary>Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.</summary>
		public byte UnalignedPrefix { get; set; }
		public override StackType ResultType { get { return StackType.Void; } }
		readonly FieldReference field;
		/// <summary>Returns the field operand.</summary>
		public FieldReference Field { get { return field; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitStFld(this);
		}
	}

	/// <summary>Load static field</summary>
	public sealed partial class LdsFld : SimpleInstruction, ISupportsVolatilePrefix, ISupportsUnalignedPrefix
	{
		public LdsFld(FieldReference field) : base(OpCode.LdsFld)
		{
			this.field = field;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.SideEffect;
		}
		/// <summary>Gets/Sets whether the memory access is volatile.</summary>
		public bool IsVolatile { get; set; }
		/// <summary>Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.</summary>
		public byte UnalignedPrefix { get; set; }
		readonly FieldReference field;
		/// <summary>Returns the field operand.</summary>
		public FieldReference Field { get { return field; } }
		public override StackType ResultType { get { return field.FieldType.GetStackType(); } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdsFld(this);
		}
	}

	/// <summary>Load static field address</summary>
	public sealed partial class LdsFlda : SimpleInstruction
	{
		public LdsFlda(FieldReference field) : base(OpCode.LdsFlda)
		{
			this.field = field;
		}
		public override StackType ResultType { get { return StackType.Ref; } }
		readonly FieldReference field;
		/// <summary>Returns the field operand.</summary>
		public FieldReference Field { get { return field; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdsFlda(this);
		}
	}

	/// <summary>Store value to static field</summary>
	public sealed partial class StsFld : UnaryInstruction, ISupportsVolatilePrefix, ISupportsUnalignedPrefix
	{
		public StsFld(ILInstruction argument, FieldReference field) : base(OpCode.StsFld, argument)
		{
			this.field = field;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.SideEffect;
		}
		/// <summary>Gets/Sets whether the memory access is volatile.</summary>
		public bool IsVolatile { get; set; }
		/// <summary>Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.</summary>
		public byte UnalignedPrefix { get; set; }
		public override StackType ResultType { get { return StackType.Void; } }
		readonly FieldReference field;
		/// <summary>Returns the field operand.</summary>
		public FieldReference Field { get { return field; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitStsFld(this);
		}
	}

	/// <summary>Test if object is instance of class or interface.</summary>
	public sealed partial class IsInst : UnaryInstruction
	{
		public IsInst(ILInstruction argument, TypeReference type) : base(OpCode.IsInst, argument)
		{
			this.type = type;
		}
		readonly TypeReference type;
		/// <summary>Returns the type operand.</summary>
		public TypeReference Type { get { return type; } }
		public override StackType ResultType { get { return type.GetStackType(); } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitIsInst(this);
		}
	}

	/// <summary>Indirect load (ref/pointer dereference).</summary>
	public sealed partial class LdObj : UnaryInstruction, ISupportsVolatilePrefix, ISupportsUnalignedPrefix
	{
		public LdObj(ILInstruction argument, TypeReference type) : base(OpCode.LdObj, argument)
		{
			this.type = type;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
		}
		readonly TypeReference type;
		/// <summary>Returns the type operand.</summary>
		public TypeReference Type { get { return type; } }
		/// <summary>Gets/Sets whether the memory access is volatile.</summary>
		public bool IsVolatile { get; set; }
		/// <summary>Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.</summary>
		public byte UnalignedPrefix { get; set; }
		public override StackType ResultType { get { return type.GetStackType(); } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdObj(this);
		}
	}

	/// <summary>Unbox a value.</summary>
	public sealed partial class UnboxAny : UnaryInstruction
	{
		public UnboxAny(ILInstruction argument, TypeReference type) : base(OpCode.UnboxAny, argument)
		{
			this.type = type;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
		}
		readonly TypeReference type;
		/// <summary>Returns the type operand.</summary>
		public TypeReference Type { get { return type; } }
		public override StackType ResultType { get { return type.GetStackType(); } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitUnboxAny(this);
		}
	}

	/// <summary>Creates an object instance and calls the constructor.</summary>
	public sealed partial class NewObj : CallInstruction
	{
		public NewObj(MethodReference method) : base(OpCode.NewObj, method)
		{
		}
		public override StackType ResultType { get { return StackType.O; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitNewObj(this);
		}
	}

	/// <summary>Throws an exception.</summary>
	public sealed partial class Throw : UnaryInstruction
	{
		public Throw(ILInstruction argument) : base(OpCode.Throw, argument)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.EndPointUnreachable;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitThrow(this);
		}
	}

	/// <summary>Returns the length of an array as 'native unsigned int'.</summary>
	public sealed partial class LdLen : UnaryInstruction
	{
		public LdLen(ILInstruction argument) : base(OpCode.LdLen, argument)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow;
		}
		public override StackType ResultType { get { return StackType.I; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdLen(this);
		}
	}


	/// <summary>
	/// Base class for visitor pattern.
	/// </summary>
	public abstract class ILVisitor<T>
	{
		/// <summary>Called by Visit*() methods that were not overridden</summary>
		protected abstract T Default(ILInstruction inst);
		
		protected internal virtual T VisitNop(Nop inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitPop(Pop inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitPeek(Peek inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitVoid(Void inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitBlockContainer(BlockContainer inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitBlock(Block inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLogicNot(LogicNot inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitAdd(Add inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitSub(Sub inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitMul(Mul inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitDiv(Div inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitRem(Rem inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitBitAnd(BitAnd inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitBitOr(BitOr inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitBitXor(BitXor inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitBitNot(BitNot inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitArglist(Arglist inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitBranch(Branch inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitIfInstruction(IfInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitDebugBreak(DebugBreak inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitCeq(Ceq inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitCgt(Cgt inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitCgt_Un(Cgt_Un inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitClt(Clt inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitClt_Un(Clt_Un inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitCall(Call inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitCallVirt(CallVirt inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitCkfinite(Ckfinite inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitConv(Conv inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdLoc(LdLoc inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdLoca(LdLoca inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitStLoc(StLoc inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdStr(LdStr inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdcI4(LdcI4 inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdcI8(LdcI8 inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdcF(LdcF inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdNull(LdNull inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitReturn(Return inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitShl(Shl inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitShr(Shr inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdFld(LdFld inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdFlda(LdFlda inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitStFld(StFld inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdsFld(LdsFld inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdsFlda(LdsFlda inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitStsFld(StsFld inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitIsInst(IsInst inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdObj(LdObj inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitUnboxAny(UnboxAny inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitNewObj(NewObj inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitThrow(Throw inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdLen(LdLen inst)
		{
			return Default(inst);
		}
	}
	
	partial class BinaryNumericInstruction
	{
		public static BinaryNumericInstruction Create(OpCode opCode, ILInstruction left, ILInstruction right, OverflowMode overflowMode)
		{
			switch (opCode) {
				case OpCode.Add:
					return new Add(left, right, overflowMode);
				case OpCode.Sub:
					return new Sub(left, right, overflowMode);
				case OpCode.Mul:
					return new Mul(left, right, overflowMode);
				case OpCode.Div:
					return new Div(left, right, overflowMode);
				case OpCode.Rem:
					return new Rem(left, right, overflowMode);
				case OpCode.BitAnd:
					return new BitAnd(left, right, overflowMode);
				case OpCode.BitOr:
					return new BitOr(left, right, overflowMode);
				case OpCode.BitXor:
					return new BitXor(left, right, overflowMode);
				case OpCode.Shl:
					return new Shl(left, right, overflowMode);
				case OpCode.Shr:
					return new Shr(left, right, overflowMode);
				default:
					throw new ArgumentException("opCode is not a binary numeric instruction");
			}
		}
	}
	
	partial class BinaryComparisonInstruction
	{
		public static BinaryComparisonInstruction Create(OpCode opCode, ILInstruction left, ILInstruction right)
		{
			switch (opCode) {
				case OpCode.Ceq:
					return new Ceq(left, right);
				case OpCode.Cgt:
					return new Cgt(left, right);
				case OpCode.Cgt_Un:
					return new Cgt_Un(left, right);
				case OpCode.Clt:
					return new Clt(left, right);
				case OpCode.Clt_Un:
					return new Clt_Un(left, right);
				default:
					throw new ArgumentException("opCode is not a binary comparison instruction");
			}
		}
	}
	
	partial class InstructionOutputExtensions
	{
		static readonly string[] originalOpCodeNames = {
			"nop",
			"pop",
			"peek",
			"void",
			"BlockContainer",
			"Block",
			"logic.not",
			"add",
			"sub",
			"mul",
			"div",
			"rem",
			"bit.and",
			"bit.or",
			"bit.xor",
			"bit.not",
			"arglist",
			"br",
			"if",
			"debug.break",
			"ceq",
			"cgt",
			"cgt.un",
			"clt",
			"clt.un",
			"call",
			"callvirt",
			"ckfinite",
			"conv",
			"ldloc",
			"ldloca",
			"stloc",
			"ldstr",
			"ldc.i4",
			"ldc.i8",
			"ldc.f",
			"ldnull",
			"ret",
			"shl",
			"shr",
			"ldfld",
			"ldflda",
			"stfld",
			"ldsfld",
			"ldsflda",
			"stsfld",
			"isinst",
			"ldobj",
			"unbox.any",
			"newobj",
			"throw",
			"ldlen",
		};
	}
}

