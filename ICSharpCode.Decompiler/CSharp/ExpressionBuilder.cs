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

using System.Diagnostics;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.CSharp
{
	/// <summary>
	/// Translates from ILAst to C# expressions.
	/// </summary>
	class ExpressionBuilder : ILVisitor<ExpressionBuilder.ConvertedExpression>
	{
		private readonly ICompilation compilation;
		readonly NRefactoryCecilMapper cecilMapper;
		readonly CSharpResolver resolver;
		readonly TypeSystemAstBuilder astBuilder;
		
		public ExpressionBuilder(ICompilation compilation, NRefactoryCecilMapper cecilMapper)
		{
			this.compilation = compilation;
			this.cecilMapper = cecilMapper;
			this.resolver = new CSharpResolver(compilation);
			this.astBuilder = new TypeSystemAstBuilder(resolver);
		}
		
		internal struct ConvertedExpression
		{
			public readonly Expression Expression;
			public readonly IType Type;
			
			public ConvertedExpression(Expression expression, IType type)
			{
				this.Expression = expression;
				this.Type = type;
			}
			
			public Expression ConvertTo(IType targetType, ExpressionBuilder expressionBuilder)
			{
				if (targetType.IsKnownType(KnownTypeCode.Boolean))
					return ConvertToBoolean();
				if (Type.Equals(targetType))
					return Expression;
				if (Type.Kind == TypeKind.ByReference && targetType.Kind == TypeKind.Pointer && Expression is DirectionExpression) {
					// convert from reference to pointer
					Expression arg = ((DirectionExpression)Expression).Expression.Detach();
					var pointerExpr = new ConvertedExpression(
						new UnaryOperatorExpression(UnaryOperatorType.AddressOf, arg),
						new PointerType(((ByReferenceType)Type).ElementType));
					// perform remaining pointer cast, if necessary
					return pointerExpr.ConvertTo(targetType, expressionBuilder);
				}
				if (Expression is PrimitiveExpression) {
					object value = ((PrimitiveExpression)Expression).Value;
					var rr = expressionBuilder.resolver.ResolveCast(targetType, new ConstantResolveResult(Type, value));
					if (rr.IsCompileTimeConstant && !rr.IsError)
						return expressionBuilder.astBuilder.ConvertConstantValue(rr);
				}
				return new CastExpression(
					expressionBuilder.astBuilder.ConvertType(targetType),
					Expression);
			}
			
			public Expression ConvertToBoolean()
			{
				if (Type.IsKnownType(KnownTypeCode.Boolean) || Type.Kind == TypeKind.Unknown)
					return Expression;
				else if (Expression is PrimitiveExpression && Type.IsKnownType(KnownTypeCode.Int32))
					return new PrimitiveExpression((int)((PrimitiveExpression)Expression).Value != 0);
				else if (Type.Kind == TypeKind.Pointer)
					return new BinaryOperatorExpression(Expression, BinaryOperatorType.InEquality, new NullReferenceExpression());
				else
					return new BinaryOperatorExpression(Expression, BinaryOperatorType.InEquality, new PrimitiveExpression(0));
			}
		}

		public Expression Convert(ILInstruction inst)
		{
			return ConvertArgument(inst).Expression;
		}
		
		public Expression Convert(ILInstruction inst, IType expectedType)
		{
			return ConvertArgument(inst).ConvertTo(expectedType, this);
		}
		
		public AstType ConvertType(Mono.Cecil.TypeReference type)
		{
			if (type == null)
				return null;
			return astBuilder.ConvertType(cecilMapper.GetType(type));
		}
		
		ConvertedExpression ConvertVariable(ILVariable variable)
		{
			Expression expr;
			if (variable.Kind == VariableKind.This)
				expr = new ThisReferenceExpression();
			else
				expr = new IdentifierExpression(variable.Name);
			expr.AddAnnotation(variable);
			if (variable.Type.SkipModifiers().MetadataType == Mono.Cecil.MetadataType.ByReference)
			{
				// When loading a by-ref parameter, use 'ref paramName'.
				// We'll strip away the 'ref' when dereferencing.
				expr = new DirectionExpression(FieldDirection.Ref, expr);
			}
			return new ConvertedExpression(expr, cecilMapper.GetType(variable.Type));
		}
		
		ConvertedExpression ConvertArgument(ILInstruction inst)
		{
			var cexpr = inst.AcceptVisitor(this);
			Debug.Assert(cexpr.Type.GetStackType() == inst.ResultType || cexpr.Type.Kind == TypeKind.Unknown || inst.ResultType == StackType.Void);
			cexpr.Expression.AddAnnotation(inst);
			return cexpr;
		}
		
		ConvertedExpression IsType(IsInst inst)
		{
			var arg = ConvertArgument(inst.Argument);
			return new ConvertedExpression(
				new IsExpression(arg.Expression, ConvertType(inst.Type)),
				compilation.FindType(TypeCode.Boolean));
		}
		
		protected internal override ConvertedExpression VisitIsInst(IsInst inst)
		{
			var arg = ConvertArgument(inst.Argument);
			return new ConvertedExpression(
				new AsExpression(arg.Expression, ConvertType(inst.Type)),
				cecilMapper.GetType(inst.Type));
		}
		
		protected internal override ConvertedExpression VisitNewObj(NewObj inst)
		{
			var oce = new ObjectCreateExpression(ConvertType(inst.Method.DeclaringType));
			oce.Arguments.AddRange(inst.Arguments.Select(i => ConvertArgument(i).Expression));
			return new ConvertedExpression(oce, cecilMapper.GetType(inst.Method.DeclaringType));
		}

		protected internal override ConvertedExpression VisitLdcI4(LdcI4 inst)
		{
			return new ConvertedExpression(
				new PrimitiveExpression(inst.Value),
				compilation.FindType(KnownTypeCode.Int32));
		}
		
		protected internal override ConvertedExpression VisitLdcI8(LdcI8 inst)
		{
			return new ConvertedExpression(
				new PrimitiveExpression(inst.Value),
				compilation.FindType(KnownTypeCode.Int64));
		}
		
		protected internal override ConvertedExpression VisitLdcF(LdcF inst)
		{
			return new ConvertedExpression(
				new PrimitiveExpression(inst.Value),
				compilation.FindType(KnownTypeCode.Double));
		}
		
		protected internal override ConvertedExpression VisitLdStr(LdStr inst)
		{
			return new ConvertedExpression(
				new PrimitiveExpression(inst.Value),
				compilation.FindType(KnownTypeCode.String));
		}
		
		protected internal override ConvertedExpression VisitLdNull(LdNull inst)
		{
			return new ConvertedExpression(
				new NullReferenceExpression(),
				SpecialType.UnknownType);
		}

		protected internal override ConvertedExpression VisitLogicNot(LogicNot inst)
		{
			return LogicNot(new ConvertedExpression(ConvertCondition(inst.Argument), compilation.FindType(KnownTypeCode.Boolean)));
		}
		
		ConvertedExpression LogicNot(ConvertedExpression expr)
		{
			return new ConvertedExpression(
				new UnaryOperatorExpression(UnaryOperatorType.Not, expr.Expression),
				compilation.FindType(KnownTypeCode.Boolean));
		}
		
		protected internal override ConvertedExpression VisitLdLoc(LdLoc inst)
		{
			return ConvertVariable(inst.Variable);
		}
		
		protected internal override ConvertedExpression VisitLdLoca(LdLoca inst)
		{
			var expr = ConvertVariable(inst.Variable);
			return new ConvertedExpression(
				new DirectionExpression(FieldDirection.Ref, expr.Expression),
				new ByReferenceType(expr.Type));
		}
		
		protected internal override ConvertedExpression VisitStLoc(StLoc inst)
		{
			return Assignment(ConvertVariable(inst.Variable), ConvertArgument(inst.Value));
		}
		
		protected internal override ConvertedExpression VisitCeq(Ceq inst)
		{
			// Translate 'e as T == null' to 'e is T'.
			// This is necessary for correctness when T is a value type.
			if (inst.Left.OpCode == OpCode.IsInst && inst.Right.OpCode == OpCode.LdNull) {
				return LogicNot(IsType((IsInst)inst.Left));
			} else if (inst.Right.OpCode == OpCode.IsInst && inst.Left.OpCode == OpCode.LdNull) {
				return LogicNot(IsType((IsInst)inst.Right));
			}
			var left = ConvertArgument(inst.Left);
			var right = ConvertArgument(inst.Right);
			return new ConvertedExpression(
				new BinaryOperatorExpression(left.Expression, BinaryOperatorType.Equality, right.Expression),
				compilation.FindType(TypeCode.Boolean));
		}
		
		protected internal override ConvertedExpression VisitClt(Clt inst)
		{
			return Comparison(inst, BinaryOperatorType.LessThan);
		}
		
		protected internal override ConvertedExpression VisitCgt(Cgt inst)
		{
			return Comparison(inst, BinaryOperatorType.GreaterThan);
		}
		
		protected internal override ConvertedExpression VisitClt_Un(Clt_Un inst)
		{
			return Comparison(inst, BinaryOperatorType.LessThan, un: true);
		}

		protected internal override ConvertedExpression VisitCgt_Un(Cgt_Un inst)
		{
			return Comparison(inst, BinaryOperatorType.GreaterThan, un: true);
		}
		
		ConvertedExpression Comparison(BinaryComparisonInstruction inst, BinaryOperatorType op, bool un = false)
		{
			var left = ConvertArgument(inst.Left);
			var right = ConvertArgument(inst.Right);
			// TODO: ensure the arguments are signed
			// or with _Un: ensure the arguments are unsigned; and that float comparisons are performed unordered
			return new ConvertedExpression(
				new BinaryOperatorExpression(left.Expression, op, right.Expression),
				compilation.FindType(TypeCode.Boolean));
		}
		
		ConvertedExpression Assignment(ConvertedExpression left, ConvertedExpression right)
		{
			return new ConvertedExpression(
				new AssignmentExpression(left.Expression, right.ConvertTo(left.Type, this)),
				left.Type);
		}
		
		protected internal override ConvertedExpression VisitAdd(Add inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.Add);
		}
		
		protected internal override ConvertedExpression VisitSub(Sub inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.Subtract);
		}
		
		protected internal override ConvertedExpression VisitMul(Mul inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.Multiply);
		}
		
		protected internal override ConvertedExpression VisitDiv(Div inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.Divide);
		}
		
		protected internal override ConvertedExpression VisitRem(Rem inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.Modulus);
		}
		
		protected internal override ConvertedExpression VisitBitXor(BitXor inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.ExclusiveOr);
		}
		
		protected internal override ConvertedExpression VisitBitAnd(BitAnd inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.BitwiseAnd);
		}
		
		protected internal override ConvertedExpression VisitBitOr(BitOr inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.BitwiseOr);
		}
		
		protected internal override ConvertedExpression VisitShl(Shl inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.ShiftLeft);
		}
		
		protected internal override ConvertedExpression VisitShr(Shr inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.ShiftRight);
		}
		
		ConvertedExpression HandleBinaryNumeric(BinaryNumericInstruction inst, BinaryOperatorType op)
		{
			var left = ConvertArgument(inst.Left);
			var right = ConvertArgument(inst.Right);
			var rr = resolver.ResolveBinaryOperator(op, new ResolveResult(left.Type), new ResolveResult(right.Type));
			if (!rr.IsError && rr.Type.GetStackType() == inst.ResultType
			    && IsCompatibleWithSign(left.Type, inst.Sign) && IsCompatibleWithSign(right.Type, inst.Sign))
			{
				return new ConvertedExpression(
					new BinaryOperatorExpression(left.Expression, op, right.Expression),
					rr.Type);
			}
			IType targetType = compilation.FindType(inst.ResultType.ToKnownTypeCode(inst.Sign));
			return new ConvertedExpression(
				new BinaryOperatorExpression(left.ConvertTo(targetType, this), op, right.ConvertTo(targetType, this)),
				targetType);
		}

		/// <summary>
		/// Gets whether <paramref name="type"/> has the specified <paramref name="sign"/>.
		/// If <paramref name="sign"/> is None, always returns true.
		/// </summary>
		bool IsCompatibleWithSign(IType type, Sign sign)
		{
			return sign == Sign.None || type.GetSign() == sign;
		}
		
		protected internal override ConvertedExpression VisitConv(Conv inst)
		{
			var arg = ConvertArgument(inst.Argument);
			Expression input = arg.Expression;
			if (arg.Type.GetSign() != inst.Sign) {
				// we need to cast the input to a type of appropriate sign
				var inputType = inst.Argument.ResultType.ToKnownTypeCode(inst.Sign);
				input = arg.ConvertTo(compilation.FindType(inputType), this);
			}
			var targetType = compilation.FindType(inst.TargetType.ToKnownTypeCode());
			return new ConvertedExpression(
				new CastExpression(astBuilder.ConvertType(targetType), input),
				targetType);
		}
		
		protected internal override ConvertedExpression VisitCall(Call inst)
		{
			return HandleCallInstruction(inst);
		}
		
		protected internal override ConvertedExpression VisitCallVirt(CallVirt inst)
		{
			return HandleCallInstruction(inst);
		}
		
		ConvertedExpression HandleCallInstruction(CallInstruction inst)
		{
			Expression target;
			if (inst.Method.HasThis) {
				var argInstruction = inst.Arguments[0];
				if (inst.OpCode == OpCode.Call && argInstruction.MatchLdThis())
					target = new BaseReferenceExpression().WithAnnotation(argInstruction);
				else
					target = Convert(argInstruction);
			} else {
				target = new TypeReferenceExpression(ConvertType(inst.Method.DeclaringType));
			}
			InvocationExpression invocation = target.Invoke(inst.Method.Name);
			int firstParamIndex = inst.Method.HasThis ? 1 : 0;
			for (int i = firstParamIndex; i < inst.Arguments.Count; i++) {
				var p = inst.Method.Parameters[i - firstParamIndex];
				var arg = ConvertArgument(inst.Arguments[i]);
				var type = cecilMapper.GetType(p.ParameterType);
				invocation.Arguments.Add(arg.ConvertTo(type, this));
			}
			return new ConvertedExpression(invocation, cecilMapper.GetType(inst.Method.ReturnType));
		}
		
		protected internal override ConvertedExpression VisitLdObj(LdObj inst)
		{
			var target = ConvertArgument(inst.Target);
			var type = cecilMapper.GetType(inst.Type);
			if (target.Type.Equals(new ByReferenceType(type)) && target.Expression is DirectionExpression)
			{
				// we can deference the managed reference by stripping away the 'ref'
				return new ConvertedExpression(
					((DirectionExpression)target.Expression).Expression.Detach(),
					type);
			}
			return new ConvertedExpression(
				new UnaryOperatorExpression(
					UnaryOperatorType.Dereference, target.ConvertTo(new PointerType(type), this)),
				type);
		}

		protected override ConvertedExpression Default(ILInstruction inst)
		{
			return ErrorExpression("OpCode not supported: " + inst.OpCode);
		}

		static ConvertedExpression ErrorExpression(string message)
		{
			var e = new ErrorExpression();
			e.AddChild(new Comment(message, CommentType.MultiLine), Roles.Comment);
			return new ConvertedExpression(e, SpecialType.UnknownType);
		}

		public Expression ConvertCondition(ILInstruction condition)
		{
			var expr = ConvertArgument(condition);
			return expr.ConvertToBoolean();
		}
	}
}
