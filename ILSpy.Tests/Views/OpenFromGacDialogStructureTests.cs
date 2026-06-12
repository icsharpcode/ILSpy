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
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Views;

/// <summary>
/// Pins the GAC browser's grid shape against the WPF dialog. WPF uses a ListView with a
/// GridView of five SortableGridViewColumns — Reference Name, Version, Culture,
/// Public Key Token, Location — each click-to-sort. The Avalonia port started with a
/// single-column ListBox that mashed every field into one TextBlock via
/// <see cref="object.ToString"/>; this fixture asserts the rebuilt dialog matches the
/// WPF column set so a future "let's go back to a simple list" doesn't quietly remove
/// the per-field sort and column widths users came to rely on.
/// </summary>
[TestFixture]
public class OpenFromGacDialogStructureTests
{
	[AvaloniaTest]
	public void Dialog_Window_Title_Comes_From_Localised_Resources()
	{
		var dialog = new OpenFromGacDialog();
		dialog.Title.Should().Be(Resources.OpenFrom,
			"the dialog title must match WPF's localised Resources.OpenFrom (currently 'Open From GAC')");
	}

	[AvaloniaTest]
	public void Dialog_Uses_A_DataGrid_With_Five_Sortable_Columns_Matching_WPF()
	{
		var dialog = new OpenFromGacDialog();
		// Force layout so the DataGrid's internal column templates are realised.
		dialog.Measure(global::Avalonia.Size.Infinity);
		dialog.Arrange(new global::Avalonia.Rect(dialog.DesiredSize));

		var grid = dialog.GetVisualDescendants().OfType<DataGrid>().FirstOrDefault();
		grid.Should().NotBeNull(
			"the GAC browser must use a DataGrid (not a ListBox) so each field gets its own "
			+ "sortable column — that's the whole reason WPF picked ListView+GridView");

		var headers = grid!.Columns.Select(c => c.Header as string).ToList();
		headers.Should().Contain(new[] {
			Resources.ReferenceName,
			Resources.Version,
			Resources.CultureLabel,
			Resources.PublicToken,
			Resources.Location,
		}, "the five WPF column headers must all be present and localised");

		grid.Columns.Should().HaveCount(5,
			"exactly five columns — no extras, no missing fields");
		grid.Columns.Should().AllSatisfy(c => c.CanUserSort.Should().BeTrue(
			"every column must be sortable to match WPF's SortableGridViewColumn.SortMode=Automatic"));
	}
}
