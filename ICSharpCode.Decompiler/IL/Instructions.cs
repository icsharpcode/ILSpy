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
		Ldloc,
		/// <summary>Loads the address of a local variable. (ldarga/ldloca)</summary>
		Ldloca,
		/// <summary>Stores a value into a local variable. (starg/stloc)</summary>
		Stloc,
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
	public sealed partial class Nop() : SimpleInstruction(OpCode.Nop)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitNop(this);
		}
	}

	/// <summary>Pops the top of the evaluation stack and returns the value.</summary>
	public sealed partial class Pop() : SimpleInstruction(OpCode.Pop)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitPop(this);
		}
	}

	/// <summary>Peeks at the top of the evaluation stack and returns the value. Corresponds to IL 'dup'.</summary>
	public sealed partial class Peek() : SimpleInstruction(OpCode.Peek)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitPeek(this);
		}
	}

	/// <summary>Ignore the arguments and produce void. Used to prevent the end result of an instruction from being pushed to the evaluation stack.</summary>
	public sealed partial class Void() : UnaryInstruction(OpCode.Void)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitVoid(this);
		}
	}

	/// <summary>A container of IL blocks.</summary>
	public sealed partial class BlockContainer() : ILInstruction(OpCode.BlockContainer)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitBlockContainer(this);
		}
	}

	/// <summary>A block of IL instructions.</summary>
	public sealed partial class Block() : ILInstruction(OpCode.Block)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitBlock(this);
		}
	}

	/// <summary>Unary operator that expects an input of type I4. Returns 1 (of type I4) if the input value is 0. Otherwise, returns 0 (of type I4).</summary>
	public sealed partial class LogicNot() : UnaryInstruction(OpCode.LogicNot)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitLogicNot(this);
		}
	}

	/// <summary>Adds two numbers.</summary>
	public sealed partial class Add(StackType opType, OverflowMode overflowMode) : BinaryNumericInstruction(OpCode.Add, opType, overflowMode)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitAdd(this);
		}
	}

	/// <summary>Subtracts two numbers</summary>
	public sealed partial class Sub(StackType opType, OverflowMode overflowMode) : BinaryNumericInstruction(OpCode.Sub, opType, overflowMode)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitSub(this);
		}
	}

	/// <summary>Multiplies two numbers</summary>
	public sealed partial class Mul(StackType opType, OverflowMode overflowMode) : BinaryNumericInstruction(OpCode.Mul, opType, overflowMode)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitMul(this);
		}
	}

	/// <summary>Divides two numbers</summary>
	public sealed partial class Div(StackType opType, OverflowMode overflowMode) : BinaryNumericInstruction(OpCode.Div, opType, overflowMode)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitDiv(this);
		}
	}

	/// <summary>Division remainder</summary>
	public sealed partial class Rem(StackType opType, OverflowMode overflowMode) : BinaryNumericInstruction(OpCode.Rem, opType, overflowMode)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitRem(this);
		}
	}

	/// <summary>Unary negation</summary>
	public sealed partial class Neg() : UnaryInstruction(OpCode.Neg)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitNeg(this);
		}
	}

	/// <summary>Bitwise AND</summary>
	public sealed partial class BitAnd(StackType opType, OverflowMode overflowMode) : BinaryNumericInstruction(OpCode.BitAnd, opType, overflowMode)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitBitAnd(this);
		}
	}

	/// <summary>Bitwise OR</summary>
	public sealed partial class BitOr(StackType opType, OverflowMode overflowMode) : BinaryNumericInstruction(OpCode.BitOr, opType, overflowMode)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitBitOr(this);
		}
	}

	/// <summary>Bitwise XOR</summary>
	public sealed partial class BitXor(StackType opType, OverflowMode overflowMode) : BinaryNumericInstruction(OpCode.BitXor, opType, overflowMode)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitBitXor(this);
		}
	}

	/// <summary>Bitwise NOT</summary>
	public sealed partial class BitNot() : UnaryInstruction(OpCode.BitNot)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitBitNot(this);
		}
	}

	/// <summary>Retrieves the RuntimeArgumentHandle.</summary>
	public sealed partial class Arglist() : SimpleInstruction(OpCode.Arglist)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitArglist(this);
		}
	}

	/// <summary><c>if (condition) goto target;</c>.</summary>
	public sealed partial class ConditionalBranch() : UnaryInstruction(OpCode.ConditionalBranch)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitConditionalBranch(this);
		}
	}

	/// <summary><c>goto target;</c>.</summary>
	public sealed partial class Branch() : SimpleInstruction(OpCode.Branch)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitBranch(this);
		}
	}

	/// <summary>Breakpoint instruction</summary>
	public sealed partial class DebugBreak() : SimpleInstruction(OpCode.DebugBreak)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitDebugBreak(this);
		}
	}

	/// <summary>Compare equal. Returns 1 (of type I4) if two numbers or object references are equal; 0 otherwise.</summary>
	public sealed partial class Ceq(StackType opType) : BinaryComparisonInstruction(OpCode.Ceq, opType)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitCeq(this);
		}
	}

	/// <summary>Compare greater than. For integers, perform a signed comparison. For floating-point numbers, return 0 for unordered numbers.</summary>
	public sealed partial class Cgt(StackType opType) : BinaryComparisonInstruction(OpCode.Cgt, opType)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitCgt(this);
		}
	}

	/// <summary>Compare greater than (unordered/unsigned). For integers, perform a signed comparison. For floating-point numbers, return 1 for unordered numbers.</summary>
	public sealed partial class Cgt_Un(StackType opType) : BinaryComparisonInstruction(OpCode.Cgt_Un, opType)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitCgt_Un(this);
		}
	}

	/// <summary>Compare less than. For integers, perform a signed comparison. For floating-point numbers, return 0 for unordered numbers.</summary>
	public sealed partial class Clt(StackType opType) : BinaryComparisonInstruction(OpCode.Clt, opType)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitClt(this);
		}
	}

	/// <summary>Compare less than (unordered/unsigned). For integers, perform a signed comparison. For floating-point numbers, return 1 for unordered numbers.</summary>
	public sealed partial class Clt_Un(StackType opType) : BinaryComparisonInstruction(OpCode.Clt_Un, opType)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitClt_Un(this);
		}
	}

	/// <summary>Non-virtual method call.</summary>
	public sealed partial class Call(MethodReference method) : CallInstruction(OpCode.Call, method)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitCall(this);
		}
	}

	/// <summary>Virtual method call.</summary>
	public sealed partial class CallVirt(MethodReference method) : CallInstruction(OpCode.CallVirt, method)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitCallVirt(this);
		}
	}

	/// <summary>Checks that the float on top of the stack is not NaN or infinite.</summary>
	public sealed partial class CkFinite() : SimpleInstruction(OpCode.CkFinite)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitCkFinite(this);
		}
	}

	/// <summary>Numeric cast.</summary>
	public sealed partial class Conv() : UnaryInstruction(OpCode.Conv)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitConv(this);
		}
	}

	/// <summary>Loads the value of a local variable. (ldarg/ldloc)</summary>
	public sealed partial class Ldloc() : SimpleInstruction(OpCode.Ldloc)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitLdloc(this);
		}
	}

	/// <summary>Loads the address of a local variable. (ldarga/ldloca)</summary>
	public sealed partial class Ldloca() : SimpleInstruction(OpCode.Ldloca)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitLdloca(this);
		}
	}

	/// <summary>Stores a value into a local variable. (starg/stloc)</summary>
	public sealed partial class Stloc() : UnaryInstruction(OpCode.Stloc)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitStloc(this);
		}
	}

	/// <summary>Loads a constant string.</summary>
	public sealed partial class LdStr() : SimpleInstruction(OpCode.LdStr)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitLdStr(this);
		}
	}

	/// <summary>Loads a constant 32-bit integer.</summary>
	public sealed partial class LdcI4() : SimpleInstruction(OpCode.LdcI4)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitLdcI4(this);
		}
	}

	/// <summary>Loads a constant 64-bit integer.</summary>
	public sealed partial class LdcI8() : SimpleInstruction(OpCode.LdcI8)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitLdcI8(this);
		}
	}

	/// <summary>Loads a constant floating-point number.</summary>
	public sealed partial class LdcF() : SimpleInstruction(OpCode.LdcF)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitLdcF(this);
		}
	}

	/// <summary>Loads the null reference.</summary>
	public sealed partial class LdNull() : SimpleInstruction(OpCode.LdNull)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitLdNull(this);
		}
	}

	/// <summary>Returns from the current method or lambda.</summary>
	public sealed partial class Return() : ILInstruction(OpCode.Return)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitReturn(this);
		}
	}

	/// <summary>Shift left</summary>
	public sealed partial class Shl(StackType opType, OverflowMode overflowMode) : BinaryNumericInstruction(OpCode.Shl, opType, overflowMode)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitShl(this);
		}
	}

	/// <summary>Shift right</summary>
	public sealed partial class Shr(StackType opType, OverflowMode overflowMode) : BinaryNumericInstruction(OpCode.Shr, opType, overflowMode)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitShr(this);
		}
	}

	/// <summary>Load instance field</summary>
	public sealed partial class Ldfld() : UnaryInstruction(OpCode.Ldfld)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitLdfld(this);
		}
	}

	/// <summary>Load address of instance field</summary>
	public sealed partial class Ldflda() : UnaryInstruction(OpCode.Ldflda)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitLdflda(this);
		}
	}

	/// <summary>Store value to instance field</summary>
	public sealed partial class Stfld() : BinaryInstruction(OpCode.Stfld)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitStfld(this);
		}
	}

	/// <summary>Load static field</summary>
	public sealed partial class Ldsfld() : SimpleInstruction(OpCode.Ldsfld)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitLdsfld(this);
		}
	}

	/// <summary>Load static field address</summary>
	public sealed partial class Ldsflda() : SimpleInstruction(OpCode.Ldsflda)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitLdsflda(this);
		}
	}

	/// <summary>Store value to static field</summary>
	public sealed partial class Stsfld() : UnaryInstruction(OpCode.Stsfld)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitStsfld(this);
		}
	}

	/// <summary>Test if object is instance of class or interface.</summary>
	public sealed partial class IsInst() : UnaryInstruction(OpCode.IsInst)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitIsInst(this);
		}
	}

	/// <summary>Indirect load (ref/pointer dereference).</summary>
	public sealed partial class LdInd() : UnaryInstruction(OpCode.LdInd)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitLdInd(this);
		}
	}

	/// <summary>Unbox a value.</summary>
	public sealed partial class UnboxAny() : UnaryInstruction(OpCode.UnboxAny)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitUnboxAny(this);
		}
	}

	/// <summary>Creates an object instance and calls the constructor.</summary>
	public sealed partial class NewObj(MethodReference method) : CallInstruction(OpCode.NewObj, method)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitNewObj(this);
		}
	}

	/// <summary>Throws an exception.</summary>
	public sealed partial class Throw() : UnaryInstruction(OpCode.Throw)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitThrow(this);
		}
	}

	/// <summary>Returns the length of an array as 'native unsigned int'.</summary>
	public sealed partial class LdLen() : UnaryInstruction(OpCode.LdLen)
	{

		public override TReturn AcceptVisitor<TReturn>(ILVisitor<TReturn> visitor)
		{
			return visitor.VisitLdLen(this);
		}
	}


	
	public abstract class ILVisitor<TReturn>
	{
		protected abstract TReturn Default(ILInstruction inst);
		
		protected internal virtual TReturn VisitNop(Nop inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitPop(Pop inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitPeek(Peek inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitVoid(Void inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitBlockContainer(BlockContainer inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitBlock(Block inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitLogicNot(LogicNot inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitAdd(Add inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitSub(Sub inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitMul(Mul inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitDiv(Div inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitRem(Rem inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitNeg(Neg inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitBitAnd(BitAnd inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitBitOr(BitOr inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitBitXor(BitXor inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitBitNot(BitNot inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitArglist(Arglist inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitConditionalBranch(ConditionalBranch inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitBranch(Branch inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitDebugBreak(DebugBreak inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitCeq(Ceq inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitCgt(Cgt inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitCgt_Un(Cgt_Un inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitClt(Clt inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitClt_Un(Clt_Un inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitCall(Call inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitCallVirt(CallVirt inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitCkFinite(CkFinite inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitConv(Conv inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitLdloc(Ldloc inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitLdloca(Ldloca inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitStloc(Stloc inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitLdStr(LdStr inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitLdcI4(LdcI4 inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitLdcI8(LdcI8 inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitLdcF(LdcF inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitLdNull(LdNull inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitReturn(Return inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitShl(Shl inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitShr(Shr inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitLdfld(Ldfld inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitLdflda(Ldflda inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitStfld(Stfld inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitLdsfld(Ldsfld inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitLdsflda(Ldsflda inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitStsfld(Stsfld inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitIsInst(IsInst inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitLdInd(LdInd inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitUnboxAny(UnboxAny inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitNewObj(NewObj inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitThrow(Throw inst)
		{
			return Default(inst);
		}
		protected internal virtual TReturn VisitLdLen(LdLen inst)
		{
			return Default(inst);
		}
	}
}

