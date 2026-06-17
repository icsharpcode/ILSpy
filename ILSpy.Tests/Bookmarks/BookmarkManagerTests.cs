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

	static Bookmark MakeBookmark(uint token, BookmarkKind kind = BookmarkKind.Token, int ilOffset = 0, string name = "")
		=> new() {
			Name = name,
			FileName = @"C:\asm\Sample.dll",
			AssemblyFullName = "Sample, Version=1.0.0.0",
			ModuleName = "Sample.dll",
			Token = token,
			Kind = kind,
			ILOffset = ilOffset,
			MemberName = "Sample.Type.Member",
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
}
