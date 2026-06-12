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

using Avalonia.Controls;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click on a metadata-grid row whose token resolves to an
	/// <see cref="ICSharpCode.Decompiler.TypeSystem.IEntity"/> → "Decompile to new tab".
	/// Opens the entity in a fresh decompiler tab via <see cref="DockWorkspace.OpenNewTab"/>,
	/// matching the gesture <see cref="DecompileInNewViewCommand"/> exposes for the
	/// assembly-tree right-click. Visible only when the click landed in a metadata grid
	/// (<see cref="TextViewContext.DataGrid"/> set) and the focused row resolves.
	/// </summary>
	[ExportContextMenuEntry(
		Header = nameof(Resources.DecompileToNewPanel),
		Category = "Navigation",
		Order = 100)]
	[Shared]
	internal sealed class DecompileMetadataRowInNewTabCommand : IContextMenuEntry
	{
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public DecompileMetadataRowInNewTabCommand(DockWorkspace dockWorkspace)
		{
			this.dockWorkspace = dockWorkspace;
		}

		public bool IsVisible(TextViewContext context) => ResolveRow(context) is not null;

		public bool IsEnabled(TextViewContext context) => true;

		public void Execute(TextViewContext context)
		{
			if (ResolveRow(context) is not { } row)
				return;
			var node = dockWorkspace.TryResolveRowToTreeNode(row);
			if (node is null)
				return;
			dockWorkspace.OpenNodeInNewTab(node);
		}

		static object? ResolveRow(TextViewContext context)
		{
			if (context.DataGrid is null)
				return null;
			if (context.OriginalSource is not DataGridCell cell)
				return null;
			return cell.DataContext;
		}
	}
}
