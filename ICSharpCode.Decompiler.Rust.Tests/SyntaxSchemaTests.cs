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
/// The engine's schema of the C# syntax tree (<c>csharp/syntax_schema.rs</c>) is generated from
/// the node classes; this test fails while the checked-in file differs from what the classes
/// give, and writes the new text next to it.
/// </summary>
[TestFixture]
public class SyntaxSchemaTests
{
	static string SchemaPath([CallerFilePath] string thisFile = "")
		=> Path.GetFullPath(Path.Combine(Path.Combine(Path.GetDirectoryName(thisFile)!, "..", "rust"), Path.Combine("ilspy-decompiler", "src", "csharp"), "syntax_schema.rs"));

	[Test]
	public void SchemaIsUpToDate()
	{
		string generated = SyntaxSchema.RustSource().Replace("\r\n", "\n");
		string path = SchemaPath();
		string current = File.Exists(path) ? File.ReadAllText(path).Replace("\r\n", "\n") : "";
		if (current != generated)
		{
			File.WriteAllText(path + ".new", generated);
			Assert.Fail($"{path} is out of date; the generated schema is in {path}.new");
		}
	}
}
