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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp
{
	struct CallBuilder
	{
		struct ExpectedTargetDetails
		{
			public OpCode CallOpCode;
			public bool NeedsBoxingConversion;
		}

		readonly DecompilerSettings settings;
		readonly ExpressionBuilder expressionBuilder;
		readonly CSharpResolver resolver;
		readonly IDecompilerTypeSystem typeSystem;

		public CallBuilder(ExpressionBuilder expressionBuilder, IDecompilerTypeSystem typeSystem, DecompilerSettings settings)
		{
			this.expressionBuilder = expressionBuilder;
			this.resolver = expressionBuilder.resolver;
			this.settings = settings;
			this.typeSystem = typeSystem;
		}

		public TranslatedExpression Build(CallInstruction inst)
		{
			if (inst is NewObj newobj && IL.Transforms.DelegateConstruction.IsDelegateConstruction(newobj, true)) {
				return HandleDelegateConstruction(newobj);
			}
			if (settings.TupleTypes && TupleTransform.MatchTupleConstruction(inst as NewObj, out var tupleElements) && tupleElements.Length >= 2) {
				var elementTypes = TupleType.GetTupleElementTypes(inst.Method.DeclaringType);
				Debug.Assert(!elementTypes.IsDefault, "MatchTupleConstruction should not success unless we got a valid tuple type.");
				Debug.Assert(elementTypes.Length == tupleElements.Length);
				var tuple = new TupleExpression();
				var elementRRs = new List<ResolveResult>();
				foreach (var (element, elementType) in tupleElements.Zip(elementTypes)) {
					var translatedElement = expressionBuilder.Translate(element, elementType)
						.ConvertTo(elementType, expressionBuilder, allowImplicitConversion: true);
					tuple.Elements.Add(translatedElement.Expression);
					elementRRs.Add(translatedElement.ResolveResult);
				}
				return tuple.WithRR(new TupleResolveResult(
					expressionBuilder.compilation,
					elementRRs.ToImmutableArray()
				)).WithILInstruction(inst);
			}
			return Build(inst.OpCode, inst.Method, inst.Arguments, constrainedTo: inst.ConstrainedTo)
				.WithILInstruction(inst);
		}

		public ExpressionWithResolveResult Build(OpCode callOpCode, IMethod method,
			IReadOnlyList<ILInstruction> callArguments,
			IReadOnlyList<int> argumentToParameterMap = null,
			IType constrainedTo = null)
		{
			// Used for Call, CallVirt and NewObj
			var expectedTargetDetails = new ExpectedTargetDetails {
				CallOpCode = callOpCode
			};
			TranslatedExpression target;
			if (callOpCode == OpCode.NewObj) {
				target = default(TranslatedExpression); // no target
			} else {
				target = expressionBuilder.TranslateTarget(
					callArguments.FirstOrDefault(),
					nonVirtualInvocation: callOpCode == OpCode.Call,
					memberStatic: method.IsStatic,
					memberDeclaringType: constrainedTo ?? method.DeclaringType);
				if (constrainedTo == null
					&& target.Expression is CastExpression cast
					&& target.ResolveResult is ConversionResolveResult conversion
					&& target.Type.IsKnownType(KnownTypeCode.Object)
					&& conversion.Conversion.IsBoxingConversion)
				{
					// boxing conversion on call target?
					// let's see if we can make that implicit:
					target = target.UnwrapChild(cast.Expression);
					// we'll need to make sure the boxing effect is preserved
					expectedTargetDetails.NeedsBoxingConversion = true;
				}
			}

			int firstParamIndex = (method.IsStatic || callOpCode == OpCode.NewObj) ? 0 : 1;
			Debug.Assert(firstParamIndex == 0 || argumentToParameterMap == null
				|| argumentToParameterMap[0] == -1);
			
			// Translate arguments to the expected parameter types
			var arguments = new List<TranslatedExpression>(method.Parameters.Count);
			string[] argumentNames = null;
			Debug.Assert(callArguments.Count == firstParamIndex + method.Parameters.Count);
			var expectedParameters = new List<IParameter>(arguments.Count); // parameters, but in argument order
			bool isExpandedForm = false;

			// Optional arguments:
			// We only allow removing optional arguments in the following cases:
			// - call arguments are not in expanded form
			// - there are no named arguments
			// This value has the following values:
			// -2 - there are no optional arguments
			// -1 - optional arguments are forbidden
			// >= 0 - the index of the first argument that can be removed, because it is optional
			// and is the default value of the parameter. 
			int firstOptionalArgumentIndex = expressionBuilder.settings.OptionalArguments ? -2 : -1;
			for (int i = firstParamIndex; i < callArguments.Count; i++) {
				IParameter parameter;
				if (argumentToParameterMap != null) {
					// Don't use optional arguments, if we have to use named arguments.
					firstOptionalArgumentIndex = -1;
					if (argumentNames == null && argumentToParameterMap[i] != i - firstParamIndex) {
						// Starting at the first argument that is out-of-place,
						// assign names to that argument and all following arguments:
						argumentNames = new string[method.Parameters.Count];
					}
					parameter = method.Parameters[argumentToParameterMap[i]];
					if (argumentNames != null) {
						argumentNames[arguments.Count] = parameter.Name;
					}
				} else {
					parameter = method.Parameters[i - firstParamIndex];
				}
				var arg = expressionBuilder.Translate(callArguments[i], parameter.Type);
				if (IsOptionalArgument(parameter, arg)) {
					if (firstOptionalArgumentIndex == -2)
						firstOptionalArgumentIndex = i - firstParamIndex;
				} else {
					firstOptionalArgumentIndex = -2;
				}
				if (parameter.IsParams && i + 1 == callArguments.Count && argumentToParameterMap == null) {
					// Parameter is marked params
					// If the argument is an array creation, inline all elements into the call and add missing default values.
					// Otherwise handle it normally.
					if (TransformParamsArgument(expectedTargetDetails, target.ResolveResult, method, parameter,
						arg, ref expectedParameters, ref arguments)) {
						Debug.Assert(argumentNames == null);
						firstOptionalArgumentIndex = -1;
						isExpandedForm = true;
						continue;
					}
				}

				IType parameterType;
				if (parameter.Type.Kind == TypeKind.Dynamic) {
					parameterType = expressionBuilder.compilation.FindType(KnownTypeCode.Object);
				} else {
					parameterType = parameter.Type;
				}

				arg = arg.ConvertTo(parameterType, expressionBuilder, allowImplicitConversion: arg.Type.Kind != TypeKind.Dynamic);

				if (parameter.IsOut) {
					arg = ExpressionBuilder.ChangeDirectionExpressionToOut(arg);
				}

				arguments.Add(arg);
				expectedParameters.Add(parameter);
			}

			if (method is VarArgInstanceMethod) {
				firstOptionalArgumentIndex = -1;
				int regularParameterCount = ((VarArgInstanceMethod)method).RegularParameterCount;
				var argListArg = new UndocumentedExpression();
				argListArg.UndocumentedExpressionType = UndocumentedExpressionType.ArgList;
				int paramIndex = regularParameterCount;
				var builder = expressionBuilder;
				Debug.Assert(argumentToParameterMap == null && argumentNames == null);
				argListArg.Arguments.AddRange(arguments.Skip(regularParameterCount).Select(arg => arg.ConvertTo(expectedParameters[paramIndex++].Type, builder).Expression));
				var argListRR = new ResolveResult(SpecialType.ArgList);
				arguments = arguments.Take(regularParameterCount)
					.Concat(new[] { argListArg.WithoutILInstruction().WithRR(argListRR) }).ToList();
				method = ((VarArgInstanceMethod)method).BaseMethod;
				expectedParameters = method.Parameters.ToList();
			}


			if (callOpCode == OpCode.NewObj) {
				if (settings.AnonymousTypes && method.DeclaringType.IsAnonymousType()) {
					Debug.Assert(argumentToParameterMap == null && argumentNames == null && firstOptionalArgumentIndex < 0);
					var argumentExpressions = arguments.SelectArray(arg => arg.Expression);
					var atce = new AnonymousTypeCreateExpression();
					if (CanInferAnonymousTypePropertyNamesFromArguments(argumentExpressions, expectedParameters)) {
						atce.Initializers.AddRange(argumentExpressions);
					} else {
						for (int i = 0; i < argumentExpressions.Length; i++) {
							atce.Initializers.Add(
								new NamedExpression {
									Name = expectedParameters[i].Name,
									Expression = arguments[i].ConvertTo(expectedParameters[i].Type, expressionBuilder)
								});
						}
					}
					return atce.WithRR(new CSharpInvocationResolveResult(
						target.ResolveResult, method, arguments.SelectArray(arg => arg.ResolveResult),
						isExpandedForm: isExpandedForm, argumentToParameterMap: argumentToParameterMap
					));
				} else {
					while (IsUnambiguousCall(expectedTargetDetails, method, null, Empty<IType>.Array, arguments,
						argumentNames, firstOptionalArgumentIndex, out _) != OverloadResolutionErrors.None) {
						if (firstOptionalArgumentIndex >= 0) {
							firstOptionalArgumentIndex = -1;
							continue;
						}
						CastArguments(arguments, expectedParameters);
						break; // make sure that we don't not end up in an infinite loop
					}
					return new ObjectCreateExpression(
						expressionBuilder.ConvertType(method.DeclaringType),
						GetArgumentExpressions(arguments, argumentNames, firstOptionalArgumentIndex)
					).WithRR(new CSharpInvocationResolveResult(
						target.ResolveResult, method,
							firstOptionalArgumentIndex < 0
								? arguments.SelectArray(arg => arg.ResolveResult)
								: arguments.Take(firstOptionalArgumentIndex).Select(arg => arg.ResolveResult).ToArray(),
						isExpandedForm: isExpandedForm, argumentToParameterMap: argumentToParameterMap
					));
				}
			} else {
				int allowedParamCount = (method.ReturnType.IsKnownType(KnownTypeCode.Void) ? 1 : 0);
				if (method.IsAccessor && (method.AccessorOwner.SymbolKind == SymbolKind.Indexer || expectedParameters.Count == allowedParamCount)) {
					Debug.Assert(argumentToParameterMap == null && argumentNames == null && firstOptionalArgumentIndex < 0);
					return HandleAccessorCall(expectedTargetDetails, method, target, arguments.ToList(), argumentNames);
				} else if (method.Name == "Invoke" && method.DeclaringType.Kind == TypeKind.Delegate && !IsNullConditional(target)) {
					return new InvocationExpression(target, GetArgumentExpressions(arguments, argumentNames, firstOptionalArgumentIndex))
						.WithRR(new CSharpInvocationResolveResult(target.ResolveResult, method,
							firstOptionalArgumentIndex < 0
								? arguments.SelectArray(arg => arg.ResolveResult)
								: arguments.Take(firstOptionalArgumentIndex).Select(arg => arg.ResolveResult).ToArray(),
							isExpandedForm: isExpandedForm));
				} else if (IsDelegateEqualityComparison(method, arguments)) {
					Debug.Assert(argumentToParameterMap == null && argumentNames == null && firstOptionalArgumentIndex < 0);
					return HandleDelegateEqualityComparison(method, arguments)
						.WithRR(new CSharpInvocationResolveResult(target.ResolveResult, method,
							arguments.SelectArray(arg => arg.ResolveResult), isExpandedForm: isExpandedForm));
				} else if (method.IsOperator && method.Name == "op_Implicit" && arguments.Count == 1) {
					Debug.Assert(firstOptionalArgumentIndex < 0);
					return HandleImplicitConversion(method, arguments[0]);
				} else {
					var transform = GetRequiredTransformationsForCall(expectedTargetDetails, method, ref target,
						arguments, argumentNames, firstOptionalArgumentIndex, expectedParameters, CallTransformation.All, out IParameterizedMember foundMethod);

					// Note: after this, 'method' and 'foundMethod' may differ,
					// but as far as allowed by IsAppropriateCallTarget().

					Expression targetExpr;
					string methodName = method.Name;
					AstNodeCollection<AstType> typeArgumentList;
					if ((transform & CallTransformation.NoOptionalArgumentAllowed) != 0) {
						firstOptionalArgumentIndex = -1;
					}
					if ((transform & CallTransformation.RequireTarget) != 0) {
						targetExpr = new MemberReferenceExpression(target.Expression, methodName);
						typeArgumentList = ((MemberReferenceExpression)targetExpr).TypeArguments;

						// HACK : convert this.Dispose() to ((IDisposable)this).Dispose(), if Dispose is an explicitly implemented interface method.
						// settings.AlwaysCastTargetsOfExplicitInterfaceImplementationCalls == true is used in Windows Forms' InitializeComponent methods.
						if (method.IsExplicitInterfaceImplementation && (target.Expression is ThisReferenceExpression || settings.AlwaysCastTargetsOfExplicitInterfaceImplementationCalls)) {
							var interfaceMember = method.ExplicitlyImplementedInterfaceMembers.First();
							var castExpression = new CastExpression(expressionBuilder.ConvertType(interfaceMember.DeclaringType), target.Expression);
							methodName = interfaceMember.Name;
							targetExpr = new MemberReferenceExpression(castExpression, methodName);
							typeArgumentList = ((MemberReferenceExpression)targetExpr).TypeArguments;
						}
					} else {
						targetExpr = new IdentifierExpression(methodName);
						typeArgumentList = ((IdentifierExpression)targetExpr).TypeArguments;
					}
					
					if ((transform & CallTransformation.RequireTypeArguments) != 0 && (!settings.AnonymousTypes || !method.TypeArguments.Any(a => a.ContainsAnonymousType())))
						typeArgumentList.AddRange(method.TypeArguments.Select(expressionBuilder.ConvertType));
					var argumentExpressions = GetArgumentExpressions(arguments, argumentNames, firstOptionalArgumentIndex);
					return new InvocationExpression(targetExpr, argumentExpressions)
						.WithRR(new CSharpInvocationResolveResult(target.ResolveResult, foundMethod, firstOptionalArgumentIndex < 0 ? arguments.Select(arg => arg.ResolveResult).ToList() : arguments.Select(arg => arg.ResolveResult).ToList().Take(firstOptionalArgumentIndex).ToList(), isExpandedForm: isExpandedForm));
				}
			}
		}

		/// <summary>
		/// Converts a call to an Add method to a collection initializer expression.
		/// </summary>
		public ExpressionWithResolveResult BuildCollectionInitializerExpression(OpCode callOpCode, IMethod method,
			InitializedObjectResolveResult target, IReadOnlyList<ILInstruction> callArguments)
		{
			// (see ECMA-334, section 12.7.11.4):
			// The collection object to which a collection initializer is applied shall be of a type that implements
			// System.Collections.IEnumerable or a compile-time error occurs. For each specified element in order,
			// the collection initializer invokes an Add method on the target object with the expression list of the
			// element initializer as argument list, applying normal overload resolution for each invocation. Thus, the
			// collection object shall contain an applicable Add method for each element initializer.

			// The list of applicable methods includes all methods (as of C# 6.0 extension methods, too) named 'Add'
			// that can be invoked on the target object, with the following exceptions:
			// - Methods with ref or out parameters may not be used,
			// - methods that have type parameters, that cannot be inferred from the parameter list may not be used,
			// - vararg methods may not be used.
			// - named arguments are not supported.
			// However, note that params methods may be used.

			// At this point, we assume that 'method' fulfills all the conditions mentioned above. We just need to make
			// sure that the correct method is called by resolving any ambiguities by inserting casts, if necessary.

			ExpectedTargetDetails expectedTargetDetails = new ExpectedTargetDetails { CallOpCode = callOpCode };

			// Special case: only in this case, collection initializers, are extension methods already transformed on
			// ILAst level, therefore we have to exclude the first argument.
			int firstParamIndex = method.IsExtensionMethod ? 1 : 0;

			var arguments = new List<TranslatedExpression>(callArguments.Count);
			Debug.Assert(callArguments.Count == method.Parameters.Count - firstParamIndex);
			var expectedParameters = new List<IParameter>(arguments.Count); // parameters, but in argument order
			bool isExpandedForm = false;

			// Optional arguments:
			// We only allow removing optional arguments in the following cases:
			// - call arguments are not in expanded form
			// - there are no named arguments
			// This value has the following values:
			// -2 - there are no optional arguments
			// -1 - optional arguments are forbidden
			// >= 0 - the index of the first argument that can be removed, because it is optional
			// and is the default value of the parameter. 
			int firstOptionalArgumentIndex = expressionBuilder.settings.OptionalArguments ? -2 : -1;

			for (int i = 0; i < callArguments.Count; i++) {
				var parameter = method.Parameters[i + firstParamIndex];

				var arg = expressionBuilder.Translate(callArguments[i], parameter.Type);
				if (IsOptionalArgument(parameter, arg)) {
					if (firstOptionalArgumentIndex == -2)
						firstOptionalArgumentIndex = i - firstParamIndex;
				} else {
					firstOptionalArgumentIndex = -2;
				}
				if (parameter.IsParams && i + 1 == callArguments.Count) {
					// Parameter is marked params
					// If the argument is an array creation, inline all elements into the call and add missing default values.
					// Otherwise handle it normally.
					if (TransformParamsArgument(expectedTargetDetails, target, method, parameter,
						arg, ref expectedParameters, ref arguments)) {
						isExpandedForm = true;
						firstOptionalArgumentIndex = -1;
						continue;
					}
				}

				IType parameterType;
				if (parameter.Type.Kind == TypeKind.Dynamic) {
					parameterType = expressionBuilder.compilation.FindType(KnownTypeCode.Object);
				} else {
					parameterType = parameter.Type;
				}

				arg = arg.ConvertTo(parameterType, expressionBuilder, allowImplicitConversion: arg.Type.Kind != TypeKind.Dynamic);

				arguments.Add(arg);
				expectedParameters.Add(parameter);
			}

			var unused = new IdentifierExpression("initializedObject").WithRR(target).WithoutILInstruction();
			var transform = GetRequiredTransformationsForCall(expectedTargetDetails, method, ref unused,
				arguments, null, firstOptionalArgumentIndex, expectedParameters, CallTransformation.None, out IParameterizedMember foundMethod);
			Debug.Assert(transform == CallTransformation.None || transform == CallTransformation.NoOptionalArgumentAllowed);

			// Calls with only one argument do not need an array initializer expression to wrap them.
			// Any special cases are handled by the caller (i.e., ExpressionBuilder.TranslateObjectAndCollectionInitializer)
			// Note: we intentionally ignore the firstOptionalArgumentIndex in this case.
			if (arguments.Count == 1)
				return arguments[0];

			if ((transform & CallTransformation.NoOptionalArgumentAllowed) != 0)
				firstOptionalArgumentIndex = -1;

			ResolveResult[] argumentResolveResults;
			IEnumerable<Expression> elements;

			if (firstOptionalArgumentIndex < 0) {
				elements = arguments.Select(a => a.Expression);
				argumentResolveResults = arguments.SelectArray(a => a.ResolveResult);
			} else {
				elements = arguments.Take(firstOptionalArgumentIndex).Select(a => a.Expression);
				argumentResolveResults = arguments.Take(firstOptionalArgumentIndex).Select(a => a.ResolveResult).ToArray();
			}
			return new ArrayInitializerExpression(elements)
				.WithRR(new CSharpInvocationResolveResult(target, method, argumentResolveResults,
					isExtensionMethodInvocation: method.IsExtensionMethod, isExpandedForm: isExpandedForm));
		}

		private bool TransformParamsArgument(ExpectedTargetDetails expectedTargetDetails, ResolveResult targetResolveResult,
			IMethod method, IParameter parameter, TranslatedExpression arg, ref List<IParameter> expectedParameters,
			ref List<TranslatedExpression> arguments)
		{
			if (CheckArgument(out int length, out IType elementType)) {
				var expandedParameters = new List<IParameter>(expectedParameters);
				var expandedArguments = new List<TranslatedExpression>(arguments);
				if (length > 0) {
					var arrayElements = ((ArrayCreateExpression)arg.Expression).Initializer.Elements.ToArray();
					for (int j = 0; j < length; j++) {
						expandedParameters.Add(new DefaultParameter(elementType, parameter.Name + j));
						if (j < arrayElements.Length)
							expandedArguments.Add(new TranslatedExpression(arrayElements[j]));
						else
							expandedArguments.Add(expressionBuilder.GetDefaultValueExpression(elementType).WithoutILInstruction());
					}
				}
				if (IsUnambiguousCall(expectedTargetDetails, method, targetResolveResult, Empty<IType>.Array, expandedArguments, null, -1, out _) == OverloadResolutionErrors.None) {
					expectedParameters = expandedParameters;
					arguments = expandedArguments.SelectList(a => new TranslatedExpression(a.Expression.Detach()));
					return true;
				}
			}
			return false;

			bool CheckArgument(out int len, out IType t)
			{
				len = 0;
				t = null;
				if (arg.ResolveResult is CSharpInvocationResolveResult csirr && 
					csirr.Arguments.Count == 0 && csirr.Member is IMethod emptyMethod && 
					emptyMethod.IsStatic &&
					"System.Array.Empty" == emptyMethod.FullName &&
					emptyMethod.TypeArguments.Count == 1)
				{
					t = emptyMethod.TypeArguments[0];
					return true;
				}

				if (arg.ResolveResult is ArrayCreateResolveResult acrr &&
					acrr.SizeArguments.Count == 1 &&
					acrr.SizeArguments[0].IsCompileTimeConstant &&
					acrr.SizeArguments[0].ConstantValue is int l)
				{
					len = l;
					t = ((ArrayType)acrr.Type).ElementType;
					return true;
				}
				return false;
			}
		}

		bool IsOptionalArgument(IParameter parameter, TranslatedExpression arg)
		{
			if (!parameter.IsOptional || !arg.ResolveResult.IsCompileTimeConstant)
				return false;
			if (parameter.GetAttributes().Any(a => a.AttributeType.IsKnownType(KnownAttribute.CallerMemberName)
				|| a.AttributeType.IsKnownType(KnownAttribute.CallerFilePath)
				|| a.AttributeType.IsKnownType(KnownAttribute.CallerLineNumber)))
				return false;
			return (parameter.ConstantValue == null && arg.ResolveResult.ConstantValue == null)
				|| (parameter.ConstantValue != null && parameter.ConstantValue.Equals(arg.ResolveResult.ConstantValue));
		}

		[Flags]
		enum CallTransformation
		{
			None = 0,
			RequireTarget = 1,
			RequireTypeArguments = 2,
			NoOptionalArgumentAllowed = 4,
			All = 7
		}

		private CallTransformation GetRequiredTransformationsForCall(ExpectedTargetDetails expectedTargetDetails, IMethod method,
			ref TranslatedExpression target, List<TranslatedExpression> arguments, string[] argumentNames, int firstOptionalArgumentIndex,
			List<IParameter> expectedParameters, CallTransformation allowedTransforms, out IParameterizedMember foundMethod)
		{
			CallTransformation transform = CallTransformation.None;

			// initialize requireTarget flag
			bool requireTarget;
			ResolveResult targetResolveResult;
			if ((allowedTransforms & CallTransformation.RequireTarget) != 0) {
				if (expressionBuilder.HidesVariableWithName(method.Name)) {
					requireTarget = true;
				} else {
					if (method.IsStatic)
						requireTarget = !expressionBuilder.IsCurrentOrContainingType(method.DeclaringTypeDefinition) || method.Name == ".cctor";
					else if (method.Name == ".ctor")
						requireTarget = true; // always use target for base/this-ctor-call, the constructor initializer pattern depends on this
					else if (target.Expression is BaseReferenceExpression)
						requireTarget = (expectedTargetDetails.CallOpCode != OpCode.CallVirt && method.IsVirtual);
					else
						requireTarget = !(target.Expression is ThisReferenceExpression);
				}
				targetResolveResult = requireTarget ? target.ResolveResult : null;
			} else {
				// HACK: this is a special case for collection initializer calls, they do not allow a target to be
				// emitted, but we still need it for overload resolution.
				requireTarget = true;
				targetResolveResult = target.ResolveResult;
			}

			// initialize requireTypeArguments flag
			bool requireTypeArguments;
			IType[] typeArguments;
			bool appliedRequireTypeArgumentsShortcut = false;
			if (method.TypeParameters.Count > 0 && (allowedTransforms & CallTransformation.RequireTypeArguments) != 0
				&& !IsPossibleExtensionMethodCallOnNull(method, arguments)) {
				// The ambiguity resolution below only adds type arguments as last resort measure, however there are
				// methods, such as Enumerable.OfType<TResult>(IEnumerable input) that always require type arguments,
				// as those cannot be inferred from the parameters, which leads to bloated expressions full of extra casts
				// that are no longer required once we add the type arguments.
				// We lend overload resolution a hand by detecting such cases beforehand and requiring type arguments,
				// if necessary.
				if (!CanInferTypeArgumentsFromParameters(method, arguments.SelectArray(a => a.ResolveResult), expressionBuilder.typeInference)) {
					requireTypeArguments = true;
					typeArguments = method.TypeArguments.ToArray();
					appliedRequireTypeArgumentsShortcut = true;
				} else {
					requireTypeArguments = false;
					typeArguments = Empty<IType>.Array;
				}
			} else {
				requireTypeArguments = false;
				typeArguments = Empty<IType>.Array;
			}

			bool targetCasted = false;
			bool argumentsCasted = false;
			OverloadResolutionErrors errors;
			while ((errors = IsUnambiguousCall(expectedTargetDetails, method, targetResolveResult, typeArguments, arguments, argumentNames, firstOptionalArgumentIndex, out foundMethod)) != OverloadResolutionErrors.None) {
				switch (errors) {
					case OverloadResolutionErrors.TypeInferenceFailed:
						if ((allowedTransforms & CallTransformation.RequireTypeArguments) != 0) {
							goto case OverloadResolutionErrors.WrongNumberOfTypeArguments;
						}
						goto default;
					case OverloadResolutionErrors.WrongNumberOfTypeArguments:
						Debug.Assert((allowedTransforms & CallTransformation.RequireTypeArguments) != 0);
						if (requireTypeArguments) goto default;
						requireTypeArguments = true;
						typeArguments = method.TypeArguments.ToArray();
						continue;
					case OverloadResolutionErrors.MissingArgumentForRequiredParameter:
						if (firstOptionalArgumentIndex == -1) goto default;
						firstOptionalArgumentIndex = -1;
						continue;
					default:
						// TODO : implement some more intelligent algorithm that decides which of these fixes (cast args, add target, cast target, add type args)
						// is best in this case. Additionally we should not cast all arguments at once, but step-by-step try to add only a minimal number of casts.
						if (firstOptionalArgumentIndex >= 0) {
							firstOptionalArgumentIndex = -1;
						} else if (!argumentsCasted) {
							// If we added type arguments beforehand, but that didn't make the code any better,
							// undo that decision and add casts first.
							if (appliedRequireTypeArgumentsShortcut) {
								requireTypeArguments = false;
								typeArguments = Empty<IType>.Array;
								appliedRequireTypeArgumentsShortcut = false;
							}
							argumentsCasted = true;
							CastArguments(arguments, expectedParameters);
						} else if ((allowedTransforms & CallTransformation.RequireTarget) != 0 && !requireTarget) {
							requireTarget = true;
							targetResolveResult = target.ResolveResult;
						} else if ((allowedTransforms & CallTransformation.RequireTarget) != 0 && !targetCasted) {
							targetCasted = true;
							target = target.ConvertTo(method.DeclaringType, expressionBuilder);
							targetResolveResult = target.ResolveResult;
						} else if ((allowedTransforms & CallTransformation.RequireTypeArguments) != 0 && !requireTypeArguments) {
							requireTypeArguments = true;
							typeArguments = method.TypeArguments.ToArray();
						} else {
							break;
						}
						continue;
				}
				// We've given up.
				foundMethod = method;
				break;
			}
			if ((allowedTransforms & CallTransformation.RequireTarget) != 0 && requireTarget)
				transform |= CallTransformation.RequireTarget;
			if ((allowedTransforms & CallTransformation.RequireTypeArguments) != 0 && requireTypeArguments)
				transform |= CallTransformation.RequireTypeArguments;
			if (firstOptionalArgumentIndex < 0)
				transform |= CallTransformation.NoOptionalArgumentAllowed;
			return transform;
		}

		private bool IsPossibleExtensionMethodCallOnNull(IMethod method, List<TranslatedExpression> arguments)
		{
			return method.IsExtensionMethod && arguments.Count > 0 && arguments[0].Expression is NullReferenceExpression;
		}

		public static bool CanInferTypeArgumentsFromParameters(IMethod method, IReadOnlyList<ResolveResult> arguments,
			TypeInference typeInference)
		{
			if (method.TypeParameters.Count == 0)
				return true;
			// always use unspecialized member, otherwise type inference fails
			method = (IMethod)method.MemberDefinition;
			typeInference.InferTypeArguments(method.TypeParameters, arguments, method.Parameters.SelectReadOnlyArray(p => p.Type),
				out bool success);
			return success;
		}

		private void CastArguments(IList<TranslatedExpression> arguments, IReadOnlyList<IParameter> expectedParameters)
		{
			for (int i = 0; i < arguments.Count; i++) {
				if (settings.AnonymousTypes && expectedParameters[i].Type.ContainsAnonymousType()) {
					if (arguments[i].Expression is LambdaExpression lambda) {
						ModifyReturnTypeOfLambda(lambda);
					}
				} else {
					IType parameterType;
					if (expectedParameters[i].Type.Kind == TypeKind.Dynamic) {
						parameterType = expressionBuilder.compilation.FindType(KnownTypeCode.Object);
					} else {
						parameterType = expectedParameters[i].Type;
					}

					arguments[i] = arguments[i].ConvertTo(parameterType, expressionBuilder, allowImplicitConversion: false);
				}
			}
		}

		private IEnumerable<Expression> GetArgumentExpressions(List<TranslatedExpression> arguments,
			string[] argumentNames, int firstOptionalArgumentIndex)
		{
			if (argumentNames == null) {
				if (firstOptionalArgumentIndex < 0)
					return arguments.Select(arg => arg.Expression);
				return arguments.Take(firstOptionalArgumentIndex).Select(arg => arg.Expression);
			} else {
				return arguments.Zip(argumentNames,
					(arg, name) => {
						if (name == null)
							return arg.Expression;
						else
							return new NamedArgumentExpression(name, arg);
					});
			}
		}

		static bool IsNullConditional(Expression expr)
		{
			return expr is UnaryOperatorExpression uoe && uoe.Operator == UnaryOperatorType.NullConditional;
		}

		private void ModifyReturnTypeOfLambda(LambdaExpression lambda)
		{
			var resolveResult = (DecompiledLambdaResolveResult)lambda.GetResolveResult();
			if (lambda.Body is Expression exprBody)
				lambda.Body = new TranslatedExpression(exprBody.Detach()).ConvertTo(resolveResult.ReturnType, expressionBuilder);
			else
				ModifyReturnStatementInsideLambda(resolveResult.ReturnType, lambda);
			resolveResult.InferredReturnType = resolveResult.ReturnType;
		}

		private void ModifyReturnStatementInsideLambda(IType returnType, AstNode parent)
		{
			foreach (var child in parent.Children) {
				if (child is LambdaExpression || child is AnonymousMethodExpression)
					continue;
				if (child is ReturnStatement ret) {
					ret.Expression = new TranslatedExpression(ret.Expression.Detach()).ConvertTo(returnType, expressionBuilder);
					continue;
				}
				ModifyReturnStatementInsideLambda(returnType, child);
			}
		}

		private bool IsDelegateEqualityComparison(IMethod method, IList<TranslatedExpression> arguments)
		{
			// Comparison on a delegate type is a C# builtin operator
			// that compiles down to a Delegate.op_Equality call.
			// We handle this as a special case to avoid inserting a cast to System.Delegate.
			return method.IsOperator
				&& method.DeclaringType.IsKnownType(KnownTypeCode.Delegate)
				&& (method.Name == "op_Equality" || method.Name == "op_Inequality")
				&& arguments.Count == 2
				&& arguments[0].Type.Kind == TypeKind.Delegate
				&& arguments[1].Type.Equals(arguments[0].Type);
		}

		private Expression HandleDelegateEqualityComparison(IMethod method, IList<TranslatedExpression> arguments)
		{
			return new BinaryOperatorExpression(
				arguments[0],
				method.Name == "op_Equality" ? BinaryOperatorType.Equality : BinaryOperatorType.InEquality,
				arguments[1]
			);
		}

		private ExpressionWithResolveResult HandleImplicitConversion(IMethod method, TranslatedExpression argument)
		{
			var conversions = CSharpConversions.Get(expressionBuilder.compilation);
			IType targetType = method.ReturnType;
			var conv = conversions.ImplicitConversion(argument.Type, targetType);
			if (!(conv.IsUserDefined && conv.Method.Equals(method))) {
				// implicit conversion to targetType isn't directly possible, so first insert a cast to the argument type
				argument = argument.ConvertTo(method.Parameters[0].Type, expressionBuilder);
				conv = conversions.ImplicitConversion(argument.Type, targetType);
			}
			return new CastExpression(expressionBuilder.ConvertType(targetType), argument.Expression)
				.WithRR(new ConversionResolveResult(targetType, argument.ResolveResult, conv));
		}

		OverloadResolutionErrors IsUnambiguousCall(ExpectedTargetDetails expectedTargetDetails, IMethod method,
			ResolveResult target, IType[] typeArguments, IList<TranslatedExpression> arguments,
			string[] argumentNames, int firstOptionalArgumentIndex,
			out IParameterizedMember foundMember)
		{
			Debug.Assert(firstOptionalArgumentIndex < 0 || argumentNames == null);
			foundMember = null;
			var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentModule);
			var or = new OverloadResolution(resolver.Compilation,
				firstOptionalArgumentIndex < 0 ? arguments.SelectArray(a => a.ResolveResult) : arguments.Take(firstOptionalArgumentIndex).Select(a => a.ResolveResult).ToArray(),
				argumentNames: argumentNames,
				typeArguments: typeArguments,
				conversions: expressionBuilder.resolver.conversions);
			if (expectedTargetDetails.CallOpCode == OpCode.NewObj) {
				foreach (IMethod ctor in method.DeclaringType.GetConstructors()) {
					if (lookup.IsAccessible(ctor, allowProtectedAccess: resolver.CurrentTypeDefinition == method.DeclaringTypeDefinition)) {
						or.AddCandidate(ctor);
					}
				}
			} else if (method.IsOperator) {
				IEnumerable<IParameterizedMember> operatorCandidates;
				if (arguments.Count == 1) {
					operatorCandidates = resolver.GetUserDefinedOperatorCandidates(arguments[0].Type, method.Name);
				} else if (arguments.Count == 2) {
					var hashSet = new HashSet<IParameterizedMember>();
					hashSet.UnionWith(resolver.GetUserDefinedOperatorCandidates(arguments[0].Type, method.Name));
					hashSet.UnionWith(resolver.GetUserDefinedOperatorCandidates(arguments[1].Type, method.Name));
					operatorCandidates = hashSet;
				} else {
					operatorCandidates = EmptyList<IParameterizedMember>.Instance;
				}
				foreach (var m in operatorCandidates) {
					or.AddCandidate(m);
				}
			} else if (target == null) {
				var result = resolver.ResolveSimpleName(method.Name, typeArguments, isInvocationTarget: true) as MethodGroupResolveResult;
				if (result == null)
					return OverloadResolutionErrors.AmbiguousMatch;
				or.AddMethodLists(result.MethodsGroupedByDeclaringType.ToArray());
			} else {
				var result = lookup.Lookup(target, method.Name, typeArguments, isInvocation: true) as MethodGroupResolveResult;
				if (result == null)
					return OverloadResolutionErrors.AmbiguousMatch;
				or.AddMethodLists(result.MethodsGroupedByDeclaringType.ToArray());
			}
			if (or.BestCandidateErrors != OverloadResolutionErrors.None)
				return or.BestCandidateErrors;
			if (or.IsAmbiguous)
				return OverloadResolutionErrors.AmbiguousMatch;
			foundMember = or.GetBestCandidateWithSubstitutedTypeArguments();
			if (!IsAppropriateCallTarget(expectedTargetDetails, method, foundMember))
				return OverloadResolutionErrors.AmbiguousMatch;
			return OverloadResolutionErrors.None;
		}

		static bool CanInferAnonymousTypePropertyNamesFromArguments(IList<Expression> args, IReadOnlyList<IParameter> parameters)
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

		bool IsUnambiguousAccess(ExpectedTargetDetails expectedTargetDetails, ResolveResult target, IMethod method,
			IList<TranslatedExpression> arguments, string[] argumentNames, out IMember foundMember)
		{
			foundMember = null;
			if (target == null) {
				var result = resolver.ResolveSimpleName(method.AccessorOwner.Name,
					EmptyList<IType>.Instance,
					isInvocationTarget: false) as MemberResolveResult;
				if (result == null || result.IsError)
					return false;
				foundMember = result.Member;
			} else {
				var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentModule);
				if (method.AccessorOwner.SymbolKind == SymbolKind.Indexer) {
					var or = new OverloadResolution(resolver.Compilation,
						arguments.SelectArray(a => a.ResolveResult),
						argumentNames: argumentNames,
						typeArguments: Empty<IType>.Array,
						conversions: expressionBuilder.resolver.conversions);
					or.AddMethodLists(lookup.LookupIndexers(target));
					if (or.BestCandidateErrors != OverloadResolutionErrors.None)
						return false;
					if (or.IsAmbiguous)
						return false;
					foundMember = or.GetBestCandidateWithSubstitutedTypeArguments();
				} else {
					var result = lookup.Lookup(target,
						method.AccessorOwner.Name,
						EmptyList<IType>.Instance,
						isInvocation: false) as MemberResolveResult;
					if (result == null || result.IsError)
						return false;
					foundMember = result.Member;
				}
			}
			return foundMember != null && IsAppropriateCallTarget(expectedTargetDetails, method.AccessorOwner, foundMember);
		}

		ExpressionWithResolveResult HandleAccessorCall(ExpectedTargetDetails expectedTargetDetails, IMethod method,
			TranslatedExpression target, IList<TranslatedExpression> arguments, string[] argumentNames)
		{
			bool requireTarget;
			if (method.AccessorOwner.SymbolKind == SymbolKind.Indexer || expressionBuilder.HidesVariableWithName(method.AccessorOwner.Name))
				requireTarget = true;
			else if (method.IsStatic)
				requireTarget = !expressionBuilder.IsCurrentOrContainingType(method.DeclaringTypeDefinition);
			else
				requireTarget = !(target.Expression is ThisReferenceExpression);
			bool targetCasted = false;
			bool isSetter = method.ReturnType.IsKnownType(KnownTypeCode.Void);
			bool argumentsCasted = (isSetter && method.Parameters.Count == 1) || (!isSetter && method.Parameters.Count == 0);
			var targetResolveResult = requireTarget ? target.ResolveResult : null;

			TranslatedExpression value = default(TranslatedExpression);
			if (isSetter) {
				value = arguments.Last();
				arguments.Remove(value);
			}

			IMember foundMember;
			while (!IsUnambiguousAccess(expectedTargetDetails, targetResolveResult, method, arguments, argumentNames, out foundMember)) {
				if (!argumentsCasted) {
					argumentsCasted = true;
					CastArguments(arguments, method.Parameters);
				} else if(!requireTarget) {
					requireTarget = true;
					targetResolveResult = target.ResolveResult;
				} else if (!targetCasted) {
					targetCasted = true;
					target = target.ConvertTo(method.AccessorOwner.DeclaringType, expressionBuilder);
					targetResolveResult = target.ResolveResult;
				} else {
					foundMember = method.AccessorOwner;
					break;
				}
			}
			
			var rr = new MemberResolveResult(target.ResolveResult, foundMember);

			if (isSetter) {
				TranslatedExpression expr;

				if (arguments.Count != 0) {
					expr = new IndexerExpression(target.Expression, arguments.Select(a => a.Expression))
						.WithoutILInstruction().WithRR(rr);
				} else if (requireTarget) {
					expr = new MemberReferenceExpression(target.Expression, method.AccessorOwner.Name)
						.WithoutILInstruction().WithRR(rr);
				} else {
					expr = new IdentifierExpression(method.AccessorOwner.Name)
						.WithoutILInstruction().WithRR(rr);
				}

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
				return new AssignmentExpression(expr, op, value.Expression).WithRR(new TypeResolveResult(method.AccessorOwner.ReturnType));
			} else {
				if (arguments.Count != 0) {
					return new IndexerExpression(target.Expression, arguments.Select(a => a.Expression))
						.WithoutILInstruction().WithRR(rr);
				} else if (requireTarget) {
					return new MemberReferenceExpression(target.Expression, method.AccessorOwner.Name)
						.WithoutILInstruction().WithRR(rr);
				} else {
					return new IdentifierExpression(method.AccessorOwner.Name)
						.WithoutILInstruction().WithRR(rr);
				}
			}
		}

		bool IsAppropriateCallTarget(ExpectedTargetDetails expectedTargetDetails, IMember expectedTarget, IMember actualTarget)
		{
			if (expectedTarget.Equals(actualTarget, NormalizeTypeVisitor.TypeErasure))
				return true;

			if (expectedTargetDetails.CallOpCode == OpCode.CallVirt && actualTarget.IsOverride) {
				if (expectedTargetDetails.NeedsBoxingConversion && actualTarget.DeclaringType.IsReferenceType != true)
					return false;
				foreach (var possibleTarget in InheritanceHelper.GetBaseMembers(actualTarget, false)) {
					if (expectedTarget.Equals(possibleTarget, NormalizeTypeVisitor.TypeErasure))
						return true;
					if (!possibleTarget.IsOverride)
						break;
				}
			}
			return false;
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
					throw new ArgumentException($"Unknown instruction type: {func.OpCode}");
			}
			var invokeMethod = inst.Method.DeclaringType.GetDelegateInvokeMethod();
			TranslatedExpression target;
			IType targetType;
			bool requireTarget;
			if (method.IsExtensionMethod && invokeMethod != null && method.Parameters.Count - 1 == invokeMethod.Parameters.Count) {
				targetType = method.Parameters[0].Type;
				target = expressionBuilder.Translate(inst.Arguments[0], targetType);
				target = ExpressionBuilder.UnwrapBoxingConversion(target);
				requireTarget = true;
			} else {
				targetType = method.DeclaringType;
				target = expressionBuilder.TranslateTarget(inst.Arguments[0],
					nonVirtualInvocation: func.OpCode == OpCode.LdFtn,
					memberStatic: method.IsStatic,
					memberDeclaringType: method.DeclaringType);
				target = ExpressionBuilder.UnwrapBoxingConversion(target);
				requireTarget = expressionBuilder.HidesVariableWithName(method.Name)
					|| (method.IsStatic ? !expressionBuilder.IsCurrentOrContainingType(method.DeclaringTypeDefinition) : !(target.Expression is ThisReferenceExpression));
			}
			var expectedTargetDetails = new ExpectedTargetDetails {
				CallOpCode = inst.OpCode
			};
			bool needsCast = false;
			ResolveResult result = null;
			var or = new OverloadResolution(resolver.Compilation, method.Parameters.SelectReadOnlyArray(p => new TypeResolveResult(p.Type)));
			if (!requireTarget) {
				result = resolver.ResolveSimpleName(method.Name, method.TypeArguments, isInvocationTarget: false);
				if (result is MethodGroupResolveResult mgrr) {
					or.AddMethodLists(mgrr.MethodsGroupedByDeclaringType.ToArray());
					requireTarget = (or.BestCandidateErrors != OverloadResolutionErrors.None || !IsAppropriateCallTarget(expectedTargetDetails, method, or.BestCandidate));
				} else {
					requireTarget = true;
				}
			}
			MemberLookup lookup = null;
			if (requireTarget) {
				lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentModule);
				var rr = lookup.Lookup(target.ResolveResult, method.Name, method.TypeArguments, false) ;
				needsCast = true;
				result = rr;
				if (rr is MethodGroupResolveResult mgrr) {
					or.AddMethodLists(mgrr.MethodsGroupedByDeclaringType.ToArray());
					needsCast = (or.BestCandidateErrors != OverloadResolutionErrors.None || !IsAppropriateCallTarget(expectedTargetDetails, method, or.BestCandidate));
				}
			}
			if (needsCast) {
				Debug.Assert(requireTarget);
				target = target.ConvertTo(targetType, expressionBuilder);
				result = lookup.Lookup(target.ResolveResult, method.Name, method.TypeArguments, false);
			}
			Expression targetExpression;
			if (requireTarget) {
				var mre = new MemberReferenceExpression(target, method.Name);
				mre.TypeArguments.AddRange(method.TypeArguments.Select(expressionBuilder.ConvertType));
				mre.WithRR(result);
				targetExpression = mre;
			} else {
				var ide = new IdentifierExpression(method.Name)
					.WithRR(result);
				targetExpression = ide;
			}
			var oce = new ObjectCreateExpression(expressionBuilder.ConvertType(inst.Method.DeclaringType), targetExpression)
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(
					inst.Method.DeclaringType,
					new MemberResolveResult(target.ResolveResult, method),
					Conversion.MethodGroupConversion(method, func.OpCode == OpCode.LdVirtFtn, false)));
			return oce;
		}

		internal TranslatedExpression CallWithNamedArgs(Block block)
		{
			Debug.Assert(block.Kind == BlockKind.CallWithNamedArgs);
			var call = (CallInstruction)block.FinalInstruction;
			var arguments = new ILInstruction[call.Arguments.Count];
			var argumentToParameterMap = new int[arguments.Length];
			int firstParamIndex = call.IsInstanceCall ? 1 : 0;
			// Arguments from temporary variables (VariableKind.NamedArgument):
			int pos = 0;
			foreach (StLoc stloc in block.Instructions) {
				Debug.Assert(stloc.Variable.LoadInstructions.Single().Parent == call);
				arguments[pos] = stloc.Value;
				argumentToParameterMap[pos] = stloc.Variable.LoadInstructions.Single().ChildIndex - firstParamIndex;
				pos++;
			}
			// Remaining argument:
			foreach (var arg in call.Arguments) {
				if (arg.MatchLdLoc(out var v) && v.Kind == VariableKind.NamedArgument) {
					continue; // already handled in loop above
				}
				arguments[pos] = arg;
				argumentToParameterMap[pos] = arg.ChildIndex - firstParamIndex;
				pos++;
			}
			Debug.Assert(pos == arguments.Length);
			return Build(call.OpCode, call.Method, arguments, argumentToParameterMap, call.ConstrainedTo)
				.WithILInstruction(call).WithILInstruction(block);
		}
	}
}