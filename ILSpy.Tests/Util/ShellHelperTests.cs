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

using System.IO;
using System.Linq;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Util;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class ShellHelperTests
{
	// Revealing several selected assemblies must collapse to one file-manager window per
	// containing folder (the WPF SHOpenFolderAndSelectItems behaviour), not one window per
	// file. GroupByFolder is the pure seam that produces those groups.

	static string Combine(params string[] parts) => Path.Combine(parts);

	[Test]
	public void Files_In_The_Same_Folder_Form_A_Single_Group()
	{
		var dir = Combine("root", "bin");
		var a = Combine(dir, "A.dll");
		var b = Combine(dir, "B.dll");

		var groups = ShellHelper.GroupByFolder(new[] { a, b });

		groups.Should().HaveCount(1);
		groups[0].Folder.Should().Be(dir);
		groups[0].Files.Should().Equal(a, b);
	}

	[Test]
	public void Files_In_Different_Folders_Form_Separate_Groups_In_First_Seen_Order()
	{
		var dir1 = Combine("root", "one");
		var dir2 = Combine("root", "two");
		var a = Combine(dir1, "A.dll");
		var b = Combine(dir2, "B.dll");
		var c = Combine(dir1, "C.dll");

		var groups = ShellHelper.GroupByFolder(new[] { a, b, c });

		// dir1 seen first; C.dll joins dir1's existing group rather than starting a new one.
		groups.Select(g => g.Folder).Should().Equal(dir1, dir2);
		groups[0].Files.Should().Equal(a, c);
		groups[1].Files.Should().Equal(b);
	}

	[Test]
	public void Duplicate_Paths_Are_Removed_Case_Insensitively()
	{
		var dir = Combine("root", "bin");
		var a = Combine(dir, "A.dll");

		var groups = ShellHelper.GroupByFolder(new[] { a, a.ToUpperInvariant() });

		groups.Should().HaveCount(1);
		groups[0].Files.Should().Equal(a);
	}

	[Test]
	public void Empty_And_Directory_Less_Entries_Are_Skipped()
	{
		var dir = Combine("root", "bin");
		var a = Combine(dir, "A.dll");

		// null, empty, and a bare file name (no containing directory) are dropped.
		var groups = ShellHelper.GroupByFolder(new[] { null, "", "bare.dll", a });

		groups.Should().HaveCount(1);
		groups[0].Folder.Should().Be(dir);
		groups[0].Files.Should().Equal(a);
	}

	[Test]
	public void No_Usable_Paths_Yields_No_Groups()
	{
		ShellHelper.GroupByFolder(System.Array.Empty<string>()).Should().BeEmpty();
		ShellHelper.GroupByFolder(null!).Should().BeEmpty();
	}
}
