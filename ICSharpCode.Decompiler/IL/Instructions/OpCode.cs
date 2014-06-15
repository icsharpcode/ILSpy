using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	enum OpCode
	{
		/// <summary>
		/// A instruction that could not be decoded correctly.
		/// Invalid instructions may appear in unreachable code.
		/// </summary>
		Invalid,
		/// <summary>
		/// No operation. Takes 0 arguments and returns void.
		/// </summary>
		Nop,
		/// <summary>
		/// Pops the top of the evaluation stack and returns the value.
		/// Does not correspond to any IL instruction, but encodes the implicit stack use by the IL instruction.
		/// </summary>
		Pop,
		/// <summary>
		/// Peeks at the top of the evaluation stack and returns the value.
		/// Corresponds to IL 'dup'.
		/// </summary>
		Peek,
		/// <summary>
		/// Ignore the arguments and produce void. Used to prevent the end result of an instruction
		/// from being pushed to the evaluation stack.
		/// </summary>
		Drop,
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
		/// Bitwise AND. <see cref="BinaryNumericInstruction"/>
		/// </summary>
		BitAnd,
		/// <summary>
		/// Retrieves the RuntimeArgumentHandle
		/// </summary>
		Arglist,
		/// <summary>
		/// <c>if (cond) goto target;</c>
		/// <see cref="ConditionalBranchInstruction"/>
		/// </summary>
		ConditionalBranch,
		/// <summary>
		/// <c>goto target;</c>
		/// <see cref="BranchInstruction"/>
		/// </summary>
		Branch,
		/// <summary>
		/// Breakpoint instruction.
		/// </summary>
		Break,
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
		Ckfinite,
		Conv,
		Div,
		LoadVar,
	}
}
