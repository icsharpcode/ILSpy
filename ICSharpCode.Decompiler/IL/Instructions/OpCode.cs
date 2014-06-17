using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	public enum OpCode
	{
		/// <summary>
		/// No operation. Takes 0 arguments and returns void.
		///  <see cref="IL.Nop"/>
		/// </summary>
		Nop,
		/// <summary>
		/// Pops the top of the evaluation stack and returns the value.
		/// Does not correspond to any IL instruction, but encodes the implicit stack use by the IL instruction.
		///  <see cref="IL.Pop"/>
		/// </summary>
		Pop,
		/// <summary>
		/// Peeks at the top of the evaluation stack and returns the value.
		/// Corresponds to IL 'dup'. <see cref="IL.Peek"/>
		/// </summary>
		Peek,
		/// <summary>
		/// Ignore the arguments and produce void. Used to prevent the end result of an instruction
		/// from being pushed to the evaluation stack.
		/// <see cref="VoidInstruction"/>
		/// </summary>
		Void,
		/// <summary>
		/// Unary operator that expects an input of type I4.
		/// Return 1 (of type I4) if the input value is 0. Otherwise, return 0 (of type I4).
		/// <see cref="LogicNotInstruction"/>
		/// </summary>
		LogicNot,
		/// <summary>
		/// Adds two numbers. <see cref="BinaryNumericInstruction"/>
		/// </summary>
		Add,
		/// <summary>
		/// Subtracts two numbers. <see cref="BinaryNumericInstruction"/>
		/// </summary>
		Sub,
		/// <summary>
		/// Multiplies two numbers. <see cref="BinaryNumericInstruction"/>
		/// </summary>
		Mul,
		/// <summary>
		/// Division. <see cref="BinaryNumericInstruction"/>
		/// </summary>
		Div,
		/// <summary>
		/// Division remainder. <see cref="BinaryNumericInstruction"/>
		/// </summary>
		Rem,
		/// <summary>
		/// Unary negation. <see cref="UnaryInstruction"/>
		/// </summary>
		Neg,
		/// <summary>
		/// Bitwise AND. <see cref="BinaryNumericInstruction"/>
		/// </summary>
		BitAnd,
		/// <summary>
		/// Bitwise OR. <see cref="BinaryNumericInstruction"/>
		/// </summary>
		BitOr,
		/// <summary>
		/// Bitwise XOR. <see cref="BinaryNumericInstruction"/>
		/// </summary>
		BitXor,
		/// <summary>
		/// Bitwise NOT. <see cref="UnaryNumericInstruction"/>
		/// </summary>
		BitNot,
		/// <summary>
		/// Retrieves the RuntimeArgumentHandle. <see cref="IL.Arglist"/>
		/// </summary>
		Arglist,
		/// <summary>
		/// <c>if (cond) goto target;</c>
		/// <see cref="IL.ConditionalBranch"/>
		/// </summary>
		ConditionalBranch,
		/// <summary>
		/// <c>goto target;</c>
		/// <see cref="IL.Branch"/>
		/// </summary>
		Branch,
		Leave,
		/// <summary>
		/// Breakpoint instruction.
		/// </summary>
		DebugBreak,
		/// <summary>
		/// Compare equal.
		/// Returns 1 (of type I4) if two numbers or object references are equal; 0 otherwise.
		/// <see cref="BinaryComparisonInstruction"/>
		/// </summary>
		Ceq,
		/// <summary>
		/// Compare greater than.
		/// For integers, perform a signed comparison.
		/// For floating-point numbers, return 0 for unordered numbers.
		/// <see cref="BinaryComparisonInstruction"/>
		/// </summary>
		Cgt,
		/// <summary>
		/// Compare greater than (unordered/unsigned).
		/// For integers, perform a signed comparison.
		/// For floating-point numbers, return 1 for unordered numbers.
		/// <see cref="BinaryComparisonInstruction"/>
		/// </summary>
		Cgt_Un,
		/// <summary>
		/// Compare less than.
		/// For integers, perform a signed comparison.
		/// For floating-point numbers, return 0 for unordered numbers.
		/// <see cref="BinaryComparisonInstruction"/>
		/// </summary>
		Clt,
		/// <summary>
		/// Compare less than (unordered/unsigned).
		/// For integers, perform a signed comparison.
		/// For floating-point numbers, return 1 for unordered numbers.
		/// <see cref="BinaryComparisonInstruction"/>
		/// </summary>
		Clt_Un,
		/// <summary>
		/// Call a method.
		/// </summary>
		Call,
		/// <summary>
		/// Call a method using virtual dispatch.
		/// </summary>
		CallVirt,
		/// <summary>
		/// Checks that the float on top of the stack is not NaN or infinite.
		/// </summary>
		Ckfinite,
		/// <summary>
		/// Numeric cast. <see cref="ConvInstruction"/>
		/// </summary>
		Conv,
		/// <summary>
		/// Loads the value of a variable. (ldarg/ldloc)
		/// <see cref="IL.LdLoc"/>
		/// </summary>
		LdLoc,
		/// <summary>
		/// Loads the address of a variable. (ldarga/ldloca)
		/// <see cref="IL.LdLoca"/>
		/// </summary>
		LdLoca,
		/// <summary>
		/// Stores a value into a variable. (starg/stloc)
		/// <see cref="IL.StLoc"/>
		/// </summary>
		StLoc,
		/// <summary>
		/// Loads a constant string. <see cref="ConstantString"/>
		/// </summary>
		LdStr,
		/// <summary>
		/// Loads a constant 32-bit integer. <see cref="ConstantI4"/>
		/// </summary>
		LdcI4,
		/// <summary>
		/// Loads a constant 64-bit integer. <see cref="ConstantI8"/>
		/// </summary>
		LdcI8,
		/// <summary>
		/// Loads a constant floating point number. <see cref="ConstantFloat"/>
		/// </summary>
		LdcF,
		/// <summary>
		/// Loads a null reference.
		/// </summary>
		LdNull,
		/// <summary>
		/// Returns from the current method or lambda.
		/// <see cref="IL.RetVoid"/> or <see cref="IL.Ret"/>, depending on whether
		/// the method has return type void.
		/// </summary>
		Ret,
		/// <summary>
		/// Shift left. <see cref="BinaryNumericInstruction"/>
		/// </summary>
		Shl,
		/// <summary>
		/// Shift right. <see cref="BinaryNumericInstruction"/>
		/// </summary>
		Shr,
		/// <summary>
		/// Load instance field. <see cref="LoadInstanceField"/>
		/// </summary>
		Ldfld,
		/// <summary>
		/// Load instance field address. <see cref="LoadInstanceField"/>
		/// </summary>
		Ldflda,
		/// <summary>
		/// Store to instance field. <see cref="StoreInstanceField"/>
		/// </summary>
		Stfld,
		/// <summary>
		/// Load static field. <see cref="LoadStaticField"/>
		/// </summary>
		Ldsfld,
		/// <summary>
		/// Load static field address. <see cref="LoadStaticField"/>
		/// </summary>
		Ldsflda,
		/// <summary>
		/// Store to static field. <see cref="StoreStaticField"/>
		/// </summary>
		Stsfld,
	}
}
