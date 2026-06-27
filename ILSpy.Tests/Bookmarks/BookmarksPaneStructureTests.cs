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
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Bookmarks;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.Views.Controls;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Bookmarks;

[TestFixture]
public class BookmarksPaneStructureTests
{
	[AvaloniaTest]
	public void Toolbar_uses_main_toolbar_chrome_and_button_content()
	{
		var pane = new BookmarksPane();
		var toolbarBorder = pane.FindControl<Border>("ToolbarBorder")!;
		var toolbarRoot = pane.FindControl<StackPanel>("ToolbarRoot")!;

		toolbarBorder.BorderThickness.Should().Be(new Avalonia.Thickness(0, 0, 0, 1));
		toolbarBorder.MinHeight.Should().Be(29);
		toolbarBorder.Padding.Should().Be(new Avalonia.Thickness(3));
		toolbarRoot.Children.OfType<Separator>().Should().HaveCount(2);
		toolbarRoot.Children.OfType<Button>().Should().AllSatisfy(button => {
			button.Content.Should().BeOfType<GrayscaleAwareImage>();
		});
	}

	[AvaloniaTest]
	public void Module_column_shows_module_name_with_full_path_tooltip()
	{
		var pane = new BookmarksPane();
		var grid = pane.FindControl<DataGrid>("BookmarkGrid")!;
		var column = grid.Columns.OfType<DataGridTemplateColumn>()
			.Single(c => Equals(c.Header, Resources.BookmarkModule));
		var bookmark = new Bookmark {
			Name = "Bookmark0",
			FileName = @"C:\assemblies\Sample.dll",
			AssemblyFullName = "Sample, Version=1.0.0.0",
			ModuleName = "Sample.dll",
			Token = 0x02000001,
			Kind = BookmarkKind.Token,
			LineNumber = 1,
			MemberName = "Sample.Type",
		};

		var textBlock = column.CellTemplate!.Build(bookmark).Should().BeOfType<TextBlock>().Subject;
		textBlock.DataContext = bookmark;
		Dispatcher.UIThread.RunJobs();

		textBlock.Text.Should().Be("Sample.dll");
		ToolTip.GetTip(textBlock).Should().Be(@"C:\assemblies\Sample.dll");
	}
}
