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
using System.Reflection;

using Avalonia.Controls;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click → "Go to token" on a metadata-table cell. Visible only when the click
	/// landed on a column annotated <c>[ColumnInfo(Kind = Token)]</c>; firing it routes
	/// through the page model's <c>NavigateToCellRequested</c> event so the dock workspace
	/// resolves the (row, column) pair to the target table row — the same path a left-click
	/// on the hyperlink-styled cell takes.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.GoToToken), Order = 100, InputGestureText = "Ctrl+G")]
	[Shared]
	public sealed class GoToTokenContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => ResolveCell(context) is not null;

		public bool IsEnabled(TextViewContext context) => true;

		public void Execute(TextViewContext context)
		{
			if (ResolveCell(context) is not (DataGridCell cell, string columnName))
				return;
			if (context.DataGrid?.DataContext is not MetadataTablePageModel page)
				return;
			page.RaiseNavigateToCell(cell.DataContext!, columnName);
		}

		static (DataGridCell Cell, string ColumnName)? ResolveCell(TextViewContext context)
		{
			if (context.DataGrid is null)
				return null;
			if (context.OriginalSource is not DataGridCell cell)
				return null;
			if (cell.OwningColumn is null)
				return null;
			var columnName = cell.OwningColumn.Tag as string
				?? cell.OwningColumn.Header?.ToString();
			if (string.IsNullOrEmpty(columnName))
				return null;
			var row = cell.DataContext;
			if (row is null)
				return null;
			var prop = row.GetType().GetProperty(columnName, BindingFlags.Public | BindingFlags.Instance);
			if (prop?.GetCustomAttribute<ColumnInfoAttribute>()?.Kind != ColumnKind.Token)
				return null;
			return (cell, columnName);
		}
	}
}
