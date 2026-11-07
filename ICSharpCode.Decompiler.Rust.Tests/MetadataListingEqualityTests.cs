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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Rust.Interop;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// The Rust metadata reader's listing of an assembly equals System.Reflection.Metadata's,
/// for the decompiler itself, every assembly of the running shared framework and, when a run of
/// ICSharpCode.Decompiler.Tests has compiled them, every test case fixture.
/// </summary>
[TestFixture]
public class MetadataListingEqualityTests
{
	static string TestCasesDirectory([CallerFilePath] string thisFile = "")
		=> Path.GetFullPath(Path.Combine(Path.GetDirectoryName(thisFile)!, "..", "ICSharpCode.Decompiler.Tests", "TestCases"));

	static IEnumerable<string> Assemblies(string directory, SearchOption option)
		=> Directory.Exists(directory)
			? Directory.EnumerateFiles(directory, "*.*", option)
				.Where(f => f.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
			: [];

	public static IEnumerable<TestCaseData> Images()
	{
		yield return new TestCaseData(typeof(CSharpDecompiler).Assembly.Location).SetArgDisplayNames("ICSharpCode.Decompiler.dll");
		string runtime = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
		foreach (var file in Assemblies(runtime, SearchOption.TopDirectoryOnly).Order())
			yield return new TestCaseData(file).SetArgDisplayNames("runtime/" + Path.GetFileName(file));
		string fixtures = TestCasesDirectory();
		foreach (var file in Assemblies(fixtures, SearchOption.AllDirectories).Order())
			yield return new TestCaseData(file).SetArgDisplayNames(Path.GetRelativePath(fixtures, file).Replace('\\', '/'));
	}

	[TestCaseSource(nameof(Images))]
	public void MetadataListingEquality(string path)
	{
		byte[] image = File.ReadAllBytes(path);
		string expected = MetadataListing.Create(image);
		string actual = NativeMethods.DebugListMembers(image);
		Assert.That(FirstDifference(expected, actual), Is.Null);
	}

	[Test]
	public void ListingCoversTypesSignaturesAndBodies()
	{
		string listing = NativeMethods.DebugListMembers(File.ReadAllBytes(typeof(CSharpDecompiler).Assembly.Location));
		string[] lines = listing.Split('\n');
		Assert.That(lines, Has.Some.Contains("name=CSharpDecompiler ns=ICSharpCode.Decompiler.CSharp"));
		Assert.That(lines, Has.Some.StartsWith("BODY ").And.Contains(" il="));
		Assert.That(lines, Has.Some.StartsWith("EH "));
		Assert.That(lines, Has.Some.StartsWith("METHOD ").And.Contains("System.Collections.Generic.List`1<"));
		Assert.That(lines.Count(l => l.StartsWith("TYPEDEF ")), Is.GreaterThan(500));
		Assert.That(listing, Does.Not.Contain("!bad"));
	}

	static string? FirstDifference(string expected, string actual)
	{
		string[] e = expected.Split('\n'), a = actual.Split('\n');
		int i = 0;
		while (i < e.Length && i < a.Length && e[i] == a[i])
			i++;
		if (i == e.Length && i == a.Length)
			return null;
		return $"line {i + 1}:\n  SRM:  {(i < e.Length ? e[i] : "<end>")}\n  Rust: {(i < a.Length ? a[i] : "<end>")}";
	}
}
