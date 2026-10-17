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

using System.Composition;
using System.Linq;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Dependencies
{
	/// <summary>Right-click one or more assemblies -> "Show Assembly Dependency Diagram".</summary>
	[ExportContextMenuEntry(Header = nameof(Resources.AssemblyDependencyDiagram), Category = "Navigation", Order = 620)]
	[Shared]
	public sealed class AssemblyDependencyDiagramContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
			=> context.SelectedTreeNodes is { Length: > 0 } nodes
				&& nodes.All(n => n is AssemblyTreeNode { LoadedAssembly.IsLoadedAsValidAssembly: true });

		public bool IsEnabled(TextViewContext context) => true;

		public void Execute(TextViewContext context)
		{
			var roots = DependencyDiagramActions.GetTargetAssemblies(context.SelectedTreeNodes, list: null);
			DependencyDiagramActions.ShowDiagramAsync(roots).HandleExceptions();
		}
	}

	/// <summary>Navigate -> "Show Assembly Dependency Diagram" for the selected assemblies, or the whole list.</summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.AssemblyDependencyDiagram), MenuCategory = "Dependencies", MenuOrder = 600)]
	[Shared]
	[method: ImportingConstructor]
	public sealed class AssemblyDependencyDiagramCommand(AssemblyTreeModel assemblyTreeModel) : SimpleCommand
	{
		public override bool CanExecute(object? parameter) => (assemblyTreeModel.AssemblyList?.Count ?? 0) > 0;

		public override void Execute(object? parameter)
		{
			var roots = DependencyDiagramActions.GetTargetAssemblies(assemblyTreeModel.SelectedItems, assemblyTreeModel.AssemblyList);
			DependencyDiagramActions.ShowDiagramAsync(roots).HandleExceptions();
		}
	}

	/// <summary>
	/// Navigate -> "Export Dependency Diagram (Mermaid HTML)..." for the selected assemblies, or the
	/// whole list. A string parameter is used as the output path instead of asking for one.
	/// </summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.ExportDependencyDiagramMermaid), MenuCategory = "Dependencies", MenuOrder = 610)]
	[Shared]
	[method: ImportingConstructor]
	public sealed class ExportDependencyDiagramMermaidCommand(AssemblyTreeModel assemblyTreeModel) : SimpleCommand
	{
		public override bool CanExecute(object? parameter) => (assemblyTreeModel.AssemblyList?.Count ?? 0) > 0;

		public override void Execute(object? parameter)
		{
			var roots = DependencyDiagramActions.GetTargetAssemblies(assemblyTreeModel.SelectedItems, assemblyTreeModel.AssemblyList);
			DependencyDiagramActions.ExportMermaidAsync(roots, parameter as string).HandleExceptions();
		}
	}
}
