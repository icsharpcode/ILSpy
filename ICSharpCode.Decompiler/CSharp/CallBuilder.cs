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
using System.Text;
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

		struct ArgumentList
		{
			public TranslatedExpression[] Arguments;
			public IParameter[] ExpectedParameters;
			public string[] ParameterNames;
			public string[] ArgumentNames;
			public int FirstOptionalArgumentIndex;
			public BitSet IsPrimitiveValue;
			public IReadOnlyList<int> ArgumentToParameterMap;

			public bool AddNamesToPrimitiveValues;
			public bool IsExpandedForm;
			public int Length => Arguments.Length;

			private int GetActualArgumentCount()
			{
				if (FirstOptionalArgumentIndex < 0)
					return Arguments.Length;
				return FirstOptionalArgumentIndex;
			}

			public IEnumerable<ResolveResult> GetArgumentResolveResults(int skipCount = 0)
			{
				return Arguments.Skip(skipCount).Take(GetActualArgumentCount()).Select(a => a.ResolveResult);
			}

			public IEnumerable<Expression> GetArgumentExpressions(int skipCount = 0)
			{
				if (AddNamesToPrimitiveValues && IsPrimitiveValue.Any() && !IsExpandedForm
					&& !ParameterNames.Any(p => string.IsNullOrEmpty(p)))
				{
					Debug.Assert(skipCount == 0);
					if (ArgumentNames == null) {
						ArgumentNames = new string[Arguments.Length];
					}

					for (int i = 0; i < Arguments.Length; i++) {
						if (IsPrimitiveValue[i] && ArgumentNames[i] == null) {
							ArgumentNames[i] = ParameterNames[i];
						}
					}
				}
				int argumentCount = GetActualArgumentCount();
				if (ArgumentNames == null) {
					return Arguments.Skip(skipCount).Take(argumentCount).Select(arg => arg.Expression);
				} else {
					Debug.Assert(skipCount == 0);
					return Arguments.Take(argumentCount).Zip(ArgumentNames.Take(argumentCount),
						(arg, name) => {
							if (name == null)
								return arg.Expression;
							else
								return new NamedArgumentExpression(name, arg);
						});
				}
			}

			public bool CanInferAnonymousTypePropertyNamesFromArguments()
			{
				for (int i = 0; i < Arguments.Length; i++) {
					string inferredName;
					switch (Arguments[i].Expression) {
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

					if (inferredName != ExpectedParameters[i].Name) {
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
			if (inst is NewObj newobj && IL.Transforms.DelegateConstruction.IsDelegateConstruction(newobj)) {
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
					elementRRs.ToImmutableArray(),
					valueTupleAssembly: inst.Method.DeclaringType.GetDefinition()?.ParentModule
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
			if (method.IsExplicitInterfaceImplementation && callOpCode == OpCode.Call) {
				// Direct non-virtual call to explicit interface implementation.
				// This can't really be represented in C#, but at least in the case where
				// the class is sealed, we can equivalently call the interface member instead:
				var interfaceMembers = method.ExplicitlyImplementedInterfaceMembers.ToList();
				if (method.DeclaringTypeDefinition?.Kind == TypeKind.Class && method.DeclaringTypeDefinition.IsSealed && interfaceMembers.Count == 1) {
					method = (IMethod)interfaceMembers.Single();
					callOpCode = OpCode.CallVirt;
				}
			}
			// Used for Call, CallVirt and NewObj
			var expectedTargetDetails = new ExpectedTargetDetails {
				CallOpCode = callOpCode
			};
			ILFunction localFunction = null;
			if (method.IsLocalFunction) {
				localFunction = expressionBuilder.ResolveLocalFunction(method);
				Debug.Assert(localFunction != null);
			}
			TranslatedExpression target;
			if (callOpCode == OpCode.NewObj) {
				target = default(TranslatedExpression); // no target
			} else if (localFunction != null) {
				var ide = new IdentifierExpression(localFunction.Name);
				if (method.TypeArguments.Count > 0) {
					int skipCount = localFunction.ReducedMethod.NumberOfCompilerGeneratedTypeParameters;
					ide.TypeArguments.AddRange(method.TypeArguments.Skip(skipCount).Select(expressionBuilder.ConvertType));
				}
				target = ide.WithoutILInstruction()
					.WithRR(ToMethodGroup(method, localFunction));
			} else {
				target = expressionBuilder.TranslateTarget(
					callArguments.FirstOrDefault(),
					nonVirtualInvocation: callOpCode == OpCode.Call || method.IsConstructor,
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

			var argumentList = BuildArgumentList(expectedTargetDetails, target.ResolveResult, method,
				firstParamIndex, callArguments, argumentToParameterMap);

			if (localFunction != null) {
				return new InvocationExpression(target, argumentList.GetArgumentExpressions())
					.WithRR(new CSharpInvocationResolveResult(target.ResolveResult, method,
						argumentList.GetArgumentResolveResults().ToList(), isExpandedForm: argumentList.IsExpandedForm));
			}
			
			if (method is VarArgInstanceMethod) {
				argumentList.FirstOptionalArgumentIndex = -1;
				argumentList.AddNamesToPrimitiveValues = false;
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

			if (settings.Ranges) {
				if (HandleRangeConstruction(out var result, callOpCode, method, argumentList)) {
					return result;
				}
			}

			if (callOpCode == OpCode.NewObj) {
				return HandleConstructorCall(expectedTargetDetails, target.ResolveResult, method, argumentList);
			}

			if (method.Name == "Invoke" && method.DeclaringType.Kind == TypeKind.Delegate && !IsNullConditional(target)) {
				return new InvocationExpression(target, argumentList.GetArgumentExpressions())
					.WithRR(new CSharpInvocationResolveResult(target.ResolveResult, method,
						argumentList.GetArgumentResolveResults().ToList(), isExpandedForm: argumentList.IsExpandedForm, isDelegateInvocation: true));
			}

			if (settings.StringInterpolation && IsInterpolatedStringCreation(method, argumentList) &&
				TryGetStringInterpolationTokens(argumentList, out string format, out var tokens))
			{
				var arguments = argumentList.Arguments;
				var content = new List<InterpolatedStringContent>();

				bool unpackSingleElementArray = !argumentList.IsExpandedForm && argumentList.Length == 2
					&& argumentList.Arguments[1].Expression is ArrayCreateExpression ace
					&& ace.Initializer?.Elements.Count == 1;

				void UnpackSingleElementArray(ref TranslatedExpression argument)
				{
					if (!unpackSingleElementArray) return;
					var arrayCreation = (ArrayCreateExpression)argumentList.Arguments[1].Expression;
					var arrayCreationRR = (ArrayCreateResolveResult)argumentList.Arguments[1].ResolveResult;
					var element = arrayCreation.Initializer.Elements.First().Detach();
					argument = new TranslatedExpression(element, arrayCreationRR.InitializerElements.First());
				}

				if (tokens.Count > 0) {
					foreach (var (kind, index, text) in tokens) {
						TranslatedExpression argument;
						switch (kind) {
							case TokenKind.String:
								content.Add(new InterpolatedStringText(text));
								break;
							case TokenKind.Argument:
								argument = arguments[index + 1];
								UnpackSingleElementArray(ref argument);
								content.Add(new Interpolation(argument));
								break;
							case TokenKind.ArgumentWithFormat:
								argument = arguments[index + 1];
								UnpackSingleElementArray(ref argument);
								content.Add(new Interpolation(argument, text));
								break;
						}
					}
					var formattableStringType = expressionBuilder.compilation.FindType(KnownTypeCode.FormattableString);
					var isrr = new InterpolatedStringResolveResult(expressionBuilder.compilation.FindType(KnownTypeCode.String),
						format, argumentList.GetArgumentResolveResults().Skip(1).ToArray());
					var expr = new InterpolatedStringExpression();
					expr.Content.AddRange(content);
					if (method.Name == "Format")
						return expr.WithRR(isrr);
					return new CastExpression(expressionBuilder.ConvertType(formattableStringType),
						expr.WithRR(isrr))
						.WithRR(new ConversionResolveResult(formattableStringType, isrr, Conversion.ImplicitInterpolatedStringConversion));
				}
			}

			int allowedParamCount = (method.ReturnType.IsKnownType(KnownTypeCode.Void) ? 1 : 0);
			if (method.IsAccessor && (method.AccessorOwner.SymbolKind == SymbolKind.Indexer || argumentList.ExpectedParameters.Length == allowedParamCount)) {
				argumentList.CheckNoNamedOrOptionalArguments();
				return HandleAccessorCall(expectedTargetDetails, method, target, argumentList.Arguments.ToList(), argumentList.ArgumentNames); 
			}
			
			if (IsDelegateEqualityComparison(method, argumentList.Arguments)) {
				argumentList.CheckNoNamedOrOptionalArguments();
				return HandleDelegateEqualityComparison(method, argumentList.Arguments)
					.WithRR(new CSharpInvocationResolveResult(target.ResolveResult, method,
						argumentList.GetArgumentResolveResults().ToList(), isExpandedForm: argumentList.IsExpandedForm));
			}
			
			if (method.IsOperator && method.Name == "op_Implicit" && argumentList.Length == 1) {
				argumentList.CheckNoNamedOrOptionalArguments();
				return HandleImplicitConversion(method, argumentList.Arguments[0]);
			}

			var transform = GetRequiredTransformationsForCall(expectedTargetDetails, method, ref target,
				ref argumentList, CallTransformation.All, out IParameterizedMember foundMethod);

			// Note: after this, 'method' and 'foundMethod' may differ,
			// but as far as allowed by IsAppropriateCallTarget().

			// Need to update list of parameter names, because foundMethod is different and thus might use different names.
			if (!method.Equals(foundMethod) && argumentList.ParameterNames.Length >= foundMethod.Parameters.Count) {
				for (int i = 0; i < foundMethod.Parameters.Count; i++) {
					argumentList.ParameterNames[i] = foundMethod.Parameters[i].Name;
				}
			}

			Expression targetExpr;
			string methodName = method.Name;
			AstNodeCollection<AstType> typeArgumentList;
			if ((transform & CallTransformation.NoOptionalArgumentAllowed) != 0) {
				argumentList.FirstOptionalArgumentIndex = -1;
			}
			if ((transform & CallTransformation.RequireTarget) != 0) {
				targetExpr = new MemberReferenceExpression(target.Expression, methodName);
				typeArgumentList = ((MemberReferenceExpression)targetExpr).TypeArguments;

				// HACK : convert this.Dispose() to ((IDisposable)this).Dispose(), if Dispose is an explicitly implemented interface method.
				// settings.AlwaysCastTargetsOfExplicitInterfaceImplementationCalls == true is used in Windows Forms' InitializeComponent methods.
				if (method.IsExplicitInterfaceImplementation && (target.Expression is ThisReferenceExpression || settings.AlwaysCastTargetsOfExplicitInterfaceImplementationCalls)) {
					var interfaceMember = method.ExplicitlyImplementedInterfaceMembers.First();
					var castExpression = new CastExpression(expressionBuilder.ConvertType(interfaceMember.DeclaringType), target.Expression.Detach());
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
			return new InvocationExpression(targetExpr, argumentList.GetArgumentExpressions())
				.WithRR(new CSharpInvocationResolveResult(target.ResolveResult, foundMethod,
					argumentList.GetArgumentResolveResults().ToList(), isExpandedForm: argumentList.IsExpandedForm));
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
			var transform = GetRequiredTransformationsForCall(expectedTargetDetails, method, ref unused,
				ref argumentList, CallTransformation.None, out _);
			Debug.Assert(transform == CallTransformation.None || transform == CallTransformation.NoOptionalArgumentAllowed);

			// Calls with only one argument do not need an array initializer expression to wrap them.
			// Any special cases are handled by the caller (i.e., ExpressionBuilder.TranslateObjectAndCollectionInitializer)
			// Note: we intentionally ignore the firstOptionalArgumentIndex in this case.
			int skipCount;
			if (method.IsExtensionMethod) {
				if (argumentList.Arguments.Length == 2)
					return argumentList.Arguments[1];
				skipCount = 1;
			} else {
				if (argumentList.Arguments.Length == 1)
					return argumentList.Arguments[0];
				skipCount = 0;
			}

			if ((transform & CallTransformation.NoOptionalArgumentAllowed) != 0)
				argumentList.FirstOptionalArgumentIndex = -1;

			return new ArrayInitializerExpression(argumentList.GetArgumentExpressions(skipCount))
				.WithRR(new CSharpInvocationResolveResult(target, method, argumentList.GetArgumentResolveResults(skipCount).ToArray(),
					isExtensionMethodInvocation: method.IsExtensionMethod, isExpandedForm: argumentList.IsExpandedForm));
		}

		public ExpressionWithResolveResult BuildDictionaryInitializerExpression(OpCode callOpCode, IMethod method,
			InitializedObjectResolveResult target, IReadOnlyList<ILInstruction> indices, ILInstruction value = null)
		{
			ExpectedTargetDetails expectedTargetDetails = new ExpectedTargetDetails { CallOpCode = callOpCode };

			var callArguments = new List<ILInstruction>();
			callArguments.Add(new LdNull());
			callArguments.AddRange(indices);
			callArguments.Add(value ?? new Nop());

			var argumentList = BuildArgumentList(expectedTargetDetails, target, method, 1, callArguments, null);
			var unused = new IdentifierExpression("initializedObject").WithRR(target).WithoutILInstruction();

			var assignment = HandleAccessorCall(expectedTargetDetails, method, unused,
				argumentList.Arguments.ToList(), argumentList.ArgumentNames);

			if (((AssignmentExpression)assignment).Left is IndexerExpression indexer && !indexer.Target.IsNull) 
				indexer.Target.ReplaceWith(n => null);

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

		private bool TryGetStringInterpolationTokens(ArgumentList argumentList, out string format, out List<(TokenKind, int, string)> tokens)
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
			tokens = new List<(TokenKind, int, string)>();
			int i = 0;
			format = (string)crr.ConstantValue;
			foreach (var (kind, data) in TokenizeFormatString(format)) {
				int index;
				switch (kind) {
					case TokenKind.Error:
						return false;
					case TokenKind.String:
						tokens.Add((kind, -1, data));
						break;
					case TokenKind.Argument:
						if (!int.TryParse(data, out index) || index != i)
							return false;
						i++;
						tokens.Add((kind, index, null));
						break;
					case TokenKind.ArgumentWithFormat:
						string[] arg = data.Split(new[] { ':' }, 2);
						if (arg.Length != 2 || arg[1].Length == 0)
							return false;
						if (!int.TryParse(arg[0], out index) || index != i)
							return false;
						i++;
						tokens.Add((kind, index, arg[1]));
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
			ArgumentWithFormat
		}

		private IEnumerable<(TokenKind, string)> TokenizeFormatString(string value)
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

			while ((next = Next()) > -1) {
				switch ((char)next) {
					case '{':
						if (Peek() == '{') {
							kind = TokenKind.String;
							sb.Append("{{");
							Next();
						} else {
							if (sb.Length > 0) {
								yield return (kind, sb.ToString());
							}
							kind = TokenKind.Argument;
							sb.Clear();
						}
						break;
					case '}':
						if (kind != TokenKind.String) {
							yield return (kind, sb.ToString());
							sb.Clear();
							kind = TokenKind.String;
						} else {
							sb.Append((char)next);
						}
						break;
					case ':':
						if (kind == TokenKind.Argument) {
							kind = TokenKind.ArgumentWithFormat;
						}
						sb.Append(':');
						break;
					default:
						sb.Append((char)next);
						break;
				}
			}
			if (sb.Length > 0) {
				if (kind == TokenKind.String)
					yield return (kind, sb.ToString());
				else
					yield return (TokenKind.Error, null);
			}
		}

		private ArgumentList BuildArgumentList(ExpectedTargetDetails expectedTargetDetails, ResolveResult target, IMethod method,
			int firstParamIndex, IReadOnlyList<ILInstruction> callArguments, IReadOnlyList<int> argumentToParameterMap)
		{
			ArgumentList list = new ArgumentList();

			// Translate arguments to the expected parameter types
			var arguments = new List<TranslatedExpression>(method.Parameters.Count);
			string[] argumentNames = null;
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
			for (int i = firstParamIndex; i < callArguments.Count; i++) {
				IParameter parameter;
				if (argumentToParameterMap != null) {
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
				if (IsPrimitiveValueThatShouldBeNamedArgument(arg, method, parameter)) {
					isPrimitiveValue.Set(arguments.Count);
				}
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
					if (TransformParamsArgument(expectedTargetDetails, target, method, parameter,
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

				if (parameter.ReferenceKind != ReferenceKind.None) {
					arg = ExpressionBuilder.ChangeDirectionExpressionTo(arg, parameter.ReferenceKind);
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
			list.AddNamesToPrimitiveValues = expressionBuilder.settings.NamedArguments && expressionBuilder.settings.NonTrailingNamedArguments;
			return list;
		}

		private bool IsPrimitiveValueThatShouldBeNamedArgument(TranslatedExpression arg, IMethod method, IParameter p)
		{
			if (!arg.ResolveResult.IsCompileTimeConstant || method.DeclaringType.IsKnownType(KnownTypeCode.NullableOfT))
				return false;
			return p.Type.IsKnownType(KnownTypeCode.Boolean);
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
				if (IsUnambiguousCall(expectedTargetDetails, method, targetResolveResult, Empty<IType>.Array,
					expandedArguments, argumentNames: null, firstOptionalArgumentIndex: -1, out _,
					out var bestCandidateIsExpandedForm) == OverloadResolutionErrors.None && bestCandidateIsExpandedForm)
				{
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
			return object.Equals(parameter.GetConstantValue(), arg.ResolveResult.ConstantValue);
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
			ref TranslatedExpression target, ref ArgumentList argumentList, CallTransformation allowedTransforms, out IParameterizedMember foundMethod)
		{
			CallTransformation transform = CallTransformation.None;

			// initialize requireTarget flag
			bool requireTarget;
			ResolveResult targetResolveResult;
			if ((allowedTransforms & CallTransformation.RequireTarget) != 0) {
				if (expressionBuilder.HidesVariableWithName(method.Name)) {
					requireTarget = true;
				} else {
					if (method.IsLocalFunction)
						requireTarget = false;
					else if (method.IsStatic)
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
				&& !IsPossibleExtensionMethodCallOnNull(method, argumentList.Arguments)) {
				// The ambiguity resolution below only adds type arguments as last resort measure, however there are
				// methods, such as Enumerable.OfType<TResult>(IEnumerable input) that always require type arguments,
				// as those cannot be inferred from the parameters, which leads to bloated expressions full of extra casts
				// that are no longer required once we add the type arguments.
				// We lend overload resolution a hand by detecting such cases beforehand and requiring type arguments,
				// if necessary.
				if (!CanInferTypeArgumentsFromParameters(method, argumentList.Arguments.SelectArray(a => a.ResolveResult), expressionBuilder.typeInference)) {
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
			while ((errors = IsUnambiguousCall(expectedTargetDetails, method, targetResolveResult, typeArguments,
				argumentList.Arguments, argumentList.ArgumentNames, argumentList.FirstOptionalArgumentIndex, out foundMethod,
				out var bestCandidateIsExpandedForm)) != OverloadResolutionErrors.None || bestCandidateIsExpandedForm != argumentList.IsExpandedForm)
			{
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
						if (argumentList.FirstOptionalArgumentIndex == -1) goto default;
						argumentList.FirstOptionalArgumentIndex = -1;
						continue;
					default:
						// TODO : implement some more intelligent algorithm that decides which of these fixes (cast args, add target, cast target, add type args)
						// is best in this case. Additionally we should not cast all arguments at once, but step-by-step try to add only a minimal number of casts.
						if (argumentList.FirstOptionalArgumentIndex >= 0) {
							argumentList.FirstOptionalArgumentIndex = -1;
						} else if (!argumentsCasted) {
							// If we added type arguments beforehand, but that didn't make the code any better,
							// undo that decision and add casts first.
							if (appliedRequireTypeArgumentsShortcut) {
								requireTypeArguments = false;
								typeArguments = Empty<IType>.Array;
								appliedRequireTypeArgumentsShortcut = false;
							}
							argumentsCasted = true;
							CastArguments(argumentList.Arguments, argumentList.ExpectedParameters);
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
			if (argumentList.FirstOptionalArgumentIndex < 0)
				transform |= CallTransformation.NoOptionalArgumentAllowed;
			return transform;
		}

		private bool IsPossibleExtensionMethodCallOnNull(IMethod method, IList<TranslatedExpression> arguments)
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

		private void CastArguments(IList<TranslatedExpression> arguments, IList<IParameter> expectedParameters)
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
			out IParameterizedMember foundMember, out bool bestCandidateIsExpandedForm)
		{
			foundMember = null;
			bestCandidateIsExpandedForm = false;
			var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentModule);

			Log.WriteLine("IsUnambiguousCall: Performing overload resolution for " + method);
			Log.WriteCollection("  Arguments: ", arguments.Select(a => a.ResolveResult));

			var or = new OverloadResolution(resolver.Compilation,
				firstOptionalArgumentIndex < 0 ? arguments.SelectArray(a => a.ResolveResult) : arguments.Take(firstOptionalArgumentIndex).Select(a => a.ResolveResult).ToArray(),
				argumentNames: firstOptionalArgumentIndex < 0 || argumentNames == null ? argumentNames : argumentNames.Take(firstOptionalArgumentIndex).ToArray(),
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
					IType argType = NullableType.GetUnderlyingType(arguments[0].Type);
					operatorCandidates = resolver.GetUserDefinedOperatorCandidates(argType, method.Name);
				} else if (arguments.Count == 2) {
					IType lhsType = NullableType.GetUnderlyingType(arguments[0].Type);
					IType rhsType = NullableType.GetUnderlyingType(arguments[1].Type);
					var hashSet = new HashSet<IParameterizedMember>();
					hashSet.UnionWith(resolver.GetUserDefinedOperatorCandidates(lhsType, method.Name));
					hashSet.UnionWith(resolver.GetUserDefinedOperatorCandidates(rhsType, method.Name));
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
			bestCandidateIsExpandedForm = or.BestCandidateIsExpandedForm;
			if (or.BestCandidateErrors != OverloadResolutionErrors.None)
				return or.BestCandidateErrors;
			if (or.IsAmbiguous)
				return OverloadResolutionErrors.AmbiguousMatch;
			foundMember = or.GetBestCandidateWithSubstitutedTypeArguments();
			if (!IsAppropriateCallTarget(expectedTargetDetails, method, foundMember))
				return OverloadResolutionErrors.AmbiguousMatch;
			return OverloadResolutionErrors.None;
		}

		bool IsUnambiguousAccess(ExpectedTargetDetails expectedTargetDetails, ResolveResult target, IMethod method,
			IList<TranslatedExpression> arguments, string[] argumentNames, out IMember foundMember)
		{
			Log.WriteLine("IsUnambiguousAccess: Performing overload resolution for " + method);
			Log.WriteCollection("  Arguments: ", arguments.Select(a => a.ResolveResult));

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
			TranslatedExpression target, List<TranslatedExpression> arguments, string[] argumentNames)
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
					CastArguments(arguments, method.Parameters.ToList());
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
					expr = new IndexerExpression(target.ResolveResult is InitializedObjectResolveResult ? null : target.Expression, arguments.Select(a => a.Expression))
						.WithoutILInstruction().WithRR(rr);
				} else if (requireTarget) {
					expr = new MemberReferenceExpression(target.Expression, method.AccessorOwner.Name)
						.WithoutILInstruction().WithRR(rr);
				} else {
					expr = new IdentifierExpression(method.AccessorOwner.Name)
						.WithoutILInstruction().WithRR(rr);
				}

				var op = AssignmentOperatorType.Assign;
				if (method.AccessorOwner is IEvent parentEvent) {
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

		ExpressionWithResolveResult HandleConstructorCall(ExpectedTargetDetails expectedTargetDetails, ResolveResult target, IMethod method, ArgumentList argumentList)
		{
			if (settings.AnonymousTypes && method.DeclaringType.IsAnonymousType()) {
				Debug.Assert(argumentList.ArgumentToParameterMap == null && argumentList.ArgumentNames == null && argumentList.FirstOptionalArgumentIndex < 0);
				var atce = new AnonymousTypeCreateExpression();
				if (argumentList.CanInferAnonymousTypePropertyNamesFromArguments()) {
					atce.Initializers.AddRange(argumentList.GetArgumentExpressions());
				} else {
					for (int i = 0; i < argumentList.Length; i++) {
						atce.Initializers.Add(
							new NamedExpression {
								Name = argumentList.ExpectedParameters[i].Name,
								Expression = argumentList.Arguments[i].ConvertTo(argumentList.ExpectedParameters[i].Type, expressionBuilder)
							});
					}
				}
				return atce.WithRR(new CSharpInvocationResolveResult(
					target, method, argumentList.GetArgumentResolveResults().ToList(),
					isExpandedForm: argumentList.IsExpandedForm, argumentToParameterMap: argumentList.ArgumentToParameterMap
				));
			} else {
				while (IsUnambiguousCall(expectedTargetDetails, method, null, Empty<IType>.Array, argumentList.Arguments,
					argumentList.ArgumentNames, argumentList.FirstOptionalArgumentIndex, out _,
					out var bestCandidateIsExpandedForm) != OverloadResolutionErrors.None || bestCandidateIsExpandedForm != argumentList.IsExpandedForm)
				{
					if (argumentList.FirstOptionalArgumentIndex >= 0) {
						argumentList.FirstOptionalArgumentIndex = -1;
						continue;
					}
					CastArguments(argumentList.Arguments, argumentList.ExpectedParameters);
					break; // make sure that we don't not end up in an infinite loop
				}
				return new ObjectCreateExpression(
					expressionBuilder.ConvertType(method.DeclaringType),
					argumentList.GetArgumentExpressions()
				).WithRR(new CSharpInvocationResolveResult(
					target, method, argumentList.GetArgumentResolveResults().ToArray(),
					isExpandedForm: argumentList.IsExpandedForm, argumentToParameterMap: argumentList.ArgumentToParameterMap
				));
			}
		}

		TranslatedExpression HandleDelegateConstruction(CallInstruction inst)
		{
			ILInstruction thisArg = inst.Arguments[0];
			ILInstruction func = inst.Arguments[1];
			IMethod method;
			ExpectedTargetDetails expectedTargetDetails = default;
			switch (func.OpCode) {
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
			if (CanUseDelegateConstruction(method, thisArg, inst.Method.DeclaringType.GetDelegateInvokeMethod())) {
				return HandleDelegateConstruction(inst.Method.DeclaringType, method, expectedTargetDetails, thisArg, inst);
			} else {
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
			if (targetMethod.IsStatic) {
				// If the invoke method is known, we can compare the parameter counts to figure out whether the
				// delegate is static or binds the first argument
				if (invokeMethod != null) {
					if (invokeMethod.Parameters.Count == targetMethod.Parameters.Count) {
						return thisArg.MatchLdNull();
					} else if (targetMethod.IsExtensionMethod && invokeMethod.Parameters.Count == targetMethod.Parameters.Count - 1) {
						return true;
					} else {
						return false;
					}
				} else {
					// delegate type unknown:
					return thisArg.MatchLdNull() || targetMethod.IsExtensionMethod;
				}
			} else {
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

		ExpressionWithResolveResult BuildDelegateReference(IMethod method, IMethod invokeMethod, ExpectedTargetDetails expectedTargetDetails, ILInstruction thisArg)
		{
			TranslatedExpression target;
			IType targetType;
			bool requireTarget;
			ResolveResult result = null;
			string methodName = method.Name;
			// There are three possible adjustments, we can make, to solve conflicts:
			// 1. add target (represented as bit 0)
			// 2. add type arguments (represented as bit 1)
			// 3. cast target (represented as bit 2)
			int step;
			ILFunction localFunction = null;
			if (method.IsLocalFunction) {
				localFunction = expressionBuilder.ResolveLocalFunction(method);
				Debug.Assert(localFunction != null);
			}
			if (localFunction != null) {
				step = 2;
				requireTarget = false;
				result = ToMethodGroup(method, localFunction);
				target = default;
				targetType = default;
				methodName = localFunction.Name;
				// TODO : think about how to handle generic local functions
			} else if (method.IsExtensionMethod && invokeMethod != null && method.Parameters.Count - 1 == invokeMethod.Parameters.Count) {
				step = 5;
				targetType = method.Parameters[0].Type;
				if (targetType.Kind == TypeKind.ByReference && thisArg is Box thisArgBox) {
					targetType = ((ByReferenceType)targetType).ElementType;
					thisArg = thisArgBox.Argument;
				}
				target = expressionBuilder.Translate(thisArg, targetType);
				// TODO : check if cast is necessary
				target = target.ConvertTo(targetType, expressionBuilder);
				requireTarget = true;
				result = new MethodGroupResolveResult(
					target.ResolveResult, method.Name,
					new MethodListWithDeclaringType[] {
						new MethodListWithDeclaringType(
							null,
							new[] { method }
						)
					},
					method.TypeArguments
				);
			} else {
				targetType = method.DeclaringType;
				if (targetType.IsReferenceType == false && thisArg is Box thisArgBox) {
					// Normal struct instance method calls (which TranslateTarget is meant for) expect a 'ref T',
					// but delegate construction uses a 'box T'.
					if (thisArgBox.Argument is LdObj ldobj) {
						thisArg = ldobj.Target;
					} else {
						thisArg = new AddressOf(thisArgBox.Argument, thisArgBox.Type);
					}
				}
				target = expressionBuilder.TranslateTarget(thisArg,
					nonVirtualInvocation: expectedTargetDetails.CallOpCode == OpCode.Call,
					memberStatic: method.IsStatic,
					memberDeclaringType: method.DeclaringType);
				requireTarget = expressionBuilder.HidesVariableWithName(method.Name)
					|| (method.IsStatic ? !expressionBuilder.IsCurrentOrContainingType(method.DeclaringTypeDefinition) : !(target.Expression is ThisReferenceExpression));
				step = requireTarget ? 1 : 0;
				var savedTarget = target;
				for (; step < 7; step++) {
					ResolveResult targetResolveResult;
					//TODO: why there is an check for IsLocalFunction here, it should be unreachable in old code
					if (localFunction == null && (step & 1) != 0) {
						targetResolveResult = savedTarget.ResolveResult;
						target = savedTarget;
					} else {
						targetResolveResult = null;
					}
					IReadOnlyList<IType> typeArguments;
					if ((step & 2) != 0) {
						typeArguments = method.TypeArguments;
					} else {
						typeArguments = EmptyList<IType>.Instance;
					}
					if (targetResolveResult != null && targetType != null && (step & 4) != 0) {
						target = target.ConvertTo(targetType, expressionBuilder);
						targetResolveResult = target.ResolveResult;
					}
					bool success = IsUnambiguousMethodReference(expectedTargetDetails, method, targetResolveResult, typeArguments, out var newResult);
					if (newResult is MethodGroupResolveResult || result == null)
						result = newResult;
					if (success)
						break;
				}
			}
			requireTarget = localFunction == null && (step & 1) != 0;
			ExpressionWithResolveResult targetExpression;
			Debug.Assert(result != null);
			if (requireTarget) {
				Debug.Assert(target.Expression != null);
				var mre = new MemberReferenceExpression(target, methodName);
				if ((step & 2) != 0)
					mre.TypeArguments.AddRange(method.TypeArguments.Select(expressionBuilder.ConvertType));
				targetExpression = mre.WithRR(result);
			} else {
				var ide = new IdentifierExpression(methodName);
				if ((step & 2) != 0) {
					int skipCount = 0;
					if (localFunction != null && method.TypeArguments.Count > 0) {
						skipCount = localFunction.ReducedMethod.NumberOfCompilerGeneratedTypeParameters;
					}
					ide.TypeArguments.AddRange(method.TypeArguments.Skip(skipCount).Select(expressionBuilder.ConvertType));
				}
				targetExpression = ide.WithRR(result);
			}
			return targetExpression;
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

		bool IsUnambiguousMethodReference(ExpectedTargetDetails expectedTargetDetails, IMethod method, ResolveResult target, IReadOnlyList<IType> typeArguments, out ResolveResult result)
		{
			Log.WriteLine("IsUnambiguousMethodReference: Performing overload resolution for " + method);

			var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentModule);
			var or = new OverloadResolution(resolver.Compilation,
				arguments: method.Parameters.SelectReadOnlyArray(p => new TypeResolveResult(p.Type)), // there are no arguments, use parameter types
				argumentNames: null, // argument names are not possible
				typeArguments.ToArray(),
				conversions: expressionBuilder.resolver.conversions
			);
			if (target == null) {
				result = resolver.ResolveSimpleName(method.Name, typeArguments, isInvocationTarget: false);
				if (!(result is MethodGroupResolveResult mgrr))
					return false;
				or.AddMethodLists(mgrr.MethodsGroupedByDeclaringType.ToArray());
			} else {
				result = lookup.Lookup(target, method.Name, typeArguments, isInvocation: false);
				if (!(result is MethodGroupResolveResult mgrr))
					return false;
				or.AddMethodLists(mgrr.MethodsGroupedByDeclaringType.ToArray());
			}

			var foundMethod = or.GetBestCandidateWithSubstitutedTypeArguments();
			if (!IsAppropriateCallTarget(expectedTargetDetails, method, foundMethod))
				return false;
			return result is MethodGroupResolveResult;
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
				}, method.TypeArguments.Skip(localFunction.ReducedMethod.NumberOfCompilerGeneratedTypeParameters).ToArray()
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

		private bool HandleRangeConstruction(out ExpressionWithResolveResult result, OpCode callOpCode, IMethod method, ArgumentList argumentList)
		{
			result = default;
			if (argumentList.ArgumentNames != null) {
				return false; // range syntax doesn't support named arguments
			}
			if (method.DeclaringType.IsKnownType(KnownTypeCode.Range)) {
				if (callOpCode == OpCode.NewObj && argumentList.Length == 2) {
					result = new BinaryOperatorExpression(argumentList.Arguments[0], BinaryOperatorType.Range, argumentList.Arguments[1])
						.WithRR(new ResolveResult(method.DeclaringType));
					return true;
				} else if (callOpCode == OpCode.Call && method.Name == "get_All" && argumentList.Length == 0) {
					result = new BinaryOperatorExpression(Expression.Null, BinaryOperatorType.Range, Expression.Null)
						.WithRR(new ResolveResult(method.DeclaringType));
					return true;
				} else if (callOpCode == OpCode.Call && method.Name == "StartAt" && argumentList.Length == 1) {
					result = new BinaryOperatorExpression(argumentList.Arguments[0], BinaryOperatorType.Range, Expression.Null)
						.WithRR(new ResolveResult(method.DeclaringType));
					return true;
				} else if (callOpCode == OpCode.Call && method.Name == "EndAt" && argumentList.Length == 1) {
					result = new BinaryOperatorExpression(Expression.Null, BinaryOperatorType.Range, argumentList.Arguments[0])
						.WithRR(new ResolveResult(method.DeclaringType));
					return true;
				}
			} else if (callOpCode == OpCode.NewObj && method.DeclaringType.IsKnownType(KnownTypeCode.Index)) {
				if (argumentList.Length != 2)
					return false;
				if (!(argumentList.Arguments[1].Expression is PrimitiveExpression pe && pe.Value is true))
					return false;
				result = new UnaryOperatorExpression(UnaryOperatorType.IndexFromEnd, argumentList.Arguments[0])
					.WithRR(new ResolveResult(method.DeclaringType));
				return true;
			}
			return false;
		}
	}
}