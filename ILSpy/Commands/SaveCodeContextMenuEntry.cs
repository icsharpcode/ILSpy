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
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click → "Save Code". Mirrors the WPF entry's two modes:
	/// <list type="bullet">
	/// <item>A single node delegates to its <see cref="ILSpyTreeNode.Save"/> override —
	/// <c>AssemblyTreeNode</c> drives project / single-file selection, <c>ResourceTreeNode</c>
	/// writes raw bytes, every other node falls through to the generic single-file decompile.</item>
	/// <item>Several selected assemblies export a Visual Studio solution (one decompiled project
	/// each) via <see cref="SolutionWriter"/>.</item>
	/// </list>
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources._SaveCode), Category = nameof(Resources.Save), Icon = "Images/Save", Order = 300)]
	[Shared]
	public sealed class SaveCodeContextMenuEntry : IContextMenuEntry
	{
		readonly LanguageService languageService;
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public SaveCodeContextMenuEntry(LanguageService languageService, DockWorkspace dockWorkspace)
		{
			this.languageService = languageService;
			this.dockWorkspace = dockWorkspace;
		}

		public bool IsVisible(TextViewContext context) => CanExecute(context.SelectedTreeNodes);

		public bool IsEnabled(TextViewContext context) => CanExecute(context.SelectedTreeNodes);

		public void Execute(TextViewContext context)
		{
			var nodes = context.SelectedTreeNodes;
			if (nodes is not { Length: > 0 })
				return;

			if (SolutionExport.TryGetAssemblies(nodes, out var assemblies))
			{
				SolutionExport.PromptAndExportAsync(assemblies, languageService.CurrentLanguage, dockWorkspace).HandleExceptions();
				return;
			}

			// Single node: let it claim its own save (resources, the assembly project/single-file
			// picker), else fall through to decompile-to-file -- the same path as Ctrl+S. Without
			// the fallback, Save Code on a type/namespace/member node (no Save override) did nothing.
			if (nodes[0] is ILSpyTreeNode node)
				SaveCodeHelper.SaveNodeAsync(node, languageService, dockWorkspace).HandleExceptions();
		}

		// Visible when exactly one ILSpyTreeNode is selected (the single-node save path), or when
		// several valid assemblies are selected (the solution-export path). Mixed or other
		// multi-selections have no well-defined "save code" meaning.
		static bool CanExecute(SharpTreeNode[]? selectedNodes)
		{
			if (selectedNodes is not { Length: > 0 })
				return false;
			if (selectedNodes.Length == 1)
				return selectedNodes[0] is ILSpyTreeNode;
			return SolutionExport.TryGetAssemblies(selectedNodes, out _);
		}
	}
}
