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
using System.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

using ILSpy.Languages;

namespace ILSpy.TreeNodes
{
	sealed class NamespaceTreeNode : ILSpyTreeNode
	{
		readonly string name;
		readonly MetadataFile module;

		public string Name => name;

		public NamespaceTreeNode(string name, MetadataFile module)
		{
			this.name = name ?? throw new ArgumentNullException(nameof(name));
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			LazyLoading = true;
		}

		public override object Text => name.Length == 0 ? "-" : name;

		public override object Icon => Images.Images.Namespace;

		// Stable identity for SessionSettings.ActiveTreeViewPath (used by AssemblyTreeModel.
		// FindNodeByPath / GetPathForNode). Without this override the default Object.ToString
		// returns the type name, which makes save/restore of namespace selections silently fail.
		public override string ToString() => name;

		protected override void LoadChildren()
		{
			var metadata = module.Metadata;
			var types = metadata.TypeDefinitions
				.Where(t => {
					var td = metadata.GetTypeDefinition(t);
					return td.GetDeclaringType().IsNil
						&& metadata.GetString(td.Namespace) == name;
				})
				.OrderBy(t => metadata.GetString(metadata.GetTypeDefinition(t).Name), NaturalStringComparer.Instance);

			foreach (var t in types)
				Children.Add(new TypeTreeNode(t, module));
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			var typeSystem = module.GetTypeSystemOrNull();
			if (typeSystem == null)
			{
				language.WriteCommentLine(output, "(type system unavailable)");
				return;
			}
			var types = typeSystem.MainModule.TypeDefinitions
				.Where(t => t.Namespace == name && t.DeclaringTypeDefinition == null);
			language.DecompileNamespace(name, types, output, options);
		}
	}
}
