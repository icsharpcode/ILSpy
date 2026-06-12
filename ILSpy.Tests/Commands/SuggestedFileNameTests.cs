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

using AwesomeAssertions;

using ICSharpCode.ILSpy.Commands;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// The default file name offered by the save dialog must never be a reserved Windows device
/// name (CON, PRN, AUX, NUL, COM1-9, LPT1-9) — Windows cannot create such files, so a type
/// named "Con" would make the save crash with an IOException (issue #3775). The suggestion is
/// produced by <c>WholeProjectDecompiler.CleanUpFileName</c>, whose sanitization rules are
/// covered in depth by ICSharpCode.Decompiler.Tests (ReservedFileSystemNameTests); these tests
/// pin the delegation and the fallback for nodes without usable text.
/// </summary>
[TestFixture]
public class SuggestedFileNameTests
{
	static readonly string[] ReservedNames = [
		"AUX", "CON", "NUL", "PRN",
		"COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
		"LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9",
	];

	[Test]
	public void Save_Code_Escapes_Reserved_Device_Names([ValueSource(nameof(ReservedNames))] string name)
	{
		// "Con" + ".cs" would resolve to \\.\CON on Windows, so the escape must end up on the
		// base name, before the extension.
		SaveCodeHelper.SuggestedFileName(name, ".cs").Should().Be(name + "_.cs");
		SaveCodeHelper.SuggestedFileName(name.ToLowerInvariant(), ".cs").Should().Be(name.ToLowerInvariant() + "_.cs");
	}

	[Test]
	public void Save_Code_Keeps_Ordinary_Type_Names_Unchanged()
	{
		SaveCodeHelper.SuggestedFileName("Console", ".cs").Should().Be("Console.cs");
	}

	[Test]
	public void Save_Code_Replaces_Invalid_File_Name_Characters()
	{
		SaveCodeHelper.SuggestedFileName("a/b", ".cs").Should().Be("a-b.cs");
	}

	[Test]
	public void Save_Code_Falls_Back_To_Output_For_Unusable_Text()
	{
		SaveCodeHelper.SuggestedFileName(null, ".cs").Should().Be("output.cs");
		SaveCodeHelper.SuggestedFileName("   ", ".cs").Should().Be("output.cs");
	}
}
