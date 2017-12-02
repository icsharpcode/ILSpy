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
				var arg = expressionBuilder.Translate(callArguments[firstParamIndex + i]);
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
						if (IsUnambiguousCall(expectedTargetDetails, method, target, Array.Empty<IType>(), expandedArguments) == OverloadResolutionErrors.None) {
							isExpandedForm = true;
							expectedParameters = expandedParameters;
							arguments = expandedArguments.SelectList(a => new TranslatedExpression(a.Expression.Detach()));
							continue;
						}
					}
				}

				arguments.Add(arg.ConvertTo(parameter.Type, expressionBuilder, allowImplicitConversion: true));

				if (parameter.IsOut && arguments[i].Expression is DirectionExpression dirExpr) {
					dirExpr.FieldDirection = FieldDirection.Out;
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

					if (IsUnambiguousCall(expectedTargetDetails, method, target, Array.Empty<IType>(), arguments) != OverloadResolutionErrors.None) {
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
				} else if (method.Name == "Invoke" && method.DeclaringType.Kind == TypeKind.Delegate) {
					return new InvocationExpression(target, arguments.Select(arg => arg.Expression)).WithRR(rr);
				} else if (IsDelegateEqualityComparison(method, arguments)) {
					return HandleDelegateEqualityComparison(method, arguments)
						.WithRR(rr);
				} else if (method.IsOperator && method.Name == "op_Implicit" && arguments.Count == 1) {
					return HandleImplicitConversion(method, arguments[0]);
				} else {
					bool requireTypeArguments = false;
					bool targetCasted = false;
					bool argumentsCasted = false;
					IType[] typeArguments = Array.Empty<IType>();

					OverloadResolutionErrors errors;
					while ((errors = IsUnambiguousCall(expectedTargetDetails, method, target, typeArguments, arguments)) != OverloadResolutionErrors.None) {
						switch (errors) {
							case OverloadResolutionErrors.TypeInferenceFailed:
							case OverloadResolutionErrors.WrongNumberOfTypeArguments:
								if (requireTypeArguments) goto default;
								requireTypeArguments = true;
								typeArguments = method.TypeArguments.ToArray();
								continue;
							default:
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
								} else if (!targetCasted) {
									targetCasted = true;
									target = target.ConvertTo(method.DeclaringType, expressionBuilder);
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

					Expression targetExpr = target.Expression;
					string methodName = method.Name;
					// HACK : convert this.Dispose() to ((IDisposable)this).Dispose(), if Dispose is an explicitly implemented interface method.
					if (method.IsExplicitInterfaceImplementation && targetExpr is ThisReferenceExpression) {
						targetExpr = new CastExpression(expressionBuilder.ConvertType(method.ImplementedInterfaceMembers[0].DeclaringType), targetExpr);
						methodName = method.ImplementedInterfaceMembers[0].Name;
					}
					var mre = new MemberReferenceExpression(targetExpr, methodName);
					if (requireTypeArguments && (!settings.AnonymousTypes || !method.TypeArguments.Any(a => a.ContainsAnonymousType())))
						mre.TypeArguments.AddRange(method.TypeArguments.Select(expressionBuilder.ConvertType));
					var argumentExpressions = arguments.Select(arg => arg.Expression);
					return new InvocationExpression(mre, argumentExpressions).WithRR(rr);
				}
			}
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
			TranslatedExpression target, IType[] typeArguments, IList<TranslatedExpression> arguments)
		{
			var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentAssembly);
			var or = new OverloadResolution(resolver.Compilation, arguments.SelectArray(a => a.ResolveResult), typeArguments: typeArguments);
			if (expectedTargetDetails.CallOpCode == OpCode.NewObj) {
				foreach (IMethod ctor in method.DeclaringType.GetConstructors()) {
					if (lookup.IsAccessible(ctor, allowProtectedAccess: resolver.CurrentTypeDefinition == method.DeclaringTypeDefinition)) {
						or.AddCandidate(ctor);
					}
				}
			} else {
				var result = lookup.Lookup(target.ResolveResult, method.Name, EmptyList<IType>.Instance, true) as MethodGroupResolveResult;
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

		ExpressionWithResolveResult HandleAccessorCall(ExpectedTargetDetails expectedTargetDetails, IMethod method, TranslatedExpression target, IList<TranslatedExpression> arguments)
		{
			var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentAssembly);
			var result = lookup.Lookup(target.ResolveResult, method.AccessorOwner.Name, EmptyList<IType>.Instance, isInvocation: false);

			if (result.IsError || (result is MemberResolveResult && !IsAppropriateCallTarget(expectedTargetDetails, method.AccessorOwner, ((MemberResolveResult)result).Member)))
				target = target.ConvertTo(method.AccessorOwner.DeclaringType, expressionBuilder);
			var rr = new MemberResolveResult(target.ResolveResult, method.AccessorOwner);

			if (method.ReturnType.IsKnownType(KnownTypeCode.Void)) {
				var value = arguments.Last();
				arguments.Remove(value);
				TranslatedExpression expr;
				if (arguments.Count == 0)
					expr = new MemberReferenceExpression(target.Expression, method.AccessorOwner.Name)
						.WithoutILInstruction().WithRR(rr);
				else
					expr = new IndexerExpression(target.Expression, arguments.Select(a => a.Expression))
						.WithoutILInstruction().WithRR(rr);
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
				if (arguments.Count == 0)
					return new MemberReferenceExpression(target.Expression, method.AccessorOwner.Name).WithRR(rr);
				else
					return new IndexerExpression(target.Expression, arguments.Select(a => a.Expression)).WithRR(rr);
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
			if (method.IsExtensionMethod && invokeMethod != null && method.Parameters.Count - 1 == invokeMethod.Parameters.Count) {
				target = expressionBuilder.Translate(inst.Arguments[0]);
				targetType = method.Parameters[0].Type;
			} else {
				target = expressionBuilder.TranslateTarget(method, inst.Arguments[0], func.OpCode == OpCode.LdFtn);
				targetType = method.DeclaringType;
			}
			var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentAssembly);
			var or = new OverloadResolution(resolver.Compilation, method.Parameters.SelectArray(p => new TypeResolveResult(p.Type)));
			var result = lookup.Lookup(target.ResolveResult, method.Name, method.TypeArguments, false);

			bool needsCast = true;
			if (result is MethodGroupResolveResult mgrr) {
				or.AddMethodLists(mgrr.MethodsGroupedByDeclaringType.ToArray());
				var expectedTargetDetails = new ExpectedTargetDetails {
					CallOpCode = inst.OpCode
				};
				needsCast = (or.BestCandidateErrors != OverloadResolutionErrors.None || !IsAppropriateCallTarget(expectedTargetDetails, method, or.BestCandidate));
			}
			if (needsCast) {
				target = target.ConvertTo(targetType, expressionBuilder);
				result = lookup.Lookup(target.ResolveResult, method.Name, method.TypeArguments, false);
			}

			var mre = new MemberReferenceExpression(target, method.Name);
			mre.TypeArguments.AddRange(method.TypeArguments.Select(expressionBuilder.ConvertType));
			mre.WithRR(result);
			var oce = new ObjectCreateExpression(expressionBuilder.ConvertType(inst.Method.DeclaringType), mre)
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(
					inst.Method.DeclaringType,
					new MemberResolveResult(target.ResolveResult, method),
					Conversion.MethodGroupConversion(method, func.OpCode == OpCode.LdVirtFtn, false)));
			return oce;
		}
	}
}