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
using System.Collections.Generic;
using System.Diagnostics;
using ICSharpCode.NRefactory.TypeSystem;

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
		ILFunction,
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
		/// <summary>Marks the end of an finally, fault or exception filter block.</summary>
		EndFinally,
		/// <summary>If statement / conditional expression. <c>if (condition) trueExpr else falseExpr</c></summary>
		IfInstruction,
		/// <summary>Infinite loop</summary>
		Loop,
		/// <summary>Try-catch statement.</summary>
		TryCatch,
		/// <summary>Catch handler within a try-catch statement.</summary>
		TryCatchHandler,
		/// <summary>Try-finally statement</summary>
		TryFinally,
		/// <summary>Try-fault statement</summary>
		TryFault,
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
		/// <summary>Checks that the input float is not NaN or infinite.</summary>
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
		/// <summary>Load method pointer</summary>
		LdFtn,
		/// <summary>Load method pointer</summary>
		LdVirtFtn,
		/// <summary>Loads runtime representation of metadata token</summary>
		LdToken,
		/// <summary>Allocates space in the stack frame</summary>
		LocAlloc,
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
		/// <summary>Casts an object to a class.</summary>
		CastClass,
		/// <summary>Test if object is instance of class or interface.</summary>
		IsInst,
		/// <summary>Indirect load (ref/pointer dereference).</summary>
		LdObj,
		/// <summary>Indirect store (store to ref/pointer).</summary>
		StObj,
		/// <summary>Boxes a value.</summary>
		Box,
		/// <summary>Compute address inside box.</summary>
		Unbox,
		/// <summary>Unbox a value.</summary>
		UnboxAny,
		/// <summary>Creates an object instance and calls the constructor.</summary>
		NewObj,
		/// <summary>Initializes the value at an address.</summary>
		InitObj,
		/// <summary>Returns the default value for a type.</summary>
		DefaultValue,
		/// <summary>Throws an exception.</summary>
		Throw,
		/// <summary>Rethrows the current exception.</summary>
		Rethrow,
		/// <summary>Gets the size of a type in bytes.</summary>
		SizeOf,
		/// <summary>Returns the length of an array as 'native unsigned int'.</summary>
		LdLen,
		/// <summary>Load address of array element.</summary>
		LdElema,
	}

	/// <summary>Instruction with a single argument</summary>
	public abstract partial class UnaryInstruction : ILInstruction
	{
		protected UnaryInstruction(OpCode opCode, ILInstruction argument) : base(opCode)
		{
			this.Argument = argument;
		}
		ILInstruction argument;
		public ILInstruction Argument {
			get { return this.argument; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.argument, value);
			}
		}
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return this.argument;
			}
		}
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Argument = this.argument.AcceptVisitor(visitor);
		}
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			this.Argument = this.argument.Inline(flagsBefore, context);
			return this;
		}
		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
			Argument.TransformStackIntoVariables(state);
		}
		protected override InstructionFlags ComputeFlags()
		{
			return argument.Flags;
		}
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write('(');
			this.argument.WriteTo(output);
			output.Write(')');
		}
	}

	/// <summary>Instruction with two arguments: Left and Right</summary>
	public abstract partial class BinaryInstruction : ILInstruction
	{
		protected BinaryInstruction(OpCode opCode, ILInstruction left, ILInstruction right) : base(opCode)
		{
			this.Left = left;
			this.Right = right;
		}
		ILInstruction left;
		public ILInstruction Left {
			get { return this.left; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.left, value);
			}
		}
		ILInstruction right;
		public ILInstruction Right {
			get { return this.right; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.right, value);
			}
		}
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return this.left;
				yield return this.right;
			}
		}
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Left = this.left.AcceptVisitor(visitor);
			this.Right = this.right.AcceptVisitor(visitor);
		}
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			this.Right = this.right.Inline(flagsBefore | ((this.left.Flags) & ~(InstructionFlags.MayPeek | InstructionFlags.MayPop)), context);
			this.Left = this.left.Inline(flagsBefore, context);
			return this;
		}
		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
			Left.TransformStackIntoVariables(state);
			Right.TransformStackIntoVariables(state);
		}
		protected override InstructionFlags ComputeFlags()
		{
			return left.Flags | right.Flags;
		}
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write('(');
			this.left.WriteTo(output);
			output.Write(", ");
			this.right.WriteTo(output);
			output.Write(')');
		}
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
	public sealed partial class ILFunction : ILInstruction
	{
		ILInstruction body;
		public ILInstruction Body {
			get { return this.body; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.body, value);
			}
		}
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return this.body;
			}
		}
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Body = this.body.AcceptVisitor(visitor);
		}
		public override StackType ResultType { get { return StackType.O; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitILFunction(this);
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
		public Add(ILInstruction left, ILInstruction right, bool checkForOverflow, Sign sign, CompoundAssignmentType compoundAssignmentType = CompoundAssignmentType.None) : base(OpCode.Add, left, right, checkForOverflow, sign, compoundAssignmentType)
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
		public Sub(ILInstruction left, ILInstruction right, bool checkForOverflow, Sign sign, CompoundAssignmentType compoundAssignmentType = CompoundAssignmentType.None) : base(OpCode.Sub, left, right, checkForOverflow, sign, compoundAssignmentType)
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
		public Mul(ILInstruction left, ILInstruction right, bool checkForOverflow, Sign sign, CompoundAssignmentType compoundAssignmentType = CompoundAssignmentType.None) : base(OpCode.Mul, left, right, checkForOverflow, sign, compoundAssignmentType)
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
		public Div(ILInstruction left, ILInstruction right, bool checkForOverflow, Sign sign, CompoundAssignmentType compoundAssignmentType = CompoundAssignmentType.None) : base(OpCode.Div, left, right, checkForOverflow, sign, compoundAssignmentType)
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
		public Rem(ILInstruction left, ILInstruction right, bool checkForOverflow, Sign sign, CompoundAssignmentType compoundAssignmentType = CompoundAssignmentType.None) : base(OpCode.Rem, left, right, checkForOverflow, sign, compoundAssignmentType)
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
		public BitAnd(ILInstruction left, ILInstruction right, bool checkForOverflow, Sign sign, CompoundAssignmentType compoundAssignmentType = CompoundAssignmentType.None) : base(OpCode.BitAnd, left, right, checkForOverflow, sign, compoundAssignmentType)
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
		public BitOr(ILInstruction left, ILInstruction right, bool checkForOverflow, Sign sign, CompoundAssignmentType compoundAssignmentType = CompoundAssignmentType.None) : base(OpCode.BitOr, left, right, checkForOverflow, sign, compoundAssignmentType)
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
		public BitXor(ILInstruction left, ILInstruction right, bool checkForOverflow, Sign sign, CompoundAssignmentType compoundAssignmentType = CompoundAssignmentType.None) : base(OpCode.BitXor, left, right, checkForOverflow, sign, compoundAssignmentType)
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
		public override StackType ResultType { get { return StackType.Void; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBranch(this);
		}
	}

	/// <summary>Marks the end of an finally, fault or exception filter block.</summary>
	public sealed partial class EndFinally : SimpleInstruction
	{
		public EndFinally() : base(OpCode.EndFinally)
		{
		}
		public override StackType ResultType { get { return StackType.Void; } }
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.EndPointUnreachable | InstructionFlags.MayBranch;
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitEndFinally(this);
		}
	}

	/// <summary>If statement / conditional expression. <c>if (condition) trueExpr else falseExpr</c></summary>
	public sealed partial class IfInstruction : ILInstruction
	{
		ILInstruction condition;
		public ILInstruction Condition {
			get { return this.condition; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.condition, value);
			}
		}
		ILInstruction trueInst;
		public ILInstruction TrueInst {
			get { return this.trueInst; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.trueInst, value);
			}
		}
		ILInstruction falseInst;
		public ILInstruction FalseInst {
			get { return this.falseInst; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.falseInst, value);
			}
		}
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return this.condition;
				yield return this.trueInst;
				yield return this.falseInst;
			}
		}
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Condition = this.condition.AcceptVisitor(visitor);
			this.TrueInst = this.trueInst.AcceptVisitor(visitor);
			this.FalseInst = this.falseInst.AcceptVisitor(visitor);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitIfInstruction(this);
		}
	}

	/// <summary>Infinite loop</summary>
	public sealed partial class Loop : ILInstruction
	{
		public Loop(ILInstruction body) : base(OpCode.Loop)
		{
			this.Body = body;
		}
		ILInstruction body;
		public ILInstruction Body {
			get { return this.body; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.body, value);
			}
		}
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return this.body;
			}
		}
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Body = this.body.AcceptVisitor(visitor);
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLoop(this);
		}
	}

	/// <summary>Try-catch statement.</summary>
	public sealed partial class TryCatch : TryInstruction
	{

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitTryCatch(this);
		}
	}

	/// <summary>Catch handler within a try-catch statement.</summary>
	public sealed partial class TryCatchHandler : ILInstruction
	{
		public TryCatchHandler(ILInstruction filter, ILInstruction body, ILVariable variable) : base(OpCode.TryCatchHandler)
		{
			this.Filter = filter;
			this.Body = body;
			Debug.Assert(variable != null);
			this.variable = variable;
		}
		ILInstruction filter;
		public ILInstruction Filter {
			get { return this.filter; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.filter, value);
			}
		}
		ILInstruction body;
		public ILInstruction Body {
			get { return this.body; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.body, value);
			}
		}
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return this.filter;
				yield return this.body;
			}
		}
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Filter = this.filter.AcceptVisitor(visitor);
			this.Body = this.body.AcceptVisitor(visitor);
		}
		readonly ILVariable variable;
		/// <summary>Returns the variable operand.</summary>
		public ILVariable Variable { get { return variable; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitTryCatchHandler(this);
		}
	}

	/// <summary>Try-finally statement</summary>
	public sealed partial class TryFinally : TryInstruction
	{

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitTryFinally(this);
		}
	}

	/// <summary>Try-fault statement</summary>
	public sealed partial class TryFault : TryInstruction
	{

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitTryFault(this);
		}
	}

	/// <summary>Breakpoint instruction</summary>
	public sealed partial class DebugBreak : SimpleInstruction
	{
		public DebugBreak() : base(OpCode.DebugBreak)
		{
		}
		public override StackType ResultType { get { return StackType.Void; } }
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.SideEffect;
		}
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
		public Call(IMethod method) : base(OpCode.Call, method)
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
		public CallVirt(IMethod method) : base(OpCode.CallVirt, method)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitCallVirt(this);
		}
	}

	/// <summary>Checks that the input float is not NaN or infinite.</summary>
	public sealed partial class Ckfinite : UnaryInstruction
	{
		public Ckfinite(ILInstruction argument) : base(OpCode.Ckfinite, argument)
		{
		}
		public override StackType ResultType { get { return StackType.Void; } }
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow;
		}
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
			Debug.Assert(variable != null);
			this.variable = variable;
		}
		readonly ILVariable variable;
		/// <summary>Returns the variable operand.</summary>
		public ILVariable Variable { get { return variable; } }
		public override StackType ResultType { get { return variable.Type.GetStackType(); } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			variable.WriteTo(output);
		}
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
			Debug.Assert(variable != null);
			this.variable = variable;
		}
		public override StackType ResultType { get { return StackType.Ref; } }
		readonly ILVariable variable;
		/// <summary>Returns the variable operand.</summary>
		public ILVariable Variable { get { return variable; } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			variable.WriteTo(output);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdLoca(this);
		}
	}

	/// <summary>Stores a value into a local variable. (starg/stloc)</summary>
	public sealed partial class StLoc : ILInstruction
	{
		public StLoc(ILInstruction value, ILVariable variable) : base(OpCode.StLoc)
		{
			this.Value = value;
			Debug.Assert(variable != null);
			this.variable = variable;
		}
		ILInstruction value;
		public ILInstruction Value {
			get { return this.value; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.value, value);
			}
		}
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return this.value;
			}
		}
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Value = this.value.AcceptVisitor(visitor);
		}
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			this.Value = this.value.Inline(flagsBefore, context);
			return this;
		}
		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
			Value.TransformStackIntoVariables(state);
		}
		readonly ILVariable variable;
		/// <summary>Returns the variable operand.</summary>
		public ILVariable Variable { get { return variable; } }
		public override StackType ResultType { get { return variable.Type.GetStackType(); } }
		protected override InstructionFlags ComputeFlags()
		{
			return value.Flags;
		}
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			variable.WriteTo(output);
			output.Write('(');
			this.value.WriteTo(output);
			output.Write(')');
		}
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
		public override StackType ResultType { get { return StackType.O; } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
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
		public override StackType ResultType { get { return StackType.I4; } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
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
		public override StackType ResultType { get { return StackType.I8; } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
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
		public override StackType ResultType { get { return StackType.F; } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
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

	/// <summary>Load method pointer</summary>
	public sealed partial class LdFtn : SimpleInstruction
	{
		public LdFtn(IMethod method) : base(OpCode.LdFtn)
		{
			this.method = method;
		}
		readonly IMethod method;
		/// <summary>Returns the method operand.</summary>
		public IMethod Method { get { return method; } }
		public override StackType ResultType { get { return StackType.I; } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, method);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdFtn(this);
		}
	}

	/// <summary>Load method pointer</summary>
	public sealed partial class LdVirtFtn : UnaryInstruction
	{
		public LdVirtFtn(ILInstruction argument, IMethod method) : base(OpCode.LdVirtFtn, argument)
		{
			this.method = method;
		}
		readonly IMethod method;
		/// <summary>Returns the method operand.</summary>
		public IMethod Method { get { return method; } }
		public override StackType ResultType { get { return StackType.I; } }
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow;
		}
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, method);
			output.Write('(');
			Argument.WriteTo(output);
			output.Write(')');
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdVirtFtn(this);
		}
	}

	/// <summary>Loads runtime representation of metadata token</summary>
	public sealed partial class LdToken : SimpleInstruction
	{
		public LdToken(Mono.Cecil.MemberReference member) : base(OpCode.LdToken)
		{
			this.member = member;
		}
		readonly Mono.Cecil.MemberReference member;
		/// <summary>Returns the token operand.</summary>
		public Mono.Cecil.MemberReference Member { get { return member; } }
		public override StackType ResultType { get { return StackType.O; } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, member);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdToken(this);
		}
	}

	/// <summary>Allocates space in the stack frame</summary>
	public sealed partial class LocAlloc : UnaryInstruction
	{
		public LocAlloc(ILInstruction argument) : base(OpCode.LocAlloc, argument)
		{
		}
		public override StackType ResultType { get { return StackType.I; } }
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow;
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLocAlloc(this);
		}
	}

	/// <summary>Returns from the current method or lambda.</summary>
	public sealed partial class Return : ILInstruction
	{
		public override StackType ResultType { get { return StackType.Void; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitReturn(this);
		}
	}

	/// <summary>Shift left</summary>
	public sealed partial class Shl : BinaryNumericInstruction
	{
		public Shl(ILInstruction left, ILInstruction right, bool checkForOverflow, Sign sign, CompoundAssignmentType compoundAssignmentType = CompoundAssignmentType.None) : base(OpCode.Shl, left, right, checkForOverflow, sign, compoundAssignmentType)
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
		public Shr(ILInstruction left, ILInstruction right, bool checkForOverflow, Sign sign, CompoundAssignmentType compoundAssignmentType = CompoundAssignmentType.None) : base(OpCode.Shr, left, right, checkForOverflow, sign, compoundAssignmentType)
		{
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitShr(this);
		}
	}

	/// <summary>Load instance field</summary>
	public sealed partial class LdFld : ILInstruction, ISupportsVolatilePrefix, ISupportsUnalignedPrefix
	{
		public LdFld(ILInstruction target, IField field) : base(OpCode.LdFld)
		{
			this.Target = target;
			this.field = field;
		}
		ILInstruction target;
		public ILInstruction Target {
			get { return this.target; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.target, value);
			}
		}
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return this.target;
			}
		}
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Target = this.target.AcceptVisitor(visitor);
		}
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			this.Target = this.target.Inline(flagsBefore, context);
			return this;
		}
		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
			Target.TransformStackIntoVariables(state);
		}
		/// <summary>Gets/Sets whether the memory access is volatile.</summary>
		public bool IsVolatile { get; set; }
		/// <summary>Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.</summary>
		public byte UnalignedPrefix { get; set; }
		readonly IField field;
		/// <summary>Returns the field operand.</summary>
		public IField Field { get { return field; } }
		public override StackType ResultType { get { return field.Type.GetStackType(); } }
		protected override InstructionFlags ComputeFlags()
		{
			return target.Flags | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
		}
		public override void WriteTo(ITextOutput output)
		{
			if (IsVolatile)
				output.Write("volatile.");
			if (UnalignedPrefix > 0)
				output.Write("unaligned(" + UnalignedPrefix + ").");
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, field);
			output.Write('(');
			this.target.WriteTo(output);
			output.Write(')');
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdFld(this);
		}
	}

	/// <summary>Load address of instance field</summary>
	public sealed partial class LdFlda : ILInstruction
	{
		public LdFlda(ILInstruction target, IField field) : base(OpCode.LdFlda)
		{
			this.Target = target;
			this.field = field;
		}
		ILInstruction target;
		public ILInstruction Target {
			get { return this.target; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.target, value);
			}
		}
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return this.target;
			}
		}
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Target = this.target.AcceptVisitor(visitor);
		}
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			this.Target = this.target.Inline(flagsBefore, context);
			return this;
		}
		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
			Target.TransformStackIntoVariables(state);
		}
		readonly IField field;
		/// <summary>Returns the field operand.</summary>
		public IField Field { get { return field; } }
		public override StackType ResultType { get { return StackType.Ref; } }
		protected override InstructionFlags ComputeFlags()
		{
			return target.Flags | InstructionFlags.MayThrow;
		}
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, field);
			output.Write('(');
			this.target.WriteTo(output);
			output.Write(')');
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdFlda(this);
		}
	}

	/// <summary>Store value to instance field</summary>
	public sealed partial class StFld : ILInstruction, ISupportsVolatilePrefix, ISupportsUnalignedPrefix
	{
		public StFld(ILInstruction target, ILInstruction value, IField field) : base(OpCode.StFld)
		{
			this.Target = target;
			this.Value = value;
			this.field = field;
		}
		ILInstruction target;
		public ILInstruction Target {
			get { return this.target; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.target, value);
			}
		}
		ILInstruction value;
		public ILInstruction Value {
			get { return this.value; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.value, value);
			}
		}
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return this.target;
				yield return this.value;
			}
		}
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Target = this.target.AcceptVisitor(visitor);
			this.Value = this.value.AcceptVisitor(visitor);
		}
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			this.Value = this.value.Inline(flagsBefore | ((this.target.Flags) & ~(InstructionFlags.MayPeek | InstructionFlags.MayPop)), context);
			this.Target = this.target.Inline(flagsBefore, context);
			return this;
		}
		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
			Target.TransformStackIntoVariables(state);
			Value.TransformStackIntoVariables(state);
		}
		/// <summary>Gets/Sets whether the memory access is volatile.</summary>
		public bool IsVolatile { get; set; }
		/// <summary>Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.</summary>
		public byte UnalignedPrefix { get; set; }
		readonly IField field;
		/// <summary>Returns the field operand.</summary>
		public IField Field { get { return field; } }
		public override StackType ResultType { get { return field.Type.GetStackType(); } }
		protected override InstructionFlags ComputeFlags()
		{
			return target.Flags | value.Flags | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
		}
		public override void WriteTo(ITextOutput output)
		{
			if (IsVolatile)
				output.Write("volatile.");
			if (UnalignedPrefix > 0)
				output.Write("unaligned(" + UnalignedPrefix + ").");
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, field);
			output.Write('(');
			this.target.WriteTo(output);
			output.Write(", ");
			this.value.WriteTo(output);
			output.Write(')');
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitStFld(this);
		}
	}

	/// <summary>Load static field</summary>
	public sealed partial class LdsFld : SimpleInstruction, ISupportsVolatilePrefix, ISupportsUnalignedPrefix
	{
		public LdsFld(IField field) : base(OpCode.LdsFld)
		{
			this.field = field;
		}
		/// <summary>Gets/Sets whether the memory access is volatile.</summary>
		public bool IsVolatile { get; set; }
		/// <summary>Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.</summary>
		public byte UnalignedPrefix { get; set; }
		readonly IField field;
		/// <summary>Returns the field operand.</summary>
		public IField Field { get { return field; } }
		public override StackType ResultType { get { return field.Type.GetStackType(); } }
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.SideEffect;
		}
		public override void WriteTo(ITextOutput output)
		{
			if (IsVolatile)
				output.Write("volatile.");
			if (UnalignedPrefix > 0)
				output.Write("unaligned(" + UnalignedPrefix + ").");
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, field);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdsFld(this);
		}
	}

	/// <summary>Load static field address</summary>
	public sealed partial class LdsFlda : SimpleInstruction
	{
		public LdsFlda(IField field) : base(OpCode.LdsFlda)
		{
			this.field = field;
		}
		public override StackType ResultType { get { return StackType.Ref; } }
		readonly IField field;
		/// <summary>Returns the field operand.</summary>
		public IField Field { get { return field; } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, field);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdsFlda(this);
		}
	}

	/// <summary>Store value to static field</summary>
	public sealed partial class StsFld : ILInstruction, ISupportsVolatilePrefix, ISupportsUnalignedPrefix
	{
		public StsFld(ILInstruction value, IField field) : base(OpCode.StsFld)
		{
			this.Value = value;
			this.field = field;
		}
		ILInstruction value;
		public ILInstruction Value {
			get { return this.value; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.value, value);
			}
		}
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return this.value;
			}
		}
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Value = this.value.AcceptVisitor(visitor);
		}
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			this.Value = this.value.Inline(flagsBefore, context);
			return this;
		}
		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
			Value.TransformStackIntoVariables(state);
		}
		/// <summary>Gets/Sets whether the memory access is volatile.</summary>
		public bool IsVolatile { get; set; }
		/// <summary>Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.</summary>
		public byte UnalignedPrefix { get; set; }
		readonly IField field;
		/// <summary>Returns the field operand.</summary>
		public IField Field { get { return field; } }
		public override StackType ResultType { get { return field.Type.GetStackType(); } }
		protected override InstructionFlags ComputeFlags()
		{
			return value.Flags | InstructionFlags.SideEffect;
		}
		public override void WriteTo(ITextOutput output)
		{
			if (IsVolatile)
				output.Write("volatile.");
			if (UnalignedPrefix > 0)
				output.Write("unaligned(" + UnalignedPrefix + ").");
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, field);
			output.Write('(');
			this.value.WriteTo(output);
			output.Write(')');
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitStsFld(this);
		}
	}

	/// <summary>Casts an object to a class.</summary>
	public sealed partial class CastClass : UnaryInstruction
	{
		public CastClass(ILInstruction argument, IType type) : base(OpCode.CastClass, argument)
		{
			this.type = type;
		}
		readonly IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type { get { return type; } }
		public override StackType ResultType { get { return type.GetStackType(); } }
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow;
		}
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, type);
			output.Write('(');
			Argument.WriteTo(output);
			output.Write(')');
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitCastClass(this);
		}
	}

	/// <summary>Test if object is instance of class or interface.</summary>
	public sealed partial class IsInst : UnaryInstruction
	{
		public IsInst(ILInstruction argument, IType type) : base(OpCode.IsInst, argument)
		{
			this.type = type;
		}
		readonly IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type { get { return type; } }
		public override StackType ResultType { get { return StackType.O; } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, type);
			output.Write('(');
			Argument.WriteTo(output);
			output.Write(')');
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitIsInst(this);
		}
	}

	/// <summary>Indirect load (ref/pointer dereference).</summary>
	public sealed partial class LdObj : ILInstruction, ISupportsVolatilePrefix, ISupportsUnalignedPrefix
	{
		public LdObj(ILInstruction target, IType type) : base(OpCode.LdObj)
		{
			this.Target = target;
			this.type = type;
		}
		ILInstruction target;
		public ILInstruction Target {
			get { return this.target; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.target, value);
			}
		}
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return this.target;
			}
		}
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Target = this.target.AcceptVisitor(visitor);
		}
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			this.Target = this.target.Inline(flagsBefore, context);
			return this;
		}
		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
			Target.TransformStackIntoVariables(state);
		}
		readonly IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type { get { return type; } }
		/// <summary>Gets/Sets whether the memory access is volatile.</summary>
		public bool IsVolatile { get; set; }
		/// <summary>Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.</summary>
		public byte UnalignedPrefix { get; set; }
		public override StackType ResultType { get { return type.GetStackType(); } }
		protected override InstructionFlags ComputeFlags()
		{
			return target.Flags | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
		}
		public override void WriteTo(ITextOutput output)
		{
			if (IsVolatile)
				output.Write("volatile.");
			if (UnalignedPrefix > 0)
				output.Write("unaligned(" + UnalignedPrefix + ").");
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, type);
			output.Write('(');
			this.target.WriteTo(output);
			output.Write(')');
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdObj(this);
		}
	}

	/// <summary>Indirect store (store to ref/pointer).</summary>
	public sealed partial class StObj : ILInstruction, ISupportsVolatilePrefix, ISupportsUnalignedPrefix
	{
		public StObj(ILInstruction target, ILInstruction value, IType type) : base(OpCode.StObj)
		{
			this.Target = target;
			this.Value = value;
			this.type = type;
		}
		ILInstruction target;
		public ILInstruction Target {
			get { return this.target; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.target, value);
			}
		}
		ILInstruction value;
		public ILInstruction Value {
			get { return this.value; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.value, value);
			}
		}
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return this.target;
				yield return this.value;
			}
		}
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Target = this.target.AcceptVisitor(visitor);
			this.Value = this.value.AcceptVisitor(visitor);
		}
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			this.Value = this.value.Inline(flagsBefore | ((this.target.Flags) & ~(InstructionFlags.MayPeek | InstructionFlags.MayPop)), context);
			this.Target = this.target.Inline(flagsBefore, context);
			return this;
		}
		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
			Target.TransformStackIntoVariables(state);
			Value.TransformStackIntoVariables(state);
		}
		readonly IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type { get { return type; } }
		/// <summary>Gets/Sets whether the memory access is volatile.</summary>
		public bool IsVolatile { get; set; }
		/// <summary>Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.</summary>
		public byte UnalignedPrefix { get; set; }
		public override StackType ResultType { get { return type.GetStackType(); } }
		protected override InstructionFlags ComputeFlags()
		{
			return target.Flags | value.Flags | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
		}
		public override void WriteTo(ITextOutput output)
		{
			if (IsVolatile)
				output.Write("volatile.");
			if (UnalignedPrefix > 0)
				output.Write("unaligned(" + UnalignedPrefix + ").");
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, type);
			output.Write('(');
			this.target.WriteTo(output);
			output.Write(", ");
			this.value.WriteTo(output);
			output.Write(')');
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitStObj(this);
		}
	}

	/// <summary>Boxes a value.</summary>
	public sealed partial class Box : UnaryInstruction
	{
		public Box(ILInstruction argument, IType type) : base(OpCode.Box, argument)
		{
			this.type = type;
		}
		readonly IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type { get { return type; } }
		public override StackType ResultType { get { return StackType.O; } }
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
		}
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, type);
			output.Write('(');
			Argument.WriteTo(output);
			output.Write(')');
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBox(this);
		}
	}

	/// <summary>Compute address inside box.</summary>
	public sealed partial class Unbox : UnaryInstruction
	{
		public Unbox(ILInstruction argument, IType type) : base(OpCode.Unbox, argument)
		{
			this.type = type;
		}
		readonly IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type { get { return type; } }
		public override StackType ResultType { get { return StackType.Ref; } }
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow;
		}
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, type);
			output.Write('(');
			Argument.WriteTo(output);
			output.Write(')');
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitUnbox(this);
		}
	}

	/// <summary>Unbox a value.</summary>
	public sealed partial class UnboxAny : UnaryInstruction
	{
		public UnboxAny(ILInstruction argument, IType type) : base(OpCode.UnboxAny, argument)
		{
			this.type = type;
		}
		readonly IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type { get { return type; } }
		public override StackType ResultType { get { return type.GetStackType(); } }
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
		}
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, type);
			output.Write('(');
			Argument.WriteTo(output);
			output.Write(')');
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitUnboxAny(this);
		}
	}

	/// <summary>Creates an object instance and calls the constructor.</summary>
	public sealed partial class NewObj : CallInstruction
	{
		public NewObj(IMethod method) : base(OpCode.NewObj, method)
		{
		}
		public override StackType ResultType { get { return StackType.O; } }
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitNewObj(this);
		}
	}

	/// <summary>Initializes the value at an address.</summary>
	public sealed partial class InitObj : UnaryInstruction
	{
		public InitObj(ILInstruction argument, IType type) : base(OpCode.InitObj, argument)
		{
			this.type = type;
		}
		readonly IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type { get { return type; } }
		public override StackType ResultType { get { return StackType.Void; } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, type);
			output.Write('(');
			Argument.WriteTo(output);
			output.Write(')');
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitInitObj(this);
		}
	}

	/// <summary>Returns the default value for a type.</summary>
	public sealed partial class DefaultValue : SimpleInstruction
	{
		public DefaultValue(IType type) : base(OpCode.DefaultValue)
		{
			this.type = type;
		}
		readonly IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type { get { return type; } }
		public override StackType ResultType { get { return type.GetStackType(); } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, type);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDefaultValue(this);
		}
	}

	/// <summary>Throws an exception.</summary>
	public sealed partial class Throw : UnaryInstruction
	{
		public Throw(ILInstruction argument) : base(OpCode.Throw, argument)
		{
		}
		public override StackType ResultType { get { return StackType.Void; } }
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.EndPointUnreachable;
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitThrow(this);
		}
	}

	/// <summary>Rethrows the current exception.</summary>
	public sealed partial class Rethrow : SimpleInstruction
	{
		public Rethrow() : base(OpCode.Rethrow)
		{
		}
		public override StackType ResultType { get { return StackType.Void; } }
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.MayThrow | InstructionFlags.EndPointUnreachable;
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitRethrow(this);
		}
	}

	/// <summary>Gets the size of a type in bytes.</summary>
	public sealed partial class SizeOf : SimpleInstruction
	{
		public SizeOf(IType type) : base(OpCode.SizeOf)
		{
			this.type = type;
		}
		readonly IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type { get { return type; } }
		public override StackType ResultType { get { return StackType.I4; } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, type);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitSizeOf(this);
		}
	}

	/// <summary>Returns the length of an array as 'native unsigned int'.</summary>
	public sealed partial class LdLen : ILInstruction
	{
		public LdLen(ILInstruction target) : base(OpCode.LdLen)
		{
			this.Target = target;
		}
		ILInstruction target;
		public ILInstruction Target {
			get { return this.target; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.target, value);
			}
		}
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return this.target;
			}
		}
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Target = this.target.AcceptVisitor(visitor);
		}
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			this.Target = this.target.Inline(flagsBefore, context);
			return this;
		}
		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
			Target.TransformStackIntoVariables(state);
		}
		public override StackType ResultType { get { return StackType.I; } }
		protected override InstructionFlags ComputeFlags()
		{
			return target.Flags | InstructionFlags.MayThrow;
		}
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write('(');
			this.target.WriteTo(output);
			output.Write(')');
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdLen(this);
		}
	}

	/// <summary>Load address of array element.</summary>
	public sealed partial class LdElema : ILInstruction
	{
		public LdElema(ILInstruction array, ILInstruction index, IType type) : base(OpCode.LdElema)
		{
			this.Array = array;
			this.Index = index;
			this.type = type;
		}
		ILInstruction array;
		public ILInstruction Array {
			get { return this.array; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.array, value);
			}
		}
		ILInstruction index;
		public ILInstruction Index {
			get { return this.index; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref this.index, value);
			}
		}
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return this.array;
				yield return this.index;
			}
		}
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Array = this.array.AcceptVisitor(visitor);
			this.Index = this.index.AcceptVisitor(visitor);
		}
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			this.Index = this.index.Inline(flagsBefore | ((this.array.Flags) & ~(InstructionFlags.MayPeek | InstructionFlags.MayPop)), context);
			this.Array = this.array.Inline(flagsBefore, context);
			return this;
		}
		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
			Array.TransformStackIntoVariables(state);
			Index.TransformStackIntoVariables(state);
		}
		readonly IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type { get { return type; } }
		public override StackType ResultType { get { return StackType.Ref; } }
		/// <summary>Gets whether the 'readonly' prefix was applied to this instruction.</summary>
		public bool IsReadOnly { get; set; }
		protected override InstructionFlags ComputeFlags()
		{
			return array.Flags | index.Flags | InstructionFlags.MayThrow;
		}
		public override void WriteTo(ITextOutput output)
		{
			if (IsReadOnly)
				output.Write("readonly.");
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, type);
			output.Write('(');
			this.array.WriteTo(output);
			output.Write(", ");
			this.index.WriteTo(output);
			output.Write(')');
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdElema(this);
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
		protected internal virtual T VisitILFunction(ILFunction function)
		{
			return Default(function);
		}
		protected internal virtual T VisitBlockContainer(BlockContainer container)
		{
			return Default(container);
		}
		protected internal virtual T VisitBlock(Block block)
		{
			return Default(block);
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
		protected internal virtual T VisitEndFinally(EndFinally inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitIfInstruction(IfInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLoop(Loop inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitTryCatch(TryCatch inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitTryCatchHandler(TryCatchHandler inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitTryFinally(TryFinally inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitTryFault(TryFault inst)
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
		protected internal virtual T VisitLdFtn(LdFtn inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdVirtFtn(LdVirtFtn inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdToken(LdToken inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLocAlloc(LocAlloc inst)
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
		protected internal virtual T VisitCastClass(CastClass inst)
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
		protected internal virtual T VisitStObj(StObj inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitBox(Box inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitUnbox(Unbox inst)
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
		protected internal virtual T VisitInitObj(InitObj inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitDefaultValue(DefaultValue inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitThrow(Throw inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitRethrow(Rethrow inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitSizeOf(SizeOf inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdLen(LdLen inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdElema(LdElema inst)
		{
			return Default(inst);
		}
	}
	
	partial class BinaryNumericInstruction
	{
		public static BinaryNumericInstruction Create(OpCode opCode, ILInstruction left, ILInstruction right, bool checkForOverflow, Sign sign, CompoundAssignmentType compoundAssignmentType = CompoundAssignmentType.None)
		{
			switch (opCode) {
				case OpCode.Add:
					return new Add(left, right, checkForOverflow, sign, compoundAssignmentType);
				case OpCode.Sub:
					return new Sub(left, right, checkForOverflow, sign, compoundAssignmentType);
				case OpCode.Mul:
					return new Mul(left, right, checkForOverflow, sign, compoundAssignmentType);
				case OpCode.Div:
					return new Div(left, right, checkForOverflow, sign, compoundAssignmentType);
				case OpCode.Rem:
					return new Rem(left, right, checkForOverflow, sign, compoundAssignmentType);
				case OpCode.BitAnd:
					return new BitAnd(left, right, checkForOverflow, sign, compoundAssignmentType);
				case OpCode.BitOr:
					return new BitOr(left, right, checkForOverflow, sign, compoundAssignmentType);
				case OpCode.BitXor:
					return new BitXor(left, right, checkForOverflow, sign, compoundAssignmentType);
				case OpCode.Shl:
					return new Shl(left, right, checkForOverflow, sign, compoundAssignmentType);
				case OpCode.Shr:
					return new Shr(left, right, checkForOverflow, sign, compoundAssignmentType);
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
			"ILFunction",
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
			"endfinally",
			"if",
			"loop",
			"try.catch",
			"try.catch.handler",
			"try.finally",
			"try.fault",
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
			"ldftn",
			"ldvirtftn",
			"ldtoken",
			"localloc",
			"ret",
			"shl",
			"shr",
			"ldfld",
			"ldflda",
			"stfld",
			"ldsfld",
			"ldsflda",
			"stsfld",
			"castclass",
			"isinst",
			"ldobj",
			"stobj",
			"box",
			"unbox",
			"unbox.any",
			"newobj",
			"initobj",
			"default.value",
			"throw",
			"rethrow",
			"sizeof",
			"ldlen",
			"ldelema",
		};
	}
}

