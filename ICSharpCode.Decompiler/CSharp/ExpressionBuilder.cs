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

namespace ICSharpCode.Decompiler.CSharp
{
	/// <summary>
	/// Translates from ILAst to C# expressions.
	/// </summary>
	class ExpressionBuilder : ILVisitor<TranslatedExpression>
	{
		internal readonly ICompilation compilation;
		internal readonly CSharpResolver resolver;
		readonly TypeSystemAstBuilder astBuilder;
		
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
			Debug.Assert(astType.Annotation<TypeResolveResult>() != null);
			return astType;
		}
		
		public ExpressionWithResolveResult ConvertConstantValue(ResolveResult rr)
		{
			var expr = astBuilder.ConvertConstantValue(rr);
			var exprRR = expr.Annotation<ResolveResult>();
			if (exprRR == null) {
				exprRR = rr;
				expr.AddAnnotation(rr);
			}
			return new ExpressionWithResolveResult(expr, exprRR);
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

		ExpressionWithResolveResult ConvertField(IField field, ILInstruction target = null)
		{
			return new MemberReferenceExpression(TranslateTarget(field, target, true), field.Name)
				.WithRR(new ResolveResult(field.ReturnType));
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
		
		protected internal override TranslatedExpression VisitNewArr(NewArr inst)
		{
			var dimensions = inst.Indices.Count;
			var args = inst.Indices.Select(arg => Translate(arg)).ToArray();
			var expr = new ArrayCreateExpression { Type = ConvertType(inst.Type) };
			expr.Arguments.AddRange(args.Select(arg => arg.Expression));
			return expr.WithILInstruction(inst)
				.WithRR(new ArrayCreateResolveResult(new ArrayType(compilation, inst.Type, dimensions), args.Select(a => a.ResolveResult).ToList(), new ResolveResult[0]));
		}
		
		protected internal override TranslatedExpression VisitLocAlloc(LocAlloc inst)
		{
			var byteType = compilation.FindType(KnownTypeCode.Byte);
			return new StackAllocExpression {
				Type = ConvertType(byteType),
				CountExpression = Translate(inst.Argument)
			}.WithILInstruction(inst).WithRR(new ResolveResult(new PointerType(byteType)));
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
		
		protected internal override TranslatedExpression VisitDefaultValue(DefaultValue inst)
		{
			return new DefaultValueExpression(ConvertType(inst.Type))
				.WithILInstruction(inst)
				.WithRR(new ConstantResolveResult(inst.Type, null));
		}
		
		protected internal override TranslatedExpression VisitSizeOf(SizeOf inst)
		{
			return new SizeOfExpression(ConvertType(inst.Type))
				.WithILInstruction(inst)
				.WithRR(new SizeOfResolveResult(compilation.FindType(KnownTypeCode.Int32), inst.Type, null));
		}
		
		protected internal override TranslatedExpression VisitLdTypeToken(LdTypeToken inst)
		{
			return new TypeOfExpression(ConvertType(inst.Type)).Member("TypeHandle")
				.WithILInstruction(inst)
				.WithRR(new TypeOfResolveResult(compilation.FindType(new TopLevelTypeName("System", "RuntimeTypeHandle")), inst.Type));
		}
		
		protected internal override TranslatedExpression VisitLogicNot(LogicNot inst)
		{
			return LogicNot(TranslateCondition(inst.Argument)).WithILInstruction(inst);
		}
		
		protected internal override TranslatedExpression VisitBitNot(BitNot inst)
		{
			var argument = Translate(inst.Argument);
			return new UnaryOperatorExpression(UnaryOperatorType.BitNot, argument)
				.WithRR(argument.ResolveResult)
				.WithILInstruction(inst);
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
				var targetType = DecompilerTypeSystemUtils.GetLargerType(left.Type, right.Type);
				if (targetType.Equals(left.Type)) {
					right = right.ConvertTo(targetType, this);
				} else {
					left = left.ConvertTo(targetType, this);
				}
				rr = new OperatorResolveResult(compilation.FindType(KnownTypeCode.Boolean),
				                               BinaryOperatorExpression.GetLinqNodeType(BinaryOperatorType.Equality, false),
				                               left.ResolveResult, right.ResolveResult);
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
			ResolveResult rr;
			if (left.Type.IsKnownType(KnownTypeCode.IntPtr) || left.Type.IsKnownType(KnownTypeCode.UIntPtr)
			    || right.Type.IsKnownType(KnownTypeCode.IntPtr) || right.Type.IsKnownType(KnownTypeCode.UIntPtr)) {
				IType targetType;
				if (inst.Sign == Sign.Unsigned) {
					targetType = compilation.FindType(KnownTypeCode.UInt64);
				} else {
					targetType = compilation.FindType(KnownTypeCode.Int64);
				}
				left = left.ConvertTo(targetType, this);
				right = right.ConvertTo(targetType, this);
				rr = new OperatorResolveResult(targetType, BinaryOperatorExpression.GetLinqNodeType(op, inst.CheckForOverflow), left.ResolveResult, right.ResolveResult);
				var resultExpr = new BinaryOperatorExpression(left.Expression, op, right.Expression)
					.WithILInstruction(inst)
					.WithRR(rr);
				return resultExpr.ConvertTo(compilation.FindType(inst.ResultType.ToKnownTypeCode()), this);
			} else {
				rr = resolverWithOverflowCheck.ResolveBinaryOperator(op, left.ResolveResult, right.ResolveResult);
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

		static bool IsDelegateConstruction(CallInstruction inst)
		{
			return inst.Arguments.Count == 2
				&& (inst.Arguments[1].OpCode == OpCode.LdFtn
				    || inst.Arguments[1].OpCode == OpCode.LdVirtFtn)
				&& inst.Method.DeclaringType.Kind == TypeKind.Delegate;
		}

		TranslatedExpression HandleDelegateConstruction(CallInstruction inst)
		{
			ILInstruction func = inst.Arguments[1];
			IMethod method;
			if (func.OpCode == OpCode.LdFtn) {
				method = ((LdFtn)func).Method;
			} else {
				method = ((LdVirtFtn)func).Method;
			}
			var target = TranslateTarget(method, inst.Arguments[0], func.OpCode == OpCode.LdFtn);
			return new ObjectCreateExpression(ConvertType(inst.Method.DeclaringType), new MemberReferenceExpression(target, method.Name))
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(method.DeclaringType,
				                                    new MemberResolveResult(target.ResolveResult, method),
				                                    Conversion.MethodGroupConversion(method, func.OpCode == OpCode.LdVirtFtn, false))); // TODO handle extension methods capturing the first argument
		}
		
		TranslatedExpression TranslateTarget(IMember member, ILInstruction target, bool nonVirtualInvocation)
		{
			if (!member.IsStatic) {
				if (nonVirtualInvocation && target.MatchLdThis() && member.DeclaringTypeDefinition != resolver.CurrentTypeDefinition) {
					return new BaseReferenceExpression()
						.WithILInstruction(target)
						.WithRR(new ThisResolveResult(member.DeclaringType, nonVirtualInvocation));
				} else {
					var translatedTarget = Translate(target);
					if (member.DeclaringType.IsReferenceType == false && translatedTarget.Expression is DirectionExpression) {
						translatedTarget = translatedTarget.UnwrapChild(((DirectionExpression)translatedTarget).Expression);
					}
					return translatedTarget.ConvertTo(member.DeclaringType, this);
				}
			} else {
				return new TypeReferenceExpression(ConvertType(member.DeclaringType))
					.WithoutILInstruction()
					.WithRR(new TypeResolveResult(member.DeclaringType));
			}
		}
		
		TranslatedExpression HandleCallInstruction(CallInstruction inst)
		{
			// Used for Call, CallVirt and NewObj
			TranslatedExpression target;
			if (inst.OpCode == OpCode.NewObj) {
				if (IsDelegateConstruction(inst)) {
					return HandleDelegateConstruction(inst);
				}
				target = default(TranslatedExpression); // no target
			} else {
				target = TranslateTarget(inst.Method, inst.Arguments.FirstOrDefault(), inst.OpCode == OpCode.Call);
			}
			
			var arguments = inst.Arguments.SelectArray(Translate);
			int firstParamIndex = (inst.Method.IsStatic || inst.OpCode == OpCode.NewObj) ? 0 : 1;
			
			// Translate arguments to the expected parameter types
			Debug.Assert(arguments.Length == firstParamIndex + inst.Method.Parameters.Count);
			for (int i = firstParamIndex; i < arguments.Length; i++) {
				var parameter = inst.Method.Parameters[i - firstParamIndex];
				arguments[i] = arguments[i].ConvertTo(parameter.Type, this);
				
				if (parameter.IsOut && arguments[i].Expression is DirectionExpression) {
					((DirectionExpression)arguments[i].Expression).FieldDirection = FieldDirection.Out;
				}
			}

			var argumentResolveResults = arguments.Skip(firstParamIndex).Select(arg => arg.ResolveResult).ToList();

			ResolveResult rr;
			if (inst.Method.IsAccessor)
				rr = new MemberResolveResult(target.ResolveResult, inst.Method.AccessorOwner);
			else
				rr = new CSharpInvocationResolveResult(target.ResolveResult, inst.Method, argumentResolveResults);
			
			var argumentExpressions = arguments.Skip(firstParamIndex).Select(arg => arg.Expression).ToList();
			if (inst.OpCode == OpCode.NewObj) {
				return new ObjectCreateExpression(ConvertType(inst.Method.DeclaringType), argumentExpressions)
					.WithILInstruction(inst).WithRR(rr);
			} else {
				Expression expr;
				IMethod method = inst.Method;
				if (method.IsAccessor) {
					if (method.ReturnType.IsKnownType(KnownTypeCode.Void)) {
						var value = argumentExpressions.Last();
						argumentExpressions.Remove(value);
						if (argumentExpressions.Count == 0)
							expr = new MemberReferenceExpression(target.Expression, method.AccessorOwner.Name);
						else
							expr = new IndexerExpression(target.Expression, argumentExpressions);
						expr = new AssignmentExpression(expr, value);
					} else {
						if (argumentExpressions.Count == 0)
							expr = new MemberReferenceExpression(target.Expression, method.AccessorOwner.Name);
						else
							expr = new IndexerExpression(target.Expression, argumentExpressions);
					}
				} else {
					var mre = new MemberReferenceExpression(target.Expression, inst.Method.Name);
					mre.TypeArguments.AddRange(inst.Method.TypeArguments.Select(a => ConvertType(a)));
					expr = new InvocationExpression(mre, argumentExpressions);
				}
				return expr.WithILInstruction(inst).WithRR(rr);
			}
		}
		
		protected internal override TranslatedExpression VisitLdObj(LdObj inst)
		{
			var target = Translate(inst.Target);
			if (target.Expression is DirectionExpression && IsCompatibleTypeForMemoryAccess(target.Type, inst.Type, isWrite: false)) {
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
			if (target.Expression is DirectionExpression && IsCompatibleTypeForMemoryAccess(target.Type, inst.Type, isWrite: true)) {
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

		/// <summary>
		/// Gets whether reading/writing an element of accessType from the pointer
		/// is equivalent to reading/writing an element of the pointer's element type.
		/// </summary>
		bool IsCompatibleTypeForMemoryAccess(IType pointerType, IType accessType, bool isWrite)
		{
			IType memoryType;
			if (pointerType is PointerType)
				memoryType = ((PointerType)pointerType).ElementType;
			else if (pointerType is ByReferenceType)
				memoryType = ((ByReferenceType)pointerType).ElementType;
			else
				return false;
			if (memoryType.Equals(accessType))
				return true;
			// If the types are not equal, the access still might produce equal results:
			if (memoryType.IsReferenceType == true && accessType.IsReferenceType == true)
				return true;
			ITypeDefinition memoryTypeDef = memoryType.GetDefinition();
			if (memoryTypeDef != null) {
				switch (memoryTypeDef.KnownTypeCode) {
					case KnownTypeCode.Byte:
					case KnownTypeCode.SByte:
						// Reading small integers of different signs is not equivalent due to sign extension,
						// but writes are equivalent (truncation works the same for signed and unsigned)
						return isWrite && (accessType.IsKnownType(KnownTypeCode.Byte) || accessType.IsKnownType(KnownTypeCode.SByte));
					case KnownTypeCode.Int16:
					case KnownTypeCode.UInt16:
						return isWrite && (accessType.IsKnownType(KnownTypeCode.Int16) || accessType.IsKnownType(KnownTypeCode.UInt16));
					case KnownTypeCode.Int32:
					case KnownTypeCode.UInt32:
						return isWrite && (accessType.IsKnownType(KnownTypeCode.Int32) || accessType.IsKnownType(KnownTypeCode.UInt32));
					case KnownTypeCode.IntPtr:
					case KnownTypeCode.UIntPtr:
						return accessType.IsKnownType(KnownTypeCode.IntPtr) || accessType.IsKnownType(KnownTypeCode.UIntPtr);
					case KnownTypeCode.Int64:
					case KnownTypeCode.UInt64:
						return accessType.IsKnownType(KnownTypeCode.Int64) || accessType.IsKnownType(KnownTypeCode.UInt64);
				}
			}
			return false;
		}
		
		protected internal override TranslatedExpression VisitLdFld(LdFld inst)
		{
			return ConvertField(inst.Field, inst.Target).WithILInstruction(inst);
		}

		protected internal override TranslatedExpression VisitStFld(StFld inst)
		{
			return Assignment(ConvertField(inst.Field, inst.Target).WithoutILInstruction(), Translate(inst.Value)).WithILInstruction(inst);
		}
		
		protected internal override TranslatedExpression VisitLdsFld(LdsFld inst)
		{
			return ConvertField(inst.Field).WithILInstruction(inst);
		}
		
		protected internal override TranslatedExpression VisitStsFld(StsFld inst)
		{
			return Assignment(ConvertField(inst.Field).WithoutILInstruction(), Translate(inst.Value)).WithILInstruction(inst);
		}

		protected internal override TranslatedExpression VisitLdLen(LdLen inst)
		{
			TranslatedExpression arrayExpr = Translate(inst.Array);
			// TODO: what if arrayExpr is not an array type?
			var lenExpr = arrayExpr.Expression.Member("LongLength")
				.WithILInstruction(inst)
				.WithRR(new ResolveResult(compilation.FindType(KnownTypeCode.Int64)));
			return lenExpr.ConvertTo(compilation.FindType(KnownTypeCode.IntPtr), this);
		}
		
		protected internal override TranslatedExpression VisitLdFlda(LdFlda inst)
		{
			var expr = ConvertField(inst.Field, inst.Target);
			return new DirectionExpression(FieldDirection.Ref, expr)
				.WithoutILInstruction().WithRR(new ResolveResult(new ByReferenceType(expr.Type)));
		}
		
		protected internal override TranslatedExpression VisitLdsFlda(LdsFlda inst)
		{
			var expr = ConvertField(inst.Field);
			return new DirectionExpression(FieldDirection.Ref, expr)
				.WithoutILInstruction().WithRR(new ResolveResult(new ByReferenceType(expr.Type)));
		}
		
		protected internal override TranslatedExpression VisitLdElema(LdElema inst)
		{
			TranslatedExpression arrayExpr = Translate(inst.Array);
			var arrayType = arrayExpr.Type as ArrayType;
			// TODO: what if arrayExpr is not an array type?
			// TODO: what if the type of the ldelema instruction does not match the array type?
			TranslatedExpression expr = new IndexerExpression(arrayExpr, inst.Indices.Select(i => Translate(i).Expression))
				.WithILInstruction(inst).WithRR(new ResolveResult(arrayType != null ? arrayType.ElementType : SpecialType.UnknownType));
			return new DirectionExpression(FieldDirection.Ref, expr)
				.WithoutILInstruction().WithRR(new ResolveResult(new ByReferenceType(expr.Type)));
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
		
		protected internal override TranslatedExpression VisitUnbox(Unbox inst)
		{
			var arg = Translate(inst.Argument);
			var castExpression = new CastExpression(ConvertType(inst.Type), arg.Expression)
				.WithRR(new ConversionResolveResult(inst.Type, arg.ResolveResult, Conversion.UnboxingConversion));
			return new DirectionExpression(FieldDirection.Ref, castExpression)
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(new ByReferenceType(inst.Type), arg.ResolveResult, Conversion.UnboxingConversion));
		}

		protected internal override TranslatedExpression VisitBox(Box inst)
		{
			var obj = compilation.FindType(KnownTypeCode.Object);
			var arg = Translate(inst.Argument).ConvertTo(inst.Type, this);
			return new CastExpression(ConvertType(obj), arg.Expression)
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(obj, arg.ResolveResult, Conversion.BoxingConversion));
		}
		
		protected internal override TranslatedExpression VisitCastClass(CastClass inst)
		{
			return Translate(inst.Argument).ConvertTo(inst.Type, this);
		}
		
		protected internal override TranslatedExpression VisitArglist(Arglist inst)
		{
			return new UndocumentedExpression { UndocumentedExpressionType = UndocumentedExpressionType.ArgListAccess }
			.WithILInstruction(inst)
				.WithRR(new TypeResolveResult(compilation.FindType(new TopLevelTypeName("System", "RuntimeArgumentHandle"))));
		}
		
		protected internal override TranslatedExpression VisitMakeRefAny(MakeRefAny inst)
		{
			var arg = Translate(inst.Argument).Expression;
			if (arg is DirectionExpression) {
				arg = ((DirectionExpression)arg).Expression;
			}
			return new UndocumentedExpression {
				UndocumentedExpressionType = UndocumentedExpressionType.MakeRef,
				Arguments = { arg.Detach() }
			}
			.WithILInstruction(inst)
				.WithRR(new TypeResolveResult(compilation.FindType(new TopLevelTypeName("System", "TypedReference"))));
		}
		
		protected internal override TranslatedExpression VisitRefAnyType(RefAnyType inst)
		{
			return new UndocumentedExpression {
				UndocumentedExpressionType = UndocumentedExpressionType.RefType,
				Arguments = { Translate(inst.Argument).Expression.Detach() }
			}.Member("TypeHandle")
				.WithILInstruction(inst)
				.WithRR(new TypeResolveResult(compilation.FindType(new TopLevelTypeName("System", "RuntimeTypeHandle"))));
		}
		
		protected internal override TranslatedExpression VisitRefAnyValue(RefAnyValue inst)
		{
			var expr = new UndocumentedExpression {
				UndocumentedExpressionType = UndocumentedExpressionType.RefValue,
				Arguments = { Translate(inst.Argument).Expression, new TypeReferenceExpression(ConvertType(inst.Type)) }
			};
			return new DirectionExpression(FieldDirection.Ref, expr.WithILInstruction(inst)).WithoutILInstruction()
				.WithRR(new ByReferenceResolveResult(inst.Type, false));
		}
		
		protected internal override TranslatedExpression VisitBlock(Block block)
		{
			TranslatedExpression expr;
			if (TranslateArrayInitializer(block, out expr))
				return expr;
			
			return base.VisitBlock(block);
		}

		bool TranslateArrayInitializer(Block block, out TranslatedExpression result)
		{
			result = default(TranslatedExpression);
			var stloc = block.Instructions.FirstOrDefault() as StLoc;
			var final = block.FinalInstruction as LdLoc;
			IType type;
			if (stloc == null || final == null || !stloc.Value.MatchNewArr(out type))
				return false;
			if (stloc.Variable != final.Variable)
				return false;
			var newArr = (NewArr)stloc.Value;
			
			var translatedDimensions = newArr.Indices.Select(i => Translate(i)).ToArray();
			
			if (!translatedDimensions.All(dim => dim.ResolveResult.IsCompileTimeConstant))
				return false;
			int dimensions = newArr.Indices.Count;
			int[] dimensionSizes = translatedDimensions.Select(dim => (int)dim.ResolveResult.ConstantValue).ToArray();
			var container = new Stack<ArrayInitializerExpression>();
			var root = new ArrayInitializerExpression();
			container.Push(root);
			var elementResolveResults = new List<ResolveResult>();
			
			for (int i = 1; i < block.Instructions.Count; i++) {
				ILInstruction target, value, array;
				IType t;
				ILVariable v;
				if (!block.Instructions[i].MatchStObj(out target, out value, out t) || !type.Equals(t))
					return false;
				if (!target.MatchLdElema(out t, out array) || !type.Equals(t))
					return false;
				if (!array.MatchLdLoc(out v) || v != final.Variable)
					return false;
				while (container.Count < dimensions) {
					var aie = new ArrayInitializerExpression();
					container.Peek().Elements.Add(aie);
					container.Push(aie);
				}
				var val = Translate(value).ConvertTo(type, this);
				container.Peek().Elements.Add(val);
				elementResolveResults.Add(val.ResolveResult);
				while (container.Count > 0 && container.Peek().Elements.Count == dimensionSizes[container.Count - 1]) {
					container.Pop();
				}
			}
			
			var expr = new ArrayCreateExpression {
				Type = ConvertType(type),
				Initializer = root
			};
			expr.Arguments.AddRange(newArr.Indices.Select(i => Translate(i).Expression));
			result = expr.WithILInstruction(block)
				.WithRR(new ArrayCreateResolveResult(new ArrayType(compilation, type, dimensions), newArr.Indices.Select(i => Translate(i).ResolveResult).ToArray(), elementResolveResults));
			
			return true;
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
