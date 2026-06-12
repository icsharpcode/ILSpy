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

using System.Composition;
using System.Linq;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click an assembly → "Generate Portable PDB". Delegates to <see cref="PdbGenerator"/>,
	/// which is shared with the File-menu command of the same name.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.GeneratePortable), Category = "Debug", Icon = "Images/ProgramDebugDatabase", Order = 410)]
	[Shared]
	public sealed class GeneratePdbContextMenuEntry : IContextMenuEntry
	{
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public GeneratePdbContextMenuEntry(DockWorkspace dockWorkspace)
		{
			this.dockWorkspace = dockWorkspace;
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context)
		{
			var selectedNodes = context.SelectedTreeNodes;
			return selectedNodes?.Length > 0
				&& selectedNodes.All(n => n is AssemblyTreeNode asm && asm.LoadedAssembly.IsLoadedAsValidAssembly);
		}

		public void Execute(TextViewContext context)
		{
			var assemblies = context.SelectedTreeNodes?.OfType<AssemblyTreeNode>()
				.Select(n => n.LoadedAssembly).ToArray();
			if (assemblies == null || assemblies.Length == 0)
				return;
			PdbGenerator.GenerateAsync(assemblies, dockWorkspace).HandleExceptions();
		}
	}
}
