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

#nullable enable

using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp.Resolver
{
	/// <summary>
	/// Marks a method group or anonymous function whose C# natural type equals the delegate type
	/// the IL constructs, i.e. the site can be emitted without naming that type: without an
	/// explicit delegate creation, or without a cast.
	/// </summary>
	sealed class NaturalTypeAnnotation
	{
		public IType DelegateType { get; }

		public NaturalTypeAnnotation(IType delegateType)
		{
			DelegateType = delegateType;
		}
	}

	/// <summary>
	/// Decides whether the C# natural type of a method group expression equals a given delegate
	/// type. Only when it does may the decompiler drop the explicit delegate construction
	/// ('var f = M;' instead of 'Action f = new Action(M);') without changing what the
	/// re-compiled code binds to.
	/// </summary>
	static class MethodGroupNaturalType
	{
		/// <summary>
		/// Determines whether the method group written as
		/// '<paramref name="target"/>.<paramref name="method"/>.Name&lt;<paramref name="typeArguments"/>&gt;'
		/// has a natural type equal to <paramref name="delegateType"/>.
		/// </summary>
		/// <param name="resolver">Resolver positioned at the decompiled member (provides using scopes).</param>
		/// <param name="target">Resolve result of the receiver, or null if the group is a simple name.</param>
		/// <param name="method">The method the IL delegate targets.</param>
		/// <param name="typeArguments">Type arguments as spelled in the emitted form (empty if omitted).</param>
		/// <param name="delegateType">The delegate type constructed in the IL.</param>
		/// <param name="scopeByScope">
		/// true for the C# 13 rules: scopes are considered one at a time and candidates that cannot
		/// be invoked (wrong arity, violated constraints, static/instance mismatch) are pruned; the
		/// first scope with surviving candidates decides. false for the C# 10 rules, where every
		/// candidate in every scope takes part and no pruning happens.
		/// </param>
		public static bool Matches(CSharpResolver resolver, ResolveResult? target, IMethod method,
			IReadOnlyList<IType> typeArguments, IType delegateType, bool scopeByScope)
		{
			IMethod? invoke = delegateType.GetDelegateInvokeMethod();
			if (invoke == null)
				return false;
			if (!IsInferrableDelegateType(delegateType, invoke))
				return false;
			// Resolve without the spelled type arguments: member lookup would otherwise already
			// filter out candidates of a different arity, but those candidates take part in the
			// natural type determination (they kill it below C# 13, and count as pruned in C# 13).
			MethodGroupResolveResult? mgrr;
			if (target == null)
			{
				mgrr = resolver.ResolveSimpleName(method.Name, EmptyList<IType>.Instance) as MethodGroupResolveResult;
			}
			else
			{
				mgrr = resolver.ResolveMemberAccess(target, method.Name, EmptyList<IType>.Instance,
					NameLookupMode.InvocationTarget) as MethodGroupResolveResult;
			}
			if (mgrr == null)
				return false;
			// Extension scopes only apply to an instance-form receiver.
			bool hasExtensionScopes = target != null && target is not TypeResolveResult;
			if (scopeByScope)
			{
				var memberScope = PruneMemberScope(mgrr.Methods, target, typeArguments, out _);
				if (memberScope.Count > 0)
					return ScopeDecides(memberScope, skippedParameters: 0, invoke, method);
				if (!hasExtensionScopes)
					return false;
				foreach (var scope in mgrr.GetExtensionMethods())
				{
					var survivors = PruneExtensionScope(scope, mgrr.TargetType, typeArguments, out _);
					if (survivors.Count > 0)
						return ScopeDecides(survivors, skippedParameters: 1, invoke, method);
				}
				return false;
			}
			else
			{
				// C# 10: all candidates from all scopes must agree on one signature. A candidate
				// the C# 13 rules would prune still takes part here, so its mere existence makes
				// the natural type undeterminable for our purposes - be conservative and keep the
				// explicit construction.
				var members = PruneMemberScope(mgrr.Methods, target, typeArguments, out bool anyPruned);
				if (anyPruned)
					return false;
				if (!ScopeDecides(members, skippedParameters: 0, invoke, method, requireTargetMethod: false))
					return false;
				bool foundTargetMethod = ContainsTargetMethod(members, method);
				if (hasExtensionScopes)
				{
					foreach (var scope in mgrr.GetExtensionMethods())
					{
						var extensions = PruneExtensionScope(scope, mgrr.TargetType, typeArguments, out anyPruned);
						if (anyPruned)
							return false;
						if (!ScopeDecides(extensions, skippedParameters: 1, invoke, method, requireTargetMethod: false))
							return false;
						foundTargetMethod |= ContainsTargetMethod(extensions, method);
					}
				}
				return foundTargetMethod;
			}
		}

		/// <summary>
		/// Applies the C# 13 candidate pruning to the member scope: static/instance mismatch with
		/// the receiver form, generic arity mismatch with explicitly given type arguments, and
		/// violated constraints. Candidates are specialized with the explicit type arguments on
		/// the way.
		/// </summary>
		static List<IMethod> PruneMemberScope(IEnumerable<IMethod> candidates, ResolveResult? target,
			IReadOnlyList<IType> typeArguments, out bool anyPruned)
		{
			var survivors = new List<IMethod>();
			anyPruned = false;
			foreach (var candidate in candidates)
			{
				if (target is TypeResolveResult ? !candidate.IsStatic : target != null && candidate.IsStatic)
				{
					anyPruned = true;
					continue;
				}
				if (!TrySpecialize(candidate, typeArguments, out var m))
				{
					anyPruned = true;
					continue;
				}
				survivors.Add(m);
			}
			return survivors;
		}

		/// <summary>
		/// Applies the C# 13 candidate pruning to one extension scope. Candidates whose this
		/// parameter does not accept the receiver are not part of the method group at all, so
		/// they do not count as pruned.
		/// </summary>
		static List<IMethod> PruneExtensionScope(IEnumerable<IMethod> candidates, IType targetType,
			IReadOnlyList<IType> typeArguments, out bool anyPruned)
		{
			var survivors = new List<IMethod>();
			anyPruned = false;
			foreach (var candidate in candidates)
			{
				if (typeArguments.Count > 0)
				{
					if (!TrySpecialize(candidate, typeArguments, out var m))
					{
						anyPruned = true;
						continue;
					}
					if (!CSharpResolver.IsEligibleExtensionMethod(targetType, m, useTypeInference: false, out _))
						continue;
					survivors.Add(m);
				}
				else
				{
					if (!CSharpResolver.IsEligibleExtensionMethod(targetType, candidate, useTypeInference: true, out var inferredTypes))
						continue;
					var m = inferredTypes != null
						? candidate.Specialize(new TypeParameterSubstitution(null, inferredTypes))
						: candidate;
					survivors.Add(m);
				}
			}
			return survivors;
		}

		/// <summary>
		/// Specializes a candidate with the explicitly given type arguments; fails on arity
		/// mismatch or violated constraints (the C# 13 pruning conditions).
		/// </summary>
		static bool TrySpecialize(IMethod candidate, IReadOnlyList<IType> typeArguments, out IMethod result)
		{
			result = candidate;
			if (typeArguments.Count == 0)
				return true;
			var definition = (IMethod)candidate.MemberDefinition;
			if (definition.TypeParameters.Count != typeArguments.Count)
				return false;
			// Member lookup hands out self-instantiated candidates (their own type parameters as
			// type arguments), so apply the explicit arguments unconditionally.
			result = result.Specialize(new TypeParameterSubstitution(null, typeArguments));
			var substitution = result.Substitution;
			for (int i = 0; i < result.TypeArguments.Count; i++)
			{
				if (!OverloadResolution.ValidateConstraints(definition.TypeParameters[i], result.TypeArguments[i], substitution))
					return false;
			}
			return true;
		}

		/// <summary>
		/// The scope that decides the natural type does so if all its candidates share the
		/// delegate's Invoke signature and the IL's target method is among them.
		/// </summary>
		static bool ScopeDecides(List<IMethod> candidates, int skippedParameters, IMethod invoke,
			IMethod targetMethod, bool requireTargetMethod = true)
		{
			foreach (var candidate in candidates)
			{
				if (!SignatureMatchesInvoke(candidate, skippedParameters, invoke))
					return false;
			}
			return !requireTargetMethod || ContainsTargetMethod(candidates, targetMethod);
		}

		static bool ContainsTargetMethod(List<IMethod> candidates, IMethod targetMethod)
		{
			return candidates.Any(c => c.MemberDefinition.Equals(targetMethod.MemberDefinition));
		}

		static bool SignatureMatchesInvoke(IMethod method, int skippedParameters, IMethod invoke)
		{
			if (method.TypeParameters.Count > 0 && method.TypeArguments.Count == 0)
			{
				// An uninstantiated generic method has no signature the natural type could use,
				// even when its type parameters do not occur in the parameter list.
				return false;
			}
			var parameters = method.Parameters;
			if (parameters.Count - skippedParameters != invoke.Parameters.Count)
				return false;
			var normalize = NormalizeTypeVisitor.IgnoreNullabilityAndTuples;
			for (int i = 0; i < invoke.Parameters.Count; i++)
			{
				var p = parameters[i + skippedParameters];
				var q = invoke.Parameters[i];
				if (p.ReferenceKind != q.ReferenceKind || p.IsParams != q.IsParams || p.IsOptional != q.IsOptional)
					return false;
				if (p.IsOptional && !Equals(p.GetConstantValue(), q.GetConstantValue()))
					return false;
				if (!normalize.EquivalentTypes(p.Type, q.Type))
					return false;
			}
			if (method.ReturnTypeIsRefReadOnly != invoke.ReturnTypeIsRefReadOnly)
				return false;
			return normalize.EquivalentTypes(method.ReturnType, invoke.ReturnType);
		}

		/// <summary>
		/// C# only ever infers System.Action/System.Func delegate types (for plain signatures)
		/// or compiler-synthesized anonymous delegate types (for everything else) as the natural
		/// type; a signature-compatible custom delegate type never round-trips through 'var'.
		/// </summary>
		internal static bool IsInferrableDelegateType(IType delegateType, IMethod invoke)
		{
			if (delegateType.IsAnonymousDelegate())
				return true;
			if (invoke.Parameters.Any(p => p.ReferenceKind != ReferenceKind.None || p.IsParams || p.IsOptional))
				return false;
			if (invoke.ReturnType.Kind == TypeKind.ByReference)
				return false;
			var definition = delegateType.GetDefinition();
			if (definition == null || definition.Namespace != "System")
				return false;
			if (invoke.ReturnType.IsKnownType(KnownTypeCode.Void))
				return definition.Name == "Action" && definition.TypeParameterCount == invoke.Parameters.Count;
			return definition.Name == "Func" && definition.TypeParameterCount == invoke.Parameters.Count + 1;
		}
	}
}
