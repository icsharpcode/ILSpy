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

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Rust.Interop;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// The Rust metadata reader's listing of an image equals System.Reflection.Metadata's.
/// </summary>
[TestFixture]
public class MetadataListingEqualityTests
{
	[TestCaseSource(typeof(Corpus), nameof(Corpus.AllImages))]
	public void MetadataListingEquality(string path)
	{
		byte[] image = Corpus.ReadFile(path);
		string expected = MetadataListing.Create(image);
		string actual = NativeMethods.DebugListMembers(image);
		Assert.That(Corpus.FirstDifference(expected, actual), Is.Null);
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
}
