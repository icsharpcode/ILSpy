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
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Rust.Interop;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

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
	static string Located(IType type) => type.ReflectionName + "@" + Assembly(type);

	static string Assembly(IType type)
	{
		var module = type.GetDefinition()?.ParentModule as MetadataModule;
		return module?.MetadataFile is PEFile ? module.AssemblyName : "?";
	}

	static string Mark(Nullability n) => n switch {
		Nullability.NotNullable => "!",
		Nullability.Nullable => "?",
		_ => "",
	};

	/// <summary>
	/// A type with the structure ReflectionName hides: tuple element names, nullability and
	/// function pointer signatures (format in rust/ilspy-decompiler/src/typesystem/listing.rs).
	/// </summary>
	static string Describe(IType t) => t switch {
		TupleType tt => "(" + string.Join(",", tt.ElementTypes.Select((e, i) => Describe(e) + (tt.ElementNames[i] is string n ? ":" + n : ""))) + ")",
		NullabilityAnnotatedType nat => Describe(nat.TypeWithoutAnnotation) + Mark(nat.Nullability),
		ParameterizedType pt => Describe(pt.GenericType) + "[" + string.Join(",", pt.TypeArguments.Select(Describe)) + "]",
		ArrayType at => Describe(at.ElementType) + "[" + new string(',', at.Dimensions - 1) + "]" + Mark(at.Nullability),
		PointerType p => Describe(p.ElementType) + "*",
		ByReferenceType b => Describe(b.ElementType) + "&",
		PinnedType p => Describe(p.ElementType) + " pinned",
		FunctionPointerType f => $"delegate*[{(int)f.CallingConvention};{string.Join(",", f.CustomCallingConventions.Select(Describe))};{Describe(f.ReturnType)}{(f.ReturnIsRefReadOnly ? " ro" : "")};{string.Join(",", f.ParameterTypes.Zip(f.ParameterReferenceKinds, (p, k) => Describe(p) + ":" + (int)k))}]@{Assembly(f)}",
		_ => Located(t),
	};

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
			$"{Describe(p.Type)},{p.Name},{p.ReferenceKind},{(p.IsOptional ? 1 : 0)},{(p.HasConstantValueInSignature ? 1 : 0)},{Value(p.GetConstantValue(throwOnInvalidMetadata: false))},{(p.IsParams ? 1 : 0)},{p.Lifetime.DeclaredScope},{(p.Lifetime.HasUnscopedRefAttribute ? 1 : 0)},{p.GetEffectiveScope()}")) + "]";

	static string TypeParams(IEnumerable<ITypeParameter> tps)
		=> "[" + string.Join("|", tps.Select(tp =>
			$"{tp.Name}:{tp.Variance}:{Mods((tp.HasDefaultConstructorConstraint, 'n'), (tp.HasReferenceTypeConstraint, 'r'), (tp.HasValueTypeConstraint, 'v'), (tp.AllowsRefLikeType, 'l'), (tp.HasUnmanagedConstraint, 'u'))}:{(int)tp.NullabilityConstraint}:{(tp.IsReferenceType == null ? "-" : tp.IsReferenceType.Value.ToString())}:{{{string.Join(";", tp.TypeConstraints.Select(c => Describe(c.Type)))}}}")) + "]";

	static string Origin(IMember member)
		=> member.MemberDefinition.MetadataToken.IsNil ? "fake" : Tok(member.MemberDefinition.MetadataToken);

	static string Types(IEnumerable<IType> types) => "[" + string.Join("|", types.Select(Describe)) + "]";

	static string MethodRef(IMethod m)
		=> $"method={Origin(m)} decl={(m.DeclaringType == null ? "-" : Describe(m.DeclaringType))} name={m.Name} static={(m.IsStatic ? 1 : 0)} targs={Types(m.TypeArguments)} ret={Describe(m.ReturnType)} params={Types(m.Parameters.Select(p => p.Type))} accessor={(int)m.AccessorKind}";

	static string FieldRef(IField f)
		=> $"field={Origin(f)} decl={(f.DeclaringType == null ? "-" : Describe(f.DeclaringType))} name={f.Name} type={Describe(f.ReturnType)}";

	static string Members(IEnumerable<IMember> members)
		=> "[" + string.Join("|", members.Select(m => Origin(m) + ":" + (m.DeclaringType == null ? "-" : Describe(m.DeclaringType)))) + "]";

	static string Lookup(ITypeDefinition td)
	{
		var invoke = td.GetDelegateInvokeMethod();
		return $"nested={Types(td.GetNestedTypes())} ctors={Members(td.GetConstructors())} methods={Members(td.GetMethods())} props={Members(td.GetProperties())} fields={Members(td.GetFields())} events={Members(td.GetEvents())} invoke={(invoke == null ? "-" : Members([invoke]))}";
	}

	static string Inheritance(IMember m)
		=> $"ovr={(m.IsOverridable ? 1 : 0)} eii={(m.IsExplicitInterfaceImplementation ? 1 : 0)} impls={Members(m.ExplicitlyImplementedInterfaceMembers)}";

	static string AttributeEntry(IAttribute a)
	{
		string ctor = a.Constructor == null ? "-" : Origin(a.Constructor);
		var named = a.NamedArguments.Select(n => $"{(n.Kind == CustomAttributeNamedArgumentKind.Field ? "F" : "P")}:{n.Name}:{AttributeArgument(n.Type, n.Value)}");
		return $"{Describe(a.AttributeType)}({ctor}) fixed=[{string.Join("|", a.FixedArguments.Select(x => AttributeArgument(x.Type, x.Value)))}] named=[{string.Join("|", named)}] err={(a.HasDecodeErrors ? 1 : 0)}";
	}

	static string Attributes(IEnumerable<IAttribute> list) => "[" + string.Join("|", list.Select(AttributeEntry)) + "]";

	static string ParameterAttributes(IEnumerable<IParameter> parameters)
		=> "[" + string.Join(",", parameters.Select(p => Attributes(p.GetAttributes()))) + "]";

	static string TypeParameterAttributes(IEnumerable<ITypeParameter> typeParameters)
		=> "[" + string.Join(",", typeParameters.Select(tp => Attributes(tp.GetAttributes()))) + "]";

	static string AttributeArgument(IType type, object? value) => Describe(type) + "=" + value switch {
		IType t => "T:" + Describe(t),
		ImmutableArray<CustomAttributeTypedArgument<IType>> items
			=> "[" + string.Join("|", items.Select(a => AttributeArgument(a.Type, a.Value))) + "]",
		_ => Value(value),
	};

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
				string enumType = td.Kind == TypeKind.Enum ? Describe(td.EnumUnderlyingType!) : "-";
				string bases = S(() => "[" + string.Join("|", td.DirectBaseTypes.Select(Describe)) + "]");
				return $"T {td.ReflectionName} kind={td.Kind} known={td.KnownTypeCode} decl={td.DeclaringTypeDefinition?.ReflectionName ?? "-"} tparams={td.TypeParameterCount} enum={enumType} base={bases} tflags={Mods((td.IsByRefLike, 'b'), (td.IsReadOnly, 'r'), (td.HasExtensions, 'x'), (td.IsStatic, 's'))}";
			});
			Append(line == "!bad" ? "T !bad" : line);
			if (line != "!bad" && td.TypeParameterCount > 0)
				Append($"G {td.ReflectionName} {S(() => TypeParams(td.TypeParameters))}");
			if (line != "!bad")
			{
				Append($"B {td.ReflectionName} {S(() => $"all={Types(td.GetAllBaseTypes())} nonif={Types(td.GetNonInterfaceBaseTypes())} dmn={((MetadataTypeDefinition)td).DefaultMemberName ?? "-"}")}");
				Append($"K {td.ReflectionName} {S(() => Lookup(td))}");
			}
		}
		foreach (var h in metadata.FieldDefinitions)
		{
			Append(Line("F", h, () => {
				var f = module.GetDefinition(h);
				return $"F {Tok(h)} decl={f.DeclaringTypeDefinition?.ReflectionName ?? "-"} name={f.Name} access={f.Accessibility} mods={Mods((f.IsStatic, 's'), (f.IsReadOnly, 'r'), (f.IsVolatile, 'v'), (f.IsConst, 'c'))} type={Describe(f.Type)} value={Value(f.GetConstantValue(throwOnInvalidMetadata: false))} retro={(f.ReturnTypeIsRefReadOnly ? 1 : 0)}";
			}));
		}
		foreach (var h in metadata.MethodDefinitions)
		{
			Append(Line("M", h, () => {
				var m = module.GetDefinition(h);
				string accessor = m.AccessorOwner == null ? "-" : $"{Tok(m.AccessorOwner.MetadataToken)}:{(int)m.AccessorKind}";
				return $"M {Tok(h)} decl={m.DeclaringTypeDefinition?.ReflectionName ?? "-"} name={m.Name} kind={m.SymbolKind} access={m.Accessibility} mods={Mods((m.IsStatic, 's'), (m.IsAbstract, 'a'), (m.IsSealed, 'e'), (m.IsVirtual, 'v'), (m.IsOverride, 'o'), (m.HasBody, 'b'))} accessor={accessor} ret={Describe(m.ReturnType)} params={Params(m.Parameters)} tparams={TypeParams(m.TypeParameters)} ext={(m.IsExtensionMethod ? 1 : 0)} thisro={(m.ThisIsRefReadOnly ? 1 : 0)} retro={(m.ReturnTypeIsRefReadOnly ? 1 : 0)} init={(m.IsInitOnly ? 1 : 0)} thisscope={m.GetThisParameterScope()} {Inheritance(m)}";
			}));
		}
		foreach (var h in metadata.PropertyDefinitions)
		{
			Append(Line("P", h, () => {
				var p = module.GetDefinition(h);
				return $"P {Tok(h)} decl={p.DeclaringTypeDefinition?.ReflectionName ?? "-"} name={p.Name} get={OptTok(p.Getter)} set={OptTok(p.Setter)} ret={Describe(p.ReturnType)} params={Params(p.Parameters)} retro={(p.ReturnTypeIsRefReadOnly ? 1 : 0)} kind={p.SymbolKind} access={p.Accessibility} mods={Mods((p.IsStatic, 's'), (p.IsAbstract, 'a'), (p.IsSealed, 'e'), (p.IsVirtual, 'v'), (p.IsOverride, 'o'))} {Inheritance(p)} bases={Members(InheritanceHelper.GetBaseMembers(p, false))}";
			}));
		}
		foreach (var h in metadata.EventDefinitions)
		{
			Append(Line("E", h, () => {
				var e = module.GetDefinition(h);
				return $"E {Tok(h)} decl={e.DeclaringTypeDefinition?.ReflectionName ?? "-"} name={e.Name} add={OptTok(e.AddAccessor)} remove={OptTok(e.RemoveAccessor)} invoke={OptTok(e.InvokeAccessor)} type={Describe(e.ReturnType)} access={e.Accessibility} mods={Mods((e.IsStatic, 's'), (e.IsAbstract, 'a'), (e.IsSealed, 'e'), (e.IsVirtual, 'v'), (e.IsOverride, 'o'))} {Inheritance(e)}";
			}));
		}
		foreach (var h in metadata.MemberReferences)
		{
			Append(Line("R", h, () => {
				string member = metadata.GetMemberReference(h).GetKind() == MemberReferenceKind.Field
					? FieldRef((IField)module.ResolveEntity(h)!)
					: MethodRef(module.ResolveMethod(h, default));
				return $"R {Tok(h)} {member}";
			}));
		}
		for (int row = 1; row <= metadata.GetTableRowCount(TableIndex.MethodSpec); row++)
		{
			var h = MetadataTokens.MethodSpecificationHandle(row);
			Append(Line("S", h, () => $"S {Tok(h)} {MethodRef(module.ResolveMethod(h, default))}"));
		}
		for (int row = 1; row <= metadata.GetTableRowCount(TableIndex.CustomAttribute); row++)
		{
			var h = MetadataTokens.CustomAttributeHandle(row);
			Append(Line("A", h, () => {
				var value = metadata.GetCustomAttribute(h).DecodeValue(module.TypeProvider);
				var named = value.NamedArguments.Select(n => $"{(n.Kind == CustomAttributeNamedArgumentKind.Field ? "F" : "P")}:{n.Name}:{AttributeArgument(n.Type, n.Value)}");
				return $"A {Tok(h)} fixed=[{string.Join("|", value.FixedArguments.Select(a => AttributeArgument(a.Type, a.Value)))}] named=[{string.Join("|", named)}]";
			}));
		}
		foreach (var td in module.TypeDefinitions)
			Append($"LT {td.ReflectionName} {S(() => $"{Attributes(td.GetAttributes())} tparams={TypeParameterAttributes(td.TypeParameters)}")}");
		foreach (var h in metadata.MethodDefinitions)
		{
			Append(Line("LM", h, () => {
				var m = module.GetDefinition(h);
				return $"LM {Tok(h)} {Attributes(m.GetAttributes())} ret={Attributes(m.GetReturnTypeAttributes())} params={ParameterAttributes(m.Parameters)} tparams={TypeParameterAttributes(m.TypeParameters)}";
			}));
		}
		foreach (var h in metadata.FieldDefinitions)
			Append(Line("LF", h, () => $"LF {Tok(h)} {Attributes(module.GetDefinition(h).GetAttributes())}"));
		foreach (var h in metadata.PropertyDefinitions)
		{
			Append(Line("LP", h, () => {
				var p = module.GetDefinition(h);
				return $"LP {Tok(h)} {Attributes(p.GetAttributes())} params={ParameterAttributes(p.Parameters)}";
			}));
		}
		foreach (var h in metadata.EventDefinitions)
			Append(Line("LE", h, () => $"LE {Tok(h)} {Attributes(module.GetDefinition(h).GetAttributes())}"));
		Append($"LA {S(() => Attributes(module.GetAssemblyAttributes()))}");
		Append($"LO {S(() => Attributes(module.GetModuleAttributes()))}");
		return sb.ToString();
	}

	[TestCaseSource(typeof(Corpus), nameof(Corpus.ManagedImages))]
	public void TypeSystemListingEquality(string path)
	{
		var ts = Load(path, TypeSystemOptions.None);
		using var table = new RustModuleTable(ts);
		Assert.That(Corpus.FirstDifference(ManagedListing(ts), table.DebugTypeListing()), Is.Null);
	}

	/// <summary>
	/// The same under TypeSystemOptions.Default: dynamic, tuples, nullability, native integers,
	/// function pointers, in/ref readonly parameters and decimal constants.
	/// </summary>
	[TestCaseSource(typeof(Corpus), nameof(Corpus.ManagedImages))]
	public void TypeSystemListingEqualityDefault(string path)
	{
		var ts = Load(path, TypeSystemOptions.Default);
		using var table = new RustModuleTable(ts);
		Assert.That(Corpus.FirstDifference(ManagedListing(ts), table.DebugTypeListing()), Is.Null);
	}

	[Test]
	public void DefaultListingCoversAttributeSemantics()
	{
		var ts = Load(typeof(CSharpDecompiler).Assembly.Location, TypeSystemOptions.Default);
		using var table = new RustModuleTable(ts);
		string[] lines = table.DebugTypeListing().Split('\n');
		Assert.That(lines, Has.Some.StartsWith("M ").And.Contains("@System.Private.CoreLib?"));
		Assert.That(lines, Has.Some.StartsWith("M ").And.Contains("@System.Private.CoreLib!"));
		Assert.That(lines, Has.Some.StartsWith("M ").And.Matches(@"\(\S+:\w+,"));
		Assert.That(lines, Has.Some.StartsWith("M ").And.Contains(",In,"));
		Assert.That(lines, Has.Some.StartsWith("M ").And.Contains(" ext=1 "));
		Assert.That(lines, Has.Some.StartsWith("T ").And.Matches(@"tflags=\S*x"));
		Assert.That(lines, Has.None.Contains("!bad"));
	}

	[Test]
	public void ListingCoversAttributeValues()
	{
		string path = Path.Combine(Corpus.TestCasesDirectory(), "Pretty", "CustomAttributes.roslyn4.dll");
		var ts = Load(path, TypeSystemOptions.Default);
		using var table = new RustModuleTable(ts);
		string[] lines = table.DebugTypeListing().Split('\n');
		Assert.That(lines, Has.Some.StartsWith("A ").And.Contains("=T:System.Int32@System.Private.CoreLib[,][,,,]]"));
		Assert.That(lines, Has.Some.StartsWith("A ").And.Contains("=T:CustomAttributes.CustomAttributes+SomeType`2@CustomAttributes.roslyn4[CustomAttributes.CustomAttributes+SomeType`1@"));
		Assert.That(lines, Has.Some.StartsWith("A ").And.Contains("System.Object@System.Private.CoreLib[]=[System.Int32@System.Private.CoreLib=Int32:1|System.UInt32@"));
		Assert.That(lines, Has.Some.StartsWith("A ").And.Contains("+ULongEnum@CustomAttributes.roslyn4=UInt64:18446744073709551615"));
		Assert.That(lines, Has.Some.StartsWith("A ").And.Contains("named=[P:AllowMultiple:System.Boolean@System.Private.CoreLib=Boolean:True]"));
		Assert.That(lines, Has.Some.StartsWith("A ").And.EndsWith("fixed=[System.String@System.Private.CoreLib=String:00480069] named=[]"));
		Assert.That(lines, Has.Some.StartsWith("A ").And.Contains("fixed=[System.Type@System.Private.CoreLib=null]"));
		Assert.That(lines, Has.None.Contains("!bad"));
	}

	[Test]
	public void ListingCoversPseudoAttributes()
	{
		var ts = Load(typeof(object).Assembly.Location, TypeSystemOptions.Default);
		using var table = new RustModuleTable(ts);
		string[] lines = table.DebugTypeListing().Split('\n');
		string[] wanted = ["StructLayoutAttribute", "DllImportAttribute", "MethodImplAttribute", "MarshalAsAttribute", "FieldOffsetAttribute", "SerializableAttribute", "PreserveSigAttribute", "SpecialNameAttribute", "AssemblyVersionAttribute", "InAttribute", "OutAttribute", "IndexerNameAttribute", "NonSerializedAttribute"];
		foreach (string name in wanted)
			Assert.That(lines, Has.Some.StartsWith("L").And.Contains("." + name + "@"), name);
		Assert.That(lines, Has.Some.StartsWith("LT ").And.Contains("StructLayoutAttribute@System.Private.CoreLib(").And.Contains("named=[F:CharSet:"));
		// IsReferenceType of type parameters, known from constraints either way.
		Assert.That(lines, Has.Some.StartsWith("G ").And.Contains(":True:{"));
		Assert.That(lines, Has.Some.StartsWith("G ").And.Contains(":False:{"));
		Assert.That(lines, Has.None.Contains("!bad"));
	}

	[Test]
	public void ListingCoversMemberLookup()
	{
		var ts = Load(typeof(CSharpDecompiler).Assembly.Location, TypeSystemOptions.Default);
		using var table = new RustModuleTable(ts);
		string[] lines = table.DebugTypeListing().Split('\n');
		Assert.That(lines, Has.Some.StartsWith("K ").And.Contains(" invoke=[06"));
		Assert.That(lines, Has.Some.StartsWith("K ").And.Contains(":System.IEquatable`1@System.Private.CoreLib[ICSharpCode."));
		Assert.That(lines, Has.Some.StartsWith("K ").And.Contains("@ICSharpCode.Decompiler[@?,@?]"));
		Assert.That(lines, Has.Some.StartsWith("K ").And.Contains("+Enumerator@ICSharpCode.Decompiler[`0@?]"));
		Assert.That(lines, Has.None.Contains("!bad"));
	}

	[Test]
	public void ListingCoversInheritance()
	{
		var ts = Load(typeof(CSharpDecompiler).Assembly.Location, TypeSystemOptions.Default);
		using var table = new RustModuleTable(ts);
		string[] lines = table.DebugTypeListing().Split('\n');
		Assert.That(lines, Has.Some.StartsWith("P ").And.Contains(" kind=Indexer ").And.Contains(" eii=1 impls=[1700"));
		Assert.That(lines, Has.Some.StartsWith("M ").And.Contains(" eii=1 impls=[0600"));
		Assert.That(lines, Has.Some.StartsWith("P ").And.Contains(" mods=o ovr=1 ").And.Contains("bases=[1700"));
		Assert.That(lines, Has.Some.StartsWith("B ").And.Contains("|System.Collections.IEnumerable@System.Private.CoreLib|").And.EndsWith(" dmn=Item"));
		Assert.That(lines, Has.None.Contains("!bad"));
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
		Assert.That(lines, Has.Some.StartsWith("T ").And.Contains("`1@System.Private.CoreLib["));
		Assert.That(lines, Has.None.Contains("!bad"));
		// Resolution across the closure, including netstandard's forwarders into the runtime.
		Assert.That(lines, Has.Some.Matches(@"base=\[System\.Object@[^?|\]]"));
		var typeLines = lines.Where(l => l.StartsWith("T ")).ToList();
		Assert.That(typeLines.Count(l => l[l.IndexOf(" base=")..].Contains("@?")), Is.LessThan(typeLines.Count / 10));
		Assert.That(lines, Has.Some.StartsWith("R ").And.Contains(" method=06"));
		Assert.That(lines, Has.Some.StartsWith("R ").And.Contains(" field=04"));
		Assert.That(lines, Has.Some.StartsWith("R ").And.Contains("`1@").And.Contains("[System."));
		Assert.That(lines, Has.Some.StartsWith("S ").And.Not.Contains("targs=[]"));
		Assert.That(lines, Has.Some.StartsWith("M ").And.Contains(" kind=Constructor "));
		Assert.That(lines, Has.Some.StartsWith("M ").And.Contains(" kind=Accessor "));
		Assert.That(lines, Has.Some.StartsWith("F ").And.Contains(" mods=sc").And.Contains("value=Int32:"));
		Assert.That(lines, Has.Some.StartsWith("P ").And.Contains(" get=0600"));
		Assert.That(lines, Has.Some.StartsWith("G ").And.Contains(":Invariant:"));
		Assert.That(lines, Has.Some.StartsWith("M ").And.Contains(",Out,"));
	}
}
