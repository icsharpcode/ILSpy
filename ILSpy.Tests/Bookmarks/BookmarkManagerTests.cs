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

using System;
using System.IO;
using System.Linq;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Bookmarks;
using ICSharpCode.ILSpyX.Settings;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Bookmarks;

// Exercises the manager's pure list/persistence behaviour without the decompiler: toggle,
// default naming, JSON round-trip, and the merge-vs-replace import rules.
[TestFixture]
public class BookmarkManagerTests
{
	Func<string>? savedProvider;
	string tempDir = "";

	[SetUp]
	public void SetUp()
	{
		savedProvider = ILSpySettings.SettingsFilePathProvider;
		tempDir = Path.Combine(Path.GetTempPath(), "ILSpyBookmarkTest_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		ILSpySettings.SettingsFilePathProvider = () => Path.Combine(tempDir, "ILSpy.xml");
	}

	[TearDown]
	public void TearDown()
	{
		ILSpySettings.SettingsFilePathProvider = savedProvider;
		try
		{
			Directory.Delete(tempDir, recursive: true);
		}
		catch
		{
			// best effort
		}
	}

	// A manager whose sidecar lives in its own subdirectory, so two managers in one test don't
	// share (and preload) each other's saved list through the common settings path.
	BookmarkManager NewManagerInFreshDir()
	{
		var dir = Path.Combine(tempDir, Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(dir);
		ILSpySettings.SettingsFilePathProvider = () => Path.Combine(dir, "ILSpy.xml");
		return new BookmarkManager();
	}

	static Bookmark MakeBookmark(uint token, BookmarkKind kind = BookmarkKind.Token, int ilOffset = 0, string name = "", int lineNumber = 1, string? locationNodeName = null)
		=> new() {
			Name = name,
			FileName = @"C:\asm\Sample.dll",
			AssemblyFullName = "Sample, Version=1.0.0.0",
			ModuleName = "Sample.dll",
			Token = token,
			Kind = kind,
			ILOffset = ilOffset,
			LineNumber = lineNumber,
			MemberName = "Sample.Type.Member",
			LocationNodeName = locationNodeName,
		};

	[Test]
	public void Toggle_adds_then_removes_the_same_anchor()
	{
		var manager = new BookmarkManager();

		manager.Toggle(MakeBookmark(0x06000001)).Should().BeTrue();
		manager.Bookmarks.Should().HaveCount(1);

		manager.Toggle(MakeBookmark(0x06000001)).Should().BeFalse();
		manager.Bookmarks.Should().BeEmpty();
	}

	[Test]
	public void Toggle_assigns_a_default_name_when_none_given()
	{
		var manager = new BookmarkManager();

		manager.Toggle(MakeBookmark(0x06000001));
		manager.Toggle(MakeBookmark(0x06000002));

		manager.Bookmarks.Select(b => b.Name).Should().Equal("Bookmark0", "Bookmark1");
	}

	[Test]
	public void Body_anchors_with_different_offsets_are_distinct()
	{
		var manager = new BookmarkManager();

		manager.Toggle(MakeBookmark(0x06000001, BookmarkKind.Body, ilOffset: 0));
		manager.Toggle(MakeBookmark(0x06000001, BookmarkKind.Body, ilOffset: 8));

		manager.Bookmarks.Should().HaveCount(2);
	}

	[Test]
	public void Line_anchors_with_different_visible_lines_are_distinct()
	{
		var manager = new BookmarkManager();

		manager.Toggle(MakeBookmark(0x02000001, BookmarkKind.Line, lineNumber: 3));
		manager.Toggle(MakeBookmark(0x02000001, BookmarkKind.Line, lineNumber: 4));

		manager.Bookmarks.Should().HaveCount(2);
	}

	[Test]
	public void Saved_bookmarks_reload_in_a_fresh_manager()
	{
		var first = new BookmarkManager();
		first.Toggle(MakeBookmark(0x06000001, BookmarkKind.Body, ilOffset: 4, name: "Mine"));

		var second = new BookmarkManager();

		second.Bookmarks.Should().HaveCount(1);
		var reloaded = second.Bookmarks[0];
		reloaded.Name.Should().Be("Mine");
		reloaded.Token.Should().Be(0x06000001);
		reloaded.Kind.Should().Be(BookmarkKind.Body);
		reloaded.ILOffset.Should().Be(4);
	}

	[Test]
	public void Saved_line_bookmarks_reload_in_a_fresh_manager()
	{
		var first = new BookmarkManager();
		first.Toggle(MakeBookmark(0x02000001, BookmarkKind.Line, lineNumber: 7, name: "Header", locationNodeName: "Sample.Type"));

		var second = new BookmarkManager();

		second.Bookmarks.Should().ContainSingle();
		var reloaded = second.Bookmarks[0];
		reloaded.Kind.Should().Be(BookmarkKind.Line);
		reloaded.LineNumber.Should().Be(7);
		reloaded.LocationNodeName.Should().Be("Sample.Type");
	}

	[Test]
	public void Saved_bookmarks_reload_their_view_state_payload()
	{
		var first = new BookmarkManager();
		var bookmark = MakeBookmark(0x06000001, name: "With view");
		bookmark.ViewState = new BookmarkViewState(1, 42, 120.5, 7.25, 100,
			new[] { new BookmarkFoldingRange(10, 20) }, SelectedTreeNodePath: new[] { "Sample", "Sample.Type" });
		first.Toggle(bookmark);

		var second = new BookmarkManager();

		second.Bookmarks.Should().ContainSingle();
		second.Bookmarks[0].ViewState.Should().NotBeNull();
		second.Bookmarks[0].ViewState!.CaretOffset.Should().Be(42);
		second.Bookmarks[0].ViewState!.ExpandedFoldings.Should().ContainSingle()
			.Which.Should().Be(new BookmarkFoldingRange(10, 20));
		second.Bookmarks[0].ViewState!.SelectedTreeNodePath.Should().Equal("Sample", "Sample.Type");
	}

	[Test]
	public void Display_location_shows_node_name_and_line()
	{
		var bookmark = MakeBookmark(0x02000001, BookmarkKind.Line, lineNumber: 7, locationNodeName: "Sample.Type.Node");
		bookmark.ViewState = new BookmarkViewState(1, 42, 120.5, 7.25, null, null,
			SelectedTreeNodePath: new[] { "Sample", "Sample.Type" });

		bookmark.DisplayLocation.Should().Be("Sample.Type.Node:7");
	}

	[Test]
	public void Display_location_uses_resolved_rendered_line_when_it_was_not_stored()
	{
		var bookmark = MakeBookmark(0x02000001, BookmarkKind.Token, lineNumber: 0, locationNodeName: "Sample.Type.Node");

		bookmark.DisplayLocation.Should().Be("Sample.Type.Node");

		bookmark.UpdateRenderedLineNumber(59);

		bookmark.LineNumber.Should().Be(59);
		bookmark.DisplayLocation.Should().Be("Sample.Type.Node:59");
	}

	[Test]
	public void Two_managers_adding_different_bookmarks_preserve_both_entries()
	{
		var first = new BookmarkManager();
		var second = new BookmarkManager();

		first.Toggle(MakeBookmark(0x06000001, name: "First"));
		second.Toggle(MakeBookmark(0x06000002, name: "Second"));

		var reloaded = new BookmarkManager();

		reloaded.Bookmarks.Select(b => b.Name).Should().BeEquivalentTo("First", "Second");
	}

	[Test]
	public void Two_managers_editing_the_same_anchor_keep_the_later_value()
	{
		var first = new BookmarkManager();
		first.Toggle(MakeBookmark(0x06000001, name: "Original"));
		var second = new BookmarkManager();

		first.Bookmarks[0].Name = "First edit";
		second.Bookmarks[0].Name = "Second edit";

		var reloaded = new BookmarkManager();

		reloaded.Bookmarks.Should().ContainSingle();
		reloaded.Bookmarks[0].Name.Should().Be("Second edit");
	}

	[Test]
	public void Two_managers_remove_and_add_preserve_the_added_bookmark()
	{
		var first = new BookmarkManager();
		first.Toggle(MakeBookmark(0x06000001, name: "Remove me"));
		var second = new BookmarkManager();

		first.Remove(first.Bookmarks[0]);
		second.Toggle(MakeBookmark(0x06000002, name: "Keep me"));

		var reloaded = new BookmarkManager();

		reloaded.Bookmarks.Select(b => b.Name).Should().Equal("Keep me");
	}

	[Test]
	public void Malformed_bookmark_file_recovers_on_next_update()
	{
		File.WriteAllText(Path.Combine(tempDir, "ILSpy.Bookmarks.json"), "not json");
		var manager = new BookmarkManager();

		manager.Toggle(MakeBookmark(0x06000001, name: "Recovered"));

		var reloaded = new BookmarkManager();
		reloaded.Bookmarks.Select(b => b.Name).Should().Equal("Recovered");
	}

	[Test]
	public void Bookmarks_without_a_file_are_dropped_on_load()
	{
		var dir = Path.Combine(tempDir, Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(dir);
		ILSpySettings.SettingsFilePathProvider = () => Path.Combine(dir, "ILSpy.xml");
		// A stray empty entry (e.g. a committed grid placeholder row) alongside a real bookmark.
		var json = "{ \"Version\": 1, \"Bookmarks\": [" +
			"{ \"Name\": \"\", \"Enabled\": true, \"FileName\": \"\", \"AssemblyFullName\": \"\", \"ModuleName\": \"\", \"Token\": 0, \"Kind\": \"Token\", \"ILOffset\": 0, \"MemberName\": \"\" }," +
			"{ \"Name\": \"Good\", \"Enabled\": true, \"FileName\": \"C:\\\\asm\\\\Sample.dll\", \"AssemblyFullName\": \"Sample\", \"ModuleName\": \"Sample.dll\", \"Token\": 100663297, \"Kind\": \"Token\", \"ILOffset\": 0, \"MemberName\": \"Sample.T.M\" }" +
			"] }";
		File.WriteAllText(Path.Combine(dir, "ILSpy.Bookmarks.json"), json);

		var manager = new BookmarkManager();

		manager.Bookmarks.Select(b => b.Name).Should().Equal("Good");
	}

	[Test]
	public void Export_then_import_replace_roundtrips()
	{
		var source = NewManagerInFreshDir();
		source.Toggle(MakeBookmark(0x06000001, name: "A"));
		source.Toggle(MakeBookmark(0x06000002, name: "B"));
		var exportPath = Path.Combine(tempDir, "export.json");
		source.Export(exportPath);

		var target = NewManagerInFreshDir();
		target.Toggle(MakeBookmark(0x0600000F, name: "Old"));
		target.Import(exportPath, BookmarkImportMode.Replace);

		target.Bookmarks.Select(b => b.Name).Should().Equal("A", "B");
	}

	[Test]
	public void Import_merge_adds_only_new_anchors()
	{
		var source = NewManagerInFreshDir();
		source.Toggle(MakeBookmark(0x06000001, name: "Shared"));
		source.Toggle(MakeBookmark(0x06000002, name: "New"));
		var exportPath = Path.Combine(tempDir, "export.json");
		source.Export(exportPath);

		var target = NewManagerInFreshDir();
		target.Toggle(MakeBookmark(0x06000001, name: "Existing"));
		target.Import(exportPath, BookmarkImportMode.Merge);

		// The shared anchor keeps the existing entry; only the genuinely new one is appended.
		target.Bookmarks.Select(b => b.Name).Should().Equal("Existing", "New");
	}

	[Test]
	public void Import_replace_of_a_corrupt_file_keeps_the_existing_bookmarks()
	{
		var corruptPath = Path.Combine(tempDir, "corrupt.json");
		File.WriteAllText(corruptPath, "{ this is not valid json");

		var target = NewManagerInFreshDir();
		target.Toggle(MakeBookmark(0x06000001, name: "Keep"));

		bool ok = target.Import(corruptPath, BookmarkImportMode.Replace);

		ok.Should().BeFalse("a file that cannot be parsed is not a successful import");
		target.Bookmarks.Select(b => b.Name).Should().Equal(new[] { "Keep" });
	}

	[Test]
	public void Import_replace_of_a_valid_empty_file_clears_the_bookmarks()
	{
		var source = NewManagerInFreshDir();
		var emptyExport = Path.Combine(tempDir, "empty.json");
		source.Export(emptyExport);

		var target = NewManagerInFreshDir();
		target.Toggle(MakeBookmark(0x06000001, name: "Old"));

		bool ok = target.Import(emptyExport, BookmarkImportMode.Replace);

		ok.Should().BeTrue("a valid empty file is a successful import");
		target.Bookmarks.Should().BeEmpty("Replace with a valid empty file legitimately clears the list");
	}
}
