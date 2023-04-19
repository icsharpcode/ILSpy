// Copyright (c) 2022 Siegfried Pammer
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
using System.Windows.Media;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX.Abstractions;
using ICSharpCode.ILSpyX.Search;

namespace ICSharpCode.ILSpy.Search
{
	internal class SearchResultFactory : ISearchResultFactory
	{
		readonly Language language;

		public SearchResultFactory(Language language)
		{
			this.language = language;
		}

		float CalculateFitness(IEntity member)
		{
			string text = member.Name;

			// Probably compiler generated types without meaningful names, show them last
			if (text.StartsWith("<"))
			{
				return 0;
			}

			// Constructors and destructors always have the same name in IL:
			// Use type name instead
			if (member.SymbolKind == SymbolKind.Constructor || member.SymbolKind == SymbolKind.Destructor)
			{
				text = member.DeclaringType.Name;
			}

			// Ignore generic arguments, it not possible to search based on them either
			text = ReflectionHelper.SplitTypeParameterCountFromReflectionName(text);

			return 1.0f / text.Length;
		}

		string GetLanguageSpecificName(IEntity member)
		{
			switch (member)
			{
				case ITypeDefinition t:
					return language.TypeToString(t, false);
				case IField f:
					return language.FieldToString(f, true, false, false);
				case IProperty p:
					return language.PropertyToString(p, true, false, false);
				case IMethod m:
					return language.MethodToString(m, true, false, false);
				case IEvent e:
					return language.EventToString(e, true, false, false);
				default:
					throw new NotSupportedException(member?.GetType() + " not supported!");
			}
		}

		static ImageSource GetIcon(IEntity member)
		{
			switch (member)
			{
				case ITypeDefinition t:
					return TypeTreeNode.GetIcon(t);
				case IField f:
					return FieldTreeNode.GetIcon(f);
				case IProperty p:
					return PropertyTreeNode.GetIcon(p);
				case IMethod m:
					return MethodTreeNode.GetIcon(m);
				case IEvent e:
					return EventTreeNode.GetIcon(e);
				default:
					throw new NotSupportedException(member?.GetType() + " not supported!");
			}
		}

		public MemberSearchResult Create(IEntity entity)
		{
			var declaringType = entity.DeclaringTypeDefinition;
			return new MemberSearchResult {
				Member = entity,
				Fitness = CalculateFitness(entity),
				Name = GetLanguageSpecificName(entity),
				Location = declaringType != null ? language.TypeToString(declaringType, includeNamespace: true) : entity.Namespace,
				Assembly = entity.ParentModule.FullAssemblyName,
				ToolTip = entity.ParentModule.PEFile?.FileName,
				Image = GetIcon(entity),
				LocationImage = declaringType != null ? TypeTreeNode.GetIcon(declaringType) : Images.Namespace,
				AssemblyImage = Images.Assembly,
			};
		}

		public ResourceSearchResult Create(PEFile module, Resource resource, ITreeNode node, ITreeNode parent)
		{
			return new ResourceSearchResult {
				Resource = resource,
				Fitness = 1.0f / resource.Name.Length,
				Image = node.Icon,
				Name = resource.Name,
				LocationImage = parent.Icon,
				Location = (string)parent.Text,
				Assembly = module.FullName,
				ToolTip = module.FileName,
				AssemblyImage = Images.Assembly,
			};
		}

		public AssemblySearchResult Create(PEFile module)
		{
			return new AssemblySearchResult {
				Module = module,
				Fitness = 1.0f / module.Name.Length,
				Name = module.Name,
				Location = module.FileName,
				Assembly = module.FullName,
				ToolTip = module.FileName,
				Image = Images.Assembly,
				LocationImage = Images.Library,
				AssemblyImage = Images.Assembly,
			};
		}

		public NamespaceSearchResult Create(PEFile module, INamespace ns)
		{
			var name = ns.FullName.Length == 0 ? "-" : ns.FullName;
			return new NamespaceSearchResult {
				Namespace = ns,
				Name = name,
				Fitness = 1.0f / name.Length,
				Location = module.Name,
				Assembly = module.FullName,
				Image = Images.Namespace,
				LocationImage = Images.Assembly,
				AssemblyImage = Images.Assembly,
			};
		}
	}

	internal class TreeNodeFactory : ITreeNodeFactory
	{
		public ITreeNode Create(Resource resource)
		{
			return ResourceTreeNode.Create(resource);
		}

		public ITreeNode CreateResourcesList(PEFile module)
		{
			return new ResourceListTreeNode(module);
		}
	}
}
