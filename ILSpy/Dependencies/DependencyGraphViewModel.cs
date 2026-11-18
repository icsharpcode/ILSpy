// Copyright (c) 2026 Piero Viano
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

using Avalonia.Threading;

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Dependencies;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Dependencies
{
	/// <summary>
	/// Tab content for the assembly dependency diagram. Holds the graph (built once, off the UI
	/// thread, before the tab opens) and the actions the view triggers: selecting a node's assembly
	/// in the tree, opening it in its own tab, and exporting the diagram as Mermaid HTML.
	/// </summary>
	public sealed class DependencyGraphViewModel : ContentPageModel
	{
		public DependencyGraphViewModel(AssemblyDependencyGraph graph, IReadOnlyList<LoadedAssembly> roots, string title)
		{
			Graph = graph ?? throw new ArgumentNullException(nameof(graph));
			Roots = roots ?? throw new ArgumentNullException(nameof(roots));
			Title = title ?? throw new ArgumentNullException(nameof(title));
			// The diagram does not depend on the decompiler language.
			SupportsLanguageSwitching = false;
		}

		public AssemblyDependencyGraph Graph { get; }

		/// <summary>The assemblies the graph was built from.</summary>
		public IReadOnlyList<LoadedAssembly> Roots { get; }

		/// <summary>The tab hosting this content, used to bring the diagram back to the front.</summary>
		public ContentTabPage? Tab { get; set; }

		/// <summary>One-line summary shown above the diagram.</summary>
		public string Summary {
			get {
				int unresolved = Graph.Nodes.Count(n => !n.IsResolved);
				int mismatches = Graph.Edges.Count(e => e.IsVersionMismatch);
				return $"{Graph.Nodes.Count} assemblies, {Graph.Edges.Count} references, {unresolved} unresolved, {mismatches} version mismatches";
			}
		}

		/// <summary>The assembly-tree node of a graph node, or null when it is unresolved or not in the list.</summary>
		public AssemblyTreeNode? FindTreeNode(int index)
		{
			if ((uint)index >= (uint)Graph.Nodes.Count)
				return null;
			var file = Graph.Nodes[index].MetadataFile;
			if (file == null)
				return null;
			var model = AppComposition.TryGetExport<AssemblyTreeModel>();
			return (model?.Root as AssemblyListTreeNode)?.FindAssemblyNode(file);
		}

		/// <summary>
		/// Selects the node's assembly in the assembly tree. Selecting a tree node shows it in the
		/// preview tab, so the diagram tab is brought back to the front afterwards.
		/// </summary>
		public bool SelectInTree(int index)
		{
			var node = FindTreeNode(index);
			var model = AppComposition.TryGetExport<AssemblyTreeModel>();
			if (node == null || model == null)
				return false;
			model.SelectedItem = node;
			var tab = Tab;
			var dockWorkspace = AppComposition.TryGetExport<DockWorkspace>();
			if (tab != null && dockWorkspace != null)
			{
				Dispatcher.UIThread.Post(() => {
					if (dockWorkspace.Documents?.VisibleDockables?.Contains(tab) == true)
						dockWorkspace.Factory.SetActiveDockable(tab);
				}, DispatcherPriority.Background);
			}
			return true;
		}

		/// <summary>Opens the node's assembly in a new document tab.</summary>
		public bool OpenInNewTab(int index)
		{
			var node = FindTreeNode(index);
			var dockWorkspace = AppComposition.TryGetExport<DockWorkspace>();
			if (node == null || dockWorkspace == null)
				return false;
			dockWorkspace.OpenNodeInNewTab(node);
			return true;
		}
	}
}
