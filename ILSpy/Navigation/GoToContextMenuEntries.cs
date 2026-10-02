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

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Navigation
{
	/// <summary>
	/// Shared behavior of the go-to context-menu entries: they act on the entity of a right-clicked
	/// code reference, or of a single selected tree node, and hand it to <see cref="GoToNavigator"/>.
	/// </summary>
	public abstract class GoToContextMenuEntry : IContextMenuEntry
	{
		/// <summary>The category every navigation action is exported under, so the Navigate To popup lists it.</summary>
		public const string NavigationCategory = "Navigation";

		readonly GoToNavigator navigator;

		protected GoToContextMenuEntry(GoToNavigator navigator)
		{
			this.navigator = navigator;
		}

		protected abstract GoToKind Kind { get; }

		public bool IsVisible(TextViewContext context)
			=> GetTarget(context) is { } entity && SymbolHierarchy.IsApplicable(Kind, entity);

		public bool IsEnabled(TextViewContext context) => true;

		public void Execute(TextViewContext context)
		{
			if (GetTarget(context) is { } entity)
				navigator.GoTo(Kind, entity, ActiveNavigationContext.AnchorOf(context));
		}

		/// <summary>The entity a go-to command acts on in <paramref name="context"/>, or null.</summary>
		public static IEntity? GetTarget(TextViewContext context)
		{
			if (context.SelectedTreeNodes is { Length: > 0 } nodes)
				return nodes.Length == 1 && nodes[0] is IMemberTreeNode { Member: { } member } ? member : null;
			return context.Reference?.Reference as IEntity;
		}
	}

	[ExportContextMenuEntry(Header = nameof(Resources.GoToDeclaration), Category = NavigationCategory, Order = 151, InputGestureText = "F12")]
	[Shared]
	[method: ImportingConstructor]
	public sealed class GoToDeclarationContextMenuEntry(GoToNavigator navigator) : GoToContextMenuEntry(navigator)
	{
		protected override GoToKind Kind => GoToKind.Declaration;
	}

	[ExportContextMenuEntry(Header = nameof(Resources.GoToImplementation), Category = NavigationCategory, Order = 152, InputGestureText = "Ctrl+F12")]
	[Shared]
	[method: ImportingConstructor]
	public sealed class GoToImplementationContextMenuEntry(GoToNavigator navigator) : GoToContextMenuEntry(navigator)
	{
		protected override GoToKind Kind => GoToKind.Implementation;
	}

	[ExportContextMenuEntry(Header = nameof(Resources.GoToBaseSymbols), Category = NavigationCategory, Order = 153, InputGestureText = "Alt+Home")]
	[Shared]
	[method: ImportingConstructor]
	public sealed class GoToBaseSymbolsContextMenuEntry(GoToNavigator navigator) : GoToContextMenuEntry(navigator)
	{
		protected override GoToKind Kind => GoToKind.BaseSymbols;
	}

	[ExportContextMenuEntry(Header = nameof(Resources.GoToDerivedSymbols), Category = NavigationCategory, Order = 154, InputGestureText = "Alt+End")]
	[Shared]
	[method: ImportingConstructor]
	public sealed class GoToDerivedSymbolsContextMenuEntry(GoToNavigator navigator) : GoToContextMenuEntry(navigator)
	{
		protected override GoToKind Kind => GoToKind.DerivedSymbols;
	}

	/// <summary>
	/// Selects and reveals, in the assembly tree, the node of the symbol under the caret or, when
	/// the caret is not on a symbol, the node the document was decompiled from.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.LocateInAssemblyExplorer), Category = GoToContextMenuEntry.NavigationCategory, Order = 160, InputGestureText = "Shift+Alt+L")]
	[Shared]
	[method: ImportingConstructor]
	public sealed class LocateInAssemblyExplorerContextMenuEntry(
		AssemblyTreeModel assemblyTreeModel,
		DockWorkspace dockWorkspace,
		GoToNavigator navigator) : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
			=> context.TextView != null
				&& (context.Reference?.Reference is IEntity || DocumentOf(context)?.CurrentNode != null);

		public bool IsEnabled(TextViewContext context) => true;

		public void Execute(TextViewContext context) => Locate(context);

		/// <summary>Runs the locate action; returns false (after showing a notice) when there is nothing to locate.</summary>
		public bool Locate(TextViewContext context)
		{
			var anchor = ActiveNavigationContext.AnchorOf(context);
			ILSpyTreeNode? node;
			if (context.Reference?.Reference is IEntity entity)
			{
				node = assemblyTreeModel.FindTreeNode(entity);
				if (node == null)
				{
					navigator.ShowNotice($"{navigator.Describe(entity)} is not in the assembly list", anchor);
					return false;
				}
			}
			else
			{
				node = DocumentOf(context)?.CurrentNode;
				if (node == null)
				{
					navigator.ShowNotice("Nothing to locate in the assembly explorer", anchor);
					return false;
				}
			}
			assemblyTreeModel.SelectNode(node);
			dockWorkspace.ShowToolPane(AssemblyTreeModel.PaneContentId);
			return true;
		}

		// The document shown by the context's text view, or the active decompiler tab when the
		// context does not come from a text view.
		DecompilerTabPageModel? DocumentOf(TextViewContext context)
			=> context.TextView != null
				? context.TextView.DataContext as DecompilerTabPageModel
				: dockWorkspace.ActiveDecompilerTab;
	}
}
