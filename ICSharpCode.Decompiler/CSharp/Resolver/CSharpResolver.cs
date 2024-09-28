// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.TypeSystem;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp.Resolver
{
	/// <summary>
	/// Contains the main resolver logic.
	/// </summary>
	/// <remarks>
	/// This class is thread-safe.
	/// </remarks>
	public class CSharpResolver : ICodeContext
	{
		static readonly ResolveResult ErrorResult = ErrorResolveResult.UnknownError;
		readonly ICompilation compilation;
		internal readonly CSharpConversions conversions;
		readonly CSharpTypeResolveContext context;
		readonly bool checkForOverflow;
		readonly bool isWithinLambdaExpression;

		#region Constructor
		public CSharpResolver(ICompilation compilation)
		{
			if (compilation == null)
				throw new ArgumentNullException(nameof(compilation));
			this.compilation = compilation;
			this.conversions = CSharpConversions.Get(compilation);
			this.context = new CSharpTypeResolveContext(compilation.MainModule);
		}

		public CSharpResolver(CSharpTypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));
			this.compilation = context.Compilation;
			this.conversions = CSharpConversions.Get(compilation);
			this.context = context;
			if (context.CurrentTypeDefinition != null)
				currentTypeDefinitionCache = new TypeDefinitionCache(context.CurrentTypeDefinition);
		}

		private CSharpResolver(ICompilation compilation, CSharpConversions conversions, CSharpTypeResolveContext context, bool checkForOverflow, bool isWithinLambdaExpression, TypeDefinitionCache currentTypeDefinitionCache, ImmutableStack<Dictionary<string, IVariable>> localVariableStack, ObjectInitializerContext objectInitializerStack)
		{
			this.compilation = compilation;
			this.conversions = conversions;
			this.context = context;
			this.checkForOverflow = checkForOverflow;
			this.isWithinLambdaExpression = isWithinLambdaExpression;
			this.currentTypeDefinitionCache = currentTypeDefinitionCache;
			this.localVariableStack = localVariableStack;
			this.objectInitializerStack = objectInitializerStack;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the compilation used by the resolver.
		/// </summary>
		public ICompilation Compilation {
			get { return compilation; }
		}

		/// <summary>
		/// Gets the current type resolve context.
		/// </summary>
		public CSharpTypeResolveContext CurrentTypeResolveContext {
			get { return context; }
		}

		IModule ITypeResolveContext.CurrentModule {
			get { return context.CurrentModule; }
		}

		CSharpResolver WithContext(CSharpTypeResolveContext newContext)
		{
			return new CSharpResolver(compilation, conversions, newContext, checkForOverflow, isWithinLambdaExpression, currentTypeDefinitionCache, localVariableStack, objectInitializerStack);
		}

		/// <summary>
		/// Gets whether the current context is <c>checked</c>.
		/// </summary>
		public bool CheckForOverflow {
			get { return checkForOverflow; }
		}

		/// <summary>
		/// Sets whether the current context is <c>checked</c>.
		/// </summary>
		public CSharpResolver WithCheckForOverflow(bool checkForOverflow)
		{
			if (checkForOverflow == this.checkForOverflow)
				return this;
			return new CSharpResolver(compilation, conversions, context, checkForOverflow, isWithinLambdaExpression, currentTypeDefinitionCache, localVariableStack, objectInitializerStack);
		}

		/// <summary>
		/// Gets whether the resolver is currently within a lambda expression or anonymous method.
		/// </summary>
		public bool IsWithinLambdaExpression {
			get { return isWithinLambdaExpression; }
		}

		/// <summary>
		/// Sets whether the resolver is currently within a lambda expression.
		/// </summary>
		public CSharpResolver WithIsWithinLambdaExpression(bool isWithinLambdaExpression)
		{
			return new CSharpResolver(compilation, conversions, context, checkForOverflow, isWithinLambdaExpression, currentTypeDefinitionCache, localVariableStack, objectInitializerStack);
		}

		/// <summary>
		/// Gets the current member definition that is used to look up identifiers as parameters
		/// or type parameters.
		/// </summary>
		public IMember CurrentMember {
			get { return context.CurrentMember; }
		}

		/// <summary>
		/// Sets the current member definition.
		/// </summary>
		/// <remarks>Don't forget to also set CurrentTypeDefinition when setting CurrentMember;
		/// setting one of the properties does not automatically set the other.</remarks>
		public CSharpResolver WithCurrentMember(IMember member)
		{
			return WithContext(context.WithCurrentMember(member));
		}

		ITypeResolveContext ITypeResolveContext.WithCurrentMember(IMember? member)
		{
			return WithCurrentMember(member);
		}

		/// <summary>
		/// Gets the current using scope that is used to look up identifiers as class names.
		/// </summary>
		public ResolvedUsingScope CurrentUsingScope {
			get { return context.CurrentUsingScope; }
		}

		/// <summary>
		/// Sets the current using scope that is used to look up identifiers as class names.
		/// </summary>
		public CSharpResolver WithCurrentUsingScope(ResolvedUsingScope usingScope)
		{
			return WithContext(context.WithUsingScope(usingScope));
		}
		#endregion

		#region Per-CurrentTypeDefinition Cache
		readonly TypeDefinitionCache currentTypeDefinitionCache;

		/// <summary>
		/// Gets the current type definition.
		/// </summary>
		public ITypeDefinition CurrentTypeDefinition {
			get { return context.CurrentTypeDefinition; }
		}

		/// <summary>
		/// Sets the current type definition.
		/// </summary>
		public CSharpResolver WithCurrentTypeDefinition(ITypeDefinition? typeDefinition)
		{
			if (this.CurrentTypeDefinition == typeDefinition)
				return this;

			TypeDefinitionCache? newTypeDefinitionCache;
			if (typeDefinition != null)
				newTypeDefinitionCache = new TypeDefinitionCache(typeDefinition);
			else
				newTypeDefinitionCache = null;

			return new CSharpResolver(compilation, conversions, context.WithCurrentTypeDefinition(typeDefinition),
									  checkForOverflow, isWithinLambdaExpression, newTypeDefinitionCache, localVariableStack, objectInitializerStack);
		}

		ITypeResolveContext ITypeResolveContext.WithCurrentTypeDefinition(ITypeDefinition? typeDefinition)
		{
			return WithCurrentTypeDefinition(typeDefinition);
		}

		sealed class TypeDefinitionCache
		{
			public readonly ITypeDefinition TypeDefinition;
			public readonly Dictionary<string, ResolveResult> SimpleNameLookupCacheExpression = new Dictionary<string, ResolveResult>();
			public readonly Dictionary<string, ResolveResult> SimpleNameLookupCacheInvocationTarget = new Dictionary<string, ResolveResult>();
			public readonly Dictionary<string, ResolveResult> SimpleTypeLookupCache = new Dictionary<string, ResolveResult>();

			public TypeDefinitionCache(ITypeDefinition typeDefinition)
			{
				this.TypeDefinition = typeDefinition;
			}
		}
		#endregion

		#region Local Variable Management

		// We store the local variables in an immutable stack.
		// The beginning of a block is marked by a null entry.

		// This data structure is used to allow efficient cloning of the resolver with its local variable context.
		readonly ImmutableStack<Dictionary<string, IVariable>> localVariableStack = ImmutableStack<Dictionary<string, IVariable>>.Empty;

		CSharpResolver WithLocalVariableStack(ImmutableStack<Dictionary<string, IVariable>> stack)
		{
			return new CSharpResolver(compilation, conversions, context, checkForOverflow, isWithinLambdaExpression, currentTypeDefinitionCache, stack, objectInitializerStack);
		}

		/// <summary>
		/// Adds new variableŝ or lambda parameters to the current block.
		/// </summary>
		public CSharpResolver AddVariables(Dictionary<string, IVariable> variables)
		{
			if (variables == null)
				throw new ArgumentNullException(nameof(variables));
			return WithLocalVariableStack(localVariableStack.Push(variables));
		}

		/// <summary>
		/// Gets all currently visible local variables and lambda parameters.
		/// Does not include method parameters.
		/// </summary>
		public IEnumerable<IVariable> LocalVariables {
			get {
				return localVariableStack.SelectMany(s => s.Values);
			}
		}
		#endregion

		#region Object Initializer Context
		sealed class ObjectInitializerContext
		{
			internal readonly ResolveResult initializedObject;
			internal readonly ObjectInitializerContext prev;

			public ObjectInitializerContext(ResolveResult initializedObject, CSharpResolver.ObjectInitializerContext prev)
			{
				this.initializedObject = initializedObject;
				this.prev = prev;
			}
		}

		readonly ObjectInitializerContext objectInitializerStack;

		CSharpResolver WithObjectInitializerStack(ObjectInitializerContext stack)
		{
			return new CSharpResolver(compilation, conversions, context, checkForOverflow, isWithinLambdaExpression, currentTypeDefinitionCache, localVariableStack, stack);
		}

		/// <summary>
		/// Pushes the type of the object that is currently being initialized.
		/// </summary>
		public CSharpResolver PushObjectInitializer(ResolveResult initializedObject)
		{
			if (initializedObject == null)
				throw new ArgumentNullException(nameof(initializedObject));
			return WithObjectInitializerStack(new ObjectInitializerContext(initializedObject, objectInitializerStack));
		}

		public CSharpResolver PopObjectInitializer()
		{
			if (objectInitializerStack == null)
				throw new InvalidOperationException();
			return WithObjectInitializerStack(objectInitializerStack.prev);
		}

		/// <summary>
		/// Gets whether this context is within an object initializer.
		/// </summary>
		public bool IsInObjectInitializer {
			get { return objectInitializerStack != null; }
		}

		/// <summary>
		/// Gets the current object initializer. This usually is an <see cref="InitializedObjectResolveResult"/>
		/// or (for nested initializers) a semantic tree based on an <see cref="InitializedObjectResolveResult"/>.
		/// Returns ErrorResolveResult if there is no object initializer.
		/// </summary>
		public ResolveResult CurrentObjectInitializer {
			get {
				return objectInitializerStack != null ? objectInitializerStack.initializedObject : ErrorResult;
			}
		}

		/// <summary>
		/// Gets the type of the object currently being initialized.
		/// Returns SharedTypes.Unknown if no object initializer is currently open (or if the object initializer
		/// has unknown type).
		/// </summary>
		public IType CurrentObjectInitializerType {
			get { return CurrentObjectInitializer.Type; }
		}
		#endregion

		#region ResolveUnaryOperator
		#region ResolveUnaryOperator method
		public ResolveResult ResolveUnaryOperator(UnaryOperatorType op, ResolveResult expression)
		{
			if (expression.Type.Kind == TypeKind.Dynamic)
			{
				if (op == UnaryOperatorType.Await)
				{
					return new AwaitResolveResult(SpecialType.Dynamic, new DynamicInvocationResolveResult(new DynamicMemberResolveResult(expression, "GetAwaiter"), DynamicInvocationType.Invocation, EmptyList<ResolveResult>.Instance), SpecialType.Dynamic, null, null, null);
				}
				else
				{
					return UnaryOperatorResolveResult(SpecialType.Dynamic, op, expression);
				}
			}

			// C# 4.0 spec: §7.3.3 Unary operator overload resolution
			string overloadableOperatorName = GetOverloadableOperatorName(op);
			if (overloadableOperatorName == null)
			{
				switch (op)
				{
					case UnaryOperatorType.Dereference:
						PointerType? p = expression.Type as PointerType;
						if (p != null)
							return UnaryOperatorResolveResult(p.ElementType, op, expression);
						else
							return ErrorResult;
					case UnaryOperatorType.AddressOf:
						return UnaryOperatorResolveResult(new PointerType(expression.Type), op, expression);
					case UnaryOperatorType.Await:
					{
						ResolveResult getAwaiterMethodGroup = ResolveMemberAccess(expression, "GetAwaiter", EmptyList<IType>.Instance, NameLookupMode.InvocationTarget);
						ResolveResult getAwaiterInvocation = ResolveInvocation(getAwaiterMethodGroup, Empty<ResolveResult>.Array, argumentNames: null, allowOptionalParameters: false);

						var lookup = CreateMemberLookup();
						IMethod? getResultMethod;
						IType awaitResultType;
						var getResultMethodGroup = lookup.Lookup(getAwaiterInvocation, "GetResult", EmptyList<IType>.Instance, true) as MethodGroupResolveResult;
						if (getResultMethodGroup != null)
						{
							var getResultOR = getResultMethodGroup.PerformOverloadResolution(compilation, Empty<ResolveResult>.Array, allowExtensionMethods: false, conversions: conversions);
							getResultMethod = getResultOR.FoundApplicableCandidate ? getResultOR.GetBestCandidateWithSubstitutedTypeArguments() as IMethod : null;
							awaitResultType = getResultMethod != null ? getResultMethod.ReturnType : SpecialType.UnknownType;
						}
						else
						{
							getResultMethod = null;
							awaitResultType = SpecialType.UnknownType;
						}

						var isCompletedRR = lookup.Lookup(getAwaiterInvocation, "IsCompleted", EmptyList<IType>.Instance, false);
						var isCompletedProperty = (isCompletedRR is MemberResolveResult ? ((MemberResolveResult)isCompletedRR).Member as IProperty : null);
						if (isCompletedProperty != null && (!isCompletedProperty.ReturnType.IsKnownType(KnownTypeCode.Boolean) || !isCompletedProperty.CanGet))
							isCompletedProperty = null;
						/*
						var interfaceOnCompleted = compilation.FindType(KnownTypeCode.INotifyCompletion).GetMethods().FirstOrDefault(x => x.Name == "OnCompleted");
						var interfaceUnsafeOnCompleted = compilation.FindType(KnownTypeCode.ICriticalNotifyCompletion).GetMethods().FirstOrDefault(x => x.Name == "UnsafeOnCompleted");

						IMethod onCompletedMethod = null;
						var candidates = getAwaiterInvocation.Type.GetMethods().Where(x => x.ImplementedInterfaceMembers.Select(y => y.MemberDefinition).Contains(interfaceUnsafeOnCompleted)).ToList();
						if (candidates.Count == 0) {
							candidates = getAwaiterInvocation.Type.GetMethods().Where(x => x.ImplementedInterfaceMembers.Select(y => y.MemberDefinition).Contains(interfaceOnCompleted)).ToList();
							if (candidates.Count == 1)
								onCompletedMethod = candidates[0];
						}
						else if (candidates.Count == 1) {
							onCompletedMethod = candidates[0];
						}

						return new AwaitResolveResult(awaitResultType, getAwaiterInvocation, getAwaiterInvocation.Type, isCompletedProperty, onCompletedMethod, getResultMethod);
						*/
						// Not adjusted to TS changes for interface impls
						// But I believe this is dead code for ILSpy anyways...
						throw new NotImplementedException();
					}

					default:
						return ErrorResolveResult.UnknownError;
				}
			}
			// If the type is nullable, get the underlying type:
			IType type = NullableType.GetUnderlyingType(expression.Type);
			bool isNullable = NullableType.IsNullable(expression.Type);

			// the operator is overloadable:
			OverloadResolution userDefinedOperatorOR = CreateOverloadResolution(new[] { expression });
			foreach (var candidate in GetUserDefinedOperatorCandidates(type, overloadableOperatorName))
			{
				userDefinedOperatorOR.AddCandidate(candidate);
			}
			if (userDefinedOperatorOR.FoundApplicableCandidate)
			{
				return CreateResolveResultForUserDefinedOperator(userDefinedOperatorOR, UnaryOperatorExpression.GetLinqNodeType(op, this.CheckForOverflow));
			}

			expression = UnaryNumericPromotion(op, ref type, isNullable, expression);
			CSharpOperators.OperatorMethod[] methodGroup;
			CSharpOperators operators = CSharpOperators.Get(compilation);
			switch (op)
			{
				case UnaryOperatorType.Increment:
				case UnaryOperatorType.Decrement:
				case UnaryOperatorType.PostIncrement:
				case UnaryOperatorType.PostDecrement:
					// C# 4.0 spec: §7.6.9 Postfix increment and decrement operators
					// C# 4.0 spec: §7.7.5 Prefix increment and decrement operators
					TypeCode code = ReflectionHelper.GetTypeCode(type);
					if ((code >= TypeCode.Char && code <= TypeCode.Decimal) || type.Kind == TypeKind.Enum || type.Kind == TypeKind.Pointer || type.IsCSharpNativeIntegerType())
						return UnaryOperatorResolveResult(expression.Type, op, expression, isNullable);
					else
						return new ErrorResolveResult(expression.Type);
				case UnaryOperatorType.Plus:
					if (type.IsCSharpNativeIntegerType())
					{
						return UnaryOperatorResolveResult(expression.Type, op, expression, isNullable);
					}
					methodGroup = operators.UnaryPlusOperators;
					break;
				case UnaryOperatorType.Minus:
					if (type.IsCSharpNativeIntegerType())
					{
						return UnaryOperatorResolveResult(expression.Type, op, expression, isNullable);
					}
					methodGroup = CheckForOverflow ? operators.CheckedUnaryMinusOperators : operators.UncheckedUnaryMinusOperators;
					break;
				case UnaryOperatorType.Not:
					methodGroup = operators.LogicalNegationOperators;
					break;
				case UnaryOperatorType.BitNot:
					if (type.Kind == TypeKind.Enum)
					{
						if (expression.IsCompileTimeConstant && !isNullable && expression.ConstantValue != null)
						{
							// evaluate as (E)(~(U)x);
							var U = compilation.FindType(expression.ConstantValue.GetType());
							var unpackedEnum = new ConstantResolveResult(U, expression.ConstantValue);
							var rr = ResolveUnaryOperator(op, unpackedEnum);
							rr = WithCheckForOverflow(false).ResolveCast(type, rr);
							if (rr.IsCompileTimeConstant)
								return rr;
						}
						return UnaryOperatorResolveResult(expression.Type, op, expression, isNullable);
					}
					else if (type.IsCSharpNativeIntegerType())
					{
						return UnaryOperatorResolveResult(expression.Type, op, expression, isNullable);
					}
					else
					{
						methodGroup = operators.BitwiseComplementOperators;
						break;
					}
				default:
					throw new InvalidOperationException();
			}
			OverloadResolution builtinOperatorOR = CreateOverloadResolution(new[] { expression });
			foreach (var candidate in methodGroup)
			{
				builtinOperatorOR.AddCandidate(candidate);
			}
			CSharpOperators.UnaryOperatorMethod m = (CSharpOperators.UnaryOperatorMethod)builtinOperatorOR.BestCandidate;
			IType resultType = m.ReturnType;
			if (builtinOperatorOR.BestCandidateErrors != OverloadResolutionErrors.None)
			{
				if (userDefinedOperatorOR.BestCandidate != null)
				{
					// If there are any user-defined operators, prefer those over the built-in operators.
					// It'll be a more informative error.
					return CreateResolveResultForUserDefinedOperator(userDefinedOperatorOR, UnaryOperatorExpression.GetLinqNodeType(op, this.CheckForOverflow));
				}
				else if (builtinOperatorOR.BestCandidateAmbiguousWith != null)
				{
					// If the best candidate is ambiguous, just use the input type instead
					// of picking one of the ambiguous overloads.
					return new ErrorResolveResult(expression.Type);
				}
				else
				{
					return new ErrorResolveResult(resultType);
				}
			}
			else if (expression.IsCompileTimeConstant && m.CanEvaluateAtCompileTime)
			{
				object? val;
				try
				{
					val = m.Invoke(this, expression.ConstantValue);
				}
				catch (ArithmeticException)
				{
					return new ErrorResolveResult(resultType);
				}
				return new ConstantResolveResult(resultType, val);
			}
			else
			{
				expression = Convert(expression, m.Parameters[0].Type, builtinOperatorOR.ArgumentConversions[0]);
				return UnaryOperatorResolveResult(resultType, op, expression,
												  builtinOperatorOR.BestCandidate is ILiftedOperator);
			}
		}

		OperatorResolveResult UnaryOperatorResolveResult(IType resultType, UnaryOperatorType op, ResolveResult expression, bool isLifted = false)
		{
			return new OperatorResolveResult(
				resultType, UnaryOperatorExpression.GetLinqNodeType(op, this.CheckForOverflow),
				null, isLifted, new[] { expression });
		}
		#endregion

		#region UnaryNumericPromotion
		ResolveResult UnaryNumericPromotion(UnaryOperatorType op, ref IType type, bool isNullable, ResolveResult expression)
		{
			// C# 4.0 spec: §7.3.6.1
			TypeCode code = ReflectionHelper.GetTypeCode(type);
			if (isNullable && type.Kind == TypeKind.Null)
				code = TypeCode.SByte; // cause promotion of null to int32
			switch (op)
			{
				case UnaryOperatorType.Minus:
					if (code == TypeCode.UInt32)
					{
						type = compilation.FindType(KnownTypeCode.Int64);
						return Convert(expression, MakeNullable(type, isNullable),
									   isNullable ? Conversion.ImplicitNullableConversion : Conversion.ImplicitNumericConversion);
					}
					goto case UnaryOperatorType.Plus;
				case UnaryOperatorType.Plus:
				case UnaryOperatorType.BitNot:
					if (code >= TypeCode.Char && code <= TypeCode.UInt16)
					{
						type = compilation.FindType(KnownTypeCode.Int32);
						return Convert(expression, MakeNullable(type, isNullable),
									   isNullable ? Conversion.ImplicitNullableConversion : Conversion.ImplicitNumericConversion);
					}
					break;
			}
			return expression;
		}
		#endregion

		#region GetOverloadableOperatorName
		static string? GetOverloadableOperatorName(UnaryOperatorType op)
		{
			switch (op)
			{
				case UnaryOperatorType.Not:
					return "op_LogicalNot";
				case UnaryOperatorType.BitNot:
					return "op_OnesComplement";
				case UnaryOperatorType.Minus:
					return "op_UnaryNegation";
				case UnaryOperatorType.Plus:
					return "op_UnaryPlus";
				case UnaryOperatorType.Increment:
				case UnaryOperatorType.PostIncrement:
					return "op_Increment";
				case UnaryOperatorType.Decrement:
				case UnaryOperatorType.PostDecrement:
					return "op_Decrement";
				default:
					return null;
			}
		}
		#endregion
		#endregion

		#region ResolveBinaryOperator
		#region ResolveBinaryOperator method
		public ResolveResult ResolveBinaryOperator(BinaryOperatorType op, ResolveResult lhs, ResolveResult rhs)
		{
			if (lhs.Type.Kind == TypeKind.Dynamic || rhs.Type.Kind == TypeKind.Dynamic)
			{
				lhs = Convert(lhs, SpecialType.Dynamic);
				rhs = Convert(rhs, SpecialType.Dynamic);
				return BinaryOperatorResolveResult(SpecialType.Dynamic, lhs, op, rhs);
			}

			// C# 4.0 spec: §7.3.4 Binary operator overload resolution
			string overloadableOperatorName = GetOverloadableOperatorName(op);
			if (overloadableOperatorName == null)
			{

				// Handle logical and/or exactly as bitwise and/or:
				// - If the user overloads a bitwise operator, that implicitly creates the corresponding logical operator.
				// - If both inputs are compile-time constants, it doesn't matter that we don't short-circuit.
				// - If inputs aren't compile-time constants, we don't evaluate anything, so again it doesn't matter that we don't short-circuit
				if (op == BinaryOperatorType.ConditionalAnd)
				{
					overloadableOperatorName = GetOverloadableOperatorName(BinaryOperatorType.BitwiseAnd);
				}
				else if (op == BinaryOperatorType.ConditionalOr)
				{
					overloadableOperatorName = GetOverloadableOperatorName(BinaryOperatorType.BitwiseOr);
				}
				else if (op == BinaryOperatorType.NullCoalescing)
				{
					// null coalescing operator is not overloadable and needs to be handled separately
					return ResolveNullCoalescingOperator(lhs, rhs);
				}
				else
				{
					return ErrorResolveResult.UnknownError;
				}
			}

			// If the type is nullable, get the underlying type:
			bool isNullable = NullableType.IsNullable(lhs.Type) || NullableType.IsNullable(rhs.Type);
			IType lhsType = NullableType.GetUnderlyingType(lhs.Type);
			IType rhsType = NullableType.GetUnderlyingType(rhs.Type);

			// the operator is overloadable:
			OverloadResolution userDefinedOperatorOR = CreateOverloadResolution(new[] { lhs, rhs });
			HashSet<IParameterizedMember> userOperatorCandidates = new HashSet<IParameterizedMember>();
			userOperatorCandidates.UnionWith(GetUserDefinedOperatorCandidates(lhsType, overloadableOperatorName));
			userOperatorCandidates.UnionWith(GetUserDefinedOperatorCandidates(rhsType, overloadableOperatorName));
			foreach (var candidate in userOperatorCandidates)
			{
				userDefinedOperatorOR.AddCandidate(candidate);
			}
			if (userDefinedOperatorOR.FoundApplicableCandidate)
			{
				return CreateResolveResultForUserDefinedOperator(userDefinedOperatorOR, BinaryOperatorExpression.GetLinqNodeType(op, this.CheckForOverflow));
			}

			if (lhsType.Kind == TypeKind.Null && rhsType.IsReferenceType == false
				|| lhsType.IsReferenceType == false && rhsType.Kind == TypeKind.Null)
			{
				isNullable = true;
			}
			if (op == BinaryOperatorType.ShiftLeft || op == BinaryOperatorType.ShiftRight || op == BinaryOperatorType.UnsignedShiftRight)
			{
				// special case: the shift operators allow "var x = null << null", producing int?.
				if (lhsType.Kind == TypeKind.Null && rhsType.Kind == TypeKind.Null)
					isNullable = true;
				// for shift operators, do unary promotion independently on both arguments
				lhs = UnaryNumericPromotion(UnaryOperatorType.Plus, ref lhsType, isNullable, lhs);
				rhs = UnaryNumericPromotion(UnaryOperatorType.Plus, ref rhsType, isNullable, rhs);
			}
			else
			{
				bool allowNullableConstants = op == BinaryOperatorType.Equality || op == BinaryOperatorType.InEquality;
				if (!BinaryNumericPromotion(isNullable, ref lhs, ref rhs, allowNullableConstants))
					return new ErrorResolveResult(lhs.Type);
			}
			// re-read underlying types after numeric promotion
			lhsType = NullableType.GetUnderlyingType(lhs.Type);
			rhsType = NullableType.GetUnderlyingType(rhs.Type);

			IEnumerable<CSharpOperators.OperatorMethod> methodGroup;
			CSharpOperators operators = CSharpOperators.Get(compilation);
			switch (op)
			{
				case BinaryOperatorType.Multiply:
					methodGroup = operators.MultiplicationOperators;
					break;
				case BinaryOperatorType.Divide:
					methodGroup = operators.DivisionOperators;
					break;
				case BinaryOperatorType.Modulus:
					methodGroup = operators.RemainderOperators;
					break;
				case BinaryOperatorType.Add:
					methodGroup = operators.AdditionOperators;
					{
						if (lhsType.Kind == TypeKind.Enum)
						{
							// E operator +(E x, U y);
							IType underlyingType = MakeNullable(GetEnumUnderlyingType(lhsType), isNullable);
							if (TryConvertEnum(ref rhs, underlyingType, ref isNullable, ref lhs))
							{
								return HandleEnumOperator(isNullable, lhsType, op, lhs, rhs);
							}
						}
						if (rhsType.Kind == TypeKind.Enum)
						{
							// E operator +(U x, E y);
							IType underlyingType = MakeNullable(GetEnumUnderlyingType(rhsType), isNullable);
							if (TryConvertEnum(ref lhs, underlyingType, ref isNullable, ref rhs))
							{
								return HandleEnumOperator(isNullable, rhsType, op, lhs, rhs);
							}
						}

						if (lhsType.Kind == TypeKind.Delegate && TryConvert(ref rhs, lhsType))
						{
							return BinaryOperatorResolveResult(lhsType, lhs, op, rhs);
						}
						else if (rhsType.Kind == TypeKind.Delegate && TryConvert(ref lhs, rhsType))
						{
							return BinaryOperatorResolveResult(rhsType, lhs, op, rhs);
						}

						if (lhsType.Kind == TypeKind.Null && rhsType.Kind == TypeKind.Null)
							return new ErrorResolveResult(SpecialType.NullType);
					}
					break;
				case BinaryOperatorType.Subtract:
					methodGroup = operators.SubtractionOperators;
					{
						if (lhsType.Kind == TypeKind.Enum)
						{
							// U operator –(E x, E y);
							if (TryConvertEnum(ref rhs, lhs.Type, ref isNullable, ref lhs, allowConversionFromConstantZero: false))
							{
								return HandleEnumSubtraction(isNullable, lhsType, lhs, rhs);
							}

							// E operator –(E x, U y);
							IType underlyingType = MakeNullable(GetEnumUnderlyingType(lhsType), isNullable);
							if (TryConvertEnum(ref rhs, underlyingType, ref isNullable, ref lhs))
							{
								return HandleEnumOperator(isNullable, lhsType, op, lhs, rhs);
							}
						}
						if (rhsType.Kind == TypeKind.Enum)
						{
							// U operator –(E x, E y);
							if (TryConvertEnum(ref lhs, rhs.Type, ref isNullable, ref rhs))
							{
								return HandleEnumSubtraction(isNullable, rhsType, lhs, rhs);
							}

							// E operator -(U x, E y);
							IType underlyingType = MakeNullable(GetEnumUnderlyingType(rhsType), isNullable);
							if (TryConvertEnum(ref lhs, underlyingType, ref isNullable, ref rhs))
							{
								return HandleEnumOperator(isNullable, rhsType, op, lhs, rhs);
							}
						}

						if (lhsType.Kind == TypeKind.Delegate && TryConvert(ref rhs, lhsType))
						{
							return BinaryOperatorResolveResult(lhsType, lhs, op, rhs);
						}
						else if (rhsType.Kind == TypeKind.Delegate && TryConvert(ref lhs, rhsType))
						{
							return BinaryOperatorResolveResult(rhsType, lhs, op, rhs);
						}

						if (lhsType.Kind == TypeKind.Null && rhsType.Kind == TypeKind.Null)
							return new ErrorResolveResult(SpecialType.NullType);
					}
					break;
				case BinaryOperatorType.ShiftLeft:
					methodGroup = operators.ShiftLeftOperators;
					break;
				case BinaryOperatorType.ShiftRight:
					methodGroup = operators.ShiftRightOperators;
					break;
				case BinaryOperatorType.UnsignedShiftRight:
					methodGroup = operators.UnsignedShiftRightOperators;
					break;
				case BinaryOperatorType.Equality:
				case BinaryOperatorType.InEquality:
				case BinaryOperatorType.LessThan:
				case BinaryOperatorType.GreaterThan:
				case BinaryOperatorType.LessThanOrEqual:
				case BinaryOperatorType.GreaterThanOrEqual:
				{
					if (lhsType.Kind == TypeKind.Enum && TryConvert(ref rhs, lhs.Type))
					{
						// bool operator op(E x, E y);
						return HandleEnumComparison(op, lhsType, isNullable, lhs, rhs);
					}
					else if (rhsType.Kind == TypeKind.Enum && TryConvert(ref lhs, rhs.Type))
					{
						// bool operator op(E x, E y);
						return HandleEnumComparison(op, rhsType, isNullable, lhs, rhs);
					}
					else if (lhsType is PointerType && rhsType is PointerType)
					{
						return BinaryOperatorResolveResult(compilation.FindType(KnownTypeCode.Boolean), lhs, op, rhs);
					}
					else if (lhsType.IsCSharpNativeIntegerType() || rhsType.IsCSharpNativeIntegerType())
					{
						if (lhsType.Equals(rhsType))
							return BinaryOperatorResolveResult(compilation.FindType(KnownTypeCode.Boolean), lhs, op, rhs, isLifted: isNullable);
						else
							return new ErrorResolveResult(compilation.FindType(KnownTypeCode.Boolean));
					}
					if (op == BinaryOperatorType.Equality || op == BinaryOperatorType.InEquality)
					{
						if (lhsType.IsReferenceType == true && rhsType.IsReferenceType == true)
						{
							// If it's a reference comparison
							if (op == BinaryOperatorType.Equality)
								methodGroup = operators.ReferenceEqualityOperators;
							else
								methodGroup = operators.ReferenceInequalityOperators;
							break;
						}
						else if (lhsType.Kind == TypeKind.Null && IsNullableTypeOrNonValueType(rhs.Type)
								 || IsNullableTypeOrNonValueType(lhs.Type) && rhsType.Kind == TypeKind.Null)
						{
							// compare type parameter or nullable type with the null literal
							return BinaryOperatorResolveResult(compilation.FindType(KnownTypeCode.Boolean), lhs, op, rhs);
						}
					}
					switch (op)
					{
						case BinaryOperatorType.Equality:
							methodGroup = operators.ValueEqualityOperators;
							break;
						case BinaryOperatorType.InEquality:
							methodGroup = operators.ValueInequalityOperators;
							break;
						case BinaryOperatorType.LessThan:
							methodGroup = operators.LessThanOperators;
							break;
						case BinaryOperatorType.GreaterThan:
							methodGroup = operators.GreaterThanOperators;
							break;
						case BinaryOperatorType.LessThanOrEqual:
							methodGroup = operators.LessThanOrEqualOperators;
							break;
						case BinaryOperatorType.GreaterThanOrEqual:
							methodGroup = operators.GreaterThanOrEqualOperators;
							break;
						default:
							throw new InvalidOperationException();
					}
				}
				break;
				case BinaryOperatorType.BitwiseAnd:
				case BinaryOperatorType.BitwiseOr:
				case BinaryOperatorType.ExclusiveOr:
				{
					if (lhsType.Kind == TypeKind.Enum)
					{
						// bool operator op(E x, E y);
						if (TryConvertEnum(ref rhs, lhs.Type, ref isNullable, ref lhs))
						{
							return HandleEnumOperator(isNullable, lhsType, op, lhs, rhs);
						}
					}

					if (rhsType.Kind == TypeKind.Enum)
					{
						// bool operator op(E x, E y);
						if (TryConvertEnum(ref lhs, rhs.Type, ref isNullable, ref rhs))
						{
							return HandleEnumOperator(isNullable, rhsType, op, lhs, rhs);
						}
					}

					switch (op)
					{
						case BinaryOperatorType.BitwiseAnd:
							methodGroup = operators.BitwiseAndOperators;
							break;
						case BinaryOperatorType.BitwiseOr:
							methodGroup = operators.BitwiseOrOperators;
							break;
						case BinaryOperatorType.ExclusiveOr:
							methodGroup = operators.BitwiseXorOperators;
							break;
						default:
							throw new InvalidOperationException();
					}
				}
				break;
				case BinaryOperatorType.ConditionalAnd:
					methodGroup = operators.LogicalAndOperators;
					break;
				case BinaryOperatorType.ConditionalOr:
					methodGroup = operators.LogicalOrOperators;
					break;
				default:
					throw new InvalidOperationException();
			}
			if (lhsType.IsCSharpNativeIntegerType() || rhsType.IsCSharpNativeIntegerType())
			{
				if (lhsType.Equals(rhsType))
				{
					return BinaryOperatorResolveResult(
						isNullable ? NullableType.Create(compilation, lhsType) : lhsType,
						lhs, op, rhs, isLifted: isNullable);
				}
				// mixing nint/nuint is not allowed
				return new ErrorResolveResult(lhsType);
			}
			OverloadResolution builtinOperatorOR = CreateOverloadResolution(new[] { lhs, rhs });
			foreach (var candidate in methodGroup)
			{
				builtinOperatorOR.AddCandidate(candidate);
			}
			CSharpOperators.BinaryOperatorMethod m = (CSharpOperators.BinaryOperatorMethod)builtinOperatorOR.BestCandidate;
			IType resultType = m.ReturnType;
			if (builtinOperatorOR.BestCandidateErrors != OverloadResolutionErrors.None)
			{
				// If there are any user-defined operators, prefer those over the built-in operators.
				// It'll be a more informative error.
				if (userDefinedOperatorOR.BestCandidate != null)
					return CreateResolveResultForUserDefinedOperator(userDefinedOperatorOR, BinaryOperatorExpression.GetLinqNodeType(op, this.CheckForOverflow));
				else
					return new ErrorResolveResult(resultType);
			}
			else if (lhs.IsCompileTimeConstant && rhs.IsCompileTimeConstant && m.CanEvaluateAtCompileTime)
			{
				object? val;
				try
				{
					val = m.Invoke(this, lhs.ConstantValue, rhs.ConstantValue);
				}
				catch (ArithmeticException)
				{
					return new ErrorResolveResult(resultType);
				}
				return new ConstantResolveResult(resultType, val);
			}
			else
			{
				lhs = Convert(lhs, m.Parameters[0].Type, builtinOperatorOR.ArgumentConversions[0]);
				rhs = Convert(rhs, m.Parameters[1].Type, builtinOperatorOR.ArgumentConversions[1]);
				return BinaryOperatorResolveResult(resultType, lhs, op, rhs,
												   builtinOperatorOR.BestCandidate is ILiftedOperator);
			}
		}

		bool IsNullableTypeOrNonValueType(IType type)
		{
			return NullableType.IsNullable(type) || type.IsReferenceType != false;
		}

		ResolveResult BinaryOperatorResolveResult(IType resultType, ResolveResult lhs, BinaryOperatorType op, ResolveResult rhs, bool isLifted = false)
		{
			return new OperatorResolveResult(
				resultType, BinaryOperatorExpression.GetLinqNodeType(op, this.CheckForOverflow),
				null, isLifted, new[] { lhs, rhs });
		}
		#endregion

		#region Pointer arithmetic
		CSharpOperators.BinaryOperatorMethod PointerArithmeticOperator(IType resultType, IType inputType1, KnownTypeCode inputType2)
		{
			return PointerArithmeticOperator(resultType, inputType1, compilation.FindType(inputType2));
		}

		CSharpOperators.BinaryOperatorMethod PointerArithmeticOperator(IType resultType, KnownTypeCode inputType1, IType inputType2)
		{
			return PointerArithmeticOperator(resultType, compilation.FindType(inputType1), inputType2);
		}

		CSharpOperators.BinaryOperatorMethod PointerArithmeticOperator(IType resultType, IType inputType1, IType inputType2)
		{
			return new CSharpOperators.BinaryOperatorMethod(compilation) {
				ReturnType = resultType,
				parameters = {
					new DefaultParameter(inputType1, string.Empty),
					new DefaultParameter(inputType2, string.Empty)
				}
			};
		}
		#endregion

		#region Enum helper methods
		IType? GetEnumUnderlyingType(IType enumType)
		{
			ITypeDefinition? def = enumType.GetDefinition();
			return def != null ? def.EnumUnderlyingType : SpecialType.UnknownType;
		}

		/// <summary>
		/// Handle the case where an enum value is compared with another enum value
		/// bool operator op(E x, E y);
		/// </summary>
		ResolveResult HandleEnumComparison(BinaryOperatorType op, IType enumType, bool isNullable, ResolveResult lhs, ResolveResult rhs)
		{
			// evaluate as ((U)x op (U)y)
			IType elementType = GetEnumUnderlyingType(enumType);
			if (lhs.IsCompileTimeConstant && rhs.IsCompileTimeConstant && !isNullable && elementType.Kind != TypeKind.Enum)
			{
				var rr = ResolveBinaryOperator(op, ResolveCast(elementType, lhs), ResolveCast(elementType, rhs));
				if (rr.IsCompileTimeConstant)
					return rr;
			}
			IType resultType = compilation.FindType(KnownTypeCode.Boolean);
			return BinaryOperatorResolveResult(resultType, lhs, op, rhs, isNullable);
		}

		/// <summary>
		/// Handle the case where an enum value is subtracted from another enum value
		/// U operator –(E x, E y);
		/// </summary>
		ResolveResult HandleEnumSubtraction(bool isNullable, IType enumType, ResolveResult lhs, ResolveResult rhs)
		{
			// evaluate as (U)((U)x – (U)y)
			IType elementType = GetEnumUnderlyingType(enumType);
			if (lhs.IsCompileTimeConstant && rhs.IsCompileTimeConstant && !isNullable && elementType.Kind != TypeKind.Enum)
			{
				var rr = ResolveBinaryOperator(BinaryOperatorType.Subtract, ResolveCast(elementType, lhs), ResolveCast(elementType, rhs));
				rr = WithCheckForOverflow(false).ResolveCast(elementType, rr);
				if (rr.IsCompileTimeConstant)
					return rr;
			}
			IType resultType = MakeNullable(elementType, isNullable);
			return BinaryOperatorResolveResult(resultType, lhs, BinaryOperatorType.Subtract, rhs, isNullable);
		}

		/// <summary>
		/// Handle the following enum operators:
		/// E operator +(E x, U y);
		/// E operator +(U x, E y);
		/// E operator –(E x, U y);
		/// E operator &amp;(E x, E y);
		/// E operator |(E x, E y);
		/// E operator ^(E x, E y);
		/// </summary>
		ResolveResult HandleEnumOperator(bool isNullable, IType enumType, BinaryOperatorType op, ResolveResult lhs, ResolveResult rhs)
		{
			// evaluate as (E)((U)x op (U)y)
			if (lhs.IsCompileTimeConstant && rhs.IsCompileTimeConstant && !isNullable)
			{
				IType elementType = GetEnumUnderlyingType(enumType);
				if (elementType.Kind != TypeKind.Enum)
				{
					var rr = ResolveBinaryOperator(op, ResolveCast(elementType, lhs), ResolveCast(elementType, rhs));
					rr = WithCheckForOverflow(false).ResolveCast(enumType, rr);
					if (rr.IsCompileTimeConstant) // only report result if it's a constant; use the regular OperatorResolveResult codepath otherwise
						return rr;
				}
			}
			IType resultType = MakeNullable(enumType, isNullable);
			return BinaryOperatorResolveResult(resultType, lhs, op, rhs, isNullable);
		}

		IType MakeNullable(IType type, bool isNullable)
		{
			if (isNullable)
				return NullableType.Create(compilation, type);
			else
				return type;
		}
		#endregion

		#region BinaryNumericPromotion
		bool BinaryNumericPromotion(bool isNullable, ref ResolveResult lhs, ref ResolveResult rhs, bool allowNullableConstants)
		{
			// C# 4.0 spec: §7.3.6.2
			var lhsUType = NullableType.GetUnderlyingType(lhs.Type);
			var rhsUType = NullableType.GetUnderlyingType(rhs.Type);
			TypeCode lhsCode = ReflectionHelper.GetTypeCode(lhsUType);
			TypeCode rhsCode = ReflectionHelper.GetTypeCode(rhsUType);
			// Treat C# 9 native integers as falling between int and long.
			// However they don't have a TypeCode, so we hack around that here:
			if (lhsUType.Kind == TypeKind.NInt)
			{
				lhsCode = TypeCode.Int32;
			}
			else if (lhsUType.Kind == TypeKind.NUInt)
			{
				lhsCode = TypeCode.UInt32;
			}
			if (rhsUType.Kind == TypeKind.NInt)
			{
				rhsCode = TypeCode.Int32;
			}
			else if (rhsUType.Kind == TypeKind.NUInt)
			{
				rhsCode = TypeCode.UInt32;
			}
			// if one of the inputs is the null literal, promote that to the type of the other operand
			if (isNullable && lhs.Type.Kind == TypeKind.Null && rhsCode >= TypeCode.Boolean && rhsCode <= TypeCode.Decimal)
			{
				lhs = CastTo(rhsCode, isNullable, lhs, allowNullableConstants);
				lhsCode = rhsCode;
			}
			else if (isNullable && rhs.Type.Kind == TypeKind.Null && lhsCode >= TypeCode.Boolean && lhsCode <= TypeCode.Decimal)
			{
				rhs = CastTo(lhsCode, isNullable, rhs, allowNullableConstants);
				rhsCode = lhsCode;
			}
			bool bindingError = false;
			if (lhsCode >= TypeCode.Char && lhsCode <= TypeCode.Decimal
				&& rhsCode >= TypeCode.Char && rhsCode <= TypeCode.Decimal)
			{
				TypeCode targetType;
				if (lhsCode == TypeCode.Decimal || rhsCode == TypeCode.Decimal)
				{
					targetType = TypeCode.Decimal;
					bindingError = (lhsCode == TypeCode.Single || lhsCode == TypeCode.Double
									|| rhsCode == TypeCode.Single || rhsCode == TypeCode.Double);
				}
				else if (lhsCode == TypeCode.Double || rhsCode == TypeCode.Double)
				{
					targetType = TypeCode.Double;
				}
				else if (lhsCode == TypeCode.Single || rhsCode == TypeCode.Single)
				{
					targetType = TypeCode.Single;
				}
				else if (lhsCode == TypeCode.UInt64 || rhsCode == TypeCode.UInt64)
				{
					targetType = TypeCode.UInt64;
					bindingError = IsSigned(lhsCode, lhs) || IsSigned(rhsCode, rhs);
				}
				else if (lhsUType.Kind == TypeKind.NUInt || rhsUType.Kind == TypeKind.NUInt)
				{
					bindingError = IsSigned(lhsCode, lhs) || IsSigned(rhsCode, rhs);
					lhs = CastTo(SpecialType.NUInt, isNullable, lhs, allowNullableConstants);
					rhs = CastTo(SpecialType.NUInt, isNullable, rhs, allowNullableConstants);
					return !bindingError;
				}
				else if (lhsCode == TypeCode.UInt32 || rhsCode == TypeCode.UInt32)
				{
					targetType = (IsSigned(lhsCode, lhs) || IsSigned(rhsCode, rhs)) ? TypeCode.Int64 : TypeCode.UInt32;
				}
				else if (lhsCode == TypeCode.Int64 || rhsCode == TypeCode.Int64)
				{
					targetType = TypeCode.Int64;
				}
				else if (lhsUType.Kind == TypeKind.NInt || rhsUType.Kind == TypeKind.NInt)
				{
					lhs = CastTo(SpecialType.NInt, isNullable, lhs, allowNullableConstants);
					rhs = CastTo(SpecialType.NInt, isNullable, rhs, allowNullableConstants);
					return !bindingError;
				}
				else
				{
					targetType = TypeCode.Int32;
				}
				lhs = CastTo(targetType, isNullable, lhs, allowNullableConstants);
				rhs = CastTo(targetType, isNullable, rhs, allowNullableConstants);
			}
			return !bindingError;
		}

		bool IsSigned(TypeCode code, ResolveResult rr)
		{
			// Determine whether the rr with code==ReflectionHelper.GetTypeCode(NullableType.GetUnderlyingType(rr.Type))
			// is a signed primitive type.
			switch (code)
			{
				case TypeCode.SByte:
				case TypeCode.Int16:
					return true;
				case TypeCode.Int32:
					// for int, consider implicit constant expression conversion
					if (rr.IsCompileTimeConstant && rr.ConstantValue != null && (int)rr.ConstantValue >= 0)
						return false;
					else
						return true;
				case TypeCode.Int64:
					// for long, consider implicit constant expression conversion
					if (rr.IsCompileTimeConstant && rr.ConstantValue != null && (long)rr.ConstantValue >= 0)
						return false;
					else
						return true;
				default:
					return false;
			}
		}

		ResolveResult CastTo(TypeCode targetType, bool isNullable, ResolveResult expression, bool allowNullableConstants)
		{
			return CastTo(compilation.FindType(targetType), isNullable, expression, allowNullableConstants);
		}

		ResolveResult CastTo(IType targetType, bool isNullable, ResolveResult expression, bool allowNullableConstants)
		{
			IType nullableType = MakeNullable(targetType, isNullable);
			if (nullableType.Equals(expression.Type))
				return expression;
			if (allowNullableConstants && expression.IsCompileTimeConstant)
			{
				if (expression.ConstantValue == null)
					return new ConstantResolveResult(nullableType, null);
				ResolveResult rr = ResolveCast(targetType, expression);
				if (rr.IsError)
					return rr;
				if (rr.IsCompileTimeConstant)
					return new ConstantResolveResult(nullableType, rr.ConstantValue);
			}
			return Convert(expression, nullableType,
							isNullable ? Conversion.ImplicitNullableConversion : Conversion.ImplicitNumericConversion);
		}
		#endregion

		#region GetOverloadableOperatorName
		static string? GetOverloadableOperatorName(BinaryOperatorType op)
		{
			switch (op)
			{
				case BinaryOperatorType.Add:
					return "op_Addition";
				case BinaryOperatorType.Subtract:
					return "op_Subtraction";
				case BinaryOperatorType.Multiply:
					return "op_Multiply";
				case BinaryOperatorType.Divide:
					return "op_Division";
				case BinaryOperatorType.Modulus:
					return "op_Modulus";
				case BinaryOperatorType.BitwiseAnd:
					return "op_BitwiseAnd";
				case BinaryOperatorType.BitwiseOr:
					return "op_BitwiseOr";
				case BinaryOperatorType.ExclusiveOr:
					return "op_ExclusiveOr";
				case BinaryOperatorType.ShiftLeft:
					return "op_LeftShift";
				case BinaryOperatorType.ShiftRight:
					return "op_RightShift";
				case BinaryOperatorType.UnsignedShiftRight:
					return "op_UnsignedRightShift";
				case BinaryOperatorType.Equality:
					return "op_Equality";
				case BinaryOperatorType.InEquality:
					return "op_Inequality";
				case BinaryOperatorType.GreaterThan:
					return "op_GreaterThan";
				case BinaryOperatorType.LessThan:
					return "op_LessThan";
				case BinaryOperatorType.GreaterThanOrEqual:
					return "op_GreaterThanOrEqual";
				case BinaryOperatorType.LessThanOrEqual:
					return "op_LessThanOrEqual";
				default:
					return null;
			}
		}
		#endregion

		#region Null coalescing operator
		ResolveResult ResolveNullCoalescingOperator(ResolveResult lhs, ResolveResult rhs)
		{
			if (NullableType.IsNullable(lhs.Type))
			{
				IType a0 = NullableType.GetUnderlyingType(lhs.Type);
				if (TryConvert(ref rhs, a0))
				{
					return BinaryOperatorResolveResult(a0, lhs, BinaryOperatorType.NullCoalescing, rhs);
				}
			}
			if (TryConvert(ref rhs, lhs.Type))
			{
				return BinaryOperatorResolveResult(lhs.Type, lhs, BinaryOperatorType.NullCoalescing, rhs);
			}
			if (TryConvert(ref lhs, rhs.Type))
			{
				return BinaryOperatorResolveResult(rhs.Type, lhs, BinaryOperatorType.NullCoalescing, rhs);
			}
			else
			{
				return new ErrorResolveResult(lhs.Type);
			}
		}
		#endregion
		#endregion

		#region Get user-defined operator candidates
		public IEnumerable<IParameterizedMember> GetUserDefinedOperatorCandidates(IType type, string operatorName)
		{
			if (operatorName == null)
				return EmptyList<IMethod>.Instance;
			TypeCode c = ReflectionHelper.GetTypeCode(type);
			if (TypeCode.Boolean <= c && c <= TypeCode.Decimal)
			{
				// The .NET framework contains some of C#'s built-in operators as user-defined operators.
				// However, we must not use those as user-defined operators (we would skip numeric promotion).
				return EmptyList<IMethod>.Instance;
			}
			// C# 4.0 spec: §7.3.5 Candidate user-defined operators
			var operators = type.GetMethods(m => m.IsOperator && m.Name == operatorName).ToList();
			LiftUserDefinedOperators(operators);
			return operators;
		}

		void LiftUserDefinedOperators(List<IMethod> operators)
		{
			int nonLiftedMethodCount = operators.Count;
			// Construct lifted operators
			for (int i = 0; i < nonLiftedMethodCount; i++)
			{
				var liftedMethod = CSharpOperators.LiftUserDefinedOperator(operators[i]);
				if (liftedMethod != null)
					operators.Add(liftedMethod);
			}
		}

		ResolveResult CreateResolveResultForUserDefinedOperator(OverloadResolution r, System.Linq.Expressions.ExpressionType operatorType)
		{
			if (r.BestCandidateErrors != OverloadResolutionErrors.None)
				return r.CreateResolveResult(null);
			IMethod method = (IMethod)r.BestCandidate;
			return new OperatorResolveResult(method.ReturnType, operatorType, method,
											 isLiftedOperator: method is ILiftedOperator,
											 operands: r.GetArgumentsWithConversions());
		}
		#endregion

		#region ResolveCast
		bool TryConvert(ref ResolveResult rr, IType targetType)
		{
			Conversion c = conversions.ImplicitConversion(rr, targetType);
			if (c.IsValid)
			{
				rr = Convert(rr, targetType, c);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rr">The input resolve result that should be converted.
		/// If a conversion exists, it is applied to the resolve result</param>
		/// <param name="targetType">The target type that we should convert to</param>
		/// <param name="isNullable">Whether we are dealing with a lifted operator</param>
		/// <param name="enumRR">The resolve result that is enum-typed.
		/// If necessary, a nullable conversion is applied.</param>
		/// <param name="allowConversionFromConstantZero">
		/// Whether the conversion from the constant zero is allowed.
		/// </param>
		/// <returns>True if the conversion is successful; false otherwise.
		/// If the conversion is not successful, the ref parameters will not be modified.</returns>
		bool TryConvertEnum(ref ResolveResult rr, IType targetType, ref bool isNullable, ref ResolveResult enumRR, bool allowConversionFromConstantZero = true)
		{
			Conversion c;
			if (!isNullable)
			{
				// Try non-nullable
				c = conversions.ImplicitConversion(rr, targetType);
				if (c.IsValid && (allowConversionFromConstantZero || !c.IsEnumerationConversion))
				{
					rr = Convert(rr, targetType, c);
					return true;
				}
			}
			// make targetType nullable if it isn't already:
			if (!targetType.IsKnownType(KnownTypeCode.NullableOfT))
				targetType = NullableType.Create(compilation, targetType);

			c = conversions.ImplicitConversion(rr, targetType);
			if (c.IsValid && (allowConversionFromConstantZero || !c.IsEnumerationConversion))
			{
				rr = Convert(rr, targetType, c);
				isNullable = true;
				// Also convert the enum-typed RR to nullable, if it isn't already
				if (!enumRR.Type.IsKnownType(KnownTypeCode.NullableOfT))
				{
					var nullableType = NullableType.Create(compilation, enumRR.Type);
					enumRR = new ConversionResolveResult(nullableType, enumRR, Conversion.ImplicitNullableConversion);
				}
				return true;
			}
			return false;
		}

		ResolveResult Convert(ResolveResult rr, IType targetType)
		{
			return Convert(rr, targetType, conversions.ImplicitConversion(rr, targetType));
		}

		ResolveResult Convert(ResolveResult rr, IType targetType, Conversion c)
		{
			if (c == Conversion.IdentityConversion)
				return rr;
			else if (rr.IsCompileTimeConstant && c != Conversion.None && !c.IsUserDefined)
				return ResolveCast(targetType, rr);
			else
				return new ConversionResolveResult(targetType, rr, c, checkForOverflow);
		}

		public ResolveResult ResolveCast(IType targetType, ResolveResult expression)
		{
			// C# 4.0 spec: §7.7.6 Cast expressions
			Conversion c = conversions.ExplicitConversion(expression, targetType);
			if (expression.IsCompileTimeConstant && !c.IsUserDefined)
			{
				IType underlyingType = targetType.GetEnumUnderlyingType();
				TypeCode code = ReflectionHelper.GetTypeCode(underlyingType);
				if (code >= TypeCode.Boolean && code <= TypeCode.Decimal && expression.ConstantValue != null)
				{
					if (expression.ConstantValue is string)
					{
						return new ErrorResolveResult(targetType);
					}
					try
					{
						return new ConstantResolveResult(targetType, CSharpPrimitiveCast(code, expression.ConstantValue));
					}
					catch (OverflowException)
					{
						return new ErrorResolveResult(targetType);
					}
					catch (InvalidCastException)
					{
						return new ErrorResolveResult(targetType);
					}
				}
				else if (code == TypeCode.String)
				{
					if (expression.ConstantValue == null || expression.ConstantValue is string)
						return new ConstantResolveResult(targetType, expression.ConstantValue);
					else
						return new ErrorResolveResult(targetType);
				}
				else if ((underlyingType.Kind == TypeKind.NInt || underlyingType.Kind == TypeKind.NUInt) && expression.ConstantValue != null)
				{
					if (expression.ConstantValue is string)
					{
						return new ErrorResolveResult(targetType);
					}
					code = (underlyingType.Kind == TypeKind.NInt ? TypeCode.Int32 : TypeCode.UInt32);
					try
					{
						return new ConstantResolveResult(targetType, Util.CSharpPrimitiveCast.Cast(code, expression.ConstantValue, checkForOverflow: true));
					}
					catch (OverflowException)
					{
						// If constant value doesn't fit into 32-bits, the conversion is not a compile-time constant
						return new ConversionResolveResult(targetType, expression, c, checkForOverflow);
					}
					catch (InvalidCastException)
					{
						return new ErrorResolveResult(targetType);
					}
				}
			}
			return new ConversionResolveResult(targetType, expression, c, checkForOverflow);
		}

		internal object CSharpPrimitiveCast(TypeCode targetType, object input)
		{
			return Util.CSharpPrimitiveCast.Cast(targetType, input, this.CheckForOverflow);
		}
		#endregion

		#region ResolveSimpleName
		public ResolveResult ResolveSimpleName(string identifier, IReadOnlyList<IType> typeArguments, bool isInvocationTarget = false)
		{
			// C# 4.0 spec: §7.6.2 Simple Names

			return LookupSimpleNameOrTypeName(
				identifier, typeArguments,
				isInvocationTarget ? NameLookupMode.InvocationTarget : NameLookupMode.Expression);
		}

		public ResolveResult LookupSimpleNameOrTypeName(string identifier, IReadOnlyList<IType> typeArguments, NameLookupMode lookupMode)
		{
			// C# 4.0 spec: §3.8 Namespace and type names; §7.6.2 Simple Names

			if (identifier == null)
				throw new ArgumentNullException(nameof(identifier));
			if (typeArguments == null)
				throw new ArgumentNullException(nameof(typeArguments));

			int k = typeArguments.Count;

			if (k == 0)
			{
				if (lookupMode == NameLookupMode.Expression || lookupMode == NameLookupMode.InvocationTarget)
				{
					// Look in local variables
					foreach (Dictionary<string, IVariable> variables in localVariableStack)
					{
						if (variables.TryGetValue(identifier, out var v))
						{
							return new LocalResolveResult(v);
						}
					}
					// Look in parameters of current method
					IParameterizedMember? parameterizedMember = this.CurrentMember as IParameterizedMember;
					if (parameterizedMember != null)
					{
						foreach (IParameter p in parameterizedMember.Parameters)
						{
							if (p.Name == identifier)
							{
								return new LocalResolveResult(p);
							}
						}
					}
				}

				// look in type parameters of current method
				IMethod? m = this.CurrentMember as IMethod;
				if (m != null)
				{
					foreach (ITypeParameter tp in m.TypeParameters)
					{
						if (tp.Name == identifier)
							return new TypeResolveResult(tp);
					}
				}
			}

			bool parameterizeResultType = !(typeArguments.Count != 0 && typeArguments.All(t => t.Kind == TypeKind.UnboundTypeArgument));

			ResolveResult? r = null;
			if (currentTypeDefinitionCache != null)
			{
				Dictionary<string, ResolveResult>? cache = null;
				bool foundInCache = false;
				if (k == 0)
				{
					switch (lookupMode)
					{
						case NameLookupMode.Expression:
							cache = currentTypeDefinitionCache.SimpleNameLookupCacheExpression;
							break;
						case NameLookupMode.InvocationTarget:
							cache = currentTypeDefinitionCache.SimpleNameLookupCacheInvocationTarget;
							break;
						case NameLookupMode.Type:
							cache = currentTypeDefinitionCache.SimpleTypeLookupCache;
							break;
					}
					if (cache != null)
					{
						lock (cache)
							foundInCache = cache.TryGetValue(identifier, out r);
					}
				}
				if (foundInCache)
				{
					r = (r != null ? r.ShallowClone() : null);
				}
				else
				{
					r = LookInCurrentType(identifier, typeArguments, lookupMode, parameterizeResultType);
					if (cache != null)
					{
						// also cache missing members (r==null)
						lock (cache)
							cache[identifier] = r;
					}
				}
				if (r != null)
					return r;
			}

			if (context.CurrentUsingScope == null)
			{
				// If no using scope was specified, we still need to look in the global namespace:
				r = LookInUsingScopeNamespace(null, compilation.RootNamespace, identifier, typeArguments, parameterizeResultType);
			}
			else
			{
				if (k == 0 && lookupMode != NameLookupMode.TypeInUsingDeclaration)
				{
					if (context.CurrentUsingScope.ResolveCache.TryGetValue(identifier, out r))
					{
						r = (r != null ? r.ShallowClone() : null);
					}
					else
					{
						r = LookInCurrentUsingScope(identifier, typeArguments, false, false);
						context.CurrentUsingScope.ResolveCache.TryAdd(identifier, r);
					}
				}
				else
				{
					r = LookInCurrentUsingScope(identifier, typeArguments, lookupMode == NameLookupMode.TypeInUsingDeclaration, parameterizeResultType);
				}
			}
			if (r != null)
				return r;

			if (typeArguments.Count == 0 && identifier == "dynamic")
			{
				return new TypeResolveResult(SpecialType.Dynamic);
			}
			else
			{
				return new UnknownIdentifierResolveResult(identifier, typeArguments.Count);
			}
		}

		public bool IsVariableReferenceWithSameType(ResolveResult rr, string identifier, [NotNullWhen(true)] out TypeResolveResult? trr)
		{
			if (!(rr is MemberResolveResult || rr is LocalResolveResult))
			{
				trr = null;
				return false;
			}
			trr = LookupSimpleNameOrTypeName(identifier, EmptyList<IType>.Instance, NameLookupMode.Type) as TypeResolveResult;
			return trr != null && trr.Type.Equals(rr.Type);
		}

		ResolveResult? LookInCurrentType(string identifier, IReadOnlyList<IType> typeArguments, NameLookupMode lookupMode, bool parameterizeResultType)
		{
			int k = typeArguments.Count;
			MemberLookup lookup = CreateMemberLookup(lookupMode);
			// look in current type definitions
			for (ITypeDefinition? t = this.CurrentTypeDefinition; t != null; t = t.DeclaringTypeDefinition)
			{
				if (k == 0)
				{
					// Look for type parameter with that name
					var typeParameters = t.TypeParameters;
					// Look at all type parameters, including those copied from outer classes,
					// so that we can fetch the version with the correct owner.
					for (int i = 0; i < typeParameters.Count; i++)
					{
						if (typeParameters[i].Name == identifier)
							return new TypeResolveResult(typeParameters[i]);
					}
				}

				if (lookupMode == NameLookupMode.BaseTypeReference && t == this.CurrentTypeDefinition)
				{
					// don't look in current type when resolving a base type reference
					continue;
				}

				ResolveResult r;
				if (lookupMode == NameLookupMode.Expression || lookupMode == NameLookupMode.InvocationTarget)
				{
					var targetResolveResult = (t == this.CurrentTypeDefinition ? ResolveThisReference() : new TypeResolveResult(t));
					r = lookup.Lookup(targetResolveResult, identifier, typeArguments, lookupMode == NameLookupMode.InvocationTarget);
				}
				else
				{
					r = lookup.LookupType(t, identifier, typeArguments, parameterizeResultType);
				}
				if (!(r is UnknownMemberResolveResult)) // but do return AmbiguousMemberResolveResult
					return r;
			}
			return null;
		}

		ResolveResult? LookInCurrentUsingScope(string identifier, IReadOnlyList<IType> typeArguments, bool isInUsingDeclaration, bool parameterizeResultType)
		{
			// look in current namespace definitions
			ResolvedUsingScope currentUsingScope = this.CurrentUsingScope;
			for (ResolvedUsingScope u = currentUsingScope; u != null; u = u.Parent)
			{
				var resultInNamespace = LookInUsingScopeNamespace(u, u.Namespace, identifier, typeArguments, parameterizeResultType);
				if (resultInNamespace != null)
					return resultInNamespace;
				// then look for aliases:
				if (typeArguments.Count == 0)
				{
					if (u.ExternAliases.Contains(identifier))
					{
						return ResolveExternAlias(identifier);
					}
					if (!(isInUsingDeclaration && u == currentUsingScope))
					{
						foreach (var pair in u.UsingAliases)
						{
							if (pair.Key == identifier)
							{
								return pair.Value.ShallowClone();
							}
						}
					}
				}
				// finally, look in the imported namespaces:
				if (!(isInUsingDeclaration && u == currentUsingScope))
				{
					IType? firstResult = null;
					foreach (var importedNamespace in u.Usings)
					{
						ITypeDefinition? def = importedNamespace.GetTypeDefinition(identifier, typeArguments.Count);
						if (def != null)
						{
							IType resultType;
							if (parameterizeResultType && typeArguments.Count > 0)
								resultType = new ParameterizedType(def, typeArguments);
							else
								resultType = def;

							if (firstResult == null || !TopLevelTypeDefinitionIsAccessible(firstResult.GetDefinition()))
							{
								if (TopLevelTypeDefinitionIsAccessible(resultType.GetDefinition()))
									firstResult = resultType;
							}
							else if (TopLevelTypeDefinitionIsAccessible(def))
							{
								return new AmbiguousTypeResolveResult(firstResult);
							}
						}
					}
					if (firstResult != null)
						return new TypeResolveResult(firstResult);
				}
				// if we didn't find anything: repeat lookup with parent namespace
			}
			return null;
		}

		ResolveResult? LookInUsingScopeNamespace(ResolvedUsingScope? usingScope, INamespace n, string identifier, IReadOnlyList<IType> typeArguments, bool parameterizeResultType)
		{
			if (n == null)
				return null;
			// first look for a namespace
			int k = typeArguments.Count;
			if (k == 0)
			{
				INamespace? childNamespace = n.GetChildNamespace(identifier);
				if (childNamespace != null)
				{
					if (usingScope != null && usingScope.HasAlias(identifier))
						return new AmbiguousTypeResolveResult(new UnknownType(null, identifier));
					return new NamespaceResolveResult(childNamespace);
				}
			}
			// then look for a type
			ITypeDefinition? def = n.GetTypeDefinition(identifier, k);
			if (def != null && TopLevelTypeDefinitionIsAccessible(def))
			{
				IType result = def;
				if (parameterizeResultType && k > 0)
				{
					result = new ParameterizedType(def, typeArguments);
				}
				if (usingScope != null && usingScope.HasAlias(identifier))
					return new AmbiguousTypeResolveResult(result);
				else
					return new TypeResolveResult(result);
			}
			return null;
		}

		bool TopLevelTypeDefinitionIsAccessible(ITypeDefinition typeDef)
		{
			if (typeDef.Accessibility == Accessibility.Internal)
			{
				return typeDef.ParentModule.InternalsVisibleTo(compilation.MainModule);
			}
			return true;
		}

		/// <summary>
		/// Looks up an alias (identifier in front of :: operator)
		/// </summary>
		public ResolveResult ResolveAlias(string identifier)
		{
			if (identifier == "global")
				return new NamespaceResolveResult(compilation.RootNamespace);

			for (ResolvedUsingScope n = this.CurrentUsingScope; n != null; n = n.Parent)
			{
				if (n.ExternAliases.Contains(identifier))
				{
					return ResolveExternAlias(identifier);
				}
				foreach (var pair in n.UsingAliases)
				{
					if (pair.Key == identifier)
					{
						return (pair.Value as NamespaceResolveResult) ?? ErrorResult;
					}
				}
			}
			return ErrorResult;
		}

		ResolveResult ResolveExternAlias(string alias)
		{
			INamespace? ns = compilation.GetNamespaceForExternAlias(alias);
			if (ns != null)
				return new NamespaceResolveResult(ns);
			else
				return ErrorResult;
		}
		#endregion

		#region ResolveMemberAccess
		public ResolveResult ResolveMemberAccess(ResolveResult target, string identifier, IReadOnlyList<IType> typeArguments, NameLookupMode lookupMode = NameLookupMode.Expression)
		{
			// C# 4.0 spec: §7.6.4

			bool parameterizeResultType = !(typeArguments.Count != 0 && typeArguments.All(t => t.Kind == TypeKind.UnboundTypeArgument));
			NamespaceResolveResult? nrr = target as NamespaceResolveResult;
			if (nrr != null)
			{
				return ResolveMemberAccessOnNamespace(nrr, identifier, typeArguments, parameterizeResultType);
			}

			if (target.Type.Kind == TypeKind.Dynamic)
				return new DynamicMemberResolveResult(target, identifier);

			MemberLookup lookup = CreateMemberLookup(lookupMode);
			ResolveResult result;
			switch (lookupMode)
			{
				case NameLookupMode.Expression:
					result = lookup.Lookup(target, identifier, typeArguments, isInvocation: false);
					break;
				case NameLookupMode.InvocationTarget:
					result = lookup.Lookup(target, identifier, typeArguments, isInvocation: true);
					break;
				case NameLookupMode.Type:
				case NameLookupMode.TypeInUsingDeclaration:
				case NameLookupMode.BaseTypeReference:
					// Don't do the UnknownMemberResolveResult/MethodGroupResolveResult processing,
					// it's only relevant for expressions.
					return lookup.LookupType(target.Type, identifier, typeArguments, parameterizeResultType);
				default:
					throw new NotSupportedException("Invalid value for NameLookupMode");
			}
			if (result is UnknownMemberResolveResult)
			{
				// We intentionally use all extension methods here, not just the eligible ones.
				// Proper eligibility checking is only possible for the full invocation
				// (after we know the remaining arguments).
				// The eligibility check in GetExtensionMethods is only intended for code completion.
				var extensionMethods = GetExtensionMethods(identifier, typeArguments);
				if (extensionMethods.Count > 0)
				{
					return new MethodGroupResolveResult(target, identifier, EmptyList<MethodListWithDeclaringType>.Instance, typeArguments) {
						extensionMethods = extensionMethods
					};
				}
			}
			else
			{
				MethodGroupResolveResult? mgrr = result as MethodGroupResolveResult;
				if (mgrr != null)
				{
					Debug.Assert(mgrr.extensionMethods == null);
					// set the values that are necessary to make MethodGroupResolveResult.GetExtensionMethods() work
					mgrr.resolver = this;
				}
			}
			return result;
		}

		ResolveResult ResolveMemberAccessOnNamespace(NamespaceResolveResult nrr, string identifier, IReadOnlyList<IType> typeArguments, bool parameterizeResultType)
		{
			if (typeArguments.Count == 0)
			{
				INamespace? childNamespace = nrr.Namespace.GetChildNamespace(identifier);
				if (childNamespace != null)
					return new NamespaceResolveResult(childNamespace);
			}
			ITypeDefinition? def = nrr.Namespace.GetTypeDefinition(identifier, typeArguments.Count);
			if (def != null)
			{
				if (parameterizeResultType && typeArguments.Count > 0)
					return new TypeResolveResult(new ParameterizedType(def, typeArguments));
				else
					return new TypeResolveResult(def);
			}
			return ErrorResult;
		}

		/// <summary>
		/// Creates a MemberLookup instance using this resolver's settings.
		/// </summary>
		public MemberLookup CreateMemberLookup()
		{
			ITypeDefinition currentTypeDefinition = this.CurrentTypeDefinition;
			bool isInEnumMemberInitializer = this.CurrentMember != null && this.CurrentMember.SymbolKind == SymbolKind.Field
				&& currentTypeDefinition != null && currentTypeDefinition.Kind == TypeKind.Enum;
			return new MemberLookup(currentTypeDefinition, this.Compilation.MainModule, isInEnumMemberInitializer);
		}

		/// <summary>
		/// Creates a MemberLookup instance using this resolver's settings.
		/// </summary>
		public MemberLookup CreateMemberLookup(NameLookupMode lookupMode)
		{
			if (lookupMode == NameLookupMode.BaseTypeReference && this.CurrentTypeDefinition != null)
			{
				// When looking up a base type reference, treat us as being outside the current type definition
				// for accessibility purposes.
				// This avoids a stack overflow when referencing a protected class nested inside the base class
				// of a parent class. (NameLookupTests.InnerClassInheritingFromProtectedBaseInnerClassShouldNotCauseStackOverflow)
				return new MemberLookup(this.CurrentTypeDefinition.DeclaringTypeDefinition, this.Compilation.MainModule, false);
			}
			else
			{
				return CreateMemberLookup();
			}
		}
		#endregion

		#region ResolveIdentifierInObjectInitializer
		public ResolveResult ResolveIdentifierInObjectInitializer(string identifier)
		{
			MemberLookup memberLookup = CreateMemberLookup();
			return memberLookup.Lookup(this.CurrentObjectInitializer, identifier, EmptyList<IType>.Instance, false);
		}
		#endregion

		#region ResolveForeach
		public ForEachResolveResult ResolveForeach(ResolveResult expression)
		{
			var memberLookup = CreateMemberLookup();

			IType collectionType, enumeratorType, elementType;
			ResolveResult getEnumeratorInvocation;
			ResolveResult? currentRR = null;
			// C# 4.0 spec: §8.8.4 The foreach statement
			if (expression.Type.Kind == TypeKind.Array || expression.Type.Kind == TypeKind.Dynamic)
			{
				collectionType = compilation.FindType(KnownTypeCode.IEnumerable);
				enumeratorType = compilation.FindType(KnownTypeCode.IEnumerator);
				if (expression.Type.Kind == TypeKind.Array)
				{
					elementType = ((ArrayType)expression.Type).ElementType;
				}
				else
				{
					elementType = SpecialType.Dynamic;
				}
				getEnumeratorInvocation = ResolveCast(collectionType, expression);
				getEnumeratorInvocation = ResolveMemberAccess(getEnumeratorInvocation, "GetEnumerator", EmptyList<IType>.Instance, NameLookupMode.InvocationTarget);
				getEnumeratorInvocation = ResolveInvocation(getEnumeratorInvocation, Empty<ResolveResult>.Array);
			}
			else
			{
				var getEnumeratorMethodGroup = memberLookup.Lookup(expression, "GetEnumerator", EmptyList<IType>.Instance, true) as MethodGroupResolveResult;
				if (getEnumeratorMethodGroup != null)
				{
					var or = getEnumeratorMethodGroup.PerformOverloadResolution(
						compilation, Empty<ResolveResult>.Array,
						allowExtensionMethods: false, allowExpandingParams: false, allowOptionalParameters: false);
					if (or.FoundApplicableCandidate && !or.IsAmbiguous && !or.BestCandidate.IsStatic && or.BestCandidate.Accessibility == Accessibility.Public)
					{
						collectionType = expression.Type;
						getEnumeratorInvocation = or.CreateResolveResult(expression);
						enumeratorType = getEnumeratorInvocation.Type;
						currentRR = memberLookup.Lookup(new ResolveResult(enumeratorType), "Current", EmptyList<IType>.Instance, false);
						elementType = currentRR.Type;
					}
					else
					{
						CheckForEnumerableInterface(expression, out collectionType, out enumeratorType, out elementType, out getEnumeratorInvocation);
					}
				}
				else
				{
					CheckForEnumerableInterface(expression, out collectionType, out enumeratorType, out elementType, out getEnumeratorInvocation);
				}
			}
			IMethod? moveNextMethod = null;
			var moveNextMethodGroup = memberLookup.Lookup(new ResolveResult(enumeratorType), "MoveNext", EmptyList<IType>.Instance, false) as MethodGroupResolveResult;
			if (moveNextMethodGroup != null)
			{
				var or = moveNextMethodGroup.PerformOverloadResolution(
					compilation, Empty<ResolveResult>.Array,
					allowExtensionMethods: false, allowExpandingParams: false, allowOptionalParameters: false);
				moveNextMethod = or.GetBestCandidateWithSubstitutedTypeArguments() as IMethod;
			}

			if (currentRR == null)
				currentRR = memberLookup.Lookup(new ResolveResult(enumeratorType), "Current", EmptyList<IType>.Instance, false);
			IProperty? currentProperty = null;
			if (currentRR is MemberResolveResult)
				currentProperty = ((MemberResolveResult)currentRR).Member as IProperty;

			var voidType = compilation.FindType(KnownTypeCode.Void);
			return new ForEachResolveResult(getEnumeratorInvocation, collectionType, enumeratorType, elementType,
											currentProperty, moveNextMethod, voidType);
		}

		void CheckForEnumerableInterface(ResolveResult expression, out IType collectionType, out IType enumeratorType, out IType elementType, out ResolveResult getEnumeratorInvocation)
		{
			elementType = expression.Type.GetElementTypeFromIEnumerable(compilation, false, out bool? isGeneric);
			if (isGeneric == true)
			{
				ITypeDefinition? enumerableOfT = compilation.FindType(KnownTypeCode.IEnumerableOfT).GetDefinition();
				if (enumerableOfT != null)
					collectionType = new ParameterizedType(enumerableOfT, new[] { elementType });
				else
					collectionType = SpecialType.UnknownType;

				ITypeDefinition? enumeratorOfT = compilation.FindType(KnownTypeCode.IEnumeratorOfT).GetDefinition();
				if (enumeratorOfT != null)
					enumeratorType = new ParameterizedType(enumeratorOfT, new[] { elementType });
				else
					enumeratorType = SpecialType.UnknownType;
			}
			else if (isGeneric == false)
			{
				collectionType = compilation.FindType(KnownTypeCode.IEnumerable);
				enumeratorType = compilation.FindType(KnownTypeCode.IEnumerator);
			}
			else
			{
				collectionType = SpecialType.UnknownType;
				enumeratorType = SpecialType.UnknownType;
			}
			getEnumeratorInvocation = ResolveCast(collectionType, expression);
			getEnumeratorInvocation = ResolveMemberAccess(getEnumeratorInvocation, "GetEnumerator", EmptyList<IType>.Instance, NameLookupMode.InvocationTarget);
			getEnumeratorInvocation = ResolveInvocation(getEnumeratorInvocation, Empty<ResolveResult>.Array);
		}
		#endregion

		#region GetExtensionMethods
		/// <summary>
		/// Gets all extension methods that are available in the current context.
		/// </summary>
		/// <param name="name">Name of the extension method. Pass null to retrieve all extension methods.</param>
		/// <param name="typeArguments">Explicitly provided type arguments.
		/// An empty list will return all matching extension method definitions;
		/// a non-empty list will return <see cref="SpecializedMethod"/>s for all extension methods
		/// with the matching number of type parameters.</param>
		/// <remarks>
		/// The results are stored in nested lists because they are grouped by using scope.
		/// That is, for "using SomeExtensions; namespace X { using MoreExtensions; ... }",
		/// the return value will be
		/// new List {
		///    new List { all extensions from MoreExtensions },
		///    new List { all extensions from SomeExtensions }
		/// }
		/// </remarks>
		public List<List<IMethod>> GetExtensionMethods(string? name = null, IReadOnlyList<IType>? typeArguments = null)
		{
			return GetExtensionMethods(null, name, typeArguments);
		}

		/// <summary>
		/// Gets the extension methods that are called 'name'
		/// and are applicable with a first argument type of 'targetType'.
		/// </summary>
		/// <param name="targetType">Type of the 'this' argument</param>
		/// <param name="name">Name of the extension method. Pass null to retrieve all extension methods.</param>
		/// <param name="typeArguments">Explicitly provided type arguments.
		/// An empty list will return all matching extension method definitions;
		/// a non-empty list will return <see cref="SpecializedMethod"/>s for all extension methods
		/// with the matching number of type parameters.</param>
		/// <param name="substituteInferredTypes">
		/// Specifies whether to produce a <see cref="SpecializedMethod"/>
		/// when type arguments could be inferred from <paramref name="targetType"/>. This parameter
		/// is only used for inferred types and has no effect if <paramref name="typeArguments"/> is non-empty.
		/// </param>
		/// <remarks>
		/// The results are stored in nested lists because they are grouped by using scope.
		/// That is, for "using SomeExtensions; namespace X { using MoreExtensions; ... }",
		/// the return value will be
		/// new List {
		///    new List { all extensions from MoreExtensions },
		///    new List { all extensions from SomeExtensions }
		/// }
		/// </remarks>
		public List<List<IMethod>> GetExtensionMethods(IType? targetType, string? name = null, IReadOnlyList<IType>? typeArguments = null, bool substituteInferredTypes = false)
		{
			var lookup = CreateMemberLookup();
			List<List<IMethod>> extensionMethodGroups = new List<List<IMethod>>();
			foreach (var inputGroup in GetAllExtensionMethods(lookup))
			{
				List<IMethod> outputGroup = new List<IMethod>();
				foreach (var method in inputGroup)
				{
					if (name != null && method.Name != name)
						continue;
					if (!lookup.IsAccessible(method, false))
						continue;
					IType[]? inferredTypes;
					if (typeArguments != null && typeArguments.Count > 0)
					{
						if (method.TypeParameters.Count != typeArguments.Count)
							continue;
						var sm = method.Specialize(new TypeParameterSubstitution(null, typeArguments));
						if (IsEligibleExtensionMethod(compilation, conversions, targetType, sm, false, out inferredTypes))
							outputGroup.Add(sm);
					}
					else
					{
						if (IsEligibleExtensionMethod(compilation, conversions, targetType, method, true, out inferredTypes))
						{
							if (substituteInferredTypes && inferredTypes != null)
							{
								outputGroup.Add(method.Specialize(new TypeParameterSubstitution(null, inferredTypes)));
							}
							else
							{
								outputGroup.Add(method);
							}
						}
					}
				}
				if (outputGroup.Count > 0)
					extensionMethodGroups.Add(outputGroup);
			}
			return extensionMethodGroups;
		}

		/// <summary>
		/// Checks whether the specified extension method is eligible on the target type.
		/// </summary>
		/// <param name="targetType">Target type that is passed as first argument to the extension method.</param>
		/// <param name="method">The extension method.</param>
		/// <param name="useTypeInference">Whether to perform type inference for the method.
		/// Use <c>false</c> if <paramref name="method"/> is already parameterized (e.g. when type arguments were given explicitly).
		/// Otherwise, use <c>true</c>.
		/// </param>
		/// <param name="outInferredTypes">If the method is generic and <paramref name="useTypeInference"/> is <c>true</c>,
		/// and at least some of the type arguments could be inferred, this parameter receives the inferred type arguments.
		/// Since only the type for the first parameter is considered, not all type arguments may be inferred.
		/// If an array is returned, any slot with an uninferred type argument will be set to the method's
		/// corresponding type parameter.
		/// </param>
		public static bool IsEligibleExtensionMethod(IType targetType, IMethod method, bool useTypeInference, out IType[]? outInferredTypes)
		{
			if (targetType == null)
				throw new ArgumentNullException(nameof(targetType));
			if (method == null)
				throw new ArgumentNullException(nameof(method));
			var compilation = method.Compilation;
			return IsEligibleExtensionMethod(compilation, CSharpConversions.Get(compilation), targetType, method, useTypeInference, out outInferredTypes);
		}

		static bool IsEligibleExtensionMethod(ICompilation compilation, CSharpConversions conversions, IType? targetType, IMethod method, bool useTypeInference, out IType[]? outInferredTypes)
		{
			outInferredTypes = null;
			if (targetType == null)
				return true;
			if (method.Parameters.Count == 0)
				return false;
			IType thisParameterType = method.Parameters[0].Type;
			if (thisParameterType.Kind == TypeKind.ByReference)
			{
				// extension method with `this in` or `this ref`
				thisParameterType = ((ByReferenceType)thisParameterType).ElementType;
			}
			if (useTypeInference && method.TypeParameters.Count > 0)
			{
				// We need to infer type arguments from targetType:
				TypeInference ti = new TypeInference(compilation, conversions);
				ResolveResult[] arguments = { new ResolveResult(targetType) };
				IType[] parameterTypes = { thisParameterType };
				var inferredTypes = ti.InferTypeArguments(method.TypeParameters, arguments, parameterTypes, out _);
				var substitution = new TypeParameterSubstitution(null, inferredTypes);
				// Validate that the types that could be inferred (aren't unknown) satisfy the constraints:
				bool hasInferredTypes = false;
				for (int i = 0; i < inferredTypes.Length; i++)
				{
					if (inferredTypes[i].Kind != TypeKind.Unknown && inferredTypes[i].Kind != TypeKind.UnboundTypeArgument)
					{
						hasInferredTypes = true;
						if (!OverloadResolution.ValidateConstraints(method.TypeParameters[i], inferredTypes[i], substitution, conversions))
							return false;
					}
					else
					{
						inferredTypes[i] = method.TypeParameters[i]; // do not substitute types that could not be inferred
					}
				}
				if (hasInferredTypes)
					outInferredTypes = inferredTypes;
				thisParameterType = thisParameterType.AcceptVisitor(substitution);
			}
			Conversion c = conversions.ImplicitConversion(targetType, thisParameterType);
			return c.IsValid && (c.IsIdentityConversion || c.IsReferenceConversion || c.IsBoxingConversion);
		}

		/// <summary>
		/// Gets all extension methods available in the current using scope.
		/// This list includes inaccessible methods.
		/// </summary>
		IList<List<IMethod>> GetAllExtensionMethods(MemberLookup lookup)
		{
			var currentUsingScope = context.CurrentUsingScope;
			if (currentUsingScope == null)
				return EmptyList<List<IMethod>>.Instance;
			List<List<IMethod>> extensionMethodGroups = LazyInit.VolatileRead(ref currentUsingScope.AllExtensionMethods);
			if (extensionMethodGroups != null)
			{
				return extensionMethodGroups;
			}
			extensionMethodGroups = new List<List<IMethod>>();
			List<IMethod> m;
			for (ResolvedUsingScope scope = currentUsingScope; scope != null; scope = scope.Parent)
			{
				INamespace ns = scope.Namespace;
				if (ns != null)
				{
					m = GetExtensionMethods(lookup, ns).ToList();
					if (m.Count > 0)
						extensionMethodGroups.Add(m);
				}

				m = scope.Usings
					.Distinct()
					.SelectMany(importedNamespace => GetExtensionMethods(lookup, importedNamespace))
					.ToList();
				if (m.Count > 0)
					extensionMethodGroups.Add(m);
			}
			return LazyInit.GetOrSet(ref currentUsingScope.AllExtensionMethods, extensionMethodGroups);
		}

		IEnumerable<IMethod> GetExtensionMethods(MemberLookup lookup, INamespace ns)
		{
			// TODO: maybe make this a property on INamespace?
			return
				from c in ns.Types
				where c.IsStatic && c.HasExtensionMethods && c.TypeParameters.Count == 0 && lookup.IsAccessible(c, false)
				from m in c.Methods
				where m.IsExtensionMethod
				select m;
		}
		#endregion

		#region ResolveInvocation

		IList<ResolveResult> AddArgumentNamesIfNecessary(ResolveResult[] arguments, string[]? argumentNames)
		{
			if (argumentNames == null)
			{
				return arguments;
			}
			else
			{
				var result = new ResolveResult[arguments.Length];
				for (int i = 0; i < arguments.Length; i++)
				{
					result[i] = (argumentNames[i] != null ? new NamedArgumentResolveResult(argumentNames[i], arguments[i]) : arguments[i]);
				}
				return result;
			}
		}

		private ResolveResult ResolveInvocation(ResolveResult target, ResolveResult[] arguments, string[]? argumentNames, bool allowOptionalParameters)
		{
			// C# 4.0 spec: §7.6.5

			if (target.Type.Kind == TypeKind.Dynamic)
			{
				return new DynamicInvocationResolveResult(target, DynamicInvocationType.Invocation, AddArgumentNamesIfNecessary(arguments, argumentNames));
			}

			bool isDynamic = arguments.Any(a => a.Type.Kind == TypeKind.Dynamic);
			MethodGroupResolveResult? mgrr = target as MethodGroupResolveResult;
			if (mgrr != null)
			{
				if (isDynamic)
				{
					// If we have dynamic arguments, we need to represent the invocation as a dynamic invocation if there is more than one applicable method.
					var or2 = CreateOverloadResolution(arguments, argumentNames, mgrr.TypeArguments.ToArray());
					var applicableMethods = mgrr.MethodsGroupedByDeclaringType.SelectMany(m => m, (x, m) => new { x.DeclaringType, Method = m }).Where(x => OverloadResolution.IsApplicable(or2.AddCandidate(x.Method))).ToList();

					if (applicableMethods.Count > 1)
					{
						ResolveResult actualTarget;
						if (applicableMethods.All(x => x.Method.IsStatic) && !(mgrr.TargetResult is TypeResolveResult))
							actualTarget = new TypeResolveResult(mgrr.TargetType);
						else
							actualTarget = mgrr.TargetResult;

						var l = new List<MethodListWithDeclaringType>();
						foreach (var m in applicableMethods)
						{
							if (l.Count == 0 || l[l.Count - 1].DeclaringType != m.DeclaringType)
								l.Add(new MethodListWithDeclaringType(m.DeclaringType));
							l[l.Count - 1].Add(m.Method);
						}
						return new DynamicInvocationResolveResult(new MethodGroupResolveResult(actualTarget, mgrr.MethodName, l, mgrr.TypeArguments), DynamicInvocationType.Invocation, AddArgumentNamesIfNecessary(arguments, argumentNames));
					}
				}

				OverloadResolution or = mgrr.PerformOverloadResolution(compilation, arguments, argumentNames, checkForOverflow: checkForOverflow, conversions: conversions, allowOptionalParameters: allowOptionalParameters);
				if (or.BestCandidate != null)
				{
					if (or.BestCandidate.IsStatic && !or.IsExtensionMethodInvocation && !(mgrr.TargetResult is TypeResolveResult))
						return or.CreateResolveResult(new TypeResolveResult(mgrr.TargetType), returnTypeOverride: isDynamic ? SpecialType.Dynamic : null);
					else
						return or.CreateResolveResult(mgrr.TargetResult, returnTypeOverride: isDynamic ? SpecialType.Dynamic : null);
				}
				else
				{
					// No candidate found at all (not even an inapplicable one).
					// This can happen with empty method groups (as sometimes used with extension methods)
					return new UnknownMethodResolveResult(
						mgrr.TargetType, mgrr.MethodName, mgrr.TypeArguments, CreateParameters(arguments, argumentNames));
				}
			}
			UnknownMemberResolveResult? umrr = target as UnknownMemberResolveResult;
			if (umrr != null)
			{
				return new UnknownMethodResolveResult(umrr.TargetType, umrr.MemberName, umrr.TypeArguments, CreateParameters(arguments, argumentNames));
			}
			UnknownIdentifierResolveResult? uirr = target as UnknownIdentifierResolveResult;
			if (uirr != null && CurrentTypeDefinition != null)
			{
				return new UnknownMethodResolveResult(CurrentTypeDefinition, uirr.Identifier, EmptyList<IType>.Instance, CreateParameters(arguments, argumentNames));
			}
			IMethod invokeMethod = target.Type.GetDelegateInvokeMethod();
			if (invokeMethod != null)
			{
				OverloadResolution or = CreateOverloadResolution(arguments, argumentNames);
				or.AddCandidate(invokeMethod);
				return new CSharpInvocationResolveResult(
					target, invokeMethod, //invokeMethod.ReturnType.Resolve(context),
					or.GetArgumentsWithConversionsAndNames(), or.BestCandidateErrors,
					isExpandedForm: or.BestCandidateIsExpandedForm,
					isDelegateInvocation: true,
					argumentToParameterMap: or.GetArgumentToParameterMap(),
					returnTypeOverride: isDynamic ? SpecialType.Dynamic : null);
			}
			return ErrorResult;
		}

		/// <summary>
		/// Resolves an invocation.
		/// </summary>
		/// <param name="target">The target of the invocation. Usually a MethodGroupResolveResult.</param>
		/// <param name="arguments">
		/// Arguments passed to the method.
		/// The resolver may mutate this array to wrap elements in <see cref="ConversionResolveResult"/>s!
		/// </param>
		/// <param name="argumentNames">
		/// The argument names. Pass the null string for positional arguments.
		/// </param>
		/// <returns>InvocationResolveResult or UnknownMethodResolveResult</returns>
		public ResolveResult ResolveInvocation(ResolveResult target, ResolveResult[] arguments, string[]? argumentNames = null)
		{
			return ResolveInvocation(target, arguments, argumentNames, allowOptionalParameters: true);
		}

		List<IParameter> CreateParameters(ResolveResult[] arguments, string[] argumentNames)
		{
			List<IParameter> list = new List<IParameter>();
			if (argumentNames == null)
			{
				argumentNames = new string[arguments.Length];
			}
			else
			{
				if (argumentNames.Length != arguments.Length)
					throw new ArgumentException();
				argumentNames = (string[])argumentNames.Clone();
			}
			for (int i = 0; i < arguments.Length; i++)
			{
				// invent argument names where necessary:
				if (argumentNames[i] == null)
				{
					string newArgumentName = GuessParameterName(arguments[i]);
					if (argumentNames.Contains(newArgumentName))
					{
						// disambiguate argument name (e.g. add a number)
						int num = 1;
						string newName;
						do
						{
							newName = newArgumentName + num.ToString();
							num++;
						} while (argumentNames.Contains(newName));
						newArgumentName = newName;
					}
					argumentNames[i] = newArgumentName;
				}

				// create the parameter:
				ByReferenceResolveResult? brrr = arguments[i] as ByReferenceResolveResult;
				if (brrr != null)
				{
					list.Add(new DefaultParameter(arguments[i].Type, argumentNames[i], referenceKind: brrr.ReferenceKind));
				}
				else
				{
					// argument might be a lambda or delegate type, so we have to try to guess the delegate type
					IType type = arguments[i].Type;
					if (type.Kind == TypeKind.Null || type.Kind == TypeKind.None)
					{
						list.Add(new DefaultParameter(compilation.FindType(KnownTypeCode.Object), argumentNames[i]));
					}
					else
					{
						list.Add(new DefaultParameter(type, argumentNames[i]));
					}
				}
			}
			return list;
		}

		static string GuessParameterName(ResolveResult rr)
		{
			MemberResolveResult? mrr = rr as MemberResolveResult;
			if (mrr != null)
				return mrr.Member.Name;

			UnknownMemberResolveResult? umrr = rr as UnknownMemberResolveResult;
			if (umrr != null)
				return umrr.MemberName;

			MethodGroupResolveResult? mgrr = rr as MethodGroupResolveResult;
			if (mgrr != null)
				return mgrr.MethodName;

			LocalResolveResult? vrr = rr as LocalResolveResult;
			if (vrr != null)
				return MakeParameterName(vrr.Variable.Name);

			if (rr.Type.Kind != TypeKind.Unknown && !string.IsNullOrEmpty(rr.Type.Name))
			{
				return MakeParameterName(rr.Type.Name);
			}
			else
			{
				return "parameter";
			}
		}

		static string MakeParameterName(string variableName)
		{
			if (string.IsNullOrEmpty(variableName))
				return "parameter";
			if (variableName.Length > 1 && variableName[0] == '_')
				variableName = variableName.Substring(1);
			return char.ToLower(variableName[0]) + variableName.Substring(1);
		}

		OverloadResolution CreateOverloadResolution(ResolveResult[] arguments, string[]? argumentNames = null, IType[]? typeArguments = null)
		{
			var or = new OverloadResolution(compilation, arguments, argumentNames, typeArguments, conversions);
			or.CheckForOverflow = checkForOverflow;
			return or;
		}
		#endregion

		#region ResolveIndexer
		/// <summary>
		/// Resolves an indexer access.
		/// </summary>
		/// <param name="target">Target expression.</param>
		/// <param name="arguments">
		/// Arguments passed to the indexer.
		/// The resolver may mutate this array to wrap elements in <see cref="ConversionResolveResult"/>s!
		/// </param>
		/// <param name="argumentNames">
		/// The argument names. Pass the null string for positional arguments.
		/// </param>
		/// <returns>ArrayAccessResolveResult, InvocationResolveResult, or ErrorResolveResult</returns>
		public ResolveResult ResolveIndexer(ResolveResult target, ResolveResult[] arguments, string[]? argumentNames = null)
		{
			switch (target.Type.Kind)
			{
				case TypeKind.Dynamic:
					return new DynamicInvocationResolveResult(target, DynamicInvocationType.Indexing, AddArgumentNamesIfNecessary(arguments, argumentNames));

				case TypeKind.Array:
				case TypeKind.Pointer:
					// §7.6.6.1 Array access / §18.5.3 Pointer element access
					AdjustArrayAccessArguments(arguments);
					return new ArrayAccessResolveResult(((TypeWithElementType)target.Type).ElementType, target, arguments);
			}

			// §7.6.6.2 Indexer access

			MemberLookup lookup = CreateMemberLookup();
			var indexers = lookup.LookupIndexers(target);

			if (arguments.Any(a => a.Type.Kind == TypeKind.Dynamic))
			{
				// If we have dynamic arguments, we need to represent the invocation as a dynamic invocation if there is more than one applicable indexer.
				var or2 = CreateOverloadResolution(arguments, argumentNames, null);
				var applicableIndexers = indexers.SelectMany(x => x).Where(m => OverloadResolution.IsApplicable(or2.AddCandidate(m))).ToList();

				if (applicableIndexers.Count > 1)
				{
					return new DynamicInvocationResolveResult(target, DynamicInvocationType.Indexing, AddArgumentNamesIfNecessary(arguments, argumentNames));
				}
			}

			OverloadResolution or = CreateOverloadResolution(arguments, argumentNames);
			or.AddMethodLists(indexers);
			if (or.BestCandidate != null)
			{
				return or.CreateResolveResult(target);
			}
			else
			{
				return ErrorResult;
			}
		}

		/// <summary>
		/// Converts all arguments to int,uint,long or ulong.
		/// </summary>
		void AdjustArrayAccessArguments(ResolveResult[] arguments)
		{
			for (int i = 0; i < arguments.Length; i++)
			{
				if (!(TryConvert(ref arguments[i], compilation.FindType(KnownTypeCode.Int32)) ||
					  TryConvert(ref arguments[i], compilation.FindType(KnownTypeCode.UInt32)) ||
					  TryConvert(ref arguments[i], compilation.FindType(KnownTypeCode.Int64)) ||
					  TryConvert(ref arguments[i], compilation.FindType(KnownTypeCode.UInt64))))
				{
					// conversion failed
					arguments[i] = Convert(arguments[i], compilation.FindType(KnownTypeCode.Int32), Conversion.None);
				}
			}
		}
		#endregion

		#region ResolveObjectCreation
		/// <summary>
		/// Resolves an object creation.
		/// </summary>
		/// <param name="type">Type of the object to create.</param>
		/// <param name="arguments">
		/// Arguments passed to the constructor.
		/// The resolver may mutate this array to wrap elements in <see cref="ConversionResolveResult"/>s!
		/// </param>
		/// <param name="argumentNames">
		/// The argument names. Pass the null string for positional arguments.
		/// </param>
		/// <param name="allowProtectedAccess">
		/// Whether to allow calling protected constructors.
		/// This should be false except when resolving constructor initializers.
		/// </param>
		/// <param name="initializerStatements">
		/// Statements for Objects/Collections initializer.
		/// <see cref="InvocationResolveResult.InitializerStatements"/>
		/// </param>
		/// <returns>InvocationResolveResult or ErrorResolveResult</returns>
		public ResolveResult ResolveObjectCreation(IType type, ResolveResult[] arguments, string[]? argumentNames = null, bool allowProtectedAccess = false, IList<ResolveResult>? initializerStatements = null)
		{
			if (type.Kind == TypeKind.Delegate && arguments.Length == 1)
			{
				ResolveResult input = arguments[0];
				IMethod invoke = input.Type.GetDelegateInvokeMethod();
				if (invoke != null)
				{
					input = new MethodGroupResolveResult(
						input, invoke.Name,
						methods: new[] { new MethodListWithDeclaringType(input.Type) { invoke } },
						typeArguments: EmptyList<IType>.Instance
					);
				}
				return Convert(input, type);
			}
			OverloadResolution or = CreateOverloadResolution(arguments, argumentNames);
			MemberLookup lookup = CreateMemberLookup();
			var allApplicable = (arguments.Any(a => a.Type.Kind == TypeKind.Dynamic) ? new List<IMethod>() : null);
			foreach (IMethod ctor in type.GetConstructors())
			{
				if (lookup.IsAccessible(ctor, allowProtectedAccess))
				{
					var orErrors = or.AddCandidate(ctor);
					if (allApplicable != null && OverloadResolution.IsApplicable(orErrors))
						allApplicable.Add(ctor);
				}
				else
					or.AddCandidate(ctor, OverloadResolutionErrors.Inaccessible);
			}

			if (allApplicable != null && allApplicable.Count > 1)
			{
				// If we have dynamic arguments, we need to represent the invocation as a dynamic invocation if there is more than one applicable constructor.
				return new DynamicInvocationResolveResult(new MethodGroupResolveResult(null, allApplicable[0].Name, new[] { new MethodListWithDeclaringType(type, allApplicable) }, null), DynamicInvocationType.ObjectCreation, AddArgumentNamesIfNecessary(arguments, argumentNames), initializerStatements);
			}

			if (or.BestCandidate != null)
			{
				return or.CreateResolveResult(null, initializerStatements);
			}
			else
			{
				return new ErrorResolveResult(type);
			}
		}
		#endregion

		#region ResolveSizeOf
		/// <summary>
		/// Resolves 'sizeof(type)'.
		/// </summary>
		public ResolveResult ResolveSizeOf(IType type)
		{
			IType int32 = compilation.FindType(KnownTypeCode.Int32);
			int? size = null;
			var typeForConstant = (type.Kind == TypeKind.Enum) ? type.GetDefinition().EnumUnderlyingType : type;

			switch (ReflectionHelper.GetTypeCode(typeForConstant))
			{
				case TypeCode.Boolean:
				case TypeCode.SByte:
				case TypeCode.Byte:
					size = 1;
					break;
				case TypeCode.Char:
				case TypeCode.Int16:
				case TypeCode.UInt16:
					size = 2;
					break;
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Single:
					size = 4;
					break;
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Double:
					size = 8;
					break;
			}
			return new SizeOfResolveResult(int32, type, size);
		}
		#endregion

		#region Resolve This/Base Reference
		/// <summary>
		/// Resolves 'this'.
		/// </summary>
		public ResolveResult ResolveThisReference()
		{
			ITypeDefinition t = CurrentTypeDefinition;
			if (t != null)
			{
				if (t.TypeParameterCount != 0)
				{
					// Self-parameterize the type
					return new ThisResolveResult(new ParameterizedType(t, t.TypeParameters));
				}
				else
				{
					return new ThisResolveResult(t);
				}
			}
			return ErrorResult;
		}

		/// <summary>
		/// Resolves 'base'.
		/// </summary>
		public ResolveResult ResolveBaseReference()
		{
			ITypeDefinition t = CurrentTypeDefinition;
			if (t != null)
			{
				foreach (IType baseType in t.DirectBaseTypes)
				{
					if (baseType.Kind != TypeKind.Unknown && baseType.Kind != TypeKind.Interface)
					{
						return new ThisResolveResult(baseType, causesNonVirtualInvocation: true);
					}
				}
			}
			return ErrorResult;
		}
		#endregion

		#region ResolveConditional
		/// <summary>
		/// Converts the input to <c>bool</c> using the rules for boolean expressions.
		/// That is, <c>operator true</c> is used if a regular conversion to <c>bool</c> is not possible.
		/// </summary>
		public ResolveResult ResolveCondition(ResolveResult input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));
			IType boolean = compilation.FindType(KnownTypeCode.Boolean);
			Conversion c = conversions.ImplicitConversion(input, boolean);
			if (!c.IsValid)
			{
				var opTrue = input.Type.GetMethods(m => m.IsOperator && m.Name == "op_True").FirstOrDefault();
				if (opTrue != null)
				{
					c = Conversion.UserDefinedConversion(opTrue, isImplicit: true, conversionBeforeUserDefinedOperator: Conversion.None, conversionAfterUserDefinedOperator: Conversion.None);
				}
			}
			return Convert(input, boolean, c);
		}

		/// <summary>
		/// Converts the negated input to <c>bool</c> using the rules for boolean expressions.
		/// Computes <c>!(bool)input</c> if the implicit cast to bool is valid; otherwise
		/// computes <c>input.operator false()</c>.
		/// </summary>
		public ResolveResult ResolveConditionFalse(ResolveResult input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));
			IType boolean = compilation.FindType(KnownTypeCode.Boolean);
			Conversion c = conversions.ImplicitConversion(input, boolean);
			if (!c.IsValid)
			{
				var opFalse = input.Type.GetMethods(m => m.IsOperator && m.Name == "op_False").FirstOrDefault();
				if (opFalse != null)
				{
					c = Conversion.UserDefinedConversion(opFalse, isImplicit: true, conversionBeforeUserDefinedOperator: Conversion.None, conversionAfterUserDefinedOperator: Conversion.None);
					return Convert(input, boolean, c);
				}
			}
			return ResolveUnaryOperator(UnaryOperatorType.Not, Convert(input, boolean, c));
		}

		public ResolveResult ResolveConditional(ResolveResult condition, ResolveResult trueExpression, ResolveResult falseExpression)
		{
			// C# 4.0 spec §7.14: Conditional operator

			bool isValid;
			IType resultType;
			if (trueExpression.Type.Kind == TypeKind.Dynamic || falseExpression.Type.Kind == TypeKind.Dynamic)
			{
				resultType = SpecialType.Dynamic;
				isValid = TryConvert(ref trueExpression, resultType) & TryConvert(ref falseExpression, resultType);
			}
			else if (HasType(trueExpression) && HasType(falseExpression))
			{
				Conversion t2f = conversions.ImplicitConversion(trueExpression, falseExpression.Type);
				Conversion f2t = conversions.ImplicitConversion(falseExpression, trueExpression.Type);
				// The operator is valid:
				// a) if there's a conversion in one direction but not the other
				// b) if there are conversions in both directions, and the types are equivalent
				if (IsBetterConditionalConversion(t2f, f2t))
				{
					resultType = falseExpression.Type;
					isValid = true;
					trueExpression = Convert(trueExpression, resultType, t2f);
				}
				else if (IsBetterConditionalConversion(f2t, t2f))
				{
					resultType = trueExpression.Type;
					isValid = true;
					falseExpression = Convert(falseExpression, resultType, f2t);
				}
				else
				{
					resultType = trueExpression.Type;
					isValid = trueExpression.Type.Equals(falseExpression.Type);
				}
			}
			else if (HasType(trueExpression))
			{
				resultType = trueExpression.Type;
				isValid = TryConvert(ref falseExpression, resultType);
			}
			else if (HasType(falseExpression))
			{
				resultType = falseExpression.Type;
				isValid = TryConvert(ref trueExpression, resultType);
			}
			else
			{
				return ErrorResult;
			}
			condition = ResolveCondition(condition);
			if (isValid)
			{
				if (condition.IsCompileTimeConstant && trueExpression.IsCompileTimeConstant && falseExpression.IsCompileTimeConstant)
				{
					bool? val = condition.ConstantValue as bool?;
					if (val == true)
						return trueExpression;
					else if (val == false)
						return falseExpression;
				}
				return new OperatorResolveResult(resultType, System.Linq.Expressions.ExpressionType.Conditional,
												 condition, trueExpression, falseExpression);
			}
			else
			{
				return new ErrorResolveResult(resultType);
			}
		}

		bool IsBetterConditionalConversion(Conversion c1, Conversion c2)
		{
			// Valid is better than ImplicitConstantExpressionConversion is better than invalid
			if (!c1.IsValid)
				return false;
			if (c1 != Conversion.ImplicitConstantExpressionConversion && c2 == Conversion.ImplicitConstantExpressionConversion)
				return true;
			return !c2.IsValid;
		}

		bool HasType(ResolveResult r)
		{
			return r.Type.Kind != TypeKind.None && r.Type.Kind != TypeKind.Null;
		}
		#endregion

		#region ResolvePrimitive
		public ResolveResult ResolvePrimitive(object value)
		{
			if (value == null)
			{
				return new ResolveResult(SpecialType.NullType);
			}
			else
			{
				TypeCode typeCode = Type.GetTypeCode(value.GetType());
				IType type = compilation.FindType(typeCode);
				return new ConstantResolveResult(type, value);
			}
		}
		#endregion

		#region ResolveDefaultValue
		public ResolveResult ResolveDefaultValue(IType type)
		{
			return new ConstantResolveResult(type, GetDefaultValue(type));
		}

		public static object? GetDefaultValue(IType type)
		{
			ITypeDefinition? typeDef = type.GetDefinition();
			if (typeDef == null)
				return null;
			if (typeDef.Kind == TypeKind.Enum)
			{
				typeDef = typeDef.EnumUnderlyingType.GetDefinition();
				if (typeDef == null)
					return null;
			}
			switch (typeDef.KnownTypeCode)
			{
				case KnownTypeCode.Boolean:
					return false;
				case KnownTypeCode.Char:
					return '\0';
				case KnownTypeCode.SByte:
					return (sbyte)0;
				case KnownTypeCode.Byte:
					return (byte)0;
				case KnownTypeCode.Int16:
					return (short)0;
				case KnownTypeCode.UInt16:
					return (ushort)0;
				case KnownTypeCode.Int32:
					return 0;
				case KnownTypeCode.UInt32:
					return 0U;
				case KnownTypeCode.Int64:
					return 0L;
				case KnownTypeCode.UInt64:
					return 0UL;
				case KnownTypeCode.Single:
					return 0f;
				case KnownTypeCode.Double:
					return 0.0;
				case KnownTypeCode.Decimal:
					return 0m;
				default:
					return null;
			}
		}
		#endregion

		#region ResolveArrayCreation
		/// <summary>
		/// Resolves an array creation.
		/// </summary>
		/// <param name="elementType">
		/// The array element type.
		/// Pass null to resolve an implicitly-typed array creation.
		/// </param>
		/// <param name="sizeArguments">
		/// The size arguments.
		/// The length of this array will be used as the number of dimensions of the array type.
		/// Negative values will be treated as errors.
		/// </param>
		/// <param name="initializerElements">
		/// The initializer elements. May be null if no array initializer was specified.
		/// The resolver may mutate this array to wrap elements in <see cref="ConversionResolveResult"/>s!
		/// </param>
		public ArrayCreateResolveResult ResolveArrayCreation(IType elementType, int[] sizeArguments, ResolveResult[]? initializerElements = null)
		{
			ResolveResult[] sizeArgResults = new ResolveResult[sizeArguments.Length];
			for (int i = 0; i < sizeArguments.Length; i++)
			{
				if (sizeArguments[i] < 0)
					sizeArgResults[i] = ErrorResolveResult.UnknownError;
				else
					sizeArgResults[i] = new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), sizeArguments[i]);
			}
			return ResolveArrayCreation(elementType, sizeArgResults, initializerElements);
		}

		/// <summary>
		/// Resolves an array creation.
		/// </summary>
		/// <param name="elementType">
		/// The array element type.
		/// Pass null to resolve an implicitly-typed array creation.
		/// </param>
		/// <param name="sizeArguments">
		/// The size arguments.
		/// The length of this array will be used as the number of dimensions of the array type.
		/// The resolver may mutate this array to wrap elements in <see cref="ConversionResolveResult"/>s!
		/// </param>
		/// <param name="initializerElements">
		/// The initializer elements. May be null if no array initializer was specified.
		/// The resolver may mutate this array to wrap elements in <see cref="ConversionResolveResult"/>s!
		/// </param>
		public ArrayCreateResolveResult ResolveArrayCreation(IType elementType, ResolveResult[] sizeArguments, ResolveResult[]? initializerElements = null)
		{
			int dimensions = sizeArguments.Length;
			if (dimensions == 0)
				throw new ArgumentException("sizeArguments.Length must not be 0");
			if (elementType == null)
			{
				TypeInference typeInference = new TypeInference(compilation, conversions);
				elementType = typeInference.GetBestCommonType(initializerElements, out _);
			}
			IType arrayType = new ArrayType(compilation, elementType, dimensions);

			AdjustArrayAccessArguments(sizeArguments);

			if (initializerElements != null)
			{
				for (int i = 0; i < initializerElements.Length; i++)
				{
					initializerElements[i] = Convert(initializerElements[i], elementType);
				}
			}
			return new ArrayCreateResolveResult(arrayType, sizeArguments, initializerElements);
		}
		#endregion

		public ResolveResult ResolveTypeOf(IType referencedType)
		{
			return new TypeOfResolveResult(compilation.FindType(KnownTypeCode.Type), referencedType);
		}

		#region ResolveAssignment
		public ResolveResult ResolveAssignment(AssignmentOperatorType op, ResolveResult lhs, ResolveResult rhs)
		{
			var linqOp = AssignmentExpression.GetLinqNodeType(op, this.CheckForOverflow);
			var bop = AssignmentExpression.GetCorrespondingBinaryOperator(op);
			if (bop == null)
			{
				return new OperatorResolveResult(lhs.Type, linqOp, lhs, this.Convert(rhs, lhs.Type));
			}
			ResolveResult bopResult = ResolveBinaryOperator(bop.Value, lhs, rhs);
			OperatorResolveResult? opResult = bopResult as OperatorResolveResult;
			if (opResult == null || opResult.Operands.Count != 2)
				return bopResult;
			return new OperatorResolveResult(lhs.Type, linqOp, opResult.UserDefinedOperatorMethod, opResult.IsLiftedOperator,
											 new[] { lhs, opResult.Operands[1] });
		}
		#endregion
	}
}
