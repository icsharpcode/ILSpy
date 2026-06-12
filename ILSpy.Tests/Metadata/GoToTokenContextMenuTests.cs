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

using Avalonia.Controls;
using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class GoToTokenContextMenuTests
{
	sealed class Row
	{
		public int RID { get; set; }

		[ColumnInfo("X8", Kind = ColumnKind.Token)]
		public int Method { get; set; }

		public string Name { get; set; } = "";
	}

	static DataGridCell MakeCell(DataGridColumn column, object dataContext)
	{
		// DataGridCell.OwningColumn has an internal setter; the registered DirectProperty is
		// public, so we route through SetValue to wire the synthetic cell up the same way the
		// DataGrid does at runtime.
		var cell = new DataGridCell { DataContext = dataContext };
		cell.SetValue(DataGridCell.OwningColumnProperty, column);
		return cell;
	}

	[AvaloniaTest]
	public void IsVisible_True_When_Right_Click_Lands_On_A_Token_Kind_Cell()
	{
		// The entry should only surface when the user clicked a hyperlink-style cell —
		// otherwise "Go to token" would be a no-op or, worse, dispatch to the wrong target.
		var entry = new GoToTokenContextMenuEntry();
		var col = new DataGridTemplateColumn { Header = "Method" };
		var cell = MakeCell(col, new Row { Method = 0x06000001 });
		var ctx = new TextViewContext { DataGrid = new DataGrid(), OriginalSource = cell };

		entry.IsVisible(ctx).Should().BeTrue();
	}

	[AvaloniaTest]
	public void IsVisible_False_When_Cell_Is_On_A_Plain_Column()
	{
		// Name is annotated-without-Kind; right-clicking it must not show the entry.
		var entry = new GoToTokenContextMenuEntry();
		var col = new DataGridTextColumn { Header = "Name" };
		var cell = MakeCell(col, new Row { Name = "X" });
		var ctx = new TextViewContext { DataGrid = new DataGrid(), OriginalSource = cell };

		entry.IsVisible(ctx).Should().BeFalse();
	}

	[AvaloniaTest]
	public void IsVisible_False_When_Context_Has_No_DataGrid()
	{
		// On the assembly tree (TreeGrid), the metadata-only entry must stay hidden — the
		// DataGrid field is the discriminator between "right-click on a metadata cell" and
		// "right-click anywhere else".
		var entry = new GoToTokenContextMenuEntry();
		var col = new DataGridTemplateColumn { Header = "Method" };
		var cell = MakeCell(col, new Row { Method = 1 });
		var ctx = new TextViewContext { OriginalSource = cell };

		entry.IsVisible(ctx).Should().BeFalse();
	}

	[AvaloniaTest]
	public void Execute_Raises_NavigateToCellRequested_On_The_Page_Model()
	{
		// Reuses the existing token-cell navigation path: the same code path that fires when
		// the user left-clicks a hyperlink cell, just triggered through the menu instead.
		var page = new MetadataTablePageModel();
		object? capturedRow = null;
		string? capturedColumn = null;
		page.NavigateToCellRequested += args => {
			capturedRow = args.Row;
			capturedColumn = args.ColumnName;
		};

		var grid = new DataGrid { DataContext = page };
		var col = new DataGridTemplateColumn { Header = "Method" };
		var row = new Row { Method = 0x06000001 };
		var cell = MakeCell(col, row);
		var ctx = new TextViewContext { DataGrid = grid, OriginalSource = cell };

		new GoToTokenContextMenuEntry().Execute(ctx);

		capturedRow.Should().BeSameAs(row);
		capturedColumn.Should().Be("Method");
	}
}
