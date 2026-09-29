// Copyright (c) 2026 Siegfried Pammer
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

#nullable enable

namespace ICSharpCode.Decompiler.CSharp
{
	/// <summary>
	/// A way of making a member reference more explicit, so that it resolves back to the
	/// member the IL referenced instead of to something the shorter spelling happens to hit.
	/// </summary>
	[Flags]
	internal enum ReferenceTransformation
	{
		None = 0,
		RequireTarget = 1,
		RequireTypeArguments = 2,
		NoOptionalArgumentAllowed = 4,
		/// <summary>
		/// Add calls to AsRefReadOnly for in parameters that did not have an explicit DirectionExpression yet.
		/// </summary>
		EnforceExplicitIn = 8,
		NoNamedArgsForPrettiness = 0x10,
		/// <summary>Cast every argument to its parameter type.</summary>
		CastArguments = 0x20,
		/// <summary>Cast the target to the declaring type.</summary>
		CastTarget = 0x40,
		All = 0x7f,
	}

	/// <summary>
	/// Finds the shortest spelling of a member reference that still resolves back to the member
	/// the IL referenced. A short spelling may bind to something else in the output's
	/// name-lookup context, so it is re-resolved and, while it does not bind back, made more
	/// explicit and tried again. Which check re-resolves it, and which escalations are open to it
	/// in what order, are both settled when it is built.
	///
	/// One of the For* factories builds it, escalates, and hands it back finished; the results
	/// are the fields below, not a return value.
	///
	/// In: the member, its declaring type, and how much room the surrounding syntax leaves for
	/// escalations.
	///
	/// In and out: <see cref="Target"/>, <see cref="TypeArguments"/> and
	/// <see cref="Arguments"/> are seeded with the shortest spelling and escalated in place.
	///
	/// Out: <see cref="RequireTarget"/> and <see cref="RequireTypeArguments"/> say whether the
	/// target and the type arguments are written at all; <see cref="Result"/> and
	/// <see cref="FoundMember"/> are what the last check resolved to. <see cref="Resolved"/> is
	/// false when no spelling resolved, leaving the most explicit one tried for the caller to
	/// emit anyway.
	/// </summary>
	internal struct Disambiguator
	{
		static readonly ReferenceTransformation[] FieldSteps = {
			ReferenceTransformation.RequireTarget,
			ReferenceTransformation.CastTarget,
		};

		/// <summary>A field access: only ever qualified and then cast.</summary>
		internal static Disambiguator ForField(ExpressionBuilder expressionBuilder, IField field,
			TranslatedExpression target, bool requireTarget)
		{
			var disambiguator = new Disambiguator(expressionBuilder, field, field.DeclaringType, target,
				requireTarget, FieldSteps, ReferenceTransformation.All);
			disambiguator.Resolved = disambiguator.Run();
			return disambiguator;
		}

		static readonly ReferenceTransformation[] MethodReferenceSteps = {
			ReferenceTransformation.RequireTypeArguments,
			ReferenceTransformation.RequireTarget,
			ReferenceTransformation.CastTarget,
		};

		/// <summary>
		/// An extension method reference always carries its target, so the only escalations
		/// left are making that target's type and then the type arguments explicit.
		/// </summary>
		static readonly ReferenceTransformation[] ExtensionMethodReferenceSteps = {
			ReferenceTransformation.CastTarget,
			ReferenceTransformation.RequireTypeArguments,
		};

		/// <summary>
		/// A method group. It has no arguments to cast, but unlike a field it can spell out its
		/// type arguments. <paramref name="castTargetUpFront"/> is for a target that carries no
		/// type of its own, such as a null literal, where the cast cannot wait its turn.
		/// </summary>
		internal static Disambiguator ForMethodReference(ExpressionBuilder expressionBuilder, IMethod method,
			IType targetType, TranslatedExpression target, bool requireTarget,
			ExpectedTargetDetails expectedTargetDetails, bool isExtensionMethodReference,
			bool castTargetUpFront = false)
		{
			var disambiguator = new Disambiguator(expressionBuilder, method, targetType, target, requireTarget,
				isExtensionMethodReference ? ExtensionMethodReferenceSteps : MethodReferenceSteps,
				ReferenceTransformation.All, expectedTargetDetails,
				isExtensionMethodReference: isExtensionMethodReference);
			if (castTargetUpFront)
			{
				disambiguator.ApplyUpFront(ReferenceTransformation.CastTarget);
			}
			disambiguator.Resolved = disambiguator.Run();
			return disambiguator;
		}

		static readonly ReferenceTransformation[] CallSteps = {
			ReferenceTransformation.NoNamedArgsForPrettiness,
			ReferenceTransformation.NoOptionalArgumentAllowed,
			ReferenceTransformation.CastArguments,
			ReferenceTransformation.RequireTarget,
			ReferenceTransformation.CastTarget,
			ReferenceTransformation.RequireTypeArguments,
			ReferenceTransformation.EnforceExplicitIn,
		};

		/// <summary>
		/// An ordinary method call - the one reference that can reach for every escalation there
		/// is. <paramref name="allowed"/> withholds the ones the syntax around the call has no
		/// room for.
		/// </summary>
		internal static Disambiguator ForCall(ExpressionBuilder expressionBuilder, IMethod method,
			ArgumentList argumentList, TranslatedExpression target, bool requireTarget,
			ExpectedTargetDetails expectedTargetDetails, ReferenceTransformation allowed)
		{
			var disambiguator = new Disambiguator(expressionBuilder, method, method.DeclaringType, target,
				requireTarget, CallSteps, allowed, expectedTargetDetails, invocationSyntax: true,
				castingArgumentsDropsImplicitlyTypedOut: true,
				skipTargetCast: method.Accessibility <= Accessibility.Protected
					&& expressionBuilder.IsBaseTypeOfCurrentType(method.DeclaringTypeDefinition)) {
				Arguments = argumentList,
			};
			disambiguator.WriteTypeArgumentsInferenceCannotReach();
			disambiguator.Resolved = disambiguator.Run();
			return disambiguator;
		}

		static readonly ReferenceTransformation[] ConstructorCallSteps = {
			ReferenceTransformation.NoNamedArgsForPrettiness,
			ReferenceTransformation.NoOptionalArgumentAllowed,
			ReferenceTransformation.CastArguments,
		};

		/// <summary>
		/// A constructor call. There is no target to qualify or cast and the type arguments come
		/// from the type being constructed, so only the arguments are left.
		/// </summary>
		internal static Disambiguator ForConstructorCall(ExpressionBuilder expressionBuilder, IMethod method,
			ArgumentList argumentList, ExpectedTargetDetails expectedTargetDetails)
		{
			var disambiguator = new Disambiguator(expressionBuilder, method, method.DeclaringType, default,
				requireTarget: false, ConstructorCallSteps, ReferenceTransformation.All,
				expectedTargetDetails, invocationSyntax: true) {
				Arguments = argumentList,
			};
			disambiguator.Resolved = disambiguator.Run();
			return disambiguator;
		}

		static readonly ReferenceTransformation[] AccessorSteps = {
			ReferenceTransformation.NoOptionalArgumentAllowed,
			ReferenceTransformation.CastArguments,
			ReferenceTransformation.RequireTarget,
			ReferenceTransformation.CastTarget,
		};

		/// <summary>
		/// A property, indexer or event access. <paramref name="argumentList"/> holds the indices,
		/// with the parameters the accessor declares: a setter's value is not part of the
		/// reference and must already have been split off.
		/// </summary>
		internal static Disambiguator ForAccessor(ExpressionBuilder expressionBuilder, IMethod accessor,
			ArgumentList argumentList, TranslatedExpression target, bool requireTarget,
			ExpectedTargetDetails expectedTargetDetails)
		{
			var disambiguator = new Disambiguator(expressionBuilder, accessor,
				accessor.AccessorOwner!.DeclaringType, target, requireTarget, AccessorSteps,
				ReferenceTransformation.All, expectedTargetDetails) {
				Arguments = argumentList,
			};
			if (argumentList.Length == 0)
			{
				// Nothing to cast, whatever the other steps do: a property access, or an indexer
				// assignment whose value has already been taken out.
				disambiguator.MarkApplied(ReferenceTransformation.CastArguments);
			}
			disambiguator.Resolved = disambiguator.Run();
			return disambiguator;
		}

		// In. Fixed for the life of the escalation.

		readonly ExpressionBuilder expressionBuilder;
		readonly IMember member;
		readonly IType declaringType;
		/// <summary>The escalations open to this reference, in the order it prefers them.</summary>
		readonly ReferenceTransformation[] steps;
		/// <summary>
		/// Whether the reference is written with invocation syntax. Nothing on an IMethod says
		/// that: a method group is not, a property access is not even though it is a call in IL,
		/// and a parameterized property's accessor is, despite being an accessor.
		/// </summary>
		readonly bool invocationSyntax;
		readonly bool isExtensionMethodReference;
		/// <summary>The escalations this caller has room for; the rest are never offered.</summary>
		readonly ReferenceTransformation allowed;
		readonly ExpectedTargetDetails expectedTargetDetails;
		/// <summary>
		/// Whether casting the arguments also means giving up "out var". It does for a call,
		/// whose out arguments are written from the argument list; a constructor call keeps
		/// the sugar, and an accessor has no out parameters to write.
		/// </summary>
		readonly bool castingArgumentsDropsImplicitlyTypedOut;
		/// <summary>
		/// Whether a protected member declared in a base type lets the reference drop its
		/// qualifier again rather than cast it, which would not compile.
		/// </summary>
		readonly bool skipTargetCast;
		/// <summary>Whether the reference was qualified before any escalation.</summary>
		readonly bool initiallyRequiredTarget;

		// In and out. The spelling: seeded short, escalated in place, read back after Run.

		public TranslatedExpression Target;
		/// <summary>The type arguments to write out; empty while they stay inferred.</summary>
		public IType[] TypeArguments;
		/// <summary>The arguments, and the decisions about how to write them.</summary>
		public ArgumentList Arguments;
		/// <summary>The escalations that are currently in effect.</summary>
		ReferenceTransformation Applied;
		/// <summary>
		/// The escalations already offered. Normally the same as <see cref="Applied"/>, but an
		/// escalation that was tried and then taken back again stays spent, so it is not
		/// offered again.
		/// </summary>
		ReferenceTransformation spent;
		/// <summary>
		/// Whether the type arguments were written up front because they cannot be inferred.
		/// Casting the arguments takes that guess back, so that casts are tried first.
		/// </summary>
		bool TypeArgumentsWereAppliedUpFront;

		// Out. Read back after Run.

		/// <summary>Whether the reference is written with its target.</summary>
		public bool RequireTarget => (Applied & ReferenceTransformation.RequireTarget) != 0;
		/// <summary>Whether the type arguments are written out rather than left to inference.</summary>
		public bool RequireTypeArguments => (Applied & ReferenceTransformation.RequireTypeArguments) != 0;
		/// <summary>What the last check resolved to.</summary>
		public ResolveResult? Result;
		/// <summary>The member the last check bound to.</summary>
		public IMember? FoundMember;
		/// <summary>Whether a spelling was found that resolves back to the member.</summary>
		public bool Resolved;

		Disambiguator(ExpressionBuilder expressionBuilder, IMember member, IType declaringType,
			TranslatedExpression target, bool requireTarget,
			ReferenceTransformation[] steps, ReferenceTransformation allowed,
			ExpectedTargetDetails expectedTargetDetails = default, bool invocationSyntax = false,
			bool isExtensionMethodReference = false,
			bool castingArgumentsDropsImplicitlyTypedOut = false, bool skipTargetCast = false)
		{
			this.expressionBuilder = expressionBuilder;
			this.member = member;
			this.declaringType = declaringType;
			this.allowed = allowed;
			this.Target = target;
			this.Applied = requireTarget ? ReferenceTransformation.RequireTarget : ReferenceTransformation.None;
			this.spent = this.Applied;
			this.TypeArguments = Empty<IType>.Array;
			this.Result = null;
			this.expectedTargetDetails = expectedTargetDetails;
			this.steps = steps;
			this.invocationSyntax = invocationSyntax;
			this.isExtensionMethodReference = isExtensionMethodReference;
			this.castingArgumentsDropsImplicitlyTypedOut = castingArgumentsDropsImplicitlyTypedOut;
			this.skipTargetCast = skipTargetCast;
			this.initiallyRequiredTarget = requireTarget;
			this.TypeArgumentsWereAppliedUpFront = false;
			this.Arguments = default;
			this.FoundMember = null;
			this.Resolved = false;
		}

		CSharpResolver resolver => expressionBuilder.resolver;

		static bool IsPossibleExtensionMethodCallOnNull(IMethod method, IList<TranslatedExpression> arguments)
		{
			return method.IsExtensionMethod && arguments.Count > 0 && arguments[0].Expression is NullReferenceExpression;
		}

		bool CanInferTypeArgumentsFromArguments(IMethod method)
		{
			if (method.TypeParameters.Count == 0)
				return true;
			// always use unspecialized member, otherwise type inference fails
			method = (IMethod)method.MemberDefinition;
			IReadOnlyList<IType> paramTypesInArgumentOrder;
			if (Arguments.ArgumentToParameterMap == null)
				paramTypesInArgumentOrder = method.Parameters.SelectReadOnlyArray(p => p.Type);
			else
				paramTypesInArgumentOrder = Arguments.ArgumentToParameterMap
					.SelectReadOnlyArray(
						index => index >= 0 ? method.Parameters[index].Type : SpecialType.UnknownType
					);
			expressionBuilder.typeInference.InferTypeArguments(method.TypeParameters,
				Arguments.Arguments.SelectReadOnlyArray(a => a.ResolveResult), paramTypesInArgumentOrder,
				out bool success);
			return success;
		}

		/// <summary>
		/// C# has no syntax to spell out an anonymous type, so a null literal cannot be given such
		/// a type with a cast. The minimal expression that produces a null value of an anonymous
		/// type is a conditional expression whose never-taken branch creates an instance of the
		/// type: <c>true ? null : new { A = default(int) }</c>.
		/// Replaces null-literal arguments of an anonymous type with such an expression, so that
		/// type arguments involving anonymous types (which cannot be written explicitly either)
		/// become inferable from the arguments.
		/// Returns true, if at least one argument was replaced.
		/// </summary>
		bool PinTypesOfNullArguments()
		{
			bool anyArgumentReplaced = false;
			for (int i = 0; i < Arguments.Length; i++)
			{
				IType expectedType = Arguments.ExpectedParameters[i].Type;
				if (Arguments.Arguments[i].Expression is not NullReferenceExpression)
					continue;
				if (!expectedType.IsAnonymousType() || NewAnonymousTypeInstance(expectedType) is not NewObj newObj)
					continue;
				var nullLiteral = Arguments.Arguments[i];
				Arguments.Arguments[i] = new ConditionalExpression(new PrimitiveExpression(true),
						nullLiteral.Expression.Detach(), expressionBuilder.Translate(newObj, expectedType))
					.WithILInstruction(nullLiteral.ILInstructions)
					.WithRR(new ResolveResult(expectedType));
				anyArgumentReplaced = true;
			}
			return anyArgumentReplaced;
		}

		/// <summary>
		/// Builds a 'newobj' instruction creating an instance of the anonymous type
		/// <paramref name="type"/> with default property values; translating it yields
		/// object-initializer syntax, the only way to name the type in source code. Returns null
		/// if a property type involves an anonymous type other than by direct nesting (e.g. an
		/// array of anonymous type), because its default value expression would have to name it.
		/// </summary>
		NewObj? NewAnonymousTypeInstance(IType type)
		{
			var newObj = new NewObj(type.GetConstructors().Single());
			foreach (var parameter in newObj.Method.Parameters)
			{
				ILInstruction? argument = parameter.Type.IsAnonymousType()
					? NewAnonymousTypeInstance(parameter.Type)
					: parameter.Type.ContainsAnonymousType() ? null : new DefaultValue(parameter.Type);
				if (argument == null)
					return null;
				newObj.Arguments.Add(argument);
			}
			return newObj;
		}
		/// <summary>
		/// The steps only reach for type arguments as a last resort, but a method such as
		/// Enumerable.OfType&lt;TResult&gt;(IEnumerable) can never infer them from its arguments,
		/// and waiting leaves the expression full of casts that writing them would have made
		/// unnecessary. So they are written up front where inference cannot succeed. Casting
		/// the arguments takes that guess back, so casts still come first where they do help.
		/// </summary>
		void WriteTypeArgumentsInferenceCannotReach()
		{
			if (member is not IMethod method || method.TypeParameters.Count == 0)
				return;
			if ((allowed & ReferenceTransformation.RequireTypeArguments) == 0)
				return;
			if (IsPossibleExtensionMethodCallOnNull(method, Arguments.Arguments))
				return;
			if (CanInferTypeArgumentsFromArguments(method))
				return;
			if (expressionBuilder.settings.AnonymousTypes
				&& method.TypeArguments.Any(a => a.ContainsAnonymousType())
				&& PinTypesOfNullArguments()
				&& CanInferTypeArgumentsFromArguments(method))
			{
				// Anonymous types cannot be written as explicit type arguments; instead the
				// null arguments were rewritten so that all type arguments are inferable.
				return;
			}
			ApplyUpFront(ReferenceTransformation.RequireTypeArguments);
			TypeArgumentsWereAppliedUpFront = true;
		}

		/// <summary>
		/// Performs an escalation before the first lookup rather than waiting for its turn, for a case the caller already knows the short spelling cannot serve.
		/// Spends it either way.
		/// </summary>
		void ApplyUpFront(ReferenceTransformation step)
		{
			TryApply(step);
		}

		/// <summary>
		/// Escalates until the reference is unambiguous. Returns false once every escalation
		/// is spent and it still is not, leaving the caller to emit its best effort.
		/// Every escalation is spent by one use, which is what ends the loop.
		/// </summary>
		bool Run()
		{
			OverloadResolutionErrors errors;
			while ((errors = Probe()) != OverloadResolutionErrors.None)
			{
				if (!TryRepair(errors) && !Escalate())
					return false;
			}
			return true;
		}

		/// <summary>
		/// Answers one specific overload resolution failure with the escalation that addresses
		/// it, instead of taking the steps from the top. Returns false to fall back to the
		/// steps, either because the failure has no targeted answer or because the answer is
		/// already spent.
		/// </summary>
		bool TryRepair(OverloadResolutionErrors errors)
		{
			switch (errors)
			{
				case OverloadResolutionErrors.OutVarTypeMismatch:
					Debug.Assert(Arguments.UseImplicitlyTypedOut);
					Arguments.UseImplicitlyTypedOut = false;
					return true;
				case OverloadResolutionErrors.TypeInferenceFailed:
				case OverloadResolutionErrors.WrongNumberOfTypeArguments:
					return TryApply(ReferenceTransformation.RequireTypeArguments);
				case OverloadResolutionErrors.MissingArgumentForRequiredParameter:
					return TryApply(ReferenceTransformation.NoOptionalArgumentAllowed);
				default:
					return false;
			}
		}

		/// <summary>
		/// The target as the checks below must see it: null while the reference is still
		/// unqualified. Not the target to annotate a result with - an unqualified reference is
		/// still translated against <see cref="Target"/>.
		/// </summary>
		ResolveResult? LookupTarget => RequireTarget ? Target.ResolveResult : null;

		/// <summary>
		/// Re-resolves the current spelling. None means it binds back to <see cref="member"/>.
		/// </summary>
		OverloadResolutionErrors Probe()
		{
			switch (member)
			{
				case IField field:
					return ProbeField(field);
				// Invocation syntax first: a parameterized property's accessor is written as a
				// call, so IsAccessor must not claim it.
				case IMethod method when invocationSyntax:
					return ProbeCall(method);
				case IMethod { IsAccessor: true } accessor:
					// The check can resolve a member and still reject it, so the result is
					// what it returns, not whether it found something.
					bool unambiguous = IsUnambiguousAccess(expectedTargetDetails,
						LookupTarget, accessor, Arguments.GetArgumentResolveResultsDirect(),
						Arguments.GetArgumentNames(), out var foundAccessorOwner)
						&& OmittedArgumentsAreDefaultsOf(Arguments, foundAccessorOwner);
					FoundMember = foundAccessorOwner;
					return unambiguous
						? OverloadResolutionErrors.None
						: OverloadResolutionErrors.AmbiguousMatch;
				case IMethod method:
					return IsUnambiguousMethodReference(expectedTargetDetails, method, LookupTarget,
						TypeArguments, isExtensionMethodReference, out Result)
						? OverloadResolutionErrors.None
						: OverloadResolutionErrors.AmbiguousMatch;
				default:
					throw new NotSupportedException(member.SymbolKind.ToString());
			}
		}

		OverloadResolutionErrors ProbeField(IField field)
		{
			MemberResolveResult? result;
			if (LookupTarget == null)
			{
				result = resolver.ResolveSimpleName(field.Name, EmptyList<IType>.Instance,
					isInvocationTarget: false) as MemberResolveResult;
			}
			else
			{
				var lookup = CreateLookup(resolver);
				result = lookup.Lookup(Target.ResolveResult, field.Name, EmptyList<IType>.Instance,
					isInvocation: false) as MemberResolveResult;
			}
			Result = result;
			if (result == null || result.IsError || !result.Member.Equals(field, NormalizeTypeVisitor.TypeErasure))
				return OverloadResolutionErrors.AmbiguousMatch;
			return OverloadResolutionErrors.None;
		}

		/// <summary>
		/// Whether the arguments left out of the call are the default values of the member it
		/// resolves to. They were compared against the parameters of the method the call
		/// instruction names, which for a virtual call is the base declaration; an override may
		/// redeclare a different default, and then leaving the argument out changes the value that
		/// is passed.
		/// </summary>
		static bool OmittedArgumentsAreDefaultsOf(ArgumentList argumentList, IMember? foundMember)
		{
			int argumentCount = argumentList.Length;
			int omittedFrom = argumentList.GetActualArgumentCount();
			if (omittedFrom >= argumentCount)
				return true;
			if (foundMember is not IParameterizedMember foundParameterizedMember)
				return false;
			var parameters = foundParameterizedMember.Parameters;
			// Names may leave out a parameter in the middle, so what was dropped is found through
			// the map rather than by position. Its first entries are the target's.
			var map = argumentList.ArgumentToParameterMap;
			int firstParamIndex = map != null ? map.Count - argumentList.Length : 0;
			for (int i = omittedFrom; i < argumentCount; i++)
			{
				int parameterIndex = map != null ? map[i + firstParamIndex] : i;
				if (parameterIndex < 0 || parameterIndex >= parameters.Count)
					return false;
				if (!CallBuilder.IsOptionalArgument(parameters[parameterIndex], argumentList.Arguments[i]))
					return false;
			}
			return true;
		}

		OverloadResolutionErrors ProbeCall(IMethod method)
		{
			var errors = IsUnambiguousCall(expressionBuilder, expectedTargetDetails, method, LookupTarget,
				TypeArguments, Arguments.GetArgumentResolveResults().ToArray(),
				Arguments.GetArgumentNames(), out var foundMember,
				out bool bestCandidateIsExpandedForm);
			FoundMember = foundMember;
			if (errors != OverloadResolutionErrors.None)
				return errors;
			// Resolution succeeding does not make the spelling right. It can have reached the
			// method in the other of its normal and expanded form, or through omitted arguments
			// that are not the defaults the member found declares; neither has an error of its
			// own to report.
			if (bestCandidateIsExpandedForm == Arguments.IsExpandedForm
				&& OmittedArgumentsAreDefaultsOf(Arguments, foundMember))
			{
				return OverloadResolutionErrors.None;
			}
			// Both causes are corrected by writing the omitted arguments out again.
			return Arguments.FirstOptionalArgumentIndex >= 0
				? OverloadResolutionErrors.MissingArgumentForRequiredParameter
				: OverloadResolutionErrors.AmbiguousMatch;
		}

		/// <summary>
		/// Records an escalation the caller performed itself, or one that has nothing to do
		/// here, so that it will not be offered.
		/// </summary>
		void MarkApplied(ReferenceTransformation step)
		{
			Applied |= step;
			spent |= step;
		}

		/// <summary>
		/// Performs the first escalation still available, in the order this form of reference
		/// prefers them.
		/// </summary>
		bool Escalate()
		{
			// TODO : implement some more intelligent algorithm that decides which of these fixes (cast args, add target, cast target, add type args)
			// is best in this case. Additionally we should not cast all arguments at once, but step-by-step try to add only a minimal number of casts.
			foreach (ReferenceTransformation step in steps)
			{
				if (TryApply(step))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Performs one escalation, unless it is already spent, forbidden by the caller, or
		/// does not apply to this kind of reference.
		/// </summary>
		bool TryApply(ReferenceTransformation step)
		{
			if ((spent & step) != 0 || (allowed & step) == 0 || !Apply(step))
				return false;
			Applied |= step;
			spent |= step;
			return true;
		}

		/// <summary>
		/// Stage one: what the name binds to from this target, looked up the way this form of
		/// reference is written. A null target is the unqualified lookup, not the absence of one.
		/// </summary>
		static ResolveResult? LookUpName(ExpressionBuilder expressionBuilder, ResolveResult? target,
			string name, IReadOnlyList<IType> typeArguments, bool invocation)
		{
			CSharpResolver resolver = expressionBuilder.resolver;
			return target == null
				? resolver.ResolveSimpleName(name, typeArguments, isInvocationTarget: invocation)
				: CreateLookup(resolver).Lookup(target, name, typeArguments, isInvocation: invocation);
		}

		/// <summary>
		/// Stage three: did overload resolution settle on the member meant? An empty candidate
		/// set carries no error of its own - there is no best candidate to hold one - so it is
		/// reported here.
		/// </summary>
		static OverloadResolutionErrors CheckBestCandidate(OverloadResolution or,
			ExpectedTargetDetails expectedTargetDetails, IMember expected, out IParameterizedMember? found)
		{
			if (or.BestCandidateErrors != OverloadResolutionErrors.None)
			{
				found = null;
				return or.BestCandidateErrors;
			}
			if (or.IsAmbiguous)
			{
				found = null;
				return OverloadResolutionErrors.AmbiguousMatch;
			}
			found = or.GetBestCandidateWithSubstitutedTypeArguments();
			if (found == null)
				return OverloadResolutionErrors.AmbiguousMatch;
			return IsAppropriateCallTarget(expectedTargetDetails, expected, found)
				? OverloadResolutionErrors.None
				: OverloadResolutionErrors.AmbiguousMatch;
		}

		static MemberLookup CreateLookup(CSharpResolver resolver)
		{
			return new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentModule);
		}

		/// <summary>
		/// Overload resolution as the ambiguity checks need it: over the compilation the
		/// reference is written in, and with its conversions, which decide whether an argument
		/// fits a parameter at all.
		/// </summary>
		static OverloadResolution CreateOverloadResolution(CSharpResolver resolver,
			IReadOnlyList<ResolveResult> arguments, string[]? argumentNames, IType[] typeArguments)
		{
			return new OverloadResolution(resolver.Compilation, arguments.ToArray(), argumentNames,
				typeArguments, conversions: resolver.conversions);
		}

		internal static OverloadResolutionErrors IsUnambiguousCall(ExpressionBuilder expressionBuilder,
			ExpectedTargetDetails expectedTargetDetails, IMethod method,
			ResolveResult? target, IType[] typeArguments, ResolveResult[] arguments,
			string[]? argumentNames,
			out IParameterizedMember? foundMember, out bool bestCandidateIsExpandedForm)
		{
			CSharpResolver resolver = expressionBuilder.resolver;
			foundMember = null;
			bestCandidateIsExpandedForm = false;
			var lookup = CreateLookup(resolver);

			Log.WriteLine("IsUnambiguousCall: Performing overload resolution for " + method);
			Log.WriteCollection("  Arguments: ", arguments);

			var or = CreateOverloadResolution(resolver, arguments, argumentNames, typeArguments);
			if (expectedTargetDetails.CallOpCode == OpCode.NewObj)
			{
				foreach (IMethod ctor in method.DeclaringType.GetConstructors())
				{
					bool allowProtectedAccess =
						resolver.CurrentTypeDefinition == method.DeclaringTypeDefinition;
					if (lookup.IsAccessible(ctor, allowProtectedAccess))
					{
						Log.Indent();
						OverloadResolutionErrors errors = or.AddCandidate(ctor);
						Log.Unindent();
						or.LogCandidateAddingResult("  Candidate", ctor, errors);
					}
				}
			}
			else if (method.IsOperator)
			{
				IEnumerable<IParameterizedMember> operatorCandidates;
				if (arguments.Length == 1)
				{
					IType argType = NullableType.GetUnderlyingType(arguments[0].Type);
					operatorCandidates = resolver.GetUserDefinedOperatorCandidates(argType, method.Name);
					if (method.Name == "op_Explicit")
					{
						// For casts, also consider candidates from the target type we are casting to.
						var hashSet = new HashSet<IParameterizedMember>(operatorCandidates);
						IType targetType = NullableType.GetUnderlyingType(method.ReturnType);
						hashSet.UnionWith(
							resolver.GetUserDefinedOperatorCandidates(targetType, method.Name)
						);
						operatorCandidates = hashSet;
					}
				}
				else if (arguments.Length == 2)
				{
					IType lhsType = NullableType.GetUnderlyingType(arguments[0].Type);
					IType rhsType = NullableType.GetUnderlyingType(arguments[1].Type);
					var hashSet = new HashSet<IParameterizedMember>();
					hashSet.UnionWith(resolver.GetUserDefinedOperatorCandidates(lhsType, method.Name));
					hashSet.UnionWith(resolver.GetUserDefinedOperatorCandidates(rhsType, method.Name));
					operatorCandidates = hashSet;
				}
				else
				{
					operatorCandidates = EmptyList<IParameterizedMember>.Instance;
				}
				foreach (var m in operatorCandidates)
				{
					or.AddCandidate(m);
				}
			}
			else
			{
				if (LookUpName(expressionBuilder, target, method.Name, typeArguments, invocation: true)
					is not MethodGroupResolveResult methodGroup)
					return OverloadResolutionErrors.AmbiguousMatch;
				or.AddMethodLists(methodGroup.MethodsGroupedByDeclaringType.ToArray());
			}
			bestCandidateIsExpandedForm = or.BestCandidateIsExpandedForm;
			var resolutionErrors = CheckBestCandidate(or, expectedTargetDetails, method, out foundMember);
			if (resolutionErrors != OverloadResolutionErrors.None)
				return resolutionErrors;
			// Reporting no error means a candidate was found.
			Debug.Assert(foundMember != null);
			var map = or.GetArgumentToParameterMap();
			for (int i = 0; i < arguments.Length; i++)
			{
				ResolveResult arg = arguments[i];
				int parameterIndex = map[i];
				if (arg is OutVarResolveResult rr && parameterIndex >= 0)
				{
					var param = foundMember.Parameters[parameterIndex];
					var paramType = param.Type.UnwrapByRef();
					if (!paramType.Equals(rr.OriginalVariableType))
						return OverloadResolutionErrors.OutVarTypeMismatch;
				}
			}

			return OverloadResolutionErrors.None;
		}

		bool IsUnambiguousAccess(ExpectedTargetDetails expectedTargetDetails, ResolveResult? target, IMethod method,
			IList<ResolveResult> arguments, string[]? argumentNames, [NotNullWhen(true)] out IMember? foundMember)
		{
			Log.WriteLine("IsUnambiguousAccess: Performing overload resolution for " + method);
			Log.WriteCollection("  Arguments: ", arguments);

			IMember accessorOwner = method.AccessorOwner!;
			// An indexer has no name to look up, so its candidates come from the indexer list and
			// overload resolution picks among them; everything else binds by name.
			if (target != null && accessorOwner.SymbolKind == SymbolKind.Indexer)
			{
				var or = CreateOverloadResolution(resolver, arguments.ToArray(),
					argumentNames, Empty<IType>.Array);
				or.AddMethodLists(CreateLookup(resolver).LookupIndexers(target));
				var errors = CheckBestCandidate(or, expectedTargetDetails, accessorOwner, out var best);
				foundMember = best;
				return errors == OverloadResolutionErrors.None;
			}
			if (LookUpName(expressionBuilder, target, accessorOwner.Name, EmptyList<IType>.Instance,
					invocation: false) is not MemberResolveResult { IsError: false } resolved)
			{
				foundMember = null;
				return false;
			}
			foundMember = resolved.Member;
			return IsAppropriateCallTarget(expectedTargetDetails, accessorOwner, foundMember);
		}

		bool IsUnambiguousMethodReference(ExpectedTargetDetails expectedTargetDetails, IMethod method, ResolveResult? target, IReadOnlyList<IType> typeArguments, bool isExtensionMethodReference, [NotNullWhen(true)] out ResolveResult? result)
		{
			Log.WriteLine("IsUnambiguousMethodReference: Performing overload resolution for " + method);

			var lookup = CreateLookup(resolver);
			OverloadResolution or;

			if (isExtensionMethodReference)
			{
				result = resolver.ResolveMemberAccess(target, method.Name, typeArguments, NameLookupMode.InvocationTarget) as MethodGroupResolveResult;
				if (result == null)
					return false;
				// The receiver is the target, not an argument: the delegate being built has one
				// parameter fewer than the method, so passing the receiver's parameter too leaves
				// overload resolution with one argument too many and it reports every candidate
				// ambiguous.
				or = ((MethodGroupResolveResult)result).PerformOverloadResolution(resolver.CurrentTypeResolveContext.Compilation,
					method.Parameters.Skip(1).Select(p => (ResolveResult)new TypeResolveResult(p.Type)).ToArray(),
					argumentNames: null, allowExtensionMethods: true);
				if (or == null || or.IsAmbiguous)
					return false;
			}
			else
			{
				// There are no arguments; the parameter types stand in for them, and argument
				// names are not possible.
				or = CreateOverloadResolution(resolver,
					method.Parameters.SelectReadOnlyArray(p => new TypeResolveResult(p.Type)),
					argumentNames: null, typeArguments.ToArray());
				result = LookUpName(expressionBuilder, target, method.Name, typeArguments, invocation: false);
				if (result is not MethodGroupResolveResult methodGroup)
					return false;
				or.AddMethodLists(methodGroup.MethodsGroupedByDeclaringType.ToArray());
			}

			// Deliberately not CheckBestCandidate: unlike the other two checks this one does not
			// reject BestCandidateErrors or an ambiguous result, and a method group carries no
			// arguments to be ambiguous over. Whether that holds for the type arguments too is
			// untested, so it is left as it was rather than tightened blind.
			var foundMethod = or.GetBestCandidateWithSubstitutedTypeArguments();
			if (!IsAppropriateCallTarget(expectedTargetDetails, method, foundMethod))
				return false;
			return result is MethodGroupResolveResult;
		}

		/// <summary>
		/// Checks whether calling `target.methodName()` will use `expected` as the method to invoke.
		/// </summary>
		internal static bool CheckSimpleCall(ExpressionBuilder expressionBuilder, ResolveResult target,
			IMethod expected, OpCode expectedCallOpCode = OpCode.Call)
		{
			var details = new ExpectedTargetDetails { CallOpCode = expectedCallOpCode, NeedsBoxingConversion = false };
			CSharpResolver resolver = expressionBuilder.resolver;
			if (resolver.ResolveMemberAccess(target, expected.Name, [], NameLookupMode.InvocationTarget)
			is not MethodGroupResolveResult mgrr)
				return false;
			var or = mgrr.PerformOverloadResolution(resolver.Compilation, []);
			if (or.BestCandidateErrors != OverloadResolutionErrors.None || or.IsAmbiguous)
				return false;
			return IsAppropriateCallTarget(details, expected, or.GetBestCandidateWithSubstitutedTypeArguments()!);
		}

		internal static bool IsAppropriateCallTarget(ExpectedTargetDetails expectedTargetDetails, IMember expectedTarget, IMember actualTarget)
		{
			if (expectedTarget.Equals(actualTarget, NormalizeTypeVisitor.TypeErasure))
				return true;

			if (expectedTargetDetails.CallOpCode == OpCode.CallVirt && actualTarget.IsOverride)
			{
				if (expectedTargetDetails.NeedsBoxingConversion && actualTarget.DeclaringType.IsReferenceType != true)
					return false;
				foreach (var possibleTarget in InheritanceHelper.GetBaseMembers(actualTarget, false))
				{
					if (expectedTarget.Equals(possibleTarget, NormalizeTypeVisitor.TypeErasure))
						return true;
					if (!possibleTarget.IsOverride)
						break;
				}
			}
			return false;
		}

		void ModifyReturnTypeOfLambda(LambdaExpression lambda)
		{
			var resolveResult = (DecompiledLambdaResolveResult)lambda.GetResolveResult();
			if (lambda.Body is Expression exprBody)
				lambda.Body = new TranslatedExpression(exprBody.Detach()).ConvertTo(resolveResult.ReturnType, expressionBuilder);
			else
				ModifyReturnStatementInsideLambda(resolveResult.ReturnType, lambda);
			resolveResult.InferredReturnType = resolveResult.ReturnType;
		}

		void CastArguments(IList<TranslatedExpression> arguments, IList<IParameter> expectedParameters)
		{
			for (int i = 0; i < arguments.Count; i++)
			{
				if (expressionBuilder.settings.AnonymousTypes && expectedParameters[i].Type.ContainsAnonymousType())
				{
					if (arguments[i].Expression is LambdaExpression lambda)
					{
						ModifyReturnTypeOfLambda(lambda);
					}
				}
				else
				{
					IParameter parameter = expectedParameters[i];
					IType parameterType;
					if (parameter.Type.Kind == TypeKind.Dynamic)
					{
						parameterType = expressionBuilder.compilation.FindType(KnownTypeCode.Object);
					}
					else
					{
						parameterType = parameter.Type;
					}

					if (parameter.ReferenceKind == ReferenceKind.In && parameterType is ByReferenceType brt && arguments[i].Type is not ByReferenceType)
					{
						parameterType = brt.ElementType;
					}

					arguments[i] = arguments[i].ConvertTo(parameterType, expressionBuilder, allowImplicitConversion: false);
				}
			}
		}

		void EnforceExplicitIn(TranslatedExpression[] arguments, IParameter[] expectedParameters)
		{
			for (int i = 0; i < arguments.Length; i++)
			{
				if (expectedParameters[i].ReferenceKind != ReferenceKind.In)
					continue;
				if (arguments[i].Expression is DirectionExpression)
					continue;

				arguments[i] = WrapInAsRefReadOnly(arguments[i]);
				expressionBuilder.statementBuilder.EmitAsRefReadOnly = true;
			}
		}

		TranslatedExpression WrapInAsRefReadOnly(TranslatedExpression arg)
		{
			return new DirectionExpression(
				FieldDirection.In,
				new InvocationExpression {
					Target = new IdentifierExpression("ILSpyHelper_AsRefReadOnly"),
					Arguments = { arg.Expression }
				}
			).WithRR(new ByReferenceResolveResult(arg.Type, ReferenceKind.In))
			.WithoutILInstruction();
		}

		void ModifyReturnStatementInsideLambda(IType returnType, AstNode parent)
		{
			foreach (var child in parent.Children)
			{
				if (child is LambdaExpression || child is AnonymousMethodExpression)
					continue;
				if (child is ReturnStatement ret)
				{
					if (ret.Expression is not null)
						ret.Expression = new TranslatedExpression(ret.Expression.Detach()).ConvertTo(returnType, expressionBuilder);
					continue;
				}
				ModifyReturnStatementInsideLambda(returnType, child);
			}
		}

		bool Apply(ReferenceTransformation step)
		{
			switch (step)
			{
				case ReferenceTransformation.RequireTarget:
					return true; // recording it in Applied is the whole effect
				case ReferenceTransformation.CastTarget:
					if (skipTargetCast && RequireTarget != initiallyRequiredTarget)
					{
						// A protected member of a base type cannot be reached through a cast
						// target, so the qualifier the previous step added is worse than useless:
						// drop it again and let the remaining escalations do the work. It stays
						// spent, so it is not offered a second time.
						Applied &= ~ReferenceTransformation.RequireTarget;
						return true;
					}
					Target = Target.ConvertTo(declaringType, expressionBuilder);
					return true;
				case ReferenceTransformation.RequireTypeArguments:
					if (member is not IMethod method)
						return false;
					TypeArguments = method.TypeArguments.ToArray();
					return true;
				case ReferenceTransformation.NoNamedArgsForPrettiness:
					if (!Arguments.AddNamesToPrimitiveValues)
						return false;
					Arguments.AddNamesToPrimitiveValues = false;
					return true;
				case ReferenceTransformation.NoOptionalArgumentAllowed:
					if (Arguments.FirstOptionalArgumentIndex < 0)
						return false;
					Arguments.FirstOptionalArgumentIndex = -1;
					return true;
				case ReferenceTransformation.CastArguments:
					if (TypeArgumentsWereAppliedUpFront)
					{
						// The guess did not help, so take it back completely: a later step may
						// still reach for it once the casts are in.
						TypeArgumentsWereAppliedUpFront = false;
						Applied &= ~ReferenceTransformation.RequireTypeArguments;
						spent &= ~ReferenceTransformation.RequireTypeArguments;
						TypeArguments = Empty<IType>.Array;
					}
					if (castingArgumentsDropsImplicitlyTypedOut)
					{
						Arguments.UseImplicitlyTypedOut = false;
					}
					// Every step list places NoOptionalArgumentAllowed before this step, so by the
					// time the casts go on no argument is left out and the whole list is cast.
					CastArguments(Arguments.Arguments, Arguments.ExpectedParameters);
					return true;
				case ReferenceTransformation.EnforceExplicitIn:
					EnforceExplicitIn(Arguments.Arguments, Arguments.ExpectedParameters);
					return true;
				default:
					return false;
			}
		}
	}
}
