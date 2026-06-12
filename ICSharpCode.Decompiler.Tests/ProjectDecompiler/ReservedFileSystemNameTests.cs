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

using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.ProjectDecompiler;

/// <summary>
/// Windows reserves the device names below as file/directory names — both bare ("CON") and,
/// on many Windows versions, with an extension appended ("CON.cs" resolves to \\.\CON).
/// Every sanitizer in <see cref="WholeProjectDecompiler"/> must escape the base name (the part
/// before the first dot) with a trailing underscore so the decompiled output is creatable on
/// Windows (issue #3775).
/// </summary>
[TestFixture]
public sealed class ReservedFileSystemNameTests
{
	static readonly string[] ReservedNames = {
		"AUX", "CON", "NUL", "PRN",
		"COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
		"LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9",
	};

	static readonly char Sep = Path.DirectorySeparatorChar;

	[Test]
	public void CleanUpFileNameEscapesReservedNames([ValueSource(nameof(ReservedNames))] string name)
	{
		Assert.That(WholeProjectDecompiler.CleanUpFileName(name, ".cs"), Is.EqualTo($"{name}_.cs"));
	}

	[Test]
	public void CleanUpFileNameEscapesReservedNamesCaseInsensitively([ValueSource(nameof(ReservedNames))] string name)
	{
		string lower = name.ToLowerInvariant();
		Assert.That(WholeProjectDecompiler.CleanUpFileName(lower, ".cs"), Is.EqualTo($"{lower}_.cs"));
	}

	[Test]
	public void CleanUpFileNameEscapesTheBaseNameOfMultiDotNames()
	{
		// The underscore must land on the base name: Windows device-name parsing ignores
		// everything after the first dot, so "con.fig.cs_" would still resolve to \\.\CON.
		Assert.That(WholeProjectDecompiler.CleanUpFileName("con.fig", ".cs"), Is.EqualTo("con_.fig.cs"));
	}

	[Test]
	public void SanitizeFileNameEscapesReservedNames([ValueSource(nameof(ReservedNames))] string name)
	{
		Assert.That(WholeProjectDecompiler.SanitizeFileName(name + ".txt"), Is.EqualTo($"{name}_.txt"));
	}

	[Test]
	public void SanitizeFileNameEscapesReservedDirectorySegments([ValueSource(nameof(ReservedNames))] string name)
	{
		Assert.That(WholeProjectDecompiler.SanitizeFileName(name + "/data.bin"), Is.EqualTo($"{name}_{Sep}data.bin"));
	}

	[Test]
	public void SanitizeFileNameEscapesReservedFinalSegments([ValueSource(nameof(ReservedNames))] string name)
	{
		Assert.That(WholeProjectDecompiler.SanitizeFileName($"dir/{name}.png"), Is.EqualTo($"dir{Sep}{name}_.png"));
	}

	[Test]
	public void CleanUpDirectoryNameEscapesReservedNames([ValueSource(nameof(ReservedNames))] string name)
	{
		Assert.That(WholeProjectDecompiler.CleanUpDirectoryName(name), Is.EqualTo(name + "_"));
	}

	[Test]
	public void CleanUpPathEscapesReservedLeadingSegments([ValueSource(nameof(ReservedNames))] string name)
	{
		Assert.That(WholeProjectDecompiler.CleanUpPath(name + ".Foo"), Is.EqualTo($"{name}_{Sep}Foo"));
	}

	[Test]
	public void CleanUpPathEscapesReservedTrailingSegments([ValueSource(nameof(ReservedNames))] string name)
	{
		Assert.That(WholeProjectDecompiler.CleanUpPath("Foo." + name), Is.EqualTo($"Foo{Sep}{name}_"));
	}

	[Test]
	[TestCase("Console")]
	[TestCase("CONtoso")]
	[TestCase("con1")]
	[TestCase("com10")]
	[TestCase("lpt10")]
	public void NamesMerelyStartingWithReservedNamesAreNotEscaped(string name)
	{
		using (Assert.EnterMultipleScope())
		{
			Assert.That(WholeProjectDecompiler.CleanUpFileName(name, ".cs"), Is.EqualTo(name + ".cs"));
			Assert.That(WholeProjectDecompiler.SanitizeFileName(name + ".txt"), Is.EqualTo(name + ".txt"));
			Assert.That(WholeProjectDecompiler.CleanUpDirectoryName(name), Is.EqualTo(name));
		}
	}

	[Test]
	public void OrdinaryPathsPassThroughUnchanged()
	{
		using (Assert.EnterMultipleScope())
		{
			Assert.That(WholeProjectDecompiler.SanitizeFileName("config.txt"), Is.EqualTo("config.txt"));
			Assert.That(WholeProjectDecompiler.SanitizeFileName("dir/file.png"), Is.EqualTo($"dir{Sep}file.png"));
			Assert.That(WholeProjectDecompiler.CleanUpPath("Foo.Bar"), Is.EqualTo($"Foo{Sep}Bar"));
		}
	}
}
