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
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Util;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click → "Open Containing Folder". Walks up the selection's parent chain to the
	/// enclosing <see cref="AssemblyTreeNode"/> (so it works on members and namespaces, not
	/// just the assembly row itself) and asks the OS file manager to reveal that file.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources._OpenContainingFolder), Category = nameof(Resources.Shell), Icon = "Images/FolderOpen", Order = 500)]
	[Shared]
	public sealed class OpenContainingFolderContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => GetPathsToReveal(context).Count > 0;

		public bool IsEnabled(TextViewContext context) => GetPathsToReveal(context).Count > 0;

		public void Execute(TextViewContext context)
		{
			// Reveal all selected files in one grouped call so that several assemblies in the same
			// folder open a single Explorer window (with them selected) rather than one per file.
			ShellHelper.RevealFiles(GetPathsToReveal(context));
		}

		/// <summary>Public for tests: returns the on-disk file paths the reveal would target,
		/// in selection order. Each path is guaranteed to exist; deep selections trace up to
		/// the enclosing assembly.</summary>
		public IReadOnlyList<string> GetPathsToReveal(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: > 0 } nodes)
				return System.Array.Empty<string>();
			var paths = new List<string>(nodes.Length);
			foreach (var node in nodes)
			{
				var assembly = AssemblyTreeNode.FindEnclosing(node);
				if (assembly is null)
					return System.Array.Empty<string>();
				var path = assembly.LoadedAssembly.FileName;
				if (!File.Exists(path))
					return System.Array.Empty<string>();
				paths.Add(path);
			}
			return paths;
		}


	}
}
