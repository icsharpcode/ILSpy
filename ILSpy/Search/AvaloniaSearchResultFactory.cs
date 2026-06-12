// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.Abstractions;
using ICSharpCode.ILSpyX.Search;

using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.Search
{
	/// <summary>
	/// Builds the <see cref="SearchResult"/> objects the search strategies stream into the
	/// pane's result queue. Mirrors WPF's <c>SearchResultFactory</c>: fitness ranking
	/// privileges short names (shorter == higher fitness == higher rank); compiler-generated
	/// names (those starting with <c>&lt;</c>) get fitness 0 so they sink to the bottom.
	/// </summary>
	internal sealed class AvaloniaSearchResultFactory : ISearchResultFactory
	{
		readonly Language language;

		public AvaloniaSearchResultFactory(Language language)
		{
			this.language = language;
		}

		public MemberSearchResult Create(IEntity entity)
		{
			var declaringType = entity.DeclaringTypeDefinition;
			return new MemberSearchResult {
				Member = entity,
				Fitness = CalculateFitness(entity),
				Name = GetLanguageSpecificName(entity),
				Location = declaringType != null
					? language.TypeToString(declaringType, ConversionFlags.UseFullyQualifiedEntityNames | ConversionFlags.UseFullyQualifiedTypeNames)
					: entity.Namespace ?? string.Empty,
				Assembly = entity.ParentModule?.FullAssemblyName ?? string.Empty,
				ToolTip = entity.ParentModule?.MetadataFile?.FileName,
				Image = GetIcon(entity),
				LocationImage = declaringType != null ? Images.Class : Images.Namespace,
				AssemblyImage = Images.Assembly,
			};
		}

		public ResourceSearchResult Create(MetadataFile module, Resource resource, ITreeNode node, ITreeNode parent)
		{
			return new ResourceSearchResult {
				Resource = resource,
				Fitness = 1.0f / Math.Max(1, resource.Name.Length),
				Image = Images.Library,
				Name = resource.Name,
				LocationImage = Images.Library,
				Location = (parent.Text as string) ?? string.Empty,
				Assembly = module.FullName,
				ToolTip = module.FileName,
				AssemblyImage = Images.Assembly,
			};
		}

		public AssemblySearchResult Create(MetadataFile module)
		{
			return new AssemblySearchResult {
				Module = module,
				Fitness = 1.0f / Math.Max(1, module.Name.Length),
				Name = module.Name,
				Location = module.FileName,
				Assembly = module.FullName,
				ToolTip = module.FileName,
				Image = Images.Assembly,
				LocationImage = Images.Library,
				AssemblyImage = Images.Assembly,
			};
		}

		public NamespaceSearchResult Create(MetadataFile module, INamespace ns)
		{
			var name = ns.FullName.Length == 0 ? "-" : ns.FullName;
			return new NamespaceSearchResult {
				Namespace = ns,
				Name = name,
				Fitness = 1.0f / Math.Max(1, name.Length),
				Location = module.Name,
				Assembly = module.FullName,
				Image = Images.Namespace,
				LocationImage = Images.Assembly,
				AssemblyImage = Images.Assembly,
			};
		}

		static float CalculateFitness(IEntity member)
		{
			var text = member.Name;
			if (text.StartsWith('<'))
				return 0;
			if (member.SymbolKind is SymbolKind.Constructor or SymbolKind.Destructor)
				text = member.DeclaringType?.Name ?? text;
			text = ReflectionHelper.SplitTypeParameterCountFromReflectionName(text);
			return 1.0f / Math.Max(1, text.Length);
		}

		string GetLanguageSpecificName(IEntity member) => member switch {
			ITypeDefinition t => language.TypeToString(t, ConversionFlags.None),
			IField or IProperty or IMethod or IEvent => language.EntityToString(member, ConversionFlags.ShowDeclaringType),
			_ => member.Name,
		};

		static object GetIcon(IEntity member) => member switch {
			ITypeDefinition => Images.Class,
			IField => Images.Field,
			IProperty => Images.Property,
			IMethod => Images.Method,
			IEvent => Images.Event,
			_ => Images.Library,
		};
	}
}
