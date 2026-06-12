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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// Root of the analyzer pane's tree. Holds one <see cref="AnalyzerEntityTreeNode"/>
	/// per analysed entity and prunes stale entries when the active
	/// <see cref="AssemblyList"/> changes — either an assembly is closed (matching
	/// children dropped) or the list itself is swapped out (entire subtree wiped).
	/// </summary>
	public sealed class AnalyzerRootNode : AnalyzerTreeNode
	{
		readonly AssemblyTreeModel? assemblyTreeModel;
		AssemblyList? activeList;

		public AnalyzerRootNode()
			: this(TryGetAssemblyTreeModel())
		{
		}

		internal AnalyzerRootNode(AssemblyTreeModel? assemblyTreeModel)
		{
			this.assemblyTreeModel = assemblyTreeModel;
			if (assemblyTreeModel == null)
				return;
			assemblyTreeModel.PropertyChanged += OnModelPropertyChanged;
			AttachToActiveList(assemblyTreeModel.AssemblyList);
		}

		static AssemblyTreeModel? TryGetAssemblyTreeModel()
		{
			try
			{
				return AppComposition.Current.GetExport<AssemblyTreeModel>();
			}
			catch
			{
				// Design-time previews / minimal tests boot without the full composition.
				// The pane still works as a passive container in that case.
				return null;
			}
		}

		void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(AssemblyTreeModel.AssemblyList))
				return;
			AttachToActiveList(assemblyTreeModel?.AssemblyList);
			// A new active list means every analysed entity references a module that may
			// not be reachable any more — wipe the tree the same way a Reset on the old
			// list would have.
			this.Children.Clear();
		}

		void AttachToActiveList(AssemblyList? list)
		{
			if (ReferenceEquals(activeList, list))
				return;
			if (activeList != null)
				activeList.CollectionChanged -= OnAssemblyListCollectionChanged;
			activeList = list;
			if (activeList != null)
				activeList.CollectionChanged += OnAssemblyListCollectionChanged;
		}

		void OnAssemblyListCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				this.Children.Clear();
				return;
			}
			var removed = e.OldItems?.Cast<LoadedAssembly>().ToArray() ?? System.Array.Empty<LoadedAssembly>();
			var added = e.NewItems?.Cast<LoadedAssembly>().ToArray() ?? System.Array.Empty<LoadedAssembly>();
			HandleAssemblyListChanged(removed, added);
		}

		public override bool HandleAssemblyListChanged(
			ICollection<LoadedAssembly> removedAssemblies,
			ICollection<LoadedAssembly> addedAssemblies)
		{
			this.Children.RemoveAll(node =>
				node is not AnalyzerTreeNode an
				|| !an.HandleAssemblyListChanged(removedAssemblies, addedAssemblies));
			return true;
		}
	}
}
