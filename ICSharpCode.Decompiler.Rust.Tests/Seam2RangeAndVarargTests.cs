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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Rust.Interop;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// Bodies of ranges, slices, vararg calls, function pointers, params spans and span helper
/// calls the engine translates itself: each decompiles to the same C# as the managed statement
/// builder writes, without the engine falling back.
/// </summary>
[TestFixture, Parallelizable(ParallelScope.All)]
public class Seam2RangeAndVarargTests
{
	[InlineArray(4)]
	struct Buffer4
	{
		int element;
	}

	static object RangeOf(int a, int b) => a..^b;

	static object RangeFrom(int a) => a..;

	static object RangeTo(int b) => ..^b;

	static object WholeRange() => ..;

	static string StringSlice(string s, int a)
	{
		var r = a..^1;
		return s[r];
	}

	static int InlineArraySlice(Buffer4 b)
	{
		Span<int> s = b[..2];
		return s.Length;
	}

	static int ParamsSpan(params ReadOnlySpan<object> items) => items.Length;

	static int ParamsSpanOfOne(object o) => ParamsSpan(o);

	static ReadOnlySpan<object> CastUp(ReadOnlySpan<string> s) => s;

	static int VarArgs(int a, __arglist) => new ArgIterator(__arglist).GetRemainingCount() + a;

	static int VarArgCall() => VarArgs(1, __arglist("a", 2, (object)"b"));

	static int Twice(int x) => x * 2;

	static unsafe delegate*<int, int> FunctionPointer() => &Twice;

	static unsafe int FunctionPointerCall(int x)
	{
		delegate*<int, int> f = &Twice;
		return f(x);
	}

	static unsafe R GenericFunctionPointer<T, R>(delegate*<T, R> f, T arg) => f(arg);

	static unsafe char Test(int i) => (char)i;

	static unsafe char FunctionPointerArgument() => GenericFunctionPointer((delegate*<int, char>)(&Test), 1);

	[TestCase(nameof(FunctionPointerArgument))]
	[TestCase(nameof(RangeOf))]
	[TestCase(nameof(RangeFrom))]
	[TestCase(nameof(RangeTo))]
	[TestCase(nameof(WholeRange))]
	[TestCase(nameof(StringSlice))]
	[TestCase(nameof(InlineArraySlice))]
	[TestCase(nameof(ParamsSpanOfOne))]
	[TestCase(nameof(CastUp))]
	[TestCase(nameof(VarArgCall))]
	[TestCase(nameof(FunctionPointer))]
	[TestCase(nameof(FunctionPointerCall))]
	public void EngineServesBody(string name)
	{
		var method = typeof(Seam2RangeAndVarargTests).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!;
		var handle = (MethodDefinitionHandle)MetadataTokens.EntityHandle(method.MetadataToken);
		string path = typeof(Seam2RangeAndVarargTests).Assembly.Location;
		string managed = DecompileMethod(path, handle, engineBodies: false);
		RustSeam2.LastFallbackReason = null;
		string engine = DecompileMethod(path, handle, engineBodies: true);
		Assert.That(RustSeam2.LastFallbackReason, Is.Null);
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
}
