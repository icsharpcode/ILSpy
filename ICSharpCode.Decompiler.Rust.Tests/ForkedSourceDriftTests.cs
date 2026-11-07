// Copyright (c) 2026 Christoph Wille
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
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// The shim compiles its own copy of CSharp/CSharpDecompiler.cs. Outside the RUST-SEAM blocks
/// that copy must stay identical to the original, so upstream edits surface here.
/// </summary>
[TestFixture]
public class ForkedSourceDriftTests
{
	static readonly Regex SeamBlock = new(
		@"^[ \t]*// RUST-SEAM-(?<n>\d+) BEGIN\n.*?^[ \t]*// RUST-SEAM-\k<n> END\n",
		RegexOptions.Multiline | RegexOptions.Singleline);

	static string RepositoryRoot([CallerFilePath] string thisFile = "")
		=> Path.GetFullPath(Path.Combine(Path.GetDirectoryName(thisFile)!, ".."));

	static string ReadNormalized(string path) => File.ReadAllText(path).Replace("\r\n", "\n");

	[Test]
	public void CSharpDecompilerMatchesOriginalOutsideSeams()
	{
		string root = RepositoryRoot();
		string original = ReadNormalized(Path.Combine(root, "ICSharpCode.Decompiler", "CSharp", "CSharpDecompiler.cs"));
		string fork = ReadNormalized(Path.Combine(root, "ICSharpCode.Decompiler.Rust", "CSharp", "CSharpDecompiler.cs"));

		Assert.That(SeamBlock.Matches(fork), Has.Count.EqualTo(2), "expected the RUST-SEAM-1 and RUST-SEAM-2 blocks");
		Assert.That(SeamBlock.Replace(fork, ""), Is.EqualTo(original));
	}
}
