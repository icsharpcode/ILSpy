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
using System.Linq;
using System.Text;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Rust.Interop;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// The Rust type system's view of the main module equals the link-compiled
/// DecompilerTypeSystem's (TypeSystemOptions.None) over the same reference closure.
/// The format is defined in rust/ilspy-decompiler/src/typesystem/listing.rs.
/// </summary>
[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class TypeSystemListingEqualityTests
{
	internal static DecompilerTypeSystem Load(string path, TypeSystemOptions options)
	{
		var file = new PEFile(path);
		var resolver = new UniversalAssemblyResolver(path, false, file.DetectTargetFrameworkId());
		return new DecompilerTypeSystem(file, resolver, options);
	}

	static string S(Func<string> f)
	{
		try
		{
			return f();
		}
		catch (Exception)
		{
			return "!bad";
		}
	}

	/// <summary>
	/// The reflection name plus the assembly that defines the type (or the generic type of a
	/// parameterized type); <c>?</c> when it resolved to nothing or only to a module without
	/// metadata, which the engine is not given.
	/// </summary>
	static string Located(IType type)
	{
		var module = type.GetDefinition()?.ParentModule as MetadataModule;
		return type.ReflectionName + "@" + (module?.MetadataFile is PEFile ? module.AssemblyName : "?");
	}

	internal static string ManagedListing(DecompilerTypeSystem ts)
	{
		var sb = new StringBuilder();
		foreach (var td in ((MetadataModule)ts.MainModule).TypeDefinitions)
		{
			string line = S(() => {
				string enumType = td.Kind == TypeKind.Enum ? Located(td.EnumUnderlyingType!) : "-";
				string bases = S(() => "[" + string.Join("|", td.DirectBaseTypes.Select(Located)) + "]");
				return $"T {td.ReflectionName} kind={td.Kind} known={td.KnownTypeCode} decl={td.DeclaringTypeDefinition?.ReflectionName ?? "-"} tparams={td.TypeParameterCount} enum={enumType} base={bases}";
			});
			sb.Append(line == "!bad" ? "T !bad" : line).Append('\n');
		}
		return sb.ToString();
	}

	[TestCaseSource(typeof(Corpus), nameof(Corpus.ManagedImages))]
	public void TypeSystemListingEquality(string path)
	{
		var ts = Load(path, TypeSystemOptions.None);
		using var table = new RustModuleTable(ts);
		Assert.That(Corpus.FirstDifference(ManagedListing(ts), table.DebugTypeListing()), Is.Null);
	}

	[Test]
	public void ListingCoversNestedGenericAndInterfaceTypes()
	{
		var ts = Load(typeof(CSharpDecompiler).Assembly.Location, TypeSystemOptions.None);
		using var table = new RustModuleTable(ts);
		string[] lines = table.DebugTypeListing().Split('\n');
		Assert.That(lines, Has.Some.StartsWith("T ICSharpCode.Decompiler.CSharp.CSharpDecompiler kind=Class"));
		Assert.That(lines, Has.Some.Contains("kind=Interface").And.Contains("base=[System.Object"));
		Assert.That(lines, Has.Some.Contains("+").And.Contains("decl=ICSharpCode."));
		Assert.That(lines, Has.Some.Contains("kind=Enum").And.Contains("enum=System.Int32"));
		Assert.That(lines, Has.Some.Contains("`1[[").And.StartsWith("T "));
		Assert.That(lines, Has.None.Contains("!bad"));
		// Resolution across the closure, including netstandard's forwarders into the runtime.
		Assert.That(lines, Has.Some.Matches(@"base=\[System\.Object@[^?|\]]"));
		Assert.That(lines.Count(l => l.Contains("@?")), Is.LessThan(lines.Length / 10));
	}
}
