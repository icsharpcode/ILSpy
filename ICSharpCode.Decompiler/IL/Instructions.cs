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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Enum representing the type of an <see cref="ILInstruction"/>.
	/// </summary>
	public enum OpCode
	{
		/// <summary>Represents invalid IL. Semantically, this instruction is considered to throw some kind of exception.</summary>
		InvalidInstruction,
		/// <summary>No operation. Takes 0 arguments and returns void.</summary>
		Nop,
		/// <summary>A container of IL blocks.</summary>
		ILFunction,
		/// <summary>A container of IL blocks.</summary>
		BlockContainer,
		/// <summary>A block of IL instructions.</summary>
		Block,
		/// <summary>A region where a pinned variable is used (initial representation of future fixed statement)</summary>
		PinnedRegion,
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
		/// <summary>Unconditional branch to end of block container. <c>goto container_end;</c>, often <c>break;</c></summary>
		Leave,
		/// <summary>If statement / conditional expression. <c>if (condition) trueExpr else falseExpr</c></summary>
		IfInstruction,
		/// <summary>Switch statement</summary>
		SwitchInstruction,
		/// <summary>Switch section within a switch statement</summary>
		SwitchSection,
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
		/// <summary>Comparison. The inputs must be both integers; or both floats; or both object references. Object references can only be compared for equality or inequality. Floating-point comparisons evaluate to 0 (false) when an input is NaN, except for 'NaN != NaN' which evaluates to 1 (true).</summary>
		Comp,
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
		/// <summary>Stores the value into an anonymous temporary variable, and returns the address of that variable.</summary>
		AddressOf,
		/// <summary>Loads a constant string.</summary>
		LdStr,
		/// <summary>Loads a constant 32-bit integer.</summary>
		LdcI4,
		/// <summary>Loads a constant 64-bit integer.</summary>
		LdcI8,
		/// <summary>Loads a constant floating-point number.</summary>
		LdcF,
		/// <summary>Loads a constant decimal.</summary>
		LdcDecimal,
		/// <summary>Loads the null reference.</summary>
		LdNull,
		/// <summary>Load method pointer</summary>
		LdFtn,
		/// <summary>Load method pointer</summary>
		LdVirtFtn,
		/// <summary>Loads runtime representation of metadata token</summary>
		LdTypeToken,
		/// <summary>Loads runtime representation of metadata token</summary>
		LdMemberToken,
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
		/// <summary>Creates an array instance.</summary>
		NewArr,
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
		/// <summary>Converts an array pointer (O) to a reference to the first element, or to a null reference if the array is null or empty.</summary>
		ArrayToPointer,
		/// <summary>Push a typed reference of type class onto the stack.</summary>
		MakeRefAny,
		/// <summary>Push the type token stored in a typed reference.</summary>
		RefAnyType,
		/// <summary>Push the address stored in a typed reference.</summary>
		RefAnyValue,
	}

	/// <summary>Instruction without any arguments</summary>
	public abstract partial class SimpleInstruction : ILInstruction
	{
		protected SimpleInstruction(OpCode opCode) : base(opCode)
		{
		}
		protected sealed override int GetChildCount()
		{
			return 0;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (SimpleInstruction)ShallowClone();
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.None;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.None;
			}
		}
	}

	/// <summary>Instruction with a single argument</summary>
	public abstract partial class UnaryInstruction : ILInstruction
	{
		protected UnaryInstruction(OpCode opCode, ILInstruction argument) : base(opCode)
		{
			this.Argument = argument;
		}
		public static readonly SlotInfo ArgumentSlot = new SlotInfo("Argument", canInlineInto: true);
		ILInstruction argument;
		public ILInstruction Argument {
			get { return this.argument; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.argument, value, 0);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 1;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.argument;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Argument = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return ArgumentSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (UnaryInstruction)ShallowClone();
			clone.Argument = this.argument.Clone();
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return argument.Flags | InstructionFlags.None;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.None;
			}
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
		public static readonly SlotInfo LeftSlot = new SlotInfo("Left", canInlineInto: true);
		ILInstruction left;
		public ILInstruction Left {
			get { return this.left; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.left, value, 0);
			}
		}
		public static readonly SlotInfo RightSlot = new SlotInfo("Right", canInlineInto: true);
		ILInstruction right;
		public ILInstruction Right {
			get { return this.right; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.right, value, 1);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 2;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.left;
				case 1:
					return this.right;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Left = value;
					break;
				case 1:
					this.Right = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return LeftSlot;
				case 1:
					return RightSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (BinaryInstruction)ShallowClone();
			clone.Left = this.left.Clone();
			clone.Right = this.right.Clone();
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return left.Flags | right.Flags | InstructionFlags.None;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.None;
			}
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

	/// <summary>Instruction with a list of arguments.</summary>
	public abstract partial class CallInstruction : ILInstruction
	{
		protected CallInstruction(OpCode opCode, params ILInstruction[] arguments) : base(opCode)
		{
			this.Arguments = new InstructionCollection<ILInstruction>(this, 0);
			this.Arguments.AddRange(arguments);
		}
		public static readonly SlotInfo ArgumentsSlot = new SlotInfo("Arguments", canInlineInto: true);
		public InstructionCollection<ILInstruction> Arguments { get; private set; }
		protected sealed override int GetChildCount()
		{
			return Arguments.Count;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				default:
					return this.Arguments[index - 0];
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				default:
					this.Arguments[index - 0] = value;
					break;
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				default:
					return ArgumentsSlot;
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (CallInstruction)ShallowClone();
			clone.Arguments = new InstructionCollection<ILInstruction>(clone, 0);
			clone.Arguments.AddRange(this.Arguments.Select(arg => arg.Clone()));
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return Arguments.Aggregate(InstructionFlags.None, (f, arg) => f | arg.Flags) | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
	}

	/// <summary>Represents invalid IL. Semantically, this instruction is considered to throw some kind of exception.</summary>
	public sealed partial class InvalidInstruction : SimpleInstruction
	{
		public InvalidInstruction() : base(OpCode.InvalidInstruction)
		{
		}
		public override StackType ResultType { get { return StackType.Void; } }
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.MayThrow | InstructionFlags.EndPointUnreachable;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayThrow | InstructionFlags.EndPointUnreachable;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitInvalidInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitInvalidInstruction(this);
		}
	}

	/// <summary>No operation. Takes 0 arguments and returns void.</summary>
	public sealed partial class Nop : SimpleInstruction
	{
		public Nop() : base(OpCode.Nop)
		{
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitNop(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitNop(this);
		}
	}

	/// <summary>A container of IL blocks.</summary>
	public sealed partial class ILFunction : ILVariableScope
	{
		public static readonly SlotInfo BodySlot = new SlotInfo("Body");
		ILInstruction body;
		public ILInstruction Body {
			get { return this.body; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.body, value, 0);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 1;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.body;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Body = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return BodySlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (ILFunction)ShallowClone();
			clone.Body = this.body.Clone();
			return clone;
		}
		public override StackType ResultType { get { return StackType.O; } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitILFunction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitILFunction(this);
		}
	}

	/// <summary>A container of IL blocks.</summary>
	public sealed partial class BlockContainer : ILInstruction
	{
		public override StackType ResultType { get { return StackType.Void; } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitBlockContainer(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBlockContainer(this);
		}
	}

	/// <summary>A block of IL instructions.</summary>
	public sealed partial class Block : ILInstruction
	{

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitBlock(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBlock(this);
		}
	}

	/// <summary>A region where a pinned variable is used (initial representation of future fixed statement)</summary>
	public sealed partial class PinnedRegion : ILInstruction
	{
		public PinnedRegion(ILInstruction pin, ILInstruction body) : base(OpCode.PinnedRegion)
		{
			this.Pin = pin;
			this.Body = body;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public static readonly SlotInfo PinSlot = new SlotInfo("Pin", canInlineInto: true);
		ILInstruction pin;
		public ILInstruction Pin {
			get { return this.pin; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.pin, value, 0);
			}
		}
		public static readonly SlotInfo BodySlot = new SlotInfo("Body");
		ILInstruction body;
		public ILInstruction Body {
			get { return this.body; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.body, value, 1);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 2;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.pin;
				case 1:
					return this.body;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Pin = value;
					break;
				case 1:
					this.Body = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return PinSlot;
				case 1:
					return BodySlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (PinnedRegion)ShallowClone();
			clone.Pin = this.pin.Clone();
			clone.Body = this.body.Clone();
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return pin.Flags | body.Flags;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.None;
			}
		}
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write('(');
			this.pin.WriteTo(output);
			output.Write(", ");
			this.body.WriteTo(output);
			output.Write(')');
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitPinnedRegion(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitPinnedRegion(this);
		}
	}

	/// <summary>Unary operator that expects an input of type I4. Returns 1 (of type I4) if the input value is 0. Otherwise, returns 0 (of type I4).</summary>
	public sealed partial class LogicNot : UnaryInstruction
	{
		public LogicNot(ILInstruction argument) : base(OpCode.LogicNot, argument)
		{
		}
		public override StackType ResultType { get { return StackType.I4; } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLogicNot(this);
		}
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

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitAdd(this);
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

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitSub(this);
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

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitMul(this);
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
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDiv(this);
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
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitRem(this);
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

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitBitAnd(this);
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

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitBitOr(this);
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

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitBitXor(this);
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

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitBitNot(this);
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitArglist(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitArglist(this);
		}
	}

	/// <summary>Unconditional branch. <c>goto target;</c></summary>
	public sealed partial class Branch : SimpleInstruction
	{
		public override StackType ResultType { get { return StackType.Void; } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitBranch(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBranch(this);
		}
	}

	/// <summary>Unconditional branch to end of block container. <c>goto container_end;</c>, often <c>break;</c></summary>
	public sealed partial class Leave : SimpleInstruction
	{
		public override StackType ResultType { get { return StackType.Void; } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLeave(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLeave(this);
		}
	}

	/// <summary>If statement / conditional expression. <c>if (condition) trueExpr else falseExpr</c></summary>
	public sealed partial class IfInstruction : ILInstruction
	{
		public static readonly SlotInfo ConditionSlot = new SlotInfo("Condition", canInlineInto: true);
		ILInstruction condition;
		public ILInstruction Condition {
			get { return this.condition; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.condition, value, 0);
			}
		}
		public static readonly SlotInfo TrueInstSlot = new SlotInfo("TrueInst");
		ILInstruction trueInst;
		public ILInstruction TrueInst {
			get { return this.trueInst; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.trueInst, value, 1);
			}
		}
		public static readonly SlotInfo FalseInstSlot = new SlotInfo("FalseInst");
		ILInstruction falseInst;
		public ILInstruction FalseInst {
			get { return this.falseInst; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.falseInst, value, 2);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 3;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.condition;
				case 1:
					return this.trueInst;
				case 2:
					return this.falseInst;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Condition = value;
					break;
				case 1:
					this.TrueInst = value;
					break;
				case 2:
					this.FalseInst = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return ConditionSlot;
				case 1:
					return TrueInstSlot;
				case 2:
					return FalseInstSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (IfInstruction)ShallowClone();
			clone.Condition = this.condition.Clone();
			clone.TrueInst = this.trueInst.Clone();
			clone.FalseInst = this.falseInst.Clone();
			return clone;
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitIfInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitIfInstruction(this);
		}
	}

	/// <summary>Switch statement</summary>
	public sealed partial class SwitchInstruction : ILInstruction
	{
		public override StackType ResultType { get { return StackType.Void; } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitSwitchInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitSwitchInstruction(this);
		}
	}

	/// <summary>Switch section within a switch statement</summary>
	public sealed partial class SwitchSection : ILInstruction
	{
		public static readonly SlotInfo BodySlot = new SlotInfo("Body");
		ILInstruction body;
		public ILInstruction Body {
			get { return this.body; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.body, value, 0);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 1;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.body;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Body = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return BodySlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (SwitchSection)ShallowClone();
			clone.Body = this.body.Clone();
			return clone;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitSwitchSection(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitSwitchSection(this);
		}
	}

	/// <summary>Try-catch statement.</summary>
	public sealed partial class TryCatch : TryInstruction
	{

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitTryCatch(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitTryCatch(this);
		}
	}

	/// <summary>Catch handler within a try-catch statement.</summary>
	public sealed partial class TryCatchHandler : ILInstruction, IInstructionWithVariableOperand
	{
		public TryCatchHandler(ILInstruction filter, ILInstruction body, ILVariable variable) : base(OpCode.TryCatchHandler)
		{
			this.Filter = filter;
			this.Body = body;
			Debug.Assert(variable != null);
			this.variable = variable;
		}
		public static readonly SlotInfo FilterSlot = new SlotInfo("Filter");
		ILInstruction filter;
		public ILInstruction Filter {
			get { return this.filter; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.filter, value, 0);
			}
		}
		public static readonly SlotInfo BodySlot = new SlotInfo("Body");
		ILInstruction body;
		public ILInstruction Body {
			get { return this.body; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.body, value, 1);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 2;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.filter;
				case 1:
					return this.body;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Filter = value;
					break;
				case 1:
					this.Body = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return FilterSlot;
				case 1:
					return BodySlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (TryCatchHandler)ShallowClone();
			clone.Filter = this.filter.Clone();
			clone.Body = this.body.Clone();
			return clone;
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitTryCatchHandler(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitTryCatchHandler(this);
		}
	}

	/// <summary>Try-finally statement</summary>
	public sealed partial class TryFinally : TryInstruction
	{

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitTryFinally(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitTryFinally(this);
		}
	}

	/// <summary>Try-fault statement</summary>
	public sealed partial class TryFault : TryInstruction
	{

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitTryFault(this);
		}
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
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDebugBreak(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDebugBreak(this);
		}
	}

	/// <summary>Comparison. The inputs must be both integers; or both floats; or both object references. Object references can only be compared for equality or inequality. Floating-point comparisons evaluate to 0 (false) when an input is NaN, except for 'NaN != NaN' which evaluates to 1 (true).</summary>
	public sealed partial class Comp : BinaryInstruction
	{
		public override StackType ResultType { get { return StackType.I4; } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitComp(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitComp(this);
		}
	}

	/// <summary>Non-virtual method call.</summary>
	public sealed partial class Call : CallInstruction
	{
		public Call(IMethod method) : base(OpCode.Call, method)
		{
		}

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitCall(this);
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

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitCallVirt(this);
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
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitCkfinite(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitCkfinite(this);
		}
	}

	/// <summary>Numeric cast.</summary>
	public sealed partial class Conv : UnaryInstruction
	{

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitConv(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitConv(this);
		}
	}

	/// <summary>Loads the value of a local variable. (ldarg/ldloc)</summary>
	public sealed partial class LdLoc : SimpleInstruction, IInstructionWithVariableOperand
	{
		public LdLoc(ILVariable variable) : base(OpCode.LdLoc)
		{
			Debug.Assert(variable != null);
			this.variable = variable;
		}
		public override StackType ResultType { get { return variable.StackType; } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			variable.WriteTo(output);
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdLoc(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdLoc(this);
		}
	}

	/// <summary>Loads the address of a local variable. (ldarga/ldloca)</summary>
	public sealed partial class LdLoca : SimpleInstruction, IInstructionWithVariableOperand
	{
		public LdLoca(ILVariable variable) : base(OpCode.LdLoca)
		{
			Debug.Assert(variable != null);
			this.variable = variable;
		}
		public override StackType ResultType { get { return StackType.Ref; } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			variable.WriteTo(output);
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdLoca(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdLoca(this);
		}
	}

	/// <summary>Stores a value into a local variable. (starg/stloc)</summary>
	public sealed partial class StLoc : ILInstruction, IInstructionWithVariableOperand
	{
		public StLoc(ILVariable variable, ILInstruction value) : base(OpCode.StLoc)
		{
			Debug.Assert(variable != null);
			this.variable = variable;
			this.Value = value;
		}
		public static readonly SlotInfo ValueSlot = new SlotInfo("Value", canInlineInto: true);
		ILInstruction value;
		public ILInstruction Value {
			get { return this.value; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.value, value, 0);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 1;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.value;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Value = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return ValueSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (StLoc)ShallowClone();
			clone.Value = this.value.Clone();
			return clone;
		}
		public override StackType ResultType { get { return variable.StackType; } }
		protected override InstructionFlags ComputeFlags()
		{
			return value.Flags;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.None;
			}
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitStLoc(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitStLoc(this);
		}
	}

	/// <summary>Stores the value into an anonymous temporary variable, and returns the address of that variable.</summary>
	public sealed partial class AddressOf : ILInstruction
	{
		public AddressOf(ILInstruction value) : base(OpCode.AddressOf)
		{
			this.Value = value;
		}
		public static readonly SlotInfo ValueSlot = new SlotInfo("Value", canInlineInto: true);
		ILInstruction value;
		public ILInstruction Value {
			get { return this.value; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.value, value, 0);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 1;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.value;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Value = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return ValueSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (AddressOf)ShallowClone();
			clone.Value = this.value.Clone();
			return clone;
		}
		public override StackType ResultType { get { return StackType.Ref; } }
		protected override InstructionFlags ComputeFlags()
		{
			return value.Flags;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.None;
			}
		}
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write('(');
			this.value.WriteTo(output);
			output.Write(')');
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitAddressOf(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitAddressOf(this);
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdStr(this);
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdcI4(this);
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdcI8(this);
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdcF(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdcF(this);
		}
	}

	/// <summary>Loads a constant decimal.</summary>
	public sealed partial class LdcDecimal : SimpleInstruction
	{
		public LdcDecimal(decimal value) : base(OpCode.LdcDecimal)
		{
			this.Value = value;
		}
		public readonly decimal Value;
		public override StackType ResultType { get { return StackType.O; } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdcDecimal(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdcDecimal(this);
		}
	}

	/// <summary>Loads the null reference.</summary>
	public sealed partial class LdNull : SimpleInstruction
	{
		public LdNull() : base(OpCode.LdNull)
		{
		}
		public override StackType ResultType { get { return StackType.O; } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdNull(this);
		}
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdFtn(this);
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
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow;
			}
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdVirtFtn(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdVirtFtn(this);
		}
	}

	/// <summary>Loads runtime representation of metadata token</summary>
	public sealed partial class LdTypeToken : SimpleInstruction
	{
		public LdTypeToken(IType type) : base(OpCode.LdTypeToken)
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
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdTypeToken(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdTypeToken(this);
		}
	}

	/// <summary>Loads runtime representation of metadata token</summary>
	public sealed partial class LdMemberToken : SimpleInstruction
	{
		public LdMemberToken(IMember member) : base(OpCode.LdMemberToken)
		{
			this.member = member;
		}
		readonly IMember member;
		/// <summary>Returns the token operand.</summary>
		public IMember Member { get { return member; } }
		public override StackType ResultType { get { return StackType.O; } }
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, member);
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdMemberToken(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdMemberToken(this);
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
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLocAlloc(this);
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitReturn(this);
		}
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

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitShl(this);
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

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitShr(this);
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
		public static readonly SlotInfo TargetSlot = new SlotInfo("Target", canInlineInto: true);
		ILInstruction target;
		public ILInstruction Target {
			get { return this.target; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.target, value, 0);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 1;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.target;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Target = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return TargetSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (LdFld)ShallowClone();
			clone.Target = this.target.Clone();
			return clone;
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
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.SideEffect | InstructionFlags.MayThrow;
			}
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdFld(this);
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
		public static readonly SlotInfo TargetSlot = new SlotInfo("Target", canInlineInto: true);
		ILInstruction target;
		public ILInstruction Target {
			get { return this.target; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.target, value, 0);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 1;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.target;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Target = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return TargetSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (LdFlda)ShallowClone();
			clone.Target = this.target.Clone();
			return clone;
		}
		readonly IField field;
		/// <summary>Returns the field operand.</summary>
		public IField Field { get { return field; } }
		public override StackType ResultType { get { return StackType.Ref; } }
		protected override InstructionFlags ComputeFlags()
		{
			return target.Flags | InstructionFlags.MayThrow;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayThrow;
			}
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdFlda(this);
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
		public static readonly SlotInfo TargetSlot = new SlotInfo("Target", canInlineInto: true);
		ILInstruction target;
		public ILInstruction Target {
			get { return this.target; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.target, value, 0);
			}
		}
		public static readonly SlotInfo ValueSlot = new SlotInfo("Value", canInlineInto: true);
		ILInstruction value;
		public ILInstruction Value {
			get { return this.value; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.value, value, 1);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 2;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.target;
				case 1:
					return this.value;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Target = value;
					break;
				case 1:
					this.Value = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return TargetSlot;
				case 1:
					return ValueSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (StFld)ShallowClone();
			clone.Target = this.target.Clone();
			clone.Value = this.value.Clone();
			return clone;
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
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.SideEffect | InstructionFlags.MayThrow;
			}
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitStFld(this);
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
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.SideEffect;
			}
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdsFld(this);
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdsFlda(this);
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
		public static readonly SlotInfo ValueSlot = new SlotInfo("Value", canInlineInto: true);
		ILInstruction value;
		public ILInstruction Value {
			get { return this.value; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.value, value, 0);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 1;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.value;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Value = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return ValueSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (StsFld)ShallowClone();
			clone.Value = this.value.Clone();
			return clone;
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
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.SideEffect;
			}
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitStsFld(this);
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
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow;
			}
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitCastClass(this);
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitIsInst(this);
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
		public static readonly SlotInfo TargetSlot = new SlotInfo("Target", canInlineInto: true);
		ILInstruction target;
		public ILInstruction Target {
			get { return this.target; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.target, value, 0);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 1;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.target;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Target = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return TargetSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (LdObj)ShallowClone();
			clone.Target = this.target.Clone();
			return clone;
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
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.SideEffect | InstructionFlags.MayThrow;
			}
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdObj(this);
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
		public static readonly SlotInfo TargetSlot = new SlotInfo("Target", canInlineInto: true);
		ILInstruction target;
		public ILInstruction Target {
			get { return this.target; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.target, value, 0);
			}
		}
		public static readonly SlotInfo ValueSlot = new SlotInfo("Value", canInlineInto: true);
		ILInstruction value;
		public ILInstruction Value {
			get { return this.value; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.value, value, 1);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 2;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.target;
				case 1:
					return this.value;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Target = value;
					break;
				case 1:
					this.Value = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return TargetSlot;
				case 1:
					return ValueSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (StObj)ShallowClone();
			clone.Target = this.target.Clone();
			clone.Value = this.value.Clone();
			return clone;
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
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.SideEffect | InstructionFlags.MayThrow;
			}
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitStObj(this);
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
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
			}
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitBox(this);
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
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow;
			}
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitUnbox(this);
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
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
			}
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitUnboxAny(this);
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
		public override StackType ResultType { get { return Method.DeclaringType.GetStackType(); } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitNewObj(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitNewObj(this);
		}
	}

	/// <summary>Creates an array instance.</summary>
	public sealed partial class NewArr : ILInstruction
	{
		public NewArr(IType type, params ILInstruction[] indices) : base(OpCode.NewArr)
		{
			this.type = type;
			this.Indices = new InstructionCollection<ILInstruction>(this, 0);
			this.Indices.AddRange(indices);
		}
		readonly IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type { get { return type; } }
		public static readonly SlotInfo IndicesSlot = new SlotInfo("Indices", canInlineInto: true);
		public InstructionCollection<ILInstruction> Indices { get; private set; }
		protected sealed override int GetChildCount()
		{
			return Indices.Count;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				default:
					return this.Indices[index - 0];
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				default:
					this.Indices[index - 0] = value;
					break;
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				default:
					return IndicesSlot;
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (NewArr)ShallowClone();
			clone.Indices = new InstructionCollection<ILInstruction>(clone, 0);
			clone.Indices.AddRange(this.Indices.Select(arg => arg.Clone()));
			return clone;
		}
		public override StackType ResultType { get { return StackType.O; } }
		protected override InstructionFlags ComputeFlags()
		{
			return Indices.Aggregate(InstructionFlags.None, (f, arg) => f | arg.Flags) | InstructionFlags.MayThrow;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayThrow;
			}
		}
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, type);
			output.Write('(');
			bool first = true;
			foreach (var indices in Indices) {
				if (!first) output.Write(", "); else first = false;
				indices.WriteTo(output);
			}
			output.Write(')');
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitNewArr(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitNewArr(this);
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDefaultValue(this);
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
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.EndPointUnreachable;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitThrow(this);
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
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayThrow | InstructionFlags.EndPointUnreachable;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitRethrow(this);
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitSizeOf(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitSizeOf(this);
		}
	}

	/// <summary>Returns the length of an array as 'native unsigned int'.</summary>
	public sealed partial class LdLen : ILInstruction
	{
		public static readonly SlotInfo ArraySlot = new SlotInfo("Array", canInlineInto: true);
		ILInstruction array;
		public ILInstruction Array {
			get { return this.array; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.array, value, 0);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 1;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.array;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Array = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return ArraySlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (LdLen)ShallowClone();
			clone.Array = this.array.Clone();
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return array.Flags | InstructionFlags.MayThrow;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayThrow;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdLen(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdLen(this);
		}
	}

	/// <summary>Load address of array element.</summary>
	public sealed partial class LdElema : ILInstruction
	{
		public LdElema(IType type, ILInstruction array, params ILInstruction[] indices) : base(OpCode.LdElema)
		{
			this.type = type;
			this.Array = array;
			this.Indices = new InstructionCollection<ILInstruction>(this, 1);
			this.Indices.AddRange(indices);
		}
		readonly IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type { get { return type; } }
		public static readonly SlotInfo ArraySlot = new SlotInfo("Array", canInlineInto: true);
		ILInstruction array;
		public ILInstruction Array {
			get { return this.array; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.array, value, 0);
			}
		}
		public static readonly SlotInfo IndicesSlot = new SlotInfo("Indices", canInlineInto: true);
		public InstructionCollection<ILInstruction> Indices { get; private set; }
		protected sealed override int GetChildCount()
		{
			return 1 + Indices.Count;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.array;
				default:
					return this.Indices[index - 1];
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Array = value;
					break;
				default:
					this.Indices[index - 1] = value;
					break;
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return ArraySlot;
				default:
					return IndicesSlot;
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (LdElema)ShallowClone();
			clone.Array = this.array.Clone();
			clone.Indices = new InstructionCollection<ILInstruction>(clone, 1);
			clone.Indices.AddRange(this.Indices.Select(arg => arg.Clone()));
			return clone;
		}
		public override StackType ResultType { get { return StackType.Ref; } }
		/// <summary>Gets whether the 'readonly' prefix was applied to this instruction.</summary>
		public bool IsReadOnly { get; set; }
		protected override InstructionFlags ComputeFlags()
		{
			return array.Flags | Indices.Aggregate(InstructionFlags.None, (f, arg) => f | arg.Flags) | InstructionFlags.MayThrow;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayThrow;
			}
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
			foreach (var indices in Indices) {
				output.Write(", ");
				indices.WriteTo(output);
			}
			output.Write(')');
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdElema(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdElema(this);
		}
	}

	/// <summary>Converts an array pointer (O) to a reference to the first element, or to a null reference if the array is null or empty.</summary>
	public sealed partial class ArrayToPointer : ILInstruction
	{
		public ArrayToPointer(ILInstruction array) : base(OpCode.ArrayToPointer)
		{
			this.Array = array;
		}
		public static readonly SlotInfo ArraySlot = new SlotInfo("Array", canInlineInto: true);
		ILInstruction array;
		public ILInstruction Array {
			get { return this.array; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.array, value, 0);
			}
		}
		protected sealed override int GetChildCount()
		{
			return 1;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.array;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Array = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return ArraySlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (ArrayToPointer)ShallowClone();
			clone.Array = this.array.Clone();
			return clone;
		}
		public override StackType ResultType { get { return StackType.Ref; } }
		protected override InstructionFlags ComputeFlags()
		{
			return array.Flags;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.None;
			}
		}
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write('(');
			this.array.WriteTo(output);
			output.Write(')');
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitArrayToPointer(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitArrayToPointer(this);
		}
	}

	/// <summary>Push a typed reference of type class onto the stack.</summary>
	public sealed partial class MakeRefAny : UnaryInstruction
	{
		public MakeRefAny(ILInstruction argument, IType type) : base(OpCode.MakeRefAny, argument)
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitMakeRefAny(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitMakeRefAny(this);
		}
	}

	/// <summary>Push the type token stored in a typed reference.</summary>
	public sealed partial class RefAnyType : UnaryInstruction
	{
		public RefAnyType(ILInstruction argument) : base(OpCode.RefAnyType, argument)
		{
		}
		public override StackType ResultType { get { return StackType.O; } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitRefAnyType(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitRefAnyType(this);
		}
	}

	/// <summary>Push the address stored in a typed reference.</summary>
	public sealed partial class RefAnyValue : UnaryInstruction
	{
		public RefAnyValue(ILInstruction argument, IType type) : base(OpCode.RefAnyValue, argument)
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
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow;
			}
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitRefAnyValue(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitRefAnyValue(this);
		}
	}


	/// <summary>
	/// Base class for visitor pattern.
	/// </summary>
	public abstract class ILVisitor
	{
		/// <summary>Called by Visit*() methods that were not overridden</summary>
		protected abstract void Default(ILInstruction inst);
		
		protected internal virtual void VisitInvalidInstruction(InvalidInstruction inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitNop(Nop inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitILFunction(ILFunction function)
		{
			Default(function);
		}
		protected internal virtual void VisitBlockContainer(BlockContainer container)
		{
			Default(container);
		}
		protected internal virtual void VisitBlock(Block block)
		{
			Default(block);
		}
		protected internal virtual void VisitPinnedRegion(PinnedRegion inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLogicNot(LogicNot inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitAdd(Add inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitSub(Sub inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitMul(Mul inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitDiv(Div inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitRem(Rem inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitBitAnd(BitAnd inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitBitOr(BitOr inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitBitXor(BitXor inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitBitNot(BitNot inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitArglist(Arglist inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitBranch(Branch inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLeave(Leave inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitIfInstruction(IfInstruction inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitSwitchInstruction(SwitchInstruction inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitSwitchSection(SwitchSection inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitTryCatch(TryCatch inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitTryCatchHandler(TryCatchHandler inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitTryFinally(TryFinally inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitTryFault(TryFault inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitDebugBreak(DebugBreak inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitComp(Comp inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitCall(Call inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitCallVirt(CallVirt inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitCkfinite(Ckfinite inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitConv(Conv inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdLoc(LdLoc inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdLoca(LdLoca inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitStLoc(StLoc inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitAddressOf(AddressOf inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdStr(LdStr inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdcI4(LdcI4 inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdcI8(LdcI8 inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdcF(LdcF inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdcDecimal(LdcDecimal inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdNull(LdNull inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdFtn(LdFtn inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdVirtFtn(LdVirtFtn inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdTypeToken(LdTypeToken inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdMemberToken(LdMemberToken inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLocAlloc(LocAlloc inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitReturn(Return inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitShl(Shl inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitShr(Shr inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdFld(LdFld inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdFlda(LdFlda inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitStFld(StFld inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdsFld(LdsFld inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdsFlda(LdsFlda inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitStsFld(StsFld inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitCastClass(CastClass inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitIsInst(IsInst inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdObj(LdObj inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitStObj(StObj inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitBox(Box inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitUnbox(Unbox inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitUnboxAny(UnboxAny inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitNewObj(NewObj inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitNewArr(NewArr inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitDefaultValue(DefaultValue inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitThrow(Throw inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitRethrow(Rethrow inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitSizeOf(SizeOf inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdLen(LdLen inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdElema(LdElema inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitArrayToPointer(ArrayToPointer inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitMakeRefAny(MakeRefAny inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitRefAnyType(RefAnyType inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitRefAnyValue(RefAnyValue inst)
		{
			Default(inst);
		}
	}
	
	/// <summary>
	/// Base class for visitor pattern.
	/// </summary>
	public abstract class ILVisitor<T>
	{
		/// <summary>Called by Visit*() methods that were not overridden</summary>
		protected abstract T Default(ILInstruction inst);
		
		protected internal virtual T VisitInvalidInstruction(InvalidInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitNop(Nop inst)
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
		protected internal virtual T VisitPinnedRegion(PinnedRegion inst)
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
		protected internal virtual T VisitLeave(Leave inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitIfInstruction(IfInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitSwitchInstruction(SwitchInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitSwitchSection(SwitchSection inst)
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
		protected internal virtual T VisitComp(Comp inst)
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
		protected internal virtual T VisitAddressOf(AddressOf inst)
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
		protected internal virtual T VisitLdcDecimal(LdcDecimal inst)
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
		protected internal virtual T VisitLdTypeToken(LdTypeToken inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdMemberToken(LdMemberToken inst)
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
		protected internal virtual T VisitNewArr(NewArr inst)
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
		protected internal virtual T VisitArrayToPointer(ArrayToPointer inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitMakeRefAny(MakeRefAny inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitRefAnyType(RefAnyType inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitRefAnyValue(RefAnyValue inst)
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
				default:
					throw new ArgumentException("opCode is not a binary comparison instruction");
			}
		}
	}
	
	partial class InstructionOutputExtensions
	{
		static readonly string[] originalOpCodeNames = {
			"invalid",
			"nop",
			"ILFunction",
			"BlockContainer",
			"Block",
			"PinnedRegion",
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
			"leave",
			"if",
			"switch",
			"switch.section",
			"try.catch",
			"try.catch.handler",
			"try.finally",
			"try.fault",
			"debug.break",
			"comp",
			"call",
			"callvirt",
			"ckfinite",
			"conv",
			"ldloc",
			"ldloca",
			"stloc",
			"addressof",
			"ldstr",
			"ldc.i4",
			"ldc.i8",
			"ldc.f",
			"ldc.decimal",
			"ldnull",
			"ldftn",
			"ldvirtftn",
			"ldtypetoken",
			"ldmembertoken",
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
			"newarr",
			"default.value",
			"throw",
			"rethrow",
			"sizeof",
			"ldlen",
			"ldelema",
			"array.to.pointer",
			"mkrefany",
			"refanytype",
			"refanyval",
		};
	}
	
	partial class ILInstruction
	{
		public bool MatchInvalidInstruction()
		{
			var inst = this as InvalidInstruction;
			if (inst != null) {
				return true;
			}
			return false;
		}
		public bool MatchNop()
		{
			var inst = this as Nop;
			if (inst != null) {
				return true;
			}
			return false;
		}
		public bool MatchPinnedRegion(out ILInstruction pin, out ILInstruction body)
		{
			var inst = this as PinnedRegion;
			if (inst != null) {
				pin = inst.Pin;
				body = inst.Body;
				return true;
			}
			pin = default(ILInstruction);
			body = default(ILInstruction);
			return false;
		}
		public bool MatchLogicNot(out ILInstruction argument)
		{
			var inst = this as LogicNot;
			if (inst != null) {
				argument = inst.Argument;
				return true;
			}
			argument = default(ILInstruction);
			return false;
		}
		public bool MatchAdd(out ILInstruction left, out ILInstruction right)
		{
			var inst = this as Add;
			if (inst != null) {
				left = inst.Left;
				right = inst.Right;
				return true;
			}
			left = default(ILInstruction);
			right = default(ILInstruction);
			return false;
		}
		public bool MatchSub(out ILInstruction left, out ILInstruction right)
		{
			var inst = this as Sub;
			if (inst != null) {
				left = inst.Left;
				right = inst.Right;
				return true;
			}
			left = default(ILInstruction);
			right = default(ILInstruction);
			return false;
		}
		public bool MatchMul(out ILInstruction left, out ILInstruction right)
		{
			var inst = this as Mul;
			if (inst != null) {
				left = inst.Left;
				right = inst.Right;
				return true;
			}
			left = default(ILInstruction);
			right = default(ILInstruction);
			return false;
		}
		public bool MatchDiv(out ILInstruction left, out ILInstruction right)
		{
			var inst = this as Div;
			if (inst != null) {
				left = inst.Left;
				right = inst.Right;
				return true;
			}
			left = default(ILInstruction);
			right = default(ILInstruction);
			return false;
		}
		public bool MatchRem(out ILInstruction left, out ILInstruction right)
		{
			var inst = this as Rem;
			if (inst != null) {
				left = inst.Left;
				right = inst.Right;
				return true;
			}
			left = default(ILInstruction);
			right = default(ILInstruction);
			return false;
		}
		public bool MatchBitAnd(out ILInstruction left, out ILInstruction right)
		{
			var inst = this as BitAnd;
			if (inst != null) {
				left = inst.Left;
				right = inst.Right;
				return true;
			}
			left = default(ILInstruction);
			right = default(ILInstruction);
			return false;
		}
		public bool MatchBitOr(out ILInstruction left, out ILInstruction right)
		{
			var inst = this as BitOr;
			if (inst != null) {
				left = inst.Left;
				right = inst.Right;
				return true;
			}
			left = default(ILInstruction);
			right = default(ILInstruction);
			return false;
		}
		public bool MatchBitXor(out ILInstruction left, out ILInstruction right)
		{
			var inst = this as BitXor;
			if (inst != null) {
				left = inst.Left;
				right = inst.Right;
				return true;
			}
			left = default(ILInstruction);
			right = default(ILInstruction);
			return false;
		}
		public bool MatchBitNot(out ILInstruction argument)
		{
			var inst = this as BitNot;
			if (inst != null) {
				argument = inst.Argument;
				return true;
			}
			argument = default(ILInstruction);
			return false;
		}
		public bool MatchArglist()
		{
			var inst = this as Arglist;
			if (inst != null) {
				return true;
			}
			return false;
		}
		public bool MatchTryCatchHandler(out ILInstruction filter, out ILInstruction body, out ILVariable variable)
		{
			var inst = this as TryCatchHandler;
			if (inst != null) {
				filter = inst.Filter;
				body = inst.Body;
				variable = inst.Variable;
				return true;
			}
			filter = default(ILInstruction);
			body = default(ILInstruction);
			variable = default(ILVariable);
			return false;
		}
		public bool MatchDebugBreak()
		{
			var inst = this as DebugBreak;
			if (inst != null) {
				return true;
			}
			return false;
		}
		public bool MatchCkfinite(out ILInstruction argument)
		{
			var inst = this as Ckfinite;
			if (inst != null) {
				argument = inst.Argument;
				return true;
			}
			argument = default(ILInstruction);
			return false;
		}
		public bool MatchLdLoc(out ILVariable variable)
		{
			var inst = this as LdLoc;
			if (inst != null) {
				variable = inst.Variable;
				return true;
			}
			variable = default(ILVariable);
			return false;
		}
		public bool MatchLdLoca(out ILVariable variable)
		{
			var inst = this as LdLoca;
			if (inst != null) {
				variable = inst.Variable;
				return true;
			}
			variable = default(ILVariable);
			return false;
		}
		public bool MatchStLoc(out ILVariable variable, out ILInstruction value)
		{
			var inst = this as StLoc;
			if (inst != null) {
				variable = inst.Variable;
				value = inst.Value;
				return true;
			}
			variable = default(ILVariable);
			value = default(ILInstruction);
			return false;
		}
		public bool MatchAddressOf(out ILInstruction value)
		{
			var inst = this as AddressOf;
			if (inst != null) {
				value = inst.Value;
				return true;
			}
			value = default(ILInstruction);
			return false;
		}
		public bool MatchLdStr(out string value)
		{
			var inst = this as LdStr;
			if (inst != null) {
				value = inst.Value;
				return true;
			}
			value = default(string);
			return false;
		}
		public bool MatchLdcI4(out int value)
		{
			var inst = this as LdcI4;
			if (inst != null) {
				value = inst.Value;
				return true;
			}
			value = default(int);
			return false;
		}
		public bool MatchLdcI8(out long value)
		{
			var inst = this as LdcI8;
			if (inst != null) {
				value = inst.Value;
				return true;
			}
			value = default(long);
			return false;
		}
		public bool MatchLdcF(out double value)
		{
			var inst = this as LdcF;
			if (inst != null) {
				value = inst.Value;
				return true;
			}
			value = default(double);
			return false;
		}
		public bool MatchLdcDecimal(out decimal value)
		{
			var inst = this as LdcDecimal;
			if (inst != null) {
				value = inst.Value;
				return true;
			}
			value = default(decimal);
			return false;
		}
		public bool MatchLdNull()
		{
			var inst = this as LdNull;
			if (inst != null) {
				return true;
			}
			return false;
		}
		public bool MatchLdFtn(out IMethod method)
		{
			var inst = this as LdFtn;
			if (inst != null) {
				method = inst.Method;
				return true;
			}
			method = default(IMethod);
			return false;
		}
		public bool MatchLdVirtFtn(out ILInstruction argument, out IMethod method)
		{
			var inst = this as LdVirtFtn;
			if (inst != null) {
				argument = inst.Argument;
				method = inst.Method;
				return true;
			}
			argument = default(ILInstruction);
			method = default(IMethod);
			return false;
		}
		public bool MatchLdTypeToken(out IType type)
		{
			var inst = this as LdTypeToken;
			if (inst != null) {
				type = inst.Type;
				return true;
			}
			type = default(IType);
			return false;
		}
		public bool MatchLdMemberToken(out IMember member)
		{
			var inst = this as LdMemberToken;
			if (inst != null) {
				member = inst.Member;
				return true;
			}
			member = default(IMember);
			return false;
		}
		public bool MatchLocAlloc(out ILInstruction argument)
		{
			var inst = this as LocAlloc;
			if (inst != null) {
				argument = inst.Argument;
				return true;
			}
			argument = default(ILInstruction);
			return false;
		}
		public bool MatchShl(out ILInstruction left, out ILInstruction right)
		{
			var inst = this as Shl;
			if (inst != null) {
				left = inst.Left;
				right = inst.Right;
				return true;
			}
			left = default(ILInstruction);
			right = default(ILInstruction);
			return false;
		}
		public bool MatchShr(out ILInstruction left, out ILInstruction right)
		{
			var inst = this as Shr;
			if (inst != null) {
				left = inst.Left;
				right = inst.Right;
				return true;
			}
			left = default(ILInstruction);
			right = default(ILInstruction);
			return false;
		}
		public bool MatchLdFld(out ILInstruction target, out IField field)
		{
			var inst = this as LdFld;
			if (inst != null) {
				target = inst.Target;
				field = inst.Field;
				return true;
			}
			target = default(ILInstruction);
			field = default(IField);
			return false;
		}
		public bool MatchLdFlda(out ILInstruction target, out IField field)
		{
			var inst = this as LdFlda;
			if (inst != null) {
				target = inst.Target;
				field = inst.Field;
				return true;
			}
			target = default(ILInstruction);
			field = default(IField);
			return false;
		}
		public bool MatchStFld(out ILInstruction target, out ILInstruction value, out IField field)
		{
			var inst = this as StFld;
			if (inst != null) {
				target = inst.Target;
				value = inst.Value;
				field = inst.Field;
				return true;
			}
			target = default(ILInstruction);
			value = default(ILInstruction);
			field = default(IField);
			return false;
		}
		public bool MatchLdsFld(out IField field)
		{
			var inst = this as LdsFld;
			if (inst != null) {
				field = inst.Field;
				return true;
			}
			field = default(IField);
			return false;
		}
		public bool MatchLdsFlda(out IField field)
		{
			var inst = this as LdsFlda;
			if (inst != null) {
				field = inst.Field;
				return true;
			}
			field = default(IField);
			return false;
		}
		public bool MatchStsFld(out ILInstruction value, out IField field)
		{
			var inst = this as StsFld;
			if (inst != null) {
				value = inst.Value;
				field = inst.Field;
				return true;
			}
			value = default(ILInstruction);
			field = default(IField);
			return false;
		}
		public bool MatchCastClass(out ILInstruction argument, out IType type)
		{
			var inst = this as CastClass;
			if (inst != null) {
				argument = inst.Argument;
				type = inst.Type;
				return true;
			}
			argument = default(ILInstruction);
			type = default(IType);
			return false;
		}
		public bool MatchIsInst(out ILInstruction argument, out IType type)
		{
			var inst = this as IsInst;
			if (inst != null) {
				argument = inst.Argument;
				type = inst.Type;
				return true;
			}
			argument = default(ILInstruction);
			type = default(IType);
			return false;
		}
		public bool MatchLdObj(out ILInstruction target, out IType type)
		{
			var inst = this as LdObj;
			if (inst != null) {
				target = inst.Target;
				type = inst.Type;
				return true;
			}
			target = default(ILInstruction);
			type = default(IType);
			return false;
		}
		public bool MatchStObj(out ILInstruction target, out ILInstruction value, out IType type)
		{
			var inst = this as StObj;
			if (inst != null) {
				target = inst.Target;
				value = inst.Value;
				type = inst.Type;
				return true;
			}
			target = default(ILInstruction);
			value = default(ILInstruction);
			type = default(IType);
			return false;
		}
		public bool MatchBox(out ILInstruction argument, out IType type)
		{
			var inst = this as Box;
			if (inst != null) {
				argument = inst.Argument;
				type = inst.Type;
				return true;
			}
			argument = default(ILInstruction);
			type = default(IType);
			return false;
		}
		public bool MatchUnbox(out ILInstruction argument, out IType type)
		{
			var inst = this as Unbox;
			if (inst != null) {
				argument = inst.Argument;
				type = inst.Type;
				return true;
			}
			argument = default(ILInstruction);
			type = default(IType);
			return false;
		}
		public bool MatchUnboxAny(out ILInstruction argument, out IType type)
		{
			var inst = this as UnboxAny;
			if (inst != null) {
				argument = inst.Argument;
				type = inst.Type;
				return true;
			}
			argument = default(ILInstruction);
			type = default(IType);
			return false;
		}
		public bool MatchNewArr(out IType type)
		{
			var inst = this as NewArr;
			if (inst != null) {
				type = inst.Type;
				return true;
			}
			type = default(IType);
			return false;
		}
		public bool MatchDefaultValue(out IType type)
		{
			var inst = this as DefaultValue;
			if (inst != null) {
				type = inst.Type;
				return true;
			}
			type = default(IType);
			return false;
		}
		public bool MatchThrow(out ILInstruction argument)
		{
			var inst = this as Throw;
			if (inst != null) {
				argument = inst.Argument;
				return true;
			}
			argument = default(ILInstruction);
			return false;
		}
		public bool MatchRethrow()
		{
			var inst = this as Rethrow;
			if (inst != null) {
				return true;
			}
			return false;
		}
		public bool MatchSizeOf(out IType type)
		{
			var inst = this as SizeOf;
			if (inst != null) {
				type = inst.Type;
				return true;
			}
			type = default(IType);
			return false;
		}
		public bool MatchLdElema(out IType type, out ILInstruction array)
		{
			var inst = this as LdElema;
			if (inst != null) {
				type = inst.Type;
				array = inst.Array;
				return true;
			}
			type = default(IType);
			array = default(ILInstruction);
			return false;
		}
		public bool MatchArrayToPointer(out ILInstruction array)
		{
			var inst = this as ArrayToPointer;
			if (inst != null) {
				array = inst.Array;
				return true;
			}
			array = default(ILInstruction);
			return false;
		}
		public bool MatchMakeRefAny(out ILInstruction argument, out IType type)
		{
			var inst = this as MakeRefAny;
			if (inst != null) {
				argument = inst.Argument;
				type = inst.Type;
				return true;
			}
			argument = default(ILInstruction);
			type = default(IType);
			return false;
		}
		public bool MatchRefAnyType(out ILInstruction argument)
		{
			var inst = this as RefAnyType;
			if (inst != null) {
				argument = inst.Argument;
				return true;
			}
			argument = default(ILInstruction);
			return false;
		}
		public bool MatchRefAnyValue(out ILInstruction argument, out IType type)
		{
			var inst = this as RefAnyValue;
			if (inst != null) {
				argument = inst.Argument;
				type = inst.Type;
				return true;
			}
			argument = default(ILInstruction);
			type = default(IType);
			return false;
		}
	}
}

