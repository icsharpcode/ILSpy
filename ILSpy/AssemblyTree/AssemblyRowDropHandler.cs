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

using Avalonia.Controls.DataGridDragDrop;
using Avalonia.Controls.DataGridHierarchical;
using Avalonia.Input;

using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.AssemblyTree
{
	/// <summary>
	/// Handles drag-reorder drops on the assembly tree. Only top-level
	/// <see cref="AssemblyTreeNode"/>s are eligible — drops onto descendants (namespaces,
	/// types, …) or "Inside" another assembly are rejected. The reorder mutation goes
	/// through <see cref="AssemblyList.Move"/> so the same persistence path the rest of
	/// the app uses (file-open, Unload) also captures user reordering.
	/// </summary>
	internal sealed class AssemblyRowDropHandler : IDataGridRowDropHandler
	{
		readonly AssemblyList assemblyList;

		public AssemblyRowDropHandler(AssemblyList assemblyList)
		{
			ArgumentNullException.ThrowIfNull(assemblyList);
			this.assemblyList = assemblyList;
		}

		public bool Validate(DataGridRowDropEventArgs args)
		{
			if (!TryResolve(args, out _, out _))
			{
				args.EffectiveEffect = DragDropEffects.None;
				return false;
			}
			args.EffectiveEffect = DragDropEffects.Move;
			return true;
		}

		public bool Execute(DataGridRowDropEventArgs args)
		{
			if (!TryResolve(args, out var dragged, out var target))
				return false;
			var ordering = assemblyList.GetAssemblies();
			int targetIndex = Array.IndexOf(ordering, target.LoadedAssembly);
			if (targetIndex < 0)
				return false;
			int insertIndex = args.Position == DataGridRowDropPosition.After
				? targetIndex + 1
				: targetIndex;
			var loaded = dragged.Select(n => n.LoadedAssembly).ToArray();
			assemblyList.Move(loaded, insertIndex);
			return true;
		}

		bool TryResolve(DataGridRowDropEventArgs args,
			out AssemblyTreeNode[] dragged,
			out AssemblyTreeNode target)
		{
			dragged = Array.Empty<AssemblyTreeNode>();
			target = null!;

			// "Inside" would mean making one assembly a child of another — not a relationship
			// that exists in the model.
			if (args.Position == DataGridRowDropPosition.Inside)
				return false;

			if (!TryUnwrapTopLevelAssemblyNode(args.TargetItem, out target))
				return false;

			var sources = new List<AssemblyTreeNode>(args.Items.Count);
			foreach (var item in args.Items)
			{
				if (!TryUnwrapTopLevelAssemblyNode(item, out var node)
					|| node.PackageEntry != null)
					return false;
				if (ReferenceEquals(node, target))
					return false;
				sources.Add(node);
			}
			if (sources.Count == 0)
				return false;

			dragged = sources.ToArray();
			return true;
		}

		internal static bool TryUnwrapTopLevelAssemblyNode(object? item, out AssemblyTreeNode node)
		{
			node = null!;
			AssemblyTreeNode? candidate = item switch {
				HierarchicalNode hn => hn.Item as AssemblyTreeNode,
				AssemblyTreeNode atn => atn,
				_ => null,
			};
			if (candidate == null)
				return false;
			// Top-level = direct child of the AssemblyListTreeNode root. Reordering deeper
			// nodes (namespaces, types) makes no sense.
			if (candidate.Parent is not AssemblyListTreeNode)
				return false;
			node = candidate;
			return true;
		}
	}
}
