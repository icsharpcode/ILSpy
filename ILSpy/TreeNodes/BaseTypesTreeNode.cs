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

using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Lists the base types of a class — the inheritance chain plus implemented interfaces, in
	/// type-itself-first → most-distant-ancestor order. Lazy: children are only resolved when
	/// the user expands the node.
	/// </summary>
	public sealed class BaseTypesTreeNode : ILSpyTreeNode
	{
		readonly MetadataFile module;
		readonly ITypeDefinition type;

		public BaseTypesTreeNode(MetadataFile module, ITypeDefinition type)
		{
			this.module = module;
			this.type = type;
			LazyLoading = true;
		}

		public override object Text => ICSharpCode.ILSpy.Properties.Resources.BaseTypes;

		public override object? NavigationText => $"{Text} ({Language.TypeToString(type)})";

		public override object Icon => Images.SuperTypes;

		protected override void LoadChildren()
		{
			AddBaseTypes(Children, module, type);
		}

		internal static void AddBaseTypes(SharpTreeNodeCollection children, MetadataFile module, ITypeDefinition typeDefinition)
		{
			// Re-resolve the type with an Uncached type system so we get a fresh inheritance
			// chain (some Decompiler-flag toggles affect how interfaces fold in). Mirrors WPF.
			var handle = (TypeDefinitionHandle)typeDefinition.MetadataToken;
			var typeSystem = new DecompilerTypeSystem(module, module.GetAssemblyResolver(),
				TypeSystemOptions.Default | TypeSystemOptions.Uncached);
			if (typeSystem.MainModule.ResolveEntity(handle) is not ITypeDefinition t)
				return;
			// GetAllBaseTypeDefinitions returns [furthest-ancestor, ..., direct-base, self]; we
			// want everything except self, in walk-up order — so reverse and skip(1).
			foreach (var td in t.GetAllBaseTypeDefinitions().Reverse().Skip(1))
			{
				if (t.Kind != TypeKind.Interface || t.Kind == td.Kind)
					children.Add(new BaseTypesEntryNode(td));
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			EnsureLazyChildren();
			foreach (var child in Children.OfType<ILSpyTreeNode>())
				child.Decompile(language, output, options);
		}
	}
}
