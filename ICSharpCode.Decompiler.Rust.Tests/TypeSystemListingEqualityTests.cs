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
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
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

	static string Tok(EntityHandle h) => MetadataTokens.GetToken(h).ToString("x8");

	static string OptTok(IEntity? e) => e == null ? "-" : Tok(e.MetadataToken);

	static string Mods(params (bool Set, char Letter)[] flags)
		=> new string(flags.Where(f => f.Set).Select(f => f.Letter).ToArray());

	static string Value(object? v) => v switch {
		null => "null",
		bool b => "Boolean:" + (b ? "True" : "False"),
		char c => "Char:" + (int)c,
		sbyte or byte or short or ushort or int or uint or long or ulong
			=> v.GetType().Name + ":" + Convert.ToString(v, CultureInfo.InvariantCulture),
		float f => "Single:" + BitConverter.SingleToInt32Bits(f).ToString("x8"),
		double d => "Double:" + BitConverter.DoubleToInt64Bits(d).ToString("x16"),
		string s => "String:" + string.Concat(s.Select(ch => ((int)ch).ToString("x4"))),
		decimal m => "Decimal:" + string.Join(",", decimal.GetBits(m).Select(i => i.ToString("x8"))),
		_ => "?" + v.GetType().Name,
	};

	static string Params(IEnumerable<IParameter> parameters)
		=> "[" + string.Join("|", parameters.Select(p =>
			$"{Located(p.Type)},{p.Name},{p.ReferenceKind},{(p.IsOptional ? 1 : 0)},{(p.HasConstantValueInSignature ? 1 : 0)},{Value(p.GetConstantValue(throwOnInvalidMetadata: false))}")) + "]";

	static string TypeParams(IEnumerable<ITypeParameter> tps)
		=> "[" + string.Join("|", tps.Select(tp =>
			$"{tp.Name}:{tp.Variance}:{Mods((tp.HasDefaultConstructorConstraint, 'n'), (tp.HasReferenceTypeConstraint, 'r'), (tp.HasValueTypeConstraint, 'v'), (tp.AllowsRefLikeType, 'l'))}:{{{string.Join(";", tp.TypeConstraints.Select(c => Located(c.Type)))}}}")) + "]";

	static string Line(string kind, EntityHandle h, Func<string> f)
	{
		string line = S(f);
		return line == "!bad" ? $"{kind} {Tok(h)} !bad" : line;
	}

	internal static string ManagedListing(DecompilerTypeSystem ts)
	{
		var module = (MetadataModule)ts.MainModule;
		var metadata = module.MetadataFile.Metadata;
		var sb = new StringBuilder();
		void Append(string line) => sb.Append(line).Append('\n');
		foreach (var td in module.TypeDefinitions)
		{
			string line = S(() => {
				string enumType = td.Kind == TypeKind.Enum ? Located(td.EnumUnderlyingType!) : "-";
				string bases = S(() => "[" + string.Join("|", td.DirectBaseTypes.Select(Located)) + "]");
				return $"T {td.ReflectionName} kind={td.Kind} known={td.KnownTypeCode} decl={td.DeclaringTypeDefinition?.ReflectionName ?? "-"} tparams={td.TypeParameterCount} enum={enumType} base={bases}";
			});
			Append(line == "!bad" ? "T !bad" : line);
			if (line != "!bad" && td.TypeParameterCount > 0)
				Append($"G {td.ReflectionName} {S(() => TypeParams(td.TypeParameters))}");
		}
		foreach (var h in metadata.FieldDefinitions)
		{
			Append(Line("F", h, () => {
				var f = module.GetDefinition(h);
				return $"F {Tok(h)} decl={f.DeclaringTypeDefinition?.ReflectionName ?? "-"} name={f.Name} access={f.Accessibility} mods={Mods((f.IsStatic, 's'), (f.IsReadOnly, 'r'), (f.IsVolatile, 'v'), (f.IsConst, 'c'))} type={Located(f.Type)} value={Value(f.GetConstantValue(throwOnInvalidMetadata: false))}";
			}));
		}
		foreach (var h in metadata.MethodDefinitions)
		{
			Append(Line("M", h, () => {
				var m = module.GetDefinition(h);
				string accessor = m.AccessorOwner == null ? "-" : $"{Tok(m.AccessorOwner.MetadataToken)}:{(int)m.AccessorKind}";
				return $"M {Tok(h)} decl={m.DeclaringTypeDefinition?.ReflectionName ?? "-"} name={m.Name} kind={m.SymbolKind} access={m.Accessibility} mods={Mods((m.IsStatic, 's'), (m.IsAbstract, 'a'), (m.IsSealed, 'e'), (m.IsVirtual, 'v'), (m.IsOverride, 'o'), (m.HasBody, 'b'))} accessor={accessor} ret={Located(m.ReturnType)} params={Params(m.Parameters)} tparams={TypeParams(m.TypeParameters)}";
			}));
		}
		foreach (var h in metadata.PropertyDefinitions)
		{
			Append(Line("P", h, () => {
				var p = module.GetDefinition(h);
				return $"P {Tok(h)} decl={p.DeclaringTypeDefinition?.ReflectionName ?? "-"} name={p.Name} get={OptTok(p.Getter)} set={OptTok(p.Setter)} ret={Located(p.ReturnType)} params={Params(p.Parameters)}";
			}));
		}
		foreach (var h in metadata.EventDefinitions)
		{
			Append(Line("E", h, () => {
				var e = module.GetDefinition(h);
				return $"E {Tok(h)} decl={e.DeclaringTypeDefinition?.ReflectionName ?? "-"} name={e.Name} add={OptTok(e.AddAccessor)} remove={OptTok(e.RemoveAccessor)} invoke={OptTok(e.InvokeAccessor)} type={Located(e.ReturnType)}";
			}));
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
		Assert.That(lines, Has.Some.StartsWith("M ").And.Contains(" kind=Constructor "));
		Assert.That(lines, Has.Some.StartsWith("M ").And.Contains(" kind=Accessor "));
		Assert.That(lines, Has.Some.StartsWith("F ").And.Contains(" mods=sc").And.Contains("value=Int32:"));
		Assert.That(lines, Has.Some.StartsWith("P ").And.Contains(" get=0600"));
		Assert.That(lines, Has.Some.StartsWith("G ").And.Contains(":Invariant:"));
		Assert.That(lines, Has.Some.StartsWith("M ").And.Contains(",Out,"));
	}
}
