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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Rust.Interop;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// G-ast(0): every fixture image, and the decompiler itself, decompiles to the same C# whether
/// the engine translates the method bodies (seam 2) or the managed statement builder does. All
/// AST transforms run managed on both sides.
/// </summary>
[TestFixture, Parallelizable(ParallelScope.All)]
public class AstPrefixEqualityTests
{
	static IEnumerable<TestCaseData> Images()
		=> Corpus.ManagedImages().Where(c => {
			var path = (string)c.Arguments[0]!;
			return path == typeof(CSharpDecompiler).Assembly.Location
				|| path.StartsWith(Corpus.TestCasesDirectory(), StringComparison.OrdinalIgnoreCase);
		});

	[TestCaseSource(nameof(Images))]
	public void AstPrefixEquality(string path)
	{
		string managed = Decompile(path, engineBodies: false);
		string engine = Decompile(path, engineBodies: true);
		Assert.That(Corpus.FirstDifference(managed, engine), Is.Null);
	}

	// The throw expression keeps the deconstruction a tuple local, whose type is written out.
	static int IndexerOfNamedTuple(Dictionary<int, (string? Function, string? Error)> map, int key, out string? function)
	{
		var (read, error) = map[key];
		function = read ?? throw new InvalidOperationException(error ?? "no function");
		return key;
	}

	/// <summary>
	/// An indexer of a generic type returns its type argument with the tuple element names and
	/// nullability the target's type gives it, which the stack slot holding the result takes.
	/// </summary>
	[Test]
	public void IndexerResultKeepsTupleNames()
	{
		string path = typeof(AstPrefixEqualityTests).Assembly.Location;
		var handle = (MethodDefinitionHandle)MetadataTokens.EntityHandle(
			typeof(AstPrefixEqualityTests).GetMethod(nameof(IndexerOfNamedTuple), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.MetadataToken);
		string managed = DecompileMethod(path, handle, engineBodies: false);
		string engine = DecompileMethod(path, handle, engineBodies: true);
		Assert.That(managed, Does.Contain("(string? Function, string? Error)"));
		Assert.That(engine, Is.EqualTo(managed));
	}

	static string DecompileMethod(string path, MethodDefinitionHandle handle, bool engineBodies)
	{
		RustSeam2.Enabled = engineBodies;
		try
		{
			var module = new PEFile(path, new MemoryStream(Corpus.ReadFile(path)));
			var resolver = new UniversalAssemblyResolver(path, false, module.DetectTargetFrameworkId());
			var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());
			return decompiler.DecompileAsString(handle);
		}
		finally
		{
			RustSeam2.Enabled = true;
		}
	}

	static string Decompile(string path, bool engineBodies)
	{
		RustSeam2.Enabled = engineBodies;
		try
		{
			var module = new PEFile(path, new MemoryStream(Corpus.ReadFile(path)));
			var resolver = new UniversalAssemblyResolver(path, false, module.DetectTargetFrameworkId());
			var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());
			return decompiler.DecompileWholeModuleAsString();
		}
		catch (Exception ex)
		{
			// A module neither side can decompile still has to fail alike.
			return ex.GetType().FullName + ": " + ex.Message;
		}
		finally
		{
			RustSeam2.Enabled = true;
		}
	}
}
