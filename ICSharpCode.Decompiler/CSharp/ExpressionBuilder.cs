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
using ICSharpCode.Decompiler.CSharp.Transforms;
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
	/// <remarks>
	/// Every translated expression must have:
	/// * an ILInstruction annotation
	/// * a ResolveResult annotation
	///   * The type of the ResolveResult must match the StackType of the corresponding ILInstruction.
	///   * If the type of the ResolveResult is <c>sbyte</c> or <c>short</c>, the evaluated value of the ILInstruction
	///     can be obtained by evaluating the C# expression and sign-extending the result to <c>int</c>.
	/// </remarks>
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
			var pe = expr as PrimitiveExpression;
			if (pe != null) {
				if (pe.Value is sbyte)
					expr = expr.CastTo(new NRefactory.CSharp.PrimitiveType("sbyte"));
				else if (pe.Value is byte)
					expr = expr.CastTo(new NRefactory.CSharp.PrimitiveType("byte"));
				else if (pe.Value is short)
					expr = expr.CastTo(new NRefactory.CSharp.PrimitiveType("short"));
				else if (pe.Value is ushort)
					expr = expr.CastTo(new NRefactory.CSharp.PrimitiveType("ushort"));
			}
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
			if (variable.Kind == VariableKind.Parameter && variable.Index < 0)
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
			var targetExpression = TranslateTarget(field, target, true);
			return new MemberReferenceExpression(targetExpression, field.Name)
				.WithRR(new MemberResolveResult(targetExpression.ResolveResult, field));
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
			var args = inst.Indices.Select(arg => TranslateArrayIndex(arg)).ToArray();
			var expr = new ArrayCreateExpression { Type = ConvertType(inst.Type) };
			var ct = expr.Type as ComposedType;
			if (ct != null) {
				// change "new (int[,])[10] to new int[10][,]"
				ct.ArraySpecifiers.MoveTo(expr.AdditionalArraySpecifiers);
			}
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
		
		protected internal override TranslatedExpression VisitLdcDecimal(LdcDecimal inst)
		{
			return new PrimitiveExpression(inst.Value)
				.WithILInstruction(inst)
				.WithRR(new ConstantResolveResult(compilation.FindType(KnownTypeCode.Decimal), inst.Value));
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
		
		readonly HashSet<ILVariable> loadedVariablesSet = new HashSet<ILVariable>();
		
		protected internal override TranslatedExpression VisitLdLoc(LdLoc inst)
		{
			if (inst.Variable.Kind == VariableKind.StackSlot && inst.Variable.IsSingleDefinition) {
				loadedVariablesSet.Add(inst.Variable);
			}
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
			var translatedValue = Translate(inst.Value);
			if (inst.Variable.Kind == VariableKind.StackSlot && inst.Variable.IsSingleDefinition
			    && inst.Variable.StackType == translatedValue.Type.GetStackType()
			    && translatedValue.Type.Kind != TypeKind.Null && !loadedVariablesSet.Contains(inst.Variable)) {
				inst.Variable.Type = translatedValue.Type;
			}
			return Assignment(ConvertVariable(inst.Variable).WithoutILInstruction(), translatedValue).WithILInstruction(inst);
		}
		
		protected internal override TranslatedExpression VisitComp(Comp inst)
		{
			if (inst.Kind.IsEqualityOrInequality()) {
				bool negateOutput;
				var result = TranslateCeq(inst, out negateOutput);
				if (negateOutput)
					return LogicNot(result).WithILInstruction(inst);
				else
					return result;
			} else {
				return TranslateComp(inst);
			}
		}
		
		/// <summary>
		/// Translates the equality comparison between left and right.
		/// </summary>
		TranslatedExpression TranslateCeq(Comp inst, out bool negateOutput)
		{
			// Translate '(e as T) == null' to '!(e is T)'.
			// This is necessary for correctness when T is a value type.
			if (inst.Left.OpCode == OpCode.IsInst && inst.Right.OpCode == OpCode.LdNull) {
				negateOutput = inst.Kind == ComparisonKind.Equality;
				return IsType((IsInst)inst.Left);
			} else if (inst.Right.OpCode == OpCode.IsInst && inst.Left.OpCode == OpCode.LdNull) {
				negateOutput = inst.Kind == ComparisonKind.Equality;
				return IsType((IsInst)inst.Right);
			}
			
			var left = Translate(inst.Left);
			var right = Translate(inst.Right);
			
			// Remove redundant bool comparisons
			if (left.Type.IsKnownType(KnownTypeCode.Boolean)) {
				if (inst.Right.MatchLdcI4(0)) {
					// 'b == 0' => '!b'
					// 'b != 0' => 'b'
					negateOutput = inst.Kind == ComparisonKind.Equality;
					return left;
				}
				if (inst.Right.MatchLdcI4(1)) {
					// 'b == 1' => 'b'
					// 'b != 1' => '!b'
					negateOutput = inst.Kind == ComparisonKind.Inequality;
					return left;
				}
			} else if (right.Type.IsKnownType(KnownTypeCode.Boolean)) {
				if (inst.Left.MatchLdcI4(0)) {
					// '0 == b' => '!b'
					// '0 != b' => 'b'
					negateOutput = inst.Kind == ComparisonKind.Equality;
					return right;
				}
				if (inst.Left.MatchLdcI4(1)) {
					// '0 == b' => '!b'
					// '0 != b' => 'b'
					negateOutput = inst.Kind == ComparisonKind.Equality;
					return right;
				}
			}
			
			var rr = resolver.ResolveBinaryOperator(inst.Kind.ToBinaryOperatorType(), left.ResolveResult, right.ResolveResult)
				as OperatorResolveResult;
			if (rr == null || rr.IsError || rr.UserDefinedOperatorMethod != null
			    || rr.Operands[0].Type.GetStackType() != inst.InputType)
			{
				var targetType = TypeUtils.GetLargerType(left.Type, right.Type);
				if (targetType.Equals(left.Type)) {
					right = right.ConvertTo(targetType, this);
				} else {
					left = left.ConvertTo(targetType, this);
				}
				rr = new OperatorResolveResult(compilation.FindType(KnownTypeCode.Boolean),
				                               BinaryOperatorExpression.GetLinqNodeType(BinaryOperatorType.Equality, false),
				                               left.ResolveResult, right.ResolveResult);
			}
			negateOutput = false;
			return new BinaryOperatorExpression(left.Expression, inst.Kind.ToBinaryOperatorType(), right.Expression)
				.WithILInstruction(inst)
				.WithRR(rr);
		}
		
		/// <summary>
		/// Handle Comp instruction, operators other than equality/inequality.
		/// </summary>
		TranslatedExpression TranslateComp(Comp inst)
		{
			var left = Translate(inst.Left);
			var right = Translate(inst.Right);
			// Ensure the inputs have the correct sign:
			KnownTypeCode inputType = KnownTypeCode.None;
			switch (inst.InputType) {
				case StackType.I: // In order to generate valid C# we need to treat (U)IntPtr as (U)Int64 in comparisons.
				case StackType.I8:
					inputType = inst.Sign == Sign.Unsigned ? KnownTypeCode.UInt64 : KnownTypeCode.Int64;
					break;
				case StackType.I4:
					inputType = inst.Sign == Sign.Unsigned ? KnownTypeCode.UInt32 : KnownTypeCode.Int32;
					break;
			}
			if (inputType != KnownTypeCode.None) {
				left = left.ConvertTo(compilation.FindType(inputType), this);
				right = right.ConvertTo(compilation.FindType(inputType), this);
			}
			var op = inst.Kind.ToBinaryOperatorType();
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
		
		protected internal override TranslatedExpression VisitShl(Shl inst)
		{
			return HandleShift(inst, BinaryOperatorType.ShiftLeft);
		}
		
		protected internal override TranslatedExpression VisitShr(Shr inst)
		{
			return HandleShift(inst, BinaryOperatorType.ShiftRight);
		}
		
		TranslatedExpression HandleShift(BinaryNumericInstruction inst, BinaryOperatorType op)
		{
			var left = Translate(inst.Left);
			var right = Translate(inst.Right);
			
			IType targetType = compilation.FindType(inst.ResultType.ToKnownTypeCode(inst.Sign));
			left = left.ConvertTo(targetType, this);
			
			// Shift operators in C# always expect type 'int' on the right-hand-side
			right = right.ConvertTo(compilation.FindType(KnownTypeCode.Int32), this);
			
			return new BinaryOperatorExpression(left.Expression, op, right.Expression)
					.WithILInstruction(inst)
					.WithRR(resolver.ResolveBinaryOperator(op, left.ResolveResult, right.ResolveResult));
		}
		
		protected internal override TranslatedExpression VisitConv(Conv inst)
		{
			var arg = Translate(inst.Argument);
			if (inst.Sign != Sign.None && arg.Type.GetSign() != inst.Sign) {
				// we need to cast the input to a type of appropriate sign
				var inputType = inst.Argument.ResultType.ToKnownTypeCode(inst.Sign);
				arg = arg.ConvertTo(compilation.FindType(inputType), this);
			}
			var targetType = compilation.FindType(inst.TargetType.ToKnownTypeCode());
			var rr = resolver.WithCheckForOverflow(inst.CheckForOverflow).ResolveCast(targetType, arg.ResolveResult);
			Expression castExpr = new CastExpression(ConvertType(targetType), arg.Expression);
			if (inst.Kind == ConversionKind.Nop || inst.Kind == ConversionKind.Truncate || inst.Kind == ConversionKind.FloatToInt
			    || (inst.Kind == ConversionKind.ZeroExtend && arg.Type.GetSign() == Sign.Signed))
			{
				castExpr.AddAnnotation(inst.CheckForOverflow ? AddCheckedBlocks.CheckedAnnotation : AddCheckedBlocks.UncheckedAnnotation);
			}
			return castExpr.WithILInstruction(inst).WithRR(rr);
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
			var mre = new MemberReferenceExpression(target, method.Name);
			mre.TypeArguments.AddRange(method.TypeArguments.Select(a => ConvertType(a)));
			return new ObjectCreateExpression(ConvertType(inst.Method.DeclaringType), mre)
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(
					inst.Method.DeclaringType,
					new MemberResolveResult(target.ResolveResult, method),
					// TODO handle extension methods capturing the first argument
					Conversion.MethodGroupConversion(method, func.OpCode == OpCode.LdVirtFtn, false)));
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
					if (translatedTarget.Expression is DirectionExpression) {
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
			IMethod method = inst.Method;
			// Used for Call, CallVirt and NewObj
			TranslatedExpression target;
			if (inst.OpCode == OpCode.NewObj) {
				if (IsDelegateConstruction(inst)) {
					return HandleDelegateConstruction(inst);
				}
				target = default(TranslatedExpression); // no target
			} else {
				target = TranslateTarget(method, inst.Arguments.FirstOrDefault(), inst.OpCode == OpCode.Call);
			}
			
			var arguments = inst.Arguments.SelectArray(Translate);
			int firstParamIndex = (method.IsStatic || inst.OpCode == OpCode.NewObj) ? 0 : 1;
			
			// Translate arguments to the expected parameter types
			Debug.Assert(arguments.Length == firstParamIndex + inst.Method.Parameters.Count);
			for (int i = firstParamIndex; i < arguments.Length; i++) {
				var parameter = method.Parameters[i - firstParamIndex];
				arguments[i] = arguments[i].ConvertTo(parameter.Type, this);
				
				if (parameter.IsOut && arguments[i].Expression is DirectionExpression) {
					((DirectionExpression)arguments[i].Expression).FieldDirection = FieldDirection.Out;
				}
			}
			
			if (method is VarArgInstanceMethod) {
				int regularParameterCount = ((VarArgInstanceMethod)method).RegularParameterCount;
				var argListArg = new UndocumentedExpression();
				argListArg.UndocumentedExpressionType = UndocumentedExpressionType.ArgList;
				argListArg.Arguments.AddRange(arguments.Skip(regularParameterCount).Select(arg => arg.Expression));
				var argListRR = new ResolveResult(SpecialType.ArgList);
				arguments = arguments.Take(regularParameterCount)
					.Concat(new[] { argListArg.WithoutILInstruction().WithRR(argListRR) }).ToArray();
				method = (IMethod)method.MemberDefinition;
			}

			var argumentResolveResults = arguments.Skip(firstParamIndex).Select(arg => arg.ResolveResult).ToList();

			ResolveResult rr;
			if (inst.Method.IsAccessor)
				rr = new MemberResolveResult(target.ResolveResult, method.AccessorOwner);
			else
				rr = new CSharpInvocationResolveResult(target.ResolveResult, method, argumentResolveResults);
			
			var argumentExpressions = arguments.Skip(firstParamIndex).Select(arg => arg.Expression).ToList();
			if (inst.OpCode == OpCode.NewObj) {
				return new ObjectCreateExpression(ConvertType(inst.Method.DeclaringType), argumentExpressions)
					.WithILInstruction(inst).WithRR(rr);
			} else {
				Expression expr;
				if (method.IsAccessor) {
					if (method.ReturnType.IsKnownType(KnownTypeCode.Void)) {
						var value = argumentExpressions.Last();
						argumentExpressions.Remove(value);
						if (argumentExpressions.Count == 0)
							expr = new MemberReferenceExpression(target.Expression, method.AccessorOwner.Name);
						else
							expr = new IndexerExpression(target.Expression, argumentExpressions);
						var op = AssignmentOperatorType.Assign;
						var parentEvent = method.AccessorOwner as IEvent;
						if (parentEvent != null) {
							if (method.Equals(parentEvent.AddAccessor)) {
								op = AssignmentOperatorType.Add;
							}
							if (method.Equals(parentEvent.RemoveAccessor)) {
								op = AssignmentOperatorType.Subtract;
							}
						}
						expr = new AssignmentExpression(expr, op, value);
					} else {
						if (argumentExpressions.Count == 0)
							expr = new MemberReferenceExpression(target.Expression, method.AccessorOwner.Name);
						else
							expr = new IndexerExpression(target.Expression, argumentExpressions);
					}
				} else {
					Expression targetExpr = target.Expression;
					string methodName = method.Name;
					// HACK : convert this.Dispose() to ((IDisposable)this).Dispose(), if Dispose is an explicitly implemented interface method.
					if (inst.Method.IsExplicitInterfaceImplementation && targetExpr is ThisReferenceExpression) {
						targetExpr = targetExpr.CastTo(ConvertType(method.ImplementedInterfaceMembers[0].DeclaringType));
						methodName = method.ImplementedInterfaceMembers[0].Name;
					}
					var mre = new MemberReferenceExpression(targetExpr, methodName);
					mre.TypeArguments.AddRange(method.TypeArguments.Select(a => ConvertType(a)));
					expr = new InvocationExpression(mre, argumentExpressions);
				}
				return expr.WithILInstruction(inst).WithRR(rr);
			}
		}
		
		protected internal override TranslatedExpression VisitLdObj(LdObj inst)
		{
			var target = Translate(inst.Target);
			if (target.Expression is DirectionExpression && TypeUtils.IsCompatibleTypeForMemoryAccess(target.Type, inst.Type)) {
				// we can dereference the managed reference by stripping away the 'ref'
				var result = target.UnwrapChild(((DirectionExpression)target.Expression).Expression);
				// we don't convert result to inst.Type, because the LdObj type
				// might be inaccurate (it's often System.Object for all reference types),
				// and our parent node should already insert casts where necessary
				result.Expression.AddAnnotation(inst); // add LdObj in addition to the existing ILInstruction annotation
				
				if (target.Type.IsSmallIntegerType() && inst.Type.IsSmallIntegerType() && target.Type.GetSign() != inst.Type.GetSign())
					return result.ConvertTo(inst.Type, this);
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
			if (target.Expression is DirectionExpression && TypeUtils.IsCompatibleTypeForMemoryAccess(target.Type, inst.Type)) {
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
			if (arrayExpr.Type.Kind != TypeKind.Array) {
				arrayExpr = arrayExpr.ConvertTo(compilation.FindType(KnownTypeCode.Array), this);
			}
			TranslatedExpression lenExpr;
			if (inst.ResultType == StackType.I4) {
				lenExpr = arrayExpr.Expression.Member("Length")
					.WithILInstruction(inst)
					.WithRR(new ResolveResult(compilation.FindType(KnownTypeCode.Int32)));
			} else {
				lenExpr = arrayExpr.Expression.Member("LongLength")
					.WithILInstruction(inst)
					.WithRR(new ResolveResult(compilation.FindType(KnownTypeCode.Int64)));
			}
			return lenExpr.ConvertTo(compilation.FindType(inst.ResultType.ToKnownTypeCode()), this);
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
			if (arrayType == null) {
				arrayType  = new ArrayType(compilation, inst.Type, inst.Indices.Count);
				arrayExpr = arrayExpr.ConvertTo(arrayType, this);
			}
			TranslatedExpression expr = new IndexerExpression(
				arrayExpr, inst.Indices.Select(i => TranslateArrayIndex(i).Expression)
			).WithILInstruction(inst).WithRR(new ResolveResult(arrayType.ElementType));
			return new DirectionExpression(FieldDirection.Ref, expr)
				.WithoutILInstruction().WithRR(new ResolveResult(new ByReferenceType(expr.Type)));
		}
		
		TranslatedExpression TranslateArrayIndex(ILInstruction i)
		{
			var stackType = i.ResultType == StackType.I4 ? KnownTypeCode.Int32 : KnownTypeCode.Int64;
			return Translate(i).ConvertTo(compilation.FindType(stackType), this);
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
			}.WithRR(new ResolveResult(inst.Type));
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
