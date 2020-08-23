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
	///      * If the IL instruction evaluates to a managed reference (Ref) created by starting tracking of an unmanaged reference,
	///        the C# instruction may evaluate to any integral/enum/pointer type that when converted to pointer type
	///        is equivalent to the managed reference.
	///      * Otherwise, the C# type of the resulting expression shall match the IL stack type,
	///        and the evaluated values shall be the same.
	/// </remarks>
	sealed class ExpressionBuilder : ILVisitor<TranslationContext, TranslatedExpression>
	{
		readonly StatementBuilder statementBuilder;
		readonly IDecompilerTypeSystem typeSystem;
		internal readonly ITypeResolveContext decompilationContext;
		internal readonly ILFunction currentFunction;
		internal readonly ICompilation compilation;
		internal readonly CSharpResolver resolver;
		internal readonly TypeSystemAstBuilder astBuilder;
		internal readonly TypeInference typeInference;
		internal readonly DecompilerSettings settings;
		readonly CancellationToken cancellationToken;
		
		public ExpressionBuilder(StatementBuilder statementBuilder, IDecompilerTypeSystem typeSystem, ITypeResolveContext decompilationContext, ILFunction currentFunction, DecompilerSettings settings, CancellationToken cancellationToken)
		{
			Debug.Assert(decompilationContext != null);
			this.statementBuilder = statementBuilder;
			this.typeSystem = typeSystem;
			this.decompilationContext = decompilationContext;
			this.currentFunction = currentFunction;
			this.settings = settings;
			this.cancellationToken = cancellationToken;
			this.compilation = decompilationContext.Compilation;
			this.resolver = new CSharpResolver(new CSharpTypeResolveContext(compilation.MainModule, null, decompilationContext.CurrentTypeDefinition, decompilationContext.CurrentMember));
			this.astBuilder = new TypeSystemAstBuilder(resolver);
			this.astBuilder.AlwaysUseShortTypeNames = true;
			this.astBuilder.AddResolveResultAnnotations = true;
			this.astBuilder.ShowAttributes = true;
			this.astBuilder.UseNullableSpecifierForValueTypes = settings.LiftNullables;
			this.typeInference = new TypeInference(compilation) { Algorithm = TypeInferenceAlgorithm.Improved };
		}

		public AstType ConvertType(IType type)
		{
			var astType = astBuilder.ConvertType(type);
			Debug.Assert(astType.Annotation<TypeResolveResult>() != null);
			return astType;
		}
		
		public ExpressionWithResolveResult ConvertConstantValue(ResolveResult rr, bool allowImplicitConversion = false)
		{
			var expr = astBuilder.ConvertConstantValue(rr);
			if (!allowImplicitConversion) {
				if (expr is NullReferenceExpression && rr.Type.Kind != TypeKind.Null) {
					expr = new CastExpression(ConvertType(rr.Type), expr);
				} else if (rr.Type.IsCSharpSmallIntegerType()) {
					expr = new CastExpression(new PrimitiveType(KnownTypeReference.GetCSharpNameByTypeCode(rr.Type.GetDefinition().KnownTypeCode)), expr);
				} else if (rr.Type.IsCSharpNativeIntegerType()) {
					expr = new CastExpression(new PrimitiveType(rr.Type.Name), expr);
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
			if (inst.ResultType != StackType.Void && cexpr.Type.Kind != TypeKind.Unknown && inst.ResultType != StackType.Unknown && cexpr.Type.Kind != TypeKind.None) {
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
				} else if (inst.ResultType == StackType.Ref) {
					Debug.Assert(cexpr.Type.GetStackType() == StackType.Ref || cexpr.Type.GetStackType().IsIntegerType());
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
				return expr.WithRR(new ByReferenceResolveResult(elementType, ReferenceKind.Ref));
			} else {
				return expr.WithRR(new ILVariableResolveResult(variable, variable.Type));
			}
		}

		internal bool HidesVariableWithName(string name)
		{
			return HidesVariableWithName(currentFunction, name);
		}

		internal static bool HidesVariableWithName(ILFunction currentFunction, string name)
		{
			return currentFunction.Ancestors.OfType<ILFunction>().Any(HidesVariableOrNestedFunction);

			bool HidesVariableOrNestedFunction(ILFunction function)
			{
				foreach (var v in function.Variables) {
					if (v.Name == name)
						return true;
				}

				foreach (var f in function.LocalFunctions) {
					if (f.Name == name)
						return true;
				}

				return false;
			}
		}

		internal ILFunction ResolveLocalFunction(IMethod method)
		{
			Debug.Assert(method.IsLocalFunction);
			method = (IMethod)((IMethod)method.MemberDefinition).ReducedFrom.MemberDefinition;
			foreach (var parent in currentFunction.Ancestors.OfType<ILFunction>()) {
				var definition = parent.LocalFunctions.FirstOrDefault(f => f.Method.MemberDefinition.Equals(method));
				if (definition != null) {
					return definition;
				}
			}
			return null;
		}

		bool RequiresQualifier(IMember member, TranslatedExpression target)
		{
			if (settings.AlwaysQualifyMemberReferences || HidesVariableWithName(member.Name))
				return true;
			if (member.IsStatic)
				return !IsCurrentOrContainingType(member.DeclaringTypeDefinition);
			return !(target.Expression is ThisReferenceExpression || target.Expression is BaseReferenceExpression);
		}

		ExpressionWithResolveResult ConvertField(IField field, ILInstruction targetInstruction = null)
		{
			var target = TranslateTarget(targetInstruction,
				nonVirtualInvocation: true,
				memberStatic: field.IsStatic,
				memberDeclaringType: field.DeclaringType);
			bool requireTarget;
			// If this is a reference to the backing field of an automatic property and we're going to transform automatic properties
			// in PatternStatementTransform, then we have to do the "requires qualifier"-check based on the property instead of the field.
			// It is easier to solve this special case here than in PatternStatementTransform, because here we perform all resolver checks.
			// It feels a bit hacky, though.
			if (settings.AutomaticProperties
				&& PatternStatementTransform.IsBackingFieldOfAutomaticProperty(field, out var property)
				&& decompilationContext.CurrentMember != property)
			{
				requireTarget = RequiresQualifier(property, target);
			} else {
				requireTarget = RequiresQualifier(field, target);
			}
			bool targetCasted = false;
			var targetResolveResult = requireTarget ? target.ResolveResult : null;

			bool IsAmbiguousAccess(out MemberResolveResult result)
			{
				if (targetResolveResult == null) {
					result = resolver.ResolveSimpleName(field.Name, EmptyList<IType>.Instance, isInvocationTarget: false) as MemberResolveResult;
				} else {
					var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentModule);
					result = lookup.Lookup(target.ResolveResult, field.Name, EmptyList<IType>.Instance, isInvocation: false) as MemberResolveResult;
				}
				return result == null || result.IsError || !result.Member.Equals(field, NormalizeTypeVisitor.TypeErasure);
			}

			MemberResolveResult mrr;
			while (IsAmbiguousAccess(out mrr)) {
				if (!requireTarget) {
					requireTarget = true;
					targetResolveResult = target.ResolveResult;
				} else if (!targetCasted) {
					targetCasted = true;
					target = target.ConvertTo(field.DeclaringType, this);
					targetResolveResult = target.ResolveResult;
				} else {
					break;
				}
			}

			if (mrr == null) {
				mrr = new MemberResolveResult(target.ResolveResult, field);
			}

			if (requireTarget) {
				return new MemberReferenceExpression(target, field.Name)
					.WithRR(mrr);
			} else {
				return new IdentifierExpression(field.Name)
					.WithRR(mrr);
			}
		}
		
		TranslatedExpression IsType(IsInst inst)
		{
			var arg = Translate(inst.Argument);
			arg = UnwrapBoxingConversion(arg);
			return new IsExpression(arg.Expression, ConvertType(inst.Type))
				.WithILInstruction(inst)
				.WithRR(new TypeIsResolveResult(arg.ResolveResult, inst.Type, compilation.FindType(TypeCode.Boolean)));
		}
		
		protected internal override TranslatedExpression VisitIsInst(IsInst inst, TranslationContext context)
		{
			var arg = Translate(inst.Argument);
			if (inst.Type.IsReferenceType == false) {
				// isinst with a value type results in an expression of "boxed value type",
				// which is not supported in C#.
				// Note that several other instructions special-case isinst arguments:
				//  unbox.any T(isinst T(expr)) ==> "expr as T" for nullable value types and class-constrained generic types
				//  comp(isinst T(expr) != null) ==> "expr is T"
				//  on block level (StatementBuilder.VisitIsInst) => "expr is T"
				if (SemanticHelper.IsPure(inst.Argument.Flags)) {
					// We can emulate isinst using
					//   expr is T ? expr : null
					return new ConditionalExpression(
						new IsExpression(arg, ConvertType(inst.Type)).WithILInstruction(inst),
						arg.Expression.Clone(),
						new NullReferenceExpression()
					).WithoutILInstruction().WithRR(new ResolveResult(arg.Type));
				} else {
					return ErrorExpression("isinst with value type is only supported in some contexts");
				}
			}
			arg = UnwrapBoxingConversion(arg);
			return new AsExpression(arg.Expression, ConvertType(inst.Type))
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(inst.Type, arg.ResolveResult, Conversion.TryCast));
		}

		internal static TranslatedExpression UnwrapBoxingConversion(TranslatedExpression arg)
		{
			if (arg.Expression is CastExpression cast
					&& arg.Type.IsKnownType(KnownTypeCode.Object)
					&& arg.ResolveResult is ConversionResolveResult crr
					&& crr.Conversion.IsBoxingConversion) {
				// When 'is' or 'as' is used with a value type or type parameter,
				// the C# compiler implicitly boxes the input.
				arg = arg.UnwrapChild(cast.Expression);
			}

			return arg;
		}

		protected internal override TranslatedExpression VisitNewObj(NewObj inst, TranslationContext context)
		{
			var type = inst.Method.DeclaringType;
			if (type.IsKnownType(KnownTypeCode.SpanOfT) || type.IsKnownType(KnownTypeCode.ReadOnlySpanOfT)) {
				if (inst.Arguments.Count == 2 && inst.Arguments[0] is Block b && b.Kind == BlockKind.StackAllocInitializer) {
					return TranslateStackAllocInitializer(b, type.TypeArguments[0]);
				}
			}
			return new CallBuilder(this, typeSystem, settings).Build(inst);
		}

		protected internal override TranslatedExpression VisitLdVirtDelegate(LdVirtDelegate inst, TranslationContext context)
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
				.WithRR(new ArrayCreateResolveResult(new ArrayType(compilation, inst.Type, dimensions), args.Select(a => a.ResolveResult).ToList(), Empty<ResolveResult>.Array));
		}

		protected internal override TranslatedExpression VisitLocAlloc(LocAlloc inst, TranslationContext context)
		{
			return TranslateLocAlloc(inst, context.TypeHint, out var elementType)
				.WithILInstruction(inst).WithRR(new ResolveResult(new PointerType(elementType)));
		}

		protected internal override TranslatedExpression VisitLocAllocSpan(LocAllocSpan inst, TranslationContext context)
		{
			return TranslateLocAllocSpan(inst, context.TypeHint, out _)
				.WithILInstruction(inst).WithRR(new ResolveResult(inst.Type));
		}

		StackAllocExpression TranslateLocAllocSpan(LocAllocSpan inst, IType typeHint, out IType elementType)
		{
			elementType = inst.Type.TypeArguments[0];
			TranslatedExpression countExpression = Translate(inst.Argument)
				.ConvertTo(compilation.FindType(KnownTypeCode.Int32), this);
			return new StackAllocExpression {
				Type = ConvertType(elementType),
				CountExpression = countExpression
			};
		}

		StackAllocExpression TranslateLocAlloc(LocAlloc inst, IType typeHint, out IType elementType)
		{
			TranslatedExpression countExpression;
			PointerType pointerType;
			if (inst.Argument.MatchBinaryNumericInstruction(BinaryNumericOperator.Mul, out var left, out var right)
				&& right.UnwrapConv(ConversionKind.SignExtend).UnwrapConv(ConversionKind.ZeroExtend).MatchSizeOf(out elementType))
			{
				// Determine the element type from the sizeof
				countExpression = Translate(left.UnwrapConv(ConversionKind.ZeroExtend));
				pointerType = new PointerType(elementType);
			} else {
				// Determine the element type from the expected pointer type in this context
				pointerType = typeHint as PointerType;
				if (pointerType != null && GetPointerArithmeticOffset(
						inst.Argument, Translate(inst.Argument),
						pointerType.ElementType, checkForOverflow: true,
						unwrapZeroExtension: true
					) is TranslatedExpression offset)
				{
					countExpression = offset;
					elementType = pointerType.ElementType;
				} else {
					elementType = compilation.FindType(KnownTypeCode.Byte);
					pointerType = new PointerType(elementType);
					countExpression = Translate(inst.Argument);
				}
			}
			countExpression = countExpression.ConvertTo(compilation.FindType(KnownTypeCode.Int32), this);
			return new StackAllocExpression {
				Type = ConvertType(elementType),
				CountExpression = countExpression
			};
		}
		
		protected internal override TranslatedExpression VisitLdcI4(LdcI4 inst, TranslationContext context)
		{
			ResolveResult rr;
			if (context.TypeHint.GetSign() == Sign.Unsigned) {
				rr = new ConstantResolveResult(
					compilation.FindType(KnownTypeCode.UInt32),
					unchecked((uint)inst.Value)
				);
			} else {
				rr = new ConstantResolveResult(
					compilation.FindType(KnownTypeCode.Int32),
					inst.Value
				);
			}
			rr = AdjustConstantToType(rr, context.TypeHint);
			astBuilder.PrintIntegralValuesAsHex = ShouldDisplayAsHex(inst.Value, inst.Parent);
			try {
				return ConvertConstantValue(rr, allowImplicitConversion: true)
					.WithILInstruction(inst);
			} finally {
				astBuilder.PrintIntegralValuesAsHex = false;
			}
		}

		protected internal override TranslatedExpression VisitLdcI8(LdcI8 inst, TranslationContext context)
		{
			ResolveResult rr;
			if (context.TypeHint.GetSign() == Sign.Unsigned) {
				rr = new ConstantResolveResult(
					compilation.FindType(KnownTypeCode.UInt64),
					unchecked((ulong)inst.Value)
				);
			} else {
				rr = new ConstantResolveResult(
					compilation.FindType(KnownTypeCode.Int64),
					inst.Value
				);
			}
			rr = AdjustConstantToType(rr, context.TypeHint);
			astBuilder.PrintIntegralValuesAsHex = ShouldDisplayAsHex(inst.Value, inst.Parent);
			try {
				return ConvertConstantValue(rr, allowImplicitConversion: true)
					.WithILInstruction(inst);
			} finally {
				astBuilder.PrintIntegralValuesAsHex = false;
			}
		}

		private bool ShouldDisplayAsHex(long value, ILInstruction parent)
		{
			if (parent is Conv conv)
				parent = conv.Parent;
			if (value <= 9)
				return false;
			switch (parent) {
				case BinaryNumericInstruction bni:
					if (bni.Operator == BinaryNumericOperator.BitAnd
						|| bni.Operator == BinaryNumericOperator.BitOr
						|| bni.Operator == BinaryNumericOperator.BitXor)
						return true;
					break;
			}
			return false;
		}

		protected internal override TranslatedExpression VisitLdcF4(LdcF4 inst, TranslationContext context)
		{
			var expr = astBuilder.ConvertConstantValue(compilation.FindType(KnownTypeCode.Single), inst.Value);
			return new TranslatedExpression(expr.WithILInstruction(inst));
		}

		protected internal override TranslatedExpression VisitLdcF8(LdcF8 inst, TranslationContext context)
		{
			var expr = astBuilder.ConvertConstantValue(compilation.FindType(KnownTypeCode.Double), inst.Value);
			return new TranslatedExpression(expr.WithILInstruction(inst));
		}

		protected internal override TranslatedExpression VisitLdcDecimal(LdcDecimal inst, TranslationContext context)
		{
			var expr = astBuilder.ConvertConstantValue(compilation.FindType(KnownTypeCode.Decimal), inst.Value);
			return new TranslatedExpression(expr.WithILInstruction(inst));
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
			Expression expr;
			IType constantType;
			object constantValue;
			if (type.IsReferenceType == true || type.IsKnownType(KnownTypeCode.NullableOfT)) {
				expr = new NullReferenceExpression();
				constantType = SpecialType.NullType;
				constantValue = null;
			} else {
				expr = new DefaultValueExpression(ConvertType(type));
				constantType = type;
				constantValue = CSharpResolver.GetDefaultValue(type);
			}
			return expr.WithRR(new ConstantResolveResult(constantType, constantValue));
		}
		
		protected internal override TranslatedExpression VisitSizeOf(SizeOf inst, TranslationContext context)
		{
			return new SizeOfExpression(ConvertType(inst.Type))
				.WithILInstruction(inst)
				.WithRR(new SizeOfResolveResult(compilation.FindType(KnownTypeCode.Int32), inst.Type, null));
		}
		
		protected internal override TranslatedExpression VisitLdTypeToken(LdTypeToken inst, TranslationContext context)
		{
			var typeofExpr = new TypeOfExpression(ConvertType(inst.Type))
				.WithRR(new TypeOfResolveResult(compilation.FindType(KnownTypeCode.Type), inst.Type));
			return new MemberReferenceExpression(typeofExpr, "TypeHandle")
				.WithILInstruction(inst)
				.WithRR(new TypeOfResolveResult(compilation.FindType(new TopLevelTypeName("System", "RuntimeTypeHandle")), inst.Type));
		}
		
		protected internal override TranslatedExpression VisitBitNot(BitNot inst, TranslationContext context)
		{
			var argument = Translate(inst.Argument);
			var argUType = NullableType.GetUnderlyingType(argument.Type);

			if (argUType.GetStackType().GetSize() < inst.UnderlyingResultType.GetSize()
				|| argUType.Kind == TypeKind.Enum && argUType.IsSmallIntegerType()
				|| (argUType.GetStackType() == StackType.I && !argUType.IsCSharpNativeIntegerType())
				|| argUType.IsKnownType(KnownTypeCode.Boolean)
				|| argUType.IsKnownType(KnownTypeCode.Char))
			{
				// Argument is undersized (even after implicit integral promotion to I4)
				// -> we need to perform sign/zero-extension before the BitNot.
				// Same if the argument is an enum based on a small integer type
				// (those don't undergo numeric promotion in C# the way non-enum small integer types do).
				// Same if the type is one that does not support ~ (IntPtr, bool and char).
				Sign sign = context.TypeHint.GetSign();
				if (sign == Sign.None) {
					sign = argUType.GetSign();
				}
				IType targetType = FindArithmeticType(inst.UnderlyingResultType, sign);
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
			// "!expr" implicitly converts to bool so we can remove the cast;
			// but only if doing so wouldn't cause us to call a user-defined "operator !"
			expr = expr.UnwrapImplicitBoolConversion(type => !type.GetMethods(m => m.IsOperator && m.Name == "op_LogicalNot").Any());
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
				.WithRR(new ByReferenceResolveResult(expr.ResolveResult, ReferenceKind.Ref));
		}
		
		protected internal override TranslatedExpression VisitStLoc(StLoc inst, TranslationContext context)
		{
			var translatedValue = Translate(inst.Value, typeHint: inst.Variable.Type);
			if (inst.Variable.Kind == VariableKind.StackSlot && !loadedVariablesSet.Contains(inst.Variable)) {
				// Stack slots in the ILAst have inaccurate types (e.g. System.Object for StackType.O)
				// so we should replace them with more accurate types where possible:
				if (CanUseTypeForStackSlot(inst.Variable, translatedValue.Type)
						&& inst.Variable.StackType == translatedValue.Type.GetStackType()
						&& translatedValue.Type.Kind != TypeKind.Null) {
					inst.Variable.Type = translatedValue.Type;
				} else if (inst.Value.MatchDefaultValue(out var type) && IsOtherValueType(type)) {
					inst.Variable.Type = type;
				}
			}
			var lhs = ConvertVariable(inst.Variable).WithoutILInstruction();
			if (lhs.Expression is DirectionExpression dirExpr && lhs.ResolveResult is ByReferenceResolveResult lhsRefRR) {
				// ref (re-)assignment, emit "ref (a = ref b)".
				lhs = lhs.UnwrapChild(dirExpr.Expression);
				translatedValue = translatedValue.ConvertTo(lhsRefRR.Type, this, allowImplicitConversion: true);
				var assign = new AssignmentExpression(lhs.Expression, translatedValue.Expression)
					.WithRR(new OperatorResolveResult(lhs.Type, ExpressionType.Assign, lhsRefRR, translatedValue.ResolveResult));
				return new DirectionExpression(FieldDirection.Ref, assign)
					.WithoutILInstruction().WithRR(lhsRefRR);
			} else {
				return Assignment(lhs, translatedValue).WithILInstruction(inst);
			}

			bool CanUseTypeForStackSlot(ILVariable v, IType type)
			{
				return v.IsSingleDefinition
					|| IsOtherValueType(type)
					|| v.StackType == StackType.Ref
					|| AllStoresUseConsistentType(v.StoreInstructions, type);
			}

			bool IsOtherValueType(IType type)
			{
				return type.IsReferenceType == false && type.GetStackType() == StackType.O;
			}

			bool AllStoresUseConsistentType(IReadOnlyList<IStoreInstruction> storeInstructions, IType expectedType)
			{
				expectedType = expectedType.AcceptVisitor(NormalizeTypeVisitor.TypeErasure);
				foreach (var store in storeInstructions) {
					if (!(store is StLoc stloc))
						return false;
					IType type = stloc.Value.InferType(compilation).AcceptVisitor(NormalizeTypeVisitor.TypeErasure);
					if (!type.Equals(expectedType))
						return false;
				}
				return true;
			}
		}
		
		protected internal override TranslatedExpression VisitComp(Comp inst, TranslationContext context)
		{
			if (inst.LiftingKind == ComparisonLiftingKind.ThreeValuedLogic) {
				if (inst.Kind == ComparisonKind.Equality && inst.Right.MatchLdcI4(0)) {
					// lifted logic.not
					var targetType = NullableType.Create(compilation, compilation.FindType(KnownTypeCode.Boolean));
					var arg = Translate(inst.Left, targetType).ConvertTo(targetType, this);
					return new UnaryOperatorExpression(UnaryOperatorType.Not, arg.Expression)
						.WithRR(new OperatorResolveResult(targetType, ExpressionType.Not, arg.ResolveResult))
						.WithILInstruction(inst);
				}
				return ErrorExpression("Nullable comparisons with three-valued-logic not supported in C#");
			}
			if (inst.Kind.IsEqualityOrInequality()) {
				var result = TranslateCeq(inst, out bool negateOutput);
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
			// Handle comparisons between unsafe pointers and null:
			if (left.Type.Kind == TypeKind.Pointer && inst.Right.MatchLdcI(0)) {
				negateOutput = false;
				right = new NullReferenceExpression().WithRR(new ConstantResolveResult(SpecialType.NullType, null))
					.WithILInstruction(inst.Right);
				return CreateBuiltinBinaryOperator(left, inst.Kind.ToBinaryOperatorType(), right)
					.WithILInstruction(inst);
			} else if (right.Type.Kind == TypeKind.Pointer && inst.Left.MatchLdcI(0)) {
				negateOutput = false;
				left = new NullReferenceExpression().WithRR(new ConstantResolveResult(SpecialType.NullType, null))
					.WithILInstruction(inst.Left);
				return CreateBuiltinBinaryOperator(left, inst.Kind.ToBinaryOperatorType(), right)
					.WithILInstruction(inst);
			}

			// Special case comparisons with enum and char literals
			left = TryUniteEqualityOperandType(left, right);
			right = TryUniteEqualityOperandType(right, left);
			
			if (IsSpecialCasedReferenceComparisonWithNull(left, right)) {
				// When comparing a string/delegate with null, the C# compiler generates a reference comparison.
				negateOutput = false;
				return CreateBuiltinBinaryOperator(left, inst.Kind.ToBinaryOperatorType(), right)
					.WithILInstruction(inst);
			}

			OperatorResolveResult rr;
			if (left.Type.IsKnownType(KnownTypeCode.String) && right.Type.IsKnownType(KnownTypeCode.String)) {
				rr = null; // it's a string comparison by-value, which is not a reference comparison
			} else {
				rr = resolver.ResolveBinaryOperator(inst.Kind.ToBinaryOperatorType(), left.ResolveResult, right.ResolveResult)
					as OperatorResolveResult;
			}
			if (rr == null || rr.IsError || rr.UserDefinedOperatorMethod != null
			    || NullableType.GetUnderlyingType(rr.Operands[0].Type).GetStackType() != inst.InputType
			    || !rr.Type.IsKnownType(KnownTypeCode.Boolean))
			{
				IType targetType;
				if (inst.InputType == StackType.O) {
					targetType = compilation.FindType(KnownTypeCode.Object);
				} else {
					var leftUType = NullableType.GetUnderlyingType(left.Type);
					var rightUType = NullableType.GetUnderlyingType(right.Type);
					if (leftUType.GetStackType() == inst.InputType && !leftUType.IsSmallIntegerType()) {
						targetType = leftUType;
					} else if (rightUType.GetStackType() == inst.InputType && !rightUType.IsSmallIntegerType()) {
						targetType = rightUType;
					} else {
						targetType = FindType(inst.InputType, leftUType.GetSign());
					}
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
					|| NullableType.GetUnderlyingType(rr.Operands[0].Type).GetStackType() != inst.InputType
					|| !rr.Type.IsKnownType(KnownTypeCode.Boolean))
				{
					// If converting one input wasn't sufficient, convert both:
					left = left.ConvertTo(targetType, this);
					right = right.ConvertTo(targetType, this);
					rr = new OperatorResolveResult(
						compilation.FindType(KnownTypeCode.Boolean),
						BinaryOperatorExpression.GetLinqNodeType(inst.Kind.ToBinaryOperatorType(), false),
						left.ResolveResult, right.ResolveResult);
				}
			}
			negateOutput = false;
			return new BinaryOperatorExpression(left.Expression, inst.Kind.ToBinaryOperatorType(), right.Expression)
				.WithILInstruction(inst)
				.WithRR(rr);
		}

		TranslatedExpression TryUniteEqualityOperandType(TranslatedExpression left, TranslatedExpression right)
		{
			// Special case for enum flag check "(enum & EnumType.SomeValue) == 0"
			// so that the const 0 value is printed as 0 integer and not as enum type, e.g. EnumType.None
			if (left.ResolveResult.IsCompileTimeConstant &&
				left.ResolveResult.Type.IsCSharpPrimitiveIntegerType() &&
				(left.ResolveResult.ConstantValue as int?) == 0 &&
				NullableType.GetUnderlyingType(right.Type).Kind == TypeKind.Enum &&
				right.Expression is BinaryOperatorExpression binaryExpr &&
				binaryExpr.Operator == BinaryOperatorType.BitwiseAnd)
			{
				return AdjustConstantExpressionToType(left, compilation.FindType(KnownTypeCode.Int32));
			} else
				return AdjustConstantExpressionToType(left, right.Type);
		}

		bool IsSpecialCasedReferenceComparisonWithNull(TranslatedExpression lhs, TranslatedExpression rhs)
		{
			if (lhs.Type.Kind == TypeKind.Null)
				ExtensionMethods.Swap(ref lhs, ref rhs);
			return rhs.Type.Kind == TypeKind.Null
				&& (lhs.Type.Kind == TypeKind.Delegate || lhs.Type.IsKnownType(KnownTypeCode.String));
		}

		ExpressionWithResolveResult CreateBuiltinBinaryOperator(
			TranslatedExpression left, BinaryOperatorType type, TranslatedExpression right,
			bool checkForOverflow = false)
		{
			return new BinaryOperatorExpression(left.Expression, type, right.Expression)
			.WithRR(new OperatorResolveResult(
				compilation.FindType(KnownTypeCode.Boolean),
				BinaryOperatorExpression.GetLinqNodeType(type, checkForOverflow),
				left.ResolveResult, right.ResolveResult));
		}
		
		/// <summary>
		/// Handle Comp instruction, operators other than equality/inequality.
		/// </summary>
		TranslatedExpression TranslateComp(Comp inst)
		{
			var op = inst.Kind.ToBinaryOperatorType();
			var left = Translate(inst.Left);
			var right = Translate(inst.Right);

			if (left.Type.Kind == TypeKind.Pointer && right.Type.Kind == TypeKind.Pointer) {
				return CreateBuiltinBinaryOperator(left, op, right)
					.WithILInstruction(inst);
			}

			left = PrepareArithmeticArgument(left, inst.InputType, inst.Sign, inst.IsLifted);
			right = PrepareArithmeticArgument(right, inst.InputType, inst.Sign, inst.IsLifted);

			// Special case comparisons with enum and char literals
			left = AdjustConstantExpressionToType(left, right.Type);
			right = AdjustConstantExpressionToType(right, left.Type);

			// attempt comparison without any additional casts
			var rr = resolver.ResolveBinaryOperator(inst.Kind.ToBinaryOperatorType(), left.ResolveResult, right.ResolveResult)
				as OperatorResolveResult;
			if (rr != null && !rr.IsError) {
				IType compUType = NullableType.GetUnderlyingType(rr.Operands[0].Type);
				if (compUType.GetSign() == inst.Sign && compUType.GetStackType() == inst.InputType) {
					return new BinaryOperatorExpression(left.Expression, op, right.Expression)
						.WithILInstruction(inst)
						.WithRR(rr);
				}
			}

			if (inst.InputType.IsIntegerType()) {
				// Ensure the inputs have the correct sign:
				IType inputType = FindArithmeticType(inst.InputType, inst.Sign);
				if (inst.IsLifted) {
					inputType = NullableType.Create(compilation, inputType);
				}
				left = left.ConvertTo(inputType, this);
				right = right.ConvertTo(inputType, this);
			}
			return new BinaryOperatorExpression(left.Expression, op, right.Expression)
				.WithILInstruction(inst)
				.WithRR(new OperatorResolveResult(compilation.FindType(TypeCode.Boolean),
				                                  BinaryOperatorExpression.GetLinqNodeType(op, false),
				                                  left.ResolveResult, right.ResolveResult));
		}

		protected internal override TranslatedExpression VisitThreeValuedBoolAnd(ThreeValuedBoolAnd inst, TranslationContext context)
		{
			return HandleThreeValuedLogic(inst, BinaryOperatorType.BitwiseAnd, ExpressionType.And);
		}

		protected internal override TranslatedExpression VisitThreeValuedBoolOr(ThreeValuedBoolOr inst, TranslationContext context)
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

		protected internal override TranslatedExpression VisitThrow(Throw inst, TranslationContext context)
		{
			return new ThrowExpression(Translate(inst.Argument))
				.WithILInstruction(inst)
				.WithRR(new ThrowResolveResult());
		}

		protected internal override TranslatedExpression VisitUserDefinedLogicOperator(UserDefinedLogicOperator inst, TranslationContext context)
		{
			var left = Translate(inst.Left, inst.Method.Parameters[0].Type).ConvertTo(inst.Method.Parameters[0].Type, this);
			var right = Translate(inst.Right, inst.Method.Parameters[1].Type).ConvertTo(inst.Method.Parameters[1].Type, this);
			BinaryOperatorType op;
			if (inst.Method.Name == "op_BitwiseAnd") {
				op = BinaryOperatorType.ConditionalAnd;
			} else if (inst.Method.Name == "op_BitwiseOr") {
				op = BinaryOperatorType.ConditionalOr;
			} else {
				throw new InvalidOperationException("Invalid method name");
			}
			return new BinaryOperatorExpression(left.Expression, op, right.Expression)
				.WithRR(new InvocationResolveResult(null, inst.Method, new ResolveResult[] { left.ResolveResult, right.ResolveResult }))
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
					return HandleBinaryNumeric(inst, BinaryOperatorType.Add, context);
				case BinaryNumericOperator.Sub:
					return HandleBinaryNumeric(inst, BinaryOperatorType.Subtract, context);
				case BinaryNumericOperator.Mul:
					return HandleBinaryNumeric(inst, BinaryOperatorType.Multiply, context);
				case BinaryNumericOperator.Div:
					return HandlePointerSubtraction(inst)
						?? HandleBinaryNumeric(inst, BinaryOperatorType.Divide, context);
				case BinaryNumericOperator.Rem:
					return HandleBinaryNumeric(inst, BinaryOperatorType.Modulus, context);
				case BinaryNumericOperator.BitAnd:
					return HandleBinaryNumeric(inst, BinaryOperatorType.BitwiseAnd, context);
				case BinaryNumericOperator.BitOr:
					return HandleBinaryNumeric(inst, BinaryOperatorType.BitwiseOr, context);
				case BinaryNumericOperator.BitXor:
					return HandleBinaryNumeric(inst, BinaryOperatorType.ExclusiveOr, context);
				case BinaryNumericOperator.ShiftLeft:
					return HandleShift(inst, BinaryOperatorType.ShiftLeft);
				case BinaryNumericOperator.ShiftRight:
					return HandleShift(inst, BinaryOperatorType.ShiftRight);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Translates pointer arithmetic:
		///   ptr + int
		///   int + ptr
		///   ptr - int
		/// Returns null if 'inst' is not performing pointer arithmetic.
		/// This function not handle 'ptr - ptr'!
		/// </summary>
		TranslatedExpression? HandlePointerArithmetic(BinaryNumericInstruction inst, TranslatedExpression left, TranslatedExpression right)
		{
			if (!(inst.Operator == BinaryNumericOperator.Add || inst.Operator == BinaryNumericOperator.Sub))
				return null;
			if (inst.CheckForOverflow || inst.IsLifted)
				return null;
			if (!(inst.LeftInputType == StackType.I && inst.RightInputType == StackType.I))
				return null;
			PointerType pointerType;
			ILInstruction byteOffsetInst;
			TranslatedExpression byteOffsetExpr;
			if (left.Type.Kind == TypeKind.Pointer) {
				byteOffsetInst = inst.Right;
				byteOffsetExpr = right;
				pointerType = (PointerType)left.Type;
			} else if (right.Type.Kind == TypeKind.Pointer) {
				if (inst.Operator != BinaryNumericOperator.Add)
					return null;
				byteOffsetInst = inst.Left;
				byteOffsetExpr = left;
				pointerType = (PointerType)right.Type;
			} else {
				return null;
			}
			TranslatedExpression offsetExpr = GetPointerArithmeticOffset(byteOffsetInst, byteOffsetExpr, pointerType.ElementType, inst.CheckForOverflow)
				?? FallBackToBytePointer();

			if (left.Type.Kind == TypeKind.Pointer) {
				Debug.Assert(inst.Operator == BinaryNumericOperator.Add || inst.Operator == BinaryNumericOperator.Sub);
				left = left.ConvertTo(pointerType, this);
				right = offsetExpr;
			} else {
				Debug.Assert(inst.Operator == BinaryNumericOperator.Add);
				Debug.Assert(right.Type.Kind == TypeKind.Pointer);
				left = offsetExpr;
				right = right.ConvertTo(pointerType, this);
			}
			var operatorType = inst.Operator == BinaryNumericOperator.Add ? BinaryOperatorType.Add : BinaryOperatorType.Subtract;
			return new BinaryOperatorExpression(left, operatorType, right)
				.WithILInstruction(inst)
				.WithRR(new OperatorResolveResult(
					pointerType, BinaryOperatorExpression.GetLinqNodeType(operatorType, inst.CheckForOverflow),
					left.ResolveResult, right.ResolveResult));

			TranslatedExpression FallBackToBytePointer()
			{
				pointerType = new PointerType(compilation.FindType(KnownTypeCode.Byte));
				return EnsureIntegerType(byteOffsetExpr);
			}
		}

		/// <summary>
		/// Translates pointer arithmetic with managed pointers:
		///   ref + int
		///   int + ref
		///   ref - int
		///   ref - ref
		/// </summary>
		TranslatedExpression? HandleManagedPointerArithmetic(BinaryNumericInstruction inst, TranslatedExpression left, TranslatedExpression right)
		{
			if (!(inst.Operator == BinaryNumericOperator.Add || inst.Operator == BinaryNumericOperator.Sub))
				return null;
			if (inst.CheckForOverflow || inst.IsLifted)
				return null;
			if (inst.Operator == BinaryNumericOperator.Sub && inst.LeftInputType == StackType.Ref && inst.RightInputType == StackType.Ref) {
				// ref - ref => i
				return CallUnsafeIntrinsic("ByteOffset", new[] { 
					// ByteOffset() expects the parameters the wrong way around, so order using named arguments
					new NamedArgumentExpression("target", left.Expression), 
					new NamedArgumentExpression("origin", right.Expression) 
				}, compilation.FindType(KnownTypeCode.IntPtr), inst);
			}
			if (inst.LeftInputType == StackType.Ref && inst.RightInputType.IsIntegerType()
				&& left.Type is ByReferenceType brt) {
				// ref [+-] int
				string name = (inst.Operator == BinaryNumericOperator.Sub ? "Subtract" : "Add");
				ILInstruction offsetInst = PointerArithmeticOffset.Detect(inst.Right, brt.ElementType, inst.CheckForOverflow);
				if (offsetInst != null) {
					if (settings.FixedBuffers && inst.Operator == BinaryNumericOperator.Add && inst.Left is LdFlda ldFlda
						&& ldFlda.Target is LdFlda nestedLdFlda && CSharpDecompiler.IsFixedField(nestedLdFlda.Field, out var elementType, out _)) {
						Expression fieldAccess = ConvertField(nestedLdFlda.Field, nestedLdFlda.Target);
						var mrr = (MemberResolveResult)fieldAccess.GetResolveResult();
						fieldAccess.RemoveAnnotations<ResolveResult>();
						var result = fieldAccess.WithRR(new MemberResolveResult(mrr.TargetResult, mrr.Member, new PointerType(elementType)))
							.WithILInstruction(inst);
						TranslatedExpression expr = new IndexerExpression(result.Expression, Translate(offsetInst).Expression)
							.WithILInstruction(inst)
							.WithRR(new ResolveResult(elementType));
						return new DirectionExpression(FieldDirection.Ref, expr)
							.WithoutILInstruction().WithRR(new ByReferenceResolveResult(expr.Type, ReferenceKind.Ref));
					}
					return CallUnsafeIntrinsic(name, new[] { left.Expression, Translate(offsetInst).Expression }, brt, inst);
				} else {
					return CallUnsafeIntrinsic(name + "ByteOffset", new[] { left.Expression, right.Expression }, brt, inst);
				}
			}
			brt = right.Type as ByReferenceType;
			if (inst.LeftInputType == StackType.I && inst.RightInputType == StackType.Ref && brt != null
				&& inst.Operator == BinaryNumericOperator.Add) {
				// int + ref
				ILInstruction offsetInst = PointerArithmeticOffset.Detect(inst.Left, brt.ElementType, inst.CheckForOverflow);
				if (offsetInst != null) {
					return CallUnsafeIntrinsic("Add", new[] {
						new NamedArgumentExpression("elementOffset", Translate(offsetInst)),
						new NamedArgumentExpression("source", right)
					}, brt, inst);
				} else {
					return CallUnsafeIntrinsic("AddByteOffset", new[] {
						new NamedArgumentExpression("byteOffset", left.Expression),
						new NamedArgumentExpression("source", right)
					}, brt, inst);
				}
			}
			return null;
		}

		internal TranslatedExpression CallUnsafeIntrinsic(string name, Expression[] arguments, IType returnType, ILInstruction inst = null, IEnumerable<IType> typeArguments = null)
		{
			var target = new MemberReferenceExpression {
				Target = new TypeReferenceExpression(astBuilder.ConvertType(compilation.FindType(KnownTypeCode.Unsafe))),
				MemberName = name
			};
			if (typeArguments != null) {
				target.TypeArguments.AddRange(typeArguments.Select(astBuilder.ConvertType));
			}
			var invocationExpr = new InvocationExpression(target, arguments);
			var invocation = inst != null ? invocationExpr.WithILInstruction(inst) : invocationExpr.WithoutILInstruction(); 
			if (returnType is ByReferenceType brt) {
				return WrapInRef(invocation.WithRR(new ResolveResult(brt.ElementType)), brt);
			} else {
				return invocation.WithRR(new ResolveResult(returnType));
			}
		}

		TranslatedExpression EnsureIntegerType(TranslatedExpression expr)
		{
			if (!expr.Type.IsCSharpPrimitiveIntegerType() && !expr.Type.IsCSharpNativeIntegerType()) {
				// pointer arithmetic accepts all primitive integer types, but no enums etc.
				expr = expr.ConvertTo(FindArithmeticType(expr.Type.GetStackType(), expr.Type.GetSign()), this); 
			}
			return expr;
		}

		TranslatedExpression? GetPointerArithmeticOffset(ILInstruction byteOffsetInst, TranslatedExpression byteOffsetExpr,
			IType pointerElementType, bool checkForOverflow, bool unwrapZeroExtension = false)
		{
			var countOffsetInst = PointerArithmeticOffset.Detect(byteOffsetInst, pointerElementType,
				checkForOverflow: checkForOverflow,
				unwrapZeroExtension: unwrapZeroExtension);
			if (countOffsetInst == null) {
				return null;
			}
			if (countOffsetInst == byteOffsetInst) {
				return EnsureIntegerType(byteOffsetExpr);
			} else {
				TranslatedExpression expr = Translate(countOffsetInst);
				// Keep original ILInstruction as annotation
				expr.Expression.RemoveAnnotations<ILInstruction>();
				return EnsureIntegerType(expr.WithILInstruction(byteOffsetInst));
			}
		}

		/// <summary>
		/// Called for divisions, detect and handles the code pattern:
		///   div(sub(a, b), sizeof(T))
		/// when a,b are of type T*.
		/// This is what the C# compiler generates for pointer subtraction.
		/// </summary>
		TranslatedExpression? HandlePointerSubtraction(BinaryNumericInstruction inst)
		{
			Debug.Assert(inst.Operator == BinaryNumericOperator.Div);
			if (inst.CheckForOverflow || inst.LeftInputType != StackType.I)
				return null;
			if (!(inst.Left is BinaryNumericInstruction sub && sub.Operator == BinaryNumericOperator.Sub))
				return null;
			if (sub.CheckForOverflow)
				return null;
			// First, attempt to parse the 'sizeof' on the RHS
			IType elementType;
			if (inst.Right.MatchLdcI(out long elementSize)) {
				elementType = null;
				// OK, might be pointer subtraction if the element size matches
			} else if (inst.Right.UnwrapConv(ConversionKind.SignExtend).MatchSizeOf(out elementType)) {
				// OK, might be pointer subtraction if the element type matches
			} else {
				return null;
			}
			var left = Translate(sub.Left);
			var right = Translate(sub.Right);
			IType pointerType;
			if (IsMatchingPointerType(left.Type)) {
				pointerType = left.Type;
			} else if (IsMatchingPointerType(right.Type)) {
				pointerType = right.Type;
			} else if (elementSize == 1 && left.Type.Kind == TypeKind.Pointer && right.Type.Kind == TypeKind.Pointer) {
				// two pointers (neither matching), we're dividing by 1 (debug builds only),
				// -> subtract two byte pointers
				pointerType = new PointerType(compilation.FindType(KnownTypeCode.Byte));
			} else {
				// neither is a matching pointer type
				// -> not a pointer subtraction after all
				return null;
			}
			// We got a pointer subtraction.
			left = left.ConvertTo(pointerType, this);
			right = right.ConvertTo(pointerType, this);
			var rr = new OperatorResolveResult(
				compilation.FindType(KnownTypeCode.Int64),
				ExpressionType.Subtract,
				left.ResolveResult, right.ResolveResult
			);
			var result = new BinaryOperatorExpression(
				left.Expression, BinaryOperatorType.Subtract, right.Expression
			).WithILInstruction(new[] { inst, sub })
			 .WithRR(rr);
			return result;

			bool IsMatchingPointerType(IType type)
			{
				if (type is PointerType pt) {
					if (elementType != null)
						return elementType.Equals(pt.ElementType);
					else if (elementSize > 0)
						return PointerArithmeticOffset.ComputeSizeOf(pt.ElementType) == elementSize;
				}
				return false;
			}
		}
		
		TranslatedExpression HandleBinaryNumeric(BinaryNumericInstruction inst, BinaryOperatorType op, TranslationContext context)
		{
			var resolverWithOverflowCheck = resolver.WithCheckForOverflow(inst.CheckForOverflow);
			var left = Translate(inst.Left);
			var right = Translate(inst.Right);

			if (left.Type.Kind == TypeKind.ByReference || right.Type.Kind == TypeKind.ByReference) {
				var ptrResult = HandleManagedPointerArithmetic(inst, left, right);
				if (ptrResult != null)
					return ptrResult.Value;
			}
			if (left.Type.Kind == TypeKind.Pointer || right.Type.Kind == TypeKind.Pointer) {
				var ptrResult = HandlePointerArithmetic(inst, left, right);
				if (ptrResult != null)
					return ptrResult.Value;
			}

			left = PrepareArithmeticArgument(left, inst.LeftInputType, inst.Sign, inst.IsLifted);
			right = PrepareArithmeticArgument(right, inst.RightInputType, inst.Sign, inst.IsLifted);

			if (op == BinaryOperatorType.Subtract && inst.Left.MatchLdcI(0)) {
				IType rightUType = NullableType.GetUnderlyingType(right.Type);
				if (rightUType.IsKnownType(KnownTypeCode.Int32) || rightUType.IsKnownType(KnownTypeCode.Int64) 
					|| rightUType.IsCSharpSmallIntegerType() || rightUType.IsCSharpNativeIntegerType()) {
					// unary minus is supported on signed int and long, and on the small integer types (since they promote to int)
					var uoe = new UnaryOperatorExpression(UnaryOperatorType.Minus, right.Expression);
					uoe.AddAnnotation(inst.CheckForOverflow ? AddCheckedBlocks.CheckedAnnotation : AddCheckedBlocks.UncheckedAnnotation);
					var resultType = FindArithmeticType(inst.RightInputType, Sign.Signed);
					if (inst.IsLifted)
						resultType = NullableType.Create(compilation, resultType);
					return uoe.WithILInstruction(inst).WithRR(new OperatorResolveResult(
						resultType,
						inst.CheckForOverflow ? ExpressionType.NegateChecked : ExpressionType.Negate,
						right.ResolveResult));
				}
			}
			if (op.IsBitwise() 
				&& left.Type.IsKnownType(KnownTypeCode.Boolean) 
				&& right.Type.IsKnownType(KnownTypeCode.Boolean)
				&& SemanticHelper.IsPure(inst.Right.Flags))
			{
				// Undo the C# compiler's optimization of "a && b" to "a & b".
				if (op == BinaryOperatorType.BitwiseAnd) {
					op = BinaryOperatorType.ConditionalAnd;
				} else if (op == BinaryOperatorType.BitwiseOr) {
					op = BinaryOperatorType.ConditionalOr;
				}
			}

			if (op.IsBitwise() && (left.Type.Kind == TypeKind.Enum || right.Type.Kind == TypeKind.Enum)) {
				left = AdjustConstantExpressionToType(left, right.Type);
				right = AdjustConstantExpressionToType(right, left.Type);
			}

			var rr = resolverWithOverflowCheck.ResolveBinaryOperator(op, left.ResolveResult, right.ResolveResult);
			if (rr.IsError || NullableType.GetUnderlyingType(rr.Type).GetStackType() != inst.UnderlyingResultType
			    || !IsCompatibleWithSign(left.Type, inst.Sign) || !IsCompatibleWithSign(right.Type, inst.Sign))
			{
				// Left and right operands are incompatible, so convert them to a common type
				Sign sign = inst.Sign;
				if (sign == Sign.None) {
					// If the sign doesn't matter, try to use the same sign as expected by the context
					sign = context.TypeHint.GetSign();
				}
				IType targetType = FindArithmeticType(inst.UnderlyingResultType, sign);
				left = left.ConvertTo(NullableType.IsNullable(left.Type) ? NullableType.Create(compilation, targetType) : targetType, this);
				right = right.ConvertTo(NullableType.IsNullable(right.Type) ? NullableType.Create(compilation, targetType) : targetType, this);
				rr = resolverWithOverflowCheck.ResolveBinaryOperator(op, left.ResolveResult, right.ResolveResult);
			}
			var resultExpr = new BinaryOperatorExpression(left.Expression, op, right.Expression)
				.WithILInstruction(inst)
				.WithRR(rr);
			if (BinaryOperatorMightCheckForOverflow(op) && !inst.UnderlyingResultType.IsFloatType()) {
				resultExpr.Expression.AddAnnotation(inst.CheckForOverflow ? AddCheckedBlocks.CheckedAnnotation : AddCheckedBlocks.UncheckedAnnotation);
			}
			return resultExpr;
		}

		/// <summary>
		/// Gets a type matching the stack type and sign.
		/// </summary>
		IType FindType(StackType stackType, Sign sign)
		{
			if (stackType == StackType.I && settings.NativeIntegers) {
				return sign == Sign.Unsigned ? SpecialType.NUInt : SpecialType.NInt;
			} else {
				return compilation.FindType(stackType.ToKnownTypeCode(sign));
			}
		}

		/// <summary>
		/// Gets a type used for performing arithmetic with the stack type and sign.
		/// 
		/// This may result in a larger type than requested when the selected C# version
		/// doesn't support native integers.
		/// Should only be used after a call to PrepareArithmeticArgument()
		/// to ensure that we're not preserving extra bits from an oversized TranslatedExpression.
		/// </summary>
		IType FindArithmeticType(StackType stackType, Sign sign)
		{
			if (stackType == StackType.I) {
				if (settings.NativeIntegers) {
					return sign == Sign.Unsigned ? SpecialType.NUInt : SpecialType.NInt;
				} else {
					// If native integers are not available, use 64-bit arithmetic instead
					stackType = StackType.I8;
				}
			} 
			return compilation.FindType(stackType.ToKnownTypeCode(sign));
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
				IType targetType = FindType(argStackType, sign);
				argUType = targetType;
				if (isLifted)
					targetType = NullableType.Create(compilation, targetType);
				arg = arg.ConvertTo(targetType, this);
			}
			if (argUType.IsKnownType(KnownTypeCode.IntPtr) || argUType.IsKnownType(KnownTypeCode.UIntPtr)) {
				// None of the operators we might want to apply are supported by IntPtr/UIntPtr.
				// Also, pointer arithmetic has different semantics (works in number of elements, not bytes).
				// So any inputs of size StackType.I must be converted to long/ulong.
				IType targetType = FindArithmeticType(StackType.I, sign);
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

			left = PrepareArithmeticArgument(left, inst.LeftInputType, inst.Sign, inst.IsLifted);

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
				IType targetType = FindArithmeticType(inst.UnderlyingResultType, sign);
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

			return new BinaryOperatorExpression(left.Expression, op, right.Expression)
				.WithILInstruction(inst)
				.WithRR(resolver.ResolveBinaryOperator(op, left.ResolveResult, right.ResolveResult));
		}

		protected internal override TranslatedExpression VisitUserDefinedCompoundAssign(UserDefinedCompoundAssign inst, TranslationContext context)
		{
			IType loadType = inst.Method.Parameters[0].Type;
			ExpressionWithResolveResult target;
			if (inst.TargetKind == CompoundTargetKind.Address) {
				target = LdObj(inst.Target, loadType);
			} else {
				target = Translate(inst.Target, loadType);
			}
			if (UserDefinedCompoundAssign.IsStringConcat(inst.Method)) {
				Debug.Assert(inst.Method.Parameters.Count == 2);
				var value = Translate(inst.Value).ConvertTo(inst.Method.Parameters[1].Type, this, allowImplicitConversion: true);
				var valueExpr = ReplaceMethodCallsWithOperators.RemoveRedundantToStringInConcat(value, inst.Method, isLastArgument: true).Detach();
				return new AssignmentExpression(target, AssignmentOperatorType.Add, valueExpr)
					.WithILInstruction(inst)
					.WithRR(new OperatorResolveResult(inst.Method.ReturnType, ExpressionType.AddAssign, inst.Method, inst.IsLifted, new[] { target.ResolveResult, value.ResolveResult }));
			} else if (inst.Method.Parameters.Count == 2) {
				var value = Translate(inst.Value).ConvertTo(inst.Method.Parameters[1].Type, this);
				AssignmentOperatorType? op = GetAssignmentOperatorTypeFromMetadataName(inst.Method.Name);
				Debug.Assert(op != null);

				return new AssignmentExpression(target, op.Value, value)
					.WithILInstruction(inst)
					.WithRR(new OperatorResolveResult(inst.Method.ReturnType, AssignmentExpression.GetLinqNodeType(op.Value, false), inst.Method, inst.IsLifted, new[] { target.ResolveResult, value.ResolveResult }));
			} else {
				UnaryOperatorType? op = GetUnaryOperatorTypeFromMetadataName(inst.Method.Name, inst.EvalMode == CompoundEvalMode.EvaluatesToOldValue);
				Debug.Assert(op != null);

				return new UnaryOperatorExpression(op.Value, target)
					.WithILInstruction(inst)
					.WithRR(new OperatorResolveResult(inst.Method.ReturnType, UnaryOperatorExpression.GetLinqNodeType(op.Value, false), inst.Method, inst.IsLifted, new[] { target.ResolveResult }));
			}
		}

		internal static AssignmentOperatorType? GetAssignmentOperatorTypeFromMetadataName(string name)
		{
			switch (name) {
				case "op_Addition":
					return AssignmentOperatorType.Add;
				case "op_Subtraction":
					return AssignmentOperatorType.Subtract;
				case "op_Multiply":
					return AssignmentOperatorType.Multiply;
				case "op_Division":
					return AssignmentOperatorType.Divide;
				case "op_Modulus":
					return AssignmentOperatorType.Modulus;
				case "op_BitwiseAnd":
					return AssignmentOperatorType.BitwiseAnd;
				case "op_BitwiseOr":
					return AssignmentOperatorType.BitwiseOr;
				case "op_ExclusiveOr":
					return AssignmentOperatorType.ExclusiveOr;
				case "op_LeftShift":
					return AssignmentOperatorType.ShiftLeft;
				case "op_RightShift":
					return AssignmentOperatorType.ShiftRight;
				default:
					return null;
			}
		}

		internal static UnaryOperatorType? GetUnaryOperatorTypeFromMetadataName(string name, bool isPostfix)
		{
			switch (name) {
				case "op_Increment":
					return isPostfix ? UnaryOperatorType.PostIncrement : UnaryOperatorType.Increment;
				case "op_Decrement":
					return isPostfix ? UnaryOperatorType.PostDecrement : UnaryOperatorType.Decrement;
				default:
					return null;
			}
		}

		protected internal override TranslatedExpression VisitNumericCompoundAssign(NumericCompoundAssign inst, TranslationContext context)
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
		
		TranslatedExpression HandleCompoundAssignment(NumericCompoundAssign inst, AssignmentOperatorType op)
		{
			ExpressionWithResolveResult target;
			if (inst.TargetKind == CompoundTargetKind.Address) {
				target = LdObj(inst.Target, inst.Type);
			} else {
				target = Translate(inst.Target, inst.Type);
			}

			TranslatedExpression resultExpr;
			if (inst.EvalMode == CompoundEvalMode.EvaluatesToOldValue) {
				Debug.Assert(op == AssignmentOperatorType.Add || op == AssignmentOperatorType.Subtract);
				Debug.Assert(inst.Value.MatchLdcI(1) || inst.Value.MatchLdcF4(1) || inst.Value.MatchLdcF8(1));
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
				var value = Translate(inst.Value);
				value = PrepareArithmeticArgument(value, inst.RightInputType, inst.Sign, inst.IsLifted);
				switch (op) {
					case AssignmentOperatorType.Add:
					case AssignmentOperatorType.Subtract:
						if (target.Type.Kind == TypeKind.Pointer) {
							var pao = GetPointerArithmeticOffset(inst.Value, value, ((PointerType)target.Type).ElementType, inst.CheckForOverflow);
							if (pao != null) {
								value = pao.Value;
							} else {
								value.Expression.AddChild(new Comment("ILSpy Error: GetPointerArithmeticOffset() failed", CommentType.MultiLine), Roles.Comment);
							}
						} else {
							IType targetType = NullableType.GetUnderlyingType(target.Type).GetEnumUnderlyingType();
							value = ConvertValue(value, targetType);
						}
						break;
					case AssignmentOperatorType.Multiply:
					case AssignmentOperatorType.Divide:
					case AssignmentOperatorType.Modulus:
					case AssignmentOperatorType.BitwiseAnd:
					case AssignmentOperatorType.BitwiseOr:
					case AssignmentOperatorType.ExclusiveOr: {
							IType targetType = NullableType.GetUnderlyingType(target.Type);
							value = ConvertValue(value, targetType);
							break;
						}
				}
				resultExpr = new AssignmentExpression(target.Expression, op, value.Expression)
					.WithILInstruction(inst)
					.WithRR(new OperatorResolveResult(target.Type, AssignmentExpression.GetLinqNodeType(op, inst.CheckForOverflow), target.ResolveResult, value.ResolveResult));
			}
			if (AssignmentOperatorMightCheckForOverflow(op) && !inst.UnderlyingResultType.IsFloatType()) {
				resultExpr.Expression.AddAnnotation(inst.CheckForOverflow ? AddCheckedBlocks.CheckedAnnotation : AddCheckedBlocks.UncheckedAnnotation);
			}
			return resultExpr;

			TranslatedExpression ConvertValue(TranslatedExpression value, IType targetType)
			{
				bool allowImplicitConversion = true;
				if (targetType.GetStackType() == StackType.I) {
					// Force explicit cast for (U)IntPtr, keep allowing implicit conversion only for n(u)int
					allowImplicitConversion = targetType.IsCSharpNativeIntegerType();
					targetType = targetType.GetSign() == Sign.Unsigned ? SpecialType.NUInt : SpecialType.NInt;
				}
				if (NullableType.IsNullable(value.Type)) {
					targetType = NullableType.Create(compilation, targetType);
				}
				return value.ConvertTo(targetType, this, inst.CheckForOverflow, allowImplicitConversion);
			}
		}
		
		TranslatedExpression HandleCompoundShift(NumericCompoundAssign inst, AssignmentOperatorType op)
		{
			Debug.Assert(inst.EvalMode == CompoundEvalMode.EvaluatesToNewValue);
			ExpressionWithResolveResult target;
			if (inst.TargetKind == CompoundTargetKind.Address) {
				target = LdObj(inst.Target, inst.Type);
			} else {
				target = Translate(inst.Target, inst.Type);
			}
			var value = Translate(inst.Value);

			// Shift operators in C# always expect type 'int' on the right-hand-side
			if (NullableType.IsNullable(value.Type)) {
				value = value.ConvertTo(NullableType.Create(compilation, compilation.FindType(KnownTypeCode.Int32)), this);
			} else {
				value = value.ConvertTo(compilation.FindType(KnownTypeCode.Int32), this);
			}

			return new AssignmentExpression(target.Expression, op, value.Expression)
				.WithILInstruction(inst)
				.WithRR(resolver.ResolveAssignment(op, target.ResolveResult, value.ResolveResult));
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
			Sign hintSign = inst.InputSign;
			if (hintSign == Sign.None) {
				hintSign = context.TypeHint.GetSign();
			}
			var arg = Translate(inst.Argument, typeHint: FindArithmeticType(inst.InputType, hintSign));
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
				// Prefer n(u)int over (U)IntPtr
				if (typeCode == KnownTypeCode.IntPtr && settings.NativeIntegers && !type.Equals(context.TypeHint)) {
					type = SpecialType.NInt;
				} else if (typeCode == KnownTypeCode.UIntPtr && settings.NativeIntegers && !type.Equals(context.TypeHint)) {
					type = SpecialType.NUInt;
				}
				if (inst.IsLifted) {
					type = NullableType.Create(compilation, type);
				}
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
				case ConversionKind.StartGCTracking:
					// A "start gc tracking" conversion is inserted in the ILAst whenever
					// some instruction expects a managed pointer, but we pass an unmanaged pointer.
					// We'll leave the C#-level conversion (from T* to ref T) to the consumer that expects the managed pointer.
					return arg;
				case ConversionKind.StopGCTracking:
					if (inputType.Kind == TypeKind.ByReference) {
						if (PointerArithmeticOffset.IsFixedVariable(inst.Argument)) {
							// cast to corresponding pointer type:
							var pointerType = new PointerType(((ByReferenceType)inputType).ElementType);
							return arg.ConvertTo(pointerType, this).WithILInstruction(inst);
						} else {
							// emit Unsafe.AsPointer() intrinsic:
							return CallUnsafeIntrinsic("AsPointer",
								arguments: new Expression[] { arg },
								returnType: new PointerType(compilation.FindType(KnownTypeCode.Void)),
								inst: inst);
						}
					} else if (arg.Type.GetStackType().IsIntegerType()) {
						// ConversionKind.StopGCTracking should only be used with managed references,
						// but it's possible that we're supposed to stop tracking something we just started to track.
						return arg;
					} else {
						goto default;
					}
				case ConversionKind.SignExtend:
					// We just need to ensure the input type before the conversion is signed.
					// Also, if the argument was translated into an oversized C# type,
					// we need to perform the truncatation to the input stack type.
					if (inputType.GetSign() != Sign.Signed || ValueMightBeOversized(arg.ResolveResult, inputStackType)) {
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
					// We know that C < B (otherwise this wouldn't be the truncation case).
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
				case ConversionKind.Invalid:
					if (inst.InputType == StackType.Unknown && inst.TargetType == IL.PrimitiveType.None && arg.Type.Kind == TypeKind.Unknown) {
						// Unknown -> O conversion.
						// Our post-condition allows us to also use expressions with unknown type where O is expected,
						// so avoid introducing an `(object)` cast because we're likely to cast back to the same unknown type,
						// just in a signature context where we know that it's a class type.
						return arg.WithILInstruction(inst);
					}
					goto default;
				default: {
						// We need to convert to inst.TargetType, or to an equivalent type.
						IType targetType;
						if (inst.TargetType == NullableType.GetUnderlyingType(context.TypeHint).ToPrimitiveType()
							&& NullableType.IsNullable(context.TypeHint) == inst.IsLifted) {
							targetType = context.TypeHint;
						} else if (inst.TargetType == IL.PrimitiveType.Ref) {
							// converting to unknown ref-type
							targetType = new ByReferenceType(compilation.FindType(KnownTypeCode.Byte));
						} else if (inst.TargetType == IL.PrimitiveType.None) {
							// convert to some object type
							// (e.g. invalid I4->O conversion)
							targetType = compilation.FindType(KnownTypeCode.Object);
						} else {
							targetType = GetType(inst.TargetType.ToKnownTypeCode());
						}
						return arg.ConvertTo(targetType, this, inst.CheckForOverflow)
							.WithILInstruction(inst);
					}
			}
		}

		/// <summary>
		/// Gets whether the ResolveResult computes a value that might be oversized for the specified stack type.
		/// </summary>
		bool ValueMightBeOversized(ResolveResult rr, StackType stackType)
		{
			IType inputType = NullableType.GetUnderlyingType(rr.Type);
			if (inputType.GetSize() <= stackType.GetSize()) {
				// The input type is smaller or equal to the stack type,
				// it can't be an oversized value.
				return false;
			}
			if (rr is OperatorResolveResult orr) {
				if (stackType == StackType.I && orr.OperatorType == ExpressionType.Subtract
					&& orr.Operands.Count == 2
					&& orr.Operands[0].Type.Kind == TypeKind.Pointer
					&& orr.Operands[1].Type.Kind == TypeKind.Pointer)
				{
					// Even though a pointer subtraction produces a value of type long in C#,
					// the value will always fit in a native int.
					return false;
				}
			}
			// We don't have any information about the value, so it might be oversized.
			return true;
		}

		protected internal override TranslatedExpression VisitCall(Call inst, TranslationContext context)
		{
			return WrapInRef(new CallBuilder(this, typeSystem, settings).Build(inst), inst.Method.ReturnType);
		}

		protected internal override TranslatedExpression VisitCallVirt(CallVirt inst, TranslationContext context)
		{
			return WrapInRef(new CallBuilder(this, typeSystem, settings).Build(inst), inst.Method.ReturnType);
		}

		TranslatedExpression WrapInRef(TranslatedExpression expr, IType type)
		{
			if (type.Kind == TypeKind.ByReference) {
				return new DirectionExpression(FieldDirection.Ref, expr.Expression)
					.WithoutILInstruction()
					.WithRR(new ByReferenceResolveResult(expr.ResolveResult, ReferenceKind.Ref));
			}
			return expr;
		}

		internal bool IsCurrentOrContainingType(ITypeDefinition type)
		{
			var currentTypeDefinition = decompilationContext.CurrentTypeDefinition;
			while (currentTypeDefinition != null) {
				if (type == currentTypeDefinition)
					return true;
				currentTypeDefinition = currentTypeDefinition.DeclaringTypeDefinition;
			}
			return false;
		}

		internal ExpressionWithResolveResult TranslateFunction(IType delegateType, ILFunction function)
		{
			var method = function.Method?.MemberDefinition as IMethod;

			// Create AnonymousMethodExpression and prepare parameters
			AnonymousMethodExpression ame = new AnonymousMethodExpression();
			ame.IsAsync = function.IsAsync;
			ame.Parameters.AddRange(MakeParameters(function.Parameters, function));
			ame.HasParameterList = ame.Parameters.Count > 0;
			StatementBuilder builder = new StatementBuilder(typeSystem, this.decompilationContext, function, settings, cancellationToken);
			var body = builder.ConvertAsBlock(function.Body);

			Comment prev = null;
			foreach (string warning in function.Warnings) {
				body.InsertChildAfter(prev, prev = new Comment(warning), Roles.Comment);
			}

			bool isLambda = false;
			if (ame.Parameters.Any(p => p.Type.IsNull)) {
				// if there is an anonymous type involved, we are forced to use a lambda expression.
				isLambda = true;
			} else if (settings.UseLambdaSyntax && ame.Parameters.All(p => p.ParameterModifier == ParameterModifier.None)) {
				// otherwise use lambda only if an expression lambda is possible
				isLambda = (body.Statements.Count == 1 && body.Statements.Single() is ReturnStatement);
			}
			// Remove the parameter list from an AnonymousMethodExpression if the parameters are not used in the method body
			var parameterReferencingIdentifiers =
				from ident in body.Descendants.OfType<IdentifierExpression>()
				let v = ident.GetILVariable()
				where v != null && v.Function == function && v.Kind == VariableKind.Parameter
				select ident;
			if (!isLambda && !parameterReferencingIdentifiers.Any()) {
				ame.Parameters.Clear();
				ame.HasParameterList = false;
			}

			Expression replacement;
			IType inferredReturnType;
			if (isLambda) {
				LambdaExpression lambda = new LambdaExpression();
				lambda.IsAsync = ame.IsAsync;
				lambda.CopyAnnotationsFrom(ame);
				ame.Parameters.MoveTo(lambda.Parameters);
				if (body.Statements.Count == 1 && body.Statements.Single() is ReturnStatement returnStmt) {
					lambda.Body = returnStmt.Expression.Detach();
					inferredReturnType = lambda.Body.GetResolveResult().Type;
				} else {
					lambda.Body = body;
					inferredReturnType = InferReturnType(body);
				}
				replacement = lambda;
			} else {
				ame.Body = body;
				inferredReturnType = InferReturnType(body);
				replacement = ame;
			}
			if (ame.IsAsync) {
				inferredReturnType = GetTaskType(inferredReturnType);
			}

			var rr = new DecompiledLambdaResolveResult(
				function, delegateType, inferredReturnType,
				hasParameterList: isLambda || ame.HasParameterList,
				isAnonymousMethod: !isLambda,
				isImplicitlyTyped: ame.Parameters.Any(p => p.Type.IsNull));

			TranslatedExpression translatedLambda = replacement.WithILInstruction(function).WithRR(rr);
			return new CastExpression(ConvertType(delegateType), translatedLambda)
				.WithRR(new ConversionResolveResult(delegateType, rr, LambdaConversion.Instance));
		}

		protected internal override TranslatedExpression VisitILFunction(ILFunction function, TranslationContext context)
		{
			return TranslateFunction(function.DelegateType, function)
				.WithILInstruction(function);
		}

		IType InferReturnType(BlockStatement body)
		{
			var returnExpressions = new List<ResolveResult>();
			CollectReturnExpressions(body);
			var ti = new TypeInference(compilation, resolver.conversions);
			return ti.GetBestCommonType(returnExpressions, out _);
			// Failure to infer a return type does not make the lambda invalid,
			// so we can ignore the 'success' value

			void CollectReturnExpressions(AstNode node)
			{
				if (node is ReturnStatement ret) {
					if (!ret.Expression.IsNull) {
						returnExpressions.Add(ret.Expression.GetResolveResult());
					}
				} else if (node is LambdaExpression || node is AnonymousMethodExpression) {
					// do not recurse into nested lambdas
					return;
				}
				foreach (var child in node.Children) {
					CollectReturnExpressions(child);
				}
			}
		}

		IType GetTaskType(IType resultType)
		{
			if (resultType.Kind == TypeKind.Unknown)
				return SpecialType.UnknownType;
			if (resultType.Kind == TypeKind.Void)
				return compilation.FindType(KnownTypeCode.Task);

			ITypeDefinition def = compilation.FindType(KnownTypeCode.TaskOfT).GetDefinition();
			if (def != null)
				return new ParameterizedType(def, new[] { resultType });
			else
				return SpecialType.UnknownType;
		}

		IEnumerable<ParameterDeclaration> MakeParameters(IReadOnlyList<IParameter> parameters, ILFunction function)
		{
			var variables = function.Variables.Where(v => v.Kind == VariableKind.Parameter).ToDictionary(v => v.Index);
			int i = 0;
			foreach (var parameter in parameters) {
				var pd = astBuilder.ConvertParameter(parameter);
				if (string.IsNullOrEmpty(pd.Name) && !pd.Type.IsArgList()) {
					// needs to be consistent with logic in ILReader.CreateILVarable(ParameterDefinition)
					pd.Name = "P_" + i;
				}
				if (settings.AnonymousTypes && parameter.Type.ContainsAnonymousType())
					pd.Type = null;
				if (variables.TryGetValue(i, out var v))
					pd.AddAnnotation(new ILVariableResolveResult(v, parameters[i].Type));
				yield return pd;
				i++;
			}
		}

		protected internal override TranslatedExpression VisitBlockContainer(BlockContainer container, TranslationContext context)
		{
			var oldReturnContainer = statementBuilder.currentReturnContainer;
			var oldResultType = statementBuilder.currentResultType;
			var oldIsIterator = statementBuilder.currentIsIterator;

			statementBuilder.currentReturnContainer = container;
			statementBuilder.currentResultType = context.TypeHint;
			statementBuilder.currentIsIterator = false;
			try {
				var body = statementBuilder.ConvertAsBlock(container);
				body.InsertChildAfter(null, new Comment(" Could not convert BlockContainer to single expression"), Roles.Comment);
				var ame = new AnonymousMethodExpression { Body = body };
				var systemFuncType = compilation.FindType(typeof(Func<>));
				var blockReturnType = InferReturnType(body);
				var delegateType = new ParameterizedType(systemFuncType, blockReturnType);
				var invocationTarget = new CastExpression(ConvertType(delegateType), ame);
				ResolveResult rr;
				// This might happen when trying to decompile an assembly built for a target framework where System.Func<T> does not exist yet.
				if (systemFuncType.Kind == TypeKind.Unknown) {
					rr = new ResolveResult(blockReturnType);
				} else {
					var invokeMethod = delegateType.GetDelegateInvokeMethod();
					rr = new CSharpInvocationResolveResult(new ResolveResult(delegateType), invokeMethod, EmptyList<ResolveResult>.Instance);
				}
				return new InvocationExpression(new MemberReferenceExpression(invocationTarget, "Invoke"))
					.WithILInstruction(container)
					.WithRR(rr);
			} finally {
				statementBuilder.currentReturnContainer = oldReturnContainer;
				statementBuilder.currentResultType = oldResultType;
				statementBuilder.currentIsIterator = oldIsIterator;
			}
		}

		internal TranslatedExpression TranslateTarget(ILInstruction target, bool nonVirtualInvocation,
			bool memberStatic, IType memberDeclaringType)
		{
			// If references are missing member.IsStatic might not be set correctly.
			// Additionally check target for null, in order to avoid a crash.
			if (!memberStatic && target != null) {
				if (nonVirtualInvocation && MatchLdThis(target) && memberDeclaringType.GetDefinition() != resolver.CurrentTypeDefinition) {
					return new BaseReferenceExpression()
						.WithILInstruction(target)
						.WithRR(new ThisResolveResult(memberDeclaringType, nonVirtualInvocation));
				} else {
					var translatedTarget = Translate(target, memberDeclaringType);
					if (CallInstruction.ExpectedTypeForThisPointer(memberDeclaringType) == StackType.Ref) {
						// When accessing members on value types, ensure we use a reference of the correct type,
						// and not a pointer or a reference to a different type (issue #1333)
						if (!(translatedTarget.Type is ByReferenceType brt && NormalizeTypeVisitor.TypeErasure.EquivalentTypes(brt.ElementType, memberDeclaringType))) {
							translatedTarget = translatedTarget.ConvertTo(new ByReferenceType(memberDeclaringType), this);
						}
					}
					if (translatedTarget.Expression is DirectionExpression) {
						// (ref x).member => x.member
						translatedTarget = translatedTarget.UnwrapChild(((DirectionExpression)translatedTarget).Expression);
					} else if (translatedTarget.Expression is UnaryOperatorExpression uoe
						&& uoe.Operator == UnaryOperatorType.NullConditional
						&& uoe.Expression is DirectionExpression) {
						// (ref x)?.member => x?.member
						translatedTarget = translatedTarget.UnwrapChild(((DirectionExpression)uoe.Expression).Expression);
						// note: we need to create a new ResolveResult for the null-conditional operator,
						// using the underlying type of the input expression without the DirectionExpression
						translatedTarget = new UnaryOperatorExpression(UnaryOperatorType.NullConditional, translatedTarget)
							.WithRR(new ResolveResult(NullableType.GetUnderlyingType(translatedTarget.Type)))
							.WithoutILInstruction();
					}
					translatedTarget = EnsureTargetNotNullable(translatedTarget, target);
					return translatedTarget;
				}
			} else {
				return new TypeReferenceExpression(ConvertType(memberDeclaringType))
					.WithoutILInstruction()
					.WithRR(new TypeResolveResult(memberDeclaringType));
			}

			bool MatchLdThis(ILInstruction inst)
			{
				// ldloc this
				if (inst.MatchLdThis())
					return true;
				if (resolver.CurrentTypeDefinition.Kind == TypeKind.Struct) {
					// box T(ldobj T(ldloc this))
					if (!inst.MatchBox(out var arg, out var type))
						return false;
					if (!arg.MatchLdObj(out var arg2, out var type2))
						return false;
					if (!type.Equals(type2) || !type.Equals(resolver.CurrentTypeDefinition))
						return false;
					return arg2.MatchLdThis();
				}
				return false;
			}
		}

		private TranslatedExpression EnsureTargetNotNullable(TranslatedExpression expr, ILInstruction inst)
		{
			// inst is the instruction that got translated into expr.
			if (expr.Type.Nullability == Nullability.Nullable) {
				if (expr.Expression is UnaryOperatorExpression uoe && uoe.Operator == UnaryOperatorType.NullConditional) {
					return expr;
				}
				if (inst.HasFlag(InstructionFlags.MayUnwrapNull)) {
					// We can't use ! in the chain of operators after a NullConditional, due to
					// https://github.com/dotnet/roslyn/issues/43659
					return expr;
				}
				return new UnaryOperatorExpression(UnaryOperatorType.SuppressNullableWarning, expr)
					.WithRR(new ResolveResult(expr.Type.ChangeNullability(Nullability.Oblivious)))
					.WithoutILInstruction();
			}
			return expr;
		}

		protected internal override TranslatedExpression VisitLdObj(LdObj inst, TranslationContext context)
		{
			var result = LdObj(inst.Target, inst.Type);
			//if (target.Type.IsSmallIntegerType() && loadType.IsSmallIntegerType() && target.Type.GetSign() != loadType.GetSign())
			//	return result.ConvertTo(loadType, this);
			return result.WithILInstruction(inst);
		}

		ExpressionWithResolveResult LdObj(ILInstruction address, IType loadType)
		{
			var target = Translate(address);
			if (TypeUtils.IsCompatiblePointerTypeForMemoryAccess(target.Type, loadType)) {
				ExpressionWithResolveResult result;
				if (target.Expression is DirectionExpression dirExpr) {
					// we can dereference the managed reference by stripping away the 'ref'
					result = target.UnwrapChild(dirExpr.Expression);
				} else if (target.Type is PointerType pointerType) {
					if (target.Expression is UnaryOperatorExpression uoe && uoe.Operator == UnaryOperatorType.AddressOf) {
						// We can dereference the pointer by stripping away the '&'
						result = target.UnwrapChild(uoe.Expression);
					} else {
						// Dereference the existing pointer
						result = new UnaryOperatorExpression(UnaryOperatorType.Dereference, target.Expression)
							.WithRR(new ResolveResult(pointerType.ElementType));
					}
				} else {
					// reference type behind non-DirectionExpression?
					// this case should be impossible, but we can use a pointer cast
					// just to make sure
					target = target.ConvertTo(new PointerType(loadType), this);
					return new UnaryOperatorExpression(UnaryOperatorType.Dereference, target.Expression)
						.WithRR(new ResolveResult(loadType));
				}
				// we don't convert result to inst.Type, because the LdObj type
				// might be inaccurate (it's often System.Object for all reference types),
				// and our parent node should already insert casts where necessary
				return result;
			} else {
				// We need to cast the pointer type:
				target = target.ConvertTo(new PointerType(loadType), this);
				return new UnaryOperatorExpression(UnaryOperatorType.Dereference, target.Expression)
					.WithRR(new ResolveResult(loadType));
			}
		}

		protected internal override TranslatedExpression VisitStObj(StObj inst, TranslationContext context)
		{
			var pointer = Translate(inst.Target);
			TranslatedExpression target;
			TranslatedExpression value = default;
			if (pointer.Expression is DirectionExpression && TypeUtils.IsCompatiblePointerTypeForMemoryAccess(pointer.Type, inst.Type)) {
				// we can deference the managed reference by stripping away the 'ref'
				target = pointer.UnwrapChild(((DirectionExpression)pointer.Expression).Expression);
			} else {
				// Cast pointer type if necessary:
				if (!TypeUtils.IsCompatiblePointerTypeForMemoryAccess(pointer.Type, inst.Type)) {
					value = Translate(inst.Value, typeHint: inst.Type);
					if (TypeUtils.IsCompatibleTypeForMemoryAccess(value.Type, inst.Type)) {
						pointer = pointer.ConvertTo(new PointerType(value.Type), this);
					} else {
						pointer = pointer.ConvertTo(new PointerType(inst.Type), this);
					}
				}
				if (pointer.Expression is UnaryOperatorExpression uoe && uoe.Operator == UnaryOperatorType.AddressOf) {
					// *&ptr -> ptr
					target = pointer.UnwrapChild(uoe.Expression);
				} else {
					target = new UnaryOperatorExpression(UnaryOperatorType.Dereference, pointer.Expression)
						.WithoutILInstruction()
						.WithRR(new ResolveResult(((TypeWithElementType)pointer.Type).ElementType));
				}
			}
			if (value.Expression == null) {
				value = Translate(inst.Value, typeHint: target.Type);
			}
			return Assignment(target, value).WithILInstruction(inst);
		}

		protected internal override TranslatedExpression VisitLdLen(LdLen inst, TranslationContext context)
		{
			IType arrayType = compilation.FindType(KnownTypeCode.Array);
			TranslatedExpression arrayExpr = Translate(inst.Array, typeHint: arrayType);
			if (arrayExpr.Type.Kind != TypeKind.Array) {
				arrayExpr = arrayExpr.ConvertTo(arrayType, this);
			}
			arrayExpr = EnsureTargetNotNullable(arrayExpr, inst.Array);
			string memberName;
			KnownTypeCode code;
			if (inst.ResultType == StackType.I4) {
				memberName = "Length";
				code = KnownTypeCode.Int32;
			} else {
				memberName = "LongLength";
				code = KnownTypeCode.Int64;
			}
			IProperty member = arrayType.GetProperties(p => p.Name == memberName).FirstOrDefault();
			ResolveResult rr = member == null
				? new ResolveResult(compilation.FindType(code))
				: new MemberResolveResult(arrayExpr.ResolveResult, member);
			return new MemberReferenceExpression(arrayExpr.Expression, memberName)
				.WithILInstruction(inst)
				.WithRR(rr);
		}
		
		protected internal override TranslatedExpression VisitLdFlda(LdFlda inst, TranslationContext context)
		{
			if (settings.FixedBuffers && inst.Field.Name == "FixedElementField"
				&& inst.Target is LdFlda nestedLdFlda
				&& CSharpDecompiler.IsFixedField(nestedLdFlda.Field, out var elementType, out _))
			{
				Expression fieldAccess = ConvertField(nestedLdFlda.Field, nestedLdFlda.Target);
				var mrr = (MemberResolveResult)fieldAccess.GetResolveResult();
				fieldAccess.RemoveAnnotations<ResolveResult>();
				var result = fieldAccess.WithRR(new MemberResolveResult(mrr.TargetResult, mrr.Member, new PointerType(elementType)))
					.WithILInstruction(inst);
				if (inst.ResultType == StackType.Ref) {
					// convert pointer back to ref
					return result.ConvertTo(new ByReferenceType(elementType), this);
				} else {
					return result;
				}
			}
			TranslatedExpression expr;
			if (TupleTransform.MatchTupleFieldAccess(inst, out IType underlyingTupleType, out var target, out int position)) {
				var translatedTarget = TranslateTarget(target,
					nonVirtualInvocation: true,
					memberStatic: false,
					memberDeclaringType: underlyingTupleType);
				if (translatedTarget.Type is TupleType tupleType && NormalizeTypeVisitor.TypeErasure.EquivalentTypes(tupleType, underlyingTupleType) && position <= tupleType.ElementNames.Length) {
					string elementName = tupleType.ElementNames[position - 1];
					if (elementName == null) {
						elementName = "Item" + position;
					}
					// tupleType.ElementTypes are more accurate w.r.t. nullability/dynamic than inst.Field.Type
					var rr = new MemberResolveResult(translatedTarget.ResolveResult, inst.Field,
						returnTypeOverride: tupleType.ElementTypes[position - 1]);
					expr = new MemberReferenceExpression(translatedTarget, elementName)
						.WithRR(rr).WithILInstruction(inst);
				} else {
					expr = ConvertField(inst.Field, inst.Target).WithILInstruction(inst);
				}
			} else {
				expr = ConvertField(inst.Field, inst.Target).WithILInstruction(inst);
			}
			if (inst.ResultType == StackType.I) {
				// ldflda producing native pointer
				return new UnaryOperatorExpression(UnaryOperatorType.AddressOf, expr)
					.WithoutILInstruction().WithRR(new ResolveResult(new PointerType(expr.Type)));
			} else {
				// ldflda producing managed pointer
				return new DirectionExpression(FieldDirection.Ref, expr)
					.WithoutILInstruction().WithRR(new ByReferenceResolveResult(expr.ResolveResult, ReferenceKind.Ref));
			}
		}
		
		protected internal override TranslatedExpression VisitLdsFlda(LdsFlda inst, TranslationContext context)
		{
			var expr = ConvertField(inst.Field).WithILInstruction(inst);
			return new DirectionExpression(FieldDirection.Ref, expr)
				.WithoutILInstruction().WithRR(new ByReferenceResolveResult(expr.Type, ReferenceKind.Ref));
		}
		
		protected internal override TranslatedExpression VisitLdElema(LdElema inst, TranslationContext context)
		{
			TranslatedExpression arrayExpr = Translate(inst.Array);
			var arrayType = arrayExpr.Type as ArrayType;
			if (arrayType == null || !TypeUtils.IsCompatibleTypeForMemoryAccess(arrayType.ElementType, inst.Type)) {
				arrayType = new ArrayType(compilation, inst.Type, inst.Indices.Count);
				arrayExpr = arrayExpr.ConvertTo(arrayType, this);
			}
			IndexerExpression indexerExpr;
			if (inst.WithSystemIndex) {
				var systemIndex = compilation.FindType(KnownTypeCode.Index);
				indexerExpr = new IndexerExpression(
					arrayExpr, inst.Indices.Select(i => Translate(i, typeHint: systemIndex).ConvertTo(systemIndex, this).Expression)
				);
			} else {
				indexerExpr = new IndexerExpression(
					arrayExpr, inst.Indices.Select(i => TranslateArrayIndex(i).Expression)
				);
			}
			TranslatedExpression expr = indexerExpr.WithILInstruction(inst).WithRR(new ResolveResult(arrayType.ElementType));
			return new DirectionExpression(FieldDirection.Ref, expr)
				.WithoutILInstruction().WithRR(new ByReferenceResolveResult(expr.Type, ReferenceKind.Ref));
		}
		
		TranslatedExpression TranslateArrayIndex(ILInstruction i)
		{
			var input = Translate(i);
			if (i.ResultType == StackType.I4 && input.Type.IsSmallIntegerType() && input.Type.Kind != TypeKind.Enum) {
				return input; // we don't need a cast, just let small integers be promoted to int
			}
			IType targetType = FindArithmeticType(i.ResultType, input.Type.GetSign());
			return input.ConvertTo(targetType, this);
		}
		
		internal static bool IsUnboxAnyWithIsInst(UnboxAny unboxAny, IsInst isInst)
		{
			return unboxAny.Type.Equals(isInst.Type)
				&& (unboxAny.Type.IsKnownType(KnownTypeCode.NullableOfT) || isInst.Type.IsReferenceType == true);
		}

		protected internal override TranslatedExpression VisitUnboxAny(UnboxAny inst, TranslationContext context)
		{
			TranslatedExpression arg;
			if (inst.Argument is IsInst isInst && IsUnboxAnyWithIsInst(inst, isInst)) {
				// unbox.any T(isinst T(expr)) ==> expr as T
				// This is used for generic types and nullable value types
				arg = UnwrapBoxingConversion(Translate(isInst.Argument));
				return new AsExpression(arg, ConvertType(inst.Type))
					.WithILInstruction(inst)
					.WithRR(new ConversionResolveResult(inst.Type, arg.ResolveResult, Conversion.TryCast));
			}

			arg = Translate(inst.Argument);
			IType targetType = inst.Type;
			if (targetType.Kind == TypeKind.TypeParameter) {
				var rr = resolver.ResolveCast(targetType, arg.ResolveResult);
				if (rr.IsError) {
					// C# 6.2.7 Explicit conversions involving type parameters:
					// if we can't directly convert to a type parameter,
					// try via its effective base class.
					arg = arg.ConvertTo(((ITypeParameter)targetType).EffectiveBaseClass, this);
				}
			} else {
				// Before unboxing arg must be a object
				arg = arg.ConvertTo(compilation.FindType(KnownTypeCode.Object), this);
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
				.WithRR(new ByReferenceResolveResult(castExpression.ResolveResult, ReferenceKind.Ref));
		}

		protected internal override TranslatedExpression VisitBox(Box inst, TranslationContext context)
		{
			IType targetType = inst.Type;
			var arg = Translate(inst.Argument, typeHint: targetType);
			if (settings.NativeIntegers && !arg.Type.Equals(targetType)) {
				if (targetType.IsKnownType(KnownTypeCode.IntPtr)) {
					targetType = SpecialType.NInt;
				} else if (targetType.IsKnownType(KnownTypeCode.UIntPtr)) {
					targetType = SpecialType.NUInt;
				}
			}
			arg = arg.ConvertTo(targetType, this);
			var obj = compilation.FindType(KnownTypeCode.Object);
			return new CastExpression(ConvertType(obj), arg.Expression)
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(obj, arg.ResolveResult, Conversion.BoxingConversion));
		}
		
		protected internal override TranslatedExpression VisitCastClass(CastClass inst, TranslationContext context)
		{
			return Translate(inst.Argument).ConvertTo(inst.Type, this);
		}

		protected internal override TranslatedExpression VisitExpressionTreeCast(ExpressionTreeCast inst, TranslationContext context)
		{
			return Translate(inst.Argument).ConvertTo(inst.Type, this, inst.IsChecked);
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
				.WithRR(new ByReferenceResolveResult(inst.Type, ReferenceKind.Ref));
		}
		
		protected internal override TranslatedExpression VisitBlock(Block block, TranslationContext context)
		{
			switch (block.Kind) {
				case BlockKind.ArrayInitializer:
					return TranslateArrayInitializer(block);
				case BlockKind.StackAllocInitializer:
					return TranslateStackAllocInitializer(block, context.TypeHint);
				case BlockKind.CollectionInitializer:
				case BlockKind.ObjectInitializer:
					return TranslateObjectAndCollectionInitializer(block);
				case BlockKind.CallInlineAssign:
					return TranslateSetterCallAssignment(block);
				case BlockKind.CallWithNamedArgs:
					return TranslateCallWithNamedArgs(block);
				default:
					return ErrorExpression("Unknown block type: " + block.Kind);
			}
		}

		private TranslatedExpression TranslateCallWithNamedArgs(Block block)
		{
			return WrapInRef(
				new CallBuilder(this, typeSystem, settings).CallWithNamedArgs(block),
				((CallInstruction)block.FinalInstruction).Method.ReturnType);
		}

		private TranslatedExpression TranslateSetterCallAssignment(Block block)
		{
			if (!block.MatchInlineAssignBlock(out var call, out var value)) {
				// should never happen unless the ILAst is invalid
				return ErrorExpression("Error: MatchInlineAssignBlock() returned false");
			}
			var arguments = call.Arguments.ToList();
			arguments[arguments.Count - 1] = value;
			return new CallBuilder(this, typeSystem, settings)
				.Build(call.OpCode, call.Method, arguments)
				.WithILInstruction(call);
		}

		TranslatedExpression TranslateObjectAndCollectionInitializer(Block block)
		{
			var stloc = block.Instructions.FirstOrDefault() as StLoc;
			var final = block.FinalInstruction as LdLoc;
			// Check basic structure of block
			if (stloc == null || final == null || stloc.Variable != final.Variable
				|| stloc.Variable.Kind != VariableKind.InitializerTarget)
				throw new ArgumentException("given Block is invalid!");
			InitializedObjectResolveResult initObjRR;
			TranslatedExpression expr;
			// Detect type of initializer
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
				case Block callWithNamedArgs when callWithNamedArgs.Kind == BlockKind.CallWithNamedArgs:
					expr = TranslateCallWithNamedArgs(callWithNamedArgs);
					initObjRR = new InitializedObjectResolveResult(expr.Type);
					break;
				default:
					throw new ArgumentException("given Block is invalid!");
			}
			// Build initializer expression
			var elementsStack = new Stack<List<TranslatedExpression>>();
			var elements = new List<TranslatedExpression>(block.Instructions.Count);
			elementsStack.Push(elements);
			List<IL.Transforms.AccessPathElement> currentPath = null;
			var indexVariables = new Dictionary<ILVariable, ILInstruction>();
			foreach (var inst in block.Instructions.Skip(1)) {
				// Collect indexer variables (for C# 6 dictionary initializers)
				if (inst is StLoc indexStore) {
					indexVariables.Add(indexStore.Variable, indexStore.Value);
					continue;
				}
				// Get current path
				var info = IL.Transforms.AccessPathElement.GetAccessPath(inst, initObjRR.Type, settings: settings);
				// This should not happen, because the IL transform should not create invalid access paths,
				// but we leave it here as sanity check.
				if (info.Kind == IL.Transforms.AccessPathKind.Invalid) continue;
				// Calculate "difference" to previous path
				if (currentPath == null) {
					currentPath = info.Path;
				} else {
					int minLen = Math.Min(currentPath.Count, info.Path.Count);
					int firstDifferenceIndex = 0;
					while (firstDifferenceIndex < minLen && info.Path[firstDifferenceIndex] == currentPath[firstDifferenceIndex])
						firstDifferenceIndex++;
					while (elementsStack.Count - 1 > firstDifferenceIndex) {
						var methodElement = currentPath[elementsStack.Count - 1];
						var pathElement = currentPath[elementsStack.Count - 2];
						var values = elementsStack.Pop();
						elementsStack.Peek().Add(MakeInitializerAssignment(initObjRR, methodElement, pathElement, values, indexVariables));
					}
					currentPath = info.Path;
				}
				// Fill the stack with empty expression lists
				while (elementsStack.Count < currentPath.Count)
					elementsStack.Push(new List<TranslatedExpression>());
				var lastElement = currentPath.Last();
				var memberRR = new MemberResolveResult(initObjRR, lastElement.Member);
				switch (info.Kind) {
					case IL.Transforms.AccessPathKind.Adder:
						Debug.Assert(lastElement.Member is IMethod);
						elementsStack.Peek().Add(
							new CallBuilder(this, typeSystem, settings)
								.BuildCollectionInitializerExpression(lastElement.OpCode, (IMethod)lastElement.Member, initObjRR, info.Values)
								.WithILInstruction(inst)
						);
						break;
					case IL.Transforms.AccessPathKind.Setter:
						Debug.Assert(lastElement.Member is IProperty || lastElement.Member is IField);
						if (lastElement.Indices?.Length > 0) {
							var property = (IProperty)lastElement.Member;
							Debug.Assert(property.IsIndexer);
							elementsStack.Peek().Add(
								new CallBuilder(this, typeSystem, settings)
									.BuildDictionaryInitializerExpression(lastElement.OpCode, property.Setter, initObjRR, GetIndices(lastElement.Indices, indexVariables).ToList(), info.Values.Single())
									.WithILInstruction(inst)
							);
						} else {
							var value = Translate(info.Values.Single(), typeHint: memberRR.Type)
								.ConvertTo(memberRR.Type, this, allowImplicitConversion: true);
							var assignment = new NamedExpression(lastElement.Member.Name, value)
								.WithILInstruction(inst).WithRR(memberRR);
							elementsStack.Peek().Add(assignment);
						}
						break;
				}
			}
			while (elementsStack.Count > 1) {
				var methodElement = currentPath[elementsStack.Count - 1];
				var pathElement = currentPath[elementsStack.Count - 2];
				var values = elementsStack.Pop();
				elementsStack.Peek().Add(
					MakeInitializerAssignment(initObjRR, methodElement, pathElement, values, indexVariables)
				);
			}
			var oce = (ObjectCreateExpression)expr.Expression;
			oce.Initializer = new ArrayInitializerExpression(elements.SelectArray(e => e.Expression));
			return expr.WithILInstruction(block);
		}

		IEnumerable<ILInstruction> GetIndices(IEnumerable<ILInstruction> indices, Dictionary<ILVariable, ILInstruction> indexVariables)
		{
			foreach (var inst in indices) {
				if (inst is LdLoc ld && indexVariables.TryGetValue(ld.Variable, out var newInst))
					yield return newInst;
				else
					yield return inst;
			}
		}

		TranslatedExpression MakeInitializerAssignment(InitializedObjectResolveResult rr, IL.Transforms.AccessPathElement memberPath,
			IL.Transforms.AccessPathElement valuePath, List<TranslatedExpression> values,
			Dictionary<ILVariable, ILInstruction> indexVariables)
		{
			TranslatedExpression value;
			if (memberPath.Member is IMethod method && method.Name == "Add") {
				value = new ArrayInitializerExpression(values.Select(v => v.Expression))
					.WithRR(new ResolveResult(SpecialType.UnknownType))
					.WithoutILInstruction();
			} else if (values.Count == 1 && !(values[0].Expression is AssignmentExpression || values[0].Expression is NamedExpression)) {
				value = values[0];
			} else {
				value = new ArrayInitializerExpression(values.Select(v => v.Expression))
					.WithRR(new ResolveResult(SpecialType.UnknownType))
					.WithoutILInstruction();
			}
			if (valuePath.Indices?.Length > 0) {
				Expression index;
				if (memberPath.Member is IProperty property) {
					index = new CallBuilder(this, typeSystem, settings)
						.BuildDictionaryInitializerExpression(valuePath.OpCode, property.Setter, rr, GetIndices(valuePath.Indices, indexVariables).ToList());
				} else {
					index = new IndexerExpression(null, GetIndices(valuePath.Indices, indexVariables).Select(i => Translate(i).Expression));
				}
				return new AssignmentExpression(index, value)
					.WithRR(new MemberResolveResult(rr, memberPath.Member))
					.WithoutILInstruction();
			} else {
				return new NamedExpression(valuePath.Member.Name, value)
					.WithRR(new MemberResolveResult(rr, valuePath.Member))
					.WithoutILInstruction();
			}
		}

		class ArrayInitializer
		{
			public ArrayInitializer(ArrayInitializerExpression expression)
			{
				this.Expression = expression;
				this.CurrentElementCount = 0;
			}

			public ArrayInitializerExpression Expression;
			// HACK: avoid using Expression.Elements.Count: https://github.com/icsharpcode/ILSpy/issues/1202
			public int CurrentElementCount;
		}

		TranslatedExpression TranslateArrayInitializer(Block block)
		{
			var stloc = block.Instructions.FirstOrDefault() as StLoc;
			var final = block.FinalInstruction as LdLoc;
			if (stloc == null || final == null || !stloc.Value.MatchNewArr(out IType type))
				throw new ArgumentException("given Block is invalid!");
			if (stloc.Variable != final.Variable || stloc.Variable.Kind != VariableKind.InitializerTarget)
				throw new ArgumentException("given Block is invalid!");
			var newArr = (NewArr)stloc.Value;

			var translatedDimensions = newArr.Indices.SelectArray(i => Translate(i));

			if (!translatedDimensions.All(dim => dim.ResolveResult.IsCompileTimeConstant))
				throw new ArgumentException("given Block is invalid!");
			int dimensions = newArr.Indices.Count;
			int[] dimensionSizes = translatedDimensions.SelectArray(dim => (int)dim.ResolveResult.ConstantValue);
			var container = new Stack<ArrayInitializer>();
			var root = new ArrayInitializer(new ArrayInitializerExpression());
			container.Push(root);
			var elementResolveResults = new List<ResolveResult>();

			for (int i = 1; i < block.Instructions.Count; i++) {
				if (!block.Instructions[i].MatchStObj(out ILInstruction target, out ILInstruction value, out IType t) || !type.Equals(t))
					throw new ArgumentException("given Block is invalid!");
				if (!target.MatchLdElema(out t, out ILInstruction array) || !type.Equals(t))
					throw new ArgumentException("given Block is invalid!");
				if (!array.MatchLdLoc(out ILVariable v) || v != final.Variable)
					throw new ArgumentException("given Block is invalid!");
				while (container.Count < dimensions) {
					var aie = new ArrayInitializerExpression();
					var parentInitializer = container.Peek();
					if (parentInitializer.CurrentElementCount > 0)
						parentInitializer.Expression.AddChild(new CSharpTokenNode(TextLocation.Empty, Roles.Comma), Roles.Comma);
					parentInitializer.Expression.Elements.Add(aie);
					parentInitializer.CurrentElementCount++;
					container.Push(new ArrayInitializer(aie));
				}
				TranslatedExpression val;
				var old = astBuilder.UseSpecialConstants;
				try {
					astBuilder.UseSpecialConstants = !type.IsCSharpPrimitiveIntegerType() && !type.IsKnownType(KnownTypeCode.Decimal);
					val = Translate(value, typeHint: type).ConvertTo(type, this, allowImplicitConversion: true);
				} finally {
					astBuilder.UseSpecialConstants = old;
				}
				var currentInitializer = container.Peek();
				if (currentInitializer.CurrentElementCount > 0)
					currentInitializer.Expression.AddChild(new CSharpTokenNode(TextLocation.Empty, Roles.Comma), Roles.Comma);
				currentInitializer.Expression.Elements.Add(val);
				currentInitializer.CurrentElementCount++;
				elementResolveResults.Add(val.ResolveResult);
				while (container.Count > 0 && container.Peek().CurrentElementCount == dimensionSizes[container.Count - 1]) {
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
				if (typeExpression is ComposedType compType && compType.ArraySpecifiers.Count > 0) {
					additionalSpecifiers = compType.ArraySpecifiers.Select(a => (ArraySpecifier)a.Clone()).ToArray();
					compType.ArraySpecifiers.Clear();
				} else {
					additionalSpecifiers = Empty<ArraySpecifier>.Array;
				}
			}
			var expr = new ArrayCreateExpression {
				Type = typeExpression,
				Initializer = root.Expression
			};
			expr.AdditionalArraySpecifiers.AddRange(additionalSpecifiers);
			if (!type.ContainsAnonymousType())
				expr.Arguments.AddRange(newArr.Indices.Select(i => Translate(i).Expression));
			return expr.WithILInstruction(block)
				.WithRR(new ArrayCreateResolveResult(new ArrayType(compilation, type, dimensions), newArr.Indices.Select(i => Translate(i).ResolveResult).ToArray(), elementResolveResults));
		}

		TranslatedExpression TranslateStackAllocInitializer(Block block, IType typeHint)
		{
			var stloc = block.Instructions.FirstOrDefault() as StLoc;
			var final = block.FinalInstruction as LdLoc;
			if (stloc == null || final == null || stloc.Variable != final.Variable || stloc.Variable.Kind != VariableKind.InitializerTarget)
				throw new ArgumentException("given Block is invalid!");
			StackAllocExpression stackAllocExpression;
			IType elementType;
			if (block.Instructions.Count < 2 || !block.Instructions[1].MatchStObj(out _, out _, out var t))
				throw new ArgumentException("given Block is invalid!");
			if (typeHint is PointerType pt && !TypeUtils.IsCompatibleTypeForMemoryAccess(t, pt.ElementType)) {
				typeHint = new PointerType(t);
			}
			switch (stloc.Value) {
				case LocAlloc locAlloc:
					stackAllocExpression = TranslateLocAlloc(locAlloc, typeHint, out elementType);
					break;
				case LocAllocSpan locAllocSpan:
					stackAllocExpression = TranslateLocAllocSpan(locAllocSpan, typeHint, out elementType);
					break;
				default:
					throw new ArgumentException("given Block is invalid!");
			}
			var initializer = stackAllocExpression.Initializer = new ArrayInitializerExpression();
			var pointerType = new PointerType(elementType);
			long expectedOffset = 0;

			for (int i = 1; i < block.Instructions.Count; i++) {
				// stobj type(binary.add.i(ldloc I_0, conv i4->i <sign extend> (ldc.i4 offset)), value)
				if (!block.Instructions[i].MatchStObj(out var target, out var value, out t) || !TypeUtils.IsCompatibleTypeForMemoryAccess(elementType, t))
					throw new ArgumentException("given Block is invalid!");
				long offset = 0;
				target = target.UnwrapConv(ConversionKind.StopGCTracking);

				if (!target.MatchLdLoc(stloc.Variable)) {
					if (!target.MatchBinaryNumericInstruction(BinaryNumericOperator.Add, out var left, out var right))
						throw new ArgumentException("given Block is invalid!");
					var binary = (BinaryNumericInstruction)target;
					left = left.UnwrapConv(ConversionKind.StopGCTracking);
					var offsetInst = PointerArithmeticOffset.Detect(right, pointerType.ElementType, binary.CheckForOverflow);
					if (!left.MatchLdLoc(final.Variable) || offsetInst == null)
						throw new ArgumentException("given Block is invalid!");
					if (!offsetInst.MatchLdcI(out offset))
						throw new ArgumentException("given Block is invalid!");
				}
				while (expectedOffset < offset) {
					initializer.Elements.Add(Translate(IL.Transforms.TransformArrayInitializers.GetNullExpression(elementType), typeHint: elementType));
					expectedOffset++;
				}
				var val = Translate(value, typeHint: elementType).ConvertTo(elementType, this, allowImplicitConversion: true);
				initializer.Elements.Add(val);
				expectedOffset++;
			}
			return stackAllocExpression.WithILInstruction(block)
				.WithRR(new ResolveResult(stloc.Variable.Type));
		}

		/// <summary>
		/// If expr is a constant integer expression, and its value fits into type,
		/// convert the expression into the target type.
		/// Otherwise, returns the expression unmodified.
		/// </summary>
		TranslatedExpression AdjustConstantExpressionToType(TranslatedExpression expr, IType typeHint)
		{
			var newRR = AdjustConstantToType(expr.ResolveResult, typeHint);
			if (newRR == expr.ResolveResult) {
				return expr;
			} else {
				return ConvertConstantValue(newRR, allowImplicitConversion: true).WithILInstruction(expr.ILInstructions);
			}
		}
		
		private ResolveResult AdjustConstantToType(ResolveResult rr, IType typeHint)
		{
			if (!rr.IsCompileTimeConstant) {
				return rr;
			}
			typeHint = NullableType.GetUnderlyingType(typeHint);
			if (rr.Type.Equals(typeHint)) {
				return rr;
			}
			// Convert to type hint, if this is possible without loss of accuracy
			if (typeHint.IsKnownType(KnownTypeCode.Boolean)) {
				if (object.Equals(rr.ConstantValue, 0) || object.Equals(rr.ConstantValue, 0u)) {
					rr = new ConstantResolveResult(typeHint, false);
				} else if (object.Equals(rr.ConstantValue, 1) || object.Equals(rr.ConstantValue, 1u)) {
					rr = new ConstantResolveResult(typeHint, true);
				}
			} else if (typeHint.Kind == TypeKind.Enum || typeHint.IsKnownType(KnownTypeCode.Char) || typeHint.IsCSharpSmallIntegerType()) {
				var castRR = resolver.WithCheckForOverflow(true).ResolveCast(typeHint, rr);
				if (castRR.IsCompileTimeConstant && !castRR.IsError) {
					rr = castRR;
				}
			}
			return rr;
		}

		protected internal override TranslatedExpression VisitNullCoalescingInstruction(NullCoalescingInstruction inst, TranslationContext context)
		{
			var value = Translate(inst.ValueInst);
			var fallback = Translate(inst.FallbackInst);
			fallback = AdjustConstantExpressionToType(fallback, value.Type);
			var rr = resolver.ResolveBinaryOperator(BinaryOperatorType.NullCoalescing, value.ResolveResult, fallback.ResolveResult);
			if (rr.IsError) {
				IType targetType;
				if (fallback.Expression is ThrowExpression && fallback.Type.Equals(SpecialType.NoType)) {
					targetType = NullableType.GetUnderlyingType(value.Type);
				} else if (!value.Type.Equals(SpecialType.NullType) && !fallback.Type.Equals(SpecialType.NullType) && !value.Type.Equals(fallback.Type)) {
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
			var trueBranch = Translate(inst.TrueInst, typeHint: context.TypeHint);
			var falseBranch = Translate(inst.FalseInst, typeHint: context.TypeHint);
			BinaryOperatorType op = BinaryOperatorType.Any;
			TranslatedExpression rhs = default(TranslatedExpression);

			if (inst.MatchLogicAnd(out var lhsInst, out var rhsInst) && !rhsInst.MatchLdcI4(1)) {
				op = BinaryOperatorType.ConditionalAnd;
				Debug.Assert(rhsInst == inst.TrueInst);
				rhs = trueBranch;
			} else if (inst.MatchLogicOr(out lhsInst, out rhsInst) && !rhsInst.MatchLdcI4(0)) {
				op = BinaryOperatorType.ConditionalOr;
				Debug.Assert(rhsInst == inst.FalseInst);
				rhs = falseBranch;
			}
			// ILAst LogicAnd/LogicOr can return a different value than 0 or 1
			// if the rhs is evaluated.
			// We can only correctly translate it to C# if the rhs is of type boolean:
			if (op != BinaryOperatorType.Any && (rhs.Type.IsKnownType(KnownTypeCode.Boolean) || IfInstruction.IsInConditionSlot(inst))) {
				rhs = rhs.ConvertToBoolean(this);
				return new BinaryOperatorExpression(condition, op, rhs)
					.WithILInstruction(inst)
					.WithRR(new ResolveResult(compilation.FindType(KnownTypeCode.Boolean)));
			}

			condition = condition.UnwrapImplicitBoolConversion();
			trueBranch = AdjustConstantExpressionToType(trueBranch, falseBranch.Type);
			falseBranch = AdjustConstantExpressionToType(falseBranch, trueBranch.Type);

			var rr = resolver.ResolveConditional(condition.ResolveResult, trueBranch.ResolveResult, falseBranch.ResolveResult);
			if (rr.IsError) {
				IType targetType;
				if (!trueBranch.Type.Equals(SpecialType.NullType) && !falseBranch.Type.Equals(SpecialType.NullType) && !trueBranch.Type.Equals(falseBranch.Type)) {
					targetType = typeInference.GetBestCommonType(new[] { trueBranch.ResolveResult, falseBranch.ResolveResult }, out bool success);
					if (!success || targetType.GetStackType() != inst.ResultType) {
						// Figure out the target type based on inst.ResultType.
						if (inst.ResultType == StackType.Ref) {
							// targetType should be a ref-type
							if (trueBranch.Type.Kind == TypeKind.ByReference) {
								targetType = trueBranch.Type;
							} else if (falseBranch.Type.Kind == TypeKind.ByReference) {
								targetType = falseBranch.Type;
							} else {
								// fall back to 'ref byte' if we can't determine a referenced type otherwise
								targetType = new ByReferenceType(compilation.FindType(KnownTypeCode.Byte));
							}
						} else {
							targetType = compilation.FindType(inst.ResultType.ToKnownTypeCode());
						}
					}
				} else {
					targetType = trueBranch.Type.Equals(SpecialType.NullType) ? falseBranch.Type : trueBranch.Type;
				}
				trueBranch = trueBranch.ConvertTo(targetType, this);
				falseBranch = falseBranch.ConvertTo(targetType, this);
				rr = new ResolveResult(targetType);
			}
			if (rr.Type.Kind == TypeKind.ByReference) {
				// C# conditional ref looks like this:
				// ref (arr != null ? ref trueBranch : ref falseBranch);
				var conditionalResolveResult = new ResolveResult(((ByReferenceType)rr.Type).ElementType);
				return new DirectionExpression(FieldDirection.Ref,
					new ConditionalExpression(condition.Expression, trueBranch.Expression, falseBranch.Expression)
						.WithILInstruction(inst)
						.WithRR(conditionalResolveResult)
				).WithoutILInstruction().WithRR(new ByReferenceResolveResult(conditionalResolveResult, ReferenceKind.Ref));
			} else {
				return new ConditionalExpression(condition.Expression, trueBranch.Expression, falseBranch.Expression)
					.WithILInstruction(inst)
					.WithRR(rr);
			}
		}

		protected internal override TranslatedExpression VisitSwitchInstruction(SwitchInstruction inst, TranslationContext context)
		{
			TranslatedExpression value;
			if (inst.Value is StringToInt strToInt) {
				value = Translate(strToInt.Argument);
			} else {
				strToInt = null;
				value = Translate(inst.Value);
			}

			IL.SwitchSection defaultSection = inst.GetDefaultSection();
			SwitchExpression switchExpr = new SwitchExpression();
			switchExpr.Expression = value;
			IType resultType;
			if (context.TypeHint.Kind != TypeKind.Unknown && context.TypeHint.GetStackType() == inst.ResultType) {
				resultType = context.TypeHint;
			} else {
				resultType = compilation.FindType(inst.ResultType.ToKnownTypeCode());
			}
			
			foreach (var section in inst.Sections) {
				if (section == defaultSection)
					continue;
				var ses = new SwitchExpressionSection();
				if (section.HasNullLabel) {
					Debug.Assert(section.Labels.IsEmpty);
					ses.Pattern = new NullReferenceExpression();
				} else {
					long val = section.Labels.Values.Single();
					var rr = statementBuilder.CreateTypedCaseLabel(val, value.Type, strToInt?.Map).Single();
					ses.Pattern = astBuilder.ConvertConstantValue(rr);
				}
				ses.Body = TranslateSectionBody(section);
				switchExpr.SwitchSections.Add(ses);
			}

			var defaultSES = new SwitchExpressionSection();
			defaultSES.Pattern = new IdentifierExpression("_");
			defaultSES.Body = TranslateSectionBody(defaultSection);
			switchExpr.SwitchSections.Add(defaultSES);

			return switchExpr.WithILInstruction(inst).WithRR(new ResolveResult(resultType));

			Expression TranslateSectionBody(IL.SwitchSection section)
			{
				var body = Translate(section.Body, resultType);
				return body.ConvertTo(resultType, this, allowImplicitConversion: true);
			}
		}

		protected internal override TranslatedExpression VisitAddressOf(AddressOf inst, TranslationContext context)
		{
			// HACK: this is only correct if the argument is an R-value; otherwise we're missing the copy to the temporary
			var value = Translate(inst.Value, inst.Type);
			value = value.ConvertTo(inst.Type, this);
			return new DirectionExpression(FieldDirection.Ref, value)
				.WithILInstruction(inst)
				.WithRR(new ByReferenceResolveResult(value.ResolveResult, ReferenceKind.Ref));
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

		protected internal override TranslatedExpression VisitNullableRewrap(NullableRewrap inst, TranslationContext context)
		{
			var arg = Translate(inst.Argument);
			IType type = arg.Type;
			if (NullableType.IsNonNullableValueType(type)) {
				type = NullableType.Create(compilation, type);
			}
			return new UnaryOperatorExpression(UnaryOperatorType.NullConditionalRewrap, arg)
				.WithILInstruction(inst)
				.WithRR(new ResolveResult(type));
		}

		protected internal override TranslatedExpression VisitNullableUnwrap(NullableUnwrap inst, TranslationContext context)
		{
			var arg = Translate(inst.Argument);
			if (inst.RefInput && !inst.RefOutput && arg.Expression is DirectionExpression dir) {
				arg = arg.UnwrapChild(dir.Expression);
			}
			return new UnaryOperatorExpression(UnaryOperatorType.NullConditional, arg)
				.WithILInstruction(inst)
				.WithRR(new ResolveResult(NullableType.GetUnderlyingType(arg.Type)));
		}

		protected internal override TranslatedExpression VisitDynamicConvertInstruction(DynamicConvertInstruction inst, TranslationContext context)
		{
			var operand = Translate(inst.Argument).ConvertTo(SpecialType.Dynamic, this);
			var result = new CastExpression(ConvertType(inst.Type), operand)
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(
					inst.Type, operand.ResolveResult,
					inst.IsExplicit ? Conversion.ExplicitDynamicConversion : Conversion.ImplicitDynamicConversion
				));
			result.Expression.AddAnnotation(inst.IsChecked ? AddCheckedBlocks.CheckedAnnotation : AddCheckedBlocks.UncheckedAnnotation);
			return result;
		}

		protected internal override TranslatedExpression VisitDynamicGetIndexInstruction(DynamicGetIndexInstruction inst, TranslationContext context)
		{
			var target = TranslateDynamicTarget(inst.Arguments[0], inst.ArgumentInfo[0]);
			var arguments = TranslateDynamicArguments(inst.Arguments.Skip(1), inst.ArgumentInfo.Skip(1)).ToList();
			return new IndexerExpression(target, arguments.Select(a => a.Expression))
				.WithILInstruction(inst)
				.WithRR(new DynamicInvocationResolveResult(target.ResolveResult, DynamicInvocationType.Indexing, arguments.Select(a => a.ResolveResult).ToArray()));
		}

		protected internal override TranslatedExpression VisitDynamicGetMemberInstruction(DynamicGetMemberInstruction inst, TranslationContext context)
		{
			var target = TranslateDynamicTarget(inst.Target, inst.TargetArgumentInfo);
			return new MemberReferenceExpression(target, inst.Name)
				.WithILInstruction(inst)
				.WithRR(new DynamicMemberResolveResult(target.ResolveResult, inst.Name));
		}

		protected internal override TranslatedExpression VisitDynamicInvokeConstructorInstruction(DynamicInvokeConstructorInstruction inst, TranslationContext context)
		{
			if (!(inst.ArgumentInfo[0].HasFlag(CSharpArgumentInfoFlags.IsStaticType) && IL.Transforms.TransformExpressionTrees.MatchGetTypeFromHandle(inst.Arguments[0], out var constructorType)))
				return ErrorExpression("Could not detect static type for DynamicInvokeConstructorInstruction");
			var arguments = TranslateDynamicArguments(inst.Arguments.Skip(1), inst.ArgumentInfo.Skip(1)).ToList();
			//var names = inst.ArgumentInfo.Skip(1).Select(a => a.Name).ToArray();
			return new ObjectCreateExpression(ConvertType(constructorType), arguments.Select(a => a.Expression))
				.WithILInstruction(inst).WithRR(new ResolveResult(constructorType));
		}

		protected internal override TranslatedExpression VisitDynamicInvokeMemberInstruction(DynamicInvokeMemberInstruction inst, TranslationContext context)
		{
			var target = TranslateDynamicTarget(inst.Arguments[0], inst.ArgumentInfo[0]);
			var arguments = TranslateDynamicArguments(inst.Arguments.Skip(1), inst.ArgumentInfo.Skip(1)).ToList();
			return new InvocationExpression(new MemberReferenceExpression(target, inst.Name, inst.TypeArguments.Select(ConvertType)), arguments.Select(a => a.Expression))
				.WithILInstruction(inst)
				.WithRR(new DynamicInvocationResolveResult(target.ResolveResult, DynamicInvocationType.Invocation, arguments.Select(a => a.ResolveResult).ToArray()));
		}

		protected internal override TranslatedExpression VisitDynamicInvokeInstruction(DynamicInvokeInstruction inst, TranslationContext context)
		{
			var target = TranslateDynamicTarget(inst.Arguments[0], inst.ArgumentInfo[0]);
			var arguments = TranslateDynamicArguments(inst.Arguments.Skip(1), inst.ArgumentInfo.Skip(1)).ToList();
			return new InvocationExpression(target, arguments.Select(a => a.Expression))
				.WithILInstruction(inst)
				.WithRR(new DynamicInvocationResolveResult(target.ResolveResult, DynamicInvocationType.Invocation, arguments.Select(a => a.ResolveResult).ToArray()));
		}

		TranslatedExpression TranslateDynamicTarget(ILInstruction inst, CSharpArgumentInfo argumentInfo)
		{
			Debug.Assert(!argumentInfo.HasFlag(CSharpArgumentInfoFlags.NamedArgument));
			Debug.Assert(!argumentInfo.HasFlag(CSharpArgumentInfoFlags.IsOut));

			if (argumentInfo.HasFlag(CSharpArgumentInfoFlags.IsStaticType) && IL.Transforms.TransformExpressionTrees.MatchGetTypeFromHandle(inst, out var callTargetType)) {
				return new TypeReferenceExpression(ConvertType(callTargetType))
					.WithoutILInstruction()
					.WithRR(new TypeResolveResult(callTargetType));
			}

			IType targetType = SpecialType.Dynamic;
			if (argumentInfo.HasFlag(CSharpArgumentInfoFlags.UseCompileTimeType)) {
				targetType = argumentInfo.CompileTimeType;
			}

			var translatedTarget = Translate(inst, targetType).ConvertTo(targetType, this);

			if (argumentInfo.HasFlag(CSharpArgumentInfoFlags.IsRef) && translatedTarget.Expression is DirectionExpression) {
				// (ref x).member => x.member
				translatedTarget = translatedTarget.UnwrapChild(((DirectionExpression)translatedTarget).Expression);
			}

			return translatedTarget;
		}

		IEnumerable<TranslatedExpression> TranslateDynamicArguments(IEnumerable<ILInstruction> arguments, IEnumerable<CSharpArgumentInfo> argumentInfo)
		{
			foreach (var (argument, info) in arguments.Zip(argumentInfo)) {
				yield return TranslateDynamicArgument(argument, info);
			}
		}

		TranslatedExpression TranslateDynamicArgument(ILInstruction argument, CSharpArgumentInfo info)
		{
			Debug.Assert(!info.HasFlag(CSharpArgumentInfoFlags.IsStaticType));

			IType typeHint = SpecialType.Dynamic;
			if (info.HasFlag(CSharpArgumentInfoFlags.UseCompileTimeType)) {
				typeHint = info.CompileTimeType;
			}
			var translatedExpression = Translate(argument, typeHint);
			if (!(typeHint.Equals(SpecialType.Dynamic) && translatedExpression.Type.Equals(SpecialType.NullType))) {
				translatedExpression = translatedExpression.ConvertTo(typeHint, this);
			}
			if (info.HasFlag(CSharpArgumentInfoFlags.IsOut)) {
				translatedExpression = ChangeDirectionExpressionTo(translatedExpression, ReferenceKind.Out);
			}
			if (info.HasFlag(CSharpArgumentInfoFlags.NamedArgument) && !string.IsNullOrWhiteSpace(info.Name)) {
				translatedExpression = new TranslatedExpression(new NamedArgumentExpression(info.Name, translatedExpression.Expression));
			}

			return translatedExpression;
		}

		internal static TranslatedExpression ChangeDirectionExpressionTo(TranslatedExpression input, ReferenceKind kind)
		{
			if (!(input.Expression is DirectionExpression dirExpr && input.ResolveResult is ByReferenceResolveResult brrr))
				return input;
			dirExpr.FieldDirection = (FieldDirection)kind;
			dirExpr.RemoveAnnotations<ByReferenceResolveResult>();
			if (brrr.ElementResult == null)
				brrr = new ByReferenceResolveResult(brrr.ElementType, kind);
			else
				brrr = new ByReferenceResolveResult(brrr.ElementResult, kind);
			dirExpr.AddAnnotation(brrr);
			return new TranslatedExpression(dirExpr);
		}

		protected internal override TranslatedExpression VisitDynamicSetIndexInstruction(DynamicSetIndexInstruction inst, TranslationContext context)
		{
			Debug.Assert(inst.Arguments.Count >= 3);
			var target = TranslateDynamicTarget(inst.Arguments[0], inst.ArgumentInfo[0]);
			var arguments = TranslateDynamicArguments(inst.Arguments.Skip(1), inst.ArgumentInfo.Skip(1)).ToList();
			var value = new TranslatedExpression(arguments.Last());
			var indexer = new IndexerExpression(target, arguments.SkipLast(1).Select(a => a.Expression))
				.WithoutILInstruction()
				.WithRR(new DynamicInvocationResolveResult(target.ResolveResult, DynamicInvocationType.Indexing, arguments.SkipLast(1).Select(a => a.ResolveResult).ToArray()));
			return Assignment(indexer, value).WithILInstruction(inst);
		}

		protected internal override TranslatedExpression VisitDynamicSetMemberInstruction(DynamicSetMemberInstruction inst, TranslationContext context)
		{
			var target = TranslateDynamicTarget(inst.Target, inst.TargetArgumentInfo);
			var value = TranslateDynamicArgument(inst.Value, inst.ValueArgumentInfo);
			var member = new MemberReferenceExpression(target, inst.Name)
				.WithoutILInstruction()
				.WithRR(new DynamicMemberResolveResult(target.ResolveResult, inst.Name));
			return Assignment(member, value).WithILInstruction(inst);
		}

		protected internal override TranslatedExpression VisitDynamicBinaryOperatorInstruction(DynamicBinaryOperatorInstruction inst, TranslationContext context)
		{
			switch (inst.Operation) {
				case ExpressionType.Add:
				case ExpressionType.AddAssign:
					return CreateBinaryOperator(BinaryOperatorType.Add, isChecked: inst.BinderFlags.HasFlag(CSharpBinderFlags.CheckedContext));
				case ExpressionType.AddChecked:
				case ExpressionType.AddAssignChecked:
					return CreateBinaryOperator(BinaryOperatorType.Add, isChecked: true);
				case ExpressionType.Subtract:
				case ExpressionType.SubtractAssign:
					return CreateBinaryOperator(BinaryOperatorType.Subtract, isChecked: inst.BinderFlags.HasFlag(CSharpBinderFlags.CheckedContext));
				case ExpressionType.SubtractChecked:
				case ExpressionType.SubtractAssignChecked:
					return CreateBinaryOperator(BinaryOperatorType.Subtract, isChecked: true);
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyAssign:
					return CreateBinaryOperator(BinaryOperatorType.Multiply, isChecked: inst.BinderFlags.HasFlag(CSharpBinderFlags.CheckedContext));
				case ExpressionType.MultiplyChecked:
				case ExpressionType.MultiplyAssignChecked:
					return CreateBinaryOperator(BinaryOperatorType.Multiply, isChecked: true);
				case ExpressionType.Divide:
				case ExpressionType.DivideAssign:
					return CreateBinaryOperator(BinaryOperatorType.Divide);
				case ExpressionType.Modulo:
				case ExpressionType.ModuloAssign:
					return CreateBinaryOperator(BinaryOperatorType.Modulus);
				case ExpressionType.Equal:
					return CreateBinaryOperator(BinaryOperatorType.Equality);
				case ExpressionType.NotEqual:
					return CreateBinaryOperator(BinaryOperatorType.InEquality);
				case ExpressionType.LessThan:
					return CreateBinaryOperator(BinaryOperatorType.LessThan);
				case ExpressionType.LessThanOrEqual:
					return CreateBinaryOperator(BinaryOperatorType.LessThanOrEqual);
				case ExpressionType.GreaterThan:
					return CreateBinaryOperator(BinaryOperatorType.GreaterThan);
				case ExpressionType.GreaterThanOrEqual:
					return CreateBinaryOperator(BinaryOperatorType.GreaterThanOrEqual);
				case ExpressionType.And:
				case ExpressionType.AndAssign:
					return CreateBinaryOperator(BinaryOperatorType.BitwiseAnd);
				case ExpressionType.Or:
				case ExpressionType.OrAssign:
					return CreateBinaryOperator(BinaryOperatorType.BitwiseOr);
				case ExpressionType.ExclusiveOr:
				case ExpressionType.ExclusiveOrAssign:
					return CreateBinaryOperator(BinaryOperatorType.ExclusiveOr);
				case ExpressionType.LeftShift:
				case ExpressionType.LeftShiftAssign:
					return CreateBinaryOperator(BinaryOperatorType.ShiftLeft);
				case ExpressionType.RightShift:
				case ExpressionType.RightShiftAssign:
					return CreateBinaryOperator(BinaryOperatorType.ShiftRight);
				default:
					return base.VisitDynamicBinaryOperatorInstruction(inst, context);
			}

			TranslatedExpression CreateBinaryOperator(BinaryOperatorType operatorType, bool? isChecked = null)
			{
				var left = TranslateDynamicArgument(inst.Left, inst.LeftArgumentInfo);
				var right = TranslateDynamicArgument(inst.Right, inst.RightArgumentInfo);
				var boe = new BinaryOperatorExpression(left.Expression, operatorType, right.Expression);
				if (isChecked == true)
					boe.AddAnnotation(AddCheckedBlocks.CheckedAnnotation);
				else if (isChecked == false)
					boe.AddAnnotation(AddCheckedBlocks.UncheckedAnnotation);
				return boe.WithILInstruction(inst).WithRR(new ResolveResult(SpecialType.Dynamic));
			}
		}

		protected internal override TranslatedExpression VisitDynamicLogicOperatorInstruction(DynamicLogicOperatorInstruction inst, TranslationContext context)
		{
			BinaryOperatorType operatorType;
			if (inst.Operation == ExpressionType.AndAlso) {
				operatorType = BinaryOperatorType.ConditionalAnd;
			} else if (inst.Operation == ExpressionType.OrElse) {
				operatorType = BinaryOperatorType.ConditionalOr;
			} else {
				Debug.Fail("Unknown operation for DynamicLogicOperatorInstruction");
				return base.VisitDynamicLogicOperatorInstruction(inst, context);
			}
			var left = TranslateDynamicArgument(inst.Left, inst.LeftArgumentInfo);
			var right = TranslateDynamicArgument(inst.Right, inst.RightArgumentInfo);
			var boe = new BinaryOperatorExpression(left.Expression, operatorType, right.Expression);
			return boe.WithILInstruction(inst).WithRR(new ResolveResult(SpecialType.Dynamic));
		}

		protected internal override TranslatedExpression VisitDynamicUnaryOperatorInstruction(DynamicUnaryOperatorInstruction inst, TranslationContext context)
		{
			switch (inst.Operation) {
				case ExpressionType.Not:
					return CreateUnaryOperator(UnaryOperatorType.Not);
				case ExpressionType.Decrement:
					return CreateUnaryOperator(UnaryOperatorType.Decrement, isChecked: inst.BinderFlags.HasFlag(CSharpBinderFlags.CheckedContext));
				case ExpressionType.Increment:
					return CreateUnaryOperator(UnaryOperatorType.Increment, isChecked: inst.BinderFlags.HasFlag(CSharpBinderFlags.CheckedContext));
				case ExpressionType.Negate:
					return CreateUnaryOperator(UnaryOperatorType.Minus, isChecked: inst.BinderFlags.HasFlag(CSharpBinderFlags.CheckedContext));
				case ExpressionType.NegateChecked:
					return CreateUnaryOperator(UnaryOperatorType.Minus, isChecked: true);
				case ExpressionType.UnaryPlus:
					return CreateUnaryOperator(UnaryOperatorType.Plus, isChecked: inst.BinderFlags.HasFlag(CSharpBinderFlags.CheckedContext));
				case ExpressionType.IsTrue:
					var operand = TranslateDynamicArgument(inst.Operand, inst.OperandArgumentInfo);
					Expression expr;
					if (inst.SlotInfo == IfInstruction.ConditionSlot) {
						// We rely on the context implicitly invoking "operator true".
						expr = new UnaryOperatorExpression(UnaryOperatorType.IsTrue, operand);
					} else {
						// Create a dummy conditional to ensure "operator true" will be invoked.
						expr = new ConditionalExpression(operand, new PrimitiveExpression(true), new PrimitiveExpression(false));
					}
					return expr.WithILInstruction(inst)
						.WithRR(new ResolveResult(compilation.FindType(KnownTypeCode.Boolean)));
				case ExpressionType.IsFalse:
					operand = TranslateDynamicArgument(inst.Operand, inst.OperandArgumentInfo);
					// Create a dummy conditional to ensure "operator false" will be invoked.
					expr = new ConditionalExpression(operand, new PrimitiveExpression(false), new PrimitiveExpression(true));
					return expr.WithILInstruction(inst)
						.WithRR(new ResolveResult(compilation.FindType(KnownTypeCode.Boolean)));
				default:
					return base.VisitDynamicUnaryOperatorInstruction(inst, context);
			}

			TranslatedExpression CreateUnaryOperator(UnaryOperatorType operatorType, bool? isChecked = null)
			{
				var operand = TranslateDynamicArgument(inst.Operand, inst.OperandArgumentInfo);
				var uoe = new UnaryOperatorExpression(operatorType, operand.Expression);
				if (isChecked == true)
					uoe.AddAnnotation(AddCheckedBlocks.CheckedAnnotation);
				else if (isChecked == false)
					uoe.AddAnnotation(AddCheckedBlocks.UncheckedAnnotation);
				return uoe.WithILInstruction(inst).WithRR(new ResolveResult(SpecialType.Dynamic));
			}
		}

		protected internal override TranslatedExpression VisitDynamicCompoundAssign(DynamicCompoundAssign inst, TranslationContext context)
		{
			ExpressionWithResolveResult target;
			if (inst.TargetKind == CompoundTargetKind.Address) {
				target = LdObj(inst.Target, SpecialType.Dynamic);
			} else {
				target = TranslateDynamicArgument(inst.Target, inst.TargetArgumentInfo);
			}
			var value = TranslateDynamicArgument(inst.Value, inst.ValueArgumentInfo);

			var ae = new AssignmentExpression(target, AssignmentExpression.GetAssignmentOperatorTypeFromExpressionType(inst.Operation).Value, value);
			if (inst.BinderFlags.HasFlag(CSharpBinderFlags.CheckedContext))
				ae.AddAnnotation(AddCheckedBlocks.CheckedAnnotation);
			else
				ae.AddAnnotation(AddCheckedBlocks.UncheckedAnnotation);
			return ae.WithILInstruction(inst)
				.WithRR(new OperatorResolveResult(SpecialType.Dynamic, inst.Operation, new[] { target.ResolveResult, value.ResolveResult }));
		}

		protected internal override TranslatedExpression VisitLdFtn(LdFtn inst, TranslationContext context)
		{
			ExpressionWithResolveResult delegateRef = new CallBuilder(this, typeSystem, settings).BuildMethodReference(inst.Method, isVirtual: false);
			return new InvocationExpression(new IdentifierExpression("__ldftn"), delegateRef)
				.WithRR(new ResolveResult(compilation.FindType(KnownTypeCode.IntPtr)))
				.WithILInstruction(inst);
		}

		protected internal override TranslatedExpression VisitLdVirtFtn(LdVirtFtn inst, TranslationContext context)
		{
			ExpressionWithResolveResult delegateRef = new CallBuilder(this, typeSystem, settings).BuildMethodReference(inst.Method, isVirtual: true);
			return new InvocationExpression(new IdentifierExpression("__ldvirtftn"), delegateRef)
				.WithRR(new ResolveResult(compilation.FindType(KnownTypeCode.IntPtr)))
				.WithILInstruction(inst);
		}

		protected internal override TranslatedExpression VisitCallIndirect(CallIndirect inst, TranslationContext context)
		{
			if (inst.IsInstance) {
				return ErrorExpression("calli with instance method signature not supportd");
			}
			var ty = new FunctionPointerType();
			if (inst.CallingConvention != System.Reflection.Metadata.SignatureCallingConvention.Default) {
				ty.CallingConvention = inst.CallingConvention.ToString().ToLowerInvariant();
			}
			foreach (var parameterType in inst.ParameterTypes) {
				ty.TypeArguments.Add(astBuilder.ConvertType(parameterType));
			}
			ty.TypeArguments.Add(astBuilder.ConvertType(inst.ReturnType));
			var functionPointer = Translate(inst.FunctionPointer);
			var invocation = new InvocationExpression(new CastExpression(ty, functionPointer));
			foreach (var (arg, paramType) in inst.Arguments.Zip(inst.ParameterTypes)) {
				invocation.Arguments.Add(Translate(arg, typeHint: paramType).ConvertTo(paramType, this, allowImplicitConversion: true));
			}
			return invocation.WithRR(new ResolveResult(inst.ReturnType)).WithILInstruction(inst);
		}

		protected internal override TranslatedExpression VisitDeconstructInstruction(DeconstructInstruction inst, TranslationContext context)
		{
			IType rhsType = inst.Pattern.Variable.Type;
			var rhs = Translate(inst.Pattern.TestedOperand, rhsType);
			rhs = rhs.ConvertTo(rhsType, this); // TODO allowImplicitConversion
			var assignments = inst.Assignments.Instructions;
			int assignmentPos = 0;
			var inits = inst.Init;
			int initPos = 0;

			Dictionary<ILVariable, ILVariable> conversionMapping = new Dictionary<ILVariable, ILVariable>();

			foreach (var conv in inst.Conversions.Instructions) {
				if (!DeconstructInstruction.IsConversionStLoc(conv, out var outputVariable, out var inputVariable))
					continue;
				conversionMapping.Add(inputVariable, outputVariable);
			}


			var lhs = ConstructTuple(inst.Pattern);
			return new AssignmentExpression(lhs, rhs)
				.WithILInstruction(inst)
				.WithRR(new ResolveResult(compilation.FindType(KnownTypeCode.Void)));

			TupleExpression ConstructTuple(MatchInstruction matchInstruction)
			{
				var expr = new TupleExpression();
				foreach (var subPattern in matchInstruction.SubPatterns.Cast<MatchInstruction>()) {
					if (subPattern.IsVar) {
						if (subPattern.HasDesignator) {
							if (!conversionMapping.TryGetValue(subPattern.Variable, out ILVariable value)) {
								value = subPattern.Variable;
							}
							expr.Elements.Add(ConstructAssignmentTarget(assignments[assignmentPos], value));
							assignmentPos++;
						} else
							expr.Elements.Add(new IdentifierExpression("_"));
					} else {
						expr.Elements.Add(ConstructTuple(subPattern));
					}
				}
				return expr;
			}

			TranslatedExpression ConstructAssignmentTarget(ILInstruction assignment, ILVariable value)
			{
				switch (assignment) {
					case StLoc stloc:
						Debug.Assert(stloc.Value.MatchLdLoc(value));
						break;
					case CallInstruction call:
						for (int i = 0; i < call.Arguments.Count - 1; i++) {
							ReplaceAssignmentTarget(call.Arguments[i]);
						}
						Debug.Assert(call.Arguments.Last().MatchLdLoc(value));
						break;
					case StObj stobj:
						var target = stobj.Target;
						while (target.MatchLdFlda(out var nestedTarget, out _))
							target = nestedTarget;
						ReplaceAssignmentTarget(target);
						Debug.Assert(stobj.Value.MatchLdLoc(value));
						break;
					default:
						throw new NotSupportedException();
				}
				var expr = Translate(assignment);
				return expr.UnwrapChild(((AssignmentExpression)expr).Left);
			}

			void ReplaceAssignmentTarget(ILInstruction target)
			{
				if (target.MatchLdLoc(out var v)
					&& v.Kind == VariableKind.DeconstructionInitTemporary)
				{
					Debug.Assert(inits[initPos].Variable == v);
					target.ReplaceWith(inits[initPos].Value);
					initPos++;
				}
			}
		}

		protected internal override TranslatedExpression VisitInvalidBranch(InvalidBranch inst, TranslationContext context)
		{
			string message = "Error";
			if (inst.StartILOffset != 0) {
				message += $" near IL_{inst.StartILOffset:x4}";
			}
			if (!string.IsNullOrEmpty(inst.Message)) {
				message += ": " + inst.Message;
			}
			return ErrorExpression(message);
		}

		protected internal override TranslatedExpression VisitInvalidExpression(InvalidExpression inst, TranslationContext context)
		{
			string message = "Error";
			if (inst.StartILOffset != 0) {
				message += $" near IL_{inst.StartILOffset:x4}";
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
