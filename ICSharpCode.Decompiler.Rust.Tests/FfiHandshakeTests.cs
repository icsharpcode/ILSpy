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

using ICSharpCode.Decompiler.Rust.Interop;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

[TestFixture]
public class FfiHandshakeTests
{
	static string CargoWorkspaceVersion([CallerFilePath] string thisFile = "")
	{
		string manifest = File.ReadAllText(Path.Combine(Path.GetDirectoryName(thisFile)!, "..", "rust", "Cargo.toml"));
		Match m = Regex.Match(manifest, @"^\[workspace\.package\][^\[]*?^version\s*=\s*""([^""]+)""", RegexOptions.Multiline | RegexOptions.Singleline);
		Assert.That(m.Success, "rust/Cargo.toml has no [workspace.package] version");
		return m.Groups[1].Value;
	}

	[Test]
	public void VersionIsTheCrateVersion()
	{
		Assert.That(NativeMethods.Version, Is.EqualTo(CargoWorkspaceVersion()));
	}

	[Test]
	public void SessionCanBeCreatedAndFreed()
	{
		using var session = RustSession.Create();
		Assert.That(session.IsInvalid, Is.False);
	}
}
