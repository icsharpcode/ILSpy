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

using System.Linq;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class MetadataColumnBuilderTests
{
	sealed class SampleEntry
	{
		public int RID { get; set; }
		[ColumnInfo("X8")]
		public int Token { get; set; }
		public string Name { get; set; } = "";
	}

	[AvaloniaTest]
	public void For_Returns_One_TextColumn_Per_Public_Property_With_Hex_Converter_For_X_Format()
	{
		// The reflection-driven column builder is the bridge between a row-shape struct/class
		// and the DataGrid view. It should produce one column per public property in
		// declaration order; properties annotated with a hex format spec must apply that spec
		// at bind-time (so enum and integer values both line up cleanly).

		var columns = MetadataColumnBuilder.For<SampleEntry>();

		columns.Should().HaveCount(3);
		columns.Select(c => c.Tag).Should().Equal("RID", "Token", "Name");

		var token = (DataGridTextColumn)columns[1];
		token.Binding.Should().NotBeNull();
		token.IsReadOnly.Should().BeTrue();

		var name = (DataGridTextColumn)columns[2];
		name.SortMemberPath.Should().Be("Name");
	}

	[AvaloniaTest]
	public void For_Returns_Fresh_Column_Instances_So_Per_Page_Headers_Stay_Independent()
	{
		// Per-column filter inputs live inside each column's Header, so two pages built
		// from the same row type must hold *separate* DataGridColumn instances — otherwise
		// a filter typed on one tab would echo into the other.
		var first = MetadataColumnBuilder.For<SampleEntry>();
		var second = MetadataColumnBuilder.For<SampleEntry>();
		first.Should().NotBeSameAs(second);
		first.Zip(second, (a, b) => (a, b)).Should()
			.AllSatisfy(p => p.a.Should().NotBeSameAs(p.b));
	}

	[AvaloniaTest]
	public void Populate_Sets_Columns_And_ColumnFilters_In_Matched_Order()
	{
		// The page model uses the ColumnFilters collection to evaluate the row predicate;
		// the view binds each column header's TextBox to the filter at the same index. The
		// order must match Columns exactly or filters would route to the wrong column.
		var page = new MetadataTablePageModel();
		MetadataColumnBuilder.Populate<SampleEntry>(page);

		page.Columns.Should().HaveCount(3);
		page.ColumnFilters.Should().HaveCount(3);
		page.ColumnFilters.Select(f => f.ColumnName).Should().Equal("RID", "Token", "Name");
	}

	sealed class SampleEntryWithTooltip
	{
		public int RID { get; set; }
		public string Name { get; set; } = "";
		public string NameTooltip => $"heap-offset for {Name}";
	}

	[AvaloniaTest]
	public void For_Skips_Tooltip_Companion_Properties()
	{
		// {Column}Tooltip properties are the hover text for their sibling column (surfaced by
		// MetadataCellTooltip on cell hover), not data in their own right. They must not become
		// their own columns -- otherwise the grid shows a raw "NameTooltip" column beside "Name".
		var columns = MetadataColumnBuilder.For<SampleEntryWithTooltip>();

		columns.Select(c => c.Tag).Should().Equal("RID", "Name");
		columns.Select(c => c.Tag).Should().NotContain("NameTooltip");
	}

	sealed class SampleEntryWithToken
	{
		public int RID { get; set; }
		[ColumnInfo("X8", Kind = ColumnKind.Token)]
		public int Method { get; set; }
		public string Name { get; set; } = "";
	}

	[AvaloniaTest]
	public void For_Emits_Template_Column_For_Token_Kind_Properties()
	{
		// Properties annotated Kind=Token render as a DataGridTemplateColumn whose cell is
		// a hyperlink-styled button. Plain Format-only columns stay as text.

		var columns = MetadataColumnBuilder.For<SampleEntryWithToken>();

		columns[0].Should().BeOfType<DataGridTextColumn>("RID has no [ColumnInfo]");
		columns[1].Should().BeOfType<DataGridTemplateColumn>("Method is Kind=Token");
		columns[2].Should().BeOfType<DataGridTextColumn>("Name has no [ColumnInfo]");
	}

	[AvaloniaTest]
	public void Token_Cell_Link_Color_Comes_From_The_Theme_And_Yields_To_The_Selected_Row()
	{
		// Foreground is inherited, so a local brush on the button would outrank the
		// DataGridRow:selected white override and leave the link unreadable on the accent fill --
		// which is exactly the row navigation lands on. The "link" class lets the row win while
		// still carrying the themed colour everywhere else.
		var grid = new DataGrid { AutoGenerateColumns = false };
		foreach (var column in MetadataColumnBuilder.For<SampleEntryWithToken>())
			grid.Columns.Add(column);
		grid.ItemsSource = new[] { new SampleEntryWithToken { RID = 1, Method = 0x06000001 } };

		var window = new Window { Content = grid, Width = 400, Height = 200 };
		window.Show();
		Dispatcher.UIThread.RunJobs();

		var link = grid.GetVisualDescendants().OfType<Button>().Single(b => b.Classes.Contains("link"));
		var themed = link.Foreground;
		themed.Should().NotBeNull();
		window.TryFindResource("ILSpy.LinkForeground", window.ActualThemeVariant, out var expected).Should().BeTrue();
		(themed as ISolidColorBrush)!.Color.Should().Be((expected as ISolidColorBrush)!.Color);

		grid.SelectedIndex = 0;
		Dispatcher.UIThread.RunJobs();

		(link.Foreground as ISolidColorBrush)!.Color.Should().Be(Colors.White,
			"the selected row paints its text white over the accent fill");
	}
}
