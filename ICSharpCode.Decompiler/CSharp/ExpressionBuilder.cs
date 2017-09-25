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
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.CSharp.TypeSystem;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;
using ExpressionType = System.Linq.Expressions.ExpressionType;
using PrimitiveType = ICSharpCode.Decompiler.CSharp.Syntax.PrimitiveType;
using System.Threading;

namespace ICSharpCode.Decompiler.CSharp
{
	/// <summary>
	/// Translates from ILAst to C# expressions.
	/// </summary>
	/// <remarks>
	/// Every translated expression must have:
	/// * an ILInstruction annotation
	/// * a ResolveResult annotation
	/// Post-condition for Translate() calls:
	///   * The type of the ResolveResult must match the StackType of the corresponding ILInstruction,
	///     except that the width of integer types does not need to match (I4, I and I8 count as the same stack type here)
	///   * Evaluating the resulting C# expression shall produce the same side effects as evaluating the ILInstruction.
	///   * If the IL instruction has <c>ResultType == StackType.Void</c>, the C# expression may evaluate to an arbitrary type and value.
	///   * Otherwise, evaluating the resulting C# expression shall produce a similar value as evaluating the ILInstruction.
	///      * If the IL instruction evaluates to an integer stack type (I4, I, or I8),
	///        the C# type of the resulting expression shall also be an integer (or enum/pointer/char/bool) type.
	///        * If sizeof(C# type) == sizeof(IL stack type), the values must be the same.
	///        * If sizeof(C# type) > sizeof(IL stack type), the C# value truncated to the width of the IL stack type must equal the IL value.
	///        * If sizeof(C# type) &lt; sizeof(IL stack type), the C# value (sign/zero-)extended to the width of the IL stack type
	///          must equal the IL value.
	///          Whether sign or zero extension is used depends on the sign of the C# type (as determined by <c>IType.GetSign()</c>).
	///      * If the IL instruction is a lifted nullable operation, and the underlying operation evaluates to an integer stack type,
	///        the C# type of the resulting expression shall be Nullable{T}, where T is an integer type (as above).
	///        The C# value shall be null iff the IL-level value evaluates to null, and otherwise the values shall correspond
	///        as with non-lifted integer operations.
	///      * Otherwise, the C# type of the resulting expression shall match the IL stack type,
	///        and the evaluated values shall be the same.
	/// </remarks>
	class ExpressionBuilder : ILVisitor<TranslationContext, TranslatedExpression>
	{
		readonly IDecompilerTypeSystem typeSystem;
		readonly ITypeResolveContext decompilationContext;
		internal readonly ICompilation compilation;
		internal readonly CSharpResolver resolver;
		readonly TypeSystemAstBuilder astBuilder;
		readonly DecompilerSettings settings;
		readonly CancellationToken cancellationToken;
		
		public ExpressionBuilder(IDecompilerTypeSystem typeSystem, ITypeResolveContext decompilationContext, DecompilerSettings settings, CancellationToken cancellationToken)
		{
			Debug.Assert(decompilationContext != null);
			this.typeSystem = typeSystem;
			this.decompilationContext = decompilationContext;
			this.settings = settings;
			this.cancellationToken = cancellationToken;
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
			if (expr is NullReferenceExpression && rr.Type.Kind != TypeKind.Null) {
				expr = new CastExpression(ConvertType(rr.Type), expr);
			} else {
				switch (rr.Type.GetDefinition()?.KnownTypeCode) {
					case KnownTypeCode.SByte:
					case KnownTypeCode.Byte:
					case KnownTypeCode.Int16:
					case KnownTypeCode.UInt16:
						expr = new CastExpression(new PrimitiveType(KnownTypeReference.GetCSharpNameByTypeCode(rr.Type.GetDefinition().KnownTypeCode)), expr);
						break;
				}
			}
			var exprRR = expr.Annotation<ResolveResult>();
			if (exprRR == null) {
				exprRR = rr;
				expr.AddAnnotation(rr);
			}
			return new ExpressionWithResolveResult(expr, exprRR);
		}
		
		public TranslatedExpression Translate(ILInstruction inst, IType typeHint = null)
		{
			Debug.Assert(inst != null);
			cancellationToken.ThrowIfCancellationRequested();
			TranslationContext context = new TranslationContext {
				TypeHint = typeHint ?? SpecialType.UnknownType
			};
			var cexpr = inst.AcceptVisitor(this, context);
			#if DEBUG
			if (inst.ResultType != StackType.Void && cexpr.Type.Kind != TypeKind.Unknown) {
				// Validate the Translate post-condition (documented at beginning of this file):
				if (inst.ResultType.IsIntegerType()) {
					Debug.Assert(cexpr.Type.GetStackType().IsIntegerType(), "IL instructions of integer type must convert into C# expressions of integer type");
					Debug.Assert(cexpr.Type.GetSign() != Sign.None, "Must have a sign specified for zero/sign-extension");
				} else if (inst is ILiftableInstruction liftable && liftable.IsLifted) {
					Debug.Assert(NullableType.IsNullable(cexpr.Type));
					IType underlying = NullableType.GetUnderlyingType(cexpr.Type);
					if (liftable.UnderlyingResultType.IsIntegerType()) {
						Debug.Assert(underlying.GetStackType().IsIntegerType(), "IL instructions of integer type must convert into C# expressions of integer type");
						Debug.Assert(underlying.GetSign() != Sign.None, "Must have a sign specified for zero/sign-extension");
					} else {
						Debug.Assert(underlying.GetStackType() == liftable.UnderlyingResultType);
					}
				} else {
					Debug.Assert(cexpr.Type.GetStackType() == inst.ResultType);
				}
			}
			#endif
			return cexpr;
		}
		
		public TranslatedExpression TranslateCondition(ILInstruction condition, bool negate = false)
		{
			var expr = Translate(condition, compilation.FindType(KnownTypeCode.Boolean));
			return expr.ConvertToBoolean(this, negate);
		}
		
		internal ExpressionWithResolveResult ConvertVariable(ILVariable variable)
		{
			Expression expr;
			if (variable.Kind == VariableKind.Parameter && variable.Index < 0)
				expr = new ThisReferenceExpression();
			else
				expr = new IdentifierExpression(variable.Name);
			if (variable.Type.Kind == TypeKind.ByReference) {
				// When loading a by-ref parameter, use 'ref paramName'.
				// We'll strip away the 'ref' when dereferencing.
				
				// Ensure that the IdentifierExpression itself also gets a resolve result, as that might
				// get used after the 'ref' is stripped away:
				var elementType = ((ByReferenceType)variable.Type).ElementType;
				expr.WithRR(new ILVariableResolveResult(variable, elementType));
				
				expr = new DirectionExpression(FieldDirection.Ref, expr);
				return expr.WithRR(new ResolveResult(variable.Type));
			} else {
				return expr.WithRR(new ILVariableResolveResult(variable, variable.Type));
			}
		}

		ExpressionWithResolveResult ConvertField(IField field, ILInstruction target = null)
		{
			var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentAssembly);
			var targetExpression = TranslateTarget(field, target, true);
			
			var result = lookup.Lookup(targetExpression.ResolveResult, field.Name, EmptyList<IType>.Instance, false) as MemberResolveResult;
			
			if (result == null || !result.Member.Equals(field))
				targetExpression = targetExpression.ConvertTo(field.DeclaringType, this);
			
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
		
		protected internal override TranslatedExpression VisitIsInst(IsInst inst, TranslationContext context)
		{
			var arg = Translate(inst.Argument);
			return new AsExpression(arg.Expression, ConvertType(inst.Type))
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(inst.Type, arg.ResolveResult, Conversion.TryCast));
		}
		
		protected internal override TranslatedExpression VisitNewObj(NewObj inst, TranslationContext context)
		{
			return new CallBuilder(this, typeSystem, settings).Build(inst);
		}
		
		protected internal override TranslatedExpression VisitNewArr(NewArr inst, TranslationContext context)
		{
			var dimensions = inst.Indices.Count;
			var args = inst.Indices.Select(arg => TranslateArrayIndex(arg)).ToArray();
			var expr = new ArrayCreateExpression { Type = ConvertType(inst.Type) };
			if (expr.Type is ComposedType ct) {
				// change "new (int[,])[10] to new int[10][,]"
				ct.ArraySpecifiers.MoveTo(expr.AdditionalArraySpecifiers);
			}
			expr.Arguments.AddRange(args.Select(arg => arg.Expression));
			return expr.WithILInstruction(inst)
				.WithRR(new ArrayCreateResolveResult(new ArrayType(compilation, inst.Type, dimensions), args.Select(a => a.ResolveResult).ToList(), new ResolveResult[0]));
		}
		
		protected internal override TranslatedExpression VisitLocAlloc(LocAlloc inst, TranslationContext context)
		{
			IType elementType;
			TranslatedExpression countExpression = TranslatePointerArgument(inst.Argument, context, out elementType);
			countExpression = countExpression.ConvertTo(compilation.FindType(KnownTypeCode.Int32), this);
			if (elementType == null)
				elementType = compilation.FindType(KnownTypeCode.Byte);
			return new StackAllocExpression {
				Type = ConvertType(elementType),
				CountExpression = countExpression
			}.WithILInstruction(inst).WithRR(new ResolveResult(new PointerType(elementType)));
		}
		
		/// <summary>
		/// Translate the argument of an operation that deals with pointers:
		/// * undoes the implicit multiplication with `sizeof(elementType)` and returns `elementType`
		/// * on failure, translates the whole expression and returns `elementType = null`.
		/// </summary>
		TranslatedExpression TranslatePointerArgument(ILInstruction countExpr, TranslationContext context, out IType elementType)
		{
			ILInstruction left;
			ILInstruction right;
			if (countExpr.MatchBinaryNumericInstruction(BinaryNumericOperator.Mul, out left, out right)
			    && right.UnwrapConv(ConversionKind.SignExtend).UnwrapConv(ConversionKind.ZeroExtend).MatchSizeOf(out elementType))
			{
				return Translate(left);
			}
			
			var pointerTypeHint = context.TypeHint as PointerType;
			if (pointerTypeHint == null) {
				elementType = null;
				return Translate(countExpr);
			}
			ResolveResult sizeofRR = resolver.ResolveSizeOf(pointerTypeHint.ElementType);
			if (!(sizeofRR.IsCompileTimeConstant && sizeofRR.ConstantValue is int)) {
				elementType = null;
				return Translate(countExpr);
			}
			int typeSize = (int)sizeofRR.ConstantValue;
			
			if (countExpr.MatchBinaryNumericInstruction(BinaryNumericOperator.Mul, out left, out right)
			    && right.UnwrapConv(ConversionKind.SignExtend).UnwrapConv(ConversionKind.ZeroExtend).MatchLdcI4(typeSize))
			{
				elementType = pointerTypeHint.ElementType;
				return Translate(left);
			}
			
			elementType = null;
			return Translate(countExpr);
		}

		protected internal override TranslatedExpression VisitLdcI4(LdcI4 inst, TranslationContext context)
		{
			return new PrimitiveExpression(inst.Value)
				.WithILInstruction(inst)
				.WithRR(new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), inst.Value));
		}
		
		protected internal override TranslatedExpression VisitLdcI8(LdcI8 inst, TranslationContext context)
		{
			return new PrimitiveExpression(inst.Value)
				.WithILInstruction(inst)
				.WithRR(new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int64), inst.Value));
		}
		
		protected internal override TranslatedExpression VisitLdcF(LdcF inst, TranslationContext context)
		{
			return new PrimitiveExpression(inst.Value)
				.WithILInstruction(inst)
				.WithRR(new ConstantResolveResult(compilation.FindType(KnownTypeCode.Double), inst.Value));
		}
		
		protected internal override TranslatedExpression VisitLdcDecimal(LdcDecimal inst, TranslationContext context)
		{
			return new PrimitiveExpression(inst.Value)
				.WithILInstruction(inst)
				.WithRR(new ConstantResolveResult(compilation.FindType(KnownTypeCode.Decimal), inst.Value));
		}
		
		protected internal override TranslatedExpression VisitLdStr(LdStr inst, TranslationContext context)
		{
			return new PrimitiveExpression(inst.Value)
				.WithILInstruction(inst)
				.WithRR(new ConstantResolveResult(compilation.FindType(KnownTypeCode.String), inst.Value));
		}
		
		protected internal override TranslatedExpression VisitLdNull(LdNull inst, TranslationContext context)
		{
			return GetDefaultValueExpression(SpecialType.NullType).WithILInstruction(inst);
		}
		
		protected internal override TranslatedExpression VisitDefaultValue(DefaultValue inst, TranslationContext context)
		{
			return GetDefaultValueExpression(inst.Type).WithILInstruction(inst);
		}

		internal ExpressionWithResolveResult GetDefaultValueExpression(IType type)
		{
			var expr = type.IsReferenceType == true ? (Expression)new NullReferenceExpression() : new DefaultValueExpression(ConvertType(type));
			var constantType = type.IsReferenceType == true ? SpecialType.NullType : type;
			return expr.WithRR(new ConstantResolveResult(constantType, null));
		}
		
		protected internal override TranslatedExpression VisitSizeOf(SizeOf inst, TranslationContext context)
		{
			return new SizeOfExpression(ConvertType(inst.Type))
				.WithILInstruction(inst)
				.WithRR(new SizeOfResolveResult(compilation.FindType(KnownTypeCode.Int32), inst.Type, null));
		}
		
		protected internal override TranslatedExpression VisitLdTypeToken(LdTypeToken inst, TranslationContext context)
		{
			return new MemberReferenceExpression(new TypeOfExpression(ConvertType(inst.Type)), "TypeHandle")
				.WithILInstruction(inst)
				.WithRR(new TypeOfResolveResult(compilation.FindType(new TopLevelTypeName("System", "RuntimeTypeHandle")), inst.Type));
		}
		
		protected internal override TranslatedExpression VisitBitNot(BitNot inst, TranslationContext context)
		{
			var argument = Translate(inst.Argument);
			var argUType = NullableType.GetUnderlyingType(argument.Type);

			if (argUType.GetStackType().GetSize() < inst.UnderlyingResultType.GetSize()
			    || argUType.Kind == TypeKind.Enum && argUType.IsSmallIntegerType()
				|| argUType.GetStackType() == StackType.I
				|| argUType.IsKnownType(KnownTypeCode.Boolean)
				|| argUType.IsKnownType(KnownTypeCode.Char))
			{
				// Argument is undersized (even after implicit integral promotion to I4)
				// -> we need to perform sign/zero-extension before the BitNot.
				// Same if the argument is an enum based on a small integer type
				// (those don't undergo numeric promotion in C# the way non-enum small integer types do).
				// Same if the type is one that does not support ~ (IntPtr, bool and char).
				StackType targetStackType = inst.UnderlyingResultType;
				if (targetStackType == StackType.I) {
					// IntPtr doesn't support operator ~.
					// Note that it's OK to use a type that's larger than necessary.
					targetStackType = StackType.I8;
				}
				IType targetType = compilation.FindType(targetStackType.ToKnownTypeCode(argUType.GetSign()));
				if (inst.IsLifted) {
					targetType = NullableType.Create(compilation, targetType);
				}
				argument = argument.ConvertTo(targetType, this);
			}
			
			return new UnaryOperatorExpression(UnaryOperatorType.BitNot, argument)
				.WithRR(resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, argument.ResolveResult))
				.WithILInstruction(inst);
		}
		
		internal ExpressionWithResolveResult LogicNot(TranslatedExpression expr)
		{
			return new UnaryOperatorExpression(UnaryOperatorType.Not, expr.Expression)
				.WithRR(new OperatorResolveResult(compilation.FindType(KnownTypeCode.Boolean), ExpressionType.Not, expr.ResolveResult));
		}
		
		readonly HashSet<ILVariable> loadedVariablesSet = new HashSet<ILVariable>();
		
		protected internal override TranslatedExpression VisitLdLoc(LdLoc inst, TranslationContext context)
		{
			if (inst.Variable.Kind == VariableKind.StackSlot && inst.Variable.IsSingleDefinition) {
				loadedVariablesSet.Add(inst.Variable);
			}
			return ConvertVariable(inst.Variable).WithILInstruction(inst);
		}
		
		protected internal override TranslatedExpression VisitLdLoca(LdLoca inst, TranslationContext context)
		{
			var expr = ConvertVariable(inst.Variable).WithILInstruction(inst);
			// Note that we put the instruction on the IdentifierExpression instead of the DirectionExpression,
			// because the DirectionExpression might get removed by dereferencing instructions such as LdObj
			return new DirectionExpression(FieldDirection.Ref, expr.Expression)
				.WithoutILInstruction()
				.WithRR(new ByReferenceResolveResult(expr.ResolveResult, isOut: false));
		}
		
		protected internal override TranslatedExpression VisitStLoc(StLoc inst, TranslationContext context)
		{
			var translatedValue = Translate(inst.Value, typeHint: inst.Variable.Type);
			if (inst.Variable.Kind == VariableKind.StackSlot && inst.Variable.IsSingleDefinition
			    && inst.Variable.StackType == translatedValue.Type.GetStackType()
			    && translatedValue.Type.Kind != TypeKind.Null && !loadedVariablesSet.Contains(inst.Variable)) {
				inst.Variable.Type = translatedValue.Type;
			}
			return Assignment(ConvertVariable(inst.Variable).WithoutILInstruction(), translatedValue).WithILInstruction(inst);
		}
		
		protected internal override TranslatedExpression VisitComp(Comp inst, TranslationContext context)
		{
			if (inst.LiftingKind == ComparisonLiftingKind.ThreeValuedLogic) {
				if (inst.Kind == ComparisonKind.Equality && inst.Right.MatchLdcI4(0)) {
					// lifted logic.not
					var targetType = NullableType.Create(compilation, compilation.FindType(KnownTypeCode.Boolean));
					var arg = Translate(inst.Left).ConvertTo(targetType, this);
					return new UnaryOperatorExpression(UnaryOperatorType.Not, arg.Expression)
						.WithRR(new OperatorResolveResult(targetType, ExpressionType.Not, arg.ResolveResult))
						.WithILInstruction(inst);
				}
				return ErrorExpression("Nullable comparisons with three-valued-logic not supported in C#");
			}
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
			Debug.Assert(inst.Kind.IsEqualityOrInequality());
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
					// '1 == b' => 'b'
					// '1 != b' => '!b'
					negateOutput = inst.Kind == ComparisonKind.Inequality;
					return right;
				}
			}

			// Special case comparisons with enum and char literals
			left = AdjustConstantExpressionToType(left, right.Type);
			right = AdjustConstantExpressionToType(right, left.Type);
			
			var rr = resolver.ResolveBinaryOperator(inst.Kind.ToBinaryOperatorType(), left.ResolveResult, right.ResolveResult)
				as OperatorResolveResult;
			if (rr == null || rr.IsError || rr.UserDefinedOperatorMethod != null
			    || NullableType.GetUnderlyingType(rr.Operands[0].Type).GetStackType() != inst.InputType)
			{
				IType targetType;
				if (inst.InputType == StackType.O) {
					targetType = compilation.FindType(KnownTypeCode.Object);
				} else {
					targetType = TypeUtils.GetLargerType(NullableType.GetUnderlyingType(left.Type), NullableType.GetUnderlyingType(right.Type));
				}
				if (inst.IsLifted) {
					targetType = NullableType.Create(compilation, targetType);
				}
				if (targetType.Equals(left.Type)) {
					right = right.ConvertTo(targetType, this);
				} else {
					left = left.ConvertTo(targetType, this);
				}
				rr = resolver.ResolveBinaryOperator(inst.Kind.ToBinaryOperatorType(),
					left.ResolveResult, right.ResolveResult) as OperatorResolveResult;
				if (rr == null || rr.IsError || rr.UserDefinedOperatorMethod != null
					|| NullableType.GetUnderlyingType(rr.Operands[0].Type).GetStackType() != inst.InputType)
				{
					// If converting one input wasn't sufficient, convert both:
					left = left.ConvertTo(targetType, this);
					right = right.ConvertTo(targetType, this);
					rr = new OperatorResolveResult(compilation.FindType(KnownTypeCode.Boolean),
												   BinaryOperatorExpression.GetLinqNodeType(BinaryOperatorType.Equality, false),
												   left.ResolveResult, right.ResolveResult);
				}
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
				IType targetType = compilation.FindType(inputType);
				if (inst.IsLifted) {
					targetType = NullableType.Create(compilation, targetType);
				}
				left = left.ConvertTo(targetType, this);
				right = right.ConvertTo(targetType, this);
			}
			var op = inst.Kind.ToBinaryOperatorType();
			return new BinaryOperatorExpression(left.Expression, op, right.Expression)
				.WithILInstruction(inst)
				.WithRR(new OperatorResolveResult(compilation.FindType(TypeCode.Boolean),
				                                  BinaryOperatorExpression.GetLinqNodeType(op, false),
				                                  left.ResolveResult, right.ResolveResult));
		}

		protected internal override TranslatedExpression VisitThreeValuedLogicAnd(ThreeValuedLogicAnd inst, TranslationContext context)
		{
			return HandleThreeValuedLogic(inst, BinaryOperatorType.BitwiseAnd, ExpressionType.And);
		}

		protected internal override TranslatedExpression VisitThreeValuedLogicOr(ThreeValuedLogicOr inst, TranslationContext context)
		{
			return HandleThreeValuedLogic(inst, BinaryOperatorType.BitwiseOr, ExpressionType.Or);
		}

		TranslatedExpression HandleThreeValuedLogic(BinaryInstruction inst, BinaryOperatorType op, ExpressionType eop)
		{
			var left = Translate(inst.Left);
			var right = Translate(inst.Right);
			IType boolType = compilation.FindType(KnownTypeCode.Boolean);
			IType nullableBoolType = NullableType.Create(compilation, boolType);
			if (NullableType.IsNullable(left.Type)) {
				left = left.ConvertTo(nullableBoolType, this);
				if (NullableType.IsNullable(right.Type)) {
					right = right.ConvertTo(nullableBoolType, this);
				} else {
					right = right.ConvertTo(boolType, this);
				}
			} else {
				left = left.ConvertTo(boolType, this);
				right = right.ConvertTo(nullableBoolType, this);
			}
			return new BinaryOperatorExpression(left.Expression, op, right.Expression)
				.WithRR(new OperatorResolveResult(nullableBoolType, eop, null, true, new[] { left.ResolveResult, right.ResolveResult }))
				.WithILInstruction(inst);
		}

		ExpressionWithResolveResult Assignment(TranslatedExpression left, TranslatedExpression right)
		{
			right = right.ConvertTo(left.Type, this, allowImplicitConversion: true);
			return new AssignmentExpression(left.Expression, right.Expression)
				.WithRR(new OperatorResolveResult(left.Type, ExpressionType.Assign, left.ResolveResult, right.ResolveResult));
		}
		
		protected internal override TranslatedExpression VisitBinaryNumericInstruction(BinaryNumericInstruction inst, TranslationContext context)
		{
			switch (inst.Operator) {
				case BinaryNumericOperator.Add:
					return HandleBinaryNumeric(inst, BinaryOperatorType.Add);
				case BinaryNumericOperator.Sub:
					return HandleBinaryNumeric(inst, BinaryOperatorType.Subtract);
				case BinaryNumericOperator.Mul:
					return HandleBinaryNumeric(inst, BinaryOperatorType.Multiply);
				case BinaryNumericOperator.Div:
					return HandleBinaryNumeric(inst, BinaryOperatorType.Divide);
				case BinaryNumericOperator.Rem:
					return HandleBinaryNumeric(inst, BinaryOperatorType.Modulus);
				case BinaryNumericOperator.BitAnd:
					return HandleBinaryNumeric(inst, BinaryOperatorType.BitwiseAnd);
				case BinaryNumericOperator.BitOr:
					return HandleBinaryNumeric(inst, BinaryOperatorType.BitwiseOr);
				case BinaryNumericOperator.BitXor:
					return HandleBinaryNumeric(inst, BinaryOperatorType.ExclusiveOr);
				case BinaryNumericOperator.ShiftLeft:
					return HandleShift(inst, BinaryOperatorType.ShiftLeft);
				case BinaryNumericOperator.ShiftRight:
					return HandleShift(inst, BinaryOperatorType.ShiftRight);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		TranslatedExpression HandleBinaryNumeric(BinaryNumericInstruction inst, BinaryOperatorType op)
		{
			var resolverWithOverflowCheck = resolver.WithCheckForOverflow(inst.CheckForOverflow);
			var left = Translate(inst.Left);
			var right = Translate(inst.Right);
			left = PrepareArithmeticArgument(left, inst.LeftInputType, inst.Sign, inst.IsLifted);
			right = PrepareArithmeticArgument(right, inst.RightInputType, inst.Sign, inst.IsLifted);
			
			var rr = resolverWithOverflowCheck.ResolveBinaryOperator(op, left.ResolveResult, right.ResolveResult);
			if (rr.IsError || NullableType.GetUnderlyingType(rr.Type).GetStackType() != inst.UnderlyingResultType
			    || !IsCompatibleWithSign(left.Type, inst.Sign) || !IsCompatibleWithSign(right.Type, inst.Sign))
			{
				// Left and right operands are incompatible, so convert them to a common type
				StackType targetStackType = inst.UnderlyingResultType == StackType.I ? StackType.I8 : inst.UnderlyingResultType;
				IType targetType = compilation.FindType(targetStackType.ToKnownTypeCode(inst.Sign));
				left = left.ConvertTo(NullableType.IsNullable(left.Type) ? NullableType.Create(compilation, targetType) : targetType, this);
				right = right.ConvertTo(NullableType.IsNullable(right.Type) ? NullableType.Create(compilation, targetType) : targetType, this);
				rr = resolverWithOverflowCheck.ResolveBinaryOperator(op, left.ResolveResult, right.ResolveResult);
			}
			var resultExpr = new BinaryOperatorExpression(left.Expression, op, right.Expression)
				.WithILInstruction(inst)
				.WithRR(rr);
			if (BinaryOperatorMightCheckForOverflow(op))
				resultExpr.Expression.AddAnnotation(inst.CheckForOverflow ? AddCheckedBlocks.CheckedAnnotation : AddCheckedBlocks.UncheckedAnnotation);
			return resultExpr;
		}

		/// <summary>
		/// Handle oversized arguments needing truncation; and avoid IntPtr/pointers in arguments.
		/// </summary>
		TranslatedExpression PrepareArithmeticArgument(TranslatedExpression arg, StackType argStackType, Sign sign, bool isLifted)
		{
			if (isLifted && !NullableType.IsNullable(arg.Type)) {
				isLifted = false; // don't cast to nullable if this input wasn't already nullable
			}
			IType argUType = isLifted ? NullableType.GetUnderlyingType(arg.Type) : arg.Type;
			if (argStackType.IsIntegerType() && argStackType.GetSize() < argUType.GetSize()) {
				// If the argument is oversized (needs truncation to match stack size of its ILInstruction),
				// perform the truncation now.
				IType targetType = compilation.FindType(argStackType.ToKnownTypeCode(sign));
				if (isLifted)
					targetType = NullableType.Create(compilation, targetType);
				arg = arg.ConvertTo(targetType, this);
			}
			if (argUType.GetStackType() == StackType.I) {
				// None of the operators we might want to apply are supported by IntPtr/UIntPtr.
				// Also, pointer arithmetic has different semantics (works in number of elements, not bytes).
				// So any inputs of size StackType.I must be converted to long/ulong.
				IType targetType = compilation.FindType(StackType.I8.ToKnownTypeCode(sign));
				if (isLifted)
					targetType = NullableType.Create(compilation, targetType);
				arg = arg.ConvertTo(targetType, this);
			}
			return arg;
		}
		
		/// <summary>
		/// Gets whether <paramref name="type"/> has the specified <paramref name="sign"/>.
		/// If <paramref name="sign"/> is None, always returns true.
		/// </summary>
		static bool IsCompatibleWithSign(IType type, Sign sign)
		{
			return sign == Sign.None || NullableType.GetUnderlyingType(type).GetSign() == sign;
		}
		
		static bool BinaryOperatorMightCheckForOverflow(BinaryOperatorType op)
		{
			switch (op) {
				case BinaryOperatorType.BitwiseAnd:
				case BinaryOperatorType.BitwiseOr:
				case BinaryOperatorType.ExclusiveOr:
				case BinaryOperatorType.ShiftLeft:
				case BinaryOperatorType.ShiftRight:
					return false;
				default:
					return true;
			}
		}

		TranslatedExpression HandleShift(BinaryNumericInstruction inst, BinaryOperatorType op)
		{
			var left = Translate(inst.Left);
			var right = Translate(inst.Right);

			Sign sign = inst.Sign;
			var leftUType = NullableType.GetUnderlyingType(left.Type);
			if (leftUType.IsCSharpSmallIntegerType() && sign != Sign.Unsigned && inst.UnderlyingResultType == StackType.I4) {
				// With small integer types, C# will promote to int and perform signed shifts.
				// We thus don't need any casts in this case.
			} else {
				// Insert cast to target type.
				if (sign == Sign.None) {
					// if we don't need a specific sign, prefer keeping that of the input:
					sign = leftUType.GetSign();
				}
				IType targetType;
				if (inst.UnderlyingResultType == StackType.I4) {
					targetType = compilation.FindType(sign == Sign.Unsigned ? KnownTypeCode.UInt32 : KnownTypeCode.Int32);
				} else {
					targetType = compilation.FindType(sign == Sign.Unsigned ? KnownTypeCode.UInt64 : KnownTypeCode.Int64);
				}
				if (NullableType.IsNullable(left.Type)) {
					targetType = NullableType.Create(compilation, targetType);
				}
				left = left.ConvertTo(targetType, this);
			}

			// Shift operators in C# always expect type 'int' on the right-hand-side
			if (NullableType.IsNullable(right.Type)) {
				right = right.ConvertTo(NullableType.Create(compilation, compilation.FindType(KnownTypeCode.Int32)), this);
			} else {
				right = right.ConvertTo(compilation.FindType(KnownTypeCode.Int32), this);
			}

			TranslatedExpression result = new BinaryOperatorExpression(left.Expression, op, right.Expression)
				.WithILInstruction(inst)
				.WithRR(resolver.ResolveBinaryOperator(op, left.ResolveResult, right.ResolveResult));
			if (inst.UnderlyingResultType == StackType.I) {
				// C# doesn't have shift operators for IntPtr, so we first shifted a long/ulong,
				// and now have to case back down to IntPtr/UIntPtr:
				IType targetType = compilation.FindType(sign == Sign.Unsigned ? KnownTypeCode.UIntPtr : KnownTypeCode.IntPtr);
				if (inst.IsLifted)
					targetType = NullableType.Create(compilation, targetType);
				result = result.ConvertTo(targetType, this);
			}
			return result;
		}
		
		protected internal override TranslatedExpression VisitCompoundAssignmentInstruction(CompoundAssignmentInstruction inst, TranslationContext context)
		{
			switch (inst.Operator) {
				case BinaryNumericOperator.Add:
					return HandleCompoundAssignment(inst, AssignmentOperatorType.Add);
				case BinaryNumericOperator.Sub:
					return HandleCompoundAssignment(inst, AssignmentOperatorType.Subtract);
				case BinaryNumericOperator.Mul:
					return HandleCompoundAssignment(inst, AssignmentOperatorType.Multiply);
				case BinaryNumericOperator.Div:
					return HandleCompoundAssignment(inst, AssignmentOperatorType.Divide);
				case BinaryNumericOperator.Rem:
					return HandleCompoundAssignment(inst, AssignmentOperatorType.Modulus);
				case BinaryNumericOperator.BitAnd:
					return HandleCompoundAssignment(inst, AssignmentOperatorType.BitwiseAnd);
				case BinaryNumericOperator.BitOr:
					return HandleCompoundAssignment(inst, AssignmentOperatorType.BitwiseOr);
				case BinaryNumericOperator.BitXor:
					return HandleCompoundAssignment(inst, AssignmentOperatorType.ExclusiveOr);
				case BinaryNumericOperator.ShiftLeft:
					return HandleCompoundShift(inst, AssignmentOperatorType.ShiftLeft);
				case BinaryNumericOperator.ShiftRight:
					return HandleCompoundShift(inst, AssignmentOperatorType.ShiftRight);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		TranslatedExpression HandleCompoundAssignment(CompoundAssignmentInstruction inst, AssignmentOperatorType op)
		{
			var target = Translate(inst.Target);
			var value = Translate(inst.Value);
			value = PrepareArithmeticArgument(value, inst.Value.ResultType, inst.Sign, isLifted: false);
			
			TranslatedExpression resultExpr;
			if (inst.CompoundAssignmentType == CompoundAssignmentType.EvaluatesToOldValue) {
				Debug.Assert(op == AssignmentOperatorType.Add || op == AssignmentOperatorType.Subtract);
				Debug.Assert(value.ResolveResult.IsCompileTimeConstant && 1.Equals(value.ResolveResult.ConstantValue));
				UnaryOperatorType unary;
				ExpressionType exprType;
				if (op == AssignmentOperatorType.Add) {
					unary = UnaryOperatorType.PostIncrement;
					exprType = ExpressionType.PostIncrementAssign;
				} else {
					unary = UnaryOperatorType.PostDecrement;
					exprType = ExpressionType.PostDecrementAssign;
				}
				resultExpr = new UnaryOperatorExpression(unary, target)
					.WithILInstruction(inst)
					.WithRR(new OperatorResolveResult(target.Type, exprType, target.ResolveResult));
			} else {
				switch (op) {
					case AssignmentOperatorType.Add:
					case AssignmentOperatorType.Subtract:
						value = value.ConvertTo(target.Type.GetEnumUnderlyingType(), this, inst.CheckForOverflow);
						break;
					case AssignmentOperatorType.Multiply:
					case AssignmentOperatorType.Divide:
					case AssignmentOperatorType.Modulus:
					case AssignmentOperatorType.BitwiseAnd:
					case AssignmentOperatorType.BitwiseOr:
					case AssignmentOperatorType.ExclusiveOr:
						value = value.ConvertTo(target.Type, this, inst.CheckForOverflow);
						break;
				}
				resultExpr = new AssignmentExpression(target.Expression, op, value.Expression)
					.WithILInstruction(inst)
					.WithRR(new OperatorResolveResult(target.Type, AssignmentExpression.GetLinqNodeType(op, inst.CheckForOverflow), target.ResolveResult, value.ResolveResult));
			}
			if (AssignmentOperatorMightCheckForOverflow(op))
				resultExpr.Expression.AddAnnotation(inst.CheckForOverflow ? AddCheckedBlocks.CheckedAnnotation : AddCheckedBlocks.UncheckedAnnotation);
			return resultExpr;
		}
		
		TranslatedExpression HandleCompoundShift(CompoundAssignmentInstruction inst, AssignmentOperatorType op)
		{
			var target = Translate(inst.Target);
			var value = Translate(inst.Value);
			
			IType targetType;
			if (inst.ResultType == StackType.I4)
				targetType = compilation.FindType(inst.Sign == Sign.Unsigned ? KnownTypeCode.UInt32 : KnownTypeCode.Int32);
			else
				targetType = compilation.FindType(inst.Sign == Sign.Unsigned ? KnownTypeCode.UInt64 : KnownTypeCode.Int64);
			target = target.ConvertTo(targetType, this);
			
			// Shift operators in C# always expect type 'int' on the right-hand-side
			value = value.ConvertTo(compilation.FindType(KnownTypeCode.Int32), this);
			
			TranslatedExpression result = new AssignmentExpression(target.Expression, op, value.Expression)
				.WithILInstruction(inst)
				.WithRR(resolver.ResolveAssignment(op, target.ResolveResult, value.ResolveResult));
			if (inst.ResultType == StackType.I) {
				// C# doesn't have shift operators for IntPtr, so we first shifted a long/ulong,
				// and now have to case back down to IntPtr/UIntPtr:
				result = result.ConvertTo(compilation.FindType(inst.Sign == Sign.Unsigned ? KnownTypeCode.UIntPtr : KnownTypeCode.IntPtr), this);
			}
			return result;
		}
		
		static bool AssignmentOperatorMightCheckForOverflow(AssignmentOperatorType op)
		{
			switch (op) {
				case AssignmentOperatorType.BitwiseAnd:
				case AssignmentOperatorType.BitwiseOr:
				case AssignmentOperatorType.ExclusiveOr:
				case AssignmentOperatorType.ShiftLeft:
				case AssignmentOperatorType.ShiftRight:
					return false;
				default:
					return true;
			}
		}
		
		protected internal override TranslatedExpression VisitConv(Conv inst, TranslationContext context)
		{
			var arg = Translate(inst.Argument);
			IType inputType = NullableType.GetUnderlyingType(arg.Type);
			StackType inputStackType = inst.InputType;
			// Note: we're dealing with two conversions here:
			// a) the implicit conversion from `inputType` to `inputStackType`
			//    (due to the ExpressionBuilder post-condition being flexible with regards to the integer type width)
			//    If this is a widening conversion, I'm calling the argument C# type "oversized".
			//    If this is a narrowing conversion, I'm calling the argument C# type "undersized".
			// b) the actual conversion instruction from `inputStackType` to `inst.TargetType`

			// Also, we need to be very careful with regards to the conversions we emit:
			// In C#, zero vs. sign-extension depends on the input type,
			// but in the ILAst conv instruction it depends on the output type.
			// However, in the conv.ovf instructions, the .NET runtime behavior seems to depend on the input type,
			// in violation of the ECMA-335 spec!

			IType GetType(KnownTypeCode typeCode)
			{
				IType type = compilation.FindType(typeCode);
				if (inst.IsLifted)
					type = NullableType.Create(compilation, type);
				return type;
			}

			if (inst.CheckForOverflow || inst.Kind == ConversionKind.IntToFloat) {
				// We need to first convert the argument to the expected sign.
				// We also need to perform any input narrowing conversion so that it doesn't get mixed up with the overflow check.
				Debug.Assert(inst.InputSign != Sign.None);
				if (inputType.GetSize() > inputStackType.GetSize() || inputType.GetSign() != inst.InputSign) {
					arg = arg.ConvertTo(GetType(inputStackType.ToKnownTypeCode(inst.InputSign)), this);
				}
				// Because casts with overflow check match C# semantics (zero/sign-extension depends on source type),
				// we can just directly cast to the target type.
				return arg.ConvertTo(GetType(inst.TargetType.ToKnownTypeCode()), this, inst.CheckForOverflow)
					.WithILInstruction(inst);
			}
			
			switch (inst.Kind) {
				case ConversionKind.StopGCTracking:
					if (inputType.Kind == TypeKind.ByReference) {
						// cast to corresponding pointer type:
						var pointerType = new PointerType(((ByReferenceType)inputType).ElementType);
						return arg.ConvertTo(pointerType, this).WithILInstruction(inst);
					} else {
						Debug.Fail("ConversionKind.StopGCTracking should only be used with managed references");
						goto default;
					}
				case ConversionKind.SignExtend:
					// We just need to ensure the input type before the conversion is signed.
					// Also, if the argument was translated into an oversized C# type,
					// we need to perform the truncatation to the input stack type.
					if (inputType.GetSign() != Sign.Signed || inputType.GetSize() > inputStackType.GetSize()) {
						// Note that an undersized C# type is handled just fine:
						// If it is unsigned we'll zero-extend it to the width of the inputStackType here,
						// and it is signed we just combine the two sign-extensions into a single sign-extending conversion.
						arg = arg.ConvertTo(GetType(inputStackType.ToKnownTypeCode(Sign.Signed)), this);
					}
					// Then, we can just return the argument as-is: the ExpressionBuilder post-condition allows us
					// to force our parent instruction to handle the actual sign-extension conversion.
					// (our caller may have more information to pick a better fitting target type)
					return arg.WithILInstruction(inst);
				case ConversionKind.ZeroExtend:
					// If overflow check cannot fail, handle this just like sign extension (except for swapped signs)
					if (inputType.GetSign() != Sign.Unsigned || inputType.GetSize() > inputStackType.GetSize()) {
						arg = arg.ConvertTo(GetType(inputStackType.ToKnownTypeCode(Sign.Unsigned)), this);
					}
					return arg.WithILInstruction(inst);
				case ConversionKind.Nop:
					// no need to generate any C# code for a nop conversion
					return arg.WithILInstruction(inst);
				case ConversionKind.Truncate:
					// Note: there are three sizes involved here:
					// A = inputType.GetSize()
					// B = inputStackType.GetSize()
					// C = inst.TargetType.GetSize().
					// We know that C <= B (otherwise this wouldn't be the truncation case).
					// 1) If C < B < A, we just combine the two truncations into one.
					// 2) If C < B = A, there's no input conversion, just the truncation
					// 3) If C <= A < B, all the extended bits get removed again by the truncation.
					// 4) If A < C < B, some extended bits remain even after truncation.
					// In cases 1-3, the overall conversion is a truncation or no-op.
					// In case 4, the overall conversion is a zero/sign extension, but to a smaller
					// size than the original conversion.
					if (inst.TargetType.IsSmallIntegerType()) {
						// If the target type is a small integer type, IL will implicitly sign- or zero-extend
						// the result after the truncation back to StackType.I4.
						// (which means there's actually 3 conversions involved!)
						// Note that we must handle truncation to small integer types ourselves:
						// our caller only sees the StackType.I4 and doesn't know to truncate to the small type.
						
						if (inputType.GetSize() <= inst.TargetType.GetSize() && inputType.GetSign() == inst.TargetType.GetSign()) {
							// There's no actual truncation involved, and the result of the Conv instruction is extended
							// the same way as the original instruction
							// -> we can return arg directly
							return arg.WithILInstruction(inst);
						} else {
							// We need to actually truncate; *or* we need to change the sign for the remaining extension to I4.
							goto default; // Emit simple cast to inst.TargetType
						}
					} else {
						Debug.Assert(inst.TargetType.GetSize() == inst.UnderlyingResultType.GetSize());
						// For non-small integer types, we can let the whole unchecked truncation
						// get handled by our caller (using the ExpressionBuilder post-condition).
						
						// Case 4 (left-over extension from implicit conversion) can also be handled by our caller.
						return arg.WithILInstruction(inst);
					}
				default:
					return arg.ConvertTo(GetType(inst.TargetType.ToKnownTypeCode()), this, inst.CheckForOverflow)
						.WithILInstruction(inst);
			}
		}

		protected internal override TranslatedExpression VisitCall(Call inst, TranslationContext context)
		{
			return new CallBuilder(this, typeSystem, settings).Build(inst);
		}
		
		protected internal override TranslatedExpression VisitCallVirt(CallVirt inst, TranslationContext context)
		{
			return new CallBuilder(this, typeSystem, settings).Build(inst);
		}

		internal TranslatedExpression TranslateFunction(TranslatedExpression objectCreateExpression, TranslatedExpression target, ILFunction function)
		{
			var method = typeSystem.Resolve(function.Method)?.MemberDefinition as IMethod;
			Debug.Assert(method != null);

			// Create AnonymousMethodExpression and prepare parameters
			AnonymousMethodExpression ame = new AnonymousMethodExpression();
			ame.IsAsync = function.IsAsync;
			ame.Parameters.AddRange(MakeParameters(method, function));
			ame.HasParameterList = true;
			StatementBuilder builder = new StatementBuilder(typeSystem.GetSpecializingTypeSystem(new SimpleTypeResolveContext(method)), this.decompilationContext, method, function, settings, cancellationToken);
			var body = builder.ConvertAsBlock(function.Body);
			bool isLambda = false;
			bool isMultiLineLambda = false;

			Comment prev = null;
			foreach (string warning in function.Warnings) {
				body.InsertChildAfter(prev, prev = new Comment(warning), Roles.Comment);
			}

			// if there is an anonymous type involved, we are forced to use a lambda expression.
			if (ame.Parameters.Any(p => p.Type.IsNull)) {
				isLambda = true;
				isMultiLineLambda = body.Statements.Count > 1;
			} else if (ame.Parameters.All(p => p.ParameterModifier == ParameterModifier.None)) {
				isLambda = (body.Statements.Count == 1 && body.Statements.Single() is ReturnStatement);
			}
			// Remove the parameter list from an AnonymousMethodExpression if the original method had no names,
			// and the parameters are not used in the method body
			if (!isLambda && method.Parameters.All(p => string.IsNullOrEmpty(p.Name))) {
				var parameterReferencingIdentifiers =
					from ident in body.Descendants.OfType<IdentifierExpression>()
					let v = ident.Annotation<ILVariable>()
					where v != null && v.Kind == VariableKind.Parameter
					select ident;
				if (!parameterReferencingIdentifiers.Any()) {
					ame.Parameters.Clear();
					ame.HasParameterList = false;
				}
			}
			
			Expression replacement;
			if (isLambda) {
				LambdaExpression lambda = new LambdaExpression();
				lambda.CopyAnnotationsFrom(ame);
				ame.Parameters.MoveTo(lambda.Parameters);
				if (isMultiLineLambda) {
					lambda.Body = body;
				} else {
					Expression returnExpr = ((ReturnStatement)body.Statements.Single()).Expression;
					returnExpr.Remove();
					lambda.Body = returnExpr;
				}
				replacement = lambda;
			} else {
				ame.Body = body;
				replacement = ame;
			}
			var expectedType = objectCreateExpression.ResolveResult.Type;
			var expectedTypeDefinition = expectedType.GetDefinition();
			if (expectedTypeDefinition != null && expectedTypeDefinition.Kind != TypeKind.Delegate) {
				var simplifiedDelegateCreation = (ObjectCreateExpression)objectCreateExpression.Expression.Clone();
				simplifiedDelegateCreation.Arguments.Clear();
				simplifiedDelegateCreation.Arguments.Add(replacement);
				replacement = simplifiedDelegateCreation;
			} else if (!settings.AnonymousTypes || !expectedType.ContainsAnonymousType()) {
				replacement = new CastExpression(ConvertType(expectedType), replacement);
			}
			return replacement
				.WithILInstruction(function)
				.WithRR(objectCreateExpression.ResolveResult);
		}
		
		IEnumerable<ParameterDeclaration> MakeParameters(IMethod method, ILFunction function)
		{
			var variables = function.Variables.Where(v => v.Kind == VariableKind.Parameter).ToDictionary(v => v.Index);
			int i = 0;
			foreach (var parameter in method.Parameters) {
				var pd = astBuilder.ConvertParameter(parameter);
				if (settings.AnonymousTypes && parameter.Type.ContainsAnonymousType())
					pd.Type = null;
				ILVariable v;
				if (variables.TryGetValue(i, out v))
					pd.AddAnnotation(new ILVariableResolveResult(v, method.Parameters[i].Type));
				yield return pd;
				i++;
			}
		}
		
		internal TranslatedExpression TranslateTarget(IMember member, ILInstruction target, bool nonVirtualInvocation)
		{
			// If references are missing member.IsStatic might not be set correctly.
			// Additionally check target for null, in order to avoid a crash.
			if (!member.IsStatic && target != null) {
				if (nonVirtualInvocation && target.MatchLdThis() && member.DeclaringTypeDefinition != resolver.CurrentTypeDefinition) {
					return new BaseReferenceExpression()
						.WithILInstruction(target)
						.WithRR(new ThisResolveResult(member.DeclaringType, nonVirtualInvocation));
				} else {
					var translatedTarget = Translate(target);
					if (member.DeclaringType.IsReferenceType == false && translatedTarget.Type.GetStackType().IsIntegerType()) {
						// when accessing members on value types, ensure we use a reference and not a pointer
						translatedTarget = translatedTarget.ConvertTo(new ByReferenceType(member.DeclaringType), this);
					}
					if (translatedTarget.Expression is DirectionExpression) {
						translatedTarget = translatedTarget.UnwrapChild(((DirectionExpression)translatedTarget).Expression);
					}
					return translatedTarget;
				}
			} else {
				return new TypeReferenceExpression(ConvertType(member.DeclaringType))
					.WithoutILInstruction()
					.WithRR(new TypeResolveResult(member.DeclaringType));
			}
		}
		
		protected internal override TranslatedExpression VisitLdObj(LdObj inst, TranslationContext context)
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

		protected internal override TranslatedExpression VisitStObj(StObj inst, TranslationContext context)
		{
			var target = Translate(inst.Target);
			TranslatedExpression result;
			if (target.Expression is DirectionExpression && TypeUtils.IsCompatibleTypeForMemoryAccess(target.Type, inst.Type)) {
				// we can deference the managed reference by stripping away the 'ref'
				result = target.UnwrapChild(((DirectionExpression)target.Expression).Expression);
			} else {
				// Cast pointer type if necessary:
				if (!TypeUtils.IsCompatibleTypeForMemoryAccess(target.Type, inst.Type)) {
					target = target.ConvertTo(new PointerType(inst.Type), this);
				}
				result = new UnaryOperatorExpression(UnaryOperatorType.Dereference, target.Expression)
					.WithoutILInstruction()
					.WithRR(new ResolveResult(((TypeWithElementType)target.Type).ElementType));
			}
			var value = Translate(inst.Value, typeHint: result.Type);
			return Assignment(result, value).WithILInstruction(inst);
		}

		protected internal override TranslatedExpression VisitLdLen(LdLen inst, TranslationContext context)
		{
			TranslatedExpression arrayExpr = Translate(inst.Array);
			if (arrayExpr.Type.Kind != TypeKind.Array) {
				arrayExpr = arrayExpr.ConvertTo(compilation.FindType(KnownTypeCode.Array), this);
			}
			if (inst.ResultType == StackType.I4) {
				return new MemberReferenceExpression(arrayExpr.Expression, "Length")
					.WithILInstruction(inst)
					.WithRR(new ResolveResult(compilation.FindType(KnownTypeCode.Int32)));
			} else {
				return new MemberReferenceExpression(arrayExpr.Expression, "LongLength")
					.WithILInstruction(inst)
					.WithRR(new ResolveResult(compilation.FindType(KnownTypeCode.Int64)));
			}
		}
		
		protected internal override TranslatedExpression VisitLdFlda(LdFlda inst, TranslationContext context)
		{
			var expr = ConvertField(inst.Field, inst.Target);
			return new DirectionExpression(FieldDirection.Ref, expr)
				.WithoutILInstruction().WithRR(new ResolveResult(new ByReferenceType(expr.Type)));
		}
		
		protected internal override TranslatedExpression VisitLdsFlda(LdsFlda inst, TranslationContext context)
		{
			var expr = ConvertField(inst.Field);
			return new DirectionExpression(FieldDirection.Ref, expr)
				.WithoutILInstruction().WithRR(new ResolveResult(new ByReferenceType(expr.Type)));
		}
		
		protected internal override TranslatedExpression VisitLdElema(LdElema inst, TranslationContext context)
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
			var input = Translate(i);
			KnownTypeCode targetType;
			if (i.ResultType == StackType.I4) {
				if (input.Type.IsSmallIntegerType() && input.Type.Kind != TypeKind.Enum) {
					return input; // we don't need a cast, just let small integers be promoted to int
				}
				targetType = input.Type.GetSign() == Sign.Unsigned ? KnownTypeCode.UInt32 : KnownTypeCode.Int32;
			} else {
				targetType = input.Type.GetSign() == Sign.Unsigned ? KnownTypeCode.UInt64 : KnownTypeCode.Int64;
			}
			return input.ConvertTo(compilation.FindType(targetType), this);
		}
		
		protected internal override TranslatedExpression VisitUnboxAny(UnboxAny inst, TranslationContext context)
		{
			var arg = Translate(inst.Argument);
			if (arg.Type.IsReferenceType != true) {
				// ensure we treat the input as a reference type
				arg = arg.ConvertTo(compilation.FindType(KnownTypeCode.Object), this);
			}

			IType targetType = inst.Type;
			if (targetType.Kind == TypeKind.TypeParameter) {
				var rr = resolver.ResolveCast(targetType, arg.ResolveResult);
				if (rr.IsError) {
					// C# 6.2.7 Explicit conversions involving type parameters:
					// if we can't directly convert to a type parameter,
					// try via its effective base class.
					arg = arg.ConvertTo(((ITypeParameter)targetType).EffectiveBaseClass, this);
				}
			}
			return new CastExpression(ConvertType(targetType), arg.Expression)
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(targetType, arg.ResolveResult, Conversion.UnboxingConversion));
		}
		
		protected internal override TranslatedExpression VisitUnbox(Unbox inst, TranslationContext context)
		{
			var arg = Translate(inst.Argument);
			var castExpression = new CastExpression(ConvertType(inst.Type), arg.Expression)
				.WithRR(new ConversionResolveResult(inst.Type, arg.ResolveResult, Conversion.UnboxingConversion));
			return new DirectionExpression(FieldDirection.Ref, castExpression)
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(new ByReferenceType(inst.Type), arg.ResolveResult, Conversion.UnboxingConversion));
		}

		protected internal override TranslatedExpression VisitBox(Box inst, TranslationContext context)
		{
			var obj = compilation.FindType(KnownTypeCode.Object);
			var arg = Translate(inst.Argument, typeHint: inst.Type).ConvertTo(inst.Type, this);
			return new CastExpression(ConvertType(obj), arg.Expression)
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(obj, arg.ResolveResult, Conversion.BoxingConversion));
		}
		
		protected internal override TranslatedExpression VisitCastClass(CastClass inst, TranslationContext context)
		{
			return Translate(inst.Argument).ConvertTo(inst.Type, this);
		}
		
		protected internal override TranslatedExpression VisitArglist(Arglist inst, TranslationContext context)
		{
			return new UndocumentedExpression { UndocumentedExpressionType = UndocumentedExpressionType.ArgListAccess }
			.WithILInstruction(inst)
				.WithRR(new TypeResolveResult(compilation.FindType(new TopLevelTypeName("System", "RuntimeArgumentHandle"))));
		}
		
		protected internal override TranslatedExpression VisitMakeRefAny(MakeRefAny inst, TranslationContext context)
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
		
		protected internal override TranslatedExpression VisitRefAnyType(RefAnyType inst, TranslationContext context)
		{
			return new MemberReferenceExpression(new UndocumentedExpression {
				UndocumentedExpressionType = UndocumentedExpressionType.RefType,
				Arguments = { Translate(inst.Argument).Expression.Detach() }
			}, "TypeHandle")
				.WithILInstruction(inst)
				.WithRR(new TypeResolveResult(compilation.FindType(new TopLevelTypeName("System", "RuntimeTypeHandle"))));
		}
		
		protected internal override TranslatedExpression VisitRefAnyValue(RefAnyValue inst, TranslationContext context)
		{
			var expr = new UndocumentedExpression {
				UndocumentedExpressionType = UndocumentedExpressionType.RefValue,
				Arguments = { Translate(inst.Argument).Expression, new TypeReferenceExpression(ConvertType(inst.Type)) }
			}.WithRR(new ResolveResult(inst.Type));
			return new DirectionExpression(FieldDirection.Ref, expr.WithILInstruction(inst)).WithoutILInstruction()
				.WithRR(new ByReferenceResolveResult(inst.Type, false));
		}
		
		protected internal override TranslatedExpression VisitBlock(Block block, TranslationContext context)
		{
			switch (block.Type) {
				case BlockType.ArrayInitializer:
					return TranslateArrayInitializer(block);
				case BlockType.CollectionInitializer:
				case BlockType.ObjectInitializer:
					return TranslateObjectAndCollectionInitializer(block);
				case BlockType.PostfixOperator:
					return TranslatePostfixOperator(block);
				default:
					return ErrorExpression("Unknown block type: " + block.Type);
			}
		}

		TranslatedExpression TranslateObjectAndCollectionInitializer(Block block)
		{
			var stloc = block.Instructions.FirstOrDefault() as StLoc;
			var final = block.FinalInstruction as LdLoc;
			if (stloc == null || final == null || stloc.Variable != final.Variable || stloc.Variable.Kind != VariableKind.InitializerTarget)
				throw new ArgumentException("given Block is invalid!");
			InitializedObjectResolveResult initObjRR;
			TranslatedExpression expr;
			switch (stloc.Value) {
				case NewObj newObjInst:
					initObjRR = new InitializedObjectResolveResult(newObjInst.Method.DeclaringType);
					expr = new CallBuilder(this, typeSystem, settings).Build(newObjInst);
					break;
				case DefaultValue defaultVal:
					initObjRR = new InitializedObjectResolveResult(defaultVal.Type);
					expr = new ObjectCreateExpression(ConvertType(defaultVal.Type))
						.WithILInstruction(defaultVal)
						.WithRR(new TypeResolveResult(defaultVal.Type));
					break;
				default:
					throw new ArgumentException("given Block is invalid!");
			}
			var elementsStack = new Stack<List<Expression>>();
			var elements = new List<Expression>(block.Instructions.Count);
			elementsStack.Push(elements);
			List<IL.Transforms.AccessPathElement> currentPath = null;
			var indexVariables = new Dictionary<ILVariable, ILInstruction>();
			foreach (var inst in block.Instructions.Skip(1)) {
				if (inst is StLoc indexStore) {
					indexVariables.Add(indexStore.Variable, indexStore.Value);
					continue;
				}
				var info = IL.Transforms.AccessPathElement.GetAccessPath(inst, initObjRR.Type);
				if (info.Kind == IL.Transforms.AccessPathKind.Invalid) continue;
				if (currentPath == null) {
					currentPath = info.Path;
				} else {
					int firstDifferenceIndex = Math.Min(currentPath.Count, info.Path.Count);
					int index = 0;
					while (index < firstDifferenceIndex && info.Path[index] == currentPath[index])
						index++;
					firstDifferenceIndex = index;
					while (elementsStack.Count - 1 > firstDifferenceIndex) {
						var methodElement = currentPath[elementsStack.Count - 1];
						var pathElement = currentPath[elementsStack.Count - 2];
						var values = elementsStack.Pop();
						elementsStack.Peek().Add(MakeInitializerAssignment(methodElement.Member, pathElement, values, indexVariables));
					}
					currentPath = info.Path;
				}
				while (elementsStack.Count < currentPath.Count)
					elementsStack.Push(new List<Expression>());
				var lastElement = currentPath.Last();
				var memberRR = new MemberResolveResult(initObjRR, lastElement.Member);
				switch (info.Kind) {
					case IL.Transforms.AccessPathKind.Adder:
						elementsStack.Peek().Add(MakeInitializerElements(info.Values, ((IMethod)lastElement.Member).Parameters));
						break;
					case IL.Transforms.AccessPathKind.Setter:
						if (lastElement.Indices?.Length > 0) {
							var indexer = new IndexerExpression(null, lastElement.Indices.SelectArray(i => Translate(i is LdLoc ld ? indexVariables[ld.Variable] : i).Expression))
								.WithILInstruction(inst).WithRR(memberRR);
							elementsStack.Peek().Add(Assignment(indexer, Translate(info.Values.Single())));
						} else {
							var target = new IdentifierExpression(lastElement.Member.Name)
								.WithILInstruction(inst).WithRR(memberRR);
							elementsStack.Peek().Add(Assignment(target, Translate(info.Values.Single())));
						}
						break;
				}
			}
			while (elementsStack.Count > 1) {
				var methodElement = currentPath[elementsStack.Count - 1];
				var pathElement = currentPath[elementsStack.Count - 2];
				var values = elementsStack.Pop();
				elementsStack.Peek().Add(MakeInitializerAssignment(methodElement.Member, pathElement, values, indexVariables));
			}
			var oce = (ObjectCreateExpression)expr.Expression;
			oce.Initializer = new ArrayInitializerExpression(elements);
			return expr.WithILInstruction(block);
		}

		Expression MakeInitializerAssignment(IMember method, IL.Transforms.AccessPathElement member, List<Expression> values, Dictionary<ILVariable, ILInstruction> indexVariables)
		{
			var target = member.Indices?.Length > 0 ? (Expression)new IndexerExpression(null, member.Indices.SelectArray(i => Translate(i is LdLoc ld ? indexVariables[ld.Variable] : i).Expression)) : new IdentifierExpression(member.Member.Name);
			Expression value;
			if (values.Count == 1 && !(values[0] is AssignmentExpression) && !(method.SymbolKind == SymbolKind.Method && method.Name == "Add"))
				value = values[0];
			else
				value = new ArrayInitializerExpression(values);
			return new AssignmentExpression(target, value);
		}

		Expression MakeInitializerElements(List<ILInstruction> values, IList<IParameter> parameters)
		{
			if (values.Count == 1)
				return Translate(values[0]).ConvertTo(parameters[0].Type, this);
			var expressions = new Expression[values.Count];
			for (int i = 0; i < values.Count; i++)
				expressions[i] = Translate(values[i]).ConvertTo(parameters[i].Type, this);
			return new ArrayInitializerExpression(expressions);
		}

		readonly static ArraySpecifier[] NoSpecifiers = new ArraySpecifier[0];

		TranslatedExpression TranslateArrayInitializer(Block block)
		{
			var stloc = block.Instructions.FirstOrDefault() as StLoc;
			var final = block.FinalInstruction as LdLoc;
			IType type;
			if (stloc == null || final == null || !stloc.Value.MatchNewArr(out type) || stloc.Variable != final.Variable || stloc.Variable.Kind != VariableKind.InitializerTarget)
				throw new ArgumentException("given Block is invalid!");
			var newArr = (NewArr)stloc.Value;
			
			var translatedDimensions = newArr.Indices.Select(i => Translate(i)).ToArray();
			
			if (!translatedDimensions.All(dim => dim.ResolveResult.IsCompileTimeConstant))
				throw new ArgumentException("given Block is invalid!");
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
					throw new ArgumentException("given Block is invalid!");
				if (!target.MatchLdElema(out t, out array) || !type.Equals(t))
					throw new ArgumentException("given Block is invalid!");
				if (!array.MatchLdLoc(out v) || v != final.Variable)
					throw new ArgumentException("given Block is invalid!");
				while (container.Count < dimensions) {
					var aie = new ArrayInitializerExpression();
					container.Peek().Elements.Add(aie);
					container.Push(aie);
				}
				var val = Translate(value).ConvertTo(type, this, allowImplicitConversion: true);
				container.Peek().Elements.Add(val);
				elementResolveResults.Add(val.ResolveResult);
				while (container.Count > 0 && container.Peek().Elements.Count == dimensionSizes[container.Count - 1]) {
					container.Pop();
				}
			}
			ArraySpecifier[] additionalSpecifiers;
			AstType typeExpression;
			if (settings.AnonymousTypes && type.ContainsAnonymousType()) {
				typeExpression = null;
				additionalSpecifiers = new[] { new ArraySpecifier() };
			} else {
				typeExpression = ConvertType(type);
				if (typeExpression is ComposedType) {
					additionalSpecifiers = ((ComposedType)typeExpression).ArraySpecifiers.SelectArray(a => (ArraySpecifier)a.Clone());
					typeExpression = ((ComposedType)typeExpression).BaseType.Clone();
				} else {
					additionalSpecifiers = NoSpecifiers;
				}
			}
			var expr = new ArrayCreateExpression {
				Type = typeExpression,
				Initializer = root
			};
			expr.AdditionalArraySpecifiers.AddRange(additionalSpecifiers);
			if (!(bool)type.ContainsAnonymousType())
				expr.Arguments.AddRange(newArr.Indices.Select(i => Translate(i).Expression));
			return expr.WithILInstruction(block)
				.WithRR(new ArrayCreateResolveResult(new ArrayType(compilation, type, dimensions), newArr.Indices.Select(i => Translate(i).ResolveResult).ToArray(), elementResolveResults));
		}
		
		TranslatedExpression TranslatePostfixOperator(Block block)
		{
			var targetInst = (block.Instructions.ElementAtOrDefault(0) as StLoc)?.Value;
			var inst = (block.Instructions.ElementAtOrDefault(1) as StLoc)?.Value as BinaryNumericInstruction;
			if (targetInst == null || inst == null || (inst.Operator != BinaryNumericOperator.Add && inst.Operator != BinaryNumericOperator.Sub))
				throw new ArgumentException("given Block is invalid!");
			var op = inst.Operator == BinaryNumericOperator.Add ? UnaryOperatorType.PostIncrement : UnaryOperatorType.PostDecrement;
			var target = Translate(targetInst);
			return new UnaryOperatorExpression(op, target)
				.WithILInstruction(block)
				.WithRR(resolver.WithCheckForOverflow(inst.CheckForOverflow).ResolveUnaryOperator(op, target.ResolveResult));
		}

		/// <summary>
		/// If expr is a constant integer expression, and its value fits into type,
		/// convert the expression into the target type.
		/// Otherwise, returns the expression unmodified.
		/// </summary>
		TranslatedExpression AdjustConstantExpressionToType(TranslatedExpression expr, IType type)
		{
			if (!expr.ResolveResult.IsCompileTimeConstant) {
				return expr;
			}
			type = NullableType.GetUnderlyingType(type);
			if (type.IsKnownType(KnownTypeCode.Boolean)
				&& (object.Equals(expr.ResolveResult.ConstantValue, 0) || object.Equals(expr.ResolveResult.ConstantValue, 1))) {
				return expr.ConvertToBoolean(this);
			} else if (type.Kind == TypeKind.Enum || type.IsKnownType(KnownTypeCode.Char)) {
				var castRR = resolver.WithCheckForOverflow(true).ResolveCast(type, expr.ResolveResult);
				if (castRR.IsCompileTimeConstant && !castRR.IsError) {
					return ConvertConstantValue(castRR).WithILInstruction(expr.ILInstructions);
				}
			}
			return expr;
		}

		protected internal override TranslatedExpression VisitNullCoalescingInstruction(NullCoalescingInstruction inst, TranslationContext context)
		{
			var value = Translate(inst.ValueInst);
			var fallback = Translate(inst.FallbackInst);
			fallback = AdjustConstantExpressionToType(fallback, value.Type);
			var rr = resolver.ResolveBinaryOperator(BinaryOperatorType.NullCoalescing, value.ResolveResult, fallback.ResolveResult);
			if (rr.IsError) {
				IType targetType;
				if (!value.Type.Equals(SpecialType.NullType) && !fallback.Type.Equals(SpecialType.NullType) && !value.Type.Equals(fallback.Type)) {
					targetType = compilation.FindType(inst.UnderlyingResultType.ToKnownTypeCode());
				} else {
					targetType = value.Type.Equals(SpecialType.NullType) ? fallback.Type : value.Type;
				}
				if (inst.Kind != NullCoalescingKind.Ref) {
					value = value.ConvertTo(NullableType.Create(compilation, targetType), this);
				} else {
					value = value.ConvertTo(targetType, this);
				}
				if (inst.Kind == NullCoalescingKind.Nullable) {
					value = value.ConvertTo(NullableType.Create(compilation, targetType), this);
				} else {
					fallback = fallback.ConvertTo(targetType, this);
				}
				rr = new ResolveResult(targetType);
			}
			return new BinaryOperatorExpression(value, BinaryOperatorType.NullCoalescing, fallback)
				.WithILInstruction(inst)
				.WithRR(rr);
		}

		protected internal override TranslatedExpression VisitIfInstruction(IfInstruction inst, TranslationContext context)
		{
			var condition = TranslateCondition(inst.Condition);
			var trueBranch = Translate(inst.TrueInst);
			var falseBranch = Translate(inst.FalseInst);
			BinaryOperatorType op = BinaryOperatorType.Any;
			TranslatedExpression rhs = default(TranslatedExpression);

			if (inst.MatchLogicAnd(out var lhsInst, out var rhsInst)) {
				op = BinaryOperatorType.ConditionalAnd;
				Debug.Assert(rhsInst == inst.TrueInst);
				rhs = trueBranch;
			} else if (inst.MatchLogicOr(out lhsInst, out rhsInst)) {
				op = BinaryOperatorType.ConditionalOr;
				Debug.Assert(rhsInst == inst.FalseInst);
				rhs = falseBranch;
			}
			// ILAst LogicAnd/LogicOr can return a different value than 0 or 1
			// if the rhs is evaluated.
			// We can only correctly translate it to C# if the rhs is of type boolean:
			if (op != BinaryOperatorType.Any && rhs.Type.IsKnownType(KnownTypeCode.Boolean)) {
				return new BinaryOperatorExpression(condition, op, rhs)
					.WithILInstruction(inst)
					.WithRR(new ResolveResult(compilation.FindType(KnownTypeCode.Boolean)));
			}

			trueBranch = AdjustConstantExpressionToType(trueBranch, falseBranch.Type);
			falseBranch = AdjustConstantExpressionToType(falseBranch, trueBranch.Type);

			var rr = resolver.ResolveConditional(condition.ResolveResult, trueBranch.ResolveResult, falseBranch.ResolveResult);
			if (rr.IsError) {
				IType targetType;
				if (!trueBranch.Type.Equals(SpecialType.NullType) && !falseBranch.Type.Equals(SpecialType.NullType) && !trueBranch.Type.Equals(falseBranch.Type)) {
					targetType = compilation.FindType(inst.ResultType.ToKnownTypeCode());
				} else {
					targetType = trueBranch.Type.Equals(SpecialType.NullType) ? falseBranch.Type : trueBranch.Type;
				}
				trueBranch = trueBranch.ConvertTo(targetType, this);
				falseBranch = falseBranch.ConvertTo(targetType, this);
				rr = new ResolveResult(targetType);
			}
			return new ConditionalExpression(condition.Expression, trueBranch.Expression, falseBranch.Expression)
				.WithILInstruction(inst)
				.WithRR(rr);
		}
		
		protected internal override TranslatedExpression VisitAddressOf(AddressOf inst, TranslationContext context)
		{
			// HACK: this is only correct if the argument is an R-value; otherwise we're missing the copy to the temporary
			var value = Translate(inst.Value);
			return new DirectionExpression(FieldDirection.Ref, value)
				.WithILInstruction(inst)
				.WithRR(new ByReferenceResolveResult(value.ResolveResult, false));
		}

		protected internal override TranslatedExpression VisitAwait(Await inst, TranslationContext context)
		{
			IType expectedType = null;
			if (inst.GetAwaiterMethod != null) {
				if (inst.GetAwaiterMethod.IsStatic) {
					expectedType = inst.GetAwaiterMethod.Parameters.FirstOrDefault()?.Type;
				} else {
					expectedType = inst.GetAwaiterMethod.DeclaringType;
				}
			}

			var value = Translate(inst.Value, typeHint: expectedType);
			if (value.Expression is DirectionExpression) {
				// we can deference the managed reference by stripping away the 'ref'
				value = value.UnwrapChild(((DirectionExpression)value.Expression).Expression);
			}
			if (expectedType != null) {
				value = value.ConvertTo(expectedType, this, allowImplicitConversion: true);
			}
			return new UnaryOperatorExpression(UnaryOperatorType.Await, value.Expression)
				.WithILInstruction(inst)
				.WithRR(new ResolveResult(inst.GetResultMethod?.ReturnType ?? SpecialType.UnknownType));
		}

		protected internal override TranslatedExpression VisitInvalidBranch(InvalidBranch inst, TranslationContext context)
		{
			string message = "Error";
			if (inst.ILRange.Start != 0) {
				message += $" near IL_{inst.ILRange.Start:x4}";
			}
			if (!string.IsNullOrEmpty(inst.Message)) {
				message += ": " + inst.Message;
			}
			return ErrorExpression(message);
		}

		protected internal override TranslatedExpression VisitInvalidExpression(InvalidExpression inst, TranslationContext context)
		{
			string message = "Error";
			if (inst.ILRange.Start != 0) {
				message += $" near IL_{inst.ILRange.Start:x4}";
			}
			if (!string.IsNullOrEmpty(inst.Message)) {
				message += ": " + inst.Message;
			}
			return ErrorExpression(message);
		}

		protected override TranslatedExpression Default(ILInstruction inst, TranslationContext context)
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
