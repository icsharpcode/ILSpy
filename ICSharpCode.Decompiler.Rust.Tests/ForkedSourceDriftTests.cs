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
using System.Linq;
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

		// Seam 1 has four blocks: a field, the analysis body, the handover before the RunTransforms
		// call and the restore after it. Seam 2 has twelve: the AST transform loop, the engine's choice of hidden top-level types with the label after the managed check it skips, the engine's extension
		// (of a marker type and of a container's block), type and method declarations and body translation with the label after the statement builder it
		// skips, and the engine's field, property and event declarations; each declaration
		// replaces the managed one.
		var blocks = SeamBlock.Matches(fork);
		Assert.That(blocks.Select(m => m.Groups["n"].Value), Is.EqualTo(new[] { "1", "1", "2", "2", "2", "2", "2", "2", "2", "1", "1", "2", "2", "2", "2", "2" }), "the RUST-SEAM blocks in file order");
		Assert.That(SeamBlock.Replace(fork, ""), Is.EqualTo(original));
	}
}
