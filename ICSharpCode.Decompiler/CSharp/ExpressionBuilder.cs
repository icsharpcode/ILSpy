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

using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ExpressionType = System.Linq.Expressions.ExpressionType;

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
	///      * If the IL instruction evaluates to a non-integer type, the C# type of the resulting expression shall match the IL stack type,
	///        and the evaluated values shall be the same.
	/// </remarks>
	class ExpressionBuilder : ILVisitor<TranslationContext, TranslatedExpression>
	{
		readonly IDecompilerTypeSystem typeSystem;
		readonly ITypeResolveContext decompilationContext;
		internal readonly ICompilation compilation;
		internal readonly CSharpResolver resolver;
		readonly TypeSystemAstBuilder astBuilder;
		
		public ExpressionBuilder(IDecompilerTypeSystem typeSystem, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext != null);
			this.typeSystem = typeSystem;
			this.decompilationContext = decompilationContext;
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
		
		public TranslatedExpression Translate(ILInstruction inst, IType typeHint = null)
		{
			Debug.Assert(inst != null);
			TranslationContext context = new TranslationContext {
				TypeHint = typeHint ?? SpecialType.UnknownType
			};
			var cexpr = inst.AcceptVisitor(this, context);
			#if DEBUG
			if (inst.ResultType != StackType.Void && cexpr.Type.Kind != TypeKind.Unknown) {
				if (inst.ResultType.IsIntegerType()) {
					Debug.Assert(cexpr.Type.GetStackType().IsIntegerType(), "IL instructions of integer type must convert into C# expressions of integer type");
					Debug.Assert(cexpr.Type.GetSign() != Sign.None, "Must have a sign specified for zero/sign-extension");
				} else {
					Debug.Assert(cexpr.Type.GetStackType() == inst.ResultType);
				}
			}
			#endif
			return cexpr;
		}
		
		public TranslatedExpression TranslateCondition(ILInstruction condition)
		{
			var expr = Translate(condition, compilation.FindType(KnownTypeCode.Boolean));
			return expr.ConvertToBoolean(this);
		}
		
		ExpressionWithResolveResult ConvertVariable(ILVariable variable)
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
			return HandleCallInstruction(inst);
		}
		
		protected internal override TranslatedExpression VisitNewArr(NewArr inst, TranslationContext context)
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
			return new NullReferenceExpression()
				.WithILInstruction(inst)
				.WithRR(new ConstantResolveResult(SpecialType.NullType, null));
		}
		
		protected internal override TranslatedExpression VisitDefaultValue(DefaultValue inst, TranslationContext context)
		{
			return new DefaultValueExpression(ConvertType(inst.Type))
				.WithILInstruction(inst)
				.WithRR(new ConstantResolveResult(inst.Type, null));
		}
		
		protected internal override TranslatedExpression VisitSizeOf(SizeOf inst, TranslationContext context)
		{
			return new SizeOfExpression(ConvertType(inst.Type))
				.WithILInstruction(inst)
				.WithRR(new SizeOfResolveResult(compilation.FindType(KnownTypeCode.Int32), inst.Type, null));
		}
		
		protected internal override TranslatedExpression VisitLdTypeToken(LdTypeToken inst, TranslationContext context)
		{
			return new TypeOfExpression(ConvertType(inst.Type)).Member("TypeHandle")
				.WithILInstruction(inst)
				.WithRR(new TypeOfResolveResult(compilation.FindType(new TopLevelTypeName("System", "RuntimeTypeHandle")), inst.Type));
		}
		
		protected internal override TranslatedExpression VisitLogicNot(LogicNot inst, TranslationContext context)
		{
			return LogicNot(TranslateCondition(inst.Argument)).WithILInstruction(inst);
		}
		
		protected internal override TranslatedExpression VisitBitNot(BitNot inst, TranslationContext context)
		{
			var argument = Translate(inst.Argument);
			
			if (argument.Type.GetStackType().GetSize() < inst.ResultType.GetSize()
			    || argument.Type.Kind == TypeKind.Enum && argument.Type.IsSmallIntegerType())
			{
				// Argument is undersized (even after implicit integral promotion to I4)
				// -> we need to perform sign/zero-extension before the BitNot.
				// Same if the argument is an enum based on a small integer type
				// (those don't undergo numeric promotion in C# the way non-enum small integer types do).
				argument = argument.ConvertTo(compilation.FindType(inst.ResultType.ToKnownTypeCode(argument.Type.GetSign())), this);
			}
			
			var type = argument.Type.GetDefinition();
			if (type != null) {
				// Handle those types that don't support operator ~
				// Note that it's OK to use a type that's larger than necessary.
				switch (type.KnownTypeCode) {
					case KnownTypeCode.Boolean:
					case KnownTypeCode.Char:
						argument = argument.ConvertTo(compilation.FindType(KnownTypeCode.UInt32), this);
						break;
					case KnownTypeCode.IntPtr:
						argument = argument.ConvertTo(compilation.FindType(KnownTypeCode.Int64), this);
						break;
					case KnownTypeCode.UIntPtr:
						argument = argument.ConvertTo(compilation.FindType(KnownTypeCode.UInt64), this);
						break;
				}
			}
			
			return new UnaryOperatorExpression(UnaryOperatorType.BitNot, argument)
				.WithRR(resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, argument.ResolveResult))
				.WithILInstruction(inst);
		}
		
		ExpressionWithResolveResult LogicNot(TranslatedExpression expr)
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
			left = PrepareArithmeticArgument(left, inst.Left.ResultType, inst.Sign);
			right = PrepareArithmeticArgument(right, inst.Right.ResultType, inst.Sign);
			
			var rr = resolverWithOverflowCheck.ResolveBinaryOperator(op, left.ResolveResult, right.ResolveResult);
			if (rr.IsError || rr.Type.GetStackType() != inst.ResultType
			    || !IsCompatibleWithSign(left.Type, inst.Sign) || !IsCompatibleWithSign(right.Type, inst.Sign))
			{
				// Left and right operands are incompatible, so convert them to a common type
				StackType targetStackType = inst.ResultType == StackType.I ? StackType.I8 : inst.ResultType;
				IType targetType = compilation.FindType(targetStackType.ToKnownTypeCode(inst.Sign));
				left = left.ConvertTo(targetType, this);
				right = right.ConvertTo(targetType, this);
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
		TranslatedExpression PrepareArithmeticArgument(TranslatedExpression arg, StackType argStackType, Sign sign)
		{
			if (argStackType.IsIntegerType() && argStackType.GetSize() < arg.Type.GetSize()) {
				// If the argument is oversized (needs truncation to match stack size of its ILInstruction),
				// perform the truncation now.
				arg = arg.ConvertTo(compilation.FindType(argStackType.ToKnownTypeCode(sign)), this);
			}
			if (arg.Type.GetStackType() == StackType.I) {
				// None of the operators we might want to apply are supported by IntPtr/UIntPtr.
				// Also, pointer arithmetic has different semantics (works in number of elements, not bytes).
				// So any inputs of size StackType.I must be converted to long/ulong.
				arg = arg.ConvertTo(compilation.FindType(StackType.I8.ToKnownTypeCode(sign)), this);
			}
			return arg;
		}
		
		/// <summary>
		/// Gets whether <paramref name="type"/> has the specified <paramref name="sign"/>.
		/// If <paramref name="sign"/> is None, always returns true.
		/// </summary>
		static bool IsCompatibleWithSign(IType type, Sign sign)
		{
			return sign == Sign.None || type.GetSign() == sign;
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
			
			IType targetType;
			if (inst.ResultType == StackType.I4)
				targetType = compilation.FindType(inst.Sign == Sign.Unsigned ? KnownTypeCode.UInt32 : KnownTypeCode.Int32);
			else
				targetType = compilation.FindType(inst.Sign == Sign.Unsigned ? KnownTypeCode.UInt64 : KnownTypeCode.Int64);
			left = left.ConvertTo(targetType, this);
			
			// Shift operators in C# always expect type 'int' on the right-hand-side
			right = right.ConvertTo(compilation.FindType(KnownTypeCode.Int32), this);
			
			TranslatedExpression result = new BinaryOperatorExpression(left.Expression, op, right.Expression)
				.WithILInstruction(inst)
				.WithRR(resolver.ResolveBinaryOperator(op, left.ResolveResult, right.ResolveResult));
			if (inst.ResultType == StackType.I) {
				// C# doesn't have shift operators for IntPtr, so we first shifted a long/ulong,
				// and now have to case back down to IntPtr/UIntPtr:
				result = result.ConvertTo(compilation.FindType(inst.Sign == Sign.Unsigned ? KnownTypeCode.UIntPtr : KnownTypeCode.IntPtr), this);
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
			value = PrepareArithmeticArgument(value, inst.Value.ResultType, inst.Sign);
			
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
			StackType inputStackType = inst.Argument.ResultType;
			// Note: we're dealing with two conversions here:
			// a) the implicit conversion from `arg.Type` to `inputStackType`
			//    (due to the ExpressionBuilder post-condition being flexible with regards to the integer type width)
			//    If this is a widening conversion, I'm calling the argument C# type "oversized".
			//    If this is a narrowing conversion, I'm calling the argument C# type "undersized".
			// b) the actual conversion instruction from `inputStackType` to `inst.TargetType`
			
			// Also, we need to be very careful with regards to the conversions we emit:
			// In C#, zero vs. sign-extension depends on the input type,
			// but in the ILAst conv instruction it depends on the output type.
			// However, in the conv.ovf instructions, the .NET runtime behavior seems to depend on the input type,
			// in violation of the ECMA-335 spec!
			
			if (inst.CheckForOverflow || inst.Kind == ConversionKind.IntToFloat) {
				// We need to first convert the argument to the expected sign.
				// We also need to perform any input narrowing conversion so that it doesn't get mixed up with the overflow check.
				Debug.Assert(inst.InputSign != Sign.None);
				if (arg.Type.GetSize() > inputStackType.GetSize() || arg.Type.GetSign() != inst.InputSign) {
					arg = arg.ConvertTo(compilation.FindType(inputStackType.ToKnownTypeCode(inst.InputSign)), this);
				}
				// Because casts with overflow check match C# semantics (zero/sign-extension depends on source type),
				// we can just directly cast to the target type.
				return arg.ConvertTo(compilation.FindType(inst.TargetType.ToKnownTypeCode()), this, true)
					.WithILInstruction(inst);
			}
			
			switch (inst.Kind) {
				case ConversionKind.StopGCTracking:
					if (arg.Type.Kind == TypeKind.ByReference) {
						// cast to corresponding pointer type:
						var pointerType = new PointerType(((ByReferenceType)arg.Type).ElementType);
						return arg.ConvertTo(pointerType, this).WithILInstruction(inst);
					} else {
						Debug.Fail("ConversionKind.StopGCTracking should only be used with managed references");
						goto default;
					}
				case ConversionKind.SignExtend:
					// We just need to ensure the input type before the conversion is signed.
					// Also, if the argument was translated into an oversized C# type,
					// we need to perform the truncatation to the input stack type.
					if (arg.Type.GetSign() != Sign.Signed || arg.Type.GetSize() > inputStackType.GetSize()) {
						// Note that an undersized C# type is handled just fine:
						// If it is unsigned we'll zero-extend it to the width of the inputStackType here,
						// and it is signed we just combine the two sign-extensions into a single sign-extending conversion.
						arg = arg.ConvertTo(compilation.FindType(inputStackType.ToKnownTypeCode(Sign.Signed)), this);
					}
					// Then, we can just return the argument as-is: the ExpressionBuilder post-condition allows us
					// to force our parent instruction to handle the actual sign-extension conversion.
					// (our caller may have more information to pick a better fitting target type)
					return arg.WithILInstruction(inst);
				case ConversionKind.ZeroExtend:
					// If overflow check cannot fail, handle this just like sign extension (except for swapped signs)
					if (arg.Type.GetSign() != Sign.Unsigned || arg.Type.GetSize() > inputStackType.GetSize()) {
						arg = arg.ConvertTo(compilation.FindType(inputStackType.ToKnownTypeCode(Sign.Unsigned)), this);
					}
					return arg.WithILInstruction(inst);
				case ConversionKind.Nop:
					// no need to generate any C# code for a nop conversion
					return arg.WithILInstruction(inst);
				case ConversionKind.Truncate:
					// Note: there are three sizes involved here:
					// A = arg.Type.GetSize()
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
						
						if (arg.Type.GetSize() <= inst.TargetType.GetSize() && arg.Type.GetSign() == inst.TargetType.GetSign()) {
							// There's no actual truncation involved, and the result of the Conv instruction is extended
							// the same way as the original instruction
							// -> we can return arg directly
							return arg.WithILInstruction(inst);
						} else {
							// We need to actually truncate; *or* we need to change the sign for the remaining extension to I4.
							goto default; // Emit simple cast to inst.TargetType
						}
					} else {
						Debug.Assert(inst.TargetType.GetSize() == inst.ResultType.GetSize());
						// For non-small integer types, we can let the whole unchecked truncation
						// get handled by our caller (using the ExpressionBuilder post-condition).
						
						// Case 4 (left-over extension from implicit conversion) can also be handled by our caller.
						return arg.WithILInstruction(inst);
					}
				default:
					return arg.ConvertTo(compilation.FindType(inst.TargetType.ToKnownTypeCode()), this, inst.CheckForOverflow)
						.WithILInstruction(inst);
			}
		}

		protected internal override TranslatedExpression VisitCall(Call inst, TranslationContext context)
		{
			return HandleCallInstruction(inst);
		}
		
		protected internal override TranslatedExpression VisitCallVirt(CallVirt inst, TranslationContext context)
		{
			return HandleCallInstruction(inst);
		}

		TranslatedExpression HandleDelegateConstruction(CallInstruction inst)
		{
			ILInstruction func = inst.Arguments[1];
			IMethod method;
			switch (func.OpCode) {
				case OpCode.LdFtn:
					method = ((LdFtn)func).Method;
					break;
				case OpCode.LdVirtFtn:
					method = ((LdVirtFtn)func).Method;
					break;
				default:
					method = (IMethod)typeSystem.Resolve(((ILFunction)func).Method);
					break;
			}
			var target = TranslateTarget(method, inst.Arguments[0], func.OpCode == OpCode.LdFtn);
			
			var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentAssembly);
			var or = new OverloadResolution(resolver.Compilation, method.Parameters.SelectArray(p => new TypeResolveResult(p.Type)));
			var result = lookup.Lookup(target.ResolveResult, method.Name, method.TypeArguments, true) as MethodGroupResolveResult;
			
			if (result == null) {
				target = target.ConvertTo(method.DeclaringType, this);
			} else {
				or.AddMethodLists(result.MethodsGroupedByDeclaringType.ToArray());
				if (or.BestCandidateErrors != OverloadResolutionErrors.None || !IsAppropriateCallTarget(method, or.BestCandidate, func.OpCode == OpCode.LdVirtFtn))
					target = target.ConvertTo(method.DeclaringType, this);
			}
			
			var mre = new MemberReferenceExpression(target, method.Name);
			mre.TypeArguments.AddRange(method.TypeArguments.Select(a => ConvertType(a)));
			var oce = new ObjectCreateExpression(ConvertType(inst.Method.DeclaringType), mre)
//				.WithAnnotation(new DelegateConstruction.Annotation(func.OpCode == OpCode.LdVirtFtn, target, method.Name))
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(
					inst.Method.DeclaringType,
					new MemberResolveResult(target.ResolveResult, method),
					// TODO handle extension methods capturing the first argument
					Conversion.MethodGroupConversion(method, func.OpCode == OpCode.LdVirtFtn, false)));
			
			if (func is ILFunction) {
				return TranslateFunction(oce, target, (ILFunction)func);
			} else {
				return oce;
			}
		}

		TranslatedExpression TranslateFunction(TranslatedExpression objectCreateExpression, TranslatedExpression target, ILFunction function)
		{
			var method = typeSystem.Resolve(function.Method)?.MemberDefinition as IMethod;
			Debug.Assert(method != null);

			// Create AnonymousMethodExpression and prepare parameters
			AnonymousMethodExpression ame = new AnonymousMethodExpression();
			ame.Parameters.AddRange(MakeParameters(method, function));
			ame.HasParameterList = true;
			StatementBuilder builder = new StatementBuilder(typeSystem.GetSpecializingTypeSystem(new SimpleTypeResolveContext(method)), this.decompilationContext, method);
			var body = builder.ConvertAsBlock(function.Body);
			bool isLambda = false;
			bool isMultiLineLambda = false;
			
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
			} else if (!expectedType.ContainsAnonymousType()) {
				replacement = replacement.CastTo(ConvertType(expectedType));
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
				if (parameter.Type.ContainsAnonymousType())
					pd.Type = null;
				ILVariable v;
				if (variables.TryGetValue(i, out v))
					pd.AddAnnotation(new ILVariableResolveResult(v, method.Parameters[i].Type));
				yield return pd;
				i++;
			}
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
		
		TranslatedExpression HandleCallInstruction(CallInstruction inst)
		{
			IMethod method = inst.Method;
			// Used for Call, CallVirt and NewObj
			TranslatedExpression target;
			if (inst.OpCode == OpCode.NewObj) {
				if (IL.Transforms.DelegateConstruction.IsDelegateConstruction((NewObj)inst, true)) {
					return HandleDelegateConstruction(inst);
				}
				target = default(TranslatedExpression); // no target
			} else {
				target = TranslateTarget(method, inst.Arguments.FirstOrDefault(), inst.OpCode == OpCode.Call);
			}
			
			int firstParamIndex = (method.IsStatic || inst.OpCode == OpCode.NewObj) ? 0 : 1;
			
			// Translate arguments to the expected parameter types
			TranslatedExpression[] arguments = new TranslatedExpression[inst.Method.Parameters.Count];
			Debug.Assert(inst.Arguments.Count == firstParamIndex + inst.Method.Parameters.Count);
			for (int i = 0; i < arguments.Length; i++) {
				var parameter = method.Parameters[i];
				arguments[i] = Translate(inst.Arguments[firstParamIndex + i]);
				// HACK : do not add casts to anonymous types, in order to avoid compile errors.
				// Ideally we should be able to remove casts to any type, if not explicitly required.
				if (!parameter.Type.ContainsAnonymousType())
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

			var argumentResolveResults = arguments.Select(arg => arg.ResolveResult).ToList();

			ResolveResult rr;
			if (inst.Method.IsAccessor)
				rr = new MemberResolveResult(target.ResolveResult, method.AccessorOwner);
			else
				rr = new CSharpInvocationResolveResult(target.ResolveResult, method, argumentResolveResults);
			
			if (inst.OpCode == OpCode.NewObj) {
				var argumentExpressions = arguments.SelectArray(arg => arg.Expression);
				if (method.DeclaringType.IsAnonymousType()) {
					AnonymousTypeCreateExpression atce = new AnonymousTypeCreateExpression();
					var parameters = inst.Method.Parameters;
					if (CanInferAnonymousTypePropertyNamesFromArguments(argumentExpressions, parameters)) {
						atce.Initializers.AddRange(argumentExpressions);
					} else {
						for (int i = 0; i < argumentExpressions.Length; i++) {
							atce.Initializers.Add(
								new NamedExpression {
									Name = parameters[i].Name,
									Expression = argumentExpressions[i]
								});
						}
					}
					return atce
						.WithILInstruction(inst)
						.WithRR(rr);
				} else {
					return new ObjectCreateExpression(ConvertType(inst.Method.DeclaringType), argumentExpressions)
						.WithILInstruction(inst).WithRR(rr);
				}
			} else {
				Expression expr;
				int allowedParamCount = (method.ReturnType.IsKnownType(KnownTypeCode.Void) ? 1 : 0);
				if (method.IsAccessor && (method.AccessorOwner.SymbolKind == SymbolKind.Indexer || method.Parameters.Count == allowedParamCount)) {
					expr = HandleAccessorCall(inst, target, method, arguments.ToList());
				} else {
					var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentAssembly);
					var or = new OverloadResolution(resolver.Compilation, arguments.Select(a => a.ResolveResult).ToArray());
					var result = lookup.Lookup(target.ResolveResult, method.Name, method.TypeArguments, true) as MethodGroupResolveResult;
					
					if (result == null) {
						target = target.ConvertTo(method.DeclaringType, this);
					} else {
						or.AddMethodLists(result.MethodsGroupedByDeclaringType.ToArray());
						if (or.BestCandidateErrors != OverloadResolutionErrors.None || !IsAppropriateCallTarget(method, or.BestCandidate, inst.OpCode == OpCode.CallVirt))
							target = target.ConvertTo(method.DeclaringType, this);
					}
					
					Expression targetExpr = target.Expression;
					string methodName = method.Name;
					// HACK : convert this.Dispose() to ((IDisposable)this).Dispose(), if Dispose is an explicitly implemented interface method.
					if (inst.Method.IsExplicitInterfaceImplementation && targetExpr is ThisReferenceExpression) {
						targetExpr = targetExpr.CastTo(ConvertType(method.ImplementedInterfaceMembers[0].DeclaringType));
						methodName = method.ImplementedInterfaceMembers[0].Name;
					}
					var mre = new MemberReferenceExpression(targetExpr, methodName);
					if (!method.TypeArguments.Any(t => t.ContainsAnonymousType()))
						mre.TypeArguments.AddRange(method.TypeArguments.Select(a => ConvertType(a)));
					var argumentExpressions = arguments.Select(arg => arg.Expression);
					expr = new InvocationExpression(mre, argumentExpressions);
				}
				return expr.WithILInstruction(inst).WithRR(rr);
			}
		}
		
		static bool CanInferAnonymousTypePropertyNamesFromArguments(IList<Expression> args, IList<IParameter> parameters)
		{
			for (int i = 0; i < args.Count; i++) {
				string inferredName;
				if (args[i] is IdentifierExpression)
					inferredName = ((IdentifierExpression)args[i]).Identifier;
				else if (args[i] is MemberReferenceExpression)
					inferredName = ((MemberReferenceExpression)args[i]).MemberName;
				else
					inferredName = null;
				
				if (inferredName != parameters[i].Name) {
					return false;
				}
			}
			return true;
		}

		Expression HandleAccessorCall(ILInstruction inst, TranslatedExpression target, IMethod method, IList<TranslatedExpression> arguments)
		{
			var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentAssembly);
			var result = lookup.Lookup(target.ResolveResult, method.AccessorOwner.Name, EmptyList<IType>.Instance, isInvocation:false);
			
			if (result.IsError || (result is MemberResolveResult && !IsAppropriateCallTarget(method.AccessorOwner, ((MemberResolveResult)result).Member, inst.OpCode == OpCode.CallVirt)))
				target = target.ConvertTo(method.AccessorOwner.DeclaringType, this);

			if (method.ReturnType.IsKnownType(KnownTypeCode.Void)) {
				var value = arguments.Last();
				arguments.Remove(value);
				Expression expr;
				if (arguments.Count == 0)
					expr = new MemberReferenceExpression(target.Expression, method.AccessorOwner.Name);
				else
					expr = new IndexerExpression(target.Expression, arguments.Select(a => a.Expression));
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
				return new AssignmentExpression(expr, op, value.Expression);
			} else {
				if (arguments.Count == 0)
					return new MemberReferenceExpression(target.Expression, method.AccessorOwner.Name);
				else
					return new IndexerExpression(target.Expression, arguments.Select(a => a.Expression));
			}
		}

		bool IsAppropriateCallTarget(IMember expectedTarget, IMember actualTarget, bool isVirtCall)
		{
			if (expectedTarget.Equals(actualTarget))
				return true;
			
			if (isVirtCall && actualTarget.IsOverride) {
				foreach (var possibleTarget in InheritanceHelper.GetBaseMembers(actualTarget, false)) {
					if (expectedTarget.Equals(possibleTarget))
						return true;
					if (!possibleTarget.IsOverride)
						break;
				}
			}
			return false;
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
				return arrayExpr.Expression.Member("Length")
					.WithILInstruction(inst)
					.WithRR(new ResolveResult(compilation.FindType(KnownTypeCode.Int32)));
			} else {
				return arrayExpr.Expression.Member("LongLength")
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
			var stackType = i.ResultType == StackType.I4 ? KnownTypeCode.Int32 : KnownTypeCode.Int64;
			return Translate(i).ConvertTo(compilation.FindType(stackType), this);
		}
		
		protected internal override TranslatedExpression VisitUnboxAny(UnboxAny inst, TranslationContext context)
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
			return new UndocumentedExpression {
				UndocumentedExpressionType = UndocumentedExpressionType.RefType,
				Arguments = { Translate(inst.Argument).Expression.Detach() }
			}.Member("TypeHandle")
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
				case BlockType.CompoundOperator:
					return TranslateCompoundOperator(block);
				default:
					return ErrorExpression("Unknown block type: " + block.Type);
			}
		}

		TranslatedExpression TranslateArrayInitializer(Block block)
		{
			var stloc = block.Instructions.FirstOrDefault() as StLoc;
			var final = block.FinalInstruction as LdLoc;
			IType type;
			if (stloc == null || final == null || !stloc.Value.MatchNewArr(out type) || stloc.Variable != final.Variable)
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
				var val = Translate(value).ConvertTo(type, this);
				container.Peek().Elements.Add(val);
				elementResolveResults.Add(val.ResolveResult);
				while (container.Count > 0 && container.Peek().Elements.Count == dimensionSizes[container.Count - 1]) {
					container.Pop();
				}
			}
			ArraySpecifier[] additionalSpecifiers;
			var typeExpression = ConvertType(type);
			if (typeExpression is ComposedType) {
				additionalSpecifiers = ((ComposedType)typeExpression).ArraySpecifiers.SelectArray(a => (ArraySpecifier)a.Clone());
				typeExpression = ((ComposedType)typeExpression).BaseType.Clone();
			} else {
				additionalSpecifiers = new ArraySpecifier[0];
			}
			var expr = new ArrayCreateExpression {
				Type = typeExpression,
				Initializer = root
			};
			expr.AdditionalArraySpecifiers.AddRange(additionalSpecifiers);
			expr.Arguments.AddRange(newArr.Indices.Select(i => Translate(i).Expression));
			return expr.WithILInstruction(block)
				.WithRR(new ArrayCreateResolveResult(new ArrayType(compilation, type, dimensions), newArr.Indices.Select(i => Translate(i).ResolveResult).ToArray(), elementResolveResults));
		}
		
		TranslatedExpression TranslateCompoundOperator(Block block)
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
		
		protected internal override TranslatedExpression VisitInvalidInstruction(InvalidInstruction inst, TranslationContext context)
		{
			string message = "Invalid IL";
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
