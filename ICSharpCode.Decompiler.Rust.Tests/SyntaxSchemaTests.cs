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
using System.IO;
using System.Runtime.CompilerServices;

using ICSharpCode.Decompiler.Rust.Interop;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// The engine's schema of the C# syntax tree (<c>csharp/syntax_schema.rs</c>) and the declaration
/// order the host sorts node members by (<c>Interop/SyntaxSchema.Order.cs</c>) are generated from
/// the node classes; these tests fail while a checked-in file differs from what the classes give,
/// and write the new text next to it.
/// </summary>
[TestFixture]
public class SyntaxSchemaTests
{
	static string SchemaPath([CallerFilePath] string thisFile = "")
		=> Path.GetFullPath(Path.Combine(Path.Combine(Path.GetDirectoryName(thisFile)!, "..", "rust"), Path.Combine("ilspy-decompiler", "src", "csharp"), "syntax_schema.rs"));

	static string InteropPath(string fileName, [CallerFilePath] string thisFile = "")
		=> Path.GetFullPath(Path.Combine(Path.GetDirectoryName(thisFile)!, "..", "ICSharpCode.Decompiler.Rust", "Interop", fileName));

	[Test]
	public void SchemaIsUpToDate()
		=> AssertUpToDate(SchemaPath(), SyntaxSchema.RustSource());

	/// <summary>
	/// The order is generated because reflection returns members in no guaranteed order and Native
	/// AOT has no metadata tokens to sort them by. Its license header is the one of SyntaxSchema.cs.
	/// </summary>
	[Test]
	public void DeclarationOrderIsUpToDate()
	{
		string schema = File.ReadAllText(InteropPath("SyntaxSchema.cs")).Replace("\r\n", "\n");
		string header = schema.Substring(0, schema.IndexOf("\n\n", StringComparison.Ordinal) + 2);
		AssertUpToDate(InteropPath("SyntaxSchema.Order.cs"), SyntaxSchema.OrderSource(header));
	}

	static void AssertUpToDate(string path, string generated)
	{
		generated = generated.Replace("\r\n", "\n");
		string current = File.Exists(path) ? File.ReadAllText(path).Replace("\r\n", "\n") : "";
		if (current != generated)
		{
			File.WriteAllText(path + ".new", generated);
			Assert.Fail($"{path} is out of date; the generated text is in {path}.new");
		}
	}
}
