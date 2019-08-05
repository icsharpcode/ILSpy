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
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Enum representing the type of an <see cref="ILInstruction"/>.
	/// </summary>
	public enum OpCode : byte
	{
		/// <summary>Represents invalid IL. Semantically, this instruction is considered to throw some kind of exception.</summary>
		InvalidBranch,
		/// <summary>Represents invalid IL. Semantically, this instruction is considered to produce some kind of value.</summary>
		InvalidExpression,
		/// <summary>No operation. Takes 0 arguments and returns void.</summary>
		Nop,
		/// <summary>A container of IL blocks.</summary>
		ILFunction,
		/// <summary>A container of IL blocks.</summary>
		BlockContainer,
		/// <summary>A block of IL instructions.</summary>
		Block,
		/// <summary>A region where a pinned variable is used (initial representation of future fixed statement).</summary>
		PinnedRegion,
		/// <summary>Common instruction for add, sub, mul, div, rem, bit.and, bit.or, bit.xor, shl and shr.</summary>
		BinaryNumericInstruction,
		/// <summary>Common instruction for numeric compound assignments.</summary>
		NumericCompoundAssign,
		/// <summary>Common instruction for user-defined compound assignments.</summary>
		UserDefinedCompoundAssign,
		/// <summary>Common instruction for dynamic compound assignments.</summary>
		DynamicCompoundAssign,
		/// <summary>Bitwise NOT</summary>
		BitNot,
		/// <summary>Retrieves the RuntimeArgumentHandle.</summary>
		Arglist,
		/// <summary>Unconditional branch. <c>goto target;</c></summary>
		Branch,
		/// <summary>Unconditional branch to end of block container. Return is represented using IsLeavingFunction and an (optional) return value. The block container evaluates to the value produced by the argument of the leave instruction.</summary>
		Leave,
		/// <summary>If statement / conditional expression. <c>if (condition) trueExpr else falseExpr</c></summary>
		IfInstruction,
		/// <summary>Null coalescing operator expression. <c>if.notnull(valueInst, fallbackInst)</c></summary>
		NullCoalescingInstruction,
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
		/// <summary>Lock statement</summary>
		LockInstruction,
		/// <summary>Using statement</summary>
		UsingInstruction,
		/// <summary>Breakpoint instruction</summary>
		DebugBreak,
		/// <summary>Comparison. The inputs must be both integers; or both floats; or both object references. Object references can only be compared for equality or inequality. Floating-point comparisons evaluate to 0 (false) when an input is NaN, except for 'NaN != NaN' which evaluates to 1 (true).</summary>
		Comp,
		/// <summary>Non-virtual method call.</summary>
		Call,
		/// <summary>Virtual method call.</summary>
		CallVirt,
		/// <summary>Unsafe function pointer call.</summary>
		CallIndirect,
		/// <summary>Checks that the input float is not NaN or infinite.</summary>
		Ckfinite,
		/// <summary>Numeric cast.</summary>
		Conv,
		/// <summary>Loads the value of a local variable. (ldarg/ldloc)</summary>
		LdLoc,
		/// <summary>Loads the address of a local variable. (ldarga/ldloca)</summary>
		LdLoca,
		/// <summary>Stores a value into a local variable. (IL: starg/stloc)
		/// Evaluates to the value that was stored (for byte/short variables: evaluates to the truncated value, sign/zero extended back to I4 based on variable.Type.GetSign())</summary>
		StLoc,
		/// <summary>Stores the value into an anonymous temporary variable, and returns the address of that variable.</summary>
		AddressOf,
		/// <summary>Three valued logic and. Inputs are of type bool? or I4, output is of type bool?. Unlike logic.and(), does not have short-circuiting behavior.</summary>
		ThreeValuedBoolAnd,
		/// <summary>Three valued logic or. Inputs are of type bool? or I4, output is of type bool?. Unlike logic.or(), does not have short-circuiting behavior.</summary>
		ThreeValuedBoolOr,
		/// <summary>The input operand must be one of:
		///   1. a nullable value type
		///   2. a reference type
		///   3. a managed reference to a type parameter.
		/// If the input is non-null, evaluates to the (unwrapped) input.
		/// If the input is null, jumps to the innermost nullable.rewrap instruction that contains this instruction.
		/// In case 3 (managed reference), the dereferenced value is the input being tested, and the nullable.unwrap instruction returns the managed reference unmodified (if the value is non-null).</summary>
		NullableUnwrap,
		/// <summary>Serves as jump target for the nullable.unwrap instruction.
		/// If the input evaluates normally, evaluates to the input value (wrapped in Nullable&lt;T&gt; if the input is a non-nullable value type).If a nullable.unwrap instruction encounters a null input and jumps to the (endpoint of the) nullable.rewrap instruction,the nullable.rewrap instruction evaluates to null.</summary>
		NullableRewrap,
		/// <summary>Loads a constant string.</summary>
		LdStr,
		/// <summary>Loads a constant 32-bit integer.</summary>
		LdcI4,
		/// <summary>Loads a constant 64-bit integer.</summary>
		LdcI8,
		/// <summary>Loads a constant 32-bit floating-point number.</summary>
		LdcF4,
		/// <summary>Loads a constant 64-bit floating-point number.</summary>
		LdcF8,
		/// <summary>Loads a constant decimal.</summary>
		LdcDecimal,
		/// <summary>Loads the null reference.</summary>
		LdNull,
		/// <summary>Load method pointer</summary>
		LdFtn,
		/// <summary>Load method pointer</summary>
		LdVirtFtn,
		/// <summary>Virtual delegate construction</summary>
		LdVirtDelegate,
		/// <summary>Loads runtime representation of metadata token</summary>
		LdTypeToken,
		/// <summary>Loads runtime representation of metadata token</summary>
		LdMemberToken,
		/// <summary>Allocates space in the stack frame</summary>
		LocAlloc,
		/// <summary>Allocates space in the stack frame and wraps it in a Span</summary>
		LocAllocSpan,
		/// <summary>memcpy(destAddress, sourceAddress, size);</summary>
		Cpblk,
		/// <summary>memset(address, value, size)</summary>
		Initblk,
		/// <summary>Load address of instance field</summary>
		LdFlda,
		/// <summary>Load static field address</summary>
		LdsFlda,
		/// <summary>Casts an object to a class.</summary>
		CastClass,
		/// <summary>Test if object is instance of class or interface.</summary>
		IsInst,
		/// <summary>Indirect load (ref/pointer dereference).</summary>
		LdObj,
		/// <summary>Indirect store (store to ref/pointer).
		/// Evaluates to the value that was stored (when using type byte/short: evaluates to the truncated value, sign/zero extended back to I4 based on type.GetSign())</summary>
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
		/// <summary>Converts an array pointer (O) to a reference to the first element, or to a null reference if the array is null or empty.
		/// Also used to convert a string to a reference to the first character.</summary>
		ArrayToPointer,
		/// <summary>Maps a string value to an integer. This is used in switch(string).</summary>
		StringToInt,
		/// <summary>ILAst representation of Expression.Convert.</summary>
		ExpressionTreeCast,
		/// <summary>Use of user-defined &amp;&amp; or || operator.</summary>
		UserDefinedLogicOperator,
		/// <summary>ILAst representation of a short-circuiting binary operator inside a dynamic expression.</summary>
		DynamicLogicOperatorInstruction,
		/// <summary>ILAst representation of a binary operator inside a dynamic expression (maps to Binder.BinaryOperation).</summary>
		DynamicBinaryOperatorInstruction,
		/// <summary>ILAst representation of a unary operator inside a dynamic expression (maps to Binder.UnaryOperation).</summary>
		DynamicUnaryOperatorInstruction,
		/// <summary>ILAst representation of a cast inside a dynamic expression (maps to Binder.Convert).</summary>
		DynamicConvertInstruction,
		/// <summary>ILAst representation of a property get method call inside a dynamic expression (maps to Binder.GetMember).</summary>
		DynamicGetMemberInstruction,
		/// <summary>ILAst representation of a property set method call inside a dynamic expression (maps to Binder.SetMember).</summary>
		DynamicSetMemberInstruction,
		/// <summary>ILAst representation of an indexer get method call inside a dynamic expression (maps to Binder.GetIndex).</summary>
		DynamicGetIndexInstruction,
		/// <summary>ILAst representation of an indexer set method call inside a dynamic expression (maps to Binder.SetIndex).</summary>
		DynamicSetIndexInstruction,
		/// <summary>ILAst representation of a method call inside a dynamic expression (maps to Binder.InvokeMember).</summary>
		DynamicInvokeMemberInstruction,
		/// <summary>ILAst representation of a constuctor invocation inside a dynamic expression (maps to Binder.InvokeConstructor).</summary>
		DynamicInvokeConstructorInstruction,
		/// <summary>ILAst representation of a delegate invocation inside a dynamic expression (maps to Binder.Invoke).</summary>
		DynamicInvokeInstruction,
		/// <summary>ILAst representation of a call to the Binder.IsEvent method inside a dynamic expression.</summary>
		DynamicIsEventInstruction,
		/// <summary>Push a typed reference of type class onto the stack.</summary>
		MakeRefAny,
		/// <summary>Push the type token stored in a typed reference.</summary>
		RefAnyType,
		/// <summary>Push the address stored in a typed reference.</summary>
		RefAnyValue,
		/// <summary>Yield an element from an iterator.</summary>
		YieldReturn,
		/// <summary>C# await operator.</summary>
		Await,
		/// <summary>Matches any node</summary>
		AnyNode,
	}
}

namespace ICSharpCode.Decompiler.IL
{
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
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write('(');
			this.argument.WriteTo(output, options);
			output.Write(')');
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write('(');
			this.left.WriteTo(output, options);
			output.Write(", ");
			this.right.WriteTo(output, options);
			output.Write(')');
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
					this.Arguments[index - 0] = (ILInstruction)value;
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
			clone.Arguments.AddRange(this.Arguments.Select(arg => (ILInstruction)arg.Clone()));
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
}
namespace ICSharpCode.Decompiler.IL.Patterns
{
	/// <summary>Base class for pattern matching in ILAst.</summary>
	public abstract partial class PatternInstruction : ILInstruction
	{
		protected PatternInstruction(OpCode opCode) : base(opCode)
		{
		}
		public override StackType ResultType { get { return StackType.Unknown; } }
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Common instruction for compound assignments.</summary>
	public abstract partial class CompoundAssignmentInstruction : ILInstruction
	{
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
			var clone = (CompoundAssignmentInstruction)ShallowClone();
			clone.Target = this.target.Clone();
			clone.Value = this.value.Clone();
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return target.Flags | value.Flags;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.None;
			}
		}
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write('(');
			this.target.WriteTo(output, options);
			output.Write(", ");
			this.value.WriteTo(output, options);
			output.Write(')');
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Instruction representing a dynamic call site.</summary>
	public abstract partial class DynamicInstruction : ILInstruction
	{
		protected DynamicInstruction(OpCode opCode) : base(opCode)
		{
		}

		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.MayThrow | InstructionFlags.SideEffect;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Represents invalid IL. Semantically, this instruction is considered to throw some kind of exception.</summary>
	public sealed partial class InvalidBranch : SimpleInstruction
	{
		public InvalidBranch() : base(OpCode.InvalidBranch)
		{
		}

		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.MayThrow | InstructionFlags.SideEffect | InstructionFlags.EndPointUnreachable;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayThrow | InstructionFlags.SideEffect | InstructionFlags.EndPointUnreachable;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitInvalidBranch(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitInvalidBranch(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitInvalidBranch(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as InvalidBranch;
			return o != null;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Represents invalid IL. Semantically, this instruction is considered to produce some kind of value.</summary>
	public sealed partial class InvalidExpression : SimpleInstruction
	{
		public InvalidExpression() : base(OpCode.InvalidExpression)
		{
		}

		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.MayThrow | InstructionFlags.SideEffect;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitInvalidExpression(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitInvalidExpression(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitInvalidExpression(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as InvalidExpression;
			return o != null;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitNop(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as Nop;
			return o != null;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>A container of IL blocks.</summary>
	public sealed partial class ILFunction : ILInstruction
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
		public static readonly SlotInfo LocalFunctionsSlot = new SlotInfo("LocalFunctions");
		public InstructionCollection<ILFunction> LocalFunctions { get; private set; }
		protected sealed override int GetChildCount()
		{
			return 1 + LocalFunctions.Count;
		}
		protected sealed override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return this.body;
				default:
					return this.LocalFunctions[index - 1];
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Body = value;
					break;
				default:
					this.LocalFunctions[index - 1] = (ILFunction)value;
					break;
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return BodySlot;
				default:
					return LocalFunctionsSlot;
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (ILFunction)ShallowClone();
			clone.Body = this.body.Clone();
			clone.LocalFunctions = new InstructionCollection<ILFunction>(clone, 1);
			clone.LocalFunctions.AddRange(this.LocalFunctions.Select(arg => (ILFunction)arg.Clone()));
			clone.CloneVariables();
			return clone;
		}
		public override StackType ResultType { get { return DelegateType?.GetStackType() ?? StackType.O; } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitILFunction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitILFunction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitILFunction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as ILFunction;
			return o != null && this.body.PerformMatch(o.body, ref match) && Patterns.ListMatch.DoMatch(this.LocalFunctions, o.LocalFunctions, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>A container of IL blocks.</summary>
	public sealed partial class BlockContainer : ILInstruction
	{
		public override StackType ResultType { get { return this.ExpectedResultType; } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitBlockContainer(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBlockContainer(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitBlockContainer(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as BlockContainer;
			return o != null && Patterns.ListMatch.DoMatch(this.Blocks, o.Blocks, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitBlock(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as Block;
			return o != null && this.Kind == o.Kind && Patterns.ListMatch.DoMatch(this.Instructions, o.Instructions, ref match) && this.FinalInstruction.PerformMatch(o.FinalInstruction, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>A region where a pinned variable is used (initial representation of future fixed statement).</summary>
	public sealed partial class PinnedRegion : ILInstruction, IStoreInstruction
	{
		public PinnedRegion(ILVariable variable, ILInstruction init, ILInstruction body) : base(OpCode.PinnedRegion)
		{
			Debug.Assert(variable != null);
			this.variable = variable;
			this.Init = init;
			this.Body = body;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		ILVariable variable;
		public ILVariable Variable {
			get { return variable; }
			set {
				Debug.Assert(value != null);
				if (IsConnected)
					variable.RemoveStoreInstruction(this);
				variable = value;
				if (IsConnected)
					variable.AddStoreInstruction(this);
			}
		}
		
		public int IndexInStoreInstructionList { get; set; } = -1;
		
		int IInstructionWithVariableOperand.IndexInVariableInstructionMapping {
			get { return ((IStoreInstruction)this).IndexInStoreInstructionList; }
			set { ((IStoreInstruction)this).IndexInStoreInstructionList = value; }
		}
		
		protected override void Connected()
		{
			base.Connected();
			variable.AddStoreInstruction(this);
		}
		
		protected override void Disconnected()
		{
			variable.RemoveStoreInstruction(this);
			base.Disconnected();
		}
		
		public static readonly SlotInfo InitSlot = new SlotInfo("Init", canInlineInto: true);
		ILInstruction init;
		public ILInstruction Init {
			get { return this.init; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.init, value, 0);
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
					return this.init;
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
					this.Init = value;
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
					return InitSlot;
				case 1:
					return BodySlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (PinnedRegion)ShallowClone();
			clone.Init = this.init.Clone();
			clone.Body = this.body.Clone();
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.MayWriteLocals | init.Flags | body.Flags;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayWriteLocals;
			}
		}
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			variable.WriteTo(output);
			output.Write('(');
			this.init.WriteTo(output, options);
			output.Write(", ");
			this.body.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitPinnedRegion(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as PinnedRegion;
			return o != null && variable == o.variable && this.init.PerformMatch(o.init, ref match) && this.body.PerformMatch(o.body, ref match);
		}
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(phase <= ILPhase.InILReader || this.IsDescendantOf(variable.Function));
			Debug.Assert(phase <= ILPhase.InILReader || variable.Function.Variables[variable.IndexInFunction] == variable);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Common instruction for add, sub, mul, div, rem, bit.and, bit.or, bit.xor, shl and shr.</summary>
	public sealed partial class BinaryNumericInstruction : BinaryInstruction
	{

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitBinaryNumericInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBinaryNumericInstruction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitBinaryNumericInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as BinaryNumericInstruction;
			return o != null && this.Left.PerformMatch(o.Left, ref match) && this.Right.PerformMatch(o.Right, ref match) && CheckForOverflow == o.CheckForOverflow && Sign == o.Sign && Operator == o.Operator && IsLifted == o.IsLifted;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Common instruction for numeric compound assignments.</summary>
	public sealed partial class NumericCompoundAssign : CompoundAssignmentInstruction
	{
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
		public override StackType ResultType { get { return type.GetStackType(); } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitNumericCompoundAssign(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitNumericCompoundAssign(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitNumericCompoundAssign(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as NumericCompoundAssign;
			return o != null && type.Equals(o.type) && CheckForOverflow == o.CheckForOverflow && Sign == o.Sign && Operator == o.Operator && this.EvalMode == o.EvalMode && this.TargetKind == o.TargetKind && Target.PerformMatch(o.Target, ref match) && Value.PerformMatch(o.Value, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Common instruction for user-defined compound assignments.</summary>
	public sealed partial class UserDefinedCompoundAssign : CompoundAssignmentInstruction
	{

		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
		}
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitUserDefinedCompoundAssign(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitUserDefinedCompoundAssign(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitUserDefinedCompoundAssign(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as UserDefinedCompoundAssign;
			return o != null && this.Method.Equals(o.Method) && this.EvalMode == o.EvalMode && this.TargetKind == o.TargetKind && Target.PerformMatch(o.Target, ref match) && Value.PerformMatch(o.Value, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Common instruction for dynamic compound assignments.</summary>
	public sealed partial class DynamicCompoundAssign : CompoundAssignmentInstruction
	{
		public override StackType ResultType { get { return StackType.O; } }
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
		}
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDynamicCompoundAssign(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDynamicCompoundAssign(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitDynamicCompoundAssign(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as DynamicCompoundAssign;
			return o != null && this.EvalMode == o.EvalMode && this.TargetKind == o.TargetKind && Target.PerformMatch(o.Target, ref match) && Value.PerformMatch(o.Value, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Bitwise NOT</summary>
	public sealed partial class BitNot : UnaryInstruction
	{

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitBitNot(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBitNot(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitBitNot(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as BitNot;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match) && IsLifted == o.IsLifted && UnderlyingResultType == o.UnderlyingResultType;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitArglist(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as Arglist;
			return o != null;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Unconditional branch. <c>goto target;</c></summary>
	public sealed partial class Branch : SimpleInstruction
	{
		public override StackType ResultType { get { return StackType.Void; } }
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.EndPointUnreachable | InstructionFlags.MayBranch;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.EndPointUnreachable | InstructionFlags.MayBranch;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitBranch(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitBranch(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitBranch(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as Branch;
			return o != null && this.TargetBlock == o.TargetBlock;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Unconditional branch to end of block container. Return is represented using IsLeavingFunction and an (optional) return value. The block container evaluates to the value produced by the argument of the leave instruction.</summary>
	public sealed partial class Leave : ILInstruction
	{
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
			var clone = (Leave)ShallowClone();
			clone.Value = this.value.Clone();
			return clone;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLeave(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLeave(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLeave(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as Leave;
			return o != null && this.value.PerformMatch(o.value, ref match) && this.TargetContainer == o.TargetContainer;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitIfInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as IfInstruction;
			return o != null && this.condition.PerformMatch(o.condition, ref match) && this.trueInst.PerformMatch(o.trueInst, ref match) && this.falseInst.PerformMatch(o.falseInst, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Null coalescing operator expression. <c>if.notnull(valueInst, fallbackInst)</c></summary>
	public sealed partial class NullCoalescingInstruction : ILInstruction
	{
		public static readonly SlotInfo ValueInstSlot = new SlotInfo("ValueInst", canInlineInto: true);
		ILInstruction valueInst;
		public ILInstruction ValueInst {
			get { return this.valueInst; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.valueInst, value, 0);
			}
		}
		public static readonly SlotInfo FallbackInstSlot = new SlotInfo("FallbackInst");
		ILInstruction fallbackInst;
		public ILInstruction FallbackInst {
			get { return this.fallbackInst; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.fallbackInst, value, 1);
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
					return this.valueInst;
				case 1:
					return this.fallbackInst;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.ValueInst = value;
					break;
				case 1:
					this.FallbackInst = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return ValueInstSlot;
				case 1:
					return FallbackInstSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (NullCoalescingInstruction)ShallowClone();
			clone.ValueInst = this.valueInst.Clone();
			clone.FallbackInst = this.fallbackInst.Clone();
			return clone;
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitNullCoalescingInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitNullCoalescingInstruction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitNullCoalescingInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as NullCoalescingInstruction;
			return o != null && this.valueInst.PerformMatch(o.valueInst, ref match) && this.fallbackInst.PerformMatch(o.fallbackInst, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitSwitchInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as SwitchInstruction;
			return o != null && IsLifted == o.IsLifted && Value.PerformMatch(o.Value, ref match) && Patterns.ListMatch.DoMatch(this.Sections, o.Sections, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitSwitchSection(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as SwitchSection;
			return o != null && this.body.PerformMatch(o.body, ref match) && this.Labels.SetEquals(o.Labels) && this.HasNullLabel == o.HasNullLabel;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitTryCatch(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as TryCatch;
			return o != null && TryBlock.PerformMatch(o.TryBlock, ref match) && Patterns.ListMatch.DoMatch(Handlers, o.Handlers, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Catch handler within a try-catch statement.</summary>
	public sealed partial class TryCatchHandler : ILInstruction, IStoreInstruction
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
		ILVariable variable;
		public ILVariable Variable {
			get { return variable; }
			set {
				Debug.Assert(value != null);
				if (IsConnected)
					variable.RemoveStoreInstruction(this);
				variable = value;
				if (IsConnected)
					variable.AddStoreInstruction(this);
			}
		}
		
		public int IndexInStoreInstructionList { get; set; } = -1;
		
		int IInstructionWithVariableOperand.IndexInVariableInstructionMapping {
			get { return ((IStoreInstruction)this).IndexInStoreInstructionList; }
			set { ((IStoreInstruction)this).IndexInStoreInstructionList = value; }
		}
		
		protected override void Connected()
		{
			base.Connected();
			variable.AddStoreInstruction(this);
		}
		
		protected override void Disconnected()
		{
			variable.RemoveStoreInstruction(this);
			base.Disconnected();
		}
		
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitTryCatchHandler(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitTryCatchHandler(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitTryCatchHandler(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as TryCatchHandler;
			return o != null && this.filter.PerformMatch(o.filter, ref match) && this.body.PerformMatch(o.body, ref match) && variable == o.variable;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitTryFinally(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as TryFinally;
			return o != null && TryBlock.PerformMatch(o.TryBlock, ref match) && finallyBlock.PerformMatch(o.finallyBlock, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitTryFault(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as TryFault;
			return o != null && TryBlock.PerformMatch(o.TryBlock, ref match) && faultBlock.PerformMatch(o.faultBlock, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Lock statement</summary>
	public sealed partial class LockInstruction : ILInstruction
	{
		public LockInstruction(ILInstruction onExpression, ILInstruction body) : base(OpCode.LockInstruction)
		{
			this.OnExpression = onExpression;
			this.Body = body;
		}
		public static readonly SlotInfo OnExpressionSlot = new SlotInfo("OnExpression", canInlineInto: true);
		ILInstruction onExpression;
		public ILInstruction OnExpression {
			get { return this.onExpression; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.onExpression, value, 0);
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
					return this.onExpression;
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
					this.OnExpression = value;
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
					return OnExpressionSlot;
				case 1:
					return BodySlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (LockInstruction)ShallowClone();
			clone.OnExpression = this.onExpression.Clone();
			clone.Body = this.body.Clone();
			return clone;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		protected override InstructionFlags ComputeFlags()
		{
			return onExpression.Flags | body.Flags | InstructionFlags.ControlFlow | InstructionFlags.SideEffect;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.ControlFlow | InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLockInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLockInstruction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLockInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LockInstruction;
			return o != null && this.onExpression.PerformMatch(o.onExpression, ref match) && this.body.PerformMatch(o.body, ref match);
		}
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(onExpression.ResultType == StackType.O);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Using statement</summary>
	public sealed partial class UsingInstruction : ILInstruction, IStoreInstruction
	{
		public UsingInstruction(ILVariable variable, ILInstruction resourceExpression, ILInstruction body) : base(OpCode.UsingInstruction)
		{
			Debug.Assert(variable != null);
			this.variable = variable;
			this.ResourceExpression = resourceExpression;
			this.Body = body;
		}
		ILVariable variable;
		public ILVariable Variable {
			get { return variable; }
			set {
				Debug.Assert(value != null);
				if (IsConnected)
					variable.RemoveStoreInstruction(this);
				variable = value;
				if (IsConnected)
					variable.AddStoreInstruction(this);
			}
		}
		
		public int IndexInStoreInstructionList { get; set; } = -1;
		
		int IInstructionWithVariableOperand.IndexInVariableInstructionMapping {
			get { return ((IStoreInstruction)this).IndexInStoreInstructionList; }
			set { ((IStoreInstruction)this).IndexInStoreInstructionList = value; }
		}
		
		protected override void Connected()
		{
			base.Connected();
			variable.AddStoreInstruction(this);
		}
		
		protected override void Disconnected()
		{
			variable.RemoveStoreInstruction(this);
			base.Disconnected();
		}
		
		public static readonly SlotInfo ResourceExpressionSlot = new SlotInfo("ResourceExpression", canInlineInto: true);
		ILInstruction resourceExpression;
		public ILInstruction ResourceExpression {
			get { return this.resourceExpression; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.resourceExpression, value, 0);
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
					return this.resourceExpression;
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
					this.ResourceExpression = value;
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
					return ResourceExpressionSlot;
				case 1:
					return BodySlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (UsingInstruction)ShallowClone();
			clone.ResourceExpression = this.resourceExpression.Clone();
			clone.Body = this.body.Clone();
			return clone;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.MayWriteLocals | resourceExpression.Flags | body.Flags | InstructionFlags.ControlFlow | InstructionFlags.SideEffect;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayWriteLocals | InstructionFlags.ControlFlow | InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitUsingInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitUsingInstruction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitUsingInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as UsingInstruction;
			return o != null && variable == o.variable && this.resourceExpression.PerformMatch(o.resourceExpression, ref match) && this.body.PerformMatch(o.body, ref match);
		}
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(phase <= ILPhase.InILReader || this.IsDescendantOf(variable.Function));
			Debug.Assert(phase <= ILPhase.InILReader || variable.Function.Variables[variable.IndexInFunction] == variable);
			Debug.Assert(resourceExpression.ResultType == StackType.O);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitDebugBreak(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as DebugBreak;
			return o != null;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Comparison. The inputs must be both integers; or both floats; or both object references. Object references can only be compared for equality or inequality. Floating-point comparisons evaluate to 0 (false) when an input is NaN, except for 'NaN != NaN' which evaluates to 1 (true).</summary>
	public sealed partial class Comp : BinaryInstruction
	{

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitComp(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitComp(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitComp(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as Comp;
			return o != null && this.Left.PerformMatch(o.Left, ref match) && this.Right.PerformMatch(o.Right, ref match) && this.Kind == o.Kind && this.Sign == o.Sign && this.LiftingKind == o.LiftingKind;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitCall(this, context);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitCallVirt(this, context);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Unsafe function pointer call.</summary>
	public sealed partial class CallIndirect : ILInstruction
	{

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitCallIndirect(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitCallIndirect(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitCallIndirect(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as CallIndirect;
			return o != null && EqualSignature(o) && Patterns.ListMatch.DoMatch(this.Arguments, o.Arguments, ref match) && this.FunctionPointer.PerformMatch(o.FunctionPointer, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitCkfinite(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as Ckfinite;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitConv(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as Conv;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match) && CheckForOverflow == o.CheckForOverflow && Kind == o.Kind && InputSign == o.InputSign && TargetType == o.TargetType && IsLifted == o.IsLifted;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Loads the value of a local variable. (ldarg/ldloc)</summary>
	public sealed partial class LdLoc : SimpleInstruction, ILoadInstruction
	{
		public LdLoc(ILVariable variable) : base(OpCode.LdLoc)
		{
			Debug.Assert(variable != null);
			this.variable = variable;
		}
		ILVariable variable;
		public ILVariable Variable {
			get { return variable; }
			set {
				Debug.Assert(value != null);
				if (IsConnected)
					variable.RemoveLoadInstruction(this);
				variable = value;
				if (IsConnected)
					variable.AddLoadInstruction(this);
			}
		}
		
		public int IndexInLoadInstructionList { get; set; } = -1;
		
		int IInstructionWithVariableOperand.IndexInVariableInstructionMapping {
			get { return ((ILoadInstruction)this).IndexInLoadInstructionList; }
			set { ((ILoadInstruction)this).IndexInLoadInstructionList = value; }
		}
		
		protected override void Connected()
		{
			base.Connected();
			variable.AddLoadInstruction(this);
		}
		
		protected override void Disconnected()
		{
			variable.RemoveLoadInstruction(this);
			base.Disconnected();
		}
		
		public override StackType ResultType { get { return variable.StackType; } }
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.MayReadLocals;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayReadLocals;
			}
		}
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdLoc(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdLoc;
			return o != null && variable == o.variable;
		}
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(phase <= ILPhase.InILReader || this.IsDescendantOf(variable.Function));
			Debug.Assert(phase <= ILPhase.InILReader || variable.Function.Variables[variable.IndexInFunction] == variable);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Loads the address of a local variable. (ldarga/ldloca)</summary>
	public sealed partial class LdLoca : SimpleInstruction, IAddressInstruction
	{
		public LdLoca(ILVariable variable) : base(OpCode.LdLoca)
		{
			Debug.Assert(variable != null);
			this.variable = variable;
		}
		public override StackType ResultType { get { return StackType.Ref; } }
		ILVariable variable;
		public ILVariable Variable {
			get { return variable; }
			set {
				Debug.Assert(value != null);
				if (IsConnected)
					variable.RemoveAddressInstruction(this);
				variable = value;
				if (IsConnected)
					variable.AddAddressInstruction(this);
			}
		}
		
		public int IndexInAddressInstructionList { get; set; } = -1;
		
		int IInstructionWithVariableOperand.IndexInVariableInstructionMapping {
			get { return ((IAddressInstruction)this).IndexInAddressInstructionList; }
			set { ((IAddressInstruction)this).IndexInAddressInstructionList = value; }
		}
		
		protected override void Connected()
		{
			base.Connected();
			variable.AddAddressInstruction(this);
		}
		
		protected override void Disconnected()
		{
			variable.RemoveAddressInstruction(this);
			base.Disconnected();
		}
		
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdLoca(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdLoca;
			return o != null && variable == o.variable;
		}
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(phase <= ILPhase.InILReader || this.IsDescendantOf(variable.Function));
			Debug.Assert(phase <= ILPhase.InILReader || variable.Function.Variables[variable.IndexInFunction] == variable);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Stores a value into a local variable. (IL: starg/stloc)
	/// Evaluates to the value that was stored (for byte/short variables: evaluates to the truncated value, sign/zero extended back to I4 based on variable.Type.GetSign())</summary>
	public sealed partial class StLoc : ILInstruction, IStoreInstruction
	{
		public StLoc(ILVariable variable, ILInstruction value) : base(OpCode.StLoc)
		{
			Debug.Assert(variable != null);
			this.variable = variable;
			this.Value = value;
		}
		ILVariable variable;
		public ILVariable Variable {
			get { return variable; }
			set {
				Debug.Assert(value != null);
				if (IsConnected)
					variable.RemoveStoreInstruction(this);
				variable = value;
				if (IsConnected)
					variable.AddStoreInstruction(this);
			}
		}
		
		public int IndexInStoreInstructionList { get; set; } = -1;
		
		int IInstructionWithVariableOperand.IndexInVariableInstructionMapping {
			get { return ((IStoreInstruction)this).IndexInStoreInstructionList; }
			set { ((IStoreInstruction)this).IndexInStoreInstructionList = value; }
		}
		
		protected override void Connected()
		{
			base.Connected();
			variable.AddStoreInstruction(this);
		}
		
		protected override void Disconnected()
		{
			variable.RemoveStoreInstruction(this);
			base.Disconnected();
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
			return InstructionFlags.MayWriteLocals | value.Flags;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayWriteLocals;
			}
		}
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			variable.WriteTo(output);
			output.Write('(');
			this.value.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitStLoc(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as StLoc;
			return o != null && variable == o.variable && this.value.PerformMatch(o.value, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write('(');
			this.value.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitAddressOf(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as AddressOf;
			return o != null && this.value.PerformMatch(o.value, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Three valued logic and. Inputs are of type bool? or I4, output is of type bool?. Unlike logic.and(), does not have short-circuiting behavior.</summary>
	public sealed partial class ThreeValuedBoolAnd : BinaryInstruction
	{
		public ThreeValuedBoolAnd(ILInstruction left, ILInstruction right) : base(OpCode.ThreeValuedBoolAnd, left, right)
		{
		}
		public override StackType ResultType { get { return StackType.O; } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitThreeValuedBoolAnd(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitThreeValuedBoolAnd(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitThreeValuedBoolAnd(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as ThreeValuedBoolAnd;
			return o != null && this.Left.PerformMatch(o.Left, ref match) && this.Right.PerformMatch(o.Right, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Three valued logic or. Inputs are of type bool? or I4, output is of type bool?. Unlike logic.or(), does not have short-circuiting behavior.</summary>
	public sealed partial class ThreeValuedBoolOr : BinaryInstruction
	{
		public ThreeValuedBoolOr(ILInstruction left, ILInstruction right) : base(OpCode.ThreeValuedBoolOr, left, right)
		{
		}
		public override StackType ResultType { get { return StackType.O; } }
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitThreeValuedBoolOr(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitThreeValuedBoolOr(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitThreeValuedBoolOr(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as ThreeValuedBoolOr;
			return o != null && this.Left.PerformMatch(o.Left, ref match) && this.Right.PerformMatch(o.Right, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>The input operand must be one of:
	///   1. a nullable value type
	///   2. a reference type
	///   3. a managed reference to a type parameter.
	/// If the input is non-null, evaluates to the (unwrapped) input.
	/// If the input is null, jumps to the innermost nullable.rewrap instruction that contains this instruction.
	/// In case 3 (managed reference), the dereferenced value is the input being tested, and the nullable.unwrap instruction returns the managed reference unmodified (if the value is non-null).</summary>
	public sealed partial class NullableUnwrap : UnaryInstruction
	{

		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayUnwrapNull;
		}
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayUnwrapNull;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitNullableUnwrap(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitNullableUnwrap(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitNullableUnwrap(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as NullableUnwrap;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Serves as jump target for the nullable.unwrap instruction.
	/// If the input evaluates normally, evaluates to the input value (wrapped in Nullable&lt;T&gt; if the input is a non-nullable value type).If a nullable.unwrap instruction encounters a null input and jumps to the (endpoint of the) nullable.rewrap instruction,the nullable.rewrap instruction evaluates to null.</summary>
	public sealed partial class NullableRewrap : UnaryInstruction
	{
		public NullableRewrap(ILInstruction argument) : base(OpCode.NullableRewrap, argument)
		{
		}

		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitNullableRewrap(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitNullableRewrap(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitNullableRewrap(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as NullableRewrap;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Loads a constant string.</summary>
	public sealed partial class LdStr : SimpleInstruction
	{
		public LdStr(string value) : base(OpCode.LdStr)
		{
			this.Value = value;
		}
		public readonly string Value;
		public override StackType ResultType { get { return StackType.O; } }
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdStr(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdStr;
			return o != null && this.Value == o.Value;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Loads a constant 32-bit integer.</summary>
	public sealed partial class LdcI4 : SimpleInstruction
	{
		public LdcI4(int value) : base(OpCode.LdcI4)
		{
			this.Value = value;
		}
		public readonly int Value;
		public override StackType ResultType { get { return StackType.I4; } }
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdcI4(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdcI4;
			return o != null && this.Value == o.Value;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Loads a constant 64-bit integer.</summary>
	public sealed partial class LdcI8 : SimpleInstruction
	{
		public LdcI8(long value) : base(OpCode.LdcI8)
		{
			this.Value = value;
		}
		public readonly long Value;
		public override StackType ResultType { get { return StackType.I8; } }
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdcI8(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdcI8;
			return o != null && this.Value == o.Value;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Loads a constant 32-bit floating-point number.</summary>
	public sealed partial class LdcF4 : SimpleInstruction
	{
		public LdcF4(float value) : base(OpCode.LdcF4)
		{
			this.Value = value;
		}
		public readonly float Value;
		public override StackType ResultType { get { return StackType.F4; } }
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdcF4(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdcF4(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdcF4(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdcF4;
			return o != null && this.Value == o.Value;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Loads a constant 64-bit floating-point number.</summary>
	public sealed partial class LdcF8 : SimpleInstruction
	{
		public LdcF8(double value) : base(OpCode.LdcF8)
		{
			this.Value = value;
		}
		public readonly double Value;
		public override StackType ResultType { get { return StackType.F8; } }
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdcF8(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdcF8(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdcF8(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdcF8;
			return o != null && this.Value == o.Value;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Loads a constant decimal.</summary>
	public sealed partial class LdcDecimal : SimpleInstruction
	{
		public LdcDecimal(decimal value) : base(OpCode.LdcDecimal)
		{
			this.Value = value;
		}
		public readonly decimal Value;
		public override StackType ResultType { get { return StackType.O; } }
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdcDecimal(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdcDecimal;
			return o != null && this.Value == o.Value;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdNull(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdNull;
			return o != null;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Load method pointer</summary>
	public sealed partial class LdFtn : SimpleInstruction, IInstructionWithMethodOperand
	{
		public LdFtn(IMethod method) : base(OpCode.LdFtn)
		{
			this.method = method;
		}
		readonly IMethod method;
		/// <summary>Returns the method operand.</summary>
		public IMethod Method { get { return method; } }
		public override StackType ResultType { get { return StackType.I; } }
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			method.WriteTo(output);
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdFtn(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdFtn(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdFtn(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdFtn;
			return o != null && method.Equals(o.method);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Load method pointer</summary>
	public sealed partial class LdVirtFtn : UnaryInstruction, IInstructionWithMethodOperand
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
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			method.WriteTo(output);
			output.Write('(');
			Argument.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdVirtFtn(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdVirtFtn;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match) && method.Equals(o.method);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Virtual delegate construction</summary>
	public sealed partial class LdVirtDelegate : UnaryInstruction, IInstructionWithMethodOperand
	{
		public LdVirtDelegate(ILInstruction argument, IType type, IMethod method) : base(OpCode.LdVirtDelegate, argument)
		{
			this.type = type;
			this.method = method;
		}
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
		readonly IMethod method;
		/// <summary>Returns the method operand.</summary>
		public IMethod Method { get { return method; } }
		public override StackType ResultType { get { return StackType.O; } }
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow;
		}
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow;
			}
		}
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			type.WriteTo(output);
			output.Write(' ');
			method.WriteTo(output);
			output.Write('(');
			Argument.WriteTo(output, options);
			output.Write(')');
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdVirtDelegate(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdVirtDelegate(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdVirtDelegate(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdVirtDelegate;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match) && type.Equals(o.type) && method.Equals(o.method);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Loads runtime representation of metadata token</summary>
	public sealed partial class LdTypeToken : SimpleInstruction
	{
		public LdTypeToken(IType type) : base(OpCode.LdTypeToken)
		{
			this.type = type;
		}
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
		public override StackType ResultType { get { return StackType.O; } }
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			type.WriteTo(output);
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdTypeToken(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdTypeToken(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdTypeToken(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdTypeToken;
			return o != null && type.Equals(o.type);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			member.WriteTo(output);
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdMemberToken(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdMemberToken(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdMemberToken(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdMemberToken;
			return o != null && member.Equals(o.member);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLocAlloc(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LocAlloc;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Allocates space in the stack frame and wraps it in a Span</summary>
	public sealed partial class LocAllocSpan : UnaryInstruction
	{
		public LocAllocSpan(ILInstruction argument, IType type) : base(OpCode.LocAllocSpan, argument)
		{
			this.type = type;
		}
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
		public override StackType ResultType { get { return StackType.O; } }
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow;
		}
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow;
			}
		}
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			type.WriteTo(output);
			output.Write('(');
			Argument.WriteTo(output, options);
			output.Write(')');
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLocAllocSpan(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLocAllocSpan(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLocAllocSpan(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LocAllocSpan;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match) && type.Equals(o.type);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>memcpy(destAddress, sourceAddress, size);</summary>
	public sealed partial class Cpblk : ILInstruction, ISupportsVolatilePrefix, ISupportsUnalignedPrefix
	{
		public Cpblk(ILInstruction destAddress, ILInstruction sourceAddress, ILInstruction size) : base(OpCode.Cpblk)
		{
			this.DestAddress = destAddress;
			this.SourceAddress = sourceAddress;
			this.Size = size;
		}
		public static readonly SlotInfo DestAddressSlot = new SlotInfo("DestAddress", canInlineInto: true);
		ILInstruction destAddress;
		public ILInstruction DestAddress {
			get { return this.destAddress; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.destAddress, value, 0);
			}
		}
		public static readonly SlotInfo SourceAddressSlot = new SlotInfo("SourceAddress", canInlineInto: true);
		ILInstruction sourceAddress;
		public ILInstruction SourceAddress {
			get { return this.sourceAddress; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.sourceAddress, value, 1);
			}
		}
		public static readonly SlotInfo SizeSlot = new SlotInfo("Size", canInlineInto: true);
		ILInstruction size;
		public ILInstruction Size {
			get { return this.size; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.size, value, 2);
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
					return this.destAddress;
				case 1:
					return this.sourceAddress;
				case 2:
					return this.size;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.DestAddress = value;
					break;
				case 1:
					this.SourceAddress = value;
					break;
				case 2:
					this.Size = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return DestAddressSlot;
				case 1:
					return SourceAddressSlot;
				case 2:
					return SizeSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (Cpblk)ShallowClone();
			clone.DestAddress = this.destAddress.Clone();
			clone.SourceAddress = this.sourceAddress.Clone();
			clone.Size = this.size.Clone();
			return clone;
		}
		/// <summary>Gets/Sets whether the memory access is volatile.</summary>
		public bool IsVolatile { get; set; }
		/// <summary>Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.</summary>
		public byte UnalignedPrefix { get; set; }
		public override StackType ResultType { get { return StackType.Void; } }
		protected override InstructionFlags ComputeFlags()
		{
			return destAddress.Flags | sourceAddress.Flags | size.Flags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			if (IsVolatile)
				output.Write("volatile.");
			if (UnalignedPrefix > 0)
				output.Write("unaligned(" + UnalignedPrefix + ").");
			output.Write(OpCode);
			output.Write('(');
			this.destAddress.WriteTo(output, options);
			output.Write(", ");
			this.sourceAddress.WriteTo(output, options);
			output.Write(", ");
			this.size.WriteTo(output, options);
			output.Write(')');
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitCpblk(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitCpblk(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitCpblk(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as Cpblk;
			return o != null && this.destAddress.PerformMatch(o.destAddress, ref match) && this.sourceAddress.PerformMatch(o.sourceAddress, ref match) && this.size.PerformMatch(o.size, ref match) && IsVolatile == o.IsVolatile && UnalignedPrefix == o.UnalignedPrefix;
		}
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(destAddress.ResultType == StackType.I || destAddress.ResultType == StackType.Ref);
			Debug.Assert(sourceAddress.ResultType == StackType.I || sourceAddress.ResultType == StackType.Ref);
			Debug.Assert(size.ResultType == StackType.I4);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>memset(address, value, size)</summary>
	public sealed partial class Initblk : ILInstruction, ISupportsVolatilePrefix, ISupportsUnalignedPrefix
	{
		public Initblk(ILInstruction address, ILInstruction value, ILInstruction size) : base(OpCode.Initblk)
		{
			this.Address = address;
			this.Value = value;
			this.Size = size;
		}
		public static readonly SlotInfo AddressSlot = new SlotInfo("Address", canInlineInto: true);
		ILInstruction address;
		public ILInstruction Address {
			get { return this.address; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.address, value, 0);
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
		public static readonly SlotInfo SizeSlot = new SlotInfo("Size", canInlineInto: true);
		ILInstruction size;
		public ILInstruction Size {
			get { return this.size; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.size, value, 2);
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
					return this.address;
				case 1:
					return this.value;
				case 2:
					return this.size;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Address = value;
					break;
				case 1:
					this.Value = value;
					break;
				case 2:
					this.Size = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return AddressSlot;
				case 1:
					return ValueSlot;
				case 2:
					return SizeSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (Initblk)ShallowClone();
			clone.Address = this.address.Clone();
			clone.Value = this.value.Clone();
			clone.Size = this.size.Clone();
			return clone;
		}
		/// <summary>Gets/Sets whether the memory access is volatile.</summary>
		public bool IsVolatile { get; set; }
		/// <summary>Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.</summary>
		public byte UnalignedPrefix { get; set; }
		public override StackType ResultType { get { return StackType.Void; } }
		protected override InstructionFlags ComputeFlags()
		{
			return address.Flags | value.Flags | size.Flags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			if (IsVolatile)
				output.Write("volatile.");
			if (UnalignedPrefix > 0)
				output.Write("unaligned(" + UnalignedPrefix + ").");
			output.Write(OpCode);
			output.Write('(');
			this.address.WriteTo(output, options);
			output.Write(", ");
			this.value.WriteTo(output, options);
			output.Write(", ");
			this.size.WriteTo(output, options);
			output.Write(')');
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitInitblk(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitInitblk(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitInitblk(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as Initblk;
			return o != null && this.address.PerformMatch(o.address, ref match) && this.value.PerformMatch(o.value, ref match) && this.size.PerformMatch(o.size, ref match) && IsVolatile == o.IsVolatile && UnalignedPrefix == o.UnalignedPrefix;
		}
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(address.ResultType == StackType.I || address.ResultType == StackType.Ref);
			Debug.Assert(value.ResultType == StackType.I4);
			Debug.Assert(size.ResultType == StackType.I4);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Load address of instance field</summary>
	public sealed partial class LdFlda : ILInstruction, IInstructionWithFieldOperand
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
		public bool DelayExceptions; // NullReferenceException/IndexOutOfBoundsException only occurs when the reference is dereferenced
		readonly IField field;
		/// <summary>Returns the field operand.</summary>
		public IField Field { get { return field; } }
		public override StackType ResultType { get { return target.ResultType.IsIntegerType() ? StackType.I : StackType.Ref; } }
		protected override InstructionFlags ComputeFlags()
		{
			return target.Flags | (DelayExceptions ? InstructionFlags.None : InstructionFlags.MayThrow);
		}
		public override InstructionFlags DirectFlags {
			get {
				return (DelayExceptions ? InstructionFlags.None : InstructionFlags.MayThrow);
			}
		}
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			if (DelayExceptions)
				output.Write("delayex.");
			output.Write(OpCode);
			output.Write(' ');
			field.WriteTo(output);
			output.Write('(');
			this.target.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdFlda(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdFlda;
			return o != null && this.target.PerformMatch(o.target, ref match) && DelayExceptions == o.DelayExceptions && field.Equals(o.field);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Load static field address</summary>
	public sealed partial class LdsFlda : SimpleInstruction, IInstructionWithFieldOperand
	{
		public LdsFlda(IField field) : base(OpCode.LdsFlda)
		{
			this.field = field;
		}
		public override StackType ResultType { get { return StackType.Ref; } }
		readonly IField field;
		/// <summary>Returns the field operand.</summary>
		public IField Field { get { return field; } }
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			field.WriteTo(output);
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitLdsFlda(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitLdsFlda(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdsFlda(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdsFlda;
			return o != null && field.Equals(o.field);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Casts an object to a class.</summary>
	public sealed partial class CastClass : UnaryInstruction
	{
		public CastClass(ILInstruction argument, IType type) : base(OpCode.CastClass, argument)
		{
			this.type = type;
		}
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
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
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			type.WriteTo(output);
			output.Write('(');
			Argument.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitCastClass(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as CastClass;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match) && type.Equals(o.type);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Test if object is instance of class or interface.</summary>
	public sealed partial class IsInst : UnaryInstruction
	{
		public IsInst(ILInstruction argument, IType type) : base(OpCode.IsInst, argument)
		{
			this.type = type;
		}
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
		public override StackType ResultType { get { return StackType.O; } }
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			type.WriteTo(output);
			output.Write('(');
			Argument.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitIsInst(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as IsInst;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match) && type.Equals(o.type);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
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
		void OriginalWriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			if (IsVolatile)
				output.Write("volatile.");
			if (UnalignedPrefix > 0)
				output.Write("unaligned(" + UnalignedPrefix + ").");
			output.Write(OpCode);
			output.Write(' ');
			type.WriteTo(output);
			output.Write('(');
			this.target.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdObj(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdObj;
			return o != null && this.target.PerformMatch(o.target, ref match) && type.Equals(o.type) && IsVolatile == o.IsVolatile && UnalignedPrefix == o.UnalignedPrefix;
		}
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(target.ResultType == StackType.Ref || target.ResultType == StackType.I);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Indirect store (store to ref/pointer).
	/// Evaluates to the value that was stored (when using type byte/short: evaluates to the truncated value, sign/zero extended back to I4 based on type.GetSign())</summary>
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
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
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
		void OriginalWriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			if (IsVolatile)
				output.Write("volatile.");
			if (UnalignedPrefix > 0)
				output.Write("unaligned(" + UnalignedPrefix + ").");
			output.Write(OpCode);
			output.Write(' ');
			type.WriteTo(output);
			output.Write('(');
			this.target.WriteTo(output, options);
			output.Write(", ");
			this.value.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitStObj(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as StObj;
			return o != null && this.target.PerformMatch(o.target, ref match) && this.value.PerformMatch(o.value, ref match) && type.Equals(o.type) && IsVolatile == o.IsVolatile && UnalignedPrefix == o.UnalignedPrefix;
		}
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(target.ResultType == StackType.Ref || target.ResultType == StackType.I);
			Debug.Assert(value.ResultType == type.GetStackType());
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Boxes a value.</summary>
	public sealed partial class Box : UnaryInstruction
	{
		public Box(ILInstruction argument, IType type) : base(OpCode.Box, argument)
		{
			this.type = type;
		}
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
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
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			type.WriteTo(output);
			output.Write('(');
			Argument.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitBox(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as Box;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match) && type.Equals(o.type);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Compute address inside box.</summary>
	public sealed partial class Unbox : UnaryInstruction
	{
		public Unbox(ILInstruction argument, IType type) : base(OpCode.Unbox, argument)
		{
			this.type = type;
		}
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
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
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			type.WriteTo(output);
			output.Write('(');
			Argument.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitUnbox(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as Unbox;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match) && type.Equals(o.type);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Unbox a value.</summary>
	public sealed partial class UnboxAny : UnaryInstruction
	{
		public UnboxAny(ILInstruction argument, IType type) : base(OpCode.UnboxAny, argument)
		{
			this.type = type;
		}
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
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
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			type.WriteTo(output);
			output.Write('(');
			Argument.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitUnboxAny(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as UnboxAny;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match) && type.Equals(o.type);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitNewObj(this, context);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Creates an array instance.</summary>
	public sealed partial class NewArr : ILInstruction
	{
		public NewArr(IType type, params ILInstruction[] indices) : base(OpCode.NewArr)
		{
			this.type = type;
			this.Indices = new InstructionCollection<ILInstruction>(this, 0);
			this.Indices.AddRange(indices);
		}
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
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
					this.Indices[index - 0] = (ILInstruction)value;
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
			clone.Indices.AddRange(this.Indices.Select(arg => (ILInstruction)arg.Clone()));
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
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			type.WriteTo(output);
			output.Write('(');
			bool first = true;
			foreach (var indices in Indices) {
				if (!first) output.Write(", "); else first = false;
				indices.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitNewArr(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as NewArr;
			return o != null && type.Equals(o.type) && Patterns.ListMatch.DoMatch(this.Indices, o.Indices, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Returns the default value for a type.</summary>
	public sealed partial class DefaultValue : SimpleInstruction
	{
		public DefaultValue(IType type) : base(OpCode.DefaultValue)
		{
			this.type = type;
		}
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
		public override StackType ResultType { get { return type.GetStackType(); } }
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			type.WriteTo(output);
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDefaultValue(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDefaultValue(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitDefaultValue(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as DefaultValue;
			return o != null && type.Equals(o.type);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Throws an exception.</summary>
	public sealed partial class Throw : UnaryInstruction
	{
		public Throw(ILInstruction argument) : base(OpCode.Throw, argument)
		{
		}
		public override StackType ResultType { get { return this.resultType; } }
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitThrow(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as Throw;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitRethrow(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as Rethrow;
			return o != null;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Gets the size of a type in bytes.</summary>
	public sealed partial class SizeOf : SimpleInstruction
	{
		public SizeOf(IType type) : base(OpCode.SizeOf)
		{
			this.type = type;
		}
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
		public override StackType ResultType { get { return StackType.I4; } }
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			type.WriteTo(output);
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitSizeOf(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitSizeOf(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitSizeOf(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as SizeOf;
			return o != null && type.Equals(o.type);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdLen(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdLen;
			return o != null && this.array.PerformMatch(o.array, ref match);
		}
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(array.ResultType == StackType.O);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
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
					this.Indices[index - 1] = (ILInstruction)value;
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
			clone.Indices.AddRange(this.Indices.Select(arg => (ILInstruction)arg.Clone()));
			return clone;
		}
		public bool DelayExceptions; // NullReferenceException/IndexOutOfBoundsException only occurs when the reference is dereferenced
		public override StackType ResultType { get { return StackType.Ref; } }
		/// <summary>Gets whether the 'readonly' prefix was applied to this instruction.</summary>
		public bool IsReadOnly { get; set; }
		protected override InstructionFlags ComputeFlags()
		{
			return array.Flags | Indices.Aggregate(InstructionFlags.None, (f, arg) => f | arg.Flags) | (DelayExceptions ? InstructionFlags.None : InstructionFlags.MayThrow);
		}
		public override InstructionFlags DirectFlags {
			get {
				return (DelayExceptions ? InstructionFlags.None : InstructionFlags.MayThrow);
			}
		}
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			if (DelayExceptions)
				output.Write("delayex.");
			if (IsReadOnly)
				output.Write("readonly.");
			output.Write(OpCode);
			output.Write(' ');
			type.WriteTo(output);
			output.Write('(');
			this.array.WriteTo(output, options);
			foreach (var indices in Indices) {
				output.Write(", ");
				indices.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitLdElema(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as LdElema;
			return o != null && type.Equals(o.type) && this.array.PerformMatch(o.array, ref match) && Patterns.ListMatch.DoMatch(this.Indices, o.Indices, ref match) && DelayExceptions == o.DelayExceptions && IsReadOnly == o.IsReadOnly;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Converts an array pointer (O) to a reference to the first element, or to a null reference if the array is null or empty.
	/// Also used to convert a string to a reference to the first character.</summary>
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
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write('(');
			this.array.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitArrayToPointer(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as ArrayToPointer;
			return o != null && this.array.PerformMatch(o.array, ref match);
		}
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(array.ResultType == StackType.O);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Maps a string value to an integer. This is used in switch(string).</summary>
	public sealed partial class StringToInt : ILInstruction
	{
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
			var clone = (StringToInt)ShallowClone();
			clone.Argument = this.argument.Clone();
			return clone;
		}
		public override StackType ResultType { get { return StackType.I4; } }
		protected override InstructionFlags ComputeFlags()
		{
			return argument.Flags;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.None;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitStringToInt(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitStringToInt(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitStringToInt(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as StringToInt;
			return o != null && this.argument.PerformMatch(o.argument, ref match);
		}
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(argument.ResultType == StackType.O);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>ILAst representation of Expression.Convert.</summary>
	public sealed partial class ExpressionTreeCast : UnaryInstruction
	{
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
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
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitExpressionTreeCast(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitExpressionTreeCast(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitExpressionTreeCast(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as ExpressionTreeCast;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match) && type.Equals(o.type) && this.IsChecked == o.IsChecked;
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Use of user-defined &amp;&amp; or || operator.</summary>
	public sealed partial class UserDefinedLogicOperator : ILInstruction, IInstructionWithMethodOperand
	{
		public UserDefinedLogicOperator(IMethod method, ILInstruction left, ILInstruction right) : base(OpCode.UserDefinedLogicOperator)
		{
			this.method = method;
			this.Left = left;
			this.Right = right;
		}
		readonly IMethod method;
		/// <summary>Returns the method operand.</summary>
		public IMethod Method { get { return method; } }
		public override StackType ResultType { get { return StackType.O; } }
		public static readonly SlotInfo LeftSlot = new SlotInfo("Left", canInlineInto: true);
		ILInstruction left;
		public ILInstruction Left {
			get { return this.left; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.left, value, 0);
			}
		}
		public static readonly SlotInfo RightSlot = new SlotInfo("Right");
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
			var clone = (UserDefinedLogicOperator)ShallowClone();
			clone.Left = this.left.Clone();
			clone.Right = this.right.Clone();
			return clone;
		}
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			method.WriteTo(output);
			output.Write('(');
			this.left.WriteTo(output, options);
			output.Write(", ");
			this.right.WriteTo(output, options);
			output.Write(')');
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitUserDefinedLogicOperator(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitUserDefinedLogicOperator(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitUserDefinedLogicOperator(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as UserDefinedLogicOperator;
			return o != null && method.Equals(o.method) && this.left.PerformMatch(o.left, ref match) && this.right.PerformMatch(o.right, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>ILAst representation of a short-circuiting binary operator inside a dynamic expression.</summary>
	public sealed partial class DynamicLogicOperatorInstruction : DynamicInstruction
	{
		public static readonly SlotInfo LeftSlot = new SlotInfo("Left", canInlineInto: true);
		ILInstruction left;
		public ILInstruction Left {
			get { return this.left; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.left, value, 0);
			}
		}
		public static readonly SlotInfo RightSlot = new SlotInfo("Right");
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
			var clone = (DynamicLogicOperatorInstruction)ShallowClone();
			clone.Left = this.left.Clone();
			clone.Right = this.right.Clone();
			return clone;
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDynamicLogicOperatorInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDynamicLogicOperatorInstruction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitDynamicLogicOperatorInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as DynamicLogicOperatorInstruction;
			return o != null && this.left.PerformMatch(o.left, ref match) && this.right.PerformMatch(o.right, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>ILAst representation of a binary operator inside a dynamic expression (maps to Binder.BinaryOperation).</summary>
	public sealed partial class DynamicBinaryOperatorInstruction : DynamicInstruction
	{
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
			var clone = (DynamicBinaryOperatorInstruction)ShallowClone();
			clone.Left = this.left.Clone();
			clone.Right = this.right.Clone();
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect | left.Flags | right.Flags;
		}
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDynamicBinaryOperatorInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDynamicBinaryOperatorInstruction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitDynamicBinaryOperatorInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as DynamicBinaryOperatorInstruction;
			return o != null && this.left.PerformMatch(o.left, ref match) && this.right.PerformMatch(o.right, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>ILAst representation of a unary operator inside a dynamic expression (maps to Binder.UnaryOperation).</summary>
	public sealed partial class DynamicUnaryOperatorInstruction : DynamicInstruction
	{
		public static readonly SlotInfo OperandSlot = new SlotInfo("Operand", canInlineInto: true);
		ILInstruction operand;
		public ILInstruction Operand {
			get { return this.operand; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.operand, value, 0);
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
					return this.operand;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					this.Operand = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		protected sealed override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return OperandSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		public sealed override ILInstruction Clone()
		{
			var clone = (DynamicUnaryOperatorInstruction)ShallowClone();
			clone.Operand = this.operand.Clone();
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect | operand.Flags;
		}
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDynamicUnaryOperatorInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDynamicUnaryOperatorInstruction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitDynamicUnaryOperatorInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as DynamicUnaryOperatorInstruction;
			return o != null && this.operand.PerformMatch(o.operand, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>ILAst representation of a cast inside a dynamic expression (maps to Binder.Convert).</summary>
	public sealed partial class DynamicConvertInstruction : DynamicInstruction
	{
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
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
			var clone = (DynamicConvertInstruction)ShallowClone();
			clone.Argument = this.argument.Clone();
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect | argument.Flags;
		}
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDynamicConvertInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDynamicConvertInstruction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitDynamicConvertInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as DynamicConvertInstruction;
			return o != null && type.Equals(o.type) && this.argument.PerformMatch(o.argument, ref match);
		}
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(argument.ResultType == StackType.O);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>ILAst representation of a property get method call inside a dynamic expression (maps to Binder.GetMember).</summary>
	public sealed partial class DynamicGetMemberInstruction : DynamicInstruction
	{
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
			var clone = (DynamicGetMemberInstruction)ShallowClone();
			clone.Target = this.target.Clone();
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect | target.Flags;
		}
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDynamicGetMemberInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDynamicGetMemberInstruction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitDynamicGetMemberInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as DynamicGetMemberInstruction;
			return o != null && this.target.PerformMatch(o.target, ref match);
		}
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(target.ResultType == StackType.O);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>ILAst representation of a property set method call inside a dynamic expression (maps to Binder.SetMember).</summary>
	public sealed partial class DynamicSetMemberInstruction : DynamicInstruction
	{
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
			var clone = (DynamicSetMemberInstruction)ShallowClone();
			clone.Target = this.target.Clone();
			clone.Value = this.value.Clone();
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect | target.Flags | value.Flags;
		}
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDynamicSetMemberInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDynamicSetMemberInstruction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitDynamicSetMemberInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as DynamicSetMemberInstruction;
			return o != null && this.target.PerformMatch(o.target, ref match) && this.value.PerformMatch(o.value, ref match);
		}
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(target.ResultType == StackType.O);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>ILAst representation of an indexer get method call inside a dynamic expression (maps to Binder.GetIndex).</summary>
	public sealed partial class DynamicGetIndexInstruction : DynamicInstruction
	{
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
					this.Arguments[index - 0] = (ILInstruction)value;
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
			var clone = (DynamicGetIndexInstruction)ShallowClone();
			clone.Arguments = new InstructionCollection<ILInstruction>(clone, 0);
			clone.Arguments.AddRange(this.Arguments.Select(arg => (ILInstruction)arg.Clone()));
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect | Arguments.Aggregate(InstructionFlags.None, (f, arg) => f | arg.Flags);
		}
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDynamicGetIndexInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDynamicGetIndexInstruction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitDynamicGetIndexInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as DynamicGetIndexInstruction;
			return o != null && Patterns.ListMatch.DoMatch(this.Arguments, o.Arguments, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>ILAst representation of an indexer set method call inside a dynamic expression (maps to Binder.SetIndex).</summary>
	public sealed partial class DynamicSetIndexInstruction : DynamicInstruction
	{
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
					this.Arguments[index - 0] = (ILInstruction)value;
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
			var clone = (DynamicSetIndexInstruction)ShallowClone();
			clone.Arguments = new InstructionCollection<ILInstruction>(clone, 0);
			clone.Arguments.AddRange(this.Arguments.Select(arg => (ILInstruction)arg.Clone()));
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect | Arguments.Aggregate(InstructionFlags.None, (f, arg) => f | arg.Flags);
		}
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDynamicSetIndexInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDynamicSetIndexInstruction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitDynamicSetIndexInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as DynamicSetIndexInstruction;
			return o != null && Patterns.ListMatch.DoMatch(this.Arguments, o.Arguments, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>ILAst representation of a method call inside a dynamic expression (maps to Binder.InvokeMember).</summary>
	public sealed partial class DynamicInvokeMemberInstruction : DynamicInstruction
	{
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
					this.Arguments[index - 0] = (ILInstruction)value;
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
			var clone = (DynamicInvokeMemberInstruction)ShallowClone();
			clone.Arguments = new InstructionCollection<ILInstruction>(clone, 0);
			clone.Arguments.AddRange(this.Arguments.Select(arg => (ILInstruction)arg.Clone()));
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect | Arguments.Aggregate(InstructionFlags.None, (f, arg) => f | arg.Flags);
		}
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDynamicInvokeMemberInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDynamicInvokeMemberInstruction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitDynamicInvokeMemberInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as DynamicInvokeMemberInstruction;
			return o != null && Patterns.ListMatch.DoMatch(this.Arguments, o.Arguments, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>ILAst representation of a constuctor invocation inside a dynamic expression (maps to Binder.InvokeConstructor).</summary>
	public sealed partial class DynamicInvokeConstructorInstruction : DynamicInstruction
	{
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
					this.Arguments[index - 0] = (ILInstruction)value;
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
			var clone = (DynamicInvokeConstructorInstruction)ShallowClone();
			clone.Arguments = new InstructionCollection<ILInstruction>(clone, 0);
			clone.Arguments.AddRange(this.Arguments.Select(arg => (ILInstruction)arg.Clone()));
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect | Arguments.Aggregate(InstructionFlags.None, (f, arg) => f | arg.Flags);
		}
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDynamicInvokeConstructorInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDynamicInvokeConstructorInstruction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitDynamicInvokeConstructorInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as DynamicInvokeConstructorInstruction;
			return o != null && Patterns.ListMatch.DoMatch(this.Arguments, o.Arguments, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>ILAst representation of a delegate invocation inside a dynamic expression (maps to Binder.Invoke).</summary>
	public sealed partial class DynamicInvokeInstruction : DynamicInstruction
	{
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
					this.Arguments[index - 0] = (ILInstruction)value;
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
			var clone = (DynamicInvokeInstruction)ShallowClone();
			clone.Arguments = new InstructionCollection<ILInstruction>(clone, 0);
			clone.Arguments.AddRange(this.Arguments.Select(arg => (ILInstruction)arg.Clone()));
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect | Arguments.Aggregate(InstructionFlags.None, (f, arg) => f | arg.Flags);
		}
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDynamicInvokeInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDynamicInvokeInstruction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitDynamicInvokeInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as DynamicInvokeInstruction;
			return o != null && Patterns.ListMatch.DoMatch(this.Arguments, o.Arguments, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>ILAst representation of a call to the Binder.IsEvent method inside a dynamic expression.</summary>
	public sealed partial class DynamicIsEventInstruction : DynamicInstruction
	{
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
			var clone = (DynamicIsEventInstruction)ShallowClone();
			clone.Argument = this.argument.Clone();
			return clone;
		}
		protected override InstructionFlags ComputeFlags()
		{
			return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect | argument.Flags;
		}
		public override InstructionFlags DirectFlags {
			get {
				return base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitDynamicIsEventInstruction(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitDynamicIsEventInstruction(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitDynamicIsEventInstruction(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as DynamicIsEventInstruction;
			return o != null && this.argument.PerformMatch(o.argument, ref match);
		}
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(argument.ResultType == StackType.O);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Push a typed reference of type class onto the stack.</summary>
	public sealed partial class MakeRefAny : UnaryInstruction
	{
		public MakeRefAny(ILInstruction argument, IType type) : base(OpCode.MakeRefAny, argument)
		{
			this.type = type;
		}
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
		public override StackType ResultType { get { return StackType.O; } }
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			type.WriteTo(output);
			output.Write('(');
			Argument.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitMakeRefAny(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as MakeRefAny;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match) && type.Equals(o.type);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitRefAnyType(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as RefAnyType;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Push the address stored in a typed reference.</summary>
	public sealed partial class RefAnyValue : UnaryInstruction
	{
		public RefAnyValue(ILInstruction argument, IType type) : base(OpCode.RefAnyValue, argument)
		{
			this.type = type;
		}
		IType type;
		/// <summary>Returns the type operand.</summary>
		public IType Type {
			get { return type; }
			set { type = value; InvalidateFlags(); }
		}
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
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			type.WriteTo(output);
			output.Write('(');
			Argument.WriteTo(output, options);
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
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitRefAnyValue(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as RefAnyValue;
			return o != null && this.Argument.PerformMatch(o.Argument, ref match) && type.Equals(o.type);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Yield an element from an iterator.</summary>
	public sealed partial class YieldReturn : ILInstruction
	{
		public YieldReturn(ILInstruction value) : base(OpCode.YieldReturn)
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
			var clone = (YieldReturn)ShallowClone();
			clone.Value = this.value.Clone();
			return clone;
		}
		public override StackType ResultType { get { return StackType.Void; } }
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.MayBranch | InstructionFlags.SideEffect | value.Flags;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayBranch | InstructionFlags.SideEffect;
			}
		}
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write('(');
			this.value.WriteTo(output, options);
			output.Write(')');
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitYieldReturn(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitYieldReturn(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitYieldReturn(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as YieldReturn;
			return o != null && this.value.PerformMatch(o.value, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>C# await operator.</summary>
	public sealed partial class Await : ILInstruction
	{
		public Await(ILInstruction value) : base(OpCode.Await)
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
			var clone = (Await)ShallowClone();
			clone.Value = this.value.Clone();
			return clone;
		}
		public override StackType ResultType { get { return GetResultMethod?.ReturnType.GetStackType() ?? StackType.Unknown; } }
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.SideEffect | value.Flags;
		}
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.SideEffect;
			}
		}
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write('(');
			this.value.WriteTo(output, options);
			output.Write(')');
		}
		public override void AcceptVisitor(ILVisitor visitor)
		{
			visitor.VisitAwait(this);
		}
		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			return visitor.VisitAwait(this);
		}
		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			return visitor.VisitAwait(this, context);
		}
		protected internal override bool PerformMatch(ILInstruction other, ref Patterns.Match match)
		{
			var o = other as Await;
			return o != null && this.value.PerformMatch(o.value, ref match);
		}
	}
}
namespace ICSharpCode.Decompiler.IL.Patterns
{
	/// <summary>Matches any node</summary>
	public sealed partial class AnyNode : PatternInstruction
	{
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
			var clone = (AnyNode)ShallowClone();
			return clone;
		}
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write('(');
			output.Write(')');
		}
	}
}

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Base class for visitor pattern.
	/// </summary>
	public abstract class ILVisitor
	{
		/// <summary>Called by Visit*() methods that were not overridden</summary>
		protected abstract void Default(ILInstruction inst);
		
		protected internal virtual void VisitInvalidBranch(InvalidBranch inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitInvalidExpression(InvalidExpression inst)
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
		protected internal virtual void VisitBinaryNumericInstruction(BinaryNumericInstruction inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitNumericCompoundAssign(NumericCompoundAssign inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitUserDefinedCompoundAssign(UserDefinedCompoundAssign inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitDynamicCompoundAssign(DynamicCompoundAssign inst)
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
		protected internal virtual void VisitNullCoalescingInstruction(NullCoalescingInstruction inst)
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
		protected internal virtual void VisitLockInstruction(LockInstruction inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitUsingInstruction(UsingInstruction inst)
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
		protected internal virtual void VisitCallIndirect(CallIndirect inst)
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
		protected internal virtual void VisitThreeValuedBoolAnd(ThreeValuedBoolAnd inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitThreeValuedBoolOr(ThreeValuedBoolOr inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitNullableUnwrap(NullableUnwrap inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitNullableRewrap(NullableRewrap inst)
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
		protected internal virtual void VisitLdcF4(LdcF4 inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdcF8(LdcF8 inst)
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
		protected internal virtual void VisitLdVirtDelegate(LdVirtDelegate inst)
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
		protected internal virtual void VisitLocAllocSpan(LocAllocSpan inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitCpblk(Cpblk inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitInitblk(Initblk inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdFlda(LdFlda inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitLdsFlda(LdsFlda inst)
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
		protected internal virtual void VisitStringToInt(StringToInt inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitExpressionTreeCast(ExpressionTreeCast inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitUserDefinedLogicOperator(UserDefinedLogicOperator inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitDynamicLogicOperatorInstruction(DynamicLogicOperatorInstruction inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitDynamicBinaryOperatorInstruction(DynamicBinaryOperatorInstruction inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitDynamicUnaryOperatorInstruction(DynamicUnaryOperatorInstruction inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitDynamicConvertInstruction(DynamicConvertInstruction inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitDynamicGetMemberInstruction(DynamicGetMemberInstruction inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitDynamicSetMemberInstruction(DynamicSetMemberInstruction inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitDynamicGetIndexInstruction(DynamicGetIndexInstruction inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitDynamicSetIndexInstruction(DynamicSetIndexInstruction inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitDynamicInvokeMemberInstruction(DynamicInvokeMemberInstruction inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitDynamicInvokeConstructorInstruction(DynamicInvokeConstructorInstruction inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitDynamicInvokeInstruction(DynamicInvokeInstruction inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitDynamicIsEventInstruction(DynamicIsEventInstruction inst)
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
		protected internal virtual void VisitYieldReturn(YieldReturn inst)
		{
			Default(inst);
		}
		protected internal virtual void VisitAwait(Await inst)
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
		
		protected internal virtual T VisitInvalidBranch(InvalidBranch inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitInvalidExpression(InvalidExpression inst)
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
		protected internal virtual T VisitBinaryNumericInstruction(BinaryNumericInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitNumericCompoundAssign(NumericCompoundAssign inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitUserDefinedCompoundAssign(UserDefinedCompoundAssign inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitDynamicCompoundAssign(DynamicCompoundAssign inst)
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
		protected internal virtual T VisitNullCoalescingInstruction(NullCoalescingInstruction inst)
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
		protected internal virtual T VisitLockInstruction(LockInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitUsingInstruction(UsingInstruction inst)
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
		protected internal virtual T VisitCallIndirect(CallIndirect inst)
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
		protected internal virtual T VisitThreeValuedBoolAnd(ThreeValuedBoolAnd inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitThreeValuedBoolOr(ThreeValuedBoolOr inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitNullableUnwrap(NullableUnwrap inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitNullableRewrap(NullableRewrap inst)
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
		protected internal virtual T VisitLdcF4(LdcF4 inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdcF8(LdcF8 inst)
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
		protected internal virtual T VisitLdVirtDelegate(LdVirtDelegate inst)
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
		protected internal virtual T VisitLocAllocSpan(LocAllocSpan inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitCpblk(Cpblk inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitInitblk(Initblk inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdFlda(LdFlda inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitLdsFlda(LdsFlda inst)
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
		protected internal virtual T VisitStringToInt(StringToInt inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitExpressionTreeCast(ExpressionTreeCast inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitUserDefinedLogicOperator(UserDefinedLogicOperator inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitDynamicLogicOperatorInstruction(DynamicLogicOperatorInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitDynamicBinaryOperatorInstruction(DynamicBinaryOperatorInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitDynamicUnaryOperatorInstruction(DynamicUnaryOperatorInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitDynamicConvertInstruction(DynamicConvertInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitDynamicGetMemberInstruction(DynamicGetMemberInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitDynamicSetMemberInstruction(DynamicSetMemberInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitDynamicGetIndexInstruction(DynamicGetIndexInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitDynamicSetIndexInstruction(DynamicSetIndexInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitDynamicInvokeMemberInstruction(DynamicInvokeMemberInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitDynamicInvokeConstructorInstruction(DynamicInvokeConstructorInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitDynamicInvokeInstruction(DynamicInvokeInstruction inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitDynamicIsEventInstruction(DynamicIsEventInstruction inst)
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
		protected internal virtual T VisitYieldReturn(YieldReturn inst)
		{
			return Default(inst);
		}
		protected internal virtual T VisitAwait(Await inst)
		{
			return Default(inst);
		}
	}

	/// <summary>
	/// Base class for visitor pattern.
	/// </summary>
	public abstract class ILVisitor<C, T>
	{
		/// <summary>Called by Visit*() methods that were not overridden</summary>
		protected abstract T Default(ILInstruction inst, C context);
		
		protected internal virtual T VisitInvalidBranch(InvalidBranch inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitInvalidExpression(InvalidExpression inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitNop(Nop inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitILFunction(ILFunction function, C context)
		{
			return Default(function, context);
		}
		protected internal virtual T VisitBlockContainer(BlockContainer container, C context)
		{
			return Default(container, context);
		}
		protected internal virtual T VisitBlock(Block block, C context)
		{
			return Default(block, context);
		}
		protected internal virtual T VisitPinnedRegion(PinnedRegion inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitBinaryNumericInstruction(BinaryNumericInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitNumericCompoundAssign(NumericCompoundAssign inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitUserDefinedCompoundAssign(UserDefinedCompoundAssign inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitDynamicCompoundAssign(DynamicCompoundAssign inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitBitNot(BitNot inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitArglist(Arglist inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitBranch(Branch inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLeave(Leave inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitIfInstruction(IfInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitNullCoalescingInstruction(NullCoalescingInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitSwitchInstruction(SwitchInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitSwitchSection(SwitchSection inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitTryCatch(TryCatch inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitTryCatchHandler(TryCatchHandler inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitTryFinally(TryFinally inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitTryFault(TryFault inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLockInstruction(LockInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitUsingInstruction(UsingInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitDebugBreak(DebugBreak inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitComp(Comp inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitCall(Call inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitCallVirt(CallVirt inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitCallIndirect(CallIndirect inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitCkfinite(Ckfinite inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitConv(Conv inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdLoc(LdLoc inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdLoca(LdLoca inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitStLoc(StLoc inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitAddressOf(AddressOf inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitThreeValuedBoolAnd(ThreeValuedBoolAnd inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitThreeValuedBoolOr(ThreeValuedBoolOr inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitNullableUnwrap(NullableUnwrap inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitNullableRewrap(NullableRewrap inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdStr(LdStr inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdcI4(LdcI4 inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdcI8(LdcI8 inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdcF4(LdcF4 inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdcF8(LdcF8 inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdcDecimal(LdcDecimal inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdNull(LdNull inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdFtn(LdFtn inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdVirtFtn(LdVirtFtn inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdVirtDelegate(LdVirtDelegate inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdTypeToken(LdTypeToken inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdMemberToken(LdMemberToken inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLocAlloc(LocAlloc inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLocAllocSpan(LocAllocSpan inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitCpblk(Cpblk inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitInitblk(Initblk inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdFlda(LdFlda inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdsFlda(LdsFlda inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitCastClass(CastClass inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitIsInst(IsInst inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdObj(LdObj inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitStObj(StObj inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitBox(Box inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitUnbox(Unbox inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitUnboxAny(UnboxAny inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitNewObj(NewObj inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitNewArr(NewArr inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitDefaultValue(DefaultValue inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitThrow(Throw inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitRethrow(Rethrow inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitSizeOf(SizeOf inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdLen(LdLen inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitLdElema(LdElema inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitArrayToPointer(ArrayToPointer inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitStringToInt(StringToInt inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitExpressionTreeCast(ExpressionTreeCast inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitUserDefinedLogicOperator(UserDefinedLogicOperator inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitDynamicLogicOperatorInstruction(DynamicLogicOperatorInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitDynamicBinaryOperatorInstruction(DynamicBinaryOperatorInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitDynamicUnaryOperatorInstruction(DynamicUnaryOperatorInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitDynamicConvertInstruction(DynamicConvertInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitDynamicGetMemberInstruction(DynamicGetMemberInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitDynamicSetMemberInstruction(DynamicSetMemberInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitDynamicGetIndexInstruction(DynamicGetIndexInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitDynamicSetIndexInstruction(DynamicSetIndexInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitDynamicInvokeMemberInstruction(DynamicInvokeMemberInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitDynamicInvokeConstructorInstruction(DynamicInvokeConstructorInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitDynamicInvokeInstruction(DynamicInvokeInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitDynamicIsEventInstruction(DynamicIsEventInstruction inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitMakeRefAny(MakeRefAny inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitRefAnyType(RefAnyType inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitRefAnyValue(RefAnyValue inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitYieldReturn(YieldReturn inst, C context)
		{
			return Default(inst, context);
		}
		protected internal virtual T VisitAwait(Await inst, C context)
		{
			return Default(inst, context);
		}
	}
	
	partial class InstructionOutputExtensions
	{
		static readonly string[] originalOpCodeNames = {
			"invalid.branch",
			"invalid.expr",
			"nop",
			"ILFunction",
			"BlockContainer",
			"Block",
			"PinnedRegion",
			"binary",
			"numeric.compound",
			"user.compound",
			"dynamic.compound",
			"bit.not",
			"arglist",
			"br",
			"leave",
			"if",
			"if.notnull",
			"switch",
			"switch.section",
			"try.catch",
			"try.catch.handler",
			"try.finally",
			"try.fault",
			"lock",
			"using",
			"debug.break",
			"comp",
			"call",
			"callvirt",
			"calli",
			"ckfinite",
			"conv",
			"ldloc",
			"ldloca",
			"stloc",
			"addressof",
			"3vl.bool.and",
			"3vl.bool.or",
			"nullable.unwrap",
			"nullable.rewrap",
			"ldstr",
			"ldc.i4",
			"ldc.i8",
			"ldc.f4",
			"ldc.f8",
			"ldc.decimal",
			"ldnull",
			"ldftn",
			"ldvirtftn",
			"ldvirtdelegate",
			"ldtypetoken",
			"ldmembertoken",
			"localloc",
			"localloc.span",
			"cpblk",
			"initblk",
			"ldflda",
			"ldsflda",
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
			"string.to.int",
			"expression.tree.cast",
			"user.logic.operator",
			"dynamic.logic.operator",
			"dynamic.binary.operator",
			"dynamic.unary.operator",
			"dynamic.convert",
			"dynamic.getmember",
			"dynamic.setmember",
			"dynamic.getindex",
			"dynamic.setindex",
			"dynamic.invokemember",
			"dynamic.invokeconstructor",
			"dynamic.invoke",
			"dynamic.isevent",
			"mkrefany",
			"refanytype",
			"refanyval",
			"yield.return",
			"await",
			"AnyNode",
		};
	}
	
	partial class ILInstruction
	{
		public bool MatchInvalidBranch()
		{
			var inst = this as InvalidBranch;
			if (inst != null) {
				return true;
			}
			return false;
		}
		public bool MatchInvalidExpression()
		{
			var inst = this as InvalidExpression;
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
		public bool MatchPinnedRegion(out ILVariable variable, out ILInstruction init, out ILInstruction body)
		{
			var inst = this as PinnedRegion;
			if (inst != null) {
				variable = inst.Variable;
				init = inst.Init;
				body = inst.Body;
				return true;
			}
			variable = default(ILVariable);
			init = default(ILInstruction);
			body = default(ILInstruction);
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
		public bool MatchLockInstruction(out ILInstruction onExpression, out ILInstruction body)
		{
			var inst = this as LockInstruction;
			if (inst != null) {
				onExpression = inst.OnExpression;
				body = inst.Body;
				return true;
			}
			onExpression = default(ILInstruction);
			body = default(ILInstruction);
			return false;
		}
		public bool MatchUsingInstruction(out ILVariable variable, out ILInstruction resourceExpression, out ILInstruction body)
		{
			var inst = this as UsingInstruction;
			if (inst != null) {
				variable = inst.Variable;
				resourceExpression = inst.ResourceExpression;
				body = inst.Body;
				return true;
			}
			variable = default(ILVariable);
			resourceExpression = default(ILInstruction);
			body = default(ILInstruction);
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
		public bool MatchThreeValuedBoolAnd(out ILInstruction left, out ILInstruction right)
		{
			var inst = this as ThreeValuedBoolAnd;
			if (inst != null) {
				left = inst.Left;
				right = inst.Right;
				return true;
			}
			left = default(ILInstruction);
			right = default(ILInstruction);
			return false;
		}
		public bool MatchThreeValuedBoolOr(out ILInstruction left, out ILInstruction right)
		{
			var inst = this as ThreeValuedBoolOr;
			if (inst != null) {
				left = inst.Left;
				right = inst.Right;
				return true;
			}
			left = default(ILInstruction);
			right = default(ILInstruction);
			return false;
		}
		public bool MatchNullableRewrap(out ILInstruction argument)
		{
			var inst = this as NullableRewrap;
			if (inst != null) {
				argument = inst.Argument;
				return true;
			}
			argument = default(ILInstruction);
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
		public bool MatchLdcF4(out float value)
		{
			var inst = this as LdcF4;
			if (inst != null) {
				value = inst.Value;
				return true;
			}
			value = default(float);
			return false;
		}
		public bool MatchLdcF8(out double value)
		{
			var inst = this as LdcF8;
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
		public bool MatchLdVirtDelegate(out ILInstruction argument, out IType type, out IMethod method)
		{
			var inst = this as LdVirtDelegate;
			if (inst != null) {
				argument = inst.Argument;
				type = inst.Type;
				method = inst.Method;
				return true;
			}
			argument = default(ILInstruction);
			type = default(IType);
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
		public bool MatchLocAllocSpan(out ILInstruction argument, out IType type)
		{
			var inst = this as LocAllocSpan;
			if (inst != null) {
				argument = inst.Argument;
				type = inst.Type;
				return true;
			}
			argument = default(ILInstruction);
			type = default(IType);
			return false;
		}
		public bool MatchCpblk(out ILInstruction destAddress, out ILInstruction sourceAddress, out ILInstruction size)
		{
			var inst = this as Cpblk;
			if (inst != null) {
				destAddress = inst.DestAddress;
				sourceAddress = inst.SourceAddress;
				size = inst.Size;
				return true;
			}
			destAddress = default(ILInstruction);
			sourceAddress = default(ILInstruction);
			size = default(ILInstruction);
			return false;
		}
		public bool MatchInitblk(out ILInstruction address, out ILInstruction value, out ILInstruction size)
		{
			var inst = this as Initblk;
			if (inst != null) {
				address = inst.Address;
				value = inst.Value;
				size = inst.Size;
				return true;
			}
			address = default(ILInstruction);
			value = default(ILInstruction);
			size = default(ILInstruction);
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
		public bool MatchUserDefinedLogicOperator(out IMethod method, out ILInstruction left, out ILInstruction right)
		{
			var inst = this as UserDefinedLogicOperator;
			if (inst != null) {
				method = inst.Method;
				left = inst.Left;
				right = inst.Right;
				return true;
			}
			method = default(IMethod);
			left = default(ILInstruction);
			right = default(ILInstruction);
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
		public bool MatchYieldReturn(out ILInstruction value)
		{
			var inst = this as YieldReturn;
			if (inst != null) {
				value = inst.Value;
				return true;
			}
			value = default(ILInstruction);
			return false;
		}
		public bool MatchAwait(out ILInstruction value)
		{
			var inst = this as Await;
			if (inst != null) {
				value = inst.Value;
				return true;
			}
			value = default(ILInstruction);
			return false;
		}
	}
}

