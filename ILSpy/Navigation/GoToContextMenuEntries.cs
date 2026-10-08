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
}
