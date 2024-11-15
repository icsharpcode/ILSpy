// Copyright (c) 2024 Holger Schmidt
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

using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.MermaidDiagrammer.Extensions;

namespace ICSharpCode.ILSpyX.MermaidDiagrammer
{
	using CD = ClassDiagrammer;

	partial class ClassDiagrammerFactory
	{
		private IProperty[] GetHasOneRelations(IProperty[] properties) => properties.Where(property => {
			IType type = property.ReturnType;

			if (type.TryGetNullableType(out var typeArg))
				type = typeArg;

			return selectedTypes!.Contains(type);
		}).ToArray();

		private (IProperty property, IType elementType)[] GetManyRelations(IProperty[] properties)
			=> properties.Select(property => {
				IType elementType = property.ReturnType.GetElementTypeFromIEnumerable(property.Compilation, true, out bool? isGeneric);

				if (isGeneric == false && elementType.IsObject())
				{
					IProperty[] indexers = property.ReturnType.GetProperties(
						p => p.IsIndexer && !p.ReturnType.IsObject(),
						GetMemberOptions.IgnoreInheritedMembers).ToArray(); // TODO mayb order by declaring type instead of filtering

					if (indexers.Length > 0)
						elementType = indexers[0].ReturnType;
				}

				return isGeneric == true && selectedTypes!.Contains(elementType) ? (property, elementType) : default;
			}).Where(pair => pair != default).ToArray();

		/// <summary>Returns the relevant direct super type the <paramref name="type"/> inherits from
		/// in a format matching <see cref="CD.Type.BaseType"/>.</summary>
		private Dictionary<string, string?>? GetBaseType(IType type)
		{
			IType? relevantBaseType = type.DirectBaseTypes.SingleOrDefault(t => !t.IsInterface() && !t.IsObject());
			return relevantBaseType == null ? default : new[] { BuildRelationship(relevantBaseType) }.ToDictionary(r => r.to, r => r.label);
		}

		/// <summary>Returns the direct interfaces implemented by <paramref name="type"/>
		/// in a format matching <see cref="CD.Type.Interfaces"/>.</summary>
		private Dictionary<string, string?[]>? GetInterfaces(ITypeDefinition type)
		{
			var interfaces = type.DirectBaseTypes.Where(t => t.IsInterface()).ToArray();

			return interfaces.Length == 0 ? null
				: interfaces.Select(i => BuildRelationship(i)).GroupBy(r => r.to)
					.ToDictionary(g => g.Key, g => g.Select(r => r.label).ToArray());
		}

		/// <summary>Returns the one-to-one relations from <paramref name="type"/> to other <see cref="CD.Type"/>s
		/// in a format matching <see cref="CD.Relationships.HasOne"/>.</summary>
		private Dictionary<string, string>? MapHasOneRelations(Dictionary<IType, IProperty[]> hasOneRelationsByType, IType type)
			=> hasOneRelationsByType.GetValue(type)?.Select(p => {
				IType type = p.ReturnType;
				string label = p.Name;

				if (p.IsIndexer)
					label += $"[{p.Parameters.Single().Type.Name} {p.Parameters.Single().Name}]";

				if (type.TryGetNullableType(out var typeArg))
				{
					type = typeArg;
					label += " ?";
				}

				return BuildRelationship(type, label);
			}).ToDictionary(r => r.label!, r => r.to);

		/// <summary>Returns the one-to-many relations from <paramref name="type"/> to other <see cref="CD.Type"/>s
		/// in a format matching <see cref="CD.Relationships.HasMany"/>.</summary>
		private Dictionary<string, string>? MapHasManyRelations(Dictionary<IType, (IProperty property, IType elementType)[]> hasManyRelationsByType, IType type)
			=> hasManyRelationsByType.GetValue(type)?.Select(relation => {
				(IProperty property, IType elementType) = relation;
				return BuildRelationship(elementType, property.Name);
			}).ToDictionary(r => r.label!, r => r.to);

		/// <summary>Builds references to super types and (one/many) relations,
		/// recording outside references on the way and applying labels if required.</summary>
		/// <param name="type">The type to reference.</param>
		/// <param name="propertyName">Used only for property one/many relations.</param>
		private (string to, string? label) BuildRelationship(IType type, string? propertyName = null)
		{
			(string id, IType? openGeneric) = GetIdAndOpenGeneric(type);
			AddOutsideReference(id, openGeneric ?? type);

			// label the relation with the property name if provided or the closed generic type for super types
			string? label = propertyName ?? (openGeneric == null ? null : GetName(type));

			return (to: id, label);
		}

		private void AddOutsideReference(string typeId, IType type)
		{
			if (!selectedTypes!.Contains(type) && outsideReferences?.ContainsKey(typeId) == false)
				outsideReferences.Add(typeId, type.Namespace + '.' + GetName(type));
		}
	}
}