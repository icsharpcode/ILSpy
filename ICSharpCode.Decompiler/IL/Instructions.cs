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
		/// <summary>Unary negation</summary>
		Neg,
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
		/// <summary><c>if (condition) goto target;</c>.</summary>
		ConditionalBranch,
		/// <summary><c>goto target;</c>.</summary>
		Branch,
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
		CkFinite,
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
		Ldfld,
		/// <summary>Load address of instance field</summary>
		Ldflda,
		/// <summary>Store value to instance field</summary>
		Stfld,
		/// <summary>Load static field</summary>
		Ldsfld,
		/// <summary>Load static field address</summary>
		Ldsflda,
		/// <summary>Store value to static field</summary>
		Stsfld,
		/// <summary>Test if object is instance of class or interface.</summary>
		IsInst,
		/// <summary>Indirect load (ref/pointer dereference).</summary>
		LdInd,
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
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
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
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
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
			return base.ComputeFlags() | InstructionFlags.MayPeek;
		}
		StackType resultType;
		public override StackType ResultType { get { return resultType; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitPeek(this);
		}
	}

	/// <summary>Ignore the arguments and produce void. Used to prevent the end result of an instruction from being pushed to the evaluation stack.</summary>
	public sealed partial class Void : UnaryInstruction
	{
		public Void() : base(OpCode.Void)
		{
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitVoid(this);
		}
	}

	/// <summary>A container of IL blocks.</summary>
	public sealed partial class BlockContainer : ILInstruction
	{
		public BlockContainer() : base(OpCode.BlockContainer)
		{
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBlockContainer(this);
		}
	}

	/// <summary>A block of IL instructions.</summary>
	public sealed partial class Block : ILInstruction
	{
		public Block() : base(OpCode.Block)
		{
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBlock(this);
		}
	}

	/// <summary>Unary operator that expects an input of type I4. Returns 1 (of type I4) if the input value is 0. Otherwise, returns 0 (of type I4).</summary>
	public sealed partial class LogicNot : UnaryInstruction
	{
		public LogicNot() : base(OpCode.LogicNot)
		{
		}
		public override StackType ResultType { get { return StackType.I4; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLogicNot(this);
		}
	}

	/// <summary>Adds two numbers.</summary>
	public sealed partial class Add : BinaryNumericInstruction
	{
		public Add(StackType opType, StackType resultType, OverflowMode overflowMode) : base(OpCode.Add, opType, resultType, overflowMode)
		{
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitAdd(this);
		}
	}

	/// <summary>Subtracts two numbers</summary>
	public sealed partial class Sub : BinaryNumericInstruction
	{
		public Sub(StackType opType, StackType resultType, OverflowMode overflowMode) : base(OpCode.Sub, opType, resultType, overflowMode)
		{
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitSub(this);
		}
	}

	/// <summary>Multiplies two numbers</summary>
	public sealed partial class Mul : BinaryNumericInstruction
	{
		public Mul(StackType opType, StackType resultType, OverflowMode overflowMode) : base(OpCode.Mul, opType, resultType, overflowMode)
		{
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitMul(this);
		}
	}

	/// <summary>Divides two numbers</summary>
	public sealed partial class Div : BinaryNumericInstruction
	{
		public Div(StackType opType, StackType resultType, OverflowMode overflowMode) : base(OpCode.Div, opType, resultType, overflowMode)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow;
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDiv(this);
		}
	}

	/// <summary>Division remainder</summary>
	public sealed partial class Rem : BinaryNumericInstruction
	{
		public Rem(StackType opType, StackType resultType, OverflowMode overflowMode) : base(OpCode.Rem, opType, resultType, overflowMode)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow;
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitRem(this);
		}
	}

	/// <summary>Unary negation</summary>
	public sealed partial class Neg : UnaryInstruction
	{
		public Neg(StackType resultType) : base(OpCode.Neg)
		{
			this.resultType = resultType;
		}
		StackType resultType;
		public override StackType ResultType { get { return resultType; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitNeg(this);
		}
	}

	/// <summary>Bitwise AND</summary>
	public sealed partial class BitAnd : BinaryNumericInstruction
	{
		public BitAnd(StackType opType, StackType resultType, OverflowMode overflowMode) : base(OpCode.BitAnd, opType, resultType, overflowMode)
		{
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBitAnd(this);
		}
	}

	/// <summary>Bitwise OR</summary>
	public sealed partial class BitOr : BinaryNumericInstruction
	{
		public BitOr(StackType opType, StackType resultType, OverflowMode overflowMode) : base(OpCode.BitOr, opType, resultType, overflowMode)
		{
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBitOr(this);
		}
	}

	/// <summary>Bitwise XOR</summary>
	public sealed partial class BitXor : BinaryNumericInstruction
	{
		public BitXor(StackType opType, StackType resultType, OverflowMode overflowMode) : base(OpCode.BitXor, opType, resultType, overflowMode)
		{
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBitXor(this);
		}
	}

	/// <summary>Bitwise NOT</summary>
	public sealed partial class BitNot : UnaryInstruction
	{
		public BitNot(StackType resultType) : base(OpCode.BitNot)
		{
			this.resultType = resultType;
		}
		StackType resultType;
		public override StackType ResultType { get { return resultType; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
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
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitArglist(this);
		}
	}

	/// <summary><c>if (condition) goto target;</c>.</summary>
	public sealed partial class ConditionalBranch : UnaryInstruction
	{
		public ConditionalBranch() : base(OpCode.ConditionalBranch)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayBranch;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitConditionalBranch(this);
		}
	}

	/// <summary><c>goto target;</c>.</summary>
	public sealed partial class Branch : SimpleInstruction
	{
		public Branch() : base(OpCode.Branch)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayBranch;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBranch(this);
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
			return base.ComputeFlags() | InstructionFlags.SideEffect;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDebugBreak(this);
		}
	}

	/// <summary>Compare equal. Returns 1 (of type I4) if two numbers or object references are equal; 0 otherwise.</summary>
	public sealed partial class Ceq : BinaryComparisonInstruction
	{
		public Ceq(StackType opType) : base(OpCode.Ceq, opType)
		{
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitCeq(this);
		}
	}

	/// <summary>Compare greater than. For integers, perform a signed comparison. For floating-point numbers, return 0 for unordered numbers.</summary>
	public sealed partial class Cgt : BinaryComparisonInstruction
	{
		public Cgt(StackType opType) : base(OpCode.Cgt, opType)
		{
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitCgt(this);
		}
	}

	/// <summary>Compare greater than (unordered/unsigned). For integers, perform a signed comparison. For floating-point numbers, return 1 for unordered numbers.</summary>
	public sealed partial class Cgt_Un : BinaryComparisonInstruction
	{
		public Cgt_Un(StackType opType) : base(OpCode.Cgt_Un, opType)
		{
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitCgt_Un(this);
		}
	}

	/// <summary>Compare less than. For integers, perform a signed comparison. For floating-point numbers, return 0 for unordered numbers.</summary>
	public sealed partial class Clt : BinaryComparisonInstruction
	{
		public Clt(StackType opType) : base(OpCode.Clt, opType)
		{
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitClt(this);
		}
	}

	/// <summary>Compare less than (unordered/unsigned). For integers, perform a signed comparison. For floating-point numbers, return 1 for unordered numbers.</summary>
	public sealed partial class Clt_Un : BinaryComparisonInstruction
	{
		public Clt_Un(StackType opType) : base(OpCode.Clt_Un, opType)
		{
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
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

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
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

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitCallVirt(this);
		}
	}

	/// <summary>Checks that the float on top of the stack is not NaN or infinite.</summary>
	public sealed partial class CkFinite : SimpleInstruction
	{
		public CkFinite() : base(OpCode.CkFinite)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayPeek | InstructionFlags.MayThrow;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitCkFinite(this);
		}
	}

	/// <summary>Numeric cast.</summary>
	public sealed partial class Conv : UnaryInstruction
	{

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitConv(this);
		}
	}

	/// <summary>Loads the value of a local variable. (ldarg/ldloc)</summary>
	public sealed partial class LdLoc : SimpleInstruction
	{
		public LdLoc() : base(OpCode.LdLoc)
		{
		}
		public override StackType ResultType { get { return Variable.Type.ToStackType(); } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdLoc(this);
		}
	}

	/// <summary>Loads the address of a local variable. (ldarga/ldloca)</summary>
	public sealed partial class LdLoca : SimpleInstruction
	{
		public LdLoca() : base(OpCode.LdLoca)
		{
		}
		public override StackType ResultType { get { return StackType.Ref; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdLoca(this);
		}
	}

	/// <summary>Stores a value into a local variable. (starg/stloc)</summary>
	public sealed partial class StLoc : UnaryInstruction
	{
		public StLoc() : base(OpCode.StLoc)
		{
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitStLoc(this);
		}
	}

	/// <summary>Loads a constant string.</summary>
	public sealed partial class LdStr : SimpleInstruction
	{
		public LdStr() : base(OpCode.LdStr)
		{
		}
		public override StackType ResultType { get { return StackType.O; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdStr(this);
		}
	}

	/// <summary>Loads a constant 32-bit integer.</summary>
	public sealed partial class LdcI4 : SimpleInstruction
	{
		public LdcI4() : base(OpCode.LdcI4)
		{
		}
		public override StackType ResultType { get { return StackType.I4; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdcI4(this);
		}
	}

	/// <summary>Loads a constant 64-bit integer.</summary>
	public sealed partial class LdcI8 : SimpleInstruction
	{
		public LdcI8() : base(OpCode.LdcI8)
		{
		}
		public override StackType ResultType { get { return StackType.I8; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdcI8(this);
		}
	}

	/// <summary>Loads a constant floating-point number.</summary>
	public sealed partial class LdcF : SimpleInstruction
	{
		public LdcF() : base(OpCode.LdcF)
		{
		}
		public override StackType ResultType { get { return StackType.F; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
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
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdNull(this);
		}
	}

	/// <summary>Returns from the current method or lambda.</summary>
	public sealed partial class Return : ILInstruction
	{
		public Return() : base(OpCode.Return)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayBranch;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitReturn(this);
		}
	}

	/// <summary>Shift left</summary>
	public sealed partial class Shl : BinaryNumericInstruction
	{
		public Shl(StackType opType, StackType resultType, OverflowMode overflowMode) : base(OpCode.Shl, opType, resultType, overflowMode)
		{
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitShl(this);
		}
	}

	/// <summary>Shift right</summary>
	public sealed partial class Shr : BinaryNumericInstruction
	{
		public Shr(StackType opType, StackType resultType, OverflowMode overflowMode) : base(OpCode.Shr, opType, resultType, overflowMode)
		{
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitShr(this);
		}
	}

	/// <summary>Load instance field</summary>
	public sealed partial class Ldfld : UnaryInstruction
	{
		public Ldfld() : base(OpCode.Ldfld)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdfld(this);
		}
	}

	/// <summary>Load address of instance field</summary>
	public sealed partial class Ldflda : UnaryInstruction
	{
		public Ldflda() : base(OpCode.Ldflda)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow;
		}
		public override StackType ResultType { get { return StackType.Ref; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdflda(this);
		}
	}

	/// <summary>Store value to instance field</summary>
	public sealed partial class Stfld : BinaryInstruction
	{
		public Stfld() : base(OpCode.Stfld)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitStfld(this);
		}
	}

	/// <summary>Load static field</summary>
	public sealed partial class Ldsfld : SimpleInstruction
	{
		public Ldsfld() : base(OpCode.Ldsfld)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.SideEffect;
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdsfld(this);
		}
	}

	/// <summary>Load static field address</summary>
	public sealed partial class Ldsflda : SimpleInstruction
	{
		public Ldsflda() : base(OpCode.Ldsflda)
		{
		}
		public override StackType ResultType { get { return StackType.Ref; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdsflda(this);
		}
	}

	/// <summary>Store value to static field</summary>
	public sealed partial class Stsfld : UnaryInstruction
	{
		public Stsfld() : base(OpCode.Stsfld)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.SideEffect;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitStsfld(this);
		}
	}

	/// <summary>Test if object is instance of class or interface.</summary>
	public sealed partial class IsInst : UnaryInstruction
	{
		public IsInst() : base(OpCode.IsInst)
		{
		}
		public override StackType ResultType { get { return StackType.O; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitIsInst(this);
		}
	}

	/// <summary>Indirect load (ref/pointer dereference).</summary>
	public sealed partial class LdInd : UnaryInstruction
	{
		public LdInd() : base(OpCode.LdInd)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdInd(this);
		}
	}

	/// <summary>Unbox a value.</summary>
	public sealed partial class UnboxAny : UnaryInstruction
	{
		public UnboxAny() : base(OpCode.UnboxAny)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
		}

		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
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
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitNewObj(this);
		}
	}

	/// <summary>Throws an exception.</summary>
	public sealed partial class Throw : UnaryInstruction
	{
		public Throw() : base(OpCode.Throw)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitThrow(this);
		}
	}

	/// <summary>Returns the length of an array as 'native unsigned int'.</summary>
	public sealed partial class LdLen : UnaryInstruction
	{
		public LdLen() : base(OpCode.LdLen)
		{
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow;
		}
		public override StackType ResultType { get { return StackType.I; } }
		public sealed override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdLen(this);
		}
	}


	
	public abstract class ILVisitor<T>
	{
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
		protected internal virtual T VisitNeg(Neg inst)
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
		protected internal virtual T VisitConditionalBranch(ConditionalBranch inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitBranch(Branch inst)
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
		protected internal virtual T VisitCkFinite(CkFinite inst)
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
		protected internal virtual T VisitLdfld(Ldfld inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdflda(Ldflda inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitStfld(Stfld inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdsfld(Ldsfld inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdsflda(Ldsflda inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitStsfld(Stsfld inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitIsInst(IsInst inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdInd(LdInd inst)
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
}

