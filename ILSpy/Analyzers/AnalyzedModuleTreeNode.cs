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
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Analyzers.TreeNodes
{
	/// <summary>
	/// Wraps an entire module as an analysable entry. <see cref="Member"/> is
	/// <see langword="null"/> — module-level analyses (e.g. "Applied To" against an
	/// assembly-level attribute) hang their results off this row.
	/// </summary>
	internal sealed class AnalyzedModuleTreeNode : AnalyzerEntityTreeNode
	{
		readonly IModule analyzedModule;

		public AnalyzedModuleTreeNode(IModule analyzedModule, IEntity? source)
		{
			this.analyzedModule = analyzedModule ?? throw new ArgumentNullException(nameof(analyzedModule));
			this.SourceMember = source;
			LazyLoading = true;
		}

		public override IEntity? Member => null;

		public override object Text => analyzedModule.AssemblyName;

		public override object Icon => Images.Assembly;

		public override object? ToolTip => analyzedModule.MetadataFile?.FileName;

		public IModule Module => analyzedModule;

		public override void ActivateItem(IPlatformRoutedEventArgs e)
		{
			e.Handled = true;
			if (analyzedModule.MetadataFile == null)
				return;
			try
			{
				var assemblyTreeModel = AppComposition.Current.GetExport<AssemblyTreeModel>();
				var loaded = assemblyTreeModel.AssemblyList?
					.GetAssemblies()
					.FirstOrDefault(a => a.GetMetadataFileOrNull() == analyzedModule.MetadataFile);
				if (loaded == null)
					return;
				var listRoot = assemblyTreeModel.Root as AssemblyListTreeNode;
				if (listRoot == null)
					return;
				var node = listRoot.FindAssemblyNode(loaded);
				if (node != null)
					assemblyTreeModel.SelectedItem = node;
			}
			catch
			{
				// Design-time / minimal-test path.
			}
		}

		protected override void LoadChildren() => AddAnalyzerChildren(analyzedModule);

		public override bool HandleAssemblyListChanged(
			ICollection<LoadedAssembly> removedAssemblies,
			ICollection<LoadedAssembly> addedAssemblies)
		{
			foreach (var asm in removedAssemblies)
			{
				if (analyzedModule.MetadataFile == asm.GetMetadataFileOrNull())
					return false; // module is gone — drop me
			}
			this.Children.RemoveAll(node =>
				node is not AnalyzerTreeNode an
				|| !an.HandleAssemblyListChanged(removedAssemblies, addedAssemblies));
			return true;
		}
	}
}
