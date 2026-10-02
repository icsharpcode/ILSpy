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

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.Analyzers;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Dependencies
{
	/// <summary>Shared plumbing of the dependency context-menu entries.</summary>
	static class DependencyMenuHelpers
	{
		/// <summary>Category shared by the entries that also appear in the Navigate To popup.</summary>
		public const string NavigationCategory = "Navigation";

		/// <summary>
		/// The module a single selected node stands for: an assembly node's own module, or the
		/// assembly an assembly-reference node resolves to. Null when there is none.
		/// </summary>
		public static IModule? GetModule(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not [var node])
				return null;
			switch (node)
			{
				case AssemblyTreeNode { LoadedAssembly.IsLoadedAsValidAssembly: true } assemblyNode:
					return assemblyNode.LoadedAssembly.GetTypeSystemOrNull()?.MainModule;
				case AssemblyReferenceTreeNode referenceNode:
					var owner = referenceNode.AncestorsAndSelf().OfType<AssemblyTreeNode>().FirstOrDefault();
					if (owner == null)
						return null;
					var resolved = owner.LoadedAssembly.GetAssemblyResolver().Resolve(referenceNode.AssemblyReference);
					if (resolved == null)
						return null;
					var loaded = owner.LoadedAssembly.AssemblyList.FindAssembly(resolved.FileName);
					return loaded?.GetTypeSystemOrNull()?.MainModule;
				default:
					return null;
			}
		}

		public static bool IsModuleSelection(TextViewContext context)
			=> context.SelectedTreeNodes is [AssemblyTreeNode { LoadedAssembly.IsLoadedAsValidAssembly: true }]
				or [AssemblyReferenceTreeNode];

		/// <summary>
		/// Adds <paramref name="module"/> to the analyzer pane, expands the analyzer row with the
		/// given header, selects it and brings the pane to the front.
		/// </summary>
		public static AnalyzerSearchTreeNode? AnalyzeWith(IModule module, string analyzerHeader,
			AnalyzerTreeViewModel analyzerTreeViewModel, DockWorkspace dockWorkspace)
		{
			var moduleNode = analyzerTreeViewModel.Analyze(module);
			moduleNode.EnsureLazyChildren();
			var row = moduleNode.Children.OfType<AnalyzerSearchTreeNode>()
				.FirstOrDefault(r => r.AnalyzerHeader == analyzerHeader);
			if (row != null)
			{
				row.IsExpanded = true;
				analyzerTreeViewModel.SelectedItems.Clear();
				analyzerTreeViewModel.SelectedItems.Add(row);
			}
			dockWorkspace.ShowToolPane(AnalyzerTreeViewModel.PaneContentId);
			return row;
		}
	}

	/// <summary>Right-click an assembly (or an assembly reference) -> "Referenced By" in the analyzer pane.</summary>
	[ExportContextMenuEntry(Header = nameof(Resources.ReferencedBy), Category = DependencyMenuHelpers.NavigationCategory, Order = 600)]
	[Shared]
	[method: ImportingConstructor]
	public sealed class ReferencedByContextMenuEntry(AnalyzerTreeViewModel analyzerTreeViewModel, DockWorkspace dockWorkspace) : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => DependencyMenuHelpers.IsModuleSelection(context);

		public bool IsEnabled(TextViewContext context) => true;

		public void Execute(TextViewContext context)
		{
			if (DependencyMenuHelpers.GetModule(context) is { } module)
				DependencyMenuHelpers.AnalyzeWith(module, ModuleAnalyzerHeaders.ReferencedBy, analyzerTreeViewModel, dockWorkspace);
		}
	}

	/// <summary>Right-click an assembly (or an assembly reference) -> "Dependent Code" in the analyzer pane.</summary>
	[ExportContextMenuEntry(Header = nameof(Resources.DependentCode), Category = DependencyMenuHelpers.NavigationCategory, Order = 610)]
	[Shared]
	[method: ImportingConstructor]
	public sealed class DependentCodeContextMenuEntry(AnalyzerTreeViewModel analyzerTreeViewModel, DockWorkspace dockWorkspace) : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => DependencyMenuHelpers.IsModuleSelection(context);

		public bool IsEnabled(TextViewContext context) => true;

		public void Execute(TextViewContext context)
		{
			if (DependencyMenuHelpers.GetModule(context) is { } module)
				DependencyMenuHelpers.AnalyzeWith(module, ModuleAnalyzerHeaders.DependentCode, analyzerTreeViewModel, dockWorkspace);
		}
	}

	/// <summary>Right-click one or more assemblies -> "Show Assembly Dependency Diagram".</summary>
	[ExportContextMenuEntry(Header = nameof(Resources.AssemblyDependencyDiagram), Category = DependencyMenuHelpers.NavigationCategory, Order = 620)]
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

	/// <summary>
	/// Navigate -> "Unresolved References Report" for the selected assembly nodes, or the whole list
	/// when no assembly node is selected.
	/// </summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.UnresolvedReferencesReport), MenuCategory = "Dependencies", MenuOrder = 620)]
	[Shared]
	[method: ImportingConstructor]
	public sealed class UnresolvedReferencesReportCommand(AssemblyTreeModel assemblyTreeModel) : SimpleCommand
	{
		public override bool CanExecute(object? parameter) => (assemblyTreeModel.AssemblyList?.Count ?? 0) > 0;

		public override void Execute(object? parameter)
		{
			var assemblies = DependencyDiagramActions.GetTargetAssemblies(assemblyTreeModel.SelectedItems, assemblyTreeModel.AssemblyList,
				includeOwnersOfSelection: false);
			DependencyDiagramActions.ShowUnresolvedReferencesReportAsync(assemblies).HandleExceptions();
		}
	}
}
