#nullable enable
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

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	public enum ComparisonKind : byte
	{
		Equality,
		Inequality,
		LessThan,
		LessThanOrEqual,
		GreaterThan,
		GreaterThanOrEqual
	}

	static class ComparisonKindExtensions
	{
		public static bool IsEqualityOrInequality(this ComparisonKind kind)
		{
			return kind == ComparisonKind.Equality || kind == ComparisonKind.Inequality;
		}

		public static ComparisonKind Negate(this ComparisonKind kind)
		{
			switch (kind)
			{
				case ComparisonKind.Equality:
					return ComparisonKind.Inequality;
				case ComparisonKind.Inequality:
					return ComparisonKind.Equality;
				case ComparisonKind.LessThan:
					return ComparisonKind.GreaterThanOrEqual;
				case ComparisonKind.LessThanOrEqual:
					return ComparisonKind.GreaterThan;
				case ComparisonKind.GreaterThan:
					return ComparisonKind.LessThanOrEqual;
				case ComparisonKind.GreaterThanOrEqual:
					return ComparisonKind.LessThan;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static BinaryOperatorType ToBinaryOperatorType(this ComparisonKind kind)
		{
			switch (kind)
			{
				case ComparisonKind.Equality:
					return BinaryOperatorType.Equality;
				case ComparisonKind.Inequality:
					return BinaryOperatorType.InEquality;
				case ComparisonKind.LessThan:
					return BinaryOperatorType.LessThan;
				case ComparisonKind.LessThanOrEqual:
					return BinaryOperatorType.LessThanOrEqual;
				case ComparisonKind.GreaterThan:
					return BinaryOperatorType.GreaterThan;
				case ComparisonKind.GreaterThanOrEqual:
					return BinaryOperatorType.GreaterThanOrEqual;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static string GetToken(this ComparisonKind kind)
		{
			return BinaryOperatorExpression.GetOperatorRole(kind.ToBinaryOperatorType()).Token;
		}
	}

	public enum ComparisonLiftingKind
	{
		/// <summary>
		/// Not a lifted comparison.
		/// </summary>
		None,
		/// <summary>
		/// C#-style lifted comparison:
		/// * operands that have a ResultType != this.InputType are expected to return a value of
		///   type Nullable{T}, where T.GetStackType() == this.InputType.
		/// * if both operands are <c>null</c>, equality comparisons evaluate to 1, all other comparisons to 0.
		/// * if one operand is <c>null</c>, inequality comparisons evaluate to 1, all other comparisons to 0.
		/// * if neither operand is <c>null</c>, the underlying comparison is performed.
		/// 
		/// Note that even though C#-style lifted comparisons set IsLifted=true,
		/// the ResultType remains I4 as with normal comparisons.
		/// </summary>
		CSharp,
		/// <summary>
		/// SQL-style lifted comparison: works like a lifted binary numeric instruction,
		/// that is, if any input operand is <c>null</c>, the comparison evaluates to <c>null</c>.
		/// </summary>
		/// <remarks>
		/// This lifting kind is currently only used for operator! on bool?.
		/// </remarks>
		ThreeValuedLogic
	}

	partial class Comp : ILiftableInstruction
	{
		ComparisonKind kind;

		public ComparisonKind Kind {
			get { return kind; }
			set {
				kind = value;
				MakeDirty();
			}
		}

		public readonly ComparisonLiftingKind LiftingKind;

		/// <summary>
		/// Gets the stack type of the comparison inputs.
		/// For lifted comparisons, this is the underlying input type.
		/// </summary>
		public StackType InputType;

		/// <summary>
		/// If this is an integer comparison, specifies the sign used to interpret the integers.
		/// </summary>
		public readonly Sign Sign;

		public Comp(ComparisonKind kind, Sign sign, ILInstruction left, ILInstruction right) : base(OpCode.Comp, left, right)
		{
			this.kind = kind;
			this.LiftingKind = ComparisonLiftingKind.None;
			this.InputType = left.ResultType;
			this.Sign = sign;
			Debug.Assert(left.ResultType == right.ResultType);
		}

		public Comp(ComparisonKind kind, ComparisonLiftingKind lifting, StackType inputType, Sign sign, ILInstruction left, ILInstruction right) : base(OpCode.Comp, left, right)
		{
			this.kind = kind;
			this.LiftingKind = lifting;
			this.InputType = inputType;
			this.Sign = sign;
		}

		public override StackType ResultType => LiftingKind == ComparisonLiftingKind.ThreeValuedLogic ? StackType.O : StackType.I4;
		public bool IsLifted => LiftingKind != ComparisonLiftingKind.None;
		public StackType UnderlyingResultType => StackType.I4;

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			if (LiftingKind == ComparisonLiftingKind.None)
			{
				Debug.Assert(Left.ResultType == InputType);
				Debug.Assert(Right.ResultType == InputType);
			}
			else
			{
				Debug.Assert(Left.ResultType == InputType || Left.ResultType == StackType.O);
				Debug.Assert(Right.ResultType == InputType || Right.ResultType == StackType.O);
			}
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			if (options.UseLogicOperationSugar && MatchLogicNot(out var arg))
			{
				output.Write("logic.not(");
				arg.WriteTo(output, options);
				output.Write(')');
				return;
			}
			output.Write(OpCode);
			output.Write('.');
			output.Write(InputType.ToString().ToLower());
			switch (Sign)
			{
				case Sign.Signed:
					output.Write(".signed");
					break;
				case Sign.Unsigned:
					output.Write(".unsigned");
					break;
			}
			switch (LiftingKind)
			{
				case ComparisonLiftingKind.CSharp:
					output.Write(".lifted[C#]");
					break;
				case ComparisonLiftingKind.ThreeValuedLogic:
					output.Write(".lifted[3VL]");
					break;
			}
			output.Write('(');
			Left.WriteTo(output, options);
			output.Write(' ');
			output.Write(Kind.GetToken());
			output.Write(' ');
			Right.WriteTo(output, options);
			output.Write(')');
		}

		public static Comp LogicNot(ILInstruction arg)
		{
			return new Comp(ComparisonKind.Equality, Sign.None, arg, new LdcI4(0));
		}

		public static Comp LogicNot(ILInstruction arg, bool isLifted)
		{
			var liftingKind = isLifted ? ComparisonLiftingKind.ThreeValuedLogic : ComparisonLiftingKind.None;
			return new Comp(ComparisonKind.Equality, liftingKind, StackType.I4, Sign.None, arg, new LdcI4(0));
		}
	}
}
