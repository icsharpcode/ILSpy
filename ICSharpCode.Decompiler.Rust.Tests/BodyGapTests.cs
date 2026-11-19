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

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Rust.Interop;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

static class BodyGapExtensions
{
	public static int ExtensionLength(this string? s) => s?.Length ?? 0;
}

class BodyGapBase
{
	public int Value() => 1;
}

class BodyGapDerived : BodyGapBase
{
	public new int Value() => 2;
}

record BodyGapRecord(int X, string Y);

/// <summary>
/// Bodies of the calls, casts, arguments and object creations the engine translates itself:
/// each decompiles to the same C# as the managed statement builder writes, without the
/// engine falling back.
/// </summary>
[TestFixture, Parallelizable(ParallelScope.All)]
public class BodyGapTests
{
	static object AnonymousInferred(int a, string b) => new { a, b };

	static object AnonymousNamed(int a, string b) => new { X = a, Y = b + "!" };

	static int ExtensionOnNull() => ((string?)null).ExtensionLength();

	static char DecimalToChar(object o) => (char)(decimal)o;

	static Func<int> HiddenMethodReference(BodyGapDerived d) => ((BodyGapBase)d).Value;

	static object WithInitializer(BodyGapRecord r) => r with { X = 2 };

	static string FormatOfArrayLiteral(int x) => string.Format("value {0}", new object[] { x });

	static void TakesDynamic(dynamic d) { }

	static object DynamicMember(dynamic d) => d.Name;

	static void DynamicSetIndexAndMember(dynamic d, int i)
	{
		d[i, "k"] = d.Value + 1;
		d.Other = -d;
	}

	static int DynamicConvert(dynamic d) => (int)d;

	static string StaticConversionFromDynamic(dynamic d) => (string)(object)d;

	static void ArgumentOfDynamicParameter() => TakesDynamic(1);

	static void DynamicCompound(dynamic d)
	{
		d.X += 1;
		d *= 2;
	}

	static object DynamicInvocations(dynamic d)
	{
		Console.WriteLine(d);
		d.Generic<int>(1, name: "n");
		return new BodyGapRecord(d, "x");
	}

	static bool DynamicConditions(dynamic d)
	{
		if (d)
			return !d;
		return d && d.Flag ? true : false;
	}

	[TestCase(nameof(DynamicMember))]
	[TestCase(nameof(DynamicSetIndexAndMember))]
	[TestCase(nameof(DynamicConvert))]
	[TestCase(nameof(StaticConversionFromDynamic))]
	[TestCase(nameof(ArgumentOfDynamicParameter))]
	[TestCase(nameof(DynamicCompound))]
	[TestCase(nameof(DynamicInvocations))]
	[TestCase(nameof(DynamicConditions))]
	[TestCase(nameof(WithInitializer))]
	[TestCase(nameof(FormatOfArrayLiteral))]
	[TestCase(nameof(AnonymousInferred))]
	[TestCase(nameof(AnonymousNamed))]
	[TestCase(nameof(ExtensionOnNull))]
	[TestCase(nameof(DecimalToChar))]
	[TestCase(nameof(HiddenMethodReference))]
	public void EngineServesBody(string name)
	{
		var method = typeof(BodyGapTests).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!;
		var handle = (MethodDefinitionHandle)MetadataTokens.EntityHandle(method.MetadataToken);
		string path = typeof(BodyGapTests).Assembly.Location;
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
