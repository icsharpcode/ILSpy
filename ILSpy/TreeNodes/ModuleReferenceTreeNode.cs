// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Module reference in ReferenceFolderTreeNode.
	/// </summary>
	sealed class ModuleReferenceTreeNode : ILSpyTreeNode
	{
		readonly AssemblyTreeNode parentAssembly;
		readonly MetadataReader metadata;
		readonly ModuleReferenceHandle handle;
		readonly ModuleReference reference;
		readonly AssemblyFileHandle fileHandle;
		readonly AssemblyFile file;
		readonly string moduleName;
		readonly bool containsMetadata;
		
		public ModuleReferenceTreeNode(AssemblyTreeNode parentAssembly, ModuleReferenceHandle r, MetadataReader module)
		{
			this.parentAssembly = parentAssembly ?? throw new ArgumentNullException(nameof(parentAssembly));
			if (r.IsNil)
				throw new ArgumentNullException(nameof(r));
			this.metadata = module;
			this.handle = r;
			this.reference = module.GetModuleReference(r);
			this.moduleName = metadata.GetString(reference.Name);

			foreach (var h in module.AssemblyFiles) {
				var file = module.GetAssemblyFile(h);
				if (module.StringComparer.Equals(file.Name, moduleName)) {
					this.file = file;
					this.fileHandle = h;
					this.containsMetadata = file.ContainsMetadata;
					break;
				}
			}
		}
		
		public override object Text {
			get { return moduleName + ((EntityHandle)handle).ToSuffixString(); }
		}

		public override object Icon => Images.Library;

		public override void ActivateItem(System.Windows.RoutedEventArgs e)
		{
			var assemblyListNode = parentAssembly.Parent as AssemblyListTreeNode;
			if (assemblyListNode != null && containsMetadata) {
				assemblyListNode.Select(assemblyListNode.FindAssemblyNode(parentAssembly.LoadedAssembly.LookupReferencedModule(parentAssembly.LoadedAssembly.GetPEFileOrNull(), metadata.GetString(reference.Name))));
				e.Handled = true;
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, moduleName);
			language.WriteCommentLine(output, containsMetadata ? "contains metadata" : "contains no metadata");
		}
	}
}
