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

using AwesomeAssertions;

using ILSpy.Metadata;
using ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class MetadataFilterRowEndToEndTests
{
	sealed class Row
	{
		public int RID { get; set; }
		public string Name { get; set; } = "";
	}

	[AvaloniaTest]
	public void Typing_Into_The_Header_TextBox_Drives_The_Matching_ColumnFilter_Text()
	{
		// Direct end-to-end of the column-builder's filter wiring: locate the TextBox
		// inside the column header that Populate built, write text to it, and confirm the
		// matching ColumnFilter.Text follows. This is the bridge that was silently broken
		// on the running app — typing produced visible text but no propagation to the
		// filter model.

		var page = new MetadataTablePageModel();
		MetadataColumnBuilder.Populate<Row>(page);

		var nameColumn = page.Columns.Single(c => (string?)c.Tag == "Name");
		var headerPanel = (StackPanel)nameColumn.Header!;
		var headerBox = headerPanel.Children.OfType<TextBox>().Single();
		var nameFilter = page.ColumnFilters.Single(f => f.ColumnName == "Name");

		headerBox.Text = "System";

		nameFilter.Text.Should().Be("System",
			"setting the header TextBox's Text must propagate to the ColumnFilter via the TextProperty observable");
	}

	[AvaloniaTest]
	public void Setting_ColumnFilter_Text_Pushes_Back_To_The_Header_TextBox()
	{
		// Reverse direction: model → view. When the page programmatically clears or seeds a
		// filter (e.g. session restore), the visible TextBox must reflect it.

		var page = new MetadataTablePageModel();
		MetadataColumnBuilder.Populate<Row>(page);

		var nameColumn = page.Columns.Single(c => (string?)c.Tag == "Name");
		var headerBox = ((StackPanel)nameColumn.Header!).Children.OfType<TextBox>().Single();
		var nameFilter = page.ColumnFilters.Single(f => f.ColumnName == "Name");

		nameFilter.Text = "System";

		headerBox.Text.Should().Be("System",
			"the filter's PropertyChanged handler must push new values back into the TextBox");
	}

	[AvaloniaTest]
	public void Setting_The_Header_TextBox_Refreshes_The_DataGridCollectionView_Filter()
	{
		// The view's DataGridCollectionView re-evaluates its predicate on every
		// ColumnFilterChanged event raised by the page. This test stubs out that event
		// path: it confirms the header TextBox → filter.Text → page.ColumnFilterChanged
		// chain fires end-to-end so the live grid would refresh its visible row set.

		var page = new MetadataTablePageModel();
		MetadataColumnBuilder.Populate<Row>(page);
		var nameColumn = page.Columns.Single(c => (string?)c.Tag == "Name");
		var headerBox = ((StackPanel)nameColumn.Header!).Children.OfType<TextBox>().Single();

		var fired = 0;
		page.ColumnFilterChanged += () => fired++;

		headerBox.Text = "System";

		fired.Should().BeGreaterThan(0,
			"the event the view subscribes to for refreshing the grid must fire when the header TextBox changes");
	}
}
