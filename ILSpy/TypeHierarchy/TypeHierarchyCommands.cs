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
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.FileStructure;
using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy.TypeHierarchy
{
	/// <summary>
	/// Navigate &gt; Type Hierarchy: opens the Type Hierarchy pane for the symbol under the caret
	/// when the code view has the focus, otherwise for the node selected in the assembly tree,
	/// falling back to the caret of the active document. Members resolve to their declaring type.
	/// </summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.TypeHierarchy),
		MenuCategory = "Structure", MenuOrder = 100, InputGestureText = Gesture)]
	[Shared]
	[method: ImportingConstructor]
	public sealed class TypeHierarchyCommand(AssemblyTreeModel assemblyTreeModel, DockWorkspace dockWorkspace,
		TypeHierarchyViewModel pane) : SimpleCommand
	{
		public const string Gesture = "Ctrl+Alt+H";

		public override void Execute(object? parameter)
		{
			if (FindTargetType() is { } type)
				Show(type, pane, dockWorkspace);
			else
				dockWorkspace.ShowToolPane(TypeHierarchyViewModel.PaneContentId);
		}

		ITypeDefinition? FindTargetType()
		{
			var list = assemblyTreeModel.AssemblyList;
			if (ActiveDocument.GetFocused() is { } focused
				&& TypeHierarchyViewModel.ResolveType(ActiveDocument.GetReferenceAtCaret(focused), list) is { } atCaret)
			{
				return atCaret;
			}
			if (TypeHierarchyViewModel.ResolveType(assemblyTreeModel.SelectedItem, list) is { } selected)
				return selected;
			return ActiveDocument.Get() is { } document
				? TypeHierarchyViewModel.ResolveType(ActiveDocument.GetReferenceAtCaret(document), list)
				: null;
		}

		internal static void Show(ITypeDefinition type, TypeHierarchyViewModel pane, DockWorkspace dockWorkspace)
		{
			pane.ShowHierarchy(type);
			dockWorkspace.ShowToolPane(TypeHierarchyViewModel.PaneContentId);
		}
	}

	/// <summary>
	/// Context-menu "Type Hierarchy" on a type or member, in the code view or the assembly tree.
	/// The Navigation category also lists it in the Navigate To popup.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.TypeHierarchy), Category = "Navigation",
		InputGestureText = TypeHierarchyCommand.Gesture, Order = 150)]
	[Shared]
	[method: ImportingConstructor]
	public sealed class TypeHierarchyContextMenuEntry(AssemblyTreeModel assemblyTreeModel, DockWorkspace dockWorkspace,
		TypeHierarchyViewModel pane) : IContextMenuEntry
	{
		// Visibility is evaluated every time a menu opens, so an unresolved metadata reference (IL
		// view) is judged by its handle kind instead of building a type system to resolve it.
		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes is { Length: > 0 } || context.Reference?.Reference is not EntityReference reference)
				return ResolveType(context) != null;
			return reference.Protocol == "decompile" && reference.Handle.Kind is HandleKind.TypeDefinition
				or HandleKind.TypeReference or HandleKind.MethodDefinition or HandleKind.FieldDefinition
				or HandleKind.PropertyDefinition or HandleKind.EventDefinition or HandleKind.MemberReference;
		}

		public bool IsEnabled(TextViewContext context) => true;

		public void Execute(TextViewContext context)
		{
			if (ResolveType(context) is { } type)
				TypeHierarchyCommand.Show(type, pane, dockWorkspace);
		}

		ITypeDefinition? ResolveType(TextViewContext context)
		{
			if (context.SelectedTreeNodes is { Length: > 0 } nodes)
			{
				return nodes.Length == 1
					? TypeHierarchyViewModel.ResolveType(nodes[0], assemblyTreeModel.AssemblyList)
					: null;
			}
			return TypeHierarchyViewModel.ResolveType(context.Reference?.Reference, assemblyTreeModel.AssemblyList);
		}
	}
}
