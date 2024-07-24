﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Module reference in ReferenceFolderTreeNode.
	/// </summary>
	sealed class ModuleReferenceTreeNode : ILSpyTreeNode
	{
		readonly MetadataFile module;
		readonly AssemblyTreeNode parentAssembly;
		readonly MetadataReader metadata;
		readonly ModuleReferenceHandle handle;
		readonly ModuleReference reference;
		readonly AssemblyFileHandle fileHandle;
		readonly AssemblyFile file;
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
			this.moduleName = Language.EscapeName(metadata.GetString(reference.Name));

			foreach (var h in metadata.AssemblyFiles)
			{
				var file = metadata.GetAssemblyFile(h);
				if (metadata.StringComparer.Equals(file.Name, moduleName))
				{
					this.file = file;
					this.fileHandle = h;
					this.containsMetadata = file.ContainsMetadata;
					break;
				}
			}
		}

		public override object Text {
			get { return moduleName + GetSuffixString(handle); }
		}

		public override object Icon => Images.Library;

		public override void ActivateItem(IPlatformRoutedEventArgs e)
		{
			var assemblyListNode = parentAssembly.Parent as AssemblyListTreeNode;
			if (assemblyListNode != null && containsMetadata)
			{
				var resolver = parentAssembly.LoadedAssembly.GetAssemblyResolver();
				var mainModule = parentAssembly.LoadedAssembly.GetMetadataFileOrNull();
				if (mainModule != null)
				{
					assemblyListNode.Select(assemblyListNode.FindAssemblyNode(resolver.ResolveModule(mainModule, metadata.GetString(reference.Name))));
					e.Handled = true;
				}
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, moduleName);
			language.WriteCommentLine(output, containsMetadata ? "contains metadata" : "contains no metadata");
		}
	}
}
