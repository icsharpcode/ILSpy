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

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;

namespace ICSharpCode.ILSpy.Navigation
{
	// Main-menu (Navigate) counterparts of the navigation context-menu entries. They act on the
	// surface the user was working in (see ActiveNavigationContext): the symbol under the caret of
	// the last focused decompiler view, otherwise the assembly-tree selection. CanExecute stays true
	// because caret moves raise no command re-query; when there is nothing to act on, a short notice
	// says so instead.

	/// <summary>Base for the Navigate-menu go-to commands.</summary>
	public abstract class GoToMenuCommand : SimpleCommand
	{
		readonly AssemblyTreeModel assemblyTreeModel;
		readonly GoToNavigator navigator;

		protected GoToMenuCommand(AssemblyTreeModel assemblyTreeModel, GoToNavigator navigator)
		{
			this.assemblyTreeModel = assemblyTreeModel;
			this.navigator = navigator;
			ActiveNavigationContext.EnsureFocusTracking();
		}

		protected abstract GoToKind Kind { get; }

		public override void Execute(object? parameter)
		{
			var context = ActiveNavigationContext.Current(assemblyTreeModel);
			var anchor = ActiveNavigationContext.AnchorOf(context);
			if (GoToContextMenuEntry.GetTarget(context) is not { } entity)
			{
				navigator.ShowNotice("No symbol to navigate from", anchor);
				return;
			}
			if (!SymbolHierarchy.IsApplicable(Kind, entity))
			{
				navigator.ShowNotice(Kind switch {
					GoToKind.BaseSymbols => "No base symbols found",
					GoToKind.DerivedSymbols => "No derived symbols found",
					_ => "No implementations found",
				}, anchor);
				return;
			}
			navigator.GoTo(Kind, entity, anchor);
		}
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.GoToDeclaration), MenuCategory = "GoTo", MenuOrder = 10, InputGestureText = "F12")]
	[Shared]
	[method: ImportingConstructor]
	sealed class GoToDeclarationCommand(AssemblyTreeModel assemblyTreeModel, GoToNavigator navigator)
		: GoToMenuCommand(assemblyTreeModel, navigator)
	{
		protected override GoToKind Kind => GoToKind.Declaration;
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.GoToImplementation), MenuCategory = "GoTo", MenuOrder = 11, InputGestureText = "Ctrl+F12")]
	[Shared]
	[method: ImportingConstructor]
	sealed class GoToImplementationCommand(AssemblyTreeModel assemblyTreeModel, GoToNavigator navigator)
		: GoToMenuCommand(assemblyTreeModel, navigator)
	{
		protected override GoToKind Kind => GoToKind.Implementation;
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.GoToBaseSymbols), MenuCategory = "GoTo", MenuOrder = 12, InputGestureText = "Alt+Home")]
	[Shared]
	[method: ImportingConstructor]
	sealed class GoToBaseSymbolsCommand(AssemblyTreeModel assemblyTreeModel, GoToNavigator navigator)
		: GoToMenuCommand(assemblyTreeModel, navigator)
	{
		protected override GoToKind Kind => GoToKind.BaseSymbols;
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.GoToDerivedSymbols), MenuCategory = "GoTo", MenuOrder = 13, InputGestureText = "Alt+End")]
	[Shared]
	[method: ImportingConstructor]
	sealed class GoToDerivedSymbolsCommand(AssemblyTreeModel assemblyTreeModel, GoToNavigator navigator)
		: GoToMenuCommand(assemblyTreeModel, navigator)
	{
		protected override GoToKind Kind => GoToKind.DerivedSymbols;
	}

	/// <summary>
	/// Navigate To: a chooser over every context-menu entry exported under the Navigation category
	/// that is visible and enabled for the current context; picking one runs it on that context.
	/// </summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.NavigateTo), MenuCategory = "NavigateTo", MenuOrder = 0, InputGestureText = "Ctrl+Shift+G")]
	[Shared]
	sealed class NavigateToCommand : SimpleCommand
	{
		readonly AssemblyTreeModel assemblyTreeModel;
		readonly GoToNavigator navigator;
		readonly ContextMenuEntryRegistry registry;

		[ImportingConstructor]
		public NavigateToCommand(AssemblyTreeModel assemblyTreeModel, GoToNavigator navigator, ContextMenuEntryRegistry registry)
		{
			this.assemblyTreeModel = assemblyTreeModel;
			this.navigator = navigator;
			this.registry = registry;
			ActiveNavigationContext.EnsureFocusTracking();
		}

		public override void Execute(object? parameter)
		{
			var context = ActiveNavigationContext.Current(assemblyTreeModel);
			var anchor = ActiveNavigationContext.AnchorOf(context);
			var choices = registry.Entries
				.Where(e => e.Metadata.Category == GoToContextMenuEntry.NavigationCategory)
				.OrderBy(e => e.Metadata.Order)
				.Where(e => e.Value.IsVisible(context) && e.Value.IsEnabled(context))
				.Select(e => new NavigationChoice(
					ResourceHelper.GetString(e.Metadata.Header).Replace("_", ""),
					() => e.Value.Execute(context)))
				.ToList();
			if (choices.Count == 0)
			{
				navigator.ShowNotice("No navigation available here", anchor);
				return;
			}
			navigator.ShowChoices(ResourceHelper.GetString(nameof(Resources.NavigateTo)), choices, anchor);
		}
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.LocateInAssemblyExplorer), MenuCategory = "Locate", MenuOrder = 20, InputGestureText = "Shift+Alt+L")]
	[Shared]
	sealed class LocateInAssemblyExplorerCommand : SimpleCommand
	{
		readonly AssemblyTreeModel assemblyTreeModel;
		readonly LocateInAssemblyExplorerContextMenuEntry locate;

		[ImportingConstructor]
		public LocateInAssemblyExplorerCommand(AssemblyTreeModel assemblyTreeModel, DockWorkspace dockWorkspace, GoToNavigator navigator)
		{
			this.assemblyTreeModel = assemblyTreeModel;
			locate = new LocateInAssemblyExplorerContextMenuEntry(assemblyTreeModel, dockWorkspace, navigator);
			ActiveNavigationContext.EnsureFocusTracking();
		}

		// From the tree the selection is not passed on: locating works on the active document.
		public override void Execute(object? parameter)
		{
			var context = ActiveNavigationContext.Current(assemblyTreeModel);
			locate.Locate(context.TextView != null ? context : new TextViewContext());
		}
	}

	/// <summary>Find Usages: analyzes the current symbol, like the Analyze context-menu entry.</summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.Analyze), MenuCategory = "Analyze", MenuOrder = 30, InputGestureText = "Shift+F12")]
	[Shared]
	sealed class FindUsagesCommand : SimpleCommand
	{
		readonly AssemblyTreeModel assemblyTreeModel;
		readonly AnalyzerTreeViewModel analyzerTreeViewModel;
		readonly DockWorkspace dockWorkspace;
		readonly GoToNavigator navigator;

		[ImportingConstructor]
		public FindUsagesCommand(AssemblyTreeModel assemblyTreeModel, AnalyzerTreeViewModel analyzerTreeViewModel,
			DockWorkspace dockWorkspace, GoToNavigator navigator)
		{
			this.assemblyTreeModel = assemblyTreeModel;
			this.analyzerTreeViewModel = analyzerTreeViewModel;
			this.dockWorkspace = dockWorkspace;
			this.navigator = navigator;
			ActiveNavigationContext.EnsureFocusTracking();
		}

		public override void Execute(object? parameter)
		{
			var context = ActiveNavigationContext.Current(assemblyTreeModel);
			if (!AnalyzeContextMenuEntry.IsVisibleForContext(context)
				|| !AnalyzeContextMenuEntry.IsEnabledForContext(context)
				|| !AnalyzeContextMenuEntry.Analyze(context, analyzerTreeViewModel, dockWorkspace))
			{
				navigator.ShowNotice("No symbol to analyze", ActiveNavigationContext.AnchorOf(context));
			}
		}
	}
}
