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
using System.Linq.Expressions;
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
/// Bodies of lifted and constant operators, unsafe code, spans, indexes and inline arrays the
/// engine translates itself: each decompiles to the same C# as the managed statement builder
/// writes, without the engine falling back.
/// </summary>
[TestFixture, Parallelizable(ParallelScope.All)]
public class Seam2UnsafeAndSpanTests
{
	[InlineArray(4)]
	struct Buffer4
	{
		int element;
	}

	ref struct RefHolder
	{
		public ref int Target;

		public RefHolder(ref int target)
		{
			Target = ref target;
		}
	}

	static bool? LiftedNot(bool? a) => !a;

	static int? CoalesceWidening(short? a, int? b) => a ?? b;

	static Expression<Func<decimal, bool>> DecimalComparison() => d => d > 0m;

	static string AppendToString(string s, int i)
	{
		s += i.ToString();
		return s;
	}

	static string AppendChar(string s, char c)
	{
		s += c;
		return s;
	}

	static int SpanCreation(int[] a) => new Span<int>(a).Length;

	static object IndexFromEnd(int i) => ^i;

	static int ArrayElementAtIndex(int[] a, int i)
	{
		var index = ^i;
		return a[index];
	}

	static int ReadRefField(RefHolder h) => h.Target;

	static int InlineArraySpan(Buffer4 b)
	{
		Span<int> s = b;
		return s.Length;
	}

	static unsafe long PointerDifference(int* p, int* q) => (long)p - (long)q;

	static unsafe int StackAllocElement(int a, int b)
	{
		int* p = stackalloc int[3];
		p[2] = a;
		return p[2] + b;
	}

	[TestCase(nameof(LiftedNot))]
	[TestCase(nameof(CoalesceWidening))]
	[TestCase(nameof(DecimalComparison))]
	[TestCase(nameof(AppendToString))]
	[TestCase(nameof(AppendChar))]
	[TestCase(nameof(SpanCreation))]
	[TestCase(nameof(IndexFromEnd))]
	[TestCase(nameof(ArrayElementAtIndex))]
	[TestCase(nameof(ReadRefField))]
	[TestCase(nameof(InlineArraySpan))]
	[TestCase(nameof(PointerDifference))]
	[TestCase(nameof(StackAllocElement))]
	public void EngineServesBody(string name)
	{
		var method = typeof(Seam2UnsafeAndSpanTests).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!;
		var handle = (MethodDefinitionHandle)MetadataTokens.EntityHandle(method.MetadataToken);
		string path = typeof(Seam2UnsafeAndSpanTests).Assembly.Location;
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
