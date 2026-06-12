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

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Folder inside a <see cref="LoadedPackage"/> (zip / .NET bundle). Lazy-loads to a mix
	/// of nested <see cref="PackageFolderTreeNode"/>s, <see cref="AssemblyTreeNode"/>s for
	/// embedded .dll/.exe entries that the package can resolve, and
	/// <see cref="ResourceTreeNode"/>s for everything else.
	/// </summary>
	sealed class PackageFolderTreeNode : ILSpyTreeNode
	{
		readonly object text;

		public PackageFolder Folder { get; }

		public PackageFolderTreeNode(PackageFolder folder, string? text = null)
		{
			Folder = folder ?? throw new ArgumentNullException(nameof(folder));
			this.text = text ?? folder.Name;
			LazyLoading = true;
		}

		public override object Text => text;

		public override object Icon => IsExpanded ? Images.FolderOpen : Images.FolderClosed;

		protected override void OnExpanding()
		{
			base.OnExpanding();
			RaisePropertyChanged(nameof(Icon));
		}

		protected override void OnCollapsing()
		{
			base.OnCollapsing();
			RaisePropertyChanged(nameof(Icon));
		}

		protected override void LoadChildren()
		{
			foreach (var child in LoadChildrenForFolder(Folder))
				Children.Add(child);
		}

		internal static IEnumerable<SharpTreeNode> LoadChildrenForFolder(PackageFolder root)
		{
			foreach (var folder in root.Folders.OrderBy(f => f.Name))
			{
				var name = folder.Name;
				var sub = folder;
				while (sub.Folders.Count == 1 && sub.Entries.Count == 0)
				{
					// Collapse single-child folder chains (a/b/c) into one display name.
					sub = sub.Folders[0];
					name = $"{name}/{sub.Name}";
				}
				yield return new PackageFolderTreeNode(sub, name);
			}
			foreach (var entry in root.Entries.OrderBy(e => e.Name))
			{
				if (entry.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
					|| entry.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
				{
					var asm = root.ResolveFileName(entry.Name);
					if (asm != null)
					{
						yield return new AssemblyTreeNode(asm, entry);
						continue;
					}
				}
				yield return ResourceTreeNode.Create(entry);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
		}
	}
}
