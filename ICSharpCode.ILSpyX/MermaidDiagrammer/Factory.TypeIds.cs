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

	public partial class ClassDiagrammerFactory
	{
		/// <summary>Generates a dictionary of unique and short, but human readable identifiers for
		/// <paramref name="types"/> to be able to safely reference them in any combination.</summary>
		private static Dictionary<IType, string> GenerateUniqueIds(IEnumerable<ITypeDefinition> types)
		{
			Dictionary<IType, string> uniqueIds = [];
			var groups = types.GroupBy(t => t.Name);

			// simplified handling for the majority of unique types
			foreach (var group in groups.Where(g => g.Count() == 1))
				uniqueIds[group.First()] = SanitizeTypeName(group.Key);

			// number non-unique types
			foreach (var group in groups.Where(g => g.Count() > 1))
			{
				var counter = 0;

				foreach (var type in group)
					uniqueIds[type] = type.Name + ++counter;
			}

			return uniqueIds;
		}

		private string GetId(IType type) => GetIdAndOpenGeneric(type).id;

		/// <summary>For a non- or open generic <paramref name="type"/>, returns a unique identifier and null.
		/// For a closed generic <paramref name="type"/>, returns the open generic type and the unique identifier of it.
		/// That helps connecting closed generic references (e.g. Store&lt;int>) to their corresponding
		/// open generic <see cref="CD.Type"/> (e.g. Store&lt;T>) like in <see cref="BuildRelationship(IType, string?)"/>.</summary>
		private (string id, IType? openGeneric) GetIdAndOpenGeneric(IType type)
		{
			// get open generic type if type is a closed generic (i.e. has type args none of which are parameters)
			var openGeneric = type is ParameterizedType generic && !generic.TypeArguments.Any(a => a is ITypeParameter)
				? generic.GenericType : null;

			type = openGeneric ?? type; // reference open instead of closed generic type

			if (uniqueIds!.TryGetValue(type, out var uniqueId))
				return (uniqueId, openGeneric); // types included by FilterTypes

			// types excluded by FilterTypes
			string? typeParams = type.TypeParameterCount == 0 ? null : ("_" + type.TypeParameters.Select(GetId).Join("_"));

			var id = SanitizeTypeName(type.FullName.Replace('.', '_'))
				+ typeParams; // to achieve uniqueness for types with same FullName (i.e. generic overloads)

			uniqueIds![type] = id; // update dictionary to avoid re-generation
			return (id, openGeneric);
		}

		private static string SanitizeTypeName(string typeName)
			=> typeName.Replace('<', '_').Replace('>', '_'); // for module of executable
	}
}