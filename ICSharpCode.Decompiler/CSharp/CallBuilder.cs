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
			return Build(inst.OpCode, inst.Method, inst.Arguments, inst.ConstrainedTo).WithILInstruction(inst);
		}

		public ExpressionWithResolveResult Build(OpCode callOpCode, IMethod method, IReadOnlyList<ILInstruction> callArguments,
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
				target = expressionBuilder.TranslateTarget(method, callArguments.FirstOrDefault(), callOpCode == OpCode.Call, constrainedTo);
				if (callOpCode == OpCode.CallVirt
					&& constrainedTo == null
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

			// Translate arguments to the expected parameter types
			var arguments = new List<TranslatedExpression>(method.Parameters.Count);
			Debug.Assert(callArguments.Count == firstParamIndex + method.Parameters.Count);
			var expectedParameters = method.Parameters.ToList();
			bool isExpandedForm = false;
			for (int i = 0; i < method.Parameters.Count; i++) {
				var parameter = expectedParameters[i];
				var arg = expressionBuilder.Translate(callArguments[firstParamIndex + i], parameter.Type);
				if (parameter.IsParams && i + 1 == method.Parameters.Count) {
					// Parameter is marked params
					// If the argument is an array creation, inline all elements into the call and add missing default values.
					// Otherwise handle it normally.
					if (arg.ResolveResult is ArrayCreateResolveResult acrr &&
						acrr.SizeArguments.Count == 1 &&
						acrr.SizeArguments[0].IsCompileTimeConstant &&
						acrr.SizeArguments[0].ConstantValue is int length) {
						var expandedParameters = expectedParameters.Take(expectedParameters.Count - 1).ToList();
						var expandedArguments = new List<TranslatedExpression>(arguments);
						if (length > 0) {
							var arrayElements = ((ArrayCreateExpression)arg.Expression).Initializer.Elements.ToArray();
							var elementType = ((ArrayType)acrr.Type).ElementType;
							for (int j = 0; j < length; j++) {
								expandedParameters.Add(new DefaultParameter(elementType, parameter.Name + j));
								if (j < arrayElements.Length)
									expandedArguments.Add(new TranslatedExpression(arrayElements[j]));
								else
									expandedArguments.Add(expressionBuilder.GetDefaultValueExpression(elementType).WithoutILInstruction());
							}
						}
						if (IsUnambiguousCall(expectedTargetDetails, method, target.ResolveResult, Empty<IType>.Array, expandedArguments) == OverloadResolutionErrors.None) {
							isExpandedForm = true;
							expectedParameters = expandedParameters;
							arguments = expandedArguments.SelectList(a => new TranslatedExpression(a.Expression.Detach()));
							continue;
						}
					}
				}

				arguments.Add(arg.ConvertTo(parameter.Type, expressionBuilder, allowImplicitConversion: true));

				if (parameter.IsOut && arguments[i].Expression is DirectionExpression dirExpr && arguments[i].ResolveResult is ByReferenceResolveResult brrr) {
					dirExpr.FieldDirection = FieldDirection.Out;
					dirExpr.RemoveAnnotations<ByReferenceResolveResult>();
					if (brrr.ElementResult == null)
						brrr = new ByReferenceResolveResult(brrr.ElementType, isOut: true);
					else
						brrr = new ByReferenceResolveResult(brrr.ElementResult, isOut: true);
					dirExpr.AddAnnotation(brrr);
					arguments[i] = new TranslatedExpression(dirExpr);
				}
			}

			if (method is VarArgInstanceMethod) {
				int regularParameterCount = ((VarArgInstanceMethod)method).RegularParameterCount;
				var argListArg = new UndocumentedExpression();
				argListArg.UndocumentedExpressionType = UndocumentedExpressionType.ArgList;
				int paramIndex = regularParameterCount;
				var builder = expressionBuilder;
				argListArg.Arguments.AddRange(arguments.Skip(regularParameterCount).Select(arg => arg.ConvertTo(expectedParameters[paramIndex++].Type, builder).Expression));
				var argListRR = new ResolveResult(SpecialType.ArgList);
				arguments = arguments.Take(regularParameterCount)
					.Concat(new[] { argListArg.WithoutILInstruction().WithRR(argListRR) }).ToList();
				method = (IMethod)method.MemberDefinition;
				expectedParameters = method.Parameters.ToList();
			}

			var argumentResolveResults = arguments.Select(arg => arg.ResolveResult).ToList();

			ResolveResult rr = new CSharpInvocationResolveResult(target.ResolveResult, method, argumentResolveResults, isExpandedForm: isExpandedForm);

			if (callOpCode == OpCode.NewObj) {
				if (settings.AnonymousTypes && method.DeclaringType.IsAnonymousType()) {
					var argumentExpressions = arguments.SelectArray(arg => arg.Expression);
					AnonymousTypeCreateExpression atce = new AnonymousTypeCreateExpression();
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
					return atce
						.WithRR(rr);
				} else {
					if (IsUnambiguousCall(expectedTargetDetails, method, null, Empty<IType>.Array, arguments) != OverloadResolutionErrors.None) {
						for (int i = 0; i < arguments.Count; i++) {
							if (settings.AnonymousTypes && expectedParameters[i].Type.ContainsAnonymousType()) {
								if (arguments[i].Expression is LambdaExpression lambda) {
									ModifyReturnTypeOfLambda(lambda);
								}
							} else {
								arguments[i] = arguments[i].ConvertTo(expectedParameters[i].Type, expressionBuilder);
							}
						}
					}
					return new ObjectCreateExpression(expressionBuilder.ConvertType(method.DeclaringType), arguments.SelectArray(arg => arg.Expression))
						.WithRR(rr);
				}
			} else {
				int allowedParamCount = (method.ReturnType.IsKnownType(KnownTypeCode.Void) ? 1 : 0);
				if (method.IsAccessor && (method.AccessorOwner.SymbolKind == SymbolKind.Indexer || expectedParameters.Count == allowedParamCount)) {
					return HandleAccessorCall(expectedTargetDetails, method, target, arguments.ToList());
				} else if (method.Name == "Invoke" && method.DeclaringType.Kind == TypeKind.Delegate && !IsNullConditional(target)) {
					return new InvocationExpression(target, arguments.Select(arg => arg.Expression)).WithRR(rr);
				} else if (IsDelegateEqualityComparison(method, arguments)) {
					return HandleDelegateEqualityComparison(method, arguments)
						.WithRR(rr);
				} else if (method.IsOperator && method.Name == "op_Implicit" && arguments.Count == 1) {
					return HandleImplicitConversion(method, arguments[0]);
				} else {
					bool requireTypeArguments = false;
					bool requireTarget;
					if (expressionBuilder.HidesVariableWithName(method.Name)) {
						requireTarget = true;
					} else {
						if (method.IsStatic)
							requireTarget = !expressionBuilder.IsCurrentOrContainingType(method.DeclaringTypeDefinition) || method.Name == ".cctor";
						else
							requireTarget = !(target.Expression is ThisReferenceExpression || target.Expression is BaseReferenceExpression) || method.Name == ".ctor";
					}
					bool targetCasted = false;
					bool argumentsCasted = false;
					IType[] typeArguments = Empty<IType>.Array;
					var targetResolveResult = requireTarget ? target.ResolveResult : null;
					OverloadResolutionErrors errors;
					while ((errors = IsUnambiguousCall(expectedTargetDetails, method, targetResolveResult, typeArguments, arguments)) != OverloadResolutionErrors.None) {
						switch (errors) {
							case OverloadResolutionErrors.TypeInferenceFailed:
							case OverloadResolutionErrors.WrongNumberOfTypeArguments:
								if (requireTypeArguments) goto default;
								requireTypeArguments = true;
								typeArguments = method.TypeArguments.ToArray();
								continue;
							default:
								// TODO : implement some more intelligent algorithm that decides which of these fixes (cast args, add target, cast target, add type args)
								// is best in this case. Additionally we should not cast all arguments at once, but step-by-step try to add only a minimal number of casts.
								if (!argumentsCasted) {
									argumentsCasted = true;
									for (int i = 0; i < arguments.Count; i++) {
										if (settings.AnonymousTypes && expectedParameters[i].Type.ContainsAnonymousType()) {
											if (arguments[i].Expression is LambdaExpression lambda) {
												ModifyReturnTypeOfLambda(lambda);
											}
										} else {
											arguments[i] = arguments[i].ConvertTo(expectedParameters[i].Type, expressionBuilder);
										}
									}
								} else if (!requireTarget) {
									requireTarget = true;
									targetResolveResult = target.ResolveResult;
								} else if (!targetCasted) {
									targetCasted = true;
									target = target.ConvertTo(method.DeclaringType, expressionBuilder);
									targetResolveResult = target.ResolveResult;
								} else if (!requireTypeArguments) {
									requireTypeArguments = true;
									typeArguments = method.TypeArguments.ToArray();
								} else {
									break;
								}
								continue;
						}
						break;
					}

					Expression targetExpr;
					string methodName = method.Name;
					AstNodeCollection<AstType> typeArgumentList;
					if (requireTarget) {
						targetExpr = new MemberReferenceExpression(target.Expression, methodName);
						typeArgumentList = ((MemberReferenceExpression)targetExpr).TypeArguments;

						// HACK : convert this.Dispose() to ((IDisposable)this).Dispose(), if Dispose is an explicitly implemented interface method.
						if (method.IsExplicitInterfaceImplementation && target.Expression is ThisReferenceExpression) {
							var castExpression = new CastExpression(expressionBuilder.ConvertType(method.ImplementedInterfaceMembers[0].DeclaringType), target.Expression);
							methodName = method.ImplementedInterfaceMembers[0].Name;
							targetExpr = new MemberReferenceExpression(castExpression, methodName);
							typeArgumentList = ((MemberReferenceExpression)targetExpr).TypeArguments;
						}
					} else {
						targetExpr = new IdentifierExpression(methodName);
						typeArgumentList = ((IdentifierExpression)targetExpr).TypeArguments;
					}

					if (requireTypeArguments && (!settings.AnonymousTypes || !method.TypeArguments.Any(a => a.ContainsAnonymousType())))
						typeArgumentList.AddRange(method.TypeArguments.Select(expressionBuilder.ConvertType));
					var argumentExpressions = arguments.Select(arg => arg.Expression);
					return new InvocationExpression(targetExpr, argumentExpressions).WithRR(rr);
				}
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
			ResolveResult target, IType[] typeArguments, IList<TranslatedExpression> arguments)
		{
			var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentAssembly);
			var or = new OverloadResolution(resolver.Compilation, arguments.SelectArray(a => a.ResolveResult), typeArguments: typeArguments);
			if (expectedTargetDetails.CallOpCode == OpCode.NewObj) {
				foreach (IMethod ctor in method.DeclaringType.GetConstructors()) {
					if (lookup.IsAccessible(ctor, allowProtectedAccess: resolver.CurrentTypeDefinition == method.DeclaringTypeDefinition)) {
						or.AddCandidate(ctor);
					}
				}
			} else if (target == null) {
				var result = resolver.ResolveSimpleName(method.Name, typeArguments, isInvocationTarget: true) as MethodGroupResolveResult;
				if (result == null)
					return OverloadResolutionErrors.AmbiguousMatch;
				or.AddMethodLists(result.MethodsGroupedByDeclaringType.ToArray());
			} else {
				var result = lookup.Lookup(target, method.Name, EmptyList<IType>.Instance, isInvocation: true) as MethodGroupResolveResult;
				if (result == null)
					return OverloadResolutionErrors.AmbiguousMatch;
				or.AddMethodLists(result.MethodsGroupedByDeclaringType.ToArray());
			}
			if (or.BestCandidateErrors != OverloadResolutionErrors.None)
				return or.BestCandidateErrors;
			if (!IsAppropriateCallTarget(expectedTargetDetails, method, or.GetBestCandidateWithSubstitutedTypeArguments()))
				return OverloadResolutionErrors.AmbiguousMatch;
			return OverloadResolutionErrors.None;
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

		bool IsUnambiguousAccess(ExpectedTargetDetails expectedTargetDetails, ResolveResult target, IMethod method)
		{
			if (target == null) {
				var result = resolver.ResolveSimpleName(method.AccessorOwner.Name, EmptyList<IType>.Instance, isInvocationTarget: false) as MemberResolveResult;
				return !(result == null || result.IsError || !IsAppropriateCallTarget(expectedTargetDetails, method.AccessorOwner, result.Member));
			} else {
				var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentAssembly);
				var result = lookup.Lookup(target, method.AccessorOwner.Name, EmptyList<IType>.Instance, isInvocation: false) as MemberResolveResult;
				return !(result == null || result.IsError || !IsAppropriateCallTarget(expectedTargetDetails, method.AccessorOwner, result.Member));
			}
		}

		ExpressionWithResolveResult HandleAccessorCall(ExpectedTargetDetails expectedTargetDetails, IMethod method, TranslatedExpression target, IList<TranslatedExpression> arguments)
		{
			bool requireTarget = expressionBuilder.HidesVariableWithName(method.AccessorOwner.Name)
				|| (method.IsStatic ? !expressionBuilder.IsCurrentOrContainingType(method.DeclaringTypeDefinition) : !(target.Expression is ThisReferenceExpression));
			bool targetCasted = false;
			var targetResolveResult = requireTarget ? target.ResolveResult : null;

			while (!IsUnambiguousAccess(expectedTargetDetails, targetResolveResult, method)) {
				if (!requireTarget) {
					requireTarget = true;
					targetResolveResult = target.ResolveResult;
				} else if (!targetCasted) {
					targetCasted = true;
					target = target.ConvertTo(method.AccessorOwner.DeclaringType, expressionBuilder);
					targetResolveResult = target.ResolveResult;
				} else {
					break;
				}
			}
			
			var rr = new MemberResolveResult(target.ResolveResult, method.AccessorOwner);

			if (method.ReturnType.IsKnownType(KnownTypeCode.Void)) {
				var value = arguments.Last();
				arguments.Remove(value);
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
			if (expectedTarget.Equals(actualTarget))
				return true;

			if (expectedTargetDetails.CallOpCode == OpCode.CallVirt && actualTarget.IsOverride) {
				if (expectedTargetDetails.NeedsBoxingConversion && actualTarget.DeclaringType.IsReferenceType != true)
					return false;
				foreach (var possibleTarget in InheritanceHelper.GetBaseMembers(actualTarget, false)) {
					if (expectedTarget.Equals(possibleTarget))
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
				target = expressionBuilder.TranslateTarget(method, inst.Arguments[0], func.OpCode == OpCode.LdFtn);
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
				lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentAssembly);
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
	}
}