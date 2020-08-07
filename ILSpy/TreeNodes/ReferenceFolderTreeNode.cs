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
using System.Linq;
using System.Windows.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// References folder.
	/// </summary>
	sealed class ReferenceFolderTreeNode : ILSpyTreeNode
	{
		readonly PEFile module;
		readonly AssemblyTreeNode parentAssembly;
		
		public ReferenceFolderTreeNode(PEFile module, AssemblyTreeNode parentAssembly)
		{
			this.module = module;
			this.parentAssembly = parentAssembly;
			this.LazyLoading = true;
		}

		public override object Text => Resources.References;

		public override object Icon => Images.ReferenceFolder;

		protected override void LoadChildren()
		{
			var metadata = module.Metadata;
			foreach (var r in module.AssemblyReferences.OrderBy(r => r.Name))
				this.Children.Add(new AssemblyReferenceTreeNode(r, parentAssembly));
			foreach (var r in metadata.GetModuleReferences().OrderBy(r => metadata.GetString(metadata.GetModuleReference(r).Name)))
				this.Children.Add(new ModuleReferenceTreeNode(parentAssembly, r, metadata));
		}
		
		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, $"Detected Target-Framework-Id: {parentAssembly.LoadedAssembly.GetTargetFrameworkIdAsync().Result}");
			App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(EnsureLazyChildren));
			output.WriteLine();
			language.WriteCommentLine(output, "Referenced assemblies (in metadata order):");
			// Show metadata order of references
			foreach (var node in this.Children.OfType<ILSpyTreeNode>())
				node.Decompile(language, output, options);

			output.WriteLine();
			output.WriteLine();
			// Show full assembly load log:
			language.WriteCommentLine(output, "Assembly load log including transitive references:");
			var info = parentAssembly.LoadedAssembly.LoadedAssemblyReferencesInfo;
			foreach (var asm in info.Entries) {
				language.WriteCommentLine(output, asm.FullName);
				output.Indent();
				foreach (var item in asm.Messages) {
					language.WriteCommentLine(output, $"{item.Item1}: {item.Item2}");
				}
				output.Unindent();
				output.WriteLine();
			}
		}
	}
}
