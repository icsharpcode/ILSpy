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
using System.Collections.Specialized;
using System.Linq;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX;

namespace ICSharpCode.ILSpy.TreeNodes
{
	sealed class AssemblyListTreeNode : ILSpyTreeNode
	{
		readonly AssemblyList assemblyList;

		public AssemblyList AssemblyList => assemblyList;

		public AssemblyListTreeNode(AssemblyList assemblyList)
		{
			ArgumentNullException.ThrowIfNull(assemblyList);
			this.assemblyList = assemblyList;

			Children.AddRange(assemblyList.GetAssemblies().Select(a => new AssemblyTreeNode(a)));

			assemblyList.CollectionChanged += (_, e) => {
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						Children.InsertRange(e.NewStartingIndex, e.NewItems!.Cast<LoadedAssembly>().Select(a => new AssemblyTreeNode(a)));
						break;
					case NotifyCollectionChangedAction.Remove:
						Children.RemoveRange(e.OldStartingIndex, e.OldItems!.Count);
						break;
					case NotifyCollectionChangedAction.Reset:
						Children.Clear();
						Children.AddRange(assemblyList.GetAssemblies().Select(a => new AssemblyTreeNode(a)));
						break;
				}
			};
		}

		public override object Text => assemblyList.ListName;

		// Decompiling the list root dumps every assembly in the list, each under a comment rule.
		public override void Decompile(Languages.Language language, ICSharpCode.Decompiler.ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "List: " + assemblyList.ListName);
			output.WriteLine();
			foreach (var asm in Children.OfType<AssemblyTreeNode>())
			{
				language.WriteCommentLine(output, new string('-', 60));
				output.WriteLine();
				asm.Decompile(language, output, options);
			}
		}

		public AssemblyTreeNode? FindAssemblyNode(LoadedAssembly asm)
			=> Children.OfType<AssemblyTreeNode>().FirstOrDefault(n => n.LoadedAssembly == asm);

		public AssemblyTreeNode? FindAssemblyNode(MetadataFile? module)
			=> module == null ? null : FindAssemblyNode(module.GetLoadedAssembly());

		// Drop target for top-level assemblies (handled generically by SharpTreeView). The payload is
		// always a set of file paths -- from a reorder drag (AssemblyTreeNode.Copy) or an external
		// file drop -- so both unify: open each (OpenAssembly dedupes already-loaded ones) then Move
		// to the drop index. View-level selection of the result is delegated back via the seam below.
		internal Action<IReadOnlyList<LoadedAssembly>>? SelectAssembliesAfterDrop;

		// Set by SharpTreeView for an external Explorer file drop (the internal reorder payload uses
		// AssemblyTreeNode.DataFormat); both carry a string[] of file paths.
		internal const string FileDropFormat = "FileDrop";

		public override bool CanDrop(
			ICSharpCode.ILSpyX.TreeView.PlatformAbstractions.IPlatformDragEventArgs e, int index)
		{
			if (e.Data.GetDataPresent(AssemblyTreeNode.DataFormat) || e.Data.GetDataPresent(FileDropFormat))
			{
				e.Effects = ICSharpCode.ILSpyX.TreeView.PlatformAbstractions.XPlatDragDropEffects.Move;
				return true;
			}
			e.Effects = ICSharpCode.ILSpyX.TreeView.PlatformAbstractions.XPlatDragDropEffects.None;
			return false;
		}

		public override void Drop(
			ICSharpCode.ILSpyX.TreeView.PlatformAbstractions.IPlatformDragEventArgs e, int index)
		{
			if ((e.Data.GetData(AssemblyTreeNode.DataFormat) ?? e.Data.GetData(FileDropFormat)) is not string[] files)
				return;
			var opened = files
				.Where(f => !string.IsNullOrEmpty(f))
				.Select(f => assemblyList.OpenAssembly(f))
				.Where(a => a != null)
				.Distinct()
				.ToArray();
			if (opened.Length == 0)
				return;
			assemblyList.Move(opened, index);
			SelectAssembliesAfterDrop?.Invoke(opened);
		}
	}
}
