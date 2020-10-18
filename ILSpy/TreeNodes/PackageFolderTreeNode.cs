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

using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Lists the embedded resources in an assembly.
	/// </summary>
	sealed class PackageFolderTreeNode : ILSpyTreeNode
	{
		readonly PackageFolder folder;

		public PackageFolderTreeNode(PackageFolder folder, string text = null)
		{
			this.folder = folder;
			this.Text = text ?? folder.Name;
			this.LazyLoading = true;
		}

		public override object Text { get; }

		public override object Icon => Images.FolderClosed;

		public override object ExpandedIcon => Images.FolderOpen;

		protected override void LoadChildren()
		{
			this.Children.AddRange(LoadChildrenForFolder(folder.Folders, folder.Entries));
		}

		internal static IEnumerable<SharpTreeNode> LoadChildrenForFolder(IReadOnlyList<PackageFolder> folders, IReadOnlyList<PackageEntry> entries)
		{
			if (folders.Count == 1 && entries.Count == 0)
			{

			}
			foreach (var folder in folders.OrderBy(f => f.Name))
			{
				string newName = folder.Name;
				var subfolder = folder;
				while (subfolder.Folders.Count == 1 && subfolder.Entries.Count == 0)
				{
					// special case: a folder that only contains a single sub-folder
					subfolder = subfolder.Folders[0];
					newName = $"{newName}/{subfolder.Name}";
				}
				yield return new PackageFolderTreeNode(subfolder, newName);
			}
			foreach (var entry in entries.OrderBy(e => e.Name))
			{
				yield return ResourceTreeNode.Create(entry);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
		}
	}
}
