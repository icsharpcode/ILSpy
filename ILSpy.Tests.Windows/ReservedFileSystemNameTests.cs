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

using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;

using ICSharpCode.ILSpy.Commands;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Windows;

/// <summary>
/// Verifies on a real Windows filesystem that the sanitized names produced for reserved device
/// names (CON, PRN, AUX, NUL, COM1-9, LPT1-9) are actually creatable (issue #3775: a type named
/// "Con" crashed saving with IOException '\\.\Con'). The cross-platform expectations live in
/// ICSharpCode.Decompiler.Tests (ReservedFileSystemNameTests) and ILSpy.Tests
/// (SuggestedFileNameTests); this fixture only proves the escaped output round-trips through
/// File/Directory creation. It deliberately does not assert that the raw names fail to be
/// created: whether "con.cs" is rejected depends on the Windows version (Windows 11 allows
/// reserved names with extensions), and bare-name behavior is already covered by the escape.
/// </summary>
[TestFixture]
public class ReservedFileSystemNameTests
{
	static readonly string[] ReservedNames = [
		"AUX", "CON", "NUL", "PRN",
		"COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
		"LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9",
	];

	string tempDir = null!;

	[SetUp]
	public void SetUp()
	{
		tempDir = Path.Combine(Path.GetTempPath(), "ILSpyReserved_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
	}

	[TearDown]
	public void TearDown()
	{
		if (Directory.Exists(tempDir))
			Directory.Delete(tempDir, recursive: true);
	}

	[Test]
	public void Sanitized_Code_File_Names_Are_Creatable([ValueSource(nameof(ReservedNames))] string name)
	{
		var fileName = WholeProjectDecompiler.CleanUpFileName(name, ".cs");
		var path = Path.Combine(tempDir, fileName);

		File.WriteAllText(path, "// " + name);

		File.Exists(path).Should().BeTrue();
		// Enumerate to guard against the name silently resolving to a device instead of a file.
		Directory.EnumerateFiles(tempDir).Select(Path.GetFileName).Should().Contain(fileName);
	}

	[Test]
	public void Sanitized_Directory_Names_Are_Creatable([ValueSource(nameof(ReservedNames))] string name)
	{
		var dirName = WholeProjectDecompiler.CleanUpDirectoryName(name);
		var dir = Path.Combine(tempDir, dirName);

		Directory.CreateDirectory(dir);
		File.WriteAllText(Path.Combine(dir, "data.bin"), name);

		File.Exists(Path.Combine(dir, "data.bin")).Should().BeTrue();
		Directory.EnumerateDirectories(tempDir).Select(Path.GetFileName).Should().Contain(dirName);
	}

	[Test]
	public void Sanitized_Resource_Paths_Are_Creatable([ValueSource(nameof(ReservedNames))] string name)
	{
		// Resource names can carry directory structure; both the directory and the file segment
		// must come out creatable.
		var relativePath = WholeProjectDecompiler.SanitizeFileName(name + "/" + name + ".png");
		var path = Path.Combine(tempDir, relativePath);

		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		File.WriteAllText(path, name);

		File.Exists(path).Should().BeTrue();
	}

	[Test]
	public void Save_Code_Default_File_Name_For_A_Type_Named_Con_Is_Creatable()
	{
		// The exact crash from issue #3775: the save dialog suggested "Con.cs" and the
		// StreamWriter in SaveCodeHelper.WriteNodeToFile failed with '\\.\Con'.
		var path = Path.Combine(tempDir, SaveCodeHelper.SuggestedFileName("Con", ".cs"));

		using (var writer = new StreamWriter(path))
		{
			writer.WriteLine("// decompiled output");
		}

		File.Exists(path).Should().BeTrue();
	}
}
