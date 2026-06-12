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
using System.Reflection.Metadata;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Module reference inside <see cref="ReferenceFolderTreeNode"/>. Modern .NET assemblies
	/// almost never have module references (multi-file assemblies were a .NET Framework
	/// feature) — the node is here so non-PE / multi-module edge cases still render
	/// correctly.
	/// </summary>
	sealed class ModuleReferenceTreeNode : ILSpyTreeNode
	{
		readonly MetadataFile module;
		readonly AssemblyTreeNode parentAssembly;
		readonly MetadataReader metadata;
		readonly ModuleReferenceHandle handle;
		readonly ModuleReference reference;
		readonly string moduleName;
		readonly bool containsMetadata;

		public ModuleReferenceTreeNode(AssemblyTreeNode parentAssembly, ModuleReferenceHandle r, MetadataFile module)
		{
			this.parentAssembly = parentAssembly ?? throw new ArgumentNullException(nameof(parentAssembly));
			if (r.IsNil)
				throw new ArgumentNullException(nameof(r));
			this.handle = r;
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			this.metadata = module.Metadata;
			this.reference = module.Metadata.GetModuleReference(r);
			this.moduleName = ILAmbience.EscapeName(metadata.GetString(reference.Name));

			foreach (var h in metadata.AssemblyFiles)
			{
				var file = metadata.GetAssemblyFile(h);
				if (metadata.StringComparer.Equals(file.Name, moduleName))
				{
					this.containsMetadata = file.ContainsMetadata;
					break;
				}
			}
		}

		public override object Text => moduleName;

		public override object? NavigationText => $"{Text} ({ICSharpCode.ILSpy.Properties.Resources.References})";

		public override object Icon => Images.Library;

		public override void ActivateItem(IPlatformRoutedEventArgs e)
		{
			if (parentAssembly.Parent is not AssemblyListTreeNode listNode || !containsMetadata)
				return;
			var resolver = parentAssembly.LoadedAssembly.GetAssemblyResolver();
			var mainModule = parentAssembly.LoadedAssembly.GetMetadataFileOrNull();
			if (mainModule == null)
				return;
			var resolved = resolver.ResolveModule(mainModule, metadata.GetString(reference.Name));
			if (resolved == null)
				return;
			var node = listNode.FindAssemblyNode(resolved);
			if (node == null)
				return;
			AppComposition.Current.GetExport<AssemblyTreeModel>().SelectedItem = node;
			e.Handled = true;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			output.WriteLine(moduleName);
			output.WriteLine(containsMetadata ? "contains metadata" : "contains no metadata");
		}
	}
}
