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
using System.Text.RegularExpressions;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpyX.MermaidDiagrammer
{
	using CD = ClassDiagrammer;

	/* See class diagram syntax
	 * reference (may be outdated!) https://mermaid.js.org/syntax/classDiagram.html
	 * lexical definition https://github.com/mermaid-js/mermaid/blob/develop/packages/mermaid/src/diagrams/class/parser/classDiagram.jison */

	/// <summary>Produces mermaid class diagram syntax for a filtered list of types from a specified .Net assembly.</summary>
	public partial class ClassDiagrammerFactory
	{
		private readonly XmlDocumentationFormatter? xmlDocs;
		private readonly DecompilerSettings decompilerSettings;

		private ITypeDefinition[]? selectedTypes;
		private Dictionary<IType, string>? uniqueIds;
		private Dictionary<IType, string>? labels;
		private Dictionary<string, string>? outsideReferences;

		public ClassDiagrammerFactory(XmlDocumentationFormatter? xmlDocs)
		{
			this.xmlDocs = xmlDocs;

			//TODO not sure LanguageVersion.Latest is the wisest choice here; maybe cap this for better mermaid compatibility?
			decompilerSettings = new DecompilerSettings(Decompiler.CSharp.LanguageVersion.Latest) {
				AutomaticProperties = true // for IsHidden to return true for backing fields
			};
		}

		public CD BuildModel(string assemblyPath, string? include, string? exclude)
		{
			CSharpDecompiler decompiler = new(assemblyPath, decompilerSettings);
			MetadataModule mainModule = decompiler.TypeSystem.MainModule;
			IEnumerable<ITypeDefinition> allTypes = mainModule.TypeDefinitions;

			selectedTypes = FilterTypes(allTypes,
				include == null ? null : new Regex(include, RegexOptions.Compiled),
				exclude == null ? null : new Regex(exclude, RegexOptions.Compiled)).ToArray();

			// generate dictionary to read names from later
			uniqueIds = GenerateUniqueIds(selectedTypes);
			labels = [];
			outsideReferences = [];

			Dictionary<string, CD.Type[]> typesByNamespace = selectedTypes.GroupBy(t => t.Namespace).OrderBy(g => g.Key).ToDictionary(g => g.Key,
				ns => ns.OrderBy(t => t.FullName).Select(type => type.Kind == TypeKind.Enum ? BuildEnum(type) : BuildType(type)).ToArray());

			string[] excluded = allTypes.Except(selectedTypes).Select(t => t.ReflectionName).ToArray();

			return new CD {
				SourceAssemblyName = mainModule.AssemblyName,
				SourceAssemblyVersion = mainModule.AssemblyVersion.ToString(),
				TypesByNamespace = typesByNamespace,
				OutsideReferences = outsideReferences,
				Excluded = excluded
			};
		}

		/// <summary>The default strategy for pre-filtering the <paramref name="typeDefinitions"/> available in the HTML diagrammer.
		/// Applies <see cref="IsIncludedByDefault(ITypeDefinition)"/> as well as
		/// matching by <paramref name="include"/> and not by <paramref name="exclude"/>.</summary>
		/// <returns>The types to effectively include in the HTML diagrammer.</returns>
		protected virtual IEnumerable<ITypeDefinition> FilterTypes(IEnumerable<ITypeDefinition> typeDefinitions, Regex? include, Regex? exclude)
			=> typeDefinitions.Where(type => IsIncludedByDefault(type)
				&& (include?.IsMatch(type.ReflectionName) != false) // applying optional whitelist filter
				&& (exclude?.IsMatch(type.ReflectionName) != true)); // applying optional blacklist filter

		/// <summary>The strategy for deciding whether a <paramref name="type"/> should be included
		/// in the HTML diagrammer by default. Excludes compiler-generated and their nested types.</summary>
		protected virtual bool IsIncludedByDefault(ITypeDefinition type)
			=> !type.IsCompilerGeneratedOrIsInCompilerGeneratedClass();
	}
}