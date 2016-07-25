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
using ICSharpCode.NRefactory.CSharp;
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
			switch (kind) {
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
					throw new NotSupportedException();
			}
		}
		
		public static BinaryOperatorType ToBinaryOperatorType(this ComparisonKind kind)
		{
			switch (kind) {
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
					throw new NotSupportedException();
			}
		}
		
		public static string GetToken(this ComparisonKind kind)
		{
			return BinaryOperatorExpression.GetOperatorRole(kind.ToBinaryOperatorType()).Token;
		}
	}
	
	partial class Comp
	{
		ComparisonKind kind;

		public ComparisonKind Kind {
			get { return kind; }
			set {
				kind = value;
				MakeDirty();
			}
		}

		/// <summary>
		/// Gets the stack type of the comparison inputs.
		/// </summary>
		public StackType InputType {
			get { return Left.ResultType; }
		}
		
		/// <summary>
		/// If this is an integer comparison, specifies the sign used to interpret the integers.
		/// </summary>
		public readonly Sign Sign;

		public Comp(ComparisonKind kind, Sign sign, ILInstruction left, ILInstruction right) : base(OpCode.Comp, left, right)
		{
			this.kind = kind;
			this.Sign = sign;
			Debug.Assert(left.ResultType == right.ResultType);
		}

		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			switch (Sign) {
				case Sign.Signed:
					output.Write(".signed");
					break;
				case Sign.Unsigned:
					output.Write(".unsigned");
					break;
			}
			output.Write('(');
			Left.WriteTo(output);
			output.Write(' ');
			output.Write(Kind.GetToken());
			output.Write(' ');
			Right.WriteTo(output);
			output.Write(')');
		}
	}
}


