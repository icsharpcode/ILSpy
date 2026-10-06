// Copyright (c) 2026 Piero Viano
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
using System.Linq;
using System.Threading;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Analyzers;

namespace ICSharpCode.ILSpy.Navigation
{
	/// <summary>The relation a go-to command follows from a symbol.</summary>
	public enum GoToKind
	{
		/// <summary>The symbol's own definition.</summary>
		Declaration,
		/// <summary>Non-abstract types / members that implement or override the symbol, at any depth.</summary>
		Implementation,
		/// <summary>Direct base types, or the members the symbol overrides or implements.</summary>
		BaseSymbols,
		/// <summary>Direct inheritors: derived / implementing types, or the members that directly override or implement the symbol.</summary>
		DerivedSymbols,
	}

	/// <summary>
	/// Resolves the inheritance relations behind the go-to commands. Inheritors are searched over
	/// the same module scope the analyzers use (<see cref="AnalyzerScope"/>): every loaded module
	/// that can see the symbol. Entities from different type systems are matched by metadata token
	/// and module, since each scanned module gets its own compilation.
	/// </summary>
	public static class SymbolHierarchy
	{
		/// <summary>True when <paramref name="kind"/> can produce a result for <paramref name="entity"/>.</summary>
		public static bool IsApplicable(GoToKind kind, IEntity entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			return kind switch {
				GoToKind.Declaration => true,
				GoToKind.Implementation or GoToKind.DerivedSymbols => CanHaveInheritors(entity),
				GoToKind.BaseSymbols => FindBaseSymbols(entity).Count > 0,
				_ => false,
			};
		}

		/// <summary>True for interfaces, non-sealed classes, and overridable or interface members.</summary>
		public static bool CanHaveInheritors(IEntity entity)
		{
			switch (entity)
			{
				case ITypeDefinition type:
					return type.Kind == TypeKind.Interface || (type.Kind == TypeKind.Class && !type.IsSealed);
				case IMember member when IsInheritableMember(member):
					return member.IsOverridable
						|| (member.DeclaringTypeDefinition?.Kind == TypeKind.Interface && (member.IsAbstract || member.IsVirtual));
				default:
					return false;
			}
		}

		/// <summary>Finds the targets of <paramref name="kind"/> for <paramref name="entity"/>, sorted by full name.</summary>
		public static IReadOnlyList<IEntity> Find(GoToKind kind, IEntity entity, AssemblyList assemblyList, CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(entity);
			ArgumentNullException.ThrowIfNull(assemblyList);
			IEnumerable<IEntity> result;
			switch (kind)
			{
				case GoToKind.Declaration:
					return new[] { entity };
				case GoToKind.BaseSymbols:
					result = FindBaseSymbols(entity);
					break;
				case GoToKind.DerivedSymbols:
					result = CanHaveInheritors(entity)
						? FindInheritors(entity, assemblyList, cancellationToken).Where(i => i.IsDirect).Select(i => i.Entity)
						: Enumerable.Empty<IEntity>();
					break;
				case GoToKind.Implementation:
					result = CanHaveInheritors(entity)
						? FindInheritors(entity, assemblyList, cancellationToken).Select(i => i.Entity).Where(IsImplementation)
						: Enumerable.Empty<IEntity>();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(kind));
			}
			return result.OrderBy(e => e.FullName, StringComparer.Ordinal).ToList();
		}

		/// <summary>Direct base types of a type, or the members a member overrides or implements.</summary>
		public static IReadOnlyList<IEntity> FindBaseSymbols(IEntity entity)
		{
			IEnumerable<IEntity> candidates = entity switch {
				// The type system reports System.Object as a base of every interface; an interface's
				// base symbols are only the interfaces it extends.
				ITypeDefinition type => type.DirectBaseTypes.Select(t => t.GetDefinition())
					.Where(d => d != null && (type.Kind != TypeKind.Interface || d.Kind == TypeKind.Interface))
					.OfType<IEntity>(),
				IMember member when IsInheritableMember(member)
					=> InheritanceHelper.GetBaseMembers(member, includeImplementedInterfaces: true).Select(m => m.MemberDefinition),
				_ => Enumerable.Empty<IEntity>(),
			};
			var result = new List<IEntity>();
			foreach (var candidate in candidates)
			{
				if (!IsSameDefinition(candidate, entity) && !result.Any(r => IsSameDefinition(r, candidate)))
					result.Add(candidate);
			}
			return result;
		}

		/// <summary>True when both entities denote the same metadata definition, whatever compilation they come from.</summary>
		public static bool IsSameDefinition(IEntity a, IEntity b)
		{
			if (ReferenceEquals(a, b))
				return true;
			return !a.MetadataToken.IsNil
				&& a.MetadataToken == b.MetadataToken
				&& a.ParentModule?.MetadataFile is { } module
				&& ReferenceEquals(module, b.ParentModule?.MetadataFile);
		}

		static bool IsInheritableMember(IMember member)
			=> (member.SymbolKind is SymbolKind.Method or SymbolKind.Property or SymbolKind.Indexer or SymbolKind.Event)
				&& (!member.IsStatic || (member.DeclaringTypeDefinition?.Kind == TypeKind.Interface && member.IsAbstract));

		static bool IsImplementation(IEntity entity) => entity switch {
			ITypeDefinition type => type.Kind != TypeKind.Interface && !type.IsAbstract,
			IMember member => !member.IsAbstract,
			_ => false,
		};

		readonly record struct Inheritor(IEntity Entity, bool IsDirect);

		static List<Inheritor> FindInheritors(IEntity entity, AssemblyList assemblyList, CancellationToken cancellationToken)
		{
			return entity switch {
				ITypeDefinition type => FindDerivedTypes(type, assemblyList, cancellationToken),
				IMember member => FindOverridingMembers(member, assemblyList, cancellationToken),
				_ => new List<Inheritor>(),
			};
		}

		static List<Inheritor> FindDerivedTypes(ITypeDefinition type, AssemblyList assemblyList, CancellationToken cancellationToken)
		{
			var result = new List<Inheritor>();
			var scope = new AnalyzerScope(assemblyList, type);
			foreach (var candidate in scope.GetTypesInScope(cancellationToken))
			{
				cancellationToken.ThrowIfCancellationRequested();
				if (IsSameDefinition(candidate, type) || result.Any(r => IsSameDefinition(r.Entity, candidate)))
					continue;
				if (!candidate.GetAllBaseTypeDefinitions().Any(b => IsSameDefinition(b, type)))
					continue;
				bool isDirect = candidate.DirectBaseTypes.Any(b => b.GetDefinition() is { } d && IsSameDefinition(d, type));
				result.Add(new Inheritor(candidate, isDirect));
			}
			return result;
		}

		static List<Inheritor> FindOverridingMembers(IMember member, AssemblyList assemblyList, CancellationToken cancellationToken)
		{
			var declaringType = member.DeclaringTypeDefinition;
			if (declaringType == null)
				return new List<Inheritor>();
			var found = new List<(IMember Member, List<IMember> Bases)>();
			var scope = new AnalyzerScope(assemblyList, member);
			foreach (var candidateType in scope.GetTypesInScope(cancellationToken))
			{
				cancellationToken.ThrowIfCancellationRequested();
				if (IsSameDefinition(candidateType, declaringType))
					continue;
				if (!candidateType.GetAllBaseTypeDefinitions().Any(b => IsSameDefinition(b, declaringType)))
					continue;
				foreach (var candidate in candidateType.Members)
				{
					if (candidate.SymbolKind != member.SymbolKind)
						continue;
					var bases = InheritanceHelper.GetBaseMembers(candidate, includeImplementedInterfaces: true).ToList();
					if (bases.Any(b => IsSameDefinition(b, member)) && !found.Any(f => IsSameDefinition(f.Member, candidate)))
						found.Add((candidate, bases));
				}
			}
			// A member is a direct inheritor unless one of the members it overrides is itself an
			// inheritor of the target (e.g. an override of an abstract member that implements the
			// interface member being searched).
			return found
				.Select(f => new Inheritor(f.Member,
					!f.Bases.Any(b => found.Any(other => IsSameDefinition(other.Member, b)))))
				.ToList();
		}
	}
}
