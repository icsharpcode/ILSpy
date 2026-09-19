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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

#nullable enable

namespace ICSharpCode.Decompiler.CSharp
{
	internal struct ExpectedTargetDetails
	{
		public OpCode CallOpCode;
		public bool NeedsBoxingConversion;
	}

	internal struct ArgumentList
	{
		public TranslatedExpression[] Arguments;
		public IParameter[] ExpectedParameters;
		public string[] ParameterNames;
		public string[]? ArgumentNames;
		public int FirstOptionalArgumentIndex;
		public BitSet IsPrimitiveValue;
		public IReadOnlyList<int>? ArgumentToParameterMap;

		public bool AddNamesToPrimitiveValues;
		public bool UseImplicitlyTypedOut;
		public bool IsExpandedForm;
		public int Length => Arguments.Length;

		public int GetActualArgumentCount()
		{
			if (FirstOptionalArgumentIndex < 0)
				return Arguments.Length;
			Debug.Assert(FirstOptionalArgumentIndex <= Arguments.Length);
			return FirstOptionalArgumentIndex;
		}

		public string[]? GetArgumentNames(int skipCount = 0)
		{
			string[]? argumentNames = ArgumentNames;
			if (AddNamesToPrimitiveValues && IsPrimitiveValue.Any() && !IsExpandedForm
					&& !ParameterNames.Any(string.IsNullOrEmpty))
			{
				Debug.Assert(skipCount == 0);
				if (argumentNames == null)
				{
					argumentNames = new string[Arguments.Length];
				}

				for (int i = 0; i < Arguments.Length; i++)
				{
					if (IsPrimitiveValue[i] && argumentNames[i] == null)
					{
						argumentNames[i] = ParameterNames[i];
					}
				}
			}

			return argumentNames;
		}

		public IList<ResolveResult> GetArgumentResolveResults(int skipCount = 0)
		{
			var expectedParameters = ExpectedParameters;
			var useImplicitlyTypedOut = UseImplicitlyTypedOut;

			return Arguments
				.SelectWithIndex(GetResolveResult)
				.Skip(skipCount)
				.Take(GetActualArgumentCount())
				.ToArray();

			ResolveResult GetResolveResult(int index, TranslatedExpression expression)
			{
				var param = expectedParameters[index];
				if (useImplicitlyTypedOut && param.ReferenceKind == ReferenceKind.Out && expression.Type is ByReferenceType brt)
					return new OutVarResolveResult(brt.ElementType);
				return expression.ResolveResult;
			}
		}

		public IList<ResolveResult> GetArgumentResolveResultsDirect(int skipCount = 0)
		{
			return Arguments
				.Skip(skipCount)
				.Take(GetActualArgumentCount())
				.Select(a => a.ResolveResult)
				.ToArray();
		}

		public IEnumerable<Expression> GetArgumentExpressions(int skipCount = 0)
		{
			var argumentNames = GetArgumentNames(skipCount);
			int argumentCount = GetActualArgumentCount();
			var useImplicitlyTypedOut = UseImplicitlyTypedOut;
			if (argumentNames == null)
			{
				return Arguments.Skip(skipCount).Take(argumentCount).Select(arg => AddAnnotations(arg.Expression));
			}
			else
			{
				Debug.Assert(skipCount == 0);
				return Arguments.Take(argumentCount).Zip(argumentNames.Take(argumentCount),
					(arg, name) => {
						if (name == null)
							return AddAnnotations(arg.Expression);
						else
							return new NamedArgumentExpression(name, AddAnnotations(arg.Expression));
					});
			}

			Expression AddAnnotations(Expression expression)
			{
				if (!useImplicitlyTypedOut)
					return expression;
				if (expression.GetResolveResult() is ByReferenceResolveResult { ReferenceKind: ReferenceKind.Out } brrr)
				{
					expression.AddAnnotation(UseImplicitlyTypedOutAnnotation.Instance);
				}
				return expression;
			}
		}

		public bool CanInferAnonymousTypePropertyNamesFromArguments()
		{
			for (int i = 0; i < Arguments.Length; i++)
			{
				string? inferredName;
				switch (Arguments[i].Expression)
				{
					case IdentifierExpression identifier:
						inferredName = identifier.Identifier;
						break;
					case MemberReferenceExpression member:
						inferredName = member.MemberName;
						break;
					default:
						inferredName = null;
						break;
				}

				if (inferredName != ExpectedParameters[i].Name)
				{
					return false;
				}
			}
			return true;
		}

		[Conditional("DEBUG")]
		public void CheckNoNamedOrOptionalArguments()
		{
			Debug.Assert(ArgumentToParameterMap == null && ArgumentNames == null && FirstOptionalArgumentIndex < 0);
		}
	}

	struct CallBuilder
	{

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

		public TranslatedExpression Build(CallInstruction inst, IType? typeHint = null)
		{
			if (inst is NewObj newobj && IL.Transforms.DelegateConstruction.MatchDelegateConstruction(newobj, out _, out _, out _))
			{
				return HandleDelegateConstruction(newobj);
			}
			if (settings.TupleTypes && TupleTransform.MatchTupleConstruction(inst as NewObj, out var tupleElements) && tupleElements.Length >= 2)
			{
				var elementTypes = TupleType.GetTupleElementTypes(inst.Method.DeclaringType);
				var elementNames = typeHint is TupleType tt ? tt.ElementNames : default;
				Debug.Assert(!elementTypes.IsDefault, "MatchTupleConstruction should not succeed unless we got a valid tuple type.");
				Debug.Assert(elementTypes.Length == tupleElements.Length);
				var tuple = new TupleExpression();
				var elementRRs = new List<ResolveResult>();
				foreach (var (index, element, elementType) in tupleElements.ZipWithIndex(elementTypes))
				{
					var translatedElement = expressionBuilder.Translate(element, elementType)
						.ConvertTo(elementType, expressionBuilder, allowImplicitConversion: true);
					if (elementNames.IsDefaultOrEmpty || elementNames.ElementAtOrDefault(index) is not string { Length: > 0 } name)
					{
						tuple.Elements.Add(translatedElement.Expression);
					}
					else
					{
						tuple.Elements.Add(new NamedArgumentExpression(name, translatedElement.Expression));
					}
					elementRRs.Add(translatedElement.ResolveResult);
				}
				return tuple.WithRR(new TupleResolveResult(
					expressionBuilder.compilation,
					elementRRs.ToImmutableArray(),
					elementNames,
					valueTupleAssembly: inst.Method.DeclaringType.GetDefinition()?.ParentModule
				)).WithILInstruction(inst);
			}
			if (settings.StringConcat && IsSpanBasedStringConcat(inst, out var operands))
			{
				return BuildStringConcat(inst.Method, operands).WithILInstruction(inst);
			}
			var result = Build(inst.OpCode, inst.Method, inst.Arguments, constrainedTo: inst.ConstrainedTo)
				.WithILInstruction(inst);
			if (inst.IsTail)
			{
				// Surface the IL 'tail.' prefix as an inline marker, e.g. '/*tail.*/Callee(x)'.
				// F# emits tail calls pervasively, and the prefix is otherwise dropped entirely.
				result.Expression.AddLeadingTrivia(new Comment("tail.", CommentType.MultiLine));
			}
			return result;
		}

		private ExpressionWithResolveResult BuildStringConcat(IMethod method, List<(ILInstruction Instruction, KnownTypeCode TypeCode)> operands)
		{
			IType type = typeSystem.FindType(operands[0].TypeCode);
			ExpressionWithResolveResult result = expressionBuilder.Translate(operands[0].Instruction, type).ConvertTo(type, expressionBuilder);
			var rr = new MemberResolveResult(null, method);

			for (int i = 1; i < operands.Count; i++)
			{
				type = typeSystem.FindType(operands[i].TypeCode);
				var expr = expressionBuilder.Translate(operands[i].Instruction, type).ConvertTo(type, expressionBuilder);
				result = new BinaryOperatorExpression(result.Expression, BinaryOperatorType.Add, expr).WithRR(rr);
			}

			return result;
		}

		static bool IsSpanBasedStringConcat(CallInstruction call, [NotNullWhen(true)] out List<(ILInstruction, KnownTypeCode)>? operands)
		{
			operands = null;

			if (!IsSpanBasedStringConcat(call.Method))
			{
				return false;
			}

			int? firstStringArgumentIndex = null;
			operands = new();

			foreach (var arg in call.Arguments)
			{
				if (arg is Call opImplicit && IsStringToReadOnlySpanCharImplicitConversion(opImplicit.Method))
				{
					firstStringArgumentIndex ??= arg.ChildIndex;
					operands.Add((opImplicit.Arguments.Single(), KnownTypeCode.String));
				}
				else if (arg is NewObj { Arguments: [AddressOf addressOf] } newObj && ILInlining.IsReadOnlySpanCharCtor(newObj.Method))
				{
					operands.Add((addressOf.Value, KnownTypeCode.Char));
				}
				else
				{
					return false;
				}
			}

			return call.Arguments.Count >= 2 && firstStringArgumentIndex <= 1;
		}

		internal static bool IsSpanBasedStringConcat(IMethod method)
		{
			if (method is not { Name: "Concat", IsStatic: true })
			{
				return false;
			}
			if (!method.DeclaringType.IsKnownType(KnownTypeCode.String))
			{
				return false;
			}

			foreach (var p in method.Parameters)
			{
				if (!p.Type.IsKnownType(KnownTypeCode.ReadOnlySpanOfT))
					return false;
				if (!p.Type.TypeArguments[0].IsKnownType(KnownTypeCode.Char))
					return false;
			}

			return true;
		}

		internal static bool IsStringToReadOnlySpanCharImplicitConversion(IMethod method)
		{
			return method.IsOperator
				&& method.Name == "op_Implicit"
				&& method.Parameters.Count == 1
				&& method.ReturnType.IsKnownType(KnownTypeCode.ReadOnlySpanOfT)
				&& method.ReturnType.TypeArguments[0].IsKnownType(KnownTypeCode.Char)
				&& method.Parameters[0].Type.IsKnownType(KnownTypeCode.String);
		}

		/// <summary>
		/// Matches MemoryExtensions.AsSpan(string), the helper the C# 14 compiler emits for the
		/// implicit span conversion from string to ReadOnlySpan&lt;char&gt;.
		/// </summary>
		internal static bool IsStringToReadOnlySpanCharAsSpan(IMethod method)
		{
			return method is { IsStatic: true, Name: "AsSpan", Parameters.Count: 1, TypeArguments.Count: 0 }
				&& method.DeclaringType.FullName == "System.MemoryExtensions"
				&& method.ReturnType.IsKnownType(KnownTypeCode.ReadOnlySpanOfT)
				&& method.ReturnType.TypeArguments[0].IsKnownType(KnownTypeCode.Char)
				&& method.Parameters[0].Type.IsKnownType(KnownTypeCode.String);
		}

		/// <summary>
		/// Matches ReadOnlySpan&lt;To&gt;.CastUp&lt;From&gt;(ReadOnlySpan&lt;From&gt;), the helper the
		/// C# 14 compiler emits for the covariant implicit span conversion.
		/// </summary>
		internal static bool IsReadOnlySpanCastUp(IMethod method)
		{
			return method is { IsStatic: true, Name: "CastUp", Parameters.Count: 1, TypeArguments.Count: 1 }
				&& method.DeclaringType.IsKnownType(KnownTypeCode.ReadOnlySpanOfT)
				&& method.Parameters[0].Type.IsKnownType(KnownTypeCode.ReadOnlySpanOfT);
		}

		// Gets whether a call to `method` is equivalent to an implicit span conversion.
		static bool IsEquivalentToSpanConversion(IMethod method)
		{
			if (method.DeclaringType.IsKnownType(KnownTypeCode.SpanOfT)
				|| method.DeclaringType.IsKnownType(KnownTypeCode.ReadOnlySpanOfT))
			{
				if (method.IsOperator
					&& method.Name == "op_Implicit")
				{
					return true;
				}
			}
			return IsStringToReadOnlySpanCharAsSpan(method)
				|| IsReadOnlySpanCastUp(method);
		}

		public ExpressionWithResolveResult Build(OpCode callOpCode, IMethod method,
			IReadOnlyList<ILInstruction> callArguments,
			IReadOnlyList<int>? argumentToParameterMap = null,
			IType? constrainedTo = null)
		{
			if (method.IsExplicitInterfaceImplementation && callOpCode == OpCode.Call)
			{
				// Direct non-virtual call to explicit interface implementation.
				// This can't really be represented in C#, but at least in the case where
				// the class is sealed, we can equivalently call the interface member instead:
				var interfaceMembers = method.ExplicitlyImplementedInterfaceMembers.ToList();
				if (method.DeclaringTypeDefinition?.Kind == TypeKind.Class && method.DeclaringTypeDefinition.IsSealed && interfaceMembers.Count == 1)
				{
					method = (IMethod)interfaceMembers.Single();
					callOpCode = OpCode.CallVirt;
				}
			}
			// Used for Call, CallVirt and NewObj
			var expectedTargetDetails = new ExpectedTargetDetails {
				CallOpCode = callOpCode
			};
			ILFunction? localFunction = null;
			if (method.IsLocalFunction)
			{
				localFunction = expressionBuilder.ResolveLocalFunction(method);
				Debug.Assert(localFunction != null);
			}
			TranslatedExpression target;
			if (callOpCode == OpCode.NewObj)
			{
				target = default(TranslatedExpression); // no target
			}
			else if (localFunction != null)
			{
				var ide = new IdentifierExpression(localFunction.Name!);
				if (method.TypeArguments.Count > 0)
				{
					ide.TypeArguments.AddRange(method.TypeArguments.Select(expressionBuilder.ConvertType));
				}
				ide.AddAnnotation(localFunction);
				target = ide.WithoutILInstruction()
					.WithRR(ToMethodGroup(method, localFunction));
			}
			else
			{
				var thisArg = callArguments.FirstOrDefault();
				if (thisArg is LdObjIfRef ldObjIfRef)
				{
					Debug.Assert(constrainedTo != null);
					thisArg = ldObjIfRef.Target;
				}
				target = expressionBuilder.TranslateTarget(
					thisArg,
					nonVirtualInvocation: callOpCode == OpCode.Call || method.IsConstructor,
					memberStatic: method.IsStatic,
					memberDeclaringType: method.DeclaringType,
					constrainedTo: constrainedTo);
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

			var argumentList = BuildArgumentList(expectedTargetDetails, target.ResolveResult, method,
				firstParamIndex, callArguments, argumentToParameterMap);

			if (localFunction != null)
			{
				return new InvocationExpression(target, argumentList.GetArgumentExpressions())
					.WithRR(new CSharpInvocationResolveResult(target.ResolveResult, method,
						argumentList.GetArgumentResolveResults(), isExpandedForm: argumentList.IsExpandedForm));
			}

			if (method is VarArgInstanceMethod)
			{
				argumentList.FirstOptionalArgumentIndex = -1;
				argumentList.AddNamesToPrimitiveValues = false;
				argumentList.UseImplicitlyTypedOut = false;
				int regularParameterCount = ((VarArgInstanceMethod)method).RegularParameterCount;
				var argListArg = new UndocumentedExpression();
				argListArg.UndocumentedExpressionType = UndocumentedExpressionType.ArgList;
				int paramIndex = regularParameterCount;
				var builder = expressionBuilder;
				Debug.Assert(argumentToParameterMap == null && argumentList.ArgumentNames == null);
				argListArg.Arguments.AddRange(argumentList.Arguments.Skip(regularParameterCount).Select(arg => arg.ConvertTo(argumentList.ExpectedParameters[paramIndex++].Type, builder).Expression));
				var argListRR = new ResolveResult(SpecialType.ArgList);
				argumentList.Arguments = argumentList.Arguments.Take(regularParameterCount)
					.Concat(new[] { argListArg.WithoutILInstruction().WithRR(argListRR) }).ToArray();
				method = ((VarArgInstanceMethod)method).BaseMethod;
				argumentList.ExpectedParameters = method.Parameters.ToArray();
			}

			if (settings.Ranges)
			{
				if (HandleRangeConstruction(out var result, callOpCode, method, target, argumentList))
				{
					return result;
				}
			}

			if (callOpCode == OpCode.NewObj)
			{
				return HandleConstructorCall(expectedTargetDetails, target.ResolveResult, method, argumentList);
			}

			if (method.Name == "Invoke" && method.DeclaringType.Kind == TypeKind.Delegate && !IsNullConditional(target))
			{
				return new InvocationExpression(target, argumentList.GetArgumentExpressions())
					.WithRR(new CSharpInvocationResolveResult(target.ResolveResult, method,
						argumentList.GetArgumentResolveResults(), isExpandedForm: argumentList.IsExpandedForm, isDelegateInvocation: true));
			}

			if (settings.StringInterpolation && IsInterpolatedStringCreation(method, argumentList))
			{
				var result = HandleStringInterpolation(method, argumentList);
				if (result.Expression != null)
					return result;
			}

			if (IsWrittenAsMemberAccess(method))
			{
				// Only an indexer access has an argument list to leave arguments out of.
				Debug.Assert(argumentList.ArgumentNames == null);
				if (method.AccessorOwner!.SymbolKind != SymbolKind.Indexer)
				{
					argumentList.CheckNoNamedOrOptionalArguments();
				}
				return HandleAccessorCall(expectedTargetDetails, method, target, argumentList);
			}

			if (IsDelegateEqualityComparison(method, argumentList.Arguments))
			{
				argumentList.CheckNoNamedOrOptionalArguments();
				return HandleDelegateEqualityComparison(method, argumentList.Arguments)
					.WithRR(new CSharpInvocationResolveResult(target.ResolveResult, method,
						argumentList.GetArgumentResolveResults(), isExpandedForm: argumentList.IsExpandedForm));
			}

			if (method.IsOperator && method.Name == "op_Implicit" && argumentList.Length == 1)
			{
				argumentList.CheckNoNamedOrOptionalArguments();
				return HandleImplicitConversion(method, argumentList.Arguments[0]);
			}

			if (settings.FirstClassSpanTypes && argumentList.Length == 1
				&& (IsStringToReadOnlySpanCharAsSpan(method) || IsReadOnlySpanCastUp(method)))
			{
				// The C# 14 compiler emits these helpers for implicit span conversions; fold the
				// call back into the conversion. Only safe when the conversion actually applies
				// to this argument type - otherwise keep the call (e.g. AsSpan on a null literal).
				var spanConv = CSharpConversions.Get(expressionBuilder.compilation)
					.ImplicitConversion(argumentList.Arguments[0].Type, method.ReturnType);
				if (spanConv.IsImplicitSpanConversion)
				{
					argumentList.CheckNoNamedOrOptionalArguments();
					return HandleImplicitConversion(method, argumentList.Arguments[0]);
				}
			}

			if (settings.InlineArrays
				&& method is { DeclaringType.FullName: "<PrivateImplementationDetails>", Name: "InlineArrayAsSpan" or "InlineArrayAsReadOnlySpan" }
				&& argumentList.Length == 2)
			{
				argumentList.CheckNoNamedOrOptionalArguments();
				var arrayType = method.TypeArguments[0];
				var arrayLength = arrayType.GetInlineArrayLength();
				var arrayElementType = arrayType.GetInlineArrayElementType();
				var argument = argumentList.Arguments[0];
				var spanLengthExpr = argumentList.Arguments[1];
				var targetType = method.ReturnType;
				var spanType = typeSystem.FindType(KnownTypeCode.SpanOfT);
				if (argument.Expression is DirectionExpression { FieldDirection: FieldDirection.In or FieldDirection.Ref, Expression: var lvalueExpr })
				{
					// `(TargetType)(in arg)` is invalid syntax.
					// Also, `f(in arg)` is invalid when there's an implicit conversion involved.
					argument = argument.UnwrapChild(lvalueExpr);
				}
				if (spanLengthExpr.ResolveResult.ConstantValue is int spanLength && spanLength <= arrayLength)
				{
					if (spanLength < arrayLength)
					{
						argument = new IndexerExpression(argument.Expression, new BinaryOperatorExpression {
							Operator = BinaryOperatorType.Range,
							Right = spanLengthExpr.Expression
						}).WithRR(new ResolveResult(new ParameterizedType(spanType, arrayElementType))).WithoutILInstruction();
						if (targetType.IsKnownType(KnownTypeCode.SpanOfT))
						{
							return argument;
						}
					}
					return new CastExpression(expressionBuilder.ConvertType(targetType), argument.Expression)
					.WithRR(new ConversionResolveResult(targetType, argument.ResolveResult, Conversion.InlineArrayConversion));
				}
			}

			if (settings.LiftNullables && method.Name == "GetValueOrDefault"
				&& method.DeclaringType.IsKnownType(KnownTypeCode.NullableOfT)
				&& method.DeclaringType.TypeArguments[0].IsKnownType(KnownTypeCode.Boolean)
				&& argumentList.Length == 0)
			{
				argumentList.CheckNoNamedOrOptionalArguments();
				return new BinaryOperatorExpression(
					target.Expression,
					BinaryOperatorType.Equality,
					new PrimitiveExpression(true))
					.WithRR(new CSharpInvocationResolveResult(target.ResolveResult, method,
						argumentList.GetArgumentResolveResults(), isExpandedForm: argumentList.IsExpandedForm));
			}

			var transform = GetRequiredTransformationsForCall(expectedTargetDetails, method, ref target,
				ref argumentList, ReferenceTransformation.All, out IParameterizedMember? foundMethod);
			// GetRequiredTransformationsForCall always assigns foundMethod (the resolved overload or 'method').
			Debug.Assert(foundMethod != null);

			// Note: after this, 'method' and 'foundMethod' may differ,
			// but as far as allowed by IsAppropriateCallTarget().

			// Need to update list of parameter names, because foundMethod is different and thus might use different names.
			if (!method.Equals(foundMethod) && argumentList.ParameterNames.Length >= foundMethod!.Parameters.Count)
			{
				for (int i = 0; i < foundMethod.Parameters.Count; i++)
				{
					argumentList.ParameterNames[i] = foundMethod.Parameters[i].Name;
				}
			}

			Expression targetExpr;
			string methodName = method.Name;
			AstNodeCollection<AstType> typeArgumentList;
			if ((transform & ReferenceTransformation.NoNamedArgsForPrettiness) != 0)
			{
				argumentList.AddNamesToPrimitiveValues = false;
			}
			if ((transform & ReferenceTransformation.NoOptionalArgumentAllowed) != 0)
			{
				argumentList.FirstOptionalArgumentIndex = -1;
			}
			if ((transform & ReferenceTransformation.RequireTarget) != 0)
			{
				targetExpr = new MemberReferenceExpression(target.Expression, methodName);
				typeArgumentList = ((MemberReferenceExpression)targetExpr).TypeArguments;

				// HACK : convert this.Dispose() to ((IDisposable)this).Dispose(), if Dispose is an explicitly implemented interface method.
				// settings.AlwaysCastTargetsOfExplicitInterfaceImplementationCalls == true is used in Windows Forms' InitializeComponent methods.
				if (method.IsExplicitInterfaceImplementation && (target.Expression is ThisReferenceExpression || settings.AlwaysCastTargetsOfExplicitInterfaceImplementationCalls))
				{
					var interfaceMember = method.ExplicitlyImplementedInterfaceMembers.First();
					var castExpression = new CastExpression(expressionBuilder.ConvertType(interfaceMember.DeclaringType), target.Expression.Detach());
					methodName = interfaceMember.Name;
					targetExpr = new MemberReferenceExpression(castExpression, methodName);
					typeArgumentList = ((MemberReferenceExpression)targetExpr).TypeArguments;
				}
				if (constrainedTo != null && targetExpr is MemberReferenceExpression { Target: CastExpression cast })
				{
					cast.AddTrailingTrivia(new Comment("cast due to constrained. prefix", CommentType.MultiLine));
				}
			}
			else
			{
				targetExpr = new IdentifierExpression(methodName);
				typeArgumentList = ((IdentifierExpression)targetExpr).TypeArguments;
			}

			if ((transform & ReferenceTransformation.RequireTypeArguments) != 0 && (!settings.AnonymousTypes || !method.TypeArguments.Any(a => a.ContainsAnonymousType())))
				typeArgumentList.AddRange(method.TypeArguments.Select(expressionBuilder.ConvertType));
			return new InvocationExpression(targetExpr, argumentList.GetArgumentExpressions())
				.WithRR(new CSharpInvocationResolveResult(target.ResolveResult, foundMethod,
					argumentList.GetArgumentResolveResultsDirect(), isExpandedForm: argumentList.IsExpandedForm));
		}

		private ExpressionWithResolveResult HandleStringInterpolation(IMethod method, ArgumentList argumentList)
		{
			if (!TryGetStringInterpolationTokens(argumentList, out string? format, out var tokens))
				return default;

			var arguments = argumentList.Arguments;
			var content = new List<InterpolatedStringContent>();

			bool unpackSingleElementArray = !argumentList.IsExpandedForm && argumentList.Length == 2
				&& argumentList.Arguments[1].Expression is ArrayCreateExpression ace
				&& ace.Initializer?.Elements.Count == 1;

			void UnpackSingleElementArray(ref TranslatedExpression argument)
			{
				if (!unpackSingleElementArray)
					return;
				var arrayCreation = (ArrayCreateExpression)argumentList.Arguments[1].Expression;
				var arrayCreationRR = (ArrayCreateResolveResult)argumentList.Arguments[1].ResolveResult;
				var element = arrayCreation.Initializer!.Elements.First().Detach();
				argument = new TranslatedExpression(element, arrayCreationRR.InitializerElements.First());
			}

			if (tokens.Count == 0)
			{
				return default;
			}

			foreach (var (kind, index, alignment, text) in tokens)
			{
				TranslatedExpression argument;
				switch (kind)
				{
					case TokenKind.String:
						content.Add(new InterpolatedStringText(text!));
						break;
					case TokenKind.Argument:
						argument = arguments[index + 1];
						UnpackSingleElementArray(ref argument);
						content.Add(new Interpolation(argument));
						break;
					case TokenKind.ArgumentWithFormat:
						argument = arguments[index + 1];
						UnpackSingleElementArray(ref argument);
						content.Add(new Interpolation(argument, suffix: text));
						break;
					case TokenKind.ArgumentWithAlignment:
						argument = arguments[index + 1];
						UnpackSingleElementArray(ref argument);
						content.Add(new Interpolation(argument, alignment));
						break;
					case TokenKind.ArgumentWithAlignmentAndFormat:
						argument = arguments[index + 1];
						UnpackSingleElementArray(ref argument);
						content.Add(new Interpolation(argument, alignment, text));
						break;
				}
			}
			var formattableStringType = expressionBuilder.compilation.FindType(KnownTypeCode.FormattableString);
			var isrr = new InterpolatedStringResolveResult(expressionBuilder.compilation.FindType(KnownTypeCode.String),
				format, argumentList.GetArgumentResolveResults(1).ToArray());
			var expr = new InterpolatedStringExpression();
			expr.Content.AddRange(content);
			if (method.Name == "Format")
				return expr.WithRR(isrr);
			return new CastExpression(expressionBuilder.ConvertType(formattableStringType),
				expr.WithRR(isrr))
				.WithRR(new ConversionResolveResult(formattableStringType, isrr, Conversion.ImplicitInterpolatedStringConversion));
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
			var unused = new IdentifierExpression("initializedObject").WithRR(target).WithoutILInstruction();
			var args = callArguments.ToList();
			if (method.IsExtensionMethod)
				args.Insert(0, new Nop());

			var argumentList = BuildArgumentList(expectedTargetDetails, target, method,
				firstParamIndex: 0, args, null);
			argumentList.ArgumentNames = null;
			argumentList.AddNamesToPrimitiveValues = false;
			argumentList.UseImplicitlyTypedOut = false;
			// Collection initializer syntax has nowhere to put a target or type arguments, so
			// casting the arguments is the only way left to pin the Add method down.
			var transform = GetRequiredTransformationsForCall(expectedTargetDetails, method, ref unused,
				ref argumentList,
				ReferenceTransformation.CastArguments | ReferenceTransformation.NoOptionalArgumentAllowed
					| ReferenceTransformation.NoNamedArgsForPrettiness,
				out _);
			Debug.Assert((transform & ~(ReferenceTransformation.NoOptionalArgumentAllowed | ReferenceTransformation.NoNamedArgsForPrettiness)) == 0);

			// Calls with only one argument do not need an array initializer expression to wrap them.
			// Any special cases are handled by the caller (i.e., ExpressionBuilder.TranslateObjectAndCollectionInitializer)
			// Note: we intentionally ignore the firstOptionalArgumentIndex in this case.
			int skipCount;
			if (method.IsExtensionMethod)
			{
				if (argumentList.Arguments.Length == 2)
					return argumentList.Arguments[1];
				skipCount = 1;
			}
			else
			{
				if (argumentList.Arguments.Length == 1)
					return argumentList.Arguments[0];
				skipCount = 0;
			}

			if ((transform & ReferenceTransformation.NoOptionalArgumentAllowed) != 0)
				argumentList.FirstOptionalArgumentIndex = -1;

			return new ArrayInitializerExpression(argumentList.GetArgumentExpressions(skipCount))
				.WithRR(new CSharpInvocationResolveResult(target, method, argumentList.GetArgumentResolveResults(skipCount).ToArray(),
					isExtensionMethodInvocation: method.IsExtensionMethod, isExpandedForm: argumentList.IsExpandedForm));
		}

		public ExpressionWithResolveResult BuildDictionaryInitializerExpression(OpCode callOpCode, IMethod method,
			InitializedObjectResolveResult target, IReadOnlyList<ILInstruction> indices, ILInstruction? value = null)
		{
			if (method is null)
				throw new ArgumentNullException(nameof(method));
			ExpectedTargetDetails expectedTargetDetails = new ExpectedTargetDetails { CallOpCode = callOpCode };

			var callArguments = new List<ILInstruction>();
			callArguments.Add(new LdNull());
			callArguments.AddRange(indices);
			callArguments.Add(value ?? new Nop());

			// An index initializer is an assignment whatever the accessor looks like, even for a
			// parameterized property, which has no access syntax of its own.
			var argumentList = BuildArgumentList(expectedTargetDetails, target, method, 1, callArguments, null,
				writtenAsAssignment: true);
			var unused = new IdentifierExpression("initializedObject").WithRR(target).WithoutILInstruction();

			var assignment = HandleAccessorCall(expectedTargetDetails, method, unused, argumentList);

			if (((AssignmentExpression)assignment).Left is IndexerExpression indexer && indexer.Target is not null)
				indexer.Target.Remove();

			if (value != null)
				return assignment;

			return new ExpressionWithResolveResult(((AssignmentExpression)assignment).Left.Detach());
		}

		private static bool IsInterpolatedStringCreation(IMethod method, ArgumentList argumentList)
		{
			return method.IsStatic && (
				(method.DeclaringType.IsKnownType(KnownTypeCode.String) && method.Name == "Format") ||
				(method.Name == "Create" && method.DeclaringType.Name == "FormattableStringFactory" &&
					method.DeclaringType.Namespace == "System.Runtime.CompilerServices")
			)
			&& argumentList.ArgumentNames == null // Argument names are not allowed
			&& (
				argumentList.IsExpandedForm // Must be expanded form
				|| !method.Parameters.Last().IsParams // -or- not a params overload
				|| (argumentList.Length == 2 && argumentList.Arguments[1].Expression is ArrayCreateExpression) // -or- an array literal
			);
		}

		private bool TryGetStringInterpolationTokens(ArgumentList argumentList, [NotNullWhen(true)] out string? format, [NotNullWhen(true)] out List<(TokenKind Kind, int Index, int Alignment, string? Format)>? tokens)
		{
			tokens = null;
			format = null;
			TranslatedExpression[] arguments = argumentList.Arguments;
			if (arguments.Length == 0 || argumentList.ArgumentNames != null || argumentList.ArgumentToParameterMap != null)
				return false;
			if (!(arguments[(int)0].ResolveResult is ConstantResolveResult crr && crr.Type.IsKnownType((KnownTypeCode)KnownTypeCode.String)))
				return false;
			if (!arguments.Skip(1).All(a => !a.Expression.DescendantsAndSelf.OfType<PrimitiveExpression>().Any(p => p.Value is string)))
				return false;
			tokens = new List<(TokenKind Kind, int Index, int Alignment, string? Format)>();
			int i = 0;
			format = (string)crr.ConstantValue!;
			foreach (var (kind, data) in TokenizeFormatString(format))
			{
				int index;
				string[] arg;
				switch (kind)
				{
					case TokenKind.Error:
						return false;
					case TokenKind.String:
						tokens.Add((kind, -1, 0, data));
						break;
					case TokenKind.Argument:
						if (!int.TryParse(data, out index) || index != i)
							return false;
						i++;
						tokens.Add((kind, index, 0, null));
						break;
					case TokenKind.ArgumentWithFormat:
						arg = data!.Split(new[] { ':' }, 2);
						if (arg.Length != 2 || arg[1].Length == 0)
							return false;
						if (!int.TryParse(arg[0], out index) || index != i)
							return false;
						i++;
						tokens.Add((kind, index, 0, arg[1]));
						break;
					case TokenKind.ArgumentWithAlignment:
						arg = data!.Split(new[] { ',' }, 2);
						if (arg.Length != 2 || arg[1].Length == 0)
							return false;
						if (!int.TryParse(arg[0], out index) || index != i)
							return false;
						if (!int.TryParse(arg[1], out int alignment))
							return false;
						i++;
						tokens.Add((kind, index, alignment, null));
						break;
					case TokenKind.ArgumentWithAlignmentAndFormat:
						arg = data!.Split(new[] { ',', ':' }, 3);
						if (arg.Length != 3 || arg[1].Length == 0 || arg[2].Length == 0)
							return false;
						if (!int.TryParse(arg[0], out index) || index != i)
							return false;
						if (!int.TryParse(arg[1], out alignment))
							return false;
						i++;
						tokens.Add((kind, index, alignment, arg[2]));
						break;
					default:
						return false;
				}
			}
			return i == arguments.Length - 1;
		}

		private enum TokenKind
		{
			Error,
			String,
			Argument,
			ArgumentWithFormat,
			ArgumentWithAlignment,
			ArgumentWithAlignmentAndFormat,
		}

		private IEnumerable<(TokenKind, string?)> TokenizeFormatString(string value)
		{
			int pos = -1;

			int Peek(int steps = 1)
			{
				if (pos + steps < value.Length)
					return value[pos + steps];
				return -1;
			}

			int Next()
			{
				int val = Peek();
				pos++;
				return val;
			}

			int next;
			TokenKind kind = TokenKind.String;
			StringBuilder sb = new StringBuilder();

			while ((next = Next()) > -1)
			{
				switch ((char)next)
				{
					case '{':
						if (Peek() == '{')
						{
							kind = TokenKind.String;
							sb.Append("{{");
							Next();
						}
						else
						{
							if (sb.Length > 0)
							{
								yield return (kind, sb.ToString());
							}
							kind = TokenKind.Argument;
							sb.Clear();
						}
						break;
					case '}':
						if (kind != TokenKind.String)
						{
							yield return (kind, sb.ToString());
							sb.Clear();
							kind = TokenKind.String;
						}
						else if (Peek() == '}')
						{
							sb.Append("}}");
							Next();
						}
						else
						{
							yield return (TokenKind.Error, null);
						}
						break;
					case ':':
						if (kind == TokenKind.Argument)
						{
							kind = TokenKind.ArgumentWithFormat;
						}
						else if (kind == TokenKind.ArgumentWithAlignment)
						{
							kind = TokenKind.ArgumentWithAlignmentAndFormat;
						}
						sb.Append(':');
						break;
					case ',':
						if (kind == TokenKind.Argument)
						{
							kind = TokenKind.ArgumentWithAlignment;
						}
						sb.Append(',');
						break;
					default:
						sb.Append((char)next);
						break;
				}
			}
			if (sb.Length > 0)
			{
				if (kind == TokenKind.String)
					yield return (kind, sb.ToString());
				else
					yield return (TokenKind.Error, null);
			}
		}

		private ArgumentList BuildArgumentList(ExpectedTargetDetails expectedTargetDetails, ResolveResult? target, IMethod method,
			int firstParamIndex, IReadOnlyList<ILInstruction> callArguments, IReadOnlyList<int>? argumentToParameterMap,
			bool writtenAsAssignment = false)
		{
			ArgumentList list = new ArgumentList();

			// Translate arguments to the expected parameter types
			var arguments = new List<TranslatedExpression>(method.Parameters.Count);
			string[]? argumentNames = null;
			Debug.Assert(callArguments.Count == firstParamIndex + method.Parameters.Count);
			var expectedParameters = new List<IParameter>(method.Parameters.Count); // parameters, but in argument order
			bool isExpandedForm = false;
			BitSet isPrimitiveValue = new BitSet(method.Parameters.Count);

			// Optional arguments:
			// This value has the following values:
			// -2 - there are no optional arguments
			// -1 - optional arguments are forbidden
			// >= 0 - the index of the first argument that can be removed, because it is optional
			// and is the default value of the parameter. 
			int firstOptionalArgumentIndex = expressionBuilder.settings.OptionalArguments ? -2 : -1;
			// Only an accessor written as an access or an index initializer takes its assigned
			// value out of the argument list; one written as a call passes it like any other.
			bool isSetter = method.ReturnType.IsKnownType(KnownTypeCode.Void)
				&& (writtenAsAssignment || IsWrittenAsMemberAccess(method));
			for (int i = firstParamIndex; i < callArguments.Count; i++)
			{
				IParameter parameter;
				if (argumentToParameterMap != null)
				{
					if (argumentNames == null && argumentToParameterMap[i] != i - firstParamIndex)
					{
						// Starting at the first argument that is out-of-place,
						// assign names to that argument and all following arguments:
						argumentNames = new string[method.Parameters.Count];
					}
					parameter = method.Parameters[argumentToParameterMap[i]];
					if (argumentNames != null && AssignVariableNames.IsValidName(parameter.Name))
					{
						argumentNames[arguments.Count] = parameter.Name;
					}
				}
				else
				{
					parameter = method.Parameters[i - firstParamIndex];
				}
				var arg = expressionBuilder.Translate(callArguments[i], parameter.Type);
				if (IsPrimitiveValueThatShouldBeNamedArgument(arg, method, parameter))
				{
					isPrimitiveValue.Set(arguments.Count);
				}
				// The assigned value of a setter is not part of the argument list, so it does not
				// end the run of optional arguments either.
				if (!(isSetter && i + 1 == callArguments.Count))
				{
					if (IsOptionalArgument(parameter, arg))
					{
						if (firstOptionalArgumentIndex == -2)
							firstOptionalArgumentIndex = i - firstParamIndex;
					}
					else if (firstOptionalArgumentIndex != -1)
					{
						firstOptionalArgumentIndex = -2;
					}
				}
				// An assignment has no argument list to spread a parameter array over, and C#
				// cannot declare a property whose value is one.
				if (expressionBuilder.settings.ExpandParamsArguments && parameter.IsParams && !isSetter
					&& i + 1 == callArguments.Count && argumentToParameterMap == null)
				{
					// Parameter is marked params
					// If the argument is an array creation, inline all elements into the call and add missing default values.
					// Otherwise handle it normally.
					if (TransformParamsArgument(expectedTargetDetails, target, method, parameter,
						arg, ref expectedParameters, ref arguments))
					{
						Debug.Assert(argumentNames == null);
						firstOptionalArgumentIndex = -1;
						isExpandedForm = true;
						continue;
					}
				}

				IType parameterType;
				if (parameter.Type.Kind == TypeKind.Dynamic)
				{
					parameterType = expressionBuilder.compilation.FindType(KnownTypeCode.Object);
				}
				else
				{
					parameterType = parameter.Type;
				}

				arg = arg.ConvertTo(parameterType, expressionBuilder, allowImplicitConversion: arg.Type.Kind != TypeKind.Dynamic);
				if (method.IsOperator)
				{
					// Operator calls do not survive as calls: ReplaceMethodCallsWithOperators turns
					// them into operator or cast syntax, where the operand determines which operator
					// is resolved, so it must keep its explicit type. Unlike the null literal, which
					// still narrows the candidate set, the default literal converts to every type:
					// C# rejects it as the operand of any binary operator except == and != (CS8310).
					arg = arg.RestoreDefaultLiteralType(expressionBuilder);
				}

				if (parameter.ReferenceKind != ReferenceKind.None)
				{
					arg = ExpressionBuilder.ChangeDirectionExpressionTo(arg, parameter.ReferenceKind, callArguments[i] is AddressOf);
					// An rvalue bound to an 'in' parameter loses its DirectionExpression above and
					// is an ordinary value expression: give a span conversion the same chance to
					// become implicit that by-value arguments get from the ConvertTo call above.
					if (arg.Expression is not DirectionExpression
						&& parameter.Type.SkipModifiers() is ByReferenceType brt
						&& arg.ResolveResult is ConversionResolveResult { Conversion.IsImplicitSpanConversion: true })
					{
						arg = arg.ConvertTo(brt.ElementType, expressionBuilder, allowImplicitConversion: true);
					}
				}

				arguments.Add(arg);
				expectedParameters.Add(parameter);
			}

			list.ExpectedParameters = expectedParameters.ToArray();
			list.Arguments = arguments.ToArray();
			list.ParameterNames = expectedParameters.SelectArray(p => p.Name);
			list.ArgumentNames = argumentNames;
			list.ArgumentToParameterMap = argumentToParameterMap;
			list.IsExpandedForm = isExpandedForm;
			list.IsPrimitiveValue = isPrimitiveValue;
			list.FirstOptionalArgumentIndex = firstOptionalArgumentIndex;
			list.UseImplicitlyTypedOut = true;
			list.AddNamesToPrimitiveValues = expressionBuilder.settings.NamedArguments && expressionBuilder.settings.NonTrailingNamedArguments;
			return list;
		}

		private bool IsPrimitiveValueThatShouldBeNamedArgument(TranslatedExpression arg, IMethod method, IParameter p)
		{
			if (!arg.ResolveResult.IsCompileTimeConstant || method.DeclaringType.IsKnownType(KnownTypeCode.NullableOfT))
				return false;
			return p.Type.IsKnownType(KnownTypeCode.Boolean);
		}

		private bool TransformParamsArgument(ExpectedTargetDetails expectedTargetDetails, ResolveResult? targetResolveResult,
			IMethod method, IParameter parameter, TranslatedExpression paramsArgument, ref List<IParameter> expectedParameters,
			ref List<TranslatedExpression> arguments)
		{
			var expressionBuilder = this.expressionBuilder;
			if (ExtractArguments(out var expandedParameters, out var expandedArguments))
			{
				expandedParameters.InsertRange(0, expectedParameters);
				expandedArguments.InsertRange(0, arguments);
				if (Disambiguator.IsUnambiguousCall(expressionBuilder, expectedTargetDetails, method, targetResolveResult, Empty<IType>.Array,
					expandedArguments.SelectArray(a => a.ResolveResult), argumentNames: null,
					firstOptionalArgumentIndex: -1, out _,
					out var bestCandidateIsExpandedForm) == OverloadResolutionErrors.None && bestCandidateIsExpandedForm)
				{
					expectedParameters = expandedParameters;
					arguments = expandedArguments.SelectList(a => new TranslatedExpression(a.Expression.Detach()));
					return true;
				}
			}
			return false;

			bool ExtractArguments([NotNullWhen(true)] out List<IParameter>? parameters, [NotNullWhen(true)] out List<TranslatedExpression>? arguments)
			{
				parameters = null;
				arguments = null;
				// Every expanded argument binds to the element type of the params collection, so
				// that is the type the parameters standing in for them carry. The argument itself
				// may be an array of a more derived element type, because the collection is
				// covariant in it. Unpack it the same way overload resolution does, and give up
				// where overload resolution would give up on the expanded form as well.
				IType paramsElementType;
				if (parameter.Type is ArrayType { Dimensions: 1 } paramsArray)
					paramsElementType = paramsArray.ElementType;
				else if (parameter.Type.IsKnownType(KnownTypeCode.ReadOnlySpanOfT)
					|| parameter.Type.IsKnownType(KnownTypeCode.SpanOfT)
					|| parameter.Type.IsArrayInterfaceType())
					paramsElementType = parameter.Type.TypeArguments[0];
				else
					return false;
				switch (paramsArgument.ResolveResult)
				{
					case CSharpInvocationResolveResult { Member: IMethod method, Arguments: var args }:
						// match System.Array.Empty<T>()
						if (args is [] && method is { IsStatic: true, FullName: "System.Array.Empty", TypeArguments: [_] })
						{
							arguments = new();
							parameters = new();
							return true;
						}
						// match System.ReadOnlySpan<T>..ctor(ref readonly T)
						if (paramsArgument.Expression is ObjectCreateExpression oce
							&& method is {
								IsConstructor: true,
								Parameters: [{ ReferenceKind: ReferenceKind.RefReadOnly, Type: ByReferenceType { ElementType: var paramType } }],
								DeclaringType: { TypeArguments: [var type2] } declaringType
							}
							&& declaringType.IsKnownType(KnownTypeCode.ReadOnlySpanOfT)
							&& paramType.Equals(type2))
						{
							arguments = new() { new TranslatedExpression(oce.Arguments.Single()) };
							parameters = new() { new DefaultParameter(paramsElementType, string.Empty) };
							return true;
						}
						return false;
					case ArrayCreateResolveResult { SizeArguments: [{ ConstantValue: int arrayLength }] }:
						arguments = new(((ArrayCreateExpression)paramsArgument.Expression).Initializer?.Elements.Select(e => new TranslatedExpression(e)) ?? []);
						parameters = new List<IParameter>(arrayLength);
						for (int i = 0; i < arrayLength; i++)
						{
							parameters.Add(new DefaultParameter(paramsElementType, string.Empty));
							if (arguments.Count <= i)
								arguments.Add(new TranslatedExpression(expressionBuilder.GetDefaultValueExpression(paramsElementType).WithoutILInstruction()));
						}
						return true;
					case ConversionResolveResult { Conversion.IsImplicitSpanConversion: true, Input: ArrayCreateResolveResult { SizeArguments: [{ ConstantValue: int arrayLength }] } }:
						var expr = paramsArgument.Expression is CastExpression cast ? cast.Expression : paramsArgument.Expression;
						arguments = new(((ArrayCreateExpression)expr).Initializer?.Elements.Select(e => new TranslatedExpression(e)) ?? []);
						parameters = new List<IParameter>(arrayLength);
						for (int i = 0; i < arrayLength; i++)
						{
							parameters.Add(new DefaultParameter(paramsElementType, string.Empty));
							if (arguments.Count <= i)
								arguments.Add(new TranslatedExpression(expressionBuilder.GetDefaultValueExpression(paramsElementType).WithoutILInstruction()));
						}
						return true;
					default:
						return false;
				}
			}
		}

		bool IsOptionalArgument(IParameter parameter, TranslatedExpression arg)
		{
			if (!parameter.IsOptional)
				return false;

			if (!arg.ResolveResult.IsCompileTimeConstant && arg.ResolveResult is not ConversionResolveResult { Conversion.IsNullLiteralConversion: true })
				return false;
			if (parameter.GetAttributes().Any(a => a.AttributeType.IsKnownType(KnownAttribute.CallerMemberName)
				|| a.AttributeType.IsKnownType(KnownAttribute.CallerFilePath)
				|| a.AttributeType.IsKnownType(KnownAttribute.CallerLineNumber)))
				return false;
			return object.Equals(parameter.GetConstantValue(), arg.ResolveResult.ConstantValue);
		}

		private ReferenceTransformation GetRequiredTransformationsForCall(ExpectedTargetDetails expectedTargetDetails, IMethod method,
			ref TranslatedExpression target, ref ArgumentList argumentList, ReferenceTransformation allowedTransforms, out IParameterizedMember? foundMethod)
		{
			ReferenceTransformation transform = ReferenceTransformation.None;

			// initialize requireTarget flag
			bool requireTarget;
			if ((allowedTransforms & ReferenceTransformation.RequireTarget) != 0)
			{
				if (method.IsLocalFunction)
				{
					// A local function is never reached through a target.
					requireTarget = settings.AlwaysQualifyMemberReferences
						|| expressionBuilder.HidesVariableWithName(method.Name);
				}
				else if (method.Name == ".ctor" || method.Name == ".cctor")
				{
					// Always use target for base/this-ctor-call, the constructor initializer pattern depends on this
					requireTarget = true;
				}
				else
				{
					requireTarget = expressionBuilder.RequiresQualifier(method, target,
						nonVirtualDispatch: expectedTargetDetails.CallOpCode != OpCode.CallVirt);
				}
			}
			else
			{
				// HACK: this is a special case for collection initializer calls, they do not allow a target to be
				// emitted, but we still need it for overload resolution.
				requireTarget = true;
			}

			var disambiguator = Disambiguator.ForCall(expressionBuilder, method, argumentList, target,
				requireTarget, expectedTargetDetails, allowedTransforms);
			// Where even the most explicit spelling stays ambiguous, the call is written as it
			// stands and annotated with the method it was meant to reach.
			foundMethod = disambiguator.Resolved ? (IParameterizedMember?)disambiguator.FoundMember : method;
			target = disambiguator.Target;
			argumentList = disambiguator.Arguments;
			requireTarget = disambiguator.RequireTarget;
			bool requireTypeArguments = disambiguator.RequireTypeArguments;

			if ((allowedTransforms & ReferenceTransformation.RequireTarget) != 0 && requireTarget)
				transform |= ReferenceTransformation.RequireTarget;
			if ((allowedTransforms & ReferenceTransformation.RequireTypeArguments) != 0 && requireTypeArguments)
				transform |= ReferenceTransformation.RequireTypeArguments;
			if (argumentList.FirstOptionalArgumentIndex < 0)
				transform |= ReferenceTransformation.NoOptionalArgumentAllowed;
			if (!argumentList.AddNamesToPrimitiveValues)
				transform |= ReferenceTransformation.NoNamedArgsForPrettiness;
			return transform;
		}

		static bool IsNullConditional(Expression expr)
		{
			return expr is UnaryOperatorExpression uoe && uoe.Operator == UnaryOperatorType.NullConditional;
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
			// The compiler emits an implicit span conversion as a call to one of the span types'
			// own members, so such a call is the conversion and folding it back is exact. Any
			// other method reaching this point is a user-defined conversion operator, which only
			// the user-defined conversion resolving to that very operator may be folded into.
			bool directlyConvertible = conv.IsValid
				&& (conv.IsUserDefined
					? conv.Method.Equals(method, NormalizeTypeVisitor.TypeErasure)
					: conv.IsImplicitSpanConversion && IsEquivalentToSpanConversion(method));
			if (!directlyConvertible)
			{
				// implicit conversion to targetType isn't directly possible, so first insert a cast to the argument type
				argument = argument.ConvertTo(method.Parameters[0].Type, expressionBuilder);
				conv = conversions.ImplicitConversion(argument.Type, targetType);
			}
			if (argument.Expression is DirectionExpression { FieldDirection: FieldDirection.In, Expression: var lvalueExpr })
			{
				// `(TargetType)(in arg)` is invalid syntax.
				// Also, `f(in arg)` is invalid when there's an implicit conversion involved.
				argument = argument.UnwrapChild(lvalueExpr);
			}
			return new CastExpression(expressionBuilder.ConvertType(targetType), argument.Expression)
				.WithRR(new ConversionResolveResult(targetType, argument.ResolveResult, conv));
		}

		/// <summary>Whether the accessor is written as a property or indexer access. One with more
		/// parameters than that syntax has room for is written as a call, assigned value and all.</summary>
		static bool IsWrittenAsMemberAccess(IMethod method)
		{
			if (!method.IsAccessor)
				return false;
			if (method.AccessorOwner!.SymbolKind == SymbolKind.Indexer)
				return true;
			return method.Parameters.Count == (method.ReturnType.IsKnownType(KnownTypeCode.Void) ? 1 : 0);
		}

		ExpressionWithResolveResult HandleAccessorCall(ExpectedTargetDetails expectedTargetDetails, IMethod method,
			TranslatedExpression target, ArgumentList argumentList)
		{
			// An indexer has no name of its own to write, so it can never drop its target.
			bool requireTarget = method.AccessorOwner!.SymbolKind == SymbolKind.Indexer
				|| expressionBuilder.RequiresQualifier(method.AccessorOwner, target,
					nonVirtualDispatch: expectedTargetDetails.CallOpCode != OpCode.CallVirt);
			bool isSetter = method.ReturnType.IsKnownType(KnownTypeCode.Void);
			// An access spells its index out anyway, so readability names have no place in one.
			argumentList.AddNamesToPrimitiveValues = false;

			TranslatedExpression value = default(TranslatedExpression);
			if (isSetter)
			{
				// The assigned value is not part of the reference being spelled out, so it is taken
				// out before anything counts, names or casts the arguments.
				value = argumentList.Arguments[argumentList.Length - 1];
				argumentList.Arguments = argumentList.Arguments.Take(argumentList.Length - 1).ToArray();
				argumentList.ArgumentToParameterMap = argumentList.ArgumentToParameterMap
					?.Take(argumentList.ArgumentToParameterMap.Count - 1).ToArray();
			}

			// Dropping every argument would turn an indexer access into a property access.
			if (argumentList.FirstOptionalArgumentIndex == 0
				&& method.AccessorOwner.SymbolKind == SymbolKind.Indexer)
			{
				argumentList.FirstOptionalArgumentIndex = 1;
			}
			var disambiguator = Disambiguator.ForAccessor(expressionBuilder, method, argumentList, target,
				requireTarget, expectedTargetDetails);
			IMember? foundMember = disambiguator.Resolved ? disambiguator.FoundMember : method.AccessorOwner!;
			requireTarget = disambiguator.RequireTarget;
			target = disambiguator.Target;
			argumentList = disambiguator.Arguments;

			var arguments = argumentList.GetArgumentExpressions().ToList();
			var rr = new MemberResolveResult(target.ResolveResult, foundMember);

			if (isSetter)
			{
				TranslatedExpression expr;

				if (arguments.Count != 0)
				{
					expr = new IndexerExpression(target.ResolveResult is InitializedObjectResolveResult ? null : target.Expression, arguments)
						.WithoutILInstruction().WithRR(rr);
				}
				else if (requireTarget)
				{
					expr = new MemberReferenceExpression(target.Expression, method.AccessorOwner!.Name)
						.WithoutILInstruction().WithRR(rr);
				}
				else
				{
					expr = new IdentifierExpression(method.AccessorOwner!.Name)
						.WithoutILInstruction().WithRR(rr);
				}

				var op = AssignmentOperatorType.Assign;
				if (method.AccessorOwner is IEvent parentEvent)
				{
					if (method.Equals(parentEvent.AddAccessor))
					{
						op = AssignmentOperatorType.Add;
					}
					if (method.Equals(parentEvent.RemoveAccessor))
					{
						op = AssignmentOperatorType.Subtract;
					}
				}
				return new AssignmentExpression(expr, op, value.Expression!).WithRR(new TypeResolveResult(method.AccessorOwner!.ReturnType));
			}
			else
			{
				if (arguments.Count != 0)
				{
					return new IndexerExpression(target.Expression, arguments)
						.WithoutILInstruction().WithRR(rr);
				}
				else if (requireTarget)
				{
					return new MemberReferenceExpression(target.Expression, method.AccessorOwner!.Name)
						.WithoutILInstruction().WithRR(rr);
				}
				else
				{
					return new IdentifierExpression(method.AccessorOwner!.Name)
						.WithoutILInstruction().WithRR(rr);
				}
			}
		}

		ExpressionWithResolveResult HandleConstructorCall(ExpectedTargetDetails expectedTargetDetails, ResolveResult? target, IMethod method, ArgumentList argumentList)
		{
			if (settings.AnonymousTypes && method.DeclaringType.IsAnonymousType())
			{
				Debug.Assert(argumentList.ArgumentToParameterMap == null && argumentList.ArgumentNames == null && argumentList.FirstOptionalArgumentIndex < 0);
				var atce = new AnonymousTypeCreateExpression();
				if (argumentList.CanInferAnonymousTypePropertyNamesFromArguments())
				{
					atce.Initializers.AddRange(argumentList.GetArgumentExpressions());
				}
				else
				{
					for (int i = 0; i < argumentList.Length; i++)
					{
						atce.Initializers.Add(
							new NamedExpression {
								Name = argumentList.ExpectedParameters[i].Name,
								Expression = argumentList.Arguments[i].ConvertTo(argumentList.ExpectedParameters[i].Type, expressionBuilder)
							});
					}
				}
				return atce.WithRR(new CSharpInvocationResolveResult(
					target, method, argumentList.GetArgumentResolveResults(),
					isExpandedForm: argumentList.IsExpandedForm, argumentToParameterMap: argumentList.ArgumentToParameterMap
				));
			}
			else
			{
				var disambiguator = Disambiguator.ForConstructorCall(expressionBuilder, method, argumentList,
					expectedTargetDetails);
				argumentList = disambiguator.Arguments;
				IType? returnTypeOverride = null;
				if (typeSystem.MainModule.TypeSystemOptions.HasFlag(TypeSystemOptions.NativeIntegersWithoutAttribute))
				{
					// For DeclaringType, we don't use nint/nuint (so that DeclaringType.GetConstructors etc. works),
					// but in NativeIntegersWithoutAttribute mode we must use nint/nuint for expression types,
					// so that the appropriate set of conversions is used for further overload resolution.
					if (method.DeclaringType.IsKnownType(KnownTypeCode.IntPtr))
						returnTypeOverride = SpecialType.NInt;
					else if (method.DeclaringType.IsKnownType(KnownTypeCode.UIntPtr))
						returnTypeOverride = SpecialType.NUInt;
				}
				return new ObjectCreateExpression(
					expressionBuilder.ConvertType(method.DeclaringType),
					argumentList.GetArgumentExpressions()
				).WithRR(new CSharpInvocationResolveResult(
					target, method, argumentList.GetArgumentResolveResults().ToArray(),
					isExpandedForm: argumentList.IsExpandedForm,
					argumentToParameterMap: argumentList.ArgumentToParameterMap,
					returnTypeOverride: returnTypeOverride
				));
			}
		}

		TranslatedExpression HandleDelegateConstruction(CallInstruction inst)
		{
			ILInstruction thisArg = inst.Arguments[0];
			ILInstruction func = inst.Arguments[1];
			IMethod method;
			ExpectedTargetDetails expectedTargetDetails = default;
			switch (func.OpCode)
			{
				case OpCode.LdFtn:
					method = ((LdFtn)func).Method;
					expectedTargetDetails.CallOpCode = OpCode.Call;
					break;
				case OpCode.LdVirtFtn:
					method = ((LdVirtFtn)func).Method;
					expectedTargetDetails.CallOpCode = OpCode.CallVirt;
					break;
				default:
					throw new ArgumentException($"Unknown instruction type: {func.OpCode}");
			}
			if (CanUseDelegateConstruction(method, thisArg, inst.Method.DeclaringType.GetDelegateInvokeMethod()))
			{
				return HandleDelegateConstruction(inst.Method.DeclaringType, method, expectedTargetDetails, thisArg, inst);
			}
			else
			{
				var argumentList = BuildArgumentList(expectedTargetDetails, null, inst.Method,
					0, inst.Arguments, null);
				return HandleConstructorCall(new ExpectedTargetDetails { CallOpCode = OpCode.NewObj }, null, inst.Method, argumentList).WithILInstruction(inst);
			}
		}

		private bool CanUseDelegateConstruction(IMethod targetMethod, ILInstruction thisArg, IMethod invokeMethod)
		{
			// Accessors cannot be directly referenced as method group in C#
			// see https://github.com/icsharpcode/ILSpy/issues/1741#issuecomment-540179101
			if (targetMethod.IsAccessor)
				return false;
			if (targetMethod.IsStatic)
			{
				// If the invoke method is known, we can compare the parameter counts to figure out whether the
				// delegate is static or binds the first argument
				if (invokeMethod != null)
				{
					if (invokeMethod.Parameters.Count == targetMethod.Parameters.Count)
					{
						return thisArg.MatchLdNull();
					}
					else if (targetMethod.IsExtensionMethod && invokeMethod.Parameters.Count == targetMethod.Parameters.Count - 1)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					// delegate type unknown:
					return thisArg.MatchLdNull() || targetMethod.IsExtensionMethod;
				}
			}
			else
			{
				// targetMethod is instance method
				if (invokeMethod != null && invokeMethod.Parameters.Count != targetMethod.Parameters.Count)
					return false;
				return true;
			}
		}

		internal TranslatedExpression Build(LdVirtDelegate inst)
		{
			return HandleDelegateConstruction(inst.Type, inst.Method, new ExpectedTargetDetails { CallOpCode = OpCode.CallVirt }, inst.Argument, inst);
		}

		internal ExpressionWithResolveResult BuildMethodReference(IMethod method, bool isVirtual)
		{
			var expr = BuildDelegateReference(method, invokeMethod: null, new ExpectedTargetDetails { CallOpCode = isVirtual ? OpCode.CallVirt : OpCode.Call }, thisArg: null);
			expr.Expression.RemoveAnnotations<ResolveResult>();
			return expr.Expression.WithRR(new MemberResolveResult(null, method));
		}

		ExpressionWithResolveResult BuildDelegateReference(IMethod method, IMethod? invokeMethod, ExpectedTargetDetails expectedTargetDetails, ILInstruction? thisArg)
		{
			ExpressionBuilder expressionBuilder = this.expressionBuilder;
			ExpressionWithResolveResult targetExpression;
			(TranslatedExpression target, bool addTypeArguments, string methodName, ResolveResult result) = DisambiguateDelegateReference(method, invokeMethod, expectedTargetDetails, thisArg);
			if (target.Expression != null)
			{
				var mre = new MemberReferenceExpression(target, methodName);
				if (addTypeArguments)
				{
					mre.TypeArguments.AddRange(method.TypeArguments.Select(expressionBuilder.ConvertType));
				}
				targetExpression = mre.WithRR(result);
			}
			else
			{
				var ide = new IdentifierExpression(methodName);
				if (addTypeArguments)
				{
					ide.TypeArguments.AddRange(method.TypeArguments.Select(expressionBuilder.ConvertType));
				}
				targetExpression = ide.WithRR(result);
			}
			return targetExpression;

		}

		(TranslatedExpression target, bool addTypeArguments, string methodName, ResolveResult result) DisambiguateDelegateReference(IMethod method, IMethod? invokeMethod, ExpectedTargetDetails expectedTargetDetails, ILInstruction? thisArg)
		{
			if (method.IsLocalFunction)
			{
				ILFunction? localFunction = expressionBuilder.ResolveLocalFunction(method);
				Debug.Assert(localFunction != null);
				return (default, addTypeArguments: true, localFunction.Name!, ToMethodGroup(method, localFunction));
			}
			if (method.IsExtensionMethod && method.Parameters.Count - 1 == invokeMethod?.Parameters.Count)
			{
				IType targetType = method.Parameters[0].Type;
				if (targetType.Kind == TypeKind.ByReference && thisArg is Box thisArgBox)
				{
					targetType = ((ByReferenceType)targetType).ElementType;
					thisArg = thisArgBox.Argument;
				}
				TranslatedExpression target = expressionBuilder.Translate(thisArg!, targetType);
				// A null literal carries no type at all, so its cast cannot wait its turn.
				var disambiguator = Disambiguator.ForMethodReference(expressionBuilder, method, targetType,
					target, requireTarget: true, expectedTargetDetails, isExtensionMethodReference: true,
					castTargetUpFront: thisArg!.MatchLdNull());
				return (disambiguator.Target, disambiguator.RequireTypeArguments, method.Name, disambiguator.Result!);
			}
			else
			{
				// Prepare call target
				IType targetType = method.DeclaringType;
				if (targetType.IsReferenceType == false && thisArg is Box thisArgBox)
				{
					// Normal struct instance method calls (which TranslateTarget is meant for) expect a 'ref T',
					// but delegate construction uses a 'box T'.
					if (thisArgBox.Argument is LdObj ldobj)
					{
						thisArg = ldobj.Target;
					}
					else
					{
						thisArg = new AddressOf(thisArgBox.Argument, thisArgBox.Type);
					}
				}
				TranslatedExpression target = expressionBuilder.TranslateTarget(thisArg,
					nonVirtualInvocation: expectedTargetDetails.CallOpCode == OpCode.Call,
					memberStatic: method.IsStatic,
					memberDeclaringType: method.DeclaringType);
				// check if target is required
				bool requireTarget = expressionBuilder.RequiresQualifier(method, target,
					nonVirtualDispatch: expectedTargetDetails.CallOpCode != OpCode.CallVirt);
				var disambiguator = Disambiguator.ForMethodReference(expressionBuilder, method, targetType,
					target, requireTarget, expectedTargetDetails, isExtensionMethodReference: false);
				ResolveResult? result = disambiguator.Result;
				if (result is MethodGroupResolveResult mgrr)
				{
					result = mgrr.WithChosenMethod(method);
				}
				// BuildDelegateReference tells a qualified reference from an unqualified one by
				// whether it got a target expression at all.
				return (disambiguator.RequireTarget ? disambiguator.Target : default,
					disambiguator.RequireTypeArguments, method.Name, result!);
			}
		}

		TranslatedExpression HandleDelegateConstruction(IType delegateType, IMethod method, ExpectedTargetDetails expectedTargetDetails, ILInstruction thisArg, ILInstruction inst)
		{
			var invokeMethod = delegateType.GetDelegateInvokeMethod();
			var targetExpression = BuildDelegateReference(method, invokeMethod, expectedTargetDetails, thisArg);
			var oce = new ObjectCreateExpression(expressionBuilder.ConvertType(delegateType), targetExpression)
				.WithILInstruction(inst)
				.WithRR(new ConversionResolveResult(
					delegateType,
					targetExpression.ResolveResult,
					Conversion.MethodGroupConversion(method, expectedTargetDetails.CallOpCode == OpCode.CallVirt, false)));
			return oce;
		}

		static MethodGroupResolveResult ToMethodGroup(IMethod method, ILFunction localFunction)
		{
			return new MethodGroupResolveResult(
				null,
				localFunction.Name,
				new[] {
					new MethodListWithDeclaringType(
						method.DeclaringType,
						new IParameterizedMember[] { method }
					)
				}, method.TypeArguments
			);
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
			foreach (StLoc stloc in block.Instructions)
			{
				Debug.Assert(stloc.Variable.LoadInstructions.Single().Parent == call);
				arguments[pos] = stloc.Value;
				argumentToParameterMap[pos] = stloc.Variable.LoadInstructions.Single().ChildIndex - firstParamIndex;
				pos++;
			}
			// Remaining argument:
			foreach (var arg in call.Arguments)
			{
				if (arg.MatchLdLoc(out var v) && v.Kind == VariableKind.NamedArgument)
				{
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

		private bool HandleRangeConstruction(out ExpressionWithResolveResult result, OpCode callOpCode, IMethod method, TranslatedExpression target, ArgumentList argumentList)
		{
			result = default;
			if (argumentList.ArgumentNames != null)
			{
				return false; // range syntax doesn't support named arguments
			}
			if (method.DeclaringType.IsKnownType(KnownTypeCode.Range))
			{
				if (callOpCode == OpCode.NewObj && argumentList.Length == 2)
				{
					result = new BinaryOperatorExpression(argumentList.Arguments[0], BinaryOperatorType.Range, argumentList.Arguments[1])
						.WithRR(new MemberResolveResult(null, method));
					return true;
				}
				else if (callOpCode == OpCode.Call && method.Name == "get_All" && argumentList.Length == 0)
				{
					result = new BinaryOperatorExpression(null, BinaryOperatorType.Range, null)
						.WithRR(new MemberResolveResult(null, method.AccessorOwner ?? method));
					return true;
				}
				else if (callOpCode == OpCode.Call && method.Name == "StartAt" && argumentList.Length == 1)
				{
					result = new BinaryOperatorExpression(argumentList.Arguments[0], BinaryOperatorType.Range, null)
						.WithRR(new MemberResolveResult(null, method));
					return true;
				}
				else if (callOpCode == OpCode.Call && method.Name == "EndAt" && argumentList.Length == 1)
				{
					result = new BinaryOperatorExpression(null, BinaryOperatorType.Range, argumentList.Arguments[0])
						.WithRR(new MemberResolveResult(null, method));
					return true;
				}
			}
			else if (callOpCode == OpCode.NewObj && method.DeclaringType.IsKnownType(KnownTypeCode.Index))
			{
				if (argumentList.Length != 2)
					return false;
				if (!(argumentList.Arguments[1].Expression is PrimitiveExpression pe && pe.Value is true))
					return false;
				result = new UnaryOperatorExpression(UnaryOperatorType.IndexFromEnd, argumentList.Arguments[0])
					.WithRR(new MemberResolveResult(null, method));
				return true;
			}
			else if (method is SyntheticRangeIndexAccessor rangeIndexAccessor && rangeIndexAccessor.IsSlicing)
			{
				// For slicing the method is called Slice()/Substring(), but we still need to output indexer notation.
				// So special-case range-based slicing here.
				result = new IndexerExpression(target, argumentList.Arguments.Select(a => a.Expression))
					.WithRR(new MemberResolveResult(target.ResolveResult, method));
				return true;
			}
			return false;
		}
	}
}
