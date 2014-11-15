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
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ExpressionType = System.Linq.Expressions.ExpressionType;
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
	class ExpressionBuilder : ILVisitor<TranslatedExpression>
	{
		internal readonly ICompilation compilation;
		internal readonly CSharpResolver resolver;
		internal readonly TypeSystemAstBuilder astBuilder;
		
		public ExpressionBuilder(ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext != null);
			this.compilation = decompilationContext.Compilation;
			this.resolver = new CSharpResolver(new CSharpTypeResolveContext(compilation.MainAssembly, null, decompilationContext.CurrentTypeDefinition, decompilationContext.CurrentMember));
			this.astBuilder = new TypeSystemAstBuilder(resolver);
			this.astBuilder.AlwaysUseShortTypeNames = true;
			this.astBuilder.AddResolveResultAnnotations = true;
		}

		public AstType ConvertType(IType type)
		{
			var astType = astBuilder.ConvertType(type);
			astType.AddAnnotation(new TypeResolveResult(type));
			return astType;
		}
		
		public TranslatedExpression Translate(ILInstruction inst)
		{
			Debug.Assert(inst != null);
			var cexpr = inst.AcceptVisitor(this);
			Debug.Assert(cexpr.Type.GetStackType() == inst.ResultType || cexpr.Type.Kind == TypeKind.Unknown || inst.ResultType == StackType.Void);
			return cexpr;
		}
		
		public TranslatedExpression TranslateCondition(ILInstruction condition)
		{
			var expr = Translate(condition);
			return expr.ConvertToBoolean(this);
		}
		
		ExpressionWithResolveResult ConvertVariable(ILVariable variable)
		{
			Expression expr;
			if (variable.Kind == VariableKind.This)
				expr = new ThisReferenceExpression();
			else
				expr = new IdentifierExpression(variable.Name);
			// TODO: use LocalResolveResult instead
			if (variable.Type.Kind == TypeKind.ByReference) {
				// When loading a by-ref parameter, use 'ref paramName'.
				// We'll strip away the 'ref' when dereferencing.
				
				// Ensure that the IdentifierExpression itself also gets a resolve result, as that might
				// get used after the 'ref' is stripped away:
				var elementType = ((ByReferenceType)variable.Type).ElementType;
				expr.WithRR(new ResolveResult(elementType));
				
				expr = new DirectionExpression(FieldDirection.Ref, expr);
			}
			return expr.WithRR(new ResolveResult(variable.Type));
		}
		
		TranslatedExpression IsType(IsInst inst)
		{
			var arg = Translate(inst.Argument);
			return new IsExpression(arg.Expression, ConvertType(inst.Type))
				.WithILInstruction(inst)
				.WithRR(new TypeIsResolveResult(arg.ResolveResult, inst.Type, compilation.FindType(TypeCode.Boolean)));
		}
		
		protected internal override TranslatedExpression VisitIsInst(IsInst inst)
		{
			var arg = Translate(inst.Argument);
			return new AsExpression(arg.Expression, ConvertType(inst.Type))
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(inst.Type, arg.ResolveResult, Conversion.TryCast));
		}
		
		protected internal override TranslatedExpression VisitNewObj(NewObj inst)
		{
			return HandleCallInstruction(inst);
		}

		protected internal override TranslatedExpression VisitLdcI4(LdcI4 inst)
		{
			return new PrimitiveExpression(inst.Value)
				.WithILInstruction(inst)
				.WithRR(new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), inst.Value));
		}
		
		protected internal override TranslatedExpression VisitLdcI8(LdcI8 inst)
		{
			return new PrimitiveExpression(inst.Value)
				.WithILInstruction(inst)
				.WithRR(new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int64), inst.Value));
		}
		
		protected internal override TranslatedExpression VisitLdcF(LdcF inst)
		{
			return new PrimitiveExpression(inst.Value)
				.WithILInstruction(inst)
				.WithRR(new ConstantResolveResult(compilation.FindType(KnownTypeCode.Double), inst.Value));
		}
		
		protected internal override TranslatedExpression VisitLdStr(LdStr inst)
		{
			return new PrimitiveExpression(inst.Value)
				.WithILInstruction(inst)
				.WithRR(new ConstantResolveResult(compilation.FindType(KnownTypeCode.String), inst.Value));
		}
		
		protected internal override TranslatedExpression VisitLdNull(LdNull inst)
		{
			return new NullReferenceExpression()
				.WithILInstruction(inst)
				.WithRR(new ConstantResolveResult(SpecialType.NullType, null));
		}
		
		protected internal override TranslatedExpression VisitLogicNot(LogicNot inst)
		{
			return LogicNot(TranslateCondition(inst.Argument)).WithILInstruction(inst);
		}
		
		ExpressionWithResolveResult LogicNot(TranslatedExpression expr)
		{
			return new UnaryOperatorExpression(UnaryOperatorType.Not, expr.Expression)
				.WithRR(new OperatorResolveResult(compilation.FindType(KnownTypeCode.Boolean), ExpressionType.Not));
		}
		
		protected internal override TranslatedExpression VisitLdLoc(LdLoc inst)
		{
			return ConvertVariable(inst.Variable).WithILInstruction(inst);
		}
		
		protected internal override TranslatedExpression VisitLdLoca(LdLoca inst)
		{
			var expr = ConvertVariable(inst.Variable).WithILInstruction(inst);
			// Note that we put the instruction on the IdentifierExpression instead of the DirectionExpression,
			// because the DirectionExpression might get removed by dereferencing instructions such as LdObj
			return new DirectionExpression(FieldDirection.Ref, expr.Expression)
				.WithoutILInstruction()
				.WithRR(new ByReferenceResolveResult(expr.ResolveResult, isOut: false));
		}
		
		protected internal override TranslatedExpression VisitStLoc(StLoc inst)
		{
			return Assignment(ConvertVariable(inst.Variable).WithoutILInstruction(), Translate(inst.Value)).WithILInstruction(inst);
		}
		
		protected internal override TranslatedExpression VisitCeq(Ceq inst)
		{
			// Translate '(e as T) == null' to '!(e is T)'.
			// This is necessary for correctness when T is a value type.
			if (inst.Left.OpCode == OpCode.IsInst && inst.Right.OpCode == OpCode.LdNull) {
				return LogicNot(IsType((IsInst)inst.Left)).WithILInstruction(inst);
			} else if (inst.Right.OpCode == OpCode.IsInst && inst.Left.OpCode == OpCode.LdNull) {
				return LogicNot(IsType((IsInst)inst.Right)).WithILInstruction(inst);
			}
			
			var left = Translate(inst.Left);
			var right = Translate(inst.Right);
			
			// Remove redundant bool comparisons
			if (left.Type.IsKnownType(KnownTypeCode.Boolean)) {
				if (inst.Right.MatchLdcI4(0))
					return LogicNot(left).WithILInstruction(inst); // 'b == 0' => '!b'
				if (inst.Right.MatchLdcI4(1))
					return left; // 'b == 1' => 'b'
			} else if (right.Type.IsKnownType(KnownTypeCode.Boolean)) {
				if (inst.Left.MatchLdcI4(0))
					return LogicNot(right).WithILInstruction(inst); // '0 == b' => '!b'
				if (inst.Left.MatchLdcI4(1))
					return right; // '1 == b' => 'b'
			}
			
			var rr = resolver.ResolveBinaryOperator(BinaryOperatorType.Equality, left.ResolveResult, right.ResolveResult);
			if (rr.IsError) {
				// TODO: insert casts to the wider type of the two input types
			}
			return new BinaryOperatorExpression(left.Expression, BinaryOperatorType.Equality, right.Expression)
				.WithILInstruction(inst)
				.WithRR(rr);
		}
		
		protected internal override TranslatedExpression VisitClt(Clt inst)
		{
			return Comparison(inst, BinaryOperatorType.LessThan);
		}
		
		protected internal override TranslatedExpression VisitCgt(Cgt inst)
		{
			return Comparison(inst, BinaryOperatorType.GreaterThan);
		}
		
		protected internal override TranslatedExpression VisitClt_Un(Clt_Un inst)
		{
			return Comparison(inst, BinaryOperatorType.LessThan, un: true);
		}

		protected internal override TranslatedExpression VisitCgt_Un(Cgt_Un inst)
		{
			return Comparison(inst, BinaryOperatorType.GreaterThan, un: true);
		}
		
		TranslatedExpression Comparison(BinaryComparisonInstruction inst, BinaryOperatorType op, bool un = false)
		{
			var left = Translate(inst.Left);
			var right = Translate(inst.Right);
			// TODO: ensure the arguments are signed
			// or with _Un: ensure the arguments are unsigned; and that float comparisons are performed unordered
			return new BinaryOperatorExpression(left.Expression, op, right.Expression)
				.WithILInstruction(inst)
				.WithRR(new OperatorResolveResult(compilation.FindType(TypeCode.Boolean),
				                                  BinaryOperatorExpression.GetLinqNodeType(op, false),
				                                  left.ResolveResult, right.ResolveResult));
		}
		
		ExpressionWithResolveResult Assignment(TranslatedExpression left, TranslatedExpression right)
		{
			right = right.ConvertTo(left.Type, this);
			return new AssignmentExpression(left.Expression, right.Expression)
				.WithRR(new OperatorResolveResult(left.Type, ExpressionType.Assign, left.ResolveResult, right.ResolveResult));
		}
		
		protected internal override TranslatedExpression VisitAdd(Add inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.Add);
		}
		
		protected internal override TranslatedExpression VisitSub(Sub inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.Subtract);
		}
		
		protected internal override TranslatedExpression VisitMul(Mul inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.Multiply);
		}
		
		protected internal override TranslatedExpression VisitDiv(Div inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.Divide);
		}
		
		protected internal override TranslatedExpression VisitRem(Rem inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.Modulus);
		}
		
		protected internal override TranslatedExpression VisitBitXor(BitXor inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.ExclusiveOr);
		}
		
		protected internal override TranslatedExpression VisitBitAnd(BitAnd inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.BitwiseAnd);
		}
		
		protected internal override TranslatedExpression VisitBitOr(BitOr inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.BitwiseOr);
		}
		
		protected internal override TranslatedExpression VisitShl(Shl inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.ShiftLeft);
		}
		
		protected internal override TranslatedExpression VisitShr(Shr inst)
		{
			return HandleBinaryNumeric(inst, BinaryOperatorType.ShiftRight);
		}
		
		TranslatedExpression HandleBinaryNumeric(BinaryNumericInstruction inst, BinaryOperatorType op)
		{
			var resolverWithOverflowCheck = resolver.WithCheckForOverflow(inst.CheckForOverflow);
			var left = Translate(inst.Left);
			var right = Translate(inst.Right);
			var rr = resolverWithOverflowCheck.ResolveBinaryOperator(op, left.ResolveResult, right.ResolveResult);
			if (rr.IsError || rr.Type.GetStackType() != inst.ResultType
			    || !IsCompatibleWithSign(left.Type, inst.Sign) || !IsCompatibleWithSign(right.Type, inst.Sign))
			{
				// Left and right operands are incompatible, so convert them to a common type
				IType targetType = compilation.FindType(inst.ResultType.ToKnownTypeCode(inst.Sign));
				left = left.ConvertTo(targetType, this);
				right = right.ConvertTo(targetType, this);
				rr = resolverWithOverflowCheck.ResolveBinaryOperator(op, left.ResolveResult, right.ResolveResult);
			}
			return new BinaryOperatorExpression(left.Expression, op, right.Expression)
				.WithILInstruction(inst)
				.WithRR(rr);
		}

		/// <summary>
		/// Gets whether <paramref name="type"/> has the specified <paramref name="sign"/>.
		/// If <paramref name="sign"/> is None, always returns true.
		/// </summary>
		bool IsCompatibleWithSign(IType type, Sign sign)
		{
			return sign == Sign.None || type.GetSign() == sign;
		}
		
		protected internal override TranslatedExpression VisitConv(Conv inst)
		{
			var arg = Translate(inst.Argument);
			if (arg.Type.GetSign() != inst.Sign) {
				// we need to cast the input to a type of appropriate sign
				var inputType = inst.Argument.ResultType.ToKnownTypeCode(inst.Sign);
				arg = arg.ConvertTo(compilation.FindType(inputType), this);
			}
			var targetType = compilation.FindType(inst.TargetType.ToKnownTypeCode());
			var rr = resolver.WithCheckForOverflow(inst.CheckForOverflow).ResolveCast(targetType, arg.ResolveResult);
			return new CastExpression(ConvertType(targetType), arg.Expression)
				.WithILInstruction(inst)
				.WithRR(rr);
		}
		
		protected internal override TranslatedExpression VisitCall(Call inst)
		{
			return HandleCallInstruction(inst);
		}
		
		protected internal override TranslatedExpression VisitCallVirt(CallVirt inst)
		{
			return HandleCallInstruction(inst);
		}
		
		TranslatedExpression HandleCallInstruction(CallInstruction inst)
		{
			// Used for Call, CallVirt and NewObj
			TranslatedExpression target;
			if (inst.OpCode == OpCode.NewObj) {
				target = default(TranslatedExpression); // no target
			} else if (!inst.Method.IsStatic) {
				var argInstruction = inst.Arguments[0];
				if (inst.OpCode == OpCode.Call && argInstruction.MatchLdThis()) {
					target = new BaseReferenceExpression()
						.WithILInstruction(argInstruction)
						.WithRR(new ThisResolveResult(inst.Method.DeclaringType, causesNonVirtualInvocation: true));
				} else {
					target = Translate(argInstruction);
				}
			} else {
				target = new TypeReferenceExpression(ConvertType(inst.Method.DeclaringType))
					.WithoutILInstruction()
					.WithRR(new TypeResolveResult(inst.Method.DeclaringType));
			}
			
			var arguments = inst.Arguments.SelectArray(Translate);
			int firstParamIndex = (inst.Method.IsStatic || inst.OpCode == OpCode.NewObj) ? 0 : 1;
			
			// Translate arguments to the expected parameter types
			Debug.Assert(arguments.Length == firstParamIndex + inst.Method.Parameters.Count);
			for (int i = firstParamIndex; i < arguments.Length; i++) {
				var parameter = inst.Method.Parameters[i - firstParamIndex];
				arguments[i] = arguments[i].ConvertTo(parameter.Type, this);
			}
			var argumentResolveResults = arguments.Skip(firstParamIndex).Select(arg => arg.ResolveResult).ToList();
			
			var rr = new CSharpInvocationResolveResult(target.ResolveResult, inst.Method, argumentResolveResults);
			
			var argumentExpressions = arguments.Skip(firstParamIndex).Select(arg => arg.Expression);
			if (inst.OpCode == OpCode.NewObj) {
				return new ObjectCreateExpression(ConvertType(inst.Method.DeclaringType), argumentExpressions)
					.WithILInstruction(inst).WithRR(rr);
			} else {
				var mre = new MemberReferenceExpression(target.Expression, inst.Method.Name);
				return new InvocationExpression(mre, argumentExpressions)
					.WithILInstruction(inst).WithRR(rr);
			}
		}
		
		protected internal override TranslatedExpression VisitLdObj(LdObj inst)
		{
			var target = Translate(inst.Target);
			if (target.Type.Equals(new ByReferenceType(inst.Type)) && target.Expression is DirectionExpression) {
				// we can deference the managed reference by stripping away the 'ref'
				var result = target.UnwrapChild(((DirectionExpression)target.Expression).Expression);
				result = result.ConvertTo(inst.Type, this);
				result.Expression.AddAnnotation(inst); // add LdObj in addition to the existing ILInstruction annotation
				return result;
			} else {
				// Cast pointer type if necessary:
				target = target.ConvertTo(new PointerType(inst.Type), this);
				return new UnaryOperatorExpression(UnaryOperatorType.Dereference, target.Expression)
					.WithILInstruction(inst)
					.WithRR(new ResolveResult(inst.Type));
			}
		}

		protected internal override TranslatedExpression VisitStObj(StObj inst)
		{
			var target = Translate(inst.Target);
			var value = Translate(inst.Value);
			TranslatedExpression result;
			if (target.Type.Equals(new ByReferenceType(inst.Type)) && target.Expression is DirectionExpression) {
				// we can deference the managed reference by stripping away the 'ref'
				result = target.UnwrapChild(((DirectionExpression)target.Expression).Expression);
			} else {
				// Cast pointer type if necessary:
				target = target.ConvertTo(new PointerType(inst.Type), this);
				result = new UnaryOperatorExpression(UnaryOperatorType.Dereference, target.Expression)
					.WithoutILInstruction()
					.WithRR(new ResolveResult(inst.Type));
			}
			return Assignment(result, value).WithILInstruction(inst);
		}

		protected internal override TranslatedExpression VisitUnboxAny(UnboxAny inst)
		{
			var arg = Translate(inst.Argument);
			if (arg.Type.IsReferenceType != true) {
				// ensure we treat the input as a reference type
				arg = arg.ConvertTo(compilation.FindType(KnownTypeCode.Object), this);
			}
			return new CastExpression(ConvertType(inst.Type), arg.Expression)
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(inst.Type, arg.ResolveResult, Conversion.UnboxingConversion));
		}

		protected internal override TranslatedExpression VisitBox(Box inst)
		{
			var obj = compilation.FindType(KnownTypeCode.Object);
			var arg = Translate(inst.Argument).ConvertTo(inst.Type, this);
			return new CastExpression(ConvertType(obj), arg.Expression)
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(obj, arg.ResolveResult, Conversion.BoxingConversion));
		}

		protected override TranslatedExpression Default(ILInstruction inst)
		{
			return ErrorExpression("OpCode not supported: " + inst.OpCode);
		}

		static TranslatedExpression ErrorExpression(string message)
		{
			var e = new ErrorExpression();
			e.AddChild(new Comment(message, CommentType.MultiLine), Roles.Comment);
			return e.WithoutILInstruction().WithRR(ErrorResolveResult.UnknownError);
		}
	}
}
